using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;
using TryTestWeb;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using Microsoft.AspNet.Identity;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using ClosedXML.Excel;
using System.Web.Configuration;
using System.Data.Entity.Validation;
using Microsoft.Office.Interop.Excel;
using TryTestWeb.Models.ModelosSistemaContable.Common;

public static class ParseExtensions
{
    private static readonly string[] _validExtensions = { ".jpg", ".gif", ".png" }; //  etc
    public static NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CreateSpecificCulture("es").NumberFormat;
    private static readonly TipoDte[] _revisarEstosCAF =
    {
        TipoDte.FacturaElectronica,
        TipoDte.FacturaElectronicaExenta,
        TipoDte.NotaCreditoElectronica,
        TipoDte.NotaDebitoElectronica
    };
    private static readonly int _LowCAFWarningOn = 2;

    public static int KeyModuloSistemaContable { get { return ParseExtensions.ParseInt(WebConfigurationManager.AppSettings["KeyModuloContabilidad"]); } }

    public static bool AreEqualOrZero(params int[] values)
    {
        return values.Any(x => x == 0);
    }

    public static bool CheckEnumValid<TEnum>(int value)
    {
        int MYENUM_MAXIMUM = Enum.GetValues(typeof(TEnum)).Cast<int>().Max();
        int MYENUM_MINIMUM = Enum.GetValues(typeof(TEnum)).Cast<int>().Min();

        if (value < MYENUM_MINIMUM) { return false; }
        if (value > MYENUM_MAXIMUM) { return false; }
        return true;
    }

    public static TEnum SafeEnumConversion<TEnum>(int value)
    {
        int MYENUM_MAXIMUM = Enum.GetValues(typeof(TEnum)).Cast<int>().Max();
        int MYENUM_MINIMUM = Enum.GetValues(typeof(TEnum)).Cast<int>().Min();

        if (value < MYENUM_MINIMUM) { return (TEnum)(object)MYENUM_MINIMUM; }
        if (value > MYENUM_MAXIMUM) { return (TEnum)(object)MYENUM_MAXIMUM; }
        else
        {
            return (TEnum)(object)value;
        }
    }

    public static DateTime CreaFechaLiteral(string fecha)
    {
        string[] ArrayFecha = new string[3];
        
        if (fecha.Contains("-"))
            ArrayFecha = fecha.Split('-');

        int Dia = Int32.Parse(ArrayFecha[0].Trim());
        int Mes = Int32.Parse(ArrayFecha[1].Trim());
        var Anio = Int32.Parse(ArrayFecha[2].Trim());

        DateTime FechaValida = new DateTime(Anio, Mes, Dia);

        return FechaValida;
    }

    public static DateTime ToDD_MM_AAAA_Multi(string dtObj)
    {
        if (string.IsNullOrEmpty(dtObj))
            return DateTime.Now;
        else
        {
            DateTime returnValue = new DateTime();
            if (DateTime.TryParseExact(dtObj, @"dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out returnValue))
                return returnValue;
            if (DateTime.TryParseExact(dtObj, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out returnValue))
                return returnValue;
            if (DateTime.TryParse(dtObj, out returnValue))
                return returnValue;
            return DateTime.Now;
        }
    }


    public static List<ClasificacionCtaContable> ListaClasificacion()
    {
        List<ClasificacionCtaContable> ListaClasificaciones = new List<ClasificacionCtaContable>();

        ListaClasificaciones.Add(ClasificacionCtaContable.ACTIVOS);
        ListaClasificaciones.Add(ClasificacionCtaContable.PASIVOS);
        ListaClasificaciones.Add(ClasificacionCtaContable.RESULTADOGANANCIA);
        ListaClasificaciones.Add(ClasificacionCtaContable.RESULTADOPERDIDA);

        return ListaClasificaciones;
    }

    public static List<Meses> ListaMeses()
    {
        List<Meses> lstMeses = new List<Meses>();

        lstMeses.Add(Meses.Enero);
        lstMeses.Add(Meses.Febrero);
        lstMeses.Add(Meses.Marzo);
        lstMeses.Add(Meses.Abril);
        lstMeses.Add(Meses.Mayo);
        lstMeses.Add(Meses.Junio);
        lstMeses.Add(Meses.Julio);
        lstMeses.Add(Meses.Agosto);
        lstMeses.Add(Meses.Septiembre);
        lstMeses.Add(Meses.Octubre);
        lstMeses.Add(Meses.Noviembre);
        lstMeses.Add(Meses.Diciembre);

        return lstMeses;
    }

    public static Tuple<decimal, decimal> TotalesGananciasYPerdidasDelMes(FacturaPoliContext db, ClientesContablesModel objcliente)
    {
        decimal TotalIngresos = 0;
        decimal TotalEgresos = 0;

        int Mes = DateTime.Now.Month;
        int Anio = DateTime.Now.Year;

        List<VoucherModel> ListaVouchers = objcliente.ListVoucher.Where(x => x.DadoDeBaja == false).ToList();

        List<CuentaContableModel> lstCuentaContables = objcliente.CtaContable.Where(x => x.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA ||
                                                                                         x.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA).ToList();

        List<DetalleVoucherModel> lstDetalle = new List<DetalleVoucherModel>();
        foreach (CuentaContableModel Cuenta in lstCuentaContables)
        {
                lstDetalle = ListaVouchers.SelectMany(x => x.ListaDetalleVoucher)
                                          .Where(x => x.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID &&
                                                     x.FechaDoc.Month == Mes && x.FechaDoc.Year == Anio)
                                          .OrderBy(x => x.ObjCuentaContable.Clasificacion).ToList();

                decimal Haber = lstDetalle.Sum(x => x.MontoHaber);
                decimal Debe = lstDetalle.Sum(x => x.MontoDebe);

                if (Haber == 0 && Debe == 0)
                    continue;

                
                decimal Saldo = Math.Abs(Haber) - Math.Abs(Debe);

                if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA)
                    TotalIngresos += Saldo;
                else if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA)
                    TotalEgresos += Saldo;
            
        }

        return Tuple.Create(TotalIngresos, TotalEgresos);
    }

    public static Tuple<decimal,decimal> TotalGananciasYPerdidasAnual(FacturaPoliContext db, ClientesContablesModel ObjCliente)
    {   
        decimal TotalGanancias = 0;
        decimal TotalPerdidas = 0;

      
        int Anio = DateTime.Now.Year;
   
        List<VoucherModel> ListaVouchers = ObjCliente.ListVoucher.Where(x => x.DadoDeBaja == false).ToList();

        List<CuentaContableModel> lstCuentaContables = ObjCliente.CtaContable.Where(x => x.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA ||
                                                                                         x.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA).ToList();

        List<DetalleVoucherModel> lstDetalle = new List<DetalleVoucherModel>();
        foreach (CuentaContableModel Cuenta in lstCuentaContables)
        {
     
                lstDetalle = ListaVouchers.SelectMany(x => x.ListaDetalleVoucher)
                                    .Where(x => x.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID &&
                                                x.FechaDoc.Year == Anio)
                                    .OrderBy(x => x.ObjCuentaContable.Clasificacion).ToList();

                decimal Haber = lstDetalle.Sum(x => x.MontoHaber);
                decimal Debe = lstDetalle.Sum(x => x.MontoDebe);

                if (Haber == 0 && Debe == 0)
                    continue;


                decimal Saldo = Math.Abs(Haber) - Math.Abs(Debe);

            if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA)
                TotalGanancias += Saldo;
            else if (Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA)
                TotalPerdidas += Saldo;
        }

        return Tuple.Create(TotalGanancias, TotalPerdidas);
    }

    public static Tuple<List<decimal>, List<decimal>, List<DateTime>> TotalGananciasYPerdidasAnio(FacturaPoliContext db, ClientesContablesModel ObjCliente)
    {
        List<decimal> TotalGanancias = new List<decimal>();
        List<decimal> TotalPerdidas = new List<decimal>();

        List<VoucherModel> ListaVoucher = ObjCliente.ListVoucher.Where(x => x.DadoDeBaja == false).ToList();

        List<CuentaContableModel> ListaCuentasContables = ObjCliente.CtaContable.Where(x => x.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA ||
                                                                                            x.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA).ToList();
        
        int Dia = 1;
        int Anio = DateTime.Now.Year;

        List<DetalleVoucherModel> lstDetalle = new List<DetalleVoucherModel>();



        List<CuentaContableModel> lstCuenta = new List<CuentaContableModel>();


        List<List<decimal>> AllSaldoGanancia = new List<List<decimal>>();
        List<List<decimal>> AllSaldoPerdida = new List<List<decimal>>();
        List<Meses> lstMeses = ParseExtensions.ListaMeses();
        foreach (CuentaContableModel Cuenta in ListaCuentasContables)
        {
            EstadoResultadoComparativoViewModel ObjARellenar = new EstadoResultadoComparativoViewModel();
            List<decimal> lstSaldoGanancia = new List<decimal>();
            List<decimal> lstSaldoPerdida = new List<decimal>();

            decimal SaldoFinalLinea = 0;

            foreach (Meses ItemMes in lstMeses)
            {
                int Mes = Convert.ToInt32(ItemMes);
              

                lstDetalle = ListaVoucher.SelectMany(x => x.ListaDetalleVoucher)
                                         .Where(x => x.ObjCuentaContable.CuentaContableModelID == Cuenta.CuentaContableModelID &&
                                                     x.FechaDoc.Month == Mes & x.FechaDoc.Year == Anio)
                                         .OrderBy(x => x.ObjCuentaContable.Clasificacion).ToList();

                decimal SumasHaber = lstDetalle.Sum(x => x.MontoHaber);
                decimal SumasDebe = lstDetalle.Sum(x => x.MontoDebe);


                decimal Saldo = Math.Abs(SumasHaber) - Math.Abs(SumasDebe);

                if(Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA) { 
                    lstSaldoGanancia.Add(Saldo);
                }
                if(Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA) { 
                    lstSaldoPerdida.Add(Saldo);
                }
            }

            if(Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA)
                AllSaldoGanancia.Add(lstSaldoGanancia);
            if(Cuenta.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA)
                AllSaldoPerdida.Add(lstSaldoPerdida);
        }

        

        List<DateTime> FechasConsultadas = new List<DateTime>();
        foreach (Meses itemMes in lstMeses)
        {
            int Meses = Convert.ToInt32(itemMes);
            DateTime FechaCreada = new DateTime(Anio, Meses, Dia);
            FechasConsultadas.Add(FechaCreada);
        }
        int CantidadMeses = lstMeses.Count();
        for (int i = 0; i < CantidadMeses; i++)
        {
            TotalGanancias.Add(AllSaldoGanancia.Sum(x => x[i]));    
        }

        for (int i = 0; i < CantidadMeses; i++)
        { 
           TotalPerdidas.Add(AllSaldoPerdida.Sum(x => x[i]));
        }

        return Tuple.Create(TotalGanancias, TotalPerdidas, FechasConsultadas);
    }




    internal static dynamic ListAsHTML_Input_Select<T>(List<T> lstAllActectos, string v, List<string> list, IQueryable<object> listadoSelecionado)
    {
        throw new NotImplementedException();
    }

    /*
    public static string GirosAsString(ICollection<GirosModel> collectionGiros)
    {
        StringBuilder sb = new StringBuilder();
        foreach (GirosModel giros in collectionGiros)
        {
            if (collectionGiros.Last() == giros)
            {
                sb.Append(giros.NombreGiro);
            }
            else
            {
                sb.Append(giros.NombreGiro + ", ");
            }
            
        }
        return sb.ToString();
    }

    public static List<GirosModel> StringMultiToGiros(string strGirosValue, int IDEmisor)
    {
        List<GirosModel> rtrnVal = new List<GirosModel>();
        if (string.IsNullOrWhiteSpace(strGirosValue))
            return rtrnVal;

        List<string> lstGirosNombre = strGirosValue.Split(',').ToList();
        foreach (string GirosNombre in lstGirosNombre)
        {
            string TrimmedName = GirosNombre.Trim();
            GirosModel objGiro = new GirosModel(TrimmedName, IDEmisor);
            rtrnVal.Add(objGiro);
        }

        return rtrnVal;
    }*/

    public static void GetValidationErrors(DbEntityValidationException e)
    {
        foreach (var eve in e.EntityValidationErrors)
        {
            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                eve.Entry.Entity.GetType().Name, eve.Entry.State);
            foreach (var ve in eve.ValidationErrors)
            {
                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                    ve.PropertyName, ve.ErrorMessage);
            }
        }
    }

    public static List<string[]> ReadCSV(HttpPostedFileBase file)
    {
        string fileStringRaw = string.Empty;
        List<string[]> csvLines = new List<string[]>();
        using (BinaryReader b = new BinaryReader(file.InputStream))
        {
            byte[] binData = b.ReadBytes(file.ContentLength);
            fileStringRaw = System.Text.Encoding.UTF8.GetString(binData);
        }
        using (StringReader reader = new StringReader(fileStringRaw))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                csvLines.Add(line.Split(';'));
              
            }
        }
        return csvLines;
    }

    public static List<string[]> ReadStructurateCSV(HttpPostedFileBase file)
    {
        string fileStringRaw = string.Empty;
        List<string[]> csvLines = new List<string[]>();
        using (BinaryReader b = new BinaryReader(file.InputStream))
        {
            byte[] binData = b.ReadBytes(file.ContentLength);
            fileStringRaw = System.Text.Encoding.UTF8.GetString(binData);
        }
        using (StringReader reader = new StringReader(fileStringRaw))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                csvLines.Add(line.Split(','));

            }
        }
        return csvLines;
    }



    public static List<string[]> ReadCSV(HttpPostedFileBase file, DateTime fechAdicional)
    {
        string fileStringRaw = string.Empty;
        List<string[]> csvLines = new List<string[]>();
        using (BinaryReader b = new BinaryReader(file.InputStream))
        {
            byte[] binData = b.ReadBytes(file.ContentLength);
            fileStringRaw = System.Text.Encoding.UTF8.GetString(binData);
        }
        using (StringReader reader = new StringReader(fileStringRaw))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                List<string> rawLines = line.Split(';').ToList();
                rawLines.Add(ParseExtensions.ToDD_MM_AAAA(fechAdicional));
                csvLines.Add(rawLines.ToArray());
            }
        }
        return csvLines;
    }

    public static List<string[]> ReadXlsGeneric(HttpPostedFileBase file)
    {
        return null;
    }




    [Authorize]
    [ModuloHandler]
    public static byte[] ProduceFileIvaNoProporcional(ClientesContablesModel objCliente, bool ComoPorcentaje, int? Año = null)
    {
        byte[] ExcelByteArray = null;

        string RutaPlanillaIVANoProporcional = ParseExtensions.Get_AppData_Path("IvaNoRecuper.xlsx");
        IEnumerable<LibrosContablesModel> lstLibros = objCliente.ListLibros.Where(r => r.TipoLibro == TipoCentralizacion.Venta);
        lstLibros = lstLibros.Where(r => r.FechaDoc.Year == Año.Value);
        try
        {
            using (XLWorkbook excelFile = new XLWorkbook(RutaPlanillaIVANoProporcional))
            {
                var workSheet = excelFile.Worksheet(1);

                for (int rowNumber = 0; rowNumber < 12; rowNumber++)
                {
                    //ParseExtensions.NumberWithDots( (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == false).Sum(r => r.MontoExento)) - (bagDetalleLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == true).Sum(r => r.MontoExento)) ),
                    workSheet.Cell(rowNumber + 3, 2).Value = ((lstLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == false && w.FechaDoc.Month == rowNumber + 1).Sum(r => r.MontoNeto)) - (lstLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == true && w.FechaDoc.Month == rowNumber + 1).Sum(r => r.MontoNeto)));
                    workSheet.Cell(rowNumber + 3, 3).Value = ((lstLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == false && w.FechaDoc.Month == rowNumber + 1).Sum(r => r.MontoExento)) - (lstLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == true && w.FechaDoc.Month == rowNumber + 1).Sum(r => r.MontoExento)));
                    workSheet.Cell(rowNumber + 3, 4).Value = ((lstLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == false && w.FechaDoc.Month == rowNumber + 1).Sum(r => r.MontoIva)) - (lstLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == true && w.FechaDoc.Month == rowNumber + 1).Sum(r => r.MontoIva)));
                    workSheet.Cell(rowNumber + 3, 5).Value = ((lstLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == false && w.FechaDoc.Month == rowNumber + 1).Sum(r => r.MontoTotal)) - (lstLibros.Where(w => w.TipoDocumento.EsUnaNotaDeCredito() == true && w.FechaDoc.Month == rowNumber + 1).Sum(r => r.MontoTotal)));
                }

                ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
                return ExcelByteArray;
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static List<decimal> GetIvaNoRecuperable(ClientesContablesModel objCliente, bool ComoPorcentaje, int? Año = null)
    {
        List<decimal> lstRetorno = new List<decimal>();
        if (objCliente == null || Año == null || Año.HasValue == false)
            return lstRetorno;

        ICollection<LibrosContablesModel> LibrosCliente = objCliente.ListLibros;
        if (LibrosCliente == null || LibrosCliente.Count == 0)
            return lstRetorno;

        DateTime TimeCounter = new DateTime(Año.Value, 1, 1);
        DateTime FirstDayOfYear = new DateTime(Año.Value, 1, 1);

        var filteredLibros = LibrosCliente.Where(r => r.FechaDoc.Year == Año.Value && r.TipoLibro == TipoCentralizacion.Venta & r.TipoDocumento.EsDTEExportacion() == false);
        while (TimeCounter.Year == Año)
        {
            if (TimeCounter > DateTime.Now)
            {
                lstRetorno.Add(0m);
                TimeCounter = TimeCounter.AddMonths(1);
                continue;
            }

            DateTime dtLastDayOfMonth = TimeCounter.AddMonths(1).AddDays(-1);

            Decimal sumMensualMontoNeto = filteredLibros.Where(r => r.FechaDoc >= FirstDayOfYear && r.FechaDoc <= dtLastDayOfMonth).Sum(w => w.MontoNeto);
            Decimal sumMensualMontoExento = filteredLibros.Where(r => r.FechaDoc >= FirstDayOfYear && r.FechaDoc <= dtLastDayOfMonth).Sum(w => w.MontoExento);

            if ((sumMensualMontoNeto + sumMensualMontoExento) == 0)
            {
                lstRetorno.Add(0m);
                TimeCounter = TimeCounter.AddMonths(1);
                continue;
            }

            Decimal MontoNoRecuperable = (sumMensualMontoNeto) / (sumMensualMontoNeto + sumMensualMontoExento); //* 100;
            if (ComoPorcentaje == true)
                MontoNoRecuperable = MontoNoRecuperable * 100;
            lstRetorno.Add(MontoNoRecuperable);

            TimeCounter = TimeCounter.AddMonths(1);
        }
        return lstRetorno;
    }

    public static List<LibrosContablesModel> FiltrarLibrosPorPrestador(ClientesContablesModel objCliente, string rutPrestador = "", string razonSocialPrestador = "")
    {
        if (string.IsNullOrWhiteSpace(rutPrestador) && string.IsNullOrWhiteSpace(razonSocialPrestador))
            return objCliente.ListLibros.ToList();

        IEnumerable<LibrosContablesModel> resultLibroContable = objCliente.ListLibros;
        if (string.IsNullOrWhiteSpace(rutPrestador) == false)
            resultLibroContable = resultLibroContable.Where(r => r.Prestador.PrestadorRut.IndexOf(rutPrestador, StringComparison.InvariantCultureIgnoreCase) >= 0);

        if (string.IsNullOrWhiteSpace(razonSocialPrestador) == false)
            resultLibroContable = resultLibroContable.Where(r => r.Prestador.PrestadorNombre.IndexOf(razonSocialPrestador, StringComparison.InvariantCultureIgnoreCase) >= 0);

       
        return resultLibroContable.ToList();
    }
    public static List<LibrosContablesModel> FiltrarLibrosPorPrestador(List<LibrosContablesModel> LstLibros, string rutPrestador = "", string razonSocialPrestador = "")
    {
        if (string.IsNullOrWhiteSpace(rutPrestador) && string.IsNullOrWhiteSpace(razonSocialPrestador))
            return LstLibros;

        if (rutPrestador == "undefined" || razonSocialPrestador == "undefined")
            return LstLibros;

        IEnumerable<LibrosContablesModel> resultLibroContable = LstLibros;


        if (string.IsNullOrWhiteSpace(rutPrestador) == false)
            resultLibroContable = resultLibroContable.Where(r => r.Prestador.PrestadorRut.IndexOf(rutPrestador, StringComparison.InvariantCultureIgnoreCase) >= 0);

        if (string.IsNullOrWhiteSpace(razonSocialPrestador) == false)
            resultLibroContable = resultLibroContable.Where(r => r.Prestador.PrestadorNombre.IndexOf(razonSocialPrestador, StringComparison.InvariantCultureIgnoreCase) >= 0);
        //if (DesdeLetra != 0)
        //    resultLibroContable = resultLibroContable.Where(r => (int)char.Parse(r.Prestador.PrestadorNombre.Substring(0, 1)) >= DesdeLetra);

        //if (HastaLetra != 0)
        //    resultLibroContable = resultLibroContable.Where(r => (int)char.Parse(r.Prestador.PrestadorNombre.Substring(0, 1)) <= HastaLetra);
        return resultLibroContable.ToList();
    }

    public static string ObtenerFechaTextualMembreteReportes(string fechaDesde, string fechaHasta, int? Anio, int? Mes, string TituloDocumento)
    {
        //agregar parametro opcional para cuenta contable como subtitulo
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TituloDocumento + " ");

            if (string.IsNullOrWhiteSpace(fechaDesde) == false && string.IsNullOrWhiteSpace(fechaHasta) == false)
            {
                sb.Append("DESDE " + fechaDesde + " A " + fechaHasta);
                return sb.ToString();
            }
            else
            {
                if (Anio == null && Mes == null)
                {
                    sb.Append(DateTime.Now.Year);
                    return sb.ToString();
                }
                else
                {
                    string nombreMes = null;
                    if (Mes != null)
                    {
                        nombreMes = ParseExtensions.obtenerNombreMes(Mes.Value);
                        sb.Append(nombreMes + " ");
                    }
                    if (Anio != null)
                    {
                        if (nombreMes != null)
                        {
                            sb.Append("de " + Anio.Value);
                        }
                        else
                        {
                            sb.Append(Anio.Value);
                        }
                    }
                    return sb.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            return "Titulo";
        }
    }

    public static string ObtenerTipoDTEDropdownAsString(int? selectedValue = null)
    {
        StringBuilder sb = new StringBuilder();
        var lstTipoDTE = ObtenerValoresPosiblesEnum<TipoDte>();

        IEnumerable<SubTipoDocumento> ienumSubTipos = lstTipoDTE.Select(r => r.GetAttribute<TipoDocumento>().SubTipo).Distinct();

        foreach (SubTipoDocumento subTipoDoc in ienumSubTipos)
        {
            sb.Append("<optgroup label=\"" + (EnumGetDisplayAttrib(subTipoDoc)) + "\">");
            foreach (TipoDte OBJ in lstTipoDTE.Where(r => r.GetAttribute<TipoDocumento>().SubTipo == subTipoDoc))
            {
                if (selectedValue != null && selectedValue.Value == (int)OBJ)
                {
                    sb.Append("<option value=\"" + (int)OBJ + "\" selected>" + EnumGetDisplayAttrib(OBJ) + "</option>");
                }
                else
                {
                    sb.Append("<option value=\"" + (int)OBJ + "\">" + EnumGetDisplayAttrib(OBJ) + "</option>");
                }
            }
            sb.Append("</optgroup>");
        }
        return sb.ToString();
    }

    public static string ObtenerTipoDTEDropdownAsStringSpecific(int valueTipoDoc)
    {
        TipoDte tipoSeleccionado = (TipoDte)valueTipoDoc;
        SubTipoDocumento subTipoDoc = tipoSeleccionado.GetAttribute<TipoDocumento>().SubTipo;

        StringBuilder sb = new StringBuilder();
        sb.Append("<optgroup label=\"" + (EnumGetDisplayAttrib(subTipoDoc)) + "\">");
        sb.Append("<option value=\"" + (int)tipoSeleccionado + "\" selected>" + EnumGetDisplayAttrib(tipoSeleccionado) + "</option>");
        sb.Append("</optgroup>");

        return sb.ToString();
    }

    public static int? ParseIntNullable(string texto)
    {
        int? returnValue = null;
        int outForConversion = -1;

        if (texto == null)
            return returnValue;
        else
        {
            if (Int32.TryParse(texto, out outForConversion))
            {
                returnValue = outForConversion as int?;
            }
        }
        return returnValue;
    }

    [Authorize]
    public static bool EstaNumeroVoucherDisponible(int numVoucherARevisar, ClientesContablesModel objCliente, FacturaPoliContext db, int Mes, int Anio)
    {
        if (objCliente == null)
            return false;
        try
        {
            IQueryable<VoucherModel> lstVoucherConEseFolio = db.DBVoucher.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                                     r.FechaEmision.Month == Mes &&
                                                                                     r.FechaEmision.Year == Anio &&
                                                                                     r.NumeroVoucher == numVoucherARevisar);
            if (lstVoucherConEseFolio.Count() > 0)
                return false;
            else
                return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }


    public static string ObtenerCuentaContableDropdownAsStringWithSelected(ClientesContablesModel ObjCliente, int IDOfSelected)
    {
        List<CuentaContableModel> ListaCuentas = ObjCliente.CtaContable.ToList();
        StringBuilder sb = new StringBuilder();

        //Agregar la opcion vacia para poder validar en contra de ella

        sb.Append("<option></option>");

        //para cuando hay subcategorias pobladas
        foreach (CuentaContableModel Cuentas in ListaCuentas.Where(r => r.SubSubClasificacion == null))
        {
            sb.Append("<option  data-auxiliar=\"0\"  data-item=\"0\"   data-cc=\"0\"   data-analisis=\"0\"  data-conciliancion=\"0\" value=\"" + Cuentas.CuentaContableModelID + "\">" + Cuentas.GetCtaContableDisplayName() + "</option>");
        }
        var lstSubSubCategorias = ListaCuentas.Where(x => x.SubSubClasificacion != null).Select(x => x.SubSubClasificacion).Distinct();
        if (lstSubSubCategorias.Count() > 0)
        {
            foreach (SubSubClasificacionCtaContable _SubSubClasificacion in lstSubSubCategorias)
            {
                sb.Append("<optgroup label=\"" + (_SubSubClasificacion.CodigoInterno + ":" + _SubSubClasificacion.NombreInterno) + "\">");
                foreach (CuentaContableModel Cuentas in ListaCuentas.Where(r => r.SubSubClasificacion == _SubSubClasificacion))
                {
                    if (Cuentas.CuentaContableModelID == IDOfSelected)
                        sb.Append("<option data-auxiliar=\"" + Cuentas.TieneAuxiliar + "\"  data-item=\"" + Cuentas.ItemsModelID + "\"   data-cc=\"" + Cuentas.CentroCostosModelID + "\"   data-analisis=\"" + Cuentas.AnalisisDeCuenta + "\"  data-conciliancion=\"" + Cuentas.Concilaciones + "\" data-tokens=\"" + Cuentas.SubSubClasificacion.NombreInterno + " " + Cuentas.GetCtaContableDisplayName() + "\"  value=\"" + Cuentas.CuentaContableModelID + "\" selected>" + Cuentas.GetCtaContableDisplayName() + "</option>");
                    else
                        sb.Append("<option data-auxiliar=\"" + Cuentas.TieneAuxiliar + "\"  data-item=\"" + Cuentas.ItemsModelID + "\"   data-cc=\"" + Cuentas.CentroCostosModelID + "\"   data-analisis=\"" + Cuentas.AnalisisDeCuenta + "\"  data-conciliancion=\"" + Cuentas.Concilaciones + "\" data-tokens=\"" + Cuentas.SubSubClasificacion.NombreInterno + " " + Cuentas.GetCtaContableDisplayName() + "\"  value=\"" + Cuentas.CuentaContableModelID + "\">" + Cuentas.GetCtaContableDisplayName() + "</option>");
                }
                sb.Append("</optgroup>");
            }
        }
        return sb.ToString();
    }

    public static string ObtenerCentrosDeCostosDropdownAsString(ClientesContablesModel ObjCliente, int SelectedId)
    {
        var LstCentroDeCostos = ObjCliente.ListCentroDeCostos.Select(x => new { x.CentroCostoModelID, x.Nombre }).ToList();
        StringBuilder sb = new StringBuilder();
        sb.Append("<option value="+0+">Selecciona</option>");

        if (LstCentroDeCostos.Any())
        {
            LstCentroDeCostos.Where(x => x.CentroCostoModelID != SelectedId)
                             .ForEach(x => sb.Append("<option value="+x.CentroCostoModelID+">" + x.Nombre + "</option>"));

            if (LstCentroDeCostos.Where(x => x.CentroCostoModelID == SelectedId).Any())
            {
                LstCentroDeCostos.Where(x => x.CentroCostoModelID == SelectedId)
                    .ForEach(x => sb.Append("<option value="+x.CentroCostoModelID+" selected>" + x.Nombre + "</option>"));
            }
        }
     
        return sb.ToString();
    }

    public static decimal CalcularIva(decimal MontoNeto)
    {
        float ReturnValues = (float)MontoNeto * 0.19f;

        return Convert.ToDecimal(Math.Round(ReturnValues, MidpointRounding.AwayFromZero));
    }

    public static decimal CalcularMontoNeto(decimal MontoTotal)
    {
        float ReturnValues = (float)MontoTotal / 1.19f;

        return Convert.ToDecimal(Math.Round(ReturnValues, MidpointRounding.AwayFromZero));
    }
    public static string ObtenerCuentaContableDropdownAsStringWithSelectedCodInterno(ClientesContablesModel ObjCliente, string IDOfSelected)
    {
        List<CuentaContableModel> ListaCuentas = ObjCliente.CtaContable.ToList();
        StringBuilder sb = new StringBuilder();

        //Agregar la opcion vacia para poder validar en contra de ella
        sb.Append("<option></option>");

        //para cuando hay subcategorias pobladas
        foreach (CuentaContableModel Cuentas in ListaCuentas.Where(r => r.SubSubClasificacion == null))
        {
            sb.Append("<option  data-auxiliar=\"0\"  data-item=\"0\"   data-cc=\"0\"   data-analisis=\"0\"  data-conciliancion=\"0\" value=\"" + Cuentas.CuentaContableModelID + "\">" + Cuentas.GetCtaContableDisplayName() + "</option>");
        }
        var lstSubSubCategorias = ListaCuentas.Where(x => x.SubSubClasificacion != null).Select(x => x.SubSubClasificacion).Distinct();
        if (lstSubSubCategorias.Count() > 0)
        {
            foreach (SubSubClasificacionCtaContable _SubSubClasificacion in lstSubSubCategorias)
            {
                sb.Append("<optgroup label=\"" + (_SubSubClasificacion.CodigoInterno + ":" + _SubSubClasificacion.NombreInterno) + "\">");
                foreach (CuentaContableModel Cuentas in ListaCuentas.Where(r => r.SubSubClasificacion == _SubSubClasificacion))
                {
                    if (Cuentas.CodInterno == IDOfSelected)
                        sb.Append("<option data-auxiliar=\"" + Cuentas.TieneAuxiliar + "\"  data-item=\"" + Cuentas.ItemsModelID + "\"   data-cc=\"" + Cuentas.CentroCostosModelID + "\"   data-analisis=\"" + Cuentas.AnalisisDeCuenta + "\"  data-conciliancion=\"" + Cuentas.Concilaciones + "\" data-tokens=\"" + Cuentas.SubSubClasificacion.NombreInterno + " " + Cuentas.GetCtaContableDisplayName() + "\"  value=\"" + Cuentas.CuentaContableModelID + "\" selected>" + Cuentas.GetCtaContableDisplayName() + "</option>");
                    else
                        sb.Append("<option data-auxiliar=\"" + Cuentas.TieneAuxiliar + "\"  data-item=\"" + Cuentas.ItemsModelID + "\"   data-cc=\"" + Cuentas.CentroCostosModelID + "\"   data-analisis=\"" + Cuentas.AnalisisDeCuenta + "\"  data-conciliancion=\"" + Cuentas.Concilaciones + "\" data-tokens=\"" + Cuentas.SubSubClasificacion.NombreInterno + " " + Cuentas.GetCtaContableDisplayName() + "\"  value=\"" + Cuentas.CuentaContableModelID + "\">" + Cuentas.GetCtaContableDisplayName() + "</option>");
                }
                sb.Append("</optgroup>");
            }
        }
        return sb.ToString();
    }




    public static string ObtenerCuentaContableDropdownAsString(ClientesContablesModel ObjCliente, int ID_CtaContable)
    {
        List<CuentaContableModel> ListaCuentas = new List<CuentaContableModel>();
        List<CuentaContableModel> ListaCuentasCliente = ObjCliente.CtaContable.ToList();
        CuentaContableModel singleCuenta = ListaCuentasCliente.SingleOrDefault(r => r.CuentaContableModelID == ID_CtaContable);
        if (singleCuenta == null)
            return string.Empty;

        ListaCuentas.Add(singleCuenta);

        StringBuilder sb = new StringBuilder();

        //para cuando hay subcategorias pobladas
        foreach (CuentaContableModel Cuentas in ListaCuentas.Where(r => r.SubSubClasificacion == null))
        {
            sb.Append("<option  data-auxiliar=\"0\"  data-item=\"0\"   data-cc=\"0\"   data-analisis=\"0\"  data-conciliancion=\"0\"  value=\"" + Cuentas.CuentaContableModelID + "\">" + Cuentas.GetCtaContableDisplayName() + "</option>");
        }
        var lstSubSubCategorias = ListaCuentas.Where(x => x.SubSubClasificacion != null).Select(x => x.SubSubClasificacion).Distinct();
        if (lstSubSubCategorias.Count() > 0)
        {
            foreach (SubSubClasificacionCtaContable _SubSubClasificacion in lstSubSubCategorias)
            {
                sb.Append("<optgroup label=\"" + (_SubSubClasificacion.CodigoInterno + ":" + _SubSubClasificacion.NombreInterno) + "\">");
                foreach (CuentaContableModel Cuentas in ListaCuentas.Where(r => r.SubSubClasificacion == _SubSubClasificacion))
                {
                    sb.Append("<option  data-auxiliar=\"" + Cuentas.TieneAuxiliar + "\"  data-item=\"" + Cuentas.ItemsModelID + "\"   data-cc=\"" + Cuentas.CentroCostosModelID + "\"   data-analisis=\"" + Cuentas.AnalisisContablesModelID + "\"  data-conciliancion=\"" + Cuentas.Concilaciones + "\"  data-tokens=\"" + Cuentas.SubSubClasificacion.NombreInterno + "\" value=\"" + Cuentas.CuentaContableModelID + "\" selected>" + Cuentas.GetCtaContableDisplayName() + "</option>");
                }
                sb.Append("</optgroup>");
            }
        }
        return sb.ToString();
    }


    public static Dictionary<int, string> GetKeyAndValueEnum()
    {
        Dictionary<int, string> KeyAndValueTipoDte = Enum.GetValues(typeof(TipoDte))
                                                .Cast<TipoDte>()
                                                .Select(v => new { value = (int)v, name = v.ToString() })
                                                .ToDictionary(x => x.value, x => x.name);

        return KeyAndValueTipoDte;
    }

    public static bool AplicaNuevaFormaDeDibujarVoucher(FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        try
        {
            bool ActualizacionAplicada = db.DBVoucher.Where(x => x.ClientesContablesModelID == objCliente.ClientesContablesModelID && x.FechaCreacion != null).Any();
            return ActualizacionAplicada;
        }
        catch(Exception ex)
        {
            throw new Exception("Error al ejecutar evaluacion [AplicaNuevaFormaDeDibujarVoucher] ERROR: " + ex.Message);
        }
    }


    //public static ObtenerNovedadNewNumVoucher(FacturaPoliContext db, ClientesContablesModel objCliente)
    //{
    //    try
    //    {
    //        int NovedadId = db.DBNovedadesRegistradasModel.Where(novedadR => novedadR.NombreNovedad == NovedadesEnumKeys.KeyNovedad.NewNumVoucherFormat.ToString())
    //                                                       .FirstOrDefault().NovedadesRegistradasModelID;

    //        DateTime FechaEjecucion = db.DBNovedadesModel.Where(novedad => novedad.Novedad.NovedadesRegistradasModelID == NovedadId && novedad.ClienteContable.ClientesContablesModelID == objCliente.ClientesContablesModelID)
    //                                                        .FirstOrDefault().FechaEjecucionNovedadEstecliente;

    //    }
    //    catch(Exception ex)
    //    {
    //        throw new Exception("No se pudo obtener la novedad ERROR: " + ex.Message);
    //    }
    //}

    public static DateTime ObtenerFechaActualizacionNumVoucher(ClientesContablesModel objCliente)
    {
        try
        {
            using(var db = new FacturaProduccionContext())
            {
                int NovedadId = db.DBNovedadesRegistradasModel.Where(novedadR => novedadR.NombreNovedad == NovedadesEnumKeys.KeyNovedad.NewNumVoucherFormat.ToString())
                                                               .FirstOrDefault().NovedadesRegistradasModelID;

                DateTime FechaEjecucion = db.DBNovedadesModel.Where(novedad => novedad.Novedad.NovedadesRegistradasModelID == NovedadId && novedad.ClienteContable.ClientesContablesModelID == objCliente.ClientesContablesModelID)
                                                            .FirstOrDefault().FechaEjecucionNovedadEstecliente;

                return FechaEjecucion;
            }
        }
        catch(Exception ex)
        {
            throw new Exception("No se pudo obtener la novedad Error:" + ex.Message);
        }
    }
    public static bool CorreDibujoNumVoucherAntiguoONuevo(ClientesContablesModel objCliente, int VoucherId)
    {
        try
        {
            using(var db = new FacturaProduccionContext())
            {
                //Ver la posibilidad de obtener la fecha de la ejecución y comparar la fecha de creacion con  la fecha de ejecucion para así no tener que estar constantemente yendo a la base de datos a comprar siempre.

                //ejemplo FechaEjecucion = ParseExtensions.GetFechaEjecucionNovedadNumVoucher(); //La idea es obtener esto una sola vez
                //foreach () -> fechaCreacion >= FechaEjecucion == true -> usar nuevoTipoDibujo  si es falso usar el dibujo antiguo
                bool CorreActualizacion = db.DBVoucher.Where(voucher => voucher.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                            voucher.VoucherModelID == VoucherId &&
                                                                                voucher.FechaCreacion != null).Any();
                return CorreActualizacion;
            }
        } 
        catch(Exception ex)
        {
            throw new Exception("Error al obtener el ultimo voucherId de este cliente ERROR:" + ex.Message);
        }
    }

    public static DateTime? GetFechaDependiendoDelTipoDeDato(string Fecha, DateTime FechaDt)
    {
        DateTime? FechaOriginStr = null;
        DateTime? FechaOriginDt = null;
        if (!string.IsNullOrWhiteSpace(Fecha))
        {
            FechaOriginStr = CreaFechaLiteral(Fecha);
        }
        if (FechaDt != null)
        {
            FechaOriginStr = FechaDt;
        }

        if (FechaOriginDt != null)
            return FechaOriginDt.Value;
        if (FechaOriginStr != null)
            return FechaOriginStr.Value;

        return null;
    }

    public static string ObtenerCuentaContableDropdownAsString(ClientesContablesModel ObjCliente, string StringABuscar = "")
    {
        List<CuentaContableModel> ListaCuentas = ObjCliente.CtaContable.ToList();
        StringBuilder sb = new StringBuilder();

        if (String.IsNullOrWhiteSpace(StringABuscar) == false)
        {
            ListaCuentas = ListaCuentas.Where(r => r.nombre.Contains(StringABuscar)).ToList();
        }
            
        //Agregar la opcion vacia para poder validar en contra de ella
        sb.Append("<option  data-auxiliar=\"0\" data-tipoauxiliar=\"0\" data-item=\"0\"   data-cc=\"0\"   data-analisis=\"0\"  data-conciliancion=\"0\"  value=\"\">Seleccione</option>");

        //para cuando hay subcategorias pobladas
        foreach (CuentaContableModel Cuentas in ListaCuentas.Where(r => r.SubSubClasificacion == null))
        {
            sb.Append("<option value=\"" + Cuentas.CuentaContableModelID + "\">" + Cuentas.GetCtaContableDisplayName() + "</option>");
        }
        var lstSubSubCategorias = ListaCuentas.Where(x => x.SubSubClasificacion != null).Select(x => x.SubSubClasificacion).Distinct();
        if (lstSubSubCategorias.Count() > 0)
        {
            foreach (SubSubClasificacionCtaContable _SubSubClasificacion in lstSubSubCategorias)
            {
                sb.Append("<optgroup label=\"" + (_SubSubClasificacion.CodigoInterno + ":" + _SubSubClasificacion.NombreInterno) + "\">");
                foreach (CuentaContableModel Cuentas in ListaCuentas.Where(r => r.SubSubClasificacion == _SubSubClasificacion))
                {
                    sb.Append("<option data-auxiliar=\"" + Cuentas.TieneAuxiliar + "\" data-tipoauxiliar=\"" + Cuentas.TipoAuxiliarQueUtiliza + "\" data-item=\"" + Cuentas.ItemsModelID + "\"   data-cc=\"" + Cuentas.CentroCostosModelID + "\"   data-analisis=\"" + Cuentas.AnalisisContablesModelID + "\"  data-conciliancion=\"" + Cuentas.Concilaciones + "\" data-tokens=\"" + Cuentas.SubSubClasificacion.NombreInterno + " " + Cuentas.GetCtaContableDisplayName() + "\"  value=\"" + Cuentas.CuentaContableModelID + "\">" + Cuentas.GetCtaContableDisplayName() + "</option>");
                }
                sb.Append("</optgroup>");
            }
        }
        return sb.ToString();
    }

    public static string ObtenerListaTipoVoucherComoInputSelect(object selectedCast = null)
    {
        StringBuilder sb = new StringBuilder();

        int? selected = null;
        selected = selectedCast as int?;

        var EnumValues = ParseExtensions.ObtenerValoresPosiblesEnum<TipoVoucher>().ToList();
        foreach (var TiposDeVoucher in EnumValues)
        {
            if (selected != null && selected == (int)TiposDeVoucher)
            {
                sb.AppendLine("<option value=\"" + (int)TiposDeVoucher + "\" selected=\"selected\">" + TiposDeVoucher.ToString() + "</option>");
            }
            else
            {
                if (EnumValues.First() == TiposDeVoucher)
                    sb.AppendLine("<option value=\"" + (int)TiposDeVoucher + "\" selected=\"selected\">" + TiposDeVoucher.ToString() + "</option>");
                else
                    sb.AppendLine("<option value=\"" + (int)TiposDeVoucher + "\">" + TiposDeVoucher.ToString() + "</option>");
            }
        }
        return sb.ToString();
    }

    public static string ObtenerEnumListaSeleccionadoTipoOrigen(object selectedCast = null)
    {
        StringBuilder sb = new StringBuilder();

        int? selected = null;
        selected = selectedCast as int?;

        var EnumValues = ParseExtensions.ObtenerValoresPosiblesEnum<TipoOrigen>().ToList();
        foreach (var TiposDeVoucher in EnumValues)
        {
       
            if (selected != null && selected == (int)TiposDeVoucher)
            {
                sb.AppendLine("<option value=\"" + (int)TiposDeVoucher + "\" selected=\"selected\">" + TiposDeVoucher.ToString() + "</option>");
            }
            else
            {
                if (EnumValues.First() == TiposDeVoucher)
                    sb.AppendLine("<option value=\"" + (int)TiposDeVoucher + "\" selected=\"selected\">" + TiposDeVoucher.ToString() + "</option>");
                else
                    sb.AppendLine("<option value=\"" + (int)TiposDeVoucher + "\">" + TiposDeVoucher.ToString() + "</option>");
            }
        }
        return sb.ToString();
    }


    public static string TipoVoucherToShortName(TipoVoucher _tipoVoucher)
    {
        if (_tipoVoucher == TipoVoucher.Traspaso)
            return "T";
        else if (_tipoVoucher == TipoVoucher.Ingreso)
            return "I";
        else if (_tipoVoucher == TipoVoucher.Egreso)
            return "E";
        else
            return "?";
    }

    public static int? GetNumVoucher(ClientesContablesModel objClienteContable, FacturaPoliContext db, int Mes, int Anio)
    {
        int? ReturnValues = 0;
        //Queda pendiente crear función AJAX que detecte cuando se cambia de mes y al cambiar de mes volver a entrar a esta función y dar el proximo voucher disponible
        //Para el mes correspondiente
        try
        {
            if (objClienteContable == null) throw new Exception("La sesión del cliente contable no existe.");
            if (Mes <= 0 || Anio <= 0) throw new Exception("Mes y anio no pueden ser 0 o vacio");

            IQueryable<int> lstNumVouchersEstecliente = db.DBVoucher.Where(x => x.ClientesContablesModelID == objClienteContable.ClientesContablesModelID &&
                                                                                x.FechaEmision.Month == Mes &&
                                                                                x.FechaEmision.Year == Anio)
                                                                        .Select(x => x.NumeroVoucher);

            if (lstNumVouchersEstecliente.Count() == 0) return 1;

            ReturnValues = lstNumVouchersEstecliente.MaxObject(numVoucher => numVoucher);
            ReturnValues++;

            return ReturnValues;

        }
        catch(Exception ex)
        {
            throw new Exception("Error al crear numero voucher: " + ex.Message);
        }
    }


    public static string BuildNewFormatNumVoucher(int NumVoucher, DateTime Fecha)
    {
        try
        {
            //el lugar que invoque esta función tiene que ir controlado por un try and catch para evitar que salga la pantalla
            //de errores y ejecutar el correcto tempdata["error"] para que salga el error al usuario
            if (NumVoucher <= 0) throw new Exception("El numero de voucher no puede venir vacio [BuildNewFormatNumVoucher]");
            if (Fecha == null) throw new Exception("La fecha no puede venir vacía [BuildNewFormatNumVoucher]");

            string NumVoucherBuilder = "";
            if (Fecha.Month > 9)
            {
                //Logica de los mese de 1 digito
                NumVoucherBuilder = Fecha.Month.ToString() + Fecha.Year.ToString() + "-" + NumVoucher.ToString();
            }
            if(Fecha.Month <= 9)
            {
                //Logica de los meses de 2 digitos
                NumVoucherBuilder = "0" + Fecha.Month.ToString() + Fecha.Year.ToString() + "-" + NumVoucher.ToString();
            }

            return NumVoucherBuilder;
        }
        catch(Exception ex)
        {
            throw new Exception("Error al dar formato al voucher [BuildNewFormatNumVoucher] ERROR: " + ex.Message);
        }
    }

    public static List<NovedadesModel> GetNovedadesEsteCliente(ClientesContablesModel objCliente, FacturaPoliContext db)
    {
        List<NovedadesModel> NovedadesEsteCliente = db.DBNovedadesModel.Where(x => x.ClienteContable.ClientesContablesModelID == objCliente.ClientesContablesModelID).ToList();

        return NovedadesEsteCliente;
    }

    //Falta crear una forma de asociar  funcionalidades que dependen de otras funcionalidades por ejemplo getNumvoucherToview depende de GetNumVoucher
    public static string GetNumVoucherToView(int NumVoucher, int Mes, int Anio,int dia, FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        //Actualizar esto. Es demasiado especifico sujeto a errores y poco escalable
        DateTime FechaEjecucionEstaFuncionalidad = GetNovedadesEsteCliente(objCliente, db)
                                                    .Where(x => x.Novedad.NombreFuncionalidadAsociada.Contains("GetNumVoucher"))
                                                        .FirstOrDefault().FechaEjecucionNovedadEstecliente;
        if(Anio == FechaEjecucionEstaFuncionalidad.Year && dia == FechaEjecucionEstaFuncionalidad.Day && Mes == FechaEjecucionEstaFuncionalidad.Month) return NumVoucher > 0 && Mes > 0 && Anio > 0 ? $"{Mes.ToString()}{Anio.ToString()}{"-"}{NumVoucher.ToString()}" : throw new Exception("No se pudo obtener el NumeroVoucher");

        return NumVoucher.ToString();
    }

    public static string GetNumVoucherToView(string NumVoucher, int Mes, int Anio, int dia, FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        //Actualizar esto. Es demasiado especifico sujeto a errores y poco escalable

        List<NovedadesModel> NovedadesEsteCliente = db.DBNovedadesModel.Where(x => x.ClienteContable.ClientesContablesModelID == objCliente.ClientesContablesModelID).ToList();
        DateTime FechaEjecucionEstaFuncionalidad = NovedadesEsteCliente
                                                    .Where(x => x.Novedad.NombreFuncionalidadAsociada.Contains("GetNumVoucher"))
                                                        .FirstOrDefault().FechaEjecucionNovedadEstecliente;
        if (Anio == FechaEjecucionEstaFuncionalidad.Year && dia == FechaEjecucionEstaFuncionalidad.Day && Mes == FechaEjecucionEstaFuncionalidad.Month) return Convert.ToInt32(NumVoucher) > 0 && Mes > 0 && Anio > 0 ? $"{Mes.ToString()}{Anio.ToString()}{"-"}{NumVoucher.ToString()}" : throw new Exception("No se pudo obtener el NumeroVoucher");

        return NumVoucher;
    }

    public static int CleanSearchNumVoucher(string numVoucherToClean, int Mes)
    {
        //Aplicar logica para limpiar el mes y el año para poder buscar en la base de datos

        try
        {
            int ReturnValues = 0;

            if (Mes <= 0 || string.IsNullOrWhiteSpace(numVoucherToClean)) throw new Exception("Error alguno de los parametros viene vacio.");
            if (Mes > 9) ReturnValues = Convert.ToInt32(numVoucherToClean.Substring(0, 7));
            if (Mes <= 9) ReturnValues = Convert.ToInt32(numVoucherToClean.Substring(0, 6));

            return ReturnValues;
        }
        catch(Exception ex)
        {
            throw new Exception("Error al limpiar el numero voucher. ERROR : " + ex.Message);
        }
       
    }

    public static int DetectedBrokenNumVoucherCorrelative()
    {
        return 0;
    }

    //queda un caso pendiente que es cuando se borra un voucher. Es necesario detectar que está roto el correlativo y el proximo voucher a crear tendrá que crearse en el espacio que estaba roto.
    //Ejemplo tenemos los numeros voucher 1,2,3,4 se borra el 3 y queda: 1,2,4 -> el proximo numero voucher a crear tiene que ser el numero 3.

    [Authorize]
    public static int? ObtenerNumeroProximoVoucherINT(ClientesContablesModel objCliente, FacturaPoliContext db)
    {
        if (objCliente == null)
            return null;
        int? NumeroVoucherRetorno = null;

        try
        {
            var lstVoucherDeEsteCliente = db.DBVoucher.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID);
            if (lstVoucherDeEsteCliente.Count() == 0)
            {
                return 1;
            }
            NumeroVoucherRetorno = lstVoucherDeEsteCliente.MaxObject(item => item.NumeroVoucher).NumeroVoucher;
            NumeroVoucherRetorno++;
        }
        catch (Exception )
        {
            return null;
        }
        return NumeroVoucherRetorno;
    }

    public static string[] RemoveDuplicates(string[] strArray)
    {
        HashSet<string> result = new HashSet<string>();
        foreach (string str in strArray)
        {
            if (!result.Contains(str))
            {
                result.Add(str);
            }
        }

        return result.ToArray();
    }

    public static int RutANumero(string rut)
    {
        if (!String.IsNullOrWhiteSpace(rut))
        {
            if (rut.Contains('.'))
            {
                rut = rut.Replace(".", "");
            }

            string stopAt = "-";
            int charLocation = rut.IndexOf(stopAt, StringComparison.Ordinal);

            if (charLocation > 0)
            {
                string parteTextualNumeroRut = rut.Substring(0, charLocation);
                return ParseExtensions.ParseInt(parteTextualNumeroRut);
            }
            else
                return 0;
        }
        else
            return 0;
    }

    public static string DecimalToStringForRazor(decimal number)
    {
        string returnValue = number.ToString();
        returnValue = returnValue.Replace(',', '.');
        return returnValue;
    }

    public static Byte[] GetExcelStream(XLWorkbook excelWorkBook)
    {
        using (MemoryStream fs = new MemoryStream())
        {
            excelWorkBook.SaveAs(fs);
            fs.Position = 0;
            return fs.ToArray();
        }
    }

    public static bool EsCertificadoDigitalValido(QuickEmisorModel ObjEmisor, out string ErrorMessage)
    {
        ErrorMessage = string.Empty;
        if (ObjEmisor == null || ObjEmisor.Certificados == null)
        {
            ErrorMessage = "Emisor o certificado nulos";
            return false;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(ObjEmisor.Certificados.CertificatePath) && !string.IsNullOrWhiteSpace(ObjEmisor.Certificados.CertificatePassword))
            {
                X509Certificate2 CertDigitalUsuario = null;
                try
                {
                    CertDigitalUsuario = new X509Certificate2(ObjEmisor.Certificados.CertificatePath, ObjEmisor.Certificados.CertificatePassword);
                }
                catch (CryptographicException ex)
                {
                    ErrorMessage = "Ocurrio un problema con el certificado digital, compruebe que la contraseña de su certificado sea correcta";
                    return false;
                }

                DateTime dtExpirationDate;
                try
                {

                    dtExpirationDate = DateTime.ParseExact(CertDigitalUsuario.GetExpirationDateString(), "G", null);
                    //dtExpirationDate = DateTime.ParseExact(CertDigitalUsuario.GetExpirationDateString(), "dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture);  //DateTime.Parse(CertDigitalUsuario.GetExpirationDateString(), CultureInfo.InvariantCulture);//DateTime.ParseExact(CertDigitalUsuario.GetExpirationDateString(), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                }
                catch (System.FormatException ex)
                {
                    ErrorMessage = "No se pudo obtener fecha de expiracion del certificado digital";
                    return false;
                }

                if (DateTime.Now >= dtExpirationDate)
                {
                    ErrorMessage = "Su certificado digital ha expirado";
                    return false;
                }
                return true;
            }
            else
            {
                ErrorMessage = "Aun no ha cargado un certificado digital";
                return false;
            }
        }
    }

    public static string NumberWithDots_para_BalanceGeneral(decimal sum)
    {
        var nfiLocal = nfi; //NumberFormatInfo)CultureInfo.CreateSpecificCulture("es").NumberFormat;
        nfiLocal.NumberGroupSeparator = ".";
        nfiLocal.NumberDecimalSeparator = ",";

        if (sum != 0)
            return sum.ToString("#,0", nfiLocal);
        else
            return "-";
    }

    public static string ParaPresupuestos(decimal sum)
    {
        var nfiLocal = nfi; //NumberFormatInfo)CultureInfo.CreateSpecificCulture("es").NumberFormat;
        nfiLocal.NumberGroupSeparator = ".";
        nfiLocal.NumberDecimalSeparator = ",";

        if (sum != 0)
            return sum.ToString("#,0", nfiLocal);
        else
            return " ";
    }

    public static bool AlertarCertificadoDigitalNearExpiration(string IDUsuario, out string ErrorMessage, HttpSessionStateBase Session)
    {
        if (HttpContext.Current.Session["AlertaCertificadoShowOnce"] == null)
        {
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(IDUsuario);

            //PENDINGFIX
            //Revisar que obtenga el certificado digital asociado a empresa
            //O es certificado por usuario?

            QuickEmisorModel ObjEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, IDUsuario);//db.Emisores.Include("Certificados").SingleOrDefault(r => r.IdentityIDEmisor == IDUsuario);
            if (ObjEmisor == null)
            {
                ErrorMessage = "No se ha seleccionado emisor";
                return false;
            }

            if (ObjEmisor.Certificados != null)
            {
                if (!string.IsNullOrWhiteSpace(ObjEmisor.Certificados.CertificatePath) && !string.IsNullOrWhiteSpace(ObjEmisor.Certificados.CertificatePassword))
                {
                    X509Certificate2 CertDigitalUsuario = null;
                    try
                    {
                        CertDigitalUsuario = new X509Certificate2(ObjEmisor.Certificados.CertificatePath, ObjEmisor.Certificados.CertificatePassword);
                    }
                    catch (CryptographicException ex)
                    {
                        ErrorMessage = "Ocurrio un problema con el certificado digital, compruebe que la contraseña de su certificado sea correcta";
                        return true;
                    }

                    DateTime dtExpirationDate;
                    try
                    {

                        dtExpirationDate = DateTime.ParseExact(CertDigitalUsuario.GetExpirationDateString(), "G", null);
                        //dtExpirationDate = DateTime.ParseExact(CertDigitalUsuario.GetExpirationDateString(), "dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture); //DateTime.Parse(CertDigitalUsuario.GetExpirationDateString(), CultureInfo.InvariantCulture);//DateTime.ParseExact(CertDigitalUsuario.GetExpirationDateString(), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch (System.FormatException ex)
                    {
                        ErrorMessage = "No se pudo obtener fecha de expiracion del certificado digital";
                        return true;
                    }

                    if (DateTime.Now >= dtExpirationDate)
                    {
                        ErrorMessage = "Su certificado digital ha expirado";
                        return true;
                    }
                    if (DateTime.Now >= dtExpirationDate.AddDays(-7))
                    {
                        ErrorMessage = "Su certificado digital expirará el " + dtExpirationDate.ToString("dd/MM/yyyy");
                        return true;
                    }
                    else
                    {
                        HttpContext.Current.Session["AlertaCertificadoShowOnce"] = true;
                        ErrorMessage = "OK";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Aun no ha cargado un certificado digital";
                    return true;
                }
            }
            else
            {
                ErrorMessage = "Objeto certificado es nulo";
                return false;
            }
        }
        else
        {
            ErrorMessage = "already showed once";
            return false;
        }
    }

    //los libros ya no deben enviarse
    /*
    public static bool MostrarAlertaLibro(string IDUsuario)
    {
        if (HttpContext.Current.Session["AlertaLibro"] == null)
        {
            if (string.IsNullOrWhiteSpace(IDUsuario))
            {
                return false;
            }
            DateTime dia = DateTime.Now;
            DateTime PeriodoAnterior = dia.AddMonths(-1);
            if (dia.Day < 20)
            {
                HttpContext.Current.Session["AlertaLibro"] = false;
                return false;
            }
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(IDUsuario);
            QuickEmisorModel ObjEmisor = db.Emisores.Single(r => r.IdentityID == IDUsuario);
            string PeriodoFacturacionAnterior = PeriodoAnterior.ToString("yyyy-MM");
            var QueryLibros = db.LibroCompra.Where(r => r.QuickEmisorModelID == ObjEmisor.QuickEmisorModelID && r.PeriodoTributario == PeriodoFacturacionAnterior);
            int CantidadLibroVenta = QueryLibros.Where(r => r.TipoOperacion == LibroTipoOperacion.VENTA).ToList().Count();//db.LibroCompra.Where(r => r.QuickEmisorModelID == ObjEmisor.QuickEmisorModelID && r.PeriodoTributario == PeriodoAnterior.Year.ToString()+ "-" + PeriodoAnterior.Month.ToString() && r.TipoOperacion == LibroTipoOperacion.VENTA).ToList().Count();
            int CantidadLibroCompra = QueryLibros.Where(r => r.TipoOperacion == LibroTipoOperacion.COMPRA).ToList().Count();//db.LibroCompra.Where(r => r.QuickEmisorModelID == ObjEmisor.QuickEmisorModelID && r.PeriodoTributario == PeriodoAnterior.Year.ToString() + "-" + PeriodoAnterior.Month.ToString() && r.TipoOperacion == LibroTipoOperacion.COMPRA).ToList().Count();
            if (CantidadLibroVenta == 0 || CantidadLibroCompra == 0)
            {
                HttpContext.Current.Session["AlertaLibro"] = true;
                return true;
            }
            else
            { 
                HttpContext.Current.Session["AlertaLibro"] = false;
                return false;
            }
        }
        else
        {
            return (bool)HttpContext.Current.Session["AlertaLibro"];
        }
        
    }*/

    public static bool IsImageExtension(string ext)
    {
        return _validExtensions.Contains(ext);
    }

    /*
     QReferenciasModel docOriginal = lstObjFacturas[i].Referencias.First(r => r.TipoDocReferencia == TipoDte.FacturaElectronica || r.TipoDocReferencia == TipoDte.FacturaElectronicaExenta || r.TipoDocReferencia == TipoDte.GuiaDespachoElectronica || r.TipoDocReferencia == TipoDte.NotaDebitoElectronica || r.TipoDocReferencia == TipoDte.NotaCreditoElectronica);
     DTE_CodRef CODI_REF = docOriginal.CodigoReferencia;
     //HACER RECURSIVO EN EL FUTURO
     if (docOriginal.TipoDocReferencia == TipoDte.NotaCreditoElectronica || docOriginal.TipoDocReferencia == TipoDte.NotaDebitoElectronica)
     {
         FacturaPoliContext db = ParseExtensions.GetDatabaseContext(SessionUseProdDatabase);
         int intResult = 0;
         int EmisorModelIDofThisOne = lstObjFacturas[i].QuickEmisorModelID;
         Int32.TryParse(docOriginal.FolioRef, out intResult);
         List<FacturaQuickModel> objFactTemp = db.Facturas.Where(r => r.QuickEmisorModelID == EmisorModelIDofThisOne && r.NumFolio == intResult && r.TipoFactura == docOriginal.TipoDocReferencia).ToList();
         if (objFactTemp != null && objFactTemp.Count > 0)
         {
             QReferenciasModel superInnerRef = objFactTemp[0].Referencias.First(r => r.TipoDocReferencia == TipoDte.FacturaElectronica || r.TipoDocReferencia == TipoDte.FacturaElectronicaExenta || r.TipoDocReferencia == TipoDte.GuiaDespachoElectronica || r.TipoDocReferencia == TipoDte.NotaDebitoElectronica || r.TipoDocReferencia == TipoDte.NotaCreditoElectronica);
             if (superInnerRef.CodigoReferencia == DTE_CodRef.CorrigeTextoDocReferencia)
                 CODI_REF = superInnerRef.CodigoReferencia;

         }
     }
     */
    //public static QReferenciasModel GetFirstOriginalDocNonExport(List<FacturaQuickModel> lstObjFacturas)
    //{
    //    QReferenciasModel docOriginal = lstObjFacturas[i].Referencias.First(r => r.TipoDocReferencia == TipoDte.FacturaElectronica || r.TipoDocReferencia == TipoDte.FacturaElectronicaExenta || r.TipoDocReferencia == TipoDte.GuiaDespachoElectronica || r.TipoDocReferencia == TipoDte.NotaDebitoElectronica || r.TipoDocReferencia == TipoDte.NotaCreditoElectronica);
    //    DTE_CodRef CODI_REF = docOriginal.CodigoReferencia;
    //}

    public static bool RangoCuatrimestral(int Anio, int Rango, out DateTime firstDayRange, out DateTime lastDayRange)
    {
        firstDayRange = new DateTime();
        lastDayRange = new DateTime();
        if (Rango == 1)
        {
            firstDayRange = new DateTime(Anio, 1, 1);
            lastDayRange = new DateTime(Anio, 4, 30);
            return true;
        }
        else if (Rango == 2)
        {
            firstDayRange = new DateTime(Anio, 5, 1);
            lastDayRange = new DateTime(Anio, 8, 31);
            return true;
        }
        else if (Rango == 3)
        {
            firstDayRange = new DateTime(Anio, 9, 1);
            lastDayRange = new DateTime(Anio, 12, 31);
            return true;
        }
        firstDayRange = new DateTime(Anio, 1, 1);
        lastDayRange = new DateTime(Anio, 4, 30);
        return false;
    }

    public static string GetRazonS(string UserID, HttpSessionStateBase Session)
    {
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

        //QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);
        QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
        if (objEmisor == null || string.IsNullOrWhiteSpace(objEmisor.IdentityIDEmisor))
            return string.Empty;
        else
            return objEmisor.RazonSocial;
        /*
        string RazonS = objEmisor.RazonSocial;
        return RazonS;*/
    }
    public static string GetRazonSocialClienteCont(string UserID, HttpSessionStateBase Session)
    {
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

        //QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);
        ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
        if (objCliente == null)
            return string.Empty;
        else
            return objCliente.RazonSocial;
        /*
        string RazonS = objEmisor.RazonSocial;
        return RazonS;*/
    }
    public static string GetLogoHtml(string UserID, HttpSessionStateBase Session)
    {
        string ReturnValue = string.Empty;
        QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

        if (objEmisor == null || string.IsNullOrWhiteSpace(objEmisor.IdentityIDEmisor))
            return string.Empty;

        string EmisorIDForPath = objEmisor.IdentityIDEmisor;

        string nonFilePath = @"C:\FE\wkhtmltopdf\" + EmisorIDForPath;

        if (System.IO.File.Exists(nonFilePath + "/logo.jpg"))
        {
            ReturnValue = @"data:image/jpeg;base64," + ParseExtensions.EncodeImageAsBase64(nonFilePath + "/logo.jpg");
            //node.InnerHtml = "<img src = '" + nonFilePath + "/logo.jpg" + "' alt = 'logo empresa' width = '150' height = '150' >";
        }
        else if (System.IO.File.Exists(nonFilePath + "/logo.png"))
        {
            ReturnValue = @"data:image/png;base64," + ParseExtensions.EncodeImageAsBase64(nonFilePath + "/logo.png");
            //node.InnerHtml = "<img src = '" + nonFilePath + "/logo.png" + "' alt = 'logo empresa' width = '150' height = '150' >";
        }
        else if (System.IO.File.Exists(nonFilePath + "/logo.gif"))
        {
            ReturnValue = @"data:image/gif;base64," + ParseExtensions.EncodeImageAsBase64(nonFilePath + "/logo.gif");
            //node.InnerHtml = "<img src = '" + nonFilePath + "/logo.gif" + "' alt = 'logo empresa' width = '150' height = '150' >";
        }
        else
        {
            ReturnValue = @"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAK8AAACDCAMAAADMIrsKAAAANlBMVEX///+/v7++vr7Y2Njr6+u7u7vJycn4+Pjw8PDd3d38/Pzg4ODOzs7j4+PDw8PV1dXz8/O1tbXS6wdMAAAC4UlEQVR4nO2bgXKrIBBFwyoqIGL//2ffrqtGTabNa4XG6T2d6cSEyJFsYIF4uwEAAAAAAAAAAAD8Gsk+oz+1jv5ZFcP3zlWZ4RFzsu+TOkzzd3zNETrZl44V/MiX6lPtXsDTtXxb+GYFvnmBb17gmxf45gW+eflLvmRseV9rvusb+r7vzrX5Gs+VptKVAgAAAFfF89+BLoY47p5p6xDqXTnPhcL48NbsdB/WVjuROOjibVxlunl1N433QuZYqBCdNeQ2xz5ZIrJkiJZ0JRo5Iv7XzGlTm2guZKvCwkffxBKDixU7Wn2+Hvhhk6qGX+nbe6EqJrkG9+Sk5XyjSEmTtWxppDlbWdp1foqBuWi0c6GxIRrKptZ735YFGm3Dms0lImouMAc4h8q0JTGwpYZBPRibikbE3pfrX464XakZdeVcr+A2DlMKLlcyX4GXBt53JUV9HcvF+TFPo/iz9vI1W14eW2nLtJlfpdJzrZ2v52/Q0gfcgjU23EZr6DCv4RZf2zSwb7wVZOc7xcDiq7HRTdbymuKnGF99N7HxDr7JL76+6SfqR9+i08pX29fLMMJjRHin9t3HLy3xK1t1nmQHcHJ/n/jd9Q9O+wc79w8cu6O29bZPKL4W81L/O65lxfeN+t/t+Gb0mxTv41tPGssyVK/jG/3G+OaVKR41Neg4JdP8gfOcKW30jmZft+QP9fAb+QNnhUojy15W8rOQ7Jp6Rdkdb6pKPvq5L+41P+slPwtFdSff7Za9pFxG898lRp3V/JeG2KjvKMKa/7rS+e/H/fcg2g84nTo06/xnmXFwT1d9aJu3juZCZW3F5uGR78Jh/ubXGd1a2tfHQgAAAAD4hKvtb15t//hy+/Pw/Rr45gW+eYFvXuCbF/jmBb55uZrvz+7HMa4+cu6MozuePv7Id9AF3A2n3693rOAP3f/2cG9aifvfzGaf/P+I1TPO3XcIz6oo/NMTAAAAAAAAAADgcvwDJvgmZrwLfn8AAAAASUVORK5CYII=";
        }

        return ReturnValue;
    }


    public static FacturaPoliContext GetDatabaseContext(string userID)
    {/*
        FacturaProduccionContext db = new FacturaProduccionContext();
        QuickEmisorModel ObjEmisor = db.Emisores.Single(r => r.IdentityID == userID);
        if (ObjEmisor.DatabaseContextToUse == 0)
            return new FacturaPoliContext();
        else
            return new FacturaPoliContext(true);*/
        FacturaProduccionContext db = new FacturaProduccionContext();
        UsuarioModel ObjEmisor = db.DBUsuarios.Single(r => r.IdentityID == userID);
        if (ObjEmisor.DatabaseContextToUse == 0)
            return new FacturaPoliContext();
        else
            return new FacturaPoliContext(true);
    }

    public static FacturaPoliContext GetDatabaseContext(UsuarioModel objUsuario)
    {
        /*
        //FacturaProduccionContext db = new FacturaProduccionContext();
        //QuickEmisorModel ObjEmisor = db.Emisores.Single(r => r.IdentityID == userID);
        if (objEmisor.DatabaseContextToUse == 0)
            return new FacturaPoliContext();
        else
            return new FacturaPoliContext(true);*/
        if (objUsuario.DatabaseContextToUse == 0)
            return new FacturaPoliContext();
        else
            return new FacturaPoliContext(true);
    }

    public static bool ParseSessionUseProdDatabase(UsuarioModel objUsuario)
    {
        /*
        if (objEmisor.DatabaseContextToUse == 0)
            return false;
        else
            return true;*/
        if (objUsuario.DatabaseContextToUse == 0)
            return false;
        else
            return true;
    }

    public static bool ItsUserOnCertificationEnvironment(string userID)
    {
        FacturaProduccionContext db = new FacturaProduccionContext();
        UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == userID);
        if (ObjUsuario.DatabaseContextToUse == 0)
            return true;
        else
            return false;
    }



    /*
    public static FacturaPoliContext GetDatabaseContext(object getProduction)
    {
        if (getProduction == null)
            return new FacturaPoliContext();

        FacturaPoliContext db;
        if ((bool)getProduction == true)
            return new FacturaPoliContext(true);
        else
            return new FacturaPoliContext();
    }

    public static bool ParseSessionUseProdDatabase(object getProduction)
    {
        if (getProduction == null)
            return false;

        if ((bool)getProduction == true)
            return true;
        else
            return false;
    }*/

    //Usage: 
    //var values = ParseExtensions.ObtenerValoresPosiblesEnum<Foos>();
    public static IEnumerable<T> ObtenerValoresPosiblesEnum<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static string RutWithDots(string rut)
    {
        if (string.IsNullOrEmpty(rut) || string.IsNullOrWhiteSpace(rut))
            return "0";
        else
        {
            if (rut.Length < 3)
                return "0";
            else
            {
                int number = 0;
                string ToMorph = rut.Substring(0, rut.Length - 2);
                Int32.TryParse(ToMorph, out number);
                string leftPart = number.ToString("N0", new NumberFormatInfo() { NumberGroupSeparator = "." });
                string rightPart = rut.Substring(rut.Length - 2, 2);
                return leftPart + rightPart;
            }
        }
    }

    public static string NumberWithDots(string sum)
    {
        int number = 0;
        Int32.TryParse(sum, out number);
        return number.ToString("N0", new NumberFormatInfo() { NumberGroupSeparator = ".", NumberDecimalSeparator = "," });
    }

    public static string ToAAAA_MM_DD(DateTime dtOBJ)
    {
        if (dtOBJ != null)
            return dtOBJ.ToString("yyyy-MM-dd");
        else
            return DateTime.Now.ToString("yyyy-MM-dd");
    }

    //otro formato
    public static string ToMM_DD_AAAA(DateTime dtOBJ)
    {
        if (dtOBJ != null)
            return dtOBJ.ToString("MM-dd-yyyy");
        else
            return DateTime.Now.ToString("MM-dd-yyyy");
    }

    public static string ToDD_MM_AAAA(DateTime dtObj)
    {
        if (dtObj != null)
            return dtObj.ToString("dd-MM-yyyy");
        else
            return DateTime.Now.ToString("dd-MM-yyyy");
    }

    public static string ToDD_MM_AAAA(DateTime? dtObj)
    {
        if (dtObj != null && dtObj.HasValue)
            return dtObj.Value.ToString("dd-MM-yyyy");
        else
            return string.Empty;
    }


    public static DateTime ToDD_MM_AAAA(string dtOBJ)
    {
        if (string.IsNullOrWhiteSpace(dtOBJ))
            return DateTime.Now;
        else
            return DateTime.ParseExact(dtOBJ, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static DateTime ToDD_MM_AAAA2(string dtOBJ)
    {
        if (string.IsNullOrWhiteSpace(dtOBJ))
            return DateTime.ParseExact(dtOBJ, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
        else
            return DateTime.Now;
    }




    public static string ToDD_MM_AAAA_WithSlashes(DateTime dtObj)
    {
        if (dtObj != null)
            return dtObj.ToString("dd/MM/yyyy");
        else
            return DateTime.Now.ToString("dd/MM/yyyy");
    }

    public static string GetFacturaTimestamp()
    {
        DateTime objDate = DateTime.Now;
        return objDate.ToString("yyyy-MM-dd") + "T" + objDate.ToString("HH:mm:ss");
    }

    //FUNCIONES COMUNES
    static bool verbose = false;

    public static RSACryptoServiceProvider crearRsaDesdePEM(string base64)
    {

        ////
        //// Extraiga de la cadena los header y footer
        base64 = base64.Replace("-----BEGIN RSA PRIVATE KEY-----", string.Empty);
        base64 = base64.Replace("-----END RSA PRIVATE KEY-----", string.Empty);

        ////
        //// el resultado que se encuentra en base 64 cambielo a
        //// resultado string
        byte[] arrPK = Convert.FromBase64String(base64);

        ////
        //// obtenga el Rsa object a partir de
        return DecodeRSAPrivateKey(arrPK);

    }

    public static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
    {
        byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

        // --------- Set up stream to decode the asn.1 encoded RSA private key ------
        MemoryStream mem = new MemoryStream(privkey);
        BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
        byte bt = 0;
        ushort twobytes = 0;
        int elems = 0;
        try
        {
            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                binr.ReadByte();	//advance 1 byte
            else if (twobytes == 0x8230)
                binr.ReadInt16();	//advance 2 bytes
            else
                return null;

            twobytes = binr.ReadUInt16();
            if (twobytes != 0x0102) //version number
                return null;
            bt = binr.ReadByte();
            if (bt != 0x00)
                return null;


            //------ all private key components are Integer sequences ----
            elems = GetIntegerSize(binr);
            MODULUS = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            E = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            D = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            P = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            Q = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            DP = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            DQ = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            IQ = binr.ReadBytes(elems);

            //Console.WriteLine("showing components ..");
            if (verbose)
            {
                showBytes("\nModulus", MODULUS);
                showBytes("\nExponent", E);
                showBytes("\nD", D);
                showBytes("\nP", P);
                showBytes("\nQ", Q);
                showBytes("\nDP", DP);
                showBytes("\nDQ", DQ);
                showBytes("\nIQ", IQ);
            }

            // ------- create RSACryptoServiceProvider instance and initialize with public key -----
            CspParameters CspParameters = new CspParameters();
            CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, CspParameters);
            RSAParameters RSAparams = new RSAParameters();
            RSAparams.Modulus = MODULUS;
            RSAparams.Exponent = E;
            RSAparams.D = D;
            RSAparams.P = P;
            RSAparams.Q = Q;
            RSAparams.DP = DP;
            RSAparams.DQ = DQ;
            RSAparams.InverseQ = IQ;
            RSA.ImportParameters(RSAparams);
            return RSA;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
            binr.Close();
        }
    }

    private static int GetIntegerSize(BinaryReader binr)
    {
        byte bt = 0;
        byte lowbyte = 0x00;
        byte highbyte = 0x00;
        int count = 0;
        bt = binr.ReadByte();
        if (bt != 0x02)   	 //expect integer
            return 0;
        bt = binr.ReadByte();

        if (bt == 0x81)
            count = binr.ReadByte();    // data size in next byte
        else
            if (bt == 0x82)
        {
            highbyte = binr.ReadByte();    // data size in next 2 bytes
            lowbyte = binr.ReadByte();
            byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
            count = BitConverter.ToInt32(modint, 0);
        }
        else
        {
            count = bt;      // we already have the data size
        }

        while (binr.ReadByte() == 0x00)
        {    //remove high order zeros in data
            count -= 1;
        }
        binr.BaseStream.Seek(-1, SeekOrigin.Current);   	 //last ReadByte wasn't a removed zero, so back up a byte
        return count;
    }

    private static void showBytes(String info, byte[] data)
    {
        Console.WriteLine("{0} [{1} bytes]", info, data.Length);
        for (int i = 1; i <= data.Length; i++)
        {
            Console.Write("{0:X2} ", data[i - 1]);
            if (i % 16 == 0)
                Console.WriteLine();
        }
        Console.WriteLine("\n\n");
    }

    public static XmlNode RemoveAllNamespaces(XmlNode documentElement)
    {
        var xmlnsPattern = "\\s+xmlns\\s*(:\\w)?\\s*=\\s*\\\"(?<url>[^\\\"]*)\\\"";
        var outerXml = documentElement.OuterXml;
        var matchCol = Regex.Matches(outerXml, xmlnsPattern);
        foreach (var match in matchCol)
            outerXml = outerXml.Replace(match.ToString(), "");

        var result = new XmlDocument();
        result.LoadXml(outerXml);

        return result;
    }

    public static XmlNode Strip(XmlNode documentElement)
    {
        var namespaceManager = new XmlNamespaceManager(documentElement.OwnerDocument.NameTable);
        foreach (var nspace in namespaceManager.GetNamespacesInScope(XmlNamespaceScope.All))
        {
            namespaceManager.RemoveNamespace(nspace.Key, nspace.Value);
        }

        return documentElement;
    }

    public static void GenerateBarCodeZXing(string data, string UniqueFileName)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.PDF_417,
            Options = new ZXing.PDF417.PDF417EncodingOptions
            {
                Width = 324,
                Height = 164,
                CharacterSet = "ISO-8859-1",
                DisableECI = true,
                ErrorCorrection = ZXing.PDF417.Internal.PDF417ErrorCorrectionLevel.L5
            } //optional
        };

        /*
         var writer = new BarcodeWriter  
            {
                Format = BarcodeFormat.QR_CODE,
 
                Renderer = new ZXing.Rendering.BitmapRenderer { Background = System.Drawing.ColorTranslator.FromHtml(strBG), Foreground = System.Drawing.ColorTranslator.FromHtml(strFG) },
 
                Options = new QrCodeEncodingOptions
                {
                    Height = Int32.Parse(strH),
                    Width = Int32.Parse(strW),
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    ErrorCorrection = ErrorCorrectionLevel.H,
                    Margin = 1,
                }
            };
         */

        var imgBitmap = writer.Write(data);
        using (var stream = new MemoryStream())
        {
            imgBitmap.Save(UniqueFileName + ".png", ImageFormat.Png);
            //return stream.ToArray();
        }
    }

   
  
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    /// <summary>
    /// Remueve ceros decimales a la derecha AKA trailing zeroes
    /// </summary>
    /// <param name="value">Valor a normalizar</param>
    /// <returns>Ejemplo: 5533.00 = 5533 </returns>
    public static decimal Normalizar(this decimal value)
    {
        return value / 1.000000000000000000000000000000000m;
    }

  

    public static string TipoDTEToFriendlyName(TipoDte tipo)
    {
        var seasonDisplayName = tipo.GetAttribute<DisplayAttribute>();
        return seasonDisplayName.Name;
    }

    /// <summary>
    /// Transforma TipoDTE a su representacion en string, este metodo tiene la diferencia
    /// la cual acorta nombres largos como el de facturas electronicas exentas
    /// </summary>
    /// <param name="tipo">Tipo de DTE a transformar</param>
    /// <returns>El tipo de DTE de forma textual</returns>
    public static string TipoDTEToFriendlyName_Short(TipoDte tipo)
    {
        if (tipo == TipoDte.FacturaElectronicaExenta)
        {
            return "Factura Exenta Electrónica";
        }
        else
            return TipoDTEToFriendlyName(tipo);
    }

    public static string EnumGetDisplayAttrib(Enum tipo)
    {
        var seasonDisplayName = tipo.GetAttribute<DisplayAttribute>();
        if (seasonDisplayName == null)
            return string.Empty;
        return seasonDisplayName.Name;
    }

    public static string ToXML(object serializableObject)
    {
        try
        {
            var stringwriter = new ISO8859StringWriter();
            //stringwriter.Encoding = Encoding.GetEncoding("ISO-8859-1");
            var serializer = new XmlSerializer(serializableObject.GetType());
            serializer.Serialize(stringwriter, serializableObject);
            return stringwriter.ToString();
        }
        catch
        {
            throw;
        }
    }

    public static string SerializeToISO<T>(T value)
    {

        if (value == null)
        {
            return null;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(T));

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = new Iso88591Encoding();// Encoding.GetEncoding("ISO-8859-1");//new UnicodeEncoding(false, false); // no BOM in a .NET string
        settings.Indent = true;
        settings.OmitXmlDeclaration = false;

        using (StringWriter textWriter = new ISO8859StringWriter())
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
            {
                serializer.Serialize(xmlWriter, value);
            }
            return textWriter.ToString();
        }
    }

    public static T Deserialize<T>(string xml)
    {

        if (string.IsNullOrEmpty(xml))
        {
            return default(T);
        }



        //XmlRootAttribute xRoot = new XmlRootAttribute();
        //xRoot.ElementName = "SetDTE";
        //xRoot.IsNullable = true;
        //XmlSerializer serializer = new XmlSerializer(typeof(T), xRoot);
       XmlSerializer serializer = new XmlSerializer(typeof(T));

        XmlReaderSettings settings = new XmlReaderSettings();

        using (StringReader textReader = new StringReader(xml))
        {
            using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
            {
               
                return (T)serializer.Deserialize(xmlReader);
            }
        }
    }

    public static T Deserialize<T>(string xml, XmlRootAttribute xmlRootAtrib)
    {
        if (string.IsNullOrEmpty(xml))
        {
            return default(T);
        }

        XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRootAtrib);

        XmlReaderSettings settings = new XmlReaderSettings();

        using (StringReader textReader = new StringReader(xml))
        {
            using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
            {
                return (T)serializer.Deserialize(xmlReader);
            }
        }
    }

    public static string ProcessStateToFriendlyName(string tipo)
    {
        switch (tipo)
        {
            case "RSC":
                return "RECHAZADO POR ERROR DE SCHEMA";
            case "SOK":
                return "Schema Validado";
            case "CRT":
                return "Carátula OK";
            case "RFR":
                return "Rechazado por Error en Firma";
            case "FOK":
                return "Firma de Envió Validada";
            case "PDR":
                return "Envió en Proceso";
            case "RCT":
                return "Rechazado por Error en Carátula";
            case "EPR":
                return "Envió Procesado";
            case "-11":
                return "Error Comunicación SII";
            case "003":
                return "Error Interno SII";
            default:
                return tipo;
        }
    }

    private static string digitoVerificador(int rut)
    {
        int Digito;
        int Contador;
        int Multiplo;
        int Acumulador;
        string RutDigito;

        Contador = 2;
        Acumulador = 0;

        while (rut != 0)
        {
            Multiplo = (rut % 10) * Contador;
            Acumulador = Acumulador + Multiplo;
            rut = rut / 10;
            Contador = Contador + 1;
            if (Contador == 8)
            {
                Contador = 2;
            }

        }

        Digito = 11 - (Acumulador % 11);
        RutDigito = Digito.ToString().Trim();
        if (Digito == 10)
        {
            RutDigito = "K";
        }
        if (Digito == 11)
        {
            RutDigito = "0";
        }
        return (RutDigito);
    }

    public static bool ValidaRut(string _rut)
    {
        string[] PasaRUT = _rut.Split('-');
        if (PasaRUT.Length >= 2)
        {
            try
            {
                string digito = digitoVerificador(Int32.Parse(PasaRUT[0]));
                if (digito == PasaRUT[1].ToUpper())
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        else
            return false;
    }

    public static string NumeroConPuntosDeMiles(int val)
    {
        //var nfi = (NumberFormatInfo)CultureInfo.CreateSpecificCulture("es").NumberFormat;
        nfi.NumberGroupSeparator = ".";
        return val.ToString("#,0", nfi);
    }

    public static string FormatoRutMembrete(string val)
    {
        if (val.Contains("-"))
        {
            string RutGuionYnumero = val.Substring(val.Length - 2);
            string RutSolonumero = val.Substring(0, val.Length - 2);

            int Val = ParseInt(RutSolonumero);

            nfi.NumberGroupSeparator = ".";
            string ReturnVAl = Val.ToString("#,0", nfi);

            if (ReturnVAl.Contains("."))
                return ReturnVAl + RutGuionYnumero;
            else
                return val;
        }
        else
        {
            return string.Empty;
        }

    }


    //public static string NumeroConPuntosDeMiles(string val)
    //{



    //    int Val = ParseInt(val);

    //    nfi.CurrencySymbol = "$";
    //    nfi.CurrencyNegativePattern = 2;
    //    nfi.CurrencyPositivePattern = 0;

    //    string ReturnVAl = Val.ToString("C0", new CultureInfo("en-US").NumberFormat = nfi);

    //    if (ReturnVAl.Contains("."))
    //        return ReturnVAl;
    //    else
    //        return val;

    //}
    public static string NumeroConPuntosDeMiles(decimal val)
    {
        //var nfi = (NumberFormatInfo)CultureInfo.CreateSpecificCulture("es").NumberFormat;
        nfi.NumberGroupSeparator = ".";
        nfi.NumberDecimalSeparator = ",";
        return val.ToString("#,0", nfi);
    }


    public static string LocalizarErrores(string _objStr)
    {
        if (string.IsNullOrWhiteSpace(_objStr))
            return _objStr;
        else
        {
            _objStr = _objStr.Replace(@"Passwords must have at least one non letter or digit character.", "Las contraseñas deben tener al menos un caracter que no sea letra ni numero." + Environment.NewLine);
            _objStr = _objStr.Replace(@"Passwords must have at least one digit ('0'-'9').", "Las contraseñas deben tener al menos un digito ('0'-'9')." + Environment.NewLine);
            _objStr = _objStr.Replace(@"Passwords must have at least one uppercase ('A'-'Z').", "Las contraseñas deben tener al menos una mayuscula ('A'-'Z')." + Environment.NewLine);
            _objStr = _objStr.Replace(@"Passwords must have at least one lowercase ('a'-'z').", "Las contraseñas deben tener al menos una minuscula ('a'-'z')." + Environment.NewLine);
            _objStr = _objStr.Replace(@"The Email field is not a valid e-mail address.", "El campo E-Mail no tiene una direccion de correo valida." + Environment.NewLine);
            return _objStr;
        }
    }

    public static string GenerateCaptcha(this HtmlHelper helper)
    {
        var captchaControl = new Recaptcha.RecaptchaControl
        {
            ID = "recaptcha",
            Theme = "clean",
            PublicKey = "6LeqAk8UAAAAAGX88c__hAA-oizzvtD6vRoY5Mix",
            PrivateKey = "6LeqAk8UAAAAAAA8bHAq4bHKIYcTNT7Fb5wmJtJT"
            /* OLD V1 CAPTCHA KEYS, Google termino de dar soporte de CaptchaV1
            PublicKey = "6Le48RAUAAAAAD75pylSu2PRRc3HF8ePDh5d2h62",
            PrivateKey = "6Le48RAUAAAAAN-qWaZeUkCDZpGpR1eS42ySike9"
            */
        };

        var htmlWriter = new HtmlTextWriter(new StringWriter());

        captchaControl.RenderControl(htmlWriter);

        return htmlWriter.InnerWriter.ToString();
    }





    public static string RandomDigitsSTR(int length)
    {
        var random = new Random();
        string s = string.Empty;
        for (int i = 0; i < length; i++)
            s = String.Concat(s, random.Next(10).ToString());
        return s;
    }

    public static int RandomDigitsINT(int length)
    {
        var random = new Random();
        string s = string.Empty;
        for (int i = 0; i < length; i++)
            s = String.Concat(s, random.Next(10).ToString());
        return ParseExtensions.ParseInt(s);
    }

    public static Tuple<string, string> GetCAFBasedOnType(TipoDte Tipo_DTE, CertificadosModels objCertificado)
    {
        Tuple<string, string> objTupleReturn;

        XmlDocument xCAFGeneric = new XmlDocument();
        xCAFGeneric.PreserveWhitespace = true;

        if (objCertificado == null)
            throw new Exception("certificado nnulo");

        if (Tipo_DTE == TipoDte.FacturaElectronica && objCertificado.PathCAF33 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF33);
        else if (Tipo_DTE == TipoDte.FacturaElectronicaExenta && objCertificado.PathCAF34 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF34);
        else if (Tipo_DTE == TipoDte.BoletaElectronica && objCertificado.PathCAF39 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF39);
        else if (Tipo_DTE == TipoDte.BoletaExentaElectronica && objCertificado.PathCAF41 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF41);
        else if (Tipo_DTE == TipoDte.GuiaDespachoElectronica && objCertificado.PathCAF52 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF52);
        else if (Tipo_DTE == TipoDte.NotaDebitoElectronica && objCertificado.PathCAF56 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF56);
        else if (Tipo_DTE == TipoDte.NotaCreditoElectronica && objCertificado.PathCAF61 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF61);
        else if (Tipo_DTE == TipoDte.FacturaExportacionElectronica && objCertificado.PathCAF110 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF110);
        else if (Tipo_DTE == TipoDte.NotaDebitoExportacionElectronica && objCertificado.PathCAF111 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF111);
        else if (Tipo_DTE == TipoDte.NotaCreditoExportacionElectronica && objCertificado.PathCAF112 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF112);
        else if (Tipo_DTE == TipoDte.FacturaCompraElectronica && objCertificado.PathCAF46 != null)
            xCAFGeneric.LoadXml(objCertificado.PathCAF46);
        else
            return new Tuple<string, string>(string.Empty, string.Empty);


        objTupleReturn = new Tuple<string, string>(xCAFGeneric.SelectSingleNode("AUTORIZACION/CAF").OuterXml, xCAFGeneric.SelectSingleNode("AUTORIZACION/RSASK").InnerText);
        return objTupleReturn;
    }

    public static bool CAF_Disponible_a_Operar(string userID, UsuarioModel objUsuario, QuickEmisorModel objEmisor, TipoDte TipoAOperar, int folioPlannedToUse, out string cafRanges)
    {
        cafRanges = string.Empty;
        if (string.IsNullOrWhiteSpace(userID) || objUsuario == null || objEmisor == null)
            return false;
        else
        {
            int FolioReturnValue = folioPlannedToUse;

            Tuple<string, string> TupleReturn = ParseExtensions.GetCAFBasedOnType(TipoAOperar, objEmisor.Certificados);
            if (TupleReturn == null || TupleReturn.Item1 == null || string.IsNullOrWhiteSpace(TupleReturn.Item1))
                return false;

            XmlDocument xDoc = new XmlDocument();
            xDoc.PreserveWhitespace = true;
            xDoc.LoadXml(TupleReturn.Item1);

            int folioFinalCaf = ParseExtensions.ParseInt(xDoc.SelectSingleNode("CAF/DA/RNG/H").InnerText);
            int folioInicialCaf = ParseExtensions.ParseInt(xDoc.SelectSingleNode("CAF/DA/RNG/D").InnerText);
            cafRanges = folioInicialCaf + "-" + folioFinalCaf;

            if (FolioReturnValue > folioFinalCaf)
            {
                //Se esta intentando usar un folio superior al limite actual del CAF
                return false;
            }
            else if (FolioReturnValue < folioInicialCaf)
            {
                //Se esta intentando usar un folio inferior al limite inferior del CAF
                return false;
            }
            else
            {
                return true;
            }

        }
    }

  
 
   
    public static int ParseInt(string _string)
    {
        int returnValue = 0;
        Int32.TryParse(_string, out returnValue);
        return returnValue;
    }

 

    public static decimal ParseDecimal(string _string)
    {
        decimal returnValue = 0m;
        Decimal.TryParse(_string, out returnValue);
        return returnValue;
    }

    //Posee restriccion que impide seleccionar tipos que no sean aptos por no tener suficientes CAF 
    //y utiliza mecanismo nuevo
    

    public static List<SelectListItem> TipoCAFCargados(CertificadosModels objCert, bool ObtenerNotas = false)
    {
        List<SelectListItem> listItems = new List<SelectListItem>();

        if (objCert == null)
            return listItems;

        List<string> GeneralProcessingList = new List<string>();

        if (ObtenerNotas == false)
        {
            GeneralProcessingList.Add(objCert.PathCAF33);
            GeneralProcessingList.Add(objCert.PathCAF34);
            //  GeneralProcessingList.Add(objCert.PathCAF110);
            //  GeneralProcessingList.Add(objCert.PathCAF52);
            //  GeneralProcessingList.Add(objCert.PathCAF39);
            //  GeneralProcessingList.Add(objCert.PathCAF41);
        }
        else
        {
            GeneralProcessingList.Add(objCert.PathCAF61);
            GeneralProcessingList.Add(objCert.PathCAF56);
            //    GeneralProcessingList.Add(objCert.PathCAF111);
            //    GeneralProcessingList.Add(objCert.PathCAF112);
            //    GeneralProcessingList.Add(objCert.PathCAF46);
        }

        foreach (string STR in GeneralProcessingList)
        {
            if (string.IsNullOrWhiteSpace(STR))
                continue;
            XmlDocument xDoc = new XmlDocument();
            xDoc.PreserveWhitespace = true;
            xDoc.LoadXml(STR);

            int TipoNumerico = ParseExtensions.ParseInt(xDoc.SelectSingleNode("//AUTORIZACION/CAF/DA/TD").InnerXml);
            TipoDte TipoDocumento = (TipoDte)TipoNumerico;
            listItems.Add(new SelectListItem { Text = ParseExtensions.TipoDTEToFriendlyName(TipoDocumento), Value = TipoNumerico.ToString() });
        }

        return listItems;

    }

    public static List<string[]> CAFCargados(CertificadosModels objCert)
    {
        List<string[]> rtrnValue = new List<string[]>();

        if (objCert == null)
            return rtrnValue;

        List<string> GeneralProcessingList = new List<string>();
        GeneralProcessingList.Add(objCert.PathCAF33);
        GeneralProcessingList.Add(objCert.PathCAF34);
        GeneralProcessingList.Add(objCert.PathCAF61);
        GeneralProcessingList.Add(objCert.PathCAF56);
        GeneralProcessingList.Add(objCert.PathCAF110);
        GeneralProcessingList.Add(objCert.PathCAF52);
        GeneralProcessingList.Add(objCert.PathCAF39);
        GeneralProcessingList.Add(objCert.PathCAF41);
        GeneralProcessingList.Add(objCert.PathCAF111);
        GeneralProcessingList.Add(objCert.PathCAF112);
        GeneralProcessingList.Add(objCert.PathCAF46);

        foreach (string STR in GeneralProcessingList)
        {
            if (string.IsNullOrWhiteSpace(STR))
                continue;
            XmlDocument xDoc = new XmlDocument();
            xDoc.PreserveWhitespace = true;
            xDoc.LoadXml(STR);

            string[] arrThingie = new string[4];

            int TipoNumerico = ParseExtensions.ParseInt(xDoc.SelectSingleNode("//AUTORIZACION/CAF/DA/TD").InnerXml);
            TipoDte TipoDocumento = (TipoDte)TipoNumerico;
            arrThingie[0] = ParseExtensions.EnumGetDisplayAttrib(TipoDocumento);
            arrThingie[1] = xDoc.SelectSingleNode("//AUTORIZACION/CAF/DA/RNG/D").InnerXml;
            arrThingie[2] = xDoc.SelectSingleNode("//AUTORIZACION/CAF/DA/RNG/H").InnerXml;
            arrThingie[3] = xDoc.SelectSingleNode("//AUTORIZACION/CAF/DA/FA").InnerXml;

            rtrnValue.Add(arrThingie);
        }

        return rtrnValue;

    }

    public static string ObtenerFechaTextualMembreteLibroVentaCompra(string fechainicio, string fechafin, int? Anio, int? Mes, string TituloDocumento)
    {
        //agregar parametro opcional para cuenta contable como subtitulo
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TituloDocumento + " ");

            if (Anio == null && Mes == null)
            {
                sb.Append(DateTime.Now.Year);
                return sb.ToString();
            }
            else
            {
                string nombreMes = null;
                if (Mes != null)
                {
                    nombreMes = ParseExtensions.obtenerNombreMes(Mes.Value);
                    sb.Append(nombreMes + " ");
                }
                if (Anio != null)
                {
                    if (nombreMes != null)
                    {
                        sb.Append("de " + Anio.Value);
                    }
                    else
                    {
                        sb.Append(Anio.Value + " ");
                    }
                }

            }

            if (string.IsNullOrWhiteSpace(fechainicio) == false)
            {
                sb.Append(" DESDE " + fechainicio);
            }
            if (string.IsNullOrWhiteSpace(fechafin) == false)
            {
                sb.Append(" HASTA " + fechafin);
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return "Titulo";
        }
    }

    public static string FirstLetterToUpper(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    public static string EncodeImageAsBase64(string filePath)
    {
        using (Image image = Image.FromFile(filePath))
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }

    public static string GetXmlEnumAttributeValueFromEnum<TEnum>(this TEnum value) where TEnum : struct, IConvertible
    {
        var enumType = typeof(TEnum);
        if (!enumType.IsEnum) return null;//or string.Empty, or throw exception

        var member = enumType.GetMember(value.ToString()).FirstOrDefault();
        if (member == null) return null;//or string.Empty, or throw exception

        var attribute = member.GetCustomAttributes(false).OfType<XmlEnumAttribute>().FirstOrDefault();
        if (attribute == null) return null;//or string.Empty, or throw exception
        return attribute.Name;
    }


    public static string Get_AppData_Path(string fileName)
    {
        string appDataFolder = HttpContext.Current.Server.MapPath("~/App_Data/");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return appDataFolder;
        }
        string NombreArchivoUnico = fileName;
        string fullPath = Path.Combine(appDataFolder, NombreArchivoUnico);
        return fullPath;
    }

    public static string Get_Temp_path(string fileName)
    {
        string appDataFolder = HttpContext.Current.Server.MapPath("~/Temp/");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return appDataFolder;
        }
        string NombreArchivoUnico = fileName;
        string fullPath = Path.Combine(appDataFolder, NombreArchivoUnico);
        return fullPath;
    }


    public static int ObtenerCenbtralizacion(TipoCentralizacion tipo) {
        if (TipoCentralizacion.Compra == tipo)
        {
            return (int)TipoCentralizacion.Compra;
        }
        else {
            return (int)TipoCentralizacion.Venta;
        }
        
    }
    public static string obtenerNombreMes(int mesNumero)
    {
        string nombreMes = string.Empty;
        if (mesNumero > 12 || mesNumero < 1)
            return nombreMes;
        DateTime tmpDateObj = new DateTime(1987, mesNumero, 1);
        nombreMes = tmpDateObj.ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));
        return nombreMes.MayusculaPrimeraLetra();
    }

    public static string ObtenerSelectInputEstadoPago(int? selectedVal = null)
    {
        //EnumGetDisplayAttrib(TiposDeVoucher) +
        StringBuilder sb = new StringBuilder();

        var enumValues = ParseExtensions.ObtenerValoresPosiblesEnum<Paid_Status>().ToList();
        enumValues.RemoveAll(r => (int)r == -1);
        foreach (var estados_pago in enumValues)
        {
            if (selectedVal.HasValue)
            {
                if (selectedVal.Value == (int)estados_pago)
                    sb.AppendLine("<option value=\"" + (int)estados_pago + "\" selected=\"selected\">" + EnumGetDisplayAttrib(estados_pago) + "</option>");
                else
                    sb.AppendLine("<option value=\"" + (int)estados_pago + "\">" + EnumGetDisplayAttrib(estados_pago) + "</option>");
            }
            else
            {
                sb.AppendLine("<option value=\"" + (int)estados_pago + "\">" + EnumGetDisplayAttrib(estados_pago) + "</option>");
            }
        }
        return sb.ToString();
    }



    public static void AddVerticalSpace(ref HtmlAgilityPack.HtmlDocument doc, string liNodeName, string textToCheck)
    {
        var fillerNode = HtmlAgilityPack.HtmlNode.CreateNode("<li> </li>");
        if (string.IsNullOrWhiteSpace(textToCheck))
        { return; }
        if (textToCheck.Length > 52)
        {
            var node = doc.GetElementbyId(liNodeName);
            int numberOfTimes = textToCheck.Length / 52;
            for (int i = 0; i < numberOfTimes; i++)
            {
                node.ParentNode.InsertAfter(fillerNode, node);
            }
        }
    }

    /// <summary>
    /// Transforma una lista de objetos en una representacion string de select values de HTML option, siempre
    /// y cuando se suministre nombres de campos validos para nombre y valor en estos objetos, de no ser asi
    /// el metodo se interrumpira en la primera instancia de estos
    /// </summary>
    /// <typeparam name="T">Tipo de lista</typeparam>
    /// <param name="passedList">Lista de objetos de los cuales se quiere construir los Selects de Option HTML</param>
    /// <param name="valueField">Nombre del atributo que se requiere rescatar como VALOR en objetos de tipo T</param>
    /// <param name="nameField">Nombre del atributo que se requiere rescatar como NOMBRE en objetos de tipo T</param>
    /// <returns>Vacio si la lista es invalida o nula, de no ser asi un string que representa los <select> de un input option en HTML</returns>
    public static string ListAsHTML_Input_Select<T>(IList<T> passedList, string valueField, string nameField, string selectedVal = null)
    {
        StringBuilder sb = new StringBuilder();
        if (passedList == null || string.IsNullOrWhiteSpace(valueField) || string.IsNullOrWhiteSpace(nameField))
            return sb.ToString();
        else
        {
            foreach (T thingie in passedList)
            {
                object objValue = GetPropValue(thingie, valueField);
                object objName = GetPropValue(thingie, nameField);
                if (objValue == null || objName == null)
                    break;
                string strValue = objValue.ToString();
                string strName = objName.ToString();

                if (selectedVal != null && selectedVal == strValue)
                    sb.AppendLine("<option selected value=\"" + strValue + "\">" + strName + "</option>");
                else
                    sb.AppendLine("<option value=\"" + strValue + "\">" + strName + "</option>");
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Transforma una lista de objetos en una representacion string de select values de HTML option, siempre
    /// y cuando se suministre nombres de campos validos para nombre y valor en estos objetos, de no ser asi
    /// el metodo se interrumpira en la primera instancia de estos 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="passedList">Lista de objetos de los cuales se quiere construir los Selects de Option HTML</param>
    /// <param name="valueField">Nombre del atributo que se requiere rescatar como VALOR en objetos de tipo T</param>
    /// <param name="nameField">Lista de strings que se utiizaran para construir el nombre del option select, todo valor despues del primero ira dentro de parentesis. No debe ser nulo o tener elementos nulos o en blanco</param>
    /// <returns>Vacio si la lista es invalida o nula, de no ser asi un string que representa los <select> de un input option en HTML</returns>
    public static string ListAsHTML_Input_Select<T>(IList<T> passedList, string valueField, List<string> nameFields, string selectedVal = null)
    {
        StringBuilder sb = new StringBuilder();
        if (passedList == null || string.IsNullOrWhiteSpace(valueField) || nameFields == null || nameFields.Count == 0 || nameFields.Any(r => string.IsNullOrWhiteSpace(r)))
            return sb.ToString();
        else
        {
            foreach (T thingie in passedList)
            {
                StringBuilder sbCompositeName = new StringBuilder();
                object objValue = GetPropValue(thingie, valueField);

                sbCompositeName.Append(GetPropValue(thingie, nameFields[0]));
                if (nameFields.Count > 1)
                {
                    sbCompositeName.Append(" (");
                    for (int i = 1; i < nameFields.Count; i++)
                    {
                        sbCompositeName.Append(" " + GetPropValue(thingie, nameFields[i]) + " ");
                    }
                    sbCompositeName.Append(")");
                }
                if (objValue == null)
                    break;
                string strValue = objValue.ToString();
                string strName = sbCompositeName.ToString();

                if (selectedVal != null && selectedVal == strValue)
                    sb.AppendLine("<option selected value=\"" + strValue + "\">" + strName + "</option>");
                else
                    sb.AppendLine("<option value=\"" + strValue + "\">" + strName + "</option>");
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Transforma una lista de objetos en una representacion string de select values de HTML option, siempre
    /// y cuando se suministre nombres de campos validos para nombre y valor en estos objetos, de no ser asi
    /// el metodo se interrumpira en la primera instancia de estos 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="passedList">Lista de objetos de los cuales se quiere construir los Selects de Option HTML</param>
    /// <param name="valueField">Nombre del atributo que se requiere rescatar como VALOR en objetos de tipo T</param>
    /// <param name="nameFields">Lista de strings que se utiizaran para construir el nombre del option select, todo valor despues del primero ira dentro de parentesis. No debe ser nulo o tener elementos nulos o en blanco</param>
    /// <param name="selectedVal">Lista de strings que corresponden a los valores seleccionados para input select que soportan campos multiples</param>
    /// <returns>Vacio si la lista es invalida o nula, de no ser asi un string que representa los <select> de un input option en HTML</returns>
    public static string ListAsHTML_Input_Select<T>(IList<T> passedList, string valueField, List<string> nameFields, List<string> selectedVal)
    {
        StringBuilder sb = new StringBuilder();
        if (passedList == null || string.IsNullOrWhiteSpace(valueField) || nameFields == null || nameFields.Count == 0 || nameFields.Any(r => string.IsNullOrWhiteSpace(r)))
            return sb.ToString();
        else
        {
            foreach (T thingie in passedList)
            {
                StringBuilder sbCompositeName = new StringBuilder();
                object objValue = GetPropValue(thingie, valueField);

                sbCompositeName.Append(GetPropValue(thingie, nameFields[0]));
                if (nameFields.Count > 1)
                {
                    sbCompositeName.Append(" (");
                    for (int i = 1; i < nameFields.Count; i++)
                    {
                        sbCompositeName.Append(" " + GetPropValue(thingie, nameFields[i]) + " ");
                    }
                    sbCompositeName.Append(")");
                }
                if (objValue == null)
                    break;
                string strValue = objValue.ToString();
                string strName = sbCompositeName.ToString();

                if (selectedVal != null && selectedVal.Contains(strValue))
                    sb.AppendLine("<option selected value=\"" + strValue + "\">" + strName + "</option>");
                else
                    sb.AppendLine("<option value=\"" + strValue + "\">" + strName + "</option>");
            }
        }
        return sb.ToString();
    }

    public static object GetPropValue(object src, string propName)
    {
        try
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        catch
        {
            return null;
        }
    }

    public static List<SelectListItem> EnumToDropDownList<T>()
    {
        var t = typeof(T);

        if (!t.IsEnum) { throw new ApplicationException("Tipo debe ser enum"); }

        var members = t.GetFields(BindingFlags.Public | BindingFlags.Static);

        var result = new List<SelectListItem>();

        foreach (var member in members)
        {
            var attributeDescription = member.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute),
                false);
            var descripcion = member.Name;

            if (attributeDescription.Any())
            {
                descripcion = ((System.ComponentModel.DescriptionAttribute)attributeDescription[0]).Description;
            }

            var valor = ((int)Enum.Parse(t, member.Name));
            result.Add(new SelectListItem()
            {
                Text = descripcion,
                Value = valor.ToString()
            });
        }
        return result;
    }

 

    public static string EnumAsHTML_Input_Select<T>(int? option = null, bool displayValueOnName = false)
    {
        StringBuilder sb = new StringBuilder();

        Type genericType = typeof(T);
        if (genericType.IsEnum)
        {
            foreach (T obj in Enum.GetValues(genericType))
            {
                Enum test = Enum.Parse(typeof(T), obj.ToString()) as Enum;
                int x = Convert.ToInt32(test); // x is the integer value of enum

                string displayName = EnumGetDisplayAttrib(test);

                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = test.ToString();

                if (displayValueOnName == true)
                    displayName = x + " " + displayName;

                if (option.HasValue && option.Value == x)
                {
                    sb.AppendLine("<option selected value=\"" + x + "\">" + displayName + "</option>");

                }
                else
                {
                    sb.AppendLine("<option value=\"" + x + "\">" + displayName + "</option>");
                }


            }
        }

        return sb.ToString();
    }

    public static DateTime FechaAnexarHoraActual(DateTime objFecha)
    {
        DateTime dtCurrent = DateTime.Now;
        TimeSpan ts = new TimeSpan(dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second);
        return objFecha.Date + ts;
    }

    public static DateTime FechaAnexarHoraActual(string strDD_MM_AAAA)
    {
        DateTime objFecha = ParseExtensions.ToDD_MM_AAAA(strDD_MM_AAAA);
        DateTime dtCurrent = DateTime.Now;
        TimeSpan ts = new TimeSpan(dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second);
        return objFecha.Date + ts;
    }

   
    public static string ArchivoABase64(HttpPostedFileBase PassedFile, int? max_size_allowed = null)
    {
        string fileAsString = null;
        if (PassedFile.ContentLength > 0 && (max_size_allowed.HasValue ? PassedFile.ContentLength <= max_size_allowed.Value : true))
        {
            byte[] fileInBytes = new byte[PassedFile.ContentLength];
            using (BinaryReader theReader = new BinaryReader(PassedFile.InputStream))
            {
                fileInBytes = theReader.ReadBytes(PassedFile.ContentLength);
            }
            fileAsString = Convert.ToBase64String(fileInBytes);
        }
        return fileAsString;
    }

    

    public static string pGetAttachmentData(string base64ArchivoAsociado, string TipoArchivoAsociado, string[] ValidAttachmentTypes)
    {
        string returnValue = string.Empty;
        if (string.IsNullOrWhiteSpace(base64ArchivoAsociado) == false && string.IsNullOrWhiteSpace(TipoArchivoAsociado) == false)
        {
            string tipoArchivoInLowerCase = TipoArchivoAsociado.ToLowerInvariant();
            if (ValidAttachmentTypes.Contains(tipoArchivoInLowerCase) == true)
            {
                if (tipoArchivoInLowerCase == ".jpg" || tipoArchivoInLowerCase == ".jpeg")
                {
                    string dataPart = @"data:image/jpeg;base64," + base64ArchivoAsociado;
                    returnValue = "<img src = '" + dataPart + "' width='100%' height='500'>";
                }
                else if (tipoArchivoInLowerCase == ".png")
                {
                    string dataPart = @"data:image/png;base64," + base64ArchivoAsociado;
                    returnValue = "<img src = '" + dataPart + "' width='100%' height='500'>";
                }
                else if (tipoArchivoInLowerCase == ".pdf")
                {
                    string dataPart = @"data:application/pdf;base64," + base64ArchivoAsociado;
                    returnValue = "<embed src= '" + dataPart + "' type='application/pdf' width='100%' height='500'/>";
                }
            }
        }
        return returnValue;
    }

    public enum IndicadorTrasladoEnum
    {
        [Display(Name = "1-Operación constituye venta")]
        Ind_1 = 1,
        [Display(Name = "2-Ventas por efectuar")]
        Ind_2 = 2,
        [Display(Name = "3-Consignaciones")]
        Ind_3 = 3,
        [Display(Name = "4-Entrega gratuita")]
        Ind_4 = 4,
        [Display(Name = "5-Traslados internos")]
        Ind_5 = 5,
        [Display(Name = "6-Otros traslados no venta")]
        Ind_6 = 6,
        [Display(Name = "7-Guía de devolución")]
        Ind_7 = 7,
        [Display(Name = "8-Traslado para exportación")]
        Ind_8 = 8,
        [Display(Name = "9-Venta para exportación")]
        Ind_9 = 9
    }


    public static string[] obtieneUnicosEnLista(IList<string[]> lista) {
        string[] unicos = { ""};
        if (lista.Count() > 0) { 
                string[] cuentas = new string[lista.Count() - 1];
            for (int i = 0; i < lista.Count() - 1; i++) {
                cuentas[i] = lista[i][9];
            }


            unicos = cuentas.Distinct().ToArray();
        }
        return unicos;
    }

    public static List<string> ObtieneLstAuxNombre(List<EstadoCuentasCorrientesViewModel> Lista)
    {
        List<string> ReturnValues = new List<string>();
        if(Lista != null)
            ReturnValues = Lista.Select(cta => "[" +  cta.CuentaContable.CodInterno + "]" + " " + cta.CuentaContable.nombre).Distinct().ToList();

        return ReturnValues;
    }

    public static List<string> ObtieneLstAuxRut(List<EstadoCuentasCorrientesViewModel> Lista)
    {
        List<string> ReturnValues = new List<string>();
        if (Lista != null)
             ReturnValues = Lista.Select(cta => cta.RutPrestador).Distinct().ToList();

        return ReturnValues;
    }

    public static List<string> ObtieneLstCodInternosActivos(List<EstCtasCtesConciliadasViewModel> Lista)
    {
        List<string> ReturnValues = new List<string>();
        if (Lista != null)
            ReturnValues = Lista.Select(cta => cta.CuentaContable.CodInterno).Distinct().ToList();

        return ReturnValues;
    }

    public static List<string> ObtieneLstAuxNombre(List<EstCtasCtesConciliadasViewModel> Lista)
    {
        List<string> ReturnValues = new List<string>();
        if (Lista != null)
            ReturnValues = Lista.Select(cta => "[" + cta.CuentaContable.CodInterno + "]" + " " + cta.CuentaContable.nombre).Distinct().ToList();

        return ReturnValues;
    }

    public static List<string> ObtieneLstAuxRut(List<EstCtasCtesConciliadasViewModel> Lista)
    {
        List<string> ReturnValues = new List<string>();
        if (Lista != null)
            ReturnValues = Lista.Select(cta => cta.RutPrestador).Distinct().ToList();

        return ReturnValues;
    }

    public static string ObtenerCodigoYNombreCtaContable(string CodInterno, ClientesContablesModel objCliente)
    {
        var BuscaCtaContable = objCliente.CtaContable.SingleOrDefault(x => x.CodInterno == CodInterno);
        string CodigoInternoConNombre = BuscaCtaContable.CodInterno + " " + BuscaCtaContable.nombre;
        return CodigoInternoConNombre;
    }

    public static CuentaContableModel ObtenerCuentaContableDesdeNombre(string Nombre, ClientesContablesModel objCliente)
    {
        CuentaContableModel ReturnValues = new CuentaContableModel();
        ReturnValues = objCliente.CtaContable.FirstOrDefault(x => x.nombre == Nombre);
        return ReturnValues;
    }

    public static CuentaContableModel ObtenerCuentaDesdeId(int IdCuenta, ClientesContablesModel ObjCliente)
    {
        CuentaContableModel ReturnValues = new CuentaContableModel();
        ReturnValues = ObjCliente.CtaContable.FirstOrDefault(x => x.CuentaContableModelID == IdCuenta);

        return ReturnValues;
    }


    public static dynamic obtieneUnicosEnLista(dynamic lista)
    {
        string[] unicos = { "" };
        if (lista.Count() > 0)
        {
            string[] cuentas = new string[lista.Count - 1];
            for (int i = 0; i < lista.Count - 1; i++)
            {
                cuentas[i] = lista[i][8];
            }


            unicos = cuentas.Distinct().ToArray();
        }
        return unicos;
    }

}



public enum Paid_Status
{
    [Display(Name = "Ninguno")]
    unknown = -1,
    [Display(Name = "Por Pagar")]
    unpaid = 0,
    [Display(Name = "Pagado")]
    paid = 1
}

public static class Extensions
{
    public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector)
      where U : IComparable<U>
    {
        if (source == null) throw new ArgumentNullException("source is null");
        bool first = true;
        T maxObj = default(T);
        U maxKey = default(U);
        foreach (var item in source)
        {
            if (first)
            {
                maxObj = item;
                maxKey = selector(maxObj);
                first = false;
            }
            else
            {
                U currentKey = selector(item);
                if (currentKey.CompareTo(maxKey) > 0)
                {
                    maxKey = currentKey;
                    maxObj = item;
                }
            }
        }
        if (first) throw new InvalidOperationException("Sequence is empty.");
        return maxObj;
    }

    public static bool Entre(this DateTime dateToCheck, DateTime fechaInicio, DateTime fechaFin)
    {
        return dateToCheck >= fechaInicio && dateToCheck < fechaFin;
    }

    public static bool Entre(this DateTime dateToCheck, DateTime? fechaInicio, DateTime? fechaFin)
    {
        if (fechaInicio == null || fechaFin == null)
            return false;
        return dateToCheck >= fechaInicio && dateToCheck <= fechaFin;
    }


    public static string Truncar(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    /// <summary>
    /// Determina si el string dado es un E-Mail
    /// </summary>
    /// <param name="STR">El string a validar</param>
    /// <returns>TRUE: Si el string es un E-Mail, FALSE en caso contrario</returns>
    public static bool IsValidEMail(this string STR)
    {
        return new EmailAddressAttribute().IsValid(STR);
    }

    public static string RemoverAcentos(this string STR)
    {
        string offendingCharacters = "ÀÂÃÄÅÆÇÈÊËÌÎÏÐÒÔÕÖØÙÛÜÝßàâãäåæçèêëìîïðòôõöøùûüýÿ";
        string replacedCharacters = "AAAAAACEEEIIIDOOOOOUUUYBaaaaaaceeeiiioooooouuuyy";

        string textoSinAcentos = string.Empty;

        foreach (var caracter in STR)
        {
            var indexConAcento = offendingCharacters.IndexOf(caracter);
            if (indexConAcento > -1)
                textoSinAcentos = textoSinAcentos + (replacedCharacters.Substring(indexConAcento, 1));
            else
                textoSinAcentos = textoSinAcentos + (caracter);
        }
        return textoSinAcentos;
        /*
        if (string.IsNullOrWhiteSpace(STR))
            return string.Empty;
        else
            return Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(STR));*/
    }
    /// <summary>
    ///     A generic extension method that aids in reflecting 
    ///     and retrieving any attribute that is applied to an `Enum`.
    /// </summary>
    public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
    {
        return enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttribute<TAttribute>();
    }

    public static string MayusculaPrimeraLetra(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;
        if (value.Length > 1)
            return char.ToUpper(value[0]) + value.Substring(1);
        else
            return value.ToUpper();
    }

    public static bool EsDTEExportacion(this TipoDte Tipo)
    {
        if (
            Tipo == TipoDte.FacturaExportacionElectronica ||
            Tipo == TipoDte.NotaCreditoExportacionElectronica ||
            Tipo == TipoDte.NotaDebitoExportacionElectronica ||
            Tipo == TipoDte.FacturaExportacionPapel
          )
        {
            return true;
        }
        else
            return false;  
    }

   
   
   

    /*
    public static bool EsUnaNotaDeCredito(this FacturaQuickModel factura)
    {
        if (factura == null)
            return false;
        if (
            factura.TipoFactura == TipoDte.NotaCreditoElectronica ||
            factura.TipoFactura == TipoDte.NotaCreditoExportacionElectronica ||
            
        )
            return true;
        else
            return false;
    }*/


   

    public static bool EsUnaNotaDeCredito(this TipoDte tipoDTE)
    {
        if (
           tipoDTE == TipoDte.NotaCreditoElectronica ||
           tipoDTE == TipoDte.NotaCreditoExportacionElectronica ||
           tipoDTE == TipoDte.NotaCreditoPapel
           )
        {
            return true;
        }
        else
            return false;
    }

  

    public static bool EsUnaNotaDeDebito(this TipoDte tipoDTE)
    {
        if (
           tipoDTE == TipoDte.NotaDebitoElectronica ||
           tipoDTE == TipoDte.NotaDebitoExportacionElectronica ||
           tipoDTE == TipoDte.NotaDebitoPapel
           )
        {
            return true;
        }
        else
            return false;
    }

   
    public static bool ExentaIVA(this TipoDte tipoFact)
    {
        if (
            tipoFact == TipoDte.FacturaElectronicaExenta ||
            tipoFact == TipoDte.FacturaExentaPapel ||
            tipoFact == TipoDte.FacturaExportacionElectronica ||
            tipoFact == TipoDte.FacturaExportacionPapel ||
            tipoFact == TipoDte.NotaCreditoExportacionElectronica ||
            tipoFact == TipoDte.NotaDebitoExportacionElectronica
          )
        {
            return true;
        }
        else
            return false;
    }
  
    public static bool EsUnaNota(this TipoDte tipoDTE)
    {
        if (
           tipoDTE == TipoDte.NotaCreditoElectronica ||
           tipoDTE == TipoDte.NotaCreditoExportacionElectronica ||
           tipoDTE == TipoDte.NotaCreditoPapel ||
           tipoDTE == TipoDte.NotaDebitoElectronica ||
           tipoDTE == TipoDte.NotaDebitoExportacionElectronica ||
           tipoDTE == TipoDte.NotaDebitoPapel
           )
        {
            return true;
        }
        else
            return false;
    }

    public static bool EsUnaNotaCredito(this TipoDte tipoDTE)
    {
        if (
           tipoDTE == TipoDte.NotaCreditoElectronica ||
           tipoDTE == TipoDte.NotaCreditoExportacionElectronica ||
           tipoDTE == TipoDte.NotaCreditoPapel
           )
        {
            return true;
        }
        else
            return false;
    }

    public static bool EsUnaNotaDebito(this TipoDte tipoDTE)
    {
        if (
           tipoDTE == TipoDte.NotaDebitoElectronica ||
           tipoDTE == TipoDte.NotaDebitoExportacionElectronica ||
           tipoDTE == TipoDte.NotaDebitoPapel
           )
        {
            return true;
        }
        else
            return false;
    }




    public static string EstaPagadaSTR(this BoletasHonorariosModel objBoletaHonorarios)
    {
        decimal totalMonto = objBoletaHonorarios.Brutos;

        decimal totalPagos = 0;
        if (objBoletaHonorarios.HistorialPagos != null)
            totalPagos = objBoletaHonorarios.HistorialPagos.Sum(r => r.MontoPago);

        string resultTest = ParseExtensions.NumeroConPuntosDeMiles(totalPagos) + " / $" + ParseExtensions.NumeroConPuntosDeMiles(totalMonto);
        return resultTest;
    }

    public static decimal PagosMontoPendiente(this BoletasHonorariosModel objBoletaHonorarios)
    {
        if (objBoletaHonorarios == null || objBoletaHonorarios.HistorialPagos == null || objBoletaHonorarios.EstaPagada())
            return 0;
        else
        {
            decimal totalMonto = objBoletaHonorarios.Brutos;
            decimal totalPagos = objBoletaHonorarios.HistorialPagos.Sum(r => r.MontoPago);
            return (totalMonto - totalPagos);
        }
    }

    public static string GetNewComunaName(this QuickReceptorModel objReceptor, string UserID)
    {
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        ComunaModels objComuna = db.DBComunas.SingleOrDefault(r => r.ComunaModelsID == objReceptor.idComuna);
        if (objComuna == null)
            return string.Empty;
        else
            return objComuna.nombre;
    }

    public static string GetNewRegionName(this QuickReceptorModel objReceptor, string UserID)
    {
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        RegionModels objRegion = db.DBRegiones.SingleOrDefault(r => r.RegionModelsID == objReceptor.idRegion);
        if (objRegion == null)
            return string.Empty;
        else
            return objRegion.nombre;
    }
   
    public static bool EstaPagada(this BoletasHonorariosModel objBoletaHonorarios)
    {
        if (objBoletaHonorarios == null)
            return false;
        if (objBoletaHonorarios.Brutos == 0)
            return true;
        else
        {
            if (objBoletaHonorarios.HistorialPagos == null)
                return false;
            if (objBoletaHonorarios.HistorialPagos.Sum(r => r.MontoPago) >= objBoletaHonorarios.Brutos)
                return true;
            else
                return false;
        }
    }



    public static bool ExcedeMontoAPagar(this ICollection<DTEPagosModel> lstPagos, DTEPagosModel PagoAAgregar, decimal MontoTotalALograr)
    {
        decimal SumaTotalPagosActuales = lstPagos.Sum(r => r.MontoPago);
        if (SumaTotalPagosActuales + PagoAAgregar.MontoPago > MontoTotalALograr)
            return true;
        return false;
    }

    public static decimal ObtenerMontoRestante(this ICollection<DTEPagosModel> lstPagos, decimal MontoTotalALograr)
    {
        try
        {
            decimal SumaTotalPagosActuales = lstPagos.Sum(r => r.MontoPago);
            decimal MontoFaltante = MontoTotalALograr - SumaTotalPagosActuales;
            if (MontoFaltante < 0)
                return 0;
            else
                return MontoFaltante;
        }
        catch
        {
            return 0;
        }
    }

    public static string GetOrNull(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;
        else
            return str;
    }

    public static string NumberWithDots(this string sum)
    {
        int number = 0;
        Int32.TryParse(sum, out number);
        return number.ToString("N0", new NumberFormatInfo() { NumberGroupSeparator = ".", NumberDecimalSeparator = "," });
    }

    public static bool ExcedeCantidadUsuarios(this QuickEmisorModel objEmisor, FacturaPoliContext db, out int TopeSuperiorUsuarios, out int CantidadUsuariosSistema)
    {
        TopeSuperiorUsuarios = objEmisor.maxUsuariosParaEstaEmpresa;
        CantidadUsuariosSistema = db.DBEmisoresHabilitados.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID).Count();
        if (CantidadUsuariosSistema >= TopeSuperiorUsuarios)
            return true;
        else
            return false;
    }
    public static bool ExcedeCantidadUsuarios(this QuickEmisorModel objEmisor, FacturaContext db, out int TopeSuperiorUsuarios, out int CantidadUsuariosSistema)
    {
        TopeSuperiorUsuarios = objEmisor.maxUsuariosParaEstaEmpresa;
        CantidadUsuariosSistema = db.DBEmisoresHabilitados.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID).Count();
        if (CantidadUsuariosSistema >= TopeSuperiorUsuarios)
            return true;
        else
            return false;
    }
    public static bool ExcedeCantidadUsuarios(this QuickEmisorModel objEmisor, FacturaProduccionContext db, out int TopeSuperiorUsuarios, out int CantidadUsuariosSistema)
    {
        TopeSuperiorUsuarios = objEmisor.maxUsuariosParaEstaEmpresa;
        CantidadUsuariosSistema = db.DBEmisoresHabilitados.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID).Count();
        if (CantidadUsuariosSistema >= TopeSuperiorUsuarios)
            return true;
        else
            return false;
    }

    internal static decimal MontosPagadosBoletas(List<BoletasHonorariosModel> qryOtrosEgresosFijosBoletas)
    {
        decimal totalResult = 0;
        foreach (BoletasHonorariosModel objBoletaHonorarios in qryOtrosEgresosFijosBoletas)
        {
            decimal totalMonto = objBoletaHonorarios.Brutos;

            decimal totalPagos = 0;
            if (objBoletaHonorarios.HistorialPagos != null) {
                totalPagos = objBoletaHonorarios.HistorialPagos.Sum(r => r.MontoPago);
            }
            if (totalMonto == totalPagos) {
                totalResult = objBoletaHonorarios.Liquido;
            }
        }
        return totalResult;
    }
   
}

internal class Iso88591Encoding : Encoding
{
    private readonly Encoding _encoding;

    public override string WebName => _encoding.WebName.ToUpper();

    public Iso88591Encoding()
    {
        _encoding = GetEncoding("ISO-8859-1");
    }

    public override int GetByteCount(char[] chars, int index, int count)
    {
        return _encoding.GetByteCount(chars, index, count);
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        return _encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        return _encoding.GetCharCount(bytes, index, count);
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        return _encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
    }

    public override int GetMaxByteCount(int charCount)
    {
        return _encoding.GetMaxByteCount(charCount);
    }

    public override int GetMaxCharCount(int byteCount)
    {
        return _encoding.GetMaxCharCount(byteCount);
    }
}

public class OnlyCertification : AuthorizeAttribute
{
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var authorized = base.AuthorizeCore(httpContext);
        if (!authorized)
        {
            return false;
        }

        var user = httpContext.User;
        if (user == null)
            return false;
        string UserID = user.Identity.GetUserId();
        if (String.IsNullOrWhiteSpace(UserID))
            return false;

        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

        UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);
        //QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);

        bool UseProduction = ParseExtensions.ParseSessionUseProdDatabase(ObjUsuario);

        if (UseProduction)
            return false;
        else
            return true;
    }




}



