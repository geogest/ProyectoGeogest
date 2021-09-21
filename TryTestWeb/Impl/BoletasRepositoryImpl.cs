using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using TryTestWeb.Models.ModelosSistemaContable.ContabilidadBoletas;
using TryTestWeb.Repository;

namespace TryTestWeb.Impl
{
    public class BoletasRepositoryImpl : IBoletasRepository
    {

        private IDbConnection db = new MySqlConnection(ConfigurationManager.ConnectionStrings["ProdConnection"].ConnectionString);

        public bool InsertarBoletas(ClientesContablesModel objClienteContable)
        {
            throw new NotImplementedException();
        }
        public BoletasCoVModel GetBoletaItemById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<BoletasCoVModel> GetBoletasItem(ClientesContablesModel objClienteContable)
        {
            throw new NotImplementedException();
        }

        public List<BoletasCoVPadreModel> GetBoletasPadre(ClientesContablesModel objClienteContable)
        {
            throw new NotImplementedException();
        }

        public BoletasCoVPadreModel GetBoletasPadresById(int Id)
        {
            throw new NotImplementedException();
        }


    }
}