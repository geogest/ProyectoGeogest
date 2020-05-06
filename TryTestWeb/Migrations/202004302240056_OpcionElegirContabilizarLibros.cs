using System;
using System.Data.Entity.Migrations;

public partial class OpcionElegirContabilizarLibros : DbMigration
{
    public override void Up()
    {
        AddColumn("AuxiliaresDetalleModel", "SeVaParaVenta", c => c.Boolean(nullable: false));
        AddColumn("AuxiliaresDetalleModel", "SeVaParaCompra", c => c.Boolean(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("AuxiliaresDetalleModel", "SeVaParaCompra");
        DropColumn("AuxiliaresDetalleModel", "SeVaParaVenta");
    }
}
