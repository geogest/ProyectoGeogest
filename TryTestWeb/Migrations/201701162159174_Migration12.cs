using System;
using System.Data.Entity.Migrations;

public partial class Migration12 : DbMigration
{
    public override void Up()
    {
        AddColumn("QuickTransporteExportacion", "CodUnidPesoNeto", c => c.Int(nullable: false));
        AddColumn("QuickTransporteExportacion", "CodTipoBultos", c => c.Int(nullable: false));
        AddColumn("QuickTransporteExportacion", "CodPuertoEmbarque", c => c.Int(nullable: false));
        AddColumn("QuickTransporteExportacion", "CodPuertoDesembarque", c => c.Int(nullable: false));
        AddColumn("QuickTransporteExportacion", "MontoFlete", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        AddColumn("QuickTransporteExportacion", "MontoSeguro", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        AlterColumn("QuickTransporteExportacion", "TotClauVenta", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        AlterColumn("QuickTransporteExportacion", "PesoBruto", c => c.Decimal(nullable: false, precision: 18, scale: 2));
    }
    
    public override void Down()
    {
        AlterColumn("QuickTransporteExportacion", "PesoBruto", c => c.Double(nullable: false));
        AlterColumn("QuickTransporteExportacion", "TotClauVenta", c => c.Double(nullable: false));
        DropColumn("QuickTransporteExportacion", "MontoSeguro");
        DropColumn("QuickTransporteExportacion", "MontoFlete");
        DropColumn("QuickTransporteExportacion", "CodPuertoDesembarque");
        DropColumn("QuickTransporteExportacion", "CodPuertoEmbarque");
        DropColumn("QuickTransporteExportacion", "CodTipoBultos");
        DropColumn("QuickTransporteExportacion", "CodUnidPesoNeto");
    }
}
