using System;
using System.Data.Entity.Migrations;

public partial class Migration16 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickTransporteExportacion", "Nacionalidad", c => c.Int(nullable: false));
        AddColumn("QuickTransporteExportacion", "RecargoGlobalExportacion", c => c.Decimal(nullable: false, precision: 18, scale: 2));
    }
    
    public override void Down()
    {
        DropColumn("QuickTransporteExportacion", "RecargoGlobalExportacion");
        DropColumn("QuickTransporteExportacion", "Nacionalidad");
    }
}
