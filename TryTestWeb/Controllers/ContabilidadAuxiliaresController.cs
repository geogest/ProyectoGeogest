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

            //Cambiar el estado en la base de datos a conciliado si el movimiento lo está al momento de calcularlo
            //En el futuro utilizar este modulo para refrescar y calcular los conciliados ¿Con qué sentido? -> 
            //R: al hacer esto por detrás y no renderizar una vista se habrán establecido los conciliados -> no conciliados y la lista completa que incluye a los 2
            //Entonces al crear la query simplemente irá a buscar los que estén conciliados y no tendrá que hacer todo el calculo nuevamente lo que permite una mejora
            //Tremenda en el tiempo de carga de estas listas.

            //Entonces paso 1 -> Según el id del movimiento si está conciliado etiquetarlo como tal.
            //Paso 2 Crear las querys correspondientes para obtener esta lista conciliada
            //Paso 3 Crear el modulo de los pendientes que solo tendrá aquellos que no estén conciliados.
            //Paso 4 Crear la manera de pagar estos documentos basandose en como se hace la conciliación bancaria.

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
        public ActionResult PendientesAuxConfiguracion()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var LstAuxConMovimiento = UsoComunAux.LstAuxConMovimientoTwo(db, objCliente);

            ViewBag.LstCtasAux = LstAuxConMovimiento;

            return View();
        }

        

 

        public ActionResult PendientesAuxiliares()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            //Queda pendiente programar los filtros y la forma de pagar.
            var LstAuxConMovimiento = UsoComunAux.LstAuxConMovimientoTwo(db, objCliente);
            ViewBag.LstCtasAux = LstAuxConMovimiento;

            //Replantearse el uso del detalle o si hay que hacer un objeto para mostrar solo rut y saldo
            //Rut y saldo... Ocupa.. ¿AuxiliarModel?

            //Esta es la posible query de los pendientes a nivel Macro
            var PendientesAux = (from Detalle in db.DBDetalleVoucher
                                 join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                 join Auxiliar in db.DBAuxiliares on Detalle.DetalleVoucherModelID equals Auxiliar.DetalleVoucherModelID
                                 join AuxiliarDetalle in db.DBAuxiliaresDetalle on Auxiliar.AuxiliaresModelID equals AuxiliarDetalle.AuxiliaresModelID

                                 where Voucher.DadoDeBaja == false &&
                                 Voucher.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                 Detalle.ObjCuentaContable.TieneAuxiliar == 1 &&
                                 Detalle.ConciliadoCtasCtes == false && Detalle.FechaDoc.Year == 2020

                                 select new 
                                 {
                                     Id = Auxiliar.AuxiliaresModelID,
                                     Rut = AuxiliarDetalle.Individuo2.RUT,
                                     RazonSocial = AuxiliarDetalle.Individuo2.RazonSocial,
                                     Debe = Detalle.MontoDebe,
                                     Haber = Detalle.MontoHaber
                                 });

            var PendientesAuxOrder = PendientesAux.GroupBy(x => new { x.Rut, x.RazonSocial })
                                                  .Select(y => new AuxPendientesViewModel 
                                                  {
                                                      Rut = y.Key.Rut,
                                                      RazonSocial = y.Key.RazonSocial,
                                                      Saldo = y.Sum(z => Math.Abs(z.Haber)) - y.Sum(z => Math.Abs(z.Debe))
                                                  }).ToList();


            //Nota Revisar nubox
            //Aqui van los filtros de la cuenta contable que se está buscando conciliar
            //¿Aquí también hay conciliación bancaria? -> No entiendo realmente como hacerlo.

            //Por hacer:
            //Todos aquellos movimientos que se hicieron en la conciliacion bancaria que no tenian información o la cuenta tenia auxiliar y no lo puso
            //Deben mapearse guardarse o encontrarse y dejarlos en este listado de pendientes


            //Por lo que pude notar esto se puede conseguir con la logica hecha en el estado de cuentas corriente
            //¿Como se van a generar los pagos?
            // Escriba aquí la planificación
            // Respuesta rápida -> Con la misma lógica que se usa al importar el excel con sus respectivos movimientos 
            return View(PendientesAuxOrder);
        }
        public ActionResult PendientesAuxDetalle(int IdAux)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var PendientesAuxDetalle = (from Detalle in db.DBDetalleVoucher
                                        join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                        join Auxiliar in db.DBAuxiliares on Detalle.DetalleVoucherModelID equals Auxiliar.DetalleVoucherModelID
                                        join AuxiliarDetalle in db.DBAuxiliaresDetalle on Auxiliar.AuxiliaresModelID equals AuxiliarDetalle.AuxiliaresModelID

                                        where Voucher.DadoDeBaja == false &&
                                              Voucher.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                              Detalle.ObjCuentaContable.TieneAuxiliar == 1 &&
                                              Detalle.ConciliadoCtasCtes == false &&
                                              Auxiliar.AuxiliaresModelID == IdAux

                                        select new AuxPendientesDetalle
                                        {
                                             Id = AuxiliarDetalle.AuxiliaresDetalleModelID,
                                             Rut = AuxiliarDetalle.Individuo2.RUT,
                                             RazonSocial = AuxiliarDetalle.Individuo2.RazonSocial,
                                             Debe = Detalle.MontoDebe > 0 ? AuxiliarDetalle.MontoTotalLinea : 0,
                                             Haber = Detalle.MontoHaber > 0 ? AuxiliarDetalle.MontoTotalLinea : 0 
                                        });


            return View(PendientesAuxDetalle.ToList());
        }
    }
}