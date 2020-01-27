using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TryTestWeb.Models;

public static class LocatorInitializationHandler
{
    public static void Initialize()
    {
        
        Database.SetInitializer(new FacturaInitializer()); //if u want to use your initializer
        using (var db = new FacturaContext())
        {
            {
                db.Database.Initialize(false);    
            }
        }

        Database.SetInitializer(new FacturaProduccionInitializer());
        using (var dvo = new FacturaProduccionContext())
        {
            {
                 dvo.Database.Initialize(false);   
            }
        }
    }
}