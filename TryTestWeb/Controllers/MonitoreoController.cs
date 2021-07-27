using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TryTestWeb.Models.Monitoreo;
using TryTestWeb.Models.Monitoreo.MonitoreoSesionViewModel;

namespace TryTestWeb.Controllers
{
    public class MonitoreoController : Controller
    {
        // GET: Monitoreo
        public ActionResult ControlarEstadoSesion(/*bool EstaIniciando*/)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var NombreUsuario = (from usuario in db.DBUsuarios
                                join name in db.DBMonitoreoSesion
                                on usuario.UsuarioModelID equals name.UsuarioID
                                select new MonitoreoSesionViewModel
                                { 
                                    Nombre = usuario.Nombre,
                                    EstaActivo = name.EstaActivo
                                }).ToList();
            
            

            return View(NombreUsuario);
        }
    }
}