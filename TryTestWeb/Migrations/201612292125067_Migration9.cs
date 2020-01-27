using System;
using System.Data.Entity.Migrations;

public partial class Migration9 : DbMigration
{
    public override void Up()
    {
        AlterColumn("QuickDetalleLibroCompraModel", "CodigoIVANoRecuperable", c => c.Int());
    }
    
    public override void Down()
    {
        AlterColumn("QuickDetalleLibroCompraModel", "CodigoIVANoRecuperable", c => c.Int(nullable: false));
    }
}
