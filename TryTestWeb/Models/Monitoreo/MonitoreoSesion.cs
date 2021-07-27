using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace TryTestWeb.Models.Monitoreo
{
    public class MonitoreoSesion
    {
        public int MonitoreoSesionID { get; set; }
        public int UsuarioID { get; set; }
        public bool EstaActivo { get; set; } = false;

        public static bool ControlarEstadoSesion(FacturaPoliContext db, string UserID, bool EstaIniciando) {

            var usuarioID = db.DBUsuarios.Where(x => x.IdentityID == UserID).FirstOrDefault().UsuarioModelID;
            var UsuarioEncontrado = db.DBMonitoreoSesion.Where(x => x.UsuarioID == usuarioID).FirstOrDefault();
            bool result = false;
            //si entra aquí quiere decir que el usuario ya está creado
            if (UsuarioEncontrado != null)
            {
                //upgrade
                UsuarioEncontrado.EstaActivo = UsuarioEncontrado.EstaActivo == true ? UsuarioEncontrado.EstaActivo = false : UsuarioEncontrado.EstaActivo = true;
                db.DBMonitoreoSesion.AddOrUpdate(UsuarioEncontrado);
                db.SaveChanges();
                result = true;

            }
            else
            {

                if (EstaIniciando)
                {
                    var DatosUsuarioAInsertar = new MonitoreoSesion()
                    {
                        UsuarioID = usuarioID,
                        EstaActivo = true
                    };

                    db.DBMonitoreoSesion.Add(DatosUsuarioAInsertar);
                    db.SaveChanges();
                    result = true;
                }
                else
                {

                    var DatosUsuarioAInsertar = new MonitoreoSesion()
                    {
                        UsuarioID = usuarioID,
                        EstaActivo = false
                    };

                    db.DBMonitoreoSesion.Add(DatosUsuarioAInsertar);
                    db.SaveChanges();
                    result = true;


                }
                //crear



            }
            return result;
        }
    }
}