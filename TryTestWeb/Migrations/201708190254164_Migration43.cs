using System;
using System.Data.Entity.Migrations;

public partial class Migration43 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickTransporteExportacion", "IdContainer", c => c.String(unicode: false));
        AddColumn("QuickTransporteExportacion", "Sello", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickTransporteExportacion", "Sello");
        DropColumn("QuickTransporteExportacion", "IdContainer");
    }
}
