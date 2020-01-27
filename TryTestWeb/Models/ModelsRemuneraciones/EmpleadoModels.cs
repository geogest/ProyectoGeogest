using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EmpleadoModel
{
    public int EmpleadoModelID { get; set; }

    public int QuickEmisorModelID { get; set; }

    public string nombre { get; set; }

    public string apellidos { get; set; }

    public string rut { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "FechaDeNacimiento")]
    [DataType(DataType.Text)]
    public DateTime FechaDeNacimiento { get; set; }

    public int telefono { get; set; }

    public string direccion { get; set; }

    public string correo { get; set; }

    public int idRegion { get; set; }

    public int idComuna { get; set; }


    public int GeneroID { get; set; }

    public int EstadoCivilID { get; set; }

    public string Nacionalidad { get; set; }

    public int CargoID { get; set; }

    public int HorasSem { get; set; }

    public int TipoContratoID { get; set; }

    public string EstadoContrato { get; set; }

    public int SucursalID { get; set; }

    public int SueldoLiquido { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "FechaDeIngreso")]
    [DataType(DataType.Text)]
    public DateTime FechaDeIngreso { get; set; }

    public string AjustSueldoBa { get; set; }

    public string BenefSemCorrida { get; set; }

    public int Sueldomes { get; set; }

    public int GratificacionID { get; set; }

    public string AsigZonaExtre { get; set; }

    public int AfpID { get; set; }

    public int TramoID { get; set; }

    public int CargasNormales { get; set; }

    public int CargasInvalidas { get; set; }

    public string SegCesantia { get; set; }

    public string AfecSeguroAccidente { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "FechaDeIngreso")]
    [DataType(DataType.Text)]
    public DateTime PeriodoCesantia { get; set; }

    public string Jubilado { get; set; }
    
    public int IsapreID { get; set; }

    public int TipoSueldoID { get; set; }

    public int TipoCuentaID { get; set; }
        
    public int NumCuenta { get; set; }

    public int BancoID { get; set; }

    public int ApvID { get; set; }

    public DateTime FechaDeRegistro { get; set; }

    public EmpleadoModel()
        {


        }

    


    }

 
  
