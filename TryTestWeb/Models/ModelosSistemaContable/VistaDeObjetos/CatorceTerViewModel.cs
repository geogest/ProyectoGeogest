using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class CatorceTerViewModel
    {
        public DateTime Fecha { get; set; }
        public TipoDte TipoDocumento { get; set; }
        public int Folio { get; set; }
        public string NombreReceptor { get; set; }
        public string RutReceptor { get; set; }
        public decimal Ingreso { get; set; }
        public decimal Egreso { get; set; }
        public string Glosa { get; set; }
        public string TipoLibro { get; set; }



        public static List<CatorceTerViewModel> GetCatorceTer(FacturaPoliContext db, ClientesContablesModel ObjCliente)
        {
            List<CatorceTerViewModel> lstCatorceTer = new List<CatorceTerViewModel>();

            string TipoReceptorCompra = "PR";
            string TipoReceptorVenta = "CL";
        //string TipoReceptorHonorarios = "H";

            var TablaPrestador = (from Voucher in db.DBVoucher
                                  join DetalleVoucher in db.DBDetalleVoucher on Voucher.VoucherModelID equals DetalleVoucher.VoucherModelID
                                  join Auxiliares in db.DBAuxiliares on DetalleVoucher.DetalleVoucherModelID equals Auxiliares.DetalleVoucherModelID
                                  join AuxiliaresDetalle in db.DBAuxiliaresDetalle on Auxiliares.AuxiliaresModelID equals AuxiliaresDetalle.AuxiliaresModelID

                                  where Voucher.DadoDeBaja == false &&
                                  Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                                  Voucher.Tipo == TipoVoucher.Ingreso &&
                                  AuxiliaresDetalle.Individuo2.tipoReceptor == TipoReceptorVenta ||
                                  Voucher.DadoDeBaja == false &&
                                  Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                                  Voucher.Tipo == TipoVoucher.Egreso &&
                                  AuxiliaresDetalle.Individuo2.tipoReceptor == TipoReceptorCompra
            

                                  select new {
                                      Haber = DetalleVoucher.MontoHaber,
                                      Debe = DetalleVoucher.MontoDebe,
                                      FechaContabilizacion = DetalleVoucher.FechaDoc,
                                      PrestadorNombre = AuxiliaresDetalle.Individuo2.RazonSocial,
                                      PrestadorRut = AuxiliaresDetalle.Individuo2.RUT,
                                      TipoPrestador = AuxiliaresDetalle.Individuo2.tipoReceptor,
                                      TipoDoc = AuxiliaresDetalle.TipoDocumento,
                                      Folio = AuxiliaresDetalle.Folio,
                                      Glosa = DetalleVoucher.GlosaDetalle
                                   });

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
                
                if(itemCatorceTer.TipoPrestador == TipoReceptorCompra)
                {
                    objTer.Egreso = TotalDebeHaber;
                    objTer.TipoLibro = "Compra";
                }else if (itemCatorceTer.TipoPrestador == TipoReceptorVenta)
                {
                    objTer.Ingreso = TotalDebeHaber;
                    objTer.TipoLibro = "Venta";
                }

                objTer.Glosa = itemCatorceTer.Glosa;

                lstCatorceTer.Add(objTer);
                }

            return lstCatorceTer;
        }

        //public static List<CatorceTerViewModel> GetCatorceTerDesdeVoucher(FacturaPoliContext db, ClientesContablesModel ObjCliente)
        //{
        //    IQueryable<VoucherModel> QueryIandE = db.DBVoucher.Where(x => x.)
        //    return null;
        //}

        public static byte[] GetExcelCatorceTer()
        {
             return null;
        } 
        
    }
