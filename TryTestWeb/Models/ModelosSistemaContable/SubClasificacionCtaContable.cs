using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class SubClasificacionCtaContable
{
    public int SubClasificacionCtaContableID { get; set; }
    public int ClientesContablesModelID { get; set; }

    public string CodigoInterno { get; set; }
    public string NombreInterno { get; set; }

    public SubClasificacionCtaContable()
    {

    }

    public string GetSubClasificacionDisplaySTR()
    {
        return CodigoInterno + " " + NombreInterno;
    }

    public SubClasificacionCtaContable(int _clienteID, string _codigoInterno, string _nombreInterno)
    {
        this.ClientesContablesModelID = _clienteID;
        this.CodigoInterno = _codigoInterno;
        this.NombreInterno = _nombreInterno;
    }
}
