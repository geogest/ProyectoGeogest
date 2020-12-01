using System;
using System.Data.Entity.Migrations;

public partial class CreandotablasCartola : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "CartolaBancariaModel",
            c => new
                {
                    CartolaBancariaModelId = c.Int(nullable: false, identity: true),
                    Fecha = c.DateTime(nullable: false, precision: 0),
                    Folio = c.Int(nullable: false),
                    Detalle = c.String(unicode: false),
                    Oficina = c.String(unicode: false),
                    Debe = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Haber = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Saldo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    EstaConciliado = c.Boolean(nullable: false),
                    CartolaBancariaMacroModelID_CartolaBancariaMacroModelID = c.Int(),
                    ClientesContablesModelID_ClientesContablesModelID = c.Int(),
                })
            .PrimaryKey(t => t.CartolaBancariaModelId)            
            .ForeignKey("CartolaBancariaMacroModel", t => t.CartolaBancariaMacroModelID_CartolaBancariaMacroModelID)
            .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModelID_ClientesContablesModelID)
            .Index(t => t.CartolaBancariaMacroModelID_CartolaBancariaMacroModelID)
            .Index(t => t.ClientesContablesModelID_ClientesContablesModelID);
        
        CreateTable(
            "CartolaBancariaMacroModel",
            c => new
                {
                    CartolaBancariaMacroModelID = c.Int(nullable: false, identity: true),
                    FechaCartola = c.DateTime(nullable: false, precision: 0),
                    NombreCartola = c.String(unicode: false),
                    ClientesContablesModelID_ClientesContablesModelID = c.Int(),
                })
            .PrimaryKey(t => t.CartolaBancariaMacroModelID)            
            .ForeignKey("ClientesContablesModel", t => t.ClientesContablesModelID_ClientesContablesModelID)
            .Index(t => t.ClientesContablesModelID_ClientesContablesModelID);
        
    }
    
    public override void Down()
    {
        DropForeignKey("CartolaBancariaModel", "ClientesContablesModelID_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("CartolaBancariaMacroModel", "ClientesContablesModelID_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("CartolaBancariaModel", "CartolaBancariaMacroModelID_CartolaBancariaMacroModelID", "CartolaBancariaMacroModel");
        DropIndex("CartolaBancariaMacroModel", new[] { "ClientesContablesModelID_ClientesContablesModelID" });
        DropIndex("CartolaBancariaModel", new[] { "ClientesContablesModelID_ClientesContablesModelID" });
        DropIndex("CartolaBancariaModel", new[] { "CartolaBancariaMacroModelID_CartolaBancariaMacroModelID" });
        DropTable("CartolaBancariaMacroModel");
        DropTable("CartolaBancariaModel");
    }
}
