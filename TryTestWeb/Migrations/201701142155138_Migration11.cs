using System;
using System.Data.Entity.Migrations;

public partial class Migration11 : DbMigration
{
    public override void Up()
    {
        AddColumn("CertificadosModels", "PathCAF111", c => c.String(unicode: false));
        AddColumn("CertificadosModels", "PathCAF112", c => c.String(unicode: false));
    }
    
    public override void Down()
    {
        DropColumn("CertificadosModels", "PathCAF112");
        DropColumn("CertificadosModels", "PathCAF111");
    }
}
