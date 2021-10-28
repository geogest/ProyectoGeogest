using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ConciliacionBancariaViewModel
{
   public int VoucherID { get; set; }
   public string CodigoInterno { get; set; }
   public string CuentaContable { get; set; }
   public long Folio { get; set; }
   public DateTime FechaContabilizacion { get; set; }
   public TipoDte Documento { get; set; }
   public string Glosa { get; set; }
   public decimal Debe { get; set; }
   public decimal Haber { get; set; }
   public decimal MontoTotal { get; set; }


    public static IQueryable<ConciliacionBancariaViewModel> GetQueryConciliacionBancaria(ClientesContablesModel objCliente, FacturaPoliContext db)
    {
        IQueryable<ConciliacionBancariaViewModel> LstCtaCorriente = (from Detalle in db.DBDetalleVoucher
                                                                       join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                                                       join Auxiliar in db.DBAuxiliares on Detalle.Auxiliar.AuxiliaresModelID equals Auxiliar.AuxiliaresModelID
                                                                       join AuxiliaresDetalle in db.DBAuxiliaresDetalle on Auxiliar.AuxiliaresModelID equals AuxiliaresDetalle.AuxiliaresModelID
                                                                       where Auxiliar.objCtaContable.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                             Voucher.DadoDeBaja == false &&
                                                                             Detalle.ObjCuentaContable.nombre.StartsWith("BANCO")

                                                                       select new ConciliacionBancariaViewModel
                                                                       {
                                                                           FechaContabilizacion = Detalle.FechaDoc,
                                                                           Folio = AuxiliaresDetalle.Folio,
                                                                           Documento = AuxiliaresDetalle.TipoDocumento,
                                                                           Debe = Detalle.MontoDebe,
                                                                           Haber = Detalle.MontoHaber,
                                                                           CodigoInterno = Detalle.ObjCuentaContable.CodInterno,
                                                                           CuentaContable = Detalle.ObjCuentaContable.nombre,
                                                                           MontoTotal = AuxiliaresDetalle.MontoTotalLinea,
                                                                           VoucherID = Voucher.VoucherModelID
                                                                       });
        return LstCtaCorriente;
    }

    public static List<CuentaContableModel> getCtasBancarias(FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        var lstCtasBancos = objCliente.CtaContable.Where(x => x.nombre.StartsWith("BANCO")).ToList();
  
        return lstCtasBancos;
    }


}
