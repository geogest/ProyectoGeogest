using System;
using System.Data.Entity.Migrations;

public partial class Migration13 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickTransporteExportacion", "Marcas", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickTransporteExportacion", "Marcas");
    }
}
