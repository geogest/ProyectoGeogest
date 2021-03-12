using System;



public class FiltroReportesContables
{
    public int cantidadRegistrosPorPagina { get; set; } = 25;
    public int pagina { get; set; } = 1;
    public int Anio { get; set; } = 0;
    public int Mes { get; set; } = 0;
    public string Rut { get; set; } = "";
    public string RazonSocial { get; set; } = "";
    public int Folio { get; set; } = 0;
    public string FechaInicio { get; set; } = "";
    public string FechaFin { get; set; } = "";
    public int CentroDeCostosID { get; set; } = 0;
    public int NumVoucher { get; set; } = 0;
    public string Glosa { get; set; } = "";
    public string CuentaContableID { get; set; } = "";
    public string RazonPrestador { get; set; } = "";
    public bool BusquedaDesdeBalance { get; set; } = false;
    public bool EstaConciliado { get; set; } = false;
    public bool Filtro { get; set; } = false;

}
