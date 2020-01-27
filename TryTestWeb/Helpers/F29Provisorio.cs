using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class F29Provisorio
{
    //503
    public int CantidadFacturasEmitidas_503;
    //509
    public int CantidadNotasCreditosEmitidas_509;
    //???
    public int CantidadNotasDebitoEmitidas;
    //511
    public decimal CreditoIVADocumentosElectronicos_511;

    //584 (CANT INT EX NO GRAV SIN DER FISCAL)
    //???
    //524(Cantidad Facturas Activo FIJO)
    ///???

    //519
    public int CantidadDocumentosFacturasRecibidasDelGiro_519;
    //527
    public int CantidadNotasCreditoRecibidas_527;
    //???
    public int CantidadNotasDebitoRecibidas;


    //502
    public decimal DebitosFacturasEmitidas_502;
    //510
    public decimal DebitosNotasCreditoEmitidas_510;
    //538
    public decimal TotalDebitos_538 { get { return (this.DebitosFacturasEmitidas_502 - this.DebitosNotasCreditoEmitidas_510); } }


    //151
    public decimal RetencionTasas10SobreLasRentas_151;
    //048
    public decimal RetencionImpuestoUnicoTrabajador_048;
}
