using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using System;


public class ModuloHandler : AuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        if (filterContext.HttpContext.User.Identity.IsAuthenticated == false)
        {
            filterContext.Result = RetornarSeleccionEmisor();
            return;
        }

        string VistaAccedida = filterContext.RouteData.Values["action"].ToString();

        string UserID = filterContext.HttpContext.User.Identity.GetUserId();
        var Session = filterContext.HttpContext.Session;

        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

        //rescata usuario
        UsuarioModel objUsuario = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);
        if (objUsuario == null)
        {
            /*REDIRECT A EL PANEL DE SELECCION DE EMISOR*/
            filterContext.Result = RetornarSeleccionEmisor();
            return;
        }

        //rescata emisor seleccionado
        QuickEmisorModel objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID);
        if (objEmisor == null)
        {
            filterContext.Result = RetornarSeleccionEmisor();
            return;
        }
        //ve si el usuario tiene acceso a la vista utilizando este emisor

        IQueryable<ModulosHabilitados> ModulosHabilitados = db.DBModulosHabilitados.Where(r => r.UsuarioModelID == objUsuario.UsuarioModelID && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
        List<string> lstFuncionesUsuario = ModulosHabilitados.Select(w => w.Funcion.NombreFuncion).ToList();
        if (lstFuncionesUsuario.Contains(VistaAccedida))
        {
            List<FuncionesModel> lstFuncionesContabilidad = ModulosHabilitados.Where(r => r.Funcion.NombreFuncion == VistaAccedida).Select(r => r.Funcion).ToList();
            //Si es una funcion de contabilidad, revisar si tiene una ClienteEmisorSeleccionado
            if (lstFuncionesContabilidad.Where(r => r.ModuloSistema != null).Any(r => r.ModuloSistema.ModuloSistemaModelID == ParseExtensions.KeyModuloSistemaContable)) 
            {
                
                ClientesContablesModel clienteSeleccionado = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
                if (clienteSeleccionado == null)
                {
                    filterContext.Result =  RetornarSeleccionClienteContable();               
                    return;
                }    
            }
          
            return;
        }
        else
        {
            filterContext.Result = RetornarSeleccionEmisor();
            return;
        }
    }

    public static bool FuncionAnyContabilidad(HttpContext context)
    {
        //List<string> lstFuncionesUsuario;

        string UserID = context.User.Identity.GetUserId();
        HttpSessionStateBase Session = new HttpSessionStateWrapper(context.Session);

        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

        UsuarioModel objUsuario = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);
        if (objUsuario == null)
            return false;

        QuickEmisorModel objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID);
        if (objEmisor == null)
            return false;

        bool hasFuncionesContablesInModulosHabilitados = db.DBModulosHabilitados
                .Where(
                    r => r.UsuarioModelID == objUsuario.UsuarioModelID &&
                    r.QuickEmisorModelID == objEmisor.QuickEmisorModelID
                ).Any
                (   r => r.Funcion.ModuloSistema.ModuloSistemaModelID == ParseExtensions.KeyModuloSistemaContable   );

        return hasFuncionesContablesInModulosHabilitados;
    }

    public static bool FuncionRequerida(HttpContext context, string ComponenteRequerido)
    {
        List<string> lstFuncionesUsuario;
        //if (context.Session["listaFuncionesAccesibles"] == null)
        //{
            string UserID = context.User.Identity.GetUserId();
            HttpSessionStateBase Session = new HttpSessionStateWrapper(context.Session);

            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            //rescata usuario
            UsuarioModel objUsuario = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);
            if (objUsuario == null)
            {
                return false;
            }

            //rescata emisor seleccionado
            QuickEmisorModel objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID);
            if (objEmisor == null)
                return false;
            lstFuncionesUsuario = db.DBModulosHabilitados
                .Where(r => r.UsuarioModelID == objUsuario.UsuarioModelID && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID)
                .Select(w => w.Funcion.NombreFuncion).ToList();

    //        context.Session["listaFuncionesAccesibles"] = lstFuncionesUsuario;
  //      }
        //lstFuncionesUsuario = (List<string>)context.Session["listaFuncionesAccesibles"];
        if (lstFuncionesUsuario != null && lstFuncionesUsuario.Contains(ComponenteRequerido, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static ActionResult RetornarSeleccionEmisor()
    {
        return new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "Home", action = "SeleccionarEmisorDesdeModulos" }));
    }

    public static ActionResult RetornarSeleccionClienteContable()
    {
        return new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "Contabilidad", action = "SeleccionarClienteContable" }));
    }
}
