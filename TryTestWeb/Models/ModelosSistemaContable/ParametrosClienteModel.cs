using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ParametrosClienteModel
{
    public int ParametrosClienteModelID { get; set; }
    public int ClientesContablesModelID { get; set; }

    public virtual CuentaContableModel CuentaIvaVentas { get; set; }
    public virtual CuentaContableModel CuentaIvaCompras { get; set; }
    public virtual CuentaContableModel CuentaRetencionHonorarios { get; set; }
    public virtual CuentaContableModel CuentaRetencionesHonorarios2 { get; set; }
    public virtual CuentaContableModel CuentaVentas { get; set; }
    public virtual CuentaContableModel CuentaCompras { get; set; }

}
