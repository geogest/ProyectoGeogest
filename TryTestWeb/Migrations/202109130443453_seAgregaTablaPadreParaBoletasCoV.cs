using System;
using System.Data.Entity.Migrations;

public partial class seAgregaTablaPadreParaBoletasCoV : DbMigration
{
    public override void Up()
    {
        DropTable("MarcaEnElTiempoNumVoucherNuevo");
    }
    
    public override void Down()
    {
        CreateTable(
            "MarcaEnElTiempoNumVoucherNuevo",
            c => new
                {
                    MarcaEnElTiempoNumVoucherNuevoID = c.Int(nullable: false, identity: true),
                    ClienteContableModelID = c.Int(nullable: false),
                    NumVoucher = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.MarcaEnElTiempoNumVoucherNuevoID)            ;
        
    }
}
