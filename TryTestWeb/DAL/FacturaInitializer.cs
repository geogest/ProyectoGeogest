using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TryTestWeb.App_Start;
using TryTestWeb.Models;

public class FacturaInitializer : System.Data.Entity.MigrateDatabaseToLatestVersion<FacturaContext, MyConfiguration> 
{
   
}



public class MyConfiguration : System.Data.Entity.Migrations.DbMigrationsConfiguration<FacturaContext>
{
    public MyConfiguration()
    {
        AutomaticMigrationsEnabled = false;
        //AutomaticMigrationDataLossAllowed = true;
        SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        CodeGenerator = new MySql.Data.Entity.MySqlMigrationCodeGenerator();
    }
}

//-----------------

public class FacturaProduccionInitializer : System.Data.Entity.MigrateDatabaseToLatestVersion<FacturaProduccionContext, MyProduccionConfiguration>
{
    /*System.Data.Entity.DropCreateDatabaseAlways<FacturaProduccionContext*/ /**/


    /*
     Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SalesToolEntities>());
     */
}

public class MyProduccionConfiguration : System.Data.Entity.Migrations.DbMigrationsConfiguration<FacturaProduccionContext>
{
    public MyProduccionConfiguration()
    {
        AutomaticMigrationsEnabled = false;
        //AutomaticMigrationDataLossAllowed = true;
        SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        CodeGenerator = new MySql.Data.Entity.MySqlMigrationCodeGenerator();
    }
}