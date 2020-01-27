using System;
using System.Data.Entity.Migrations;

public partial class Migration33 : DbMigration
{
    public override void Up()
    {
        AddColumn("FacturaQuickModel", "IDElementoLibroDetalle", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("FacturaQuickModel", "IDElementoLibroDetalle");
    }
}
