using System;
using System.Data.Entity.Migrations;

public partial class Migration1 : DbMigration
{
    public override void Up()
    {
        AddColumn("CertificadosModels", "PathCAF39", c => c.String(unicode: false));
        AddColumn("CertificadosModels", "PathCAF41", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("CertificadosModels", "PathCAF41");
        DropColumn("CertificadosModels", "PathCAF39");
    }
}
