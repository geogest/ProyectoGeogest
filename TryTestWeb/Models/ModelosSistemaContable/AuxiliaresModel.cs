using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using System.Web;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;

public class AuxiliaresModel
{
    public int AuxiliaresModelID { get; set; }
    public int DetalleVoucherModelID { get; set; }

    //public virtual PrestadoresModel Prestador { get; set; }

    public int LineaNumeroDetalle { get; set; }
    public virtual CuentaContableModel objCtaContable { get; set; }
    public decimal MontoTotal { get; set; }

    public TipoAuxiliar Tipo { get; set; }

    //public TipoAuxiliarCentralizacion TipoCentralizacion { get; set; }

    public virtual ICollection<AuxiliaresDetalleModel> ListaDetalleAuxiliares { get; set; }

}

public class AuxiliaresDetalleModel
{
    public const decimal LiquidABruto = 0.9M;
    public const decimal BrutoALiquid = 0.1M;

    public int AuxiliaresDetalleModelID { get; set; }
    public int AuxiliaresModelID { get; set; }
    public int NumeroCorrelativo { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaContabilizacion { get; set; }
    //El prestador contiene RUT y Razon Social
    public virtual AuxiliaresPrestadoresModel Individuo { get; set; }
    public TipoDte TipoDocumento { get; set; }
    public int Folio { get; set; }
    public int FolioHasta { get; set; }
    //Utilizado por TipoAuxiliar = 1 / ProveedoresDeudores
    public decimal MontoNetoLinea { get; set; }
    public decimal MontoExentoLinea { get; set; }
    public decimal MontoIVALinea { get; set; }
    public decimal MontoIVANoRecuperable { get; set; }
    public decimal MontoIVAUsoComun { get; set; }
    public decimal MontoIVAActivoFijo { get; set; }

    //Utilizado por TipoAuxiliar = 2 / BoletaHonorarios
    public decimal ValorLiquido { get; set; }
    public decimal ValorRetencion { get; set; }
    //En tipo 1 corresponde a la suma de los montos del DOCUMENTO TRIBUTARIO
    //En tipo 2 corresponde al monto BRUTO  
    public decimal MontoTotalLinea { get; set; }
    //Fecha de vencimiento de pago asociada al DTE dentro de un auxiliar
    //o fecha de vencimiento de pago asociada a un auxiliar de honorarios
    public DateTime FechaVencimiento { get; set; }

    public virtual QuickReceptorModel Individuo2 { get; set; }
    public bool SeVaParaVenta { get; set; } = false;
    public bool SeVaParaCompra { get; set; } = false;

    public static List<string[]> GetAuxiliaresVistaLibrosCentralizacion(List<AuxiliaresDetalleModel> LstAux/*,int Anio,int Mes*/)
    {
        //var nfi = (NumberFormatInfo)CultureInfo.CreateSpecificCulture("es").NumberFormat;
        List<string[]> ReturnValues = new List<string[]>();
        if (LstAux == null || LstAux.Count == 0)
            return ReturnValues;

        int NumeroRow = 1;

        foreach (AuxiliaresDetalleModel Auxiliar in LstAux)
        {
            string[] BalanceRow = new string[] { "-", "-", "-", "-", "-", "-", "0", "0", "0", "0" };
            //Numero Correlativo
            BalanceRow[0] = NumeroRow.ToString();
            //Fecha
            BalanceRow[1] = ParseExtensions.ToDD_MM_AAAA(Auxiliar.Fecha);
            //Documento
            BalanceRow[2] = ParseExtensions.EnumGetDisplayAttrib(Auxiliar.TipoDocumento);
            BalanceRow[3] = ParseExtensions.DecimalToStringForRazor(Auxiliar.Folio); ;
            //Nombre prestador
            if (Auxiliar.Individuo2 != null)
            {
                BalanceRow[4] = Auxiliar.Individuo2.RazonSocial;
                //Rut prestador
                BalanceRow[5] = Auxiliar.Individuo2.RUT;
            }
            else {
                BalanceRow[4] = "";
                //Rut prestador
                BalanceRow[5] = "";

            }
           
            //MONTO EXENTO
            BalanceRow[6] = ParseExtensions.NumeroConPuntosDeMiles(Auxiliar.MontoExentoLinea);
            //MONTO AFECTO
            BalanceRow[7] = ParseExtensions.NumeroConPuntosDeMiles(Auxiliar.MontoNetoLinea);
            //MONTO IVA
            BalanceRow[8] = ParseExtensions.NumeroConPuntosDeMiles(Auxiliar.MontoIVALinea);
            //MONTO TOTAL
            BalanceRow[9] = ParseExtensions.NumeroConPuntosDeMiles(Auxiliar.MontoTotalLinea);

            ReturnValues.Add(BalanceRow);
            NumeroRow++;
        }
        return ReturnValues;
    }

    public static List<AuxiliaresDetalleModel> RescatarLibroCentralizacion(ClientesContablesModel objCliente, TipoCentralizacion tipoLibroCentralizacion, FacturaPoliContext db, string FechaInicio = "", string FechaFin = "", int Anio = 0, int Mes = 0)
    {
        bool ConversionFechaInicioExitosa = false;  
        DateTime dtFechaInicio = new DateTime();
        bool ConversionFechaFinExitosa = false;
        DateTime dtFechaFin = new DateTime();

        ConversionFechaInicioExitosa = DateTime.TryParse(FechaInicio, out dtFechaInicio);
        ConversionFechaFinExitosa = DateTime.TryParse(FechaFin, out dtFechaFin);

        var CuentasContableCliente = db.DBCuentaContable.Where(w => w.ClientesContablesModelID == objCliente.ClientesContablesModelID && w.TipoCentralizacionAuxiliares == tipoLibroCentralizacion);
        List<VoucherModel> LstVoucher = db.DBVoucher.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID && r.DadoDeBaja == false).ToList();
        List<AuxiliaresDetalleModel> LaLista = new List<AuxiliaresDetalleModel>();

        foreach (var voucher in LstVoucher)
        {
            List<DetalleVoucherModel> lstDetalleVoucher = voucher.ListaDetalleVoucher.Where(p => CuentasContableCliente.Any(ee => ee.CodInterno == p.ObjCuentaContable.CodInterno)).ToList();
            foreach (var detalleVoucher in lstDetalleVoucher)
            {
                List<AuxiliaresModel> lstAuxiliar = db.DBAuxiliares.Where(r => r.DetalleVoucherModelID == detalleVoucher.DetalleVoucherModelID).ToList();
                foreach (var auxiliar in lstAuxiliar)
                {
                    List<AuxiliaresDetalleModel> LstDetalleAuxiliares = db.DBAuxiliaresDetalle.Where(r => r.AuxiliaresModelID == auxiliar.AuxiliaresModelID).ToList();
                    if (Anio != 0)
                        LstDetalleAuxiliares = LstDetalleAuxiliares.Where(r => r.FechaContabilizacion.Year == Anio).ToList();
                    if (Mes != 0)
                        LstDetalleAuxiliares = LstDetalleAuxiliares.Where(r => r.FechaContabilizacion.Month == Mes).ToList();
                    if (ConversionFechaInicioExitosa)
                        LstDetalleAuxiliares = LstDetalleAuxiliares.Where(r => r.FechaContabilizacion >= dtFechaInicio).ToList();
                    if (ConversionFechaFinExitosa)
                        LstDetalleAuxiliares = LstDetalleAuxiliares.Where(r => r.FechaContabilizacion <= dtFechaFin).ToList();
                    LaLista.AddRange(LstDetalleAuxiliares);
                }
            }
        }
        return LaLista;
    }

    public static byte[] ExportExcelLibroVentaCompraNormal(List<string[]> cachedLibroCompra, ClientesContablesModel objCliente, bool InformarMembrete, TipoCentralizacion tipoLibroCentralizacion, string tituloDocumento, string FechaInicio = "", string FechaFin = "", int Anio = 0, int Mes = 0, bool TieneFiltros = false)
    {
     
        byte[] ExcelByteArray = null;

        
        string pathFileArchivosLibro = ParseExtensions.Get_AppData_Path("LibroTemplate.xlsx");

        if (tipoLibroCentralizacion == TipoCentralizacion.Compra)
        {
            pathFileArchivosLibro = ParseExtensions.Get_AppData_Path("LibroTemplate.xlsx");
        }else if(tipoLibroCentralizacion == TipoCentralizacion.Venta)
        {
            pathFileArchivosLibro = ParseExtensions.Get_AppData_Path("LibroTemplate2.xlsx");
        }
             
        

      

        using (XLWorkbook excelFile = new XLWorkbook(pathFileArchivosLibro))
        {
            var workSheet = excelFile.Worksheet(1);

            FacturaPoliContext db = new FacturaPoliContext();

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = "Rut: " + ParseExtensions.FormatoRutMembrete(objCliente.RUTEmpresa);
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A6").Value = objCliente.Direccion;
                workSheet.Cell("A7").Value = objCliente.Ciudad;
                workSheet.Cell("A8").Value = ParseExtensions.FormatoRutMembrete(objCliente.RUTRepresentante) + objCliente.Representante;
               // workSheet.Cell("A9").Value = objCliente.RUTRepresentante;
            }
            else
            {
                workSheet.Cell("A1").Value = string.Empty;
                workSheet.Cell("A2").Value = string.Empty;
                workSheet.Cell("A3").Value = string.Empty;
                workSheet.Cell("A6").Value = string.Empty;
                workSheet.Cell("A7").Value = string.Empty;
                workSheet.Cell("A8").Value = string.Empty;
                workSheet.Cell("A9").Value = string.Empty;
            }

            if (TieneFiltros)
            {
                workSheet.Cell("C4").Value = tituloDocumento;
            }
            else if(!TieneFiltros)
            {
                workSheet.Cell("C4").Value = "TODOS LOS AÑOS";
            }

            if(tipoLibroCentralizacion == TipoCentralizacion.Compra)
            {
                //H a M
                workSheet.Columns("H:M").Style.NumberFormat.Format = "#,##0 ;-#,##0";
            }else if(tipoLibroCentralizacion == TipoCentralizacion.Venta)
            {
                workSheet.Columns("H:K").Style.NumberFormat.Format = "#,##0 ;-#,##0";
            }

            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            ConversionFechaInicioExitosa = DateTime.TryParse(FechaInicio, out dtFechaInicio);
            ConversionFechaFinExitosa = DateTime.TryParse(FechaFin, out dtFechaFin);

          //  IEnumerable<LibrosContablesModel> bagDetalleLibros = objCliente.ListLibros.Where(r => r.TipoLibro == tipoLibroCentralizacion);

         

          /*  if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                bagDetalleLibros = bagDetalleLibros.Where(r => r.FechaDoc >= dtFechaInicio && r.FechaDoc <= dtFechaFin);
            }
            else
            {
                bagDetalleLibros = bagDetalleLibros.Where(r => r.FechaDoc.Year == Anio);
                if (Mes != 0)
                    bagDetalleLibros = bagDetalleLibros.Where(r => r.FechaDoc.Month == Mes);
            }*/

            int NumeroFilaExcel = 15;
            int NumeroCorrelativoDummy = 1;
            //int IgnoraUltimo = cachedLibroCompra.Count() - 2;

            foreach (String[] tableRow in  cachedLibroCompra) {

                List<string> valuesForExcel = new List<string>();

                //Correlativo
                valuesForExcel.Add(tableRow[0]);
                //Fecha documento
                valuesForExcel.Add(tableRow[1]);
                //Fecha contabilización 
                valuesForExcel.Add(tableRow[2]);
                //Comprobante
                valuesForExcel.Add(tableRow[3]);
                //Folio
                valuesForExcel.Add(tableRow[4]);
                //Nombre
                valuesForExcel.Add(tableRow[5]);
                //Rut
                valuesForExcel.Add(tableRow[6]);
                //Exento
                valuesForExcel.Add(tableRow[7]);
                //Monto Afecto
                valuesForExcel.Add(tableRow[8]);
                //IVA Recuperable (IVA a secas)
                valuesForExcel.Add(tableRow[9]);
              if(tipoLibroCentralizacion == TipoCentralizacion.Compra) { 
                //IVA no recuperable
                valuesForExcel.Add(tableRow[10]);
                //IVA uso común
                valuesForExcel.Add(tableRow[11]);
              }
                //TOTAL
                valuesForExcel.Add(tableRow[12]);

   
                string[] theRow = valuesForExcel.ToArray();
                for (int i = 0; i < theRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = theRow[i]; //.Value = theRow[i];
                }
                if(tipoLibroCentralizacion == TipoCentralizacion.Compra) { 
                workSheet.Range("A" + NumeroFilaExcel + ":M" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Se establece el rango que cubrirá el borde del excel 
                                                                                                                                                //workSheet.Range("A" + NumeroFilaExcel + "J" + NumeroFilaExcel).Rows().Style.Border.InsideBorder.Get = XLBorderStyleValues.Double;
                }else if(tipoLibroCentralizacion == TipoCentralizacion.Venta)
                {
                   workSheet.Range("A" + NumeroFilaExcel + ":K" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
                NumeroFilaExcel++;
                NumeroCorrelativoDummy++;

            }

            /*   foreach (LibrosContablesModel tableRow in bagDetalleLibros)
               {
                   List<string> valuesForExcel = new List<string>();
                   //FECHA
                   valuesForExcel.Add(ParseExtensions.ToDD_MM_AAAA(tableRow.FechaDoc));
                   //NUMERO CORRELATIVO
                   valuesForExcel.Add(NumeroCorrelativoDummy.ToString());
                   //TIPO DE DOCUMENTO 
                   valuesForExcel.Add(ParseExtensions.EnumGetDisplayAttrib(tableRow.TipoDocumento));
                   //FOLIO
                   valuesForExcel.Add(tableRow.Folio.ToString());
                   //RUT PRESTADOR
                   valuesForExcel.Add(tableRow.individuo.RUT);
                   //NOMBRE PRESTADOR
                   valuesForExcel.Add(tableRow.individuo.RazonSocial);

                   //EXENTO
                   valuesForExcel.Add(tableRow.MontoExento.ToString());
                   //MONTO
                   valuesForExcel.Add(tableRow.MontoNeto.ToString());
                   //IVA
                   valuesForExcel.Add(tableRow.MontoIva.ToString());
                   //TOTAL
                   valuesForExcel.Add(tableRow.MontoTotal.ToString());

                   string[] theRow = valuesForExcel.ToArray();
                   for (int i = 0; i < theRow.Length; i++)
                   {
                       workSheet.Cell(NumeroFilaExcel, i + 1).SetValue<string>(theRow[i]); //.Value = theRow[i];
                   }
                   workSheet.Range("A" + NumeroFilaExcel + ":J" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                   //workSheet.Range("A" + NumeroFilaExcel + "J" + NumeroFilaExcel).Rows().Style.Border.InsideBorder.Get = XLBorderStyleValues.Double;

                   NumeroFilaExcel++;
                   NumeroCorrelativoDummy++;
               }*/

            //OBTIENE TOTALES POR TIPO DE DOCUMENTO
        
            List<string[]> lstTotalValues = new List<string[]>();
            var objX = cachedLibroCompra.Select(x => x[3]).Distinct();
            if(tipoLibroCentralizacion == TipoCentralizacion.Compra) { 
            foreach (string tipo_dte in objX)
            {
            
                lstTotalValues.Add(new string[] {
                    "Total",
                    cachedLibroCompra.Where(r => r[3] == tipo_dte).Count() +" " + tipo_dte,
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[7]))),
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[8]))),
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[9]))),
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[10]))),
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[11]))),
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[12])))
                });

            }

            lstTotalValues.Add(new string[] {
                    "Total",
                    cachedLibroCompra.Count() +" " + "Comprobantes",
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[7]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[7]))) ),
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[8]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[8]))) ),
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[9]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[9]))) ),
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[10]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[10]))) ),
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[11]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[11]))) ),
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[12]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[12]))) )
            });
           }else if(tipoLibroCentralizacion == TipoCentralizacion.Venta)
            {

                foreach (string tipo_dte in objX)
                {

                    lstTotalValues.Add(new string[] {
                    "Total",
                    cachedLibroCompra.Where(r => r[3] == tipo_dte).Count() +" " + tipo_dte,
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[7]))),
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[8]))),
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[9]))),
                    ParseExtensions.NumeroConPuntosDeMiles(cachedLibroCompra.Where(r => r[3] == tipo_dte).Sum(x => int.Parse(x[12])))
                });

                }

                lstTotalValues.Add(new string[] {
                    "Total",
                    cachedLibroCompra.Count() +" " + "Comprobantes",
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[7]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[7]))) ),
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[8]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[8]))) ),
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[9]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[9]))) ),
                    ParseExtensions.NumeroConPuntosDeMiles( (cachedLibroCompra.Where(w => bool.Parse(w[13]) == false).Sum(r => int.Parse(r[12]) )) - (cachedLibroCompra.Where(w => bool.Parse(w[13]) == true).Sum(r => int.Parse(r[12]))) )
            });

            }





            /*}


                        List<string[]> lstTotalValues = new List<string[]>();
                        var objX = bagDetalleLibros.Select(x => x.TipoDocumento).Distinct();
                        foreach (TipoDte tipo_dte in objX)
                        {
                            lstTotalValues.Add(new string[] {
                                "Total",
                                bagDetalleLibros.Where(r => r.TipoDocumento == tipo_dte).Count() +" " + ParseExtensions.EnumGetDisplayAttrib(tipo_dte),
                                ParseExtensions.NumeroConPuntosDeMiles(bagDetalleLibros.Where(r => r.TipoDocumento == tipo_dte).Sum(r => r.MontoExento)),
                                ParseExtensions.NumeroConPuntosDeMiles(bagDetalleLibros.Where(r => r.TipoDocumento == tipo_dte).Sum(r => r.MontoNeto)),
                                ParseExtensions.NumeroConPuntosDeMiles(bagDetalleLibros.Where(r => r.TipoDocumento == tipo_dte).Sum(r => r.MontoIva)),
                                ParseExtensions.NumeroConPuntosDeMiles((bagDetalleLibros.Where(r => r.TipoDocumento == tipo_dte).Sum(r => r.MontoTotal)))
                            });
                        }

                        lstTotalValues.Add(new string[] {
                                "Total",
                                bagDetalleLibros.Count() +" " + "Comprobantes",
                                ParseExtensions.NumeroConPuntosDeMiles( (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaCredito() == false).Sum(r => r.MontoExento)) - (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaCredito() == true).Sum(r => r.MontoExento)) ),
                                ParseExtensions.NumeroConPuntosDeMiles( (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaCredito() == false).Sum(r => r.MontoNeto)) - (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaCredito() == true).Sum(r => r.MontoNeto)) ),
                                ParseExtensions.NumeroConPuntosDeMiles( (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaCredito() == false).Sum(r => r.MontoIva)) - (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaCredito() == true).Sum(r => r.MontoIva)) ),
                                ParseExtensions.NumeroConPuntosDeMiles( (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaCredito() == false).Sum(r => r.MontoTotal)) - (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaCredito() == true).Sum(r => r.MontoTotal)) )
                        });*/

            if (lstTotalValues.Count > 0)
            {
                int lastRowLocation = NumeroFilaExcel + 1;
                workSheet.Cell(lastRowLocation, 6).InsertData(lstTotalValues);
            }
            else
                workSheet.Cell(16, 6).InsertData(lstTotalValues);
            workSheet.Columns().AdjustToContents();
            ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
              
            
       
        }
        if (ExcelByteArray == null)
            return null;
        else
        {
            return ExcelByteArray;
        }
        
    }
    

    public static byte[] ExportExcelLibroCentralizacionAuxiliaresVentaCompra(ClientesContablesModel objCliente, bool InformarMembrete, TipoCentralizacion tipoLibroCentralizacion, string tituloDocumento, FacturaPoliContext db, string FechaInicio = "", string FechaFin = "", int Anio = 0, int Mes = 0)
    {
        byte[] ExcelByteArray = null;
        string pathFileArchivosLibro = ParseExtensions.Get_AppData_Path("LibroTemplate.xlsx");


        using (XLWorkbook excelFile = new XLWorkbook(pathFileArchivosLibro))
        {
            var workSheet = excelFile.Worksheet(1);

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = objCliente.RUTEmpresa;
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A6").Value = objCliente.Direccion;
                workSheet.Cell("A7").Value = objCliente.Ciudad;
                workSheet.Cell("A8").Value = objCliente.Representante;
                workSheet.Cell("A9").Value = objCliente.RUTRepresentante;
            }
            else
            {
                workSheet.Cell("A1").Value = string.Empty;
                workSheet.Cell("A2").Value = string.Empty;
                workSheet.Cell("A3").Value = string.Empty;
                workSheet.Cell("A6").Value = string.Empty;
                workSheet.Cell("A7").Value = string.Empty;
                workSheet.Cell("A8").Value = string.Empty;
                workSheet.Cell("A9").Value = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(tituloDocumento) == false)
            {
                workSheet.Cell("A12").Value = tituloDocumento;
            }
            else
            {
                workSheet.Cell("A12").Value = string.Empty;
            }

            List<AuxiliaresDetalleModel> lstAuxiliares = RescatarLibroCentralizacion(objCliente, tipoLibroCentralizacion, db, FechaInicio, FechaFin, Anio, Mes);

            int NumeroFilaExcel = 15;
            int NumeroCorrelativoDummy = 1;


            foreach (AuxiliaresDetalleModel tableRow in lstAuxiliares)
            {
                List<string> valuesForExcel = new List<string>();
                //FECHA
                valuesForExcel.Add(ParseExtensions.ToDD_MM_AAAA(tableRow.Fecha));
                //NUMERO CORRELATIVO
                valuesForExcel.Add(NumeroCorrelativoDummy.ToString());
                //TIPO DE DOCUMENTO 
                valuesForExcel.Add(ParseExtensions.EnumGetDisplayAttrib(tableRow.TipoDocumento));
                //FOLIO
                valuesForExcel.Add(tableRow.Folio.ToString());
                //RUT PRESTADOR
                valuesForExcel.Add(tableRow.Individuo.PrestadorRut);
                //NOMBRE PRESTADOR
                valuesForExcel.Add(tableRow.Individuo.PrestadorNombre);

                //EXENTO
                valuesForExcel.Add(tableRow.MontoExentoLinea.ToString());
                //MONTO
                valuesForExcel.Add(tableRow.MontoNetoLinea.ToString());
                //IVA
                valuesForExcel.Add(tableRow.MontoIVALinea.ToString());
                //TOTAL
                valuesForExcel.Add(tableRow.MontoTotalLinea.ToString());

                string[] theRow =valuesForExcel.ToArray();
                for (int i = 0; i < theRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = theRow[i];
                }
                workSheet.Range("A" + NumeroFilaExcel + ":J" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                //workSheet.Range("A" + NumeroFilaExcel + "J" + NumeroFilaExcel).Rows().Style.Border.InsideBorder.Get = XLBorderStyleValues.Double;

                NumeroFilaExcel++;
                NumeroCorrelativoDummy++;
            }

            //OBTIENE TOTALES POR TIPO DE DOCUMENTO
            List<string[]> lstTotalValues = new List<string[]>();
            var objX = lstAuxiliares.Select(x => x.TipoDocumento).Distinct();
            foreach (TipoDte tipo_dte in objX)
            {
                lstTotalValues.Add(new string[] {
                    "Total",
                    lstAuxiliares.Where(r => r.TipoDocumento == tipo_dte).Count() +" " + ParseExtensions.EnumGetDisplayAttrib(tipo_dte),
                    ParseExtensions.NumeroConPuntosDeMiles(lstAuxiliares.Where(r => r.TipoDocumento == tipo_dte).Sum(r => r.MontoExentoLinea)),
                    ParseExtensions.NumeroConPuntosDeMiles(lstAuxiliares.Where(r => r.TipoDocumento == tipo_dte).Sum(r => r.MontoNetoLinea)),
                    ParseExtensions.NumeroConPuntosDeMiles(lstAuxiliares.Where(r => r.TipoDocumento == tipo_dte).Sum(r => r.MontoIVALinea)),
                    ParseExtensions.NumeroConPuntosDeMiles((lstAuxiliares.Where(r => r.TipoDocumento == tipo_dte).Sum(r => r.MontoTotalLinea)))
                                            //+ lstTotalValues.Where(r => r.TipoFactura == tipo_dte).Sum(r => r.ObjTotals.ImpuestoAdicionalMonto)))
                                            //MANEJAR A FUTURO IMPUESTO ADICIONAL TOTAL DE LOS OTROS TIPOS?
                });
            }

            lstTotalValues.Add(new string[] {
                    "Total",
                    lstAuxiliares.Count() +" " + "Comprobantes",
                    ParseExtensions.NumeroConPuntosDeMiles( lstAuxiliares.Sum(r => r.MontoExentoLinea) ),
                    ParseExtensions.NumeroConPuntosDeMiles( lstAuxiliares.Sum(r => r.MontoExentoLinea) ),
                    ParseExtensions.NumeroConPuntosDeMiles( lstAuxiliares.Sum(r => r.MontoExentoLinea) ),
                    ParseExtensions.NumeroConPuntosDeMiles( lstAuxiliares.Sum(r => r.MontoExentoLinea) )
            });
            /*
            lstTotalValues.Add(new string[] {
                    "Total",
                    lstVentasDelMes.Count() +" " + "Comprobantes",
                    ParseExtensions.NumeroConPuntosDeMiles(rQuery.Where(CheckNonCredito).Sum(r => r.ObjTotals.MontoExento) - rQuery.Where(CheckCredito).Sum(r => r.ObjTotals.MontoExento)),
                    ParseExtensions.NumeroConPuntosDeMiles(rQuery.Where(CheckNonCredito).Sum(r => r.ObjTotals.MontoNetoMonto) - rQuery.Where(CheckCredito).Sum(r => r.ObjTotals.MontoNetoMonto)),
                    ParseExtensions.NumeroConPuntosDeMiles(rQuery.Where(CheckNonCredito).Sum(r => r.ObjTotals.IVAMonto) - rQuery.Where(CheckCredito).Sum(r => r.ObjTotals.IVAMonto)),
                    ParseExtensions.NumeroConPuntosDeMiles((rQuery.Where(CheckNonCredito).Sum(r => r.ObjTotals.TotalMonto) + rQuery.Sum(r => r.ObjTotals.ImpuestoAdicionalMonto))
                                                         - (rQuery.Where(CheckCredito).Sum(r => r.ObjTotals.TotalMonto) + rQuery.Where(CheckCredito).Sum(r => r.ObjTotals.ImpuestoAdicionalMonto)))
            });*/

            if (lstTotalValues.Count > 0)
            {
                //var rangeWithArrays = workSheet.Cell(NumeroFilaExcel, 1).InsertData(lstTotalValues);
                //rangeWithArrays.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                int lastRowLocation = NumeroFilaExcel + 1;
                workSheet.Cell(lastRowLocation, 5).InsertData(lstTotalValues);
            }
            else
                workSheet.Cell(16, 5).InsertData(lstTotalValues);


            workSheet.Columns().AdjustToContents();

            ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
        }
        if (ExcelByteArray == null)
            return null;
        else
        {
            return ExcelByteArray;
        }
    }

    
    public static byte[] GetExcelLibrosVentaCompra(List<string[]> lstAuxiliares, ClientesContablesModel objCliente, bool InformarMembrete, string titulo)
    {
        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(@"C:\LibroVentaCompra.xlsx"))
        {
            var workSheet = excelFile.Worksheet(1);

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = objCliente.RUTEmpresa;
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A4").Value = objCliente.Direccion;
                workSheet.Cell("A5").Value = objCliente.Ciudad;
                workSheet.Cell("A6").Value = objCliente.Representante;
                workSheet.Cell("A7").Value = objCliente.RUTRepresentante;
            }
            else
            {
                workSheet.Cell("A1").Value = string.Empty;
                workSheet.Cell("A2").Value = string.Empty;
                workSheet.Cell("A3").Value = string.Empty;
                workSheet.Cell("A4").Value = string.Empty;
                workSheet.Cell("A5").Value = string.Empty;
                workSheet.Cell("A6").Value = string.Empty;
                workSheet.Cell("A7").Value = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(titulo) == false)
            {
                workSheet.Cell("D4").Value = titulo;
            }
            else
            {
                workSheet.Cell("D4").Value = string.Empty;
            }

            int NumeroFilaExcel = 8;
            foreach (string[] tableRow in lstAuxiliares)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }
                workSheet.Range("B" + NumeroFilaExcel + ":K" + NumeroFilaExcel)/*.Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Medium*/;
                workSheet.Range("B" + NumeroFilaExcel + ":K" + NumeroFilaExcel)/*.Rows().Style.Border.InsideBorder = XLBorderStyleValues.Double*/;
                NumeroFilaExcel++;
            }
            ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
        }
        if (ExcelByteArray == null)
            return null;
        else
        {
            return ExcelByteArray;
        }
    }

}

public class AuxiliaresPrestadoresModel
{
    public int AuxiliaresPrestadoresModelID { get; set; }
    public int ClientesContablesModelID { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Rut Prestador")]
    [StringLength(10)]
    [ValidaRut(ErrorMessage = "El Rut ingresado es Invalido.")]
    public string PrestadorRut { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Razon Social Prestador")]
    public string PrestadorNombre { get; set; }

    public int PrestadorRutNumerico { get { return ParseExtensions.RutANumero(PrestadorRut); } }

    public static AuxiliaresPrestadoresModel CrearOActualizarPrestadorPorRut(string RUTPrestador, string NombrePrestador, ClientesContablesModel objCliente, FacturaPoliContext db)
    {
        AuxiliaresPrestadoresModel objPrestadores = db.DBAuxiliaresPrestadores.SingleOrDefault(r => r.PrestadorRut == RUTPrestador && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);
        if (objPrestadores == null)
        {
            objPrestadores = new AuxiliaresPrestadoresModel();
            objPrestadores.ClientesContablesModelID = objCliente.ClientesContablesModelID;
            objPrestadores.PrestadorRut = RUTPrestador;
            objPrestadores.PrestadorNombre = NombrePrestador;
            db.DBAuxiliaresPrestadores.Add(objPrestadores);
            db.SaveChanges();
            return objPrestadores;
        }
        else
        {
            if (objPrestadores.PrestadorNombre == NombrePrestador)
            {
                return objPrestadores;
            }
            else
            {
                objPrestadores.PrestadorNombre = NombrePrestador;
                db.DBAuxiliaresPrestadores.AddOrUpdate(objPrestadores);
                db.SaveChanges();
                return objPrestadores;
            }
        }
    }
}
/*public class DatosBaseModel
{
    int DatosBaseModelID { get; set; }
    string valor { get; set; }
    string tipo { get; set; }
    bool estado { get; set;   }
}*/
    public enum TipoAuxiliar
{
    ProveedorDeudor = 1, // Vale por compra y venta.
    Honorarios = 2,
    Remuneracion = 3
}

//OLD NAME: TipoAuxiliarCentralizacion
/*
 ¿Se tiene que seguir usando en los auxiliares?
*/
public enum TipoCentralizacion
{
    [Display(Name = "Ninguno")]
    Ninguno = 0,
    [Display(Name = "Venta")]
    Venta = 1,
    [Display(Name = "Compra")]
    Compra = 2,
    [Display(Name = "Honorarios")]
    Honorarios = 3
}
