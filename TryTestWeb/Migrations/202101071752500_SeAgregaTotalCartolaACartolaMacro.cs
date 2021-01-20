using System;
using System.Data.Entity.Migrations;

public partial class SeAgregaTotalCartolaACartolaMacro : DbMigration
{
    public override void Up()
    {
        AddColumn("CartolaBancariaMacroModel", "TotalCartola", c => c.Decimal(nullable: false, precision: 18, scale: 2));
    }
    
    public override void Down()
    {
        DropColumn("CartolaBancariaMacroModel", "TotalCartola");
    }
}
