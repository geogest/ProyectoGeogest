using System;
using System.Data.Entity.Migrations;

public partial class EliminadoCafHistorico : DbMigration
{
    public override void Up()
    {
        DropForeignKey("CAFHistoricoModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropIndex("CAFHistoricoModel", new[] { "QuickEmisorModelID" });
        DropTable("CAFHistoricoModel");
    }
    
    public override void Down()
    {
        CreateTable(
            "CAFHistoricoModel",
            c => new
                {
                    CAFHistoricoModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    FechaUpload = c.DateTime(nullable: false, precision: 0),
                    XmlCAF = c.String(unicode: false),
                    tipoCAF = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.CAFHistoricoModelID)            ;
        
        CreateIndex("CAFHistoricoModel", "QuickEmisorModelID");
        AddForeignKey("CAFHistoricoModel", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
    }
}
