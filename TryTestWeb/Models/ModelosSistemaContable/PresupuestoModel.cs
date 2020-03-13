using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class PresupuestoModel
    {
        public int PresupuestoModelID { get; set; }
        public ClientesContablesModel Cliente { get; set; }
        public string NombrePresupuesto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool EstaVencido { get; set; } = false;
        public bool DadoDeBaja { get; set; } = false;
    }
