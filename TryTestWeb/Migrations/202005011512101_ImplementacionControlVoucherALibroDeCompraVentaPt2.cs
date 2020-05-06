using System;
using System.Data.Entity.Migrations;

public partial class ImplementacionControlVoucherALibroDeCompraVentaPt2 : DbMigration
{
    public override void Up()
    {
        AddColumn("LibrosContablesModel", "FolioHasta", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("LibrosContablesModel", "FolioHasta");
    }
}
