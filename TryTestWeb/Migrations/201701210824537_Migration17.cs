using System;
using System.Data.Entity.Migrations;

public partial class Migration17 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickDetalleModel", "GlosaDescuentoRecargo", c => c.String(unicode: false));
        AddColumn("QuickDetalleModel", "TipoDescuentoRecargo", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickDetalleModel", "TipoDescuentoRecargo");
        DropColumn("QuickDetalleModel", "GlosaDescuentoRecargo");
    }
}
