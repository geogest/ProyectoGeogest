using System;
using System.Data.Entity.Migrations;

public partial class SeBorraColumna : DbMigration
{
    public override void Up()
    {
        DropColumn("LibroHonorariosDeTerceros", "EstaDadoDeBaja");
    }
    
    public override void Down()
    {
        AddColumn("LibroHonorariosDeTerceros", "EstaDadoDeBaja", c => c.Boolean(nullable: false));
    }
}
