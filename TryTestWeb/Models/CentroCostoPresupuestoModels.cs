using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class CentroCostoPresupuestoModels
    {
        public int CentroCostoPresupuestoModelsID { get; set; }

        public int CentroCostoModelID { get; set; }

        public int ClienteContableModelID { get; set; }

        public decimal Presupuesto { get; set; }

        public DateTime FechaInicioPresu { get; set; }

        public DateTime FechaVencimientoPresu { get; set; }

    }
