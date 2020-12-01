using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ObjetoVoucher
{
    public VoucherModel DatosTbVoucher { get; set; }
    public DetalleVoucherModel DatosTbDetalle { get; set; }
    public AuxiliaresModel DatosTbAux { get; set; }
    public AuxiliaresDetalleModel DatosTbAuxDetalle { get; set; }
}

public class UtilesContabilidad
{
    public static bool CrearVoucher()
    {
        bool Result = false;
        return Result;
    }

    public static CuentaContableModel CuentaContableDesdeCodInterno(string CodInterno, ClientesContablesModel ObjCliente)
    {
        CuentaContableModel ReturnValues = new CuentaContableModel();
        if (ObjCliente != null)
        {
            ReturnValues = ObjCliente.CtaContable.SingleOrDefault(x => x.CodInterno == CodInterno);
        }
       
        return ReturnValues;
    }
}
