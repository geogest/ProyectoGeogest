using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

/*
 * 
AUTOR       : ALEX  MARTINEZ
FECHA       : 19/12/2018
DESCRIPCION : SE GENERA MODULOS DE MANTENDORES DE CLIENTES Y PROVEEDORES
 *  
*/
public class ClientesProveedoresModel
{
    public int ClientesProveedoresModelID { get; set; }
    public int ClientesContablesModelID { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Campo Rut")]
    [StringLength(10)]
    [ValidaRut(ErrorMessage = "El Rut ingresado es Invalido.")]
    public string Rut { get; set; }
    public string Nombre { get; set; }

    /*
        1. Clientes
        2. Provedoores
     */
    public int Tipo { get; set; }

  
    public bool Estado { get; set;  }

    public ClientesProveedoresModel()
    {


    }
}
