using AE.Net.Mail;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using ClosedXML.Excel;
using System.Globalization;
using PagedList;
using System.Text;

namespace TryTestWeb.Controllers
{
    public class FMSendController : Controller
    {
        [Authorize]
        [ModuloHandler]
        public ActionResult AdminFunciones()
        {
            return View();
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult AdminFuncionesUpdate(int ambiente, int usuario, int empresa, int[] funciones)
        {
            if (ambiente <= 0 || usuario <= 0 || empresa <= 0)
            {
                return null;
            }
            FacturaPoliContext db = null;
            if (ambiente == 1) //Certificacion
                db = new FacturaPoliContext();
            else if (ambiente == 2) //Produccion
                db = new FacturaPoliContext(true);
            else
                return null;

            UsuarioModel objUser = db.DBUsuarios.SingleOrDefault(r => r.UsuarioModelID == usuario);
            if (objUser == null)
                return null;
            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.QuickEmisorModelID == empresa);
            if (objEmisor == null)
                return null;

            var lstModulosHabilitados = db.DBModulosHabilitados.Where
                                                            (r => r.UsuarioModelID == objUser.UsuarioModelID
                                                            && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);

            //Borra todos los privilegios de este usuario para asi luego crearlos nuevamente
            db.DBModulosHabilitados.RemoveRange(lstModulosHabilitados);
            db.SaveChanges();

            var lstFunciones = db.DBFunciones.ToList();

            List<ModulosHabilitados> lstModulosHabilitadosNew = new List<ModulosHabilitados>();
            if (funciones != null)
            {
                foreach (int funcion in funciones)
                {
                    FuncionesModel modelFuncion = lstFunciones.SingleOrDefault(r => r.FuncionesModelID == funcion);
                    lstModulosHabilitadosNew.Add(new ModulosHabilitados(objUser.UsuarioModelID, objEmisor.QuickEmisorModelID, modelFuncion));
                }
                db.DBModulosHabilitados.AddRange(lstModulosHabilitadosNew);
                db.SaveChanges();
            }
            return RedirectToAction("AdminFunciones", "FMSend");
        }

        [Authorize]
        public JsonResult obtenerFuncionesAdminFunciones(int dataContext, int SelectedUser, int SelectedEmisor)
        {
            StringBuilder optionSelect = new StringBuilder();
            if(SelectedUser <= 0 || SelectedEmisor <= 0)
                return Json(new { ok = true, selectInput = optionSelect }, JsonRequestBehavior.AllowGet);
            FacturaPoliContext db = null;
            if (dataContext == 1) //Certificacion
                db = new FacturaPoliContext();
            else if (dataContext == 2) //Produccion
                db = new FacturaPoliContext(true);
            else //No determinado, no nos interesa
            {
                return Json(new { ok = true, selectInput = optionSelect }, JsonRequestBehavior.AllowGet);
            }

            UsuarioModel objUser = db.DBUsuarios.SingleOrDefault(r => r.UsuarioModelID == SelectedUser);
            if (objUser == null)
                return Json(new { ok = true, selectInput = optionSelect }, JsonRequestBehavior.AllowGet);

            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.QuickEmisorModelID == SelectedEmisor);
            if(objEmisor == null)
                return Json(new { ok = true, selectInput = optionSelect }, JsonRequestBehavior.AllowGet);

            List<FuncionesModel> lstFuncionesSistema = db.DBFunciones.OrderBy(r => r.NombreModulo).ThenBy(r => r.NombreFuncion).ToList();
            List<ModulosHabilitados> lstModulosHabilitados = db.DBModulosHabilitados.Where
                                                            (r => r.UsuarioModelID == objUser.UsuarioModelID 
                                                            && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID)
                                                            .ToList();

            foreach (FuncionesModel Funcion in lstFuncionesSistema)
            {
                if (lstModulosHabilitados.Any(r => r.Funcion.FuncionesModelID == Funcion.FuncionesModelID))
                {
                    optionSelect.Append("<option selected value=\"" + Funcion.FuncionesModelID + "\">" + Funcion.NombreModulo + " - " + Funcion.NombreFuncion +"</option>"); //+ " (" + EmisorDisabled.RUTEmpresa + ")
                }
                else
                {
                    optionSelect.Append("<option value=\"" + Funcion.FuncionesModelID + "\">" + Funcion.NombreModulo + " - " + Funcion.NombreFuncion +"</option>"); //+ " (" + EmisorDisabled.RUTEmpresa + ")
                }
            }
            return Json(new
            {
                ok = true,
                selectInput = optionSelect.ToString()
            }, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        public JsonResult obtenerEmisoresAdminFunciones(int dataContext, int SelectedUser)
        {
            StringBuilder optionSelect = new StringBuilder();
            if (SelectedUser <= 0)
                return Json(new { ok = true, selectInput = optionSelect.ToString() }, JsonRequestBehavior.AllowGet);
            FacturaPoliContext db = null;
            if (dataContext == 1) //Certificacion
                db = new FacturaPoliContext();
            else if (dataContext == 2) //Produccion
                db = new FacturaPoliContext(true);
            else //No determinado, no nos interesa
            {
                return Json(new { ok = true, selectInput = optionSelect.ToString() }, JsonRequestBehavior.AllowGet);
            }

            UsuarioModel objUser = db.DBUsuarios.SingleOrDefault(r => r.UsuarioModelID == SelectedUser);
            if (objUser == null)
                return Json(new { ok = true, selectInput = optionSelect.ToString() }, JsonRequestBehavior.AllowGet);

            //Obtiene todos los emisores con los que el usuario especifico puede operar
            List<QuickEmisorModel> objEmisoresEnabled = new List<QuickEmisorModel>();
            bool HayUsuariosConFuncionalidades = ModuloHelper.GetEmisoresHabilitados(objUser.IdentityID, out objEmisoresEnabled, db);
            //Obtiene todos los emisores del sistema
            List<QuickEmisorModel> objEmisoresAll = db.Emisores.ToList();
            //Utiliza ambos para obtener la diferencia entre ambas listas, y determinar los emisores con los que NO puede operar
            List<QuickEmisorModel> objEmisoresNoFuncion = objEmisoresAll.Except(objEmisoresEnabled).ToList();

            optionSelect.Append("<option value='0'>Empresa</option>");
            //test branch 2
            if (objEmisoresEnabled.Count > 0)
            {
                optionSelect.Append("<optgroup label='Con Acceso'>");
                foreach (QuickEmisorModel EmisorEnabled in objEmisoresEnabled)
                {
                    optionSelect.Append("<option value=\"" + EmisorEnabled.QuickEmisorModelID + "\">" + EmisorEnabled.RazonSocial+" ("+EmisorEnabled.RUTEmpresa + ")</option>");
                }
                optionSelect.Append("</optgroup>");
            }

            if (objEmisoresNoFuncion.Count > 0)
            {
                optionSelect.Append("<optgroup label='Sin Acceso'>");
                foreach (QuickEmisorModel EmisorDisabled in objEmisoresNoFuncion)
                {
                    optionSelect.Append("<option value=\"" + EmisorDisabled.QuickEmisorModelID + "\">" + EmisorDisabled.RazonSocial + " (" + EmisorDisabled.RUTEmpresa + ")</option>");
                }
                optionSelect.Append("</optgroup>");
            }

            return Json(new
            {
                ok = true,
                selectInput = optionSelect.ToString()
            }, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        public JsonResult obtenerUsuariosAdminFunciones(int dataContext)
        {
            StringBuilder optionSelect = new StringBuilder();
            FacturaPoliContext db = null;

            if (dataContext == 1) //Certificacion
                db = new FacturaPoliContext();
            else if (dataContext == 2) //Produccion
                db = new FacturaPoliContext(true);
            else //No determinado, no nos interesa
            {
                return Json(new { ok = true, selectInput = optionSelect.ToString() }, JsonRequestBehavior.AllowGet);
            }

            List<UsuarioModel> lstUsuario = db.DBUsuarios.ToList();

            optionSelect.Append("<option value='0'>Usuarios</option>");
            string ResultOptionSelect = ParseExtensions.ListAsHTML_Input_Select<UsuarioModel>(lstUsuario, "UsuarioModelID", new List<string> { "Nombre", "RUT" });
            optionSelect.Append(ResultOptionSelect);

            return Json(new
            {
                ok = true,
                selectInput = optionSelect.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

   
     

        [Authorize]
        public ActionResult DTERecibido()
        {
            return View();
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult SwitchPlatformCert()
        {
            string UserID = User.Identity.GetUserId();
            FacturaProduccionContext db = new FacturaProduccionContext();
            /*
            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);
            objEmisor.DatabaseContextToUse = 0;
            */
            UsuarioModel objUsuario = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);
            objUsuario.DatabaseContextToUse = 0;
            Session["AlertaLibro"] = null;
            db.DBUsuarios.AddOrUpdate(c => c.UsuarioModelID, objUsuario);
            //db.DBUsuarios.AddOrUpdate(c => c.QuickEmisorModelID, objEmisor);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult SwitchPlatformProd()
        {
            string UserID = User.Identity.GetUserId();
            FacturaProduccionContext db = new FacturaProduccionContext();
            /*
            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);
            objEmisor.DatabaseContextToUse = 1;
            */
            UsuarioModel objUsuario = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);
            objUsuario.DatabaseContextToUse = 1;
            Session["AlertaLibro"] = null;
            db.DBUsuarios.AddOrUpdate(c => c.UsuarioModelID, objUsuario);
            //db.Emisores.AddOrUpdate(c => c.QuickEmisorModelID, objEmisor);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

    
        [Authorize]
        public ActionResult ImportDocuments()
        {
            bool canOperate = ModuloHandler.FuncionRequerida(System.Web.HttpContext.Current, "ImportDocumentsAction");
            if (canOperate == false)
            {
                return RedirectToAction("SeleccionarEmisorDesdeModulos", "Home");
            }
            return View();
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
     
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
      

    }
}
