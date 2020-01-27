using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

public class ReminderHelper
{
    private static object Lock = new object();

    public void EjecutarProcesoRecordatorioImpagas(bool useProd = false)
    {
        if (Monitor.TryEnter(Lock))
        {
            try
            {

            }
            finally
            {
                Monitor.Exit(Lock);
            }
        }
    }
}

   

   