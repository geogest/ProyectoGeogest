using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Text;
using ClosedXML.Excel;
using Elmah;
using PagedList;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TryTestWeb.Controllers
{
    public class HomeController : Controller
    {
        public const string xPathStatusTransact = "SII:RESPUESTA/SII:RESP_HDR/SII:ESTADO";
        public const string xPathStatusRecibido = "SII:RESPUESTA/SII:RESP_BODY/RECIBIDO";
        public const string xPathREAL_STATUS = "SII:RESPUESTA/SII:RESP_BODY/ESTADO";
        public const string xPathREAL_GLOSA = "SII:RESPUESTA/SII:RESP_BODY/GLOSA";


        public ActionResult Honorarios()
        {
            return View();
        }

        [Authorize]
        [ModuloHandler]
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        public ActionResult ListaHonorarios(string anno, string meses, int? page, string id_pagados, string Exportar = null, string ExportarLibro = null)
        {
            string UserID = User.Identity.GetUserId();

            int numAnno = DateTime.Now.Year;
            int numMeses = DateTime.Now.Month;

            if (page == null)
            {
                page = 1;
            }
            if (meses == null)
            {

                meses = DateTime.Now.Month + "";

            }

            if (anno == null)
            {

                anno = DateTime.Now.Year + "";

            }

            ViewBag.Meses = meses;
            ViewBag.Anno = anno;

            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            ViewBag.FormaDePago = ParseExtensions.EnumAsHTML_Input_Select<FormaPago>();
            ViewBag.TipoDePago = ParseExtensions.EnumAsHTML_Input_Select<TipoPago>();

            ViewBag.ConSinRetenciones = ParseExtensions.EnumAsHTML_Input_Select<OpcionRetencion>();

            List<BoletasHonorariosModel> lstHonorarios;
            if (objEmisor.collectionBoletasHonorarios == null)
            {
                lstHonorarios = new List<BoletasHonorariosModel>();
            }
            else
            {
                lstHonorarios = objEmisor.collectionBoletasHonorarios.OrderByDescending(x => x.Fecha).ToList();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            /*
            if (!String.IsNullOrEmpty(meses))
            {
                lstHonorarios = (List<BoletasHonorariosModel>)lstHonorarios.Where(s => s.Fecha.Month == int.Parse(meses) && s.Fecha.Year == int.Parse(anno)).ToList();
            }*/
            if (!String.IsNullOrEmpty(meses))
            {
                numAnno = ParseExtensions.ParseInt(anno);
                numMeses = ParseExtensions.ParseInt(meses);
                if (numAnno < DateTime.MinValue.Year || numAnno > DateTime.MaxValue.Year)
                {
                    numAnno = DateTime.Now.Year;
                }
                if (numMeses > 13 || numMeses < 0)
                {
                    numMeses = DateTime.Now.Month;
                }
                lstHonorarios = (List<BoletasHonorariosModel>)lstHonorarios.Where(s => s.Fecha.Year == numAnno).ToList();
                if (numMeses != 13 && numMeses != 0)
                {
                    lstHonorarios = lstHonorarios.Where(s => s.Fecha.Month == numMeses).ToList();
                }
            }

            ViewBag.id_pagados = id_pagados;
            if (string.IsNullOrWhiteSpace(id_pagados) == false)
            {
                int NumericPagosFlag = ParseExtensions.ParseInt(id_pagados);
                if (NumericPagosFlag == 1)
                {
                    lstHonorarios = lstHonorarios.Where(r => r.EstaPagada() == true).ToList();
                }
                else if (NumericPagosFlag == 2)
                {
                    lstHonorarios = lstHonorarios.Where(r => r.EstaPagada() == false).ToList();
                }
            }

            if (Exportar != null)
            {
                return ExportarHonorarios(lstHonorarios, objEmisor, numAnno, numMeses);
            }

            if (ExportarLibro != null)
            {
                return ExportarLibroHonorarios(lstHonorarios, objEmisor, numAnno, numMeses);
            }

            return View(lstHonorarios.ToPagedList(pageNumber, pageSize));
        }

        public FileContentResult ExportarClientes(List<QuickReceptorModel> lstClientes, QuickEmisorModel objEmisor)
        {
            bool BleachMembrette = false;

            string Ruta = ParseExtensions.Get_AppData_Path("clientesExport.xlsx");
            using (XLWorkbook excelFile = new XLWorkbook(Ruta))
            {
                var workSheet = excelFile.Worksheet(1);

                //SETUP MEMBRETTEEEEE
                if (BleachMembrette)
                {
                    workSheet.Cells("A1").Value = string.Empty;
                    workSheet.Cells("A2").Value = string.Empty;
                    workSheet.Cells("A3").Value = string.Empty;
                    workSheet.Cells("A4").Value = string.Empty;
                    workSheet.Cells("A5").Value = string.Empty;
                    workSheet.Cells("A6").Value = string.Empty;
                    workSheet.Cells("A7").Value = string.Empty;
                    workSheet.Cells("I2").Value = string.Empty;
                }
                else
                {
                    workSheet.Cells("A1").Value = objEmisor.RazonSocial;
                    workSheet.Cells("A2").Value = objEmisor.RUTEmpresa;
                    workSheet.Cells("A3").Value = objEmisor.Giro;
                    workSheet.Cells("A4").Value = objEmisor.Direccion;
                    workSheet.Cells("A5").Value = ParseExtensions.FirstLetterToUpper(objEmisor.Ciudad);
                    workSheet.Cells("A6").Value = objEmisor.Representante;//"RepresentanteLegal";
                    workSheet.Cells("A7").Value = objEmisor.RUTRepresentante;//"RutRepresentanteLegal";
                }

                int correlCounter = 1;
                List<string[]> ListaOnDoc = new List<string[]>();
                foreach (QuickReceptorModel Cliente in lstClientes)
                {
                    ListaOnDoc.Add(new string[] {
                        correlCounter.ToString(),
                        Cliente.RazonSocial, //RAZON SOCIAL
                        Cliente.Giro, //GIRO

                        Cliente.RUT,
                        Cliente.Contacto
                    });
                    correlCounter++;
                }

                //workSheet.Cells("D11").Value = ParseExtensions.FirstLetterToUpper(CultureInfo.CreateSpecificCulture("es").DateTimeFormat.GetMonthName(dFirstDayOfMonth.Month)) + " " + dFirstDayOfMonth.Year;

                int lastRowLocation = 0;
                if (ListaOnDoc.Count > 0)
                {
                    var rangeWithArrays = workSheet.Cell(14, 1).InsertData(ListaOnDoc);
                    rangeWithArrays.Cells().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    rangeWithArrays.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    lastRowLocation = rangeWithArrays.LastRowUsed().RowNumber() + 1;
                }
                else
                {
                    lastRowLocation = 15;
                }

                workSheet.Columns().AdjustToContents();

                using (var ms = new MemoryStream())
                {
                    excelFile.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.ms-excel", "CLIENTES_" + objEmisor.RUTEmpresa + ".xlsx");
                }
            }
        }

      

      

        public FileContentResult ExportarLibroHonorarios(List<BoletasHonorariosModel> lstHonorarios, QuickEmisorModel objEmisor, int YearToLook, int MesToLook, bool BleachMembrette = false)
        {
            string RutaExport = ParseExtensions.Get_AppData_Path("LibroHonorariosTemplate.xlsx");
            using (XLWorkbook excelFile = new XLWorkbook(RutaExport))
            {
                var workSheet = excelFile.Worksheet(1);
                #region oldRegionMembrete
                //SETUP MEMBRETTEEEEE
                /*
                if (BleachMembrette)
                {
                    workSheet.Cells("C1").Value = string.Empty;
                    workSheet.Cells("C2").Value = string.Empty;
                    workSheet.Cells("C3").Value = string.Empty;
                    workSheet.Cells("C4").Value = string.Empty;
                    workSheet.Cells("C5").Value = string.Empty;
                    workSheet.Cells("C6").Value = string.Empty;
                }
                else
                {
                    workSheet.Cells("C1").Value = objEmisor.RazonSocial;
                    workSheet.Cells("C2").Value = objEmisor.RUTEmpresa;
                    workSheet.Cells("C3").Value = objEmisor.Giro;
                    workSheet.Cells("C4").Value = objEmisor.Direccion;
                    workSheet.Cells("C5").Value = ParseExtensions.FirstLetterToUpper(objEmisor.Ciudad) +" "+ ParseExtensions.FirstLetterToUpper(objEmisor.Comuna);
                    workSheet.Cells("C6").Value = objEmisor.RUTRepresentante +" "+ objEmisor.Representante;
                }*/
                #endregion

                workSheet.Cells("C1").Value = objEmisor.RazonSocial;
                workSheet.Cells("C2").Value = objEmisor.RUTEmpresa;
                workSheet.Cells("C3").Value = objEmisor.Giro;
                workSheet.Cells("C4").Value = objEmisor.Direccion;
                workSheet.Cells("C5").Value = ParseExtensions.FirstLetterToUpper(objEmisor.Ciudad) + " " + ParseExtensions.FirstLetterToUpper(objEmisor.Comuna);
                workSheet.Cells("C6").Value = objEmisor.RUTRepresentante + " " + objEmisor.Representante;

                string NombreMonth = string.Empty;
                if (MesToLook != 13)
                    NombreMonth = ParseExtensions.FirstLetterToUpper(CultureInfo.CreateSpecificCulture("es").DateTimeFormat.GetMonthName(MesToLook)) + " de";
                workSheet.Cells("A8").Value = "LIBRO DE RETENCIONES DE HONORARIOS  " + NombreMonth + " " + YearToLook;

                int correlCounter = 1;
                List<string[]> ListaOnDoc = new List<string[]>();
                foreach (BoletasHonorariosModel Honorario in lstHonorarios)
                {
                    ListaOnDoc.Add(new string[] {
                        correlCounter.ToString(),
                        ParseExtensions.ToDD_MM_AAAA(Honorario.Fecha),
                        "B/H-EL",
                        Honorario.NumeroBoleta.ToString(),
                        Honorario.RUT_txt,
                        Honorario.RazonSocial,
                        "NO",
                        Honorario.GlosaDescripcion,//Honorario.Descripcion,
                        ParseExtensions.NumeroConPuntosDeMiles(Honorario.Brutos),
                        ParseExtensions.NumeroConPuntosDeMiles(Honorario.Retenido),
                        ParseExtensions.NumeroConPuntosDeMiles(Honorario.Liquido)
                    });
                    correlCounter++;
                }

                int lastRowLocation = 0;
                if (ListaOnDoc.Count > 0)
                {
                    var rangeWithArrays = workSheet.Cell(12, 1).InsertData(ListaOnDoc);
                    rangeWithArrays.Cells().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    rangeWithArrays.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    lastRowLocation = rangeWithArrays.LastRowUsed().RowNumber() + 1;
                }
                else
                {
                    lastRowLocation = 12;
                }

                //TOTALES
                workSheet.Cell(lastRowLocation, 1).Value = "TOTALES";
                var rangeTotals = workSheet.Range(lastRowLocation, 1, lastRowLocation, 8).Merge();
                rangeTotals.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                decimal sumaBrutos = lstHonorarios.Sum(r => r.Brutos);
                decimal sumaRetenido = lstHonorarios.Sum(r => r.Retenido);
                decimal sumaLiquido = lstHonorarios.Sum(r => r.Liquido);

                workSheet.Cell(lastRowLocation, 9).SetValue(ParseExtensions.NumeroConPuntosDeMiles(sumaBrutos)).SetDataType(XLDataType.Text); 
                workSheet.Cell(lastRowLocation, 10).SetValue(ParseExtensions.NumeroConPuntosDeMiles(sumaRetenido)).SetDataType(XLDataType.Text);
                workSheet.Cell(lastRowLocation, 11).SetValue(ParseExtensions.NumeroConPuntosDeMiles(sumaLiquido)).SetDataType(XLDataType.Text);

                var RangeTotalesFull = workSheet.Range(lastRowLocation, 1, lastRowLocation, 11);
                RangeTotalesFull.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                lastRowLocation = rangeTotals.LastRowUsed().RowNumber() + 3;

                //AGREGAR RESUMEN AQUI
                workSheet.Cell(lastRowLocation, 7).Value = "RESUMEN";
                var rangeResumen = workSheet.Range(lastRowLocation, 7, lastRowLocation, 11).Merge();
                rangeResumen.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeResumen.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                lastRowLocation = lastRowLocation + 1;

                List<string[]> ListaResumen = new List<string[]>();
                ListaResumen.Add(new string[] {
                            lstHonorarios.Count.ToString(),
                            "BOLETAS DE HONORARIOS ELECT. (B/H-EL)",
                            ParseExtensions.NumeroConPuntosDeMiles(sumaBrutos),
                            ParseExtensions.NumeroConPuntosDeMiles(sumaRetenido),
                            ParseExtensions.NumeroConPuntosDeMiles(sumaLiquido)
                        });

                var neoRangeResumen = workSheet.Cell(lastRowLocation, 7).InsertData(ListaResumen);
                neoRangeResumen.Cells().Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                lastRowLocation = neoRangeResumen.LastRowUsed().RowNumber() + 1;

                //FILE OUTPUT
                workSheet.Columns().AdjustToContents();

                using (var ms = new MemoryStream())
                {
                    excelFile.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.ms-excel", "LIBRO_HONORARIOS_" + objEmisor.RUTEmpresa + "_" + NombreMonth + " " + YearToLook + ".xlsx");
                }

            }
        }

        public FileContentResult ExportarHonorarios(List<BoletasHonorariosModel> lstHonorarios, QuickEmisorModel objEmisor, int YearToLook, int MesToLook)
        {
            bool BleachMembrette = false;

            string RutaExport = ParseExtensions.Get_AppData_Path("honorariosExport.xlsx");
            using (XLWorkbook excelFile = new XLWorkbook(RutaExport))
            {
                var workSheet = excelFile.Worksheet(1);
                /*
                //SETUP MEMBRETTEEEEE
                if (BleachMembrette)
                {
                    workSheet.Cells("A1").Value = string.Empty;
                    workSheet.Cells("A2").Value = string.Empty;
                    workSheet.Cells("A3").Value = string.Empty;
                    workSheet.Cells("A4").Value = string.Empty;
                    workSheet.Cells("A5").Value = string.Empty;
                    workSheet.Cells("A6").Value = string.Empty;
                    workSheet.Cells("A7").Value = string.Empty;
                    workSheet.Cells("I2").Value = string.Empty;
                }
                else
                {
                    workSheet.Cells("A1").Value = objEmisor.RazonSocial;
                    workSheet.Cells("A2").Value = objEmisor.RUTEmpresa;
                    workSheet.Cells("A3").Value = objEmisor.Giro;
                    workSheet.Cells("A4").Value = objEmisor.Direccion;
                    workSheet.Cells("A5").Value = ParseExtensions.FirstLetterToUpper(objEmisor.Ciudad);
                    workSheet.Cells("A6").Value = objEmisor.Representante;//"RepresentanteLegal";
                    workSheet.Cells("A7").Value = objEmisor.RUTRepresentante;//"RutRepresentanteLegal";
                }*/

                int correlCounter = 1;
                List<string[]> ListaOnDoc = new List<string[]>();
                foreach (BoletasHonorariosModel Honorario in lstHonorarios)
                {
                    ListaOnDoc.Add(new string[] {
                        correlCounter.ToString(),
                        Honorario.NumeroBoleta.ToString(),
                        Honorario.RazonSocial,
                        ParseExtensions.ToDD_MM_AAAA(Honorario.Fecha),
                        ParseExtensions.NumeroConPuntosDeMiles(Honorario.Brutos),
                        ParseExtensions.NumeroConPuntosDeMiles(Honorario.Retenido),
                        ParseExtensions.NumeroConPuntosDeMiles(Honorario.Liquido),
                        Honorario.EstaPagada() ? "Pagado" : "Impago"
                    });
                    correlCounter++;
                }

                string NombreMonth = string.Empty;
                if (MesToLook != 13)
                    NombreMonth = ParseExtensions.FirstLetterToUpper(CultureInfo.CreateSpecificCulture("es").DateTimeFormat.GetMonthName(MesToLook));
                workSheet.Cells("D2").Value = NombreMonth + " " + YearToLook;

                int lastRowLocation = 0;
                if (ListaOnDoc.Count > 0)
                {
                    var rangeWithArrays = workSheet.Cell(5, 1).InsertData(ListaOnDoc);
                    rangeWithArrays.Cells().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    rangeWithArrays.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    lastRowLocation = rangeWithArrays.LastRowUsed().RowNumber() + 1;
                }
                else
                {
                    lastRowLocation = 6;
                }

                workSheet.Columns().AdjustToContents();

                using (var ms = new MemoryStream())
                {
                    excelFile.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.ms-excel", "HONORARIOS_" + objEmisor.RUTEmpresa + "_" + NombreMonth + " " + YearToLook + ".xlsx");
                }
            }
        }

      

      
      

        [Authorize]
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        public ActionResult FlipEstadoBoletaHonorarios(int IDBoleta)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            BoletasHonorariosModel boletaToFlip = db.DBBoletasHonorarios.SingleOrDefault(r => r.BoletasHonorariosModelID == IDBoleta && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            if (boletaToFlip != null)
            {
                if (boletaToFlip.Estado == EstadoHonorarios.Vigente)
                    boletaToFlip.Estado = EstadoHonorarios.Anulado;
                else
                    boletaToFlip.Estado = EstadoHonorarios.Vigente;
                TryUpdateModel(boletaToFlip);
                db.SaveChanges();
            }

            return RedirectToAction("ListaHonorarios", "Home");
        }

    
        public JsonResult PrepararEditHonorarios(int IDEgresoFijo)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            BoletasHonorariosModel honorarioToEdit = db.DBBoletasHonorarios.SingleOrDefault(r => r.BoletasHonorariosModelID == IDEgresoFijo && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);

            bool allowEditPagos = true;
            if (honorarioToEdit != null && honorarioToEdit.HistorialPagos != null)
            {
                int countPagos = honorarioToEdit.HistorialPagos.Count;
                if (countPagos > 0)
                    allowEditPagos = false;
            }

            if (honorarioToEdit != null)
            {
                return Json(new
                {
                    ok = true,
                    IDEgresoFijo = honorarioToEdit.BoletasHonorariosModelID,
                    tipoRetencion = honorarioToEdit.ConSinRetencion,
                    NumeroBoleta = honorarioToEdit.NumeroBoleta,
                    fechaGasto = ParseExtensions.ToDD_MM_AAAA(honorarioToEdit.Fecha),
                    montoTotal = honorarioToEdit.Brutos,
                    rut = honorarioToEdit.RUT_txt,
                    RazonSocial = honorarioToEdit.RazonSocial,
                    allowEditPagos = allowEditPagos,
                    displayFile = honorarioToEdit.GetAttachmentData
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
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Administrador)]
        public ActionResult NuevaInvitacion()
        {
            //TODO: Hacer este mas estricto solo habilitando usuarios que sean hijos de este usuario
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            string UsuarioId = Request.Form.GetValues("usuarios")[0];
            int IdUsuario = Int32.Parse(UsuarioId);
            UsuarioModel objUsuario = db.DBUsuarios.SingleOrDefault(r => r.UsuarioModelID == IdUsuario);
            string Priv = Request.Form.GetValues("priv")[0];
            int Privi = Int32.Parse(Priv);


            EmisoresHabilitados objEmisorHabilitado = new EmisoresHabilitados();
            objEmisorHabilitado.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
            objEmisorHabilitado.UsuarioModelID = objUsuario.UsuarioModelID;
            objEmisorHabilitado.privilegiosAcceso = (Privilegios)Privi;

            db.DBEmisoresHabilitados.AddOrUpdate(p => new { p.UsuarioModelID, p.QuickEmisorModelID }, objEmisorHabilitado);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        //PENDING SPECIAL PRIVILEGE FOR THIS LITTLE GUY
        [Authorize]
        public ActionResult AsignarModulo()
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            int UsuarioID = ParseExtensions.ParseInt(Request.Form.GetValues("usuarios")[0]);
            int FuncionID = ParseExtensions.ParseInt(Request.Form.GetValues("funcion")[0]);
            int PrivilegioID = ParseExtensions.ParseInt(Request.Form.GetValues("priv")[0]);

            if (UsuarioID == 0 || FuncionID == 0)
            {
                CustomErrorModel CError = new CustomErrorModel("Usuario o funcion no autorizados");
                return View("../Shared/CError", CError);
            }

            UsuarioModel objUsuarioCurrent = db.DBUsuarios.Single(r => r.IdentityID == UserID);
            PosesionUsuarios objUsuarioPoseido = objUsuarioCurrent.lstUsuariosPoseidos.SingleOrDefault(r => r.UsuarioPoseidoID == UsuarioID);
            if (objUsuarioPoseido == null)
            {
                CustomErrorModel CError = new CustomErrorModel("Usuario no autorizado");
                return View("../Shared/CError", CError);
            }
            FuncionesModel objFuncion = db.DBFunciones.SingleOrDefault(r => r.FuncionesModelID == FuncionID);
            if (objFuncion == null)
            {
                CustomErrorModel CError = new CustomErrorModel("No se pudo rescatar la funcion");
                return View("../Shared/CError", CError);
            }
            UsuarioModel objUsuarioFuncionNueva = db.DBUsuarios.SingleOrDefault(r => r.UsuarioModelID == objUsuarioPoseido.UsuarioPoseidoID);

            if (db.DBModulosHabilitados.Where(r => r.privilegiosAcceso == (Privilegios)PrivilegioID && r.UsuarioModelID == UsuarioID && r.Funcion.FuncionesModelID == FuncionID && r.QuickEmisorModelID == objEmisor.QuickEmisorModelID).Count() != 0)
            {
                return RedirectToAction("Index", "Home");
                //return RedirectToAction("MantenedorUsuarioModulo", "Home");a
            }
            ModulosHabilitados objModulo = new ModulosHabilitados();
            objModulo.UsuarioModelID = objUsuarioFuncionNueva.UsuarioModelID;
            objModulo.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
            objModulo.privilegiosAcceso = (Privilegios)PrivilegioID;
            objModulo.Funcion = objFuncion;
            db.DBModulosHabilitados.Add(objModulo);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
            //return RedirectToAction("MantenedorUsuarioModulo", "Home");
        }

        [Authorize]
        [ModuloHandler]
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Administrador)]
        public ActionResult InvitarUsuario()
        {
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, User.Identity.GetUserId());
            if (objEmisor == null)
            {
                throw new NotImplementedException("No obtuvo ninguna empresa de este usuario");
                return View(new List<SelectListItem>());
            }
            else
            {
                List<SelectListItem> ListaRetorno = PerfilamientoModule.ObtenerUsuariosPoseidos(User.Identity.GetUserId());
                return View(ListaRetorno);
            }
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult MantenedorUsuarioModulo()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID, db);
            if (objEmisor == null)
            {
                return RedirectToAction("SeleccionarEmisorDesdeModulos", "Home");
            }
            else
            {
                ViewBag.ListaFunciones = db.DBFunciones.ToList();
                List<SelectListItem> ListaRetorno = PerfilamientoModule.ObtenerUsuariosPoseidos(User.Identity.GetUserId());
                return View(ListaRetorno);
            }
        }

        /*
        [Authorize]
        public ActionResult SeleccionarEmisor()
        {
            string UserID = User.Identity.GetUserId();
            //UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<EmisoresHabilitados> objEmisoresEnabled = new List<EmisoresHabilitados>();
            PerfilamientoModule.GetEmisoresHabilitados(UserID, out objEmisoresEnabled, db);
            List<QuickEmisorModel> lstRealReturn = objEmisoresEnabled.TranslateToFull(db);
            return View(lstRealReturn); 
        }*/

        [Authorize]
        public ActionResult SeleccionarEmisorDesdeModulos()
        {
            ModuloHelper.PurgeSession(Session);
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<QuickEmisorModel> objEmisoresEnabled = new List<QuickEmisorModel>();
            ModuloHelper.GetEmisoresHabilitados(UserID, out objEmisoresEnabled, db);
            return View(objEmisoresEnabled);
        }

        [Authorize]
        public ActionResult DoSeleccionarEmisorModulos()
        {
            string UserID = User.Identity.GetUserId();
            bool Esta_En_Certificacion = ParseExtensions.ItsUserOnCertificationEnvironment(UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);

            int outID = -1;
            QuickEmisorModel objEmisor = null;
            if (Int32.TryParse(Request.Params["EmisorID"], out outID))
            {
                bool successGettingEmisor = ModuloHelper.GetEmisorHabilitado(UserID, outID, out objEmisor, db);//PerfilamientoModule.GetEmisor(UserID, outID, out objEmisor);
                if (successGettingEmisor)
                {
                    if (Esta_En_Certificacion)
                    {
                        Session["EmisorSeleccionado_CERT"] = objEmisor.QuickEmisorModelID;
                        Session["EmisorSeleccionadoNombre_CERT"] = objEmisor.RazonSocial;

                        //EmisoresHabilitados objEmisorHabilitado = db.DBEmisoresHabilitados.SingleOrDefault(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.UsuarioModelID == ObjUsuario.UsuarioModelID);
                        //Privilegios PrivilegioDelUsuario = objEmisorHabilitado.privilegiosAcceso;
                        //Session["PrivCert"] = PrivilegioDelUsuario;
                    }
                    else
                    {
                        Session["EmisorSeleccionado"] = objEmisor.QuickEmisorModelID;
                        Session["EmisorSeleccionadoNombre"] = objEmisor.RazonSocial;

                        //EmisoresHabilitados objEmisorHabilitado = db.DBEmisoresHabilitados.SingleOrDefault(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.UsuarioModelID == ObjUsuario.UsuarioModelID);
                        //Privilegios PrivilegioDelUsuario = objEmisorHabilitado.privilegiosAcceso;
                        //Session["PrivProd"] = PrivilegioDelUsuario;
                    }

                    int vista1 = db.DBFunciones.Where(ar => ar.NombreFuncion == "Index").FirstOrDefault().FuncionesModelID;
                    int vista2 = db.DBFunciones.Where(ar => ar.NombreFuncion == "PanelClienteContable").FirstOrDefault().FuncionesModelID;

                    //  QuickEmisorModel objEmisor2 = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
                    int listaVista1 = db.DBModulosHabilitados.Where(ar => ar.QuickEmisorModelID == objEmisor.QuickEmisorModelID && ar.Funcion.FuncionesModelID == vista1 && ObjUsuario.UsuarioModelID == ar.UsuarioModelID).Count();
                    int listaVista2 = db.DBModulosHabilitados.Where(ar => ar.QuickEmisorModelID == objEmisor.QuickEmisorModelID && ar.Funcion.FuncionesModelID == vista2 && ObjUsuario.UsuarioModelID == ar.UsuarioModelID).Count();

                    if (listaVista1 > 0)
                    {
                         return Json(new { success = true, inicio = "Home/Index" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (listaVista2 > 0)
                        {
                            return Json(new { success = true, inicio = "Contabilidad/SeleccionarClienteContable" }, JsonRequestBehavior.AllowGet);
                        }
                    }


                    ///return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult DoSeleccionarEmisor()
        {
            //SPLIT IT ACCORDING AMBIENTE UTILIZADO
            string UserID = User.Identity.GetUserId();
            bool Esta_En_Certificacion = ParseExtensions.ItsUserOnCertificationEnvironment(UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);

            int outID = -1;
            QuickEmisorModel objEmisor = null;
            if (Int32.TryParse(Request.Params["EmisorID"], out outID))
            {
                bool successGettingEmisor = PerfilamientoModule.GetEmisor(UserID, outID, out objEmisor);
                if (successGettingEmisor)
                {
                    if (Esta_En_Certificacion)
                    {
                        Session["EmisorSeleccionado_CERT"] = objEmisor.QuickEmisorModelID;
                        Session["EmisorSeleccionadoNombre_CERT"] = objEmisor.RazonSocial;

                        EmisoresHabilitados objEmisorHabilitado = db.DBEmisoresHabilitados.SingleOrDefault(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.UsuarioModelID == ObjUsuario.UsuarioModelID);
                        Privilegios PrivilegioDelUsuario = objEmisorHabilitado.privilegiosAcceso;
                        Session["PrivCert"] = PrivilegioDelUsuario;
                    }
                    else
                    {
                        Session["EmisorSeleccionado"] = objEmisor.QuickEmisorModelID;
                        Session["EmisorSeleccionadoNombre"] = objEmisor.RazonSocial;

                        EmisoresHabilitados objEmisorHabilitado = db.DBEmisoresHabilitados.SingleOrDefault(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.UsuarioModelID == ObjUsuario.UsuarioModelID);
                        Privilegios PrivilegioDelUsuario = objEmisorHabilitado.privilegiosAcceso;
                        Session["PrivProd"] = PrivilegioDelUsuario;


                    }

                    int vista1 = db.DBFunciones.Where(ar => ar.NombreModulo  == "Index").FirstOrDefault().FuncionesModelID;
                    int vista2 = db.DBFunciones.Where(ar => ar.NombreModulo == "PanelClienteContable").FirstOrDefault().FuncionesModelID;
                    
                    //  QuickEmisorModel objEmisor2 = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
                    int listaVista1 = db.DBModulosHabilitados.Where(ar => ar.QuickEmisorModelID == objEmisor.QuickEmisorModelID && ar.Funcion.FuncionesModelID == vista1).Count();
                    int listaVista2 = db.DBModulosHabilitados.Where(ar => ar.QuickEmisorModelID == objEmisor.QuickEmisorModelID && ar.Funcion.FuncionesModelID == vista2).Count();

                    if (listaVista1 > 0) {
                        return Json(new { success = true, inicio = "Home/Index" }, JsonRequestBehavior.AllowGet);
                    }
                    else{
                        if (listaVista2 > 0)
                        {
                            return Json(new { success = true, inicio = "Contabilidad/PanelClienteContable" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    
                    
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult ItemContableDO()
        {
            /*
			string UserID = User.Identity.GetUserId();
			FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
			QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);

			

			string valorSelect = Request.Form.GetValues("CuentaContable")[0];
			string Glosa = Request.Form.GetValues("Glosa")[0];

			DateTime FechaDoc = DateTime.ParseExact(Request.Form.GetValues("FechaDoc")[0], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

			//DateTime FechaDoc = ParseExtensions.ToMM_DD_AAAA(Request.Form.GetValues("FechaDoc")[0]);

			decimal ValorMonto = 0;
			try
			{
				string StrMonto = Request.Form.GetValues("Monto")[0];
				decimal.TryParse(StrMonto, out ValorMonto);
			}
			catch { }
			decimal Monto = ValorMonto;
			DateTime FechaIngreso = DateTime.Now;

			int IDCuentaContable = ClasificacionCuentaContableModel.MakeFromSelect(valorSelect, UserID);
			ClasificacionCuentaContableModel objCtaContable = db.CuentaContable.Single(r => r.ClasificacionCuentaContableModelID == IDCuentaContable);

			ItemContableModel newItemContable = new ItemContableModel();
			newItemContable.Glosa = Glosa;
			newItemContable.FechaDocumento = FechaDoc;
			newItemContable.FechaIngreso = FechaIngreso;
			newItemContable.Monto = ValorMonto;
			newItemContable.CtaContable = objCtaContable;
			newItemContable.CuentaContableModelID = IDCuentaContable;
			newItemContable.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
			db.DocumentoContable.Add(newItemContable);
			db.SaveChanges();
			*/
            return RedirectToAction("CuentaContable", "Home");

        }



        [Authorize]
        public ActionResult CuentaContable()
        {
            /*
			string UserID = User.Identity.GetUserId();
			FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
			QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);
			
			IQueryable<ItemContableModel> QueryItemContable = db.DocumentoContable.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
		   
			List<ItemContableModel> lstItemContable = QueryItemContable.ToList();
			*/

            //string UserID = User.Identity.GetUserId();

            //FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            //QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);
            //IQueryable<FacturaQuickModel> QueryFacturas = db.Facturas.Include("ObjTotals").Include("Detalle.ImpuestoAdicionalProducto").Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            //List<FacturaQuickModel> lstFacturas = QueryFacturas.ToList();

            //QueryEstUpService objSIITrackCheck = new QueryEstUpService();

            //CertificadosModels objCertificate = db.Certificados.Find(objEmisor.QuickEmisorModelID);
            //if (objCertificate == null)
            //{
            //    CustomErrorModel CError = new CustomErrorModel("No ha cargado certificado");
            //    return View("../Shared/CError", CError);
            //}
            //return View(lstFacturas);
            //return View(lstItemContable);
            return View();
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
       
     

        [Authorize]
        [OnlyCertification]
        [ModuloHandler]
        public ActionResult Certificacion()
        {
            return View();
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        public ActionResult EstadoResultadoPartial(string Periodo, string Anio)
        {
            string UserID = User.Identity.GetUserId();
            EstadoResultadoModel objEstadoResultado = null;
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            //QuickEmisorModel objEmisor = db.Emisores.Include("Certificados").SingleOrDefault(r => r.IdentityID == UserID);

            //Adaptar comportamiento del otro elemento? VistaFlujoCaja THE NON PARTIAL
            DateTime periodoFlujo;
            if (Request.Form == null || string.IsNullOrWhiteSpace(Periodo) || string.IsNullOrWhiteSpace(Anio))
            {
                DateTime FechaHoy = DateTime.Now;
                periodoFlujo = new DateTime(FechaHoy.Year, FechaHoy.Month, 1);
            }
            else if (Periodo == "13")
            {
                int CURRENT_YEAR = DateTime.Now.Year;
                var dFirstDayOfYear = new DateTime(CURRENT_YEAR, 1, 1);
                var dLastDayOfYear = dFirstDayOfYear.AddYears(1).AddDays(-1);
                List<EstadoResultadoModel> lstFlujoAnual = db.EstadoResultado.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID && i.Periodo >= dFirstDayOfYear && i.Periodo <= dLastDayOfYear).ToList();
                objEstadoResultado = EstadoResultadoModel.GetFlujoCumulativoAnio(lstFlujoAnual, objEmisor.QuickEmisorModelID);
                return PartialView("EstadoResultadoPartial", objEstadoResultado);
            }
            else
            {
                periodoFlujo = new DateTime(ParseExtensions.ParseInt(Anio), ParseExtensions.ParseInt(Periodo), 1);
            }

            IQueryable<EstadoResultadoModel> LstModel = db.EstadoResultado.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID && periodoFlujo == i.Periodo);
            List<EstadoResultadoModel> ListaDatos = LstModel.ToList();
            if (ListaDatos.Count > 0)
                objEstadoResultado = ListaDatos[0];
            else
                objEstadoResultado = new EstadoResultadoModel();
            objEstadoResultado.Periodo = periodoFlujo;

            return PartialView("EstadoResultadoPartial", objEstadoResultado);
            //turn View(objFlujo);
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
       

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
       
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        public ActionResult EstadoResultado(string Periodo, string Anio)
        {
            string UserID = User.Identity.GetUserId();
            EstadoResultadoModel objEstadoResultado = null;
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            //QuickEmisorModel objEmisor = db.Emisores.Include("Certificados").SingleOrDefault(r => r.IdentityID == UserID);

            DateTime periodoFlujo;

            if (string.IsNullOrWhiteSpace(Periodo) || string.IsNullOrWhiteSpace(Anio))
            {
                int CURRENT_YEAR = DateTime.Now.Year;
                var dFirstDayOfYear = new DateTime(CURRENT_YEAR, 1, 1);
                var dLastDayOfYear = dFirstDayOfYear.AddYears(1).AddDays(-1);
                List<EstadoResultadoModel> lstEstadoResultadoAnual = db.EstadoResultado.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID && i.Periodo >= dFirstDayOfYear && i.Periodo <= dLastDayOfYear).ToList();
                objEstadoResultado = EstadoResultadoModel.GetFlujoCumulativoAnio(lstEstadoResultadoAnual, objEmisor.QuickEmisorModelID);
                return View(objEstadoResultado);
            }
            else
            {
                periodoFlujo = new DateTime(ParseExtensions.ParseInt(Anio), ParseExtensions.ParseInt(Periodo), 1);
                IQueryable<EstadoResultadoModel> LstModel = db.EstadoResultado.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID && periodoFlujo == i.Periodo);
                List<EstadoResultadoModel> ListaDatos = LstModel.ToList();
                if (ListaDatos.Count > 0)
                    objEstadoResultado = ListaDatos[0];
                else
                    objEstadoResultado = new EstadoResultadoModel();
                objEstadoResultado.Periodo = periodoFlujo;
                return View(objEstadoResultado);
            }
        }

       

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        [ModuloHandler]
        public ActionResult InformeIngresos()
        {
            return View();
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        [ModuloHandler]
        public ActionResult InformeResultados()
        {
            return View();
        }

      
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
      
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
      

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
       

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
      

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        public ActionResult AutocompleteRutReceptor(string term)
        {
            if (Session["AutoCompleteReceptoresRUT"] == null)
            {
                string UserID = User.Identity.GetUserId();
                FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

                QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
                //QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);

                string[] sessionCachedReceptoresRUT = db.Receptores.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID && i.RUT != "55555555-5").Select(x => x.RUT).ToArray();
                Session["AutoCompleteReceptoresRUT"] = sessionCachedReceptoresRUT;
            }
            string[] rutReceptores = (string[])Session["AutoCompleteReceptoresRUT"];
            var filteredItems = rutReceptores.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AutoCompleteRazonSocialReceptor(string term)
        {
            if (Session["AutoCompleteReceptoresRznSoc"] == null)
            {
                string UserID = User.Identity.GetUserId();
                FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

                QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
                //QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityID == UserID);

                string[] sessionCachedReceptoresRznSoc = db.Receptores.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID).Select(x => x.RUT).ToArray();
                Session["AutoCompleteReceptoresRznSoc"] = sessionCachedReceptoresRznSoc;
            }
            string[] rznSocReceptores = (string[])Session["AutoCompleteReceptoresRznSoc"];
            var filteredItems = rznSocReceptores.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

      

      

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        public ActionResult IngFacturaCompra()
        {
            return View();
        }

      
        [Authorize]
        [ModuloHandler]
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        public ActionResult Index()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);


            //QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            QuickEmisorModel objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID, db);

         
            return View();
        }
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Homey()
        {
            return RedirectToAction("Index", "Home");
           //return View();
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
       
        [Authorize]
        public ActionResult FormFactura()
        {
            ViewBag.Message = "Hacer factura";
            return View();
            //var user = User.Identity.GetUserId();
        }

        [Authorize]
        public ActionResult MakeFacturaNeo(int? facturaID)
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = null;

            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            if (UserID == null || UserID == string.Empty)
            {
                UserID = "AAA";
            }

            objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            /*
            IQueryable<QuickEmisorModel> lstModel = db.Emisores.Where(i => i.IdentityID == UserID);
            List<QuickEmisorModel> ListaEmisores = lstModel.ToList();
            if (ListaEmisores != null && ListaEmisores.Count > 0)
                objEmisor = ListaEmisores[0];
                */

            return View(objEmisor);
        }

      

        [Authorize]
        public JsonResult UpdateReceptorSelect(int? selectedVal)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = null;
            objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID, db);
            List<QuickReceptorModel> lstReceptores = objEmisor.collectionReceptores.ToList();

            StringBuilder sb = new StringBuilder();

            foreach (QuickReceptorModel receptor in lstReceptores)
            {
                if (selectedVal.HasValue)
                {
                    if (selectedVal.Value == (int)receptor.QuickReceptorModelID)
                        sb.AppendLine("<option value=\"" + (int)receptor.QuickReceptorModelID + "\" selected=\"selected\">" + receptor.RazonSocial + " - " + receptor.RUT + "</option>");
                    else
                        sb.AppendLine("<option value=\"" + (int)receptor.QuickReceptorModelID + "\">" + receptor.RazonSocial + " - " + receptor.RUT + "</option>");
                }
                else
                {
                    sb.AppendLine("<option value=\"" + (int)receptor.QuickReceptorModelID + "\">" + receptor.RazonSocial + " - " + receptor.RUT + "</option>");
                }
            }
            return Json(new
            {
                ok = true,
                selectInput = sb.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult NeoFacturaGLineaDetalle(int lineaDetalle)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div id='Detalle" + lineaDetalle + "' class='row m-t-30'> ");
            sb.AppendLine("    <div class='col-md-3'> ");

            if(lineaDetalle == 1)
                sb.AppendLine("         <label class='control-label text-center'>Descripción</label>  ");

            sb.AppendLine("         <input class='form-control' placeholder='Nombre del producto' name='EFXP_NMB_" + lineaDetalle + "' id='EFXP_NMB_" + lineaDetalle + "' type='text' maxlength='75'>  ");
            sb.AppendLine("    </div> ");
            sb.AppendLine("    <div class='col-md-1'> ");

            if(lineaDetalle == 1)
                sb.AppendLine("        <label class='control-label text-center'>Cantidad</label>  ");

            sb.AppendLine("        <input class='form-control' placeholder='Cantidad' name='EFXP_QTY_" + lineaDetalle + "' id='EFXP_QTY_" + lineaDetalle + "' type='number' lang='en-150'> ");
            sb.AppendLine("     </div> ");
            sb.AppendLine("    <div class='col-md-1'> ");

            if(lineaDetalle == 1)
                sb.AppendLine("         <label class='control-label text-center'>Medida</label> ");

            sb.AppendLine("         <input class='form-control' placeholder='Unidad de Medida' name='UnmdItem_" + lineaDetalle + "' id='UnmdItem_" + lineaDetalle + "' type='text' maxlength='4'> ");
            sb.AppendLine("    </div> ");
            sb.AppendLine("    <div class='col-md-2'> ");

            if(lineaDetalle == 1)
                sb.AppendLine("         <label class='control-label text-center'>Precio</label> ");

            sb.AppendLine("         <input class='form-control' placeholder='Precio' name='EFXP_PRC_" + lineaDetalle + "' id='EFXP_PRC_" + lineaDetalle + "' type='number' min='0' step='1' lang='en-150'>  ");
            sb.AppendLine("    </div> ");

            //Seccion impuestos adicionales de preferencia deberia usar el metodo que construye los impuestos programables
            sb.AppendLine("    <div class='col-md-2'> ");

            if(lineaDetalle == 1)
                sb.AppendLine("        <label class='control-label'>Impuesto Adicional</label> ");

            sb.AppendLine("        <select name='IMPUESTO_ADICIONAL_" + lineaDetalle + "' id='IMPUESTO_ADICIONAL_" + lineaDetalle + "' class='form-control'> ");
            sb.AppendLine("            <option value='[0,0]'>Ninguno</option> ");
            sb.AppendLine("            <option value='[23,15]'>Art. de Oro, Joyas y Pieles Finas (15%)</option> ");
            sb.AppendLine("            <option value='[44,15]'>Tapices, Casas Rodantes, Caviar y Armas de Aire (15%)</option> ");
            sb.AppendLine("            <option value='[24,31.5]'>Licores, Pisco, Destilados (31.5%)</option> ");
            sb.AppendLine("            <option value='[25,20.5]'>Vinos, Chichas, Sidras (20.5%)</option> ");
            sb.AppendLine("            <option value='[26,20.5]'>Cervezas y Otras Bebidas Alcoholicas (20.5%)</option> ");
            sb.AppendLine("            <option value='[27,10]'>Aguas Minerales y Bebidas Analcoholicas (10%)</option> ");
            sb.AppendLine("            <option value='[271,18]'>Bebidas Analcoholicas con alto contenido de Azucar (18%)</option> ");
            sb.AppendLine("            <option value='[15,19]'>IVA Retenido Total</option> ");
            sb.AppendLine("        </select> ");
            sb.AppendLine("    </div> ");

            sb.AppendLine("    <div class='col-md-1'> ");

            if (lineaDetalle == 1)
                sb.AppendLine("        <label class='control-label text-center'>Exento</label> ");                                 

            sb.AppendLine("        <input class='form-control' placeholder='%' name='EFXP_EXENTO_" + lineaDetalle + "' id='EFXP_EXENTO_" + lineaDetalle + "' min='0' type='checkbox'> ");
            sb.AppendLine("    </div> ");
            sb.AppendLine("    <div class='col-md-1'> ");


            if (lineaDetalle == 1)
                sb.AppendLine("        <label class='control-label text-center'>Desc. (%)</label> ");

            sb.AppendLine("        <input class='form-control' name='Descuento_" + lineaDetalle + "' id='Descuento_" + lineaDetalle + "' min='0' type='number'> ");
            sb.AppendLine("    </div> ");
            sb.AppendLine("    <div class='col-md-2'> ");

            if(lineaDetalle == 1)
                sb.AppendLine("        <label class='control-label text-center'>Subtotal</label> ");

            sb.AppendLine("        <input class='form-control' readonly name='EFXP_SUBT_" + lineaDetalle + "' id='EFXP_SUBT_" + lineaDetalle + "' type='number'>  ");
            sb.AppendLine("    </div> ");
            sb.AppendLine("</div> ");


            return Json(new
            {
                ok = true,
                detailLine = sb.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult DescuentoRecargoTempModal(int lineaDetalle)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = null;

            objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID);
            ViewBag.lineaDetalle = lineaDetalle;
            //VE SI EXISTE UNA LISTA DE AUXILIARES
            if (Session["sessionDscRcg"] != null)
            {
                //De existir ve si existe una info de DESCUENTO/RECARGO ya para esta linea
                List<List<string>> lstInfoDescuentoRecargo = (List<List<string>>)Session["sessionDscRcg"];
                List<string> objDscRcg = new List<string>();
                int index = lstInfoDescuentoRecargo.FindIndex(item => ParseExtensions.ParseInt(item[0]) == lineaDetalle);
                if (index >= 0)
                {
                    objDscRcg = lstInfoDescuentoRecargo[index];
                    return PartialView(objDscRcg);
                }
            }
            return PartialView(null);
        }

        [Authorize]
        public ActionResult EditarReceptorModal(int IDReceptor)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = null;
            objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID);

            if (objEmisor == null)
                return null;

            QuickReceptorModel objReceptorACargar = objEmisor.collectionReceptores.SingleOrDefault(r => r.QuickReceptorModelID == IDReceptor);
            if (objReceptorACargar == null)
                return null;

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;

            return PartialView(objReceptorACargar);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AgregarDescuentoRecargoTemp()
        {
            string NroLineaDetalle = Request.Form.GetValues("LineaDetalle")[0];

            List<List<string>> lstDscRcgos = new List<List<string>>();
            List<string> currentDscPagos = null;

            int indiceTemp = -1;

            //VE SI EXISTE UNA LISTA DE AUXILIARES
            if (Session["sessionDscRcg"] == null)
                lstDscRcgos = new List<List<string>>(); //si no existe la crea
            else
            {
                //De existir ve si existe un auxiliar ya para esa linea
                lstDscRcgos = (List<List<string>>)Session["sessionDscRcg"];
                int index = lstDscRcgos.FindIndex(item => item[0] == NroLineaDetalle);
                if (index >= 0)
                {
                    currentDscPagos = lstDscRcgos[index];
                    indiceTemp = index;
                }
            }
            if (currentDscPagos == null)
            {
                currentDscPagos = new List<string>();
            }

            /*
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = null;
            objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID, db);*/

            currentDscPagos.Clear();
            currentDscPagos.Add(NroLineaDetalle);
            currentDscPagos.Add(Request.Form.GetValues("TpoMov")[0]);
            currentDscPagos.Add(Request.Form.GetValues("GlosaDR")[0]);
            currentDscPagos.Add(Request.Form.GetValues("TpoValor")[0]);
            currentDscPagos.Add(Request.Form.GetValues("ValorDR")[0]);

            if (indiceTemp != -1)
            {
                lstDscRcgos[indiceTemp] = currentDscPagos;
            }
            else
            {
                //(List<AuxiliaresModel>)Session["sessionAuxiliares"];
                lstDscRcgos.Add(currentDscPagos);
                Session["sessionDscRcg"] = lstDscRcgos;
            }

            return Json(new { ok = true });

        }

        [HttpPost]
        [Authorize]
        public JsonResult EditarReceptorAjax()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = null;
            objEmisor = ModuloHelper.GetEmisorSeleccionado(Session, UserID, db);

            if (objEmisor == null)
                return Json(new { ok = false });

            int IDReceptor = ParseExtensions.ParseInt(Request.Form.GetValues("ReceptorID")[0].ToString());

            QuickReceptorModel objReceptorACargar = objEmisor.collectionReceptores.SingleOrDefault(r => r.QuickReceptorModelID == IDReceptor);

            if (objReceptorACargar == null)
                return Json(new { ok = false });

            /*
            EFXP_RUT_RECEP
            EFXP_RZN_SOC_RECEP
            EFXP_DIR_RECEP
            EFXP_REGION_RECEP
            EFXP_CMNA_RECEP
            EFXP_GIRO_RECEP
            ReceptorID 
             */

            string RutReceptor = Request.Form.GetValues("EFXP_RUT_RECEP")[0];
            string RznSocReceptor = Request.Form.GetValues("EFXP_RZN_SOC_RECEP")[0];
            string GiroReceptor = Request.Form.GetValues("EFXP_GIRO_RECEP")[0];
            //THESE ARE NUMERIC NOW
            int IDComuna = ParseExtensions.ParseInt(Request.Form.GetValues("EFXP_CMNA_RECEP")[0]);
            int IDRegion = ParseExtensions.ParseInt(Request.Form.GetValues("EFXP_REGION_RECEP")[0]);
            string DireccionReceptor = Request.Form.GetValues("EFXP_DIR_RECEP")[0];

            objReceptorACargar.RUT = RutReceptor;
            objReceptorACargar.RazonSocial = RznSocReceptor;
            objReceptorACargar.Giro = GiroReceptor;
            objReceptorACargar.idComuna = IDComuna;
            objReceptorACargar.idRegion = IDRegion;
            objReceptorACargar.Direccion = DireccionReceptor;

            TryUpdateModel(objReceptorACargar);
            db.SaveChanges();



            return Json(new { ok = true });
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult MakeFacturaRecurrente()
        {
            Session["sessionDscRcg"] = null;

            string UserID = User.Identity.GetUserId();

            QuickEmisorModel objEmisor = null;

            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            UsuarioModel ObjUsuario = db.DBUsuarios.Single(r => r.IdentityID == UserID);

            objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            List<QuickReceptorModel> lstReceptores = objEmisor.collectionReceptores.ToList();
            ViewBag.ConjuntoReceptores = lstReceptores;

            Tuple<QuickEmisorModel, bool> rtrnVal = new Tuple<QuickEmisorModel, bool>(objEmisor, false);

            //List<SelectListItem> lstValidDocs = ParseExtensions.TipoCAFCargados_NEW(ObjUsuario, objEmisor, false);

            List<TipoDte> lstTiposARevisar = new List<TipoDte>();
            List<SelectListItem> listItems = new List<SelectListItem>();

            lstTiposARevisar.Add(TipoDte.FacturaElectronica);
            lstTiposARevisar.Add(TipoDte.FacturaElectronicaExenta);

            foreach (TipoDte TipoDocumento in lstTiposARevisar)
            {
                listItems.Add(new SelectListItem { Text = ParseExtensions.TipoDTEToFriendlyName(TipoDocumento), Value = ((int)TipoDocumento).ToString() });    
            }

            ViewBag.lstValidDocs = listItems;//lstValidDocs;
            ViewBag.vObjUsuario = ObjUsuario;

            return View(rtrnVal);
        }

     
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Facturador)]
        [Authorize]
        [ModuloHandler]
        public ActionResult MakeFacturaExp(string opt, int? facturaID)
        {
            Session["sessionDscRcg"] = null;

            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = null;

            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            bool PackAndSend = true;
            if (!string.IsNullOrEmpty(opt))
                PackAndSend = false;

            List<SelectListItem> listReceptoresSelect = new List<SelectListItem>();

            List<QuickReceptorModel> lstReceptores = objEmisor.collectionReceptores.ToList();
            ViewBag.ConjuntoReceptores = lstReceptores;

            Tuple<QuickEmisorModel, bool> rtrnVal = new Tuple<QuickEmisorModel, bool>(objEmisor, PackAndSend);
            return View(rtrnVal);

            /*
            ViewBag.AvaiableEnums = dynamicTextEnumsAvaiable.Select(x => 
                                  new SelectListItem() 
                                  {
                                      Text = x.ToString()
                                  }); 
             */

            /*
            foreach (string STR in GeneralProcessingList)
             {
                 if (string.IsNullOrWhiteSpace(STR))
                     continue;
                 XmlDocument xDoc = new XmlDocument();
                 xDoc.PreserveWhitespace = true;
                 xDoc.LoadXml(STR);

                 int TipoNumerico = ParseExtensions.ParseInt(xDoc.SelectSingleNode("//AUTORIZACION/CAF/DA/TD").InnerXml);
                 TipoDte TipoDocumento = (TipoDte)TipoNumerico;
                 listItems.Add(new SelectListItem { Text = ParseExtensions.TipoDTEToFriendlyName(TipoDocumento), Value = TipoNumerico.ToString() });
             }
             */
        }

    
      
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [ModuloHandler]
        [Authorize]
        public ActionResult DatosEmpresa()
        {
            //Tuple<QuickEmisorModel, List<string[]>> valToUse;

            string UserID = User.Identity.GetUserId();
            QuickEmisorModel obj = null;
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID) ;

            if (UserID == null || UserID == string.Empty)
            {
                UserID = "AAA";
            }

            /*
            IQueryable<QuickEmisorModel> lstModel = db.Emisores.Include("Certificados").Where(i => i.IdentityID == UserID);
            List<QuickEmisorModel> ListaEmisores = lstModel.ToList();
            if (ListaEmisores != null && ListaEmisores.Count > 0)
                obj = ListaEmisores[0];
            */

            UsuarioModel UsuarioActual = db.DBUsuarios.SingleOrDefault(x => x.IdentityID == UserID);

            PerfilUsuarioModel PerfilUsuario = db.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == UsuarioActual.PerfilUsuarioModelID);

            if(PerfilUsuario != null)
             ViewBag.TipoUsuario = PerfilUsuario.NombrePerfil;

            List<ActividadEconomicaModel> lstAllActectos = db.DBActeco.ToList();

            obj = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            List<string> lstCodActEcoSelected = obj.lstActividadEconomica.Select(r => r.CodigoInterno).ToList();

            ViewBag.HtmlStr = ParseExtensions.ListAsHTML_Input_Select<ActividadEconomicaModel>(lstAllActectos, "CodigoInterno", new List<string> { "CodigoInterno", "NombreActividad" }, lstCodActEcoSelected);

            //List<string[]> lstDatosCafCargados = ParseExtensions.CAFCargados(obj.Certificados);

            //valToUse = new Tuple<QuickEmisorModel, List<string[]>>(obj, lstDatosCafCargados);
            //return View(valToUse);
            return View(obj);
        }

      
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        [ModuloHandler]
        public ActionResult NuevoCliente()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            QuickReceptorModel receptor = new QuickReceptorModel();
            receptor.tipoReceptor = "CL";
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            return View(receptor);
        }


        [Authorize]
        [ModuloHandler]
        public ActionResult NuevoClienteC()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            QuickReceptorModel receptor = new QuickReceptorModel();
            receptor.tipoReceptor = "CL";
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            return View(receptor);
        }


        [Authorize]
        [ModuloHandler]
        public ActionResult NuevoProveedor()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            QuickReceptorModel receptor = new QuickReceptorModel();
            receptor.tipoReceptor = "PR";
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            return View(receptor);
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult NuevoPersona()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            QuickReceptorModel receptor = new QuickReceptorModel();
            receptor.tipoReceptor = "P";
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            return View(receptor);
        }



        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        [ModuloHandler]
        public ActionResult EditarCliente(int id_receptor)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            QuickReceptorModel receptor = new QuickReceptorModel();

            receptor = db.Receptores.AsQueryable().Where(s => s.QuickReceptorModelID == id_receptor).FirstOrDefault();  //db.Emisores.AsQueryable().Where(s => s.QuickEmisorModelID == 1   );

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;

            return View(receptor);
        }
        [Authorize]
        [ModuloHandler]
        public ActionResult EditarClienteC(int id_receptor)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            QuickReceptorModel receptor = new QuickReceptorModel();

            receptor = db.Receptores.AsQueryable().Where(s => s.QuickReceptorModelID == id_receptor).FirstOrDefault();  //db.Emisores.AsQueryable().Where(s => s.QuickEmisorModelID == 1   );

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;

            return View(receptor);
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult EditarProveedor(int id_receptor)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            QuickReceptorModel receptor = new QuickReceptorModel();

            receptor = db.Receptores.AsQueryable().Where(s => s.QuickReceptorModelID == id_receptor).FirstOrDefault();  //db.Emisores.AsQueryable().Where(s => s.QuickEmisorModelID == 1   );

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;

            return View(receptor);
        }
        [Authorize]
        [ModuloHandler]
        public ActionResult EditarPersona(int id_receptor)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            QuickReceptorModel receptor = new QuickReceptorModel();

            receptor = db.Receptores.AsQueryable().Where(s => s.QuickReceptorModelID == id_receptor).FirstOrDefault();  //db.Emisores.AsQueryable().Where(s => s.QuickEmisorModelID == 1   );

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;

            return View(receptor);
        }


        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [ModuloHandler]
        [Authorize]
        public ActionResult ListarCliente(string sortOrder, string currentFilter, string searchString, int? page, string Exportar = null)
        {     
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            //var lstRecepciones = from s in db.Receptores.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID) select s;
            var lstRecepciones = db.Receptores.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.tipoReceptor == "CL");
            if (!String.IsNullOrEmpty(searchString))
            {
               lstRecepciones = lstRecepciones.Where(s => s.RazonSocial.Contains(searchString));
            }

            List<QuickReceptorModel> lstRecepcione2 = lstRecepciones.OrderByDescending(x => x.QuickReceptorModelID).AsQueryable().ToList();


            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (Exportar != null)
            {
                return ExportarClientes(lstRecepcione2, objEmisor);
            }

            return View(lstRecepcione2.ToPagedList(pageNumber, pageSize)  );
            /*CertificadosModels objCertificate = db.Certificados.Find(objEmisor.QuickEmisorModelID);
            if (objCertificate == null)
            {
                CustomErrorModel CError = new CustomErrorModel("No ha cargado certificado");
                return View("../Shared/CError", CError);
            }*/
        }
        [Authorize]
        [ModuloHandler]
        public ActionResult ListarClienteC(string sortOrder, string currentFilter, string searchString, int? page, string Exportar = null)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsStringWithSelectedCodInterno(objCliente, "510101");

            


            ViewBag.ClienteSeleccionado = objCliente;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            

            //var lstRecepciones = from s in db.Receptores.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID) select s;
            var lstRecepciones = db.Receptores.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                          r.tipoReceptor == "CL" &&
                                                          r.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                          r.DadoDeBaja == false);


            if (!String.IsNullOrEmpty(searchString))
            {
                lstRecepciones = lstRecepciones.Where(s => s.RazonSocial.Contains(searchString));
            }

            List<QuickReceptorModel> lstRecepcione2 = lstRecepciones.OrderByDescending(x => x.QuickReceptorModelID).AsQueryable().ToList();

            List<CuentaContableModel> CuentasContablesDelosReceptores = new List<CuentaContableModel>();

            foreach (QuickReceptorModel ReceptorRelacionado in lstRecepcione2)
            {
                CuentaContableModel CuentaDelReceptor = new CuentaContableModel();
                if (ReceptorRelacionado.CuentaConToReceptor != null) { 
                      CuentaDelReceptor = objCliente.CtaContable.SingleOrDefault(x => x.CuentaContableModelID == ReceptorRelacionado.CuentaConToReceptor.CuentaContableModelID);
                }

                if (CuentaDelReceptor.CuentaContableModelID != 0) { 
                    CuentasContablesDelosReceptores.Add(CuentaDelReceptor);
                }
            }

            if(CuentasContablesDelosReceptores.Count() > 0) { 
                ViewBag.CtasDelReceptor = CuentasContablesDelosReceptores;
            }

            ViewBag.Cliente = objCliente;


            /*
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            */

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            if (Exportar != null)
            {
                return ExportarClientes(lstRecepcione2, objEmisor);
            }

            return View(lstRecepcione2.ToPagedList(pageNumber, pageSize));
            /*CertificadosModels objCertificate = db.Certificados.Find(objEmisor.QuickEmisorModelID);
            if (objCertificate == null)
            {
                CustomErrorModel CError = new CustomErrorModel("No ha cargado certificado");
                return View("../Shared/CError", CError);
            }*/
        }

        [Authorize]
        public JsonResult GuardarRelacionCC(int IDReceptor, int CtaCont)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool Result = false;

            QuickReceptorModel ReceptorARelacionar = db.Receptores.SingleOrDefault(x => x.QuickReceptorModelID == IDReceptor);

            if(ReceptorARelacionar != null && objCliente != null)
            {
                CuentaContableModel CuentaARelacionar = objCliente.CtaContable.SingleOrDefault(x => x.CuentaContableModelID == CtaCont);
         

                if (CuentaARelacionar != null)
                {
                    ReceptorARelacionar.CuentaConToReceptor = CuentaARelacionar;
                    db.DBCuentaContable.AddOrUpdate(CuentaARelacionar);
                    db.SaveChanges();

                    Result = true;
                }
            }

            return Json(Result);
        }

        [Authorize]
        public ActionResult ListarProveedor(string sortOrder, string currentFilter, string searchString, int? page, string Exportar = null)
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.HtmlStr = ParseExtensions.ObtenerCuentaContableDropdownAsStringWithSelectedCodInterno(objCliente, "410101");

            //var lstRecepciones = from s in db.Receptores.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID) select s;
            var lstRecepciones = db.Receptores.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                          r.tipoReceptor == "PR" &&
                                                          objCliente.ClientesContablesModelID == r.ClientesContablesModelID &&
                                                          r.DadoDeBaja == false);
            if (!String.IsNullOrEmpty(searchString))
            {
                lstRecepciones = lstRecepciones.Where(s => s.RazonSocial.Contains(searchString));
            }

            List<QuickReceptorModel> lstRecepcione2 = new List<QuickReceptorModel>();
            if (lstRecepciones != null) {
               lstRecepcione2 = lstRecepciones.OrderByDescending(x => x.QuickReceptorModelID).AsQueryable().ToList();
            }
            List<QuickReceptorModel> lstReceptoresConCC = new List<QuickReceptorModel>();

            foreach (QuickReceptorModel ReceptorWithCC in lstRecepcione2)
            {
                if(ReceptorWithCC.CuentaConToReceptor != null)
                {
                    lstReceptoresConCC.Add(ReceptorWithCC);
                }
            }

            if(lstReceptoresConCC.Count() > 0) { 
              ViewBag.ListaReceptoresConCC = lstReceptoresConCC;
            }

            ViewBag.ObjCliente = objCliente;

            /*
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            */

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            if (Exportar != null)
            {
                return ExportarClientes(lstRecepcione2, objEmisor);
            }

            return View(lstRecepcione2.ToPagedList(pageNumber, pageSize));
            /*CertificadosModels objCertificate = db.Certificados.Find(objEmisor.QuickEmisorModelID);
            if (objCertificate == null)
            {
                CustomErrorModel CError = new CustomErrorModel("No ha cargado certificado");
                return View("../Shared/CError", CError);
            }*/
        }

        [Authorize]
        public ActionResult NuevoClienteHonorario(string NombrePHonor,string RutPHonor)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            QuickReceptorModel PHonorAInsertar = new QuickReceptorModel();

            int ValidarRedundancia = db.Receptores.Where(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                              x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                              x.RUT == RutPHonor &&
                                                              x.tipoReceptor == "H" &&
                                                              x.DadoDeBaja == false).Count();

            if(ValidarRedundancia == 0) { 

                if(!string.IsNullOrWhiteSpace(NombrePHonor) && !string.IsNullOrWhiteSpace(RutPHonor) &&  objCliente != null) { 

                    PHonorAInsertar.NombreFantasia = NombrePHonor;
                    PHonorAInsertar.RazonSocial = NombrePHonor;
                    PHonorAInsertar.RUT = RutPHonor;
                    PHonorAInsertar.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                    PHonorAInsertar.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
                    PHonorAInsertar.tipoReceptor = "H";

                    db.Receptores.Add(PHonorAInsertar);
                    db.SaveChanges();

                    TempData["Correcto"] = "Prestador ingresado con éxito.";

                }
                else
                {
                    TempData["Error"] = "Error inesperado.";
                }
            }
            else
            {
                TempData["Error"] = "Ya existe el prestador.";
            }

            return RedirectToAction("ListarHonorarios", "Home");
        }

        [Authorize]
        public ActionResult CrearPersona(string NombrePpersona, string RutPpersona)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            QuickReceptorModel PpersonaAInsertar = new QuickReceptorModel();

            int ValidarRedundancia = db.Receptores.Where(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                              x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                              x.RUT == RutPpersona &&
                                                              x.tipoReceptor == "P" &&
                                                              x.DadoDeBaja == false).Count();

            if (ValidarRedundancia == 0)
            {

                if (!string.IsNullOrWhiteSpace(NombrePpersona) && !string.IsNullOrWhiteSpace(RutPpersona) && objCliente != null)
                {

                    PpersonaAInsertar.NombreFantasia = NombrePpersona;
                    PpersonaAInsertar.RazonSocial = NombrePpersona;
                    PpersonaAInsertar.RUT = RutPpersona;
                    PpersonaAInsertar.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                    PpersonaAInsertar.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
                    PpersonaAInsertar.tipoReceptor = "P";

                    db.Receptores.Add(PpersonaAInsertar);
                    db.SaveChanges();

                    TempData["Correcto"] = "Prestador ingresado con éxito.";

                }
                else
                {
                    TempData["Error"] = "Error inesperado.";
                }
            }
            else
            {
                TempData["Error"] = "Ya existe el prestador.";
            }

            return RedirectToAction("ListarPersona", "Home");
        }

        [Authorize]
        public JsonResult ObtenerPHonorario(int IDPrestador)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objEmisor == null || objCliente == null || IDPrestador == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            QuickReceptorModel PhonorarioAEditar = db.Receptores.SingleOrDefault(x => x.QuickReceptorModelID == IDPrestador);
            if (PhonorarioAEditar == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = true,
                    IDPrestador = PhonorarioAEditar.QuickReceptorModelID,
                    IDcienteContable = PhonorarioAEditar.ClientesContablesModelID,
                    NombrePrestador = PhonorarioAEditar.RazonSocial,
                    RutPrestador = PhonorarioAEditar.RUT
              
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult EditarPHonorario(int IDPrestadorE, string NombrePHonorE, string RutPHonorE)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            QuickReceptorModel PHonorarioAEditar = db.Receptores.SingleOrDefault(x => x.QuickReceptorModelID == IDPrestadorE);

            if(PHonorarioAEditar != null)
            {
                PHonorarioAEditar.NombreFantasia = NombrePHonorE;
                PHonorarioAEditar.RazonSocial = NombrePHonorE;
                PHonorarioAEditar.RUT = RutPHonorE;
                db.Receptores.AddOrUpdate(PHonorarioAEditar);
                db.SaveChanges();

                TempData["Correcto"] = "Prestador actualizado con éxito.";

            }else
            {
                TempData["Error"] = "El prestador no existe";
            }

            return RedirectToAction("ListarHonorarios","Home");
        }

        [Authorize]
        public JsonResult ObtenerPpersona(int IDPrestador)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            if (objEmisor == null || objCliente == null || IDPrestador == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            QuickReceptorModel PpersonaAEditar = db.Receptores.SingleOrDefault(x => x.QuickReceptorModelID == IDPrestador);
            if (PpersonaAEditar == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = true,
                    IDPrestador = PpersonaAEditar.QuickReceptorModelID,
                    IDcienteContable = PpersonaAEditar.ClientesContablesModelID,
                    NombrePrestador = PpersonaAEditar.RazonSocial,
                    RutPrestador = PpersonaAEditar.RUT

                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult EditarPrestadorPersona(int IDPrestadorE, string NombrePpersonaE, string RutPpersonaE)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            QuickReceptorModel PpersonaAEditar = db.Receptores.SingleOrDefault(x => x.QuickReceptorModelID == IDPrestadorE);

            if (PpersonaAEditar != null)
            {
                PpersonaAEditar.NombreFantasia = NombrePpersonaE;
                PpersonaAEditar.RazonSocial = NombrePpersonaE;
                PpersonaAEditar.RUT = RutPpersonaE;
                db.Receptores.AddOrUpdate(PpersonaAEditar);
                db.SaveChanges();

                TempData["Correcto"] = "Prestador actualizado con éxito.";

            }
            else
            {
                TempData["Error"] = "El prestador no existe";
            }

            return RedirectToAction("ListarPersona", "Home");
        }

        [Authorize]
        public ActionResult ListarHonorarios()
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<QuickReceptorModel> lstReceptoresHonorarios = new List<QuickReceptorModel>();

            if(objCliente != null) { 

            lstReceptoresHonorarios = db.Receptores.Where(x => x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                               x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                               x.tipoReceptor == "H" &&
                                                               x.DadoDeBaja == false).ToList();
            }else
            {
              return RedirectToAction("SeleccionarClienteContable", "Contabilidad");
            }

            return View(lstReceptoresHonorarios);
        }

        [Authorize]
        [ModuloHandler]
        
        public ActionResult ListarPersona(string sortOrder, string currentFilter, string searchString, int? page, string Exportar = null)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            

            //var lstRecepciones = from s in db.Receptores.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID) select s;
            var lstRecepciones = db.Receptores.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                          r.tipoReceptor == "P" &&
                                                          r.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                          r.DadoDeBaja == false);
            if (!String.IsNullOrEmpty(searchString))
            {
                lstRecepciones = lstRecepciones.Where(s => s.RazonSocial.Contains(searchString));
            }

            List<QuickReceptorModel> lstRecepcione2 = lstRecepciones.OrderByDescending(x => x.QuickReceptorModelID).AsQueryable().ToList();

            /*
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            */

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            if (Exportar != null)
            {
                return ExportarClientes(lstRecepcione2, objEmisor);
            }

            return View(lstRecepcione2.ToPagedList(pageNumber, pageSize));
            /*CertificadosModels objCertificate = db.Certificados.Find(objEmisor.QuickEmisorModelID);
            if (objCertificate == null)
            {
                CustomErrorModel CError = new CustomErrorModel("No ha cargado certificado");
                return View("../Shared/CError", CError);
            }*/
        }

        [Authorize]
        public ActionResult ReestablecerPrestador(string TipoPrestador = "")
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            List<QuickReceptorModel> lstPrestador = new List<QuickReceptorModel>();
            
            //Lista Prestador Clientes.
            if(!string.IsNullOrWhiteSpace(TipoPrestador)){
                
                lstPrestador = db.Receptores.Where(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                        x.tipoReceptor == TipoPrestador &&
                                                        x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                        x.DadoDeBaja == true).ToList();


                if (TipoPrestador == "CL")
                    ViewBag.TipoPrestador = "Clientes";
                else if (TipoPrestador == "P")
                    ViewBag.TipoPrestador = "Personas";
                else if (TipoPrestador == "PR")
                    ViewBag.TipoPrestador = "Proveedores";
                else if (TipoPrestador == "H")
                    ViewBag.TipoPrestador = "Honorarios";
                else
                    TempData["Error"] = "Error inesperado";
            }
           
        
            return View(lstPrestador);
        }

       

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
     
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        public ActionResult GetReceptorByRUT(string RUT)
        {
            string UserIdentityID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserIdentityID);
            if (string.IsNullOrEmpty(UserIdentityID))
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);

            QuickEmisorModel QryEmisores = PerfilamientoModule.GetEmisorSeleccionado(Session, UserIdentityID);
            //QuickEmisorModel QryEmisores = db.Emisores.Single(i => i.IdentityID == UserIdentityID);

            IQueryable<QuickReceptorModel> lstReceptores = db.Receptores.Where(i => i.RUT == RUT && i.QuickEmisorModelID == QryEmisores.QuickEmisorModelID);
            List<QuickReceptorModel> ListaReceptores = lstReceptores.ToList();
            if (ListaReceptores == null || ListaReceptores.Count == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ok = true,
                    rzSocial = ListaReceptores[0].RazonSocial,
                    direccion = ListaReceptores[0].Direccion,
                    comuna = ListaReceptores[0].Comuna,
                    ciudad = ListaReceptores[0].Ciudad,
                    giro = ListaReceptores[0].Giro,
                    contacto = ListaReceptores[0].Contacto,
                    telefonoCobranza = ListaReceptores[0].TelefonoContacto
                }, JsonRequestBehavior.AllowGet);
            }
        }


        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        [HttpPost]
        public ActionResult Updatecliente(QuickReceptorModel model) {

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db1 = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db1.DBRegiones.ToList();
            List<ComunaModels> comunas = db1.DBComunas.ToList();
            //     QuickReceptorModel model = new QuickReceptorModel();
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;


            if (!ModelState.IsValid)
            {
                return View("EditarCliente", model);
            }

            var db = new FacturaContext();
            var dbProd = new FacturaProduccionContext();


            string UserIdentityID = User.Identity.GetUserId();
            QuickEmisorModel objEmisorSelected = PerfilamientoModule.GetEmisorSeleccionado(Session, UserIdentityID);
            //IMPORTANT SOLVE THIS ONE
            //QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            //QuickEmisorModel objEmisorProd = dbProd.Emisores.SingleOrDefault(i => i.IdentityID == UserIdentityID);

            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorCertificacion(userID, )
            QuickEmisorModel objEmisorProd = dbProd.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorProduccion 


            QuickReceptorModel objReceptor = db.Receptores.SingleOrDefault(r => r.QuickReceptorModelID == model.QuickReceptorModelID); //new QuickReceptorModel();

            IQueryable<QuickReceptorModel> QryReceptores = db.Receptores.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<QuickReceptorModel> ListaReceptores = QryReceptores.ToList();
            if (ListaReceptores != null && ListaReceptores.Count > 0)
                objReceptor.QuickReceptorModelID = ListaReceptores[0].QuickReceptorModelID;
            objReceptor.RUT = model.RUT;
            objReceptor.RUTSolicitante = model.RUTSolicitante;
            objReceptor.RazonSocial = model.RazonSocial;
            objReceptor.NombreFantasia = model.NombreFantasia;
            objReceptor.NombreContacto = model.NombreContacto;
            objReceptor.Giro = model.Giro;
            objReceptor.Direccion = model.Direccion;
            objReceptor.Contacto = model.Contacto;
            objReceptor.TelefonoContacto = model.TelefonoContacto;


            objReceptor.idRegion = model.idRegion;
            objReceptor.idComuna = model.idComuna;


            objReceptor.Ciudad = regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptor.Comuna = comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);

            



            //QuickReceptorModel objReceptorProd = new QuickReceptorModel();
            QuickReceptorModel objReceptorProd = db.Receptores.SingleOrDefault(r => r.QuickReceptorModelID == model.QuickReceptorModelID); 
            IQueryable<QuickReceptorModel> QryReceptoresProd = dbProd.Receptores.Where(i => i.QuickEmisorModelID == objEmisorProd.QuickEmisorModelID);
            List<QuickReceptorModel> ListaReceptoresProd = QryReceptoresProd.ToList();
            if (ListaReceptoresProd != null && ListaReceptoresProd.Count > 0)
                objReceptorProd = ListaReceptoresProd[0];
            objReceptorProd.RUT = model.RUT;
            objReceptorProd.RUTSolicitante = model.RUTSolicitante;
            objReceptorProd.RazonSocial = model.RazonSocial;
            objReceptorProd.NombreFantasia = model.NombreFantasia;
            objReceptorProd.NombreContacto = model.NombreContacto;
            objReceptorProd.Giro = model.Giro;
            objReceptorProd.Direccion = model.Direccion;
            objReceptorProd.Contacto = model.Contacto;
            objReceptorProd.TelefonoContacto = model.TelefonoContacto;

            objReceptorProd.idRegion = model.idRegion;
            objReceptorProd.idComuna = model.idComuna;


            objReceptorProd.Ciudad = regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptorProd.Comuna = comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);

            objReceptor.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
            if (objReceptor.RUT != "5555555-5")
            {
                db.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT }, objReceptor);
            }
            else
            {
                db.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT, c.RazonSocial }, objReceptor);
            }

            objReceptorProd.QuickEmisorModelID = objEmisorProd.QuickEmisorModelID;
            if (objReceptorProd.RUT != "5555555-5")
            {
                dbProd.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT }, objReceptorProd);
            }
            else
            {
                dbProd.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT, c.RazonSocial }, objReceptorProd);
            }

            db.SaveChanges();
            dbProd.SaveChanges();

            return RedirectToAction("Index", "Home");



        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateclienteC(QuickReceptorModel model)
        {

            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db1 = ParseExtensions.GetDatabaseContext(UserID);
            List<RegionModels> regiones = db1.DBRegiones.ToList();
            List<ComunaModels> comunas = db1.DBComunas.ToList();
            //     QuickReceptorModel model = new QuickReceptorModel();
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;


            if (!ModelState.IsValid)
            {
                return View("EditarCliente", model);
            }

            var db = new FacturaContext();
            var dbProd = new FacturaProduccionContext();


            string UserIdentityID = User.Identity.GetUserId();
            QuickEmisorModel objEmisorSelected = PerfilamientoModule.GetEmisorSeleccionado(Session, UserIdentityID);
            //IMPORTANT SOLVE THIS ONE
            //QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            //QuickEmisorModel objEmisorProd = dbProd.Emisores.SingleOrDefault(i => i.IdentityID == UserIdentityID);




            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorCertificacion(userID, )
            QuickEmisorModel objEmisorProd = dbProd.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorProduccion 


            QuickReceptorModel objReceptor = db.Receptores.SingleOrDefault(r => r.QuickReceptorModelID == model.QuickReceptorModelID); //new QuickReceptorModel();

            IQueryable<QuickReceptorModel> QryReceptores = db.Receptores.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<QuickReceptorModel> ListaReceptores = QryReceptores.ToList();
            if (ListaReceptores != null && ListaReceptores.Count > 0)
                objReceptor.QuickReceptorModelID = ListaReceptores[0].QuickReceptorModelID;
            objReceptor.RUT = model.RUT;
            objReceptor.RUTSolicitante = model.RUTSolicitante;
            objReceptor.RazonSocial = model.RazonSocial;
            objReceptor.NombreFantasia = model.NombreFantasia;
            objReceptor.NombreContacto = model.NombreContacto;
            objReceptor.Giro = model.Giro;
            objReceptor.Direccion = model.Direccion;
            objReceptor.Contacto = model.Contacto;
            objReceptor.TelefonoContacto = model.TelefonoContacto;


            objReceptor.idRegion = model.idRegion;
            objReceptor.idComuna = model.idComuna;


            objReceptor.Ciudad = regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptor.Comuna = comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);





            //QuickReceptorModel objReceptorProd = new QuickReceptorModel();
            QuickReceptorModel objReceptorProd = db.Receptores.SingleOrDefault(r => r.QuickReceptorModelID == model.QuickReceptorModelID);
            IQueryable<QuickReceptorModel> QryReceptoresProd = dbProd.Receptores.Where(i => i.QuickEmisorModelID == objEmisorProd.QuickEmisorModelID);
            List<QuickReceptorModel> ListaReceptoresProd = QryReceptoresProd.ToList();
            if (ListaReceptoresProd != null && ListaReceptoresProd.Count > 0)
                objReceptorProd = ListaReceptoresProd[0];
            objReceptorProd.RUT = model.RUT;
            objReceptorProd.RUTSolicitante = model.RUTSolicitante;
            objReceptorProd.RazonSocial = model.RazonSocial;
            objReceptorProd.NombreFantasia = model.NombreFantasia;
            objReceptorProd.NombreContacto = model.NombreContacto;
            objReceptorProd.Giro = model.Giro;
            objReceptorProd.Direccion = model.Direccion;
            objReceptorProd.Contacto = model.Contacto;
            objReceptorProd.TelefonoContacto = model.TelefonoContacto;

            objReceptorProd.idRegion = model.idRegion;
            objReceptorProd.idComuna = model.idComuna;


            objReceptorProd.Ciudad = regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptorProd.Comuna = comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);





            objReceptor.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
            if (objReceptor.RUT != "5555555-5")
            {
                db.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT }, objReceptor);
            }
            else
            {
                db.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT, c.RazonSocial }, objReceptor);
            }

            objReceptorProd.QuickEmisorModelID = objEmisorProd.QuickEmisorModelID;
            if (objReceptorProd.RUT != "5555555-5")
            {
                dbProd.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT }, objReceptorProd);
            }
            else
            {
                dbProd.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT, c.RazonSocial }, objReceptorProd);
            }

            db.SaveChanges();
            dbProd.SaveChanges();

            return RedirectToAction("ListarClienteC", "Home");



        }

        public ActionResult UpdateReceptorA(QuickReceptorModel model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            model.idComuna = 1;
            model.idRegion = 1;

            if (!ModelState.IsValid)
            {
                return View("NuevoCliente", model);
            }
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            QuickReceptorModel objReceptorNew = db.Receptores.SingleOrDefault(r => r.QuickReceptorModelID == model.QuickReceptorModelID);

            objReceptorNew.RazonSocial = model.RazonSocial;
            objReceptorNew.NombreFantasia = model.NombreFantasia;
            objReceptorNew.NombreContacto = model.NombreContacto;
            objReceptorNew.Giro = model.Giro;
            objReceptorNew.Direccion = model.Direccion;
            objReceptorNew.Contacto = model.Contacto;

            objReceptorNew.TelefonoContacto = model.TelefonoContacto;


            objReceptorNew.idRegion = model.idRegion;
            objReceptorNew.idComuna = model.idComuna;


            objReceptorNew.Ciudad = regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptorNew.Comuna = comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);

           


            /*
            objReceptorNew

            objReceptorNew.RUT = model.RUT;
            objReceptorNew.RUTSolicitante = model.RUTSolicitante;
           
            */
            TryUpdateModel<QuickReceptorModel>(objReceptorNew);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult UpdateReceptorC(QuickReceptorModel model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            model.idComuna = 1;
            model.idRegion = 1;

            if (!ModelState.IsValid)
            {
                return View("NuevoCliente", model);
            }
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            QuickReceptorModel objReceptorNew = db.Receptores.SingleOrDefault(r => r.QuickReceptorModelID == model.QuickReceptorModelID);

            objReceptorNew.RazonSocial = model.RazonSocial;
            objReceptorNew.NombreFantasia = model.NombreFantasia;
            objReceptorNew.NombreContacto = model.NombreContacto;
            objReceptorNew.Giro = model.Giro;
            objReceptorNew.Direccion = model.Direccion;
            objReceptorNew.Contacto = model.Contacto;

            objReceptorNew.TelefonoContacto = model.TelefonoContacto;


            objReceptorNew.idRegion = model.idRegion;
            objReceptorNew.idComuna = model.idComuna;


            objReceptorNew.Ciudad = regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptorNew.Comuna = comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);




            /*
            objReceptorNew

            objReceptorNew.RUT = model.RUT;
            objReceptorNew.RUTSolicitante = model.RUTSolicitante;
           
            */
            TryUpdateModel<QuickReceptorModel>(objReceptorNew);
            db.SaveChanges();

            return RedirectToAction("ListarClienteC", "Home");
        }

        public ActionResult UpdateReceptorProveedor(QuickReceptorModel model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            model.idComuna = 1;
            model.idRegion = 1;

            if (!ModelState.IsValid)
            {
                return View("EditarProveedor", model);
            }
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            QuickReceptorModel objReceptorNew = db.Receptores.SingleOrDefault(r => r.QuickReceptorModelID == model.QuickReceptorModelID);

            objReceptorNew.RazonSocial = model.RazonSocial;
            objReceptorNew.NombreFantasia = model.NombreFantasia;
            objReceptorNew.NombreContacto = model.NombreContacto;
            objReceptorNew.Giro = model.Giro;
            objReceptorNew.Direccion = model.Direccion;
            objReceptorNew.Contacto = model.Contacto;

            objReceptorNew.TelefonoContacto = model.TelefonoContacto;


            objReceptorNew.idRegion = model.idRegion;
            objReceptorNew.idComuna = model.idComuna;


            objReceptorNew.Ciudad = regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptorNew.Comuna = comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);




            /*
            objReceptorNew

            objReceptorNew.RUT = model.RUT;
            objReceptorNew.RUTSolicitante = model.RUTSolicitante;
           
            */
            TryUpdateModel<QuickReceptorModel>(objReceptorNew);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult UpdateReceptorPersona(QuickReceptorModel model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            model.idComuna = 1;
            model.idRegion = 1;

            if (!ModelState.IsValid)
            {
                return View("NuevoPersona", model);
            }
            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            QuickReceptorModel objReceptorNew = db.Receptores.SingleOrDefault(r => r.QuickReceptorModelID == model.QuickReceptorModelID);

            objReceptorNew.RazonSocial = model.RazonSocial;
            objReceptorNew.NombreFantasia = model.NombreFantasia;
            objReceptorNew.NombreContacto = model.NombreContacto;
            objReceptorNew.Giro = model.Giro;
            objReceptorNew.Direccion = model.Direccion;
            objReceptorNew.Contacto = model.Contacto;

            objReceptorNew.TelefonoContacto = model.TelefonoContacto;


            objReceptorNew.idRegion = model.idRegion;
            objReceptorNew.idComuna = model.idComuna;


            objReceptorNew.Ciudad = regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptorNew.Comuna = comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);

            /*
            objReceptorNew
            objReceptorNew.RUT = model.RUT;
            objReceptorNew.RUTSolicitante = model.RUTSolicitante;
            */
            TryUpdateModel<QuickReceptorModel>(objReceptorNew);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [Authorize]
        [HttpPost]
        public ActionResult UpsertCliente(QuickReceptorModel model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
           

            if (!ModelState.IsValid)
            {
                return View("EditarCliente", model);
            }

            //var db = new FacturaContext();
            //var dbProd = new FacturaProduccionContext();


            string UserIdentityID = User.Identity.GetUserId();
            QuickEmisorModel objEmisorSelected = PerfilamientoModule.GetEmisorSeleccionado(Session, UserIdentityID);

            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorCertificacion(userID, )
            //QuickEmisorModel objEmisorProd = dbProd.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorProduccion 


            QuickReceptorModel objReceptor = new QuickReceptorModel();

            IQueryable<QuickReceptorModel> QryReceptores = db.Receptores.Where(i => i.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<QuickReceptorModel> ListaReceptores = QryReceptores.ToList();
            if (ListaReceptores != null && ListaReceptores.Count > 0)
                objReceptor.QuickReceptorModelID = ListaReceptores[0].QuickReceptorModelID;
            objReceptor.RUT = model.RUT;
            objReceptor.RUTSolicitante = model.RUTSolicitante;
            objReceptor.RazonSocial = model.RazonSocial;
            objReceptor.NombreFantasia = model.NombreFantasia;
            objReceptor.NombreContacto = model.NombreContacto;
            objReceptor.Giro = model.Giro;
            objReceptor.Direccion = model.Direccion;
            objReceptor.Contacto = model.Contacto;

            objReceptor.TelefonoContacto = model.TelefonoContacto;


            objReceptor.idRegion = model.idRegion;
            objReceptor.idComuna = model.idComuna;

            objReceptor.tipoReceptor = "CL";


            objReceptor.Ciudad = db.DBRegiones.SingleOrDefault(r => r.RegionModelsID == objReceptor.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);//regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptor.Comuna = db.DBComunas.SingleOrDefault(r => r.ComunaModelsID == objReceptor.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);//comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);


            /*
            QuickReceptorModel objReceptorProd = new QuickReceptorModel();
            IQueryable<QuickReceptorModel> QryReceptoresProd = dbProd.Receptores.Where(i => i.QuickEmisorModelID == objEmisorProd.QuickEmisorModelID);
            List<QuickReceptorModel> ListaReceptoresProd = QryReceptoresProd.ToList();
            if (ListaReceptoresProd != null && ListaReceptoresProd.Count > 0)
                objReceptorProd = ListaReceptoresProd[0];
            objReceptorProd.RUT = model.RUT;
            objReceptorProd.RUTSolicitante = model.RUTSolicitante;
            objReceptorProd.RazonSocial = model.RazonSocial;
            objReceptorProd.NombreFantasia = model.NombreFantasia;
            objReceptorProd.NombreContacto = model.NombreContacto;
            objReceptorProd.Giro = model.Giro;
            objReceptorProd.Direccion = model.Direccion;
            objReceptorProd.Contacto = model.Contacto;

            objReceptorProd.TelefonoContacto = model.TelefonoContacto;

            objReceptorProd.idRegion = model.idRegion;
            objReceptorProd.idComuna = model.idComuna;

            objReceptorProd.Ciudad = dbProd.DBRegiones.SingleOrDefault(r => r.RegionModelsID == objReceptorProd.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptorProd.Comuna = dbProd.DBComunas.SingleOrDefault(r => r.ComunaModelsID == objReceptorProd.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);
            */

            objReceptor.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
            if (objReceptor.RUT != "5555555-5")
            {
                db.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT }, objReceptor);
            }
            else
            {
                db.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT, c.RazonSocial }, objReceptor);
            }

            /*
            objReceptorProd.QuickEmisorModelID = objEmisorProd.QuickEmisorModelID;
            if (objReceptorProd.RUT != "5555555-5")
            {
                dbProd.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT }, objReceptorProd);
            }
            else
            {
                dbProd.Receptores.AddOrUpdate(c => new { c.QuickEmisorModelID, c.RUT, c.RazonSocial }, objReceptorProd);
            }*/

            db.SaveChanges();
            //dbProd.SaveChanges();

            return RedirectToAction("ListarCliente", "Home");
        }

        [Authorize]
        public ActionResult UpsertClienteC(QuickReceptorModel model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            //     QuickReceptorModel model = new QuickReceptorModel();
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;


            if (!ModelState.IsValid)
            {
                return View("EditarClienteC", model);
            }

      

            string UserIdentityID = User.Identity.GetUserId();
            QuickEmisorModel objEmisorSelected = PerfilamientoModule.GetEmisorSeleccionado(Session, UserIdentityID);

            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorCertificacion(userID, )
            //QuickEmisorModel objEmisorProd = dbProd.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorProduccion 


            QuickReceptorModel objReceptor = new QuickReceptorModel();
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            int ValidarRedundancia = db.Receptores.Where(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                              x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                              x.tipoReceptor == "CL" &&
                                                              x.RUT == model.RUT &&
                                                              x.DadoDeBaja == false).Count();
            if(ValidarRedundancia == 0) { 

            objReceptor.RUT = model.RUT;
            objReceptor.RUTSolicitante = model.RUTSolicitante;
            objReceptor.RazonSocial = model.RazonSocial;
            objReceptor.NombreFantasia = model.NombreFantasia;
            objReceptor.NombreContacto = model.NombreContacto;
            objReceptor.Giro = model.Giro;
            objReceptor.Direccion = model.Direccion;
            objReceptor.Contacto = model.Contacto;

            objReceptor.TelefonoContacto = model.TelefonoContacto;


            objReceptor.idRegion = model.idRegion;
            objReceptor.idComuna = model.idComuna;

            objReceptor.ClientesContablesModelID = objCliente.ClientesContablesModelID;
            objReceptor.tipoReceptor = "CL";


            objReceptor.Ciudad = db.DBRegiones.SingleOrDefault(r => r.RegionModelsID == objReceptor.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);//regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptor.Comuna = db.DBComunas.SingleOrDefault(r => r.ComunaModelsID == objReceptor.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);//comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);

            objReceptor.QuickEmisorModelID = objEmisor.QuickEmisorModelID;

            db.Receptores.Add(objReceptor);
            db.SaveChanges();
            //dbProd.SaveChanges();

            TempData["Correcto"] = "Prestador agregado correctamente";
            }else
            {
                TempData["Error"] = "Ya existe el prestador.";
            }
            return RedirectToAction("ListarClienteC", "Home");
        }

        [Authorize]
        public ActionResult UpsertProveedor(QuickReceptorModel model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            //     QuickReceptorModel model = new QuickReceptorModel();
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;


            if (!ModelState.IsValid)
            {
                return View("NuevoProveedor", model);
            }

            //var db = new FacturaContext();
            //var dbProd = new FacturaProduccionContext();


            string UserIdentityID = User.Identity.GetUserId();
            QuickEmisorModel objEmisorSelected = PerfilamientoModule.GetEmisorSeleccionado(Session, UserIdentityID);

            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorCertificacion(userID, )
            //QuickEmisorModel objEmisorProd = dbProd.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorProduccion 


            QuickReceptorModel objReceptor = new QuickReceptorModel();
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            int ValidarRedundancia  = db.Receptores.Where(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                               x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                               x.tipoReceptor == "PR" &&
                                                               x.RUT == model.RUT &&
                                                               x.DadoDeBaja == false).Count();
           
            if (ValidarRedundancia == 0) { 
  
            objReceptor.RUT = model.RUT;
            objReceptor.RUTSolicitante = model.RUTSolicitante;
            objReceptor.RazonSocial = model.RazonSocial;
            objReceptor.NombreFantasia = model.NombreFantasia;
            objReceptor.NombreContacto = model.NombreContacto;
            objReceptor.Giro = model.Giro;
            objReceptor.Direccion = model.Direccion;
            objReceptor.Contacto = model.Contacto;
            objReceptor.tipoReceptor = "PR";
            objReceptor.ClientesContablesModelID = objCliente.ClientesContablesModelID;

            objReceptor.TelefonoContacto = model.TelefonoContacto;


            objReceptor.idRegion = model.idRegion;
            objReceptor.idComuna = model.idComuna;


            objReceptor.Ciudad = db.DBRegiones.SingleOrDefault(r => r.RegionModelsID == objReceptor.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);//regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
            objReceptor.Comuna = db.DBComunas.SingleOrDefault(r => r.ComunaModelsID == objReceptor.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);//comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);

            objReceptor.QuickEmisorModelID = objEmisor.QuickEmisorModelID;

            db.Receptores.Add(objReceptor);     
            db.SaveChanges();
            //dbProd.SaveChanges();

            TempData["Correcto"] = "Prestador agregado correctamente";
            }else
            {
                TempData["Error"] = "El prestador ya existe.";
            }
            return RedirectToAction("ListarProveedor", "Home");
        }

        [Authorize]
        public ActionResult UpsertPersona(QuickReceptorModel model)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();

            //     QuickReceptorModel model = new QuickReceptorModel();
            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;


            if (!ModelState.IsValid)
            {
                return View("EditarPersona", model);
            }

            //var db = new FacturaContext();
            //var dbProd = new FacturaProduccionContext();


            string UserIdentityID = User.Identity.GetUserId();
            QuickEmisorModel objEmisorSelected = PerfilamientoModule.GetEmisorSeleccionado(Session, UserIdentityID);

            QuickEmisorModel objEmisor = db.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorCertificacion(userID, )
            //QuickEmisorModel objEmisorProd = dbProd.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorSelected.IdentityIDEmisor);//PerfilamientoModule.GetEmisorProduccion 


            QuickReceptorModel objReceptor = new QuickReceptorModel();
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            //Buscamos que no exista.
            int ValidaRedundancia = db.Receptores.Where(x => x.QuickEmisorModelID == objEmisor.QuickEmisorModelID &&
                                                             x.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                             x.tipoReceptor == "P" &&
                                                             x.RUT == model.RUT).Count();

            if (ValidaRedundancia == 0)
            {
                objReceptor.RUT = model.RUT;
                objReceptor.RUTSolicitante = model.RUTSolicitante;
                objReceptor.RazonSocial = model.RazonSocial;
                objReceptor.NombreFantasia = model.NombreFantasia;
                objReceptor.NombreContacto = model.NombreContacto;
                objReceptor.Giro = model.Giro;
                objReceptor.Direccion = model.Direccion;
                objReceptor.Contacto = model.Contacto;
                objReceptor.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                objReceptor.tipoReceptor = "P";

                objReceptor.TelefonoContacto = model.TelefonoContacto;

                objReceptor.idRegion = model.idRegion;
                objReceptor.idComuna = model.idComuna;

                objReceptor.Ciudad = db.DBRegiones.SingleOrDefault(r => r.RegionModelsID == objReceptor.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);//regiones.ElementAt(model.idRegion).nombre.PadRight(20, ' ').Substring(0, 20);
                objReceptor.Comuna = db.DBComunas.SingleOrDefault(r => r.ComunaModelsID == objReceptor.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);//comunas.ElementAt(model.idComuna).nombre.PadRight(20, ' ').Substring(0, 20);

                objReceptor.QuickEmisorModelID = objEmisor.QuickEmisorModelID;


                db.Receptores.Add(objReceptor);
                db.SaveChanges();
                //dbProd.SaveChanges();
                TempData["Correcto"] = "Prestador agregado correctamente";
            }else
            {
                TempData["Error"] = "Este Prestador ya existe.";
            }
            return RedirectToAction("ListarPersona", "Home");
        }

        [Authorize]
        public JsonResult DarDeBajaPrestador(int IDreceptor)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool Result = false;
            QuickReceptorModel ReceptorADarDeBaja = db.Receptores.Single(x => x.QuickReceptorModelID == IDreceptor);

            if(ReceptorADarDeBaja != null)
            {
                ReceptorADarDeBaja.DadoDeBaja = true;
                db.SaveChanges();
                Result = true;
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult ActivarPrestador(int IDreceptor)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            bool result = false;

            QuickReceptorModel PrestadorActivar = db.Receptores.SingleOrDefault(x => x.QuickReceptorModelID == IDreceptor);

            if(PrestadorActivar != null)
            {
                PrestadorActivar.DadoDeBaja = false;
                db.Receptores.AddOrUpdate(PrestadorActivar);
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
      
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        [HttpPost]
        [Authorize]
        public ActionResult UpdateEstadoResultado(EstadoResultadoModel model)
        {

            string UserIdentityID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserIdentityID);

            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserIdentityID);
            //QuickEmisorModel objEmisor = db.Emisores.Single(i => i.IdentityID == UserIdentityID);

            EstadoResultadoModel objFlujo = model;

            objFlujo.QuickEmisorModelID = objEmisor.QuickEmisorModelID;

            int MesOut = -1;
            int YearOut = -1;

            Int32.TryParse(Request.Form.GetValues("Periodo")[0].ToString(), out MesOut);
            Int32.TryParse(Request.Form.GetValues("Anio")[0].ToString(), out YearOut);

            DateTime periodoFlujo = new DateTime(YearOut, MesOut, 1);

            objFlujo.Periodo = periodoFlujo;


            objFlujo.BrutoHonorarios = model.BrutoHonorarios;
            objFlujo.BrutoRemuneraciones = model.BrutoRemuneraciones;
            objFlujo.GastosVarios = model.GastosVarios;
            objFlujo.IngresosVarios = model.IngresosVarios;

            db.EstadoResultado.AddOrUpdate(c => c.Periodo, objFlujo);
            db.SaveChanges();

            return RedirectToAction("EstadoResultado", new { Periodo = objFlujo.Periodo.Month.ToString(), Anio = objFlujo.Periodo.Year.ToString() });
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
        

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
      
        public static int HtmlToPdf(string inputFilePath, string outputFilename)
        {
            //Process P = Process.Start(ConfigurationManager.AppSettings["HtmlToPdfExePath"], inputFilePath + " " + outputFilename);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = ConfigurationManager.AppSettings["HtmlToPdfExePath"];
            startInfo.Arguments = inputFilePath + " " + outputFilename;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process P = Process.Start(startInfo);

            // ...then wait n milliseconds for exit (as after exit, it can't read the output)
            P.WaitForExit(20000);

            // read the exit code, close process
            if (!P.HasExited)
                P.Kill();

            int result = P.ExitCode;

            // if 0 or 2, it worked (not sure about other values, I want a better way to confirm this)
            //return (returnCode == 0 || returnCode == 2);
            return result;
        }

        public ActionResult SendCorreo()
        {
            string secretKey = "6LeqAk8UAAAAAAA8bHAq4bHKIYcTNT7Fb5wmJtJT";
            string userResponse = Request.Form["g-Recaptcha-Response"];

            var webClient = new System.Net.WebClient();
            string verification = webClient.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, userResponse));

            var verificationJson = Newtonsoft.Json.Linq.JObject.Parse(verification);
            if (verificationJson["success"].Value<bool>() == false)
            {
                return RedirectToAction("Homey", "Home");
                /*
                Session["I_AM_NOT_A_ROBOT"] = "true";
                return RedirectToAction("Index", "Demo");*/
            }
            else
            {
                string _name = Request.Form.GetValues("nameContact")[0];
                string _nameEmpresa = Request.Form.GetValues("nameEmpresa")[0];
                string _email = Request.Form.GetValues("email")[0];
                string _phone = Request.Form.GetValues("phone")[0];
                string _msg = Request.Form.GetValues("message")[0];
                MailHelper.SendMailSoporte(_name, _email, _phone, _msg, _nameEmpresa);
                return RedirectToAction("Homey", "Home");
                /*
				return Json(new
				{
					name = "captch",
					ok = false
					//cuenta = Listafacturas.Count,
				}, JsonRequestBehavior.AllowGet);*/
            }
        }

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
   
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
      
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
  
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
     

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
     
        [Authorize]
        [ModuloHandler]
        public ActionResult MakeFactura(string opt, int? facturaID)
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = null;

            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            if (UserID == null || UserID == string.Empty)
            {
                UserID = "AAA";
            }

            objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            /*
            IQueryable<QuickEmisorModel> lstModel = db.Emisores.Include("Certificados").Where(i => i.IdentityID == UserID);
            List<QuickEmisorModel> ListaEmisores = lstModel.ToList();
            if (ListaEmisores != null && ListaEmisores.Count > 0)
                objEmisor = ListaEmisores[0];
                */

            bool PackAndSend = true;
            if (!string.IsNullOrEmpty(opt))
                PackAndSend = false;

            Tuple<QuickEmisorModel, bool> rtrnVal = new Tuple<QuickEmisorModel, bool>(objEmisor, PackAndSend);
            return View(rtrnVal);
        }

    

        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Informador)]
  

        [Authorize]
        public ActionResult VLibroVentas()
        {
            return View();
        }
        [Authorize]
        public ActionResult VLibroCompras()
        {
            return View();
        }
        [Authorize]
        public ActionResult VLibroRemuneraciones()
        {
            return View();
        }
        [Authorize]
        public ActionResult VLibroHonorarios()
        {
            return View();
        }

        public FileContentResult ExportarFlujoCaja(List<List<string>> lstInfo, QuickEmisorModel objEmisor, int YearToLook, List<string> TitleData, string titleOverrideTo = "", bool skipAddTotalToTitles = false)
        {
            bool BleachMembrette = false;

            List<string[]> importoData = new List<string[]>();
            foreach (List<string> INFO in lstInfo) 
            {
                importoData.Add(INFO.ToArray());
            }

            if(skipAddTotalToTitles == false)
                TitleData.Add("TOTALES");
            string[] Titulos = TitleData.ToArray();
            List<string[]> ExcelTitleData = new List<string[]>();
            ExcelTitleData.Add(Titulos);

            string RutaExportPagosTemaple = ParseExtensions.Get_AppData_Path("exportFlujoCaja.xlsx");
            using (XLWorkbook excelFile = new XLWorkbook(RutaExportPagosTemaple))
            {
                var workSheet = excelFile.Worksheet(1);
                /*
                //SETUP MEMBRETTEEEEE
                if (BleachMembrette)
                {
                    workSheet.Cells("A1").Value = string.Empty;
                    workSheet.Cells("A2").Value = string.Empty;
                    workSheet.Cells("A3").Value = string.Empty;
                    workSheet.Cells("A4").Value = string.Empty;
                    workSheet.Cells("A5").Value = string.Empty;
                    workSheet.Cells("A6").Value = string.Empty;
                    workSheet.Cells("A7").Value = string.Empty;
                    workSheet.Cells("I2").Value = string.Empty;
                }
                else
                {
                    workSheet.Cells("A1").Value = objEmisor.RazonSocial;
                    workSheet.Cells("A2").Value = objEmisor.RUTEmpresa;
                    workSheet.Cells("A3").Value = objEmisor.Giro;
                    workSheet.Cells("A4").Value = objEmisor.Direccion;
                    workSheet.Cells("A5").Value = ParseExtensions.FirstLetterToUpper(objEmisor.Ciudad);
                    workSheet.Cells("A6").Value = objEmisor.Representante;//"RepresentanteLegal";
                    workSheet.Cells("A7").Value = objEmisor.RUTRepresentante;//"RutRepresentanteLegal";
                }*/

                string baseFileNameTitle = "FlujoCaja";
                if (string.IsNullOrWhiteSpace(titleOverrideTo) == false)
                {
                    workSheet.Cells("D1").Value = titleOverrideTo;
                    baseFileNameTitle = titleOverrideTo;
                }


                workSheet.Cells("D2").Value = YearToLook;

                var rangoTitulos = workSheet.Cell(3, 2).InsertData(ExcelTitleData);
                rangoTitulos.Cells().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rangoTitulos.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                int lastRowLocation = 0;
                if (importoData.Count > 0)
                {
                    var rangeWithArrays = workSheet.Cell(4, 1).InsertData(importoData);
                    rangeWithArrays.Cells().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    rangeWithArrays.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    lastRowLocation = rangeWithArrays.LastRowUsed().RowNumber() + 1;
                }
                else
                {
                    lastRowLocation = 5;
                }

                workSheet.Columns().AdjustToContents();

                using (var ms = new MemoryStream())
                {
                    excelFile.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.ms-excel", baseFileNameTitle + objEmisor.RUTEmpresa + "_" + YearToLook + ".xlsx");
                }
            }
        }

      

    }
}