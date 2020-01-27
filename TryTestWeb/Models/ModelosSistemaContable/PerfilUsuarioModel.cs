using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class PerfilUsuarioModel
    {
        public int PerfilUsuarioModelID { get; set; }
        public string NombrePerfil { get; set; }

        public PerfilUsuarioModel()
        {

        }

        public static List<FuncionesModel> Superadmin(FacturaProduccionContext db)
        {
            List<FuncionesModel> ReturnValues = new List<FuncionesModel>();

            ReturnValues = db.DBFunciones.ToList();
            return ReturnValues;
        }

        public static List<FuncionesModel> ContadorSuperAdmin(FacturaProduccionContext db)
        {
            List<FuncionesModel> ReturnValues = new List<FuncionesModel>();

            ReturnValues = db.DBFunciones.Where(x => x.NombreModulo == "Contabilidad-SuperAdmin" ||
                                                     x.NombreModulo == "Contabilidad-Admin" ||
                                                     x.NombreModulo == "Contabilidad-Comun" ||
                                                     x.NombreFuncion == "Index").ToList();
            return ReturnValues;
        }

    //Este es el perfil del usuario que contrata.
        public static List<FuncionesModel> AdminContador(FacturaProduccionContext db)
        {
            List<FuncionesModel> ReturnValues = new List<FuncionesModel>();

            ReturnValues = db.DBFunciones.Where(x => x.NombreModulo == "Contabilidad-Comun" ||
                                                     x.NombreModulo == "Contabilidad-Admin" ||
                                                     x.NombreFuncion == "PanelClienteContable").ToList();

            return ReturnValues;
        }

        public static List<FuncionesModel> Contador(FacturaProduccionContext db)
        {
             List<FuncionesModel> ReturnValues = new List<FuncionesModel>();
             ReturnValues = db.DBFunciones.Where(x => x.NombreModulo == "Contabilidad-Comun" ||
                                                      x.NombreFuncion == "PanelClienteContable").ToList();

             return ReturnValues;
        }

        public static List<FuncionesModel> PerfilUsuario(FacturaProduccionContext db, int TipoUsuario)
        {
            PerfilUsuarioModel TipoUsuarioEncontrado = db.DBPerfilUsuario.SingleOrDefault(x => x.PerfilUsuarioModelID == TipoUsuario);
            List<FuncionesModel> ReturnValues = new List<FuncionesModel>();

            if(TipoUsuarioEncontrado != null)
            {
                  if(TipoUsuarioEncontrado.NombrePerfil == "SuperAdmin")
                  {
                    ReturnValues = db.DBFunciones.ToList();
                  }
                  if(TipoUsuarioEncontrado.NombrePerfil == "ContabilidadSuperAdmin")
                  {
                    ReturnValues = db.DBFunciones.Where(x => x.NombreModulo == "Contabilidad-SuperAdmin" ||
                                                       x.NombreModulo == "Contabilidad-Admin" ||
                                                       x.NombreModulo == "Contabilidad-Comun" ||
                                                       x.NombreFuncion == "Index").ToList();
                  }
                  if(TipoUsuarioEncontrado.NombrePerfil == "Admin")
                  {
                    ReturnValues = db.DBFunciones.Where(x => x.NombreModulo == "Contabilidad-Comun" ||
                                                         x.NombreModulo == "Contabilidad-Admin" ||
                                                         x.NombreFuncion == "PanelClienteContable").ToList();
                  }
                  if(TipoUsuarioEncontrado.NombrePerfil == "Contador")
                  {
                    ReturnValues = db.DBFunciones.Where(x => x.NombreModulo == "Contabilidad-Comun" ||
                                                             x.NombreFuncion == "PanelClienteContable").ToList();
                  }
            }
            return ReturnValues;
        }

        

    }