
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data.Entity.Migrations;
using SpreadsheetLight;

public class CartolaBancariaMacroModel
{
    public int CartolaBancariaMacroModelID { get; set; }
    public DateTime FechaCartola { get; set; }
    public int NumeroCartola { get; set; }
    public decimal TotalCartola { get; set; }
    public  CuentaContableModel CuentaContableModelID { get; set; }
    public  ClientesContablesModel ClientesContablesModelID { get; set; }
    public  List<CartolaBancariaModel> CartolaDetalle { get; set; }



    public static bool GuardarCartolaBancaria(List<CartolaBancariaModel> LstCartolaConInfo, List<CartolaBancariaModel> LstCartolaSinInfo, string FechaCartola,int NumeroCartola, CuentaContableModel CuentaConsultada, ClientesContablesModel ObjCliente, FacturaPoliContext db)
    {
        bool Result = false;

        List<CartolaBancariaModel> CartolaBancariaCompleta = new List<CartolaBancariaModel>();
        CartolaBancariaCompleta.AddRange(LstCartolaConInfo);
        CartolaBancariaCompleta.AddRange(LstCartolaSinInfo);

        CartolaBancariaMacroModel CartolaBancariaMacro = new CartolaBancariaMacroModel();
        CartolaBancariaMacro.FechaCartola = ParseExtensions.ToDD_MM_AAAA_Multi(FechaCartola);
        CartolaBancariaMacro.ClientesContablesModelID = ObjCliente;
        CartolaBancariaMacro.CuentaContableModelID = CuentaConsultada;
        CartolaBancariaMacro.NumeroCartola = NumeroCartola;
        CartolaBancariaMacro.CartolaDetalle = CartolaBancariaCompleta;

        db.DBCartolaBMacro.Add(CartolaBancariaMacro);
        db.SaveChanges();
        Result = true;

        return Result;
    }
        
    public static bool ActualizarEstadosConciliacion(FacturaPoliContext db, ClientesContablesModel ObjCliente, List<RelacionadosYConciliados> DatosConciliacion, CuentaContableModel CuentaConsultada, int IdCartola)
    {
        //Se actualiza la tabla DetalleVoucher && CartolaBancaria.
        bool Result = false;
        List<int> IdsDetalleAactualizar = DatosConciliacion.Select(x => x.IdDetalle).ToList();

        List<DetalleVoucherModel> DetalleEncontrados = db.DBDetalleVoucher.Where(x => IdsDetalleAactualizar.Contains(x.DetalleVoucherModelID)).ToList();
        var ActualizaEncontrados = DetalleEncontrados.Select(x =>
                                                             {
                                                                 x.Conciliado = true;
                                                                 return x;
                                                             }).ToList();

        db.DBDetalleVoucher.AddOrUpdate(ActualizaEncontrados.ToArray());

        CartolaBancariaMacroModel CartolaAactualizar = db.DBCartolaBMacro.Include("CartolaDetalle").SingleOrDefault(x => x.CartolaBancariaMacroModelID == IdCartola);
                                                              
        foreach (CartolaBancariaModel itemCArtola in CartolaAactualizar.CartolaDetalle)
        {
            RelacionadosYConciliados RelacionCartolaYLibroMayor = new RelacionadosYConciliados();
            RelacionCartolaYLibroMayor = DatosConciliacion.SingleOrDefault(x => x.IdCartolaDetalle == itemCArtola.CartolaBancariaModelId);

            if(RelacionCartolaYLibroMayor != null) { 
                itemCArtola.VoucherModelID = RelacionCartolaYLibroMayor.VoucherId;
                itemCArtola.CuentaContableModelID = CuentaConsultada;
                itemCArtola.EstaConciliado = true;
                itemCArtola.ClientesContablesModelID = ObjCliente;
            }
        }

        db.DBCartolaBMacro.AddOrUpdate(CartolaAactualizar);
        db.SaveChanges();
        Result = true;

        return Result;
    }

    public static bool ActualizarCartolaMacroConciliacion(FacturaPoliContext db, ClientesContablesModel ObjCliente, int Id, int IdCuentaContable, int IdVoucher)
    {
        bool Result = false;




        return Result;
    }

    public static bool GuardarCartolaBancariaManual(List<CartolaBancariaModel> LstCartolaAGuardar, string FechaCartola, int NumeroCartola, ClientesContablesModel ObjCliente, FacturaPoliContext db)
    {
        bool Result = false;

        decimal TotalCartola = Math.Abs(LstCartolaAGuardar.Sum(x => x.Haber) - LstCartolaAGuardar.Sum(x => x.Debe));

        CartolaBancariaMacroModel CartolaBancariaMacro = new CartolaBancariaMacroModel();
        CartolaBancariaMacro.FechaCartola = ParseExtensions.ToDD_MM_AAAA_Multi(FechaCartola);
        CartolaBancariaMacro.ClientesContablesModelID = ObjCliente;
        CartolaBancariaMacro.NumeroCartola = NumeroCartola;
        CartolaBancariaMacro.TotalCartola = TotalCartola;
        CartolaBancariaMacro.CartolaDetalle = LstCartolaAGuardar;

        db.DBCartolaBMacro.Add(CartolaBancariaMacro);
        db.SaveChanges();
        Result = true;

        return Result;
    }
    public static List<CartolaBancariaModel> ObtenerCartolaParaResultadoConciliacion(List<CartolaBancariaModel> NoEstanEnElMayor, List<CartolaBancariaModel> Conciliados)
    {
        List<CartolaBancariaModel> CartolaCompleta = new List<CartolaBancariaModel>();
        CartolaCompleta.AddRange(NoEstanEnElMayor);
        CartolaCompleta.AddRange(Conciliados);

        return CartolaCompleta;
    }


    public static decimal CalcularSaldosCartola()
    {

        return 0;
    } 
    public static bool GuardarCartolaBancaria(CartolaBancariaMacroModel Cartola, FacturaPoliContext db)
    {
        bool Result = false;

        return Result;
    }


    //public static List<ObjCartolaYVouchers> ConvertirAObjetoCartola(List<string[]> Cartola/*, string NombreCtaCont*/)
    //{
    //    List<ObjCartolaYVouchers> LstObjCartolaAutomatica = new List<ObjCartolaYVouchers>();

    //    if(Cartola.Count() > 1)
    //    {
    //        Cartola.RemoveAt(0);
    //        foreach (var itemCartola in Cartola)
    //        {
    //            if (itemCartola.All(x => string.IsNullOrWhiteSpace(x)))
    //            {
    //                continue;
    //            }
    //            else
    //            {
    //                //Datos Cartola
    //                ObjCartolaYVouchers ObjCartolaAutomatica = new ObjCartolaYVouchers();
    //                ObjCartolaAutomatica.Fecha = ParseExtensions.ToDD_MM_AAAA_Multi(itemCartola[0]);
    //                ObjCartolaAutomatica.Docum = Convert.ToInt32(itemCartola[1]);
    //                ObjCartolaAutomatica.Detalle = itemCartola[2];
    //                ObjCartolaAutomatica.Debe = Convert.ToDecimal(itemCartola[3]);
    //                ObjCartolaAutomatica.Haber = Convert.ToDecimal(itemCartola[4]);
    //                ObjCartolaAutomatica.Saldo = Convert.ToDecimal(itemCartola[5]);

    //                //Datos Voucher
    //                ObjCartolaAutomatica.CodigoInterno = itemCartola[6];
    //                ObjCartolaAutomatica.Rut = itemCartola[7];
    //                ObjCartolaAutomatica.Glosa = itemCartola[8];

    //                LstObjCartolaAutomatica.Add(ObjCartolaAutomatica);
    //            }

    //        }
    //    }


    //    return LstObjCartolaAutomatica;
    //}

    public static CartolaBancariaMacroModel GetCartolaById(FacturaPoliContext db, ClientesContablesModel ObjCliente, int id)
    {
        CartolaBancariaMacroModel ReturnValues = new CartolaBancariaMacroModel();

        ReturnValues = db.DBCartolaBMacro.Include("CartolaDetalle").Where(x => x.ClientesContablesModelID.ClientesContablesModelID == ObjCliente.ClientesContablesModelID).FirstOrDefault();

        return ReturnValues;
    }
    public static List<CartolaBancariaMacroModel> GetListaCartola(FacturaPoliContext db, ClientesContablesModel ObjCliente)
    {
        var LstCartola = new List<CartolaBancariaMacroModel>();

        LstCartola = db.DBCartolaBMacro.Include("CuentaContableModelID").Where(x => x.ClientesContablesModelID.ClientesContablesModelID == ObjCliente.ClientesContablesModelID).ToList();

        return LstCartola;
    }

    public static List<CartolaBancariaPuraModel> DeExcelAObjetoCartolaYVoucher(HttpPostedFileBase file)
    {
        List<CartolaBancariaPuraModel> ReturnValues = new List<CartolaBancariaPuraModel>();
        SLDocument Excel = new SLDocument(file.InputStream);

        int row = 2;
        while (!string.IsNullOrEmpty(Excel.GetCellValueAsString(row, 1)))
        {
            DateTime Fecha = Excel.GetCellValueAsDateTime(row, 1);
            int Docum = Excel.GetCellValueAsInt32(row,2);
            string Detalle = Excel.GetCellValueAsString(row, 3);
            decimal Debe = Excel.GetCellValueAsDecimal(row, 4);
            decimal Haber = Excel.GetCellValueAsDecimal(row,5);
            decimal Saldo = Excel.GetCellValueAsDecimal(row, 6);

            CartolaBancariaPuraModel FilaAGuardar = new CartolaBancariaPuraModel() 
            { 
                Fecha = Fecha,
                Docum = Docum,
                Detalle = Detalle,
                Debe = Debe,
                Haber = Haber,
                Saldo = Saldo,
            };

            ReturnValues.Add(FilaAGuardar);

            row++;
        }


        return ReturnValues;
    }

    public static List<ObjCartolaYVouchers> ConvertirAObjetoCartola(HttpPostedFileBase file)
    {
        List<ObjCartolaYVouchers> ReturnValues = new List<ObjCartolaYVouchers>();
        SLDocument Excel = new SLDocument(file.InputStream);

        int row = 2;
        while (!string.IsNullOrEmpty(Excel.GetCellValueAsString(row, 1)))
        {
            DateTime Fecha = Excel.GetCellValueAsDateTime(row, 1);
            int Docum = Excel.GetCellValueAsInt32(row, 2);
            string Detalle = Excel.GetCellValueAsString(row, 3);
            decimal Debe = Excel.GetCellValueAsDecimal(row, 4);
            decimal Haber = Excel.GetCellValueAsDecimal(row, 5);
            decimal Saldo = Excel.GetCellValueAsDecimal(row, 6);
            string CodigoInterno = Excel.GetCellValueAsString(row, 7);
            string Rut = Excel.GetCellValueAsString(row, 8);
            string Glosa = Excel.GetCellValueAsString(row, 9);

            ObjCartolaYVouchers FilaAGuardar = new ObjCartolaYVouchers()
            {
                Fecha = Fecha,
                Docum = Docum,
                Detalle = Detalle,
                Debe = Debe,
                Haber = Haber,
                Saldo = Saldo,
                CodigoInterno = CodigoInterno,
                Rut = Rut,
                Glosa = Glosa 
            };

            ReturnValues.Add(FilaAGuardar);
            row++;
        }

        return ReturnValues;
    }

    //Queda pendiente crear este mismo metodo pero generico para reutilizar.
    //public static List<ObjCartolaYVouchers> ConvertirAObjetoCartola(HttpPostedFileBase file)
    //{
    //    List<ObjCartolaYVouchers> ReturnValues = new List<ObjCartolaYVouchers>();

    //    if (file == null || file.ContentLength == 0)
    //    {
    //        string Error = "Error Excel Vacio";
    //    }
    //    else
    //    {
    //        if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
    //        {
    //            string path = ParseExtensions.Get_Temp_path(file.FileName); // Le indicamos la ruta donde guardará el excel.

    //            if (File.Exists(path))
    //            {
    //                File.Delete(path); //Si ya existe lo elimina.
    //            }
    //            file.SaveAs(path); //Guardamos momentaneamente el fichero. -> La idea es extraer su información y luego eliminarlo.

    //            Application application = new Application();
    //            Workbook workBook = application.Workbooks.Open(path);
    //            Worksheet worksheet = workBook.ActiveSheet;
    //            Range range = worksheet.UsedRange;

    //            for (int row = 2; row <= range.Rows.Count; row++)
    //            {
    //                ObjCartolaYVouchers FilaAGuardar = new ObjCartolaYVouchers();

    //                FilaAGuardar.Fecha = ParseExtensions.ToDD_MM_AAAA_Multi(((Range)range.Cells[row, 1]).Text);
    //                FilaAGuardar.Docum = Convert.ToInt32(((Range)range.Cells[row, 2]).Text);
    //                FilaAGuardar.Detalle = ((Range)range.Cells[row, 3]).Text;
    //                FilaAGuardar.Debe = decimal.Parse(((Range)range.Cells[row, 4]).Text);
    //                FilaAGuardar.Haber = decimal.Parse(((Range)range.Cells[row, 5]).Text);
    //                FilaAGuardar.Saldo = decimal.Parse(((Range)range.Cells[row, 6]).Text);
    //                //Parte del voucher
    //                FilaAGuardar.CodigoInterno = ((Range)range.Cells[row, 7]).Text;
    //                FilaAGuardar.Rut = ((Range)range.Cells[row, 8]).Text;
    //                FilaAGuardar.Glosa = ((Range)range.Cells[row, 9]).Text;

    //                ReturnValues.Add(FilaAGuardar);
    //            }
    //            workBook.Close();
    //            File.Delete(path); 
    //        }
    //    }

    //    return ReturnValues;
    //}

    public static Tuple<bool, List<CartolaBancariaModel>> ConvertirAVoucher(List<ObjCartolaYVouchers> LstCartolaYVouchers, ClientesContablesModel ObjCliente,FacturaPoliContext db,CuentaContableModel CuentaConsultada, string FechaCartola, int NumeroCartola)
    {
        
        bool Result = false;
        List<CartolaBancariaModel> LosQueNoEstanEnElMayor = new List<CartolaBancariaModel>();

        List<ObjCartolaYVouchers> LosQueTienenInformacion = LstCartolaYVouchers.Where(x => !string.IsNullOrWhiteSpace(x.CodigoInterno)).ToList();
        //Los que están en la cartola pero no en el mayor...
        List<ObjCartolaYVouchers> Pendientes = LstCartolaYVouchers.Where(x => string.IsNullOrWhiteSpace(x.CodigoInterno)).ToList();

        LosQueNoEstanEnElMayor = Pendientes.Select(x => new CartolaBancariaModel
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

        List<CartolaBancariaModel> CartolaCompleta = new List<CartolaBancariaModel>();
        

        if (LstCartolaYVouchers.Count() > 0)
        {
                DateTime FechaConvertida = ParseExtensions.ToDD_MM_AAAA_Multi(FechaCartola);
                SiExisteReemplazala(FechaConvertida, NumeroCartola,db, ObjCliente);
                List<CartolaBancariaModel> CartolaDetalle = new List<CartolaBancariaModel>();
                foreach (var itemCartola in LosQueTienenInformacion)
                {
                    CuentaContableModel CuentaAUsar = UtilesContabilidad.CuentaContableDesdeCodInterno(itemCartola.CodigoInterno, ObjCliente);
                    var Prestador = UtilesContabilidad.ObtenerPrestadorSiExiste(itemCartola.Rut, db, ObjCliente);
                    var TipoPrestador = UtilesContabilidad.RetornaTipoReceptor(Prestador);

                    if(CuentaAUsar.TieneAuxiliar == 1 && Prestador == null)
                    {
                        CartolaBancariaModel LineaConAuxiliarNoValida = new CartolaBancariaModel()
                        {
                            VoucherModelID = 0,
                            Fecha = itemCartola.Fecha,
                            ClientesContablesModelID = ObjCliente,
                            CuentaContableModelID = null,
                            Folio = itemCartola.Docum,
                            Detalle = itemCartola.Detalle,
                            Oficina = "",
                            Debe = itemCartola.Debe,
                            Haber = itemCartola.Haber,
                            Saldo = itemCartola.Saldo,
                            EstaConciliado = false
                        };

                        LosQueNoEstanEnElMayor.Add(LineaConAuxiliarNoValida);
                    }

                    if (CuentaAUsar.TieneAuxiliar == 1 && Prestador != null || CuentaAUsar.TieneAuxiliar == 0 && Prestador == null)
                    {
                        int? nullableProxVoucherNumber = ParseExtensions.ObtenerNumeroProximoVoucherINT(ObjCliente, db);
                        int baseNumberFolio = nullableProxVoucherNumber.Value;

                        VoucherModel CapaVoucher = new VoucherModel();

                        CapaVoucher.TipoOrigenVoucher = TipoPrestador;

                        CapaVoucher.FechaEmision = itemCartola.Fecha;
                        CapaVoucher.NumeroVoucher = baseNumberFolio;
                        CapaVoucher.ClientesContablesModelID = ObjCliente.ClientesContablesModelID;
                        CapaVoucher.Glosa = itemCartola.Glosa;

                        if (itemCartola.Debe > 0 && itemCartola.Haber == 0)
                            CapaVoucher.Tipo = TipoVoucher.Ingreso;
                        else if (itemCartola.Haber > 0 && itemCartola.Debe == 0)
                            CapaVoucher.Tipo = TipoVoucher.Egreso;

                        //Armamos tabla Detalle Voucher
                        List<DetalleVoucherModel> LstDetalle = new List<DetalleVoucherModel>();
                        //1
                        DetalleVoucherModel DetalleCartola = new DetalleVoucherModel(); //cada linea es solo 1 monto
                        DetalleCartola.VoucherModelID = CapaVoucher.VoucherModelID;
                        DetalleCartola.ObjCuentaContable = CuentaConsultada;
                        DetalleCartola.FechaDoc = itemCartola.Fecha;
                        DetalleCartola.GlosaDetalle = itemCartola.Glosa;

                        if (itemCartola.Debe > 0 && itemCartola.Haber == 0)
                            DetalleCartola.MontoDebe = itemCartola.Debe;
                        else if (itemCartola.Haber > 0 && itemCartola.Debe == 0)
                            DetalleCartola.MontoHaber = itemCartola.Haber;

                        //2
                        DetalleVoucherModel DetalleConciliacion = new DetalleVoucherModel();
                        DetalleConciliacion.VoucherModelID = CapaVoucher.VoucherModelID;
                        DetalleConciliacion.FechaDoc = itemCartola.Fecha;
                        DetalleConciliacion.ObjCuentaContable = CuentaAUsar;
                        DetalleConciliacion.GlosaDetalle = itemCartola.Glosa;
                        if (DetalleCartola.MontoDebe > 0 && DetalleCartola.MontoHaber == 0)
                            DetalleConciliacion.MontoHaber = DetalleCartola.MontoDebe;
                        else if (DetalleCartola.MontoHaber > 0 && DetalleCartola.MontoDebe == 0)
                            DetalleConciliacion.MontoDebe = DetalleCartola.MontoHaber;

                        LstDetalle.Add(DetalleCartola);
                        LstDetalle.Add(DetalleConciliacion);

                        //Guardamos los detalles en una lista de detalles
                        if (LstDetalle.Sum(r => r.MontoDebe) == LstDetalle.Sum(r => r.MontoHaber))
                        {
                            db.DBVoucher.Add(CapaVoucher);
                            db.SaveChanges();

                            foreach (var itemDetalle in LstDetalle)
                            {
                                itemDetalle.VoucherModelID = CapaVoucher.VoucherModelID;
                                itemDetalle.Conciliado = true;
                            }

                            db.DBDetalleVoucher.AddRange(LstDetalle);
                            db.SaveChanges();

                            CartolaBancariaModel LineaCartola = new CartolaBancariaModel();
                            LineaCartola.VoucherModelID = DetalleCartola.VoucherModelID;
                            LineaCartola.Fecha = DetalleCartola.FechaDoc;
                            LineaCartola.Folio = itemCartola.Docum;
                            LineaCartola.EstaConciliado = true;
                            LineaCartola.Detalle = DetalleCartola.GlosaDetalle;
                            LineaCartola.CuentaContableModelID = DetalleCartola.ObjCuentaContable;
                            LineaCartola.ClientesContablesModelID = ObjCliente;
                            LineaCartola.Debe = DetalleCartola.MontoDebe;
                            LineaCartola.Haber = DetalleCartola.MontoHaber;
                            CartolaDetalle.Add(LineaCartola);


                            if (CuentaAUsar.TieneAuxiliar == 1 && Prestador != null)
                            {
                                foreach (DetalleVoucherModel NuevoDetalleVoucher in LstDetalle)
                                {
                                    if (NuevoDetalleVoucher.ObjCuentaContable == CuentaAUsar)
                                    {
                                        AuxiliaresModel Auxiliar = new AuxiliaresModel();
                                        Auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                                        Auxiliar.LineaNumeroDetalle = CapaVoucher.ListaDetalleVoucher.Count;
                                        Auxiliar.MontoTotal = NuevoDetalleVoucher.MontoDebe + NuevoDetalleVoucher.MontoHaber;
                                        Auxiliar.objCtaContable = NuevoDetalleVoucher.ObjCuentaContable;

                                        NuevoDetalleVoucher.Auxiliar = Auxiliar;
                                        db.DBAuxiliares.Add(Auxiliar);
                                        db.SaveChanges();

                                        AuxiliaresDetalleModel nuevoAuxDetalle = new AuxiliaresDetalleModel();
                                        nuevoAuxDetalle.TipoDocumento = TipoDte.FacturaElectronica;
                                        nuevoAuxDetalle.Fecha = itemCartola.Fecha;
                                        nuevoAuxDetalle.FechaContabilizacion = itemCartola.Fecha;
                                        nuevoAuxDetalle.Folio = itemCartola.Docum;
                                        nuevoAuxDetalle.Individuo2 = Prestador;
                                        nuevoAuxDetalle.MontoNetoLinea = 0;
                                        nuevoAuxDetalle.MontoExentoLinea = 0;
                                        nuevoAuxDetalle.MontoIVALinea = 0;
                                        nuevoAuxDetalle.MontoTotalLinea = NuevoDetalleVoucher.MontoDebe + NuevoDetalleVoucher.MontoHaber;
                                        nuevoAuxDetalle.AuxiliaresModelID = Auxiliar.AuxiliaresModelID;
                                        nuevoAuxDetalle.MontoIVANoRecuperable = 0;
                                        nuevoAuxDetalle.MontoIVAUsoComun = 0;
                                        nuevoAuxDetalle.MontoIVAActivoFijo = 0;

                                        db.DBAuxiliaresDetalle.Add(nuevoAuxDetalle);
                                        db.SaveChanges();
                                    }
                                }
                            }

                            baseNumberFolio++;
                        }
                    }



                }
                //Se inserta toda la cartola bancaria para tener respaldo.
                CartolaCompleta = ObtenerCartolaParaResultadoConciliacion(LosQueNoEstanEnElMayor, CartolaDetalle);
                var ResultadoInsercionCartolaBancaria = GuardarCartolaBancaria(CartolaDetalle, LosQueNoEstanEnElMayor, FechaCartola, NumeroCartola, CuentaConsultada, ObjCliente, db);
                Result = true;  
        }
        return Tuple.Create(Result, CartolaCompleta);
    }

    public static object ProcesarResultadoConciliacion(List<ObjCartolaYVouchers> Cartola)
    {
        object none = new object();
        //Cuando se importan los datos de la conciliación -> solo se crearan los vouchers que tengan contra cuenta los demás datos quedan incompletos
        //                                                   por ende se van a los que están en la cartola que no están en el mayor
        //El resultado de la conciliación siempre tirará que hay problemas en el libro mayor por que los vouchers que se crearan serán solo de los que haya información
        //Entonces la cartola siempre tendrá los datos y el mayor no a no ser que se ingrese toda la cartola
        //Es necesario hacer una consulta nueva al mayor para poder comparar los que no están en el mayor pero si en la cartola



        //Requerimientos:
        //1.-Datos del banco libro mayor -> Ya está
        //2.-Datos de la cartola
        //Se comparan ambos
        //Se obtiene el resultado

        return none;
    }

    public static void SiExisteReemplazala(DateTime FechaCartola, int NumeroCartola, FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        CartolaBancariaMacroModel CartolaEncontrada = db.DBCartolaBMacro.Include("CartolaDetalle").Where(x => x.ClientesContablesModelID.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                x.FechaCartola.Month == FechaCartola.Month &&
                                                                x.FechaCartola.Year == FechaCartola.Year &&
                                                                x.NumeroCartola == NumeroCartola).FirstOrDefault();
        List<int> lstVouchersId = new List<int>();
        if(CartolaEncontrada != null)
            lstVouchersId = CartolaEncontrada.CartolaDetalle.Select(x => x.VoucherModelID).ToList();

        List<VoucherModel> VouchersADarDeBaja = new List<VoucherModel>();
        if (lstVouchersId.Any())
            VouchersADarDeBaja = db.DBVoucher.Where(x => lstVouchersId.Contains(x.VoucherModelID)).ToList();

        if (VouchersADarDeBaja.Any())
        {
            VouchersADarDeBaja = VouchersADarDeBaja.Select(x => { x.DadoDeBaja = true; return x; }).ToList();
            db.DBVoucher.AddOrUpdate(VouchersADarDeBaja.ToArray());
        }

        if (CartolaEncontrada != null)
            db.DBCartolaBMacro.Remove(CartolaEncontrada);

        db.SaveChanges();
    }

    //Crear objeto voucher
    //public static bool ValidarRedundanciaVoucher()
    //{

    //}


    public static bool ExistenRepetidos(DateTime FechaCartola, int NumeroCartola, FacturaPoliContext db, ClientesContablesModel  objCliente)
    {
        //Numero de la cartola es igual a uno existente en la db.
        //Cuando pertenecen al mismo mes y año.
        bool Result = false;
        var lstCartolaMacroModel = db.DBCartolaBMacro.Where(x => x.ClientesContablesModelID.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                 x.FechaCartola.Month == FechaCartola.Month &&
                                                                 x.FechaCartola.Year == FechaCartola.Year &&
                                                                 x.NumeroCartola == NumeroCartola).ToList();

        if(lstCartolaMacroModel.Count() > 0)
        {
            Result = true;  
        }

        return Result;
    }
}



public class ObjCartolaMacro
{
    [Required]
    public int NumeroCartola { get; set; }
    [Required]
    public string FechaCartola { get; set; }
    [Required]
    public HttpPostedFileBase files { get; set; }
}

public class DatosProcesoConciliacion
{
    [Required]
    public int IdCuentaContable { get; set; }
    [Required]
    public int IdCartola { get; set; }
    [Required]
    public int Anio { get; set; }
    [Required]
    public int Mes { get; set; }
}

public class CartolaBancariaPuraModel
{
    public DateTime Fecha { get; set; }
    public int Docum { get; set; }
    public string Detalle { get; set; }
    public decimal Debe { get; set; }
    public decimal Haber { get; set; }
    public decimal Saldo { get; set; }

}

public class RelacionadosYConciliados
{
    public int IdDetalle { get; set; }
    public int VoucherId { get; set; }
    public int IdCartolaDetalle { get; set; }

}


    public class ObjCartolaYVouchers
    {
        public DateTime Fecha { get; set; }
        public int Docum { get; set; }
        public string Detalle { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Saldo { get; set; }

        //Parte del voucher

        public string CodigoInterno { get; set; }
        public string Rut { get; set; }
        public string Glosa { get; set; }
    }
