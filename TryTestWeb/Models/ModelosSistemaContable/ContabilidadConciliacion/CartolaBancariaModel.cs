using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CartolaBancariaModel
{
    public int CartolaBancariaModelId { get; set; }
    public DateTime Fecha { get; set; }
    public virtual ClientesContablesModel ClientesContablesModelID { get; set; }
    public virtual CuentaContableModel CuentaContableModelID { get; set; }
    public virtual CartolaBancariaMacroModel CartolaBancariaMacroModelID { get; set; }
    public int Folio { get; set; }
    public string Detalle { get; set; }
    public string Oficina { get; set; }
    public decimal Debe { get; set; }
    public decimal Haber { get; set; }
    public decimal Saldo { get; set; }
    public bool EstaConciliado { get; set; } = false;

    public static CartolaBancariaMacroModel getListaCartolaBancaria(List<string[]> Cartola, string FechaCartola, string NombreCartola, ClientesContablesModel ObjCliente)
    {
        CartolaBancariaMacroModel CartolaBancariaMacro = new CartolaBancariaMacroModel();

        List<CartolaBancariaModel> LstCartola = new List<CartolaBancariaModel>();

        if (Cartola.Count() > 1)
        {
            Cartola.RemoveAt(0);
            foreach (string[] FilaCartola in Cartola)
            {
                if(FilaCartola.All(x => string.IsNullOrWhiteSpace(x)))
                {
                    continue;
                }
                else
                {
                    CartolaBancariaModel ObjetoCartola = new CartolaBancariaModel();
                    ObjetoCartola.Fecha = ParseExtensions.ToDD_MM_AAAA_Multi(FilaCartola[0]);
                    ObjetoCartola.Folio = ParseExtensions.ParseInt(FilaCartola[1]);
                    ObjetoCartola.Detalle = FilaCartola[2];
                    ObjetoCartola.ClientesContablesModelID = ObjCliente;
                    ObjetoCartola.Debe = ParseExtensions.ParseDecimal(FilaCartola[3]);
                    ObjetoCartola.Haber = ParseExtensions.ParseDecimal(FilaCartola[4]);
                    ObjetoCartola.Saldo = ParseExtensions.ParseDecimal(FilaCartola[5]);

                    LstCartola.Add(ObjetoCartola);
                }

            }

            if(LstCartola.Count() > 0)
            {
                CartolaBancariaMacro.FechaCartola = ParseExtensions.ToDD_MM_AAAA_Multi(FechaCartola);
                CartolaBancariaMacro.ClientesContablesModelID = ObjCliente;
                CartolaBancariaMacro.NombreCartola = NombreCartola;
                CartolaBancariaMacro.CartolaDetalle = LstCartola;
            }
        }
        return CartolaBancariaMacro;
    }



    public static bool CrearVoucherDesdeCartola(List<string[]> Cartola)
    {
        bool Result = false;
        //Posiciones 6,7,8 tienen los datos del voucher a crear

        return Result;
    }

    public static List<LibroMayorConciliacion> getListaLibroMayor(List<string[]> MayorDeLaCuenta)
    {
        List<LibroMayorConciliacion> LstLibroMayor = new List<LibroMayorConciliacion>();

        if (MayorDeLaCuenta.Count() > 0) {
            foreach (string[] itemMayor in MayorDeLaCuenta)
            {
                LibroMayorConciliacion objMayor = new LibroMayorConciliacion();

                objMayor.FechaContabilizacion = ParseExtensions.ToDD_MM_AAAA2(itemMayor[1]);
                objMayor.Comprobante = itemMayor[2];
                objMayor.Glosa = itemMayor[3];
                objMayor.RazonSocial = itemMayor[4];
                objMayor.Rut = itemMayor[5];
                if (itemMayor[6].Contains("."))
                {
                    itemMayor[6] = itemMayor[6].Replace(".", "");
                }
                itemMayor[7] = itemMayor[7].Replace(".", "");
                itemMayor[8] = itemMayor[8].Replace(".", "");
                objMayor.Debe = Convert.ToDecimal(itemMayor[6]);
                objMayor.Haber = Convert.ToDecimal(itemMayor[7]);
                objMayor.Saldo = Convert.ToDecimal(itemMayor[8]);
                objMayor.NombreCuentaContable = itemMayor[9];
              
                LstLibroMayor.Add(objMayor);
            }
        }

        return LstLibroMayor;
    }

    public static ComparacionConciliacionBancariaViewModel ConciliarSiSePuede(ComparacionConciliacionBancariaViewModel DatosConciliacion)
    {
        foreach (var itemCartola in DatosConciliacion.lstCartola)
        {
            var Coincidentes = DatosConciliacion.lstLibroMayor.Where(x => x.NumDocAsignado == itemCartola.Folio).ToList();

            if (Coincidentes.Count() > 0)
            {
                foreach (var itemMayorCoincidente in Coincidentes)
                {
                    if (itemMayorCoincidente.Haber > 0 && itemCartola.Debe > 0)
                    {
                        //Intenta Conciliar
                        decimal Resultado = Math.Abs(itemMayorCoincidente.Haber) - Math.Abs(itemCartola.Debe);
                        if (Resultado == 0)
                        {
                            itemMayorCoincidente.EstaConciliado = true;
                        }

                    }
                    else if (itemMayorCoincidente.Debe > 0 && itemCartola.Haber > 0)
                    {
                        //Intenta Conciliar
                        decimal Resultado = Math.Abs(itemCartola.Haber) - Math.Abs(itemMayorCoincidente.Debe);
                        if (Resultado == 0)
                        {
                            itemMayorCoincidente.EstaConciliado = true;
                        }
                    }
                }
            }
        }
        return DatosConciliacion;
    }
}

public class ComparacionConciliacionBancariaViewModel
{
    public List<LibroMayorConciliacion> lstLibroMayor { get; set; }
    public List<CartolaBancariaModel> lstCartola { get; set; }
    public List<CartolaBancariaModel> LstNoContabilizados { get; set; }
    public List<LibroMayorConciliacion> LstNoRegistradosCartola { get; set; }
}
