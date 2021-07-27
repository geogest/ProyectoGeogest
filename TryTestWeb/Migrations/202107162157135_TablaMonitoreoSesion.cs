using System;
using System.Data.Entity.Migrations;

public partial class TablaMonitoreoSesion : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "MonitoreoSesion",
            c => new
                {
                    MonitoreoSesionID = c.Int(nullable: false, identity: true),
                    UsuarioID = c.Int(nullable: false),
                    EstaActivo = c.Boolean(nullable: false),
                })
            .PrimaryKey(t => t.MonitoreoSesionID)            ;
        
    }
    
    public override void Down()
    {
        DropTable("MonitoreoSesion");
    }
}
