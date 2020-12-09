using System;
using System.Data.Entity.Migrations;

public partial class SeModicaTablaCartolaBancariaMacro : DbMigration
{
    public override void Up()
    {
        AddColumn("CartolaBancariaMacroModel", "NumeroCartola", c => c.Int(nullable: false));
        DropColumn("CartolaBancariaMacroModel", "NombreCartola");
    }
    
    public override void Down()
    {
        AddColumn("CartolaBancariaMacroModel", "NombreCartola", c => c.String(unicode: false));
        DropColumn("CartolaBancariaMacroModel", "NumeroCartola");
    }
}
