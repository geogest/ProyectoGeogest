using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;


public class CartolaBancariaMacroModel
{
    public int CartolaBancariaMacroModelID { get; set; }
    public DateTime FechaCartola { get; set; }
    public string NombreCartola { get; set; }
    public virtual CuentaContableModel CuentaContableModelID { get; set; }
    public virtual ClientesContablesModel ClientesContablesModelID { get; set; }
    public virtual List<CartolaBancariaModel> CartolaDetalle { get; set; }


    public static bool GuardarCartolaBancaria(CartolaBancariaMacroModel Cartola, FacturaPoliContext db)
    {
        bool Result = false;
        db.DBCartolaBMacro.Add(Cartola);
        db.SaveChanges();
        Result = true;
        return Result;
    }


    public static List<ObjCartolaYVouchers> ConvertirAObjetoCartola(List<string[]> Cartola/*, string NombreCtaCont*/)
    {
        List<ObjCartolaYVouchers> LstObjCartolaAutomatica = new List<ObjCartolaYVouchers>();

        if(Cartola.Count() > 1)
        {
            Cartola.RemoveAt(0);
            foreach (var itemCartola in Cartola)
            {
                if (itemCartola.All(x => string.IsNullOrWhiteSpace(x)))
                {
                    continue;
                }
                else
                {
                    //Datos Cartola
                    ObjCartolaYVouchers ObjCartolaAutomatica = new ObjCartolaYVouchers();
                    ObjCartolaAutomatica.Fecha = ParseExtensions.ToDD_MM_AAAA_Multi(itemCartola[0]);
                    ObjCartolaAutomatica.Docum = Convert.ToInt32(itemCartola[1]);
                    ObjCartolaAutomatica.Detalle = itemCartola[2];
                    ObjCartolaAutomatica.Debe = Convert.ToDecimal(itemCartola[3]);
                    ObjCartolaAutomatica.Haber = Convert.ToDecimal(itemCartola[4]);
                    ObjCartolaAutomatica.Saldo = Convert.ToDecimal(itemCartola[5]);

                    //Datos Voucher
                    ObjCartolaAutomatica.CodigoInterno = itemCartola[6];
                    ObjCartolaAutomatica.Rut = itemCartola[7];
                    ObjCartolaAutomatica.Glosa = itemCartola[8];

                    LstObjCartolaAutomatica.Add(ObjCartolaAutomatica);
                }

            }
        }


        return LstObjCartolaAutomatica;
    }

    public static bool ConvertirAVoucher(List<ObjCartolaYVouchers> LstCartolaYVouchers, ClientesContablesModel ObjCliente,FacturaPoliContext db,CuentaContableModel CuentaConsultada)
    {

        bool Result = false;
        if(LstCartolaYVouchers.Count() > 0)
        {
                
                //List<VoucherModel> LstNuevosVouchers = new List<VoucherModel>();
                List<DetalleVoucherModel> debug = new List<DetalleVoucherModel>();
        
        
                foreach (var itemCartola in LstCartolaYVouchers)
                {
                    //El voucher se compone por:
                    //1.-Capa Voucher Unitario
                    //2.-Capa Detalle Voucher Plural
                    //3.-Capa Auxiliar Plural
                    //4- Capa Detalle Auxiliar Plural

                    //Notas
                    // 1 voucher por iteración
                    // 2 detalles del voucher por voucher
                    // 1 auxiliar y auxiliar detalle por iteración

                    //Armamos primero la tabla del voucher

                    CuentaContableModel CuentaAUsar = UtilesContabilidad.CuentaContableDesdeCodInterno(itemCartola.CodigoInterno, ObjCliente);
                    int? nullableProxVoucherNumber = ParseExtensions.ObtenerNumeroProximoVoucherINT(ObjCliente, db);
                    int baseNumberFolio = nullableProxVoucherNumber.Value;

                    VoucherModel CapaVoucher = new VoucherModel();

                    CapaVoucher.FechaEmision = itemCartola.Fecha;
                    CapaVoucher.NumeroVoucher = baseNumberFolio;
                    CapaVoucher.ClientesContablesModelID = ObjCliente.ClientesContablesModelID;
                    if (itemCartola.Debe > 0 && itemCartola.Haber == 0)
                        CapaVoucher.Tipo = TipoVoucher.Egreso;
                    else if (itemCartola.Haber > 0 && itemCartola.Debe == 0)
                        CapaVoucher.Tipo = TipoVoucher.Ingreso;
                    CapaVoucher.Glosa = itemCartola.Detalle;



                    //Armamos tabla Detalle Voucher
                    List<DetalleVoucherModel> LstDetalle = new List<DetalleVoucherModel>();
                    //1
                    DetalleVoucherModel DetalleCartola = new DetalleVoucherModel(); //cada linea es solo 1 monto
                    DetalleCartola.VoucherModelID = CapaVoucher.VoucherModelID;
                    DetalleCartola.ObjCuentaContable = CuentaConsultada;
                    DetalleCartola.FechaDoc = itemCartola.Fecha;
                    DetalleCartola.GlosaDetalle = itemCartola.Detalle;
                    if (itemCartola.Debe > 0 && itemCartola.Haber == 0)
                    {
                        DetalleCartola.MontoDebe = itemCartola.Debe;
                    }

                    else if (itemCartola.Haber > 0 && itemCartola.Debe == 0)
                    {
                        DetalleCartola.MontoHaber = itemCartola.Haber;
                    }


                    //2
                    DetalleVoucherModel DetalleConciliacion = new DetalleVoucherModel();
                    DetalleConciliacion.VoucherModelID = CapaVoucher.VoucherModelID;
                    DetalleConciliacion.FechaDoc = itemCartola.Fecha;
                    DetalleConciliacion.ObjCuentaContable = CuentaAUsar;
                    DetalleConciliacion.GlosaDetalle = itemCartola.Glosa;
                    if (DetalleCartola.MontoDebe > 0 && DetalleCartola.MontoHaber == 0)
                    {
                        DetalleConciliacion.MontoHaber = DetalleCartola.MontoDebe;
                    }
                    else if (DetalleCartola.MontoHaber > 0 && DetalleCartola.MontoDebe == 0)
                    {
                        DetalleConciliacion.MontoDebe = DetalleCartola.MontoHaber;
                    }

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
                        }

                        db.DBDetalleVoucher.AddRange(LstDetalle);
                        db.SaveChanges();

                        debug.AddRange(LstDetalle);


                    foreach (DetalleVoucherModel NuevoDetalleVoucher in LstDetalle)
                    {
                        //CREO NUEVO DOCUMENTO AUXILIAR 
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
                            nuevoAuxDetalle.TipoDocumento = TipoDte.BoletaDeBanco;
                            nuevoAuxDetalle.Fecha = itemCartola.Fecha;
                            nuevoAuxDetalle.FechaContabilizacion = itemCartola.Fecha;

                            //revisar
                            // nuevoAuxDetalle.FechaVencimiento =   entradaLibro.fe
                            nuevoAuxDetalle.Folio = itemCartola.Docum;
                            //nuevoAuxDetalle.Individuo2 = db.Receptores.FirstOrDefault(x => x.RUT == "76011284-4" && x.ClientesContablesModelID == x.ClientesContablesModelID);
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

       
        return Result;
    }
}


public class ObjCartolaMacro
{
    public string NombreCartola { get; set; }
    public string FechaCartola { get; set; }
    public IEnumerable<HttpPostedFileBase> files { get; set; }
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
