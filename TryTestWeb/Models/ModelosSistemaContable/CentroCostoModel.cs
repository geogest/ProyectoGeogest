using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class CentroCostoModel
{
    public int CentroCostoModelID { get; set; }

    public int ClientesContablesModelID { get; set; }

    public int contador { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Nombre { get; set; }


    public CentroCostoModel()
    {

    }

    public decimal GetPresupuesto(string UserID, int idCentroCosto)
    {
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        CentroCostoPresupuestoModels lstCentroCostoPresupuesto = db.DBCentroCostoPresupuesto.SingleOrDefault(r => r.CentroCostoModelID == idCentroCosto);

        decimal Presupuesto = 0;
        if (lstCentroCostoPresupuesto != null)
        {
            Presupuesto = lstCentroCostoPresupuesto.Presupuesto;
        }

        return Presupuesto;
    }

}




