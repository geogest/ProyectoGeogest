using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Migrations;

public partial class EliminadoMultiplesFacturacion : DbMigration
{
    public override void Up()
    {
        DehabilitaLlavesForaneas();
        //DropForeignKey("QuickDetalleModel", "ImpuestoAdicionalProducto_ID", "ImpuestoAdicionalRetencionesModel");
        //DropIndex("QuickDetalleModel", new[] { "ImpuestoAdicionalProducto_ID" });
        DropTable("QuickDetalleModel");
        DropTable("ImpuestoAdicionalRetencionesModel");
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
            "ImpuestoAdicionalRetencionesModel",
            c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    DescripcionImpuesto = c.String(unicode: false),
                    TasaImpuesto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CodImpuesto = c.Double(nullable: false),
                })
            .PrimaryKey(t => t.ID)            ;
        
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
                    Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Precio = c.Int(nullable: false),
                    porcentajeDescuentoRecargoProducto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    GlosaDescuentoRecargo = c.String(unicode: false),
                    TipoDescuentoRecargo = c.Int(nullable: false),
                    DescuentoGlobal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DescuentoGlobalEsPorcentaje = c.Boolean(nullable: false),
                    ExentoIVA = c.Boolean(nullable: false),
                    CodImpuesto = c.Double(nullable: false),
                    ImpuestoAdicionalProducto_ID = c.Int(),
                })
            .PrimaryKey(t => t.ID)            ;
        
        CreateIndex("QuickDetalleModel", "ImpuestoAdicionalProducto_ID");
        AddForeignKey("QuickDetalleModel", "ImpuestoAdicionalProducto_ID", "ImpuestoAdicionalRetencionesModel", "ID");
    }
}
