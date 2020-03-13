using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class CtasContablesPresupuestoModel
    {
    public int CtasContablesPresupuestoModelID { get; set; }
    public int PresupuestoModelID { get; set; }

    public int CuentasContablesModelID { get; set; } 

    public int ClientesContablesModelID { get; set; } 

    public decimal Presupuesto { get; set; } 

    public DateTime FechaInicioPresu { get; set; }

    public DateTime FechaVencimientoPresu { get; set; }
    }
