using System;
using System.Data.Entity.Migrations;

public partial class CorreccionMigration : DbMigration
{
    public override void Up()
    {
        AddColumn("LibroDeHonorariosModel", "TipoOrigenVoucher", c => c.Int(nullable: false));
        DropColumn("LibroDeHonorariosModel", "TipoOrigenVoucher");
    }
    
    public override void Down()
    {
        AddColumn("LibroDeHonorariosModel", "TipoOrigenVoucher", c => c.Int(nullable: false));
    }
}
