using System;
using System.Data.Entity.Migrations;

public partial class folioALong : DbMigration
{
    public override void Up()
    {
        AlterColumn("LibrosContablesModel", "Folio", c => c.Long(nullable: false));
        AlterColumn("LibrosContablesModel", "FolioHasta", c => c.Long(nullable: false));
        AlterColumn("AuxiliaresDetalleModel", "FolioHasta", c => c.Long(nullable: false));
        AlterColumn("CartolaBancariaModel", "Folio", c => c.Long(nullable: false));
    }
    
    public override void Down()
    {
        AlterColumn("CartolaBancariaModel", "Folio", c => c.Int(nullable: false));
        AlterColumn("AuxiliaresDetalleModel", "FolioHasta", c => c.Int(nullable: false));
        AlterColumn("LibrosContablesModel", "FolioHasta", c => c.Int(nullable: false));
        AlterColumn("LibrosContablesModel", "Folio", c => c.Int(nullable: false));
    }
}
