using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


public class EmisoresHabilitados
{
    public int EmisoresHabilitadosID { get; set; }
    public int UsuarioModelID { get; set; }
    public int QuickEmisorModelID { get; set; }

    public Privilegios privilegiosAcceso { get; set; }
    //public virtual FuncionesModel Funcion { get; set; }

    public string NombreCompania; //Interno (No se refleja en el modelo)



    public EmisoresHabilitados()
    {

    }
}

public enum Privilegios
{
    [Display(Name = "Administrador")]
    Administrador = 0,
    [Display(Name = "Facturador")]
    Facturador = 1,
    [Display(Name = "Informador")]
    Informador = 2
}