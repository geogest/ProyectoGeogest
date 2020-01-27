using System;
using System.Data.Entity.Migrations;

public partial class Migration8 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickDetalleLibroCompraModel", "CodigoIVANoRecuperable", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickDetalleLibroCompraModel", "CodigoIVANoRecuperable");
    }
}
