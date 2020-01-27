using System;
using System.Data.Entity.Migrations;

public partial class EliminadoFlujoCajaNewModel : DbMigration
{
    public override void Up()
    {
        DropTable("FlujoCajaNewModel");
    }
    
    public override void Down()
    {
        CreateTable(
            "FlujoCajaNewModel",
            c => new
                {
                    FlujoCajaNewModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    Periodo = c.DateTime(nullable: false, precision: 0),
                    PagoProveedores = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoImposiciones = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoImpuestos = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoCargoBanco = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoRemuneracionesLiquido = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoHonorariosLiquido = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoRetiros = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoArriendo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoGastosComunes = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoLuz = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoAgua = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoGas = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoTelefonos = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoGastosAdministrativos = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoReembolsos = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PagoOtros = c.Decimal(nullable: false, precision: 18, scale: 2),
                    IngresoPagoCliente = c.Decimal(nullable: false, precision: 18, scale: 2),
                    IngresoDepositoBanco = c.Decimal(nullable: false, precision: 18, scale: 2),
                    IngresosOtrosBanco = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
            .PrimaryKey(t => t.FlujoCajaNewModelID)            ;
        
    }
}
