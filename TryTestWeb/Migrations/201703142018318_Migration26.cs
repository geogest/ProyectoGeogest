using System;
using System.Data.Entity.Migrations;

public partial class Migration26 : DbMigration
{
    public override void Up()
    {
        AddColumn("IntercambioModel", "RutToSendEMail", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("IntercambioModel", "RutToSendEMail");
    }
}
