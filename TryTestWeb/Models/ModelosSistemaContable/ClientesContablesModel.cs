using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;



public class ClientesContablesModel
{
    public int ClientesContablesModelID { get; set; }

    public int QuickEmisorModelID { get; set; }
    //public int CompaniaModelID { get; set; }
    //public string IdentityID { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Rut Empresa")]
    [StringLength(10)]
    [ValidaRut(ErrorMessage = "El Rut ingresado es Invalido.")]
    public string RUTEmpresa { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Razón Social")]
    public string RazonSocial { get; set; }

    public string Direccion { get; set; }

    public virtual ComunaModels Comuna { get; set; }

    public string Ciudad { get; set; }


    public string ActEcono { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Telefono")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Giro")]
    public string Giro { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Rut Representante")]
    [StringLength(10)]
    [ValidaRut(ErrorMessage = "El Rut ingresado es Invalido.")]

    public string RUTRepresentante { get; set; }

    [Display(Name = "Nombre Representante")]
    public string Representante { get; set; }

    // [Required(ErrorMessage = "Este campo es obligatorio.")]
    // [Display(Name = "E-Mail Contacto")]
    public string Email { get; set; }



    // Agrego Actividad
    //public virtual ICollection<ActividadEconomicaModel> lstActividadEconomica { get; set; }


    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Región Contacto")]
    public int idRegion { get; set; }
    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Comuna Contacto")]
    public int idComuna { get; set; }






    public virtual ICollection<CuentaContableModel> CtaContable { get; set; }

    public virtual ICollection<VoucherModel> ListVoucher { get; set; }

    public virtual ICollection<CentroCostoModel> ListCentroDeCostos { get; set; }

    public virtual ICollection<ItemModel> ListItems { get; set; }


    public virtual ICollection<LibrosContablesModel> ListLibros { get; set; }

    public virtual ParametrosClienteModel ParametrosCliente { get; set; }

    public ClientesContablesModel()
    {


    }
}


public class ClientesContablesEmisorModel
{

    public int ClientesContablesEmisorModelID { get; set; }
    public int ClientesContablesModelID { get; set; }
    public int QuickReceptorModelID { get; set; }


}
