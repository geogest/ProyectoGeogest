using System;
using System.Data.Entity.Migrations;

public partial class SeCreaLibroHonorTerceros : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "LibroHonorariosDeTerceros",
            c => new
                {
                    LibroHonorariosDeTercerosID = c.Int(nullable: false, identity: true),
                    NumOFolio = c.Int(nullable: false),
                    Estado = c.String(unicode: false),
                    FechaInicial = c.DateTime(nullable: false, precision: 0),
                    RutEmpresa = c.String(unicode: false),
                    NombreEmpresa = c.String(unicode: false),
                    FechaFinal = c.DateTime(nullable: false, precision: 0),
                    FechaContabilizacion = c.DateTime(nullable: false, precision: 0),
                    RutReceptor = c.String(unicode: false),
                    NombreReceptor = c.String(unicode: false),
                    Brutos = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Retenidos = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Pagado = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TipoLibro = c.Int(nullable: false),
                    TipoV = c.Int(nullable: false),
                    TipoO = c.Int(nullable: false),
                    HaSidoConvertidoAVoucher = c.Boolean(nullable: false),
                    EstaDadoDeBaja = c.Boolean(nullable: false),
                    FechaDeCreacion = c.DateTime(nullable: false, precision: 0),
                    ClienteContable_ClientesContablesModelID = c.Int(),
                    Receptor_QuickReceptorModelID = c.Int(),
                    VoucherModel_VoucherModelID = c.Int(),
                })
            .PrimaryKey(t => t.LibroHonorariosDeTercerosID)            
            .ForeignKey("ClientesContablesModel", t => t.ClienteContable_ClientesContablesModelID)
            .ForeignKey("QuickReceptorModel", t => t.Receptor_QuickReceptorModelID)
            .ForeignKey("VoucherModel", t => t.VoucherModel_VoucherModelID)
            .Index(t => t.ClienteContable_ClientesContablesModelID)
            .Index(t => t.Receptor_QuickReceptorModelID)
            .Index(t => t.VoucherModel_VoucherModelID);
        
        //AddColumn("LibroDeHonorariosModel", "TipoOrigenVoucher", c => c.Int(nullable: false));
    }
    
    public override void Down()
    {
        DropForeignKey("LibroHonorariosDeTerceros", "VoucherModel_VoucherModelID", "VoucherModel");
        DropForeignKey("LibroHonorariosDeTerceros", "Receptor_QuickReceptorModelID", "QuickReceptorModel");
        DropForeignKey("LibroHonorariosDeTerceros", "ClienteContable_ClientesContablesModelID", "ClientesContablesModel");
        DropIndex("LibroHonorariosDeTerceros", new[] { "VoucherModel_VoucherModelID" });
        DropIndex("LibroHonorariosDeTerceros", new[] { "Receptor_QuickReceptorModelID" });
        DropIndex("LibroHonorariosDeTerceros", new[] { "ClienteContable_ClientesContablesModelID" });
        DropColumn("LibroDeHonorariosModel", "TipoOrigenVoucher");
        DropTable("LibroHonorariosDeTerceros");
    }
}
