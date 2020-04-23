using System;
using System.Data.Entity.Migrations;

public partial class SeAgregaDarDeBajaUser : DbMigration
{
    public override void Up()
    {
        AddColumn("UsuarioModel", "EstaDadoDeBaja", c => c.Boolean(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("UsuarioModel", "EstaDadoDeBaja");
    }
}
