using System;
using System.Data.Entity.Migrations;

public partial class CampoConciliacionEstaCtasCtes : DbMigration
{
    public override void Up()
    {
        AddColumn("DetalleVoucherModel", "ConciliadoCtasCtes", c => c.Boolean(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("DetalleVoucherModel", "ConciliadoCtasCtes");
    }
}
