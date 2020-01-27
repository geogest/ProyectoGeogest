using System;
using System.Data.Entity.Migrations;

public partial class Migration36 : DbMigration
{
    public override void Up()
    {
        AlterColumn("FacturaQuickModel", "DescuentoGlobal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
    }
    
    public override void Down()
    {
        AlterColumn("FacturaQuickModel", "DescuentoGlobal", c => c.Double(nullable: false));
    }
}
