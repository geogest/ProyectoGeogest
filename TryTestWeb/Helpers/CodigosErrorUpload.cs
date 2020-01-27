using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


public enum CodigosErrorUpload
{
    [Display(Name = "OK")]
    UploadOK = 0,
    [Display(Name = "El sender no tiene permiso para enviar")]
    SenderCannotSend = 1,
    [Display(Name = "Error en tamaño del archivo (muy grande o muy chico)")]
    WrongFilesize = 2,
    [Display(Name = "Archivo cortado (tamaño distinto del parametro size)")]
    ArchivoCortado = 3,
    [Display(Name = "No esta autenticado")]
    NotAuth = 5,
    [Display(Name = "Empresa no autorizada a enviar archivos")]
    EmpresaNoAutorizada = 6,
    [Display(Name = "Esquema invalido")]
    EsquemaInvalido = 7,
    [Display(Name = "Firma del Documento")]
    FirmaDelDocumento = 8,
    [Display(Name = "Sistema bloqueado")]
    SistemaBloqueado = 9,
    [Display(Name = "Otro")]
    Otro = -1
}

/*
//TIPOS ELECTRONICOS
    [Display(Name = "Factura Electrónica")]
    FacturaElectronica = 33,
    [Display(Name = "Factura Exenta Electrónica")]
    FacturaElectronicaExenta = 34,
    [Display(Name = "Liquidación Factura Electrónica")]
    Liquidacion_FacturaElectronica = 43,
    [Display(Name = "Factura de Compra Electrónica")]
    FacturaCompraElectronica = 46,
    [Display(Name = "Guía de Despacho Electrónica")]

     */
