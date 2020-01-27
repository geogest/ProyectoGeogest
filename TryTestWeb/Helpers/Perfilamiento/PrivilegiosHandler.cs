using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Web.Routing;


public class PrivilegiosHandler : AuthorizeAttribute
{
    protected Privilegios PrivilegioMin = Privilegios.Administrador;
    public Privilegios PrivilegioMinimoRequerido { get; set; }

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        //Obtiene el privilegio minimo requerido del parametro expuesto al interno
        PrivilegioMin = PrivilegioMinimoRequerido;

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

        QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
        if (objEmisor == null)
        {
            /*REDIRECT A EL PANEL DE SELECCION DE EMISOR*/
            filterContext.Result = RetornarSeleccionEmisor();
            return;
        }

        //Revisar su tabla de compañias autorizadas
        EmisoresHabilitados objEmisorHabilitado = db.DBEmisoresHabilitados.SingleOrDefault(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.UsuarioModelID == objUsuario.UsuarioModelID);
        if (objEmisorHabilitado == null)
        {
            /*REDIRECT A EL PANEL DE SELECCION DE EMISOR*/
            filterContext.Result = RetornarSeleccionEmisor();
            return;
        }

        Privilegios PrivilegioDelUsuario = objEmisorHabilitado.privilegiosAcceso;
        bool EstaAutorizado = ManejarPrivilegios(PrivilegioMin, PrivilegioDelUsuario);

        if (EstaAutorizado == false)
        {
            /*REDIRECT A EL PANEL DE SELECCION DE EMISOR*/
            filterContext.Result = RetornarSeleccionEmisor();
            return;
        }
    }

    public static bool ManejarPrivilegios(Privilegios PrivilegioRequerido, Privilegios PrivilegioDado)
    {
        if (PrivilegioRequerido == Privilegios.Administrador)
        {
            if (PrivilegioDado == Privilegios.Administrador)
                return true;
            else
                return false;
        }
        else if (PrivilegioRequerido == Privilegios.Facturador)
        {
            if (PrivilegioDado == Privilegios.Administrador || PrivilegioDado == Privilegios.Facturador)
                return true;
            else
                return false;
        }
        else if (PrivilegioRequerido == Privilegios.Informador)
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
}
