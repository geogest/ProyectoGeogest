using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public enum TipoDte
{

    //TIPOS PAPEL
    [Display(Name = "Factura de Inicio")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    FacturaInicioPapel = 29,

    [Display(Name = "Factura")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    FacturaPapel = 30,
    [Display(Name = "Factura Exenta")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    FacturaExentaPapel = 32,
    [Display(Name = "Liquidación Factura")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    Liquidacion_FacturaPapel = 40,
    [Display(Name = "Factura de Compra")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    FacturaCompraPapel = 45,
    [Display(Name = "Guía de Despacho")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    GuiaDeDespachoPapel = 50,
    [Display(Name = "Nota de Débito")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    NotaDebitoPapel = 55,
    [Display(Name = "Nota de Crédito")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    NotaCreditoPapel = 60,
    [Display(Name = "Factura de Exportación")]
    [TipoDocumento(SubTipoDocumento.Papel)]
    FacturaExportacionPapel = 101,



    //TIPOS ELECTRONICOS
    [Display(Name = "Factura Electrónica")]
    [TipoDocumento(SubTipoDocumento.Electronico)]
    FacturaElectronica = 33,

    [Display(Name = "Factura no afecta o exenta electrónica")]
    [TipoDocumento(SubTipoDocumento.Electronico)]
    FacturaElectronicaExenta = 34,

    [Display(Name = "Liquidación Factura Electrónica")]
    [TipoDocumento(SubTipoDocumento.Electronico)]
    Liquidacion_FacturaElectronica = 43,

    [Display(Name = "Factura de Compra Electrónica")]
    [TipoDocumento(SubTipoDocumento.Electronico)]
    FacturaCompraElectronica = 46,

    [TipoDocumento(SubTipoDocumento.Electronico)]
    [Display(Name = "Guía de Despacho Electrónica")]
    GuiaDespachoElectronica = 52,

    [TipoDocumento(SubTipoDocumento.Electronico)]
    [Display(Name = "Nota de Débito Electrónica")]
    NotaDebitoElectronica = 56,

    [TipoDocumento(SubTipoDocumento.Electronico)]
    [Display(Name = "Nota de Crédito Electrónica")]
    NotaCreditoElectronica = 61,

    [TipoDocumento(SubTipoDocumento.Electronico)]
    [Display(Name = "Factura de Exportación Electrónica")]
    FacturaExportacionElectronica = 110,

    [TipoDocumento(SubTipoDocumento.Electronico)]
    [Display(Name = "Nota de Débito Exportación Electrónica")]
    NotaDebitoExportacionElectronica = 111,

    [TipoDocumento(SubTipoDocumento.Electronico)]
    [Display(Name = "Nota de Credito Exportación Electrónica")]
    NotaCreditoExportacionElectronica = 112,

    [TipoDocumento(SubTipoDocumento.Electronico)]
    [Display(Name = "SET")]
    SET = -4444,

    [TipoDocumento(SubTipoDocumento.Electronico)]
    [Display(Name = "Declaración de ingreso (DIN)")]
    DeclaracionDeIngreso = 914,

    //Boletas
    [TipoDocumento(SubTipoDocumento.Boletas)]
    [Display(Name = "Boleta Electrónica")]
    BoletaElectronica = 39,

    [TipoDocumento(SubTipoDocumento.Boletas)]
    [Display(Name = "Boleta Electrónica Exenta")]
    BoletaExentaElectronica = 41,

    [TipoDocumento(SubTipoDocumento.Boletas)]
    [Display(Name = "Boleta Venta")]
    BoletaDeVenta = 42,

    [TipoDocumento(SubTipoDocumento.Boletas)]
    [Display(Name = "Boleta Banco")]
    BoletaDeBanco = 44,



    //Tipos Exportacion
    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Orden de Compra")]
    OrdenDeCompra = 801,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Nota de Pedido")]
    NotaDePedido = 802,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Contrato")]
    Contrato = 803,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Resolución")]
    Resolucion = 804,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Proceso ChileCompra")]
    ProcesoChileCompra = 805,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Ficha ChileCompra")]
    FichaChileCompra = 806,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "DUS")]
    DUS = 807,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "B/L (Conocimiento de embarque)")]
    BL = 808,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "AWB (Air Will Bill)")]
    AWB = 809,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "MIC/DTA")]
    MIC_DTA = 810,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Carta de Porte")]
    CartaDePorte = 811,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Resolución del SNA donde califica Servicios de Exportación")]
    ResolucionSNA = 812,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Pasaporte")]
    Pasaporte = 813,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Certificado de Depósito Bolsa Prod. Chile.")]
    CertDepositoBolsaProdChile = 814,

    [TipoDocumento(SubTipoDocumento.Exportacion)]
    [Display(Name = "Vale de Prenda Bolsa Prod. Chile")]
    ValePrendaBolsaProdChile = 815

}

public class TipoDocumento : Attribute
{
    public SubTipoDocumento SubTipo { get; set; }

    public TipoDocumento(SubTipoDocumento txt)
    {
        SubTipo = txt;
    }
}

public enum SubTipoDocumento
{
    [Display(Name = "Documentos Electronicos")]
    Electronico = 1,
    [Display(Name = "Documentos Papel")]
    Papel = 2,
    [Display(Name = "Documentos Boletas")]
    Boletas = 3,
    [Display(Name = "Documentos Exportacion")]
    Exportacion = 4
}

