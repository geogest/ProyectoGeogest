using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Migrations;

public partial class EliminadoMultiplesTablasFacturacion2 : DbMigration
{
    public override void Up()
    {
        DehabilitaLlavesForaneas();
        //DropForeignKey("FacturaQuickModel", "ACTECO_factura_ActividadEconomicaModelID", "ActividadEconomicaModel");
        //DropForeignKey("QuickDetalleModel", "FacturaQuickModelID", "FacturaQuickModel");
        //DropForeignKey("FacturaQuickModel", "QuickEmisorModelID", "QuickEmisorModel");
        //DropForeignKey("DTEPagosModel", "FacturaQuickModel_FacturaQuickModelID", "FacturaQuickModel");
        //DropForeignKey("TotalesModel", "FacturaQuickModelID", "FacturaQuickModel");
        //DropForeignKey("FacturaQuickModel", "Receptor_QuickReceptorModelID", "QuickReceptorModel");
        //DropForeignKey("QReferenciasModel", "FacturaQuickModelID", "FacturaQuickModel");
        //DropForeignKey("FacturaQuickModel", "Transporte_QuickTransporteExportacionID", "QuickTransporteExportacion");
        //DropIndex("DTEPagosModel", new[] { "FacturaQuickModel_FacturaQuickModelID" });
        //DropIndex("FacturaQuickModel", new[] { "QuickEmisorModelID" });
        //DropIndex("FacturaQuickModel", new[] { "ACTECO_factura_ActividadEconomicaModelID" });
        //DropIndex("FacturaQuickModel", new[] { "Receptor_QuickReceptorModelID" });
        //DropIndex("FacturaQuickModel", new[] { "Transporte_QuickTransporteExportacionID" });
        //DropIndex("QuickDetalleModel", new[] { "FacturaQuickModelID" });
        //DropIndex("TotalesModel", new[] { "FacturaQuickModelID" });
        //DropIndex("QReferenciasModel", new[] { "FacturaQuickModelID" });
        //DropColumn("DTEPagosModel", "FacturaQuickModel_FacturaQuickModelID");
        DropTable("FacturaQuickModel");
        DropTable("TotalesModel");
        DropTable("QReferenciasModel");
        DropTable("QuickTransporteExportacion");
        DropTable("ReferenciasFacturaRecurrente");
        DropTable("IntercambioModel");
    }


    private void DehabilitaLlavesForaneas()
    {
        string DBProduccion = "Server=localhost;Port=3306;Database=ProductionDB;Uid=root;Pwd=root;SslMode=Preferred;";

        using (MySqlConnection db = new MySqlConnection(DBProduccion))
        {
            db.Open();
            MySqlCommand DeshabilitandoLlaves = new MySqlCommand("SET FOREIGN_KEY_CHECKS=0;", db);
            DeshabilitandoLlaves.ExecuteNonQuery();
        }

        string DBCertificacion = "Server=localhost;Port=3306;Database=FacturaDB;Uid=root;Pwd=root;SslMode=Preferred;";

        using (MySqlConnection dbCert = new MySqlConnection(DBCertificacion))
        {
            dbCert.Open();
            MySqlCommand DeshabilitandoLlavesCert = new MySqlCommand("SET FOREIGN_KEY_CHECKS=0;", dbCert);
            DeshabilitandoLlavesCert.ExecuteNonQuery();
        }
    }
    public override void Down()
    {
        CreateTable(
            "IntercambioModel",
            c => new
                {
                    IntercambioModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    xmlDocumentAttachment = c.String(unicode: false),
                    attachmentName = c.String(unicode: false),
                    mailUID = c.String(unicode: false),
                    rutEmisor = c.String(unicode: false),
                    fromMail = c.String(unicode: false),
                    dateProcessed = c.DateTime(nullable: false, precision: 0),
                    tipoOperacion = c.Int(nullable: false),
                    xmlGeneratedResponse = c.String(unicode: false),
                    status = c.Int(nullable: false),
                    GlosaDescriptiva = c.String(unicode: false),
                    IdentificadorIntercambioEnvio = c.Int(nullable: false),
                    RutToSendEMail = c.String(unicode: false),
                })
            .PrimaryKey(t => t.IntercambioModelID)            ;
        
        CreateTable(
            "ReferenciasFacturaRecurrente",
            c => new
                {
                    ReferenciasFacturaRecurrenteID = c.Int(nullable: false, identity: true),
                    FacturaRecurrenteModelID = c.Int(nullable: false),
                    TipoDocReferencia = c.Int(nullable: false),
                    IndicadorReferenciaGlobal = c.Int(nullable: false),
                    FolioRef = c.String(unicode: false),
                    FechaRef = c.DateTime(nullable: false, precision: 0),
                    CodigoReferencia = c.Int(nullable: false),
                    RazonReferencia = c.String(unicode: false),
                })
            .PrimaryKey(t => t.ReferenciasFacturaRecurrenteID)            ;
        
        CreateTable(
            "QuickTransporteExportacion",
            c => new
                {
                    QuickTransporteExportacionID = c.Int(nullable: false, identity: true),
                    FacturaQuickModelID = c.Int(nullable: false),
                    DirDest = c.String(unicode: false),
                    CiudadDest = c.String(unicode: false),
                    ComunaDest = c.String(unicode: false),
                    Nacionalidad = c.Int(nullable: false),
                    CodModVenta = c.Int(nullable: false),
                    CodClauVenta = c.Int(nullable: false),
                    TotClauVenta = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CodViaTransp = c.Int(nullable: false),
                    CodUnidMedTara = c.Int(nullable: false),
                    PesoBruto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CodUnidPesoBruto = c.Int(nullable: false),
                    CodUnidPesoNeto = c.Int(nullable: false),
                    CodTipoBultos = c.Int(nullable: false),
                    TotBultos = c.Int(nullable: false),
                    Marcas = c.String(unicode: false),
                    IdContainer = c.String(unicode: false),
                    Sello = c.String(unicode: false),
                    CodPaisRecep = c.Int(nullable: false),
                    CodPaisDestin = c.Int(nullable: false),
                    TpoMoneda = c.String(unicode: false),
                    CodPuertoEmbarque = c.Int(nullable: false),
                    CodPuertoDesembarque = c.Int(nullable: false),
                    MontoFlete = c.Decimal(nullable: false, precision: 18, scale: 2),
                    MontoSeguro = c.Decimal(nullable: false, precision: 18, scale: 2),
                    FormaPagoExportacion = c.Int(nullable: false),
                    RecargoGlobalExportacion = c.Decimal(nullable: false, precision: 18, scale: 2),
                    IndicadorServicio = c.Int(nullable: false),
                    ValorDROtrMnda = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RecargoPct = c.Decimal(nullable: false, precision: 18, scale: 2),
                    NombreTransp = c.String(unicode: false),
                    TipoDespacho = c.Int(nullable: false),
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
            .PrimaryKey(t => t.QReferenciasModelID)            ;
        
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
            .PrimaryKey(t => t.FacturaQuickModelID)            ;
        
        CreateTable(
            "FacturaQuickModel",
            c => new
                {
                    FacturaQuickModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    NumFolio = c.Int(nullable: false),
                    DescuentoGlobal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DescuentoGlobalAfectaExentos = c.Boolean(nullable: false),
                    TipoFactura = c.Int(nullable: false),
                    FechaEmision = c.DateTime(nullable: false, precision: 0),
                    FechaIngresoSistema = c.DateTime(nullable: false, precision: 0),
                    FechaAcuseRecibo = c.DateTime(nullable: false, precision: 0),
                    TrackID = c.String(unicode: false),
                    Status = c.String(unicode: false),
                    StatusIntercambio = c.String(unicode: false),
                    StatusIntercambioGlosa = c.String(unicode: false),
                    IndicadorServicio = c.Int(nullable: false),
                    GeneratedXML = c.String(unicode: false),
                    GeneratedXMLIntercambio = c.String(unicode: false),
                    IDElementoLibroDetalle = c.Int(nullable: false),
                    ID_Factura_Periodica = c.Int(nullable: false),
                    FechaVencPago = c.DateTime(nullable: false, precision: 0),
                    EnumFechaPago = c.Int(nullable: false),
                    ACTECO_factura_ActividadEconomicaModelID = c.Int(),
                    Receptor_QuickReceptorModelID = c.Int(),
                    Transporte_QuickTransporteExportacionID = c.Int(),
                })
            .PrimaryKey(t => t.FacturaQuickModelID)            ;
        
        AddColumn("DTEPagosModel", "FacturaQuickModel_FacturaQuickModelID", c => c.Int());
        CreateIndex("QReferenciasModel", "FacturaQuickModelID");
        CreateIndex("TotalesModel", "FacturaQuickModelID");
        CreateIndex("QuickDetalleModel", "FacturaQuickModelID");
        CreateIndex("FacturaQuickModel", "Transporte_QuickTransporteExportacionID");
        CreateIndex("FacturaQuickModel", "Receptor_QuickReceptorModelID");
        CreateIndex("FacturaQuickModel", "ACTECO_factura_ActividadEconomicaModelID");
        CreateIndex("FacturaQuickModel", "QuickEmisorModelID");
        CreateIndex("DTEPagosModel", "FacturaQuickModel_FacturaQuickModelID");
        AddForeignKey("FacturaQuickModel", "Transporte_QuickTransporteExportacionID", "QuickTransporteExportacion", "QuickTransporteExportacionID");
        AddForeignKey("QReferenciasModel", "FacturaQuickModelID", "FacturaQuickModel", "FacturaQuickModelID", cascadeDelete: true);
        AddForeignKey("FacturaQuickModel", "Receptor_QuickReceptorModelID", "QuickReceptorModel", "QuickReceptorModelID");
        AddForeignKey("TotalesModel", "FacturaQuickModelID", "FacturaQuickModel", "FacturaQuickModelID");
        AddForeignKey("DTEPagosModel", "FacturaQuickModel_FacturaQuickModelID", "FacturaQuickModel", "FacturaQuickModelID");
        AddForeignKey("FacturaQuickModel", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
        AddForeignKey("QuickDetalleModel", "FacturaQuickModelID", "FacturaQuickModel", "FacturaQuickModelID", cascadeDelete: true);
        AddForeignKey("FacturaQuickModel", "ACTECO_factura_ActividadEconomicaModelID", "ActividadEconomicaModel", "ActividadEconomicaModelID");
    }
}
