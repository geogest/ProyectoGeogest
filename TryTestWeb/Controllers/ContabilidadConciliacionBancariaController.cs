using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Text;

namespace TryTestWeb.Controllers
{
    [Authorize]
    public class ContabilidadConciliacionBancariaController : Controller
    {
        // GET: ContabilidadConciliacionBancaria
        public ActionResult ConciliacionBancaria(FiltrosEstadoCtasCorrientes Filtros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            //Consulta cuentas contables Banco
            var lstCuentasDeBanco = ConciliacionBancariaViewModel.getCtasBancarias(db, objCliente);
            ViewBag.CuentasBancarias = lstCuentasDeBanco;

            return View();
        }
        public ActionResult ImportarCartolaBancariaManual(ObjCartolaMacro DataCartola)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            CartolaBancariaMacroModel CartolaMacro = new CartolaBancariaMacroModel();
            List<CartolaBancariaModel> LstDetalleCartola = new List<CartolaBancariaModel>();
            bool Result = false;

            bool SiExiste = CartolaBancariaMacroModel.ExistenRepetidos(ParseExtensions.ToDD_MM_AAAA_Multi(DataCartola.FechaCartola),  DataCartola.NumeroCartola, db, objCliente);
            if (SiExiste)
            {
                TempData["Error"] = "La cartola que se intenta importar ya existe.";
                RedirectToAction("ConciliacionBancaria", "ContabilidadConciliacionBancaria");
            }
            if (DataCartola.files != null && DataCartola.files.ContentLength > 0) {

                var CartolaPura = CartolaBancariaMacroModel.DeExcelAObjetoCartolaYVoucher(DataCartola.files);
                //var CartolaPura = CartolaBancariaModel.DeExcelACartolaBancaria(DataCartola.files, db);
                LstDetalleCartola = CartolaBancariaModel.ObtenerCartolaBancariaManual(CartolaPura, objCliente);
                Result = CartolaBancariaMacroModel.GuardarCartolaBancariaManual(LstDetalleCartola, DataCartola.FechaCartola, DataCartola.NumeroCartola, objCliente, db);

                if (Result == true)
                    TempData["Correcto"] = "Cartola Importada con éxito.";
                else
                    TempData["Error"] = "Hubo un error al importar la cartola.";
            }else
            {
                TempData["Error"] = "No hay datos para importar";
            }

            return RedirectToAction("ConciliacionBancaria", "ContabilidadConciliacionBancaria");
        }
        public ActionResult ImportarCartolaBancaria(DatosProcesoConciliacion DatosConciliacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var DatosAcomparar = new ComparacionConciliacionBancariaViewModel();
            var LibroMayor = new List<LibroMayorConciliacion>();

            var DetalleCartola = CartolaBancariaModel.ObtenerDetalleCartola(DatosConciliacion.IdCartola, db, objCliente);
            var LibroMayorConsultado = VoucherModel.GetLibroMayorTwo(1, 0, objCliente, db, "", "", DatosConciliacion.Anio, DatosConciliacion.Mes, "", "", DatosConciliacion.IdCuentaContable.ToString(), "", 0, true, 0, true);
            LibroMayor = CartolaBancariaModel.getListaLibroMayor(LibroMayorConsultado.ResultStringArray);

            if (DetalleCartola.Count() > 0 && LibroMayor.Count() > 1)
            {
                DatosAcomparar.lstCartola = DetalleCartola;
                DatosAcomparar.lstLibroMayor = LibroMayor;
                DatosAcomparar.IdCuentaContable = DatosConciliacion.IdCuentaContable;
                DatosAcomparar.IdCartola = DatosConciliacion.IdCartola;
            }
            else
            {
                TempData["Error"] = "No hay datos para iniciar el proceso.";
                return RedirectToAction("ConciliacionBancaria", "ContabilidadConciliacionBancaria");
            }

            return View(DatosAcomparar);
        }
        public ActionResult EjecutarConciliacion(ComparacionConciliacionBancariaViewModel DatosConciliacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            CuentaContableModel CuentaConsultada = ParseExtensions.ObtenerCuentaDesdeId(DatosConciliacion.IdCuentaContable, objCliente);

            var Reporte = new ReporteResultadoConciliacion();
            var DatosConciliacionActualizados = CartolaBancariaModel.ConciliarSiSePuede(DatosConciliacion, db, objCliente);
            var ActualizarTablas = CartolaBancariaMacroModel.ActualizarEstadosConciliacion(db, objCliente, DatosConciliacionActualizados.Item2, CuentaConsultada, DatosConciliacionActualizados.Item1.IdCartola);
            var ReporteConciliacion = CartolaBancariaModel.calcularReporteConciliacionManual(DatosConciliacionActualizados.Item1.lstCartola, DatosConciliacionActualizados.Item1.lstLibroMayor);

            return View(ReporteConciliacion);
        }
        public ActionResult ConciliacionBAutomatica()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.CuentasBancarias = ConciliacionBancariaViewModel.getCtasBancarias(db, objCliente);

            return View();
        }
        public ActionResult ImportarExcelConciliacionBancaria(ObjCartolaMacro DataCartola)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            Session["CartolaImportada"] = null;

            if (DataCartola.files != null && DataCartola.files.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(DataCartola.files.FileName);

                if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {
                    List<string[]> MayorConsultado = Session["LibroMayorTwo"] as List<string[]>;
                    List<LibroMayorConciliacion> MayorConsultadoLista = CartolaBancariaModel.getListaLibroMayor(MayorConsultado);

                    string NombreCtaCont = MayorConsultado[0][9];
                    ViewBag.NombreCuentaContable = NombreCtaCont;

                    int CuentaConsultadaID = (int)Session["ObjetoCuentaContableConsultada"];
                    CuentaContableModel CuentaConsultada = UtilesContabilidad.CuentaContableDesdeID(CuentaConsultadaID, objCliente);
                    //DeExcelAObjetoCartolaYVoucher
                    
                    var ObjCartolaCompleto = CartolaBancariaMacroModel.ConvertirAObjetoCartola(DataCartola.files /*NombreCtaCont*/);
                    var ResultadoInsercion = CartolaBancariaMacroModel.ConvertirAVoucher(ObjCartolaCompleto, objCliente, db, CuentaConsultada, DataCartola.FechaCartola, DataCartola.NumeroCartola);

                    //Usar para el reporte de conciliacion bancaria
                    var CartolaCompleta = ResultadoInsercion.Item2;
                    Session["ReporteConciliacionCartola"] = CartolaCompleta;
                    Session["ReporteConciliacionMayor"] = MayorConsultadoLista;


                    //Queda pendiente reporte del resultado de la conciliación
                    //Queda pendiente Corregir lo de la plantilla que se debe llenar para conciliar -> revisar que tanto va a cambiar el flujo algoritmico.
                    if (ResultadoInsercion.Item1 == false)
                    {
                        TempData["Error"] = "Esta cartola ya existe.";
                        return RedirectToAction("ConciliacionBAutomatica", "ContabilidadConciliacionBancaria");
                    }
                }
            }
            else
            {
                TempData["Correcto"] = "No existen registros en el fichero.";
                return RedirectToAction("ConciliacionBAutomatica", "ContabilidadConciliacionBancaria");
            }

            TempData["Correcto"] = "Cartola y Vouchers creados con éxito.";
            return RedirectToAction("ResultadoConciliacion", "ContabilidadConciliacionBancaria");
        }
        public FileResult PlantillaExcel()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string FileVirtualPath = ParseExtensions.Get_AppData_Path("PlantillaConciliacion.xlsx");
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));
        }
        public FileResult PlantillaExcelConciliacionManual()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string FileVirtualPath = ParseExtensions.Get_AppData_Path("PlantillaConciliacionBancaria.xlsx");
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));
        }
        public ActionResult ListaCartolaBancaria()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var ListaCartola = CartolaBancariaMacroModel.GetListaCartola(db, objCliente);
            return View(ListaCartola);
        }
        public JsonResult ObtenerListadoCartolas()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var ListaCartola = CartolaBancariaMacroModel.GetListaCartola(db, objCliente);

            StringBuilder optionSelect = new StringBuilder();

            bool Result = false;

            if(ListaCartola.Count() > 0)
            {
                optionSelect.Append("<option> Selecciona </option>");
                foreach (var itemCartola in ListaCartola)
                {
                    optionSelect.Append("<option value=\"" + itemCartola.CartolaBancariaMacroModelID + "\">" + "<b>Numero:</b> " + itemCartola.NumeroCartola + " " + "<b>Fecha:</b>  " + ParseExtensions.ToDD_MM_AAAA(itemCartola.FechaCartola) + "</option>");
                }
            }else
            {
                optionSelect.Append("<option>No existen cartolas importadas</option>");
            }


            return Json(new
            {
                ok = Result,
                selectInput = optionSelect.ToString()
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DetalleCartola(int Id)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var LstDetalle = CartolaBancariaModel.ObtenerDetalleCartola(Id, db, objCliente);

            return View(LstDetalle);
        }
        public ActionResult ResultadoConciliacion()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            var CartolaCompleta = Session["ReporteConciliacionCartola"] as List<CartolaBancariaModel>;
            var LibroMayorConsultado = Session["ReporteConciliacionMayor"] as List<LibroMayorConciliacion>;

            //Queda pendiente proveer el descargar el resultado de la conciliación en PDF
            var DatosResultadoConciliacion = CartolaBancariaModel.CalcularReporteConciliacion(CartolaCompleta, LibroMayorConsultado, db, objCliente);

            return View(DatosResultadoConciliacion);
        }
    }
}