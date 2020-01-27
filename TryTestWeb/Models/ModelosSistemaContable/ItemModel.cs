using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*
    AUTOR: ALEX MARTINEZ   
    FECHA: 13/12/2018  
    DESCRIPCION: SE CREA MODELO DE ITEMS PARA ASOCIAR LISTADO DE ITEMS A CLIENTE CONTABLES  CON CUENTAS CONTABLES.
     
 */


public class ItemModel
{
    
    public int ItemModelID { get; set; }
    public int ClienteContableID { get; set; }
    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string NombreItem { get; set; }
    public bool Estado { get; set; }
    public int contador { get; set; }
    public ItemModel()
    {

    }
}
