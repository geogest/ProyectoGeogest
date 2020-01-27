using System;
using System.Data.Entity.Migrations;

public partial class Migration30 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickDetalleLibroCompraModel", "CuentaContableModelID", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickDetalleLibroCompraModel", "CuentaContableModelID");
    }
}
