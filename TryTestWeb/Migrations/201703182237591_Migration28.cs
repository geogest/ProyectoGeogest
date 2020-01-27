using System;
using System.Data.Entity.Migrations;

public partial class Migration28 : DbMigration
{
    public override void Up()
    {
        CreateIndex("ItemContableModel", "CuentaContableModelID");
        AddForeignKey("ItemContableModel", "CuentaContableModelID", "CuentaContableModel", "CuentaContableModelID", cascadeDelete: true);
    }
    
    public override void Down()
    {
        DropForeignKey("ItemContableModel", "CuentaContableModelID", "CuentaContableModel");
        DropIndex("ItemContableModel", new[] { "CuentaContableModelID" });
    }
}
