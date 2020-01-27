using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


public class QuickEmisorModel
{
    public int QuickEmisorModelID { get; set; }

    public string IdentityIDEmisor { get; set; }

    [Required (ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Rut Empresa")]
    [StringLength(10)]
    [ValidaRut(ErrorMessage ="El Rut ingresado es Invalido.")]
    public string RUTEmpresa { get; set; }

    [Required (ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Rut Usuario")]
    [StringLength(10)]
    [ValidaRut(ErrorMessage = "El Rut ingresado es Invalido.")]
    public string RUTUsuario { get; set; }

    
    [Display(Name = "Rut Representante Legal")]
    [StringLength(10)]
    [ValidaRut(ErrorMessage = "El Rut ingresado es Invalido.")]
    public string RUTRepresentante { get; set; }

    
    [Display(Name = "Representante Legal")]
    [StringLength(30)]
    public string Representante { get; set; }

    [Required (ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Razón Social")]
    [StringLength(100)]
    public string RazonSocial { get; set; }


    public string Direccion { get; set; }

    public string Comuna { get; set; }

    public string Ciudad { get; set; }

    public string EMail { get; set; }

    
    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Giro")]
    [StringLength(80)]
    public string Giro { get; set; }




    //[Required (ErrorMessage = "Este campo es obligatorio.")]
    //[Display(Name = "Actividad Económica")]
    //[StringLength(6)]
    //public string ActEcono { get; set; }


    
    public virtual ICollection<ActividadEconomicaModel> lstActividadEconomica { get; set; }



    public string Telefono { get; set; }

    public int idRegion { get; set; }

    public int idComuna { get; set; }

    public virtual CertificadosModels Certificados { get; set; }
    public int CertificadosIDKey { get; set; }

    [Required (ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Fecha Resolución")]
    public DateTime FechaResolucion { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Número Resolución")]  
    public int NumeroResolucion { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Código Sucursal SII")]
    public int CodigoSucursalSII { get; set; }

    public virtual ICollection<QuickReceptorModel> collectionReceptores { get; set; }

    public virtual ICollection<BoletasHonorariosModel> collectionBoletasHonorarios { get; set; }

    public int maxUsuariosParaEstaEmpresa { get; set; } = 1;

    public int maxClientesContablesParaEstaEmpresa { get; set; } = 0;

    //public int DatabaseContextToUse { get; set; } = 0;

    //public virtual ICollection<ItemContableModel> collectionITEMCtaContable { get; set; }

    public virtual ICollection<ReportesImpagosLog> ReportesImpagosGenerados { get; set; }

    //Campos sistema de contabilidad
    public virtual ICollection<ClientesContablesModel> lstClientesCompania { get; set; }

    public QuickEmisorModel()
    {
        this.lstActividadEconomica = new HashSet<ActividadEconomicaModel>();
    }
    public QuickEmisorModel(string _IdentityID)
    {
        this.IdentityIDEmisor = _IdentityID;
        this.lstActividadEconomica = new HashSet<ActividadEconomicaModel>();
    }

    public QuickEmisorModel(string _RUT, string _RazonSocial, string _Direccion, string _Comuna, string _Ciudad, string _EMail,
        string _Giro, /*string _ActEcono,*/ string _Telefono, /*string dia, string mes, string año,*/ DateTime _fechaResolucion, int _numeroResolucion, int _codigoSucursalSII)
    {
        //this.TipoFactura = _tipo;
        this.RUTEmpresa = _RUT;
        this.RazonSocial = _RazonSocial;
        this.Direccion = _Direccion;
        this.Comuna = _Comuna;
        this.Ciudad = _Ciudad;
        this.EMail = _EMail;

        //this.Giros.Add(new GirosModel(_Giro));
        this.Giro = _Giro;

        //this.ActEcono = _ActEcono;


        this.Telefono = _Telefono;
        //this.FechaEmision = new DateTime(int.Parse(año), int.Parse(mes), int.Parse(dia));

        this.FechaResolucion = _fechaResolucion;
        this.NumeroResolucion = _numeroResolucion;
        this.CodigoSucursalSII = _codigoSucursalSII;

        this.CertificadosIDKey = -1;

        this.lstActividadEconomica = new HashSet<ActividadEconomicaModel>();

    }
}
public class ValidaRut : ValidationAttribute
{

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }
        string rut = value.ToString();
        if(string.IsNullOrEmpty(rut))
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        bool EsCorrecto = ParseExtensions.ValidaRut(rut);
        if (EsCorrecto == false)
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        else
            return null;
    }


}






