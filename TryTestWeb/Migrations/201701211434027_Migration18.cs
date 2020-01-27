using System;
using System.Data.Entity.Migrations;

public partial class Migration18 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickTransporteExportacion", "IndicadorServicio", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickTransporteExportacion", "IndicadorServicio");
    }
}
