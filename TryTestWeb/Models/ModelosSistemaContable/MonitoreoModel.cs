using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class MonitoreoModel
{
    public int MonitoreoModelID { get; set; }

    public virtual UsuarioModel Usuario { get; set; }
    public string NombreUsuario { get; set; }

    public string Controlador { get; set; }
    public AccionMonitoreo AccionTipo { get; set; }
    public string CambiosRealizados { get; set; }
    public string QueryStrings { get; set; }

    public string AccionNombre { get; set; }

    public virtual QuickEmisorModel Compania { get; set; }
    public string CompaniaSeleccionadaNombre { get; set; }

    public virtual ClientesContablesModel Cliente { get; set; }
    public string ClienteSeleccionadoNombre { get; set; }

    public TimeSpan Tiempo_de_ejecucion { get; set; }
    public DateTime Hora_Ejecucion { get; set; }

    public MonitoreoModel()
    {

    }

    public MonitoreoModel(FacturaPoliContext _db, string _NombreUsuario, string _EmisorSelectNombre, int _EmisorSeleccionadoID)
    {
        NombreUsuario = _NombreUsuario;
        FacturaPoliContext db = _db;

        //El nombre de usuario en IdentityASP es actualmente el EMail, asi que la comparacion se debe hacer en base de eso
        // TODO: En el futuro esta linea debe ser reemplazada por el nombre del usuario, cabe mencionar que el nombre del usuario
        // debe poblarse al registrar un usuario (cambio que tambien esta pendiente) 
        Usuario = db.DBUsuarios.Single(r => r.Email == NombreUsuario);
        if (_EmisorSeleccionadoID != 0)
        {
            Compania = db.Emisores.Single(r => r.QuickEmisorModelID == _EmisorSeleccionadoID);
        }
        if (string.IsNullOrWhiteSpace(_EmisorSelectNombre) == false)
            CompaniaSeleccionadaNombre = _EmisorSelectNombre;
    }

    public MonitoreoModel(FacturaPoliContext _db, string _NombreUsuario, string _EmisorSelectNombre, string _ClienteContableNombre, int _EmisorSeleccionadoID, int _ClienteContableID)
    {
        NombreUsuario = _NombreUsuario;
        FacturaPoliContext db = _db;

        //El nombre de usuario en IdentityASP es actualmente el EMail, asi que la comparacion se debe hacer en base de eso
        // TODO: En el futuro esta linea debe ser reemplazada por el nombre del usuario, cabe mencionar que el nombre del usuario
        // debe poblarse al registrar un usuario (cambio que tambien esta pendiente) 
        Usuario = db.DBUsuarios.Single(r => r.Email == NombreUsuario);
        if (_EmisorSeleccionadoID != 0)
        {
            Compania = db.Emisores.Single(r => r.QuickEmisorModelID == _EmisorSeleccionadoID);
        }
        if (string.IsNullOrWhiteSpace(_EmisorSelectNombre) == false)
            CompaniaSeleccionadaNombre = _EmisorSelectNombre;


        if (_ClienteContableID != 0)
        {
            Cliente = db.DBClientesContables.Single(r => r.ClientesContablesModelID == _ClienteContableID);
        }
        if (string.IsNullOrWhiteSpace(_ClienteContableNombre) == false)
            ClienteSeleccionadoNombre = _ClienteContableNombre;
    }
}
