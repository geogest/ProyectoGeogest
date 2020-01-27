using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web;



public class BoletasHonorariosModel
{
    public static readonly string[] ValidAttachmentTypes = { ".jpg", ".jpeg", ".png", ".pdf" };

    public int BoletasHonorariosModelID { get; set; }
    public int QuickEmisorModelID { get; set; }

    public int NumeroBoleta { get; set; }

    //Cambiar a ENUM una vez que se sepa cuales son los posibles valores de este campo
    public EstadoHonorarios Estado { get; set; }

    public DateTime Fecha { get; set; }

    private string RUT { get; set; }
    public int RUT_num { get; set; }

    public string RazonSocial { get; set; }
    public bool SociedadProfesional { get; set; } = false;

    public decimal Brutos { get; set; }
    public decimal Retenido { get; set; }
    public decimal Liquido { get; set; }

    public OpcionRetencion ConSinRetencion { get; set; }

    public virtual ICollection<DTEPagosModel> HistorialPagos { get; set; }

    public string GetAttachmentData { get { return ParseExtensions.pGetAttachmentData(base64ArchivoAsociado, TipoArchivoAsociado, ValidAttachmentTypes); } }

    public decimal setMontoTotal { set
    {
            decimal passedValue = value;
            this.Brutos = Math.Round(passedValue, 0, MidpointRounding.AwayFromZero);
            this.Liquido = Math.Round((this.Brutos * 0.9m), 0, MidpointRounding.AwayFromZero);
            this.Retenido = this.Brutos - this.Liquido;
    }}

    public string RUT_txt
    {
        get { return this.RUT; }
        set
        {
            this.RUT = value;
            string tempValue = value;
            if (string.IsNullOrWhiteSpace(tempValue))
            {
                this.RUT_num = 0;
                return;
            }

            if (tempValue.Contains("."))
            {
                tempValue = tempValue.Replace(".", "");
            }
            if (value.Contains('-'))
            {
                int indexOfGuion = value.IndexOf('-');
                if (indexOfGuion > 0)
                {
                    tempValue = tempValue.Substring(0, indexOfGuion);
                }
            }
            int rutIntValue = ParseExtensions.ParseInt(tempValue);
            if (rutIntValue != 0)
            {
                this.RUT_num = rutIntValue;
            }
        }
    }

    public string base64ArchivoAsociado { get; set; }
    public string TipoArchivoAsociado { get; set; }

    public string SociedadProfesionalSTR()
    {
        if (this.SociedadProfesional == true)
            return "SI";
        else
            return "NO";
    }

    public string GlosaDescripcion { get; set; }
}

public enum OpcionRetencion
{
    [Display(Name = "Con Retencion")]
    Con_Retencion =1,
    [Display(Name = "Sin Retencion")]
    Sin_Retencion = 2
}

public enum EstadoHonorarios
{
    Vigente = 1,
    Anulado = 2
}


