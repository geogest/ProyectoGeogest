using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TryTestWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LocatorInitializationHandler.Initialize();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FuncionesModel.CrearFuncionesBases();

            FacturaPoliContext dbCertificacion = new FacturaPoliContext();
            FacturaPoliContext dbProduccion = new FacturaPoliContext(true);//new FacturaProduccionContext();
            List<FacturaPoliContext> lstDataContext = new List<FacturaPoliContext> { dbCertificacion, dbProduccion };
            ActividadEconomicaModel.SetupActividadEconomicas(lstDataContext);
        }

        public static Dictionary<String, String[]> RegistroEmpresasPalena = File.ReadLines(@"C:\FE\wkhtmltopdf\bin\EMPRESAS_PALENA.csv").Select(line => line.Split(';')).ToDictionary(
                    items => items[0],
                    items => items
                );

        protected void Application_Error(object sender, EventArgs e)
        {
            bool isLocal = Request.IsLocal;
            if (isLocal == false)
            {
                Exception exception = Server.GetLastError();
                Response.Clear();

                Response.TrySkipIisCustomErrors = true;

                HttpException httpException = exception as HttpException;

                int error = httpException != null ? httpException.GetHttpCode() : 0;

                Server.ClearError();
                Response.Redirect(String.Format("~/Error/Error?error={0}", error, exception.Message));
            }
        }
    }


}
