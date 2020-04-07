using System;
using System.Data.Entity.Migrations;

public partial class ConfigBoletasVentas : DbMigration
{
    public override void Up()
    {
        AddColumn("AuxiliaresDetalleModel", "FolioHasta", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("AuxiliaresDetalleModel", "FolioHasta");
    }
}
