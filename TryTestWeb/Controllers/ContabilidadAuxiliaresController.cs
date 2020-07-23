using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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

                List<CuentaContableModel> lstCtasAux = objCliente.CtaContable.Where(cta => cta.TieneAuxiliar == 1).ToList();
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

            
        
        }
    }