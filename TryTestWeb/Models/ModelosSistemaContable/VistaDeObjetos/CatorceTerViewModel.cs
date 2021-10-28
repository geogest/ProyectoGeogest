using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Routing;

public class CatorceTerViewModel
{
    public DateTime Fecha { get; set; }
    public TipoDte TipoDocumento { get; set; }
    public long Folio { get; set; }
    public string NombreReceptor { get; set; }
    public string RutReceptor { get; set; }
    public decimal Ingreso { get; set; }
    public decimal Egreso { get; set; }
    public string Glosa { get; set; }
    public string TipoLibro { get; set; }
    public string CuentaContable { get; set; }
    public decimal TotalIngreso { get; set; }
    public decimal TotalEgreso { get; set; }


    public static PaginadorModel GetCatorceTer(FacturaPoliContext db, ClientesContablesModel ObjCliente, string FechaInicio = "", string FechaFin = "", int Anio = 0, int Mes = 0, int pagina = 0, int cantidadRegistrosPorPagina = 0, string Rut = "", string RazonSocial = "", int Folio = 0)
    {

        bool ConversionFechaInicioExitosa = false;
        DateTime dtFechaInicio = new DateTime();
        bool ConversionFechaFinExitosa = false;
        DateTime dtFechaFin = new DateTime();

        if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
        {
            ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
            ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
        }

        List<CatorceTerViewModel> lstCatorceTer = new List<CatorceTerViewModel>();

        string TipoReceptorCompra = "PR";
        string TipoReceptorVenta = "CL";
        string TipoReceptorHonorario = "H";
        string TipoReceptorRemu = "P";

        //Aquí se consultarán los tipos de voucher y se irán según la logica contable entregada
        //No se discrimina por tipo de voucher aquí están, tanto, ingresos, egresos, traspasos

        var TablaPrestador = (from Voucher in db.DBVoucher
                              join DetalleVoucher in db.DBDetalleVoucher on Voucher.VoucherModelID equals DetalleVoucher.VoucherModelID
                              join Auxiliares in db.DBAuxiliares on DetalleVoucher.DetalleVoucherModelID equals Auxiliares.DetalleVoucherModelID
                              join AuxiliaresDetalle in db.DBAuxiliaresDetalle on Auxiliares.AuxiliaresModelID equals AuxiliaresDetalle.AuxiliaresModelID

                              where
                              Voucher.DadoDeBaja == false && //Ingreso
                              Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                              AuxiliaresDetalle.Individuo2.tipoReceptor == TipoReceptorVenta ||

                              Voucher.DadoDeBaja == false && //Egreso
                              Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                              AuxiliaresDetalle.Individuo2.tipoReceptor == TipoReceptorCompra ||

                              Voucher.DadoDeBaja == false && //Egreso
                              Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                              AuxiliaresDetalle.Individuo2.tipoReceptor == TipoReceptorHonorario ||

                              Voucher.DadoDeBaja == false && //Egreso
                              Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                              AuxiliaresDetalle.Individuo2.tipoReceptor == TipoReceptorRemu


                              select new
                              {
                                  Haber = DetalleVoucher.MontoHaber,
                                  Debe = DetalleVoucher.MontoDebe,
                                  FechaContabilizacion = DetalleVoucher.FechaDoc,
                                  PrestadorNombre = AuxiliaresDetalle.Individuo2.RazonSocial,
                                  PrestadorRut = AuxiliaresDetalle.Individuo2.RUT,
                                  TipoPrestador = AuxiliaresDetalle.Individuo2.tipoReceptor,
                                  TipoDoc = AuxiliaresDetalle.TipoDocumento,
                                  Folio = AuxiliaresDetalle.Folio,
                                  CuentaCont = DetalleVoucher.ObjCuentaContable.CodInterno + "  " + DetalleVoucher.ObjCuentaContable.nombre
                              });

        if (Anio != 0 && Anio > 0)
            TablaPrestador = TablaPrestador.Where(r => r.FechaContabilizacion.Year == Anio);
        if (Mes != 0 && Mes > 0)
            TablaPrestador = TablaPrestador.Where(r => r.FechaContabilizacion.Month == Mes);
        if (ConversionFechaInicioExitosa && ConversionFechaInicioExitosa)
            TablaPrestador = TablaPrestador.Where(r => r.FechaContabilizacion >= dtFechaInicio && r.FechaContabilizacion <= dtFechaFin);
        if (!string.IsNullOrWhiteSpace(Rut))
            TablaPrestador = TablaPrestador.Where(r => r.PrestadorRut.Contains(Rut));
        if (!string.IsNullOrWhiteSpace(RazonSocial))
            TablaPrestador = TablaPrestador.Where(r => r.PrestadorNombre.Contains(RazonSocial));
        if (Folio != 0 && Folio > 0)
            TablaPrestador = TablaPrestador.Where(r => r.Folio == Folio);


        int totalDeRegistros = TablaPrestador.Count();
        if (cantidadRegistrosPorPagina != 0)
        {
            TablaPrestador = TablaPrestador.OrderBy(r => r.FechaContabilizacion)
                                           .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                                           .Take(cantidadRegistrosPorPagina);

        }
        else if (cantidadRegistrosPorPagina == 0)
        {
            TablaPrestador = TablaPrestador.OrderBy(r => r.FechaContabilizacion);
        }

        decimal TotalIngreso = 0;
        decimal TotalEgreso = 0;

        foreach (var itemCatorceTer in TablaPrestador)
        {
            decimal Haber = itemCatorceTer.Haber;
            decimal Debe = itemCatorceTer.Debe;

            decimal TotalDebeHaber = Math.Abs(Haber) - Math.Abs(Debe);

            CatorceTerViewModel objTer = new CatorceTerViewModel();
            objTer.Fecha = itemCatorceTer.FechaContabilizacion;
            objTer.NombreReceptor = itemCatorceTer.PrestadorNombre;
            objTer.RutReceptor = itemCatorceTer.PrestadorRut;
            objTer.Folio = itemCatorceTer.Folio;
            objTer.TipoDocumento = itemCatorceTer.TipoDoc;

            if (itemCatorceTer.TipoPrestador == TipoReceptorCompra)
            {
                objTer.Egreso = Math.Abs(TotalDebeHaber);
                objTer.TipoLibro = "Compra";
                TotalEgreso += Math.Abs(TotalDebeHaber);
            }
            else if (itemCatorceTer.TipoPrestador == TipoReceptorVenta)
            {
                objTer.Ingreso = Math.Abs(TotalDebeHaber);
                objTer.TipoLibro = "Venta";
                TotalIngreso += Math.Abs(TotalDebeHaber);
            }
            else if (itemCatorceTer.TipoPrestador == TipoReceptorHonorario)
            {
                objTer.Egreso = Math.Abs(TotalDebeHaber);
                objTer.TipoLibro = "Honorario";
                TotalEgreso += Math.Abs(TotalDebeHaber);
            }
            else if (itemCatorceTer.TipoPrestador == TipoReceptorRemu)
            {
                objTer.Egreso = Math.Abs(TotalDebeHaber);
                objTer.TipoLibro = "Remuneracion";
                TotalEgreso += Math.Abs(TotalDebeHaber);
            }

            objTer.CuentaContable = itemCatorceTer.CuentaCont;

            lstCatorceTer.Add(objTer);
        }

        CatorceTerViewModel Totales = new CatorceTerViewModel();
        Totales.TotalIngreso = Math.Abs(TotalIngreso);
        Totales.TotalEgreso = Math.Abs(TotalEgreso);
        lstCatorceTer.Add(Totales);
    
        var Paginador = new PaginadorModel();
        Paginador.LstCatorceTer = lstCatorceTer;
        Paginador.PaginaActual = pagina;
        Paginador.TotalDeRegistros = totalDeRegistros;
        Paginador.RegistrosPorPagina = cantidadRegistrosPorPagina;
        Paginador.ValoresQueryString = new RouteValueDictionary();

        if (cantidadRegistrosPorPagina != 25)
            Paginador.ValoresQueryString["cantidadRegistrosPorPagina"] = cantidadRegistrosPorPagina;
        if (Anio != 0)
            Paginador.ValoresQueryString["Anio"] = Anio;
        if (Mes != 0)
            Paginador.ValoresQueryString["Mes"] = Mes;
        if (!string.IsNullOrWhiteSpace(Rut))
            Paginador.ValoresQueryString["Rut"] = Rut;
        if (!string.IsNullOrWhiteSpace(RazonSocial))
            Paginador.ValoresQueryString["RazonSocial"] = RazonSocial;
        if (ConversionFechaInicioExitosa && ConversionFechaInicioExitosa)
        {
            Paginador.ValoresQueryString["FechaInicio"] = FechaInicio;
            Paginador.ValoresQueryString["FechaFin"] = FechaFin;
        }

        return Paginador;
    }

    //public static List<CatorceTerViewModel> GetCatorceTerDesdeVoucher(FacturaPoliContext db, ClientesContablesModel ObjCliente)
    //{
    //    IQueryable<VoucherModel> QueryIandE = db.DBVoucher.Where(x => x.)
    //    return null;
    //}

    public static byte[] GetExcelCatorceTer(List<CatorceTerViewModel> lstCatorceTer, ClientesContablesModel objCliente, bool InformarMembrete, SessionParaExcel Fechas)
    {
        string RutaEsteExcel = ParseExtensions.Get_AppData_Path("14Ter.xlsx");

        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(RutaEsteExcel))
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
                
                if(string.IsNullOrWhiteSpace(Fechas.Mes) && string.IsNullOrWhiteSpace(Fechas.Anio))
                {
                    workSheet.Cell("F8").Value = "Informe 14 Ter";
                }
                else if(!string.IsNullOrWhiteSpace(Fechas.Mes) && !string.IsNullOrWhiteSpace(Fechas.Anio))
                {
                    workSheet.Cell("F8").Value = "Informe 14 Ter" + "  " + ParseExtensions.obtenerNombreMes(Convert.ToInt32(Fechas.Mes)) + "  " + Fechas.Anio;
                }
                else if(!string.IsNullOrWhiteSpace(Fechas.Anio) && string.IsNullOrWhiteSpace(Fechas.Mes))
                {
                    workSheet.Cell("F8").Value = "Informe 14 Ter" + "  " + Fechas.Anio;
                }
            workSheet.Column("B").Style.DateFormat.Format = "dd-MM-yyyy";
            workSheet.Columns("G:H").Style.NumberFormat.Format = "#,##0 ;-#,##0";
            workSheet.Columns("A","K").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


            int NumeroFilaExcel = 13;
                int Correlativo = 1;
                foreach (var itemTer in lstCatorceTer)
                {
                        if(itemTer.TotalIngreso > 0 || itemTer.TotalEgreso > 0)
                        {
                            workSheet.Cell(NumeroFilaExcel, "F").Value = "TOTAL:";
                            workSheet.Cell(NumeroFilaExcel, "G").Value = itemTer.TotalIngreso;
                            workSheet.Cell(NumeroFilaExcel, "H").Value = itemTer.TotalEgreso;
                        }else
                        {
                        workSheet.Cell(NumeroFilaExcel, "A").Value = Correlativo;
                        workSheet.Cell(NumeroFilaExcel, "B").Value = itemTer.Fecha;
                        if(itemTer.TipoDocumento != 0) { 
                        workSheet.Cell(NumeroFilaExcel, "C").Value = itemTer.TipoDocumento.ToString();
                        }else
                        {
                            workSheet.Cell(NumeroFilaExcel, "C").Value = "Otros";
                        }
                        workSheet.Cell(NumeroFilaExcel, "D").Value = itemTer.Folio;
                        workSheet.Cell(NumeroFilaExcel, "E").Value = itemTer.NombreReceptor;
                        workSheet.Cell(NumeroFilaExcel, "F").Value = itemTer.RutReceptor;
                        workSheet.Cell(NumeroFilaExcel, "G").Value = itemTer.Ingreso;
                        workSheet.Cell(NumeroFilaExcel, "H").Value = itemTer.Egreso;
                        workSheet.Cell(NumeroFilaExcel, "I").Value = itemTer.CuentaContable;
                    }

                workSheet.Range("A" + NumeroFilaExcel + ":K" + NumeroFilaExcel).Rows
                ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("A" + NumeroFilaExcel + ":K" + NumeroFilaExcel).Rows
                ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                Correlativo++;
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
