using System;
using System.Data.Entity.Migrations;

public partial class Migration3 : DbMigration
{
    public override void Up()
    {
        AddColumn("FacturaQuickModel", "DescuentoGlobalAfectaExentos", c => c.Boolean(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("FacturaQuickModel", "DescuentoGlobalAfectaExentos");
    }
}
