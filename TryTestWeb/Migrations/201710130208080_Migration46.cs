using System;
using System.Data.Entity.Migrations;

public partial class Migration46 : DbMigration
{
    public override void Up()
    {
        AlterColumn("QuickReceptorModel", "Direccion", c => c.String(nullable: false, maxLength: 70, storeType: "nvarchar"));
        AlterColumn("QuickReceptorModel", "Comuna", c => c.String(nullable: false, maxLength: 20, storeType: "nvarchar"));
        AlterColumn("QuickReceptorModel", "Ciudad", c => c.String(maxLength: 20, storeType: "nvarchar"));
    }
    
    public override void Down()
    {
        AlterColumn("QuickReceptorModel", "Ciudad", c => c.String(unicode: false));
        AlterColumn("QuickReceptorModel", "Comuna", c => c.String(unicode: false));
        AlterColumn("QuickReceptorModel", "Direccion", c => c.String(unicode: false));
    }
}
