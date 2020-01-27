using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


public class DTEPagosModel
{
    public int DTEPagosModelID { get; set; }
    public int QuickEmisorModelID { get; set; }

    public decimal MontoPago { get; set; }
    public DateTime FechaPago { get; set; }
    public FormaPago FormaDePago { get; set; }
     
    public int FolioDocAsociado { get; set; }

    public TipoPago TipoDePago {get; set;}

    public TipoDocumentoAsociadoAlPago TipoDocumentoPagado { get; set; }
    public int FolioDocumentoPagado { get; set; } //THE INNER RELATED DOC

    //public static bool RevisarPagoExcesivo

}



public enum FormaPago
{
    [Display(Name = "Efectivo")]
    Efectivo = 1,
    [Display(Name = "Deposito")]
    Factura = 2,
    [Display(Name = "Cheque")]
    Cheque = 3,
    [Display(Name = "Transferencia")]
    Transferencia = 4,
    [Display(Name = "Tarjeta de Credito")]
    TarjetaCredito = 5,
    [Display(Name = "Red Compra")]
    RedCompra = 6
}

public enum TipoPago
{
    [Display(Name = "Parcial")]
    Parcial = 1,
    [Display(Name = "Total")]
    Total = 2
}

public enum TipoDocumentoAsociadoAlPago
{
    [Display(Name = "Venta")]
    Venta = 1,
    [Display(Name = "Compra")]
    Compra = 2,
    [Display(Name = "Gastos")]
    OtrosEgresosFijos = 3,
    [Display(Name = "Boleta Honorarios")]
    BoletasHonorarios = 4,
    [Display(Name = "Formulario 29")]
    Formulario29 = 5,
    [Display(Name = "Liquidacion")]
    Liquidaciones = 6,
    [Display(Name = "Honorarios")]
    Honorarios = 7
}