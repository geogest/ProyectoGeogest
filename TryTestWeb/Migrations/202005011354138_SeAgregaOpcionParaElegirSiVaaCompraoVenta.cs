using System;
using System.Data.Entity.Migrations;

public partial class SeAgregaOpcionParaElegirSiVaaCompraoVenta : DbMigration
{
    public override void Up()
    {
        AddColumn("LibrosContablesModel", "EsUnRegistroManual", c => c.Boolean(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("LibrosContablesModel", "EsUnRegistroManual");
    }
}
