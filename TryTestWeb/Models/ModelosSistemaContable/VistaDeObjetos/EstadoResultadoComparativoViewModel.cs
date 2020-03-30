using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class EstadoResultadoComparativoViewModel
{
    public string CodigoCta { get; set; }

    public string SubClasCodigo { get; set; }
    public string SubSubClasCodigo { get; set; }
    public string NombreCtaContable { get; set; }
    public string CodigoSubClasiMarca { get; set; }
    public string CodigoSubSubClasiMarca { get; set; }
    public List<decimal> Saldo { get; set; }
    public List<decimal> SaldoTotal { get; set; }

    public static Tuple<List<EstadoResultadoComparativoViewModel>,List<decimal>,List<decimal>,List<decimal>,List<string>,List<string>,List<DateTime>> GetEstadoResultadoComparativo(ClientesContablesModel objCliente, int Mes, int Anio, int MesesAMostrar = 3)
    {

        List<EstadoResultadoComparativoViewModel> EstadoResultadoComp = new List<EstadoResultadoComparativoViewModel>();
        List<VoucherModel> ListaVoucher = objCliente.ListVoucher.Where(x => x.DadoDeBaja == false).ToList();

        List<CuentaContableModel> ListaCuentasContables = objCliente.CtaContable.Where(x => x.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA ||
                                                                                            x.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA).ToList();

        int Dia = 1;

        List<int> MesesConsultados = new List<int>();
        List<DetalleVoucherModel> lstDetalle = new List<DetalleVoucherModel>();

        List<string> lstSubClasificaciones = new List<string>();
        List<string> lstSubSubClasificaciones = new List<string>();

        foreach (CuentaContableModel Cuenta in ListaCuentasContables)
        {
            EstadoResultadoComparativoViewModel ObjARellenar = new EstadoResultadoComparativoViewModel();

            List<decimal> lstSaldo = new List<decimal>();
            decimal SaldoFinalLinea = 0;

            for (int i = 1; i <= MesesAMostrar; i++)
            {
                lstDetalle = ListaVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                         .Where(x => x.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID &&
                                                     x.FechaDoc.Month == Mes && x.FechaDoc.Year == Anio)
                                         .OrderBy(x => x.ObjCuentaContable.Clasificacion).ToList();

                decimal SumasHaber = lstDetalle.Sum(x => x.MontoHaber);
                decimal SumasDebe = lstDetalle.Sum(x => x.MontoDebe);

                decimal Saldo = Math.Abs(SumasHaber) - Math.Abs(SumasDebe);


                if (SaldoFinalLinea == MesesAMostrar)
                    SaldoFinalLinea = 0;

                lstSaldo.Add(Math.Abs(Saldo));

                if (i <= MesesAMostrar && MesesConsultados.Count < MesesAMostrar)
                    MesesConsultados.Add(Mes);

                Mes--;

                if (i == MesesAMostrar)
                    Mes = DateTime.Now.Month;
            }

            if (lstSaldo.All(x => x == 0))
                continue;

            string SubClasificacionCod = Cuenta.SubClasificacion.CodigoInterno;
            string SubSubClasificacionCod = Cuenta.SubSubClasificacion.CodigoInterno;

            string SubClasificacionNombre = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasificacionNombre = Cuenta.SubSubClasificacion.NombreInterno;

            int SinRepetidosSubClasificacion = EstadoResultadoComp.Where(x => x.SubClasCodigo == SubClasificacionCod).Select(x => x.SubClasCodigo).Count();
            int SinRepetidosSubSubClasificicacion = EstadoResultadoComp.Where(x => x.SubSubClasCodigo == SubSubClasificacionCod).Select(x => x.SubSubClasCodigo).Count();

            if (SinRepetidosSubClasificacion == 0)
            {
                ObjARellenar.SubClasCodigo = SubClasificacionCod;
                lstSubClasificaciones.Add(SubClasificacionCod + " " + SubClasificacionNombre);
            }
            if (SinRepetidosSubSubClasificicacion == 0)
            {
                ObjARellenar.SubSubClasCodigo = SubSubClasificacionCod;
                lstSubSubClasificaciones.Add(SubSubClasificacionCod + " " + SubSubClasificacionNombre);
            }

            ObjARellenar.CodigoSubClasiMarca = SubClasificacionCod + " " + SubClasificacionNombre;
            ObjARellenar.CodigoSubSubClasiMarca = SubSubClasificacionCod + " " + SubSubClasificacionNombre;
            ObjARellenar.CodigoCta = Cuenta.CodInterno;
            ObjARellenar.NombreCtaContable = Cuenta.nombre;

            SaldoFinalLinea = lstSaldo.Sum(x => x);
            lstSaldo.Add(SaldoFinalLinea);

            ObjARellenar.Saldo = lstSaldo;


            EstadoResultadoComp.Add(ObjARellenar);

           
        }

        List<DateTime> FechasConsultadas = new List<DateTime>();
        foreach (int itemMes in MesesConsultados)
        {
            DateTime FechaCreada = new DateTime(Anio, itemMes, Dia);
            FechasConsultadas.Add(FechaCreada);
        }

        List<List<decimal>> lstSaldos = new List<List<decimal>>();
        lstSaldos.AddRange(EstadoResultadoComp.Select(x => x.Saldo));
        List<decimal> TotalesGlobales = new List<decimal>();
        int CantidadColumnas = 0;
        if(lstSaldos.Count() > 0)
            CantidadColumnas = lstSaldos.First().Count();
        for (int i = 0; i < CantidadColumnas; i++)
        {
            TotalesGlobales.Add(lstSaldos.Sum(x => x[i]));
        }

        List<List<decimal>> lstSaldosGanancias = new List<List<decimal>>();
        lstSaldosGanancias.AddRange(EstadoResultadoComp.Where(x => x.CodigoSubClasiMarca.StartsWith("5")).Select(x => x.Saldo));
        List<decimal> TotalesGanancias = new List<decimal>();
        for (int i = 0; i < CantidadColumnas; i++)
        {
            TotalesGanancias.Add(lstSaldosGanancias.Sum(x => x[i]));
        }

        List<List<decimal>> lstSaldosPerdidas = new List<List<decimal>>();
        lstSaldosPerdidas.AddRange(EstadoResultadoComp.Where(x => x.CodigoSubClasiMarca.StartsWith("4")).Select(x => x.Saldo));
        List<decimal> TotalesPerdidas = new List<decimal>();
        for (int i = 0; i < CantidadColumnas; i++)
        {
            TotalesPerdidas.Add(lstSaldosPerdidas.Sum(x => x[i]));
        }


        return Tuple.Create(EstadoResultadoComp,TotalesGanancias,TotalesPerdidas,TotalesGlobales, lstSubClasificaciones, lstSubSubClasificaciones,FechasConsultadas);
    }

    public static Tuple<List<EstadoResultadoComparativoViewModel>, List<decimal>, List<decimal>, List<decimal>, List<string>, List<string>, List<DateTime>> EstadoResultadoComparativoConFiltros(ClientesContablesModel objCliente, List<string> Meses, int Anio, int AnioDesde, int AnioHasta)
    {
        List<string> ListaSinErroresMeses = new List<string>();
        ListaSinErroresMeses = Meses;
        List<EstadoResultadoComparativoViewModel> EstadoResultadoComp = new List<EstadoResultadoComparativoViewModel>();
        List<VoucherModel> ListaVoucher = objCliente.ListVoucher.Where(x => x.DadoDeBaja == false).ToList();

        List<CuentaContableModel> ListaCuentasContables = objCliente.CtaContable.Where(x => x.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA ||
                                                                                            x.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA).ToList();

        List<int> AniosConsultados = new List<int>();

        if(AnioDesde > 0 && AnioHasta > 0) { 

            for (int i = AnioDesde; i <= AnioHasta; i++)
            {
                AniosConsultados.Add(AnioDesde++);
            }

        }

        int CantidadDeMeses = 0;

        if(ListaSinErroresMeses != null)
            CantidadDeMeses = Meses.Count();

        int Dia = 1;

        List<int> MesesConsultados = new List<int>();
        List<DetalleVoucherModel> lstDetalle = new List<DetalleVoucherModel>();

        List<string> lstSubClasificaciones = new List<string>();
        List<string> lstSubSubClasificaciones = new List<string>();


        foreach (CuentaContableModel Cuenta in ListaCuentasContables)
        {

            EstadoResultadoComparativoViewModel ObjARellenar = new EstadoResultadoComparativoViewModel();

            List<decimal> lstSaldo = new List<decimal>();
            decimal SaldoFinalLinea = 0;

            if(CantidadDeMeses > 0) { 
                foreach (string Mes in Meses)
                {
                    int ConvertMes = Convert.ToInt32(Mes);
                    lstDetalle = ListaVoucher.SelectMany(x => x.ListaDetalleVoucher)
                           .Where(x => x.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID &&
                                       x.FechaDoc.Month == ConvertMes && x.FechaDoc.Year == Anio)
                           .OrderBy(x => x.ObjCuentaContable.Clasificacion).ToList();

                    decimal SumasHaber = lstDetalle.Sum(x => x.MontoHaber);
                    decimal SumasDebe = lstDetalle.Sum(x => x.MontoDebe);

                    decimal Saldo = Math.Abs(SumasHaber) - Math.Abs(SumasDebe);

                    lstSaldo.Add(Saldo);
                }   
            }else if(AniosConsultados.Count() > 0)
            {
                foreach (int itemAnio in AniosConsultados)
                {
                    lstDetalle = ListaVoucher.SelectMany(x => x.ListaDetalleVoucher)
                          .Where(x => x.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID &&
                                      x.FechaDoc.Year == itemAnio)
                          .OrderBy(x => x.ObjCuentaContable.Clasificacion).ToList();

                    decimal SumasHaber = lstDetalle.Sum(x => x.MontoHaber);
                    decimal SumasDebe = lstDetalle.Sum(x => x.MontoDebe);

                    decimal Saldo = Math.Abs(SumasHaber) - Math.Abs(SumasDebe);

                    lstSaldo.Add(Saldo);
                }
            }

            if (lstSaldo.All(x => x == 0))
                continue;

            string SubClasificacionCod = Cuenta.SubClasificacion.CodigoInterno;
            string SubSubClasificacionCod = Cuenta.SubSubClasificacion.CodigoInterno;

            string SubClasificacionNombre = Cuenta.SubClasificacion.NombreInterno;
            string SubSubClasificacionNombre = Cuenta.SubSubClasificacion.NombreInterno;

            int SinRepetidosSubClasificacion = EstadoResultadoComp.Where(x => x.SubClasCodigo == SubClasificacionCod).Select(x => x.SubClasCodigo).Count();
            int SinRepetidosSubSubClasificicacion = EstadoResultadoComp.Where(x => x.SubSubClasCodigo == SubSubClasificacionCod).Select(x => x.SubSubClasCodigo).Count();

            if (SinRepetidosSubClasificacion == 0)
            {
                ObjARellenar.SubClasCodigo = SubClasificacionCod;
                lstSubClasificaciones.Add(SubClasificacionCod + " " + SubClasificacionNombre);
            }
            if (SinRepetidosSubSubClasificicacion == 0)
            {
                ObjARellenar.SubSubClasCodigo = SubSubClasificacionCod;
                lstSubSubClasificaciones.Add(SubSubClasificacionCod + " " + SubSubClasificacionNombre);
            }

            ObjARellenar.CodigoSubClasiMarca = SubClasificacionCod + " " + SubClasificacionNombre;
            ObjARellenar.CodigoSubSubClasiMarca = SubSubClasificacionCod + " " + SubSubClasificacionNombre;
            ObjARellenar.CodigoCta = Cuenta.CodInterno;
            ObjARellenar.NombreCtaContable = Cuenta.nombre;

            SaldoFinalLinea = lstSaldo.Sum(x => x);
            lstSaldo.Add(SaldoFinalLinea);

            ObjARellenar.Saldo = lstSaldo;

            EstadoResultadoComp.Add(ObjARellenar);
        }

        List<DateTime> FechasConsultadas = new List<DateTime>();
        if (CantidadDeMeses > 0) {
            
            foreach (string itemMes in Meses)
            {
                int MesConvert = Convert.ToInt32(itemMes);
                DateTime FechaCreada = new DateTime(Anio, MesConvert, Dia);
                FechasConsultadas.Add(FechaCreada);
            }
        }
        
        if (AniosConsultados.Count() > 0)
        {
            int Mes = 1;
            foreach (int itemAnio in AniosConsultados)
            {
                DateTime FechaOnlyYear = new DateTime(itemAnio, Mes,Dia);
                FechasConsultadas.Add(FechaOnlyYear);
            }
        }

        var ComprobarSiHayElementos = EstadoResultadoComp.Select(x => x.Saldo).ToList();
        int CantidadDeColumnasSaldo = 0;
        if (ComprobarSiHayElementos.Count() > 0) 
            CantidadDeColumnasSaldo = EstadoResultadoComp.Select(x => x.Saldo).First().Count();
        
        List<List<decimal>> lstSaldosGanancias = new List<List<decimal>>();
        lstSaldosGanancias.AddRange(EstadoResultadoComp.Where(x => x.CodigoSubClasiMarca.StartsWith("5")).Select(x => x.Saldo));
        List<decimal> TotalesGanancias = new List<decimal>();
        for (int i = 0; i < CantidadDeColumnasSaldo; i++)
        {
            TotalesGanancias.Add(lstSaldosGanancias.Sum(x => x[i]));
        }

        List<List<decimal>> lstSaldosPerdidas = new List<List<decimal>>();
        lstSaldosPerdidas.AddRange(EstadoResultadoComp.Where(x => x.CodigoSubClasiMarca.StartsWith("4")).Select(x => x.Saldo));
        List<decimal> TotalesPerdidas = new List<decimal>();
        for (int i = 0; i < CantidadDeColumnasSaldo; i++)
        {
            TotalesPerdidas.Add(lstSaldosPerdidas.Sum(x => x[i]));
        }

        List<decimal> TotalesGlobales = new List<decimal>();
        for (int i = 0; i < CantidadDeColumnasSaldo; i++)
        {
            TotalesGlobales.Add(Math.Abs(TotalesGanancias[i]) - Math.Abs(TotalesPerdidas[i]));
        }
            
        return Tuple.Create(EstadoResultadoComp, TotalesGanancias, TotalesPerdidas, TotalesGlobales, lstSubClasificaciones, lstSubSubClasificaciones, FechasConsultadas);
    }

    public static byte[] GetExcelEstadoComp(List<EstadoResultadoComparativoViewModel> lstEstadoComp,
                                            List<DateTime> FechasConsultadas,List<decimal> ResultadoGanancia,
                                            List<decimal> ResultadoPerdida,List<decimal> ResultadoDelEjercicio,
                                            ClientesContablesModel objCliente, bool InformarMembrete, int AnioDesde, int AnioHasta,
                                            bool BusquedaPorAnio, int Anio)
    {
        string RutaEsteExcel = ParseExtensions.Get_AppData_Path("EstadoResultadoComparativo.xlsx");

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
            if (AnioDesde > 0 && AnioHasta > 0 && BusquedaPorAnio == true)
                workSheet.Cell("C8").Value = "Estado Resultado Comparativo" + "  " + AnioDesde + "  " + "Hasta: " + AnioHasta;
            else if (BusquedaPorAnio == false)
                workSheet.Cell("C8").Value = "Estado Resultado Comparativo" + "  " + Anio;


            workSheet.Column("B").Width = 35;
               
            
            int NumeroFilaColumnas = 12;
            workSheet.Cell(NumeroFilaColumnas,"A").Value = "Codigo";
            workSheet.Cell(NumeroFilaColumnas,"B").Value = "Cuentas";
            int UltimaColumna = FechasConsultadas.Count() + 3;
            for (int i = 0; i < FechasConsultadas.Count(); i++)
            {
                if (AnioDesde > 0 && AnioHasta > 0 && BusquedaPorAnio == true)
                    workSheet.Cell(NumeroFilaColumnas, i + 3).Value = "Año: " + FechasConsultadas[i].Year.ToString();
                else if (BusquedaPorAnio == false)
                    workSheet.Cell(NumeroFilaColumnas, i + 3).Value = ParseExtensions.obtenerNombreMes(FechasConsultadas[i].Month);
            }
            workSheet.Cell(NumeroFilaColumnas, UltimaColumna).Value = "Totales";

            workSheet.Columns(3,UltimaColumna).Style.NumberFormat.Format = "#,##0 ;(#,##0)";
            workSheet.Columns(3, UltimaColumna).Width = 13;

            int NumeroFilaExcel = 13;
            foreach (EstadoResultadoComparativoViewModel ItemGanancia in lstEstadoComp.Where(x => x.CodigoSubClasiMarca.StartsWith("5")))
            {
                workSheet.Cell(NumeroFilaExcel, "A").Value = ItemGanancia.CodigoCta;
                workSheet.Cell(NumeroFilaExcel, "B").Value = ItemGanancia.NombreCtaContable;
                List<decimal> lstSaldo = ItemGanancia.Saldo;
                for (int i = 0; i < lstSaldo.Count(); i++)
                {
                    workSheet.Cell(NumeroFilaExcel, 3 + i).Value = lstSaldo[i];
                }
                NumeroFilaExcel++;
            }

            NumeroFilaExcel++;
            foreach (EstadoResultadoComparativoViewModel ItemPerdida in lstEstadoComp.Where(x => x.CodigoSubClasiMarca.StartsWith("4")))
            {
                workSheet.Cell(NumeroFilaExcel, "A").Value = ItemPerdida.CodigoCta;
                workSheet.Cell(NumeroFilaExcel, "B").Value = ItemPerdida.NombreCtaContable;
                List<decimal> lstSaldo = ItemPerdida.Saldo;
                for (int i = 0; i < lstSaldo.Count(); i++)
                {
                    workSheet.Cell(NumeroFilaExcel, 3 + i).Value = lstSaldo[i];
                }
                NumeroFilaExcel++;
            }

            NumeroFilaExcel++;
            workSheet.Cell(NumeroFilaExcel, "B").Value = "RESULTADO GANANCIA: ";
            for (int i = 0; i < ResultadoGanancia.Count(); i++)
            {

                workSheet.Cell(NumeroFilaExcel, i + 3).Value = ResultadoGanancia[i];
            }

            NumeroFilaExcel++;
            workSheet.Cell(NumeroFilaExcel, "B").Value = "RESULTADO PERDIDA: ";
            for (int i = 0; i < ResultadoPerdida.Count(); i++)
            {
                workSheet.Cell(NumeroFilaExcel, i + 3).Value = ResultadoPerdida[i];
            }

            NumeroFilaExcel++;
            workSheet.Cell(NumeroFilaExcel, "B").Value = "RESULTADO DEL EJERCICIO: ";
            for (int i = 0; i < ResultadoDelEjercicio.Count(); i++)
            {
                workSheet.Cell(NumeroFilaExcel, i + 3).Value = ResultadoDelEjercicio[i]; 
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

    


