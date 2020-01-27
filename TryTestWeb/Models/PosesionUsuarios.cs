using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

public class PosesionUsuarios
{
    //Primary KEY
    public int PosesionUsuariosID { get; set; }

    //Quien es el en usuarios
    public int UsuarioModelID { get; set; }

    //Y de quien es dueño
    public int UsuarioPoseidoID { get; set; }

    public PosesionUsuarios()
    {

    }
}
