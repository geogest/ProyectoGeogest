using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class FiltrosEstadoResultado
{
    public string FechaDesde { get; set; }
    public string FechaHasta { get; set; }
    public int Mes { get; set; } = 0;
    public int Anio { get; set; } = DateTime.Now.Year;
}
