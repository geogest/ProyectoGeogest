using System;
using System.Data.Entity.Migrations;

public partial class Migration34 : DbMigration
{
    public override void Up()
    {
        DropForeignKey("ItemContableModel", "CuentaContableModelID", "CuentaContableModel");
        DropIndex("ItemContableModel", new[] { "CuentaContableModelID" });
        CreateTable(
            "ClasificacionCuentaContableModel",
            c => new
                {
                    ClasificacionCuentaContableModelID = c.Int(nullable: false, identity: true),
                    Nombre = c.String(unicode: false),
                    Clasificacion = c.Int(nullable: false),
                    SubClasificacion = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.ClasificacionCuentaContableModelID)            ;
        
        AddColumn("ItemContableModel", "CtaContable_ClasificacionCuentaContableModelID", c => c.Int());
        CreateIndex("ItemContableModel", "CtaContable_ClasificacionCuentaContableModelID");
        AddForeignKey("ItemContableModel", "CtaContable_ClasificacionCuentaContableModelID", "ClasificacionCuentaContableModel", "ClasificacionCuentaContableModelID");
        DropTable("CuentaContableModel");
    }
    
    public override void Down()
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
        
        DropForeignKey("ItemContableModel", "CtaContable_ClasificacionCuentaContableModelID", "ClasificacionCuentaContableModel");
        DropIndex("ItemContableModel", new[] { "CtaContable_ClasificacionCuentaContableModelID" });
        DropColumn("ItemContableModel", "CtaContable_ClasificacionCuentaContableModelID");
        DropTable("ClasificacionCuentaContableModel");
        CreateIndex("ItemContableModel", "CuentaContableModelID");
        AddForeignKey("ItemContableModel", "CuentaContableModelID", "CuentaContableModel", "CuentaContableModelID", cascadeDelete: true);
    }
}
