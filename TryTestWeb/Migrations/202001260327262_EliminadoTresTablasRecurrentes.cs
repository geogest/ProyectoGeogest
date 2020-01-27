using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Migrations;

public partial class EliminadoTresTablasRecurrentes : DbMigration
{
    public override void Up()
    {
        DehabilitaLlavesForaneas();
        //DropForeignKey("DetalleFacturaRecurrente", "ImpuestoAdicionalProducto_ID", "ImpuestoAdicionalRetencionesModel");
        //DropForeignKey("DetalleFacturaRecurrente", "FacturaRecurrenteModelID", "FacturaRecurrenteModel");
        //DropForeignKey("FacturaRecurrenteModel", "QuickEmisorModelID", "QuickEmisorModel");
        //DropForeignKey("FacturaRecurrenteModel", "ObjTotals_TotalesFacturaRecurrenteID", "TotalesFacturaRecurrente");
        //DropForeignKey("FacturaRecurrenteModel", "Receptor_QuickReceptorModelID", "QuickReceptorModel");
        //DropForeignKey("ReferenciasFacturaRecurrente", "FacturaRecurrenteModelID", "FacturaRecurrenteModel");
        //DropIndex("DetalleFacturaRecurrente", new[] { "FacturaRecurrenteModelID" });
        //DropIndex("DetalleFacturaRecurrente", new[] { "ImpuestoAdicionalProducto_ID" });
        //DropIndex("FacturaRecurrenteModel", new[] { "QuickEmisorModelID" });
        //DropIndex("FacturaRecurrenteModel", new[] { "ObjTotals_TotalesFacturaRecurrenteID" });
        //DropIndex("FacturaRecurrenteModel", new[] { "Receptor_QuickReceptorModelID" });
        //DropIndex("ReferenciasFacturaRecurrente", new[] { "FacturaRecurrenteModelID" });
        DropTable("DetalleFacturaRecurrente");
        DropTable("FacturaRecurrenteModel");
        DropTable("TotalesFacturaRecurrente");
    }


    private void DehabilitaLlavesForaneas()
    {
        string DBProduccion = "Server=localhost;Port=3306;Database=ProductionDB;Uid=root;Pwd=root;SslMode=Preferred;";

        using (MySqlConnection db = new MySqlConnection(DBProduccion))
        {
            db.Open();
            MySqlCommand DeshabilitandoLlaves = new MySqlCommand("SET FOREIGN_KEY_CHECKS=0;", db);
            DeshabilitandoLlaves.ExecuteNonQuery();
        }

        string DBCertificacion = "Server=localhost;Port=3306;Database=FacturaDB;Uid=root;Pwd=root;SslMode=Preferred;";

        using (MySqlConnection dbCert = new MySqlConnection(DBCertificacion))
        {
            dbCert.Open();
            MySqlCommand DeshabilitandoLlavesCert = new MySqlCommand("SET FOREIGN_KEY_CHECKS=0;", dbCert);
            DeshabilitandoLlavesCert.ExecuteNonQuery();
        }
    }

    public override void Down()
    {
        CreateTable(
            "TotalesFacturaRecurrente",
            c => new
                {
                    TotalesFacturaRecurrenteID = c.Int(nullable: false, identity: true),
                    FacturaRecurrenteModelID = c.Int(nullable: false),
                    SubTotalMonto = c.Int(nullable: false),
                    DescuentoGlobalMonto = c.Int(nullable: false),
                    MontoNetoMonto = c.Int(nullable: false),
                    MontoExento = c.Int(nullable: false),
                    IVAMonto = c.Int(nullable: false),
                    ImpuestoAdicionalMonto = c.Int(nullable: false),
                    TotalMonto = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.TotalesFacturaRecurrenteID)            ;
        
        CreateTable(
            "FacturaRecurrenteModel",
            c => new
                {
                    FacturaRecurrenteModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    NumFolio = c.Int(nullable: false),
                    DescuentoGlobal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DescuentoGlobalAfectaExentos = c.Boolean(nullable: false),
                    FechaRecurrenteInicio = c.DateTime(nullable: false, precision: 0),
                    FechaRecurrenteFinal = c.DateTime(precision: 0),
                    FrecuenciaRecurrencia = c.Int(nullable: false),
                    EstadoRecurrencia = c.Int(nullable: false),
                    NextExecution = c.DateTime(nullable: false, precision: 0),
                    TipoFactura = c.Int(nullable: false),
                    FechaVencPago = c.DateTime(nullable: false, precision: 0),
                    EnumFechaPago = c.Int(nullable: false),
                    DetalleLastError = c.String(unicode: false),
                    FechaLastError = c.DateTime(precision: 0),
                    ObjTotals_TotalesFacturaRecurrenteID = c.Int(),
                    Receptor_QuickReceptorModelID = c.Int(),
                })
            .PrimaryKey(t => t.FacturaRecurrenteModelID)            ;
        
        CreateTable(
            "DetalleFacturaRecurrente",
            c => new
                {
                    DetalleFacturaRecurrenteID = c.Int(nullable: false, identity: true),
                    FacturaRecurrenteModelID = c.Int(nullable: false),
                    CodProd = c.Int(nullable: false),
                    NombreProducto = c.String(unicode: false),
                    Descripcion = c.String(unicode: false),
                    UMedida = c.String(unicode: false),
                    Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Precio = c.Int(nullable: false),
                    porcentajeDescuentoRecargoProducto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    GlosaDescuentoRecargo = c.String(unicode: false),
                    TipoDescuentoRecargo = c.Int(nullable: false),
                    DescuentoGlobal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DescuentoGlobalEsPorcentaje = c.Boolean(nullable: false),
                    ExentoIVA = c.Boolean(nullable: false),
                    CodImpuesto = c.Double(nullable: false),
                    ImpuestoAdicionalProducto_ID = c.Int(),
                })
            .PrimaryKey(t => t.DetalleFacturaRecurrenteID)            ;
        
        CreateIndex("ReferenciasFacturaRecurrente", "FacturaRecurrenteModelID");
        CreateIndex("FacturaRecurrenteModel", "Receptor_QuickReceptorModelID");
        CreateIndex("FacturaRecurrenteModel", "ObjTotals_TotalesFacturaRecurrenteID");
        CreateIndex("FacturaRecurrenteModel", "QuickEmisorModelID");
        CreateIndex("DetalleFacturaRecurrente", "ImpuestoAdicionalProducto_ID");
        CreateIndex("DetalleFacturaRecurrente", "FacturaRecurrenteModelID");
        AddForeignKey("ReferenciasFacturaRecurrente", "FacturaRecurrenteModelID", "FacturaRecurrenteModel", "FacturaRecurrenteModelID", cascadeDelete: true);
        AddForeignKey("FacturaRecurrenteModel", "Receptor_QuickReceptorModelID", "QuickReceptorModel", "QuickReceptorModelID");
        AddForeignKey("FacturaRecurrenteModel", "ObjTotals_TotalesFacturaRecurrenteID", "TotalesFacturaRecurrente", "TotalesFacturaRecurrenteID");
        AddForeignKey("FacturaRecurrenteModel", "QuickEmisorModelID", "QuickEmisorModel", "QuickEmisorModelID", cascadeDelete: true);
        AddForeignKey("DetalleFacturaRecurrente", "FacturaRecurrenteModelID", "FacturaRecurrenteModel", "FacturaRecurrenteModelID", cascadeDelete: true);
        AddForeignKey("DetalleFacturaRecurrente", "ImpuestoAdicionalProducto_ID", "ImpuestoAdicionalRetencionesModel", "ID");
    }
}
