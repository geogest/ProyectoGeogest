using System;
using System.Data.Entity.Migrations;

public partial class Migration22 : DbMigration
{
    public override void Up()
    {
        AddColumn("CAFHistoricoModel", "tipoCAF", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("CAFHistoricoModel", "tipoCAF");
    }
}
