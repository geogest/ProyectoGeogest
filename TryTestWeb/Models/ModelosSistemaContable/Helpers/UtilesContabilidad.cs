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
            ReturnValues = ObjCliente.CtaContable.FirstOrDefault(x => x.CodInterno == CodInterno);
        }
       
        return ReturnValues;
    }

    public static CuentaContableModel CuentaContableDesdeID(int CuentaContableID, ClientesContablesModel ObjCliente)
    {
        CuentaContableModel ReturnValues = new CuentaContableModel();
        if (ObjCliente != null)
        {
            ReturnValues = ObjCliente.CtaContable.SingleOrDefault(x => x.CuentaContableModelID == CuentaContableID);
        }

        return ReturnValues;
    }

    public static string ObtenerNombreCuentaContable(int CuentaContableID, ClientesContablesModel ObjCliente)
    {
        string NombreCtaCont = string.Empty;
        CuentaContableModel CuentaCont = new CuentaContableModel();
        if (ObjCliente != null)
            CuentaCont = ObjCliente.CtaContable.SingleOrDefault(x => x.CuentaContableModelID == CuentaContableID);

        NombreCtaCont = CuentaCont.CodInterno + "  " + CuentaCont.nombre;
        return NombreCtaCont;
    }

    public static QuickReceptorModel ObtenerPrestadorSiExiste(string Rut, FacturaPoliContext db, ClientesContablesModel ObjCliente)
    {
        var Receptor = new QuickReceptorModel();

        if (!string.IsNullOrWhiteSpace(Rut))
        {
            Receptor = db.Receptores.FirstOrDefault(x => x.RUT == Rut && x.ClientesContablesModelID == ObjCliente.ClientesContablesModelID && x.tipoReceptor == "PR"||
                                                         x.RUT == Rut && x.ClientesContablesModelID == ObjCliente.ClientesContablesModelID && x.tipoReceptor == "P" ||
                                                         x.RUT == Rut && x.ClientesContablesModelID == ObjCliente.ClientesContablesModelID && x.tipoReceptor == "H" ||
                                                         x.RUT == Rut && x.ClientesContablesModelID == ObjCliente.ClientesContablesModelID && x.tipoReceptor == "CL");
        }else
        {
            return null;
        }

        return Receptor;
    }

    public static TipoOrigen RetornaTipoReceptor(QuickReceptorModel Prestador)
    {
        var ReturnValues = new TipoOrigen();

        if (Prestador != null && Prestador.tipoReceptor == "PR")
            ReturnValues = TipoOrigen.Compra;
        else if (Prestador != null && Prestador.tipoReceptor == "CL")
            ReturnValues = TipoOrigen.Venta;
        else if (Prestador != null && Prestador.tipoReceptor == "P")
            ReturnValues = TipoOrigen.Remuneraciones;
        else if (Prestador != null && Prestador.tipoReceptor == "H")
            ReturnValues = TipoOrigen.Honorario;
        else
            ReturnValues = TipoOrigen.Otros;

        return ReturnValues;
    }
}
