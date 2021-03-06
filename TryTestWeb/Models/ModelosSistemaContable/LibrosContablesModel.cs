﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.Web.Routing;
using System.Globalization;

public class LibrosContablesModel
{
    //ID BASE DE DATOS

    public int LibrosContablesModelID { get; set; }
    public int ClientesContablesModelID { get; set; }
    public int VoucherModelID { get; set; }
    public int CodigoUnionImpuesto { get; set; }
    //Tipo al cual corresponde
    public TipoCentralizacion TipoLibro { get; set; }

    //DATOS DOCUMENTO CONTABLE
    /// <summary>
    /// NOTE: [1] en el archivo del SII
    /// </summary>
    [Display(Name = "Tipo Documento")]
    public TipoDte TipoDocumento { get; set; }

    /// <summary>
    /// NOTE:
    /// [3] RUT PRESTADOR
    /// [4] RAZON SOCIAL PRESTADOR
    /// </summary>
    public virtual AuxiliaresPrestadoresModel Prestador { get; set; }

    /// <summary>
    /// NOTE: [5] en el archivo del SII
    /// </summary>
    public int Folio { get; set; }
    public int FolioHasta { get; set; }
    /// <summary>
    /// NOTE: [6] en el archivo del SII
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "Fecha Documento")]
    [DataType(DataType.Text)]
    public DateTime FechaDoc { get; set; }
    /// <summary>
    /// NOTE: [7] en el archivo del SII
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "Fecha Recepción")]
    [DataType(DataType.Text)]
    public DateTime FechaRecep { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "Fecha Contabilizacion")]
    [DataType(DataType.Text)]
    public DateTime FechaContabilizacion { get; set; }



    //MONTOS DEL DOCUMENTO CONTABLE
    /// <summary>
    /// NOTE: [10] en el archivo del SII
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:c0}")]
    [Display(Name = "Monto Exento")]
    public Decimal MontoExento { get; set; }
    /// <summary>
    /// NOTE: [11] en el archivo del SII
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:c0}")]
    [Display(Name = "Monto Neto")]
    public Decimal MontoNeto { get; set; }
    /// <summary>
    /// NOTE: [12] en el archivo del SII
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:c0}")]
    [Display(Name = "Monto IVA")]
    public Decimal MontoIva { get; set; }
    /// <summary>
    /// NOTE: [13] en el archivo del SII
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:c0}")]
    [Display(Name = "Monto Total")]
    public Decimal MontoTotal { get; set; }

    //AGREGAR ACA IVAS RETENIDOS EN EL FUTURO DE SER NECESARIO

    //REFERENCIA DEL DOCUMENTO CONTABLE
    /// <summary>
    /// NOTE: [24] en el archivo del SII
    /// </summary>
    [Display(Name = "Documento Referencia")]
    public TipoDte TipoDocReferencia { get; set; }
    /// <summary>
    /// NOTE: [25] en el archivo del SII
    /// </summary>
    [Display(Name = "Folio Documento Referencia")]
    public int? FolioDocReferencia { get; set; }

    public bool HaSidoConvertidoAVoucher { get; set; } = false;


    public Decimal MontoIvaNoRecuperable { get; set; }

    public Decimal MontoIvaUsocomun { get; set; }

    public Decimal MontoIvaActivoFijo { get; set; }


    public bool estado { get; set; }

    public virtual QuickReceptorModel individuo { get; set; }
    public bool EsUnRegistroManual { get; set; } = false;
     
    public static string ProcesarLibrosContablesAVoucher(List<LibrosContablesModel> lstEntradasLibro, ClientesContablesModel objCliente, FacturaPoliContext db, List<CuentaContableModel> lstCuentaContable)
    {

        using(var dbContextTransaction = db.Database.BeginTransaction())
        {
            string FeedBack = "";
            try
            {
                if (lstEntradasLibro == null || lstEntradasLibro.Count == 0 || objCliente == null || objCliente.ParametrosCliente == null)
                    throw new Exception("Error no hay datos para procesar.");

                var folioExcel = lstEntradasLibro.First().Folio.ToString();

                if (lstEntradasLibro.Count != lstCuentaContable.Count)
                    throw new Exception("Error no hay cuentas contables asignadas para cada registro.");

                CuentaContableModel CuentaIVAAUsar = null;
                if (lstEntradasLibro.First().TipoLibro == TipoCentralizacion.Compra)
                    CuentaIVAAUsar = db.DBCuentaContable.SingleOrDefault(r => r.CuentaContableModelID == objCliente.ParametrosCliente.CuentaIvaCompras.CuentaContableModelID && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                else if (lstEntradasLibro.First().TipoLibro == TipoCentralizacion.Venta)
                    CuentaIVAAUsar = db.DBCuentaContable.SingleOrDefault(r => r.CuentaContableModelID == objCliente.ParametrosCliente.CuentaIvaVentas.CuentaContableModelID && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                else
                    throw new Exception("Error el documento no es de compra ni venta.");
                if (CuentaIVAAUsar == null)
                    throw new Exception("Error no se encontró configuración valida en el modulo parametros clientes, Por favor parametrizar las cuentas contables.");

                List<VoucherModel> lstNuevosVouchers = new List<VoucherModel>();
                int contadorAnexo = 0;

                int? nullableProxVoucherNumber = ParseExtensions.ObtenerNumeroProximoVoucherINT(objCliente, db);
                int baseNumberFolio = nullableProxVoucherNumber.Value;
                CuentaContableModel cuentaPrincipal = new CuentaContableModel();

                foreach (LibrosContablesModel entradaLibro in lstEntradasLibro)
                {
                    //CuentaIVAAUsar = db.DBCuentaContable.SingleOrDefault(r => r.CuentaContableModelID == objCliente.ParametrosCliente.CuentaIvaCompras.CuentaContableModelID && r.ClientesContablesModelID == objCliente.ClientesContablesModelID );
                    VoucherModel nuevoVoucher = new VoucherModel();
                    if (entradaLibro.TipoLibro == TipoCentralizacion.Venta)
                    {
                        nuevoVoucher.TipoOrigen = "Venta";
                        nuevoVoucher.TipoOrigenVoucher = TipoOrigen.Venta;
                    }

                    if (entradaLibro.TipoLibro == TipoCentralizacion.Compra)
                    {
                        nuevoVoucher.TipoOrigen = "Compra";
                        nuevoVoucher.TipoOrigenVoucher = TipoOrigen.Compra;
                    }

                    nuevoVoucher.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                    nuevoVoucher.FechaEmision = entradaLibro.FechaContabilizacion;
                    nuevoVoucher.Tipo = TipoVoucher.Traspaso;

                    string FullDescripcionDocOriginal = (int)entradaLibro.TipoDocumento + " / Folio: " + entradaLibro.Folio + " / " + entradaLibro.individuo.RazonSocial;

                    nuevoVoucher.Glosa = FullDescripcionDocOriginal;

                    nuevoVoucher.NumeroVoucher = baseNumberFolio;

                    //DEFINIR CUAL ES LA CUENTA DE VENTA O COMPRA DONDE VAN LAS VENTAS O COMPRAS
                    List<DetalleVoucherModel> DetalleVoucher = new List<DetalleVoucherModel>();


                    if (entradaLibro.TipoLibro == TipoCentralizacion.Venta)
                    {
                        // decimal CostoNeto = entradaLibro.MontoTotal - entradaLibro.MontoIva;
                        decimal CostoNeto = entradaLibro.MontoNeto;
                        decimal MontoTotal = entradaLibro.MontoTotal;
                        decimal MontoIva = entradaLibro.MontoIva;
                        decimal MontoExento = entradaLibro.MontoExento;


                        DetalleVoucherModel detalleGastoNeto = new DetalleVoucherModel();
                        detalleGastoNeto.FechaDoc = entradaLibro.FechaContabilizacion;

                        //EN VENTA EL TOTAL VA AL DEBE, EN EL HABER VA EL GASTO NETO Y EL IVA ... pero si es una nota de credito los del haber van al debe y viceversa
                        if (entradaLibro.TipoDocumento.EsUnaNotaCredito() == false)
                        {
                            detalleGastoNeto.MontoHaber = CostoNeto + MontoExento;
                            detalleGastoNeto.MontoDebe = 0;
                        }
                        else
                        {
                            detalleGastoNeto.MontoHaber = 0;
                            detalleGastoNeto.MontoDebe = CostoNeto + MontoExento;
                        }

                        detalleGastoNeto.GlosaDetalle = "Costo Neto " + FullDescripcionDocOriginal;

                        detalleGastoNeto.ObjCuentaContable = lstCuentaContable[contadorAnexo];

                        DetalleVoucher.Add(detalleGastoNeto);

                        DetalleVoucherModel detalleGastoIVA = new DetalleVoucherModel();
                        if (entradaLibro.MontoIva > 0)
                        {
                            detalleGastoIVA.FechaDoc = entradaLibro.FechaContabilizacion;

                            //EN VENTA EL TOTAL VA AL DEBE, EN EL HABER VA EL GASTO NETO Y EL IVA ... pero si es una nota de credito los del haber van al debe y viceversa
                            if (entradaLibro.TipoDocumento.EsUnaNotaCredito() == false)
                            {
                                detalleGastoIVA.MontoHaber = MontoIva;
                                detalleGastoIVA.MontoDebe = 0;
                            }
                            else
                            {
                                detalleGastoIVA.MontoHaber = 0;
                                detalleGastoIVA.MontoDebe = MontoIva;
                            }

                            detalleGastoIVA.GlosaDetalle = "IVA Ventas " + FullDescripcionDocOriginal;
                            detalleGastoIVA.ObjCuentaContable = CuentaIVAAUsar;//objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == objCliente.ParametrosCliente.CuentaIvaVentas.CuentaContableModelID);
                            DetalleVoucher.Add(detalleGastoIVA);
                        }

                        DetalleVoucherModel detalleCtaVenta = new DetalleVoucherModel();
                        detalleCtaVenta.FechaDoc = entradaLibro.FechaContabilizacion;

                        //EN VENTA EL TOTAL VA AL DEBE, EN EL HABER VA EL GASTO NETO Y EL IVA ... pero si es una nota de credito los del haber van al debe y viceversa
                        if (entradaLibro.TipoDocumento.EsUnaNotaCredito() == false)
                        {
                            detalleCtaVenta.MontoHaber = 0;
                            detalleCtaVenta.MontoDebe = MontoTotal;
                        }
                        else
                        {
                            detalleCtaVenta.MontoHaber = MontoTotal;
                            detalleCtaVenta.MontoDebe = 0;
                        }

                        detalleCtaVenta.ObjCuentaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == objCliente.ParametrosCliente.CuentaVentas.CuentaContableModelID);
                        cuentaPrincipal = detalleCtaVenta.ObjCuentaContable;
                        detalleCtaVenta.GlosaDetalle = detalleCtaVenta.ObjCuentaContable.nombre + " (Venta) " + FullDescripcionDocOriginal;
                        DetalleVoucher.Add(detalleCtaVenta);
                    }
                    //EN COMPRA EL TOTAL VA AL HABER, EN EL DEBE VA EL GASTO NETO Y EL IVA... ...pero si es una nota de credito los del debe van al haber y viceversa
                    else if (entradaLibro.TipoLibro == TipoCentralizacion.Compra)
                    {
                        decimal CostoNeto = entradaLibro.MontoNeto;//entradaLibro.MontoTotal - entradaLibro.MontoIva;
                        decimal CostoIvaNoRecuperable = entradaLibro.MontoIvaNoRecuperable;
                        decimal CostoIvaActivoFijo = entradaLibro.MontoIvaActivoFijo;
                        decimal CostoIvaUsoComun = entradaLibro.MontoIvaUsocomun;
                        decimal MontoTotal = entradaLibro.MontoTotal;
                        decimal MontoIva = entradaLibro.MontoIva;
                        decimal montoExento = entradaLibro.MontoExento;
                        String tipocompra = "";
                        //Iva No Recuperable
                        if (CostoIvaNoRecuperable > 0 && CostoIvaUsoComun == 0 && MontoIva == 0)
                        {
                            tipocompra = "IvaNoRecuperable";
                        }
                        if (CostoIvaUsoComun == 0 && MontoIva > 0 && CostoIvaNoRecuperable == 0)
                        {
                            tipocompra = "IvaRecuperable";
                        }
                        if (CostoIvaNoRecuperable > 0 || CostoIvaNoRecuperable == 0 && MontoIva == 0 && CostoIvaUsoComun > 0)
                        {
                            tipocompra = "IvaUsoComun";
                        }

                        DetalleVoucherModel detalleGastoNeto = new DetalleVoucherModel();
                        detalleGastoNeto.FechaDoc = entradaLibro.FechaContabilizacion;

                        //EN COMPRA EL TOTAL VA AL HABER, EN EL DEBE VA EL GASTO NETO Y EL IVA...
                        //pero si es una nota de credito los del debe van al haber y viceversa
                        if (entradaLibro.TipoDocumento.EsUnaNotaCredito() == false)
                        {
                            detalleGastoNeto.MontoHaber = 0;
                            detalleGastoNeto.MontoDebe = CostoNeto + montoExento;
                        }
                        else
                        {
                            detalleGastoNeto.MontoHaber = CostoNeto + montoExento;
                            detalleGastoNeto.MontoDebe = 0;
                        }

                        detalleGastoNeto.GlosaDetalle = "Costo Neto " + FullDescripcionDocOriginal;

                        detalleGastoNeto.ObjCuentaContable = lstCuentaContable[contadorAnexo];

                        DetalleVoucher.Add(detalleGastoNeto);


                        //DetalleVoucherModel detalleGastoNetoCopiaIva = new DetalleVoucherModel();
                        //if (tipocompra == "IvaNoRecuperable" || tipocompra == "IvaUsoComun")
                        //{
                        //    detalleGastoNetoCopiaIva.FechaDoc = entradaLibro.FechaContabilizacion;
                        //    if (entradaLibro.TipoDocumento.EsUnaNotaCredito() == false)
                        //    {
                        //        detalleGastoNetoCopiaIva.MontoHaber = 0;
                        //        detalleGastoNetoCopiaIva.MontoDebe = CostoIvaNoRecuperable;
                        //    }
                        //    else
                        //    {
                        //        detalleGastoNetoCopiaIva.MontoHaber = CostoIvaNoRecuperable;
                        //        detalleGastoNetoCopiaIva.MontoDebe = 0;
                        //    }

                        //    detalleGastoNetoCopiaIva.GlosaDetalle = "Costo Iva No Recuperable " + FullDescripcionDocOriginal;

                        //    detalleGastoNetoCopiaIva.ObjCuentaContable = lstCuentaContable[contadorAnexo];

                        //    DetalleVoucher.Add(detalleGastoNetoCopiaIva);


                        //}
                        DetalleVoucherModel detalleGastoIVA = new DetalleVoucherModel();
                        // if (entradaLibro.MontoIva > 0)
                        //Genero lineas según tipoCompra
                        if (tipocompra == "IvaRecuperable" || tipocompra == "IvaUsoComun")
                        {
                            detalleGastoIVA.FechaDoc = entradaLibro.FechaContabilizacion;

                            decimal montoIvaTotal = MontoIva;

                            if (tipocompra == "IvaUsoComun")
                            {
                                montoIvaTotal = CostoIvaUsoComun; //CostoIvaNoRecuperable + CostoIvaUsoComun;
                            }

                            //EN COMPRA EL TOTAL VA AL HABER, EN EL DEBE VA EL GASTO NETO Y EL IVA...
                            //pero si es una nota de credito los del debe van al haber y viceversa
                            if (entradaLibro.TipoDocumento.EsUnaNotaCredito() == false)
                            {
                                detalleGastoIVA.MontoHaber = 0;
                                detalleGastoIVA.MontoDebe = montoIvaTotal;//MontoIva;
                            }
                            else
                            {
                                detalleGastoIVA.MontoHaber = montoIvaTotal;// MontoIva;
                                detalleGastoIVA.MontoDebe = 0;
                            }

                            detalleGastoIVA.GlosaDetalle = "IVA Compras " + FullDescripcionDocOriginal;
                            detalleGastoIVA.ObjCuentaContable = CuentaIVAAUsar;//objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == objCliente.ParametrosCliente.CuentaIvaVentas.CuentaContableModelID);
                            DetalleVoucher.Add(detalleGastoIVA);
                        }

                        DetalleVoucherModel detalleCtaCompra = new DetalleVoucherModel();
                        detalleCtaCompra.FechaDoc = entradaLibro.FechaContabilizacion;

                        //EN COMPRA EL TOTAL VA AL HABER, EN EL DEBE VA EL GASTO NETO Y EL IVA...
                        //pero si es una nota de credito los del debe van al haber y viceversa
                        if (tipocompra == "IvaUsoComun")
                        {
                            if (entradaLibro.TipoDocumento.EsUnaNotaCredito() == false)
                            {
                                detalleCtaCompra.MontoHaber = montoExento + CostoNeto + CostoIvaUsoComun;
                                detalleCtaCompra.MontoDebe = 0;
                            }
                            else
                            {
                                detalleCtaCompra.MontoHaber = 0;
                                detalleCtaCompra.MontoDebe = montoExento + CostoNeto + CostoIvaUsoComun;
                            }
                        }
                        else
                        {
                            if (entradaLibro.TipoDocumento.EsUnaNotaCredito() == false)
                            {
                                detalleCtaCompra.MontoHaber = montoExento + CostoNeto + MontoIva;
                                detalleCtaCompra.MontoDebe = 0;
                            }
                            else
                            {
                                detalleCtaCompra.MontoHaber = 0;
                                detalleCtaCompra.MontoDebe = montoExento + CostoNeto + MontoIva;
                            }
                        }

                        detalleCtaCompra.ObjCuentaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == objCliente.ParametrosCliente.CuentaCompras.CuentaContableModelID);
                        cuentaPrincipal = detalleCtaCompra.ObjCuentaContable;
                        detalleCtaCompra.GlosaDetalle = detalleCtaCompra.ObjCuentaContable.nombre + " (Compra) " + FullDescripcionDocOriginal;
                        DetalleVoucher.Add(detalleCtaCompra);
                    }

                    if (DetalleVoucher.Sum(r => r.MontoDebe) == DetalleVoucher.Sum(r => r.MontoHaber))
                    {
                        nuevoVoucher.ListaDetalleVoucher = DetalleVoucher;
                        entradaLibro.HaSidoConvertidoAVoucher = true;

                        //Convertimosa voucher solo si el libro ya es convertido a voucher.
                        List<ImpuestosAdRelacionModel> AConvertir = db.DBImpuestosAdRelacionSII.Where(x => x.CodigoUnionImpuesto == entradaLibro.CodigoUnionImpuesto).ToList();
                        if (AConvertir.Count != 0)
                        {
                            foreach (ImpuestosAdRelacionModel Convertidor in AConvertir)
                            {
                                Convertidor.HaSidoConvertidoAVoucher = true;
                                db.DBImpuestosAdRelacionSII.AddOrUpdate(Convertidor);
                                db.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        contadorAnexo++;
                        continue; //Pensar en como reportar que hubo problemas importando la linea X del libro
                    }

                    lstNuevosVouchers.Add(nuevoVoucher);

                    foreach (DetalleVoucherModel NuevoDetalleVoucher in nuevoVoucher.ListaDetalleVoucher)
                    {
                        if (NuevoDetalleVoucher.ObjCuentaContable == cuentaPrincipal)
                        {
                            AuxiliaresModel Auxiliar = new AuxiliaresModel();

                            Auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                            Auxiliar.LineaNumeroDetalle = nuevoVoucher.ListaDetalleVoucher.Count;
                            Auxiliar.MontoTotal = NuevoDetalleVoucher.MontoDebe + NuevoDetalleVoucher.MontoHaber;
                            Auxiliar.objCtaContable = NuevoDetalleVoucher.ObjCuentaContable;
                            //Auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                            db.DBAuxiliares.Add(Auxiliar);

                            AuxiliaresDetalleModel nuevoAuxDetalle = new AuxiliaresDetalleModel();
                            nuevoAuxDetalle.TipoDocumento = entradaLibro.TipoDocumento;
                            nuevoAuxDetalle.Fecha = entradaLibro.FechaDoc;
                            nuevoAuxDetalle.FechaContabilizacion = entradaLibro.FechaContabilizacion;

                            //revisar
                            // nuevoAuxDetalle.FechaVencimiento =   entradaLibro.fe
                            nuevoAuxDetalle.Folio = entradaLibro.Folio;
                            nuevoAuxDetalle.Individuo2 = entradaLibro.individuo;
                            nuevoAuxDetalle.MontoNetoLinea = entradaLibro.MontoNeto;
                            nuevoAuxDetalle.MontoExentoLinea = entradaLibro.MontoExento;
                            nuevoAuxDetalle.MontoIVALinea = entradaLibro.MontoIva;

                            nuevoAuxDetalle.MontoTotalLinea = entradaLibro.MontoTotal;
                            nuevoAuxDetalle.AuxiliaresModelID = Auxiliar.AuxiliaresModelID;

                            if (entradaLibro.TipoLibro == TipoCentralizacion.Compra)
                            {
                                nuevoAuxDetalle.MontoIVANoRecuperable = entradaLibro.MontoIvaNoRecuperable;
                                nuevoAuxDetalle.MontoIVAUsoComun = entradaLibro.MontoIvaUsocomun;
                                nuevoAuxDetalle.MontoIVAActivoFijo = entradaLibro.MontoIvaActivoFijo;
                            }
                            db.DBAuxiliaresDetalle.Add(nuevoAuxDetalle);
                            db.SaveChanges();
                        }
                    }
                    contadorAnexo++;
                    baseNumberFolio++;
                }

                if (lstNuevosVouchers != null && lstNuevosVouchers.Count > 0)
                {
                    foreach (VoucherModel NuevoVoucher in lstNuevosVouchers)
                    {
                        //objCliente.ListVoucher.Add(NuevoVoucher);
                        db.DBVoucher.Add(NuevoVoucher);
                    }
                    db.SaveChanges();
                    int posicion = 0;
                    foreach (VoucherModel NuevoVoucher in lstNuevosVouchers)
                    {
                        int posicion2 = 0;
                        foreach (LibrosContablesModel entradaLibro in lstEntradasLibro)
                        {
                            if (posicion == posicion2)
                            {
                                entradaLibro.VoucherModelID = NuevoVoucher.VoucherModelID;
                                entradaLibro.estado = true;
                                db.DBLibrosContables.AddOrUpdate(entradaLibro);
                                db.SaveChanges();
                            }
                            posicion2++;
                        }

                        int total = NuevoVoucher.ListaDetalleVoucher.Count;
                        foreach (DetalleVoucherModel NuevoDetalleVoucher in NuevoVoucher.ListaDetalleVoucher)
                        {
                            if (NuevoDetalleVoucher.ObjCuentaContable == cuentaPrincipal)
                            {
                                AuxiliaresModel auxiliar = (from c in db.DBAuxiliares
                                                            where c.LineaNumeroDetalle == total && c.objCtaContable.ClientesContablesModelID == cuentaPrincipal.ClientesContablesModelID && c.DetalleVoucherModelID == 0
                                                            select c).FirstOrDefault();
                                if (auxiliar != null)
                                {
                                    auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                                    db.DBAuxiliares.AddOrUpdate(auxiliar);

                                    NuevoDetalleVoucher.Auxiliar = auxiliar;
                                    db.DBDetalleVoucher.AddOrUpdate(NuevoDetalleVoucher);

                                    db.SaveChanges();
                                }
                            }
                        }
                        posicion++;
                    }
                }
                dbContextTransaction.Commit();
                FeedBack = "Exito";
                return FeedBack;
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                FeedBack = "Error inesperado " + ex.Message;
                return FeedBack;
            }
        }
    }

    public static void ProcesarLibroHonorarioAVoucher(List<LibroDeHonorariosModel> lstLibroHonorImport, ClientesContablesModel objCliente, FacturaPoliContext db, List<CuentaContableModel> lstCuentaContable)
    {
        if (lstLibroHonorImport == null || lstLibroHonorImport.Count == 0 || objCliente == null || objCliente.ParametrosCliente == null)
            return;


        if (lstLibroHonorImport.Count != lstCuentaContable.Count)
        {
            throw new Exception();
        }

        CuentaContableModel Retencion = null;
        CuentaContableModel Retencion2 = null;

        Retencion = db.DBCuentaContable.SingleOrDefault(x => x.CuentaContableModelID == objCliente.ParametrosCliente.CuentaRetencionHonorarios.CuentaContableModelID && x.ClientesContablesModelID == objCliente.ClientesContablesModelID);
        Retencion2 = db.DBCuentaContable.SingleOrDefault(x => x.CuentaContableModelID == objCliente.ParametrosCliente.CuentaRetencionesHonorarios2.CuentaContableModelID && x.ClientesContablesModelID == objCliente.ClientesContablesModelID);

        if (Retencion == null)
            return;

        List<VoucherModel> lstNuevosVouchers = new List<VoucherModel>();
        int contadorAnexo = 0;

        int? nullableProxVoucherNumber = ParseExtensions.ObtenerNumeroProximoVoucherINT(objCliente, db); //Contamos el prox voucher

        int baseNumberFolio = nullableProxVoucherNumber.Value;
        CuentaContableModel cuentaPrincipal = new CuentaContableModel();


        foreach (LibroDeHonorariosModel itemLibroHonor in lstLibroHonorImport)
        {
            decimal MontoBruto = itemLibroHonor.Brutos; // Debe
            decimal MontoRetenido = itemLibroHonor.Retenido; //Haber
            decimal MontoPagado = itemLibroHonor.Pagado; // Haber

            VoucherModel nuevoVoucher = new VoucherModel();

            nuevoVoucher.TipoOrigen = "Honorario";
            nuevoVoucher.TipoOrigenVoucher = TipoOrigen.Honorario;

            nuevoVoucher.ClientesContablesModelID = objCliente.ClientesContablesModelID;
            nuevoVoucher.FechaEmision = itemLibroHonor.FechaContabilizacion;
            nuevoVoucher.Tipo = itemLibroHonor.TipoVoucher; // Traspaso

            string FullDescripcionDocOriginal = lstCuentaContable[contadorAnexo].nombre + " / Folio: " + itemLibroHonor.NumIdenficiador + " / " + itemLibroHonor.Prestador.RazonSocial;

            nuevoVoucher.Glosa = FullDescripcionDocOriginal;
            nuevoVoucher.NumeroVoucher = baseNumberFolio;

            List<DetalleVoucherModel> DetalleVoucher = new List<DetalleVoucherModel>();

            DetalleVoucherModel DetalleVhonorarios = new DetalleVoucherModel(); // Linea 1 Del Voucher

            DetalleVhonorarios.FechaDoc = itemLibroHonor.FechaContabilizacion;

            DetalleVhonorarios.GlosaDetalle = "Honorarios Profesionales" + FullDescripcionDocOriginal;

            DetalleVhonorarios.MontoDebe = MontoBruto;
            DetalleVhonorarios.MontoHaber = 0;

            DetalleVhonorarios.ObjCuentaContable = lstCuentaContable[contadorAnexo];

            DetalleVoucher.Add(DetalleVhonorarios);

            DetalleVoucherModel DetalleVhonorarios2 = new DetalleVoucherModel(); //Linea 2 Del Voucher

            if(itemLibroHonor.Retenido > 0)
            {
                DetalleVhonorarios2.FechaDoc = itemLibroHonor.FechaContabilizacion;

                DetalleVhonorarios2.MontoDebe = 0;
                DetalleVhonorarios2.MontoHaber = MontoRetenido;
                DetalleVhonorarios2.ObjCuentaContable = Retencion;
               

                DetalleVhonorarios2.GlosaDetalle = Retencion.nombre + FullDescripcionDocOriginal;

                DetalleVoucher.Add(DetalleVhonorarios2);
            }

            DetalleVoucherModel DetalleVhonorarios2parte2 = new DetalleVoucherModel(); // Linea 3 del voucher

            if(itemLibroHonor.Pagado > 0)
            {
                DetalleVhonorarios2parte2.FechaDoc = itemLibroHonor.FechaContabilizacion;
                DetalleVhonorarios2parte2.MontoDebe = 0;
                DetalleVhonorarios2parte2.MontoHaber = MontoPagado;

                DetalleVhonorarios2parte2.GlosaDetalle = Retencion2.nombre + FullDescripcionDocOriginal;
                DetalleVhonorarios2parte2.ObjCuentaContable = Retencion2;
                cuentaPrincipal = Retencion2;

                DetalleVoucher.Add(DetalleVhonorarios2parte2);
            }

            if (DetalleVoucher.Sum(x => x.MontoDebe) == DetalleVoucher.Sum(x => x.MontoHaber))
            {
                nuevoVoucher.ListaDetalleVoucher = DetalleVoucher;
                itemLibroHonor.HaSidoConvertidoAVoucher = true;
            }
            else
            {
                contadorAnexo++;
                continue;
            }

            lstNuevosVouchers.Add(nuevoVoucher);

            foreach (DetalleVoucherModel NuevoDetalleVoucher in nuevoVoucher.ListaDetalleVoucher)
            {
                if(NuevoDetalleVoucher.ObjCuentaContable == cuentaPrincipal) { 
                    AuxiliaresModel Auxiliar = new AuxiliaresModel();

                    Auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                    Auxiliar.LineaNumeroDetalle = nuevoVoucher.ListaDetalleVoucher.Count;
                    Auxiliar.MontoTotal = NuevoDetalleVoucher.MontoDebe + NuevoDetalleVoucher.MontoHaber;
                    Auxiliar.objCtaContable = NuevoDetalleVoucher.ObjCuentaContable;
                    Auxiliar.Tipo = TipoAuxiliar.Honorarios;
                    db.DBAuxiliares.Add(Auxiliar);  

                    AuxiliaresDetalleModel nuevoAuxDetalle = new AuxiliaresDetalleModel();

                    nuevoAuxDetalle.Fecha = itemLibroHonor.Fecha;
                    nuevoAuxDetalle.FechaContabilizacion = itemLibroHonor.FechaContabilizacion;
                    nuevoAuxDetalle.Folio = itemLibroHonor.NumIdenficiador;
                    nuevoAuxDetalle.Individuo2 = itemLibroHonor.Prestador;
                    nuevoAuxDetalle.MontoBrutoLinea = MontoBruto;
                    nuevoAuxDetalle.ValorLiquido = MontoPagado;
                    nuevoAuxDetalle.ValorRetencion = MontoRetenido;
                    nuevoAuxDetalle.MontoTotalLinea = MontoPagado;
                    nuevoAuxDetalle.AuxiliaresModelID = Auxiliar.AuxiliaresModelID;

                    db.DBAuxiliaresDetalle.Add(nuevoAuxDetalle);
                    db.SaveChanges();

                    //decimal MontoBruto = itemLibroHonor.Brutos; // Debe
                    //decimal MontoRetenido = itemLibroHonor.Retenido; //Haber
                    //decimal MontoPagado = itemLibroHonor.Pagado; // Haber
                }
            }
            contadorAnexo++;
            baseNumberFolio++;

        }


        if (lstNuevosVouchers != null && lstNuevosVouchers.Count > 0)
        {
            foreach (VoucherModel NuevoVoucher in lstNuevosVouchers)
            {
                db.DBVoucher.Add(NuevoVoucher);
            }
            db.SaveChanges();
            int posicion = 0;
            foreach (VoucherModel NuevoVoucher in lstNuevosVouchers)
            {
                int posicion2 = 0;
                foreach (LibroDeHonorariosModel itemHonor in lstLibroHonorImport)
                {
                    if (posicion == posicion2)
                    {
                        itemHonor.VoucherModelID = NuevoVoucher.VoucherModelID;
                        db.DBLibroDeHonorarios.AddOrUpdate(itemHonor);
                        db.SaveChanges();
                    }
                    posicion2++;
                }

                int total = NuevoVoucher.ListaDetalleVoucher.Count;

                foreach (DetalleVoucherModel NuevoDetalleVoucher in NuevoVoucher.ListaDetalleVoucher)
                {
                    if(NuevoDetalleVoucher.ObjCuentaContable == cuentaPrincipal) { 

                        AuxiliaresModel auxiliar = db.DBAuxiliares.Where(x => x.LineaNumeroDetalle == total &&
                                                                              x.objCtaContable.ClientesContablesModelID == cuentaPrincipal.ClientesContablesModelID &&
                                                                              x.DetalleVoucherModelID == 0).FirstOrDefault();

                        if (auxiliar != null)
                        {
                            auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                            db.DBAuxiliares.AddOrUpdate(auxiliar);

                            NuevoDetalleVoucher.Auxiliar = auxiliar;
                            db.DBDetalleVoucher.AddOrUpdate(NuevoDetalleVoucher);
                            db.SaveChanges();
                        }
                    }
                }
                posicion++;
            }
            
        }


    }

    public static IQueryable<AuxiliaresDetalleModel> ObtenerLibrosPrestadores(ClientesContablesModel objCliente, FacturaPoliContext db,  string TipoReceptor = "",int Mes = 0, int Anio = 0, string RazonSocial = "", string Rut = "", string FechaInicio = "", string FechaFin = "",int Folio = 0)
    {

        bool ConversionFechaInicioExitosa = false;
        DateTime dtFechaInicio = new DateTime();
        bool ConversionFechaFinExitosa = false;
        DateTime dtFechaFin = new DateTime();

        if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
        {
            ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
            ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
        }

        
        IQueryable<AuxiliaresDetalleModel> TablaPrestador = (from Voucher in db.DBVoucher
                                                              join DetalleVoucher in db.DBDetalleVoucher on Voucher.VoucherModelID equals DetalleVoucher.VoucherModelID
                                                              join Auxiliares in db.DBAuxiliares on DetalleVoucher.DetalleVoucherModelID equals Auxiliares.DetalleVoucherModelID
                                                              join AuxiliaresDetalle in db.DBAuxiliaresDetalle on Auxiliares.AuxiliaresModelID equals AuxiliaresDetalle.AuxiliaresModelID

                                                              where Voucher.DadoDeBaja == false && Voucher.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                              Voucher.Tipo == TipoVoucher.Traspaso && AuxiliaresDetalle.Individuo2.tipoReceptor == TipoReceptor 

                                                              select AuxiliaresDetalle);

        if (Mes > 0)
            TablaPrestador = TablaPrestador.Where(x => x.FechaContabilizacion.Month == Mes);
        if (Anio > 0)
            TablaPrestador = TablaPrestador.Where(x => x.FechaContabilizacion.Year == Anio);
        if (!string.IsNullOrWhiteSpace(RazonSocial))
            TablaPrestador = TablaPrestador.Where(x => x.Individuo2.RazonSocial.Contains(RazonSocial));
        if (!string.IsNullOrWhiteSpace(Rut))
            TablaPrestador = TablaPrestador.Where(x => x.Individuo2.RUT.Contains(Rut));
        if (Folio > 0)
            TablaPrestador = TablaPrestador.Where(x => x.Folio == Folio);
        if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            TablaPrestador = TablaPrestador.Where(x => x.FechaContabilizacion >= dtFechaInicio && x.FechaContabilizacion <= dtFechaFin);


        return TablaPrestador;
    }

    public static PaginadorModel RescatarLibroCentralizacion(ClientesContablesModel objCliente, TipoCentralizacion tipoLibroCentralizacion, FacturaPoliContext db, string FechaInicio = "", string FechaFin = "", int Anio = 0, int Mes = 0, int pagina = 0, int cantidadRegistrosPorPagina = 0, string Rut = "", string RazonSocial = "", int Folio = 0)
    {
        bool ConversionFechaInicioExitosa = false;
        DateTime dtFechaInicio = new DateTime();
        bool ConversionFechaFinExitosa = false;
        DateTime dtFechaFin = new DateTime();

        if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
        {
            ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
            ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
        }

        // Solo si es convertido a voucher se listará en el libro de compras.

        IQueryable<LibrosContablesModel> lstlibro = (from LibrosContables in db.DBLibrosContables
                                                     join Vouchers in db.DBVoucher on LibrosContables.VoucherModelID equals Vouchers.VoucherModelID
                                                     where Vouchers.DadoDeBaja == false && LibrosContables.HaSidoConvertidoAVoucher == true &&
                                                     Vouchers.ClientesContablesModelID == objCliente.ClientesContablesModelID && Vouchers.Tipo == TipoVoucher.Traspaso &&
                                                     LibrosContables.TipoLibro == tipoLibroCentralizacion

                                                     select LibrosContables);


        //IQueryable<LibrosContablesModel> lstlibro = db.DBLibrosContables.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID && r.TipoLibro == tipoLibroCentralizacion &&  r.HaSidoConvertidoAVoucher == true);

        //(r => r.Fecha >= dtFechaInicio && r.Fecha <= dtFechaFin);

        if (Anio != 0 && Anio > 0)
            lstlibro = lstlibro.Where(r => r.FechaContabilizacion.Year == Anio);
        if (Mes != 0 && Mes > 0)
            lstlibro = lstlibro.Where(r => r.FechaContabilizacion.Month == Mes);
        if (ConversionFechaInicioExitosa && ConversionFechaInicioExitosa)
            lstlibro = lstlibro.Where(r => r.FechaContabilizacion >= dtFechaInicio && r.FechaContabilizacion <= dtFechaFin);
        if (!string.IsNullOrWhiteSpace(Rut))
            lstlibro = lstlibro.Where(r => r.individuo.RUT.Contains(Rut));
        if (!string.IsNullOrWhiteSpace(RazonSocial))
            lstlibro = lstlibro.Where(r => r.individuo.RazonSocial.Contains(RazonSocial));
        if (Folio != 0 && Folio > 0)
            lstlibro = lstlibro.Where(r => r.Folio == Folio);


        int totalDeRegistros = lstlibro.Count();
        if (cantidadRegistrosPorPagina != 0) {
            lstlibro = lstlibro.OrderBy(r => r.FechaContabilizacion)
                               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                               .Take(cantidadRegistrosPorPagina);

        } else if (cantidadRegistrosPorPagina == 0)
        {
            lstlibro = lstlibro.OrderBy(r => r.FechaContabilizacion);
        }

        List<string[]> ReturnValues = new List<string[]>();

        decimal TotalExento = 0;
        decimal TotalNeto = 0;
        decimal TotalIva = 0;
        decimal TotalIvaNoRecuperable = 0;
        decimal TotalIvaUsocomun = 0;
        decimal TotalMontoTotal = 0;


        int NumeroRow = 1;
        foreach (LibrosContablesModel Item in lstlibro.ToList())
        {

                string[] BalanceRow = new string[] { "-", "-", "-", "-", "-", "-", "-", "0", "0", "0", "0", "0", "0", "False" };
                BalanceRow[0] = NumeroRow.ToString();
                BalanceRow[1] = ParseExtensions.ToDD_MM_AAAA(Item.FechaDoc);
                BalanceRow[2] = ParseExtensions.ToDD_MM_AAAA(Item.FechaContabilizacion);
                BalanceRow[3] = ParseExtensions.EnumGetDisplayAttrib(Item.TipoDocumento);
                if(Item.FolioHasta > 0 && Item.Folio > 0)
                {
                BalanceRow[4] = Item.Folio.ToString() + " - " + Item.FolioHasta.ToString();
                }else { 
                    BalanceRow[4] = Item.Folio.ToString();
                }
                if (Item.individuo != null)
                {
                    //Nombre prestador
                    BalanceRow[5] = Item.individuo.RazonSocial;
                    //Rut prestador
                    BalanceRow[6] = Item.individuo.RUT;
                }
                else
                {
                    //Nombre prestador
                    BalanceRow[5] = "";
                    //Rut prestador
                    BalanceRow[6] = "";
                }

                BalanceRow[7] = ParseExtensions.NumeroConPuntosDeMiles(Item.MontoExento);
                TotalExento += Item.MontoExento;
                BalanceRow[8] = ParseExtensions.NumeroConPuntosDeMiles(Item.MontoNeto);
                TotalNeto += Item.MontoNeto;
                BalanceRow[9] = ParseExtensions.NumeroConPuntosDeMiles(Item.MontoIva);
                TotalIva += Item.MontoIva;
                BalanceRow[10] = ParseExtensions.NumeroConPuntosDeMiles(Item.MontoIvaNoRecuperable);
                TotalIvaNoRecuperable += Item.MontoIvaNoRecuperable;
                BalanceRow[11] = ParseExtensions.NumeroConPuntosDeMiles(Item.MontoIvaUsocomun);
                TotalIvaUsocomun += Item.MontoIvaUsocomun;
                BalanceRow[12] = ParseExtensions.NumeroConPuntosDeMiles(Item.MontoTotal);
                TotalMontoTotal += Item.MontoTotal;
                BalanceRow[13] = "True";

                if (Item.TipoDocumento.EsUnaNotaCredito() == false)
                {
                    BalanceRow[13] = "False";
                }

                ReturnValues.Add(BalanceRow);

                NumeroRow++;
            
        }

        string[] Totales = new string[] { "-", "-", "-", "-", "-", "-", "-", "0", "0", "0", "0", "0", "0", "False" };
        Totales[6] = "TOTAL: ";
        Totales[7] = ParseExtensions.NumeroConPuntosDeMiles(TotalExento);
        Totales[8] = ParseExtensions.NumeroConPuntosDeMiles(TotalNeto);
        Totales[9] = ParseExtensions.NumeroConPuntosDeMiles(TotalIva);
        Totales[10] = ParseExtensions.NumeroConPuntosDeMiles(TotalIvaNoRecuperable);
        Totales[11] = ParseExtensions.NumeroConPuntosDeMiles(TotalIvaUsocomun);
        Totales[12] = ParseExtensions.NumeroConPuntosDeMiles(TotalMontoTotal);

        ReturnValues.Add(Totales);



        var Paginador = new PaginadorModel();
        Paginador.ResultStringArray = ReturnValues;
        Paginador.PaginaActual = pagina;
        Paginador.TotalDeRegistros = totalDeRegistros;
        Paginador.RegistrosPorPagina = cantidadRegistrosPorPagina;
        Paginador.ValoresQueryString = new RouteValueDictionary();

        if (cantidadRegistrosPorPagina != 25)
            Paginador.ValoresQueryString["cantidadRegistrosPorPagina"] = cantidadRegistrosPorPagina;
        if (Anio != 0)
            Paginador.ValoresQueryString["Anio"] = Anio;
        if (Mes != 0)
            Paginador.ValoresQueryString["Mes"] = Mes;
        if (!string.IsNullOrWhiteSpace(Rut))
            Paginador.ValoresQueryString["Rut"] = Rut;
        if (!string.IsNullOrWhiteSpace(RazonSocial))
            Paginador.ValoresQueryString["RazonSocial"] = RazonSocial;
        if (ConversionFechaInicioExitosa && ConversionFechaInicioExitosa)
        {
            Paginador.ValoresQueryString["FechaInicio"] = FechaInicio;
            Paginador.ValoresQueryString["FechaFin"] = FechaFin;
        }

        return Paginador;
    }

}



