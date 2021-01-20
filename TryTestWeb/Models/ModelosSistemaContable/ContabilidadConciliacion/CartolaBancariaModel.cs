using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


public class CartolaBancariaModel
{
    public int CartolaBancariaModelId { get; set; }
    public int VoucherModelID { get; set; }
    public DateTime Fecha { get; set; }
    public  ClientesContablesModel ClientesContablesModelID { get; set; }
    public  CuentaContableModel CuentaContableModelID { get; set; }
    public  CartolaBancariaMacroModel CartolaBancariaMacroModelID { get; set; }
    public int Folio { get; set; }
    public string Detalle { get; set; }
    public string Oficina { get; set; }
    public decimal Debe { get; set; }
    public decimal Haber { get; set; }
    public decimal Saldo { get; set; }
    public bool EstaConciliado { get; set; } = false;

    public static List<LibroMayorConciliacion> getListaLibroMayor(List<string[]> MayorDeLaCuenta)
    {
        List<LibroMayorConciliacion> LstLibroMayor = new List<LibroMayorConciliacion>();

        if (MayorDeLaCuenta.Count() > 0) {
            foreach (string[] itemMayor in MayorDeLaCuenta)
            {
                LibroMayorConciliacion objMayor = new LibroMayorConciliacion();

                objMayor.FechaContabilizacion = ParseExtensions.ToDD_MM_AAAA_Multi(itemMayor[1]);
                objMayor.Comprobante = itemMayor[2];
                objMayor.Glosa = itemMayor[3];
                objMayor.RazonSocial = itemMayor[4];
                objMayor.Rut = itemMayor[5];
                if (itemMayor[6].Contains("."))
                {
                    itemMayor[6] = itemMayor[6].Replace(".", "");
                }
                itemMayor[7] = itemMayor[7].Replace(".", "");
                itemMayor[8] = itemMayor[8].Replace(".", "");
                objMayor.Debe = Convert.ToDecimal(itemMayor[6]);
                objMayor.Haber = Convert.ToDecimal(itemMayor[7]);
                objMayor.Saldo = Convert.ToDecimal(itemMayor[8]);
                objMayor.NombreCuentaContable = itemMayor[9];
                if(itemMayor[5] != "Total Final:")
                {
                    objMayor.VoucherId = Convert.ToInt32(itemMayor[11]);
                    objMayor.DetalleVoucherID = Convert.ToInt32(itemMayor[12]);
                }
    
                LstLibroMayor.Add(objMayor);
            }
        }

        return LstLibroMayor;
    }

    public static List<CartolaBancariaModel> ObtenerDetalleCartola(int Id,FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        var LstDetalle = db.DBCartolaBancaria.Where(x => x.CartolaBancariaMacroModelID.CartolaBancariaMacroModelID == Id && x.EstaConciliado == false).ToList();

        return LstDetalle;
    }

    public static List<CartolaBancariaModel> ObtenerNoConciliados(int IdCartolaMacro, FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        var CartolaPadre = db.DBCartolaBMacro.Include("CartolaDetalle")
                                             .SingleOrDefault(x => x.CartolaBancariaMacroModelID == IdCartolaMacro &&
                                             x.ClientesContablesModelID.ClientesContablesModelID == objCliente.ClientesContablesModelID);

        var CartolaHijoNoConciliados = CartolaPadre.CartolaDetalle.Where(x => x.EstaConciliado == false).ToList();

        return CartolaHijoNoConciliados;
    }


    public static List<CartolaBancariaModel> ObtenerCartolaPura(List<ObjCartolaYVouchers> ExcelConCartola, ClientesContablesModel ObjCliente)
    {
        var CartolaPura = ExcelConCartola.Select(x => new CartolaBancariaModel
                                                {
                                                    VoucherModelID = 0,
                                                    Fecha = x.Fecha,
                                                    ClientesContablesModelID = ObjCliente,
                                                    CuentaContableModelID = null,
                                                    Folio = x.Docum,
                                                    Detalle = x.Detalle,
                                                    Oficina = "",
                                                    Debe = x.Debe,
                                                    Haber = x.Haber,
                                                    Saldo = x.Saldo,
                                                    EstaConciliado = false
                                                }).ToList();

        return CartolaPura;
    }

    public static List<CartolaBancariaModel> ObtenerCartolaBancariaManual(List<CartolaBancariaPuraModel> ExcelConCartola, ClientesContablesModel ObjCliente)
    {
        var CartolaPura = ExcelConCartola.Select(x => new CartolaBancariaModel
        {
            VoucherModelID = 0,
            Fecha = x.Fecha,
            ClientesContablesModelID = ObjCliente,
            CuentaContableModelID = null,
            Folio = x.Docum,
            Detalle = x.Detalle,
            Oficina = "",
            Debe = x.Debe,
            Haber = x.Haber,
            Saldo = x.Saldo,
            EstaConciliado = false
        }).ToList();

        return CartolaPura;
    }

    public static List<CartolaBancariaPuraModel> DeExcelACartolaBancaria(HttpPostedFileBase file)
    {
        List<CartolaBancariaPuraModel> ReturnValues = new List<CartolaBancariaPuraModel>();

        if (file == null || file.ContentLength == 0)
        {
            string Error = "Error Excel Vacio";
        }
        else
        {
            if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
            {
                string path = ParseExtensions.Get_Temp_path(file.FileName); // Le indicamos la ruta donde guardará el excel.

                if (File.Exists(path))
                {
                    File.Delete(path); //Si ya existe lo elimina.
                }
                file.SaveAs(path); //Guardamos momentaneamente el fichero. -> La idea es extraer su información y luego eliminarlo.

                Application application = new Application();
                Workbook workBook = application.Workbooks.Open(path);
                Worksheet worksheet = workBook.ActiveSheet;
                Range range = worksheet.UsedRange;

                for (int row = 2; row <= range.Rows.Count; row++)
                {
                    CartolaBancariaPuraModel FilaAGuardar = new CartolaBancariaPuraModel();

                    FilaAGuardar.Fecha = ParseExtensions.ToDD_MM_AAAA_Multi(((Range)range.Cells[row, 1]).Text);
                    FilaAGuardar.Docum = Convert.ToInt32(((Range)range.Cells[row, 2]).Text);
                    FilaAGuardar.Detalle = ((Range)range.Cells[row, 3]).Text;
                    FilaAGuardar.Debe = decimal.Parse(((Range)range.Cells[row, 4]).Text);
                    FilaAGuardar.Haber = decimal.Parse(((Range)range.Cells[row, 5]).Text);
                    FilaAGuardar.Saldo = decimal.Parse(((Range)range.Cells[row, 6]).Text);
                    //Parte del voucher

                    ReturnValues.Add(FilaAGuardar);
                }
                workBook.Close();
                File.Delete(path);
            }
        }

        return ReturnValues;
    }

    public static ReporteResultadoConciliacion calcularReporteConciliacionManual(List<CartolaBancariaModel> Cartola, List<LibroMayorConciliacion> LibroMayor)
    {
        var LibroMayorVacio = new List<LibroMayorConciliacion>();
        var Reporte = new ReporteResultadoConciliacion();

        var NoContEnElMayor = Cartola.Where(x => x.EstaConciliado == false).ToList();

        var NoRegisCartola = (from LibroM in LibroMayor
                              where !Cartola.Any(x => x.Folio == LibroM.NumDocAsignado)

                              select LibroM).ToList();

        decimal TotalDocumNoContMayor = Math.Abs(NoContEnElMayor.Sum(x => x.Haber)) - Math.Abs(NoContEnElMayor.Sum(x => x.Debe));
        decimal TotalNoRegisCartola = Math.Abs(NoRegisCartola.Sum(x => x.Haber)) - Math.Abs(NoRegisCartola.Sum(x => x.Debe));
        decimal TotalSaldoMayor = Math.Abs(LibroMayor.Sum(x => x.Haber)) - Math.Abs(LibroMayor.Sum(x => x.Debe));
        decimal TotalSaldoCartola = Math.Abs(Cartola.Sum(x => x.Haber)) - Math.Abs(Cartola.Sum(x => x.Debe));
        decimal TotalMayor = Math.Abs(TotalSaldoMayor) - Math.Abs(TotalDocumNoContMayor);
        decimal TotalCartola = TotalSaldoCartola;

        Reporte.LstNoContabilizadosEnElMayor = NoContEnElMayor;
        Reporte.LstNoContabilizadosEnLaCartola = NoRegisCartola;
        Reporte.TotalSaldoNoContabilizadosMayor = TotalDocumNoContMayor;
        Reporte.TotalMayor = TotalMayor;
        Reporte.TotalCartola = TotalCartola;
        Reporte.TotalDocumNoContaCartola = TotalNoRegisCartola;
        Reporte.TotalSaldoCartola = TotalSaldoCartola;
        Reporte.TotalSaldoMayor = TotalSaldoMayor;

        return Reporte;
    }
  

    public static ReporteResultadoConciliacion CalcularReporteConciliacion(List<CartolaBancariaModel> Cartola, List<LibroMayorConciliacion> LibroMayor, FacturaPoliContext db, ClientesContablesModel ObjCliente)
    {
        var LibroMayorVacio = new List<LibroMayorConciliacion>();
        var Reporte = new ReporteResultadoConciliacion();
        var NoContEnElMayor = Cartola.Where(x => x.EstaConciliado == false).ToList();

        decimal TotalDocumNoContMayor = Math.Abs(NoContEnElMayor.Sum(x => x.Haber)) - Math.Abs(NoContEnElMayor.Sum(x => x.Debe));
        decimal TotalSaldoMayor = Math.Abs(LibroMayor.Sum(x => x.Haber)) - Math.Abs(LibroMayor.Sum(x => x.Debe));
        decimal TotalSaldoCartola = Math.Abs(Cartola.Sum(x => x.Haber)) - Math.Abs(Cartola.Sum(x => x.Debe));
        decimal TotalMayor = Math.Abs(TotalSaldoMayor) - Math.Abs(TotalDocumNoContMayor);
        decimal TotalCartola = TotalSaldoCartola;

        Reporte.LstNoContabilizadosEnElMayor = NoContEnElMayor;
        Reporte.LstNoContabilizadosEnLaCartola = LibroMayor;
        Reporte.TotalSaldoNoContabilizadosMayor = TotalDocumNoContMayor;
        Reporte.TotalMayor = TotalMayor;
        Reporte.TotalCartola = TotalCartola;
        Reporte.TotalSaldoCartola = TotalSaldoCartola;
        Reporte.TotalSaldoMayor = TotalSaldoMayor;

        return Reporte;
    }

    public static Tuple<ComparacionConciliacionBancariaViewModel, List<RelacionadosYConciliados>> ConciliarSiSePuede(ComparacionConciliacionBancariaViewModel DatosConciliacion, FacturaPoliContext db, ClientesContablesModel ObjCliente)
    {

        List<RelacionadosYConciliados> Relacionados = (from LibroM in DatosConciliacion.lstLibroMayor
                                                       join CartolaB in DatosConciliacion.lstCartola on LibroM.NumDocAsignado equals CartolaB.Folio
                                                       where LibroM.Haber > 0 && CartolaB.Debe > 0 && Math.Abs(LibroM.Haber) - Math.Abs(CartolaB.Debe) == 0 ||
                                                              CartolaB.Haber > 0 && LibroM.Debe > 0 && Math.Abs(CartolaB.Haber) - Math.Abs(LibroM.Debe) == 0

                                                       select new RelacionadosYConciliados
                                                                  {
                                                                    IdDetalle = LibroM.DetalleVoucherID,
                                                                    IdCartolaDetalle = CartolaB.CartolaBancariaModelId,
                                                                    VoucherId = LibroM.VoucherId
                                                                  }).ToList();


        var ListaConciliada = (from LibroM in DatosConciliacion.lstLibroMayor
                               join CartolaB in DatosConciliacion.lstCartola on LibroM.NumDocAsignado equals CartolaB.Folio
                               where LibroM.Haber > 0 && CartolaB.Debe > 0 && Math.Abs(LibroM.Haber) - Math.Abs(CartolaB.Debe) == 0 ||
                                       CartolaB.Haber > 0 && LibroM.Debe > 0 && Math.Abs(CartolaB.Haber) - Math.Abs(LibroM.Debe) == 0

                               select new Tuple<LibroMayorConciliacion, CartolaBancariaModel>(LibroM,CartolaB)).Select(x => { x.Item1.EstaConciliado = true; x.Item2.EstaConciliado = true; return x; }).ToList();


        return Tuple.Create(DatosConciliacion, Relacionados);
    }
}

public class ComparacionConciliacionBancariaViewModel
{
    public List<LibroMayorConciliacion> lstLibroMayor { get; set; }
    public List<CartolaBancariaModel> lstCartola { get; set; }
    public int IdCuentaContable { get; set; } 
    public int IdCartola { get; set; }
    public List<CartolaBancariaModel> LstNoContabilizados { get; set; }
    public List<LibroMayorConciliacion> LstNoRegistradosCartola { get; set; }
}


public class ReporteResultadoConciliacion
{
    public List<CartolaBancariaModel> LstNoContabilizadosEnElMayor { get; set; }
    public List<LibroMayorConciliacion> LstNoContabilizadosEnLaCartola { get; set; }
    public decimal TotalSaldoMayor { get; set; }
    public decimal TotalSaldoNoContabilizadosMayor { get; set; }
    public decimal TotalMayor { get; set; }
    public decimal TotalSaldoCartola { get; set; }
    public decimal TotalDocumNoContaCartola { get; set; }
    public decimal TotalCartola { get; set; }
}