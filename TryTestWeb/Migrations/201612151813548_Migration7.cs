using System;
using System.Data.Entity.Migrations;

public partial class Migration7 : DbMigration
{
    public override void Up()
    {
        AlterColumn("QuickReceptorModel", "RUT", c => c.String(nullable: false, maxLength: 10, storeType: "nvarchar"));
        AlterColumn("QuickReceptorModel", "RazonSocial", c => c.String(nullable: false, maxLength: 100, storeType: "nvarchar"));
        AlterColumn("QuickReceptorModel", "Giro", c => c.String(nullable: false, maxLength: 80, storeType: "nvarchar"));
    }
    
    public override void Down()
    {
        AlterColumn("QuickReceptorModel", "Giro", c => c.String(unicode: false));
        AlterColumn("QuickReceptorModel", "RazonSocial", c => c.String(unicode: false));
        AlterColumn("QuickReceptorModel", "RUT", c => c.String(unicode: false));
    }
}
