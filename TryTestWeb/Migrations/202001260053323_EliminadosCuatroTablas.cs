using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Migrations;

public partial class EliminadosCuatroTablas : DbMigration
{
    public override void Up()
    {
        DehabilitaLlavesForaneas();
        //DropForeignKey("DetalleCartolaModel", "CartolaModelID", "CartolaModel");
        //DropForeignKey("CartolaModel", "ClientesContablesModelID", "ClientesContablesModel");
        //DropForeignKey("ConciliacionModel", "Cartola_CartolaModelID", "CartolaModel");
        //DropForeignKey("ConciliacionModel", "CtaContableLibroDiario_CuentaContableModelID", "CuentaContableModel");
        //DropForeignKey("ConciliacionModel", "HechoPorUsuario_UsuarioModelID", "UsuarioModel");
        //DropForeignKey("DetalleConciliacionModel", "ConciliacionModelID", "ConciliacionModel");
        //DropIndex("CartolaModel", new[] { "ClientesContablesModelID" });
        //DropIndex("DetalleCartolaModel", new[] { "CartolaModelID" });
        //DropIndex("ConciliacionModel", new[] { "Cartola_CartolaModelID" });
        //DropIndex("ConciliacionModel", new[] { "CtaContableLibroDiario_CuentaContableModelID" });
        //DropIndex("ConciliacionModel", new[] { "HechoPorUsuario_UsuarioModelID" });
        //DropIndex("DetalleConciliacionModel", new[] { "ConciliacionModelID" });
        DropTable("CartolaModel");
        DropTable("DetalleCartolaModel");
        DropTable("ConciliacionModel");
        DropTable("DetalleConciliacionModel");
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
            "DetalleConciliacionModel",
            c => new
                {
                    DetalleConciliacionModelID = c.Int(nullable: false, identity: true),
                    ConciliacionModelID = c.Int(nullable: false),
                    Fecha = c.DateTime(nullable: false, precision: 0),
                    Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Glosa = c.String(unicode: false),
                    Tipo = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.DetalleConciliacionModelID)            ;
        
        CreateTable(
            "ConciliacionModel",
            c => new
                {
                    ConciliacionModelID = c.Int(nullable: false, identity: true),
                    ClientesContablesModelID = c.Int(nullable: false),
                    FechaEmision = c.DateTime(nullable: false, precision: 0),
                    FechaLibroDiario = c.DateTime(nullable: false, precision: 0),
                    PathToFile = c.String(unicode: false),
                    Cartola_CartolaModelID = c.Int(),
                    CtaContableLibroDiario_CuentaContableModelID = c.Int(),
                    HechoPorUsuario_UsuarioModelID = c.Int(),
                })
            .PrimaryKey(t => t.ConciliacionModelID)            ;
        
        CreateTable(
            "DetalleCartolaModel",
            c => new
                {
                    DetalleCartolaModelID = c.Int(nullable: false, identity: true),
                    CartolaModelID = c.Int(nullable: false),
                    DescripcionMovimiento = c.String(unicode: false),
                    Cargo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Abono = c.Decimal(nullable: false, precision: 18, scale: 2),
                    FechaMovimiento = c.DateTime(nullable: false, precision: 0),
                })
            .PrimaryKey(t => t.DetalleCartolaModelID)            ;
        
        CreateTable(
            "CartolaModel",
            c => new
                {
                    CartolaModelID = c.Int(nullable: false, identity: true),
                    ClientesContablesModelID = c.Int(nullable: false),
                    Fecha = c.DateTime(nullable: false, precision: 0),
                })
            .PrimaryKey(t => t.CartolaModelID)            ;
        
        CreateIndex("DetalleConciliacionModel", "ConciliacionModelID");
        CreateIndex("ConciliacionModel", "HechoPorUsuario_UsuarioModelID");
        CreateIndex("ConciliacionModel", "CtaContableLibroDiario_CuentaContableModelID");
        CreateIndex("ConciliacionModel", "Cartola_CartolaModelID");
        CreateIndex("DetalleCartolaModel", "CartolaModelID");
        CreateIndex("CartolaModel", "ClientesContablesModelID");
        AddForeignKey("DetalleConciliacionModel", "ConciliacionModelID", "ConciliacionModel", "ConciliacionModelID", cascadeDelete: true);
        AddForeignKey("ConciliacionModel", "HechoPorUsuario_UsuarioModelID", "UsuarioModel", "UsuarioModelID");
        AddForeignKey("ConciliacionModel", "CtaContableLibroDiario_CuentaContableModelID", "CuentaContableModel", "CuentaContableModelID");
        AddForeignKey("ConciliacionModel", "Cartola_CartolaModelID", "CartolaModel", "CartolaModelID");
        AddForeignKey("CartolaModel", "ClientesContablesModelID", "ClientesContablesModel", "ClientesContablesModelID", cascadeDelete: true);
        AddForeignKey("DetalleCartolaModel", "CartolaModelID", "CartolaModel", "CartolaModelID", cascadeDelete: true);
    }
}
