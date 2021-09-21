using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TryTestWeb.Models.ModelosSistemaContable.ContabilidadBoletas
{
    public class BoletasCoVPadreModel
    {
        public int BoletasCoVPadreModelID { get; set; }
        public List<BoletasCoVModel> BoletaItems { get; set; }
        public ClientesContablesModel ClienteContableModelID { get; set; }
        public DateTime FechaBoletas { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now; //es realmente necesario esto?
        public TipoCentralizacion tipoCentralizacion { get; set; }
        public decimal TotalNeto { get; set; } //¿solo habrá un total? -> bruto.. neto..
        public decimal TotalIva { get; set; }
    }
}