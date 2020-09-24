using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace TryTestWeb.Controllers
{
    [Authorize]
    public class ContabilidadAuxiliaresController : Controller
    {

        public ActionResult EstadoCtasCorrientes(FiltrosEstadoCtasCorrientes Filtros)
        {
                string UserID = User.Identity.GetUserId();
                FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
                ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

                var lstCtasAux = UsoComunAux.LstAuxConMovimiento(db, objCliente);

                ViewBag.lstCtasCtes = lstCtasAux;
                
                IQueryable<EstadoCuentasCorrientesViewModel> QueryCtaCorriente = EstadoCuentasCorrientesViewModel.GetLstCtaCorriente(db, objCliente);
                IQueryable<EstadoCuentasCorrientesViewModel> LstCtaCorrienteBusqueda = EstadoCuentasCorrientesViewModel.FiltrosCtaCorriente(QueryCtaCorriente, Filtros);
                PaginadorModel LstCtasConPaginacion = EstadoCuentasCorrientesViewModel.PaginacionCtasCorrientes(LstCtaCorrienteBusqueda, Filtros);

                Session["EstadoCtaCorriente"] = LstCtasConPaginacion.LstCtasCorrientes;
                
                return View(LstCtasConPaginacion);
            }

          public ActionResult GetExcelEstadoCtaCorriente()
          {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["EstadoCtaCorriente"] != null)
            {
                List<EstadoCuentasCorrientesViewModel> LstCtasCorrientes = Session["EstadoCtaCorriente"] as List<EstadoCuentasCorrientesViewModel>;
                if (LstCtasCorrientes != null)
                { 
                    var cachedStream = EstadoCuentasCorrientesViewModel.GetExcelCtaCorriente(LstCtasCorrientes, objCliente, true);
                    return File(cachedStream, "application/vnd.ms-excel", "Estado Cuentas Corrientes" + Guid.NewGuid() + ".xlsx");
                }
            }
            return null;
        }

        public ActionResult EstCtasCtesConciliado(FiltrosEstadoCtasCorrientes Filtros)
        {
            //Condiciones Si elige un tipo de listado.
            //Si se quiere mostrar no conciliada "Eliminar las que esten conciliadas".
            //Si se quiere mostrar las conciliadas "Retornar solo las conciliadas".
            //Si se quiere mostrar todas entonces no pasar por este proceso.
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var lstCtasAux = UsoComunAux.LstAuxConMovimiento(db, objCliente);

            ViewBag.lstCtasCtes = lstCtasAux;
            ViewBag.ObjCliente = objCliente;

            List<ObjetoCtasCtesPorConciliar> ListaOrdenadaConAcumulados = new List<ObjetoCtasCtesPorConciliar>();
            List<EstCtasCtesConciliadasViewModel> ListaProcesada = new List<EstCtasCtesConciliadasViewModel>();
            List<ObjetoCtasCtesPorConciliar> ListaOrdenada = new List<ObjetoCtasCtesPorConciliar>();

            IQueryable<EstCtasCtesConciliadasViewModel> QueryCtaCorrienteTodosLosAnios = EstCtasCtesConciliadasViewModel.GetlstCtasCtesConciliadas(db, objCliente);
            IQueryable<EstCtasCtesConciliadasViewModel> ListaFiltrada = EstCtasCtesConciliadasViewModel.FiltrosCtaCorriente(QueryCtaCorrienteTodosLosAnios, Filtros);

            ListaProcesada = EstCtasCtesConciliadasViewModel.CalcularYConciliarLista(db, objCliente, ListaFiltrada, Filtros);
            ListaOrdenada = EstCtasCtesConciliadasViewModel.OrdenarListaCtasCtes(ListaProcesada);
            
            ListaOrdenadaConAcumulados = EstCtasCtesConciliadasViewModel.CalcularAcumulados(ListaOrdenada, QueryCtaCorrienteTodosLosAnios, db, objCliente, Filtros);

            decimal TotalAcumuladosGenerales = EstCtasCtesConciliadasViewModel.CalcularAcumuladosGenerales(ListaOrdenadaConAcumulados, QueryCtaCorrienteTodosLosAnios, Filtros);

            ViewBag.TotalSaldoAcumulado = TotalAcumuladosGenerales;

            Session["EstadoDeCuentasCorrientes"] = ListaOrdenadaConAcumulados;
            Session["TotalAcumEstadoCuentasCorrientes"] = TotalAcumuladosGenerales;
            Session["Filtros"] = Filtros; // Para manejar los filtros consultados para el reporte de excel.

            return View(ListaOrdenadaConAcumulados);
        }

        public ActionResult getExcelEstadoCuentasCorrientes()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["EstadoDeCuentasCorrientes"] != null && Session["TotalAcumEstadoCuentasCorrientes"] != null)
            {

                decimal SaldoAperturaGeneral = (decimal)Session["TotalAcumEstadoCuentasCorrientes"];
                List<ObjetoCtasCtesPorConciliar> LstCtasCorrientes = Session["EstadoDeCuentasCorrientes"] as List<ObjetoCtasCtesPorConciliar>;
                if (LstCtasCorrientes != null)
                {
                    var cachedStream = EstadoCuentasCorrientesViewModel.GetExcelCuentasCorrientes(LstCtasCorrientes,SaldoAperturaGeneral, objCliente, true);
                    return File(cachedStream, "application/vnd.ms-excel", "Estado Cuentas Corrientes" + Guid.NewGuid() + ".xlsx");
                }
            }
            return null;
        }
    }
}