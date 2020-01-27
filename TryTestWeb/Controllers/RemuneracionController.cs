using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Migrations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MySql.Data.MySqlClient;
using PagedList;
using System.Globalization;
using ClosedXML.Excel;

namespace TryTestWeb.Controllers
{
    public class RemuneracionController : Controller
    {
        // GET: Remuneracion
        [Authorize]
        [ModuloHandler]

        public ActionResult ListadoEmpleado(string sortOrder, string currentFilter, string searchString, int? page, string Exportar = null)
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

            var lstEmpleados = db.DBempleados.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);

            if (!String.IsNullOrEmpty(searchString))
            {
                lstEmpleados = lstEmpleados.Where(s => s.nombre.Contains(searchString));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            List<EmpleadoModel> lstEmpleados2 = lstEmpleados.OrderByDescending(x => x.EmpleadoModelID).AsQueryable().ToList();

            return View(lstEmpleados2.ToPagedList(pageNumber, pageSize));
        }

        [Authorize]
        [ModuloHandler]
        public ActionResult AgregarEmpleado()
        {
            string UserID = User.Identity.GetUserId();

            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            EmpleadoModel empleado = new EmpleadoModel();

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            List<TipoContratoRemuModels> Tcontrato = db.DBtipoContratoRemu.ToList();
            List<GeneroRemuModels> genero = db.DBgeneroRemu.ToList();
            List<EstadoCivilRemuModels> ecivil = db.DBestadoCivilRemu.ToList();
            List<AfpRemuModels> afp = db.DBafpRemu.ToList();
            List<IsapreRemuModels> isapre = db.DBisapreRemu.ToList();
            List<TramoFaRemumodels> tramoFa = db.DBtramoRemu.ToList();
            List<TsueldoBaRemuModels> tsueldo = db.DBtsueldoBaseRemu.ToList();
            List<BancoRemumodels> banco = db.DBbancoRemu.ToList();
            List<GratificacionRemuModels> gratificacion = db.DBgratificacionRemu.ToList();
            List<InsApvRemuModels> apv = db.DBinsApvRemu.ToList();
            List<TcuentaRemuModels> tipocuenta = db.DBtipoCuentaRemu.ToList();

            IQueryable<CargoRemuModels> lstCargos = db.DBcargoRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<CargoRemuModels> lstCargos2 = lstCargos.ToList();


            //Sucursales propias del emisor.
            IQueryable<SucursalRemuModels> lstSucursal = db.DBsucursalRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<SucursalRemuModels> lstSucursal2 = lstSucursal.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            ViewBag.Cargos = lstCargos2;
            ViewBag.TipoContrato = Tcontrato;
            ViewBag.Genero = genero;
            ViewBag.Ecivil = ecivil;
            ViewBag.Afp = afp;
            ViewBag.Isapre = isapre;
            ViewBag.Tramo = tramoFa;
            ViewBag.Sucursal = lstSucursal2;
            ViewBag.TipoSueldo = tsueldo;
            ViewBag.Banco = banco;
            ViewBag.Gratificacion = gratificacion;
            ViewBag.Apv = apv;
            ViewBag.TipoCuenta = tipocuenta;

            return View(empleado);
        }


        [Authorize]
        [HttpPost]
        public ActionResult InsertarEmpleado(EmpleadoModel model, string FechaDeIngreso, string FechaDeNacimiento, string PeriodoCesantia)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            List<TipoContratoRemuModels> Tcontrato = db.DBtipoContratoRemu.ToList();
            List<GeneroRemuModels> genero = db.DBgeneroRemu.ToList();
            List<EstadoCivilRemuModels> ecivil = db.DBestadoCivilRemu.ToList();
            List<AfpRemuModels> afp = db.DBafpRemu.ToList();
            List<IsapreRemuModels> isapre = db.DBisapreRemu.ToList();
            List<TramoFaRemumodels> tramoFa = db.DBtramoRemu.ToList();
            List<TsueldoBaRemuModels> tsueldo = db.DBtsueldoBaseRemu.ToList();
            List<BancoRemumodels> banco = db.DBbancoRemu.ToList();
            List<GratificacionRemuModels> gratificacion = db.DBgratificacionRemu.ToList();

            //Cargos propios del Emisor
            IQueryable<CargoRemuModels> lstCargos = db.DBcargoRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<CargoRemuModels> lstCargos2 = lstCargos.ToList();
            //Sucursales propias del Emisor.
            IQueryable<SucursalRemuModels> lstSucursal = db.DBsucursalRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<SucursalRemuModels> lstSucursal2 = lstSucursal.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            ViewBag.Cargos = lstCargos2;
            ViewBag.TipoContrato = Tcontrato;
            ViewBag.Genero = genero;
            ViewBag.Ecivil = ecivil;
            ViewBag.Afp = afp;
            ViewBag.Isapre = isapre;
            ViewBag.Tramo = tramoFa;
            ViewBag.Sucursal = lstSucursal2;
            ViewBag.TipoSueldo = tsueldo;
            ViewBag.Banco = banco;
            ViewBag.Gratificacion = gratificacion;

            EmpleadoModel insert = new EmpleadoModel();
            //insertando empleado

            // Evitando la redundancia en el rut
            IQueryable<EmpleadoModel> lstEmpleados = db.DBempleados.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.rut.Contains(model.rut));
            List<EmpleadoModel> lstEmpleados2 = lstEmpleados.ToList();

            if (lstEmpleados2 != null && lstEmpleados2.Count > 0)
            {

                return RedirectToAction("AgregarEmpleado", "Remuneracion");
            }
            else
            {
                DateTime dtFechaDeNacimiento;
                DateTime dtFechaDeIngreso;
                DateTime dtPeriodoCesantia;

                DateTime.TryParseExact(FechaDeNacimiento, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaDeNacimiento);
                DateTime.TryParseExact(FechaDeIngreso, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaDeIngreso);
                DateTime.TryParseExact(PeriodoCesantia, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtPeriodoCesantia);


                insert.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
                //Datos Personales--------------------------------------   
                insert.nombre = model.nombre;
                insert.apellidos = model.apellidos;
                insert.rut = model.rut;
                insert.FechaDeNacimiento = dtFechaDeNacimiento;
                insert.telefono = model.telefono;
                insert.correo = model.correo;
                insert.direccion = model.direccion;
                insert.idRegion = model.idRegion;
                insert.idComuna = model.idComuna;
                insert.GeneroID = model.GeneroID;
                insert.EstadoCivilID = model.EstadoCivilID;
                insert.Nacionalidad = model.Nacionalidad;

                //Información de contrato--------------------------------

                insert.CargoID = model.CargoID;
                insert.HorasSem = model.HorasSem;
                insert.TipoContratoID = model.TipoContratoID;
                insert.EstadoContrato = model.EstadoContrato;
                insert.SucursalID = model.SucursalID;
                insert.SueldoLiquido = model.SueldoLiquido;
                insert.FechaDeIngreso = dtFechaDeIngreso;
                insert.AjustSueldoBa = model.AjustSueldoBa;
                insert.BenefSemCorrida = model.BenefSemCorrida;

                //Remuneración--------------------------------------------

                insert.TipoSueldoID = model.TipoSueldoID;
                insert.Sueldomes = model.Sueldomes;
                insert.GratificacionID = model.GratificacionID;
                insert.AsigZonaExtre = model.AsigZonaExtre;

                //Información Previsional----------------------------------   

                insert.AfpID = model.AfpID;
                insert.TramoID = model.TramoID;
                insert.Jubilado = model.Jubilado;
                insert.CargasNormales = model.CargasNormales;
                insert.CargasInvalidas = model.CargasInvalidas;
                insert.SegCesantia = model.SegCesantia;
                insert.AfecSeguroAccidente = model.AfecSeguroAccidente;
                insert.PeriodoCesantia = dtPeriodoCesantia;
                insert.IsapreID = model.IsapreID;
                insert.NumCuenta = model.NumCuenta;
                insert.BancoID = model.BancoID;


                db.DBempleados.Add(insert);
                db.SaveChanges();

                return RedirectToAction("AgregarEmpleado", "Remuneracion");

            }

        }

        [Authorize]
        [ModuloHandler]
        //[HttpPost]
        public ActionResult EditarEmpleado(int empleado_id, string FechaDeNacimiento, string FechaDeIngreso)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            List<TipoContratoRemuModels> Tcontrato = db.DBtipoContratoRemu.ToList();
            List<GeneroRemuModels> genero = db.DBgeneroRemu.ToList();
            List<EstadoCivilRemuModels> ecivil = db.DBestadoCivilRemu.ToList();
            List<AfpRemuModels> afp = db.DBafpRemu.ToList();
            List<IsapreRemuModels> isapre = db.DBisapreRemu.ToList();
            List<TramoFaRemumodels> tramoFa = db.DBtramoRemu.ToList();
            List<TsueldoBaRemuModels> tsueldo = db.DBtsueldoBaseRemu.ToList();
            List<BancoRemumodels> banco = db.DBbancoRemu.ToList();
            List<GratificacionRemuModels> gratificacion = db.DBgratificacionRemu.ToList();

            IQueryable<CargoRemuModels> lstCargos = db.DBcargoRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<CargoRemuModels> lstCargos2 = lstCargos.ToList();

            //Sucursales propias del emisor.
            IQueryable<SucursalRemuModels> lstSucursal = db.DBsucursalRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<SucursalRemuModels> lstSucursal2 = lstSucursal.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            ViewBag.Cargos = lstCargos2;
            ViewBag.TipoContrato = Tcontrato;
            ViewBag.Genero = genero;
            ViewBag.Ecivil = ecivil;
            ViewBag.Afp = afp;
            ViewBag.Isapre = isapre;
            ViewBag.Tramo = tramoFa;
            ViewBag.Sucursal = lstSucursal2;
            ViewBag.TipoSueldo = tsueldo;
            ViewBag.Banco = banco;
            ViewBag.Gratificacion = gratificacion;
            EmpleadoModel datosEmpleado = new EmpleadoModel();

            DateTime dtFechaDeNacimiento;
            DateTime dtFechaDeIngreso;

            DateTime.TryParseExact(FechaDeNacimiento, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaDeNacimiento);
            DateTime.TryParseExact(FechaDeIngreso, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaDeIngreso);

            datosEmpleado = db.DBempleados.AsQueryable().Where(s => s.EmpleadoModelID.Equals(empleado_id)).FirstOrDefault();

            return View(datosEmpleado);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateEmpleado(EmpleadoModel model, string FechaDeNacimiento, string FechaDeIngreso, string PeriodoCesantia)
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            model.idComuna = 1;
            model.idRegion = 1;
            model.CargoID = 1;
            model.TipoContratoID = 1;
            model.GeneroID = 1;
            model.EstadoCivilID = 1;
            model.AfpID = 1;
            model.IsapreID = 1;
            model.TramoID = 1;
            model.TipoSueldoID = 1;
            model.SucursalID = 1;
            model.BancoID = 1;
            model.GratificacionID = 1;

            List<RegionModels> regiones = db.DBRegiones.ToList();
            List<ComunaModels> comunas = db.DBComunas.ToList();
            List<TipoContratoRemuModels> Tcontrato = db.DBtipoContratoRemu.ToList();
            List<GeneroRemuModels> genero = db.DBgeneroRemu.ToList();
            List<EstadoCivilRemuModels> ecivil = db.DBestadoCivilRemu.ToList();
            List<AfpRemuModels> afp = db.DBafpRemu.ToList();
            List<IsapreRemuModels> isapre = db.DBisapreRemu.ToList();
            List<TramoFaRemumodels> tramoFa = db.DBtramoRemu.ToList();
            List<TsueldoBaRemuModels> tsueldo = db.DBtsueldoBaseRemu.ToList();
            List<BancoRemumodels> banco = db.DBbancoRemu.ToList();
            List<GratificacionRemuModels> gratificacion = db.DBgratificacionRemu.ToList();

            IQueryable<CargoRemuModels> lstCargos = db.DBcargoRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<CargoRemuModels> lstCargos2 = lstCargos.ToList();

            //Sucursales propias del emisor.
            IQueryable<SucursalRemuModels> lstSucursal = db.DBsucursalRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<SucursalRemuModels> lstSucursal2 = lstSucursal.ToList();

            ViewBag.Regiones = regiones;
            ViewBag.Comunas = comunas;
            ViewBag.Cargos = lstCargos2;
            ViewBag.TipoContrato = Tcontrato;
            ViewBag.Genero = genero;
            ViewBag.Ecivil = ecivil;
            ViewBag.Afp = afp;
            ViewBag.Isapre = isapre;
            ViewBag.Tramo = tramoFa;
            ViewBag.Sucursal = lstSucursal2;
            ViewBag.TipoSueldo = tsueldo;
            ViewBag.Banco = banco;
            ViewBag.Gratificacion = gratificacion;


            EmpleadoModel update = db.DBempleados.SingleOrDefault(r => r.EmpleadoModelID.Equals(model.EmpleadoModelID));

            DateTime dtFechaDeNacimiento;
            DateTime dtFechaDeIngreso;
            DateTime dtPeriodoCesantia;

            DateTime.TryParseExact(FechaDeNacimiento, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaDeNacimiento);
            DateTime.TryParseExact(FechaDeIngreso, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaDeIngreso);
            DateTime.TryParseExact(PeriodoCesantia, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtPeriodoCesantia);

            //Actualización de los datos

            //Datos Personales--------------------------------------   
            update.nombre = model.nombre;
            update.apellidos = model.apellidos;
            update.rut = model.rut;
            update.FechaDeNacimiento = dtFechaDeNacimiento;
            update.telefono = model.telefono;
            update.correo = model.correo;
            update.direccion = model.direccion;
            update.idRegion = model.idRegion;
            update.idComuna = model.idComuna;
            update.GeneroID = model.GeneroID;
            update.EstadoCivilID = model.EstadoCivilID;
            update.Nacionalidad = model.Nacionalidad;

            //Información de contrato--------------------------------

            update.CargoID = model.CargoID;
            update.HorasSem = model.HorasSem;
            update.TipoContratoID = model.TipoContratoID;
            update.EstadoContrato = model.EstadoContrato;
            update.SucursalID = model.SucursalID;
            update.SueldoLiquido = model.SueldoLiquido;
            update.FechaDeIngreso = dtFechaDeIngreso;
            update.AjustSueldoBa = model.AjustSueldoBa;
            update.BenefSemCorrida = model.BenefSemCorrida;

            //Remuneración--------------------------------------------

            update.TipoSueldoID = model.TipoSueldoID;
            update.Sueldomes = model.Sueldomes;
            update.GratificacionID = model.GratificacionID;
            update.AsigZonaExtre = model.AsigZonaExtre;

            //Información Previsional----------------------------------   

            update.AfpID = model.AfpID;
            update.TramoID = model.TramoID;
            update.Jubilado = model.Jubilado;
            update.CargasNormales = model.CargasNormales;
            update.CargasInvalidas = model.CargasInvalidas;
            update.SegCesantia = model.SegCesantia;
            update.AfecSeguroAccidente = model.AfecSeguroAccidente;
            update.PeriodoCesantia = dtPeriodoCesantia;
            update.IsapreID = model.IsapreID;
            update.NumCuenta = model.NumCuenta;
            update.BancoID = model.BancoID;


            TryUpdateModel<EmpleadoModel>(update);
            db.SaveChanges();

            return RedirectToAction("ListadoEmpleado", "Remuneracion");

        }


        [Authorize]
        [ModuloHandler]
        public ActionResult ListadoSucursal()
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            var lstSucursal = db.DBsucursalRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<SucursalRemuModels> lstSucursal2 = lstSucursal.OrderByDescending(x => x.SucursalRemuModelsID).ToList();

            return View(lstSucursal2);//lstSucursal2);
        }

        [Authorize]
        [HttpPost]
        public ActionResult InsertarSucursal(string NombreSucursal)
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            IQueryable<SucursalRemuModels> lstSucursal = db.DBsucursalRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.NombreSucursal.Contains(NombreSucursal));
            List<SucursalRemuModels> lstSucursal2 = lstSucursal.ToList();

            if (lstSucursal2 != null && lstSucursal2.Count > 0)
            {
                return RedirectToAction("ListadoSucursal", "Remuneracion");
            }
            else
            {

                SucursalRemuModels Sucur = new SucursalRemuModels();

                Sucur.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
                Sucur.NombreSucursal = NombreSucursal;

                db.DBsucursalRemu.Add(Sucur);
                db.SaveChanges();

                return RedirectToAction("ListadoSucursal", "Remuneracion");
            }

        }

        [Authorize]
        public ActionResult CargosEmpleado()
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);


            var lstCargos = db.DBcargoRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);
            List<CargoRemuModels> lstCargos2 = lstCargos.OrderByDescending(x => x.CargoRemuModelsID).ToList();

            return View(lstCargos2);
        }

        [Authorize]
        [HttpPost]
        public ActionResult InsertarCargo(string NombreCargo)
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            IQueryable<CargoRemuModels> lstCargos = db.DBcargoRemu.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID && r.NombreCargo.Contains(NombreCargo));
            List<CargoRemuModels> lstCargos2 = lstCargos.ToList();

            if (lstCargos2 != null && lstCargos2.Count > 0)
            {

                return RedirectToAction("CargosEmpleado", "Remuneracion");

            } else
            {
                CargoRemuModels cargo = new CargoRemuModels();

                cargo.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
                cargo.NombreCargo = NombreCargo;

                db.DBcargoRemu.Add(cargo);
                db.SaveChanges();

                return RedirectToAction("CargosEmpleado", "Remuneracion");

            }


        }

        public ActionResult LiquidacionTest()
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            var lstEmpleados = db.DBempleados.Where(r => r.QuickEmisorModelID == objEmisor.QuickEmisorModelID);

            List<EmpleadoModel> lstEmpleados2 = lstEmpleados.OrderByDescending(x => x.EmpleadoModelID).ToList();


            return View(lstEmpleados2);
        }

        [Authorize]
        public ActionResult CambiarDatosVariables()
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            List<UfRemuModels> uf = db.DBufRemu.ToList();
            List<UtmRemuModels> utm = db.DButmRemu.ToList();
            List<SminimoRemuModels> sminimo = db.DBsueldoMinimoRemu.ToList();
            List<AfpRemuModels> afp = db.DBafpRemu.ToList();

            ViewBag.UF = uf;
            ViewBag.UTM = utm;
            ViewBag.SueldoMinimo = sminimo;
            ViewBag.Afp = afp;


            DatosVariablesRemuModels vista = new DatosVariablesRemuModels();

            return View(vista);
        }

        [HttpPost]
        public ActionResult UpdateDatosVariables(UfRemuModels Uf,UtmRemuModels Utm,SminimoRemuModels Sminimo,AfpRemuModels Afp)
        {
            string UserID = User.Identity.GetUserId();
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);

            List<UfRemuModels> uf = db.DBufRemu.ToList();
            List<UtmRemuModels> utm = db.DButmRemu.ToList();
            List<SminimoRemuModels> sminimo = db.DBsueldoMinimoRemu.ToList();
            List<AfpRemuModels> afp = db.DBafpRemu.ToList();

            //Update

            UfRemuModels UF = new UfRemuModels();
            UF.ValorUf = Uf.ValorUf;

            TryUpdateModel<UfRemuModels>(UF);
            db.SaveChanges();

            UtmRemuModels UTM = new UtmRemuModels();
            UTM.ValorUtm = Utm.ValorUtm;

            TryUpdateModel<UtmRemuModels>(UTM);
            db.SaveChanges();

            SminimoRemuModels SueldoMinimo = new SminimoRemuModels();
            SueldoMinimo.SdepAndIndep = Sminimo.SdepAndIndep;
            SueldoMinimo.Men18May65 = Sminimo.Men18May65;
            SueldoMinimo.NoRemuValor = Sminimo.NoRemuValor;
            SueldoMinimo.TcasaParticular = Sminimo.TcasaParticular;

            TryUpdateModel<SminimoRemuModels>(SueldoMinimo);
            db.SaveChanges();

            AfpRemuModels AFP = new AfpRemuModels();
            AFP.TasaAfpDependiente = Afp.TasaAfpDependiente;
            AFP.TasaAfpIndependiente = Afp.TasaAfpIndependiente;

            TryUpdateModel<AfpRemuModels>(AFP);
            db.SaveChanges();


            return View();
        }

      
    }
}