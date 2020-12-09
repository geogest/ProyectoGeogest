using System;
using System.Data.Entity.Migrations;

public partial class SeEliminaColumnaCartola : DbMigration
{
    public override void Up()
    {
        DropColumn("CartolaBancariaMacroModel", "VoucherModelID");
    }
    
    public override void Down()
    {
        AddColumn("CartolaBancariaMacroModel", "VoucherModelID", c => c.Int(nullable: false));
    }
}
