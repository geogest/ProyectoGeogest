using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TryTestWeb.Controllers
{
    public class PartialHelpersController : Controller
    {
        // GET: PartialHelpers
        public ActionResult DatosClienteParaPDFPArtial()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            ViewBag.ObjClienteContable = objCliente;
            return PartialView();
        }
    }
}
