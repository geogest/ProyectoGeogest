using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class SubSubCtaPresupuestoModel
    {
     public int SubSubCtaPresupuestoModelID { get; set; }
     public int SubSubClasificacionCtaContableID { get; set; }
     public int ClientesContablesModelID { get; set; } 
     public decimal Presupuesto { get; set; }
     public DateTime FechaInicio { get; set; }
     public DateTime FechaVencimiento { get; set; }

       
    }
