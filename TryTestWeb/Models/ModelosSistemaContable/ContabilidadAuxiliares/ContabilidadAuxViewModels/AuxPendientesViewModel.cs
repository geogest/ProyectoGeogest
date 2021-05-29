using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class AuxPendientesViewModel
{
    public int Id { get; set; }
    public string Rut { get; set; }
    public string RazonSocial { get; set; }
    public decimal Saldo { get; set; }
}

public class AuxPendientesDetalle
{
    public int Id { get; set; }
    public string Rut { get; set; }
    public string RazonSocial { get; set; }
    public decimal Debe { get; set; }
    public decimal Haber { get; set; }
}
