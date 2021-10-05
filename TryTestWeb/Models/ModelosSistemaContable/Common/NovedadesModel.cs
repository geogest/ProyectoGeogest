using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TryTestWeb.Models.ModelosSistemaContable.Common
{
    public class NovedadesModel
    {
        public int NovedadesModelID { get; set; }
        public NovedadesRegistradasModel Novedad { get; set; }
        public ClientesContablesModel ClienteContable { get; set; }
        public DateTime FechaEjecucionNovedadEstecliente { get; set; }

        public static void AgregaUpdateTransversal(ClientesContablesModel objClienteContable, FacturaPoliContext db)
        {
            List<int> NovedadesRegistradasUsuario = db.DBNovedadesModel.Where(x => x.ClienteContable.ClientesContablesModelID == objClienteContable.ClientesContablesModelID).Select(x => x.Novedad.NovedadesRegistradasModelID).ToList();
            List<int> ListaNovedadesGeneralesRegistradas = db.DBNovedadesRegistradasModel.Select(x => x.NovedadesRegistradasModelID).ToList();
            List<int> NovedadesFaltantesUsuario = ListaNovedadesGeneralesRegistradas.Except(NovedadesRegistradasUsuario).ToList();

            List<NovedadesRegistradasModel> NovedadesAregistrarUsuario = db.DBNovedadesRegistradasModel.Where(x => NovedadesFaltantesUsuario.Contains(x.NovedadesRegistradasModelID)).ToList();

            List<NovedadesModel> lstAInsertar = NovedadesAregistrarUsuario.Select(NovedadFaltante => new NovedadesModel 
                                                                            {
                                                                                Novedad = NovedadFaltante,
                                                                                ClienteContable = objClienteContable,
                                                                                FechaEjecucionNovedadEstecliente = DateTime.Now
                                                                            }).ToList();
            if (lstAInsertar.Any())
            {
                db.DBNovedadesModel.AddRange(lstAInsertar);
                db.SaveChanges();
            }
        }
    }

    public class NovedadesRegistradasModel
    {
        public int NovedadesRegistradasModelID { get; set; }
        public string NombreNovedad { get; set; }
        public DateTime FechaCreacionNovedad { get; set; }
        public string NombreFuncionalidadAsociada { get; set; }

        //Si se quiere agregar más de una funcionalidad asociada por favor usar (,) el separador será la COMA 
        public static void InsertNovedad()
        {
            FacturaProduccionContext dbProduccion = new FacturaProduccionContext();
            FacturaContext dbTestContext = new FacturaContext();

            if (!dbProduccion.DBNovedadesRegistradasModel.Any())
            {
                NovedadesRegistradasModel nuevaNovedad = new NovedadesRegistradasModel()
                {
                    NombreNovedad = NovedadesEnumKeys.KeyNovedad.NewNumVoucherFormat.ToString(),
                    FechaCreacionNovedad = DateTime.Now,
                    NombreFuncionalidadAsociada = NovedadesEnumKeys.KeyNovedadFuncionalidadAsociada.GetNumVoucher.ToString()
                };

                dbProduccion.DBNovedadesRegistradasModel.Add(nuevaNovedad);
                dbProduccion.SaveChanges();
            }

            if (!dbTestContext.DBNovedadesRegistradasModel.Any())
            {
                NovedadesRegistradasModel nuevaNovedad = new NovedadesRegistradasModel()
                {
                    NombreNovedad = NovedadesEnumKeys.KeyNovedad.NewNumVoucherFormat.ToString(),
                    FechaCreacionNovedad = DateTime.Now,
                    NombreFuncionalidadAsociada = NovedadesEnumKeys.KeyNovedadFuncionalidadAsociada.GetNumVoucher.ToString()
                };

                dbTestContext.DBNovedadesRegistradasModel.Add(nuevaNovedad);
                dbTestContext.SaveChanges();
            }
        }

    }
}