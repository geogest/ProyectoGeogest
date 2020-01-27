using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Migrations;

public partial class ElimnadoLiquidacionesModel : DbMigration
{
    public override void Up()
    {
        DehabilitaLlavesForaneas();
        //DropForeignKey("DTEPagosModel", "LiquidacionesModel_LiquidacionesModelID", "LiquidacionesModel");
        //DropForeignKey("LiquidacionesModel", "QuickEmisorModelID", "QuickEmisorModel");
        //DropIndex("DTEPagosModel", new[] { "LiquidacionesModel_LiquidacionesModelID" });
        //DropIndex("LiquidacionesModel", new[] { "QuickEmisorModelID" });
        //DropColumn("DTEPagosModel", "LiquidacionesModel_LiquidacionesModelID");
        DropTable("LiquidacionesModel");
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
            "LiquidacionesModel",
            c => new
                {
                    LiquidacionesModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    FolioDocumento = c.Int(nullable: false),
                    FechaIngreso = c.DateTime(nullable: false, precision: 0),
                    RUT_num = c.Int(nullable: false),
                    RUT_txt = c.String(unicode: false),
                    Glosa = c.String(unicode: false),
                    NombreTrabajador = c.String(unicode: false),
                    SueldoLiquido = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PreviredMonto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    SueldoBase = c.Decimal(nullable: false, precision: 18, scale: 2),
                    GratificacionLegal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    HorasExtras = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Movilizacion = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Viatico = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CapitalFondoPensiones = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CapitalSeguroAdicional = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CotizacionLegal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    ImpuestoUnico = c.Decimal(nullable: false, precision: 18, scale: 2),
                    SeguroDeCesantia = c.Decimal(nullable: false, precision: 18, scale: 2),
                    base64ArchivoAsociado = c.String(unicode: false),
                    TipoArchivoAsociado = c.String(unicode: false),
                })
            .PrimaryKey(t => t.LiquidacionesModelID)            ;
        
        AddColumn("DTEPagosModel", "LiquidacionesModel_LiquidacionesModelID", c => c.Int());
        CreateIndex("LiquidacionesModel", "QuickEmisorModelID");
        CreateIndex("DTEPagosModel", "LiquidacionesModel_LiquidacionesModelID");
        AddForeignKey("LiquidacionesModel", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
        AddForeignKey("DTEPagosModel", "LiquidacionesModel_LiquidacionesModelID", "LiquidacionesModel", "LiquidacionesModelID");
    }
}
