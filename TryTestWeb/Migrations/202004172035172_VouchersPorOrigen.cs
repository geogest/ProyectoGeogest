using System;
using System.Data.Entity.Migrations;

public partial class VouchersPorOrigen : DbMigration
{
    public override void Up()
    {
        AddColumn("VoucherModel", "TipoOrigenVoucher", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropColumn("VoucherModel", "TipoOrigenVoucher");
    }
}
