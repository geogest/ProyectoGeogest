using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class UsoComunAux
    {
        public static List<CuentaContableModel> LstAuxConMovimiento(FacturaPoliContext db, ClientesContablesModel objCliente)
        {
               IQueryable<CuentaContableModel> CtasContMovimiento = (from Detalle in db.DBDetalleVoucher
                                                                    join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                                                    join Auxiliar in db.DBAuxiliares on Detalle.Auxiliar.AuxiliaresModelID equals Auxiliar.AuxiliaresModelID
                                                                    where Auxiliar.objCtaContable.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                            Voucher.DadoDeBaja == false &&
                                                                            Auxiliar.objCtaContable.TieneAuxiliar == 1
                                                                    select Detalle.ObjCuentaContable ).Distinct();

            return CtasContMovimiento.ToList();
        }

        public static object LstAuxConMovimientoTwo(FacturaPoliContext db, ClientesContablesModel objCliente)
        {
            object CtasConMovimiento = new object();

             CtasConMovimiento = (from Detalle in db.DBDetalleVoucher
                                  join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                  join Auxiliar in db.DBAuxiliares on Detalle.Auxiliar.AuxiliaresModelID equals Auxiliar.AuxiliaresModelID
                                  where Auxiliar.objCtaContable.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                        Voucher.DadoDeBaja == false &&
                                        Auxiliar.objCtaContable.TieneAuxiliar == 1
                                  select new
                                  {
                                      Id = Detalle.ObjCuentaContable.CuentaContableModelID,
                                      CodigoInterno = Detalle.ObjCuentaContable.CodInterno,
                                      Nombre = Detalle.ObjCuentaContable.nombre
                                  }).Distinct();

        return CtasConMovimiento;
        }
    }
