using System;
using System.Data.Entity.Migrations;

public partial class SeAgreganTablasParaControlarUpdates : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "NovedadesModel",
            c => new
                {
                    NovedadesModelID = c.Int(nullable: false, identity: true),
                    FechaEjecucionNovedadEstecliente = c.DateTime(nullable: false, precision: 0),
                    ClienteContable_ClientesContablesModelID = c.Int(),
                    Novedad_NovedadesRegistradasModelID = c.Int(),
                })
            .PrimaryKey(t => t.NovedadesModelID)            
            .ForeignKey("ClientesContablesModel", t => t.ClienteContable_ClientesContablesModelID)
            .ForeignKey("NovedadesRegistradasModel", t => t.Novedad_NovedadesRegistradasModelID)
            .Index(t => t.ClienteContable_ClientesContablesModelID)
            .Index(t => t.Novedad_NovedadesRegistradasModelID);
        
        CreateTable(
            "NovedadesRegistradasModel",
            c => new
                {
                    NovedadesRegistradasModelID = c.Int(nullable: false, identity: true),
                    NombreNovedad = c.String(unicode: false),
                    FechaCreacionNovedad = c.DateTime(nullable: false, precision: 0),
                    NombreFuncionalidadAsociada = c.String(unicode: false),
                })
            .PrimaryKey(t => t.NovedadesRegistradasModelID)            ;
        
    }
    
    public override void Down()
    {
        DropForeignKey("NovedadesModel", "Novedad_NovedadesRegistradasModelID", "NovedadesRegistradasModel");
        DropForeignKey("NovedadesModel", "ClienteContable_ClientesContablesModelID", "ClientesContablesModel");
        DropIndex("NovedadesModel", new[] { "Novedad_NovedadesRegistradasModelID" });
        DropIndex("NovedadesModel", new[] { "ClienteContable_ClientesContablesModelID" });
        DropTable("NovedadesRegistradasModel");
        DropTable("NovedadesModel");
    }
}
