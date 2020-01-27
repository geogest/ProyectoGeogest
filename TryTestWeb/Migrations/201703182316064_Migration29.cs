using System;
using System.Data.Entity.Migrations;

public partial class Migration29 : DbMigration
{
    public override void Up()
    {
        AddColumn("ItemContableModel", "QuickEmisorModelID", c => c.Int(nullable: false));
        CreateIndex("ItemContableModel", "QuickEmisorModelID");
        AddForeignKey("ItemContableModel", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
    }
    
    public override void Down()
    {
        DropForeignKey("ItemContableModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropIndex("ItemContableModel", new[] { "QuickEmisorModelID" });
        DropColumn("ItemContableModel", "QuickEmisorModelID");
    }
}
