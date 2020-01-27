using System;
using System.Data.Entity.Migrations;

public partial class Migration35 : DbMigration
{
    public override void Up()
    {
        AddColumn("CertificadosModels", "PathCAF46", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("CertificadosModels", "PathCAF46");
    }
}
