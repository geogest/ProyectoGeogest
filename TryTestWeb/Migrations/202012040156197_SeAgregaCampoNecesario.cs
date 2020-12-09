using System;
using System.Data.Entity.Migrations;

public partial class SeAgregaCampoNecesario : DbMigration
{
    public override void Up()
    {
        AddColumn("CartolaBancariaModel", "VoucherModelID", c => c.Int(nullable: false));
        AddColumn("CartolaBancariaMacroModel", "VoucherModelID", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("CartolaBancariaMacroModel", "VoucherModelID");
        DropColumn("CartolaBancariaModel", "VoucherModelID");
    }
}
