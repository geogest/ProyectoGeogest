using System;
using System.Data.Entity.Migrations;

public partial class SeAgregaColumnaPresupuesto : DbMigration
{
    public override void Up()
    {
        AddColumn("PresupuestoModel", "DadoDeBaja", c => c.Boolean(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("PresupuestoModel", "DadoDeBaja");
    }
}
