using System;
using System.Data.Entity.Migrations;

public partial class Migration10 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickDetalleLibroCompraModel", "FactorProporcionalidad", c => c.Decimal(nullable: false, precision: 18, scale: 2));
    }
    
    public override void Down()
    {
        DropColumn("QuickDetalleLibroCompraModel", "FactorProporcionalidad");
    }
}
