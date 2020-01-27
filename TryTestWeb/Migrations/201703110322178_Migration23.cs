using System;
using System.Data.Entity.Migrations;

public partial class Migration23 : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "IntercambioModel",
            c => new
                {
                    IntercambioModelID = c.Int(nullable: false, identity: true),
                    QuickEmisorModelID = c.Int(nullable: false),
                    xmlDocumentAttachment = c.String(unicode: false),
                    attachmentName = c.String(unicode: false),
                    mailUID = c.String(unicode: false),
                    rutEmisor = c.String(unicode: false),
                    fromMail = c.String(unicode: false),
                    dateProcessed = c.DateTime(nullable: false, precision: 0),
                    tipoOperacion = c.Int(nullable: false),
                    xmlGeneratedResponse = c.String(unicode: false),
                    status = c.Int(nullable: false),
                })
            .PrimaryKey(t => t.IntercambioModelID)            ;
        
    }
    
    public override void Down()
    {
        DropTable("IntercambioModel");
    }
}
