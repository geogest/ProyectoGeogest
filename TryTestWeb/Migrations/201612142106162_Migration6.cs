using System;
using System.Data.Entity.Migrations;

public partial class Migration6 : DbMigration
{
    public override void Up()
    {
        AlterColumn("QuickEmisorModel", "RUTEmpresa", c => c.String(nullable: false, maxLength: 10, storeType: "nvarchar"));
        AlterColumn("QuickEmisorModel", "RUTUsuario", c => c.String(nullable: false, maxLength: 10, storeType: "nvarchar"));
        AlterColumn("QuickEmisorModel", "RazonSocial", c => c.String(nullable: false, maxLength: 100, storeType: "nvarchar"));
        AlterColumn("QuickEmisorModel", "Giro", c => c.String(nullable: false, maxLength: 80, storeType: "nvarchar"));
        AlterColumn("QuickEmisorModel", "ActEcono", c => c.String(nullable: false, maxLength: 6, storeType: "nvarchar"));
    }
    
    public override void Down()
    {
        AlterColumn("QuickEmisorModel", "ActEcono", c => c.String(unicode: false));
        AlterColumn("QuickEmisorModel", "Giro", c => c.String(unicode: false));
        AlterColumn("QuickEmisorModel", "RazonSocial", c => c.String(unicode: false));
        AlterColumn("QuickEmisorModel", "RUTUsuario", c => c.String(unicode: false));
        AlterColumn("QuickEmisorModel", "RUTEmpresa", c => c.String(unicode: false));
    }
}
