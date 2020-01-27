using System;
using System.Data.Entity.Migrations;

public partial class Migration44 : DbMigration
{
    public override void Up()
    {
        AddColumn("FacturaQuickModel", "FechaIngresoSistema", c => c.DateTime(nullable: false, precision: 0));
        AddColumn("FacturaQuickModel", "FechaAcuseRecibo", c => c.DateTime(nullable: false, precision: 0));
    }
    
    public override void Down()
    {
        DropColumn("FacturaQuickModel", "FechaAcuseRecibo");
        DropColumn("FacturaQuickModel", "FechaIngresoSistema");
    }
}
