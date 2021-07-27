using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TryTestWeb.Models;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Migrations;
using System.Text;
using TryTestWeb.Models.Monitoreo;

namespace TryTestWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated == true)
            {
                return RedirectToAction("SeleccionarEmisorDesdeModulos", "Home");
            }
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, shouldLockout: false);


            switch (result)
            {
                case SignInStatus.Success:
                    //Session["UseProdDatabase"] = false;
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                    return View(model);
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous
        [Authorize]
        [ModuloHandler]
        public ActionResult Register()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            List<PerfilUsuarioModel> lstPerfiles = db.DBPerfilUsuario.ToList();

            ViewBag.TiposUsuarios = lstPerfiles;
            return View();
        }

        
        [HttpPost]
        [Authorize]
        [ModuloHandler]
        public async Task<ActionResult> Register(RegisterViewModel model, int TipoUsuario)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated
                    if((System.Web.HttpContext.Current.User == null) || System.Web.HttpContext.Current.User.Identity.IsAuthenticated == false)
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //TABLAS AMBIENTES DE CERTIFICACION
                    string TextID = user.Id;
                    QuickEmisorModel newEmisor = new QuickEmisorModel(TextID);
                    newEmisor.RUTEmpresa = "88888888-8";
                    newEmisor.RUTUsuario = "88888888-8";
                    newEmisor.RazonSocial = "Ingresar Razon Social";

                    newEmisor.Giro = "Ingresar Giro";
                    //newEmisor.Giros.Add(new GirosModel("Ingresar Giro"));

                    //newEmisor.ActEcono = "000000";


                    newEmisor.FechaResolucion = DateTime.Now;
                    newEmisor.NumeroResolucion = 1;
                    newEmisor.CodigoSucursalSII = 1234;
                    //newEmisor.DatabaseContextToUse = 0;
                    newEmisor.RUTRepresentante = "88888888-8";
                    newEmisor.maxUsuariosParaEstaEmpresa = 1;
                    //Preparar view model para ingresar informacion basica de usuario
                    UsuarioModel newUsuario = new UsuarioModel();
                    newUsuario.IdentityID = user.Id;
                    newUsuario.Email = model.Email;
                    newUsuario.RUT = "88888888-8";
                    newUsuario.Nombre = model.Email;
                    newUsuario.SuperAdminUser = true;
                    newUsuario.DatabaseContextToUse = 0;

                    //TABLAS AMBIENTES DE PRODUCCION
                    QuickEmisorModel newEmisorProduccion = new QuickEmisorModel(TextID);
                    newEmisorProduccion.RUTEmpresa = "88888888-8";
                    newEmisorProduccion.RUTUsuario = "88888888-8";
                    newEmisorProduccion.RazonSocial = "Ingresar Razon Social";

                    newEmisorProduccion.Giro = "Ingresar Giro";
                    //newEmisorProduccion.Giros.Add(new GirosModel("Ingresar Giro"));

                    //newEmisorProduccion.ActEcono = "000000";


                    newEmisorProduccion.FechaResolucion = DateTime.Now;
                    newEmisorProduccion.NumeroResolucion = 1;
                    newEmisorProduccion.CodigoSucursalSII = 1234;
                    //newEmisorProduccion.DatabaseContextToUse = 0;
                    newEmisorProduccion.RUTRepresentante = "88888888-8";
                    newEmisorProduccion.maxUsuariosParaEstaEmpresa = 1;
                    //Preparar view model para ingresar informacion basica de usuario
                    UsuarioModel newUsuarioProduccion = new UsuarioModel();
                    newUsuarioProduccion.IdentityID = user.Id;
                    newUsuarioProduccion.Email = model.Email;
                    newUsuarioProduccion.RUT = "88888888-8";
                    newUsuarioProduccion.Nombre = model.Email;
                    newUsuarioProduccion.SuperAdminUser = true;
                    newUsuarioProduccion.DatabaseContextToUse = 1;


                    FacturaContext db = new FacturaContext();
                    FacturaProduccionContext dbProduccion = new FacturaProduccionContext();


                    PerfilUsuarioModel TipoUsuarioAGuardar = db.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == TipoUsuario);
                    PerfilUsuarioModel TipoUsuarioProd = dbProduccion.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == TipoUsuario);
                   
                    if(TipoUsuarioAGuardar != null)
                        newUsuario.PerfilUsuarioModelID = TipoUsuarioAGuardar.PerfilUsuarioModelID;

                    if(TipoUsuarioProd != null)
                        newUsuarioProduccion.PerfilUsuarioModelID = TipoUsuarioProd.PerfilUsuarioModelID;

                    db.Emisores.Add(newEmisor);
                    db.DBUsuarios.Add(newUsuario);

                    dbProduccion.Emisores.Add(newEmisorProduccion);
                    dbProduccion.DBUsuarios.Add(newUsuarioProduccion);


                    //Insertamos las funciones en certificación
                    //List<FuncionesModel> lstFunciones = new List<FuncionesModel>();
                    //lstFunciones = db.DBFunciones.Where(x => x.NombreModulo.Contains("Contabilidad") ||
                    //                                         x.NombreModulo == "Configuracion" ||
                    //                                         x.NombreModulo == "Cliente").ToList();

                    //foreach (FuncionesModel FuncAsignar in lstFunciones)
                    //{
                    //    ModulosHabilitados Mod = new ModulosHabilitados();
                    //    Mod.UsuarioModelID = newUsuario.UsuarioModelID;
                    //    Mod.QuickEmisorModelID = newEmisor.QuickEmisorModelID;
                    //    Mod.Funcion = FuncAsignar;
                    //    db.DBModulosHabilitados.Add(Mod);
                    //    db.SaveChanges();
                    //}

                    //Insertamos las funciones en producción


                    try
                    {
                        await db.SaveChangesAsync();
                        await dbProduccion.SaveChangesAsync();

                        EmisoresHabilitados EmisorHabilitadoNewUser = new EmisoresHabilitados();
                        EmisorHabilitadoNewUser.UsuarioModelID = newUsuario.UsuarioModelID;
                        EmisorHabilitadoNewUser.QuickEmisorModelID = newEmisor.QuickEmisorModelID;
                        EmisorHabilitadoNewUser.privilegiosAcceso = Privilegios.Administrador;
                        db.DBEmisoresHabilitados.Add(EmisorHabilitadoNewUser);

                        EmisoresHabilitados EmisorHabilitadoNewUserProduccion = new EmisoresHabilitados();
                        EmisorHabilitadoNewUserProduccion.UsuarioModelID = newUsuarioProduccion.UsuarioModelID;
                        EmisorHabilitadoNewUserProduccion.QuickEmisorModelID = newEmisorProduccion.QuickEmisorModelID;
                        EmisorHabilitadoNewUserProduccion.privilegiosAcceso = Privilegios.Administrador;
                        dbProduccion.DBEmisoresHabilitados.Add(EmisorHabilitadoNewUserProduccion);

                        await db.SaveChangesAsync();
                        await dbProduccion.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Property: {0} Error: {1}",
                                                        validationError.PropertyName,
                                                        validationError.ErrorMessage);
                            }
                        }
                    }

                    List<FuncionesModel> lstFuncionesProduccion = new List<FuncionesModel>();

                   if(TipoUsuarioProd.NombrePerfil == "SuperAdmin")
                    {
                        lstFuncionesProduccion = PerfilUsuarioModel.Superadmin(dbProduccion);
                    }
                   if(TipoUsuarioProd.NombrePerfil == "ContabilidadSuperAdmin")
                    {
                        lstFuncionesProduccion = PerfilUsuarioModel.ContadorSuperAdmin(dbProduccion);
                    }
                   if(TipoUsuarioProd.NombrePerfil == "Admin")
                    {
                        lstFuncionesProduccion = PerfilUsuarioModel.AdminContador(dbProduccion);
                    }
                   if(TipoUsuarioProd.NombrePerfil == "Contador")
                    {
                        lstFuncionesProduccion = PerfilUsuarioModel.Contador(dbProduccion);
                    }


                    if (newUsuarioProduccion.UsuarioModelID != 0)
                    {
                        if (lstFuncionesProduccion.Count() > 0)
                        {
                            foreach (FuncionesModel FuncAsignar in lstFuncionesProduccion)
                            {
                                ModulosHabilitados Mod = new ModulosHabilitados();
                                Mod.UsuarioModelID = newUsuarioProduccion.UsuarioModelID;
                                Mod.QuickEmisorModelID = newEmisorProduccion.QuickEmisorModelID;
                                Mod.Funcion = FuncAsignar;
                                dbProduccion.DBModulosHabilitados.Add(Mod);
                                dbProduccion.SaveChanges();
                            }
                        }
                    }


                    return RedirectToAction("Index", "Home");
                }
                if (result.Errors.Count() > 0)
                {
                    //result.Errors.ElementAt(0) 
                    string value = ParseExtensions.LocalizarErrores(result.Errors.ElementAt(0));
                    IList<string> errs = new List<string>();
                    errs.Add(value);
                    IdentityResult ox = new IdentityResult(errs);
                    AddErrors(ox);
                }
                else
                    AddErrors(result);

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize]
        [ModuloHandler]
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Administrador)]
        public ActionResult AgregarUsuario()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            UsuarioModel objUser = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);

            PerfilUsuarioModel TipoUsuario = db.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == objUser.PerfilUsuarioModelID);

            if (TipoUsuario.NombrePerfil != "SuperAdmin" && TipoUsuario.NombrePerfil != "Admin" && TipoUsuario.NombrePerfil != "ContabilidadSuperAdmin")
            {
                TempData["Error"] = "No tienes permisos para acceder.";
                return RedirectToAction("PanelClienteContable", "Contabilidad");
            }

            ViewBag.UsuariosPoseidos = PerfilamientoModule.ObtenerUsuariosPoseidos(User.Identity.GetUserId());
            ViewBag.NombreEmpresaActual = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID).RazonSocial;
             
            if(TipoUsuario != null) { 
                List<PerfilUsuarioModel> lstPerfil = new List<PerfilUsuarioModel>();

                if (TipoUsuario.NombrePerfil == "SuperAdmin" || TipoUsuario.NombrePerfil == "ContabilidadSuperAdmin")
                {
                    lstPerfil = db.DBPerfilUsuario.ToList();
                }else if(TipoUsuario.NombrePerfil == "Admin")
                {
                    lstPerfil = db.DBPerfilUsuario.Where(x => x.NombrePerfil != "SuperAdmin" &&
                                                              x.NombrePerfil != "Admin" &&
                                                              x.NombrePerfil != "ContabilidadSuperAdmin").ToList();
                }
                    
                ViewBag.Perfilamiento = lstPerfil;
                
            }

            //Preguntar si dejar esto o seguir con esto
            //if (objUser.IsUserSuperAdmin() == false)
            //    return PrivilegiosHandler.RetornarSeleccionEmisor();
            //else
            return View();
        }

        [HttpPost]
        [Authorize]
        [ModuloHandler]
        //[PrivilegiosHandler(PrivilegioMinimoRequerido = Privilegios.Administrador)]
        public async Task<ActionResult> AgregarUsuario(RegisterViewModelAddUser model, int TipoUsuario)
        {

            //Queda pendiente guardar el usuario dependiendo de si está en certificación o producción.
            //Por ahora los guarda por defecto en certificación y se necesita crear el usuario en producción.
            if (ModelState.IsValid == true)
            {
                string UserID = User.Identity.GetUserId();

                //FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
                FacturaContext dbCertificacion = new FacturaContext();
                FacturaProduccionContext dbProduccion = new FacturaProduccionContext();

                //CERT USER
                UsuarioModel objUserCert = dbCertificacion.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);
             
                //PROD USER
                UsuarioModel objUserProd = dbProduccion.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);
               
                //if (objUserProd.IsUserSuperAdmin() == false)
                //    return PrivilegiosHandler.RetornarSeleccionEmisor();

                QuickEmisorModel objEmisorAny = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
                QuickEmisorModel objEmisorCert = dbCertificacion.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorAny.IdentityIDEmisor);
                QuickEmisorModel objEmisorProd = dbProduccion.Emisores.SingleOrDefault(r => r.IdentityIDEmisor == objEmisorAny.IdentityIDEmisor);


                //Entender en que contexto se encuentra para encontrar ka verdad
                bool ExcedeMaximoDeUsuarios = false;
                bool Esta_En_Certificacion = ParseExtensions.ItsUserOnCertificationEnvironment(UserID);

                int maxUsuarios = 0;
                int currentUsuarios = 0;

                if (Esta_En_Certificacion)
                {
                    ExcedeMaximoDeUsuarios = objEmisorCert.ExcedeCantidadUsuarios(dbCertificacion, out maxUsuarios, out currentUsuarios);
                }
                else
                {
                    ExcedeMaximoDeUsuarios = objEmisorProd.ExcedeCantidadUsuarios(dbProduccion, out maxUsuarios, out currentUsuarios);
                }

                if (ExcedeMaximoDeUsuarios)
                {
                    ModelState.AddModelError("", "Usted excede su maximo de usuarios disponibles. MAX: " + maxUsuarios + " / Actuales:" + currentUsuarios);
                    //ViewBag.EmptyWarning = "Usted excede su maximo de usuarios disponibles. MAX: "+maxUsuarios+" / Actuales:"+currentUsuarios;
                    return View(model);
                }

                //handle this case
                if (objEmisorCert == null || objEmisorProd == null)
                    throw new Exception();

                var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //CERTIFICACION
                    UsuarioModel objNewUserCert = new UsuarioModel();
                    objNewUserCert.IdentityID = user.Id;//User.Identity.GetUserId();
                    objNewUserCert.Email = model.Email;
                    //string rut =//Request.Form.GetValues("rut")[0];
                    objNewUserCert.RUT = model.RutUsuario;
                    //string nombre = model.NombreUsuario;//Request.Form.GetValues("nombre")[0];
                    objNewUserCert.Nombre = model.NombreUsuario;
                    objNewUserCert.PerfilUsuarioModelID = TipoUsuario; // Es lo que hace la relación con el tipo de usuario ya sea (SuperAdmin, Admin, User)
                    objNewUserCert.DatabaseContextToUse = 0;


                    dbCertificacion.DBUsuarios.Add(objNewUserCert);
                    dbCertificacion.SaveChanges();

                    PosesionUsuarios objPosesionCert = new PosesionUsuarios();
                    objPosesionCert.UsuarioModelID = objUserCert.UsuarioModelID;
                    objPosesionCert.UsuarioPoseidoID = objNewUserCert.UsuarioModelID;

                    objUserCert.lstUsuariosPoseidos.Add(objPosesionCert);

                    dbCertificacion.DBUsuarios.AddOrUpdate(objUserCert);

                    dbCertificacion.SaveChanges();

                    EmisoresHabilitados objEmisorHabilitadoCert = new EmisoresHabilitados();
                    objEmisorHabilitadoCert.QuickEmisorModelID = objEmisorCert.QuickEmisorModelID;
                    objEmisorHabilitadoCert.UsuarioModelID = objNewUserCert.UsuarioModelID;

                    string Priv = Request.Form.GetValues("priv")[0];
                    int Privi = Int32.Parse(Priv);

                    objEmisorHabilitadoCert.privilegiosAcceso = (Privilegios)Privi;

                    dbCertificacion.DBEmisoresHabilitados.AddOrUpdate(p => new { p.UsuarioModelID, p.EmisoresHabilitadosID }, objEmisorHabilitadoCert);

                    dbCertificacion.SaveChanges();

                    //Asignamos Funciones Automaticamente

                    
                    //PRODUCCION
                    UsuarioModel objNewUserProd = new UsuarioModel();
                    objNewUserProd.IdentityID = user.Id;//User.Identity.GetUserId();
                    objNewUserProd.Email = model.Email;
                    //string rut =//Request.Form.GetValues("rut")[0];
                    objNewUserProd.RUT = model.RutUsuario;
                    //string nombre = model.NombreUsuario;//Request.Form.GetValues("nombre")[0];
                    objNewUserProd.Nombre = model.NombreUsuario;
                    objNewUserProd.PerfilUsuarioModelID = TipoUsuario;
                    objNewUserProd.DatabaseContextToUse = 1; //Usa la DB de producción
                    objNewUserProd.HeredaDeUsuario = objUserProd.UsuarioModelID;

                    dbProduccion.DBUsuarios.Add(objNewUserProd);
                    dbProduccion.SaveChanges();

                    List<FuncionesModel> lstFunciones = new List<FuncionesModel>();

                    PerfilUsuarioModel PerfilUsuario = dbProduccion.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == TipoUsuario);

                    //Se le asignarán por defecto todas las funcionalidades del sistema dependiendo del tipo de usuario
                    if (PerfilUsuario.NombrePerfil == "SuperAdmin")
                    {
                        lstFunciones = PerfilUsuarioModel.Superadmin(dbProduccion); // Si es SuperAdmin Obtiene todas las funciones del sistema.
                    }
                    if(PerfilUsuario.NombrePerfil == "ContabilidadSuperAdmin")
                    {
                        lstFunciones = PerfilUsuarioModel.ContadorSuperAdmin(dbProduccion);
                    }
                    if (PerfilUsuario.NombrePerfil == "Admin")
                    {
                        lstFunciones = PerfilUsuarioModel.AdminContador(dbProduccion);
                    }
                    if(PerfilUsuario.NombrePerfil == "Contador") {
                        lstFunciones = PerfilUsuarioModel.Contador(dbProduccion);
                    }
                    
                    if(lstFunciones != null) { 
                        foreach (FuncionesModel FuncAsignar in lstFunciones)
                        {
                            ModulosHabilitados Mod = new ModulosHabilitados();

                            Mod.UsuarioModelID = objNewUserProd.UsuarioModelID;
                            Mod.QuickEmisorModelID = objEmisorProd.QuickEmisorModelID;
                            Mod.Funcion = FuncAsignar;
                            dbProduccion.DBModulosHabilitados.Add(Mod);
                            dbProduccion.SaveChanges();
                        }
                    }
                    ////Le pasamos al contador todos los clientes contables habilitados que contenga el creador.            
                    //List<UserClientesContablesModels> ClientesDelCreador = new List<UserClientesContablesModels>();
       
                    //ClientesDelCreador = dbProduccion.DBUserToClientesContables.Where(x => x.UsuarioModelID == objUserProd.UsuarioModelID).ToList();

                    //if(ClientesDelCreador != null) { 

                    //    foreach (UserClientesContablesModels ClienteHeredado in ClientesDelCreador)
                    //    {
                    //        ClienteHeredado.UsuarioModelID = objNewUserProd.UsuarioModelID;
                    //        if(PerfilUsuario.NombrePerfil == "Contador") { 
                    //            ClienteHeredado.HeredaDeUsuario = objUserProd.UsuarioModelID;
                    //        }else
                    //        {
                    //            ClienteHeredado.HeredaDeUsuario = 0;
                    //        }
                    //        ClienteHeredado.QuickEmisorModelID = objEmisorProd.QuickEmisorModelID;
                    //        ClienteHeredado.ClientesContablesHabilitadosID = ClienteHeredado.ClientesContablesHabilitadosID;
                    //        dbProduccion.DBUserToClientesContables.Add(ClienteHeredado);
                    //        dbProduccion.SaveChanges();
                    //    }
                    //}


                    PosesionUsuarios objPosesionProd = new PosesionUsuarios();
                    objPosesionProd.UsuarioModelID = objUserProd.UsuarioModelID;
                    objPosesionProd.UsuarioPoseidoID = objNewUserProd.UsuarioModelID;

                    objUserProd.lstUsuariosPoseidos.Add(objPosesionProd);

                    dbProduccion.DBUsuarios.AddOrUpdate(objUserProd);

                    dbProduccion.SaveChanges();

                    EmisoresHabilitados objEmisorHabilitadoProd = new EmisoresHabilitados();
                    objEmisorHabilitadoProd.QuickEmisorModelID = objEmisorProd.QuickEmisorModelID;
                    objEmisorHabilitadoProd.UsuarioModelID = objNewUserProd.UsuarioModelID;

                    objEmisorHabilitadoProd.privilegiosAcceso = (Privilegios)Privi;

                    dbProduccion.DBEmisoresHabilitados.AddOrUpdate(p => new { p.UsuarioModelID, p.EmisoresHabilitadosID }, objEmisorHabilitadoProd);

                    dbProduccion.SaveChanges();

                    TempData["Correcto"] = "Usuario creado con éxito.";
                    return RedirectToAction("AgregarUsuario", "Account");
                }
            }
            return View(model);
        }
        


        [Authorize]
        public ActionResult AsignarCantidadUsuariosYClientes()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            UsuarioModel UsuarioActivo = db.DBUsuarios.SingleOrDefault(x => x.IdentityID == UserID);

            PerfilUsuarioModel PerfilUsuarioActivo = db.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == UsuarioActivo.PerfilUsuarioModelID);
            
            if(PerfilUsuarioActivo.NombrePerfil != "SuperAdmin" && PerfilUsuarioActivo.NombrePerfil != "ContabilidadSuperAdmin")
            {
                TempData["Error"] = "No tienes permiso para acceder";
                return RedirectToAction("PanelClienteContable", "Contabilidad");
            }

            List<QuickEmisorModel> lstEmisores = db.Emisores.ToList();

            List<UsuarioModel> lstUsuariosRepresentantes = new List<UsuarioModel>();

            foreach (QuickEmisorModel EmpresaUnica in lstEmisores)
            {
                UsuarioModel BuscaUserRepresentante = db.DBUsuarios.SingleOrDefault(x => x.IdentityID == EmpresaUnica.IdentityIDEmisor);

                if(BuscaUserRepresentante != null)
                {
                    lstUsuariosRepresentantes.Add(BuscaUserRepresentante);
                }
            }

            List<string[]> lstUsuarioEmpresa = new List<string[]>();

            foreach (UsuarioModel Usuario in lstUsuariosRepresentantes)
            {
                QuickEmisorModel Empresa = db.Emisores.SingleOrDefault(x => x.IdentityIDEmisor == Usuario.IdentityID);

                if(Empresa != null) { 
                    string[] ArrayUsuarioEmpresa = new string[4];
                    ArrayUsuarioEmpresa[0] = Empresa.RazonSocial;
                    ArrayUsuarioEmpresa[1] = Usuario.Nombre;
                    ArrayUsuarioEmpresa[2] = Empresa.maxUsuariosParaEstaEmpresa.ToString();
                    ArrayUsuarioEmpresa[3] = Empresa.maxClientesContablesParaEstaEmpresa.ToString();

                    lstUsuarioEmpresa.Add(ArrayUsuarioEmpresa);
                }
            }
            
            if(lstUsuarioEmpresa != null)
            {
                ViewBag.tablaUsuarioEmpresa = lstUsuarioEmpresa;
            }

            return View(lstUsuariosRepresentantes);
        }

        
        [Authorize]
        public ActionResult AsignarCantidadUsuariosYClientesUpdate(int UsuarioID = 0, int MaxUsuarios = 0, int MaxClientesCont = 0)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            UsuarioModel UsuarioAEditar = db.DBUsuarios.SingleOrDefault(x => x.UsuarioModelID == UsuarioID);

            QuickEmisorModel EmisorAEditar = db.Emisores.SingleOrDefault(x => x.IdentityIDEmisor == UsuarioAEditar.IdentityID);

            if (EmisorAEditar != null)
            {
                EmisorAEditar.maxUsuariosParaEstaEmpresa = MaxUsuarios;
                EmisorAEditar.maxClientesContablesParaEstaEmpresa = MaxClientesCont;
                db.Emisores.AddOrUpdate(EmisorAEditar);
                db.SaveChanges();
            }
            else
            {
                TempData["Error"] = "Ha ocurrido un error inesperado.";
                return RedirectToAction("AsignarCantidadUsuariosYClientes", "Account");
            }

            TempData["Correcto"] = "Se ha actualizado el usuario correctamente.";
            return RedirectToAction("AsignarCantidadUsuariosYClientes", "Account");
        }

        [Authorize]
        public ActionResult MisUsuarios()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            UsuarioModel UsuarioActivo = db.DBUsuarios.SingleOrDefault(x => x.IdentityID == UserID);

            PerfilUsuarioModel TipoUsuario = db.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == UsuarioActivo.PerfilUsuarioModelID);

            List<UsuarioModel> TusUsuarios = new List<UsuarioModel>();

            if(TipoUsuario.NombrePerfil == "Admin" || TipoUsuario.NombrePerfil == "SuperAdmin" || TipoUsuario.NombrePerfil == "ContabilidadSuperAdmin")
            {
                TusUsuarios = db.DBUsuarios.Where(x => x.HeredaDeUsuario == UsuarioActivo.UsuarioModelID).ToList();

            }else
            {
                TempData["Error"] = "No tienes permisos para acceder.";
                return RedirectToAction("PanelClienteContable", "Contabilidad");
            }

            return View(TusUsuarios);
        }


        [Authorize]
        public JsonResult ObtenerConfiguracionUsuario(int UsuarioID)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            UsuarioModel UsuarioActivo = db.DBUsuarios.SingleOrDefault(x => x.IdentityID == UserID);

            if (objEmisor == null || UsuarioID == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            UsuarioModel UsuarioAObtener = db.DBUsuarios.SingleOrDefault(x => x.UsuarioModelID == UsuarioID);
            if (UsuarioAObtener == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                StringBuilder optionSelect = new StringBuilder();

                List<ClientesContablesModel> ListaCCEsteUsuario = (from Clientes in db.DBClientesContables
                                                                   join RelacionCCUser in db.DBUserToClientesContables on Clientes.ClientesContablesModelID equals RelacionCCUser.ClientesContablesHabilitadosID
                                                                   where RelacionCCUser.UsuarioModelID == UsuarioActivo.UsuarioModelID

                                                                   select Clientes).ToList();

                List<ClientesContablesModel> ListaCCUsuarioSeleccionado = (from Clientes in db.DBClientesContables
                                                                           join RelacionCCUser in db.DBUserToClientesContables on Clientes.ClientesContablesModelID equals RelacionCCUser.ClientesContablesHabilitadosID
                                                                           where RelacionCCUser.UsuarioModelID == UsuarioAObtener.UsuarioModelID

                                                                           select Clientes).ToList();

                optionSelect.Append("<option>Selecciona</option>");

                foreach (ClientesContablesModel Cliente in ListaCCEsteUsuario)
                {
                    if(ListaCCUsuarioSeleccionado.Any(x => x.ClientesContablesModelID == Cliente.ClientesContablesModelID)) { 
                        optionSelect.Append("<option selected value=\"" + Cliente.ClientesContablesModelID + "\">" + Cliente.RazonSocial + "</option>");
                    }
                    else
                    {
                        optionSelect.Append("<option  value=\"" + Cliente.ClientesContablesModelID + "\">" + Cliente.RazonSocial + "</option>");
                    }
                }

                return Json(new
                {
                    ok = true,
                    IDUsuario = UsuarioAObtener.UsuarioModelID,
                    UsuarioNombre = UsuarioAObtener.Nombre,
                    UsuarioRUT = UsuarioAObtener.RUT,
                    ListadoClientes = optionSelect.ToString()

                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult GuardarConfiguracionUsuario(int IDUsuario, string NombreUsuario, string RutUsuario, int[]ClientesContables)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            QuickEmisorModel objEmisor = PerfilamientoModule.GetEmisorSeleccionado(Session, UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);

            UsuarioModel UsuarioAConfigurar = db.DBUsuarios.SingleOrDefault(x => x.UsuarioModelID == IDUsuario);

            if(UsuarioAConfigurar != null)
            {
                List<UserClientesContablesModels> UpdateCC = db.DBUserToClientesContables.Where(x => x.UsuarioModelID == UsuarioAConfigurar.UsuarioModelID).ToList();
                db.DBUserToClientesContables.RemoveRange(UpdateCC);
                db.SaveChanges();

                UserClientesContablesModels AgregarClientes = new UserClientesContablesModels();

                foreach (int ClientesContablesID in ClientesContables)
                {
                    AgregarClientes.QuickEmisorModelID = objEmisor.QuickEmisorModelID;
                    AgregarClientes.UsuarioModelID = UsuarioAConfigurar.UsuarioModelID;
                    AgregarClientes.ClientesContablesHabilitadosID = ClientesContablesID;
                    db.DBUserToClientesContables.Add(AgregarClientes);
                    db.SaveChanges();
                }

                UsuarioAConfigurar.Nombre = NombreUsuario;
                UsuarioAConfigurar.RUT = RutUsuario;
                db.DBUsuarios.AddOrUpdate(UsuarioAConfigurar);
                db.SaveChanges();
            }
            else
            {
                TempData["Error"] = "Ha ocurrido un error inesperado";
            }


            TempData["Correcto"] = "Se ha configurado el usuario con éxito.";
            return RedirectToAction("MisUsuarios","Account");
        }


        [Authorize]
        public ActionResult AsignarPerfilUsuario()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            UsuarioModel objUser = db.DBUsuarios.SingleOrDefault(r => r.IdentityID == UserID);

            PerfilUsuarioModel TipoUsuario = db.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == objUser.PerfilUsuarioModelID);


            List<UsuarioModel> lstUsuarios = new List<UsuarioModel>();


            if(TipoUsuario != null && TipoUsuario.PerfilUsuarioModelID != 0 && TipoUsuario.NombrePerfil == "SuperAdmin" || TipoUsuario.NombrePerfil == "ContabilidadSuperAdmin")
            {
                lstUsuarios = db.DBUsuarios.ToList();
            }
            else
            {
                TempData["Error"] = "No tienes permisos para acceder";
                return RedirectToAction("PanelClienteContable", "Contabilidad");
            }


            return View(lstUsuarios);
        }

        [Authorize]
        public JsonResult EmpresasDeUsuarioSeleccionado(int UsuarioID)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            bool Result = false;

            FacturaProduccionContext dbProduccion = new FacturaProduccionContext();

            UsuarioModel UsuarioSeleccionado = dbProduccion.DBUsuarios.SingleOrDefault(x => x.UsuarioModelID == UsuarioID);

            StringBuilder DibujaLstempresa = new StringBuilder();


            List<int> IDEmpresasUsuario = new List<int>();
            List<QuickEmisorModel> EmpresasARetornar = new List<QuickEmisorModel>();

            if (UsuarioSeleccionado != null)
            {
                IDEmpresasUsuario = dbProduccion.DBModulosHabilitados.Where(x => x.UsuarioModelID == UsuarioSeleccionado.UsuarioModelID).Select(x => x.QuickEmisorModelID).ToList();
                
                IDEmpresasUsuario = IDEmpresasUsuario.Distinct().ToList();

                if(IDEmpresasUsuario.Count() > 0) {  
                    foreach (int BuscaEmpresa in IDEmpresasUsuario)
                    {
                        QuickEmisorModel EmpresaEncontrada = dbProduccion.Emisores.SingleOrDefault(x => x.QuickEmisorModelID == BuscaEmpresa);

                        if(EmpresaEncontrada != null)
                        {
                            EmpresasARetornar.Add(EmpresaEncontrada);
                        }

                    }
                }

                if (EmpresasARetornar.Count() > 0)
                {
                    DibujaLstempresa.Append("<option>Selecciona</option>");

                    foreach (QuickEmisorModel EmpresaADibujar in EmpresasARetornar)
                    {
                        DibujaLstempresa.Append("<option  value=\"" + EmpresaADibujar.QuickEmisorModelID + "\">" + EmpresaADibujar.RazonSocial + "</option>");
                    }

                    Result = true;
                }
                    
            }

            return Json(new { Result = Result, lstEmpresas = DibujaLstempresa.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult ObtenerPerfilUsuario(int UsuarioID)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            bool Result = false;

            FacturaProduccionContext dbProduccion = new FacturaProduccionContext();
            StringBuilder lstPerfiles = new StringBuilder();

            UsuarioModel UsuarioSeleccionado = dbProduccion.DBUsuarios.SingleOrDefault(x => x.UsuarioModelID == UsuarioID);

            if(UsuarioSeleccionado != null)
            {
                PerfilUsuarioModel TipoDeUsuario = dbProduccion.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == UsuarioSeleccionado.PerfilUsuarioModelID);
                List<PerfilUsuarioModel> TodosLosPerfiles = dbProduccion.DBPerfilUsuario.ToList();

                lstPerfiles.Append("<option>Selecciona</option>");
                foreach (PerfilUsuarioModel Perfil in TodosLosPerfiles)
                    {
                        
                    if(TipoDeUsuario != null) { 
                        if(TipoDeUsuario.PerfilUsuarioModelID == Perfil.PerfilUsuarioModelID)
                        {
                            lstPerfiles.Append("<option selected value=\"" + Perfil.PerfilUsuarioModelID + "\">" + Perfil.NombrePerfil + "</option>");
                        }
                        else
                        {
                            lstPerfiles.Append("<option  value=\"" + Perfil.PerfilUsuarioModelID + "\">" + Perfil.NombrePerfil + "</option>");
                        }
                    }
                    else
                    {
                        lstPerfiles.Append("<option  value=\"" + Perfil.PerfilUsuarioModelID + "\">" + Perfil.NombrePerfil + "</option>");
                    }
                    
                }
            }

            Result = true;
            return Json(new { Result = Result, lstPerfiles = lstPerfiles.ToString() },JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult LimpiarYReestablecerPerfil(int TipoUsuario,int EmpresaID, int UsuarioID)
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            ClientesContablesModel objCliente = PerfilamientoModule.GetClienteContableSeleccionado(Session, UserID, db);
            
            FacturaProduccionContext dbProduccion = new FacturaProduccionContext();

            //Verificamos que lo que viene por parametro exista en la db.
            UsuarioModel UsuarioAEditar = dbProduccion.DBUsuarios.SingleOrDefault(x => x.UsuarioModelID == UsuarioID);
            PerfilUsuarioModel PerfilUsuario = dbProduccion.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == TipoUsuario);
            QuickEmisorModel EmpresaSolicitada = dbProduccion.Emisores.SingleOrDefault(x => x.QuickEmisorModelID == EmpresaID);

            List<ModulosHabilitados> lstAEliminar = dbProduccion.DBModulosHabilitados.Where(x => x.UsuarioModelID == UsuarioAEditar.UsuarioModelID &&
                                                                                                 x.QuickEmisorModelID == EmpresaSolicitada.QuickEmisorModelID).ToList();

            //Eliminamos todos los habilitados para ingresar la nueva lista.
            if(lstAEliminar.Count() > 0) { 
                dbProduccion.DBModulosHabilitados.RemoveRange(lstAEliminar);
                dbProduccion.SaveChanges();
            }


            List<FuncionesModel> lstFunciones = new List<FuncionesModel>();

            //Se le asignarán por defecto todas las funcionalidades del sistema dependiendo del tipo de usuario
            if (PerfilUsuario.NombrePerfil == "SuperAdmin")
            {
                lstFunciones = PerfilUsuarioModel.Superadmin(dbProduccion); // Si es SuperAdmin Obtiene todas las funciones del sistema.
            }
            if (PerfilUsuario.NombrePerfil == "ContabilidadSuperAdmin")
            {
                lstFunciones = PerfilUsuarioModel.ContadorSuperAdmin(dbProduccion);
            }
            if (PerfilUsuario.NombrePerfil == "Admin")
            {
                lstFunciones = PerfilUsuarioModel.AdminContador(dbProduccion);
            }
            if (PerfilUsuario.NombrePerfil == "Contador")
            {
                lstFunciones = PerfilUsuarioModel.Contador(dbProduccion);
            }

            if (lstFunciones.Count() > 0)
            {
                foreach (FuncionesModel FuncAsignar in lstFunciones)
                {
                    ModulosHabilitados Mod = new ModulosHabilitados();

                    Mod.UsuarioModelID = UsuarioAEditar.UsuarioModelID;
                    Mod.QuickEmisorModelID = EmpresaSolicitada.QuickEmisorModelID;
                    Mod.Funcion = FuncAsignar;
                    dbProduccion.DBModulosHabilitados.Add(Mod);
                    dbProduccion.SaveChanges();
                }

                UsuarioAEditar.PerfilUsuarioModelID = PerfilUsuario.PerfilUsuarioModelID;
                dbProduccion.DBUsuarios.AddOrUpdate(UsuarioAEditar);
                dbProduccion.SaveChanges();
            }

            

            TempData["correcto"] = "Cambios realizados con éxito";
            return RedirectToAction("AsignarPerfilUsuario", "Account");
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous] 
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                //var user = await UserManager.FindByNameAsync(model.Email);

                string MensajeGenerico = "Por favor revise su casilla de E-Mail para resetear su contraseña.";
                TempData["ProblemasPagos"] = MensajeGenerico;

                //ESTA EL CORREO CONFIRMADO
                if (user == null /*|| !(await UserManager.IsEmailConfirmedAsync(user.Id))*/)
                {
                    // Don't reveal that the user does not exist or is not confirmed

                    
                    return RedirectToAction("ForgotPassword", "Account");
                    //return View("ForgotPassword", "Account");
                    //return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                await UserManager.SendEmailAsync(user.Id, "Reestablecer Contraseña", MailHelper.GetMensajeRecuperarContraseña(callbackUrl) /*"Por favor reestablezca su contraseña haciendo click en el siguiente <a href=\"" + callbackUrl + "\">link</a>"*/);

                return RedirectToAction("ForgotPassword", "Account");
                //return View("ForgotPassword", "Account");
                //return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

            
        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        
        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        /*
        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }*/

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        /*
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]     
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }*/

        //
        // POST: /Account/LogOff
        [HttpPost]       
        public ActionResult LogOff()
        {
            string UserID = User.Identity.GetUserId();
            FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
            bool resultado = MonitoreoSesion.ControlarEstadoSesion(db, UserID, false);
            Session.Clear();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        /*
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}