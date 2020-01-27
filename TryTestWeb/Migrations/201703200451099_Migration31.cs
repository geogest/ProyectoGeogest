using System;
using System.Data.Entity.Migrations;

public partial class Migration31 : DbMigration
{
    public override void Up()
    {
        AddColumn("FacturaQuickModel", "GeneratedXML", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("FacturaQuickModel", "GeneratedXML");
    }
}
