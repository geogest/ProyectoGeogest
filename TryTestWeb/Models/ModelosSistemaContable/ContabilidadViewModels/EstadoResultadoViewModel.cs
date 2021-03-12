using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ObjCtaContable
{
    public string CodigoCuenta { get; set; }
    public string NombreCuenta { get; set; }
    public ClasificacionCtaContable ClasiCta { get; set; }

}

public class FatherObjEstadoResultado
{
    public decimal Haber { get; set; }
    public decimal Debe { get; set; }
    public DateTime Fecha { get; set; }
    public ObjCtaContable InfoCuenta { get; set; }
}
public class EstadoResultadoViewModel
{
    public string CodigoInterno { get; set; }
    public string Nombre { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public ClasificacionCtaContable Clasificacion { get; set; }


    public static List<EstadoResultadoViewModel> GetReporteEstadoResultado(FacturaPoliContext db, ClientesContablesModel ObjClienteContable)
    {
         List<EstadoResultadoViewModel> ReturnValues = new List<EstadoResultadoViewModel>();
        //Queda pendiente incluir los totales

         ReturnValues = ObjClienteContable.ListVoucher.Where(x => x.DadoDeBaja == false)
                                         .SelectMany(x => x.ListaDetalleVoucher)
                                         .Where(x => x.ObjCuentaContable.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA ||
                                         x.ObjCuentaContable.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA && x.FechaDoc.Year == 2020)
                                         .Select(x => new FatherObjEstadoResultado{
                                                    InfoCuenta = 
                                                    {
                                                        CodigoCuenta = x.ObjCuentaContable.CodInterno,
                                                        NombreCuenta = x.ObjCuentaContable.nombre,
                                                        ClasiCta = x.ObjCuentaContable.Clasificacion
                                                    },
                                                    Haber = x.MontoHaber,
                                                    Debe = x.MontoDebe
                                                })
                                        .GroupBy(x => new { x.InfoCuenta })
                                        .Select(grp => new EstadoResultadoViewModel
                                                {
                                                    CodigoInterno = grp.Key.InfoCuenta.CodigoCuenta,
                                                    Nombre = grp.Key.InfoCuenta.NombreCuenta,
                                                    Clasificacion = grp.Key.InfoCuenta.ClasiCta,
                                                    Monto = grp.Sum(x => Math.Abs(x.Haber) - Math.Abs(x.Debe))
                                                })
                                        .OrderBy(x => x.Clasificacion).ToList();

        return ReturnValues;
    }





    public static IQueryable<EstadoResultadoViewModel> GetEstadoResultadoTwo(FacturaPoliContext db, ClientesContablesModel ObjCliente)
    {
        IQueryable<EstadoResultadoViewModel> ReturnValues = (from Detalle in db.DBDetalleVoucher
                                                             join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                                             where
                                                                   Voucher.DadoDeBaja == false &&
                                                                   Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                                                                   Detalle.ObjCuentaContable.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA ||

                                                                   Voucher.DadoDeBaja == false &&
                                                                   Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                                                                   Detalle.ObjCuentaContable.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA
                                                             select new
                                                             {
                                                                 CuentaContable = new
                                                                 {
                                                                     CodigoCuentaContable = Detalle.ObjCuentaContable.CodInterno,
                                                                     NombreCuenta = Detalle.ObjCuentaContable.nombre,
                                                                     ClasiCta = Detalle.ObjCuentaContable.Clasificacion
                                                                 },
                                                                 Haber = Detalle.MontoHaber,
                                                                 Debe = Detalle.MontoDebe,
                                                             }
                                                             ).GroupBy(x => x.CuentaContable)
                                                             .Select(grp => new EstadoResultadoViewModel
                                                             {
                                                                 CodigoInterno = grp.Key.CodigoCuentaContable,
                                                                 Nombre = grp.Key.NombreCuenta,
                                                                 Clasificacion = grp.Key.ClasiCta,
                                                                 Monto = grp.Sum(x => Math.Abs(x.Haber) - Math.Abs(x.Debe))
                                                             });

        return ReturnValues;
    }

    public static IQueryable<FatherObjEstadoResultado> QueryEstadoResultado(FacturaPoliContext db, ClientesContablesModel ObjCliente)
    {
        var ReturnValues = (from Detalle in db.DBDetalleVoucher
                            join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                            where
                                Voucher.DadoDeBaja == false &&
                                Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                                Detalle.ObjCuentaContable.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA ||

                                Voucher.DadoDeBaja == false &&
                                Voucher.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                                Detalle.ObjCuentaContable.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA
                            select new FatherObjEstadoResultado
                            {
                                InfoCuenta = new ObjCtaContable
                                {
                                    CodigoCuenta = Detalle.ObjCuentaContable.CodInterno,
                                    NombreCuenta = Detalle.ObjCuentaContable.nombre,
                                    ClasiCta = Detalle.ObjCuentaContable.Clasificacion
                                },
                                Haber = Detalle.MontoHaber,
                                Debe = Detalle.MontoDebe,
                                Fecha = Detalle.FechaDoc
                            });




        return ReturnValues;
    }

    public static IQueryable<FatherObjEstadoResultado> GetEstadoResultadoFiltrado(IQueryable<FatherObjEstadoResultado> EstadoResultado, FiltrosEstadoResultado Filtros)
    {
        if(Filtros != null)
        {
            if (Filtros.Anio > 0)
                EstadoResultado = EstadoResultado.Where(x => x.Fecha.Year == Filtros.Anio);
            if (Filtros.Mes > 0)
                EstadoResultado = EstadoResultado.Where(x => x.Fecha.Month == Filtros.Mes);
            if(!string.IsNullOrWhiteSpace(Filtros.FechaDesde) && !string.IsNullOrWhiteSpace(Filtros.FechaHasta))
            {
                DateTime dtFechaDesde = ParseExtensions.ToDD_MM_AAAA_Multi(Filtros.FechaDesde);
                DateTime dtFechaHasta = ParseExtensions.ToDD_MM_AAAA_Multi(Filtros.FechaHasta);
                EstadoResultado = EstadoResultado.Where(x => x.Fecha >= dtFechaDesde && x.Fecha <= dtFechaHasta);
            }
        }
        return EstadoResultado;
    }

    public static List<EstadoResultadoViewModel> EstadoResultadoProcesado(IQueryable<FatherObjEstadoResultado> EstadoResultado)
    {
        List<EstadoResultadoViewModel> ReturnValues = EstadoResultado.GroupBy(x => x.InfoCuenta)
                                                                           .Select(grp => new EstadoResultadoViewModel
                                                                           {
                                                                               CodigoInterno = grp.Key.CodigoCuenta,
                                                                               Nombre = grp.Key.NombreCuenta,
                                                                               Clasificacion = grp.Key.ClasiCta,
                                                                               Monto = Math.Abs(grp.Sum(x => Math.Abs(x.Haber) - Math.Abs(x.Debe)))
                                                                           })
                                                                           .OrderBy(x => x.Clasificacion)
                                                                           .ToList();
        return ReturnValues;
    }
}

