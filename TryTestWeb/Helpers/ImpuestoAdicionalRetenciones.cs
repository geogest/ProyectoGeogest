using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class ImpuestoAdicionalRetencionesModel
{
    /*
     a) Tasa* (Suma de líneas de detalle con código de Impuesto adicional o retención), excepto
        Diesel, Gasolina, margen de comercialización e “Iva anticipado faenamiento carne”
     b) Tasa * Monto base faenamiento para Iva anticipado faenamiento carne
     c) Valor numérico en otros casos > 0
     */
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    public string DescripcionImpuesto { get; set; }
    public decimal TasaImpuesto { get; set; }
    public double CodImpuesto { get; set; }

    public static List<SelectListItem> GetDropDownListHTML()
    {
        List<SelectListItem> listItems = new List<SelectListItem>();
        var tax23 = TaxOroJoyas();
        var tax44 = TaxTapicesCaviarAirsoft();
        var tax24 = TaxDestilados();
        var tax25 = TaxVinoChichaCidra();
        var tax26 = TaxCervezaOtherAlcohol();
        var tax27 = TaxMineralWaterAnalcoholicDrink();
        var tax271 = TaxHighSugarDrink();
        var tax15 = IvaRetenidoTotal();

        listItems.Add(new SelectListItem { Text = "Ninguno", Value = "[0,0]", Selected = true });
        listItems.Add(new SelectListItem { Text = tax23.DescripcionImpuesto, Value = "[" + tax23.CodImpuesto.ToString() + "," + tax23.TasaImpuesto.ToString() + "]" });
        listItems.Add(new SelectListItem { Text = tax44.DescripcionImpuesto, Value = "[" + tax44.CodImpuesto.ToString() + "," + tax44.TasaImpuesto.ToString() + "]" });
        listItems.Add(new SelectListItem { Text = tax24.DescripcionImpuesto, Value = "[" + tax24.CodImpuesto.ToString() + "," + tax24.TasaImpuesto.ToString() + "]" });
        listItems.Add(new SelectListItem { Text = tax25.DescripcionImpuesto, Value = "[" + tax25.CodImpuesto.ToString() + "," + tax25.TasaImpuesto.ToString() + "]" });
        listItems.Add(new SelectListItem { Text = tax26.DescripcionImpuesto, Value = "[" + tax26.CodImpuesto.ToString() + "," + tax26.TasaImpuesto.ToString() + "]" });
        listItems.Add(new SelectListItem { Text = tax27.DescripcionImpuesto, Value = "[" + tax27.CodImpuesto.ToString() + "," + tax27.TasaImpuesto.ToString() + "]" });
        listItems.Add(new SelectListItem { Text = tax271.DescripcionImpuesto, Value = "["+tax271.CodImpuesto.ToString() +","+ tax271.TasaImpuesto.ToString()+"]" });
        listItems.Add(new SelectListItem { Text = tax15.DescripcionImpuesto, Value = "[" + tax15.CodImpuesto.ToString() + "," + tax15.TasaImpuesto.ToString() + "]" });
        return listItems;
    }


    public static ImpuestoAdicionalRetencionesModel GetTaxByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return null;
        if (code.Contains(','))
        {
            code = code.Replace("[", string.Empty);
            code = code.Replace("]", string.Empty);
            int L = code.IndexOf(",");
            if (L > 0)
            {
                code = code.Substring(0, L);
            }
        }

        if (code == "23")
            return TaxOroJoyas();
        else if (code == "44")
            return TaxTapicesCaviarAirsoft();
        else if (code == "24")
            return TaxDestilados();
        else if (code == "25")
            return TaxVinoChichaCidra();
        else if (code == "26")
            return TaxCervezaOtherAlcohol();
        else if (code == "27")
            return TaxMineralWaterAnalcoholicDrink();
        else if (code == "271")
            return TaxHighSugarDrink();
        else if (code == "15")
            return IvaRetenidoTotal();
        else
            return null;
    }

    //public static ImpuestoAdicionalRetencionesModel GetTaxByCodeSimpler(string code)

    public static ImpuestoAdicionalRetencionesModel TaxOroJoyas()
    {
        ImpuestoAdicionalRetencionesModel objTax = new ImpuestoAdicionalRetencionesModel();
        objTax.DescripcionImpuesto = "Art. de Oro, Joyas y Pieles Finas (15%)";
        objTax.TasaImpuesto = 15;
        objTax.CodImpuesto = 23;
        return objTax;
    }

    public static ImpuestoAdicionalRetencionesModel TaxTapicesCaviarAirsoft()
    {
        ImpuestoAdicionalRetencionesModel objTax = new ImpuestoAdicionalRetencionesModel();
        objTax.DescripcionImpuesto = "Tapices, Casas Rodantes, Caviar y Armas de Aire (15%)";
        objTax.TasaImpuesto = 15;
        objTax.CodImpuesto = 44;
        return objTax;
    }

    public static ImpuestoAdicionalRetencionesModel TaxDestilados()
    {
        ImpuestoAdicionalRetencionesModel objTax = new ImpuestoAdicionalRetencionesModel();
        objTax.DescripcionImpuesto = "Licores, Pisco, Destilados (31.5%)";
        objTax.TasaImpuesto = 31.5m;
        objTax.CodImpuesto = 24;
        return objTax;
    }

    public static ImpuestoAdicionalRetencionesModel TaxVinoChichaCidra()
    {
        ImpuestoAdicionalRetencionesModel objTax = new ImpuestoAdicionalRetencionesModel();
        objTax.DescripcionImpuesto = "Vinos, Chichas, Sidras (20.5%)";
        objTax.TasaImpuesto = 20.5m;
        objTax.CodImpuesto = 25;
        return objTax;
    }

    public static ImpuestoAdicionalRetencionesModel TaxCervezaOtherAlcohol()
    {
        ImpuestoAdicionalRetencionesModel objTax = new ImpuestoAdicionalRetencionesModel();
        objTax.DescripcionImpuesto = "Cervezas y Otras Bebidas Alcoholicas (20.5%)";
        objTax.TasaImpuesto = 20.5m;
        objTax.CodImpuesto = 26;
        return objTax;
    }

    public static ImpuestoAdicionalRetencionesModel TaxMineralWaterAnalcoholicDrink()
    {
        ImpuestoAdicionalRetencionesModel objTax = new ImpuestoAdicionalRetencionesModel();
        objTax.DescripcionImpuesto = "Aguas Minerales y Bebidas Analcoholicas (10%)";
        objTax.TasaImpuesto = 10;
        objTax.CodImpuesto = 27;
        return objTax;
    }

    public static ImpuestoAdicionalRetencionesModel TaxHighSugarDrink()
    {
        ImpuestoAdicionalRetencionesModel objTax = new ImpuestoAdicionalRetencionesModel();
        objTax.DescripcionImpuesto = "Bebidas Analcoholicas con alto contenido de Azucar (18%)";
        objTax.TasaImpuesto = 18;
        objTax.CodImpuesto = 271;
        return objTax;
    }

    public static ImpuestoAdicionalRetencionesModel IvaRetenidoTotal()
    {
        ImpuestoAdicionalRetencionesModel objTax = new ImpuestoAdicionalRetencionesModel();
        objTax.DescripcionImpuesto = "IVA retenido total";
        objTax.TasaImpuesto = 19.0m;
        objTax.CodImpuesto = 15;
        return objTax;
    }

    

    

        /*
    public static List<ImpuestoAdicionalRetencionesModel> GetMontosImpuestoAdicionalXML(ICollection<QuickDetalleModel> Detalle, TipoDte tipoDTE)
    {
        var lstCodTaxes = Detalle.Select(c => c.ImpuestoAdicionalProducto).OfType<ImpuestoAdicionalRetencionesModel>().GroupBy(o => o.CodImpuesto).Select(o => o.FirstOrDefault());
    } */
}

public class TaxLibroResumenHelper
{
    public string CodTax;
    public int MontoTax;

    public TaxLibroResumenHelper(double code, decimal monto)
    {
        ImpuestoAdicionalRetencionesModel objImpuestos = ImpuestoAdicionalRetencionesModel.GetTaxByCode(code.ToString());
        if (objImpuestos == null)
            return;

        //double outVar = 0;
        //Double.TryParse(monto, out outVar);

        CodTax = objImpuestos.CodImpuesto.ToString();
        MontoTax = (int)Math.Round ( monto * (objImpuestos.TasaImpuesto/100), 0, MidpointRounding.AwayFromZero);

        //(int)Math.Round(MontoNetoSTEP * (Element.ImpuestoAdicionalProducto.TasaImpuesto / 100), 0);
    }
}