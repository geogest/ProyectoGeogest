using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class FiltrosParaLibros
    {
        public int pagina { get; set; } = 1;
        public int cantidadRegistrosPorPagina { get; set; } = 25;
        public string FechaInicio { get; set; } = "";
        public string FechaFin { get; set; } = "";
        public int Anio { get; set; } = 0;
        public int Mes { get; set; } = 0;
        public string Rut { get; set; } = "";
        public string Glosa { get; set; } = "";
        public string CuentaContableID { get; set; } = "";
        public string RazonPrestador { get; set; } = "";
        public int NumVoucher { get; set; } = 0;
        public bool BusquedaDesdeBalance { get; set; } = false;
        public string Folio { get; set; } = "";
        public int IFolio { get; set; } = 0;
        public string RazonSocial { get; set; } = "";
        public bool Filtro { get; set; } = false;
        public int CentroDeCostosID { get; set; } = 0;
        public bool EstaConciliado { get; set; } = false;
    }
