using System;
using System.Data.Entity.Migrations;

public partial class Migration32 : DbMigration
{
    public override void Up()
    {
        AddColumn("FacturaQuickModel", "EstadoPago", c => c.Int(nullable: false));
        AddColumn("QuickDetalleLibroCompraModel", "EstadoPago", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickDetalleLibroCompraModel", "EstadoPago");
        DropColumn("FacturaQuickModel", "EstadoPago");
    }
}
