using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



public class LibroDeHonorariosModel
{
    public int LibroDeHonorariosModelID { get; set; }
    public int ClientesContablesID { get; set; }
    public int VoucherModelID { get; set; }
    public int NumIdenficiador { get; set; }
    public int QuickEmisorModelID { get; set; }
    public TipoCentralizacion TipoLibro { get; set; } = TipoCentralizacion.Honorarios;
    public string SocProf { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaContabilizacion { get; set; }
    public string Estado { get; set; }
    public DateTime FechaAnulacion { get; set; }
    public string Rut { get; set; }
    public string RazonSocial { get; set; }
    public decimal Brutos { get; set; } //DEBE Ctaque se elige en la importacion
    public decimal Retenido { get; set; } //HABER Retencion1
    public decimal Pagado { get; set; } //HABER Retencion2
    public virtual QuickReceptorModel Prestador { get; set; }
    public TipoVoucher TipoVoucher { get; set; } = TipoVoucher.Traspaso;
    public bool HaSidoConvertidoAVoucher { get; set; } = false;

    public LibroDeHonorariosModel()
    {

    }

}
