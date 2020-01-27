using System;
using System.Data.Entity.Migrations;

public partial class Migration37 : DbMigration
{
    public override void Up()
    {
        //DropForeignKey("ItemContableModel", "CtaContable_ClasificacionCuentaContableModelID", "ClasificacionCuentaContableModel");
        //DropForeignKey("ItemContableModel", "QuickEmisorModelID", "QuickEmisorModel");
        //DropIndex("ItemContableModel", new[] { "QuickEmisorModelID" });
        DropIndex("ItemContableModel", new[] { "CtaContable_ClasificacionCuentaContableModelID" });
        DropTable("ItemContableModel");
        DropTable("ClasificacionCuentaContableModel");
    }
    
    public override void Down()
    {
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
        
        CreateTable(
            "ItemContableModel",
            c => new
                {
                    ItemContableModelID = c.Int(nullable: false, identity: true),
                    CuentaContableModelID = c.Int(nullable: false),
                    QuickEmisorModelID = c.Int(nullable: false),
                    Glosa = c.String(unicode: false),
                    Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    FechaDocumento = c.DateTime(nullable: false, precision: 0),
                    FechaIngreso = c.DateTime(nullable: false, precision: 0),
                    CtaContable_ClasificacionCuentaContableModelID = c.Int(),
                })
            .PrimaryKey(t => t.ItemContableModelID)            ;
        
        CreateIndex("ItemContableModel", "CtaContable_ClasificacionCuentaContableModelID");
        CreateIndex("ItemContableModel", "QuickEmisorModelID");
        AddForeignKey("ItemContableModel", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
        AddForeignKey("ItemContableModel", "CtaContable_ClasificacionCuentaContableModelID", "ClasificacionCuentaContableModel", "ClasificacionCuentaContableModelID");
    }
}
