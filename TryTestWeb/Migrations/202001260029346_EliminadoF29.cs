using System;
using System.Data.Entity.Migrations;

public partial class EliminadoF29 : DbMigration
{
    public override void Up()
    {
        DropForeignKey("DTEPagosModel", "F29Model_F29ModelID", "F29Model");
        DropForeignKey("F29Model", "QuickEmisorModelID", "QuickEmisorModel");
        DropIndex("DTEPagosModel", new[] { "F29Model_F29ModelID" });
        DropIndex("F29Model", new[] { "QuickEmisorModelID" });
        DropColumn("DTEPagosModel", "F29Model_F29ModelID");
        DropTable("F29Model");
    }
    
    public override void Down()
    {
        CreateTable(
            "F29Model",
            c => new
                {
                    F29ModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    Folio = c.Int(nullable: false),
                    Periodo = c.DateTime(nullable: false, precision: 0),
                    Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
            .PrimaryKey(t => t.F29ModelID)            ;
        
        AddColumn("DTEPagosModel", "F29Model_F29ModelID", c => c.Int());
        CreateIndex("F29Model", "QuickEmisorModelID");
        CreateIndex("DTEPagosModel", "F29Model_F29ModelID");
        AddForeignKey("F29Model", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
        AddForeignKey("DTEPagosModel", "F29Model_F29ModelID", "F29Model", "F29ModelID");
    }
}
