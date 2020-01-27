using System;
using System.Data.Entity.Migrations;

public partial class Migration21 : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "CAFHistoricoModel",
            c => new
                {
                    CAFHistoricoModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    FechaUpload = c.DateTime(nullable: false, precision: 0),
                    XmlCAF = c.String(unicode: false),
                })
            .PrimaryKey(t => t.CAFHistoricoModelID)            
            .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
            .Index(t => t.QuickEmisorModelID);
        
    }
    
    public override void Down()
    {
        DropForeignKey("CAFHistoricoModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropIndex("CAFHistoricoModel", new[] { "QuickEmisorModelID" });
        DropTable("CAFHistoricoModel");
    }
}
