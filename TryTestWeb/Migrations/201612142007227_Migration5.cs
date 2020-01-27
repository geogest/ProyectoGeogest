using System;
using System.Data.Entity.Migrations;

public partial class Migration5 : DbMigration
{
    public override void Up()
    {
        AlterColumn("QuickDetalleModel", "Cantidad", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        AlterColumn("QuickDetalleModel", "DescuentoProducto", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        AlterColumn("QuickDetalleModel", "DescuentoGlobal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        AlterColumn("ImpuestoAdicionalRetencionesModel", "TasaImpuesto", c => c.Decimal(nullable: false, precision: 18, scale: 2));
    }
    
    public override void Down()
    {
        AlterColumn("ImpuestoAdicionalRetencionesModel", "TasaImpuesto", c => c.Double(nullable: false));
        AlterColumn("QuickDetalleModel", "DescuentoGlobal", c => c.Double(nullable: false));
        AlterColumn("QuickDetalleModel", "DescuentoProducto", c => c.Double(nullable: false));
        AlterColumn("QuickDetalleModel", "Cantidad", c => c.Double(nullable: false));
    }
}
