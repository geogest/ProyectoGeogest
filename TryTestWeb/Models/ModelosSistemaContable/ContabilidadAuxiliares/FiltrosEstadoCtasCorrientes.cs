using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class FiltrosEstadoCtasCorrientes
    {
        public int pagina { get; set; } = 1;
        public int cantidadRegistrosPorPagina { get; set; } = 25;
        public int Anio { get; set; }
        public int CuentaAuxiliar { get; set; }
        public int Mes { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string Rut { get; set; }
        public string RazonSocial { get; set; }
        public int? Folio { get; set; }
}

    
