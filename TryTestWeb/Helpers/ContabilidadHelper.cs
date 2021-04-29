using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ContabilidadHelper
{
    public static string VerificaRepetidosEnExcelImportSIICoV(List<string[]> CsvSII , ClientesContablesModel objCliente, FacturaPoliContext db)
    {
        TipoCentralizacion tipoCentralizacion = TipoCentralizacion.Ninguno;
        int folio = 0;
        TipoDte tipoDte = 0;
        var IdentificadorRepetidos = new List<string>();
        string ReturnValues = "";
        foreach (string[] strFilaCSV in CsvSII)
        {
            if (CsvSII.First() == strFilaCSV)
            {

                if (strFilaCSV[2] == "Tipo Compra")
                {
                    tipoCentralizacion = TipoCentralizacion.Compra;
                }
                else if (strFilaCSV[2] == "Tipo Venta")
                {
                    tipoCentralizacion = TipoCentralizacion.Venta;
                }
                else
                {
                    return null;
                }

                continue;
           }
            tipoDte = (TipoDte)ParseExtensions.ParseInt(strFilaCSV[1]);
            string RutPrestador = strFilaCSV[3];
            folio = Convert.ToInt32(strFilaCSV[5]);
            
            var SinRepetidos = db.DBLibrosContables.Where(x => x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                     x.Folio ==  folio &&
                                                                     x.TipoDocumento == tipoDte &&
                                                                     x.individuo.RUT == RutPrestador &&
                                                                     x.HaSidoConvertidoAVoucher == true &&
                                                                     x.TipoLibro == tipoCentralizacion)
                                                            .Select(x => new { x.Folio, x.VoucherModelID })
                                                            .Distinct()
                                                            .ToList();

            if (SinRepetidos != null)
            {
               
                foreach (var itemRepetido in SinRepetidos)
                {
                    var VoucherEncontrado = db.DBVoucher.Where(x => x.VoucherModelID == itemRepetido.VoucherModelID).Select(x => new { x.DadoDeBaja, x.Tipo }).FirstOrDefault();
                    if (VoucherEncontrado.DadoDeBaja == false && VoucherEncontrado.Tipo == TipoVoucher.Traspaso)
                    {
                        IdentificadorRepetidos.Add(itemRepetido.Folio.ToString());
                    }
                    
                }
            }
        }

        if(IdentificadorRepetidos.Count > 0)
            ReturnValues = string.Join(",", IdentificadorRepetidos);

        return ReturnValues;
    }
}
