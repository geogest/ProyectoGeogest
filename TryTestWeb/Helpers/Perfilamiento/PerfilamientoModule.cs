using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public static class PerfilamientoModule
{

    public static QuickEmisorModel GetEmisorSeleccionado(HttpSessionStateBase objSession, string userID, FacturaPoliContext db = null)
    {
        if (db == null)
            db = ParseExtensions.GetDatabaseContext(userID);

        return ModuloHelper.GetEmisorSeleccionado(objSession, userID, db);
    }

    public static ClientesContablesModel GetClienteContableSeleccionado(HttpSessionStateBase objSession, string UserID, FacturaPoliContext db = null)
    {
        return ModuloHelper.GetClienteContableSeleccionado(objSession, UserID, db);
    }

    public static bool GetEmisor(string userID, int IDEmisor, out QuickEmisorModel objEmisor, FacturaPoliContext db = null)
    {
        if (db == null)
            db = ParseExtensions.GetDatabaseContext(userID);

        EmisoresHabilitados objEmisorHabilitado;
        if (GetEmisorHabilitado(userID, IDEmisor, out objEmisorHabilitado, db))
        {
            objEmisor = db.Emisores.Include("Certificados").SingleOrDefault(r => r.QuickEmisorModelID == objEmisorHabilitado.QuickEmisorModelID);
            if (objEmisor == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            objEmisor = null;
            return false;
        }
    }
    public static bool GetEmisorCertificacion(string userID, int IDEmisor, out QuickEmisorModel objEmisor)
    {
        //Este constructor hace referencia puntual a la base de datos de CERTIFICACION
        FacturaPoliContext db = new FacturaPoliContext();

        EmisoresHabilitados objEmisorHabilitado;
        if (GetEmisorHabilitado(userID, IDEmisor, out objEmisorHabilitado, db))
        {
            objEmisor = db.Emisores.Include("Certificados").SingleOrDefault(r => r.QuickEmisorModelID == objEmisorHabilitado.QuickEmisorModelID);
            if (objEmisor == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            objEmisor = null;
            return false;
        }
    }

    public static bool GetEmisorProduccion(string userID, int IDEmisor, out QuickEmisorModel objEmisor)
    {
        //Este constructor hace referencia puntual a la base de datos de PRODUCCION
        FacturaPoliContext db = new FacturaPoliContext(true);

        EmisoresHabilitados objEmisorHabilitado;
        if (GetEmisorHabilitado(userID, IDEmisor, out objEmisorHabilitado, db))
        {
            objEmisor = db.Emisores.Include("Certificados").SingleOrDefault(r => r.QuickEmisorModelID == objEmisorHabilitado.QuickEmisorModelID);
            if (objEmisor == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            objEmisor = null;
            return false;
        }
    }
    public static bool GetEmisorHabilitado(string userID, int IDEmisor, out EmisoresHabilitados objEmisorHabilitado, FacturaPoliContext db = null)
    {
        if (db == null)
            db = ParseExtensions.GetDatabaseContext(userID);

        List<EmisoresHabilitados> colcEmisores = null;
        if (GetEmisoresHabilitados(userID, out colcEmisores, db))
        {
            IEnumerable<EmisoresHabilitados> lstEmisores = colcEmisores.Where(r => r.QuickEmisorModelID == IDEmisor);
            if (lstEmisores.Count() > 0)
            {
                if (lstEmisores.Count() == 1)
                {
                    objEmisorHabilitado = lstEmisores.ElementAt(0);
                    return true;
                }
                else
                {
                    int min = lstEmisores.Min(r => (int)r.privilegiosAcceso);
                    objEmisorHabilitado = lstEmisores.SingleOrDefault(r => (int)r.privilegiosAcceso == min);
                    return true;
                }
            }
        }
        objEmisorHabilitado = null;
        return false;
    }

    public static List<QuickEmisorModel> TranslateToFull(this List<EmisoresHabilitados> lstEmisorEnabled, FacturaPoliContext db)
    {
        List<QuickEmisorModel> lstQuickEmisorModel = new List<QuickEmisorModel>();
        foreach (EmisoresHabilitados elementEmisorHabilitado in lstEmisorEnabled)
        {
            var EmisorReal = db.Emisores.SingleOrDefault(r => r.QuickEmisorModelID == elementEmisorHabilitado.QuickEmisorModelID);
            if (EmisorReal != null)
                lstQuickEmisorModel.Add(EmisorReal);
        }
        return lstQuickEmisorModel;
    }

    

    public static bool GetEmisoresHabilitados(string userID, out List<EmisoresHabilitados> objEmisores, FacturaPoliContext db = null)
    {
        if (db == null)
            db = ParseExtensions.GetDatabaseContext(userID);

        UsuarioModel objUser = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == userID);
        if (objUser != null && objUser.lstEmisoresAccesibles != null && objUser.lstEmisoresAccesibles.Count > 0)
        {
            objEmisores = objUser.lstEmisoresAccesibles.ToList();

            List<QuickEmisorModel> realEmisores = db.Emisores.ToList();
            Dictionary<int, string> map = realEmisores.ToDictionary(x => x.QuickEmisorModelID, x => x.RazonSocial);

            foreach (var item in objEmisores)
                if (map.ContainsKey(item.QuickEmisorModelID))
                    item.NombreCompania = map[item.QuickEmisorModelID];
        }
        else
        {
            objEmisores = null;
            return false;
        }
        return true;
    }

    public static bool MostrarConPrivilegio(Privilegios min, string UserID)
    {
        if (HttpContext.Current.Session != null)
        { 
            bool Esta_En_Certificacion = ParseExtensions.ItsUserOnCertificationEnvironment(UserID);
            if (Esta_En_Certificacion)
            {
                if (HttpContext.Current.Session["PrivCert"] != null)
                {
                    Privilegios PrivilegioEnCache = (Privilegios)HttpContext.Current.Session["PrivCert"];
                    return PrivilegiosHandler.ManejarPrivilegios(min, PrivilegioEnCache);
                }
                return false;
            }
            else
            {
                if (HttpContext.Current.Session["PrivProd"] != null)
                {
                    Privilegios PrivilegioEnCache = (Privilegios)HttpContext.Current.Session["PrivProd"];
                    return PrivilegiosHandler.ManejarPrivilegios(min, PrivilegioEnCache);
                }
                return false;
            }
        }
        return false;
    }

    public static List<SelectListItem> ObtenerUsuariosPoseidos(string userID)
    {
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(userID);
        List<SelectListItem> ListaRetorno = new List<SelectListItem>();

        UsuarioModel objUsuario = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == userID);
        if (objUsuario.IsUserSuperAdmin() == false)
            return ListaRetorno;

        foreach (PosesionUsuarios possesed in objUsuario.lstUsuariosPoseidos)
        {
            UsuarioModel Usuario = db.DBUsuarios.SingleOrDefault(r => r.UsuarioModelID == possesed.UsuarioPoseidoID);
            ListaRetorno.Add(new SelectListItem { Text = Usuario.RUT + " " + Usuario.Nombre, Value = Usuario.UsuarioModelID.ToString() });
        }
        return ListaRetorno;
    }

    public static bool IsUserSuperAdmin(this UsuarioModel objUsuario)
    {
        if (objUsuario == null || objUsuario.SuperAdminUser == false)
            return false;
        else
            return true;
    }
}