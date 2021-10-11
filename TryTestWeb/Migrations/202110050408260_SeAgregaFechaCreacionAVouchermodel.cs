using System;
using System.Data.Entity.Migrations;

public partial class SeAgregaFechaCreacionAVouchermodel : DbMigration
{
    public override void Up()
    {
        AddColumn("VoucherModel", "FechaCreacion", c => c.DateTime(precision: 0));
    }
    
    public override void Down()
    {
        DropColumn("VoucherModel", "FechaCreacion");
    }
}
