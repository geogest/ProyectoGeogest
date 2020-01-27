using System;
using System.Data.Entity.Migrations;

public partial class Migration25 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickDetalleLibroCompraModel", "IdentificadorIntercambioEnvio", c => c.Int(nullable: false));
        AddColumn("QuickDetalleLibroCompraModel", "IdentificadorIntercambioDoc", c => c.Int(nullable: false));
        AddColumn("IntercambioModel", "IdentificadorIntercambioEnvio", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("IntercambioModel", "IdentificadorIntercambioEnvio");
        DropColumn("QuickDetalleLibroCompraModel", "IdentificadorIntercambioDoc");
        DropColumn("QuickDetalleLibroCompraModel", "IdentificadorIntercambioEnvio");
    }
}
