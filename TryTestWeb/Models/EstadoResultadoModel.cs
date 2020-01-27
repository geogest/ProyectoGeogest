using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class EstadoResultadoModel
    {

    public int EstadoResultadoModelID { get; set; }

    public int QuickEmisorModelID { get; set; }

    public DateTime Periodo { get; set; }

    //Ingresos
    public decimal IngresosVarios { get; set; } = 0;

    //Gastos
    public decimal BrutoRemuneraciones { get; set; } = 0;

    public decimal BrutoHonorarios { get; set; } = 0;

    public decimal GastosVarios { get; set; } = 0;

    public static EstadoResultadoModel GetFlujoCumulativoAnio(List<EstadoResultadoModel> lstFlujoCaja, int _QuickEmisorModelID)
    {
        EstadoResultadoModel returnObj = new EstadoResultadoModel();
        if (lstFlujoCaja == null || lstFlujoCaja.Count == 0)
            return returnObj;

        returnObj.IngresosVarios = lstFlujoCaja.Sum(r => r.IngresosVarios);
        returnObj.BrutoHonorarios = lstFlujoCaja.Sum(r => r.BrutoHonorarios);
        returnObj.BrutoRemuneraciones = lstFlujoCaja.Sum(r => r.BrutoRemuneraciones);
        returnObj.GastosVarios = lstFlujoCaja.Sum(r => r.GastosVarios);
        returnObj.QuickEmisorModelID = _QuickEmisorModelID;

        return returnObj;
    }

}
