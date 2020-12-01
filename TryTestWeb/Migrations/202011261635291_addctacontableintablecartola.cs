using System;
using System.Data.Entity.Migrations;

public partial class addctacontableintablecartola : DbMigration
{
    public override void Up()
    {
        AddColumn("CartolaBancariaModel", "CuentaContableModelID_CuentaContableModelID", c => c.Int());
        AddColumn("CartolaBancariaMacroModel", "CuentaContableModelID_CuentaContableModelID", c => c.Int());
        CreateIndex("CartolaBancariaModel", "CuentaContableModelID_CuentaContableModelID");
        CreateIndex("CartolaBancariaMacroModel", "CuentaContableModelID_CuentaContableModelID");
        AddForeignKey("CartolaBancariaMacroModel", "CuentaContableModelID_CuentaContableModelID", "CuentaContableModel", "CuentaContableModelID");
        AddForeignKey("CartolaBancariaModel", "CuentaContableModelID_CuentaContableModelID", "CuentaContableModel", "CuentaContableModelID");
    }
    
    public override void Down()
    {
        DropForeignKey("CartolaBancariaModel", "CuentaContableModelID_CuentaContableModelID", "CuentaContableModel");
        DropForeignKey("CartolaBancariaMacroModel", "CuentaContableModelID_CuentaContableModelID", "CuentaContableModel");
        DropIndex("CartolaBancariaMacroModel", new[] { "CuentaContableModelID_CuentaContableModelID" });
        DropIndex("CartolaBancariaModel", new[] { "CuentaContableModelID_CuentaContableModelID" });
        DropColumn("CartolaBancariaMacroModel", "CuentaContableModelID_CuentaContableModelID");
        DropColumn("CartolaBancariaModel", "CuentaContableModelID_CuentaContableModelID");
    }
}
