using System;
using System.Data.Entity.Migrations;

public partial class seAgregaRelacionPadre : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "BoletasCoVPadreModel",
            c => new
                {
                    BoletasCoVPadreModelID = c.Int(nullable: false, identity: true),
                    FechaBoletas = c.DateTime(nullable: false, precision: 0),
                    FechaCreacion = c.DateTime(nullable: false, precision: 0),
                    TotalNeto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TotalIva = c.Decimal(nullable: false, precision: 18, scale: 2),
                    ClienteContableModelID_ClientesContablesModelID = c.Int(),
                })
            .PrimaryKey(t => t.BoletasCoVPadreModelID)            
            .ForeignKey("ClientesContablesModel", t => t.ClienteContableModelID_ClientesContablesModelID)
            .Index(t => t.ClienteContableModelID_ClientesContablesModelID);
        
        AddColumn("BoletasCoVModel", "BoletaCoVPadre_BoletasCoVPadreModelID", c => c.Int());
        CreateIndex("BoletasCoVModel", "BoletaCoVPadre_BoletasCoVPadreModelID");
        AddForeignKey("BoletasCoVModel", "BoletaCoVPadre_BoletasCoVPadreModelID", "BoletasCoVPadreModel", "BoletasCoVPadreModelID");
    }
    
    public override void Down()
    {
        DropForeignKey("BoletasCoVPadreModel", "ClienteContableModelID_ClientesContablesModelID", "ClientesContablesModel");
        DropForeignKey("BoletasCoVModel", "BoletaCoVPadre_BoletasCoVPadreModelID", "BoletasCoVPadreModel");
        DropIndex("BoletasCoVModel", new[] { "BoletaCoVPadre_BoletasCoVPadreModelID" });
        DropIndex("BoletasCoVPadreModel", new[] { "ClienteContableModelID_ClientesContablesModelID" });
        DropColumn("BoletasCoVModel", "BoletaCoVPadre_BoletasCoVPadreModelID");
        DropTable("BoletasCoVPadreModel");
    }
}
