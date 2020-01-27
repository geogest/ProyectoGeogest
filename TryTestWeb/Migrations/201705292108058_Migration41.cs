using System;
using System.Data.Entity.Migrations;

public partial class Migration41 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickTransporteExportacion", "TipoDespacho", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickTransporteExportacion", "TipoDespacho");
    }
}
