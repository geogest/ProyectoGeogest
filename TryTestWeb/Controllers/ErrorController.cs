using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TryTestWeb.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error(int error = 0)
        {
            Response.TrySkipIisCustomErrors = true;
            switch (error)
            {
                case 400:
                    ViewBag.Title = "Solicitud incorrecta";
                    ViewBag.Description = "Su navegador envía una solicitud válida";
                    ViewBag.Code = 400;
                    break;

                case 403:
                    ViewBag.Title = "Prohibida";
                    ViewBag.Description = "Usted no puede acceder a esta pagina";
                    ViewBag.Code = 403;
                    break;

                case 404:
                    ViewBag.Title = "No Encontrada";
                    ViewBag.Description = "La pagina que esta buscando no existe";
                    ViewBag.Code = 404;
                    break;

                case 500:
                    ViewBag.Title = "Error interno";
                    ViewBag.Description = "El servidor experimento un error interno";
                    ViewBag.Code = 500;
                    break;

                case 503:
                    ViewBag.Title = "Servicio no disponible";
                    ViewBag.Description = "El servidor no puede manejar su peticion en este momento, por favor intente mas tarde";
                    ViewBag.Code = 400;
                    break;

                default:
                    ViewBag.Title = "Solicitud incorrecta";
                    ViewBag.Description = "Su navegador envía una solicitud válida";
                    ViewBag.Code = 400;
                    break;
            }

            return View("~/Views/Shared/ErrorP.cshtml");
        }
    }
}