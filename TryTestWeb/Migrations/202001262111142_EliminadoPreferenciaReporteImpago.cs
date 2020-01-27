using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Migrations;

public partial class EliminadoPreferenciaReporteImpago : DbMigration
{
    public override void Up()
    {
        DehabilitaLlavesForaneas();
        //DropForeignKey("QuickEmisorModel", "DatosReporteriaImpagos_PreferenciaReporteImpagosID", "PreferenciaReporteImpagos");
        //DropIndex("QuickEmisorModel", new[] { "DatosReporteriaImpagos_PreferenciaReporteImpagosID" });
        //DropColumn("QuickEmisorModel", "DatosReporteriaImpagos_PreferenciaReporteImpagosID");
        DropTable("PreferenciaReporteImpagos");
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
            "PreferenciaReporteImpagos",
            c => new
                {
                    PreferenciaReporteImpagosID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    FechaEnvioInicio = c.DateTime(nullable: false, precision: 0),
                    FechaNextExecution = c.DateTime(nullable: false, precision: 0),
                    TipoRecurrencia = c.Int(nullable: false),
                    Frecuencia = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.PreferenciaReporteImpagosID)            ;
        
        AddColumn("QuickEmisorModel", "DatosReporteriaImpagos_PreferenciaReporteImpagosID", c => c.Int());
        CreateIndex("QuickEmisorModel", "DatosReporteriaImpagos_PreferenciaReporteImpagosID");
        AddForeignKey("QuickEmisorModel", "DatosReporteriaImpagos_PreferenciaReporteImpagosID", "PreferenciaReporteImpagos", "PreferenciaReporteImpagosID");
    }
}
