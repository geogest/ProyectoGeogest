using System;
using System.Data.Entity.Migrations;

public partial class CampoDeConciliacionBancariaDetalleVoucher : DbMigration
{
    public override void Up()
    {
        AddColumn("DetalleVoucherModel", "Conciliado", c => c.Boolean(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("DetalleVoucherModel", "Conciliado");
    }
}
