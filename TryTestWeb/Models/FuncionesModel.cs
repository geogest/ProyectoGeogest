using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


 public class FuncionesModel : ICloneable
{
    public int FuncionesModelID { get; set; }

    public string NombreFuncion { get; set; }
    public string NombreModulo { get; set; }

    public virtual ModuloSistemaModel ModuloSistema { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public static void CrearFuncionesBases()
    {
        FacturaContext dbCertificacion = new FacturaContext();
        FacturaProduccionContext dbProduccion = new FacturaProduccionContext();

        if (dbCertificacion.DBFunciones.Count() > 0 || dbProduccion.DBFunciones.Count() > 0)
            return;

        List<Tuple<string, string>> lstFunciones = new List<Tuple<string, string>>();

        //Inicio
        lstFunciones.Add(new Tuple<string, string>("homey", "Inicio")); //MANAGE/INDEX TURN INTO

        //Configuracion
        lstFunciones.Add(new Tuple<string, string>("DatosEmpresa","Configuracion"));
        lstFunciones.Add(new Tuple<string, string>("Register","Configuracion"));
        lstFunciones.Add(new Tuple<string, string>("Certificacion","Configuracion"));
        lstFunciones.Add(new Tuple<string, string>("Index","Inicio")); 
        lstFunciones.Add(new Tuple<string, string>("AgregarUsuario", "Configuracion"));

        //Clientes
        lstFunciones.Add(new Tuple<string, string>("ListarCliente", "Clientes"));
        lstFunciones.Add(new Tuple<string, string>("EditarCliente", "Clientes"));
        lstFunciones.Add(new Tuple<string, string>("NuevoCliente", "Clientes"));

        //FACTURAS //not complete
        lstFunciones.Add(new Tuple<string, string>("ListaFacturas", "Facturas"));
        lstFunciones.Add(new Tuple<string, string>("ListaNotaCredito", "Facturas"));
        lstFunciones.Add(new Tuple<string, string>("ListaNotaDebito", "Facturas"));

        lstFunciones.Add(new Tuple<string, string>("MakeFacturaExp", "Facturas"));
        lstFunciones.Add(new Tuple<string, string>("MakeFacturaNeoNote", "Facturas"));

        //COMPRAS
        lstFunciones.Add(new Tuple<string, string>("DTEsRecibidos", "Compras"));

        //GASTOS //not complete
        lstFunciones.Add(new Tuple<string, string>("ListaHonorarios", "Gastos"));
        lstFunciones.Add(new Tuple<string, string>("ListaEgresosFijos", "Gastos"));

        //PAGOS
        lstFunciones.Add(new Tuple<string, string>("ListaPagos", "Pagos"));

        //DECLARACIONES
        lstFunciones.Add(new Tuple<string, string>("ListaF29", "Declaraciones"));

        //Informes
        lstFunciones.Add(new Tuple<string, string>("InformeIngresos","Informes"));
        lstFunciones.Add(new Tuple<string, string>("InformeEgresos","Informes"));
        lstFunciones.Add(new Tuple<string, string>("InformeResultados", "Informes"));
        lstFunciones.Add(new Tuple<string, string>("ter","Informes"));

        //AMBIENTES
        lstFunciones.Add(new Tuple<string, string>("SwitchPlatformCert", "Ambientes"));
        lstFunciones.Add(new Tuple<string, string>("SwitchPlatformProd", "Ambientes"));

        foreach (Tuple<string, string> pairFuncion in lstFunciones)
        {
            FuncionesModel fnFuncionObjGenerico = new FuncionesModel();
            fnFuncionObjGenerico.NombreFuncion = pairFuncion.Item1;
            fnFuncionObjGenerico.NombreModulo = pairFuncion.Item2;
            dbCertificacion.DBFunciones.Add(fnFuncionObjGenerico);
            dbProduccion.DBFunciones.Add((FuncionesModel)fnFuncionObjGenerico.Clone());
        }

        dbCertificacion.SaveChanges();
        dbProduccion.SaveChanges();
    }
}
