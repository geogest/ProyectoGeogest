using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ModuloHelper
{
    public static ClientesContablesModel GetClienteContableSeleccionado(HttpSessionStateBase objSession, string userID, FacturaPoliContext db = null)
    {
        ClientesContablesModel returnValue = null;
        if (db == null)
            db = ParseExtensions.GetDatabaseContext(userID);

        QuickEmisorModel objEmisor = GetEmisorSeleccionado(objSession, userID, db);
        if (objEmisor == null)
            return returnValue;

        bool Esta_En_Certificacion = ParseExtensions.ItsUserOnCertificationEnvironment(userID);
        //CERTIFICACION
        int IDClienteContable = 0;
        if (Esta_En_Certificacion)
        {
            if (objSession["ClienteContableSeleccionado_CERT"] == null)
                return null;
            IDClienteContable = (int)objSession["ClienteContableSeleccionado_CERT"];
        }
        //PRODUCCION
        else
        {
            if (objSession["ClienteContableSeleccionado"] == null)
                return null;
            IDClienteContable = (int)objSession["ClienteContableSeleccionado"];
        }
        returnValue = objEmisor.lstClientesCompania.SingleOrDefault(r => r.ClientesContablesModelID == IDClienteContable);
        return returnValue;
    }

    public static QuickEmisorModel GetEmisorSeleccionado(HttpSessionStateBase objSession, string userID, FacturaPoliContext db = null)
    {
        if (db == null)
            db = ParseExtensions.GetDatabaseContext(userID);

        bool Esta_En_Certificacion = ParseExtensions.ItsUserOnCertificationEnvironment(userID);
        //CERTIFICACION
        int IDEmisor = -1;
        if (Esta_En_Certificacion)
        {
            if (objSession["EmisorSeleccionado_CERT"] == null)
                return null;
            IDEmisor = (int)objSession["EmisorSeleccionado_CERT"];
        }
        //PRODUCCION
        else
        {
            if (objSession["EmisorSeleccionado"] == null)
                return null;
            IDEmisor = (int)objSession["EmisorSeleccionado"];
        }
        QuickEmisorModel returnValue = null;

        ModuloHelper.GetEmisorHabilitado(userID, IDEmisor, out returnValue, db);
        return returnValue;
    }

   

    public static bool GetEmisorHabilitado(string userID, int IDEmisor, out QuickEmisorModel objEmisorHabilitado, FacturaPoliContext db)
    {
        if (db == null)
            db = ParseExtensions.GetDatabaseContext(userID);

        List<QuickEmisorModel> emisoresHabilitados = null;
        if (GetEmisoresHabilitados(userID, out emisoresHabilitados, db))
        {
            objEmisorHabilitado = emisoresHabilitados.SingleOrDefault(r => r.QuickEmisorModelID == IDEmisor);
            if (objEmisorHabilitado != null)
                return true;
        }
        objEmisorHabilitado = null;
        return false;
    }

    public static bool GetEmisoresHabilitados(string userID, out List<QuickEmisorModel> objEmisores, FacturaPoliContext db = null)
    {
        if (db == null)
            db = ParseExtensions.GetDatabaseContext(userID);

        UsuarioModel objUser = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == userID);

        List<int> lstIDEmpresasHabilitadas = db.DBModulosHabilitados
        .Where(r => r.UsuarioModelID == objUser.UsuarioModelID)
        .Select(w => w.QuickEmisorModelID).Distinct().ToList();

        List<QuickEmisorModel> lstEmisoresHabilitados = new List<QuickEmisorModel>();
        foreach (int IDEmpresa in lstIDEmpresasHabilitadas)
        {
            QuickEmisorModel objEmpresa = db.Emisores.Include("Certificados").SingleOrDefault(R => R.QuickEmisorModelID == IDEmpresa);
            lstEmisoresHabilitados.Add(objEmpresa);
        }
        objEmisores = lstEmisoresHabilitados;
        if (objEmisores == null || objEmisores.Count() == 0)
            return false;
        return true;
    }

    public static void PurgeSession(HttpSessionStateBase objSession)
    {
        objSession["EmisorSeleccionadoNombre_CERT"] = null;
        objSession["EmisorSeleccionadoNombre"] = null;
        objSession["EmisorSeleccionado_CERT"] = null;
        objSession["EmisorSeleccionado"] = null;
        objSession["listaFuncionesAccesibles"] = null;
        objSession["AlertaCertificadoShowOnce"] = null;

        //Contabilidad
        objSession["ClienteContableSeleccionado_CERT"] = null;
        objSession["ClienteContableSeleccionado_CERT_nombre"] = null;

        objSession["ClienteContableSeleccionado"] = null;
        objSession["ClienteContableSeleccionado_nombre"] = null;
    }

    public static void PurgeSessionContable(HttpSessionStateBase objSession)
    {
        objSession["ClienteContableSeleccionado_CERT"] = null;
        objSession["ClienteContableSeleccionado_CERT_nombre"] = null;

        objSession["ClienteContableSeleccionado"] = null;
        objSession["ClienteContableSeleccionado_nombre"] = null;
    }
}
