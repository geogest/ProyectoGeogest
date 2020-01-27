using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.AspNet.Identity;


public class Monitoreo : ActionFilterAttribute
{
    protected DateTime tiempo_inicio;

    protected AccionMonitoreo Accion;
    public string AccionDeclarada { get; set; }


    public static string ReporteMonitoreo(MonitoreoModel objMonitoreo)
    {
        StringBuilder sb = new StringBuilder();
        return sb.ToString();
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        tiempo_inicio = DateTime.Now;
        if (filterContext.ActionParameters.ContainsKey(AccionDeclarada))
        {
            Accion = (AccionMonitoreo)filterContext.ActionParameters[AccionDeclarada];
        }
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        HttpSessionStateBase Session = filterContext.HttpContext.Session;

        string NombreUsuario = filterContext.HttpContext.User.Identity.Name;
        string UserID = filterContext.HttpContext.User.Identity.GetUserId();  //User.Identity.GetUserId();

        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
        ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

        MonitoreoModel objMonitoreo;
        if (objCliente == null)
            objMonitoreo = new MonitoreoModel(db, NombreUsuario, objEmisor.RazonSocial, objEmisor.QuickEmisorModelID);
        else
            objMonitoreo = new MonitoreoModel(db, NombreUsuario, objEmisor.RazonSocial, objCliente.RazonSocial, objEmisor.QuickEmisorModelID, objCliente.ClientesContablesModelID);

        RouteData route_data = filterContext.RouteData;

        objMonitoreo.Tiempo_de_ejecucion = (DateTime.Now - tiempo_inicio);
        objMonitoreo.Controlador = (string)route_data.Values["controller"];
        objMonitoreo.CambiosRealizados = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).Form.ToString();
        objMonitoreo.QueryStrings = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).QueryString.ToString();

        objMonitoreo.AccionTipo = Accion;
        objMonitoreo.AccionNombre = (string)route_data.Values["action"];
        objMonitoreo.Hora_Ejecucion = DateTime.Now;

        db.DBMonitoreo.Add(objMonitoreo);
        db.SaveChanges();

        #region oldnotes
        /*
        RouteData route_data = filterContext.RouteData;
        TimeSpan duration = (DateTime.Now - tiempo_inicio);
        string controller = (string)route_data.Values["controller"];

        string cambiosRealizados =((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).Form.ToString();
        string queryStrings = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).QueryString.ToString();

        string[] cambiosRealizadosArray = cambiosRealizados.Split('&');

        string action = (string)route_data.Values["action"];
        
        DateTime hora_Ejecucion = DateTime.Now;
        */
        #endregion
    }
}

public enum AccionMonitoreo
{
    None = 1,
    Creacion = 2,
    Edicion = 4,
    Acceso = 8
}

#region oldnotes
///HOW TO GET THE DATABASE QUERY BEING EXECUTED ON ENTITY FRAMEWORK
/*
public class MonitoreoAlternativo
{
    public string output = string.Empty; 
    public string LogOperation(SistemaContableContext db)
    {
        db.Database.Log = i => AddToOutput(i);
        return output;
    }

    public void AddToOutput(string text)
    {
        output += text +Environment.NewLine;
    }
}
*/
#endregion