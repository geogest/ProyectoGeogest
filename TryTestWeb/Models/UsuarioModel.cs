using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

public class UsuarioModel
{
    public static readonly string UsuarioDemoNombreBlock = "demo@e-factu.cl";

    public int UsuarioModelID { get; set; }

    public string IdentityID { get; set; }

    public string RUT { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }

    public bool SuperAdminUser { get; set; }

    public int DatabaseContextToUse { get; set; } = 0;
    public int PerfilUsuarioModelID { get; set; }
    public int HeredaDeUsuario { get; set; }

    public virtual ICollection<EmisoresHabilitados> lstEmisoresAccesibles {get; set;}
    
    public virtual ICollection<PosesionUsuarios> lstUsuariosPoseidos { get; set; }


    public virtual ICollection<ClientesContablesModel> ClientesContablesAutorizados { get; set; }
    public bool DEMO_BLOCK { get { return Block_UsuarioDEMO(); } }

    public UsuarioModel()
    {

    }

    private bool Block_UsuarioDEMO()
    {
        if (Nombre == UsuarioDemoNombreBlock || Email == UsuarioDemoNombreBlock)
            return true;
        else
            return false;
    }

}
