using System;
using System.Data.Entity.Migrations;

public partial class Migration27 : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "CuentaContableModel",
            c => new
                {
                    CuentaContableModelID = c.Int(nullable: false, identity: true),
                    Nombre = c.String(unicode: false),
                    Clasificacion = c.Int(nullable: false),
                    SubClasificacion = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.CuentaContableModelID)            ;
        
        CreateTable(
            "ItemContableModel",
            c => new
                {
                    ItemContableModelID = c.Int(nullable: false, identity: true),
                    CuentaContableModelID = c.Int(nullable: false),
                    Glosa = c.String(unicode: false),
                    Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    FechaDocumento = c.DateTime(nullable: false, precision: 0),
                    FechaIngreso = c.DateTime(nullable: false, precision: 0),
                })
            .PrimaryKey(t => t.ItemContableModelID)            ;
        
    }
    
    public override void Down()
    {
        DropTable("ItemContableModel");
        DropTable("CuentaContableModel");
    }
}
