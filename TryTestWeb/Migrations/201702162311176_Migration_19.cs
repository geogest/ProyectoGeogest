using System;
using System.Data.Entity.Migrations;

public partial class Migration_19 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickTransporteExportacion", "ValorDROtrMnda", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        AddColumn("QuickTransporteExportacion", "RecargoPct", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        AddColumn("QuickTransporteExportacion", "NombreTransp", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickTransporteExportacion", "NombreTransp");
        DropColumn("QuickTransporteExportacion", "RecargoPct");
        DropColumn("QuickTransporteExportacion", "ValorDROtrMnda");
    }
}
