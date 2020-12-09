using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;

namespace TryTestWeb.Controllers
{
    public class ContabilidadConciliacionBancariaController : Controller
    {
        // GET: ContabilidadConciliacionBancaria
        [Authorize]
        public ActionResult ConciliacionBancaria(FiltrosEstadoCtasCorrientes Filtros)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            //Consulta cuentas contables Banco
            var lstCuentasDeBanco = ConciliacionBancariaViewModel.getCtasBancarias(db, objCliente);
            ViewBag.CuentasBancarias = lstCuentasDeBanco;
            IQueryable<ConciliacionBancariaViewModel> QueryConciliacionBancaria = ConciliacionBancariaViewModel.GetQueryConciliacionBancaria(objCliente, db);

            return View();
        }

        [Authorize]
        public ActionResult ImportarCartolaBancaria(IEnumerable<HttpPostedFileBase> files)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            Session["LstCartolaBancaria"] = null;
            ComparacionConciliacionBancariaViewModel lstComparacion = new ComparacionConciliacionBancariaViewModel();

            if (files != null && files.Count() > 0)
            {

                HttpPostedFileBase file = files.ElementAt(0);
                if (file != null && file.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName);

                    if (fileExtension == ".csv" || fileExtension == ".xlsx")
                    {
                        List<string[]> csv = ParseExtensions.ReadStructurateCSV(file);
                        if (csv.Count() > 0)
                        {
                            //Analizar en que afecta la conciliación automatica de la manual y luego terminar este metodo
                            //List<CartolaBancariaModel> LstCartola = CartolaBancariaModel.getListaCartolaBancaria(csv);

                            List<string[]> MayorConsultado = Session["LibroMayorTwo"] as List<string[]>;
                            List<LibroMayorConciliacion> MayorConsultadoLista = CartolaBancariaModel.getListaLibroMayor(MayorConsultado);

                            string NombreCtaCont = MayorConsultado[0][9];
                            ViewBag.NombreCuentaContable = NombreCtaCont;

                            //CartolaBancariaModel.Conciliar(LstCartola, MayorConsultado);

                            //if (LstCartola.Count() > 0 && MayorConsultadoLista.Count() > 0)
                            //{
                            //    lstComparacion.lstCartola = LstCartola;
                            //    lstComparacion.lstLibroMayor = MayorConsultadoLista;
                            //}


                        }
                        else
                        {
                            TempData["Error"] = "No hay registros";
                            return RedirectToAction("ConciliacionBancaria", "ContabilidadConciliacionBancaria");
                        }
                    } else
                    {
                        TempData["Error"] = "La extensión debe ser .csv";
                        return RedirectToAction("ConciliacionBancaria", "ContabilidadConciliacionBancaria");
                    }
                }
            } else
            {
                TempData["Error"] = "Error al leer el archivo o no hay registros dentro";
                return RedirectToAction("ConciliacionBancaria", "ContabilidadConciliacionBancaria");
            }

            return View(lstComparacion);
        }

        [Authorize]
        public ActionResult EjecutarConciliacion(ComparacionConciliacionBancariaViewModel DatosConciliacion)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            string NombreCtaCont = DatosConciliacion.lstLibroMayor.Select(x => x.NombreCuentaContable).FirstOrDefault();
            ViewBag.NombreCuentaContable = NombreCtaCont;

            var ReturnDatosConciliacion = new ComparacionConciliacionBancariaViewModel();


            ReturnDatosConciliacion = CartolaBancariaModel.ConciliarSiSePuede(DatosConciliacion);

            List<LibroMayorConciliacion> lstNoRegistradosCartola = new List<LibroMayorConciliacion>();

            //Cuando la cartola viene sin numero de documento, eso significa que no está contabilizado en el mayor? revisar esa lógica.
            var LstNoContabilizados = ReturnDatosConciliacion.lstCartola.Where(x => x.Folio == 0).ToList();

            var LstNoRegistradosCartola = ReturnDatosConciliacion.lstLibroMayor.Where(x => !ReturnDatosConciliacion.lstCartola.Select(y => y.Folio).Contains(x.NumDocAsignado)).ToList();

            ReturnDatosConciliacion.LstNoContabilizados = LstNoContabilizados;
            ReturnDatosConciliacion.LstNoRegistradosCartola = LstNoRegistradosCartola;

            ViewBag.CantidadDeConciliados = ReturnDatosConciliacion.lstLibroMayor.Where(x => x.EstaConciliado == true).Count();

            return View(ReturnDatosConciliacion);
        }

        [Authorize]
        public ActionResult ConciliacionBAutomatica()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.CuentasBancarias = ConciliacionBancariaViewModel.getCtasBancarias(db, objCliente);

            return View();
        }

        [Authorize]
        public ActionResult ImportarExcelConciliacionBancaria(ObjCartolaMacro DataCartola)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            Session["CartolaImportada"] = null;
       
                if (DataCartola.files != null && DataCartola.files.Count() > 0)
                {
                    HttpPostedFileBase file = DataCartola.files.ElementAt(0);
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileExtension = Path.GetExtension(file.FileName);

                        if (fileExtension == ".csv" || fileExtension == ".xlsx")
                        {
                            List<string[]> csv = ParseExtensions.ReadStructurateCSV(file);
                            if (csv.Count() > 0)
                            {
                                List<string[]> MayorConsultado = Session["LibroMayorTwo"] as List<string[]>;
                                List<LibroMayorConciliacion> MayorConsultadoLista = CartolaBancariaModel.getListaLibroMayor(MayorConsultado);

                                string NombreCtaCont = MayorConsultado[0][9];
                                ViewBag.NombreCuentaContable = NombreCtaCont;

                                int CuentaConsultadaID = (int)Session["ObjetoCuentaContableConsultada"];
                                CuentaContableModel CuentaConsultada = UtilesContabilidad.CuentaContableDesdeID(CuentaConsultadaID, objCliente);

                                var ObjCartolaCompleto = CartolaBancariaMacroModel.ConvertirAObjetoCartola(csv /*NombreCtaCont*/);
                                var ResultadoInsercion = CartolaBancariaMacroModel.ConvertirAVoucher(ObjCartolaCompleto,  objCliente, db, CuentaConsultada,DataCartola.FechaCartola, DataCartola.NumeroCartola);
                                if(ResultadoInsercion == false)
                                {
                                    TempData["Error"] = "Esta cartola ya existe.";
                                    return RedirectToAction("ConciliacionBAutomatica", "ContabilidadConciliacionBancaria");
                                }
                            }
                            else
                            {
                                TempData["Error"] = "Error al leer fichero.";
                                return RedirectToAction("ConciliacionBAutomatica", "ContabilidadConciliacionBancaria");
                            }
                        }

                    }
                }
                else
                {
                    TempData["Correcto"] = "No existen registros en el fichero.";
                    return RedirectToAction("ConciliacionBAutomatica", "ContabilidadConciliacionBancaria");
                }

            TempData["Correcto"] = "Cartola y Vouchers creados con éxito.";
            return RedirectToAction("ConciliacionBAutomatica", "ContabilidadConciliacionBancaria");
        }

        [Authorize]
        public FileResult PlantillaExcel()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            var FileVirtualPath = ParseExtensions.Get_AppData_Path("PlantillaConciliacionAutomatica.csv");
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));
        }
    }
 }