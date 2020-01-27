using System;
using System.Data.Entity.Migrations;

public partial class Migration42 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickLibroCompraModel", "StatusLibro", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickLibroCompraModel", "StatusLibro");
    }
}
