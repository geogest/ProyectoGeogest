using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ModulosHabilitados
{
    public int ModulosHabilitadosID { get; set; }
    public int UsuarioModelID { get; set; }
    public int QuickEmisorModelID { get; set; }
    public Privilegios privilegiosAcceso { get; set; }
    public virtual FuncionesModel Funcion { get; set; }

    public string NombreCompania; //Interno (No se refleja en el modelo)

    public ModulosHabilitados()
    {

    }

    public ModulosHabilitados(int _UsuarioModelID, int _QuickEmisorModelID, FuncionesModel _Funcion)
    {
        this.UsuarioModelID = _UsuarioModelID;
        this.QuickEmisorModelID = _QuickEmisorModelID;
        this.privilegiosAcceso = Privilegios.Administrador;
        this.Funcion = _Funcion;
    }
}
