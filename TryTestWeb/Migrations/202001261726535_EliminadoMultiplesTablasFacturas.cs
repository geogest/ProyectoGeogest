using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Migrations;

public partial class EliminadoMultiplesTablasFacturas : DbMigration
{
    public override void Up()
    {

        DehabilitaLlavesForaneas();
        //DropForeignKey("DTEPagosModel", "QuickDetalleLibroCompraModel_QuickDetalleLibroCompraModelID", "QuickDetalleLibroCompraModel");
        //DropForeignKey("OtrosImpModel", "QuickDetalleLibroCompraModelID", "QuickDetalleLibroCompraModel");
        //DropForeignKey("QuickDetalleLibroCompraModel", "QuickLibroCompraModelID", "QuickLibroCompraModel");
        //DropForeignKey("QuickLibroCompraModel", "QuickEmisorModelID", "QuickEmisorModel");
        //DropIndex("DTEPagosModel", new[] { "QuickDetalleLibroCompraModel_QuickDetalleLibroCompraModelID" });
        //DropIndex("QuickLibroCompraModel", new[] { "QuickEmisorModelID" });
        //DropIndex("QuickDetalleLibroCompraModel", new[] { "QuickLibroCompraModelID" });
        //DropIndex("OtrosImpModel", new[] { "QuickDetalleLibroCompraModelID" });
        //DropColumn("DTEPagosModel", "QuickDetalleLibroCompraModel_QuickDetalleLibroCompraModelID");
        DropTable("QuickLibroCompraModel");
        DropTable("QuickDetalleLibroCompraModel");
        DropTable("OtrosImpModel");
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
            "OtrosImpModel",
            c => new
                {
                    OtrosImpModelID = c.Int(nullable: false, identity: true),
                    QuickDetalleLibroCompraModelID = c.Int(nullable: false),
                    CodImp = c.Int(nullable: false),
                    TasaImp = c.Single(nullable: false),
                    MntImp = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.OtrosImpModelID)            ;
        
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
                    CodigoIVANoRecuperable = c.Int(),
                    FactorProporcionalidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                    codigoEstadoReception = c.Int(nullable: false),
                    GlosaRechazo = c.String(unicode: false),
                    IdentificadorIntercambioEnvio = c.Int(nullable: false),
                    IdentificadorIntercambioDoc = c.Int(nullable: false),
                    CuentaContableModelID = c.Int(nullable: false),
                    FacturaCompraXMLOutput = c.String(unicode: false),
                })
            .PrimaryKey(t => t.QuickDetalleLibroCompraModelID)            ;
        
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
                    LibroXMLOutput = c.String(unicode: false),
                    StatusLibro = c.String(unicode: false),
                })
            .PrimaryKey(t => t.QuickLibroCompraModelID)            ;
        
        AddColumn("DTEPagosModel", "QuickDetalleLibroCompraModel_QuickDetalleLibroCompraModelID", c => c.Int());
        CreateIndex("OtrosImpModel", "QuickDetalleLibroCompraModelID");
        CreateIndex("QuickDetalleLibroCompraModel", "QuickLibroCompraModelID");
        CreateIndex("QuickLibroCompraModel", "QuickEmisorModelID");
        CreateIndex("DTEPagosModel", "QuickDetalleLibroCompraModel_QuickDetalleLibroCompraModelID");
        AddForeignKey("QuickLibroCompraModel", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
        AddForeignKey("QuickDetalleLibroCompraModel", "QuickLibroCompraModelID", "QuickLibroCompraModel", "QuickLibroCompraModelID", cascadeDelete: true);
        AddForeignKey("OtrosImpModel", "QuickDetalleLibroCompraModelID", "QuickDetalleLibroCompraModel", "QuickDetalleLibroCompraModelID", cascadeDelete: true);
        AddForeignKey("DTEPagosModel", "QuickDetalleLibroCompraModel_QuickDetalleLibroCompraModelID", "QuickDetalleLibroCompraModel", "QuickDetalleLibroCompraModelID");
    }
}
