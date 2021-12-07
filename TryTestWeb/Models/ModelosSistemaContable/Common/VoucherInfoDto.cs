using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TryTestWeb.Models.ModelosSistemaContable.Common
{
    public class VoucherInfoDto
    {
        public int VoucherModelID { get; set; }
        public string Glosa { get; set; }
        public DateTime FechaContabilizacion { get; set; }
        public TipoOrigen TipoOrigenVoucher { get; set; }
        public int NumVoucher { get; set; }
        public TipoVoucher Tipo { get; set; }
        public List<DetalleVoucherDto> DetalleVoucher { get; set; }

    }

    public class DetalleVoucherDto
    {
        public int CuentaContableID { get; set; }
        public int CentroDeCostoID { get; set; }
        public decimal MontoDebe { get; set; }
        public decimal MontoHaber { get; set; }
        public List<AuxiliaresDetalleDto> AuxiliarDetalle { get; set; }
    }

    public class AuxiliaresDetalleDto
    {
        public int AuxiliarDetalleID { get; set; }
        public DateTime FechaContabilizacion { get; set; }
        public string TipoReceptor { get; set; }
        public string Rut { get; set; }
        public string RazonSocial { get; set; }
        public long Folio { get; set; }
        public decimal MontoBruto { get; set; }
        public decimal MontoNeto { get; set; }
        public decimal MontoIvaUsoComun { get; set; }
        public decimal MontoIvaNoRecuperable { get; set; }
        public decimal MontoIvaLinea { get; set; }
        public decimal MontoIvaActivoLinea { get; set; }
        public decimal ValorLiquido { get; set; }
        public decimal MontoTotalLinea { get; set; }
        public decimal MontoExento { get; set; }
        public TipoDte TipoDTE { get; set; }
    }

}