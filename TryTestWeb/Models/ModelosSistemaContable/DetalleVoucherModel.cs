using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class DetalleVoucherModel
{
    public int DetalleVoucherModelID { get; set; }

    public int VoucherModelID { get; set; }

    public virtual CuentaContableModel ObjCuentaContable { get; set; }
    public virtual CentroCostoModel objCentroCostro { get; set; }

    public decimal MontoDebe { get; set; }

    public decimal MontoHaber { get; set; }

    public string GlosaDetalle { get; set; }

    public DateTime FechaDoc { get; set; }

    public string RUTDoc { get; set; }

    public Pagado Pago { get; set; }

    public DateTime FechaVenc { get; set; }

    public string RazonSocialDoc { get; set; }

    public string DocDePago { get; set; }

    public int ItemModelID { get; set; }
    public int CentroCostoID { get; set; }
    public int ClienteProveedor { get; set; }


    public virtual AuxiliaresModel Auxiliar { get; set; }

    //public virtual ICollection<AuxiliaresDetalleModel> RelacionPrestador { get; set; }

}
public enum Pagado
{
    NoPagado = 0,
    Pagado = 1,

}
