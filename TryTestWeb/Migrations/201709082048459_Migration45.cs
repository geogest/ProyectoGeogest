using System;
using System.Data.Entity.Migrations;

public partial class Migration45 : DbMigration
{
    public override void Up()
    {
        AddColumn("FacturaQuickModel", "FechaVencPago", c => c.DateTime(nullable: false, precision: 0));
        AddColumn("FacturaQuickModel", "EnumFechaPago", c => c.Int(nullable: false));
        AddColumn("QuickReceptorModel", "EMailCobranza", c => c.String(unicode: false));
        AddColumn("QuickReceptorModel", "TelefonoCobranza", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("QuickReceptorModel", "TelefonoCobranza");
        DropColumn("QuickReceptorModel", "EMailCobranza");
        DropColumn("FacturaQuickModel", "EnumFechaPago");
        DropColumn("FacturaQuickModel", "FechaVencPago");
    }
}
