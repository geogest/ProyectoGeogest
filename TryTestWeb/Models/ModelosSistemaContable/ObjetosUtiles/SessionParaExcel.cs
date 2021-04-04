using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class SessionParaExcel
    {
        public string Anio { get; set; }
        public string Mes { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string CtaContId { get; set; }
        
        public static SessionParaExcel ObtenerObjetoExcel(string anio, string mes, string fechaInicio, string fechaFin, string CtaContId)
        {
            var ObjExcel = new SessionParaExcel() 
            {
                Anio = anio,
                Mes = mes,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                CtaContId = CtaContId
            };

            return ObjExcel;
        }

        public static bool TieneFiltrosActivos(SessionParaExcel FiltrosExcel) {
            bool Result = false;
            if (FiltrosExcel.Anio != "0" ||
                FiltrosExcel.Mes != "0" ||
                !string.IsNullOrWhiteSpace(FiltrosExcel.FechaInicio) &&
                !string.IsNullOrWhiteSpace(FiltrosExcel.FechaFin))
                Result = true;

            return Result;
        }

    }
