using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class UserModels
{
    public string RazonSocial_Emisor;
    public List<string> Giro_Emisor;

    public string Dir_Origen;
    public string Cmna_Origen;
    public string Ciudad_Origen;

    public string EMail_Emisor;
    public string Fono_Emisor;

    public List<ACTECO> Emisor_Actecos;
}

public class ACTECO
{
    public string Nombre;
    public int Value;
}