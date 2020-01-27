 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

public class QuickReceptorModel
{
    public int QuickReceptorModelID { get; set; }
    public int QuickEmisorModelID { get; set; }

    public int ClientesContablesModelID { get; set; }
 
    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Rut")]
    [StringLength(10)]
    [ValidaRut(ErrorMessage = "El Rut ingresado es Invalido.")]
    public string RUT { get; set; }

    public string NombreFantasia { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Display(Name = "Razón Social")]
    [StringLength(100)]
    public string RazonSocial { get; set; }

    //[Required(ErrorMessage = "Este campo es obligatorio.")]
    //[StringLength(70)]
    public string Direccion { get; set; }

  //  [Required(ErrorMessage = "Este campo es obligatorio.")]
  //  [StringLength(20)]
    public string Comuna { get; set; }

   // [StringLength(20)]
    public string Ciudad { get; set; }

    //[Required(ErrorMessage = "Este campo es obligatorio.")]
    //[Display(Name = "Giro")]
    //[StringLength(80)]
    public string Giro { get; set; }
    public string Contacto { get; set; }

    public string NombreContacto { get; set; }

    public string RUTSolicitante { get; set; }

    public string TelefonoContacto { get; set; }

    public int idRegion { get; set; }

    public int idComuna { get; set; }

    public String tipoReceptor { get; set; }


    public bool DadoDeBaja { get; set; } = false;

    public CuentaContableModel CuentaConToReceptor { get; set; }
    public QuickReceptorModel()
    {

    }

    public QuickReceptorModel(string _RUT, string _RazonSocial, string _Direccion, string _Comuna, string _Ciudad, string _Giro,
        string _Contacto, string _RUTSolicitante)
    {
        RUT = _RUT;
        RazonSocial = _RazonSocial;
        Direccion = _Direccion;
        Comuna = _Comuna;
        Ciudad = _Ciudad;
        Giro = _Giro;
        Contacto = _Contacto;
        RUTSolicitante = _RUTSolicitante;
    }

    public static QuickReceptorModel GetReceptorByIDandEmisor(string UserID, QuickEmisorModel _emisor, int IDReceptor)
    {
        QuickReceptorModel objReceptor = null;
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        IQueryable<QuickReceptorModel> objQueryable = db.Receptores.Where(r => r.QuickEmisorModelID == _emisor.QuickEmisorModelID);

        //List<QuickReceptorModel> lstReceptarTryTest = objQueryable.Where(r => r.QuickReceptorModelID == IDReceptor).ToList();

        objReceptor = objQueryable.SingleOrDefault(r => r.QuickReceptorModelID == IDReceptor);

        return objReceptor;
    }
    public  static QuickReceptorModel CrearOActualizarPrestadorPorRut(string RUTPrestador, string NombrePrestador, ClientesContablesModel objCliente, FacturaPoliContext db, String tipoReceptor )
    {
        int existe = db.Receptores.Where(r => r.RUT == RUTPrestador && r.QuickEmisorModelID == objCliente.QuickEmisorModelID && r.tipoReceptor == tipoReceptor && r.ClientesContablesModelID == objCliente.ClientesContablesModelID).Count();

        if (existe < 1)
        {
            QuickReceptorModel objPrestadores = new QuickReceptorModel();

            objPrestadores.QuickEmisorModelID = objCliente.QuickEmisorModelID;
            objPrestadores.RUT = RUTPrestador;
            objPrestadores.RazonSocial = NombrePrestador;
            objPrestadores.tipoReceptor = tipoReceptor;
            objPrestadores.Direccion = "1";
            objPrestadores.Giro = "1";
            objPrestadores.ClientesContablesModelID = objCliente.ClientesContablesModelID;
          

            db.Receptores.Add(objPrestadores);
            db.SaveChanges();
            //agrego filtro para saber que clientes tiene el cliente contable
            //int  clienteEmisor = db.DBClientesContablesEmisor.Where(r => objCliente.QuickEmisorModelID == r.QuickReceptorModelID && r.ClientesContablesModelID == objCliente.ParametrosCliente.ClientesContablesModelID).Count();
            //ClientesContablesEmisorModel clientesContablesEmisor = ((from t1 in db.DBClientesContablesEmisor
            //                                                         where t1.QuickReceptorModelID == objCliente.QuickEmisorModelID && t1.ClientesContablesModelID == objCliente.ClientesContablesModelID

            //                                                         select t1
            //                   ).FirstOrDefault());

            ClientesContablesEmisorModel clientesContablesEmisor = db.DBClientesContablesEmisor.Where(x => x.QuickReceptorModelID == objCliente.QuickEmisorModelID && x.ClientesContablesModelID == objCliente.ClientesContablesModelID).FirstOrDefault();

            if (clientesContablesEmisor == null) {
                ClientesContablesEmisorModel clienteEmisor2 = new ClientesContablesEmisorModel();
                clienteEmisor2.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                clienteEmisor2.QuickReceptorModelID = objCliente.QuickEmisorModelID;
                db.DBClientesContablesEmisor.Add(clienteEmisor2);


            }

            db.SaveChanges();
            return objPrestadores;
        }
        else
        {
            QuickReceptorModel objPrestadores = db.Receptores.Where(r => r.RUT == RUTPrestador && r.QuickEmisorModelID == objCliente.QuickEmisorModelID && r.tipoReceptor == tipoReceptor).First();

            ClientesContablesEmisorModel clientesContablesEmisor =  db.DBClientesContablesEmisor.Where(t1 => t1.QuickReceptorModelID == objCliente.QuickEmisorModelID && t1.ClientesContablesModelID == objCliente.ClientesContablesModelID).FirstOrDefault();

                          
                
                
               // db.DBClientesContablesEmisor.Where(r => objCliente.QuickEmisorModelID == r.QuickReceptorModelID && r.ClientesContablesModelID == objCliente.ParametrosCliente.ClientesContablesModelID).Count();

            if (clientesContablesEmisor == null)
            {
                ClientesContablesEmisorModel  clienteEmisor = new ClientesContablesEmisorModel();
                clienteEmisor.ClientesContablesModelID = objCliente.ClientesContablesModelID;
                clienteEmisor.QuickReceptorModelID = objCliente.QuickEmisorModelID;
                db.DBClientesContablesEmisor.Add(clienteEmisor);
                db.SaveChanges();

            }

            if (objPrestadores.RazonSocial == NombrePrestador)
            {
                return objPrestadores;
            }
            else
            {
                objPrestadores.RazonSocial = NombrePrestador;
                db.Receptores.AddOrUpdate(objPrestadores);
                db.SaveChanges();
                return objPrestadores;
            }
        }
    }

}
