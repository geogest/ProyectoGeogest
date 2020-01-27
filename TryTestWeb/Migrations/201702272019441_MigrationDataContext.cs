using System;
using System.Data.Entity.Migrations;

public partial class MigrationDataContext : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickEmisorModel", "DatabaseContextToUse", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickEmisorModel", "DatabaseContextToUse");
    }
}
