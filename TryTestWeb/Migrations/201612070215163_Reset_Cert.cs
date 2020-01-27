using System;
using System.Data.Entity.Migrations;

public partial class Reset_Cert : DbMigration
{
    public override void Up()
    {
        /*
        CreateTable(
            "CertificadosModels",
            c => new
                {
                    QuickEmisorModelID = c.Int(nullable: false),
                    CertificadosModelsID = c.Int(nullable: false),
                    PathCAF33 = c.String(unicode: false),
                    PathCAF34 = c.String(unicode: false),
                    PathCAF61 = c.String(unicode: false),
                    PathCAF56 = c.String(unicode: false),
                    PathCAF110 = c.String(unicode: false),
                    PathCAF52 = c.String(unicode: false),
                    CertificatePath = c.String(unicode: false),
                    CertificatePassword = c.String(unicode: false),
                })
            .PrimaryKey(t => t.QuickEmisorModelID)            
            .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID)
            .Index(t => t.QuickEmisorModelID);
        
        CreateTable(
            "QuickEmisorModel",
            c => new
                {
                    QuickEmisorModelID = c.Int(nullable: false, identity: true),
                    IdentityID = c.String(unicode: false),
                    RUTEmpresa = c.String(unicode: false),
                    RUTUsuario = c.String(unicode: false),
                    RazonSocial = c.String(unicode: false),
                    Direccion = c.String(unicode: false),
                    Comuna = c.String(unicode: false),
                    Ciudad = c.String(unicode: false),
                    EMail = c.String(unicode: false),
                    Giro = c.String(unicode: false),
                    ActEcono = c.String(unicode: false),
                    Telefono = c.String(unicode: false),
                    CertificadosIDKey = c.Int(nullable: false),
                    FechaResolucion = c.DateTime(nullable: false, precision: 0),
                    NumeroResolucion = c.Int(nullable: false),
                    CodigoSucursalSII = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.QuickEmisorModelID)            ;
        
        CreateTable(
            "FacturaQuickModel",
            c => new
                {
                    FacturaQuickModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    NumFolio = c.Int(nullable: false),
                    DescuentoGlobal = c.Double(nullable: false),
                    TipoFactura = c.Int(nullable: false),
                    FechaEmision = c.DateTime(nullable: false, precision: 0),
                    TrackID = c.String(unicode: false),
                    Status = c.String(unicode: false),
                    Receptor_QuickReceptorModelID = c.Int(),
                    Transporte_QuickTransporteExportacionID = c.Int(),
                })
            .PrimaryKey(t => t.FacturaQuickModelID)            
            .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
            .ForeignKey("QuickReceptorModel", t => t.Receptor_QuickReceptorModelID)
            .ForeignKey("QuickTransporteExportacion", t => t.Transporte_QuickTransporteExportacionID)
            .Index(t => t.QuickEmisorModelID)
            .Index(t => t.Receptor_QuickReceptorModelID)
            .Index(t => t.Transporte_QuickTransporteExportacionID);
        
        CreateTable(
            "QuickDetalleModel",
            c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    FacturaQuickModelID = c.Int(nullable: false),
                    CodProd = c.Int(nullable: false),
                    NombreProducto = c.String(unicode: false),
                    Descripcion = c.String(unicode: false),
                    UMedida = c.String(unicode: false),
                    Cantidad = c.Double(nullable: false),
                    Precio = c.Int(nullable: false),
                    DescuentoProducto = c.Double(nullable: false),
                    DescuentoGlobal = c.Double(nullable: false),
                    ExentoIVA = c.Boolean(nullable: false),
                    CodImpuesto = c.Double(nullable: false),
                    ImpuestoAdicionalProducto_ID = c.Int(),
                })
            .PrimaryKey(t => t.ID)            
            .ForeignKey("ImpuestoAdicionalRetencionesModel", t => t.ImpuestoAdicionalProducto_ID)
            .ForeignKey("FacturaQuickModel", t => t.FacturaQuickModelID, cascadeDelete: true)
            .Index(t => t.FacturaQuickModelID)
            .Index(t => t.ImpuestoAdicionalProducto_ID);
        
        CreateTable(
            "ImpuestoAdicionalRetencionesModel",
            c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    DescripcionImpuesto = c.String(unicode: false),
                    TasaImpuesto = c.Double(nullable: false),
                    CodImpuesto = c.Double(nullable: false),
                })
            .PrimaryKey(t => t.ID)            ;
        
        CreateTable(
            "TotalesModel",
            c => new
                {
                    FacturaQuickModelID = c.Int(nullable: false),
                    SubTotalMonto = c.Int(nullable: false),
                    DescuentoGlobalMonto = c.Int(nullable: false),
                    MontoNetoMonto = c.Int(nullable: false),
                    MontoExento = c.Int(nullable: false),
                    IVAMonto = c.Int(nullable: false),
                    ImpuestoAdicionalMonto = c.Int(nullable: false),
                    TotalMonto = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.FacturaQuickModelID)            
            .ForeignKey("FacturaQuickModel", t => t.FacturaQuickModelID)
            .Index(t => t.FacturaQuickModelID);
        
        CreateTable(
            "QuickReceptorModel",
            c => new
                {
                    QuickReceptorModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    RUT = c.String(unicode: false),
                    NombreFantasia = c.String(unicode: false),
                    RazonSocial = c.String(unicode: false),
                    Direccion = c.String(unicode: false),
                    Comuna = c.String(unicode: false),
                    Ciudad = c.String(unicode: false),
                    Giro = c.String(unicode: false),
                    Contacto = c.String(unicode: false),
                    NombreContacto = c.String(unicode: false),
                    RUTSolicitante = c.String(unicode: false),
                })
            .PrimaryKey(t => t.QuickReceptorModelID)            
            .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
            .Index(t => t.QuickEmisorModelID);
        
        CreateTable(
            "QReferenciasModel",
            c => new
                {
                    QReferenciasModelID = c.Int(nullable: false, identity: true),
                    FacturaQuickModelID = c.Int(nullable: false),
                    TipoDocReferencia = c.Int(nullable: false),
                    IndicadorReferenciaGlobal = c.Int(nullable: false),
                    FolioRef = c.String(unicode: false),
                    FechaRef = c.DateTime(nullable: false, precision: 0),
                    CodigoReferencia = c.Int(nullable: false),
                    RazonReferencia = c.String(unicode: false),
                })
            .PrimaryKey(t => t.QReferenciasModelID)            
            .ForeignKey("FacturaQuickModel", t => t.FacturaQuickModelID, cascadeDelete: true)
            .Index(t => t.FacturaQuickModelID);
        
        CreateTable(
            "QuickTransporteExportacion",
            c => new
                {
                    QuickTransporteExportacionID = c.Int(nullable: false, identity: true),
                    FacturaQuickModelID = c.Int(nullable: false),
                    DirDest = c.String(unicode: false),
                    CiudadDest = c.String(unicode: false),
                    ComunaDest = c.String(unicode: false),
                    CodModVenta = c.Int(nullable: false),
                    CodClauVenta = c.Int(nullable: false),
                    TotClauVenta = c.Double(nullable: false),
                    CodViaTransp = c.Int(nullable: false),
                    CodUnidMedTara = c.Int(nullable: false),
                    PesoBruto = c.Double(nullable: false),
                    CodUnidPesoBruto = c.Int(nullable: false),
                    TotBultos = c.Int(nullable: false),
                    CodPaisRecep = c.Int(nullable: false),
                    CodPaisDestin = c.Int(nullable: false),
                    TpoMoneda = c.String(unicode: false),
                    IndTraslado = c.Int(nullable: false),
                    CodTraslado = c.Int(nullable: false),
                    FolioAutorizacion = c.Int(nullable: false),
                    FechaAutorizacion = c.DateTime(nullable: false, precision: 0),
                    Patente = c.String(unicode: false),
                    RUTTransportista = c.String(unicode: false),
                    RUTChofer = c.String(unicode: false),
                    NombreChofer = c.String(unicode: false),
                })
            .PrimaryKey(t => t.QuickTransporteExportacionID)            ;
        
        CreateTable(
            "QuickLibroCompraModel",
            c => new
                {
                    QuickLibroCompraModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    RutEmisorLibro = c.String(unicode: false),
                    RutEnvia = c.String(unicode: false),
                    PeriodoTributario = c.String(unicode: false),
                    FchResol = c.String(unicode: false),
                    NroResol = c.String(unicode: false),
                    TipoOperacion = c.Int(nullable: false),
                    LibroTipoLibro = c.Int(nullable: false),
                    TipoEnvio = c.Int(nullable: false),
                    FolioNotificacion = c.String(unicode: false),
                    TrackID = c.String(unicode: false),
                })
            .PrimaryKey(t => t.QuickLibroCompraModelID)            
            .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
            .Index(t => t.QuickEmisorModelID);
        
        CreateTable(
            "QuickDetalleLibroCompraModel",
            c => new
                {
                    QuickDetalleLibroCompraModelID = c.Int(nullable: false, identity: true),
                    QuickLibroCompraModelID = c.Int(nullable: false),
                    TpoDoc = c.Int(nullable: false),
                    NroDoc = c.Int(nullable: false),
                    TpoImp = c.Int(nullable: false),
                    FchDoc = c.DateTime(nullable: false, precision: 0),
                    RUTDoc = c.String(unicode: false),
                    RznSoc = c.String(unicode: false),
                    MntExe = c.Int(nullable: false),
                    MntNeto = c.Int(nullable: false),
                    MntIVA = c.Int(nullable: false),
                    MntTotal = c.Int(nullable: false),
                    tipoFactura = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.QuickDetalleLibroCompraModelID)            
            .ForeignKey("QuickLibroCompraModel", t => t.QuickLibroCompraModelID, cascadeDelete: true)
            .Index(t => t.QuickLibroCompraModelID);
        
        CreateTable(
            "OtrosImpModel",
            c => new
                {
                    OtrosImpModelID = c.Int(nullable: false, identity: true),
                    QuickDetalleLibroCompraModelID = c.Int(nullable: false),
                    CodImp = c.Int(nullable: false),
                    TasaImp = c.Single(nullable: false),
                    MntImp = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.OtrosImpModelID)            
            .ForeignKey("QuickDetalleLibroCompraModel", t => t.QuickDetalleLibroCompraModelID, cascadeDelete: true)
            .Index(t => t.QuickDetalleLibroCompraModelID);
        
    }
    
    public override void Down()
    {
        DropForeignKey("CertificadosModels", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("QuickReceptorModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("QuickLibroCompraModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("QuickDetalleLibroCompraModel", "QuickLibroCompraModelID", "QuickLibroCompraModel");
        DropForeignKey("OtrosImpModel", "QuickDetalleLibroCompraModelID", "QuickDetalleLibroCompraModel");
        DropForeignKey("FacturaQuickModel", "Transporte_QuickTransporteExportacionID", "QuickTransporteExportacion");
        DropForeignKey("QReferenciasModel", "FacturaQuickModelID", "FacturaQuickModel");
        DropForeignKey("FacturaQuickModel", "Receptor_QuickReceptorModelID", "QuickReceptorModel");
        DropForeignKey("TotalesModel", "FacturaQuickModelID", "FacturaQuickModel");
        DropForeignKey("FacturaQuickModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropForeignKey("QuickDetalleModel", "FacturaQuickModelID", "FacturaQuickModel");
        DropForeignKey("QuickDetalleModel", "ImpuestoAdicionalProducto_ID", "ImpuestoAdicionalRetencionesModel");
        DropIndex("OtrosImpModel", new[] { "QuickDetalleLibroCompraModelID" });
        DropIndex("QuickDetalleLibroCompraModel", new[] { "QuickLibroCompraModelID" });
        DropIndex("QuickLibroCompraModel", new[] { "QuickEmisorModelID" });
        DropIndex("QReferenciasModel", new[] { "FacturaQuickModelID" });
        DropIndex("QuickReceptorModel", new[] { "QuickEmisorModelID" });
        DropIndex("TotalesModel", new[] { "FacturaQuickModelID" });
        DropIndex("QuickDetalleModel", new[] { "ImpuestoAdicionalProducto_ID" });
        DropIndex("QuickDetalleModel", new[] { "FacturaQuickModelID" });
        DropIndex("FacturaQuickModel", new[] { "Transporte_QuickTransporteExportacionID" });
        DropIndex("FacturaQuickModel", new[] { "Receptor_QuickReceptorModelID" });
        DropIndex("FacturaQuickModel", new[] { "QuickEmisorModelID" });
        DropIndex("CertificadosModels", new[] { "QuickEmisorModelID" });
        DropTable("OtrosImpModel");
        DropTable("QuickDetalleLibroCompraModel");
        DropTable("QuickLibroCompraModel");
        DropTable("QuickTransporteExportacion");
        DropTable("QReferenciasModel");
        DropTable("QuickReceptorModel");
        DropTable("TotalesModel");
        DropTable("ImpuestoAdicionalRetencionesModel");
        DropTable("QuickDetalleModel");
        DropTable("FacturaQuickModel");
        DropTable("QuickEmisorModel");
        DropTable("CertificadosModels");*/
    }
}
