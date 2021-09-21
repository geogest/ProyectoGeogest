using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.Globalization;
using Newtonsoft.Json;
using System.Data.Entity.Validation;
using System.IO;
using System.Xml;
using ClosedXML.Excel;
using System.Web.Routing;
using System.Data.Entity;
using TryTestWeb.Models.ModelosSistemaContable.ContabilidadBoletas;

namespace TryTestWeb.Controllers
{
    /*
        string UserID = User.Identity.GetUserId();
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
        UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID); 
    */
    public class ContabilidadController : Controller
    {
        [ModuloHandler]
        [Authorize]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult NuevoClienteContable(AccionMonitoreo accion = AccionMonitoreo.Creacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            List<ActividadEconomicaModel> actividades = db.DBActeco.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            ViewBag.Actividades = actividades;


            List<ComunaModels> lstComunas = db.DBComunas.ToList();
            ViewBag.opComuna = ParseExtensions.ListAsHTML_Input_Select<ComunaModels>(lstComunas, "ComunaModelsID", "nombre");

            ClientesContablesModel cliente = new ClientesContablesModel();


            List<ActividadEconomicaModel> lstAllActectos = db.DBActeco.ToList();
            QuickEmisorModel obj = null;
            obj = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            // List<string> lstCodActEcoSelected = obj.lstActividadEconomica.Select(r => r.CodigoInterno).ToList();

            ViewBag.HtmlStr = ParseExtensions.ListAsHTML_Input_Select<ActividadEconomicaModel>(lstAllActectos, "CodigoInterno", new List<string> { "CodigoInterno", "NombreActividad" });


            return View(cliente);

        }

        public ActionResult PanelClienteContable()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel clienteSeleccionado = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var TotalesPorMes = ParseExtensions.TotalesGananciasYPerdidasDelMes(db, clienteSeleccionado);
 
            ViewBag.GananciasMes = Math.Abs(TotalesPorMes.Item1);
            ViewBag.PerdidasMes = Math.Abs(TotalesPorMes.Item2);

            var TotalAnual = ParseExtensions.TotalGananciasYPerdidasAnual(db, clienteSeleccionado);

            ViewBag.GananciasAnual = Math.Abs(TotalAnual.Item1);
            ViewBag.PerdidasAnual = Math.Abs(TotalAnual.Item2);

            var TotalAnualYTodoslosMeses = ParseExtensions.TotalGananciasYPerdidasAnio(db, clienteSeleccionado);
            
            ViewBag.TotalesMesesGanancias = TotalAnualYTodoslosMeses.Item1.Select(x => Math.Abs(x));
            ViewBag.TotalesMesesPerdidas = TotalAnualYTodoslosMeses.Item2.Select(x => Math.Abs(x));
            ViewBag.MesesYAnios = TotalAnualYTodoslosMeses.Item3.Select(x => ParseExtensions.obtenerNombreMes(x.Month));
            ViewBag.Anio = TotalAnualYTodoslosMeses.Item3.Select(x => x.Year).FirstOrDefault();
            

            string GastosXIntereses = "42";

            SubClasificacionCtaContable ExisteGastosXIntereses = new SubClasificacionCtaContable();
            if(clienteSeleccionado != null) { 
                ExisteGastosXIntereses = db.DBSubClasificacion.SingleOrDefault(x => x.CodigoInterno == GastosXIntereses &&
                                                                                    x.ClientesContablesModelID == clienteSeleccionado.ClientesContablesModelID);
            }

            if (ExisteGastosXIntereses == null)
            {
                CuentaContableModel.UpdateCtaContParaAntiguos(clienteSeleccionado.ClientesContablesModelID, db);
            }

            return View();
        }

        public ActionResult testkim() {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            //se crea la base de datos
            List<UsuarioModel> usuarios = db.DBUsuarios.ToList();
            //se crea la consulta y luego se pasa a la vista
            //string UserID = User.Identity.GetUserId();
            //FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            //List<RegionModels> vicente = new List<RegionModels>();
            //vicente = db.DBRegiones.ToList();
            //ViewBag.lucho = vicente;
            return View(usuarios);
        }

        [ModuloHandler]
        [Authorize]
        public ActionResult EditarClienteContable()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            List<ActividadEconomicaModel> actividades = db.DBActeco.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            ViewBag.Actividades = actividades;


            // ClientesContablesModel clienteSeleccionado = db.DBClientesContables.SingleOrDefault(r => r.ClientesContablesModelID == idClienteContable);
            ClientesContablesModel clienteSeleccionado = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            ViewBag.idComuna = clienteSeleccionado.idComuna;


            List<ComunaModels> lstComunas = db.DBComunas.ToList();

            List<ActividadEconomicaModel> lstAllActectos = db.DBActeco.ToList();


            //   List<string> lstCodActEcoSelected = clienteSeleccionado.lstActividadEconomica.Select(r => r.CodigoInterno).ToList();
            var lstCodActEcoSelected = (from t1 in db.DBActividadECContable
                                        join t2 in db.DBActeco on t1.CodigoInterno equals t2.CodigoInterno
                                        where t1.ClienteContableModelID == clienteSeleccionado.ClientesContablesModelID
                                        select new { Codigo = t2.CodigoInterno }).Select(r => r.Codigo).ToList();



            //(db.ActividadECContable.Where(r => r.ClienteContableModelID == (Int32)clienteSeleccionado.ClientesContablesModelID ).ToList()).
            ViewBag.HtmlStr = ParseExtensions.ListAsHTML_Input_Select<ActividadEconomicaModel>(lstAllActectos, "CodigoInterno", new List<string> { "CodigoInterno", "NombreActividad" }, lstCodActEcoSelected);


            return View(clienteSeleccionado);

        }

        [Authorize]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult EditarClientePost(ClientesContablesModel obj, string[] idActividad, AccionMonitoreo accion = AccionMonitoreo.Edicion)
        {

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel clienteSeleccionado = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            if (!ModelState.IsValid || idActividad.Count() == 0)
            {

                List<RegionModels> regiones = db.DBRegiones.ToList();
                List<ComunaModels> comunas = db.DBComunas.ToList();
                List<ActividadEconomicaModel> actividades = db.DBActeco.ToList();

                ViewBag.Regiones = regiones;
                ViewBag.Comunas = comunas;
                ViewBag.Actividades = actividades;

                List<ActividadEconomicaModel> lstAllActectos = db.DBActeco.ToList();



                var lstCodActEcoSelected = (from t1 in db.DBActividadECContable
                                            join t2 in db.DBActeco on t1.CodigoInterno equals t2.CodigoInterno
                                            where t1.ClienteContableModelID == clienteSeleccionado.ClientesContablesModelID
                                            select new { Codigo = t2.CodigoInterno }).Select(r => r.Codigo).ToList();




                ViewBag.HtmlStr = ParseExtensions.ListAsHTML_Input_Select<ActividadEconomicaModel>(lstAllActectos, "CodigoInterno", new List<string> { "CodigoInterno", "NombreActividad" }, lstCodActEcoSelected);
                ViewBag.idComuna = obj.idComuna;

                return View("EditarClienteContable", obj);
            }




            //  obj.lstActividadEconomica.Clear();

            //borro las lista de clientes anterior  



            List<ActividadEconomicaModelCuentaContableModel> lstsCodActEcoSelected = db.DBActividadECContable.Where(r => r.ClienteContableModelID == clienteSeleccionado.ClientesContablesModelID).ToList();


            foreach (ActividadEconomicaModelCuentaContableModel AEMCCM in lstsCodActEcoSelected)
            {
                db.DBActividadECContable.Remove(AEMCCM);
            }

            foreach (string singularActeco in idActividad)
            {
                ActividadEconomicaModel objActeco = db.DBActeco.SingleOrDefault(r => r.CodigoInterno == singularActeco);

                if (objActeco != null)
                {
                    ActividadEconomicaModelCuentaContableModel AEMCCM = new ActividadEconomicaModelCuentaContableModel();
                    AEMCCM.CodigoInterno = singularActeco;
                    AEMCCM.ClienteContableModelID = clienteSeleccionado.ClientesContablesModelID;



                    TryUpdateModel(AEMCCM);
                    db.DBActividadECContable.Add(AEMCCM);

                }

            }


            clienteSeleccionado.RazonSocial = obj.RazonSocial;
            clienteSeleccionado.RUTRepresentante = obj.RUTRepresentante;
            clienteSeleccionado.Representante = obj.Representante;
            clienteSeleccionado.Ciudad = obj.Ciudad;
            clienteSeleccionado.Telefono = obj.Telefono;
            clienteSeleccionado.Email = obj.Email;
            clienteSeleccionado.Giro = obj.Giro;
            clienteSeleccionado.ActEcono = obj.ActEcono;

            //ComunaModels objComuna = null;
            /* if (ComunaNumber != 0)
             {
                 objComuna = db.DBComunas.SingleOrDefault(r => r.ComunaModelsID == ComunaNumber);
                 if (objComuna != null)
                     clienteSeleccionado.Comuna = objComuna;
             }
             */
            TryUpdateModel(clienteSeleccionado);
            db.SaveChanges();
            TempData["Correcto"] = "Se ha actualizado el cliente contable satisfactoriamente";
            return RedirectToAction("EditarClienteContable", "Contabilidad");
        }

        [Authorize]
        public ActionResult NuevoClientePost(ClientesContablesModel obj, string[] idActividad)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);


            if (!ModelState.IsValid || idActividad.Count() == 0)
            {
                List<RegionModels> regiones = db.DBRegiones.ToList();
                List<ComunaModels> comunas = db.DBComunas.ToList();
                List<ActividadEconomicaModel> actividades = db.DBActeco.ToList();
                var lstCodActEcoSelected = (from t1 in db.DBActeco

                                            where idActividad.Contains(t1.CodigoInterno)

                                            select new { Codigo = t1.CodigoInterno }).Select(r => r.Codigo).ToList();



                ViewBag.HtmlStr = ParseExtensions.ListAsHTML_Input_Select<ActividadEconomicaModel>(actividades, "CodigoInterno", new List<string> { "CodigoInterno", "NombreActividad" }, lstCodActEcoSelected);
                ViewBag.Regiones = regiones;
                ViewBag.Comunas = comunas;
                // ViewBag.Actividades = actividades;
                return View("NuevoClienteContable", obj);
            }


            //UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);
            /*ComunaModels objComuna = null;
            if (ComunaNumber != 0)
            {
                objComuna = db.DBComunas.SingleOrDefault(r => r.ComunaModelsID == ComunaNumber);
                if (objComuna != null)
                    obj.Comuna = objComuna;


            }*/

            //actividadobjEmisor
            // objEmisor.
            UsuarioModel usuario = db.DBUsuarios.Where(r => r.IdentityID == UserID).FirstOrDefault();
            int totalClientesContables = 0;// db.DBClientesContables.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID).Count();
            int ambiente = 2;
            if (ParseExtensions.ItsUserOnCertificationEnvironment(User.Identity.GetUserId()))
            {
                ambiente = 1;
            }
            var clientesContables = db.DBClientesContables.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);  //obtenerClientesContables2(ambiente, usuario.UsuarioModelID, objEmisor.QuickEmisorModelID);
            if (clientesContables != null)
            {
                totalClientesContables = clientesContables.Count();
            }
            //agrego validación  
            if (objEmisor.maxClientesContablesParaEstaEmpresa < totalClientesContables)
            {

                List<RegionModels> regiones = db.DBRegiones.ToList();
                List<ComunaModels> comunas = db.DBComunas.ToList();
                List<ActividadEconomicaModel> actividades = db.DBActeco.ToList();



                var lstCodActEcoSelected = (from t1 in db.DBActeco

                                            where idActividad.Contains(t1.CodigoInterno)

                                            select new { Codigo = t1.CodigoInterno }).Select(r => r.Codigo).ToList();



                ViewBag.HtmlStr = ParseExtensions.ListAsHTML_Input_Select<ActividadEconomicaModel>(actividades, "CodigoInterno", new List<string> { "CodigoInterno", "NombreActividad" }, lstCodActEcoSelected);
                ViewBag.Regiones = regiones;
                ViewBag.Comunas = comunas;
                // ViewBag.Actividades = actividades;
                TempData["Error"] = "Cantidad Máxima de Cliente Contable Excedida, favor comuniquese con el adminstrador para actualizar su plan";
                return View("NuevoClienteContable", obj);


            }

            obj.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
            db.DBClientesContables.Add(obj);
            db.SaveChanges();


            //Le asignamos al usuario el cliente contable recién creado.
            UserClientesContablesModels ActivadorClienteContable = new UserClientesContablesModels();

            ActivadorClienteContable.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
            ActivadorClienteContable.ClientesContablesHabilitadosID = obj.ClientesContablesModelID;
            ActivadorClienteContable.UsuarioModelID = usuario.UsuarioModelID;

            db.DBUserToClientesContables.Add(ActivadorClienteContable);
            db.SaveChanges();


            foreach (string singularActeco in idActividad)
            {
                ActividadEconomicaModel objActeco = db.DBActeco.SingleOrDefault(r => r.CodigoInterno == singularActeco);

                if (objActeco != null)
                {
                    ActividadEconomicaModelCuentaContableModel AEMCCM = new ActividadEconomicaModelCuentaContableModel();
                    AEMCCM.CodigoInterno = singularActeco;
                    AEMCCM.ClienteContableModelID = obj.ClientesContablesModelID;



                    TryUpdateModel(AEMCCM);
                    db.DBActividadECContable.Add(AEMCCM);
                    db.SaveChanges();

                }

            }


            CuentaContableModel.ListNuevaCuentaContableBasica(obj.ClientesContablesModelID, db);

            //Configurar parametros cliente una vez el plan de cuentas este hecho
            ParametrosClienteModel NuevosParametrosCliente = new ParametrosClienteModel();
            NuevosParametrosCliente.ClientesContablesModelID = obj.ClientesContablesModelID;


            NuevosParametrosCliente.CuentaIvaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "220401" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            // NuevosParametrosCliente.CuentaIvaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "410101" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            NuevosParametrosCliente.CuentaIvaCompras = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "110701" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            // NuevosParametrosCliente.CuentaIvaCompras = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "510101" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            NuevosParametrosCliente.CuentaRetencionHonorarios = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "220405" && w.ClientesContablesModelID == obj.ClientesContablesModelID);//220405

            NuevosParametrosCliente.CuentaRetencionesHonorarios2 = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "220502" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            // NuevosParametrosCliente.CuentaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "110401" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            NuevosParametrosCliente.CuentaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "110401" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            //NuevosParametrosCliente.CuentaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "510101" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            NuevosParametrosCliente.CuentaCompras = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "220101" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
            //NuevosParametrosCliente.CuentaCompras = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "410101" && w.ClientesContablesModelID == obj.ClientesContablesModelID);

            obj.ParametrosCliente = NuevosParametrosCliente;

            TryUpdateModel(obj);
            db.SaveChanges();


            //agrego acceso a empresa
            int[] idsCuentaContables = { obj.ClientesContablesModelID };
            UsuarioModel user = AdminUsuariosContablesUpdate2(ambiente, usuario.UsuarioModelID, objEmisor.QuickEmisorModelID, idsCuentaContables);
            TryUpdateModel(user);
            /*ClientesContablesEmisorModel nuevaRelacion = new ClientesContablesEmisorModel();
            nuevaRelacion.ClientesContablesModelID = obj.ClientesContablesModelID;
            nuevaRelacion.QuickReceptorModelID = objEmisor.QuickEmisorModelID;
            db.DBClientesContablesEmisor.Add(nuevaRelacion);
            db.SaveChanges();*/

            TempData["Correcto"] = "Se ha creado el nuevo cliente contable satisfactoriamente";
            return RedirectToAction("NuevoClienteContable", "Contabilidad");
        }


        [Authorize]
        [ModuloHandler]
        public ActionResult AdminUsuariosContables()
        {
            return View();
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult AdminUsuariosContablesUpdate(int ambiente, int usuario, int empresa, int[] funciones)
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

            UserClientesContablesModels Model = new UserClientesContablesModels();

            IEnumerable<UserClientesContablesModels> UsuariosAResetear = db.DBUserToClientesContables.Where(r => r.UsuarioModelID == usuario && r.QuickEmisorModelID == empresa);

            if (UsuariosAResetear == null)
                return null;
            

            if (objUser == null)
                return null;

            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.QuickEmisorModelID == empresa);
            if (objEmisor == null)
                return null;

            //Limpiamos Antes que nada.
            foreach (UserClientesContablesModels ItemABorrar in UsuariosAResetear.ToList())
            {
                db.DBUserToClientesContables.Remove(ItemABorrar);
                db.SaveChanges();
            }

            //objUser.ClientesContablesAutorizados.Clear();
            //TryUpdateModel(objUser);
            //db.SaveChanges();

            List<ClientesContablesModel> lstClientesContablesAbsolut = db.DBClientesContables.ToList();
            if (lstClientesContablesAbsolut != null)
            {
                if (funciones != null)
                {
                    foreach (int funcion in funciones)
                    {
                        ClientesContablesModel modelClienteContable = lstClientesContablesAbsolut.SingleOrDefault(r => r.ClientesContablesModelID == funcion);
                        if (modelClienteContable != null)

                            //Volvemos a insertar.
                            Model.ClientesContablesHabilitadosID = funcion;
                            Model.UsuarioModelID = objUser.UsuarioModelID;
                            Model.QuickEmisorModelID = objEmisor.QuickEmisorModelID;

                            db.DBUserToClientesContables.Add(Model);
                            db.SaveChanges();
                    }
                }
            }
            //TryUpdateModel(objUser);
            //db.SaveChanges();

            return RedirectToAction("AdminUsuariosContables", "Contabilidad");
        }

        public static UsuarioModel AdminUsuariosContablesUpdate2(int ambiente, int usuario, int empresa, int[] funciones)
        {

            UsuarioModel objUser = new UsuarioModel();
            if (ambiente <= 0 || usuario <= 0 || empresa <= 0)
            {
                return objUser;
            }
            FacturaPoliContext db = null;
            if (ambiente == 1) //Certificacion
                db = new FacturaPoliContext();
            else if (ambiente == 2) //Produccion
                db = new FacturaPoliContext(true);
            else
                return objUser;

            objUser = db.DBUsuarios.SingleOrDefault(r => r.UsuarioModelID == usuario);
            if (objUser == null)
                return objUser;
            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.QuickEmisorModelID == empresa);
            if (objEmisor == null)
                return objUser;

            // objUser.ClientesContablesAutorizados.Clear();
            //TryUpdateModel(objUser);
            //  db.SaveChanges();

            List<ClientesContablesModel> lstClientesContablesAbsolut = db.DBClientesContables.ToList();
            if (lstClientesContablesAbsolut != null)
            {
                if (funciones != null)
                {
                    foreach (int funcion in funciones)
                    {
                        ClientesContablesModel modelClienteContable = lstClientesContablesAbsolut.SingleOrDefault(r => r.ClientesContablesModelID == funcion);
                        if (modelClienteContable != null)
                            objUser.ClientesContablesAutorizados.Add(modelClienteContable);
                    }
                }
            }
            //TryUpdateModel(objUser);
            db.SaveChanges();

            return objUser;
        }


        [Authorize]



        public JsonResult obtenerClientesContables(int dataContext, int SelectedUser, int SelectedEmisor)
        {
            StringBuilder optionSelect = new StringBuilder();
            if (SelectedUser <= 0 || SelectedEmisor <= 0)
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
            if (objEmisor == null)
                return Json(new { ok = true, selectInput = optionSelect }, JsonRequestBehavior.AllowGet);

            List<ClientesContablesModel> lstClientesContables = objEmisor.lstClientesCompania.OrderBy(r => r.RazonSocial).ToList();

            List<UserClientesContablesModels> lstHabilitados = db.DBUserToClientesContables.Where(r => r.UsuarioModelID == SelectedUser).ToList();

            foreach (ClientesContablesModel clienteContable in lstClientesContables)
            {
                if (lstHabilitados.Any(r => r.ClientesContablesHabilitadosID == clienteContable.ClientesContablesModelID))
                {
                    optionSelect.Append("<option selected value=\"" + clienteContable.ClientesContablesModelID + "\">" + clienteContable.RazonSocial + "</option>");
                }
                else
                {
                    optionSelect.Append("<option value=\"" + clienteContable.ClientesContablesModelID + "\">" + clienteContable.RazonSocial + "</option>");
                }
            }
            return Json(new
            {
                ok = true,
                selectInput = optionSelect.ToString()
            }, JsonRequestBehavior.AllowGet);
        }


        public static List<ClientesContablesModel> obtenerClientesContables2(int dataContext, int SelectedUser, int SelectedEmisor)
        {
            StringBuilder optionSelect = new StringBuilder();
            List<ClientesContablesModel> lstSalida = new List<ClientesContablesModel>();
            if (SelectedUser <= 0 || SelectedEmisor <= 0)
                return lstSalida;
            FacturaPoliContext db = null;
            if (dataContext == 1) //Certificacion
                db = new FacturaPoliContext();
            else if (dataContext == 2) //Produccion
                db = new FacturaPoliContext(true);
            else //No determinado, no nos interesa
            {
                return lstSalida;
            }

            UsuarioModel objUser = db.DBUsuarios.SingleOrDefault(r => r.UsuarioModelID == SelectedUser);
            if (objUser == null)
                return lstSalida;

            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.QuickEmisorModelID == SelectedEmisor);
            if (objEmisor == null)
                return lstSalida;

            List<ClientesContablesModel> lstClientesContables = objEmisor.lstClientesCompania.OrderBy(r => r.RazonSocial).ToList();

            foreach (ClientesContablesModel clienteContable in lstClientesContables)
            {
                if (objUser.ClientesContablesAutorizados.Any(r => r.ClientesContablesModelID == clienteContable.ClientesContablesModelID))
                {
                    lstSalida.Add(clienteContable);

                }

            }
            return lstSalida;
        }


        [Authorize]
        [ModuloHandler]
        public ActionResult SeleccionarClienteContable(string ClienteContable)
        {
            
            ModuloHelper.PurgeSessionContable(Session);
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);

            if (ObjUsuario == null)
                return null;

            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            if (ObjUsuario == null)
                return null;

            PerfilUsuarioModel TipoUsuario = db.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == ObjUsuario.PerfilUsuarioModelID);

            if(TipoUsuario != null)
                ViewBag.PerfilUsuario = TipoUsuario.NombrePerfil;


            //List<ClientesContablesModel> ReturnValues = new List<ClientesContablesModel>();
            //List<UserClientesContablesModels> Habilitados = db.DBUserToClientesContables.Where(r => r.UsuarioModelID == ObjUsuario.UsuarioModelID && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID).ToList();



            //    foreach (UserClientesContablesModels ItemHabilitados in Habilitados)
            //    {
            //        List<ClientesContablesModel> ClientesEnables = db.DBClientesContables.Where(r => r.ClientesContablesModelID == ItemHabilitados.ClientesContablesHabilitadosID).ToList();

            //        foreach (ClientesContablesModel ItemProcesado in ClientesEnables)
            //        {
            //            ReturnValues.Add(ItemProcesado);
            //        }
            //    }

            IQueryable<ClientesContablesModel> ReturnValues = null;

            List<int> HabilitadosId = db.DBUserToClientesContables.Where(r => r.UsuarioModelID == ObjUsuario.UsuarioModelID && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID).Select(x => x.ClientesContablesHabilitadosID).ToList();

            if (HabilitadosId.Any())
            {
                ReturnValues = db.DBClientesContables.Where(x => HabilitadosId.Contains(x.ClientesContablesModelID));
                if (!string.IsNullOrWhiteSpace(ClienteContable))
                    ReturnValues = ReturnValues.Where(x => x.RazonSocial.Contains(ClienteContable));
            }

            return View(ReturnValues.ToList());
        }

        [Authorize]
        public ActionResult DoSeleccionarClienteContable()
        {
            string UserID = User.Identity.GetUserId();
            bool Esta_En_Certificacion = ParseExtensions.ItsUserOnCertificationEnvironment(UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            int outID = -1;
            if (Int32.TryParse(Request.Params["EmisorID"], out outID))
            {
                //bool successGettingEmisor = PerfilamientoModule.GetEmisor(UserID, outID, out objEmisor);




                ClientesContablesModel clienteContable = db.DBClientesContables.SingleOrDefault(r => r.ClientesContablesModelID == outID);

                // ClientesContablesModel clienteContable = ObjUsuario.ClientesContablesAutorizados.SingleOrDefault(r => r.ClientesContablesModelID == outID);

                if (clienteContable != null)
                {
                    if (Esta_En_Certificacion)
                    {
                        Session["ClienteContableSeleccionado_CERT"] = clienteContable.ClientesContablesModelID;
                        Session["ClienteContableSeleccionado_CERT_nombre"] = clienteContable.RazonSocial;
                    }
                    else
                    {
                        Session["ClienteContableSeleccionado"] = clienteContable.ClientesContablesModelID;
                        Session["ClienteContableSeleccionado_nombre"] = clienteContable.RazonSocial;
                    }
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }



        [Authorize]
        [ModuloHandler]
        public ActionResult ParametrizacionCliente()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            //UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);

            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<CuentaContableModel> lstCuentasContables = objCliente.CtaContable.ToList();

            CuentaContableModel ctaIVAVentas = objCliente.ParametrosCliente.CuentaIvaVentas;
            CuentaContableModel ctaIVACompras = objCliente.ParametrosCliente.CuentaIvaCompras;
            CuentaContableModel ctaRetencionHonorarios = objCliente.ParametrosCliente.CuentaRetencionHonorarios;
            CuentaContableModel ctaRetencionesHonorarios2 = objCliente.ParametrosCliente.CuentaRetencionesHonorarios2;
            CuentaContableModel ctaVentas = objCliente.ParametrosCliente.CuentaVentas;
            CuentaContableModel ctaCompras = objCliente.ParametrosCliente.CuentaCompras;



            if (ctaRetencionesHonorarios2 == null || ctaRetencionHonorarios == null) { 
    
                ParametrosClienteModel NuevosParametrosCliente = new ParametrosClienteModel();
                NuevosParametrosCliente.ClientesContablesModelID = objCliente.ClientesContablesModelID;
               
                NuevosParametrosCliente.CuentaIvaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "220401" && w.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                // NuevosParametrosCliente.CuentaIvaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "410101" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
                NuevosParametrosCliente.CuentaIvaCompras = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "110701" && w.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                // NuevosParametrosCliente.CuentaIvaCompras = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "510101" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
                NuevosParametrosCliente.CuentaRetencionHonorarios = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "220405" && w.ClientesContablesModelID == objCliente.ClientesContablesModelID);//220405

                NuevosParametrosCliente.CuentaRetencionesHonorarios2 = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "220502" && w.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                // NuevosParametrosCliente.CuentaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "110401" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
                NuevosParametrosCliente.CuentaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "110401" && w.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                //NuevosParametrosCliente.CuentaVentas = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "510101" && w.ClientesContablesModelID == obj.ClientesContablesModelID);
                NuevosParametrosCliente.CuentaCompras = db.DBCuentaContable.SingleOrDefault(w => w.CodInterno == "220101" && w.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                objCliente.ParametrosCliente = NuevosParametrosCliente;

                TryUpdateModel(objCliente);
                db.SaveChanges();
            }
            ctaRetencionesHonorarios2 = objCliente.ParametrosCliente.CuentaRetencionesHonorarios2;

            ViewBag.CuentaIVAVentas = ParseExtensions.ListAsHTML_Input_Select<CuentaContableModel>(lstCuentasContables, "CuentaContableModelID", new List<string> { "nombre", "CodInterno" }, ctaIVAVentas.CuentaContableModelID.ToString());
            ViewBag.CuentaIVACompras = ParseExtensions.ListAsHTML_Input_Select<CuentaContableModel>(lstCuentasContables, "CuentaContableModelID", new List<string> { "nombre", "CodInterno" }, ctaIVACompras.CuentaContableModelID.ToString());

            ViewBag.CuentaIVAHonorarios = ParseExtensions.ListAsHTML_Input_Select<CuentaContableModel>(lstCuentasContables, "CuentaContableModelID", new List<string> { "nombre", "CodInterno" }, ctaRetencionHonorarios.CuentaContableModelID.ToString());
      
             ViewBag.CuentaIVAHonorarios2 = ParseExtensions.ListAsHTML_Input_Select<CuentaContableModel>(lstCuentasContables, "CuentaContableModelID", new List<string> { "nombre", "CodInterno" }, ctaRetencionesHonorarios2.CuentaContableModelID.ToString());

            ViewBag.CuentaVentas = ParseExtensions.ListAsHTML_Input_Select<CuentaContableModel>(lstCuentasContables, "CuentaContableModelID", new List<string> { "nombre", "CodInterno" }, ctaVentas.CuentaContableModelID.ToString());
            ViewBag.CuentaCompras = ParseExtensions.ListAsHTML_Input_Select<CuentaContableModel>(lstCuentasContables, "CuentaContableModelID", new List<string> { "nombre", "CodInterno" }, ctaCompras.CuentaContableModelID.ToString());

            return View();
        }


        [Authorize]
        public ActionResult CambiarParametrosCliente(int ctaventa, int ivaventa, int ctacompra, int ivacompra, int retencion, int retencion2)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            //QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (ivaventa > 0)
                objCliente.ParametrosCliente.CuentaIvaVentas = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == ivaventa);

            if (ivacompra > 0)
                objCliente.ParametrosCliente.CuentaIvaCompras = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == ivacompra);

            if (retencion > 0)
                objCliente.ParametrosCliente.CuentaRetencionHonorarios = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == retencion);

            if (retencion > 0)
                objCliente.ParametrosCliente.CuentaRetencionesHonorarios2 = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == retencion2);

            if (ctaventa > 0)
                objCliente.ParametrosCliente.CuentaVentas = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == ctaventa);

            if (ctacompra > 0)
                objCliente.ParametrosCliente.CuentaCompras = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == ctacompra);

            TryUpdateModel(objCliente);
            db.SaveChanges();

            TempData["Correcto"] = "Se han actualizado los parametros del cliente satisfactoriamente";

            return RedirectToAction("ParametrizacionCliente");
        }

        //[Authorize]
        //[ModuloHandler]
        //public ActionResult IngresoCtaContable()
        //{
        //    CuentaContableModel ObjCtaContable = new CuentaContableModel();

        //    //Html.DropDownListFor(r => r.Clasificacion, Extensions.ToSelectList(Model.Clasificacion, true), new { @id = "clasificacion", @class = "required show-tick form-control", @data_live_search = "true", @data_size = "10", @title = "Clasificación" })
        //    string osCuentasContables = ParseExtensions.EnumAsHTML_Input_Select<ClasificacionCtaContable>(null, true);
        //    ViewBag.osCuentasContables = osCuentasContables;
        //    return View(ObjCtaContable);
        //}


        [Authorize]
        public ActionResult EditCtaContable(string nombrectaedit, int editFlag, int centroCostoE, int itemE, int analisisE, int concilacionE, int auxiliarE)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (string.IsNullOrWhiteSpace(nombrectaedit) || editFlag == 0)
                return RedirectToAction("ListaCuentasContables"); //manejar error en el futuro
            try
            {
                CuentaContableModel editCtaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == editFlag); //db.DBCuentaContable.SingleOrDefault(r => r.CuentaContableModelID == editFlag);
                if (editCtaContable == null)
                    return RedirectToAction("ListaCuentasContables"); //manejar error en el futuro

                editCtaContable.nombre = nombrectaedit;
                editCtaContable.TieneCentroDeCosto = centroCostoE;
                editCtaContable.ItemsModelID = itemE;
                editCtaContable.AnalisisContablesModelID = analisisE;
                editCtaContable.Concilaciones = concilacionE;
                editCtaContable.TieneAuxiliar = auxiliarE;

                db.DBCuentaContable.AddOrUpdate(editCtaContable);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/ErrorP.cshtml");
            }

            return RedirectToAction("ListaCuentasContables");
        }

        [Authorize]
        [ModuloHandler]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult NuevaCtaContable(string nombrecta, int clasificacion, int subclasificacion, int subsubclasificacion, string codcta, int centroCosto, int item, int analisis, int concilacion, int auxiliar, AccionMonitoreo accion = AccionMonitoreo.Creacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            //QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<CentroCostoModel> lstCentroCostos = objCliente.ListCentroDeCostos.ToList();

            ViewBag.CentroDeCostos = lstCentroCostos; // Iterar en la vista.

            if (ParseExtensions.AreEqualOrZero(new int[] { clasificacion, subclasificacion, subsubclasificacion }) || string.IsNullOrWhiteSpace(nombrecta))
                return View("~/Views/Shared/ErrorP.cshtml");//return RedirectToAction("IngresoCtaContable");

            string CodSuggested = ObtenerCodSugeridoSTR(subsubclasificacion);
            if (codcta != CodSuggested)
                return View("~/Views/Shared/ErrorP.cshtml");//return RedirectToAction("IngresoCtaContable");

            try
            {
                CuentaContableModel objCuenta = new CuentaContableModel();
                objCuenta.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                objCuenta.CodInterno = CodSuggested;
                objCuenta.nombre = nombrecta;
                objCuenta.TieneCentroDeCosto = centroCosto;
                objCuenta.ItemsModelID = item;
                objCuenta.AnalisisContablesModelID = analisis;
                objCuenta.Concilaciones = concilacion;
                objCuenta.Clasificacion = (ClasificacionCtaContable)clasificacion;
                objCuenta.SubClasificacion = db.DBSubClasificacion.Single(r => r.SubClasificacionCtaContableID == subclasificacion && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                objCuenta.SubSubClasificacion = db.DBSubSubClasificacion.Single(r => r.SubSubClasificacionCtaContableID == subsubclasificacion && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                objCuenta.AnalisisDeCuenta = false;
                objCuenta.TieneAuxiliar = auxiliar;

                db.DBCuentaContable.Add(objCuenta);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/ErrorP.cshtml");
                //return RedirectToAction("IngresoCtaContable");
            }

            return RedirectToAction("ListaCuentasContables");
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult ListaCuentasContables(string codigo = "", string nombre = "")
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            //QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string osCuentasContables = ParseExtensions.EnumAsHTML_Input_Select<ClasificacionCtaContable>(null, true);
            ViewBag.osCuentasContables = osCuentasContables;
            List<CuentaContableModel> lstCuentasContables = objCliente.CtaContable.OrderBy(r => r.CodInterno).ToList();
            ViewBag.ObjClienteContable = objCliente;

            if (codigo != "" && codigo != "")
            {
                ViewBag.codigo = codigo;
                lstCuentasContables = lstCuentasContables.Where(r => r.CodInterno.StartsWith(codigo)).ToList();
            }

            if (nombre != "" && nombre != "")
            {
                ViewBag.nombre = nombre;
                lstCuentasContables = lstCuentasContables.Where(r => r.nombre.ToUpper().Contains(nombre.ToUpper())).ToList();
            }


            return View(lstCuentasContables);
        }


        // Aquí se listarán las cuentas contables y se les podrá asignar un presupuesto.
        [Authorize]
        public ActionResult CtasContPresupuesto(string nombre = "", string codigo = "")
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            string osCuentasContables = ParseExtensions.EnumAsHTML_Input_Select<ClasificacionCtaContable>(null, true);
            ViewBag.osCuentasContables = osCuentasContables;

            List<CuentaContableModel> lstCuentasContables = objCliente.CtaContable.OrderBy(r => r.CodInterno).ToList();

            if (codigo != "" && codigo != "")
            {
                ViewBag.codigo = codigo;
                lstCuentasContables = lstCuentasContables.Where(r => r.CodInterno.StartsWith(codigo)).ToList();

            }

            if (nombre != "" && nombre != "")
            {
                ViewBag.nombre = nombre;
                lstCuentasContables = lstCuentasContables.Where(r => r.nombre.ToUpper().Contains(nombre.ToUpper())).ToList();

            }
            return View(lstCuentasContables);
        }

        [Authorize]
        public ActionResult GetExcelPresupuesto()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["Presupuesto"] != null)
            {
                List<string[]> cachedPresupuesto = Session["Presupuesto"] as List<string[]>;

                if (cachedPresupuesto != null)
                {
                    for (int i = 0; i < cachedPresupuesto.Count(); i++)
                    {
                        cachedPresupuesto[i] = cachedPresupuesto[i].Select(r => r.Replace(".", "")).ToArray();
                    }


                    string textAnioLibroMayor = "";
                    string textMesLibroMayor = "";
                    string textFechaInicio = (string)Session["FechaInicio"];
                    string textFechaFin = (string)Session["FechaVencimiento"];

                    //tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBalanceGeneralFechaInicio"] as string, Session["strBalanceGeneralFechaFin"] as string, Session["strBalanceGeneralAnio"] as int?, Session["strBalanceGeneralMes"] as int?, "LIBRO MAYOR");
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(textFechaInicio, textFechaFin, ParseExtensions.ParseInt(textAnioLibroMayor), ParseExtensions.ParseInt(textMesLibroMayor), "REPORTE PRESUPUESTO");

                    var cachedStream = VoucherModel.GetExcelPresupuestos(cachedPresupuesto, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Presupuesto" + Guid.NewGuid() + ".xlsx");
                }
            }
            return null;
        }


        [Authorize]
        public ActionResult GuardarPresupuesto(int[] Presupuesto, int[] ctacontid, string FechaInicio = "", string FechaVencimiento = "", string NombrePresupuesto = "")
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (Presupuesto != null && objCliente.ClientesContablesModelID != 0 && ctacontid != null && Presupuesto.Any(x => x != 0))  // Encontrar la forma de ir recorriendo la tabla que aparece desde la vista con el fin de guardar todos los datos del formulario
            {

                DateTime dtFechaInicio = new DateTime();
                DateTime dtFechaVencimiento = new DateTime();

                if (!string.IsNullOrWhiteSpace(FechaInicio) && !string.IsNullOrWhiteSpace(FechaVencimiento))
                {
                    dtFechaInicio = ParseExtensions.ToDD_MM_AAAA_Multi(FechaInicio);
                    dtFechaVencimiento = ParseExtensions.ToDD_MM_AAAA_Multi(FechaVencimiento);
                }

                // Hacemos que los 2 foreach se recorran en base al foreach padre (Osea el de presupuesto).
                // Y esto funciona solo por qué ambosarrays contienen la misma cantidad de elementos.

                CtasContablesPresupuestoModel InsertPresupuesto = new CtasContablesPresupuestoModel();


                PresupuestoModel LineaPresupuesto = new PresupuestoModel();
                LineaPresupuesto.Cliente = objCliente;
                LineaPresupuesto.FechaInicio = dtFechaInicio;
                LineaPresupuesto.FechaVencimiento = dtFechaVencimiento;
                LineaPresupuesto.NombrePresupuesto = NombrePresupuesto;
                db.DBPresupuestos.Add(LineaPresupuesto);
                db.SaveChanges();


                int ContadorPadre = -1;
                foreach (int Presu in Presupuesto)
                {
                    ContadorPadre++;
                    int PresupuestoToControl = Presu;
                    foreach (int CuentaContable in ctacontid.Skip(ContadorPadre))
                    {
                        if (Presu != 0)
                        {
                            InsertPresupuesto.PresupuestoModelID = LineaPresupuesto.PresupuestoModelID;
                            InsertPresupuesto.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                            InsertPresupuesto.Presupuesto = Convert.ToDecimal(Presu);
                            InsertPresupuesto.FechaInicioPresu = dtFechaInicio;
                            InsertPresupuesto.FechaVencimientoPresu = dtFechaVencimiento;
                            InsertPresupuesto.CuentasContablesModelID = CuentaContable;
                            db.DBCCPresupuesto.Add(InsertPresupuesto);
                            db.SaveChanges();

                            PresupuestoToControl = 0;

                        }
                        break;
                    }
                }
            }
            else
            {
                TempData["Error"] = "Debes establecer 1 o más montos en las cuentas contables";
                return RedirectToAction("CtasContPresupuesto", "Contabilidad");
            }

            TempData["Correcto"] = "Presupuesto añadido con éxito.";
            return RedirectToAction("CtasContPresupuesto", "Contabilidad");
        }

        [Authorize]
        public ActionResult MisPresupuestos()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<PresupuestoModel> Presupuestos = db.DBPresupuestos.Where(x => x.Cliente.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                               x.DadoDeBaja == false).ToList();

            return View(Presupuestos);
        }

        [Authorize]
        public ActionResult SubSubCtaPresupuesto(int[] Presupuesto, int[] subsubid)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            List<SubSubClasificacionCtaContable> lstSubSubCta = db.DBSubSubClasificacion.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID).ToList();


            if(Presupuesto != null && subsubid != null && objCliente.ClientesContablesModelID != 0)
            {
                

                List<SubSubCtaPresupuestoModel> ListaRemover = db.DBSubSubCtaPresupuesto.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID).ToList();

                //Limpiamos.
                foreach (SubSubCtaPresupuestoModel ItemARemover in ListaRemover)
                {
                    db.DBSubSubCtaPresupuesto.Remove(ItemARemover);
                    db.SaveChanges();
                }


                DateTime FechaInicio = DateTime.Now;
                DateTime FechaVencimiento = DateTime.Now;

                FechaVencimiento = FechaVencimiento.AddYears(1); // llevamos la fecha de vencimiento a 1 año.

                SubSubCtaPresupuestoModel InsertPresupuesto = new SubSubCtaPresupuestoModel();

                int ContadorPadre = -1;
                foreach (int Presu in Presupuesto)
                {
                    ContadorPadre++;
                    int PresupuestoToControl = Presu;
                    foreach (int SubSubCta in subsubid.Skip(ContadorPadre))
                    {
                        if (PresupuestoToControl != 0)
                        {

                            InsertPresupuesto.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                            InsertPresupuesto.Presupuesto = Convert.ToDecimal(Presu);
                            InsertPresupuesto.FechaInicio = FechaInicio;
                            InsertPresupuesto.FechaVencimiento = FechaVencimiento;
                            InsertPresupuesto.SubSubClasificacionCtaContableID = SubSubCta;
                            db.DBSubSubCtaPresupuesto.Add(InsertPresupuesto);
                            db.SaveChanges();

                            PresupuestoToControl = 0;
                        }
                        break;
                    }
                }
            }

            return View(lstSubSubCta);
        }


        [Authorize]
        [ModuloHandler]
        public ActionResult IngresoVoucher(int? IDVoucher = null)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            List<ItemModel> lstItems = db.DBItems.Where(r => r.ClienteContableID == objCliente.ClientesContablesModelID && r.Estado == true).ToList();
            List<ClientesProveedoresModel> lstRuts = db.DBClientesProveedores.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID && r.Estado == true).ToList();

            ViewBag.items = lstItems; // ParseExtensions.ListAsHTML_Input_Select<ItemModel>(lstItems, "ItemModelID",  "NombreItem" );
                                      //ViewBag.opComuna = ParseExtensions.ListAsHTML_Input_Select<ComunaModels>(lstComunas, "ComunaModelsID", "nombre");
            ViewBag.ruts = lstRuts;
            ViewBag.oClienteContable = objCliente;
            
            List<CentroCostoModel> lstCentroCosto = objCliente.ListCentroDeCostos.ToList();
            if (IDVoucher == null)  
            {
                //Creacion (e Importacion de Vouchers)
                Session["sessionAuxiliares"] = null;

                //FUERZA RENOVAR PRESTADORES DE RUT
                Session["AutoCompletePRESTADORES_RUT"] = null;


                ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);
                ViewBag.TipoOrigenVoucher = ParseExtensions.EnumToDropDownList<TipoOrigen>();

                List<string[]> lstDatosImportacionPrevia = new List<string[]>();
                if (Session["filasCentralizacion"] != null)
                {
                    lstDatosImportacionPrevia = (List<string[]>)Session["filasCentralizacion"];
                    Session["filasCentralizacion"] = null;
                    ViewBag.hacerAuxiliarOpcional = true;
                }

                Tuple<List<CentroCostoModel>, List<string[]>> tupleReturno = new Tuple<List<CentroCostoModel>, List<string[]>>(lstCentroCosto, lstDatosImportacionPrevia);
                ViewBag.NumVoucher = ParseExtensions.ObtenerNumeroProximoVoucherINT(objCliente, db);
                return View(tupleReturno);
            }
            else

            {
                //revisar si tiene privilegios para poder editar vouchers
                if (ModuloHandler.FuncionRequerida(System.Web.HttpContext.Current, "EditarVoucher") == false)
                {
                    return RedirectToAction("SeleccionarClienteContable", "Contabilidad");
                }

                //Edicion de Vouchers
                VoucherModel VoucherAEditar = db.DBVoucher.SingleOrDefault(r => r.VoucherModelID == IDVoucher && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                if (VoucherAEditar == null || VoucherAEditar.ListaDetalleVoucher == null || VoucherAEditar.ListaDetalleVoucher.Count == 0)
                {
                    return RedirectToAction("SeleccionarClienteContable", "Contabilidad");
                }

                List<AuxiliaresModel> lstAuxs = new List<AuxiliaresModel>();

                ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);

                List<string[]> valoresVoucherAEditar = new List<string[]>();

                ViewBag.NumVoucher = VoucherAEditar.NumeroVoucher;

                foreach (DetalleVoucherModel detalle in VoucherAEditar.ListaDetalleVoucher)
                {
                    string[] lineaDetalle = {
                            ParseExtensions.ToDD_MM_AAAA(detalle.FechaDoc),
                            detalle.ObjCuentaContable.CuentaContableModelID.ToString(),
                            detalle.GlosaDetalle,
                            detalle.MontoDebe.Normalizar().ToString(CultureInfo.InvariantCulture),
                            detalle.MontoHaber.Normalizar().ToString(CultureInfo.InvariantCulture),
                            detalle.CentroCostoID.ToString(),
                            detalle.ItemModelID.ToString()



                        };

                    valoresVoucherAEditar.Add(lineaDetalle);

                    if (detalle.Auxiliar != null)
                    {
                        lstAuxs.Add(detalle.Auxiliar);
                    }


                }

                Tuple<List<CentroCostoModel>, List<string[]>> tupleReturno = new Tuple<List<CentroCostoModel>, List<string[]>>(lstCentroCosto, valoresVoucherAEditar);

                ViewBag.FechaEmisionEdit = ParseExtensions.ToDD_MM_AAAA(VoucherAEditar.FechaEmision);

                ViewBag.GlosaVoucherEdit = VoucherAEditar.Glosa;

                ViewBag.TipoVoucherEdit = (int)VoucherAEditar.Tipo;

                ViewBag.TipoOrigenVoucher = (int)VoucherAEditar.TipoOrigenVoucher;

                if (VoucherAEditar.CentroDeCosto != null)
                {
                    ViewBag.CentroDeCostoEdit = (int)VoucherAEditar.CentroDeCosto.CentroCostoModelID;
                }

                ViewBag.ncc = 1;
                ViewBag.vbVoucherModel = VoucherAEditar;

                //Guarda sus auxiliares en forma temporal
                Session["sessionAuxiliares"] = null;
                if (lstAuxs != null && lstAuxs.Count > 0)
                {
                    Session["sessionAuxiliares"] = lstAuxs;
                }
                return View(tupleReturno);
            }
        }

        [Authorize]
        public ActionResult InfoImportada()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (Session["InfoImportada"] != null)
            {
                List<LibrosContablesModel> LstInfoImportada = (List<LibrosContablesModel>)Session["InfoImportada"];
                int existeCompra = 0;

                foreach (LibrosContablesModel libro in LstInfoImportada)
                {

                    if (ParseExtensions.ObtenerCenbtralizacion(libro.TipoLibro) == 2)
                    {

                        existeCompra = 1;

                    }
                }
                //Venta
                //   ViewBag.HtmlStr = ParseExtensions.ListAsHTML_Input_Select<CuentaContableModel>(objCliente.CtaContable, "CodInterno", new List<string> { "CodInterno", "nombre" }, new List<string> { "510101" });
                ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsStringWithSelectedCodInterno(objCliente, "510101");

                ViewBag.CentroDeCostos = ParseExtensions.ObtenerCentrosDeCostosDropdownAsString(objCliente,0);

              //  ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);
                if (existeCompra == 1)
                { // Compra
                //    ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);
                    ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsStringWithSelectedCodInterno(objCliente, "410101");
                }

           
                List<QuickReceptorModel> ReceptoresConRelacion = new List<QuickReceptorModel>();

                string TipoReceptorCompra = "PR";
                string TipoReceptorVenta = "CL";
                
                foreach (LibrosContablesModel ItemLibro in LstInfoImportada)
                {
                    QuickReceptorModel ReceptorAEncontrar = new QuickReceptorModel();

                    if (existeCompra == 1)
                    {
                        ReceptorAEncontrar = db.Receptores.SingleOrDefault(x => x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                                x.RUT == ItemLibro.individuo.RUT &&
                                                                                x.tipoReceptor == TipoReceptorCompra);
                    }else
                    {
                        ReceptorAEncontrar = db.Receptores.SingleOrDefault(x => x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                                x.RUT == ItemLibro.individuo.RUT &&
                                                                                x.tipoReceptor == TipoReceptorVenta);
                    }
                  
                    if(ReceptorAEncontrar != null && ReceptorAEncontrar.CuentaConToReceptor != null)
                    {
                        ReceptoresConRelacion.Add(ReceptorAEncontrar);
                    }
                    
                }

                if(ReceptoresConRelacion.Count() > 0)
                {
                    ViewBag.lstReceptoresConCta = ReceptoresConRelacion;
                    ViewBag.ObjClienteToView = objCliente;
                } 

              

                if (TempData["ErrorMensaje"] != null)
                    ViewBag.ErrorMensaje = "Por favor asignar una cuenta contable para todos los elementos";

                return View(LstInfoImportada);
            }
            else
            {
                ViewBag.ErrorMensaje = "Hubo un error al importar documento";
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult ImportarLibroContableAVoucher(IList<LibrosContablesModel> model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            try
            {
                List<LibrosContablesModel> lstProperLibros = new List<LibrosContablesModel>();
                foreach (LibrosContablesModel LibrosID in model)
                {
                    var elLibroReal = db.DBLibrosContables.Where(r => r.LibrosContablesModelID == LibrosID.LibrosContablesModelID &&
                                                                      r.ClientesContablesModelID == objCliente.ClientesContablesModelID)
                                                          .FirstOrDefault();

                    lstProperLibros.Add(elLibroReal);
                }

                int totalVenta = 0;
                int totalCompra = 0;
                foreach (LibrosContablesModel libro in lstProperLibros)
                {
                    if (ParseExtensions.ObtenerCenbtralizacion(libro.TipoLibro) == 1)
                    {
                        totalVenta++;
                    }
                    if (ParseExtensions.ObtenerCenbtralizacion(libro.TipoLibro) == 2)
                    {
                        totalCompra++;
                    }
                }
                string[] valuesCuentaContable = Request.Form.GetValues("cuenta");
                string[] valuiesCentroDeCosto = Request.Form.GetValues("CentroCosto");

                if (valuesCuentaContable == null || valuesCuentaContable.Length == 0 || valuesCuentaContable.Length != model.Count() || valuesCuentaContable.Any(r => String.IsNullOrWhiteSpace(r)))
                {
                    TempData["ErrorMensaje"] = "Falta por favor asignar una cuenta contable para todos los elementos";
                    return RedirectToAction("InfoImportada", "Contabilidad");
                }

                List<int> IdsCentroDeCostos = valuiesCentroDeCosto.Select(x => Convert.ToInt32(x)).ToList();

                List<CuentaContableModel> lstCuentaContable = new List<CuentaContableModel>();
                foreach (string strCuentaContable in valuesCuentaContable)
                {
                    int keyCuentaContable = ParseExtensions.ParseInt(strCuentaContable);
                    CuentaContableModel objCuentaContable = db.DBCuentaContable.Find(keyCuentaContable);
                    lstCuentaContable.Add(objCuentaContable);
                }
                string ResultadoProceso = LibrosContablesModel.ProcesarLibrosContablesAVoucher(lstProperLibros, objCliente, db, lstCuentaContable, IdsCentroDeCostos);
                if (ResultadoProceso.Contains("Error"))
                {
                    TempData["Error"] = ResultadoProceso;
                    return RedirectToAction("CargarLibros", "Contabilidad");
                }
                TempData["Correcto"] = "Libros ingresados con éxito.";
                return RedirectToAction("CargarLibros", "Contabilidad");
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Ha ocurrido un error inesperado" + ex.Message;
               return RedirectToAction("CargarLibros", "Contabilidad");
            }
        }

        [Authorize]
        [ModuloHandler]
        //[HttpGet]
        public ActionResult ListaVoucher(int cantidadRegistrosPorPagina = 25,
                                                              int pagina = 1,
                                                              int Mes = 0,
                                                              int Anio = 0,
                                                              string FechaInicio = "",
                                                              string FechaFin = "",
                                                              string Glosa = "",
                                                              int voucherID = 0,
                                                              int? TipoOrigenVoucher = null)
        {    
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }
            //List<LibrosContablesModel> LaLista = LibrosContablesModel.RescatarLibroCentralizacion(objCliente, TipoCentralizacion.Venta, db, FechaInicio, FechaFin, Anio, Mes);

            List<VoucherModel> LstVoucher;
         
            ViewBag.NombreCliente = objCliente.RazonSocial;

            //Predicado Base
            IQueryable<VoucherModel> Predicado = db.DBVoucher.Where(r => r.DadoDeBaja == false && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);


            // Filtros
            if(Anio != 0)
                Predicado = Predicado.Where(r => r.FechaEmision.Year == Anio);
            
            if(Mes != 0)
                Predicado = Predicado.Where(r => r.FechaEmision.Month == Mes);
            
            if (!string.IsNullOrWhiteSpace(Glosa))
                Predicado = Predicado.Where(r => r.Glosa.Contains(Glosa));
            
            if(voucherID != 0)
                Predicado = Predicado.Where(r => r.NumeroVoucher == voucherID);

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
                Predicado = Predicado.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin);

            if (TipoOrigenVoucher != null) {
                TipoOrigen TipoVoucherOrigen = (TipoOrigen)TipoOrigenVoucher;
                Predicado = Predicado.Where(r => r.TipoOrigenVoucher == (TipoOrigen)TipoOrigenVoucher || r.TipoOrigen == TipoVoucherOrigen.ToString());
            }

            LstVoucher = Predicado
                        .OrderByDescending(x => x.NumeroVoucher)
                        .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                        .Take(cantidadRegistrosPorPagina)
                        .ToList();

            var totalDeRegistros = Predicado.Count();

            // Pasamos los datos necesarios para que funcione el paginador generico.
            var Paginador = new PaginadorModel();
            Paginador.VoucherList = LstVoucher;
            Paginador.PaginaActual = pagina;
            Paginador.TotalDeRegistros = totalDeRegistros;
            Paginador.RegistrosPorPagina = cantidadRegistrosPorPagina;
            Paginador.ValoresQueryString = new RouteValueDictionary();
            //Mandar estos parametros por ajax.
            //En la vista
            // Parametros de busqueda llamados por GET.
            if (cantidadRegistrosPorPagina != 25)
                Paginador.ValoresQueryString["cantidadRegistrosPorPagina"] = cantidadRegistrosPorPagina;

            if (Anio != 0)
                Paginador.ValoresQueryString["Anio"] = Anio;

            if (Mes != 0)
                Paginador.ValoresQueryString["Mes"] = Mes;

            if (!string.IsNullOrWhiteSpace(Glosa))
                Paginador.ValoresQueryString["Glosa"] = Glosa;

            if (voucherID != 0)
                Paginador.ValoresQueryString["VoucherID"] = voucherID;

            if(ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                Paginador.ValoresQueryString["FechaInicio"] = FechaInicio;
                Paginador.ValoresQueryString["FechaFin"] = FechaFin;
            }
            if (TipoOrigenVoucher != null)
                Paginador.ValoresQueryString["TipoOrigenVoucher"] = TipoOrigenVoucher;

           
           return View(Paginador);
        }


        [Authorize]
        public JsonResult ValidarAuxiliar(List<string> IdsCtasContSeleccionadas)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool result = false;
            string Mensaje = "";

            if (objCliente == null)
                Mensaje = "Termino la sesión vuelve a iniciar.";

            List<int> lstCtasCont = IdsCtasContSeleccionadas.Select(x => Convert.ToInt32(x)).ToList();

            bool ExisteCuentaContConAuxiliar = objCliente.CtaContable.Where(x => lstCtasCont.Contains(x.CuentaContableModelID))
                                                                        .Where(x => x.TieneAuxiliar == 1)
                                                                           .Any();

            if (Session["sessionAuxiliares"] == null && ExisteCuentaContConAuxiliar) 
                Mensaje = "Debes rellenar los auxiliares";
            else
                result = true;

            return Json(new {ok = result, Mensaje = Mensaje });
        }

        [Authorize]
        public JsonResult BorrarMultiplesVouchers(List<string> VouchersID)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool Result = false;

            if(VouchersID.Count() > 0) { 
                foreach (string IDVoucher in VouchersID)
                {
                    int VoucherConvertido = Convert.ToInt32(IDVoucher);

                    VoucherModel VoucherABorrar = objCliente.ListVoucher.SingleOrDefault(x => x.DadoDeBaja == false && x.VoucherModelID == VoucherConvertido);

                    if(VoucherABorrar != null)
                    {
                       VoucherABorrar.DadoDeBaja = true;
                       db.SaveChanges();
                       Result = true;
                       TempData["Correcto"] = "Vouchers dados de baja con éxito.";
                    }else
                    {
                        Result = false;
                        TempData["Error"] = "Error inesperado";
                   
                    }
                }
            }
            return Json(Result, JsonRequestBehavior.AllowGet); 
        }

        [Authorize]
        public JsonResult RestaurarMultiplesVouchers(List<string> VouchersID)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool Result = false;

            if (VouchersID.Count() > 0)
            {
                foreach (string IDVoucher in VouchersID)
                {
                    int VoucherConvertido = Convert.ToInt32(IDVoucher);

                    VoucherModel VoucherABorrar = objCliente.ListVoucher.SingleOrDefault(x => x.DadoDeBaja == true && x.VoucherModelID == VoucherConvertido);

                    if (VoucherABorrar != null)
                    {
                        VoucherABorrar.DadoDeBaja = false;
                        db.SaveChanges();
                        Result = true;
                        TempData["Correcto"] = "Vouchers dados de baja con éxito.";
                    }
                    else
                    {
                        Result = false;
                        TempData["Error"] = "Error inesperado";

                    }
                }
            }
            return Json(Result, JsonRequestBehavior.AllowGet); 
        }



        [Authorize]
        [ModuloHandler]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult NuevoVoucher(AccionMonitoreo accion = AccionMonitoreo.Creacion) //ListaVouchersBaja(pagina))
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            VoucherModel objVoucher = new VoucherModel();

            //Numero de Voucher que sera utilizado en la insercion/actualizacion
            int NewNumeroVoucher = 0;

            List<int> lstIdsCtasContables = Request.Form.GetValues("ctacont").Select(x => Convert.ToInt32(x)).ToList();
            bool HayCuentaConAuxiliar = objCliente.CtaContable.Where(x => lstIdsCtasContables.Contains(x.CuentaContableModelID))
                                                               .Where(x => x.TieneAuxiliar == 1)
                                                                .Any();

            if(Session["sessionAuxiliares"] == null && HayCuentaConAuxiliar)
            {
                TempData["Error"] = "Debes rellenar los auxiliares.";
                return RedirectToAction("IngresoVoucher","Contabilidad");
            }
            

            //Intentar obtener y convertir valor NumVoucher para revisiones en ambos flujos
            int? numVoucherDesdeFormulario = null;
            if (Request.Form.GetValues("numVoucher") != null)
            {
                string strNumVoucher = Request.Form.GetValues("numVoucher")[0];
                numVoucherDesdeFormulario = ParseExtensions.ParseIntNullable(strNumVoucher);
                if (numVoucherDesdeFormulario.HasValue)
                    numVoucherDesdeFormulario = Math.Abs(numVoucherDesdeFormulario.Value);
            }

            //Edit Voucher 
            // a) rompe la relacion del voucher con el centro de costos para hacerla otra vez
            // b) borra todo el detalle del voucher y lo construye otra vez
            int IDVoucherAEditar = 0;
            if (Request.Form.GetValues("editFlag") != null)
            {
                int idToEdit = ParseExtensions.ParseInt(Request.Form.GetValues("editFlag")[0]);
                if (idToEdit == 0)
                    throw new InvalidOperationException("Invalid edit on ID 0");

                IDVoucherAEditar = idToEdit;
                objVoucher = db.DBVoucher.Include("CentroDeCosto").Single(r => r.VoucherModelID == idToEdit && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);

                if (objVoucher.NumeroVoucher == numVoucherDesdeFormulario)
                {
                    NewNumeroVoucher = objVoucher.NumeroVoucher;
                }
                else
                {
                    //verificar si el nuevo numero de voucher desde edicion no hace conflicto con algun
                    //numero de voucher ya existente para este cliente
                    bool estaDisponible = ParseExtensions.EstaNumeroVoucherDisponible(numVoucherDesdeFormulario.Value, objCliente, db);
                    if (estaDisponible) //si esta disponible lo asigna
                    {
                        NewNumeroVoucher = numVoucherDesdeFormulario.Value;
                    }
                    else // si no esta disponible le asigna uno disponible
                    {
                        int? nullableProxVoucherNumber = ParseExtensions.ObtenerNumeroProximoVoucherINT(objCliente, db);
                        if (nullableProxVoucherNumber.HasValue)
                            NewNumeroVoucher = nullableProxVoucherNumber.Value;
                    }
                }

                db.DBDetalleVoucher.RemoveRange(objVoucher.ListaDetalleVoucher);
                objVoucher.CentroDeCosto = null;
                db.SaveChanges();
            }

            //intentar obtener numero de voucher si es que viene, revisar si esta disponible y agregarle el que corresponde
            if (NewNumeroVoucher == 0)
            {
                //revisar si viene uno desde el formulario
                if (numVoucherDesdeFormulario.HasValue)
                {
                    bool estaDisponible = ParseExtensions.EstaNumeroVoucherDisponible(numVoucherDesdeFormulario.Value, objCliente, db);
                    if (estaDisponible) //si esta disponible lo asigna
                    {
                        NewNumeroVoucher = numVoucherDesdeFormulario.Value;
                    }
                    else // si no esta disponible le asigna uno disponible
                    {
                        int? nullableProxVoucherNumber = ParseExtensions.ObtenerNumeroProximoVoucherINT(objCliente, db);
                        if (nullableProxVoucherNumber.HasValue)
                            NewNumeroVoucher = nullableProxVoucherNumber.Value;
                    }
                }
                else
                {
                    int? nullableProxVoucherNumber = ParseExtensions.ObtenerNumeroProximoVoucherINT(objCliente, db);
                    if (nullableProxVoucherNumber.HasValue)
                        NewNumeroVoucher = nullableProxVoucherNumber.Value;
                }
            }

            List<DetalleVoucherModel> ListaDetalle = new List<DetalleVoucherModel>();
            objVoucher.ClientesContablesModelID = objCliente.ClientesContablesModelID;
            string fecha = Request.Form.GetValues("fecha")[0];
            objVoucher.FechaEmision = ParseExtensions.ToDD_MM_AAAA(fecha);
            string glosa = Request.Form.GetValues("glosa")[0];
            objVoucher.Glosa = glosa;
            string tipo = Request.Form.GetValues("tipo")[0];
            objVoucher.Tipo = (TipoVoucher)Int32.Parse(tipo);
            string TipoOrigenVoucher = Request.Form.GetValues("TipoOrigen")[0];
            objVoucher.TipoOrigenVoucher = (TipoOrigen)Int32.Parse(TipoOrigenVoucher);

            if (NewNumeroVoucher > 0)
                objVoucher.NumeroVoucher = NewNumeroVoucher;

            if (Request.Form.GetValues("centrocost") != null && !string.IsNullOrEmpty(Request.Form.GetValues("centrocost")[0]))
            {
                int IDCentroCost = Int32.Parse(Request.Form.GetValues("centrocost")[0]);
                CentroCostoModel objCentroCosto = objCliente.ListCentroDeCostos.SingleOrDefault(r => r.CentroCostoModelID == IDCentroCost);
                objVoucher.CentroDeCosto = objCentroCosto;
            }
            else
            {
                objVoucher.CentroDeCosto = null;
            }
            for (int i = 0; i < Request.Form.GetValues("ctacont").Count(); i++)
            {
                DetalleVoucherModel objDetalle = new DetalleVoucherModel();
                string IDCuentaContable = Request.Form.GetValues("ctacont")[i];
                CuentaContableModel objCuenta = objCliente.CtaContable.Single(r => r.CuentaContableModelID.ToString() == IDCuentaContable);
                objDetalle.ObjCuentaContable = objCuenta;
                string debe = Request.Form.GetValues("debe")[i];
                objDetalle.MontoDebe = decimal.Parse(debe);
                string haber = Request.Form.GetValues("haber")[i];
                objDetalle.MontoHaber = decimal.Parse(haber);
                string GlosaDetalle = Request.Form.GetValues("glosaDetalle")[i];
                objDetalle.GlosaDetalle = GlosaDetalle;
                string fechadoc = Request.Form.GetValues("fechadoc")[i];
                objDetalle.FechaDoc = ParseExtensions.ToDD_MM_AAAA(fechadoc);

                //int ItemID = -1;
                //if (!string.IsNullOrEmpty(Request.Form.GetValues("idItem")[i]))
                //{
                //    ItemID = int.Parse(Request.Form.GetValues("idItem")[i]);
                //}
                //objDetalle.ItemModelID = ItemID;

                int CentroCostoID = -1;
                if (!string.IsNullOrEmpty(Request.Form.GetValues("centrocosto")[i]))
                {
                    CentroCostoID = int.Parse(Request.Form.GetValues("centrocosto")[i]);
                }
                objDetalle.CentroCostoID = CentroCostoID;


                ListaDetalle.Add(objDetalle);
            }
            if (Request.Form.GetValues("contracuenta") != null && !string.IsNullOrEmpty(Request.Form.GetValues("contracuenta")[0]))
            {
                decimal sumMontoDeb = ListaDetalle.Sum(r => r.MontoDebe);
                decimal sumMontoHaber = ListaDetalle.Sum(r => r.MontoHaber);
                decimal differenciaContraCuenta = sumMontoDeb - sumMontoHaber;
                if (differenciaContraCuenta != 0)
                {
                    if (differenciaContraCuenta < 0)
                    {
                        throw new NotImplementedException("No se puede efectuar contracuenta ya que monto haber es mayor que el debe");
                    }
                    DetalleVoucherModel objDetalle = new DetalleVoucherModel();
                    string IDCuentaContableContraCuenta = Request.Form.GetValues("contracuenta")[0];
                    CuentaContableModel objCuenta = objCliente.CtaContable.Single(r => r.CuentaContableModelID.ToString() == IDCuentaContableContraCuenta);
                    objDetalle.ObjCuentaContable = objCuenta;
                    objDetalle.MontoHaber = differenciaContraCuenta;
                    objDetalle.MontoDebe = 0;
                    objDetalle.GlosaDetalle = "Saldo Contracuenta";
                    objDetalle.FechaDoc = DateTime.Now;

                    ListaDetalle.Add(objDetalle);
                }
            }

            objVoucher.ListaDetalleVoucher = ListaDetalle;

            if (Request.Form.GetValues("editFlag") != null)
            {
                int idToEdit = ParseExtensions.ParseInt(Request.Form.GetValues("editFlag")[0]);
                if (idToEdit == 0)
                    throw new InvalidOperationException("Invalid edit on ID 0");
                objVoucher.VoucherModelID = idToEdit;
                db.DBVoucher.AddOrUpdate(objVoucher);
            }
            else
            {
                db.DBVoucher.Add(objVoucher);
            }
            db.SaveChanges();



            if (Session["sessionAuxiliares"] != null)
            {

                List<AuxiliaresModel> lstAuxs = (List<AuxiliaresModel>)Session["sessionAuxiliares"];
                foreach (AuxiliaresModel objAux in lstAuxs)
                {
                    int numLineaDetalle = objAux.LineaNumeroDetalle;


                    AuxiliaresModel brandNewAuxiliarObject = new AuxiliaresModel();
                    brandNewAuxiliarObject.ListaDetalleAuxiliares = new List<AuxiliaresDetalleModel>();

                    brandNewAuxiliarObject.DetalleVoucherModelID = objVoucher.ListaDetalleVoucher.ElementAt(numLineaDetalle - 1).DetalleVoucherModelID;
                    brandNewAuxiliarObject.LineaNumeroDetalle = objAux.LineaNumeroDetalle;
                    brandNewAuxiliarObject.objCtaContable = objVoucher.ListaDetalleVoucher.ElementAt(numLineaDetalle - 1).ObjCuentaContable;
                    brandNewAuxiliarObject.MontoTotal = objAux.MontoTotal;
                    objVoucher.ListaDetalleVoucher.ElementAt(numLineaDetalle - 1).Auxiliar = brandNewAuxiliarObject;
                    db.DBVoucher.AddOrUpdate(objVoucher);
                    db.SaveChanges();


                    if (objVoucher.VoucherModelID > 0)
                    {
                        VoucherModel VerificaExistencia = db.DBVoucher.SingleOrDefault(x => x.VoucherModelID == objVoucher.VoucherModelID && x.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                        if(VerificaExistencia != null) {
                            if(VerificaExistencia.DadoDeBaja == false) {  
                                List<LibrosContablesModel> SiexisteDestruyelo = db.DBLibrosContables.Where(x => x.VoucherModelID == objVoucher.VoucherModelID).ToList();
                                if (SiexisteDestruyelo.Count() > 0)
                                {
                                    db.DBLibrosContables.RemoveRange(SiexisteDestruyelo);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }

         
                    foreach (AuxiliaresDetalleModel auxSession in objAux.ListaDetalleAuxiliares)
                    {
                        LibrosContablesModel ObjLibroCompraOVenta = new LibrosContablesModel();
                        AuxiliaresDetalleModel brandNewAuxDetail = new AuxiliaresDetalleModel();
                        brandNewAuxDetail.AuxiliaresModelID = brandNewAuxiliarObject.AuxiliaresModelID;
                        string FechaContParaAux = Request.Form.GetValues("fecha")[0];
                        brandNewAuxDetail.Fecha = auxSession.Fecha;
                        brandNewAuxDetail.FechaContabilizacion = ParseExtensions.ToDD_MM_AAAA_Multi(FechaContParaAux); //Revisar si se está haciendo bien la conversion
                            //ACA DEBE IR A BUSCAR EL PRESTADOR DE AUXILIAR CON ESTA INFORMACION Y CREAR O EDITARLO SEGUN SEA NECESARIO
                            // string RUTPrestadorAUXDesdeSession = auxSession.Individuo.PrestadorRut;
                            // string NombrePrestadorAUXDesdeSession = auxSession.Individuo.PrestadorNombre;
                        string TipoPrestadorAUXDesdeSession = auxSession.Individuo2.tipoReceptor;
                        string RUTPrestadorAUXDesdeSession = auxSession.Individuo2.RUT;
                        string NombrePrestadorAUXDesdeSession = auxSession.Individuo2.RazonSocial;

                        //Estas 3 variables no se están insertando en la base de datos.

                        //brandNewAuxDetail.Individuo = AuxiliaresPrestadoresModel.CrearOActualizarPrestadorPorRut(RUTPrestadorAUXDesdeSession, NombrePrestadorAUXDesdeSession, objCliente, db);
                        brandNewAuxDetail.Individuo2 = QuickReceptorModel.CrearOActualizarPrestadorPorRut(RUTPrestadorAUXDesdeSession, NombrePrestadorAUXDesdeSession, objCliente, db, TipoPrestadorAUXDesdeSession);
                        brandNewAuxDetail.Folio = auxSession.Folio;
                        if(auxSession.FolioHasta > 0) { 
                            brandNewAuxDetail.FolioHasta = auxSession.FolioHasta;
                        }
                        if (brandNewAuxiliarObject.objCtaContable.TipoAuxiliarQueUtiliza == TipoAuxiliar.Honorarios)
                        {
                            brandNewAuxDetail.TipoDocumento = auxSession.TipoDocumento;
                            brandNewAuxDetail.ValorLiquido = auxSession.ValorLiquido;
                            brandNewAuxDetail.ValorRetencion = auxSession.ValorRetencion;
                            brandNewAuxDetail.MontoTotalLinea = auxSession.ValorLiquido;
                            brandNewAuxDetail.MontoBrutoLinea = auxSession.MontoBrutoLinea;
                        }
                        else if(brandNewAuxiliarObject.objCtaContable.TipoAuxiliarQueUtiliza == TipoAuxiliar.ProveedorDeudor)
                        {
                            brandNewAuxDetail.TipoDocumento = auxSession.TipoDocumento;
                            brandNewAuxDetail.MontoNetoLinea = auxSession.MontoNetoLinea;
                            brandNewAuxDetail.MontoExentoLinea = auxSession.MontoExentoLinea;
                            brandNewAuxDetail.MontoIVALinea = auxSession.MontoIVALinea;
                
                                if(auxSession.SeVaParaVenta == true && auxSession.SeVaParaCompra == false)
                                {   
                                    brandNewAuxDetail.SeVaParaVenta = auxSession.SeVaParaVenta;
                                    ObjLibroCompraOVenta.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                                    ObjLibroCompraOVenta.EsUnRegistroManual = true;
                                    ObjLibroCompraOVenta.TipoLibro = TipoCentralizacion.Venta;
                                    ObjLibroCompraOVenta.Folio = auxSession.Folio;
                                    ObjLibroCompraOVenta.individuo = brandNewAuxDetail.Individuo2;
                                    if (auxSession.FolioHasta > 0)
                                    {
                                        ObjLibroCompraOVenta.FolioHasta = auxSession.FolioHasta;
                                    }
                                    ObjLibroCompraOVenta.FechaContabilizacion = ParseExtensions.ToDD_MM_AAAA_Multi(FechaContParaAux);
                                    ObjLibroCompraOVenta.FechaDoc = auxSession.Fecha;
                                    ObjLibroCompraOVenta.MontoNeto = auxSession.MontoNetoLinea;
                                    ObjLibroCompraOVenta.MontoExento = auxSession.MontoExentoLinea;
                                    ObjLibroCompraOVenta.MontoIva = auxSession.MontoIVALinea;
                                    ObjLibroCompraOVenta.TipoDocumento = auxSession.TipoDocumento;

                                    ObjLibroCompraOVenta.VoucherModelID = objVoucher.VoucherModelID;
                                    ObjLibroCompraOVenta.HaSidoConvertidoAVoucher = true;

                                    db.DBLibrosContables.Add(ObjLibroCompraOVenta);
                                    db.SaveChanges();
                                }
                                if(auxSession.SeVaParaCompra == true && auxSession.SeVaParaVenta == false)
                                {
                                    brandNewAuxDetail.SeVaParaCompra = auxSession.SeVaParaCompra;
                                    ObjLibroCompraOVenta.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                                    ObjLibroCompraOVenta.EsUnRegistroManual = true;
                                    ObjLibroCompraOVenta.TipoLibro = TipoCentralizacion.Compra;
                                    ObjLibroCompraOVenta.Folio = auxSession.Folio;
                                    ObjLibroCompraOVenta.individuo = brandNewAuxDetail.Individuo2;
                                    if (auxSession.FolioHasta > 0)
                                    {
                                        ObjLibroCompraOVenta.FolioHasta = auxSession.FolioHasta;
                                    }
                                    ObjLibroCompraOVenta.FechaContabilizacion = ParseExtensions.ToDD_MM_AAAA_Multi(FechaContParaAux);
                                    ObjLibroCompraOVenta.FechaDoc = auxSession.Fecha;
                                    ObjLibroCompraOVenta.MontoNeto = auxSession.MontoNetoLinea;
                                    ObjLibroCompraOVenta.MontoExento = auxSession.MontoExentoLinea;
                                    ObjLibroCompraOVenta.MontoIva = auxSession.MontoIVALinea;
                                    ObjLibroCompraOVenta.TipoDocumento = auxSession.TipoDocumento;

                                    ObjLibroCompraOVenta.VoucherModelID = objVoucher.VoucherModelID;
                                    ObjLibroCompraOVenta.HaSidoConvertidoAVoucher = true;

                                    db.DBLibrosContables.Add(ObjLibroCompraOVenta);
                                    db.SaveChanges();
                            }   
                        }
                        if(brandNewAuxiliarObject.objCtaContable.TipoAuxiliarQueUtiliza != TipoAuxiliar.Honorarios)
                            brandNewAuxDetail.MontoTotalLinea = auxSession.MontoTotalLinea;

                        if(auxSession.SeVaParaVenta == true || auxSession.SeVaParaCompra == true) { 
                        ObjLibroCompraOVenta.MontoTotal = auxSession.MontoTotalLinea;
                        }
                        brandNewAuxiliarObject.ListaDetalleAuxiliares.Add(brandNewAuxDetail);
                    }

                    db.DBVoucher.AddOrUpdate(objVoucher);
                    db.SaveChanges();
                }
            }
            if (Request.Form.GetValues("editFlag") == null) { 
                TempData["Correcto"] = "Se ha ingresado el voucher satisfactoriamente.";
            }else if(Request.Form.GetValues("editFlag") != null)
            {
                TempData["Correcto"] = "Se ha editado el voucher satisfactoriamente.";
            }

            RedirectToRouteResult RutaADireccionar = RedirectToAction("","");

            if (Request.Form.GetValues("editFlag") == null) { 
                RutaADireccionar = RedirectToAction("IngresoVoucher", "Contabilidad");
            }else if (Request.Form.GetValues("editFlag") != null) { 
                RutaADireccionar = RedirectToAction("IngresoVoucher",new RouteValueDictionary(new{ Controller = "Contabilidad", Action = "IngresoVoucher", IDVoucher = IDVoucherAEditar }));
            }
            return RutaADireccionar;
       }

        [HttpGet]
        [Authorize]
        public ActionResult IngresoVoucherAuxiliarModel(int lineaDetalle, int valueCuentaContable, decimal montoTotal)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente, valueCuentaContable);

            //Obtiene el tipo de auxiliar que admite la cuenta contable
            CuentaContableModel singleCuenta = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == valueCuentaContable);
            if (singleCuenta != null)
            {
                ViewBag.TipoAuxiliarAUsar = singleCuenta.TipoAuxiliarQueUtiliza;
            }

            ViewBag.TiposDTE = ParseExtensions.ObtenerTipoDTEDropdownAsString();
            ViewBag.AUXlineaDetalle = lineaDetalle;
            ViewBag.AUXvalueCuentaContable = valueCuentaContable;
            ViewBag.AUXmontoTotal = montoTotal;

            //VE SI EXISTE UNA LISTA DE AUXILIARES
            if (Session["sessionAuxiliares"] != null)
            {
                //De existir ve si existe un auxiliar ya para esa linea
                List<AuxiliaresModel> lstAuxiliares = (List<AuxiliaresModel>)Session["sessionAuxiliares"];
                AuxiliaresModel objAuxiliar = new AuxiliaresModel();
                int index = lstAuxiliares.FindIndex(item => item.LineaNumeroDetalle == lineaDetalle);
                if (index >= 0)
                {

                    objAuxiliar = lstAuxiliares[index];
                    return PartialView(objAuxiliar);
                }
            }
            return PartialView(null);
        }

        [Authorize]
        public ActionResult ListaBoletas()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            IQueryable<AuxiliaresDetalleModel> ListaBoletas = (from AuxiliaresDetalle in db.DBAuxiliaresDetalle
                                                               join Auxiliares in db.DBAuxiliares on AuxiliaresDetalle.AuxiliaresModelID equals Auxiliares.AuxiliaresModelID
                                                               join DetalleVoucher in db.DBDetalleVoucher on Auxiliares.DetalleVoucherModelID equals DetalleVoucher.DetalleVoucherModelID
                                                               join Voucher in db.DBVoucher on DetalleVoucher.VoucherModelID equals Voucher.VoucherModelID
                                                               where Voucher.ClientesContablesModelID == objCliente.ClientesContablesModelID && Voucher.DadoDeBaja == false &&
                                                               AuxiliaresDetalle.FolioHasta > 0
                                
                                                               select AuxiliaresDetalle);

            return View(ListaBoletas.ToList());
        }

        [Authorize]
        public ActionResult ImportarBoletas()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            return View();
        }

        [Authorize]
        public ActionResult ProcesarExcelBoleta()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            //List<BoletasExcelModel> ReturnValues = BoletasExcelModel.DeExcelAObjetoBoleta(Excel);

            List<BoletasExcelModel> adsfg = new List<BoletasExcelModel>();
            var asdf = BoletasCoVModel.InsertBoletasCoV(objCliente, adsfg);

            //Procesar las boletas con su contra cuenta ¿Tiene contracuenta?
            //

            return View(asdf);
        }

        [Authorize]
        public ActionResult VistaPrevisaProcesoInsercionBoletas()
        {


            return View();
        }
        
        [Authorize]
        public JsonResult ObtenerPrestador(string TipoPrestador)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            StringBuilder optionSelect = new StringBuilder();// Con esto dibujaremos el Select donde se recibirán los datos.

            List<QuickReceptorModel> lstPrestador = new List<QuickReceptorModel>();
            
            bool ok = false;
            if (!string.IsNullOrWhiteSpace(TipoPrestador))
            {
                lstPrestador = db.Receptores.Where(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                        x.tipoReceptor == TipoPrestador &&
                                                        x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                        x.DadoDeBaja == false).ToList();

                optionSelect.Append("<option>Selecciona</option>");
             
                foreach (QuickReceptorModel Prestador in lstPrestador)
                {
                    optionSelect.Append("<option value=\"" + Prestador.QuickReceptorModelID + "\">" + Prestador.RazonSocial + "</option>");
                }
                ok = true;
            }
            return Json(new { ok, selectInput = optionSelect.ToString() },JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult ObtenerPrestadorRazonSocial(string TipoPrestador)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            StringBuilder optionSelect = new StringBuilder();// Con esto dibujaremos el Select donde se recibirán los datos.

            List<QuickReceptorModel> lstPrestador = new List<QuickReceptorModel>();

            bool ok = false;
            if (!string.IsNullOrWhiteSpace(TipoPrestador))
            {
                lstPrestador = db.Receptores.Where(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                        x.tipoReceptor == TipoPrestador &&
                                                        x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                        x.DadoDeBaja == false).ToList();

                optionSelect.Append("<option>Selecciona</option>");

                foreach (QuickReceptorModel Prestador in lstPrestador)
                {
                    optionSelect.Append("<option value=\"" + Prestador.RazonSocial + "\">" + Prestador.RazonSocial + "</option>");
                }
                ok = true;
            }
            return Json(new { ok, selectInput = optionSelect.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult ObtenerRutPrestador(int IDPrestador)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            bool ok = false;

            QuickReceptorModel PrestadorSeleccionado = db.Receptores.SingleOrDefault(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                                                          x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                                          x.QuickReceptorModelID == IDPrestador);
            if (PrestadorSeleccionado != null)
                ok = true;
            else
                return Json(ok = false, JsonRequestBehavior.AllowGet);
            

            return Json(new {ok, RutPrestador = PrestadorSeleccionado.RUT }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        [Authorize]
        public ActionResult DisplayVoucherAuxiliarModel(int idAuxiliar)
       {

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<AuxiliaresDetalleModel> auxDetalles = db.DBAuxiliaresDetalle.Where(x => x.AuxiliaresModelID == idAuxiliar).ToList();
            AuxiliaresModel aux = db.DBAuxiliares.SingleOrDefault(x => x.AuxiliaresModelID == idAuxiliar);
            DetalleVoucherModel ccm = db.DBDetalleVoucher.SingleOrDefault(x => x.DetalleVoucherModelID == aux.DetalleVoucherModelID);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente, ccm.ObjCuentaContable.CuentaContableModelID);

            //Obtiene el tipo de auxiliar que admite la cuenta contable
            CuentaContableModel singleCuenta = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == ccm.ObjCuentaContable.CuentaContableModelID);
            if (singleCuenta != null)
            {
                ViewBag.TipoAuxiliarAUsar = singleCuenta.TipoAuxiliarQueUtiliza;
            }

            ViewBag.TiposDTE = ParseExtensions.ObtenerTipoDTEDropdownAsString();
            ViewBag.AUXlineaDetalle = aux.LineaNumeroDetalle;
            ViewBag.AUXvalueCuentaContable = "[" + ccm.ObjCuentaContable.nombre + "] " + ccm.ObjCuentaContable.nombre;
            ViewBag.AUXmontoTotal = ccm.MontoDebe + ccm.MontoHaber;
            ViewBag.NOTOPERABLE = null;
            //VE SI EXISTE UNA LISTA DE AUXILIARES
            //  if (Session["sessionAuxiliares"] != null)

            //De existir ve si existe un auxiliar ya para esa linea
            //List<AuxiliaresModel> lstAuxiliares = auxDetalles;  // (List<AuxiliaresModel>)Session["sessionAuxiliares"];
            // AuxiliaresModel objAuxiliar = new AuxiliaresModel();
            // int index = lstAuxiliares.FindIndex(item => item.LineaNumeroDetalle == lineaDetalle);
            if (aux.ListaDetalleAuxiliares.Count() >= 0)
            {

                // objAuxiliar = lstAuxiliares[index];
                //  aux.ListaDetalleAuxiliares = auxDetalles;
                return PartialView(aux);
            }
            //}

            return PartialView();
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult ObtenerDetalleVoucher(int idVoucher)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente == null)
            {
                return Json(new
                {
                    ok = false
                }, JsonRequestBehavior.AllowGet);
            }

            //VoucherModel objVoucher = db.DBVoucher.Single(r => r.VoucherModelID == idVoucher);

            //objCliente = db.DBClientesContables.SingleOrDefault(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.ClientesContablesModelID == objVoucher.ClientesContablesModelID);

            if (objCliente != null)
            {
                //IList<DetalleVoucherModel> lstDetalle = db.DBDetalleVoucher.Including(cta => cta.ObjCuentaContable).Where(b => b.VoucherModel.VoucherModelID == idVoucher).ToList();
                VoucherModel Voucher = objCliente.ListVoucher.SingleOrDefault(v => v.VoucherModelID == idVoucher);
              
              

                return Json(new
                {
                    ok = true,
                    rutEmpresa = objCliente.RUTEmpresa,
                    razonsocial = objCliente.RazonSocial,
                    fecha = ParseExtensions.ToDD_MM_AAAA(Voucher.FechaEmision),
                    glosa = Voucher.Glosa,
                    centroCosto = (Voucher.CentroDeCosto == null ? "" : Voucher.CentroDeCosto.Nombre),
                    detalleVoucher = Voucher.ListaDetalleVoucher,
                    totaldeb = Voucher.ListaDetalleVoucher.Sum(d => d.MontoDebe),
                    totalhab = Voucher.ListaDetalleVoucher.Sum(h => h.MontoHaber),
                    numeroVoucher = Voucher.NumeroVoucher
                }, JsonRequestBehavior.AllowGet);


            }

            return Json(new
            {
                ok = false
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ModuloHandler]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult BorrarBajaVoucher(int idVoucher, AccionMonitoreo accion = AccionMonitoreo.Edicion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            //QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente != null)
            {
                VoucherModel objVoucher = db.DBVoucher.SingleOrDefault(x => x.VoucherModelID == idVoucher && x.ClientesContablesModelID == objCliente.ClientesContablesModelID);

                if (objVoucher != null)
                {
                    objVoucher.DadoDeBaja = true;
                    db.SaveChanges();
                    LibrosContablesModel libro = db.DBLibrosContables.SingleOrDefault(x => x.VoucherModelID == objVoucher.VoucherModelID);
                    if (libro != null)
                    {

                        libro.estado = false;
                        db.SaveChanges();

                    }
                    var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri)
                    {
                        Path = Url.Action("ListaVoucher", "Contabilidad"),
                        Query = null,
                    };

                    Uri uri = urlBuilder.Uri;
                    string url = urlBuilder.ToString();

                    return Json(new
                    {
                        ok = true,
                        ReturnURL = url,
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    ok = false
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult IngresarAuxiliarTemp()
        {
            List<AuxiliaresModel> lstAuxiliares;
            AuxiliaresModel objAuxiliar = null;

            if (Request.Form.GetValues("AUXfecha") == null || Request.Form.GetValues("AUXfecha").Count() == 0)
            {
                return Json(new { ok = false });
            }

            //NEXT-TO-DO: Retener indice de carga posible de un auxiliar previo y guardar directa o apropiadamente en sessions de AUXILIARES
            int indiceTemp = -1;

            
            //linea de detalle a la cual el auxiliar hace referencia
            string auxItemTxtSTR = Request.Form.GetValues("AUXitem")[0];
            //Cuenta contable a la cual el auxiliar hace referencia
            string codCuentaSTR = Request.Form.GetValues("AUXcuenta")[0];
            //Monto total de la linea auxiliar de este detalle
            string totalAuxiliarSTR = Request.Form.GetValues("AUXvaloritem")[0];

            string SeVaACompra = "";
            if(Request.Form.GetValues("ContaLibroCompra") != null) { 
                SeVaACompra = Request.Form.Get("ContaLibroCompra");
            }
            string SeVaAVenta = "";
            if(Request.Form.GetValues("ContaLibroVenta") != null) { 
                SeVaAVenta = Request.Form.Get("ContaLibroVenta");
            }


            int cod_cuenta_contable = ParseExtensions.ParseInt(codCuentaSTR);
            int aux_lineaNumeroDetalle = ParseExtensions.ParseInt(auxItemTxtSTR);
            decimal totalAuxiliar = ParseExtensions.ParseDecimal(totalAuxiliarSTR);

            //VE SI EXISTE UNA LISTA DE AUXILIARES
            if (Session["sessionAuxiliares"] == null)
                lstAuxiliares = new List<AuxiliaresModel>(); //si no existe la crea
            else
            {
                //De existir ve si existe un auxiliar ya para esa linea
                lstAuxiliares = (List<AuxiliaresModel>)Session["sessionAuxiliares"];
                int index = lstAuxiliares.FindIndex(item => item.LineaNumeroDetalle == aux_lineaNumeroDetalle);
                if (index >= 0)
                {
                    objAuxiliar = lstAuxiliares[index];
                    indiceTemp = index;
                }
            }

            if (objAuxiliar == null)
            {
                objAuxiliar = new AuxiliaresModel();
                objAuxiliar.ListaDetalleAuxiliares = new List<AuxiliaresDetalleModel>();
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            //QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            if (objCliente == null || objCliente.CtaContable == null)
            {
                return Json(new { ok = false });
            }

            objAuxiliar.objCtaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == cod_cuenta_contable);
            if (objAuxiliar.objCtaContable == null)
                return Json(new { ok = false });
            objAuxiliar.LineaNumeroDetalle = aux_lineaNumeroDetalle;
            objAuxiliar.MontoTotal = totalAuxiliar;

            TipoAuxiliar TipoDeAux = objAuxiliar.objCtaContable.TipoAuxiliarQueUtiliza;
            objAuxiliar.Tipo = TipoDeAux;

            objAuxiliar.ListaDetalleAuxiliares.Clear();
            if (Request.Form.GetValues("AUXfecha") != null)
            {
                for (int i = 0; i < Request.Form.GetValues("AUXfecha").Length; i++)
                {
                    AuxiliaresDetalleModel objAuxiliarDetalle = new AuxiliaresDetalleModel();

                    objAuxiliarDetalle.Fecha = ParseExtensions.ToDD_MM_AAAA(Request.Form.GetValues("AUXFecha")[i]);
                    objAuxiliarDetalle.Folio = ParseExtensions.ParseInt(Request.Form.GetValues("AuxFolio")[i]);
                    
                    if(Request.Form.GetValues("FolioHasta") != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Request.Form.GetValues("FolioHasta")[i])) { 
                            objAuxiliarDetalle.FolioHasta = ParseExtensions.ParseInt(Request.Form.GetValues("FolioHasta")[i]);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(SeVaACompra) && SeVaACompra == "Compra")
                    {
                        objAuxiliarDetalle.SeVaParaCompra = true;
                    }

                    if (!string.IsNullOrWhiteSpace(SeVaAVenta) && SeVaAVenta == "Venta")
                    {
                        objAuxiliarDetalle.SeVaParaVenta = true;
                    }

                    string TipoPrestadorAuxiliar = Request.Form.GetValues("tipoIndividuo")[i];
                    string NombrePrestadorAuxiliar = Request.Form.GetValues("AUXrazoncta")[i];
                    string RutPrestadorAuxiliar = Request.Form.GetValues("AUXrut")[i];

                    //aca debe crear un dummy que sera utilizado en la creacion de verdad para CREAR o ACTUALIZAR el prestador
                    //AuxiliaresPrestadoresModel objPrestadorTEMP = new AuxiliaresPrestadoresModel();
                    //objPrestadorTEMP.PrestadorNombre = NombrePrestadorAuxiliar;
                    //objPrestadorTEMP.PrestadorRut = RutPrestadorAuxiliar;
                    //objAuxiliarDetalle.Individuo = AuxiliaresPrestadoresModel.CrearOActualizarPrestadorPorRut(RutPrestadorAuxiliar, NombrePrestadorAuxiliar, objCliente, db);//objPrestadorTEMP; //AuxiliaresPrestadoresModel.CrearOActualizarPrestadorPorRut(RutPrestadorAuxiliar, NombrePrestadorAuxiliar, objCliente, null);




                    objAuxiliarDetalle.Individuo2 = QuickReceptorModel.CrearOActualizarPrestadorPorRut(RutPrestadorAuxiliar, NombrePrestadorAuxiliar, objCliente, db, TipoPrestadorAuxiliar);
                    if (TipoDeAux == TipoAuxiliar.Honorarios)
                    {
                        //Modificar aquí
                        objAuxiliarDetalle.TipoDocumento = TipoDte.FacturaElectronica;
                        objAuxiliarDetalle.MontoBrutoLinea = ParseExtensions.ParseDecimal(Request.Form.GetValues("AuxTotal")[i]);
                        objAuxiliarDetalle.ValorLiquido = ParseExtensions.ParseDecimal(Request.Form.GetValues("AuxValorLiquido")[i]);
                        objAuxiliarDetalle.ValorRetencion = ParseExtensions.ParseDecimal(Request.Form.GetValues("AuxValorRetencion")[i]);
                        objAuxiliarDetalle.MontoTotalLinea = ParseExtensions.ParseDecimal(Request.Form.GetValues("AuxValorLiquido")[i]);
                    }
                    else if (TipoDeAux == TipoAuxiliar.ProveedorDeudor)
                    {
                        objAuxiliarDetalle.TipoDocumento = (TipoDte)ParseExtensions.ParseInt(Request.Form.GetValues("AuxTipoDTE")[i]);
                        objAuxiliarDetalle.MontoNetoLinea = ParseExtensions.ParseDecimal(Request.Form.GetValues("AUXNeto")[i]);
                        objAuxiliarDetalle.MontoExentoLinea = ParseExtensions.ParseDecimal(Request.Form.GetValues("AUXExento")[i]);
                        objAuxiliarDetalle.MontoIVALinea = ParseExtensions.ParseDecimal(Request.Form.GetValues("AUXIva")[i]);
                        objAuxiliarDetalle.MontoTotalLinea = ParseExtensions.ParseDecimal(Request.Form.GetValues("AuxTotal")[i]);
                    }
                    else if(TipoDeAux == TipoAuxiliar.Remuneracion)
                    {
                        objAuxiliarDetalle.MontoTotalLinea = ParseExtensions.ParseDecimal(Request.Form.GetValues("AuxTotal")[i]);
                    }

                    objAuxiliar.ListaDetalleAuxiliares.Add(objAuxiliarDetalle);
                }
            }

            //Salio de una variable de session presente
            if (indiceTemp != -1)
            {
                lstAuxiliares[indiceTemp] = objAuxiliar;
            }
            else
            {
                //(List<AuxiliaresModel>)Session["sessionAuxiliares"];
                lstAuxiliares.Add(objAuxiliar);
                Session["sessionAuxiliares"] = lstAuxiliares;
            }
            TempData["Correcto"] = "Se ha ingresado auxiliar(es) satisfactoriamente";

            return Json(new { ok = true });
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult BalanceGeneral()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBalanceGeneralFechaInicio"] = null;
            Session["strBalanceGeneralFechaFin"] = null;
            Session["strBalanceGeneralAnio"] = null;
            Session["strBalanceGeneralMes"] = null;

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            ViewBag.ObjClienteContable = objCliente;

            var lstCentroDeCostos = objCliente.ListCentroDeCostos.ToList();
            ViewBag.lstCentroDeCostos = lstCentroDeCostos;

            List<string[]> returnValue = VoucherModel.GetBalanceGeneral(lstVoucherCliente, lstCuentasContablesClientes,0);
            Session["BalanceGeneralF"] = returnValue;
            return View(returnValue);
        }

        [ModuloHandler]
        [Authorize]
        public ActionResult BalanceGeneralPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "", int CentroDeCostoID = 0)
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();
            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBalanceGeneralFechaInicio"] = null;
            Session["strBalanceGeneralFechaFin"] = null;
            Session["strBalanceGeneralAnio"] = null;
            Session["strBalanceGeneralMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = new List<CuentaContableModel>(); 
  
            List<VoucherModel> lstVoucherModel;

            if(CentroDeCostoID > 0)
            {
                string NombreCentroCosto = CentroCostoModel.GetNombreCentroDeCosto(CentroDeCostoID, objCliente);

                ViewBag.NombreCC = NombreCentroCosto;
            }

            lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strBalanceGeneralFechaInicio"] = FechaInicio;
                Session["strBalanceGeneralFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strBalanceGeneralAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strBalanceGeneralMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetBalanceGeneral(lstVoucherModel, lstCuentasContablesClientes, CentroDeCostoID);
            Session["BalanceGeneralF"] = returnValue;
            return PartialView(returnValue);
        }

        [ModuloHandler]
        [Authorize]
        public ActionResult GetExcelBalanceGeneral()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["BalanceGeneralF"] != null)
            {
                List<string[]> cachedBalanceGeneral = Session["BalanceGeneralF"] as List<string[]>;
                if (cachedBalanceGeneral != null)
                {
                    for (int i = 0; i < cachedBalanceGeneral.Count; i++)
                    {
                        cachedBalanceGeneral[i] = cachedBalanceGeneral[i].Select(r => r.Replace(".", "")).ToArray();
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBalanceGeneralFechaInicio"] as string, Session["strBalanceGeneralFechaFin"] as string, Session["strBalanceGeneralAnio"] as int?, Session["strBalanceGeneralMes"] as int?, "BALANCE GENERAL");
                    var cachedStream = VoucherModel.GetExcelResultBalanceGeneral(cachedBalanceGeneral, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "BalanceGeneral" + Guid.NewGuid() + ".xlsx");
                }
            }

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBalanceGeneralFechaInicio"] as string, Session["strBalanceGeneralFechaFin"] as string, Session["strBalanceGeneralAnio"] as int?, Session["strBalanceGeneralMes"] as int?, "BALANCE GENERAL");

            var ExcelStream = VoucherModel.GetExcelResultBalanceGeneral(lstVoucherCliente, objCliente, lstCuentasContablesClientes, true, tituloDocumento);
            return File(ExcelStream, "application/vnd.ms-excel", "BalanceGeneral" + Guid.NewGuid() + ".xlsx");
        }


        //[Authorize]
        //public ActionResult DetalleCtaConsultada(int IdCtaContable)
        //{
        //    string UserID = User.Identity.GetUserId();
        //    FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        //    ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

        //    int ExisteCuenta = objCliente.CtaContable.Where(cta => cta.CuentaContableModelID == IdCtaContable).Count();

        //    if(ExisteCuenta == 1)
        //    {
        //        IQueryable<DetalleCtaConsultadaViewModel> DetalleObtenido = DetalleCtaConsultadaViewModel.QueryDetalleCuenta(IdCtaContable, db);
        //    }



        //    return View();
        //}

        // BA = Balance
        [Authorize]
        [ModuloHandler]
        public ActionResult BAActivoCirculante()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetBActivoCirculante(lstVoucherCliente, lstCuentasContablesClientes);

            Session["BAActivoCirculante"] = returnValue;
            return View(returnValue);
        }
        [Authorize]
        [ModuloHandler]
        public ActionResult BAActivoCirculantePartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBAActivoCirculanteFechaInicio"] = null;
            Session["strBAActivoCirculanteFechaFin"] = null;
            Session["strBAActivoCirculanteAnio"] = null;
            Session["strBAActivoCirculanteMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strBAActivoCirculanteFechaInicio"] = FechaInicio;
                Session["strBAActivoCirculanteFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strBAActivoCirculanteAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strBAActivoCirculanteMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetBActivoCirculante(lstVoucherModel, lstCuentasContablesClientes);
            Session["BAActivoCirculante"] = returnValue;

            return PartialView(returnValue);
        }

        
        [Authorize]
        public ActionResult GetExcelBalanceActivoFijoCirculante()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["BAActivoCirculante"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["BAActivoCirculante"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i].DefaultIfEmpty() == null)
                        {
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                        }
                    }// Esto es lo que debes verificiar ahora.

                }
                tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBAActivoCirculanteFechaInicio"] as string, Session["strBAActivoCirculanteFechaFin"] as string, Session["strBAActivoCirculanteAnio"] as int?, Session["strBAActivoCirculanteMes"] as int?, "ACTIVO CIRCULANTE");
                var cachedStream = VoucherModel.GetExcelInfomesBalance(cachedEstadoResultado, objCliente, true, tituloDocumento);
                return File(cachedStream, "application/vnd.ms-excel", "Informe Activo Circulante" + Guid.NewGuid() + ".xlsx");
            }

            return View();

        }
 
       
        [Authorize]
        [ModuloHandler]
        public ActionResult BAActivoFijo()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetBActivoFijo(lstVoucherCliente, lstCuentasContablesClientes);

            Session["BAActivoFijo"] = returnValue;
            return View(returnValue);
        }

        [Authorize]
        public ActionResult BAActivoFijoPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBAActivoFijoFechaInicio"] = null;
            Session["strBAActivoFijoFechaFin"] = null;
            Session["strBAActivoFijoAnio"] = null;
            Session["strBAActivoFijoMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strBAActivoFijoFechaInicio"] = FechaInicio;
                Session["strBAActivoFijoFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strBAActivoFijoAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strBAActivoFijoMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetBActivoFijo(lstVoucherModel, lstCuentasContablesClientes);
            Session["BAActivoFijo"] = returnValue;

            return PartialView(returnValue);
        }
        
        [Authorize]
        public ActionResult GetExcelBalanceActivoFijo()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["BAActivoFijo"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["BAActivoFijo"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                         if(cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBAActivoFijoechaInicio"] as string, Session["strBAActivoFijoFechaFin"] as string, Session["strBAActivoFijoAnio"] as int?, Session["strBAActivoFijoMes"] as int?, "BALANCE ACTIVO FIJO");
                    var cachedStream = VoucherModel.GetExcelInfomesBalance(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Activo Fijo" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }

        [Authorize]
        public ActionResult BATotalActivos()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetTotalActivo(lstVoucherCliente, lstCuentasContablesClientes);

            Session["BAActivoFijo"] = returnValue;
            return View(returnValue);
        }
        [Authorize]
        public ActionResult BATotalActivosPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBAActivoFijoFechaInicio"] = null;
            Session["strBAActivoFijoFechaFin"] = null;
            Session["strBAActivoFijoAnio"] = null;
            Session["strBAActivoFijoMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strBAActivoFijoFechaInicio"] = FechaInicio;
                Session["strBAActivoFijoFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strBAActivoFijoAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strBAActivoFijoMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetTotalActivo(lstVoucherModel, lstCuentasContablesClientes);
            Session["BAActivoFijo"] = returnValue;

            return PartialView(returnValue);
        }
        [Authorize]
        public ActionResult GetExcelBATotalActivos()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["BAActivoFijo"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["BAActivoFijo"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if(cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBAActivoFijoechaInicio"] as string, Session["strBAActivoFijoFechaFin"] as string, Session["strBAActivoFijoAnio"] as int?, Session["strBAActivoFijoMes"] as int?, "TOTAL ACTIVOS");
                    var cachedStream = VoucherModel.GetExcelInfomesBalance(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Total Activos" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }


        [Authorize]
        [ModuloHandler]
        public ActionResult BAPasivoFijoCirculante()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetBPasivoCirculante(lstVoucherCliente, lstCuentasContablesClientes);

            Session["BAPasivoFijoCirculante"] = returnValue;
            return View(returnValue);
        }

        [Authorize]
        public ActionResult BAPasivoFijoCirculantePartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBAPasivoFijoCirculanteFechaInicio"] = null;
            Session["strBAPasivoFijoCirculanteFechaFin"] = null;
            Session["strBAPasivoFijoCirculanteAnio"] = null;
            Session["strBAPasivoFijoCirculanteMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strBAPasivoFijoCirculanteFechaInicio"] = FechaInicio;
                Session["strBAPasivoFijoCirculanteFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strBAPasivoFijoCirculanteAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strBAPasivoFijoCirculanteMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetBPasivoCirculante(lstVoucherModel, lstCuentasContablesClientes);
            Session["BAPasivoFijoCirculante"] = returnValue;

            return PartialView(returnValue);
        }

        [Authorize]
        public ActionResult GetExcelBalancePasivoFijoCirculante()
        {
            //informes Anual.
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["BAPasivoFijoCirculante"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["BAPasivoFijoCirculante"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if(cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBAPasivoFijoCirculanteFechaInicio"] as string, Session["strBAPasivoFijoCirculanteFechaFin"] as string, Session["strBAPasivoFijoCirculanteAnio"] as int?, Session["strBAPasivoFijoCirculanteMes"] as int?, "BALANCE PASIVO FIJO CIRCULANTE");
                    var cachedStream = VoucherModel.GetExcelInfomesBalance(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Pasivo Circulante" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }




        [Authorize]
        [ModuloHandler]
        public ActionResult BAPasivoFijoNoCorriente()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetBPasivoNoCorriente(lstVoucherCliente, lstCuentasContablesClientes);

            Session["BAPasivoFijoNoCorriente"] = returnValue;
            return View(returnValue);
        }

        [Authorize]
        public ActionResult BAPasivoFijoNoCorrientePartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBAPasivoFijoNoCorrienteFechaInicio"] = null;
            Session["strBAPasivoFijoNoCorrienteFechaFin"] = null;
            Session["strBAPasivoFijoNoCorrienteAnio"] = null;
            Session["strBAPasivoFijoNoCorrienteMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strBAPasivoFijoNoCorrienteFechaInicio"] = FechaInicio;
                Session["strBAPasivoFijoNoCorrienteFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strBAPasivoFijoNoCorrienteAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strBAPasivoFijoNoCorrienteMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetBPasivoNoCorriente(lstVoucherModel, lstCuentasContablesClientes);
            Session["BAPasivoFijoNoCorriente"] = returnValue;

            return PartialView(returnValue);
        }

        [Authorize]
        public ActionResult GetExcelBaPasivoFijoNoCorriente()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["BAPasivoFijoNoCorriente"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["BAPasivoFijoNoCorriente"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if(cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBAPasivoFijoNoCorrienteFechaInicio"] as string, Session["strBAPasivoFijoNoCorrienteFechaFin"] as string, Session["strBAPasivoFijoNoCorrienteAnio"] as int?, Session["strBAPasivoFijoNoCorrienteMes"] as int?, "BALANCE PASIVO FIJO NO CORRIENTE");
                    var cachedStream = VoucherModel.GetExcelInfomesBalance(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Pasivo No Corriente" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult BAPasivoAndPatrimonioNeto()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetBTotalPasivoAndPatrimonioNeto(lstVoucherCliente, lstCuentasContablesClientes);

            Session["BAPasivoAndPatrimonioNeto"] = returnValue;
            return View(returnValue);
        }


        [Authorize]
        public ActionResult BAPasivoAndPatrimonioNetoPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBAPasivoAndPatrimonioNetoFechaInicio"] = null;
            Session["strBAPasivoAndPatrimonioNetoFechaFin"] = null;
            Session["strBAPasivoAndPatrimonioNetoAnio"] = null;
            Session["strBAPasivoAndPatrimonioNetoMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strBAPasivoAndPatrimonioNetoFechaInicio"] = FechaInicio;
                Session["strBAPasivoAndPatrimonioNetoFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strBAPasivoAndPatrimonioNetoAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strBAPasivoAndPatrimonioNetoMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetBTotalPasivoAndPatrimonioNeto(lstVoucherModel, lstCuentasContablesClientes);
            Session["BAPasivoAndPatrimonioNeto"] = returnValue;

            return PartialView(returnValue);
        }

        [Authorize]
        public ActionResult GetExcelBaPasivoAndPatrimonioNeto()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["BAPasivoAndPatrimonioNeto"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["BAPasivoAndPatrimonioNeto"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if(cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBAPasivoAndPatrimonioNetoFechaInicio"] as string, Session["strBAPasivoAndPatrimonioNetoFechaFin"] as string, Session["strBAPasivoAndPatrimonioNetoAnio"] as int?, Session["strBAPasivoAndPatrimonioNetoMes"] as int?, "BALANCE PASIVO + PATRIMONIO NETO");
                    var cachedStream = VoucherModel.GetExcelInfomesBalance(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Patrimonio Neto" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }

        [Authorize]
        public ActionResult BATotalPasivos()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetTotalPasivos(lstVoucherCliente, lstCuentasContablesClientes);

            Session["BATotalPasivos"] = returnValue;
            return View(returnValue);
        }

        [Authorize]
        public ActionResult BATotalPasivosPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strBATotalPasivosFechaInicio"] = null;
            Session["strBATotalPasivosFechaFin"] = null;
            Session["strBATotalPasivosAnio"] = null;
            Session["strBATotalPasivosMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strBATotalPasivosFechaInicio"] = FechaInicio;
                Session["strBATotalPasivosFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strBATotalPasivosAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strBATotalPasivosMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetTotalPasivos(lstVoucherModel, lstCuentasContablesClientes);
            Session["BATotalPasivos"] = returnValue;

            return PartialView(returnValue);
        }

        [Authorize]
        public ActionResult GetExcelBATotalPasivos()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["BATotalPasivos"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["BATotalPasivos"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if(cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBATotalPasivosFechaInicio"] as string, Session["strBATotalPasivosFechaFin"] as string, Session["strBATotalPasivosAnio"] as int?, Session["strBATotalPasivosMes"] as int?, "TOTAL PASIVOS");
                    var cachedStream = VoucherModel.GetExcelInfomesBalance(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Total Pasivos" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }


        [Authorize]
       // [ModuloHandler]
        public ActionResult LibroMayor()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);

            List<VoucherModel> lstVoucherModel = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false).ToList();

            //CuentaContableModel singleCuentaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID.ToString() == IDCuentaContable);
            string anno = DateTime.Now.ToString("yyyy");

            List<string[]> returnValue = VoucherModel.GetLibroMayor(lstVoucherModel, db, null, null, null, anno, null);
            Session["LibroMayorF"] = returnValue;
            return View(returnValue);
        }

        [Authorize]
      //  [ModuloHandler]
        public ActionResult LibroMayorPartial(string IDCuentaContable, string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "", string Folio = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();
            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherModel = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false).ToList();
            /*
            if (string.IsNullOrWhiteSpace(Anio) == false)
            {
                // lstVoucherModel = lstVoucherModel.Where(r => r.FechaEmision.Year == ParseExtensions.ParseInt(Anio)).ToList();
                lstVoucherModel = lstVoucherModel.Where(r => r.ListaDetalleVoucher.Where( f => f.FechaDoc.Year == ParseExtensions.ParseInt(Anio) ).ToList();
            }
            if (string.IsNullOrWhiteSpace(Mes) == false)
            {
                lstVoucherModel = lstVoucherModel.Where(r => r.FechaEmision.Month == ParseExtensions.ParseInt(Mes)).ToList();

            }*/

            CuentaContableModel singleCuentaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID.ToString() == IDCuentaContable);

            List<string[]> returnValue = new List<string[]>();
            if (ConversionFechaFinExitosa && ConversionFechaInicioExitosa)
            {
                returnValue = VoucherModel.GetLibroMayor(lstVoucherModel, db, singleCuentaContable ,dtFechaInicio, dtFechaFin, Anio, Mes, Folio);
                Session["LibroMayorF"] = returnValue;
            }
            else
            {
                returnValue = VoucherModel.GetLibroMayor(lstVoucherModel, db, singleCuentaContable, null, null, Anio, Mes, Folio);
                Session["LibroMayorF"] = returnValue;
            }
            return PartialView(returnValue);
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult GetExcelLibroMayor()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["LibroMayorF"] != null)
            {
                List<string[]> cachedLibroMayor = Session["LibroMayorF"] as List<string[]>;
                if (cachedLibroMayor != null)
                {
                    for (int i = 0; i < cachedLibroMayor.Count; i++)
                    {
                        cachedLibroMayor[i] = cachedLibroMayor[i].Select(r => r.Replace(".", "")).ToArray();
                        
                    }

                    string textAnioLibroMayor = "";
                    string textMesLibroMayor = "";
                    string textFechaInicio = "";
                    string textFechaFin = "";
                    string textCtaCont = "";

                    textAnioLibroMayor = Request.Form.GetValues("AnioLibroMayor") != null ? Request.Form.GetValues("AnioLibroMayor")[0] : string.Empty;
                    textMesLibroMayor = Request.Form.GetValues("MesLibroMayor") != null ? Request.Form.GetValues("MesLibroMayor")[0] : string.Empty;
                    textFechaInicio = Request.Form.GetValues("fechainicio") != null ? Request.Form.GetValues("fechainicio")[0] : string.Empty;
                    textFechaFin = Request.Form.GetValues("fechafin") != null ? Request.Form.GetValues("fechafin")[0] : string.Empty;
                    textCtaCont = Request.Form.GetValues("ctacont") != null ? Request.Form.GetValues("ctacont")[0] : string.Empty;

                    if (string.IsNullOrWhiteSpace(textCtaCont) == false)
                    {
                        CuentaContableModel objCtaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == ParseExtensions.ParseInt(textCtaCont));
                        if (objCtaContable != null)
                        {
                            textCtaCont = objCtaContable.CodInterno + " " + objCtaContable.nombre;
                        }
                        else
                        {
                            textCtaCont = string.Empty;
                        }
                    }


                    //tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBalanceGeneralFechaInicio"] as string, Session["strBalanceGeneralFechaFin"] as string, Session["strBalanceGeneralAnio"] as int?, Session["strBalanceGeneralMes"] as int?, "LIBRO MAYOR");
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(textFechaInicio, textFechaFin, ParseExtensions.ParseInt(textAnioLibroMayor), ParseExtensions.ParseInt(textMesLibroMayor), "LIBRO MAYOR");

                    var cachedStream = VoucherModel.GetExcelLibroMayor(cachedLibroMayor, objCliente, true, tituloDocumento, textCtaCont);
                    return File(cachedStream, "application/vnd.ms-excel", "LibroMayor" + Guid.NewGuid() + ".xlsx");
                }
            }
            return null;
        }

        [Authorize]
        public ActionResult LibroMayorTwo(FiltrosParaLibros flibros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);
            ViewBag.ObjClienteContable = objCliente;

            PaginadorModel ReturnValues = new PaginadorModel();

            bool Filtro = false;






            if (flibros.Anio > 0 || !string.IsNullOrWhiteSpace(flibros.FechaInicio) && !string.IsNullOrWhiteSpace(flibros.FechaFin))
            {
                flibros.Filtro = true;
            }
            else if (flibros.Filtro == false)
            {
                ViewBag.AnioSinFiltro = "Registros del año" + " " + DateTime.Now.Year + " " + "Y Mes: " + ParseExtensions.obtenerNombreMes(DateTime.Now.Month);
            }

            //Levar esta conversión al modelo y luego pasarle las fechas en formato String.

            if (!string.IsNullOrWhiteSpace(flibros.FechaInicio) && !string.IsNullOrWhiteSpace(flibros.FechaFin))
            {
                ReturnValues = VoucherModel.GetLibroMayorTwo(flibros, objCliente, db);
                Session["LibroMayorTwo"] = ReturnValues.ResultStringArray;
            }
            else
            {
                ReturnValues = VoucherModel.GetLibroMayorTwo(flibros, objCliente, db);
                Session["LibroMayorTwo"] = ReturnValues.ResultStringArray;
            }

            if (flibros.Anio <= 0)
                flibros.Anio = DateTime.Now.Year;

            var FechasExcel = new SessionParaExcel() 
            { 
                Anio = flibros.Anio.ToString(),
                Mes = flibros.Mes.ToString(),
                FechaInicio = flibros.FechaInicio,
                FechaFin = flibros.FechaFin,
                CtaContId = flibros.CuentaContableID
            };

            Session["FechasExcel"] = FechasExcel;

            return View(ReturnValues);
        }


        [Authorize]
        public ActionResult GetExcelLibroMayorTwo()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["LibroMayorTwo"] != null)
            {
                List<string[]> cachedLibroMayor = Session["LibroMayorTwo"] as List<string[]>;
                if (cachedLibroMayor != null)
                {
                    for (int i = 0; i < cachedLibroMayor.Count(); i++)
                    {
                        cachedLibroMayor[i] = cachedLibroMayor[i].Select(r => r.Replace(".", "")).ToArray();
                    }

                    var FechasExcel = new SessionParaExcel();
                    FechasExcel = (SessionParaExcel)Session["FechasExcel"];

                    string NombreCuentacont = string.Empty;
                    if (!string.IsNullOrWhiteSpace(FechasExcel.CtaContId))
                         NombreCuentacont = UtilesContabilidad.ObtenerNombreCuentaContable(Convert.ToInt32(FechasExcel.CtaContId),objCliente);


                    //tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBalanceGeneralFechaInicio"] as string, Session["strBalanceGeneralFechaFin"] as string, Session["strBalanceGeneralAnio"] as int?, Session["strBalanceGeneralMes"] as int?, "LIBRO MAYOR");
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(FechasExcel.FechaInicio, FechasExcel.FechaFin, ParseExtensions.ParseInt(FechasExcel.Anio), ParseExtensions.ParseInt(FechasExcel.Mes), "LIBRO MAYOR");
                    var cachedStream = VoucherModel.GetExcelLibroMayor(cachedLibroMayor, objCliente, true, tituloDocumento, NombreCuentacont);
                    return File(cachedStream, "application/vnd.ms-excel", "LibroMayorTwo" + Guid.NewGuid() + ".xlsx");
                }
            }
            return null;
        }

        [Authorize]
        public JsonResult LibroMayorDesdeBalance(FiltrosParaLibros flibros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);

            PaginadorModel ReturnValues = new PaginadorModel();

            flibros.Filtro = true;

            if (!string.IsNullOrWhiteSpace(flibros.FechaInicio) && !string.IsNullOrWhiteSpace(flibros.FechaFin))
            {
                ReturnValues = VoucherModel.GetLibroMayorTwo(flibros, objCliente, db);
                Session["LibroMayorTwo"] = ReturnValues.ResultStringArray;
            }
            else
            {
                ReturnValues = VoucherModel.GetLibroMayorTwo(flibros, objCliente, db);
                Session["LibroMayorTwo"] = ReturnValues.ResultStringArray;
            }
            int CtaContID = ParseExtensions.ParseInt(flibros.CuentaContableID);
            Session["ObjetoCuentaContableConsultada"] = db.DBCuentaContable.SingleOrDefault(x => x.ClientesContablesModelID == objCliente.ClientesContablesModelID && x.CuentaContableModelID == CtaContID).CuentaContableModelID;
            db.Dispose();

            return Json(ReturnValues.ResultStringArray);
        }

        [Authorize]
        public JsonResult ServicioLibroMayorConciliacion(FiltrosParaLibros flibros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);

            PaginadorModel ReturnValues = new PaginadorModel();

            if (flibros.Anio > 0 || !string.IsNullOrWhiteSpace(flibros.FechaInicio) && !string.IsNullOrWhiteSpace(flibros.FechaFin))
            {
                flibros.Filtro = true;
            }
            else if (flibros.Filtro == false)
            {
                ViewBag.AnioSinFiltro = "Registros del año" + " " + DateTime.Now.Year;
            }

            //Levar esta conversión al modelo y luego pasarle las fechas en formato String.

            if (!string.IsNullOrWhiteSpace(flibros.FechaInicio) && !string.IsNullOrWhiteSpace(flibros.FechaFin))
            {
                ReturnValues = VoucherModel.GetLibroMayorTwo(flibros, objCliente, db);
                Session["LibroMayorTwo"] = ReturnValues.ResultStringArray;
            }
            else
            {
                ReturnValues = VoucherModel.GetLibroMayorTwo(flibros, objCliente, db);
                Session["LibroMayorTwo"] = ReturnValues.ResultStringArray;
            }
            int CtaContID = ParseExtensions.ParseInt(flibros.CuentaContableID);
            Session["ObjetoCuentaContableConsultada"] = db.DBCuentaContable.SingleOrDefault(x => x.ClientesContablesModelID == objCliente.ClientesContablesModelID && x.CuentaContableModelID == CtaContID).CuentaContableModelID;
            db.Dispose();

            return Json(ReturnValues.ResultStringArray);
        }


        [Authorize]
        public ActionResult LibroMayorDesdeModelo()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


     IQueryable<LibroMayor> LibroMayorCompleto = from detalleVoucher in db.DBDetalleVoucher
                                         join Voucher in db.DBVoucher on detalleVoucher.VoucherModelID equals Voucher.VoucherModelID into vouchGroup
                                         from Voucher in vouchGroup.DefaultIfEmpty()
                                         join ClienteContable in db.DBClientesContables on Voucher.ClientesContablesModelID equals ClienteContable.ClientesContablesModelID into clientecontGroup
                                         from ClienteContable in clientecontGroup.DefaultIfEmpty()
                                         join Auxiliar in db.DBAuxiliares on detalleVoucher.DetalleVoucherModelID equals Auxiliar.DetalleVoucherModelID into auxGroup
                                         from Auxiliar in auxGroup.DefaultIfEmpty()
                                         join AuxiliarDetalle in db.DBAuxiliaresDetalle on Auxiliar.AuxiliaresModelID equals AuxiliarDetalle.AuxiliaresModelID into auxdetallGroup
                                         from AuxiliarDetalle in auxdetallGroup.DefaultIfEmpty()
                                         join CtaContable in db.DBCuentaContable on detalleVoucher.ObjCuentaContable.CuentaContableModelID equals CtaContable.CuentaContableModelID into ctaGroup
                                         from CtaContable in ctaGroup.DefaultIfEmpty()
                                         join Receptor in db.Receptores on AuxiliarDetalle.Individuo2.QuickReceptorModelID equals Receptor.QuickReceptorModelID into recepGroup
                                         from Receptor in recepGroup.DefaultIfEmpty()

                                         select new LibroMayor
                                         {
                                             Haber = detalleVoucher.MontoHaber,
                                             Debe = detalleVoucher.MontoDebe,
                                             Glosa = detalleVoucher.GlosaDetalle,
                                             FechaContabilizacion = detalleVoucher.FechaDoc,
                                             Rut = Receptor.RUT == null ? "-" : Receptor.RUT,
                                             CodigoInterno = CtaContable.CodInterno,
                                             CtaContNombre = CtaContable.nombre,
                                             CtaContablesID = CtaContable.CuentaContableModelID,
                                             CtaContableClasi = CtaContable.Clasificacion,
                                             Comprobante = Voucher.Tipo,
                                             ComprobanteP2 = Voucher.NumeroVoucher.ToString(),
                                             ComprobanteP3 = Auxiliar.LineaNumeroDetalle.ToString()
                                         };
            


            return View(LibroMayorCompleto.ToList()); // Funciona
        }



        //Queda pendiente modificar vista para integrar la paginación.
        [Authorize]
        [ModuloHandler]
        public ActionResult LibroDiario(FiltrosParaLibros flibros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.ObjClienteContable = objCliente;

            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            ConversionFechaInicioExitosa = DateTime.TryParse(flibros.FechaInicio, out dtFechaInicio);
            ConversionFechaFinExitosa = DateTime.TryParse(flibros.FechaFin, out dtFechaFin);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);

            //Anio = DateTime.Now.Year;

            List<VoucherModel> lstVoucherModel = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false).ToList();
            ViewBag.AnioFrontEnd = "Diario General " + " de " + flibros.Anio;

            PaginadorModel returnValue = new PaginadorModel();
            if (ConversionFechaFinExitosa && ConversionFechaInicioExitosa)
            {
                returnValue = VoucherModel.GetLibroDiario(flibros.pagina, flibros.cantidadRegistrosPorPagina, lstVoucherModel, db, false, flibros.Anio, flibros.Mes, dtFechaInicio, dtFechaFin, flibros.Folio, flibros.Glosa, flibros.CuentaContableID); //GetLibroMayor(lstVoucherModel, singleCuentaContable, dtFechaInicio, dtFechaFin);
                Session["LibroDiarioF"] = returnValue.ResultStringArray;
            }
            else
            {
                returnValue = VoucherModel.GetLibroDiario(flibros.pagina, flibros.cantidadRegistrosPorPagina, lstVoucherModel, db, false, flibros.Anio, flibros.Mes, null, null, flibros.Folio, flibros.Glosa, flibros.CuentaContableID);
                Session["LibroDiarioF"] = returnValue.ResultStringArray;
            }

            var FechaExcel = SessionParaExcel.ObtenerObjetoExcel(flibros.Anio.ToString(),flibros.Mes.ToString(),flibros.FechaInicio,flibros.FechaFin,flibros.CuentaContableID);
            Session["FechasExcel"] = FechaExcel;


            return View(returnValue);
        }



        [Authorize]
        [ModuloHandler]
        public ActionResult GetExcelLibroDiario()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["LibroDiarioF"] != null)
            {
                List<string[]> cachedLibroDiario = Session["LibroDiarioF"] as List<string[]>;
                if (cachedLibroDiario != null)
                {
                    for (int i = 0; i < cachedLibroDiario.Count; i++)
                    {
                        cachedLibroDiario[i] = cachedLibroDiario[i].Select(r => r.Replace(".", "")).ToArray();
                    }

                    var FechasExcel = new SessionParaExcel();
                    FechasExcel = (SessionParaExcel)Session["FechasExcel"];

                    string CtaCont = string.Empty;
                    if (!string.IsNullOrWhiteSpace(FechasExcel.CtaContId)) CtaCont = UtilesContabilidad.ObtenerNombreCuentaContable(Convert.ToInt32(FechasExcel.CtaContId), objCliente);

                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(FechasExcel.FechaInicio, FechasExcel.FechaFin, ParseExtensions.ParseInt(FechasExcel.Anio), ParseExtensions.ParseInt(FechasExcel.Mes), "LIBRO DIARIO");
                    var TieneFiltrosParaExcel = SessionParaExcel.TieneFiltrosActivos(FechasExcel);
                    var cachedStream = VoucherModel.GetExcelLibroDiario(cachedLibroDiario, objCliente, true, tituloDocumento, CtaCont, TieneFiltrosParaExcel);
                    return File(cachedStream, "application/vnd.ms-excel", "LibroDiario" + Guid.NewGuid() + ".xlsx");
                }
            }
            return null;
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult ListaVouchersBaja(int cantidadRegistrosPorPagina = 25,
                                                              int pagina = 1,
                                                              int Mes = 0,
                                                              int Anio = 0,
                                                              string FechaInicio = "",
                                                              string FechaFin = "",
                                                              string Glosa = "",
                                                              int voucherID = 0,
                                                              int? TipoOrigenVoucher = null)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }
            //List<LibrosContablesModel> LaLista = LibrosContablesModel.RescatarLibroCentralizacion(objCliente, TipoCentralizacion.Venta, db, FechaInicio, FechaFin, Anio, Mes);

            List<VoucherModel> LstVoucher;

            ViewBag.NombreCliente = objCliente.RazonSocial;

            //Predicado Base
            IQueryable<VoucherModel> Predicado = db.DBVoucher.Where(r => r.DadoDeBaja == true && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);


            // Filtros
            if (Anio != 0)
                Predicado = Predicado.Where(r => r.FechaEmision.Year == Anio);

            if (Mes != 0)
                Predicado = Predicado.Where(r => r.FechaEmision.Month == Mes);

            if (!string.IsNullOrWhiteSpace(Glosa))
                Predicado = Predicado.Where(r => r.Glosa.Contains(Glosa));

            if (voucherID != 0)
                Predicado = Predicado.Where(r => r.NumeroVoucher == voucherID);

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
                Predicado = Predicado.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin);

            LstVoucher = Predicado
                        .OrderByDescending(x => x.NumeroVoucher)
                        .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                        .Take(cantidadRegistrosPorPagina)
                        .ToList();

            var totalDeRegistros = Predicado.Count();

            // Pasamos los datos necesarios para que funcione el paginador generico.
            var Paginador = new PaginadorModel();
            Paginador.VoucherList = LstVoucher;
            Paginador.PaginaActual = pagina;
            Paginador.TotalDeRegistros = totalDeRegistros;
            Paginador.RegistrosPorPagina = cantidadRegistrosPorPagina;
            Paginador.ValoresQueryString = new RouteValueDictionary();
            //Mandar estos parametros por ajax.
            //En la vista
            // Parametros de busqueda llamados por GET.
            if (cantidadRegistrosPorPagina != 25)
                Paginador.ValoresQueryString["cantidadRegistrosPorPagina"] = cantidadRegistrosPorPagina;

            if (Anio != 0)
                Paginador.ValoresQueryString["Anio"] = Anio;

            if (Mes != 0)
                Paginador.ValoresQueryString["Mes"] = Mes;

            if (!string.IsNullOrWhiteSpace(Glosa))
                Paginador.ValoresQueryString["Glosa"] = Glosa;

            if (voucherID != 0)
                Paginador.ValoresQueryString["VoucherID"] = voucherID;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                Paginador.ValoresQueryString["FechaInicio"] = FechaInicio;
                Paginador.ValoresQueryString["FechaFin"] = FechaFin;
            }
            if (TipoOrigenVoucher != null)
                Paginador.ValoresQueryString["TipoOrigenVoucher"] = TipoOrigenVoucher;


            return View(Paginador);
        }

        [Authorize]
        [ModuloHandler]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult RestablecerVoucher(int idVoucher, AccionMonitoreo accion = AccionMonitoreo.Edicion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente != null)
            {
                VoucherModel objVoucher = db.DBVoucher.SingleOrDefault(x => x.VoucherModelID == idVoucher && x.ClientesContablesModelID == objCliente.ClientesContablesModelID);
                if (objVoucher != null)
                {
                    objVoucher.DadoDeBaja = false;
                    db.SaveChanges();
                    LibrosContablesModel libro = db.DBLibrosContables.SingleOrDefault(x => x.VoucherModelID == objVoucher.VoucherModelID);
                    if (libro != null)
                    {

                        libro.estado = true;
                        db.SaveChanges();

                    }
                    var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri)
                    {
                        Path = Url.Action("ListaVouchersBaja", "Contabilidad"),
                        Query = null,
                    };

                    Uri uri = urlBuilder.Uri;
                    string url = urlBuilder.ToString();

                    return Json(new
                    {
                        ok = true,
                        ReturnURL = url,
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    ok = false
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult ListaCentroCosto()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<CentroCostoModel> lstCentroCosto = objCliente.ListCentroDeCostos.ToList();

            return View(lstCentroCosto);
        }

        [Authorize]
        public ActionResult CentroCostosPresupuesto(int[] Presupuesto, int[] centrocostoid)
        {

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<CentroCostoModel> lstCentroCosto = objCliente.ListCentroDeCostos.ToList();

            if (Presupuesto != null && centrocostoid != null && objCliente.ClientesContablesModelID != 0)
            {

                List<CentroCostoPresupuestoModels> lstCCPresupuesto = db.DBCentroCostoPresupuesto.Where(r => r.ClienteContableModelID == objCliente.ClientesContablesModelID).ToList();

                // Limpiamos.
                foreach (CentroCostoPresupuestoModels ItemARemover in lstCCPresupuesto)
                {
                    db.DBCentroCostoPresupuesto.Remove(ItemARemover);
                    db.SaveChanges();
                }
                CentroCostoPresupuestoModels InsertPresupuesto = new CentroCostoPresupuestoModels();



                DateTime FechaInicio = DateTime.Now;
                DateTime FechaVencimiento = DateTime.Now;

                FechaVencimiento = FechaVencimiento.AddYears(1);

                int ContadorPadre = -1;
                foreach (int Presu in Presupuesto)
                {
                    ContadorPadre++;
                    int PresupuestoToControl = Presu;
                    foreach (int CentroDeCostos in centrocostoid.Skip(ContadorPadre))
                    {
                        if (Presu != 0)
                        {

                            InsertPresupuesto.ClienteContableModelID = objCliente.ClientesContablesModelID;
                            InsertPresupuesto.Presupuesto = Convert.ToDecimal(Presu);
                            InsertPresupuesto.FechaInicioPresu = FechaInicio;
                            InsertPresupuesto.FechaVencimientoPresu = FechaVencimiento;
                            InsertPresupuesto.CentroCostoModelID = CentroDeCostos;
                            db.DBCentroCostoPresupuesto.Add(InsertPresupuesto);
                            db.SaveChanges();

                            PresupuestoToControl = 0;
                        }
                        break;
                    }
                }
            }

            return View(lstCentroCosto);
        }

        [Authorize]
        public ActionResult ComparacionCtaPresu(int PresupuestoID)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["Presupuesto"] = null;
            Session["FechaInicio"] = null;
            Session["FechaVencimiento"] = null;

            List<string[]> TablaPresupuesto = new List<string[]>();

            PresupuestoModel PresupuestoConsultado = db.DBPresupuestos.SingleOrDefault(x => x.PresupuestoModelID == PresupuestoID);

            if (PresupuestoConsultado != null)
            {

                ViewBag.NombrePresupuesto = PresupuestoConsultado.NombrePresupuesto;
                ViewBag.FechaInicio = ParseExtensions.ToDD_MM_AAAA(PresupuestoConsultado.FechaInicio);
                ViewBag.FechaVencimiento = ParseExtensions.ToDD_MM_AAAA(PresupuestoConsultado.FechaVencimiento);

                Session["FechaInicio"] = ViewBag.FechaInicio;
                Session["FechaVencimiento"] = ViewBag.FechaVencimiento;

                List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false).ToList();
                List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();


                TablaPresupuesto = VoucherModel.TablaPresupuesto(lstVoucherCliente, lstCuentasContablesClientes, db, objCliente, PresupuestoConsultado);
                Session["Presupuesto"] = TablaPresupuesto;

            }
            else
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                return RedirectToAction("MisPresupuestos", "Contabilidad");
            }
            return View(TablaPresupuesto);
        }

        [Authorize]
        public JsonResult DarDeBajaPresupuesto(int IDPresupuesto)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool Result = false;

            PresupuestoModel ReceptorDarDeBaja = db.DBPresupuestos.SingleOrDefault(x => x.PresupuestoModelID == IDPresupuesto &&
                                                                                        x.Cliente.ClientesContablesModelID == objCliente.ClientesContablesModelID);

            if (ReceptorDarDeBaja != null)
            {
                ReceptorDarDeBaja.DadoDeBaja = true;
                db.SaveChanges();
                Result = true;
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult ListaItems()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<ItemModel> lstItems = objCliente.ListItems.ToList();

            return View(lstItems);
        }

        [Authorize]
        // [ModuloHandler]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult NuevoItem(string NombreItem, AccionMonitoreo accion = AccionMonitoreo.Creacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente == null || string.IsNullOrWhiteSpace(NombreItem))
            {
                return RedirectToAction("SeleccionarClienteContable", "Contabilidad");
            }
            else
            {
                int totales = db.DBItems.Where(r => r.ClienteContableID == objCliente.ClientesContablesModelID).Count();


                ItemModel objItem = new ItemModel();

                objItem.NombreItem = NombreItem;
                objItem.Estado = true;
                objItem.ClienteContableID = objCliente.ClientesContablesModelID;
                objItem.contador = totales + 1;
                db.DBItems.Add(objItem);
                objCliente.ListItems.Add(objItem);
                db.SaveChanges();
                return RedirectToAction("ListaItems", "Contabilidad");
            }
        }

        [Authorize]
        public JsonResult ObtenerItemEdit(int IDItem)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objEmisor == null || objCliente == null || IDItem == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            ItemModel Item = objCliente.ListItems.SingleOrDefault(r => r.ItemModelID == IDItem);
            if (Item == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = true,
                    IDItem = Item.ItemModelID,
                    IDClienteContable = Item.ClienteContableID,
                    NombreItem = Item.NombreItem,

                }, JsonRequestBehavior.AllowGet);
            }
        }
        //[ModuloHandler]
        [Authorize]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult EditarItem(int IDItem, string NombreItemE, AccionMonitoreo accion = AccionMonitoreo.Edicion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente == null || string.IsNullOrWhiteSpace(NombreItemE))
            {
                return RedirectToAction("SeleccionarClienteContable", "Contabilidad");
            }
            else
            {
                ItemModel objItem = objCliente.ListItems.SingleOrDefault(r => r.ItemModelID == IDItem);

                objItem.NombreItem = NombreItemE;
                // objCentro.ClientesContablesModelID = objCliente.ClientesContablesModelID;

                //db.DBCentroCosto.Add(objCentro);
                objCliente.ListItems.Add(objItem);
                db.SaveChanges();
                return RedirectToAction("ListaItems", "Contabilidad");
            }
        }

        [Authorize]
        [ModuloHandler]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult NuevoCentroCosto(string NombreCentroCosto, AccionMonitoreo accion = AccionMonitoreo.Creacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente == null || string.IsNullOrWhiteSpace(NombreCentroCosto))
            {
                return RedirectToAction("SeleccionarClienteContable", "Contabilidad");
            }
            else
            {
                CentroCostoModel objCentro = new CentroCostoModel();

                int total = db.DBCentroCosto.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID).Count();

                objCentro.Nombre = NombreCentroCosto;
                objCentro.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                objCentro.contador = total + 1;

                db.DBCentroCosto.Add(objCentro);
                objCliente.ListCentroDeCostos.Add(objCentro);

                db.SaveChanges();
                return RedirectToAction("ListaCentroCosto", "Contabilidad");
            }
        }

        [Authorize]
        // [ModuloHandler]
        [Monitoreo(AccionDeclarada = "accion")]
        public ActionResult EditarCentroCosto(int IDCentroCostoE, string NombreCentroCostoE, AccionMonitoreo accion = AccionMonitoreo.Edicion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente == null || string.IsNullOrWhiteSpace(NombreCentroCostoE))
            {
                return RedirectToAction("SeleccionarClienteContable", "Contabilidad");
            }
            else
            {
                CentroCostoModel objCentro = objCliente.ListCentroDeCostos.SingleOrDefault(r => r.CentroCostoModelID == IDCentroCostoE);

                objCentro.Nombre = NombreCentroCostoE;
                // objCentro.ClientesContablesModelID = objCliente.ClientesContablesModelID;

                //db.DBCentroCosto.Add(objCentro);
                objCliente.ListCentroDeCostos.Add(objCentro);
                db.SaveChanges();
                return RedirectToAction("ListaCentroCosto", "Contabilidad");
            }
        }

        [Authorize]
       // [ModuloHandler]
        public ActionResult IEstadoresultado()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetInfomeEstadoResultado(lstVoucherCliente, lstCuentasContablesClientes);

            Session["IEstadoResultadoF"] = returnValue;
            return View(returnValue);
        }

        public ActionResult IEstadoResultadoPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }
    
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strIEstadoResultadoFechaInicio"] = null;
            Session["strIEstadoResultadoFechaFin"] = null;
            Session["strIEstadoResultadoAnio"] = null;
            Session["strIEstadoResultadoMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strIEstadoResultadoFechaInicio"] = FechaInicio;
                Session["strIEstadoResultadoFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetInfomeEstadoResultado(lstVoucherModel, lstCuentasContablesClientes);
            Session["IEstadoResultadoF"] = returnValue;

            return PartialView(returnValue);
        }

        public ActionResult GetExcelIEstadoResultado()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["IEstadoResultadoF"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["IEstadoResultadoF"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i].All(x => x != null)) {
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strIEstadoResultadoFechaInicio"] as string, Session["strIEstadoResultadoFechaFin"] as string, Session["strIEstadoResultadoAnio"] as int?, Session["strIEstadoResultadoMes"] as int?, "RESULTADO DEL EJERCICIO");
                    var cachedStream = VoucherModel.GetExcelResultIEstadoResultado(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Estado Resultado" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }

        [Authorize]
        public ActionResult EstadoResultadoComparativo(int Mes = 0, int Anio = 0, int MesesAMostrar = 3)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Anio = DateTime.Now.Year;
            Mes = DateTime.Now.Month;

            var EstadoResultadoProcesado = EstadoResultadoComparativoViewModel.GetEstadoResultadoComparativo(objCliente, Mes, Anio, MesesAMostrar);

            bool Filtros = false;

            ViewBag.TotalesGanancias = EstadoResultadoProcesado.Item2;
            ViewBag.TotalesPerdidas = EstadoResultadoProcesado.Item3;
            ViewBag.TotalesGlobales = EstadoResultadoProcesado.Item4;
            ViewBag.lstSubClasificacion = EstadoResultadoProcesado.Item5;
            ViewBag.lstSubSubClasificacion = EstadoResultadoProcesado.Item6;
            ViewBag.ListaFechas = EstadoResultadoProcesado.Item7;
            ViewBag.BusquedaPorAnio = Filtros;
            ViewBag.Meses = ParseExtensions.EnumToDropDownList<Meses>();

            List<ClasificacionCtaContable> Clasificaciones = ParseExtensions.ListaClasificacion();
            Clasificaciones = Clasificaciones.Where(x => x == ClasificacionCtaContable.RESULTADOGANANCIA || x == ClasificacionCtaContable.RESULTADOPERDIDA).ToList();

            List<CentroCostoModel> lstCentroDeCostos = objCliente.ListCentroDeCostos.ToList();  

            ViewBag.lstCentroDeCostos = lstCentroDeCostos;

            ViewBag.Clasificacion = Clasificaciones;

            Session["TipoFiltro"] = Filtros;

            Session["Anio"] = Anio;

            Session["EstadoResultadoComparativo"] = EstadoResultadoProcesado.Item1;
            Session["FechasEstadoComparativo"] = EstadoResultadoProcesado.Item7;
            
    
            Session["TotalGananciasEstaComp"] = EstadoResultadoProcesado.Item2;
            Session["TotalPerdidasEstaComp"] = EstadoResultadoProcesado.Item3;
            Session["TotalesGlobalesEstaComp"] = EstadoResultadoProcesado.Item4;

            return View(EstadoResultadoProcesado.Item1);
        }


        [Authorize]
        public ActionResult EstadoResultadoComparativoPartial(List<string> Meses, int Anio = 0,int AnioDesde = 0, int AnioHasta = 0, int CentroDeCostoID = 0)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var EstadoResultadoProcesado = EstadoResultadoComparativoViewModel.EstadoResultadoComparativoConFiltros(objCliente, Meses, Anio, AnioDesde, AnioHasta, CentroDeCostoID);

            List<ClasificacionCtaContable> Clasificaciones = ParseExtensions.ListaClasificacion();
            Clasificaciones = Clasificaciones.Where(x => x == ClasificacionCtaContable.RESULTADOGANANCIA || x == ClasificacionCtaContable.RESULTADOPERDIDA).ToList();

            ViewBag.Clasificacion = Clasificaciones;
            
            if(CentroDeCostoID > 0)
            {
                string CentroDeCostoNombre = CentroCostoModel.GetNombreCentroDeCosto(CentroDeCostoID, objCliente);
                ViewBag.NombreCC = CentroDeCostoNombre;
            }

            bool BusquedaPorAnio = false;

            if (AnioDesde > 0 && AnioHasta > 0)
                BusquedaPorAnio = true;

            ViewBag.TotalesGanancias = EstadoResultadoProcesado.Item2;
            ViewBag.TotalesPerdidas = EstadoResultadoProcesado.Item3;
            ViewBag.TotalesGlobales = EstadoResultadoProcesado.Item4;
            ViewBag.lstSubClasificacion = EstadoResultadoProcesado.Item5;
            ViewBag.lstSubSubClasificacion = EstadoResultadoProcesado.Item6;
            ViewBag.ListaFechas = EstadoResultadoProcesado.Item7;
            ViewBag.BusquedaPorAnio = BusquedaPorAnio;

            Session["EstadoResultadoComparativo"] = EstadoResultadoProcesado.Item1;
            Session["FechasEstadoComparativo"] = EstadoResultadoProcesado.Item7;

            Session["TotalGananciasEstaComp"] = EstadoResultadoProcesado.Item2;
            Session["TotalPerdidasEstaComp"] = EstadoResultadoProcesado.Item3;
            Session["TotalesGlobalesEstaComp"] = EstadoResultadoProcesado.Item4;
            Session["Anio"] = Anio;
            Session["TipoFiltro"] = BusquedaPorAnio;
            Session["AnioDesde"] = null;
            Session["AnioHasta"] = null;
            if(BusquedaPorAnio == true) { 
                Session["AnioDesde"] = AnioDesde;
                Session["AnioHasta"] = AnioHasta;
            }

            return PartialView(EstadoResultadoProcesado.Item1);
        }

        [Authorize]
        public ActionResult GetExcelEstadoResultadoComparativo()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<string[]> TestExport = new List<string[]>();
            if(Session["EstadoResultadoComparativo"] != null)
            {
                List<EstadoResultadoComparativoViewModel> lstEstadoCompExcel = Session["EstadoResultadoComparativo"] as List<EstadoResultadoComparativoViewModel>;
                List<DateTime> FechasConsultadas = Session["FechasEstadoComparativo"] as List<DateTime>;
                List<decimal> ResultadoGanancia = Session["TotalGananciasEstaComp"] as List<decimal>;
                List<decimal> ResultadoPerdida = Session["TotalPerdidasEstaComp"] as List<decimal>;
                List<decimal> ResultadoDelEjercicio = Session["TotalesGlobalesEstaComp"] as List<decimal>;
                bool BusquedaPorAnio = (bool)Session["TipoFiltro"];
                int Anio = 0;
                if (BusquedaPorAnio == false)
                    Anio = (int)Session["Anio"];
                int AnioDesde = 0;
                int AnioHasta = 0;

                if(Session["AnioDesde"] != null && Session["AnioHasta"] != null) { 
                    AnioDesde = (int)Session["AnioDesde"];
                    AnioHasta = (int)Session["AnioHasta"];
                }

                var cachedStream = EstadoResultadoComparativoViewModel.GetExcelEstadoComp(lstEstadoCompExcel,FechasConsultadas,ResultadoGanancia,ResultadoPerdida,ResultadoDelEjercicio, objCliente, true,AnioDesde,AnioHasta,BusquedaPorAnio,Anio);
                return File(cachedStream, "application/vnd.ms-excel", "EstadoResultadoComparativo" + Guid.NewGuid() + ".xlsx");
            }

            return null;
        }

        [Authorize]
      //  [ModuloHandler]
        public ActionResult EstadoResultadoCtaBruto()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
           
            List<string[]> returnValue = VoucherModel.GetIEBruto(lstVoucherCliente, lstCuentasContablesClientes);

            Session["IEstadoResultadoBruto"] = returnValue;
            return View(returnValue);
        }

        public ActionResult EstadoResultadoCtaBrutoPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
           
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strIEstadoResultadoBrutoFechaInicio"] = null;
            Session["strIEstadoResultadoBrutoFechaFin"] = null;
            Session["strIEstadoResultadoBrutoAnio"] = null;
            Session["strIEstadoResultadoBrutoMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
         
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strIEstadoResultadoBrutoFechaInicio"] = FechaInicio;
                Session["strIEstadoResultadoBrutoFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoBrutoAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoBrutoMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetIEBruto(lstVoucherModel, lstCuentasContablesClientes);
            Session["IEstadoResultadoBruto"] = returnValue;

            return PartialView(returnValue);
        }

        public ActionResult GetExcelIEBruto()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["IEstadoResultadoBruto"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["IEstadoResultadoBruto"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if (cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strIEstadoResultadoBrutoFechaInicio"] as string, Session["strIEstadoResultadoBrutoFechaFin"] as string, Session["strIEstadoResultadoBrutoAnio"] as int?, Session["strIEstadoResultadoBrutoMes"] as int?, "INFORME MARGEN BRUTO");
                    var cachedStream = VoucherModel.GetExcelInformesEstadoResultado(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Estado Resultado Bruto" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }


        //EBITDA
        [Authorize]
        public ActionResult EstadoResultadoEBITDA()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();

            List<string[]> returnValue = VoucherModel.GetIEEBITDA(lstVoucherCliente, lstCuentasContablesClientes);

            Session["IEstadoResultadoEBITDA"] = returnValue;
            return View(returnValue);
        }

        public ActionResult EstadoResultadoEBITDAPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
        
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strIEstadoResultadoEBITDAFechaInicio"] = null;
            Session["strIEstadoResultadoEBITDAFechaFin"] = null;
            Session["strIEstadoResultadoEBITDAAnio"] = null;
            Session["strIEstadoResultadoEBITDAMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
      
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strIEstadoResultadoEBITDAFechaInicio"] = FechaInicio;
                Session["strIEstadoResultadoEBITDAFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoEBITDAAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoEBITDAMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetIEEBITDA(lstVoucherModel, lstCuentasContablesClientes);
            Session["IEstadoResultadoEBITDA"] = returnValue;

            return PartialView(returnValue);
        }

        public ActionResult GetExcelEstadoResultadoEBITDA()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["IEstadoResultadoEBITDA"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["IEstadoResultadoEBITDA"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if(cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strIEstadoResultadoEBITDAFechaInicio"] as string, Session["strIEstadoResultadoEBITDAFechaFin"] as string, Session["strIEstadoResultadoEBITDAAnio"] as int?, Session["strIEstadoResultadoEBITDAMes"] as int?, "INFORME EBITDA");
                    var cachedStream = VoucherModel.GetExcelInformesEstadoResultado(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Estado Resultado EBITDA" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }


        //EBIT

        [Authorize]
        public ActionResult EstadoResultadoEBIT()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            //List<CuentaContableModel> lstCuentasContablesClientes = db.DBCuentaContable.ToList(); //en vez de acceder directamente por el cliente contable accedemos directo a la base de datos.
            //var listaSubSubClasificacion = lstCuentasContablesClientes.Select(n => n.SubSubClasificacion.NombreInterno).Distinct().ToList();

            List<string[]> returnValue = VoucherModel.GetIEEBIT(lstVoucherCliente, lstCuentasContablesClientes);

            Session["IEstadoResultadoEBIT"] = returnValue;
            return View(returnValue);
        }

        public ActionResult EstadoResultadoEBITPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            //Queda pendiente Agregar las columnas de los meses y terminar el filtro de Mes.
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strIEstadoResultadoEBITFechaInicio"] = null;
            Session["strIEstadoResultadoEBITFechaFin"] = null;
            Session["strIEstadoResultadoEBITAnio"] = null;
            Session["strIEstadoResultadoEBITMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
     
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strIEstadoResultadoEBITFechaInicio"] = FechaInicio;
                Session["strIEstadoResultadoEBITFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoEBITAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoEBITMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetIEEBIT(lstVoucherModel, lstCuentasContablesClientes);
            Session["IEstadoResultadoEBIT"] = returnValue;

            return PartialView(returnValue);
        }

        public ActionResult GetExcelEstadoResultadoEBIT()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["IEstadoResultadoEBIT"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["IEstadoResultadoEBIT"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if (cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }// Esto es lo que debes verificiar ahora.
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strIEstadoResultadoEBITFechaInicio"] as string, Session["strIEstadoResultadoEBITFechaFin"] as string, Session["strIEstadoResultadoEBITAnio"] as int?, Session["strIEstadoResultadoEBITMes"] as int?, "INFORME EBIT");
                    var cachedStream = VoucherModel.GetExcelInformesEstadoResultado(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Estado Resultado EBIT" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }



        //EBT
        [Authorize]
        public ActionResult EstadoResultadoEBT()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherCliente = objCliente.ListVoucher.Where(r => r.FechaEmision.Year == DateTime.Now.Year && r.DadoDeBaja == false).ToList();
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
            //List<CuentaContableModel> lstCuentasContablesClientes = db.DBCuentaContable.ToList(); //en vez de acceder directamente por el cliente contable accedemos directo a la base de datos.
            //var listaSubSubClasificacion = lstCuentasContablesClientes.Select(n => n.SubSubClasificacion.NombreInterno).Distinct().ToList();

            List<string[]> returnValue = VoucherModel.GetIEEBT(lstVoucherCliente, lstCuentasContablesClientes);

            Session["IEstadoResultadoEBT"] = returnValue;
            return View(returnValue);
        }

        public ActionResult EstadoResultadoEBTPartial(string FechaInicio = "", string FechaFin = "", string Anio = "", string Mes = "")
        {
            //Queda pendiente Agregar las columnas de los meses y terminar el filtro de Mes.
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();

            if (string.IsNullOrWhiteSpace(FechaInicio) == false && string.IsNullOrWhiteSpace(FechaFin) == false)
            {
                ConversionFechaInicioExitosa = DateTime.TryParseExact(FechaInicio, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaInicio);
                ConversionFechaFinExitosa = DateTime.TryParseExact(FechaFin, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["strIEstadoResultadoEBTFechaInicio"] = null;
            Session["strIEstadoResultadoEBTFechaFin"] = null;
            Session["strIEstadoResultadoEBTAnio"] = null;
            Session["strIEstadoResultadoEBTMes"] = null;

            IEnumerable<VoucherModel> CollectionVoucherCliente = objCliente.ListVoucher.Where(r => r.DadoDeBaja == false);
            List<CuentaContableModel> lstCuentasContablesClientes = objCliente.CtaContable.ToList();
          
            List<VoucherModel> lstVoucherModel;

            if (ConversionFechaInicioExitosa && ConversionFechaFinExitosa)
            {
                lstVoucherModel = CollectionVoucherCliente.Where(r => r.FechaEmision >= dtFechaInicio && r.FechaEmision <= dtFechaFin).ToList();
                Session["strIEstadoResultadoEBTFechaInicio"] = FechaInicio;
                Session["strIEstadoResultadoEBTFechaFin"] = FechaFin;
                //save start/end date for export
            }
            else if (string.IsNullOrWhiteSpace(Anio) == false || string.IsNullOrWhiteSpace(Mes))
            {
                int AnioToLook = ParseExtensions.ParseInt(Anio);
                int MesToLook = ParseExtensions.ParseInt(Mes);
                if (AnioToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Year == AnioToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoEBTAnio"] = AnioToLook;
                }
                if (MesToLook != 0)
                {
                    CollectionVoucherCliente = CollectionVoucherCliente.Where(r => r.FechaEmision.Month == MesToLook).ToList();
                    //save start/end date for export
                    Session["strIEstadoResultadoEBTMes"] = MesToLook;
                }
                lstVoucherModel = CollectionVoucherCliente.ToList();
            }
            else
                lstVoucherModel = CollectionVoucherCliente.ToList();

            List<string[]> returnValue = VoucherModel.GetIEEBT(lstVoucherModel, lstCuentasContablesClientes);
            Session["IEstadoResultadoEBT"] = returnValue;

            return PartialView(returnValue);
        }

        public ActionResult GetExcelEstadoResultadoEBT()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["IEstadoResultadoEBT"] != null)
            {
                List<string[]> cachedEstadoResultado = Session["IEstadoResultadoEBT"] as List<string[]>;
                if (cachedEstadoResultado != null)
                {
                    for (int i = 0; i < cachedEstadoResultado.Count; i++)
                    {
                        if (cachedEstadoResultado[i] != null)
                        {
                            if (cachedEstadoResultado[i].DefaultIfEmpty() == null) { 
                            cachedEstadoResultado[i] = cachedEstadoResultado[i].Select(r => r.Replace(".", "")).ToArray();
                            }
                        }
                    }
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strIEstadoResultadoEBTFechaInicio"] as string, Session["strIEstadoResultadoEBTFechaFin"] as string, Session["strIEstadoResultadoEBTAnio"] as int?, Session["strIEstadoResultadoEBTMes"] as int?, "INFORME EBT");
                    var cachedStream = VoucherModel.GetExcelInformesEstadoResultado(cachedEstadoResultado, objCliente, true, tituloDocumento);
                    return File(cachedStream, "application/vnd.ms-excel", "Informe Estado Resultado EBT" + Guid.NewGuid() + ".xlsx");
                }
            }

            return View();
        }

        

        [Authorize]
        [ModuloHandler]
        public ActionResult IngresarDocumentoVenta()
        {
            ViewBag.OSTipoDte = ParseExtensions.ObtenerTipoDTEDropdownAsString();
            return View();
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult NuevoDocumentoVenta(string AUXRut, string AUXRazoncta, int TipoDocumento, int Folio,
            string FechaDoc, string FechaRecep, int MontoExento, int MontoNeto, int MontoIva, int MontoTotal,
            int? TipoDocReferencia, int? FolioDocReferencia)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            try
            {

                // valido que no este repedito el documento de venta por los conceptos de folio, proveedor, tipo_factura, 
                var docVenta = db.DBLibrosContables.SingleOrDefault(x => x.FolioDocReferencia == Folio && x.TipoDocumento == (TipoDte)TipoDocumento && x.ClientesContablesModelID == objCliente.ClientesContablesModelID);

                if (docVenta != null)
                {
                    TempData["Error"] = "Documento de venta ya se encuentra registrado";
                    return RedirectToAction("IngresarDocumentoVenta", "Contabilidad");
                }


                LibrosContablesModel objDoc = new LibrosContablesModel();
                objDoc.ClientesContablesModelID = objCliente.ClientesContablesModelID;


                string rutPrestador = Request.Form.GetValues("AUXrut")[0];
                string nombrePrestador = Request.Form.GetValues("AUXrazoncta")[0];

                objDoc.Prestador = AuxiliaresPrestadoresModel.CrearOActualizarPrestadorPorRut(rutPrestador, nombrePrestador, objCliente, db);
                /*
                objDoc.Prestador = new AuxiliaresPrestadoresModel();
                objDoc.Prestador.PrestadorRut = rutPrestador;
                objDoc.Prestador.PrestadorNombre = nombrePrestador;
                objDoc.Prestador.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                */
                objDoc.TipoLibro = TipoCentralizacion.Venta;

                objDoc.TipoDocumento = (TipoDte)TipoDocumento;
                objDoc.Folio = Folio;


                DateTime dtFechaDoc;
                DateTime dtFechaRecep;

                DateTime.TryParseExact(FechaDoc, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaDoc);
                DateTime.TryParseExact(FechaRecep, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaRecep);

                objDoc.FechaDoc = dtFechaDoc;
                objDoc.FechaRecep = dtFechaRecep;

                objDoc.MontoExento = MontoExento;
                objDoc.MontoNeto = MontoNeto;
                objDoc.MontoIva = MontoIva;
                objDoc.MontoTotal = MontoTotal;

                if (TipoDocReferencia.HasValue && TipoDocReferencia != 0)
                {
                    objDoc.TipoDocReferencia = (TipoDte)TipoDocReferencia;
                }
                if (FolioDocReferencia.HasValue && FolioDocReferencia != 0)
                {
                    objDoc.FolioDocReferencia = FolioDocReferencia;
                }

                db.DBLibrosContables.Add(objDoc);
                db.SaveChanges();

                TempData["Correcto"] = "Documento de venta ya se encuentra registrado";
            }
            catch (DbEntityValidationException e)
            {
                ParseExtensions.GetValidationErrors(e);
                throw;
            }
            return RedirectToAction("IngresarDocumentoVenta", "Contabilidad");
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult IngresarDocumentoCompra()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.OSTipoDte = ParseExtensions.ObtenerTipoDTEDropdownAsString();
            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsString(objCliente);
            return View();
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult NuevoDocumentoCompra(int cuenta, string AUXRut, string AUXRazoncta, int TipoDocumento, int Folio,
            string FechaDoc, string FechaRecep, int MontoExento, int MontoNeto, int MontoIva, int MontoTotal,
            int? TipoDocReferencia, int? FolioDocReferencia)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            try
            {

                var docVenta = db.DBLibrosContables.SingleOrDefault(x => x.FolioDocReferencia == Folio && x.TipoDocumento == (TipoDte)TipoDocumento && x.ClientesContablesModelID == objCliente.ClientesContablesModelID);

                if (docVenta != null)
                {
                    TempData["Error"] = "Documento de compra ya se encuentra registrado";
                    return RedirectToAction("IngresarDocumentoCompra", "Contabilidad");
                }


                LibrosContablesModel objDoc = new LibrosContablesModel();
                objDoc.ClientesContablesModelID = objCliente.ClientesContablesModelID;


                string rutPrestador = Request.Form.GetValues("AUXrut")[0];
                string nombrePrestador = Request.Form.GetValues("AUXrazoncta")[0];

                objDoc.Prestador = AuxiliaresPrestadoresModel.CrearOActualizarPrestadorPorRut(rutPrestador, nombrePrestador, objCliente, db);

                /*
                objDoc.Prestador = new AuxiliaresPrestadoresModel();
                objDoc.Prestador.PrestadorRut = rutPrestador;
                objDoc.Prestador.PrestadorNombre = nombrePrestador;
                objDoc.Prestador.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                */
                objDoc.TipoLibro = TipoCentralizacion.Compra;

                objDoc.TipoDocumento = (TipoDte)TipoDocumento;
                objDoc.Folio = Folio;

                DateTime dtFechaDoc;
                DateTime dtFechaRecep;

                DateTime.TryParseExact(FechaDoc, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaDoc);
                DateTime.TryParseExact(FechaRecep, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaRecep);

                objDoc.FechaDoc = dtFechaDoc;
                objDoc.FechaRecep = dtFechaRecep;

                objDoc.MontoExento = MontoExento;
                objDoc.MontoNeto = MontoNeto;
                objDoc.MontoIva = MontoIva;
                objDoc.MontoTotal = MontoTotal;

                if (TipoDocReferencia.HasValue && TipoDocReferencia != 0)
                {
                    objDoc.TipoDocReferencia = (TipoDte)TipoDocReferencia;
                }
                if (FolioDocReferencia.HasValue && FolioDocReferencia != 0)
                {
                    objDoc.FolioDocReferencia = FolioDocReferencia;
                }

                db.DBLibrosContables.Add(objDoc);

                List<CuentaContableModel> lstCuentaCont = new List<CuentaContableModel>();
                CuentaContableModel objCuentaContable = db.DBCuentaContable.Find(cuenta);
                lstCuentaCont.Add(objCuentaContable);
                List<LibrosContablesModel> lstDocs = new List<LibrosContablesModel>();
                lstDocs.Add(objDoc);
                db.SaveChanges();
                List<int> centrodecostos = new List<int>();
                LibrosContablesModel.ProcesarLibrosContablesAVoucher(lstDocs, objCliente, db, lstCuentaCont, centrodecostos);
                TempData["Correcto"] = "Documento de venta ya se encuentra registrado";
            }
            catch (DbEntityValidationException e)
            {
                ParseExtensions.GetValidationErrors(e);
                throw;
            }
            return RedirectToAction("IngresarDocumentoCompra", "Contabilidad");
        }

        //PENDING
        [Authorize]
        [ModuloHandler]
        public ActionResult LibroVenta(FiltrosParaLibros flibros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var Paginador = new PaginadorModel();
            ViewBag.ObjClienteContable = objCliente;

            Paginador = LibrosContablesModel.RescatarLibroCentralizacion(objCliente, TipoCentralizacion.Venta, db, flibros.FechaInicio, flibros.FechaFin, flibros.Anio, flibros.Mes, flibros.pagina, flibros.cantidadRegistrosPorPagina, flibros.Rut, flibros.RazonSocial,flibros.IFolio);

            var FechasExcel = SessionParaExcel.ObtenerObjetoExcel(flibros.Anio.ToString(), flibros.Mes.ToString(),flibros.FechaInicio,flibros.FechaFin, string.Empty);

            Session["FechasExcel"] = FechasExcel;
            Session["LibroVenta"] = Paginador.ResultStringArray;

            return View(Paginador);
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult GetExcelLibroVenta()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            SessionParaExcel FechasExcel = (SessionParaExcel)Session["FechasExcel"];

          
            //TO-DO: Verificar si vuelve el filtro entre fechas
            string strFechaInicio = string.Empty;//Request.Form.GetValues("fechainicio")[0];
            string strFechaFin = string.Empty;//Request.Form.GetValues("fechafin")[0];

            string tituloDocumento = string.Empty;

            List<string[]> cachedLibroVenta = Session["LibroVenta"] as List<string[]>;
            if (cachedLibroVenta != null)
            {
                for (int i = 0; i < cachedLibroVenta.Count; i++)
                {
                    cachedLibroVenta[i] = cachedLibroVenta[i].Select(r => r.Replace(".", "")).ToArray();
                }
                tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteLibroVentaCompra(FechasExcel.FechaInicio, FechasExcel.FechaFin, Convert.ToInt32(FechasExcel.Anio), Convert.ToInt32(FechasExcel.Mes), "LIBRO DE VENTAS");
                var cachedStream = AuxiliaresDetalleModel.ExportExcelLibroVentaCompraNormal(cachedLibroVenta, objCliente, true, TipoCentralizacion.Venta, tituloDocumento, strFechaInicio, strFechaFin, Convert.ToInt32(FechasExcel.Anio),  Convert.ToInt32(FechasExcel.Mes)); //AuxiliaresDetalleModel.ExportExcelLibroCentralizacionAuxiliaresVentaCompra(objCliente, true, TipoCentralizacion.Venta, tituloDocumento, strFechaInicio, strFechaFin, intAnio, intMes); //AGREGAR ACA FILTRO DE FECHAS 
                return File(cachedStream, "application/vnd.ms-excel", "LibroVenta" + Guid.NewGuid() + ".xlsx");
            }

            return View("LibroVenta");
        }

        //PENDING
        [Authorize]
        [ModuloHandler]
        public ActionResult LibroCompra(FiltrosParaLibros flibros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.ObjClienteContable = objCliente;

            var Paginador = new PaginadorModel();

            Paginador = LibrosContablesModel.RescatarLibroCentralizacion(objCliente, TipoCentralizacion.Compra, db,flibros.FechaInicio,flibros.FechaFin,flibros.Anio,flibros.Mes,flibros.pagina,flibros.cantidadRegistrosPorPagina,flibros.Rut,flibros.RazonSocial,flibros.IFolio);

      
            var FechasExcel = SessionParaExcel.ObtenerObjetoExcel(flibros.Anio.ToString(), flibros.Mes.ToString(), flibros.FechaInicio, flibros.FechaFin, string.Empty);

            Session["FechasExcel"] = FechasExcel;
            Session["LibroCompra"] = Paginador.ResultStringArray;
     
            return View(Paginador);
        }


        [Authorize]
        [ModuloHandler]
        public ActionResult GetExcelLibroCompra()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

        
            //TO-DO: Verificar si vuelve el filtro entre fechas
            string strFechaInicio = string.Empty;//Request.Form.GetValues("fechainicio")[0];
            string strFechaFin = string.Empty;//Request.Form.GetValues("fechafin")[0];

            string tituloDocumento = string.Empty;

            List<string[]> cachedLibroCompra = Session["LibroCompra"] as List<string[]>;
            if (cachedLibroCompra != null)
            {
                for (int i = 0; i < cachedLibroCompra.Count; i++)
                {
                    cachedLibroCompra[i] = cachedLibroCompra[i].Select(r => r.Replace(".", "")).ToArray();
                }

                var FechasExcel = new SessionParaExcel();
                FechasExcel = (SessionParaExcel)Session["FechasExcel"];

                tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteLibroVentaCompra(FechasExcel.FechaInicio, FechasExcel.FechaFin, Convert.ToInt32(FechasExcel.Anio), Convert.ToInt32(FechasExcel.Mes), "LIBRO DE COMPRAS");
                var TieneFiltros = SessionParaExcel.TieneFiltrosActivos(FechasExcel);
                var cachedStream = AuxiliaresDetalleModel.ExportExcelLibroVentaCompraNormal(cachedLibroCompra, objCliente, true, TipoCentralizacion.Compra, tituloDocumento, strFechaInicio, strFechaFin, Convert.ToInt32(FechasExcel.Anio), Convert.ToInt32(FechasExcel.Mes), TieneFiltros); //AuxiliaresDetalleModel.ExportExcelLibroCentralizacionAuxiliaresVentaCompra(objCliente, true, TipoCentralizacion.Venta, tituloDocumento, strFechaInicio, strFechaFin, intAnio, intMes); //AGREGAR ACA FILTRO DE FECHAS 


                return File(cachedStream, "application/vnd.ms-excel", "LibroCompra" + Guid.NewGuid() + ".xlsx");
            }

            return View("LibroCompra");
        }

        //IMPORTER SECTION BEGIN
        //IMPORT DOCUMENTO XML?
        [Authorize]
        [ModuloHandler]
        public ActionResult ImportarDocumento()
        {
            return View();
        }

    
        //BALANCE
        [Authorize]
        [ModuloHandler]
        public ActionResult ImportarBalance()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ModuloHandler]
        public ActionResult ImportExcelABalance(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                string UserID = User.Identity.GetUserId();
                FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
                ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

                if (files != null && files.Count() > 0)
                {
                    HttpPostedFileBase file = files.ElementAt(0);
                    List<string[]> valoresImportados = ObtenerBalanceGeneralDesdeExcel(file);
                    if (valoresImportados != null && valoresImportados.Count > 0)
                    {
                        Session["filasCentralizacion"] = valoresImportados;
                        return RedirectToAction("IngresoVoucher", "Contabilidad", new { ncc = "1" });
                    }
                    else
                    {
                        return null;
                        //throw new NotImplementedException("No se pudo rescatar informacion del archivo dado.");
                    }
                }
                return null;
            }
            return null;
        }

        //IMPORT LIBRO SII
        [Authorize]
        // [ModuloHandler]
        public ActionResult CargarLibros()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        [ModuloHandler]
        public ActionResult ImportarLibroSIICentralizacion(IEnumerable<HttpPostedFileBase> files, DateTime fecont)
        {
            Session["InfoImportada"] = null;
            if (files != null)
            {
                string UserID = User.Identity.GetUserId();
                FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
                ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

                List<LibrosContablesModel> InfoImportada = new List<LibrosContablesModel>();
                var InfoImportadaTrue = new Tuple<List<LibrosContablesModel>, string>(InfoImportada,"");
                if (files != null && files.Count() > 0)
                {
                    HttpPostedFileBase file = files.ElementAt(0);
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileExtension = Path.GetExtension(file.FileName);

                        if (fileExtension == ".csv")
                        {

                            List<string[]> csv = ParseExtensions.ReadCSV(file, fecont);

                            //AAA PARAMETRIZAR EL TIPO DE CCENTRALIZACION RECIBIDA
              
                            InfoImportadaTrue = ProcesarLibroCentralizacion(csv, objCliente, db);

                            if (InfoImportadaTrue.Item2.Contains("Error"))
                            {
                                TempData["Error"] = "Ha ocurrido un error inesperado " + InfoImportadaTrue.Item2;
                                return RedirectToAction("CargarLibros", "Contabilidad");
                            }
                                
                            
                            //csv = ObtenerFilaDetalleDesdeImport(csv);
                            //Session["filasCentralizacion"] = csv;

                        }
                    }
                    Session["InfoImportada"] = InfoImportadaTrue.Item1;
                    
                    return RedirectToAction("InfoImportada", "Contabilidad");
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult CargarLibrosHonorarios()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            return View();
        }

        
        [Authorize]
        public JsonResult ImportarLibrosSIIHonorarios(IEnumerable<HttpPostedFileBase> files)
        {
            Session["InfoImportadaHonorarios"] = null;
            List<string[]> csv = new List<string[]>();
            StringBuilder Dibuja = new StringBuilder();

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            if (files != null)
            {

                List<LibroDeHonorariosModel> InfoImportadaHonorarios = new List<LibroDeHonorariosModel>();
                if (files != null && files.Count() > 0)
                {
                    HttpPostedFileBase file = files.ElementAt(0);
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileExtension = Path.GetExtension(file.FileName);

                        if (fileExtension == ".xls")
                        {
                             csv = ParseExtensions.ReadCSV(file);

                            foreach (string[] Tabla in csv)
                            {
                                for (int i = 0; i < Tabla.Count(); i++)
                                {
                                    Dibuja.Append(Tabla[i]);
                                }
                            }
                        }else
                        {
                            TempData["Error"] = "El archivo debe tener la extensión .xls";
                        }
                    }

                }
            }
            return Json(new { TablaHonorarios = Dibuja.ToString() });
        }


        [Authorize]
        public JsonResult ProcesarLibrosHonorarios(List<string[]> valores)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            List<LibroDeHonorariosModel> lstHonorarios = new List<LibroDeHonorariosModel>();

            string tipoReceptor = "H";

            bool Result = false;

            int CantidadNecesariaARecorrer = valores.Count() - 1;
            int CantidadDeItems = valores.Count() - 1; // Count() Parte del 1, para simular la cantidad de items de un array le restamos 1

            string FechaContabilizacion = valores[CantidadDeItems][0]; //Almacenamos la fecha de contabilizacion

          
            foreach (string[] Fila in valores.Take(CantidadDeItems))
            {
                DateTime FechaHonor = new DateTime();
                DateTime FechaAnulacion = new DateTime();

                int Num = ParseExtensions.ParseInt(Fila[0]);

                if (!string.IsNullOrWhiteSpace(Fila[1]))
                    FechaHonor = ParseExtensions.ToDD_MM_AAAA_Multi(Fila[1]);

                string Estado = Fila[2];

                if (!string.IsNullOrWhiteSpace(Fila[3]))
                    FechaAnulacion = ParseExtensions.ToDD_MM_AAAA_Multi(Fila[3]);

                string RutPrestador = Fila[4];
                string RazonSocialP = Fila[5];
                string Soc = Fila[6];
                decimal Brutos = ParseExtensions.ParseDecimal(Fila[7]);
                decimal Retencion = ParseExtensions.ParseDecimal(Fila[8]);
                decimal Pagado = ParseExtensions.ParseDecimal(Fila[9]);

                List<LibroDeHonorariosModel> SinRepetidos = db.DBLibroDeHonorarios.Where(x => x.ClientesContablesID == objCliente.ClientesContablesModelID &&
                                                                                              x.NumIdenficiador == Num &&
                                                                                              x.Prestador.RUT == RutPrestador &&
                                                                                              x.HaSidoConvertidoAVoucher == true &&
                                                                                              x.TipoLibro == TipoCentralizacion.Honorarios).ToList();

                List<VoucherModel> EstaVigenteEncontrado = new List<VoucherModel>();
                VoucherModel VoucherEncontrado = new VoucherModel();

                if (SinRepetidos != null || SinRepetidos.Count() > 0)
                {
                    foreach (var ItemRepetido in SinRepetidos)
                    {
                         VoucherEncontrado = db.DBVoucher.SingleOrDefault(x => x.VoucherModelID == ItemRepetido.VoucherModelID);

                        if(VoucherEncontrado.DadoDeBaja == false && VoucherEncontrado.Tipo == TipoVoucher.Traspaso)
                        {
                            EstaVigenteEncontrado.Add(VoucherEncontrado);
                        }
                    }
                }

                if (SinRepetidos.Count() > 0 && SinRepetidos != null && EstaVigenteEncontrado.Count() > 0)
                {
                    TempData["Error"] = "Ya existe la información que se intentó ingresar.";
                    Result = false;
                    continue;
                }
                else
                {

                    LibroDeHonorariosModel FilaAGuardar = new LibroDeHonorariosModel();

                    if (Estado != "NULA")
                    {

                        FilaAGuardar.ClientesContablesID = objCliente.ClientesContablesModelID;
                        FilaAGuardar.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
                        FilaAGuardar.TipoLibro = TipoCentralizacion.Honorarios;

                        FilaAGuardar.NumIdenficiador = Num;
                        FilaAGuardar.Fecha = FechaHonor;
                        FilaAGuardar.Estado = Estado;
                        FilaAGuardar.FechaAnulacion = FechaAnulacion;
                        FilaAGuardar.Rut = RutPrestador;
                        FilaAGuardar.RazonSocial = RazonSocialP;
                        FilaAGuardar.SocProf = Soc;
                        FilaAGuardar.Brutos = Brutos;
                        FilaAGuardar.Retenido = Retencion;
                        FilaAGuardar.Pagado = Pagado;

                        QuickReceptorModel objPrestador = QuickReceptorModel.CrearOActualizarPrestadorPorRut(RutPrestador, RazonSocialP, objCliente, db, tipoReceptor);

                        FilaAGuardar.Prestador = objPrestador;
                        FilaAGuardar.FechaContabilizacion = ParseExtensions.ToDD_MM_AAAA_Multi(FechaContabilizacion);


                        lstHonorarios.Add(FilaAGuardar);
                    }
                  }
                }

                if (lstHonorarios.Count() > 0)
                {
                    db.DBLibroDeHonorarios.AddRange(lstHonorarios);
                    db.SaveChanges();
                    Result = true;
                    Json(Result);
                    Session["LibroHonorariosAExportar"] = lstHonorarios;
                }
                else
                {
                    TempData["Error"] = "Ocurrió un error al procesar el libro, Revise la extensión y si existen registros.";
                }

                return Json(Result);
        }

        [Authorize]
        public ActionResult VistaPreviaLibroHonorario()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsStringWithSelectedCodInterno(objCliente, "410709");

            List<QuickReceptorModel> ReceptoresConRelacion = new List<QuickReceptorModel>();

            string TipoPrestador = "H";

            List<LibroDeHonorariosModel> lstVistaPrevia = Session["LibroHonorariosAExportar"] as List<LibroDeHonorariosModel>;

            foreach (LibroDeHonorariosModel ItemLibro in lstVistaPrevia)
            {
                QuickReceptorModel ReceptorEncontrado = db.Receptores.SingleOrDefault(x => x.RUT == ItemLibro.Prestador.RUT &&
                                                                                           x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                                           x.tipoReceptor == TipoPrestador);

                if (ReceptorEncontrado != null && ReceptorEncontrado.CuentaConToReceptor != null)
                {
                    ReceptoresConRelacion.Add(ReceptorEncontrado);
                }
            }

            if (ReceptoresConRelacion.Count() > 0)
            {
                ViewBag.lstReceptoresConCta = ReceptoresConRelacion;
                ViewBag.ObjCliente = objCliente;
            }

            return View(lstVistaPrevia);
        }


        [Authorize]
        public ActionResult ImportarLibroHonorAVoucher(IList<LibroDeHonorariosModel> lstImportado)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            List<LibroDeHonorariosModel> LibroAConvertir = new List<LibroDeHonorariosModel>();
            
            foreach (LibroDeHonorariosModel Verifica in lstImportado)
            {
                LibroDeHonorariosModel Verificado = db.DBLibroDeHonorarios.SingleOrDefault(x => x.LibroDeHonorariosModelID == Verifica.LibroDeHonorariosModelID && x.HaSidoConvertidoAVoucher == false &&
                                                                                                x.ClientesContablesID == objCliente.ClientesContablesModelID);
                
                LibroAConvertir.Add(Verificado);
             }

            string[] valuesCuentaContable = Request.Form.GetValues("Cuenta");

            if (valuesCuentaContable == null || valuesCuentaContable.Length == 0 || valuesCuentaContable.Length != lstImportado.Count() || valuesCuentaContable.Any(r => String.IsNullOrWhiteSpace(r)))
            {
                TempData["Error"] = "Falta por favor asignar una cuenta contable para todos los elementos";
                return RedirectToAction("CargarLibrosHonorarios", "Contabilidad");
            }

            List<CuentaContableModel> lstCuentaContable = new List<CuentaContableModel>();
            foreach (string strCuentaContable in valuesCuentaContable)
            {
                int keyCuentaContable = ParseExtensions.ParseInt(strCuentaContable);
                CuentaContableModel objCuentaContable = db.DBCuentaContable.Find(keyCuentaContable);
                lstCuentaContable.Add(objCuentaContable);
            }

            LibrosContablesModel.ProcesarLibroHonorarioAVoucher(LibroAConvertir, objCliente, db, lstCuentaContable);

            TempData["Correcto"] = "Libros de honorarios importados con éxito.";
            return RedirectToAction("CargarLibrosHonorarios","Contabilidad");
        }

        [Authorize]
        public ActionResult LibroDeHonorarios(FiltrosParaLibros flibros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            ViewBag.ObjClienteContable = objCliente;

            List<string[]> ReturnValues = new List<string[]>();
           //Definir filtros

            string TipoReceptor = "H";

            IQueryable<AuxiliaresDetalleModel> LibroHonorario = LibrosContablesModel.ObtenerLibrosPrestadores(objCliente, db, TipoReceptor, flibros.Mes, flibros.Anio, flibros.RazonSocial, flibros.Rut, flibros.FechaInicio, flibros.FechaFin,flibros.IFolio);

            int TotalRegistros = LibroHonorario.Count();

            if (flibros.cantidadRegistrosPorPagina != 0)
            {
                LibroHonorario = LibroHonorario.OrderBy(x => x.FechaContabilizacion)
                                         .Skip((flibros.pagina - 1) * flibros.cantidadRegistrosPorPagina)
                                         .Take(flibros.cantidadRegistrosPorPagina);
            }
            else if (flibros.cantidadRegistrosPorPagina == 0)
            {
                LibroHonorario = LibroHonorario.OrderBy(x => x.FechaContabilizacion);
            }

            int Correlativo = 1;

            decimal Bruto = 0;
            decimal Retencion = 0;
            decimal Neto = 0;

            foreach (AuxiliaresDetalleModel itemHonor in LibroHonorario.ToList())
            {
                string[] TablaHonorStrings = new string[] { "-", "-", "-", "-", "-", "-", "-", "-","-"};

                TablaHonorStrings[0] = Correlativo.ToString();
                TablaHonorStrings[1] = itemHonor.Folio.ToString();
                TablaHonorStrings[2] = itemHonor.FechaContabilizacion.ToString("dd-MM-yyyy");
                TablaHonorStrings[3] = itemHonor.Fecha.ToString("dd-MM-yyyy");
                TablaHonorStrings[4] = itemHonor.Individuo2.RUT;
                TablaHonorStrings[5] = itemHonor.Individuo2.RazonSocial;
                TablaHonorStrings[6] = ParseExtensions.NumeroConPuntosDeMiles(itemHonor.ValorLiquido);
                TablaHonorStrings[7] = ParseExtensions.NumeroConPuntosDeMiles(itemHonor.ValorRetencion);
                TablaHonorStrings[8] = ParseExtensions.NumeroConPuntosDeMiles(itemHonor.MontoTotalLinea);


  
                Bruto += itemHonor.ValorLiquido;
                Retencion += itemHonor.ValorRetencion;
                Neto += itemHonor.MontoTotalLinea;

                Correlativo++;

                ReturnValues.Add(TablaHonorStrings);
            }

            string[] Totales = new string[] { "-", "-", "-", "-", "-", "-", "-", "-","-"};
            Totales[5] = "TOTAL: ";
            Totales[6] = ParseExtensions.NumeroConPuntosDeMiles(Bruto);
            Totales[7] = ParseExtensions.NumeroConPuntosDeMiles(Retencion);
            Totales[8] = ParseExtensions.NumeroConPuntosDeMiles(Neto);

            ReturnValues.Add(Totales);
            
            var Paginacion = new PaginadorModel();
            Paginacion.ResultStringArray = ReturnValues;
            Paginacion.PaginaActual = flibros.pagina;
            Paginacion.TotalDeRegistros = TotalRegistros;
            Paginacion.RegistrosPorPagina = flibros.cantidadRegistrosPorPagina;
            Paginacion.ValoresQueryString = new RouteValueDictionary();

            if (flibros.cantidadRegistrosPorPagina != 25)
                Paginacion.ValoresQueryString["cantidadRegistrosPorPagina"] = flibros.cantidadRegistrosPorPagina;
            if (flibros.Mes != 0)
                Paginacion.ValoresQueryString["Mes"] = flibros.Mes;
            if (flibros.Anio != 0)
                Paginacion.ValoresQueryString["Anio"] = flibros.Anio; 
            if (!string.IsNullOrWhiteSpace(flibros.RazonSocial))
                Paginacion.ValoresQueryString["RazonSocial"] = flibros.RazonSocial;
            if (!string.IsNullOrWhiteSpace(flibros.Rut))
                Paginacion.ValoresQueryString["Rut"] = flibros.Rut;
            if (!string.IsNullOrWhiteSpace(flibros.FechaInicio) && !string.IsNullOrWhiteSpace(flibros.FechaFin))
            {
                Paginacion.ValoresQueryString["FechaInicio"] = flibros.FechaInicio;
                Paginacion.ValoresQueryString["FechaFin"] = flibros.FechaFin;
            }

            Session["LibroDeHonorarios"] = ReturnValues;
            var FechasExcel = SessionParaExcel.ObtenerObjetoExcel(flibros.Anio.ToString(), flibros.Mes.ToString(), flibros.FechaInicio, flibros.FechaFin, string.Empty);
            Session["FechasExcel"] = FechasExcel;

            return View(Paginacion);
        }

        [Authorize]
        public ActionResult LibroDeHonorariosTwo(int pagina = 1, int cantidadRegistrosPorPagina = 25, int Mes = 0, int Anio = 0, string RazonSocial = "", string Rut = "", string FechaInicio = "", string FechaFin = "", int Folio = 0)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            ViewBag.ObjClienteContable = objCliente;

            List<string[]> ReturnValues = new List<string[]>();
            //Definir filtros

            string TipoReceptor = "H";

            IQueryable<AuxiliaresDetalleModel> LibroHonorario = LibrosContablesModel.ObtenerLibrosPrestadores(objCliente, db, TipoReceptor, Mes, Anio, RazonSocial, Rut, FechaInicio, FechaFin, Folio);

            int TotalRegistros = LibroHonorario.Count();

            if (cantidadRegistrosPorPagina != 0)
            {
                LibroHonorario = LibroHonorario.OrderBy(x => x.FechaContabilizacion)
                                         .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                                         .Take(cantidadRegistrosPorPagina);
            }
            else if (cantidadRegistrosPorPagina == 0)
            {
                LibroHonorario = LibroHonorario.OrderBy(x => x.FechaContabilizacion);
            }

            int Correlativo = 1;

            decimal Bruto = 0;
            decimal Retencion = 0;
            decimal Neto = 0;

            foreach (AuxiliaresDetalleModel itemHonor in LibroHonorario.ToList())
            {
                string[] TablaHonorStrings = new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "-" };

                TablaHonorStrings[0] = Correlativo.ToString();
                TablaHonorStrings[1] = itemHonor.Folio.ToString();
                TablaHonorStrings[2] = itemHonor.FechaContabilizacion.ToString("dd-MM-yyyy");
                TablaHonorStrings[3] = itemHonor.Fecha.ToString("dd-MM-yyyy");
                TablaHonorStrings[4] = itemHonor.Individuo2.RUT;
                TablaHonorStrings[5] = itemHonor.Individuo2.RazonSocial;
                TablaHonorStrings[6] = ParseExtensions.NumeroConPuntosDeMiles(itemHonor.MontoBrutoLinea);
                TablaHonorStrings[7] = ParseExtensions.NumeroConPuntosDeMiles(itemHonor.ValorRetencion);
                TablaHonorStrings[8] = ParseExtensions.NumeroConPuntosDeMiles(itemHonor.MontoTotalLinea);

                Bruto += itemHonor.MontoBrutoLinea;
                Retencion += itemHonor.ValorRetencion;
                Neto += itemHonor.MontoTotalLinea;

                Correlativo++;

                ReturnValues.Add(TablaHonorStrings);
            }

            string[] Totales = new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "-" };
            Totales[5] = "TOTAL: ";
            Totales[6] = ParseExtensions.NumeroConPuntosDeMiles(Bruto);
            Totales[7] = ParseExtensions.NumeroConPuntosDeMiles(Retencion);
            Totales[8] = ParseExtensions.NumeroConPuntosDeMiles(Neto);

            ReturnValues.Add(Totales);

            var Paginacion = new PaginadorModel();
            Paginacion.ResultStringArray = ReturnValues;
            Paginacion.PaginaActual = pagina;
            Paginacion.TotalDeRegistros = TotalRegistros;
            Paginacion.RegistrosPorPagina = cantidadRegistrosPorPagina;
            Paginacion.ValoresQueryString = new RouteValueDictionary();

            if (cantidadRegistrosPorPagina != 25)
                Paginacion.ValoresQueryString["cantidadRegistrosPorPagina"] = cantidadRegistrosPorPagina;
            if (Mes != 0)
                Paginacion.ValoresQueryString["Mes"] = Mes;
            if (Anio != 0)
                Paginacion.ValoresQueryString["Anio"] = Anio;
            if (!string.IsNullOrWhiteSpace(RazonSocial))
                Paginacion.ValoresQueryString["RazonSocial"] = RazonSocial;
            if (!string.IsNullOrWhiteSpace(Rut))
                Paginacion.ValoresQueryString["Rut"] = Rut;
            if (!string.IsNullOrWhiteSpace(FechaInicio) && !string.IsNullOrWhiteSpace(FechaFin))
            {
                Paginacion.ValoresQueryString["FechaInicio"] = FechaInicio;
                Paginacion.ValoresQueryString["FechaFin"] = FechaFin;
            }

            Session["LibroDeHonorarios"] = ReturnValues;
            var FechasExcel = SessionParaExcel.ObtenerObjetoExcel(Anio.ToString(), Mes.ToString(), FechaInicio, FechaFin, string.Empty);
            Session["FechasExcel"] = FechasExcel;

            return View(Paginacion);
        }



        [Authorize]
        public ActionResult GetExcelLibroDeHonorarios()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string tituloDocumento = string.Empty;

            if (Session["LibroDeHonorarios"] != null)
            {
                List<string[]> cachedLibroHonorarios = Session["LibroDeHonorarios"] as List<string[]>;
                if (cachedLibroHonorarios != null)
                {
                    for (int i = 0; i < cachedLibroHonorarios.Count(); i++)
                    {
                        cachedLibroHonorarios[i] = cachedLibroHonorarios[i].Select(r => r.Replace(".", "")).ToArray();
                    }

                    var FechasExcel = new SessionParaExcel();
                    FechasExcel = (SessionParaExcel)Session["FechasExcel"];

                    //tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(Session["strBalanceGeneralFechaInicio"] as string, Session["strBalanceGeneralFechaFin"] as string, Session["strBalanceGeneralAnio"] as int?, Session["strBalanceGeneralMes"] as int?, "LIBRO MAYOR");
                    tituloDocumento = ParseExtensions.ObtenerFechaTextualMembreteReportes(FechasExcel.FechaInicio, FechasExcel.FechaFin, ParseExtensions.ParseInt(FechasExcel.Anio), ParseExtensions.ParseInt(FechasExcel.Mes), "LIBRO HONORARIOS");
                    var TieneFiltro = SessionParaExcel.TieneFiltrosActivos(FechasExcel);
                    var cachedStream = VoucherModel.GetExcelLibroHonorarios(cachedLibroHonorarios, objCliente, true, tituloDocumento,string.Empty, TieneFiltro);
                    return File(cachedStream, "application/vnd.ms-excel", "LibroHonorarios" + Guid.NewGuid() + ".xlsx");
                }
            }
            return null;
        }

        [Authorize]
        public ActionResult CargarLibrosHonorTerceros()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            return View();
        }

        [Authorize]
        public ActionResult ImportarLibrosSIIHonorTerceros(IEnumerable<HttpPostedFileBase> files, string fecont)
        {
            Session["InfoImportadaHonorarios"] = null;
            List<string[]> csv = new List<string[]>();
       
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            if (files != null)
            {
                List<LibroHonorariosDeTerceros> LibroProcesado = new List<LibroHonorariosDeTerceros>();
                if (files != null && files.Count() > 0)
                {
                    HttpPostedFileBase file = files.ElementAt(0);
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileExtension = Path.GetExtension(file.FileName);

                        if (fileExtension == ".csv")
                        {
                            csv = ParseExtensions.ReadCSV(file);
                            if(csv.Count() > 0)
                            {
                              LibroProcesado = LibroHonorariosDeTerceros.ProcesarLibroHonorariosTerceros(db,objCliente,objEmisor,csv,fecont);
                                if (LibroProcesado.Count() > 0) { 
                                    Session["InfoImportadaHonorariosTerceros"] = LibroProcesado;
                                    return RedirectToAction("LibrosHonorTercerosProcesAImportar", "Contabilidad");
                                }
                                else { 
                                    TempData["Error"] = "No hay elementos para importar O ya existe la información que intentas ingresar.";
                                    return RedirectToAction("CargarLibrosHonorTerceros", "Contabilidad");
                                }
                            }
                        }
                        else
                        {
                            TempData["Error"] = "El archivo debe tener la extensión .csv";
                            return RedirectToAction("CargarLibrosHonorTerceros", "Contabilidad");
                        }
                    }

                }
            }
            return null;
        }

        [Authorize]
        public ActionResult LibrosHonorTercerosProcesAImportar()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsStringWithSelectedCodInterno(objCliente, "410709");

            List<LibroHonorariosDeTerceros> ListaAimportar = (List<LibroHonorariosDeTerceros>)Session["InfoImportadaHonorariosTerceros"];

            return View(ListaAimportar);
        }

        [Authorize]
        public ActionResult LibrosHonorTerceroAVoucher(IList<LibroHonorariosDeTerceros> lstImportado)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<LibroHonorariosDeTerceros> lstExistenteAConvertir = new List<LibroHonorariosDeTerceros>();

            foreach (LibroHonorariosDeTerceros ItemABuscar in lstImportado)
            {
                LibroHonorariosDeTerceros Verificado = db.DBLibroHonorariosTerceros.SingleOrDefault(x => x.LibroHonorariosDeTercerosID == ItemABuscar.LibroHonorariosDeTercerosID &&
                                                                                                         x.HaSidoConvertidoAVoucher == false &&
                                                                                                         x.ClienteContable.ClientesContablesModelID == objCliente.ClientesContablesModelID);

                if (Verificado != null)
                    lstExistenteAConvertir.Add(Verificado);
            }

            string[] valuesCuentaContable = Request.Form.GetValues("Cuenta");

            if (valuesCuentaContable == null || valuesCuentaContable.Length == 0 || valuesCuentaContable.Length != lstExistenteAConvertir.Count() || valuesCuentaContable.Any(r => string.IsNullOrWhiteSpace(r)))
            {
                TempData["Error"] = "Falta por favor asignar una cuenta contable para todos los elementos";
                return RedirectToAction("CargarLibrosHonorTerceros", "Contabilidad");
            }

            List<CuentaContableModel> lstCuentaContable = new List<CuentaContableModel>();
            foreach (string strCuentaContable in valuesCuentaContable)
            {
                int keyCuentaContable = ParseExtensions.ParseInt(strCuentaContable);
                CuentaContableModel objCuentaContable = db.DBCuentaContable.Find(keyCuentaContable);
                lstCuentaContable.Add(objCuentaContable);
            }

            LibroHonorariosDeTerceros.ProcesarLibroHonorTerceroAVoucher(lstExistenteAConvertir, objCliente, db, lstCuentaContable);

            TempData["Correcto"] = "Libros de honorarios de terceros importados con éxito.";
            return RedirectToAction("CargarLibrosHonorTerceros", "Contabilidad");
        }

        [Authorize]
        public ActionResult CatorceTer(int pagina = 1, int cantidadRegistrosPorPagina = 25, string FechaInicio = "", string FechaFin = "", int Anio = 0, int Mes = 0, string Rut = "", string RazonSocial = "", int Folio = 0)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var lstCatorceTer = new PaginadorModel();
            lstCatorceTer = CatorceTerViewModel.GetCatorceTer(db,objCliente, FechaInicio, FechaFin, Anio, Mes, pagina, cantidadRegistrosPorPagina, Rut, RazonSocial, Folio);

          
            SessionParaExcel Fechas = new SessionParaExcel();

            Session["FechasCatorceTer"] = null;
            if(Anio > 0 || Mes > 0) { 

                if(Anio > 0) { 
                    Fechas.Anio = Anio.ToString();
                }
                if (Mes > 0) { 
                    Fechas.Mes = Mes.ToString();
                }

                Session["FechasCatorceTer"] = Fechas;
            }

            Session["CatorceTer"] = lstCatorceTer.LstCatorceTer;

            return View(lstCatorceTer);
        }

        [Authorize]
        public ActionResult GetExcelCatorceTer()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            if(Session["CatorceTer"] != null) { 
            List<CatorceTerViewModel> lstAExportar = (List<CatorceTerViewModel>)Session["CatorceTer"];
                var Fechas14Ter = new SessionParaExcel();
                if(Session["FechasCatorceTer"] != null) { 
                      Fechas14Ter = (SessionParaExcel)Session["FechasCatorceTer"];
                }


                var cachedStream = CatorceTerViewModel.GetExcelCatorceTer(lstAExportar, objCliente, true, Fechas14Ter);
                return File(cachedStream, "application/vnd.ms-excel", "14Ter" + Guid.NewGuid() + ".xlsx");
            }

            return null;
        }

        [Authorize]
        public ActionResult EstadoResultado(FiltrosEstadoResultado Filtros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            IQueryable<FatherObjEstadoResultado> Query = EstadoResultadoViewModel.QueryEstadoResultado(db,  objCliente);
            IQueryable<FatherObjEstadoResultado> QueryFiltrado = EstadoResultadoViewModel.GetEstadoResultadoFiltrado(Query, Filtros);
            List<EstadoResultadoViewModel> ReporteProcesado = EstadoResultadoViewModel.EstadoResultadoProcesado(QueryFiltrado);

            decimal Ganancias = ReporteProcesado.Where(x => x.Clasificacion == ClasificacionCtaContable.RESULTADOGANANCIA).Sum(x => x.Monto);
            decimal Perdidas = ReporteProcesado.Where(x => x.Clasificacion == ClasificacionCtaContable.RESULTADOPERDIDA).Sum(x => x.Monto);
            decimal Resultado = Math.Abs(Ganancias) - Math.Abs(Perdidas);

            //SUMAS
            ViewBag.TotalGanancias = Ganancias;
            ViewBag.TotalPerdidas = Perdidas;

            //RESULTADO
            if(Ganancias > Perdidas)
            {
                ViewBag.Resultado = "RESULTADO GANANCIA";
                ViewBag.TotalesGanancias = ParseExtensions.NumeroConPuntosDeMiles(Ganancias);
                decimal Total = Math.Abs(Perdidas) + Math.Abs(Resultado);
                ViewBag.TotalesPerdidas = ParseExtensions.NumeroConPuntosDeMiles(Total);
            }
            else if(Perdidas > Ganancias)
            {
                ViewBag.Resultado = "RESULTADO PERDIDA";
                ViewBag.TotalesPerdidas = ParseExtensions.NumeroConPuntosDeMiles(Perdidas);
                decimal Total = Math.Abs(Ganancias) + Math.Abs(Resultado);
                ViewBag.TotalesGanancias = ParseExtensions.NumeroConPuntosDeMiles(Total);
            }

            ViewBag.ResultadoMonto = ParseExtensions.NumeroConPuntosDeMiles(Math.Abs(Resultado));
         
            return View(ReporteProcesado);
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult ImportaCartola()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            return View();
        }

        [Authorize]
        public ActionResult ParcialSaldoLibro(string IDCuentaContable, string FechaInicio = "", string FechaFin = "")
        {
            bool ConversionFechaInicioExitosa = false;
            DateTime dtFechaInicio = new DateTime();
            bool ConversionFechaFinExitosa = false;
            DateTime dtFechaFin = new DateTime();
            if (string.IsNullOrWhiteSpace(FechaInicio) == false/* && string.IsNullOrWhiteSpace(FechaFin) == false*/)
            {
                ConversionFechaInicioExitosa = DateTime.TryParse(FechaInicio, out dtFechaInicio);
                if (ConversionFechaInicioExitosa == true)
                {
                    int diasEnEsteMes = DateTime.DaysInMonth(dtFechaInicio.Year, dtFechaInicio.Month);
                    dtFechaFin = new DateTime(dtFechaInicio.Year, dtFechaInicio.Month, diasEnEsteMes);
                    ConversionFechaFinExitosa = true;
                }
                //ConversionFechaFinExitosa = DateTime.TryParse(FechaFin, out dtFechaFin);
            }

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<VoucherModel> lstVoucherModel = objCliente.ListVoucher.ToList();
            CuentaContableModel singleCuentaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID.ToString() == IDCuentaContable);

            List<string[]> returnValue = new List<string[]>();
            if (singleCuentaContable == null)
                return PartialView(returnValue);
            if (ConversionFechaFinExitosa && ConversionFechaInicioExitosa)
            {
                List<string[]> preRetorno = VoucherModel.GetLibroMayor(lstVoucherModel, db, singleCuentaContable, dtFechaInicio, dtFechaFin);
                if (preRetorno.Count > 0)
                    preRetorno.RemoveAt(preRetorno.Count() - 1);

                decimal totalDebe = lstVoucherModel.Where(rt => rt.FechaEmision >= dtFechaInicio && rt.FechaEmision <= dtFechaFin).Select(r => r.ListaDetalleVoucher.Where(i => i.ObjCuentaContable.CodInterno == singleCuentaContable.CodInterno).Sum(f => f.MontoDebe)).Sum();
                decimal totalHaber = lstVoucherModel.Where(rt => rt.FechaEmision >= dtFechaInicio && rt.FechaEmision <= dtFechaFin).Select(r => r.ListaDetalleVoucher.Where(i => i.ObjCuentaContable.CodInterno == singleCuentaContable.CodInterno).Sum(f => f.MontoHaber)).Sum();
                ViewBag.totalDebe = totalDebe;
                ViewBag.totalHaber = totalHaber;

                returnValue = preRetorno;
            }
            else
            {
                List<string[]> preRetorno = VoucherModel.GetLibroMayor(lstVoucherModel, db, singleCuentaContable);
                if (preRetorno.Count > 0)
                    preRetorno.RemoveAt(preRetorno.Count() - 1);

                decimal totalDebe = lstVoucherModel.Select(r => r.ListaDetalleVoucher.Where(i => i.ObjCuentaContable.CodInterno == singleCuentaContable.CodInterno).Sum(f => f.MontoDebe)).Sum();
                decimal totalHaber = lstVoucherModel.Select(r => r.ListaDetalleVoucher.Where(i => i.ObjCuentaContable.CodInterno == singleCuentaContable.CodInterno).Sum(f => f.MontoHaber)).Sum();

                ViewBag.totalDebe = totalDebe;
                ViewBag.totalHaber = totalHaber;

                returnValue = preRetorno;
            }

            return PartialView(returnValue);
        }

   
       
      
        [Authorize]
        public static List<string[]> ObtenerFilaDetalleDesdeImport(List<string[]> csvInput)
        {
            //TODO : Add transformation of DTE to make document thingies more descriptive
            List<string[]> lstRetornoSimplificado = new List<string[]>();
            decimal sumatoriaIva = 0;

            if (csvInput.Count > 0)
                csvInput.RemoveAt(0);

            foreach (string[] csvFileInputRow in csvInput)
            {
                decimal montoExentoTransform = 0;
                decimal montoNetoTransform = 0;

                //8 exento
                //9 neto
                decimal.TryParse(csvFileInputRow[8], out montoExentoTransform);
                decimal.TryParse(csvFileInputRow[9], out montoNetoTransform);

                string[] row = {
                    csvFileInputRow[6],
                    "0",
                    "Doc tipo:"+csvFileInputRow[0] + "/ F: " + csvFileInputRow[1] + " "+csvFileInputRow[4],
                    (montoExentoTransform + montoNetoTransform).ToString(),
                    "0"};

                //Ir obteniendo el total de los montos IVA para agregarlos como otra linea de detalle del voucher
                decimal montoIvaTransform = 0;
                decimal.TryParse(csvFileInputRow[10], out montoIvaTransform);
                sumatoriaIva += montoIvaTransform;

                lstRetornoSimplificado.Add(row);
            }
            if (lstRetornoSimplificado.Count > 0)
            {
                string[] rowTax =
                {
                    lstRetornoSimplificado[0][0],
                    "0",
                    "Sumatoria montos IVA",
                    sumatoriaIva.ToString(),
                    "0"
                };
                lstRetornoSimplificado.Add(rowTax);
            }
            return lstRetornoSimplificado;
        }
      

        [Authorize]
        public static List<string[]> ObtenerBalanceGeneralDesdeExcel(HttpPostedFileBase file)
        {
            List<string[]> returnValue = new List<string[]>();
            if (file != null && file.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {

                    using (XLWorkbook excelFile = new XLWorkbook(file.InputStream))
                    {
                        var workSheet = excelFile.Worksheet(1);
                        int StartRow = 3;

                        var row = workSheet.Row(StartRow);
                        while (row.IsEmpty() == false)
                        {
                            string Glosa = (string)row.Cell(1).Value;
                            string CondicionPareArtificial = Glosa.ToUpperInvariant();
                            if (CondicionPareArtificial == "SUMAS" || CondicionPareArtificial == "UTILIDADES DEL EJERCICIO"
                                || CondicionPareArtificial == "TOTALES")
                            {
                                break;
                            }

                            double MontoDebe = (double)row.Cell(6).Value;
                            double MontoHaber = (double)row.Cell(7).Value;
                            if (MontoDebe == 0 && MontoHaber != 0)
                            {
                                string[] valueToAdd = {
                                                ParseExtensions.ToDD_MM_AAAA(DateTime.Now),
                                                "0", Glosa, "0", MontoHaber.ToString() };
                                returnValue.Add(valueToAdd);

                            }
                            else if (MontoHaber == 0 && MontoDebe != 0)
                            {
                                string[] valueToAdd = {
                                                ParseExtensions.ToDD_MM_AAAA(DateTime.Now),
                                                "0", Glosa, MontoDebe.ToString(), "0" };
                                returnValue.Add(valueToAdd);
                            }
                            else if (MontoHaber != 0 && MontoDebe != 0)
                            {
                                throw new NotImplementedException("Estado inventario tiene montos distintos de 0 en ambos estados");
                            }
                            else
                            {

                            }
                            /*
                            if (MontoDebe != 0)
                            {
                                string[] valueToAdd = {
                                                ParseExtensions.ToDD_MM_AAAA(DateTime.Now),
                                                "0", Glosa, MontoDebe.ToString(), "0" };
                                returnValue.Add(valueToAdd);
                            }

                            if (MontoHaber != 0)
                            {
                                string[] valueToAdd = {
                                                ParseExtensions.ToDD_MM_AAAA(DateTime.Now),
                                                "0", Glosa, "0", MontoHaber.ToString() };
                                returnValue.Add(valueToAdd);
                            }*/
                            StartRow++;
                            row = workSheet.Row(StartRow);
                        }
                        return returnValue;
                    }

                }
                return returnValue;
            }
            else
                return returnValue;
        }

        [Authorize]
        public  static Tuple<List<LibrosContablesModel>, string>  ProcesarLibroCentralizacion(List<string[]> csvInput, ClientesContablesModel objCliente, FacturaPoliContext db)
        {
            //Si la data del CSV no contiene nada regresar
            if (csvInput == null || csvInput.Count == 0)
                return null;
            //Variable que contendra la importacion de la data como modelo
            // Obtiene todos los registros sin excepción
            List<LibrosContablesModel> lstLibroContableCentralizacionImpuestos = new List<LibrosContablesModel>(); //Obtendrá todos los impuestos.
            List<LibrosContablesModel> lstLibroContableCentralizacionImpuestosLimpios = new List<LibrosContablesModel>(); //Sumamos todos los exentos de los repetidos y dejamos 1 solo registro
            List<LibrosContablesModel> lstLibroContableCentralizacionAretornar = new List<LibrosContablesModel>(); // Lista que obtendrá todos los registros limpios y listos para retornar.

            TipoCentralizacion tipoCentralizacion = TipoCentralizacion.Ninguno;
            //Esto es para compras.
            //Datos a duplicar
            
            TipoDte TipoDocDuple = 0; //[1]
            var FolioDuple = ""; //[5]
            var ClienteContable = objCliente.ClientesContablesModelID;
            var RutDupleDuple = ""; //[3]
            var RazonSocialDuple = ""; //[4]
            var FechaDocumentoDuple = ""; //[6]
            var FechaRecepcionDuple = ""; //[7]
            TipoCentralizacion TipoLibro = TipoCentralizacion.Ninguno;
            string tipoReceptor = "";

            decimal MontoExentoDuple = 0; //[9]
            var MontoNetoDuple = ""; //[10]
            var MontoIvaDuple = ""; //[11]
            var MontoTotalDuple = ""; //[14]

            var MontoIvaNorecuperable = ""; //[12]
            var MontoIvaUsoComun = ""; //[17]
            var MontoIvaActivoFijo = ""; //[16]

           using(var dbContextTransaction = db.Database.BeginTransaction())
           {
                try
                {
                    var VerificaRepetidos = ContabilidadHelper.VerificaRepetidosEnExcelImportSIICoV(csvInput, objCliente, db);
                    if (!string.IsNullOrWhiteSpace(VerificaRepetidos))
                    {
                        throw new Exception("Error Algunos de los datos que intentas exportar ya existen en tus libros. Los siguientes folios están repetidos:  " + VerificaRepetidos);
                    }
                    foreach (string[] strFilaCSV in csvInput)
                    {
                        if (csvInput.First() == strFilaCSV)
                        {

                            if (strFilaCSV[2] == "Tipo Compra")
                            {
                                tipoCentralizacion = TipoCentralizacion.Compra;
                                tipoReceptor = "PR";
                            }
                            else if (strFilaCSV[2] == "Tipo Venta")
                            {
                                tipoCentralizacion = TipoCentralizacion.Venta;
                                tipoReceptor = "CL";
                            }
                            else
                            {
                                //MANEJAR ERROR ACA, NO SE PUEDE MANEJAR UN LIBRO QUE NO SEA DE VENTA O COMPRA
                                return null;
                            }

                            continue;
                        }
                        //Crear nuevo doc centralizacion
                        LibrosContablesModel NewLibroContableModel = new LibrosContablesModel();

                        //Si corresponde a un libro de VENTA o COMPRA (¿O Honorarios?)
                        NewLibroContableModel.TipoLibro = tipoCentralizacion;

                        //Asignar a este cliente
                        NewLibroContableModel.ClientesContablesModelID = objCliente.ClientesContablesModelID;


                        NewLibroContableModel.TipoDocumento = (TipoDte)ParseExtensions.ParseInt(strFilaCSV[1]);


                        //Razon social (CLIENTE / PROVEEDOR) y RUT se transforman en un objeto de tipo Prestador
                        string RutPrestador = strFilaCSV[3];
                        string RazonSocialPrestador = strFilaCSV[4];

                        if (!string.IsNullOrWhiteSpace(strFilaCSV[0]))
                        {
                            RutDupleDuple = strFilaCSV[3];
                            RazonSocialDuple = strFilaCSV[4];
                        }
                        //Aquí construimos la fila que copiaremos.
                        if (string.IsNullOrWhiteSpace(strFilaCSV[0]) && !string.IsNullOrWhiteSpace(strFilaCSV[25]))
                        {
                            QuickReceptorModel objPrestador = QuickReceptorModel.CrearOActualizarPrestadorPorRut(RutDupleDuple, RazonSocialDuple, objCliente, db, tipoReceptor);

                            NewLibroContableModel.individuo = objPrestador;
                        }



                        if (!string.IsNullOrWhiteSpace(strFilaCSV[0]))
                        {
                            //AuxiliaresPrestadoresModel objPrestador = AuxiliaresPrestadoresModel.CrearOActualizarPrestadorPorRut(RutPrestador, RazonSocialPrestador, objCliente, db);
                            QuickReceptorModel objPrestador = QuickReceptorModel.CrearOActualizarPrestadorPorRut(RutPrestador, RazonSocialPrestador, objCliente, db, tipoReceptor);

                            // NewLibroContableModel.Prestador = objPrestador;
                            NewLibroContableModel.individuo = objPrestador;

                        }

                        NewLibroContableModel.Folio = ParseExtensions.ParseInt(strFilaCSV[5]);

                        // Volver a activar después de hacer las pruebas.
                        //Errores en caso de que ya exista el libro y haya sido dado de alta.
                        if (!string.IsNullOrWhiteSpace(strFilaCSV[0]))
                        {
                            //FECHA DE DOCUMENTO
                            NewLibroContableModel.FechaDoc = ParseExtensions.ToDD_MM_AAAA_Multi(strFilaCSV[6]);

                            //FECHA RECEPCION
                            NewLibroContableModel.FechaRecep = ParseExtensions.ToDD_MM_AAAA_Multi(strFilaCSV[7]);
                        }
                        //Siempre parece estar vacia en los documentos del SII

                        //Montos documento contable

                        if (!string.IsNullOrWhiteSpace(strFilaCSV[0]))
                        {
                            TipoDocDuple = (TipoDte)ParseExtensions.ParseInt(strFilaCSV[1]);

                            FolioDuple = strFilaCSV[5];
                            FechaDocumentoDuple = strFilaCSV[6];
                            FechaRecepcionDuple = strFilaCSV[7];
                            TipoLibro = tipoCentralizacion;
                        }

                        if (string.IsNullOrWhiteSpace(strFilaCSV[0]) && !string.IsNullOrWhiteSpace(strFilaCSV[25]))
                        {
                            NewLibroContableModel.TipoDocumento = TipoDocDuple;
                            NewLibroContableModel.Folio = ParseExtensions.ParseInt(FolioDuple);
                            NewLibroContableModel.FechaDoc = ParseExtensions.ToDD_MM_AAAA_Multi(FechaDocumentoDuple);
                            NewLibroContableModel.FechaRecep = ParseExtensions.ToDD_MM_AAAA_Multi(FechaRecepcionDuple);
                        }

                        // Para fecha de contabilización tanto en compra como en venta
                        int CantidadDeFilas = strFilaCSV.Count();
                        int NumeroIndiceVariable = CantidadDeFilas - 1;

                        if (tipoCentralizacion == TipoCentralizacion.Venta)
                        {
                            if (!string.IsNullOrWhiteSpace(strFilaCSV[NumeroIndiceVariable]))
                            {
                                NewLibroContableModel.FechaContabilizacion = ParseExtensions.ToDD_MM_AAAA(strFilaCSV[NumeroIndiceVariable]);
                            }


                            NewLibroContableModel.MontoExento = ParseExtensions.ParseDecimal(strFilaCSV[10]);
                            NewLibroContableModel.MontoNeto = ParseExtensions.ParseDecimal(strFilaCSV[11]);
                            NewLibroContableModel.MontoIva = ParseExtensions.ParseDecimal(strFilaCSV[12]);
                            NewLibroContableModel.MontoTotal = ParseExtensions.ParseDecimal(strFilaCSV[13]);
                        }
                        else if (tipoCentralizacion == TipoCentralizacion.Compra)
                        {

                            if (!string.IsNullOrWhiteSpace(strFilaCSV[0]))
                            {
                                MontoExentoDuple = ParseExtensions.ParseDecimal(strFilaCSV[25]);
                                MontoNetoDuple = strFilaCSV[10];
                                MontoIvaDuple = strFilaCSV[11];
                                MontoTotalDuple = strFilaCSV[14];
                                MontoIvaNorecuperable = strFilaCSV[12];
                                MontoIvaUsoComun = strFilaCSV[17];
                                MontoIvaActivoFijo = strFilaCSV[16];
                            }
                            //Duplicamos las filas para que queden iguales para luego dejar al que tenga la suma del exento mayor
                            if (string.IsNullOrWhiteSpace(strFilaCSV[0]) && !string.IsNullOrWhiteSpace(strFilaCSV[25]))
                            {
                                NewLibroContableModel.MontoExento = ParseExtensions.ParseDecimal(strFilaCSV[9]);
                                NewLibroContableModel.MontoNeto = ParseExtensions.ParseDecimal(MontoNetoDuple);
                                NewLibroContableModel.MontoIva = ParseExtensions.ParseDecimal(MontoIvaDuple);
                                NewLibroContableModel.MontoTotal = ParseExtensions.ParseDecimal(MontoTotalDuple);

                                NewLibroContableModel.MontoIvaNoRecuperable = ParseExtensions.ParseDecimal(MontoIvaNorecuperable);
                                NewLibroContableModel.MontoIvaUsocomun = ParseExtensions.ParseDecimal(MontoIvaUsoComun);
                                NewLibroContableModel.MontoIvaActivoFijo = ParseExtensions.ParseDecimal(MontoIvaActivoFijo);
                            }



                            if (!string.IsNullOrWhiteSpace(strFilaCSV[0]))
                            {
                                NewLibroContableModel.MontoExento = ParseExtensions.ParseDecimal(strFilaCSV[9]);
                                NewLibroContableModel.MontoNeto = ParseExtensions.ParseDecimal(strFilaCSV[10]);
                                NewLibroContableModel.MontoIva = ParseExtensions.ParseDecimal(strFilaCSV[11]);
                                NewLibroContableModel.MontoTotal = ParseExtensions.ParseDecimal(strFilaCSV[14]);

                                NewLibroContableModel.MontoIvaNoRecuperable = ParseExtensions.ParseDecimal(strFilaCSV[12]);
                                NewLibroContableModel.MontoIvaUsocomun = ParseExtensions.ParseDecimal(strFilaCSV[17]);
                                NewLibroContableModel.MontoIvaActivoFijo = ParseExtensions.ParseDecimal(strFilaCSV[16]);
                            }
                            if (string.IsNullOrWhiteSpace(strFilaCSV[20]) != true)
                            {
                                decimal tabacoPuros = 0;
                                decimal tabacosPurosMasMontoExento = 0;
                                tabacoPuros = ParseExtensions.ParseDecimal(strFilaCSV[20]);

                                tabacosPurosMasMontoExento = ParseExtensions.ParseDecimal(strFilaCSV[9]);

                                NewLibroContableModel.MontoExento = (tabacosPurosMasMontoExento + tabacoPuros);
                            }
                            if (string.IsNullOrWhiteSpace(strFilaCSV[21]) != true)
                            {
                                decimal tabacosCigarrillos = 0;
                                decimal tabacosCigarrillosMasMontoExento = 0;
                                tabacosCigarrillos = ParseExtensions.ParseDecimal(strFilaCSV[21]);

                                tabacosCigarrillosMasMontoExento = ParseExtensions.ParseDecimal(strFilaCSV[9]);

                                NewLibroContableModel.MontoExento = (tabacosCigarrillosMasMontoExento + tabacosCigarrillos);
                            }
                            if (string.IsNullOrWhiteSpace(strFilaCSV[22]) != true)
                            {
                                decimal tabacosElaborados = 0;
                                decimal tabacosTabacosElaMasMontoExento = 0;
                                tabacosElaborados = ParseExtensions.ParseDecimal(strFilaCSV[22]);

                                tabacosTabacosElaMasMontoExento = ParseExtensions.ParseDecimal(strFilaCSV[9]);

                                NewLibroContableModel.MontoExento = (tabacosTabacosElaMasMontoExento + tabacosElaborados);
                            }
                            if (string.IsNullOrWhiteSpace(strFilaCSV[25]) != true)
                            {
                                decimal valorOtrosImpuestos = 0;
                                decimal valorOtrosImpMasMontoExento = 0;
                                valorOtrosImpuestos = ParseExtensions.ParseDecimal(strFilaCSV[25]);

                                valorOtrosImpMasMontoExento = ParseExtensions.ParseDecimal(strFilaCSV[9]);

                                NewLibroContableModel.MontoExento = (valorOtrosImpMasMontoExento + valorOtrosImpuestos);
                            }
                            if (string.IsNullOrWhiteSpace(strFilaCSV[0]) && !string.IsNullOrWhiteSpace(strFilaCSV[25])) // Esta regla se aplica para las lineas de impuestos adcionales, se usa para acumular el exento.
                            {
                                decimal valorOtrosImpuestos = 0;
                                decimal valorOtrosImpMasMontoExento = 0;
                                valorOtrosImpuestos = ParseExtensions.ParseDecimal(strFilaCSV[25]);

                                valorOtrosImpMasMontoExento = ParseExtensions.ParseDecimal(strFilaCSV[9]);

                                NewLibroContableModel.MontoExento = (valorOtrosImpMasMontoExento + valorOtrosImpuestos);
                            }

                            if (!string.IsNullOrWhiteSpace(strFilaCSV[NumeroIndiceVariable]))
                            {
                                NewLibroContableModel.FechaContabilizacion = ParseExtensions.ToDD_MM_AAAA(strFilaCSV[NumeroIndiceVariable]);
                            }


                        }

                        NewLibroContableModel.estado = true;


                        //AGREGAR ACA IVAS RETENIDOS EN EL FUTURO DE SER NECESARIO

                        //REFERENCIA DEL DOCUMENTO CONTABLE
                        if (string.IsNullOrWhiteSpace(strFilaCSV[24]) == false && strFilaCSV[24] != "0")
                        {
                            NewLibroContableModel.TipoDocReferencia = (TipoDte)ParseExtensions.ParseInt(strFilaCSV[24]);
                            NewLibroContableModel.FolioDocReferencia = ParseExtensions.ParseInt(strFilaCSV[25]);
                        }

                        //Agregamos lo necesario 


                        //Caso 1: Recibe Todos para poder evaluar.


                        if (tipoCentralizacion == TipoCentralizacion.Venta)
                        {
                            lstLibroContableCentralizacionAretornar.Add(NewLibroContableModel);
                        }

                        if (tipoCentralizacion == TipoCentralizacion.Compra)
                        {

                            if (string.IsNullOrWhiteSpace(strFilaCSV[0]) && !string.IsNullOrWhiteSpace(strFilaCSV[25]))
                            {
                                lstLibroContableCentralizacionImpuestos.Add(NewLibroContableModel);
                            }

                            //Caso 3: Recibe todos aquellos que no contienen impuestos
                            if (!string.IsNullOrWhiteSpace(strFilaCSV[0]) && string.IsNullOrWhiteSpace(strFilaCSV[25]))
                            {
                                lstLibroContableCentralizacionAretornar.Add(NewLibroContableModel);
                            }
                            //Caso 4: Recibe todos los que tienen impuestos más no las lineas hijas de estos impuestos.
                            if (!string.IsNullOrWhiteSpace(strFilaCSV[0]) && !string.IsNullOrWhiteSpace(strFilaCSV[25]))
                            {
                                lstLibroContableCentralizacionImpuestos.Add(NewLibroContableModel);
                            }
                        }
                    }

                    if (lstLibroContableCentralizacionImpuestos != null && tipoCentralizacion == TipoCentralizacion.Compra)
                    {
                        //Si no está vacio unelo con los hijos.

                        //En caso de necesitar comparar más atributos para mayor seguridad, añadirlo en estas query y en la query del foreach.
                        var TodosLosImpuestos = lstLibroContableCentralizacionImpuestos.Select(x => new { x.Folio, x.MontoIva, x.MontoNeto, x.MontoTotal });

                        TodosLosImpuestos = TodosLosImpuestos.Distinct().ToList();
                        foreach (var item in TodosLosImpuestos)
                        {
                            List<LibrosContablesModel> ResultOneTaxe = lstLibroContableCentralizacionImpuestos.Where(x => x.Folio == item.Folio &&
                                                                                                                          x.MontoNeto == item.MontoNeto &&
                                                                                                                          x.MontoIva == item.MontoIva &&
                                                                                                                          x.MontoTotal == item.MontoTotal).ToList();
                            //Codigo Que los Relacionará
                            Random ObjCodigoUnion = new Random();
                            int CodigoUnion = ObjCodigoUnion.Next();

                            decimal MontoSumado = ResultOneTaxe.Sum(x => x.MontoExento);

                            foreach (LibrosContablesModel item2 in ResultOneTaxe)
                            {
                                ImpuestosAdRelacionModel InsertarImpuesto = new ImpuestosAdRelacionModel();

                                InsertarImpuesto.CodigoUnionImpuesto = CodigoUnion;
                                InsertarImpuesto.CodigoImpuesto = Convert.ToInt32(item2.TipoDocReferencia);
                                InsertarImpuesto.ClienteContableModelID = objCliente.ClientesContablesModelID;

                                List<ImpuestosAdicionalesModel> lstImpuestos = db.DBImpuestosAdicionalesSII.ToList();
                                ImpuestosAdicionalesModel BuscandoID = lstImpuestos.SingleOrDefault(x => x.CodigoImpuesto == InsertarImpuesto.CodigoImpuesto);

                                if (BuscandoID != null)
                                {
                                    InsertarImpuesto.ImpuestosAdicionalesModelID = BuscandoID.ImpuestosAdicionalesModelID;
                                }

                                db.DBImpuestosAdRelacionSII.Add(InsertarImpuesto);
                                db.SaveChanges();

                                item2.MontoExento = MontoSumado;
                                item2.CodigoUnionImpuesto = CodigoUnion;
                            }

                        }

                        foreach (var item in TodosLosImpuestos)
                        {
                            LibrosContablesModel ResultOneTaxe = lstLibroContableCentralizacionImpuestos.Where(x => x.Folio == item.Folio &&
                                                                                                                    x.MontoNeto == item.MontoNeto &&
                                                                                                                    x.MontoIva == item.MontoIva &&
                                                                                                                    x.MontoTotal == item.MontoTotal).FirstOrDefault();
                            lstLibroContableCentralizacionImpuestosLimpios.Add(ResultOneTaxe);
                        }
                        lstLibroContableCentralizacionAretornar.AddRange(lstLibroContableCentralizacionImpuestosLimpios);

                    }


                    foreach (LibrosContablesModel DetalleLibro in lstLibroContableCentralizacionAretornar)
                    {
                        db.DBLibrosContables.Add(DetalleLibro);
                    }

                    // db.SaveChanges();
                    int NumFilasAlteradas = db.SaveChanges();


                    if (NumFilasAlteradas == 0)
                    {
                       throw new Exception("Error inesperado no se pudo procesar el excel.");
                    }
                    else
                    {
                        dbContextTransaction.Commit();
                        return Tuple.Create(lstLibroContableCentralizacionAretornar, "Exito");
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Tuple.Create(lstLibroContableCentralizacionAretornar, ex.Message);
                }
            }
        }



 

        //IMPORTER SECTION END

        /// <summary>
        /// Obtiene subclasificacion de cuenta contable
        /// </summary>
        /// <param name="companiaAutorizada">Variable out que entrega la coleccion de compañias que este usuario puede operar</param>
        /// <param name="objCliente">Variable con la informacion cliente</param>
        /// <param name="lstSubClass">Lista de subclasificaciones por una clasificacion de cuentas contables</param>
        /// <returns>TRUE si logra obtener lista, FALSE si no lo logra</returns>
        [Authorize]
        public JsonResult ObtenerSubclasificacion(int Clasificacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<SubClasificacionCtaContable> lstSubClass = objCliente.CtaContable.ToList().Select(m => m.SubClasificacion).Where(r => r.CodigoInterno.StartsWith(Clasificacion.ToString())).ToList();
            lstSubClass = lstSubClass.GroupBy(r => r.SubClasificacionCtaContableID).Select(r => r.First()).ToList();
            var SubClasificaciones = lstSubClass.Select(r => new { Value = r.SubClasificacionCtaContableID, Text = r.GetSubClasificacionDisplaySTR() });
            return Json(SubClasificaciones, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Obtiene subsubclasificacion de cuenta contable
        /// </summary>
        /// <param name="companiaAutorizada">Variable out que entrega la coleccion de compañias que este usuario puede operar</param>
        /// <param name="objCliente">Variable con la informacion cliente</param>
        /// <param name="listaSubsub">Lista de subsubclasificaciones por una subclasificacion de cuentas contables</param>
        /// <returns>TRUE si logra obtener lista, FALSE si no lo logra</returns>
        [Authorize]
        public JsonResult ObtenerSubSubclasificacion(int SubClasificacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<CuentaContableModel> listaCtas = objCliente.CtaContable.ToList();
            List<SubSubClasificacionCtaContable> listaSubsub = listaCtas.Select(m => m.SubSubClasificacion).ToList();
            SubClasificacionCtaContable properClasificacion = db.DBSubClasificacion.Single(r => r.SubClasificacionCtaContableID == SubClasificacion);
            List<SubSubClasificacionCtaContable> lstSubSubClass = listaSubsub.Where(r => r.CodigoInterno.StartsWith(properClasificacion.CodigoInterno)).ToList();
            lstSubSubClass = lstSubSubClass.GroupBy(r => r.SubSubClasificacionCtaContableID).Select(r => r.First()).ToList();
            var SubSubClasificaciones = lstSubSubClass.Select(r => new { Value = r.SubSubClasificacionCtaContableID, Text = r.GetSubSubClasificacionDisplaySTR() });
            return Json(SubSubClasificaciones, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Obtiene codigo interno de cuenta contable
        /// </summary>
        /// <param name="companiaAutorizada">Variable out que entrega la coleccion de compañias que este usuario puede operar</param>
        /// <param name="objCliente">Variable con la informacion cliente</param>
        /// <param name="codultimo">variable que indica el último código interno para una subsubclasificacion</param>
        /// <returns>TRUE si logra obtener numero, FALSE si no lo logra</returns>
        [Authorize]
        public JsonResult ObtenerCodSugerido(int SubSubClasificacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            SubSubClasificacionCtaContable objSubsub = new SubSubClasificacionCtaContable();
            objSubsub = db.DBSubSubClasificacion.Single(r => r.SubSubClasificacionCtaContableID == SubSubClasificacion);
            CuentaContableModel objCta = new CuentaContableModel();
            List<CuentaContableModel> LstCtas = db.DBCuentaContable.Where(r => r.SubSubClasificacion.SubSubClasificacionCtaContableID == objSubsub.SubSubClasificacionCtaContableID).ToList();
            var cantidad = LstCtas.Count();
            var ultimo = LstCtas[cantidad - 1];
            var ultimo1 = LstCtas.Last();
            string codultimo = ultimo1.CodInterno;
            int ultimoDigitoInt;
            Int32.TryParse(codultimo, out ultimoDigitoInt);
            int sugerido = ultimoDigitoInt + 1;
            string stringSugerido = sugerido.ToString("00");
            return Json(new
            {
                a_stringSugerido = stringSugerido,
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public string ObtenerCodSugeridoSTR(int SubSubClasificacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            SubSubClasificacionCtaContable objSubsub = new SubSubClasificacionCtaContable();
            objSubsub = db.DBSubSubClasificacion.Single(r => r.SubSubClasificacionCtaContableID == SubSubClasificacion);
            CuentaContableModel objCta = new CuentaContableModel();
            List<CuentaContableModel> LstCtas = db.DBCuentaContable.Where(r => r.SubSubClasificacion.SubSubClasificacionCtaContableID == objSubsub.SubSubClasificacionCtaContableID).ToList();
            var cantidad = LstCtas.Count();
            var ultimo = LstCtas[cantidad - 1];
            var ultimo1 = LstCtas.Last();
            string codultimo = ultimo1.CodInterno;
            int ultimoDigitoInt;
            Int32.TryParse(codultimo, out ultimoDigitoInt);
            int sugerido = ultimoDigitoInt + 1;
            string stringSugerido = sugerido.ToString("00");
            return stringSugerido;
        }

        [Authorize]
        public JsonResult ObtenerCCEdit(int IDCtaContable)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objEmisor == null || objCliente == null || IDCtaContable == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            CuentaContableModel objCuentaContable = objCliente.CtaContable.SingleOrDefault(r => r.CuentaContableModelID == IDCtaContable);
            if (objCuentaContable == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = true,
                    jIDCuenta = objCuentaContable.CuentaContableModelID,
                    jCodInterno = objCuentaContable.CodInterno,
                    jNombreCuenta = objCuentaContable.nombre,
                    jClasificacion = objCuentaContable.GetClasificacionDisplaySTR(),
                    jSubClasificacion = objCuentaContable.GetSubClasificacionDisplaySTRLong(),
                    JSubSubClasificacion = objCuentaContable.GetSubSubClasificacionDisplaySTRLong(),
                    jCentroCosto = objCuentaContable.TieneCentroDeCosto,
                    jItem = objCuentaContable.ItemsModelID,
                    jAnalisis = objCuentaContable.AnalisisContablesModelID,
                    jConsiliacion = objCuentaContable.Concilaciones,
                    JAuxiliar = objCuentaContable.TieneAuxiliar,

                }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public JsonResult ObtenerCentroCostoEdit(int IDcentroCosto)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objEmisor == null || objCliente == null || IDcentroCosto == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            CentroCostoModel CentroCosto = objCliente.ListCentroDeCostos.SingleOrDefault(r => r.CentroCostoModelID == IDcentroCosto);
            if (CentroCosto == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = true,
                    IDcentroCosto = CentroCosto.CentroCostoModelID,
                    IDcienteContable = CentroCosto.ClientesContablesModelID,
                    NombreCentro = CentroCosto.Nombre,

                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ObtenerCentrosDeCostos(int IdCuentaContable = 0)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool Result = false;

            var TieneCentroDeCosto = objCliente.ListCentroDeCostos.Count();

            if (TieneCentroDeCosto <= 0)
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);


            var lstCentroDeCostos = objCliente.ListCentroDeCostos.Select(x => new { x.CentroCostoModelID, x.Nombre }).ToList();

            if (lstCentroDeCostos.Count > 0)
                Result = true;
            else
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);

            int idSeleccionado = 0;

            idSeleccionado = objCliente.CtaContable.Where(x => x.CuentaContableModelID == IdCuentaContable).Count();
            if(idSeleccionado > 0)
                idSeleccionado =  objCliente.CtaContable.SingleOrDefault(x => x.CuentaContableModelID == IdCuentaContable).CentroCostosModelID;

            StringBuilder optionSelect = new StringBuilder();
            optionSelect.Append("<option>Selecciona</option>");

            bool tieneCentroDeCosto = false;

            foreach (var centroDeCosto in lstCentroDeCostos)
            {
                if (centroDeCosto.CentroCostoModelID == idSeleccionado)
                {
                    optionSelect.Append("<option selected  value=\"" + centroDeCosto.CentroCostoModelID + "\">" + centroDeCosto.Nombre + "</option>");
                    tieneCentroDeCosto = true;
                }
                else { 
                    optionSelect.Append("<option  value=\"" + centroDeCosto.CentroCostoModelID + "\">" + centroDeCosto.Nombre + "</option>");
                }
            }

            return Json(new { ok = Result, lstCentroCostos = optionSelect.ToString(), tieneCC = tieneCentroDeCosto }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AllowAnonymous]
        public JsonResult VerificarAuxiliares(string data)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<AuxiliaresModel> lstAuxiliares = null;
            if (Session["sessionAuxiliares"] != null)
                lstAuxiliares = (List<AuxiliaresModel>)Session["sessionAuxiliares"];

            dynamic superObj = JsonConvert.DeserializeObject<dynamic>(data);
            if (superObj["ctacont"] != null && superObj["ctacont"].HasValues && superObj["ctacont"].Count > 0)
            {
                for (int i = 0; i < superObj["ctacont"].Count; i++)
                {
                    string strCtaContKey = superObj["ctacont"][i];
                    int cuentaContableKey = ParseExtensions.ParseInt(strCtaContKey);
                    if (cuentaContableKey != 0)
                    {
                        CuentaContableModel cuentaModel = db.DBCuentaContable.SingleOrDefault(r => r.CuentaContableModelID == cuentaContableKey);
                        if (cuentaModel.AnalisisContablesModelID == 1)
                        {
                            if (lstAuxiliares != null)
                            {
                                AuxiliaresModel objAuxiliar = lstAuxiliares.SingleOrDefault(r => r.LineaNumeroDetalle == (i + 1));
                                if (objAuxiliar == null)
                                {
                                    //NO TENGO AUXILIARES Y LOS REQUIERO, HELP HELP
                                    return Json(new
                                    {
                                        ok = false,
                                        reqAux = (i + 1)
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                //NO TENGO AUXILIARES Y LOS REQUIERO, HELP HELP
                                return Json(new
                                {
                                    ok = false,
                                    reqAux = (i + 1),
                                    msg = "Debe agregar auxiliares a las cuentas contables",
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        //
                        return Json(new
                        {
                            ok = false,
                            msg = "Debe al menos utilizar 2 lineas y seleccionar cuentas contables",
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new
                {
                    ok = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //NO ENCONTRO LINEAS  A LAS CUALES REVISAR LOS AUXILIARES
                return Json(new
                {
                    ok = false,
                    msg = "Debe seleccionar cuentas contables para cada linea",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult AutocompleteRutPrestadorAuxiliar(string term)
        {
            if (Session["AutoCompletePRESTADORES_RUT"] != null && ((string[])Session["AutoCompletePRESTADORES_RUT"]).Count() > 0)
            {
                string[] rutPrestadoresCACHE = (string[])Session["AutoCompletePRESTADORES_RUT"];
                var filteredItemsCACHE = rutPrestadoresCACHE.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
                return Json(filteredItemsCACHE, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string UserID = User.Identity.GetUserId();
                FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
                ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

                if (objCliente == null)
                    return null;

                var lstPrestadores = db.DBAuxiliaresPrestadores.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID && r.PrestadorRut != "55555555-5");
                string[] sessionCachedReceptoresRUT = lstPrestadores.Select(x => x.PrestadorRut).ToArray();
                Session["AutoCompletePRESTADORES_RUT"] = sessionCachedReceptoresRUT;

                string[] rutPrestadores = sessionCachedReceptoresRUT;
                var filteredItems = rutPrestadores.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
                return Json(filteredItems, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult AutocompleteRazonSocialPrestadorAuxiliar(string term)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente == null)
            {
                return null;
            }
            var lstPrestadoresRazonSocial = db.DBAuxiliaresPrestadores.Where(r => r.ClientesContablesModelID == objCliente.ClientesContablesModelID && r.PrestadorRut != "55555555-5");
            string[] sessionCachedReceptoresRazonSocial = lstPrestadoresRazonSocial.Select(x => x.PrestadorNombre).ToArray();
            Session["AutoCompletePRESTADORES_RazonSocial"] = sessionCachedReceptoresRazonSocial;

            string[] RazonSocialPrestadores = sessionCachedReceptoresRazonSocial;
            var filteredItems = RazonSocialPrestadores.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPrestadorByRUT(string RUT, string TIPO)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objCliente == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }



            //AuxiliaresPrestadoresModel QryPrestadores = db.DBAuxiliaresPrestadores.SingleOrDefault(r => r.PrestadorRut == RUT && r.ClientesContablesModelID == objCliente.ClientesContablesModelID);
            QuickReceptorModel individuo = db.Receptores.SingleOrDefault(r => r.RUT == RUT && r.tipoReceptor == TIPO);
            if (individuo == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = true,
                    RUT = individuo.RUT,
                    RazonSocial = individuo.RazonSocial
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}