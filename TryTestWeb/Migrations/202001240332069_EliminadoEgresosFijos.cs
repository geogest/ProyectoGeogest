using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Migrations;
using MySql.Data.Entity;

public partial class EliminadoEgresosFijos : DbMigration
{
    public override void Up()
    {

        DehabilitaLlavesForaneas();
      //  DropForeignKey("DTEPagosModel", "EgresosFijosModel_EgresosFijosModelID", "EgresosFijosModel");
        //DropForeignKey("EgresosFijosModel", "Tipo_TipoEgresoFijoModelID", "TipoEgresoFijoModel");
        //DropForeignKey("EgresosFijosModel", "QuickEmisorModelID", "QuickEmisorModel");
        //DropIndex("DTEPagosModel", new[] { "EgresosFijosModel_EgresosFijosModelID" });
        //DropIndex("EgresosFijosModel", new[] { "QuickEmisorModelID" });
        //DropIndex("EgresosFijosModel", new[] { "Tipo_TipoEgresoFijoModelID" });
        //DropColumn("DTEPagosModel", "EgresosFijosModel_EgresosFijosModelID");
        DropTable("EgresosFijosModel");
        DropTable("TipoEgresoFijoModel");
        //HabilitaLlavesForaneas();
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

    private void HabilitaLlavesForaneas()
    {
        string DBProduccion = "Server=localhost;Port=3306;Database=ProductionDB;Uid=root;Pwd=root;SslMode=Preferred;";

        using (MySqlConnection db = new MySqlConnection(DBProduccion))
        {
            db.Open();
            MySqlCommand DeshabilitandoLlaves = new MySqlCommand("SET FOREIGN_KEY_CHECKS=1;", db);
            DeshabilitandoLlaves.ExecuteNonQuery();
        }

        string DBCertificacion = "Server=localhost;Port=3306;Database=FacturaDB;Uid=root;Pwd=root;SslMode=Preferred;";

        using (MySqlConnection dbCert = new MySqlConnection(DBCertificacion))
        {
            dbCert.Open();
            MySqlCommand DeshabilitandoLlavesCert = new MySqlCommand("SET FOREIGN_KEY_CHECKS=1;", dbCert);
            DeshabilitandoLlavesCert.ExecuteNonQuery();
        }
    }




    public override void Down()
    {
        CreateTable(
            "TipoEgresoFijoModel",
            c => new
                {
                    TipoEgresoFijoModelID = c.Int(nullable: false, identity: true),
                    TipoEgresoFijoSTR = c.String(unicode: false),
                    TipoAceptacion = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.TipoEgresoFijoModelID)            ;
        
        CreateTable(
            "EgresosFijosModel",
            c => new
                {
                    EgresosFijosModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    FechaIngreso = c.DateTime(nullable: false, precision: 0),
                    Descripcion = c.String(unicode: false),
                    Folio = c.Int(nullable: false),
                    base64ArchivoAsociado = c.String(unicode: false),
                    TipoArchivoAsociado = c.String(unicode: false),
                    Tipo_TipoEgresoFijoModelID = c.Int(),
                })
            .PrimaryKey(t => t.EgresosFijosModelID)            ;
        
        AddColumn("DTEPagosModel", "EgresosFijosModel_EgresosFijosModelID", c => c.Int());
        CreateIndex("EgresosFijosModel", "Tipo_TipoEgresoFijoModelID");
        CreateIndex("EgresosFijosModel", "QuickEmisorModelID");
        CreateIndex("DTEPagosModel", "EgresosFijosModel_EgresosFijosModelID");
        AddForeignKey("EgresosFijosModel", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
        AddForeignKey("EgresosFijosModel", "Tipo_TipoEgresoFijoModelID", "TipoEgresoFijoModel", "TipoEgresoFijoModelID");
        AddForeignKey("DTEPagosModel", "EgresosFijosModel_EgresosFijosModelID", "EgresosFijosModel", "EgresosFijosModelID");
    }
}
