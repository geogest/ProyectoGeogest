using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class LibroMayor
    {
       public int NumVoucher { get; set; }
       public int VoucherId { get; set; }
       public DateTime FechaContabilizacion { get; set; }
       public TipoVoucher Comprobante { get; set; }
       public string ComprobanteP2 { get; set; }
       public string ComprobanteP3 { get; set; }
       public string Glosa { get; set; }
       public string Rut { get; set; }
       public string RazonSocial { get; set; }
       public decimal Debe { get; set; }
       public decimal Haber { get; set; }
       public decimal Saldo { get; set; }
       public string CtaContNombre { get; set; }
       public ClasificacionCtaContable CtaContableClasi { get; set; }
       public int CtaContablesID { get; set; }
       public string Nombre { get; set; }
       public string CodigoInterno { get; set; }



    }
