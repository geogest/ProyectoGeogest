using System;
using System.Data.Entity.Migrations;

public partial class Migration15 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickDetalleModel", "porcentajeDescuentoRecargoProducto", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        DropColumn("QuickDetalleModel", "DescuentoProducto");
    }
    
    public override void Down()
    {
        AddColumn("QuickDetalleModel", "DescuentoProducto", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        DropColumn("QuickDetalleModel", "porcentajeDescuentoRecargoProducto");
    }
}
