using System;
using System.Data.Entity.Migrations;

public partial class Migration40 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickLibroCompraModel", "LibroXMLOutput", c => c.String(unicode: false));
        AddColumn("QuickDetalleLibroCompraModel", "FacturaCompraXMLOutput", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickDetalleLibroCompraModel", "FacturaCompraXMLOutput");
        DropColumn("QuickLibroCompraModel", "LibroXMLOutput");
    }
}
