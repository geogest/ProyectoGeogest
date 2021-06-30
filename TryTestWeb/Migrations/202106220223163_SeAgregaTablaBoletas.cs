using System;
using System.Data.Entity.Migrations;

public partial class SeAgregaTablaBoletas : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "BoletasCoVModel",
            c => new
                {
                    BoletasCoVModelID = c.Int(nullable: false, identity: true),
                    CuentaAuxiliar = c.String(unicode: false),
                    VoucherModelID = c.Int(nullable: false),
                    HaSidoConvertidoAVoucher = c.Int(nullable: false),
                    FechaInsercion = c.DateTime(nullable: false, precision: 0),
                    Fecha = c.DateTime(nullable: false, precision: 0),
                    NumeroDeDocumento = c.Int(nullable: false),
                    TipoDocumento = c.Int(nullable: false),
                    FechaVencimiento = c.DateTime(nullable: false, precision: 0),
                    CuentaContable = c.String(unicode: false),
                    Neto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Iva = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CentroDeCostos = c.Int(nullable: false),
                    FechaPeriodoTributario = c.DateTime(nullable: false, precision: 0),
                    ClienteContable_ClientesContablesModelID = c.Int(),
                    Prestador_QuickReceptorModelID = c.Int(),
                })
            .PrimaryKey(t => t.BoletasCoVModelID)            
            .ForeignKey("ClientesContablesModel", t => t.ClienteContable_ClientesContablesModelID)
            .ForeignKey("QuickReceptorModel", t => t.Prestador_QuickReceptorModelID)
            .Index(t => t.ClienteContable_ClientesContablesModelID)
            .Index(t => t.Prestador_QuickReceptorModelID);
        
    }
    
    public override void Down()
    {
        DropForeignKey("BoletasCoVModel", "Prestador_QuickReceptorModelID", "QuickReceptorModel");
        DropForeignKey("BoletasCoVModel", "ClienteContable_ClientesContablesModelID", "ClientesContablesModel");
        DropIndex("BoletasCoVModel", new[] { "Prestador_QuickReceptorModelID" });
        DropIndex("BoletasCoVModel", new[] { "ClienteContable_ClientesContablesModelID" });
        DropTable("BoletasCoVModel");
    }
}
