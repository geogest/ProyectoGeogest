using System;
using System.Data.Entity.Migrations;

public partial class SeAgregaTablaErrorMensaje : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "ErrorMensajeMonitoreo",
            c => new
                {
                    ErrorMensajeMonitoreoID = c.Int(nullable: false, identity: true),
                    Mensaje = c.String(unicode: false),
                })
            .PrimaryKey(t => t.ErrorMensajeMonitoreoID);
        
    }
    
    public override void Down()
    {
        DropTable("ErrorMensajeMonitoreo");
    }
}
