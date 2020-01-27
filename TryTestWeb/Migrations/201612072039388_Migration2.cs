using System;
using System.Data.Entity.Migrations;

public partial class Migration2 : DbMigration
{
    public override void Up()
    {
        AddColumn("FacturaQuickModel", "IndicadorServicio", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("FacturaQuickModel", "IndicadorServicio");
    }
}
