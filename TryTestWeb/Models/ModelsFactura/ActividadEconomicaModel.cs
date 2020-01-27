using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;


public class ActividadEconomicaModel
{
    public int ActividadEconomicaModelID { get; set; }
    
    public string CodigoInterno { get; set; }
    
    public string NombreActividad { get; set; } 

    public ACTECO_AfectoIVA AfectoAIva { get; set; }
  
    public ACTECO_CategoriaTributaria CategoriaTributaria { get; set; } = ACTECO_CategoriaTributaria.C1;
    public bool DisponibleInternet { get; set; }

    public virtual ICollection<QuickEmisorModel> Emisores { get; set; }
  




    public ActividadEconomicaModel()
    {
    //    this.ClientesContables = new HashSet<ClientesContablesModel>();
    }

    public ActividadEconomicaModel(string _codInterno, string _nombreActividad, ACTECO_AfectoIVA _afectoIVA, ACTECO_CategoriaTributaria _C, bool _DisponibleInternet)
    {
        this.CodigoInterno = _codInterno;
        this.NombreActividad = _nombreActividad;
        this.AfectoAIva = _afectoIVA;
        this.CategoriaTributaria = _C;
        this.DisponibleInternet = _DisponibleInternet;

        this.Emisores = new HashSet<QuickEmisorModel>();
      //  this.ClientesContables = new HashSet<ClientesContablesModel>();
    }

    public static void SetupActividadEconomicas(List<FacturaPoliContext> lstDataContext)
    {
        foreach (FacturaPoliContext db in lstDataContext)
        {
            using (db)
            {
                if (db.DBActeco.Count() == 0)
                {
                    /*
                    db.DBActeco.Add(new ActividadEconomicaModel("722000", "ASESORES Y CONSULTORES EN INFORMÁTICA (SOFTWARE)", ACTECO_AfectoIVA.NO, ACTECO_CategoriaTributaria.C2, true));
                    db.DBActeco.Add(new ActividadEconomicaModel("724000", "PROCESAMIENTO DE DATOS Y ACTIVIDADES RELACIONADAS CON BASES DE DATOS", ACTECO_AfectoIVA.ND, ACTECO_CategoriaTributaria.C1, true));
                    db.DBActeco.Add(new ActividadEconomicaModel("726000", "EMPRESAS DE SERVICIOS INTEGRALES DE INFORMÁTICA", ACTECO_AfectoIVA.ND, ACTECO_CategoriaTributaria.C1, true));
                    */
                    List<ActividadEconomicaModel> lstActividadEconomicaBase = LeerArchivoACTECO();
                    db.DBActeco.AddRange(lstActividadEconomicaBase);
                    db.SaveChanges();
                }
            }     
        }
    }

    public static List<ActividadEconomicaModel> LeerArchivoACTECO()
    {
        string RutaFileActeco = ParseExtensions.Get_AppData_Path("ACTECOLIST.txt");

        List<ActividadEconomicaModel> lstReturnActectos = new List<ActividadEconomicaModel>();
        var lines = File.ReadLines(RutaFileActeco); //File.ReadLines(@"C:\FE\wkhtmltopdf\bin\ACTECOLIST.txt");
        foreach (var line in lines)
        {
            string[] datosActeco = Regex.Split(line, @"\//");
            string codInterino = datosActeco[0].Trim();
            string nombreInterino = datosActeco[1].Trim();

            ACTECO_AfectoIVA EsAfectoIVA;
            string AfectoIVAstr = datosActeco[2].Trim();
            if (AfectoIVAstr == "SI")
                EsAfectoIVA = ACTECO_AfectoIVA.SI;
            else if (AfectoIVAstr == "NO")
                EsAfectoIVA = ACTECO_AfectoIVA.NO;
            else
                EsAfectoIVA = ACTECO_AfectoIVA.ND;

            ACTECO_CategoriaTributaria CategoriaTrib;
            string CategoriaTributariaSTR = datosActeco[3].Trim();
            if (CategoriaTributariaSTR == "1")
                CategoriaTrib = ACTECO_CategoriaTributaria.C1;
            else if (CategoriaTributariaSTR == "2")
                CategoriaTrib = ACTECO_CategoriaTributaria.C2;
            else
                CategoriaTrib = ACTECO_CategoriaTributaria.ND;

            bool bl_disponibleInternet = true;
            string strDisponibleInternet = datosActeco[4].Trim();
            if (strDisponibleInternet == "SI")
                bl_disponibleInternet = true;
            else
                bl_disponibleInternet = false;

            ActividadEconomicaModel objActividad = new ActividadEconomicaModel(codInterino, nombreInterino, EsAfectoIVA, CategoriaTrib, bl_disponibleInternet);
            lstReturnActectos.Add(objActividad);
        }

        return lstReturnActectos;
    }
}

public enum ACTECO_CategoriaTributaria
{
    ND = 0,
    C1 = 1,
    C2 = 2
}

public enum ACTECO_AfectoIVA
{
    ND = 0,
    SI = 1,
    NO = 2
}