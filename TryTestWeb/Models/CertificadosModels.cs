using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


public class CertificadosModels
{
    
    public int CertificadosModelsID { get; set; }

    [Key, ForeignKey("QuickEmisorModel")]
    public int QuickEmisorModelID { get; set; }
    public virtual QuickEmisorModel QuickEmisorModel { get; set; }

    public string PathCAF33 { get; set; }
    public string PathCAF34 { get; set; }
    public string PathCAF61 { get; set; }
    public string PathCAF56 { get; set; }

    public string PathCAF110 { get; set; }
    public string PathCAF52 { get; set; }

    public string PathCAF39 { get; set; }
    public string PathCAF41 { get; set; }

    public string CertificatePath { get; set; }
    public string CertificatePassword { get; set; }

    public string PathCAF111 { get; set; }
    public string PathCAF112 { get; set; }

    public string PathCAF46 { get; set; }

}
