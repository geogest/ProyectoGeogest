using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TryTestWeb.Models.ModelosSistemaContable.ContabilidadBoletas
{
    public class BoletasCoVModel
    { 
        public int BoletasCoVModelID { get; set; }
        public string CuentaAuxiliar { get; set; }
        public ClientesContablesModel ClienteContable { get; set; }
        public QuickReceptorModel Prestador { get; set; }
        public int VoucherModelID { get; set; }
        public int HaSidoConvertidoAVoucher { get; set; }
        public DateTime FechaInsercion { get; set; }
        public DateTime Fecha { get; set; }
        public int NumeroDeDocumento { get; set; }
        public TipoDte TipoDocumento { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string CuentaContable { get; set; } //Compra o venta
        public decimal Neto { get; set; }
        public decimal Iva { get; set; }
        public int CentroDeCostos { get; set; }
        public DateTime FechaPeriodoTributario { get; set; }
    }
}