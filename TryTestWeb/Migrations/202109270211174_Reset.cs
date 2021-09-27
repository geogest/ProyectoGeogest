using System;
using System.Data.Entity.Migrations;

public partial class Reset : DbMigration
{
    public override void Up()
    {
        //CreateTable(
        //    "CertificadosModels",
        //    c => new
        //        {
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            CertificadosModelsID = c.Int(nullable: false),
        //            PathCAF33 = c.String(unicode: false),
        //            PathCAF34 = c.String(unicode: false),
        //            PathCAF61 = c.String(unicode: false),
        //            PathCAF56 = c.String(unicode: false),
        //            PathCAF110 = c.String(unicode: false),
        //            PathCAF52 = c.String(unicode: false),
        //            PathCAF39 = c.String(unicode: false),
        //            PathCAF41 = c.String(unicode: false),
        //            CertificatePath = c.String(unicode: false),
        //            CertificatePassword = c.String(unicode: false),
        //            PathCAF111 = c.String(unicode: false),
        //            PathCAF112 = c.String(unicode: false),
        //            PathCAF46 = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.QuickEmisorModelID)            
        //    .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID)
        //    .Index(t => t.QuickEmisorModelID);
        
        //CreateTable(
        //    "QuickEmisorModel",
        //    c => new
        //        {
        //            QuickEmisorModelID = c.Int(nullable: false, identity: true),
        //            IdentityIDEmisor = c.String(unicode: false),
        //            RUTEmpresa = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
        //            RUTUsuario = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
        //            RUTRepresentante = c.String(maxLength: 10, storeType: "nvarchar"),
        //            Representante = c.String(maxLength: 30, storeType: "nvarchar"),
        //            RazonSocial = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
        //            Direccion = c.String(unicode: false),
        //            Comuna = c.String(unicode: false),
        //            Ciudad = c.String(unicode: false),
        //            EMail = c.String(unicode: false),
        //            Giro = c.String(nullable: false, maxLength: 80, storeType: "nvarchar"),
        //            Telefono = c.String(unicode: false),
        //            idRegion = c.Int(nullable: false),
        //            idComuna = c.Int(nullable: false),
        //            CertificadosIDKey = c.Int(nullable: false),
        //            FechaResolucion = c.DateTime(nullable: false, precision: 0),
        //            NumeroResolucion = c.Int(nullable: false),
        //            CodigoSucursalSII = c.Int(nullable: false),
        //            maxUsuariosParaEstaEmpresa = c.Int(nullable: false),
        //            maxClientesContablesParaEstaEmpresa = c.Int(nullable: false),
        //        })
        //    .PrimaryKey(t => t.QuickEmisorModelID)            ;
        
        //CreateTable(
        //    "BoletasHonorariosModel",
        //    c => new
        //        {
        //            BoletasHonorariosModelID = c.Int(nullable: false, identity: true),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            NumeroBoleta = c.Int(nullable: false),
        //            Estado = c.Int(nullable: false),
        //            Fecha = c.DateTime(nullable: false, precision: 0),
        //            RUT_num = c.Int(nullable: false),
        //            RazonSocial = c.String(unicode: false),
        //            SociedadProfesional = c.Boolean(nullable: false),
        //            Brutos = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Retenido = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Liquido = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            ConSinRetencion = c.Int(nullable: false),
        //            RUT_txt = c.String(unicode: false),
        //            base64ArchivoAsociado = c.String(unicode: false),
        //            TipoArchivoAsociado = c.String(unicode: false),
        //            GlosaDescripcion = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.BoletasHonorariosModelID)            
        //    .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
        //    .Index(t => t.QuickEmisorModelID);
        
        //CreateTable(
        //    "DTEPagosModel",
        //    c => new
        //        {
        //            DTEPagosModelID = c.Int(nullable: false, identity: true),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            MontoPago = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            FechaPago = c.DateTime(nullable: false, precision: 0),
        //            FormaDePago = c.Int(nullable: false),
        //            FolioDocAsociado = c.Int(nullable: false),
        //            TipoDePago = c.Int(nullable: false),
        //            TipoDocumentoPagado = c.Int(nullable: false),
        //            FolioDocumentoPagado = c.Int(nullable: false),
        //            BoletasHonorariosModel_BoletasHonorariosModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.DTEPagosModelID)            
        //    .ForeignKey("BoletasHonorariosModel", t => t.BoletasHonorariosModel_BoletasHonorariosModelID)
        //    .Index(t => t.BoletasHonorariosModel_BoletasHonorariosModelID);
        
        //CreateTable(
        //    "QuickReceptorModel",
        //    c => new
        //        {
        //            QuickReceptorModelID = c.Int(nullable: false, identity: true),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            RUT = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
        //            NombreFantasia = c.String(unicode: false),
        //            RazonSocial = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
        //            Direccion = c.String(unicode: false),
        //            Comuna = c.String(unicode: false),
        //            Ciudad = c.String(unicode: false),
        //            Giro = c.String(unicode: false),
        //            Contacto = c.String(unicode: false),
        //            NombreContacto = c.String(unicode: false),
        //            RUTSolicitante = c.String(unicode: false),
        //            TelefonoContacto = c.String(unicode: false),
        //            idRegion = c.Int(nullable: false),
        //            idComuna = c.Int(nullable: false),
        //            tipoReceptor = c.String(unicode: false),
        //            DadoDeBaja = c.Boolean(nullable: false),
        //            CuentaConToReceptor_CuentaContableModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.QuickReceptorModelID)            
        //    .ForeignKey("CuentaContableModel", t => t.CuentaConToReceptor_CuentaContableModelID)
        //    .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
        //    .Index(t => t.QuickEmisorModelID)
        //    .Index(t => t.CuentaConToReceptor_CuentaContableModelID);
        
        //CreateTable(
        //    "CuentaContableModel",
        //    c => new
        //        {
        //            CuentaContableModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            nombre = c.String(unicode: false),
        //            CodInterno = c.String(unicode: false),
        //            Clasificacion = c.Int(nullable: false),
        //            AnalisisDeCuenta = c.Boolean(nullable: false),
        //            InvertirMontosDepreciacion = c.Boolean(nullable: false),
        //            TipoAuxiliarQueUtiliza = c.Int(nullable: false),
        //            TipoCentralizacionAuxiliares = c.Int(nullable: false),
        //            ItemsModelID = c.Int(nullable: false),
        //            CentroCostosModelID = c.Int(nullable: false),
        //            AnalisisContablesModelID = c.Int(nullable: false),
        //            Concilaciones = c.Int(nullable: false),
        //            TieneAuxiliar = c.Int(nullable: false),
        //            TieneCentroDeCosto = c.Int(nullable: false),
        //            SubClasificacion_SubClasificacionCtaContableID = c.Int(),
        //            SubSubClasificacion_SubSubClasificacionCtaContableID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.CuentaContableModelID)            
        //    .ForeignKey("SubClasificacionCtaContable", t => t.SubClasificacion_SubClasificacionCtaContableID)
        //    .ForeignKey("SubSubClasificacionCtaContable", t => t.SubSubClasificacion_SubSubClasificacionCtaContableID)
        //    .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModelID, cascadeDelete: true)
        //    .Index(t => t.ClientesContablesModelID)
        //    .Index(t => t.SubClasificacion_SubClasificacionCtaContableID)
        //    .Index(t => t.SubSubClasificacion_SubSubClasificacionCtaContableID);
        
        //CreateTable(
        //    "SubClasificacionCtaContable",
        //    c => new
        //        {
        //            SubClasificacionCtaContableID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            CodigoInterno = c.String(unicode: false),
        //            NombreInterno = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.SubClasificacionCtaContableID)            ;
        
        //CreateTable(
        //    "SubSubClasificacionCtaContable",
        //    c => new
        //        {
        //            SubSubClasificacionCtaContableID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            CodigoInterno = c.String(unicode: false),
        //            NombreInterno = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.SubSubClasificacionCtaContableID)            ;
        
        //CreateTable(
        //    "ActividadEconomicaModel",
        //    c => new
        //        {
        //            ActividadEconomicaModelID = c.Int(nullable: false, identity: true),
        //            CodigoInterno = c.String(unicode: false),
        //            NombreActividad = c.String(unicode: false),
        //            AfectoAIva = c.Int(nullable: false),
        //            CategoriaTributaria = c.Int(nullable: false),
        //            DisponibleInternet = c.Boolean(nullable: false),
        //        })
        //    .PrimaryKey(t => t.ActividadEconomicaModelID)            ;
        
        //CreateTable(
        //    "ClientesContablesModel",
        //    c => new
        //        {
        //            ClientesContablesModelID = c.Int(nullable: false, identity: true),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            RUTEmpresa = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
        //            RazonSocial = c.String(nullable: false, unicode: false),
        //            Direccion = c.String(unicode: false),
        //            Ciudad = c.String(unicode: false),
        //            ActEcono = c.String(unicode: false),
        //            Telefono = c.String(nullable: false, unicode: false),
        //            Giro = c.String(nullable: false, unicode: false),
        //            RUTRepresentante = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
        //            Representante = c.String(unicode: false),
        //            Email = c.String(unicode: false),
        //            idRegion = c.Int(nullable: false),
        //            idComuna = c.Int(nullable: false),
        //            Comuna_ComunaModelsID = c.Int(),
        //            ParametrosCliente_ParametrosClienteModelID = c.Int(),
        //            UsuarioModel_UsuarioModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.ClientesContablesModelID)            
        //    .ForeignKey("ComunaModels", t => t.Comuna_ComunaModelsID)
        //    .ForeignKey("ParametrosClienteModel", t => t.ParametrosCliente_ParametrosClienteModelID)
        //    .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
        //    .ForeignKey("UsuarioModel", t => t.UsuarioModel_UsuarioModelID)
        //    .Index(t => t.QuickEmisorModelID)
        //    .Index(t => t.Comuna_ComunaModelsID)
        //    .Index(t => t.ParametrosCliente_ParametrosClienteModelID)
        //    .Index(t => t.UsuarioModel_UsuarioModelID);
        
        //CreateTable(
        //    "ComunaModels",
        //    c => new
        //        {
        //            ComunaModelsID = c.Int(nullable: false, identity: true),
        //            nombre = c.String(unicode: false),
        //            idRegion = c.Int(nullable: false),
        //        })
        //    .PrimaryKey(t => t.ComunaModelsID)            ;
        
        //CreateTable(
        //    "CentroCostoModel",
        //    c => new
        //        {
        //            CentroCostoModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            contador = c.Int(nullable: false),
        //            Nombre = c.String(nullable: false, unicode: false),
        //        })
        //    .PrimaryKey(t => t.CentroCostoModelID)            
        //    .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModelID, cascadeDelete: true)
        //    .Index(t => t.ClientesContablesModelID);
        
        //CreateTable(
        //    "ItemModel",
        //    c => new
        //        {
        //            ItemModelID = c.Int(nullable: false, identity: true),
        //            ClienteContableID = c.Int(nullable: false),
        //            NombreItem = c.String(nullable: false, unicode: false),
        //            Estado = c.Boolean(nullable: false),
        //            contador = c.Int(nullable: false),
        //            ClientesContablesModel_ClientesContablesModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.ItemModelID)            
        //    .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModel_ClientesContablesModelID)
        //    .Index(t => t.ClientesContablesModel_ClientesContablesModelID);
        
        //CreateTable(
        //    "LibrosContablesModel",
        //    c => new
        //        {
        //            LibrosContablesModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            VoucherModelID = c.Int(nullable: false),
        //            CodigoUnionImpuesto = c.Int(nullable: false),
        //            TipoLibro = c.Int(nullable: false),
        //            TipoDocumento = c.Int(nullable: false),
        //            Folio = c.Int(nullable: false),
        //            FolioHasta = c.Int(nullable: false),
        //            FechaDoc = c.DateTime(nullable: false, precision: 0),
        //            FechaRecep = c.DateTime(nullable: false, precision: 0),
        //            FechaContabilizacion = c.DateTime(nullable: false, precision: 0),
        //            MontoExento = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoNeto = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoIva = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            TipoDocReferencia = c.Int(nullable: false),
        //            FolioDocReferencia = c.Int(),
        //            HaSidoConvertidoAVoucher = c.Boolean(nullable: false),
        //            MontoIvaNoRecuperable = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoIvaUsocomun = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoIvaActivoFijo = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            estado = c.Boolean(nullable: false),
        //            EsUnRegistroManual = c.Boolean(nullable: false),
        //            individuo_QuickReceptorModelID = c.Int(),
        //            Prestador_AuxiliaresPrestadoresModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.LibrosContablesModelID)            
        //    .ForeignKey("QuickReceptorModel", t => t.individuo_QuickReceptorModelID)
        //    .ForeignKey("AuxiliaresPrestadoresModel", t => t.Prestador_AuxiliaresPrestadoresModelID)
        //    .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModelID, cascadeDelete: true)
        //    .Index(t => t.ClientesContablesModelID)
        //    .Index(t => t.individuo_QuickReceptorModelID)
        //    .Index(t => t.Prestador_AuxiliaresPrestadoresModelID);
        
        //CreateTable(
        //    "AuxiliaresPrestadoresModel",
        //    c => new
        //        {
        //            AuxiliaresPrestadoresModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            PrestadorRut = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
        //            PrestadorNombre = c.String(nullable: false, unicode: false),
        //        })
        //    .PrimaryKey(t => t.AuxiliaresPrestadoresModelID)            ;
        
        //CreateTable(
        //    "VoucherModel",
        //    c => new
        //        {
        //            VoucherModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            Glosa = c.String(unicode: false),
        //            FechaEmision = c.DateTime(nullable: false, precision: 0),
        //            Tipo = c.Int(nullable: false),
        //            TipoOrigen = c.String(unicode: false),
        //            TipoOrigenVoucher = c.Int(nullable: false),
        //            NumeroVoucher = c.Int(nullable: false),
        //            DadoDeBaja = c.Boolean(nullable: false),
        //            ClientesProveedoresModelID = c.Int(nullable: false),
        //            CentroDeCosto_CentroCostoModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.VoucherModelID)            
        //    .ForeignKey("CentroCostoModel", t => t.CentroDeCosto_CentroCostoModelID)
        //    .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModelID, cascadeDelete: true)
        //    .Index(t => t.ClientesContablesModelID)
        //    .Index(t => t.CentroDeCosto_CentroCostoModelID);
        
        //CreateTable(
        //    "DetalleVoucherModel",
        //    c => new
        //        {
        //            DetalleVoucherModelID = c.Int(nullable: false, identity: true),
        //            VoucherModelID = c.Int(nullable: false),
        //            MontoDebe = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoHaber = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Conciliado = c.Boolean(nullable: false),
        //            ConciliadoCtasCtes = c.Boolean(nullable: false),
        //            GlosaDetalle = c.String(unicode: false),
        //            FechaDoc = c.DateTime(nullable: false, precision: 0),
        //            RUTDoc = c.String(unicode: false),
        //            Pago = c.Int(nullable: false),
        //            FechaVenc = c.DateTime(nullable: false, precision: 0),
        //            RazonSocialDoc = c.String(unicode: false),
        //            DocDePago = c.String(unicode: false),
        //            ItemModelID = c.Int(nullable: false),
        //            CentroCostoID = c.Int(nullable: false),
        //            ClienteProveedor = c.Int(nullable: false),
        //            Auxiliar_AuxiliaresModelID = c.Int(),
        //            objCentroCostro_CentroCostoModelID = c.Int(),
        //            ObjCuentaContable_CuentaContableModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.DetalleVoucherModelID)            
        //    .ForeignKey("AuxiliaresModel", t => t.Auxiliar_AuxiliaresModelID)
        //    .ForeignKey("CentroCostoModel", t => t.objCentroCostro_CentroCostoModelID)
        //    .ForeignKey("CuentaContableModel", t => t.ObjCuentaContable_CuentaContableModelID)
        //    .ForeignKey("VoucherModel", t => t.VoucherModelID, cascadeDelete: true)
        //    .Index(t => t.VoucherModelID)
        //    .Index(t => t.Auxiliar_AuxiliaresModelID)
        //    .Index(t => t.objCentroCostro_CentroCostoModelID)
        //    .Index(t => t.ObjCuentaContable_CuentaContableModelID);
        
        //CreateTable(
        //    "AuxiliaresModel",
        //    c => new
        //        {
        //            AuxiliaresModelID = c.Int(nullable: false, identity: true),
        //            DetalleVoucherModelID = c.Int(nullable: false),
        //            LineaNumeroDetalle = c.Int(nullable: false),
        //            MontoTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Tipo = c.Int(nullable: false),
        //            objCtaContable_CuentaContableModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.AuxiliaresModelID)            
        //    .ForeignKey("CuentaContableModel", t => t.objCtaContable_CuentaContableModelID)
        //    .Index(t => t.objCtaContable_CuentaContableModelID);
        
        //CreateTable(
        //    "AuxiliaresDetalleModel",
        //    c => new
        //        {
        //            AuxiliaresDetalleModelID = c.Int(nullable: false, identity: true),
        //            AuxiliaresModelID = c.Int(nullable: false),
        //            NumeroCorrelativo = c.Int(nullable: false),
        //            Fecha = c.DateTime(nullable: false, precision: 0),
        //            FechaContabilizacion = c.DateTime(nullable: false, precision: 0),
        //            TipoDocumento = c.Int(nullable: false),
        //            Folio = c.Int(nullable: false),
        //            FolioHasta = c.Int(nullable: false),
        //            MontoBrutoLinea = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoNetoLinea = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoExentoLinea = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoIVALinea = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoIVANoRecuperable = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoIVAUsoComun = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoIVAActivoFijo = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            ValorLiquido = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            ValorRetencion = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            MontoTotalLinea = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            FechaVencimiento = c.DateTime(nullable: false, precision: 0),
        //            SeVaParaVenta = c.Boolean(nullable: false),
        //            SeVaParaCompra = c.Boolean(nullable: false),
        //            Individuo_AuxiliaresPrestadoresModelID = c.Int(),
        //            Individuo2_QuickReceptorModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.AuxiliaresDetalleModelID)            
        //    .ForeignKey("AuxiliaresPrestadoresModel", t => t.Individuo_AuxiliaresPrestadoresModelID)
        //    .ForeignKey("QuickReceptorModel", t => t.Individuo2_QuickReceptorModelID)
        //    .ForeignKey("AuxiliaresModel", t => t.AuxiliaresModelID, cascadeDelete: true)
        //    .Index(t => t.AuxiliaresModelID)
        //    .Index(t => t.Individuo_AuxiliaresPrestadoresModelID)
        //    .Index(t => t.Individuo2_QuickReceptorModelID);
        
        //CreateTable(
        //    "ParametrosClienteModel",
        //    c => new
        //        {
        //            ParametrosClienteModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            CuentaCompras_CuentaContableModelID = c.Int(),
        //            CuentaIvaCompras_CuentaContableModelID = c.Int(),
        //            CuentaIvaVentas_CuentaContableModelID = c.Int(),
        //            CuentaRetencionesHonorarios2_CuentaContableModelID = c.Int(),
        //            CuentaRetencionHonorarios_CuentaContableModelID = c.Int(),
        //            CuentaVentas_CuentaContableModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.ParametrosClienteModelID)            
        //    .ForeignKey("CuentaContableModel", t => t.CuentaCompras_CuentaContableModelID)
        //    .ForeignKey("CuentaContableModel", t => t.CuentaIvaCompras_CuentaContableModelID)
        //    .ForeignKey("CuentaContableModel", t => t.CuentaIvaVentas_CuentaContableModelID)
        //    .ForeignKey("CuentaContableModel", t => t.CuentaRetencionesHonorarios2_CuentaContableModelID)
        //    .ForeignKey("CuentaContableModel", t => t.CuentaRetencionHonorarios_CuentaContableModelID)
        //    .ForeignKey("CuentaContableModel", t => t.CuentaVentas_CuentaContableModelID)
        //    .Index(t => t.CuentaCompras_CuentaContableModelID)
        //    .Index(t => t.CuentaIvaCompras_CuentaContableModelID)
        //    .Index(t => t.CuentaIvaVentas_CuentaContableModelID)
        //    .Index(t => t.CuentaRetencionesHonorarios2_CuentaContableModelID)
        //    .Index(t => t.CuentaRetencionHonorarios_CuentaContableModelID)
        //    .Index(t => t.CuentaVentas_CuentaContableModelID);
        
        //CreateTable(
        //    "ReportesImpagosLog",
        //    c => new
        //        {
        //            ReportesImpagosLogID = c.Int(nullable: false, identity: true),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            MensajeCorreoEnviado = c.String(unicode: false),
        //            DireccionCorreoEnviada = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.ReportesImpagosLogID)            
        //    .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
        //    .Index(t => t.QuickEmisorModelID);
        
        //CreateTable(
        //    "ActividadEconomicaModelCuentaContableModel",
        //    c => new
        //        {
        //            ActividadEconomicaModelCuentaContableModelID = c.Int(nullable: false, identity: true),
        //            CodigoInterno = c.String(unicode: false),
        //            ClienteContableModelID = c.Int(nullable: false),
        //        })
        //    .PrimaryKey(t => t.ActividadEconomicaModelCuentaContableModelID)            ;
        
        //CreateTable(
        //    "BoletasCoVPadreModel",
        //    c => new
        //        {
        //            BoletasCoVPadreModelID = c.Int(nullable: false, identity: true),
        //            FechaBoletas = c.DateTime(nullable: false, precision: 0),
        //            FechaCreacion = c.DateTime(nullable: false, precision: 0),
        //            tipoCentralizacion = c.Int(nullable: false),
        //            TotalNeto = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            TotalIva = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            ClienteContableModelID_ClientesContablesModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.BoletasCoVPadreModelID)            
        //    .ForeignKey("ClientesContablesModel", t => t.ClienteContableModelID_ClientesContablesModelID)
        //    .Index(t => t.ClienteContableModelID_ClientesContablesModelID);
        
        //CreateTable(
        //    "BoletasCoVModel",
        //    c => new
        //        {
        //            BoletasCoVModelID = c.Int(nullable: false, identity: true),
        //            CuentaAuxiliar = c.String(unicode: false),
        //            VoucherModelID = c.Int(nullable: false),
        //            HaSidoConvertidoAVoucher = c.Int(nullable: false),
        //            FechaInsercion = c.DateTime(nullable: false, precision: 0),
        //            Fecha = c.DateTime(nullable: false, precision: 0),
        //            NumeroDeDocumento = c.Int(nullable: false),
        //            TipoDocumento = c.Int(nullable: false),
        //            FechaVencimiento = c.DateTime(nullable: false, precision: 0),
        //            CuentaContable = c.String(unicode: false),
        //            Neto = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Iva = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            CentroDeCostos = c.Int(nullable: false),
        //            FechaPeriodoTributario = c.DateTime(nullable: false, precision: 0),
        //            BoletaCoVPadre_BoletasCoVPadreModelID = c.Int(),
        //            ClienteContable_ClientesContablesModelID = c.Int(),
        //            Prestador_QuickReceptorModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.BoletasCoVModelID)            
        //    .ForeignKey("BoletasCoVPadreModel", t => t.BoletaCoVPadre_BoletasCoVPadreModelID)
        //    .ForeignKey("ClientesContablesModel", t => t.ClienteContable_ClientesContablesModelID)
        //    .ForeignKey("QuickReceptorModel", t => t.Prestador_QuickReceptorModelID)
        //    .Index(t => t.BoletaCoVPadre_BoletasCoVPadreModelID)
        //    .Index(t => t.ClienteContable_ClientesContablesModelID)
        //    .Index(t => t.Prestador_QuickReceptorModelID);
        
        //CreateTable(
        //    "CartolaBancariaModel",
        //    c => new
        //        {
        //            CartolaBancariaModelId = c.Int(nullable: false, identity: true),
        //            VoucherModelID = c.Int(nullable: false),
        //            Fecha = c.DateTime(nullable: false, precision: 0),
        //            Folio = c.Int(nullable: false),
        //            Detalle = c.String(unicode: false),
        //            Oficina = c.String(unicode: false),
        //            Debe = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Haber = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Saldo = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            EstaConciliado = c.Boolean(nullable: false),
        //            CartolaBancariaMacroModelID_CartolaBancariaMacroModelID = c.Int(),
        //            ClientesContablesModelID_ClientesContablesModelID = c.Int(),
        //            CuentaContableModelID_CuentaContableModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.CartolaBancariaModelId)            
        //    .ForeignKey("CartolaBancariaMacroModel", t => t.CartolaBancariaMacroModelID_CartolaBancariaMacroModelID)
        //    .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModelID_ClientesContablesModelID)
        //    .ForeignKey("CuentaContableModel", t => t.CuentaContableModelID_CuentaContableModelID)
        //    .Index(t => t.CartolaBancariaMacroModelID_CartolaBancariaMacroModelID)
        //    .Index(t => t.ClientesContablesModelID_ClientesContablesModelID)
        //    .Index(t => t.CuentaContableModelID_CuentaContableModelID);
        
        //CreateTable(
        //    "CartolaBancariaMacroModel",
        //    c => new
        //        {
        //            CartolaBancariaMacroModelID = c.Int(nullable: false, identity: true),
        //            FechaCartola = c.DateTime(nullable: false, precision: 0),
        //            NumeroCartola = c.Int(nullable: false),
        //            TotalCartola = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            ClientesContablesModelID_ClientesContablesModelID = c.Int(),
        //            CuentaContableModelID_CuentaContableModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.CartolaBancariaMacroModelID)            
        //    .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModelID_ClientesContablesModelID)
        //    .ForeignKey("CuentaContableModel", t => t.CuentaContableModelID_CuentaContableModelID)
        //    .Index(t => t.ClientesContablesModelID_ClientesContablesModelID)
        //    .Index(t => t.CuentaContableModelID_CuentaContableModelID);
        
        //CreateTable(
        //    "CtasContablesPresupuestoModel",
        //    c => new
        //        {
        //            CtasContablesPresupuestoModelID = c.Int(nullable: false, identity: true),
        //            PresupuestoModelID = c.Int(nullable: false),
        //            CuentasContablesModelID = c.Int(nullable: false),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            Presupuesto = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            FechaInicioPresu = c.DateTime(nullable: false, precision: 0),
        //            FechaVencimientoPresu = c.DateTime(nullable: false, precision: 0),
        //        })
        //    .PrimaryKey(t => t.CtasContablesPresupuestoModelID)            ;
        
        //CreateTable(
        //    "CentroCostoPresupuestoModels",
        //    c => new
        //        {
        //            CentroCostoPresupuestoModelsID = c.Int(nullable: false, identity: true),
        //            CentroCostoModelID = c.Int(nullable: false),
        //            ClienteContableModelID = c.Int(nullable: false),
        //            Presupuesto = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            FechaInicioPresu = c.DateTime(nullable: false, precision: 0),
        //            FechaVencimientoPresu = c.DateTime(nullable: false, precision: 0),
        //        })
        //    .PrimaryKey(t => t.CentroCostoPresupuestoModelsID)            ;
        
        //CreateTable(
        //    "ClientesContablesEmisorModel",
        //    c => new
        //        {
        //            ClientesContablesEmisorModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            QuickReceptorModelID = c.Int(nullable: false),
        //        })
        //    .PrimaryKey(t => t.ClientesContablesEmisorModelID)            ;
        
        //CreateTable(
        //    "ClientesProveedoresModel",
        //    c => new
        //        {
        //            ClientesProveedoresModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            Rut = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
        //            Nombre = c.String(unicode: false),
        //            Tipo = c.Int(nullable: false),
        //            Estado = c.Boolean(nullable: false),
        //        })
        //    .PrimaryKey(t => t.ClientesProveedoresModelID)            ;
        
        //CreateTable(
        //    "EmisoresHabilitados",
        //    c => new
        //        {
        //            EmisoresHabilitadosID = c.Int(nullable: false, identity: true),
        //            UsuarioModelID = c.Int(nullable: false),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            privilegiosAcceso = c.Int(nullable: false),
        //        })
        //    .PrimaryKey(t => t.EmisoresHabilitadosID)            
        //    .ForeignKey("UsuarioModel", t => t.UsuarioModelID, cascadeDelete: true)
        //    .Index(t => t.UsuarioModelID);
        
        //CreateTable(
        //    "ErrorMensajeMonitoreo",
        //    c => new
        //        {
        //            ErrorMensajeMonitoreoID = c.Int(nullable: false, identity: true),
        //            Mensaje = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.ErrorMensajeMonitoreoID)            ;
        
        //CreateTable(
        //    "FuncionesModel",
        //    c => new
        //        {
        //            FuncionesModelID = c.Int(nullable: false, identity: true),
        //            NombreFuncion = c.String(unicode: false),
        //            NombreModulo = c.String(unicode: false),
        //            ModuloSistema_ModuloSistemaModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.FuncionesModelID)            
        //    .ForeignKey("ModuloSistemaModel", t => t.ModuloSistema_ModuloSistemaModelID)
        //    .Index(t => t.ModuloSistema_ModuloSistemaModelID);
        
        //CreateTable(
        //    "ModuloSistemaModel",
        //    c => new
        //        {
        //            ModuloSistemaModelID = c.Int(nullable: false, identity: true),
        //            NombreModuloSistema = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.ModuloSistemaModelID)            ;
        
        //CreateTable(
        //    "ImpuestosAdicionalesModel",
        //    c => new
        //        {
        //            ImpuestosAdicionalesModelID = c.Int(nullable: false, identity: true),
        //            CodigoImpuesto = c.Int(nullable: false),
        //            CodigoImpRetencion = c.Int(nullable: false),
        //            NombreImpuesto = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.ImpuestosAdicionalesModelID)            ;
        
        //CreateTable(
        //    "ImpuestosAdRelacionModel",
        //    c => new
        //        {
        //            ImpuestosAdRelacionModelID = c.Int(nullable: false, identity: true),
        //            CodigoUnionImpuesto = c.Int(nullable: false),
        //            ImpuestosAdicionalesModelID = c.Int(nullable: false),
        //            ClienteContableModelID = c.Int(nullable: false),
        //            CodigoImpuesto = c.Int(nullable: false),
        //            HaSidoConvertidoAVoucher = c.Boolean(nullable: false),
        //        })
        //    .PrimaryKey(t => t.ImpuestosAdRelacionModelID)            ;
        
        //CreateTable(
        //    "LibroDeHonorariosModel",
        //    c => new
        //        {
        //            LibroDeHonorariosModelID = c.Int(nullable: false, identity: true),
        //            ClientesContablesID = c.Int(nullable: false),
        //            VoucherModelID = c.Int(nullable: false),
        //            NumIdenficiador = c.Int(nullable: false),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            TipoLibro = c.Int(nullable: false),
        //            SocProf = c.String(unicode: false),
        //            Fecha = c.DateTime(nullable: false, precision: 0),
        //            FechaContabilizacion = c.DateTime(nullable: false, precision: 0),
        //            Estado = c.String(unicode: false),
        //            FechaAnulacion = c.DateTime(nullable: false, precision: 0),
        //            Rut = c.String(unicode: false),
        //            RazonSocial = c.String(unicode: false),
        //            Brutos = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Retenido = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Pagado = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            TipoVoucher = c.Int(nullable: false),
        //            HaSidoConvertidoAVoucher = c.Boolean(nullable: false),
        //            Prestador_QuickReceptorModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.LibroDeHonorariosModelID)            
        //    .ForeignKey("QuickReceptorModel", t => t.Prestador_QuickReceptorModelID)
        //    .Index(t => t.Prestador_QuickReceptorModelID);
        
        //CreateTable(
        //    "LibroHonorariosDeTerceros",
        //    c => new
        //        {
        //            LibroHonorariosDeTercerosID = c.Int(nullable: false, identity: true),
        //            NumOFolio = c.Int(nullable: false),
        //            Estado = c.String(unicode: false),
        //            FechaInicial = c.DateTime(nullable: false, precision: 0),
        //            RutEmpresa = c.String(unicode: false),
        //            NombreEmpresa = c.String(unicode: false),
        //            FechaFinal = c.DateTime(nullable: false, precision: 0),
        //            FechaContabilizacion = c.DateTime(nullable: false, precision: 0),
        //            RutReceptor = c.String(unicode: false),
        //            NombreReceptor = c.String(unicode: false),
        //            Brutos = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Retenidos = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            Pagado = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            TipoLibro = c.Int(nullable: false),
        //            TipoV = c.Int(nullable: false),
        //            TipoO = c.Int(nullable: false),
        //            HaSidoConvertidoAVoucher = c.Boolean(nullable: false),
        //            FechaDeCreacion = c.DateTime(nullable: false, precision: 0),
        //            ClienteContable_ClientesContablesModelID = c.Int(),
        //            Receptor_QuickReceptorModelID = c.Int(),
        //            VoucherModel_VoucherModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.LibroHonorariosDeTercerosID)            
        //    .ForeignKey("ClientesContablesModel", t => t.ClienteContable_ClientesContablesModelID)
        //    .ForeignKey("QuickReceptorModel", t => t.Receptor_QuickReceptorModelID)
        //    .ForeignKey("VoucherModel", t => t.VoucherModel_VoucherModelID)
        //    .Index(t => t.ClienteContable_ClientesContablesModelID)
        //    .Index(t => t.Receptor_QuickReceptorModelID)
        //    .Index(t => t.VoucherModel_VoucherModelID);
        
        //CreateTable(
        //    "ModulosHabilitados",
        //    c => new
        //        {
        //            ModulosHabilitadosID = c.Int(nullable: false, identity: true),
        //            UsuarioModelID = c.Int(nullable: false),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            privilegiosAcceso = c.Int(nullable: false),
        //            Funcion_FuncionesModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.ModulosHabilitadosID)            
        //    .ForeignKey("FuncionesModel", t => t.Funcion_FuncionesModelID)
        //    .Index(t => t.Funcion_FuncionesModelID);
        
        //CreateTable(
        //    "MonitoreoModel",
        //    c => new
        //        {
        //            MonitoreoModelID = c.Int(nullable: false, identity: true),
        //            NombreUsuario = c.String(unicode: false),
        //            Controlador = c.String(unicode: false),
        //            AccionTipo = c.Int(nullable: false),
        //            CambiosRealizados = c.String(unicode: false),
        //            QueryStrings = c.String(unicode: false),
        //            AccionNombre = c.String(unicode: false),
        //            CompaniaSeleccionadaNombre = c.String(unicode: false),
        //            ClienteSeleccionadoNombre = c.String(unicode: false),
        //            Tiempo_de_ejecucion = c.Time(nullable: false, precision: 0),
        //            Hora_Ejecucion = c.DateTime(nullable: false, precision: 0),
        //            Cliente_ClientesContablesModelID = c.Int(),
        //            Compania_QuickEmisorModelID = c.Int(),
        //            Usuario_UsuarioModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.MonitoreoModelID)            
        //    .ForeignKey("ClientesContablesModel", t => t.Cliente_ClientesContablesModelID)
        //    .ForeignKey("QuickEmisorModel", t => t.Compania_QuickEmisorModelID)
        //    .ForeignKey("UsuarioModel", t => t.Usuario_UsuarioModelID)
        //    .Index(t => t.Cliente_ClientesContablesModelID)
        //    .Index(t => t.Compania_QuickEmisorModelID)
        //    .Index(t => t.Usuario_UsuarioModelID);
        
        //CreateTable(
        //    "UsuarioModel",
        //    c => new
        //        {
        //            UsuarioModelID = c.Int(nullable: false, identity: true),
        //            IdentityID = c.String(unicode: false),
        //            RUT = c.String(unicode: false),
        //            Nombre = c.String(unicode: false),
        //            Email = c.String(unicode: false),
        //            SuperAdminUser = c.Boolean(nullable: false),
        //            DatabaseContextToUse = c.Int(nullable: false),
        //            PerfilUsuarioModelID = c.Int(nullable: false),
        //            HeredaDeUsuario = c.Int(nullable: false),
        //            EstaDadoDeBaja = c.Boolean(nullable: false),
        //        })
        //    .PrimaryKey(t => t.UsuarioModelID)            ;
        
        //CreateTable(
        //    "PosesionUsuarios",
        //    c => new
        //        {
        //            PosesionUsuariosID = c.Int(nullable: false, identity: true),
        //            UsuarioModelID = c.Int(nullable: false),
        //            UsuarioPoseidoID = c.Int(nullable: false),
        //        })
        //    .PrimaryKey(t => t.PosesionUsuariosID)            
        //    .ForeignKey("UsuarioModel", t => t.UsuarioModelID, cascadeDelete: true)
        //    .Index(t => t.UsuarioModelID);
        
        //CreateTable(
        //    "MonitoreoSesion",
        //    c => new
        //        {
        //            MonitoreoSesionID = c.Int(nullable: false, identity: true),
        //            UsuarioID = c.Int(nullable: false),
        //            EstaActivo = c.Boolean(nullable: false),
        //        })
        //    .PrimaryKey(t => t.MonitoreoSesionID)            ;
        
        //CreateTable(
        //    "PerfilUsuarioModel",
        //    c => new
        //        {
        //            PerfilUsuarioModelID = c.Int(nullable: false, identity: true),
        //            NombrePerfil = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.PerfilUsuarioModelID)            ;
        
        //CreateTable(
        //    "PresupuestoModel",
        //    c => new
        //        {
        //            PresupuestoModelID = c.Int(nullable: false, identity: true),
        //            NombrePresupuesto = c.String(unicode: false),
        //            FechaInicio = c.DateTime(nullable: false, precision: 0),
        //            FechaVencimiento = c.DateTime(nullable: false, precision: 0),
        //            EstaVencido = c.Boolean(nullable: false),
        //            DadoDeBaja = c.Boolean(nullable: false),
        //            Cliente_ClientesContablesModelID = c.Int(),
        //        })
        //    .PrimaryKey(t => t.PresupuestoModelID)            
        //    .ForeignKey("ClientesContablesModel", t => t.Cliente_ClientesContablesModelID)
        //    .Index(t => t.Cliente_ClientesContablesModelID);
        
        //CreateTable(
        //    "RegionModels",
        //    c => new
        //        {
        //            RegionModelsID = c.Int(nullable: false, identity: true),
        //            nombre = c.String(unicode: false),
        //        })
        //    .PrimaryKey(t => t.RegionModelsID)            ;
        
        //CreateTable(
        //    "SubSubCtaPresupuestoModel",
        //    c => new
        //        {
        //            SubSubCtaPresupuestoModelID = c.Int(nullable: false, identity: true),
        //            SubSubClasificacionCtaContableID = c.Int(nullable: false),
        //            ClientesContablesModelID = c.Int(nullable: false),
        //            Presupuesto = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            FechaInicio = c.DateTime(nullable: false, precision: 0),
        //            FechaVencimiento = c.DateTime(nullable: false, precision: 0),
        //        })
        //    .PrimaryKey(t => t.SubSubCtaPresupuestoModelID)            ;
        
        //CreateTable(
        //    "UserClientesContablesModels",
        //    c => new
        //        {
        //            UserClientesContablesModelsID = c.Int(nullable: false, identity: true),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            ClientesContablesHabilitadosID = c.Int(nullable: false),
        //            UsuarioModelID = c.Int(nullable: false),
        //        })
        //    .PrimaryKey(t => t.UserClientesContablesModelsID)            ;
        
        //CreateTable(
        //    "EstadoResultadoModel",
        //    c => new
        //        {
        //            EstadoResultadoModelID = c.Int(nullable: false, identity: true),
        //            QuickEmisorModelID = c.Int(nullable: false),
        //            Periodo = c.DateTime(nullable: false, precision: 0),
        //            IngresosVarios = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            BrutoRemuneraciones = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            BrutoHonorarios = c.Decimal(nullable: false, precision: 18, scale: 2),
        //            GastosVarios = c.Decimal(nullable: false, precision: 18, scale: 2),
        //        })
        //    .PrimaryKey(t => t.EstadoResultadoModelID)            ;
        
        //CreateTable(
        //    "ActividadEconomicaModelQuickEmisorModel",
        //    c => new
        //        {
        //            ActividadEconomicaModel_ActividadEconomicaModelID = c.Int(nullable: false),
        //            QuickEmisorModel_QuickEmisorModelID = c.Int(nullable: false),
        //        })
        //    .PrimaryKey(t => new { t.ActividadEconomicaModel_ActividadEconomicaModelID, t.QuickEmisorModel_QuickEmisorModelID })            
        //    .ForeignKey("ActividadEconomicaModel", t => t.ActividadEconomicaModel_ActividadEconomicaModelID, cascadeDelete: true)
        //    .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModel_QuickEmisorModelID, cascadeDelete: true)
        //    .Index(t => t.ActividadEconomicaModel_ActividadEconomicaModelID)
        //    .Index(t => t.QuickEmisorModel_QuickEmisorModelID);
        
    }
    
    public override void Down()
    {
        DropForeignKey("PresupuestoModel", "Cliente_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("MonitoreoModel", "Usuario_UsuarioModelID", "UsuarioModel");
        DropForeignKey("PosesionUsuarios", "UsuarioModelID", "UsuarioModel");
        DropForeignKey("EmisoresHabilitados", "UsuarioModelID", "UsuarioModel");
        DropForeignKey("ClientesContablesModel", "UsuarioModel_UsuarioModelID", "UsuarioModel");
        DropForeignKey("MonitoreoModel", "Compania_QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("MonitoreoModel", "Cliente_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("ModulosHabilitados", "Funcion_FuncionesModelID", "FuncionesModel");
        DropForeignKey("LibroHonorariosDeTerceros", "VoucherModel_VoucherModelID", "VoucherModel");
        DropForeignKey("LibroHonorariosDeTerceros", "Receptor_QuickReceptorModelID", "QuickReceptorModel");
        DropForeignKey("LibroHonorariosDeTerceros", "ClienteContable_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("LibroDeHonorariosModel", "Prestador_QuickReceptorModelID", "QuickReceptorModel");
        DropForeignKey("FuncionesModel", "ModuloSistema_ModuloSistemaModelID", "ModuloSistemaModel");
        DropForeignKey("CartolaBancariaModel", "CuentaContableModelID_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("CartolaBancariaModel", "ClientesContablesModelID_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("CartolaBancariaMacroModel", "CuentaContableModelID_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("CartolaBancariaMacroModel", "ClientesContablesModelID_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("CartolaBancariaModel", "CartolaBancariaMacroModelID_CartolaBancariaMacroModelID", "CartolaBancariaMacroModel");
        DropForeignKey("BoletasCoVPadreModel", "ClienteContableModelID_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("BoletasCoVModel", "Prestador_QuickReceptorModelID", "QuickReceptorModel");
        DropForeignKey("BoletasCoVModel", "ClienteContable_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("BoletasCoVModel", "BoletaCoVPadre_BoletasCoVPadreModelID", "BoletasCoVPadreModel");
        DropForeignKey("CertificadosModels", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("ReportesImpagosLog", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("ClientesContablesModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("ClientesContablesModel", "ParametrosCliente_ParametrosClienteModelID", "ParametrosClienteModel");
        DropForeignKey("ParametrosClienteModel", "CuentaVentas_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("ParametrosClienteModel", "CuentaRetencionHonorarios_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("ParametrosClienteModel", "CuentaRetencionesHonorarios2_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("ParametrosClienteModel", "CuentaIvaVentas_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("ParametrosClienteModel", "CuentaIvaCompras_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("ParametrosClienteModel", "CuentaCompras_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("VoucherModel", "ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("DetalleVoucherModel", "VoucherModelID", "VoucherModel");
        DropForeignKey("DetalleVoucherModel", "ObjCuentaContable_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("DetalleVoucherModel", "objCentroCostro_CentroCostoModelID", "CentroCostoModel");
        DropForeignKey("DetalleVoucherModel", "Auxiliar_AuxiliaresModelID", "AuxiliaresModel");
        DropForeignKey("AuxiliaresModel", "objCtaContable_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("AuxiliaresDetalleModel", "AuxiliaresModelID", "AuxiliaresModel");
        DropForeignKey("AuxiliaresDetalleModel", "Individuo2_QuickReceptorModelID", "QuickReceptorModel");
        DropForeignKey("AuxiliaresDetalleModel", "Individuo_AuxiliaresPrestadoresModelID", "AuxiliaresPrestadoresModel");
        DropForeignKey("VoucherModel", "CentroDeCosto_CentroCostoModelID", "CentroCostoModel");
        DropForeignKey("LibrosContablesModel", "ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("LibrosContablesModel", "Prestador_AuxiliaresPrestadoresModelID", "AuxiliaresPrestadoresModel");
        DropForeignKey("LibrosContablesModel", "individuo_QuickReceptorModelID", "QuickReceptorModel");
        DropForeignKey("ItemModel", "ClientesContablesModel_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("CentroCostoModel", "ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("CuentaContableModel", "ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("ClientesContablesModel", "Comuna_ComunaModelsID", "ComunaModels");
        DropForeignKey("ActividadEconomicaModelQuickEmisorModel", "QuickEmisorModel_QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("ActividadEconomicaModelQuickEmisorModel", "ActividadEconomicaModel_ActividadEconomicaModelID", "ActividadEconomicaModel");
        DropForeignKey("QuickReceptorModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("QuickReceptorModel", "CuentaConToReceptor_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("CuentaContableModel", "SubSubClasificacion_SubSubClasificacionCtaContableID", "SubSubClasificacionCtaContable");
        DropForeignKey("CuentaContableModel", "SubClasificacion_SubClasificacionCtaContableID", "SubClasificacionCtaContable");
        DropForeignKey("BoletasHonorariosModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("DTEPagosModel", "BoletasHonorariosModel_BoletasHonorariosModelID", "BoletasHonorariosModel");
        DropIndex("ActividadEconomicaModelQuickEmisorModel", new[] { "QuickEmisorModel_QuickEmisorModelID" });
        DropIndex("ActividadEconomicaModelQuickEmisorModel", new[] { "ActividadEconomicaModel_ActividadEconomicaModelID" });
        DropIndex("PresupuestoModel", new[] { "Cliente_ClientesContablesModelID" });
        DropIndex("PosesionUsuarios", new[] { "UsuarioModelID" });
        DropIndex("MonitoreoModel", new[] { "Usuario_UsuarioModelID" });
        DropIndex("MonitoreoModel", new[] { "Compania_QuickEmisorModelID" });
        DropIndex("MonitoreoModel", new[] { "Cliente_ClientesContablesModelID" });
        DropIndex("ModulosHabilitados", new[] { "Funcion_FuncionesModelID" });
        DropIndex("LibroHonorariosDeTerceros", new[] { "VoucherModel_VoucherModelID" });
        DropIndex("LibroHonorariosDeTerceros", new[] { "Receptor_QuickReceptorModelID" });
        DropIndex("LibroHonorariosDeTerceros", new[] { "ClienteContable_ClientesContablesModelID" });
        DropIndex("LibroDeHonorariosModel", new[] { "Prestador_QuickReceptorModelID" });
        DropIndex("FuncionesModel", new[] { "ModuloSistema_ModuloSistemaModelID" });
        DropIndex("EmisoresHabilitados", new[] { "UsuarioModelID" });
        DropIndex("CartolaBancariaMacroModel", new[] { "CuentaContableModelID_CuentaContableModelID" });
        DropIndex("CartolaBancariaMacroModel", new[] { "ClientesContablesModelID_ClientesContablesModelID" });
        DropIndex("CartolaBancariaModel", new[] { "CuentaContableModelID_CuentaContableModelID" });
        DropIndex("CartolaBancariaModel", new[] { "ClientesContablesModelID_ClientesContablesModelID" });
        DropIndex("CartolaBancariaModel", new[] { "CartolaBancariaMacroModelID_CartolaBancariaMacroModelID" });
        DropIndex("BoletasCoVModel", new[] { "Prestador_QuickReceptorModelID" });
        DropIndex("BoletasCoVModel", new[] { "ClienteContable_ClientesContablesModelID" });
        DropIndex("BoletasCoVModel", new[] { "BoletaCoVPadre_BoletasCoVPadreModelID" });
        DropIndex("BoletasCoVPadreModel", new[] { "ClienteContableModelID_ClientesContablesModelID" });
        DropIndex("ReportesImpagosLog", new[] { "QuickEmisorModelID" });
        DropIndex("ParametrosClienteModel", new[] { "CuentaVentas_CuentaContableModelID" });
        DropIndex("ParametrosClienteModel", new[] { "CuentaRetencionHonorarios_CuentaContableModelID" });
        DropIndex("ParametrosClienteModel", new[] { "CuentaRetencionesHonorarios2_CuentaContableModelID" });
        DropIndex("ParametrosClienteModel", new[] { "CuentaIvaVentas_CuentaContableModelID" });
        DropIndex("ParametrosClienteModel", new[] { "CuentaIvaCompras_CuentaContableModelID" });
        DropIndex("ParametrosClienteModel", new[] { "CuentaCompras_CuentaContableModelID" });
        DropIndex("AuxiliaresDetalleModel", new[] { "Individuo2_QuickReceptorModelID" });
        DropIndex("AuxiliaresDetalleModel", new[] { "Individuo_AuxiliaresPrestadoresModelID" });
        DropIndex("AuxiliaresDetalleModel", new[] { "AuxiliaresModelID" });
        DropIndex("AuxiliaresModel", new[] { "objCtaContable_CuentaContableModelID" });
        DropIndex("DetalleVoucherModel", new[] { "ObjCuentaContable_CuentaContableModelID" });
        DropIndex("DetalleVoucherModel", new[] { "objCentroCostro_CentroCostoModelID" });
        DropIndex("DetalleVoucherModel", new[] { "Auxiliar_AuxiliaresModelID" });
        DropIndex("DetalleVoucherModel", new[] { "VoucherModelID" });
        DropIndex("VoucherModel", new[] { "CentroDeCosto_CentroCostoModelID" });
        DropIndex("VoucherModel", new[] { "ClientesContablesModelID" });
        DropIndex("LibrosContablesModel", new[] { "Prestador_AuxiliaresPrestadoresModelID" });
        DropIndex("LibrosContablesModel", new[] { "individuo_QuickReceptorModelID" });
        DropIndex("LibrosContablesModel", new[] { "ClientesContablesModelID" });
        DropIndex("ItemModel", new[] { "ClientesContablesModel_ClientesContablesModelID" });
        DropIndex("CentroCostoModel", new[] { "ClientesContablesModelID" });
        DropIndex("ClientesContablesModel", new[] { "UsuarioModel_UsuarioModelID" });
        DropIndex("ClientesContablesModel", new[] { "ParametrosCliente_ParametrosClienteModelID" });
        DropIndex("ClientesContablesModel", new[] { "Comuna_ComunaModelsID" });
        DropIndex("ClientesContablesModel", new[] { "QuickEmisorModelID" });
        DropIndex("CuentaContableModel", new[] { "SubSubClasificacion_SubSubClasificacionCtaContableID" });
        DropIndex("CuentaContableModel", new[] { "SubClasificacion_SubClasificacionCtaContableID" });
        DropIndex("CuentaContableModel", new[] { "ClientesContablesModelID" });
        DropIndex("QuickReceptorModel", new[] { "CuentaConToReceptor_CuentaContableModelID" });
        DropIndex("QuickReceptorModel", new[] { "QuickEmisorModelID" });
        DropIndex("DTEPagosModel", new[] { "BoletasHonorariosModel_BoletasHonorariosModelID" });
        DropIndex("BoletasHonorariosModel", new[] { "QuickEmisorModelID" });
        DropIndex("CertificadosModels", new[] { "QuickEmisorModelID" });
        DropTable("ActividadEconomicaModelQuickEmisorModel");
        DropTable("EstadoResultadoModel");
        DropTable("UserClientesContablesModels");
        DropTable("SubSubCtaPresupuestoModel");
        DropTable("RegionModels");
        DropTable("PresupuestoModel");
        DropTable("PerfilUsuarioModel");
        DropTable("MonitoreoSesion");
        DropTable("PosesionUsuarios");
        DropTable("UsuarioModel");
        DropTable("MonitoreoModel");
        DropTable("ModulosHabilitados");
        DropTable("LibroHonorariosDeTerceros");
        DropTable("LibroDeHonorariosModel");
        DropTable("ImpuestosAdRelacionModel");
        DropTable("ImpuestosAdicionalesModel");
        DropTable("ModuloSistemaModel");
        DropTable("FuncionesModel");
        DropTable("ErrorMensajeMonitoreo");
        DropTable("EmisoresHabilitados");
        DropTable("ClientesProveedoresModel");
        DropTable("ClientesContablesEmisorModel");
        DropTable("CentroCostoPresupuestoModels");
        DropTable("CtasContablesPresupuestoModel");
        DropTable("CartolaBancariaMacroModel");
        DropTable("CartolaBancariaModel");
        DropTable("BoletasCoVModel");
        DropTable("BoletasCoVPadreModel");
        DropTable("ActividadEconomicaModelCuentaContableModel");
        DropTable("ReportesImpagosLog");
        DropTable("ParametrosClienteModel");
        DropTable("AuxiliaresDetalleModel");
        DropTable("AuxiliaresModel");
        DropTable("DetalleVoucherModel");
        DropTable("VoucherModel");
        DropTable("AuxiliaresPrestadoresModel");
        DropTable("LibrosContablesModel");
        DropTable("ItemModel");
        DropTable("CentroCostoModel");
        DropTable("ComunaModels");
        DropTable("ClientesContablesModel");
        DropTable("ActividadEconomicaModel");
        DropTable("SubSubClasificacionCtaContable");
        DropTable("SubClasificacionCtaContable");
        DropTable("CuentaContableModel");
        DropTable("QuickReceptorModel");
        DropTable("DTEPagosModel");
        DropTable("BoletasHonorariosModel");
        DropTable("QuickEmisorModel");
        DropTable("CertificadosModels");
    }
}
