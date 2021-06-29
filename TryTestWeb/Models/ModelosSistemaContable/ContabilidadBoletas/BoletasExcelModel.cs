using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace TryTestWeb.Models.ModelosSistemaContable.ContabilidadBoletas
{
    public class BoletasExcelModel
    {
        public string CuentaAuxiliar { get; set; }
        public string Rut { get; set; }
        public string RazonSocial { get; set; }
        public DateTime Fecha { get; set; }
        public int NumeroDeDocumento { get; set; }
        public TipoDte TipoDocumento { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string CuentaContable { get; set; } //Compra o venta
        public decimal Neto { get; set; }
        public decimal Iva { get; set; }
        public int CentroDeCostos { get; set; }
        public int MesPeriodoTributario { get; set; }
        public int AnioPeriodoTributario { get; set; }
        public int NumeroFinalBoleta { get; set; }

        public static List<BoletasExcelModel> DeExcelAObjetoBoleta(HttpPostedFileBase File)
        {
            var ReturnValues = new List<BoletasExcelModel>();
            SLDocument Excel = new SLDocument(File.InputStream);
            int row = 2;
            while (!string.IsNullOrEmpty(Excel.GetCellValueAsString(row, 1)))
            {
                string CuentaAuxiliar = Excel.GetCellValueAsString(row,1);
                string Rut = Excel.GetCellValueAsString(row, 2);
                string RazonSocial = Excel.GetCellValueAsString(row, 3);
                DateTime Fecha = Excel.GetCellValueAsDateTime(row,4);
                int NumeroDeDocumento = Excel.GetCellValueAsInt32(row,5);
                TipoDte TipoDocumento = (TipoDte)Excel.GetCellValueAsInt32(row, 6);
                DateTime FechaVencimiento = Excel.GetCellValueAsDateTime(row, 7);
                string CuentaContable = Excel.GetCellValueAsString(row, 8);
                decimal Neto = Excel.GetCellValueAsDecimal(row, 9);
                decimal Iva = Excel.GetCellValueAsDecimal(row, 10);
                int CentroDeCostos = Excel.GetCellValueAsInt32(row, 11);
                int MesPeriodoTributario = Excel.GetCellValueAsInt32(row, 12);
                int AnioPeriodoTributario = Excel.GetCellValueAsInt32(row, 13);
                int NumeroFinalBoleta = Excel.GetCellValueAsInt32(row, 14);

                BoletasExcelModel ObjBoleta = new BoletasExcelModel()
                {
                    CuentaAuxiliar = CuentaAuxiliar,
                    Rut = Rut,
                    RazonSocial = RazonSocial,
                    Fecha = Fecha,
                    NumeroDeDocumento = NumeroDeDocumento,
                    TipoDocumento = TipoDocumento,
                    FechaVencimiento = FechaVencimiento,
                    CuentaContable = CuentaContable,
                    Neto = Neto,
                    Iva = Iva,
                    CentroDeCostos = CentroDeCostos,
                    MesPeriodoTributario = MesPeriodoTributario,
                    AnioPeriodoTributario = AnioPeriodoTributario,
                    NumeroFinalBoleta = NumeroFinalBoleta
                };

                ReturnValues.Add(ObjBoleta);

            }

            return ReturnValues;
        }
    }
}