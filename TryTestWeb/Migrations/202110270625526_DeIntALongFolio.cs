using System;
using System.Data.Entity.Migrations;

public partial class DeIntALongFolio : DbMigration
{
    public override void Up()
    {
        AlterColumn("AuxiliaresDetalleModel", "Folio", c => c.Long(nullable: false));
    }
    
    public override void Down()
    {
        AlterColumn("AuxiliaresDetalleModel", "Folio", c => c.Int(nullable: false));
    }
}
