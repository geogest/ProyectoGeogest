using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;


public class ActividadEconomicaModelCuentaContableModel
{
    public int ActividadEconomicaModelCuentaContableModelID { get; set; }
    //  public int ActividadEconomicaModelID { get; set; }

    public string CodigoInterno { get; set; }
    public int ClienteContableModelID { get; set; }


    public ActividadEconomicaModelCuentaContableModel()
    {

    }




}
