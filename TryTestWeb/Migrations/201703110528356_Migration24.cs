using System;
using System.Data.Entity.Migrations;

public partial class Migration24 : DbMigration
{
    public override void Up()
    {
        AddColumn("FacturaQuickModel", "StatusIntercambio", c => c.String(unicode: false));
        AddColumn("FacturaQuickModel", "StatusIntercambioGlosa", c => c.String(unicode: false));
        AddColumn("IntercambioModel", "GlosaDescriptiva", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("IntercambioModel", "GlosaDescriptiva");
        DropColumn("FacturaQuickModel", "StatusIntercambioGlosa");
        DropColumn("FacturaQuickModel", "StatusIntercambio");
    }
}
