using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryTestWeb.Models.ModelosSistemaContable.ContabilidadBoletas;

namespace TryTestWeb.Repository
{
    public interface IBoletasRepository
    {
        bool InsertarBoletas(ClientesContablesModel objClienteContable);
        List<BoletasCoVModel> GetBoletasItem(ClientesContablesModel objClienteContable);
        List<BoletasCoVPadreModel> GetBoletasPadre(ClientesContablesModel objClienteContable);
        BoletasCoVModel GetBoletaItemById(int Id);
        BoletasCoVPadreModel GetBoletasPadresById(int Id);
    }
}
