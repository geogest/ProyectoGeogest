using System;
using System.Data.Entity.Migrations;

public partial class Migration39 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickEmisorModel", "RUTRepresentante", c => c.String(maxLength: 10, storeType: "nvarchar"));
        AddColumn("QuickEmisorModel", "Representante", c => c.String(maxLength: 30, storeType: "nvarchar"));
        AddColumn("FacturaQuickModel", "GeneratedXMLIntercambio", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("FacturaQuickModel", "GeneratedXMLIntercambio");
        DropColumn("QuickEmisorModel", "Representante");
        DropColumn("QuickEmisorModel", "RUTRepresentante");
    }
}
