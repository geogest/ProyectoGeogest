using System;
using System.Data.Entity.Migrations;

public partial class Migration38 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickDetalleModel", "DescuentoGlobalEsPorcentaje", c => c.Boolean(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickDetalleModel", "DescuentoGlobalEsPorcentaje");
    }
}
