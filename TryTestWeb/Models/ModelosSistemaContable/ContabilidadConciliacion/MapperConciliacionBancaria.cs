using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TryTestWeb.Controllers;

public class MapperConciliacionBancaria
{

    public static FiltrosParaLibros MapperFiltrosParaLibros(int pagina, int registrosPorPagina, string fechaInicio, string fechaFin, int anio, int mes, string rut, string glosa, string cuentacontableId, string razonprestador, int numvoucher, bool filtro, int centrocosto, bool estaconciliado)
    {
        FiltrosParaLibros ReturnValues = new FiltrosParaLibros()
        {
            pagina = pagina,
            cantidadRegistrosPorPagina = registrosPorPagina,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Anio = anio,
            Mes = mes,
            Rut = rut,
            Glosa = glosa,
            CuentaContableID = cuentacontableId,
            RazonPrestador = razonprestador,
            NumVoucher = numvoucher,
            Filtro = filtro,
            CentroDeCostosID = centrocosto,
            EstaConciliado = estaconciliado
        };

        return ReturnValues;
    }
}