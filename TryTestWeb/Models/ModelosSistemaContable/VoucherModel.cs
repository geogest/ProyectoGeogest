using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using System.Web;
using System.Globalization;
using System.Web.Routing;
using TryTestWeb.Controllers;

public class VoucherModel
{
    public int VoucherModelID { get; set; }

    public int ClientesContablesModelID { get; set; }

    public string Glosa { get; set; }

    public DateTime FechaEmision { get; set; }

    public virtual CentroCostoModel CentroDeCosto { get; set; }

    public virtual ICollection<DetalleVoucherModel> ListaDetalleVoucher { get; set; }
    private ICollection<DetalleVoucherModel> _ListaDetalleVoucher

    {

        get { return _ListaDetalleVoucher.OrderBy(x => x.FechaDoc).ToList(); }
        set { _ListaDetalleVoucher = value; }
    }

    public TipoVoucher Tipo { get; set; }

    public String TipoOrigen { get; set; }

    public TipoOrigen TipoOrigenVoucher { get; set; }

    public int NumeroVoucher { get; set; }

    public bool DadoDeBaja { get; set; } = false;

    public int ClientesProveedoresModelID { get; set; }

    public static decimal GetTotalDebe(List<VoucherModel> lstVoucher)
    {
        return lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Sum(x => x.MontoDebe);
    }

    public static decimal GetTotalDebe(List<DetalleVoucherModel> lstDetalle)
    {
        return lstDetalle.Sum(x => x.MontoDebe);
    }

    public static decimal GetTotalHaber(List<VoucherModel> lstVoucher)
    {
        return lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Sum(x => x.MontoHaber);
    }

    public static decimal GetTotalHaber(List<DetalleVoucherModel> lstDetalle)
    {
        return lstDetalle.Sum(x => x.MontoHaber);
    }

    /// <summary>
    /// TOTAL DEBE - TOTAL HABER
    /// </summary>
    /// <param name="lstVoucher">Lista de Vouchers de los cuales obtener el total</param>
    /// <returns>Si es POSITIVO va a SALDOS DEUDOR : Si es NEGATIVO va a SALDOS ACREEDOR</returns>
    public static decimal GetTotalSALDOS(List<VoucherModel> lstVoucher)
    {
        return GetTotalDebe(lstVoucher) - GetTotalHaber(lstVoucher);
    }
    public static decimal GetTotalSALDOS(List<DetalleVoucherModel> lstDetalle)
    {
        return GetTotalDebe(lstDetalle) - GetTotalHaber(lstDetalle);
    }


    public static PaginadorModel GetLibroDiario(int pagina, int cantidadRegistrosPorPagina, List<VoucherModel> lstVoucher, FacturaPoliContext db,
                                                bool truncateNombreCuenta = false, int Anio = 0, int Mes = 0, DateTime? FechaInicio = null,
                                                DateTime? FechaFin = null, string folio = null, string Glosa = "", string CuentaContableID = "")
    {
        List<string[]> ReturnValues = new List<string[]>();
        //if (lstVoucher == null || lstVoucher.Count == 0)
        //    return Paginacion;

        decimal TotalHaber = 0;
        decimal TotalDebe = 0;

        lstVoucher = lstVoucher.OrderBy(r => r.FechaEmision).ToList();
        int NumeroRow = 1;

     

        foreach (VoucherModel Voucher in lstVoucher)
        {
            IEnumerable<DetalleVoucherModel> lstDetallesVoucher = Voucher.ListaDetalleVoucher.OrderBy(x => x.FechaDoc);
            //filtro por fecha 
            //Aplicar Filtros acá
            if (Anio != 0)
                lstDetallesVoucher = lstDetallesVoucher.Where(x => x.FechaDoc.Year == Anio);
           
            if (Mes != 0)
                    lstDetallesVoucher = lstDetallesVoucher.Where(r => r.FechaDoc.Month == Mes);

            if (FechaInicio != null && FechaFin != null)
                    lstDetallesVoucher = lstDetallesVoucher.Where(r => r.FechaDoc >= FechaInicio && r.FechaDoc <= FechaFin);
                
            if (!string.IsNullOrWhiteSpace(Glosa))
                lstDetallesVoucher = lstDetallesVoucher.Where(x => x.GlosaDetalle.Contains(Glosa));

            if (!string.IsNullOrWhiteSpace(CuentaContableID))
                lstDetallesVoucher = lstDetallesVoucher.Where(x => x.ObjCuentaContable.CuentaContableModelID == Convert.ToInt32(CuentaContableID));


            
            int numeroLineaDetalle = 0;
            foreach (DetalleVoucherModel detalleVoucher in lstDetallesVoucher)
            {
                if (folio != null && folio != "")
                {
                    //  int folioI = Int32.Parse(folio);
                    var listado = (from c in db.DBDetalleVoucher
                                   join a in db.DBAuxiliares on c.DetalleVoucherModelID equals a.DetalleVoucherModelID
                                   join ad in db.DBAuxiliaresDetalle on a.AuxiliaresModelID equals ad.AuxiliaresModelID

                                   where c.DetalleVoucherModelID == detalleVoucher.DetalleVoucherModelID && ad.Folio.ToString() == folio //Convert.ToInt64(folio)

                                   select new { esta = 1 }).Count();



                    if (listado == 0)
                    {
                        continue;
                    }


                }

                numeroLineaDetalle++;

                // TODO: AGREGAR ACUMULACIONES ANTERIORES

                string[] BalanceRow = new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "" };
                BalanceRow[0] = NumeroRow.ToString();
                BalanceRow[1] = ParseExtensions.ToDD_MM_AAAA(detalleVoucher.FechaDoc);//ParseExtensions.ToDD_MM_AAAA(Voucher.FechaEmision);
                BalanceRow[2] = ParseExtensions.TipoVoucherToShortName(Voucher.Tipo) + " " + Voucher.NumeroVoucher + " " + numeroLineaDetalle;
                BalanceRow[3] = detalleVoucher.GlosaDetalle;

                BalanceRow[4] = ParseExtensions.NumberWithDots_para_BalanceGeneral(detalleVoucher.MontoDebe);
                TotalDebe += detalleVoucher.MontoDebe;
                BalanceRow[5] = ParseExtensions.NumberWithDots_para_BalanceGeneral(detalleVoucher.MontoHaber);
                TotalHaber += detalleVoucher.MontoHaber;

                BalanceRow[6] = detalleVoucher.ObjCuentaContable.CodInterno;
                string nombreCuentaContable = detalleVoucher.ObjCuentaContable.nombre;
                if (truncateNombreCuenta == true)
                    nombreCuentaContable = nombreCuentaContable.Truncar(20);
                BalanceRow[7] = nombreCuentaContable;
                if (detalleVoucher.CentroCostoID != 0)
                {

                    var nombre = (CentroCostoModel)(from t1 in db.DBCentroCosto
                                                    where t1.CentroCostoModelID == detalleVoucher.CentroCostoID
                                                    select t1).FirstOrDefault();
                    //   select new { Nombre =  t1.Nombre }.Nombre.FirstOrDefault();
                    if (nombre != null)
                    {

                        BalanceRow[8] = "[" + (detalleVoucher.CentroCostoID).ToString() + "] " + nombre.Nombre;
                    }
                    else
                    {
                        BalanceRow[8] = "";
                    }

                }
                ReturnValues.Add(BalanceRow);

              
                NumeroRow++;
            }
        }

        int TotalRegistros = ReturnValues.Count();

        if(cantidadRegistrosPorPagina != 0) { 
            ReturnValues = ReturnValues
                        .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                        .Take(cantidadRegistrosPorPagina).ToList();
        }
        

        var Paginador = new PaginadorModel();

        Paginador.ResultStringArray = ReturnValues;
        Paginador.PaginaActual = pagina;
        Paginador.TotalDeRegistros = TotalRegistros;
        Paginador.RegistrosPorPagina = cantidadRegistrosPorPagina;
        Paginador.ValoresQueryString = new RouteValueDictionary();

        if (cantidadRegistrosPorPagina != 25)
            Paginador.ValoresQueryString["cantidadRegistrosPorPagina"] = cantidadRegistrosPorPagina;

        if (Anio != 0)
            Paginador.ValoresQueryString["Anio"] = Anio;

        if (Mes != 0)
            Paginador.ValoresQueryString["Mes"] = Mes;

        if (!String.IsNullOrWhiteSpace(Glosa))
            Paginador.ValoresQueryString["Glosa"] = Glosa;

        if (!String.IsNullOrWhiteSpace(CuentaContableID))
            Paginador.ValoresQueryString["CuentaContableID"] = CuentaContableID;

        if (FechaInicio != null && FechaFin != null)
        {
            Paginador.ValoresQueryString["FechaInicio"] = FechaInicio;
            Paginador.ValoresQueryString["FechaFin"] = FechaFin;
        }
       
        return Paginador;
    }

    public static byte[] GetExcelLibroDiario(List<string[]> lstLibroDiario, ClientesContablesModel objCliente, bool InformarMembrete, string titulo, string subtitulo = "",bool TieneFiltrosActivos = false)
    {
        List<string[]> newStrList = new List<string[]>();
        //lstLibroDiario.OrderBy();
        foreach (string[] newOrderValues in lstLibroDiario)
        {
            string[] newIngreso = { newOrderValues[1], newOrderValues[2], newOrderValues[3], newOrderValues[4], newOrderValues[5], newOrderValues[6], newOrderValues[7], newOrderValues[8] };
            newStrList.Add(newIngreso);
        }

        string RutaPlanillaLibroMayor = ParseExtensions.Get_AppData_Path("EXCELDIARIO.xlsx");

        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(RutaPlanillaLibroMayor))
        {
            var workSheet = excelFile.Worksheet(1);

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = "Rut: " + ParseExtensions.FormatoRutMembrete(objCliente.RUTEmpresa);
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A4").Value = objCliente.Direccion;
                workSheet.Cell("A5").Value = objCliente.Ciudad;
                workSheet.Cell("A6").Value = ParseExtensions.FormatoRutMembrete(objCliente.RUTRepresentante) + objCliente.Representante;
                //workSheet.Cell("A7").Value = objCliente.RUTRepresentante;
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

            if (TieneFiltrosActivos == false)
            {
                workSheet.Cell("C4").Value = "TODOS LOS AÑOS";
            }
            else
            {
                workSheet.Cell("C4").Value = titulo;
            }

            if (string.IsNullOrWhiteSpace(subtitulo) == false)
            {
                workSheet.Cell("C9").Value = subtitulo;
            }
            else
            {
                workSheet.Cell("C9").Value = string.Empty;
            }

            workSheet.Columns("D:E").Style.NumberFormat.Format = "#,##0 ;-#,##0";

            int NumeroFilaExcel = 12;
            foreach (string[] tableRow in newStrList)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }
                workSheet.Range("A" + NumeroFilaExcel + ":H" + NumeroFilaExcel).Rows
                ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("A" + NumeroFilaExcel + ":H" + NumeroFilaExcel).Rows
                ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                NumeroFilaExcel++;
            }
            ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
        }
        if (ExcelByteArray == null)
            return null;
        else
            return ExcelByteArray;
    }

    public static byte[] GetExcelLibroMayor(List<string[]> lstLibroMayor, ClientesContablesModel objCliente, bool InformarMembrete, string titulo, string subtitulo = "")
    {
        List<string[]> newStrList = new List<string[]>();
        foreach (string[] newOrderValues in lstLibroMayor)
        {
            string[] newIngreso = { newOrderValues[1], newOrderValues[2], newOrderValues[3], newOrderValues[4], newOrderValues[5], newOrderValues[6], newOrderValues[7], newOrderValues[8], newOrderValues[9]};
            newStrList.Add(newIngreso);
        }

        var UnicosEnLaLista = ParseExtensions.obtieneUnicosEnLista(lstLibroMayor);

        string RutaPlanillaLibroMayor = ParseExtensions.Get_AppData_Path("EXCELMAYOR.xlsx");

        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(RutaPlanillaLibroMayor))
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
                workSheet.Cell("C8").Value = titulo;
            }
            else
            {
                workSheet.Cell("C8").Value = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(subtitulo) == false)
            {
                workSheet.Cell("C10").Value = subtitulo;
            }
            else
            {
                workSheet.Cell("C10").Value = string.Empty;
            }

            workSheet.Column("A").Style.DateFormat.Format = "dd-MM-yyyy";
            workSheet.Columns("F:H").Style.NumberFormat.Format = "#,##0 ;-#,##0";

            DateTime ObjFechaUtil = new DateTime();
            int NumeroFilaExcel = 13;


            for (int i = 0; i < UnicosEnLaLista.Count(); i++)
            {
                decimal Debe = 0;
                decimal Haber = 0;
                decimal Saldo = 0;

                workSheet.Cell(NumeroFilaExcel, "A").Value = UnicosEnLaLista[i];
                workSheet.Cell(NumeroFilaExcel, "A").Style.Font.Bold = true;
                NumeroFilaExcel++;

                workSheet.Range("A" + NumeroFilaExcel + ":H" + NumeroFilaExcel).Rows
                     ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("A" + NumeroFilaExcel + ":H" + NumeroFilaExcel).Rows
                ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                workSheet.Cell(NumeroFilaExcel, "A").Value = "FECHA";
                workSheet.Cell(NumeroFilaExcel, "B").Value = "COMPROBANTE";
                workSheet.Cell(NumeroFilaExcel, "C").Value = "GLOSA";
                workSheet.Cell(NumeroFilaExcel, "D").Value = "RAZON SOCIAL";
                workSheet.Cell(NumeroFilaExcel, "E").Value = "RUT";
                workSheet.Cell(NumeroFilaExcel, "F").Value = "DEBE";
                workSheet.Cell(NumeroFilaExcel, "G").Value = "HABER";
                workSheet.Cell(NumeroFilaExcel, "H").Value = "SALDO";
             

                NumeroFilaExcel++;
     
             
                foreach (var ItemMayor in newStrList)
                {
                    
                    if (ItemMayor[8] == UnicosEnLaLista[i]) {
                        
                        for (int j = 0; j < ItemMayor.Count() - 1; j++)
                        {
                            if(ItemMayor[j] == ItemMayor[0] && ItemMayor[j] != "-")
                            {
                                ObjFechaUtil = ParseExtensions.ToDD_MM_AAAA_Multi(ItemMayor[j]);
                            }
                            if(j == 5 && ItemMayor[j] != "0")
                            {
                                Debe += Convert.ToDecimal(ItemMayor[j]);
                            }
                            if (j == 6 && ItemMayor[j] != "0")
                            {
                                Haber += Convert.ToDecimal(ItemMayor[j]);
                            }
                            workSheet.Cell(NumeroFilaExcel, j + 1).Value = ItemMayor[j];
                            
                        }
                        workSheet.Range("A" + NumeroFilaExcel + ":H" + NumeroFilaExcel).Rows
                        ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        workSheet.Range("A" + NumeroFilaExcel + ":H" + NumeroFilaExcel).Rows
                        ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        NumeroFilaExcel++;
                    }
                }
                workSheet.Cell(NumeroFilaExcel, "E").Value = "TOTAL";
                workSheet.Cell(NumeroFilaExcel, "E").Style.Font.Bold = true;
                workSheet.Cell(NumeroFilaExcel, "F").Value = Debe;
                workSheet.Cell(NumeroFilaExcel, "F").Style.Font.Bold = true;
                workSheet.Cell(NumeroFilaExcel, "G").Value = Haber;
                workSheet.Cell(NumeroFilaExcel, "G").Style.Font.Bold = true;

                Saldo = Haber - Debe;

                workSheet.Cell(NumeroFilaExcel, "H").Value = Math.Abs(Saldo);
                workSheet.Cell(NumeroFilaExcel, "H").Style.Font.Bold = true;

                workSheet.Range("A" + NumeroFilaExcel + ":H" + NumeroFilaExcel).Rows
                ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("A" + NumeroFilaExcel + ":H" + NumeroFilaExcel).Rows
                ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                NumeroFilaExcel+=2;
            }

            int TotalFinal = newStrList.Count() - 1;
            string[] totales = newStrList[TotalFinal];

      
            workSheet.Cell(NumeroFilaExcel, "E").Value = "TOTAL FINAL:";
            workSheet.Cell(NumeroFilaExcel, "E").Style.Font.Bold = true;
            workSheet.Cell(NumeroFilaExcel, "F").Value = totales[5];
            workSheet.Cell(NumeroFilaExcel, "F").Style.Font.Bold = true;
            workSheet.Cell(NumeroFilaExcel, "G").Value = totales[6];
            workSheet.Cell(NumeroFilaExcel, "G").Style.Font.Bold = true;
            workSheet.Cell(NumeroFilaExcel, "H").Value = totales[7];
            workSheet.Cell(NumeroFilaExcel, "H").Style.Font.Bold = true;

            ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
        }
        if (ExcelByteArray == null)
            return null;
        else
            return ExcelByteArray;
    }

    public static byte[] GetExcelLibroHonorarios(List<string[]> lstLibroHonorarios, ClientesContablesModel objCliente, bool InformarMembrete, string titulo, string subtitulo = "", bool tieneFiltro = false)
    {
        List<string[]> newStrList = new List<string[]>();
        foreach (string[] newOrderValues in lstLibroHonorarios)
        {
            string[] newIngreso = { newOrderValues[0], newOrderValues[1], newOrderValues[2], newOrderValues[3], newOrderValues[4], newOrderValues[5], newOrderValues[6], newOrderValues[7],newOrderValues[8] };
            newStrList.Add(newIngreso);
        }

        string RutaPlanillaLibroMayor = ParseExtensions.Get_AppData_Path("EXCELHONORARIOS.xlsx");

        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(RutaPlanillaLibroMayor))
        {
            var workSheet = excelFile.Worksheet(1);

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = "Rut: " + ParseExtensions.FormatoRutMembrete(objCliente.RUTEmpresa);
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A4").Value = objCliente.Direccion;
                workSheet.Cell("A5").Value = objCliente.Ciudad;
                workSheet.Cell("A6").Value = ParseExtensions.FormatoRutMembrete(objCliente.RUTRepresentante) + " " + objCliente.Representante;
               // workSheet.Cell("A7").Value = objCliente.RUTRepresentante;
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

            if (tieneFiltro)
            {
                workSheet.Cell("F4").Value = titulo;
            }
            else if(!tieneFiltro)
            {
                workSheet.Cell("F4").Value = "TODOS LOS AÑOS";
            }

            if (string.IsNullOrWhiteSpace(subtitulo) == false)
            {
                workSheet.Cell("C10").Value = subtitulo;
            }
            else
            {
                workSheet.Cell("C10").Value = string.Empty;
            }

            workSheet.Columns("D:E").Style.DateFormat.Format = "dd-MM-yyyy";
            workSheet.Columns("H:J").Style.NumberFormat.Format = "#,##0 ;-#,##0";

            int NumeroFilaExcel = 13;
            foreach (string[] tableRow in newStrList)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    if (tableRow[i] == tableRow[2] || tableRow[i] == tableRow[3])
                    {
                        if (tableRow[i] != "-") { 
                            DateTime FechaTramp = new DateTime();
                            FechaTramp = ParseExtensions.ToDD_MM_AAAA_Multi(tableRow[i]);
                            tableRow[i] = FechaTramp.ToString();
                        }
                    }
                    workSheet.Cell(NumeroFilaExcel, i + 2).Value = tableRow[i];
                }
                workSheet.Range("B" + NumeroFilaExcel + ":J" + NumeroFilaExcel).Rows
                ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("B" + NumeroFilaExcel + ":J" + NumeroFilaExcel).Rows
                ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                NumeroFilaExcel++;
            }
            ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
        }
        if (ExcelByteArray == null)
            return null;
        else
            return ExcelByteArray;
    }

    public static byte[] GetExcelPresupuestos(List<string[]> lstPresupuesto, ClientesContablesModel objCliente, bool InformarMembrete, string titulo, string subtitulo = "")
    {
        List<string[]> newStrList = new List<string[]>();
        foreach (string[] newOrderValues in lstPresupuesto)
        {
            string[] newIngreso = { newOrderValues[0], newOrderValues[1], newOrderValues[2], newOrderValues[3], newOrderValues[4], newOrderValues[5] };
            newStrList.Add(newIngreso);
        }

        string RutaPlanillaLibroMayor = ParseExtensions.Get_AppData_Path("ExcelPresupuestos.xlsx");

        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(RutaPlanillaLibroMayor))
        {
            var workSheet = excelFile.Worksheet(1);

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = "Rut: " + ParseExtensions.FormatoRutMembrete(objCliente.RUTEmpresa);
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A4").Value = objCliente.Direccion;
                workSheet.Cell("A5").Value = objCliente.Ciudad;
                workSheet.Cell("A6").Value = ParseExtensions.FormatoRutMembrete(objCliente.RUTRepresentante) + " " + objCliente.Representante;
                // workSheet.Cell("A7").Value = objCliente.RUTRepresentante;
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
                workSheet.Cell("D5").Value = titulo;
            }
            else
            {
                workSheet.Cell("D8").Value = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(subtitulo) == false)
            {
                workSheet.Cell("C10").Value = subtitulo;
            }
            else
            {
                workSheet.Cell("C10").Value = string.Empty;
            }

            workSheet.Columns("C:E").Style.NumberFormat.Format = "#,##0 ;-#,##0";
            workSheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            int NumeroFilaExcel = 13;
            foreach (string[] tableRow in newStrList)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }
                workSheet.Range("A" + NumeroFilaExcel + ":F" + NumeroFilaExcel).Rows
                ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("A" + NumeroFilaExcel + ":F" + NumeroFilaExcel).Rows
                ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                NumeroFilaExcel++;
            }
            ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
        }
        if (ExcelByteArray == null)
            return null;
        else
            return ExcelByteArray;
    }

    // TODO : OBTENER CLARIDAD REFERENTE A PORQUE TODAS LAS HOJAS DICEN LIBRO MAYOR
    public static List<string[]> GetLibroMayor(List<VoucherModel> lstVoucher, FacturaPoliContext db, CuentaContableModel singleCuentaContable = null, DateTime? fechaInicio = null, DateTime? fechaFin = null, string anno = null, string mes = null, string folio = null)
    {
        List<string[]> ReturnValues = new List<string[]>();
        if (lstVoucher == null || lstVoucher.Count() == 0)
            return ReturnValues;

        //TODO : Determinar cuando muestra el monto negativo o positivo dependiendo de la diferencia entre debe y haber
        decimal TotalCarreado = 0;

        decimal TotalMontoDebe = 0;
        decimal TotalMontoHaber = 0;

        //List<DetalleVoucherModel> lstDetalleVoucherModel = lstVoucher.SelectMany(r => r.ListaDetalleVoucher).Where(r=> r.ObjCuentaContable == singleCuentaContable).ToList();
        lstVoucher = lstVoucher.OrderBy(r => r.FechaEmision).ToList();

        int NumeroRow = 1;

        foreach (VoucherModel Voucher in lstVoucher)
        {

            IEnumerable<DetalleVoucherModel> lstDetallesVoucher = Voucher.ListaDetalleVoucher.OrderBy(x => x.FechaDoc);
            //filtro por fecha 
            // lstDetallesVoucher = lstDetallesVoucher.OrderBy(r => r.ObjCuentaContable.CuentaContableModelID).ToList();
            lstDetallesVoucher = lstDetallesVoucher.OrderBy(r => r.FechaDoc).ToList();

            if (anno != null && anno != "")
            {

                lstDetallesVoucher = lstDetallesVoucher.Where(r => r.FechaDoc.Year == Convert.ToInt32(anno)).ToList();
            }

            if (mes != null && mes != "")
            {
                lstDetallesVoucher = lstDetallesVoucher.Where(r => r.FechaDoc.Month == Convert.ToInt32(mes)).ToList();
            }

            if (fechaInicio != null && fechaFin != null)
            {
                lstDetallesVoucher = lstDetallesVoucher.Where(r => r.FechaDoc.Entre(fechaInicio, fechaFin)).ToList();
            }



            int numeroLineaDetalle = 0;
            foreach (DetalleVoucherModel detalleVoucher in lstDetallesVoucher)
            {
                if (folio != null && folio != "")
                {
                    // int folioI = Convert.ToInt32(folio);
                    var listado = (from c in db.DBDetalleVoucher
                                   join a in db.DBAuxiliares on c.DetalleVoucherModelID equals a.DetalleVoucherModelID
                                   join ad in db.DBAuxiliaresDetalle on a.AuxiliaresModelID equals ad.AuxiliaresModelID

                                   where c.DetalleVoucherModelID == detalleVoucher.DetalleVoucherModelID && ad.Folio.ToString() == folio

                                   select new { esta = 1 }).Count();



                    if (listado == 0)
                    {
                        continue;
                    }


                }



                numeroLineaDetalle++;
                if (singleCuentaContable != null)
                {
                    if (detalleVoucher.ObjCuentaContable != singleCuentaContable)
                        continue;
                }
                if (fechaInicio != null && fechaFin != null)
                {
                    //if (Voucher.FechaEmision.Entre(fechaInicio, fechaFin) == false)
                    if (detalleVoucher.FechaDoc.Entre(fechaInicio, fechaFin) == false)
                        continue;
                }
                string[] BalanceRow = new string[] { "-", "-", "-", "-", "-", "-", "-", "", "" };
                BalanceRow[0] = NumeroRow.ToString();
                //   BalanceRow[1] = ParseExtensions.ToDD_MM_AAAA(Voucher.FechaEmision); // FECHA
                BalanceRow[1] = ParseExtensions.ToDD_MM_AAAA(detalleVoucher.FechaDoc);
                BalanceRow[2] = ParseExtensions.TipoVoucherToShortName(Voucher.Tipo) + " " + Voucher.NumeroVoucher + " " + numeroLineaDetalle;
                BalanceRow[3] = detalleVoucher.GlosaDetalle;
                //Pendiente probar esto
              
                

                decimal totalLineaMontoDebe = detalleVoucher.MontoDebe;
                decimal totalLineaMontoHaber = detalleVoucher.MontoHaber;

                totalLineaMontoDebe += TotalMontoDebe;
                totalLineaMontoHaber += TotalMontoHaber;

                decimal TotalCarreadoEstaLinea = totalLineaMontoDebe - totalLineaMontoHaber;
                TotalCarreado = TotalCarreado + TotalCarreadoEstaLinea;

                BalanceRow[4] = ParseExtensions.NumberWithDots_para_BalanceGeneral(totalLineaMontoDebe);
                BalanceRow[5] = ParseExtensions.NumberWithDots_para_BalanceGeneral(totalLineaMontoHaber);
                BalanceRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalCarreado);
                //BalanceRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalCarreado);
                if (detalleVoucher.CentroCostoID != 0)
                {

                    var nombre = (CentroCostoModel)(from t1 in db.DBCentroCosto
                                                    where t1.CentroCostoModelID == detalleVoucher.CentroCostoID
                                                    select t1).FirstOrDefault();
                    //   select new { Nombre =  t1.Nombre }.Nombre.FirstOrDefault();
                    if (nombre != null)
                    {

                        BalanceRow[7] = "[" + (detalleVoucher.CentroCostoID).ToString() + "] " + nombre.Nombre;
                    }
                    else
                    {
                        BalanceRow[7] = "";
                    }

                }
                else
                {
                    BalanceRow[7] = "";
                }


                BalanceRow[7] = "[" + detalleVoucher.ObjCuentaContable.CodInterno + "] " + detalleVoucher.ObjCuentaContable.nombre;
                //   BalanceRow[9] = detalleVoucher.Auxiliar.
                ReturnValues.Add(BalanceRow);
                NumeroRow++;
            }
        }

        string[] TotalRow = new string[]
        {
            "",
            "",
            "",
            "Total Final",
            ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalMontoDebe),
            ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalMontoHaber),
            ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalCarreado),
            "",
            ""
        };
        ReturnValues.Add(TotalRow); 

        return ReturnValues;

    }

    public static PaginadorModel GetLibroMayorTwo(FiltrosParaLibros flibros,ClientesContablesModel objCliente, 
                                                  FacturaPoliContext db)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalFinalMontoDebe = 0;
        decimal TotalFinalMontohaber = 0;
        decimal TotalFinalSaldo = 0;

        decimal TotalMontoDebe = 0;
        decimal TotalMontoHaber = 0;
        decimal TotalSaldo = 0;

        bool ConversionFechaInicioExitosa = false;
        DateTime dtFechaInicio = new DateTime();
        bool ConversionFechaFinExitosa = false;
        DateTime dtFechaFin = new DateTime();

        if (string.IsNullOrWhiteSpace(flibros.FechaInicio) == false && string.IsNullOrWhiteSpace(flibros.FechaFin) == false)
        {
            ConversionFechaInicioExitosa = DateTime.TryParseExact(flibros.FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
            ConversionFechaFinExitosa = DateTime.TryParseExact(flibros.FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
        }



        //Si me robo un auto Tesla, se transforma en un auto Edison?
        var LibroMayorCompleto = (from detalleVoucher in db.DBDetalleVoucher
                                  join Voucher in db.DBVoucher on detalleVoucher.VoucherModelID equals Voucher.VoucherModelID into vouchGroup
                                  from Voucher in vouchGroup.DefaultIfEmpty()
                                  join Auxiliar in db.DBAuxiliares on detalleVoucher.DetalleVoucherModelID equals Auxiliar.DetalleVoucherModelID into auxGroup
                                  from Auxiliar in auxGroup.DefaultIfEmpty()
                                  join AuxiliarDetalle in db.DBAuxiliaresDetalle on Auxiliar.AuxiliaresModelID equals AuxiliarDetalle.AuxiliaresModelID into auxdetallGroup
                                  from AuxiliarDetalle in auxdetallGroup.DefaultIfEmpty()

                                  where Voucher.DadoDeBaja == false && Voucher.ClientesContablesModelID == objCliente.ClientesContablesModelID

                                  select new LibroMayor
                                  {
                                      Haber = detalleVoucher.MontoHaber,
                                      Debe = detalleVoucher.MontoDebe,
                                      Glosa = detalleVoucher.GlosaDetalle,
                                      FechaContabilizacion = detalleVoucher.FechaDoc,
                                      Rut = AuxiliarDetalle.Individuo2.RUT == null ? "-" : AuxiliarDetalle.Individuo2.RUT,
                                      RazonSocial = AuxiliarDetalle.Individuo2.RazonSocial == null ? "-" : AuxiliarDetalle.Individuo2.RazonSocial,
                                      CodigoInterno = detalleVoucher.ObjCuentaContable.CodInterno,
                                      CtaContNombre = detalleVoucher.ObjCuentaContable.nombre,
                                      CtaContablesID = detalleVoucher.ObjCuentaContable.CuentaContableModelID,
                                      CtaContableClasi = detalleVoucher.ObjCuentaContable.Clasificacion,
                                      Comprobante = Voucher.Tipo,
                                      ComprobanteP2 = Voucher.NumeroVoucher.ToString(),
                                      ComprobanteP3 = Auxiliar.LineaNumeroDetalle.ToString(),
                                      NumVoucher = Voucher.NumeroVoucher,
                                      VoucherId = Voucher.VoucherModelID,
                                      DetalleVoucherId = detalleVoucher.DetalleVoucherModelID,
                                      EstaConciliado = detalleVoucher.Conciliado
                                  });


        if (flibros.Anio != 0)
        {
            LibroMayorCompleto = LibroMayorCompleto.Where(r => r.FechaContabilizacion.Year == flibros.Anio); // Funcionando
        }

        if (flibros.Filtro == false)
        {
            LibroMayorCompleto = LibroMayorCompleto.Where(r => r.FechaContabilizacion.Year == DateTime.Now.Year);
        }

        if (flibros.Mes != 0)
        {
            LibroMayorCompleto = LibroMayorCompleto.Where(r => r.FechaContabilizacion.Month == flibros.Mes); // Funcionando
        }

        if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
        {

            LibroMayorCompleto = LibroMayorCompleto.Where(r => r.FechaContabilizacion >= dtFechaInicio && r.FechaContabilizacion <= dtFechaFin); // Funcionando
        }

        if (!string.IsNullOrWhiteSpace(flibros.Rut))
        {

            LibroMayorCompleto = LibroMayorCompleto.Where(r => r.Rut.Contains(flibros.Rut)); // Funcionando
        }

        if (!string.IsNullOrWhiteSpace(flibros.Glosa))
        {
            LibroMayorCompleto = LibroMayorCompleto.Where(r => r.Glosa.Contains(flibros.Glosa)); // Funcionando
        }

        if (!string.IsNullOrWhiteSpace(flibros.CuentaContableID))
        {
            LibroMayorCompleto = LibroMayorCompleto.Where(r => r.CtaContablesID.ToString() == flibros.CuentaContableID); // Funcionando
        }

        if (!string.IsNullOrWhiteSpace(flibros.RazonPrestador))
        {
            LibroMayorCompleto = LibroMayorCompleto.Where(x => x.RazonSocial.Contains(flibros.RazonPrestador));
        }
        if (flibros.NumVoucher != 0 && flibros.NumVoucher > 0)
        {
            LibroMayorCompleto = LibroMayorCompleto.Where(x => x.NumVoucher == flibros.NumVoucher);
        }
        if (flibros.EstaConciliado == true)
        {
            LibroMayorCompleto = LibroMayorCompleto.Where(x => x.EstaConciliado == false);
        }




        int TotalRegistros = LibroMayorCompleto.Count();

        if (flibros.cantidadRegistrosPorPagina != 0)
        {
            LibroMayorCompleto = LibroMayorCompleto.OrderBy(r => r.FechaContabilizacion)
                                     .Skip((flibros.pagina - 1) * flibros.cantidadRegistrosPorPagina)
                                     .Take(flibros.cantidadRegistrosPorPagina);

        }
        else if (flibros.cantidadRegistrosPorPagina == 0)
        {
            LibroMayorCompleto = LibroMayorCompleto.OrderBy(r => r.FechaContabilizacion);
        }

        int NumLinea = 1;

        foreach (var itemLibroMayor in LibroMayorCompleto)
        {
            string[] ArrayLibroMayor = new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-" };

            string Comprobante = ParseExtensions.TipoVoucherToShortName(itemLibroMayor.Comprobante) + " " + itemLibroMayor.ComprobanteP2.ToString() + "   " + itemLibroMayor.ComprobanteP3;
            //int EvitarRedundanciaPrestadores = ReturnValues.Where(x => x.Contains(itemLibroMayor.Rut) && x.Contains(Comprobante)).Count();
            if (itemLibroMayor.Rut != "-")
            {
                int EvitarRedundanciaPrestadores = ReturnValues.Where(x => x.Contains(itemLibroMayor.Rut) && x.Contains(Comprobante)).Count();
                if (EvitarRedundanciaPrestadores > 0)
                    continue;
            }

            ArrayLibroMayor[4] = itemLibroMayor.RazonSocial;
            ArrayLibroMayor[5] = itemLibroMayor.Rut;

            ArrayLibroMayor[0] = NumLinea.ToString();
            ArrayLibroMayor[1] = ParseExtensions.ToDD_MM_AAAA(itemLibroMayor.FechaContabilizacion);
            ArrayLibroMayor[2] = Comprobante;
            ArrayLibroMayor[3] = itemLibroMayor.Glosa;

            decimal TotalLineaMontoDebe = itemLibroMayor.Debe; // Listo
            decimal TotalLineaMontoHaber = itemLibroMayor.Haber;// Listo

            TotalLineaMontoDebe += TotalMontoDebe;
            TotalLineaMontoHaber += TotalMontoHaber;

            decimal TotalSaldoEstaLinea = 0;
            // Si es Cuenta 1 o 5
            if (itemLibroMayor.CtaContableClasi == ClasificacionCtaContable.ACTIVOS ||
               itemLibroMayor.CtaContableClasi == ClasificacionCtaContable.RESULTADOPERDIDA) // Los activos se comportan como las cuentas 5 en terminos de lo que es el calculo del saldo.
            {
                TotalSaldoEstaLinea = TotalLineaMontoDebe - TotalLineaMontoHaber;
            }

            // Si es Cuenta 2 o 4
            if (itemLibroMayor.CtaContableClasi == ClasificacionCtaContable.PASIVOS ||
               itemLibroMayor.CtaContableClasi == ClasificacionCtaContable.RESULTADOGANANCIA) // Los Pasivos se comportan como las cuentas 4 en terminos de lo que es el calculo del saldo.
            {
                TotalSaldoEstaLinea = TotalLineaMontoHaber - TotalLineaMontoDebe;
            }

            IEnumerable<bool> ResetSaldo = ReturnValues.Select(r => !r.Contains("[" + itemLibroMayor.CodigoInterno + "] " + itemLibroMayor.CtaContNombre));

            if (ResetSaldo.All(x => x)) // ".All(x => x)" Si la condición es verdadera 
            {
                if (TotalLineaMontoHaber != 0)
                    TotalSaldo = TotalLineaMontoHaber - TotalLineaMontoHaber;

                if (TotalLineaMontoDebe != 0)
                    TotalSaldo = TotalLineaMontoDebe - TotalLineaMontoDebe;
            }

            TotalSaldo = TotalSaldo + TotalSaldoEstaLinea;

            ArrayLibroMayor[6] = ParseExtensions.NumeroConPuntosDeMiles(TotalLineaMontoDebe); //Se eliminan los puntos de los decimales para poder parsearlo a int y sacar los calculos en la vista.
            ArrayLibroMayor[7] = ParseExtensions.NumeroConPuntosDeMiles(TotalLineaMontoHaber);
            ArrayLibroMayor[8] = ParseExtensions.NumeroConPuntosDeMiles(TotalSaldo);
            ArrayLibroMayor[9] = "[" + itemLibroMayor.CodigoInterno + "] " + itemLibroMayor.CtaContNombre;
            ArrayLibroMayor[10] = itemLibroMayor.NumVoucher.ToString();
            ArrayLibroMayor[11] = itemLibroMayor.VoucherId.ToString();
            ArrayLibroMayor[12] = itemLibroMayor.DetalleVoucherId.ToString();

            ReturnValues.Add(ArrayLibroMayor);

            TotalFinalMontoDebe += TotalLineaMontoDebe;
            TotalFinalMontohaber += TotalLineaMontoHaber;
            TotalFinalSaldo = TotalFinalMontohaber - TotalFinalMontoDebe;

            NumLinea++;
        }

        string[] TotalRow = new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "-", "-" };

        TotalRow[5] = "Total Final:";
        TotalRow[6] = ParseExtensions.NumeroConPuntosDeMiles(TotalFinalMontoDebe);
        TotalRow[7] = ParseExtensions.NumeroConPuntosDeMiles(TotalFinalMontohaber);
        TotalRow[8] = ParseExtensions.NumeroConPuntosDeMiles(Math.Abs(TotalFinalSaldo));
        ReturnValues.Add(TotalRow);

        //Nota Para el futuro, los valores query strings se tienen que llamar igual que la variable que se desea conservar en el tiempo.
        var Paginacion = new PaginadorModel();
        Paginacion.ResultStringArray = ReturnValues;
        Paginacion.PaginaActual = flibros.pagina;
        Paginacion.TotalDeRegistros = TotalRegistros;
        Paginacion.RegistrosPorPagina = flibros.cantidadRegistrosPorPagina;
        Paginacion.ValoresQueryString = new RouteValueDictionary();

        if (flibros.cantidadRegistrosPorPagina != 25)
            Paginacion.ValoresQueryString["cantidadRegistrosPorPagina"] = flibros.cantidadRegistrosPorPagina;

        if (flibros.Anio != 0)
            Paginacion.ValoresQueryString["Anio"] = flibros.Anio;

        if (flibros.Mes != 0)
            Paginacion.ValoresQueryString["Mes"] = flibros.Mes;

        if (!string.IsNullOrWhiteSpace(flibros.CuentaContableID))
            Paginacion.ValoresQueryString["CuentaContableID"] = flibros.CuentaContableID;

        if (!string.IsNullOrWhiteSpace(flibros.Rut))
            Paginacion.ValoresQueryString["Rut"] = flibros.Rut;

        if (!string.IsNullOrWhiteSpace(flibros.Glosa))
            Paginacion.ValoresQueryString["Glosa"] = flibros.Glosa;

        if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
        {
            Paginacion.ValoresQueryString["FechaInicio"] = flibros.FechaInicio;
            Paginacion.ValoresQueryString["FechaFin"] = flibros.FechaFin;
        }

        if (!string.IsNullOrWhiteSpace(flibros.RazonPrestador))
            Paginacion.ValoresQueryString["RazonPrestador"] = flibros.RazonPrestador;

        if (flibros.NumVoucher != 0 && flibros.NumVoucher > 0)
            Paginacion.ValoresQueryString["NumVoucher"] = flibros.NumVoucher;

        return Paginacion;
    }





    //Balance
    public static List<string[]> GetBalanceGeneral(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable, int CentroCostoID)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalSumasDebitos = 0;
        decimal TotalSumasCreditos = 0;

        decimal TotalSumasSaldoDeudor = 0;
        decimal TotalSumasSaldoAcreedor = 0;

        decimal TotalSumasInventarioActivo = 0;
        decimal TotalSumasInventarioPasivo = 0;
        decimal TotalSumasResultadoPerdidas = 0;
        decimal TotalSumasResultadoGanancias = 0;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            List<DetalleVoucherModel> lstDetalle = new List<DetalleVoucherModel>();
            if(CentroCostoID > 0)
            {
                lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID &&
                                                                                          r.CentroCostoID == CentroCostoID).ToList();
            }
            else
            {
                lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID).ToList();
            }
            if (lstDetalle.Count == 0)
                continue;

            string[] BalanceRow = new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "-", "-","-" };

            decimal SumasDebitosEstaCuenta = 0;
            decimal SumasCreditosEstaCuenta = 0;

            SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
            SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

            /*
            if (Cuenta.InvertirMontosDepreciacion == false)
            {
                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);
            }
            else
            {
                SumasDebitosEstaCuenta = GetTotalHaber(lstDetalle); 
                SumasCreditosEstaCuenta = GetTotalDebe(lstDetalle);
            }*/



            TotalSumasDebitos += SumasDebitosEstaCuenta;
            TotalSumasCreditos += SumasCreditosEstaCuenta;
            decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

            BalanceRow[0] = Cuenta.CodInterno; //Cuenta.GetCtaContableDisplayName();
            BalanceRow[1] = Cuenta.nombre;
            BalanceRow[2] = ParseExtensions.NumberWithDots_para_BalanceGeneral(SumasDebitosEstaCuenta);
            BalanceRow[3] = ParseExtensions.NumberWithDots_para_BalanceGeneral(SumasCreditosEstaCuenta);
            //Si DEBITO - CREDITO es POSITIVO va en la columna SALDOS DEUDOR
            if (TotalSaldoEstaCuenta > 0)
            {
                TotalSumasSaldoDeudor += Math.Abs(TotalSaldoEstaCuenta);
                BalanceRow[4] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
            }
            //Si DEBITO - CREDITO es NEGATIVO va en la columna SALDOS ACREEDOR
            else if (0 > TotalSaldoEstaCuenta)
            {
                TotalSumasSaldoAcreedor += Math.Abs(TotalSaldoEstaCuenta);
                BalanceRow[5] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
            }

            if (Cuenta.Clasificacion == ClasificacionCtaContable.ACTIVOS)
            {
                //fix 04-03-2019
                if (TotalSaldoEstaCuenta >= 0) // si es saldo deudor
                {

                    if (Cuenta.InvertirMontosDepreciacion == false)
                    {
                        BalanceRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioActivo += Math.Abs(TotalSaldoEstaCuenta);
                    }
                    else
                    {
                        BalanceRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioPasivo += Math.Abs(TotalSaldoEstaCuenta);
                    }
                }
                else // si es saldo acreedor
                {

                    if (Cuenta.InvertirMontosDepreciacion == false && !Cuenta.CodInterno.Contains("1202"))
                    {
                        BalanceRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioPasivo += Math.Abs(TotalSaldoEstaCuenta);
                    }
                    else if (Cuenta.CodInterno.Contains("1202"))
                    {
                        BalanceRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioPasivo += Math.Abs(TotalSaldoEstaCuenta);
                    }
                    else
                    {
                        BalanceRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioActivo += Math.Abs(TotalSaldoEstaCuenta);
                    }

                }
            }

            else if (Cuenta.Clasificacion == ClasificacionCtaContable.PASIVOS)
            {
                //fix 04-03-2019
                if (TotalSaldoEstaCuenta >= 0) // si es saldo deudor
                {
                    if (Cuenta.InvertirMontosDepreciacion == false)
                    {
                        BalanceRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioActivo += Math.Abs(TotalSaldoEstaCuenta);
                    }
                    else
                    {
                        BalanceRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioPasivo += Math.Abs(TotalSaldoEstaCuenta);
                    }
                }
                else // si es saldo acreedor
                {
                    if (Cuenta.InvertirMontosDepreciacion == false)
                    {
                        BalanceRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioPasivo += Math.Abs(TotalSaldoEstaCuenta);
                    }
                    else
                    {
                        BalanceRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                        TotalSumasInventarioActivo += Math.Abs(TotalSaldoEstaCuenta);
                    }
                }
            }
            else if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA)
            {
                if (TotalSaldoEstaCuenta >= 0) // si es saldo deudor
                {
                    TotalSumasResultadoPerdidas += Math.Abs(TotalSaldoEstaCuenta);
                    BalanceRow[8] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                }
                else
                {
                    TotalSumasResultadoGanancias += Math.Abs(TotalSaldoEstaCuenta);
                    BalanceRow[9] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                }
            }
            else if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA)
            {
                if (TotalSaldoEstaCuenta >= 0) // si es saldo deudor
                {
                    TotalSumasResultadoPerdidas += Math.Abs(TotalSaldoEstaCuenta);
                    BalanceRow[8] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                }
                else
                {
                    TotalSumasResultadoGanancias += Math.Abs(TotalSaldoEstaCuenta);
                    BalanceRow[9] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSaldoEstaCuenta));
                }
            }
            else
            {

            }

            BalanceRow[10] = Cuenta.CuentaContableModelID.ToString();
            ReturnValues.Add(BalanceRow);
        }
        ReturnValues = ReturnValues.OrderBy(r => r[0]).ToList();
        //SUMAS de los totales
        string[] OtherRow = new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "-", "-","-"};
        OtherRow[0] = "";
        OtherRow[1] = "SUMAS";
        OtherRow[2] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasDebitos);
        OtherRow[3] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasCreditos);
        OtherRow[4] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasSaldoDeudor);
        OtherRow[5] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasSaldoAcreedor);
        OtherRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasInventarioActivo);
        OtherRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasInventarioPasivo);
        OtherRow[8] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasResultadoPerdidas);
        OtherRow[9] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasResultadoGanancias);
        OtherRow[10] = "";
        ReturnValues.Add(OtherRow);
        //UTILIDADES del ejercicio
        OtherRow = new string[] { "", "", "", "", "", "", "", "", "", "", "" };
        if (TotalSumasResultadoGanancias > TotalSumasResultadoPerdidas)
        {
            OtherRow[1] = "RESULTADO GANANCIA";
        }else if (TotalSumasResultadoGanancias < TotalSumasResultadoPerdidas)
        {
            OtherRow[1] = "RESULTADO PERDIDA";
        }

        
        decimal DiferenciaInventarioTotal = TotalSumasInventarioActivo - TotalSumasInventarioPasivo;
        decimal DiferenciaResultadoTotal = TotalSumasResultadoPerdidas - TotalSumasResultadoGanancias;
        if (DiferenciaInventarioTotal > 0)
        {
            OtherRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(DiferenciaInventarioTotal));
        }
        else if (DiferenciaInventarioTotal < 0)
        {
            OtherRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(DiferenciaInventarioTotal));
        }

        if (DiferenciaResultadoTotal > 0)
        {
            OtherRow[9] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(DiferenciaResultadoTotal));
        }
        else if (DiferenciaResultadoTotal < 0)
        {
            OtherRow[8] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(DiferenciaResultadoTotal));
        }
        ReturnValues.Add(OtherRow);
        //TOTALES
        OtherRow = new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-"};
        OtherRow[1] = "TOTALES";
        OtherRow[2] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasDebitos);
        OtherRow[3] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasCreditos);
        OtherRow[4] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasSaldoDeudor);
        OtherRow[5] = ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasSaldoAcreedor);
        if (DiferenciaInventarioTotal > 0)
        {
            OtherRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasInventarioPasivo) + Math.Abs(DiferenciaInventarioTotal));
            OtherRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasInventarioActivo));
        }
        else if (DiferenciaInventarioTotal < 0)
        {
            OtherRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasInventarioActivo) + Math.Abs(DiferenciaInventarioTotal));
            OtherRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasInventarioPasivo));
        }
        else
        {
            OtherRow[7] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasInventarioPasivo) + Math.Abs(DiferenciaInventarioTotal));
            OtherRow[6] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasInventarioActivo) + Math.Abs(DiferenciaInventarioTotal));
        }
        if (DiferenciaResultadoTotal > 0)
        {
            OtherRow[9] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoGanancias) + Math.Abs(DiferenciaResultadoTotal));
            OtherRow[8] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoPerdidas));
        }
        else if (DiferenciaResultadoTotal < 0)
        {
            OtherRow[8] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoPerdidas) + Math.Abs(DiferenciaResultadoTotal));
            OtherRow[9] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoGanancias));
        }
        else
        {
            OtherRow[9] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoGanancias) + Math.Abs(DiferenciaResultadoTotal));
            OtherRow[8] = ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoPerdidas) + Math.Abs(DiferenciaResultadoTotal));
        }
        OtherRow[10] = "";
        ReturnValues.Add(OtherRow);
        return ReturnValues;
    }

    public static byte[] GetExcelResultBalanceGeneral(List<VoucherModel> lstVoucher, ClientesContablesModel objCliente, List<CuentaContableModel> lstCuentaContable, bool InformarMembrete, string titulo)
    {
        byte[] ExcelByteArray = null;
        List<string[]> lstBalanceGeneral = GetBalanceGeneral(lstVoucher, lstCuentaContable,0);
        using (XLWorkbook excelFile = new XLWorkbook(@"C:\PROTOEXCEL.xlsx"))
        {
            var workSheet = excelFile.Worksheet(1);

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = "Rut: " + objCliente.RUTEmpresa;
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A4").Value = objCliente.Direccion;
                workSheet.Cell("A5").Value = objCliente.Ciudad;
                workSheet.Cell("A6").Value = objCliente.RUTRepresentante + "  " + objCliente.Representante;
                //workSheet.Cell("A7").Value = objCliente.RUTRepresentante;
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

            int NumeroFilaExcel = 13;
            foreach (string[] tableRow in lstBalanceGeneral)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }
                workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
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
    // Este es el que se llama al controlador.
    public static byte[] GetExcelResultBalanceGeneral(List<string[]> lstBalanceGeneral, ClientesContablesModel objCliente, bool InformarMembrete, string titulo)
    {
        string RutaPlanillaLibroMayor = ParseExtensions.Get_AppData_Path("PROTOEXCEL.xlsx");

        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(RutaPlanillaLibroMayor))
        {
            var workSheet = excelFile.Worksheet(1);

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = "Rut: " + ParseExtensions.FormatoRutMembrete(objCliente.RUTEmpresa);
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A4").Value = objCliente.Direccion;
                workSheet.Cell("A5").Value = objCliente.Ciudad;
                workSheet.Cell("A6").Value = ParseExtensions.FormatoRutMembrete(objCliente.RUTRepresentante) + "  " +objCliente.Representante;
               // workSheet.Cell("A7").Value = ;
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
                workSheet.Cell("C4").Value = titulo;
            }
            else
            {
                workSheet.Cell("C8").Value = string.Empty;
            }

            workSheet.Columns("C:J").Style.NumberFormat.Format = "#,##0 ;-#,##0";

            int NumeroFilaExcel = 13;
            foreach (string[] tableRow in lstBalanceGeneral)
            {
                for (int i = 0; i < tableRow.Length - 1; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }
                workSheet.Range("A" + NumeroFilaExcel + ":J" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("A" + NumeroFilaExcel + ":J" + NumeroFilaExcel).Rows().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
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

    


    public static List<string[]> GetBActivoCirculante(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        //Diferenciar estas variables de las demás tipos de cuentas contables.
        decimal TotalSumaActivos = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.ACTIVOS
                && SubClasiCodigo == "11"
                && SubSubClasiCodigo.Contains("110")
               )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];
                TotalSumaActivos += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);
            }
        }
        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "ACTIVO CIRCULANTE: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumaActivos));

        ReturnValues.Add(Result);

        return ReturnValues;
    }

    public static List<string[]> GetBActivoFijo(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalSumasActivo = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.ACTIVOS
                && SubClasiCodigo == "12"
                && SubSubClasiCodigo.Contains("120")
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                TotalSumasActivo += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);
            }
        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL ACTIVO FIJO: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasActivo);

        ReturnValues.Add(Result);

        return ReturnValues;

    }

    //Balance
    public static List<string[]> GetTotalActivo(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalSumaActivoCirculante = 0;
        decimal TotalSumaActivoFijo = 0;
        decimal TotalSumaActivo = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.ACTIVOS
                && SubClasiCodigo == "11"
                && SubSubClasiCodigo.Contains("110")
               )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];
                TotalSumaActivo += Math.Abs(TotalSaldoEstaCuenta);
                TotalSumaActivoCirculante += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);
            }
        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "ACTIVO CIRCULANTE: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumaActivoCirculante);

        ReturnValues.Add(Result);

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.ACTIVOS
                && SubClasiCodigo == "12"
                && SubSubClasiCodigo.Contains("120")
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                TotalSumaActivo += Math.Abs(TotalSaldoEstaCuenta);
                TotalSumaActivoFijo += Math.Abs(TotalSaldoEstaCuenta);


                ReturnValues.Add(IEstResultRow);
            }
        }

        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL ACTIVO FIJO: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumaActivoFijo);

        ReturnValues.Add(Result);


        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL ACTIVOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumaActivo);

        ReturnValues.Add(Result);

        return ReturnValues;
    }
    public static List<string[]> GetBPasivoCirculante(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalSumasPasivo = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();


            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.PASIVOS
                && SubClasiCodigo == "22"
                && SubSubClasiCodigo.Contains("220")
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                TotalSumasPasivo += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);

            }

        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL PASIVO CIRCULANTE: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumasPasivo);

        ReturnValues.Add(Result);


        return ReturnValues;
    }

    public static List<string[]> GetBPasivoNoCorriente(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalSumaPasivo = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.PASIVOS
                && SubClasiCodigo == "23"
                && SubSubClasiCodigo.Contains("230")
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                TotalSumaPasivo += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);

            }

        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL PASIVO NO CORRIENTE: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumaPasivo);

        ReturnValues.Add(Result);

        return ReturnValues;
    }

    public static List<string[]> GetBTotalPasivoAndPatrimonioNeto(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalSumaPasivo = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string Capital = "2401";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.PASIVOS
                && SubSubClasiCodigo == Capital)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                TotalSumaPasivo += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);

            }

        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL PATRIMONIO NETO: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalSumaPasivo);

        ReturnValues.Add(Result);

        return ReturnValues;
    }

    public static List<string[]> GetTotalPasivos(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalSumasPasivo = 0;
        decimal TotalPasivoCirculante = 0;
        decimal TotalPasivoNoCorriente = 0;
        decimal TotalPatrimonioNeto = 0;

        int NumeroDeColumnas = 3;

        //Pasivo Circulante

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();


            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.PASIVOS
                && SubClasiCodigo == "22"
                && SubSubClasiCodigo.Contains("220")
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                TotalPasivoCirculante += Math.Abs(TotalSaldoEstaCuenta);
                TotalSumasPasivo += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);

            }

        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL PASIVO CIRCULANTE: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalPasivoCirculante));

        ReturnValues.Add(Result);

        //Fin pasivo Circulante


        //Pasivo No Corriente

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.PASIVOS
                && SubClasiCodigo == "23"
                && SubSubClasiCodigo.Contains("230")
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                TotalPasivoNoCorriente += Math.Abs(TotalSaldoEstaCuenta);
                TotalSumasPasivo += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);

            }

        }

        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL PASIVO NO CORRIENTE: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalPasivoNoCorriente));

        ReturnValues.Add(Result);

        //Fin pasivo No Corriente

        //Patrimonio Neto

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();



            string Capital = "2401";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.PASIVOS
                && SubSubClasiCodigo == Capital)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                TotalPatrimonioNeto += Math.Abs(TotalSaldoEstaCuenta);
                TotalSumasPasivo += Math.Abs(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);
            }
        }
        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL PATRIMONIO NETO: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalPatrimonioNeto));

        ReturnValues.Add(Result);
        //Fin patrimonio neto

        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL PASIVO:" + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasPasivo));

        ReturnValues.Add(Result);

        return ReturnValues;
    }

    public static byte[] GetExcelInfomesBalance(List<string[]> lstIEstadoResultado, ClientesContablesModel objCliente, bool InformarMembrete, string titulo)
    {
        string EsteExcel = ParseExtensions.Get_AppData_Path("InformesBalance.xlsx");
            
        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(EsteExcel))
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
                workSheet.Cell("C8").Value = titulo;
            }
            else
            {
                workSheet.Cell("C8").Value = string.Empty;
            }

            int NumeroFilaExcel = 13;
            foreach (string[] tableRow in lstIEstadoResultado)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }

                workSheet.Range("A" + NumeroFilaExcel + ":C" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                workSheet.Range("A" + NumeroFilaExcel + ":C" + NumeroFilaExcel).Rows().Style.Border.InsideBorder = XLBorderStyleValues.Double;
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

    public static byte[] GetExcelInfomesBalance(List<VoucherModel> lstVoucher, ClientesContablesModel objCliente, List<CuentaContableModel> lstCuentaContable, bool InformarMembrete, string titulo)
    {
        byte[] ExcelByteArray = null;

        string EsteExcel = ParseExtensions.Get_AppData_Path("InformesBalance.xlsx");
        List<string[]> lstActivoCirculante = GetBActivoCirculante(lstVoucher, lstCuentaContable);
        using (XLWorkbook excelFile = new XLWorkbook(EsteExcel))
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

            int NumeroFilaExcel = 13;
            foreach (string[] tableRow in lstActivoCirculante)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }
                workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows().Style.Border.InsideBorder = XLBorderStyleValues.Double;
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




    //Estado de resultados.

    public static List<string[]> GetInfomeEstadoResultado(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        //Diferenciar estas variables de las demás tipos de cuentas contables.
        decimal TotalSumasResultadoPerdidas = 0;
        decimal TotalSumasResultadoGanancias = 0;

        decimal TotalSumaContextoEBITDA = 0;

        decimal TotalSumaContextoEBIT = 0;

        decimal TotalSumaGxIntereses = 0;
        decimal TotalSumaIxIntereses = 0;

        decimal TotalSumaImpuestos = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                    .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                    .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno; // Subclasificacion consta de 2 digitos
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName(); //SubSubClasificacion consta de 4 digitos
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string IngresosPorVenta = "5101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA
                && SubSubClasiCodigo == IngresosPorVenta)
            {

                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasCreditosEstaCuenta - SumasDebitosEstaCuenta;
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); 
                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubClasi = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubClasi.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(subsubrow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = ParseExtensions.NumeroConPuntosDeMiles(TotalSaldoEstaCuenta);

                TotalSumasResultadoGanancias += TotalSaldoEstaCuenta;

                ReturnValues.Add(IEstResultRow);
            }
        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL GANANCIA: " + ParseExtensions.NumeroConPuntosDeMiles(TotalSumasResultadoGanancias);
        ReturnValues.Add(Result);

        foreach (CuentaContableModel Cuenta in lstCuentaContable) // Sacamos Todas las Perdidas
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID).OrderBy(x => x.FechaDoc).ToList();

            if (lstDetalle.Count == 0)
                continue;

            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string CostosDeLaMercaderiaVendida = "4101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == CostosDeLaMercaderiaVendida)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasCreditosEstaCuenta - SumasDebitosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;

                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;

                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName)); //Si no existe entonces 

                if (FiltroRepetidos.All(x => x)) //Verificamos que la lista solo tenga lo pedido solo si no la contiene la va a agregar esto es como decir (True)
                {
                    ReturnValues.Add(subsubrow); //Agregamos solo si no está repetida dentro de la colección "FiltroRepetidos".

                }
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); -> en caso de necesitar en un futuro la fecha.
                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                IEstResultRow[2] = ParseExtensions.NumeroConPuntosDeMiles(TotalSaldoEstaCuenta);

                TotalSumasResultadoPerdidas += TotalSaldoEstaCuenta;

                ReturnValues.Add(IEstResultRow);

            }

        }
        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL PERDIDA: " + ParseExtensions.NumeroConPuntosDeMiles(TotalSumasResultadoPerdidas);

        decimal TotalBruto = 0;
      
        TotalBruto = TotalSumasResultadoGanancias - Math.Abs(TotalSumasResultadoPerdidas);
 

  

        string[] resultBruto = new string[NumeroDeColumnas];
        resultBruto[2] = "TOTAL MARGEN BRUTO: " + ParseExtensions.NumeroConPuntosDeMiles(TotalBruto);

        ReturnValues.Add(Result);
        ReturnValues.Add(resultBruto);

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string Amortizacion = "4112";
            string GastosPorIntereses = "4201";
            string IngresosPorIntereses = "5201";
            string Impuestos = "4113";
            string CostoMercVend = "4101";


            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA &&
                SubSubClasiCodigo != Amortizacion &&
                SubSubClasiCodigo != GastosPorIntereses &&
                SubSubClasiCodigo != IngresosPorIntereses &&
                SubSubClasiCodigo != Impuestos &&
                SubSubClasiCodigo != CostoMercVend
                )
                {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoPerdidas += TotalSaldoEstaCuenta;
                TotalSumaContextoEBITDA += TotalSaldoEstaCuenta;
                IEstResultRow[2] = ParseExtensions.NumeroConPuntosDeMiles(TotalSaldoEstaCuenta);



                ReturnValues.Add(IEstResultRow);
            }
        }

        decimal TotalEBITDA = TotalBruto - TotalSumaContextoEBITDA;

        Result = new string[NumeroDeColumnas];
        Result[2] = "EBITDA: " + ParseExtensions.NumeroConPuntosDeMiles(TotalEBITDA);

        ReturnValues.Add(Result);


        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string Amortizacion = "4112";


            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == Amortizacion)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasCreditosEstaCuenta - SumasDebitosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoPerdidas += TotalSaldoEstaCuenta;
                TotalSumaContextoEBIT += TotalSaldoEstaCuenta;
                IEstResultRow[2] = ParseExtensions.NumeroConPuntosDeMiles(TotalSaldoEstaCuenta);



                ReturnValues.Add(IEstResultRow);
            }
        }

        decimal TotalEBIT = TotalEBITDA - TotalSumaContextoEBIT;

        Result = new string[NumeroDeColumnas];
        Result[2] = "EBIT: " + ParseExtensions.NumeroConPuntosDeMiles(TotalEBIT);

        ReturnValues.Add(Result);

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                            .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                            .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string GxIntereses = "4201";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == GxIntereses)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasCreditosEstaCuenta - SumasDebitosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoPerdidas += TotalSaldoEstaCuenta;
                TotalSumaGxIntereses += TotalSaldoEstaCuenta;
                IEstResultRow[2] = ParseExtensions.NumeroConPuntosDeMiles(TotalSaldoEstaCuenta);



                ReturnValues.Add(IEstResultRow);
            }

        }

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                            .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                            .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string IxIntereses = "5201";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA && SubSubClasiCodigo == IxIntereses)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasCreditosEstaCuenta - SumasDebitosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoGanancias += TotalSaldoEstaCuenta;
                TotalSumaIxIntereses += TotalSaldoEstaCuenta;
                IEstResultRow[2] = ParseExtensions.NumeroConPuntosDeMiles(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);
            }

        }

        decimal TotalGastos = TotalSumaGxIntereses - TotalSumaIxIntereses;
        decimal TotalEBT = 0;

        if (TotalSumaIxIntereses > TotalSumaGxIntereses)
        {
            TotalEBT = TotalGastos + TotalSumaContextoEBIT;
        }
        else if (TotalSumaIxIntereses < TotalSumaGxIntereses)
        {
            TotalEBT = TotalGastos - TotalSumaContextoEBIT;
        }


        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL EBT: " + ParseExtensions.NumeroConPuntosDeMiles(TotalEBT);

        ReturnValues.Add(Result);



        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                            .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                            .OrderBy(x => x.FechaDoc).ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();
            string Impuestos = "4113";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == Impuestos)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasCreditosEstaCuenta - SumasDebitosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoPerdidas += TotalSaldoEstaCuenta;
                TotalSumaImpuestos += TotalSaldoEstaCuenta;
                IEstResultRow[2] = ParseExtensions.NumeroConPuntosDeMiles(TotalSaldoEstaCuenta);

                ReturnValues.Add(IEstResultRow);
            }

        }


        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL IMPUESTOS: " + ParseExtensions.NumeroConPuntosDeMiles(TotalSumaImpuestos);

        ReturnValues.Add(Result);

        decimal ResultadoDelEjercicio = TotalSumasResultadoGanancias - Math.Abs(TotalSumasResultadoPerdidas);

        Result = new string[NumeroDeColumnas];
        Result[2] = "RESULTADO DEL EJERCICIO: " + ParseExtensions.NumeroConPuntosDeMiles(ResultadoDelEjercicio);

        ReturnValues.Add(Result);

        return ReturnValues;
    }

    public static byte[] GetExcelResultIEstadoResultado(List<string[]> lstIEstadoResultado, ClientesContablesModel objCliente, bool InformarMembrete, string titulo)
    {
        string RutaPlanillaLibroMayor = ParseExtensions.Get_AppData_Path("EstadoResultado.xlsx");

        byte[] ExcelByteArray = null;
        using (XLWorkbook excelFile = new XLWorkbook(RutaPlanillaLibroMayor))
        {
            var workSheet = excelFile.Worksheet(1);

            if (InformarMembrete == true)
            {
                workSheet.Cell("A1").Value = objCliente.RazonSocial;
                workSheet.Cell("A2").Value = ParseExtensions.FormatoRutMembrete(objCliente.RUTEmpresa);
                workSheet.Cell("A3").Value = objCliente.Giro;
                workSheet.Cell("A4").Value = objCliente.Direccion;
                workSheet.Cell("A5").Value = objCliente.Ciudad;
                workSheet.Cell("A6").Value = objCliente.Representante;
                workSheet.Cell("A7").Value = ParseExtensions.FormatoRutMembrete(objCliente.RUTRepresentante);
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
                workSheet.Cell("B8").Value = titulo;
            }
            else
            {
                workSheet.Cell("C8").Value = string.Empty;
            }

            
            workSheet.Column("C").Style.NumberFormat.Format = "#,##0 ;-#,##0";

            int NumeroFilaExcel = 13;
            foreach (string[] tableRow in lstIEstadoResultado)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }

                workSheet.Range("A" + NumeroFilaExcel + ":C" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                workSheet.Range("A" + NumeroFilaExcel + ":C" + NumeroFilaExcel).Rows().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
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


    public static List<string[]> GetIEBruto(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();

        //Diferenciar estas variables de las demás tipos de cuentas contables.
        decimal TotalSumasResultadoPerdidas = 0;
        decimal TotalSumasResultadoGanancias = 0;

        int NumeroDeColumnas = 3;


        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                    .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                    .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno; // Subclasificacion consta de 2 digitos
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName(); //SubSubClasificacion consta de 4 digitos
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string IngresosPorVenta = "5101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA
                && SubSubClasiCodigo == IngresosPorVenta)
            {

                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); 
                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubClasi = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubClasi.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(subsubrow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoGanancias += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];


                ReturnValues.Add(IEstResultRow);

            }

        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL INGRESOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoGanancias));
        ReturnValues.Add(Result);


        foreach (CuentaContableModel Cuenta in lstCuentaContable) // Sacamos Todas las Perdidas
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID).OrderBy(x => x.FechaDoc).ToList();

            if (lstDetalle.Count == 0)
                continue;

            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string CostosDeLaMercaderiaVendida = "410";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo.Contains(CostosDeLaMercaderiaVendida))
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;

                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;

                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName)); //Si no existe entonces 

                if (FiltroRepetidos.All(x => x)) //Verificamos que la lista solo tenga lo pedido solo si no la contiene la va a agregar esto es como decir (True)
                {
                    ReturnValues.Add(subsubrow); //Agregamos solo si no está repetida dentro de la colección "FiltroRepetidos".
                                                 //Quizá no podemos hacer algunas acciones con un array de string quizá no se puedan hacer ciertas consultas LINQ.
                                                 //pero podemos usar otro tipo de opciones para  llegar y evaluar lo que debe o no pasar.
                }
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); -> en caso de necesitar en un futuro la fecha.
                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoPerdidas += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];




                ReturnValues.Add(IEstResultRow);

            }

        }
        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL EGRESOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoPerdidas));

        decimal TotalBruto = TotalSumasResultadoGanancias - TotalSumasResultadoPerdidas;

        string[] resultBruto = new string[NumeroDeColumnas];
        resultBruto[2] = "TOTAL MARGEN BRUTO: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalBruto));

        ReturnValues.Add(Result);
        ReturnValues.Add(resultBruto);

        return ReturnValues;
    }

    public static byte[] GetExcelInformesEstadoResultado(List<string[]> lstIEstadoResultado, ClientesContablesModel objCliente, bool InformarMembrete, string titulo)
    {
        string RutaEsteExcel = ParseExtensions.Get_AppData_Path("InformesEstadoResultado.xlsx");

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

            if (string.IsNullOrWhiteSpace(titulo) == false)
            {
                workSheet.Cell("C8").Value = titulo;
            }
            else
            {
                workSheet.Cell("C8").Value = string.Empty;
            }

            int NumeroFilaExcel = 13;
            foreach (string[] tableRow in lstIEstadoResultado)
            {
                for (int i = 0; i < tableRow.Length; i++)
                {
                    workSheet.Cell(NumeroFilaExcel, i + 1).Value = tableRow[i];
                }

                workSheet.Range("A" + NumeroFilaExcel + ":C" + NumeroFilaExcel).Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                workSheet.Range("A" + NumeroFilaExcel + ":C" + NumeroFilaExcel).Rows().Style.Border.InsideBorder = XLBorderStyleValues.Double;
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

    public static List<string[]> GetIEEBITDA(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {

        List<string[]> ReturnValues = new List<string[]>();

        //Diferenciar estas variables de las demás tipos de cuentas contables.
        decimal TotalSumasResultadoPerdidas = 0;
        decimal TotalSumasResultadoGanancias = 0;
        decimal TotalSumaContextoEBITDA = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            // List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID).ToList();
            //if (lstDetalle.Count == 0)
            //    continue;
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                    .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                    .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno; // Subclasificacion consta de 2 digitos
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName(); //SubSubClasificacion consta de 4 digitos
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string IngresosPorVenta = "5101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA
                && SubSubClasiCodigo == IngresosPorVenta)
            {

                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); 
                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubClasi = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubClasi.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(subsubrow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoGanancias += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];


                ReturnValues.Add(IEstResultRow);

            }

        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL INGRESOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoGanancias));
        ReturnValues.Add(Result);

        foreach (CuentaContableModel Cuenta in lstCuentaContable) // Sacamos Todas las Perdidas
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID).OrderBy(x => x.FechaDoc).ToList();

            if (lstDetalle.Count == 0)
                continue;

            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string CostosDeLaMercaderiaVendida = "4101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == CostosDeLaMercaderiaVendida)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;

                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;

                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName)); //Si no existe entonces 

                if (FiltroRepetidos.All(x => x)) //Verificamos que la lista solo tenga lo pedido solo si no la contiene la va a agregar esto es como decir (True)
                {
                    ReturnValues.Add(subsubrow); //Agregamos solo si no está repetida dentro de la colección "FiltroRepetidos".
                                                 //Quizá no podemos hacer algunas acciones con un array de string quizá no se puedan hacer ciertas consultas LINQ.
                                                 //pero podemos usar otro tipo de opciones para  llegar y evaluar lo que debe o no pasar.
                }
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); -> en caso de necesitar en un futuro la fecha.
                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoPerdidas += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);

            }

        }
        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL EGRESOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoPerdidas));

        decimal TotalBruto = TotalSumasResultadoGanancias - TotalSumasResultadoPerdidas;

        string[] resultBruto = new string[NumeroDeColumnas];
        resultBruto[2] = "TOTAL MARGEN BRUTO: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalBruto));

        ReturnValues.Add(Result);
        ReturnValues.Add(resultBruto);



        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string Amortizacion = "4112";
            string GastosPorIntereses = "4201";
            string IngresosPorIntereses = "5201";
            string Impuestos = "4113";
            string CostoMercVend = "4101";


            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA &&
                SubSubClasiCodigo != Amortizacion &&
                SubSubClasiCodigo != GastosPorIntereses &&
                SubSubClasiCodigo != IngresosPorIntereses &&
                SubSubClasiCodigo != Impuestos &&
                SubSubClasiCodigo != CostoMercVend
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumaContextoEBITDA += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);
            }
        }

        decimal TotalEBITDA = TotalBruto - TotalSumaContextoEBITDA;

        Result = new string[NumeroDeColumnas];
        Result[2] = "EBITDA: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalEBITDA));

        ReturnValues.Add(Result);

        return ReturnValues;
    }

    public static List<string[]> GetIEEBIT(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {

        List<string[]> ReturnValues = new List<string[]>();

        //Diferenciar estas variables de las demás tipos de cuentas contables.
        decimal TotalSumasResultadoPerdidas = 0;
        decimal TotalSumasResultadoGanancias = 0;
        decimal TotalSumaContextoEBITDA = 0;
        decimal TotalSumaContextoEBIT = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            // List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID).ToList();
            //if (lstDetalle.Count == 0)
            //    continue;
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                    .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                    .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno; // Subclasificacion consta de 2 digitos
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName(); //SubSubClasificacion consta de 4 digitos
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string IngresosPorVenta = "5101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA
                && SubSubClasiCodigo == IngresosPorVenta)
            {

                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); 
                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubClasi = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubClasi.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(subsubrow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoGanancias += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);

            }

        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL INGRESOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoGanancias));
        ReturnValues.Add(Result);

        foreach (CuentaContableModel Cuenta in lstCuentaContable) // Sacamos Todas las Perdidas
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID).OrderBy(x => x.FechaDoc).ToList();

            if (lstDetalle.Count == 0)
                continue;

            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string CostosDeLaMercaderiaVendida = "4101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == CostosDeLaMercaderiaVendida)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;

                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;

                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName)); //Si no existe entonces 

                if (FiltroRepetidos.All(x => x)) //Verificamos que la lista solo tenga lo pedido solo si no la contiene la va a agregar esto es como decir (True)
                {
                    ReturnValues.Add(subsubrow); //Agregamos solo si no está repetida dentro de la colección "FiltroRepetidos".

                }
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); -> en caso de necesitar en un futuro la fecha.
                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoPerdidas += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);

            }

        }
        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL EGRESOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoPerdidas));

        decimal TotalBruto = TotalSumasResultadoGanancias - TotalSumasResultadoPerdidas;

        string[] resultBruto = new string[NumeroDeColumnas];
        resultBruto[2] = "TOTAL MARGEN BRUTO: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalBruto));

        ReturnValues.Add(Result);
        ReturnValues.Add(resultBruto);

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string Amortizacion = "4112";
            string GastosPorIntereses = "4201";
            string IngresosPorIntereses = "5201";
            string Impuestos = "4113";
            string CostoMercVend = "4101";


            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA &&
                SubSubClasiCodigo != Amortizacion &&
                SubSubClasiCodigo != GastosPorIntereses &&
                SubSubClasiCodigo != IngresosPorIntereses &&
                SubSubClasiCodigo != Impuestos &&
                SubSubClasiCodigo != CostoMercVend
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumaContextoEBITDA += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] =Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);
            }
        }

        decimal TotalEBITDA = TotalBruto - TotalSumaContextoEBITDA;

        Result = new string[NumeroDeColumnas];
        Result[2] = "EBITDA: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalEBITDA));

        ReturnValues.Add(Result);


        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string Amortizacion = "4112";


            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == Amortizacion)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumaContextoEBIT += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);
            }
        }

        decimal TotalEBIT = TotalEBITDA - TotalSumaContextoEBIT;

        Result = new string[NumeroDeColumnas];
        Result[2] = "EBIT: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalEBIT));

        ReturnValues.Add(Result);

        return ReturnValues;
    }


    public static List<string[]> GetIEEBT(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable)
    {
        List<string[]> ReturnValues = new List<string[]>();


        decimal TotalSumasResultadoPerdidas = 0;
        decimal TotalSumasResultadoGanancias = 0;

        decimal TotalSumaContextoEBITDA = 0;

        decimal TotalSumaContextoEBIT = 0;

        decimal TotalSumaGxIntereses = 0;
        decimal TotalSumaIxIntereses = 0;

        int NumeroDeColumnas = 3;

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                    .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                    .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno; // Subclasificacion consta de 2 digitos
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName(); //SubSubClasificacion consta de 4 digitos
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string IngresosPorVenta = "5101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA
                && SubSubClasiCodigo == IngresosPorVenta)
            {

                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); 
                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubClasi = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubClasi.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(subsubrow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoGanancias += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);

            }

        }

        string[] Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL INGRESOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoGanancias));
        ReturnValues.Add(Result);

        foreach (CuentaContableModel Cuenta in lstCuentaContable) // Sacamos Todas las Perdidas
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID).OrderBy(x => x.FechaDoc).ToList();

            if (lstDetalle.Count == 0)
                continue;

            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;

            string CostosDeLaMercaderiaVendida = "4101";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == CostosDeLaMercaderiaVendida)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;

                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] subsubrow = new string[NumeroDeColumnas];
                subsubrow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;

                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName)); //Si no existe entonces 

                if (FiltroRepetidos.All(x => x)) //Verificamos que la lista solo tenga lo pedido solo si no la contiene la va a agregar esto es como decir (True)
                {
                    ReturnValues.Add(subsubrow); //Agregamos solo si no está repetida dentro de la colección "FiltroRepetidos".

                }
                //string Fecha = ParseExtensions.ToDD_MM_AAAA(lstDetalle.First().FechaDoc); -> en caso de necesitar en un futuro la fecha.
                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumasResultadoPerdidas += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);

            }

        }
        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL EGRESOS: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalSumasResultadoPerdidas));

        decimal TotalBruto = TotalSumasResultadoGanancias - TotalSumasResultadoPerdidas;

        string[] resultBruto = new string[NumeroDeColumnas];
        resultBruto[2] = "TOTAL MARGEN BRUTO: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalBruto));

        ReturnValues.Add(Result);
        ReturnValues.Add(resultBruto);

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string Amortizacion = "4112";
            string GastosPorIntereses = "4201";
            string IngresosPorIntereses = "5201";
            string Impuestos = "4113";
            string CostoMercVend = "4101";


            if (
                Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA &&
                SubSubClasiCodigo != Amortizacion &&
                SubSubClasiCodigo != GastosPorIntereses &&
                SubSubClasiCodigo != IngresosPorIntereses &&
                SubSubClasiCodigo != Impuestos &&
                SubSubClasiCodigo != CostoMercVend
                )
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumaContextoEBITDA += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);
            }
        }

        decimal TotalEBITDA = TotalBruto - TotalSumaContextoEBITDA;

        Result = new string[NumeroDeColumnas];
        Result[2] = "EBITDA: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalEBITDA));

        ReturnValues.Add(Result);


        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {

            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                             .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                             .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string Amortizacion = "4112";


            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == Amortizacion)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumaContextoEBIT += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);
            }
        }

        decimal TotalEBIT = TotalEBITDA - TotalSumaContextoEBIT;

        Result = new string[NumeroDeColumnas];
        Result[2] = "EBIT: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(Math.Abs(TotalEBIT));

        ReturnValues.Add(Result);

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                            .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                            .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string GxIntereses = "4201";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && SubSubClasiCodigo == GxIntereses)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumaGxIntereses += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);
            }

        }

        foreach (CuentaContableModel Cuenta in lstCuentaContable)
        {
            List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                                            .Where(r => r.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID)
                                                            .ToList();
            if (lstDetalle.Count == 0)
                continue;

            string SubClasiCodigo = Cuenta.SubClasificacion.CodigoInterno;
            string SubClasiName = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasiCodigo = Cuenta.SubSubClasificacion.CodigoInterno;
            string SubSubClasiName = Cuenta.GetSubSubClasificacionName();

            string IxIntereses = "5201";

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA && SubSubClasiCodigo == IxIntereses)
            {
                decimal SumasDebitosEstaCuenta = 0;
                decimal SumasCreditosEstaCuenta = 0;

                SumasDebitosEstaCuenta = GetTotalDebe(lstDetalle);
                SumasCreditosEstaCuenta = GetTotalHaber(lstDetalle);

                decimal TotalSaldoEstaCuenta = SumasDebitosEstaCuenta - SumasCreditosEstaCuenta;

                string[] subrow = new string[NumeroDeColumnas];
                subrow[0] = SubClasiCodigo + "   " + SubClasiName;
                var FiltroRepetidosSubRow = ReturnValues.Select(r => !r.Contains(SubClasiCodigo + "   " + SubClasiName));

                if (FiltroRepetidosSubRow.All(x => x))
                {
                    ReturnValues.Add(subrow);
                }

                string[] SubSubRow = new string[NumeroDeColumnas];
                SubSubRow[0] = SubSubClasiCodigo + "   " + SubSubClasiName;
                var FiltroRepetidos = ReturnValues.Select(r => !r.Contains(SubSubClasiCodigo + "   " + SubSubClasiName));

                if (FiltroRepetidos.All(x => x))
                {
                    ReturnValues.Add(SubSubRow);
                }

                string[] IEstResultRow = new string[NumeroDeColumnas];
                IEstResultRow[0] = Cuenta.CodInterno;
                IEstResultRow[1] = Cuenta.nombre;
                TotalSumaIxIntereses += Math.Abs(TotalSaldoEstaCuenta);
                IEstResultRow[2] = Math.Abs(TotalSaldoEstaCuenta).ToString().Split('.')[0];

                ReturnValues.Add(IEstResultRow);
            }

        }

        decimal TotalGastos = TotalSumaGxIntereses - TotalSumaIxIntereses;
        decimal TotalEBT = 0;

        if (TotalSumaIxIntereses > TotalSumaGxIntereses)
        {
            TotalEBT = TotalGastos + TotalSumaContextoEBIT;
        }
        else if (TotalSumaIxIntereses < TotalSumaGxIntereses)
        {
            TotalEBT = TotalGastos - TotalSumaContextoEBIT;
        }


        Result = new string[NumeroDeColumnas];
        Result[2] = "TOTAL EBT: " + ParseExtensions.NumberWithDots_para_BalanceGeneral(TotalEBT);

        ReturnValues.Add(Result);

        return ReturnValues;
    }



    public static List<string[]> TablaPresupuesto(List<VoucherModel> lstVoucher, List<CuentaContableModel> lstCuentaContable, FacturaPoliContext db, ClientesContablesModel objCliente, PresupuestoModel PresupuestoConsultado)
    {
        List<string[]> ReturnValues = new List<string[]>();

        decimal FinalPresupuesto = 0;
        decimal FinalGastoReal = 0;
        decimal FinalVariacion = 0;
        decimal Porcentaje = 100;
        decimal FinalPorcentaje = 0;
       

        foreach (CuentaContableModel TablaCuenta in lstCuentaContable)
        {

            List<CtasContablesPresupuestoModel> lstCuentasConPresupuesto = db.DBCCPresupuesto.Where(x => x.CuentasContablesModelID == TablaCuenta.CuentaContableModelID &&
                                                                                                         x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                                                         x.PresupuestoModelID == PresupuestoConsultado.PresupuestoModelID).ToList();

            if (lstCuentasConPresupuesto.Count == 0)
                continue;

            foreach (CtasContablesPresupuestoModel ConPresupuesto in lstCuentasConPresupuesto)
            {
                List<DetalleVoucherModel> lstDetalle = lstVoucher.SelectMany(x => x.ListaDetalleVoucher).Where(r => r.ObjCuentaContable.CuentaContableModelID == ConPresupuesto.CuentasContablesModelID &&
                                                                                                                    r.FechaDoc >= ConPresupuesto.FechaInicioPresu &&
                                                                                                                    r.FechaDoc <= ConPresupuesto.FechaVencimientoPresu).ToList();



                decimal TotalHaber = GetTotalHaber(lstDetalle);
                decimal TotalDebe = GetTotalDebe(lstDetalle);
                decimal TotalGastoReal = TotalHaber - TotalDebe;
                string Porcentaje0 = "";
                var TotalPorcentaje = TotalGastoReal * Porcentaje / ConPresupuesto.Presupuesto / 100;
                var TotalPorcentajeRe = Math.Round(TotalPorcentaje);


                if (TotalGastoReal==0)
                {
                    Porcentaje0 = "0 %";
                }
              
                decimal TotalVariacion = Math.Abs(TotalGastoReal) - Math.Abs(ConPresupuesto.Presupuesto);

                FinalPresupuesto += ConPresupuesto.Presupuesto;
                FinalGastoReal += TotalGastoReal;
                FinalVariacion += TotalVariacion;
                FinalPorcentaje += TotalPorcentajeRe;

                string[] Result = new string[] { "-", "-", "-", "-", "-", "-", "-"};
                Result[0] = TablaCuenta.CodInterno;
                Result[1] = TablaCuenta.nombre;
                Result[2] = ParseExtensions.NumeroConPuntosDeMiles(ConPresupuesto.Presupuesto);
                Result[3] = ParseExtensions.NumeroConPuntosDeMiles(Math.Abs(TotalGastoReal));
                Result[4] = ParseExtensions.NumeroConPuntosDeMiles(Math.Abs(TotalVariacion));

                if(TotalGastoReal== 0)
                {
                    Result[5] = Porcentaje0;
                }
                else if (TotalGastoReal != 0)
                {
                    

                    Result[5] = Math.Abs(TotalPorcentajeRe).ToString() + " %";
                }
                

                if (Math.Abs(ConPresupuesto.Presupuesto) >= Math.Abs(TotalGastoReal))
                {
                    Result[6] = "Success";
                } else
                {
                    Result[6] = "Danger";
                }

                ReturnValues.Add(Result);

            }


        }
        string[] FinalResult = new string[] { "-", "TOTAL", "-", "-", "-", "-", "-"};

 

        FinalResult[2] = ParseExtensions.NumeroConPuntosDeMiles(Math.Abs(FinalPresupuesto));
        FinalResult[3] = ParseExtensions.NumeroConPuntosDeMiles(Math.Abs(FinalGastoReal));
        FinalResult[4] = ParseExtensions.NumeroConPuntosDeMiles(Math.Abs(FinalVariacion));
        FinalResult[5] = Math.Abs(FinalPorcentaje).ToString() + " %";

        if (Math.Abs(FinalPresupuesto) >= Math.Abs(FinalGastoReal))
        {
            FinalResult[6] = "Success";
        }
        else
        {
            FinalResult[6] = "Danger";
        }

        ReturnValues.Add(FinalResult);
        
        return ReturnValues;
    }   
}

public enum TipoVoucher
{
    Traspaso = 1,
    Ingreso = 2,
    Egreso = 3,
    Apertura = 4
}


