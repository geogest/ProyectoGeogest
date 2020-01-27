using System;
using System.Data.Entity.Migrations;

public partial class Migration4 : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "FlujoCajaModel",
            c => new
                {
                    FlujoCajaModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    Periodo = c.DateTime(nullable: false, precision: 0),
                    EgresosProveedores = c.Int(nullable: false),
                    EgresosIvas = c.Int(nullable: false),
                    EgresosCotizacionesPrevisionales = c.Int(nullable: false),
                    EgresosCargosBancarios = c.Int(nullable: false),
                    EgresosRemuneraciones = c.Int(nullable: false),
                    EgresosHonorariosFijos = c.Int(nullable: false),
                    EgresosRetiros = c.Int(nullable: false),
                    EgresosArriendo = c.Int(nullable: false),
                    EgresosGastosComunes = c.Int(nullable: false),
                    EgresosLuz = c.Int(nullable: false),
                    EgresosAgua = c.Int(nullable: false),
                    EgresosTelefonos = c.Int(nullable: false),
                    EgresosGastosAdministrativos = c.Int(nullable: false),
                    EgresosEstacionamiento = c.Int(nullable: false),
                    EgresosReembolsos = c.Int(nullable: false),
                    EgresosOtros = c.Int(nullable: false),
                    EgresosVariableHonorarios = c.Int(nullable: false),
                    EgresosVariableInteresesSobregiro = c.Int(nullable: false),
                    EgresosVariablePagoTarjeta = c.Int(nullable: false),
                    EgresosVariableAbonoLineaCredito = c.Int(nullable: false),
                    EgresosVariableInteresLineaCredito = c.Int(nullable: false),
                    EgresosVariableReembolso = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.FlujoCajaModelID)            
            .ForeignKey("QuickEmisorModel", t => t.QuickEmisorModelID, cascadeDelete: true)
            .Index(t => t.QuickEmisorModelID);
        
    }
    
    public override void Down()
    {
        DropForeignKey("FlujoCajaModel", "QuickEmisorModelID", "QuickEmisorModel");
        DropIndex("FlujoCajaModel", new[] { "QuickEmisorModelID" });
        DropTable("FlujoCajaModel");
    }
}
