using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TryTestWeb.Models.ModelosSistemaContable.Common
{
    public class VoucherDataDto
    {
        public int VoucherModelID { get; set; }
        public string Glosa { get; set; }
        public DateTime FechaContabilizacion { get; set; }
        public TipoOrigen TipoOrigenVoucher { get; set; }
        public int NumVoucher { get; set; }
        public TipoVoucher TipoVoucher { get; set; }
        public List<DatalleDataVoucherDto> DetalleVoucher { get; set; }

    }

    public class DatalleDataVoucherDto
    {
        public int NumeroLinea { get; set; }
        public int CuentaContableID { get; set; }
        public int CentroDeCostoID { get; set; }
        public string GlosaDetalle { get; set; }
        public decimal MontoDebe { get; set; }
        public decimal MontoHaber { get; set; }
        public List<AuxiliaresDataDto> AuxiliaresDetalle { get; set; }
    }
    public class AuxiliaresDataDto
    {
        public int NumLinea { get; set; }
        public int AuxiliarDetalleID { get; set; }
        public DateTime FechaContabilizacion { get; set; }
        public string TipoReceptor { get; set; }
        public string Rut { get; set; }
        public int RazonSocialID { get; set; }
        public Boolean EsBoleta { get; set; }
        public Boolean ContabilizaCompra { get; set; }
        public Boolean ContabilizaVenta { get; set; }
        public List<AuxiliarDetalleDto> AuxiliarDetalle { get; set; }

    }
    public class AuxiliarDetalleDto
    {
        public long FolioDesde { get; set; }
        public long FolioHasta { get; set; }
        public decimal MontoBruto { get; set; }
        public decimal MontoNeto { get; set; }
        public decimal MontoIvaUsoComun { get; set; }
        public decimal MontoIvaNoRecuperable { get; set; }
        public decimal MontoIvaLinea { get; set; }
        public decimal MontoIvaActivoLinea { get; set; }
        public decimal ValorLiquido { get; set; }
        public decimal MontoTotalLinea { get; set; }
        public decimal MontoExento { get; set; }
        public decimal MontoRetencion { get; set; }
        public TipoDte TipoDTE { get; set; }
    }
}