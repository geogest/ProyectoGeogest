using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SubSubClasificacionCtaContable
{
    public int SubSubClasificacionCtaContableID { get; set; }
    public int ClientesContablesModelID { get; set; }

    public string CodigoInterno { get; set; }
    public string NombreInterno { get; set; }

    public SubSubClasificacionCtaContable()
    {

    }

    public string GetSubSubClasificacionDisplaySTR()
    {
        return CodigoInterno + " " + NombreInterno;
    }

    public SubSubClasificacionCtaContable(int _clienteID, string _codigoInterno, string _nombreInterno)
    {
        this.ClientesContablesModelID = _clienteID;
        this.CodigoInterno = _codigoInterno;
        this.NombreInterno = _nombreInterno;
    }

    public decimal GetPresupuesto(string UserID, int subsubid)
    {
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        SubSubCtaPresupuestoModel lstSubSubCtaPresupuesto = db.DBSubSubCtaPresupuesto.SingleOrDefault(r => r.SubSubClasificacionCtaContableID == subsubid);

        decimal Presupuesto = 0;

        if (lstSubSubCtaPresupuesto != null)
        {
            Presupuesto = lstSubSubCtaPresupuesto.Presupuesto;
        }

        return Presupuesto;
    }

}
