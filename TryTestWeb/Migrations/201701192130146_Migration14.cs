using System;
using System.Data.Entity.Migrations;

public partial class Migration14 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickTransporteExportacion", "FormaPagoExportacion", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickTransporteExportacion", "FormaPagoExportacion");
    }
}
