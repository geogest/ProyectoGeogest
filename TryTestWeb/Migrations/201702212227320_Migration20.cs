using System;
using System.Data.Entity.Migrations;

public partial class Migration20 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickDetalleLibroCompraModel", "codigoEstadoReception", c => c.Int(nullable: false));
        AddColumn("QuickDetalleLibroCompraModel", "GlosaRechazo", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickDetalleLibroCompraModel", "GlosaRechazo");
        DropColumn("QuickDetalleLibroCompraModel", "codigoEstadoReception");
    }
}
