﻿using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace TryTestWeb.Models.ModelosSistemaContable.ContabilidadBoletas
{
    public class BoletasCoVModel
    { 
        public int BoletasCoVModelID { get; set; }
        public string CuentaAuxiliar { get; set; }

        public BoletasCoVPadreModel BoletaCoVPadre { get;set; }
        public ClientesContablesModel ClienteContable { get; set; }
        public QuickReceptorModel Prestador { get; set; }
        public int VoucherModelID { get; set; }
        public int HaSidoConvertidoAVoucher { get; set; }
        public DateTime FechaInsercion { get; set; }
        public DateTime Fecha { get; set; }
        public int NumeroDeDocumento { get; set; }
        public TipoDte TipoDocumento { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string CuentaContable { get; set; } //Compra o venta
        public decimal Neto { get; set; }
        public decimal Iva { get; set; }
        public int CentroDeCostos { get; set; }
        public DateTime FechaPeriodoTributario { get; set; }

        public static bool InsertBoletasCoV(ClientesContablesModel ObjCliente, List<BoletasExcelModel> BoletasItems, TipoCentralizacion Tipo)
        {
            bool result = false;

            
            using (IDbConnection db = new MySqlConnection(ConfigurationManager.ConnectionStrings["ProdConnection"].ConnectionString))
            {
                db.Open();
                using (var dbContextTransaction = db.BeginTransaction())
                {
                    //Obtener la información de cada hijo y insertar al final de la ejecución
                    List<BoletasCoVModel> ListaBoletasHijo = new List<BoletasCoVModel>();

                    //Recuerda tener la cuenta Auxiliar como prioritaria para crear los registros de los auxiliares.

                    decimal TotalNeto = 0;
                    decimal TotalIva = 0;

                    TipoReceptor tipoReceptor = new TipoReceptor();

                   
                    if (Tipo == TipoCentralizacion.Compra) tipoReceptor = TipoReceptor.PR;
                    if (Tipo == TipoCentralizacion.Venta) tipoReceptor = TipoReceptor.CL;

                    //esto debe ser insertado al final.


                    // HASTA AQUÍ ESTAMOS BIEN

                    //QuickReceptorModel.CrearOActualizarPrestadorPorRut(RutDupleDuple, RazonSocialDuple, objCliente, db, tipoReceptor);

                    int CuentaContableIDIvaAUsar = ParametrosClienteModel.GetCuentaContableIvaAUsar(ObjCliente);
                    //Son 3 detalles, este pertenece a la que contenga el iva

                    foreach (BoletasExcelModel ItemBoleta in BoletasItems)
                    {
                        List<DetalleVoucherModelDTO> detalleVoucher = new List<DetalleVoucherModelDTO>();

                        QuickReceptorModel Receptor = QuickReceptorModel.CrearOActualizarPrestadorPorRut(ItemBoleta.Rut, ItemBoleta.RazonSocial, ObjCliente, tipoReceptor.ToString());

                        int CuentaContableSeleccionada = 0;



                        string QueryCuentaContableAuxiliar = $"SELECT CuentaContableModelID FROM CuentaContableModel WHERE ClientesContablesModelID = {ObjCliente.ClientesContablesModelID} AND CodInterno ={ItemBoleta.CuentaAuxiliar}";
                        int IdCuentaContableAuxiliar = db.Query<int>(QueryCuentaContableAuxiliar).FirstOrDefault(); //esta es la cuenta que lleva la suma del Iva y del Neto
                        

                        //Crear query de conseguir la cuenta asociada de IVA 
                        string QueryGetCuentaContable = $"SELECT CuentaContableModelID FROM CuentaContableModel WHERE ClientesContablesModelID = {ObjCliente.ClientesContablesModelID} AND CodInterno ={ItemBoleta.CuentaContable}";
                        var IdCuentaContable = db.Query<int>(QueryGetCuentaContable).FirstOrDefault(); // Esta es la cuenta del neto

                        //Recuerda se insertan 2 detallevoucher ya que uno pertenece al neto y el otro al iva
                        string QueryInsertVoucher = "INSERT INTO VoucherModel (ClientesContablesModelID,Glosa,FechaEmision,Tipo,NumeroVoucher,DadoDeBaja,CentroDeCosto_CentroCostoModelID,TipoOrigen,TipoOrigenVoucher)" +
                                                     "VALUES(@ClientesContablesModelID,@Glosa,@FechaEmision,@Tipo,@NumeroVoucher,@DadoDeBaja,@CentroDeCosto_CentroCostoModelID,@TipoOrigen,@TipoOrigenVoucher)";

                        var QueryVoucherResult = db.Execute(QueryInsertVoucher, 
                        new {
                            ClientesContablesModelID = ObjCliente.ClientesContablesModelID,
                            Glosa = "BOLETA DE COMPRA " + ItemBoleta.CuentaContable,
                            FechaEmision = ItemBoleta.Fecha,
                            Tipo = "",
                            NumeroVoucher = 0,
                            DadoDeBaja = 0,
                            CentroDeCosto_CentroCostoModelID = 0,
                            TipoOrigen = 1,
                            TipoOrigenVoucher = 1
                        });

                        string QueryObtenerUltimoVoucher = $"SELECT MAX(VoucherModelID) FROM VoucherModel WHERE ClientesContablesModelID = {ObjCliente.ClientesContablesModelID}";
                        int ultimoVoucherId = db.Query<int>(QueryObtenerUltimoVoucher).FirstOrDefault();

                        //Se generan 2 lineas
                        string QueryInsertDetalleVoucher = "INSERT INTO DetalleVoucherModel (VoucherModelID,MontoDebe,MontoHaber,GlosaDetalle,FechaDoc,Auxiliar_AuxiliaresModelID,ObjCuentaContable_CuentaContableModelID,CentroCostoID)" +
                                                           "VALUES (@VoucherModelID,@MontoDebe,@MontoHaber,@GlosaDetalle,@FechaDoc,@Auxiliar_AuxiliaresModelID,@ObjCuentaContable_CuentaContableModelID,@CentroCostoID)";

                        var QueryDetalleVoucherResult = db.Execute(QueryInsertDetalleVoucher,
                        new
                        {
                            VoucherModelID = ultimoVoucherId,
                            MontoDebe = ItemBoleta.Neto,
                            MontoHaber = 0,
                            GlosaDetalle = "BOLETA DE " + ItemBoleta.CuentaContable,
                            FechaDoc = ItemBoleta.Fecha,
                            Auxiliar_AuxiliaresModelID = 0,
                            ObjCuentaContable_CuentaContableModelID = IdCuentaContable,
                            CentroCostoID = ItemBoleta.CentroDeCostos
                        });

                        string QueryObtenerUltimoDetalleVoucher = $"SELECT MAX(DetalleVoucherModelID) FROM DetalleVoucherModel WHERE ClientesContablesModelID = {ObjCliente.ClientesContablesModelID}";
                        int ultimoDetalleVoucherId = db.Query<int>(QueryObtenerUltimoDetalleVoucher).FirstOrDefault();

                        detalleVoucher.Add(new DetalleVoucherModelDTO
                        {
                            DetalleVoucherModelID = ultimoDetalleVoucherId,
                            VoucherModelID = ultimoVoucherId,
                            MontoDebe = ItemBoleta.Neto,
                            MontoHaber = 0,
                            GlosaDetalle = "BOLETA DE " + ItemBoleta.CuentaContable,
                            FechaDoc = ItemBoleta.Fecha,
                            AuxiliaresModelID = 0,
                            CuentaContableModelID = IdCuentaContable,
                            CentroCostoID = ItemBoleta.CentroDeCostos
                        });
                        //Execute
                        string QueryInsertDetalleVoucherDos = "INSERT INTO DetalleVoucherModel (VoucherModelID,MontoDebe,MontoHaber,GlosaDetalle,FechaDoc,RazonSocialDoc,Auxiliar_AuxiliaresModelID,ObjCuentaContable_CuentaContableModelID,CentroCostoID)" +
                                   "VALUES (@VoucherModelID,@MontoDebe,@MontoHaber,@GlosaDetalle,@FechaDoc,@RazonSocialDoc,@Auxiliar_AuxiliaresModelID,@ObjCuentaContable_CuentaContableModelID,@CentroCostoID)";

                        var QueryDetalleVoucherResultDos = db.Execute(QueryInsertDetalleVoucherDos,
                        new
                        {
                            VoucherModelID = ultimoVoucherId,
                            MontoDebe =  0,
                            MontoHaber = ItemBoleta.Iva,
                            GlosaDetalle = "BOLETA DE " + ItemBoleta.CuentaContable,
                            FechaDoc = ItemBoleta.Fecha,
                            Auxiliar_AuxiliaresModelID = 0,
                            ObjCuentaContable_CuentaContableModelID = CuentaContableSeleccionada,
                            CentroCostoID = ItemBoleta.CentroDeCostos
                        });

                        string QueryObtenerUltimoDetalleVoucherDos = $"SELECT MAX(DetalleVoucherModelID) FROM DetalleVoucherModel WHERE ClientesContablesModelID = {ObjCliente.ClientesContablesModelID}";
                        int ultimoDetalleVoucherIdDos = db.Query<int>(QueryInsertDetalleVoucherDos).FirstOrDefault();

                        detalleVoucher.Add(new DetalleVoucherModelDTO
                        {
                            DetalleVoucherModelID = ultimoDetalleVoucherIdDos,
                            VoucherModelID = ultimoVoucherId,
                            MontoDebe = 0,
                            MontoHaber = ItemBoleta.Iva,
                            GlosaDetalle = "BOLETA DE " + ItemBoleta.CuentaContable,
                            FechaDoc = ItemBoleta.Fecha,
                            AuxiliaresModelID = 0,
                            CuentaContableModelID = IdCuentaContable,
                            CentroCostoID = ItemBoleta.CentroDeCostos
                        });
                        //Execute

                        //falta la logica de los auxiliares.

                        //por cada detalle voucher se crea un auxiliar

                        foreach (DetalleVoucherModelDTO item in detalleVoucher)
                        {
                            //revisar como funciona la lógica de esta parte ¿Es completamente necesario

                                //AuxiliaresModel Auxiliar = new AuxiliaresModel();

                                //Auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                                //Auxiliar.LineaNumeroDetalle = nuevoVoucher.ListaDetalleVoucher.Count;
                                //Auxiliar.MontoTotal = NuevoDetalleVoucher.MontoDebe + NuevoDetalleVoucher.MontoHaber;
                                //Auxiliar.objCtaContable = NuevoDetalleVoucher.ObjCuentaContable;
                                ////Auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                                //db.DBAuxiliares.Add(Auxiliar);

                                //AuxiliaresDetalleModel nuevoAuxDetalle = new AuxiliaresDetalleModel();
                                //nuevoAuxDetalle.TipoDocumento = entradaLibro.TipoDocumento;
                                //nuevoAuxDetalle.Fecha = entradaLibro.FechaDoc;
                                //nuevoAuxDetalle.FechaContabilizacion = entradaLibro.FechaContabilizacion;

                                ////revisar
                                //// nuevoAuxDetalle.FechaVencimiento =   entradaLibro.fe
                                //nuevoAuxDetalle.Folio = entradaLibro.Folio;
                                //nuevoAuxDetalle.Individuo2 = entradaLibro.individuo;
                                //nuevoAuxDetalle.MontoNetoLinea = entradaLibro.MontoNeto;
                                //nuevoAuxDetalle.MontoExentoLinea = entradaLibro.MontoExento;
                                //nuevoAuxDetalle.MontoIVALinea = entradaLibro.MontoIva;

                                //nuevoAuxDetalle.MontoTotalLinea = entradaLibro.MontoTotal;
                                //nuevoAuxDetalle.AuxiliaresModelID = Auxiliar.AuxiliaresModelID;

                                //if (entradaLibro.TipoLibro == TipoCentralizacion.Compra)
                                //{
                                //    nuevoAuxDetalle.MontoIVANoRecuperable = entradaLibro.MontoIvaNoRecuperable;
                                //    nuevoAuxDetalle.MontoIVAUsoComun = entradaLibro.MontoIvaUsocomun;
                                //    nuevoAuxDetalle.MontoIVAActivoFijo = entradaLibro.MontoIvaActivoFijo;
                                //}
                                //Condición de cuenta auxiliar.
                                string QueryInsertAuxiliares = "INSERT INTO AuxiliaresModel (DetalleVoucherModelID, LineaNumeroDetalle, MontoTotal, Tipo, objCtaContable_CuentaContableModelID)" +
                                " VALUES(@DetalleVoucherModelID,@LineaNumeroDetalle,@MontoTotal,@Tipo,@objCtaContable_CuentaContableModelID)";


                            string QueryInsertAuxiliaresDetalle = "INSERT INTO AuxiliresDetalleModel (TipoDocumento,Fecha,,FechaContabilizacion,Folio,Individuo2_QuickReceptorModelID,MontoNetoLinea,MontoExentoLinea,MontoIVALinea,MontoTotalLinea,AuxiliaresModelID)" +
                            " VALUES(@TipoDocumento,@Fecha,@FechaContabilizacion,@Folio,@Individuo2_QuickReceptorModelID,@MontoNetoLinea,@MontoExentoLinea,@MontoIVALinea,@MontoTotalLinea,@AuxiliaresModelID)";

                        }


                        string QueryObtenerIdTablaPadre = $"SELECT MAX(BoletasCoVPadreModelID) FROM BoletasCoVPadreModel WHERE ClienteContableModelID_ClientesContablesModelID = {ObjCliente.ClientesContablesModelID}";
                        int idTablaPadre = db.Query<int>(QueryObtenerIdTablaPadre).FirstOrDefault();


                        string queryReceptorDummy = "SELECT * FROM QuickReceptorModel LIMIT 1";
                        QuickReceptorModel receptor = db.Query<QuickReceptorModel>(queryReceptorDummy).FirstOrDefault();




                        string QueryInsertHijo = "INSERT INTO BoletasCoVModel (CuentaAuxiliar, BoletaCoVPadre, ClienteContable_ClientesContablesModelID, Prestador," +
                                                 " VoucherModelID, HaSidoConvertidoAVoucher, FechaInsercion, Fecha, NumeroDeDocumento, TipoDocumento," +
                                                 " FechaVencimiento, CuentaContable, Neto, Iva, CentroDeCostos, FechaPeriodoTributario)" +
                                                 " VALUES(@CuentaAuxiliar, @BoletaCoVPadre, @ClienteContable, @Prestador, @VoucherModelID, @HaSidoConvertidoAVoucher," +
                                                 "@FechaInsercion, @Fecha, @NumeroDeDocumento, @TipoDocumento, @FechaVencimiento, @CuentaContable, @Neto, @Iva, @CentroDeCostos," +
                                                 "@FechaPeriodoTributario)";

                        var ResultadoInsercionHijos = db.Execute(QueryInsertHijo,
                            new
                            {
                                CuentaAuxiliar = ItemBoleta.CuentaAuxiliar,
                                BoletaCoVPadre = idTablaPadre,
                                ClienteContable_ClientesContablesModelID = ObjCliente,
                                Prestador = Receptor,
                                VoucherModelID = ultimoVoucherId,
                                HaSidoConvertidoAVoucher = 1,
                                FechaInsercion = DateTime.Now,
                                Fecha = ItemBoleta.Fecha,
                                NumeroDeDocumento = ItemBoleta.NumeroDeDocumento,
                                TipoDocumento = TipoDte.BoletaElectronica,
                                FechaVencimiento = DateTime.Now,
                                CuentaContable = "123",
                                Neto = 100,
                                Iva = 100,
                                CentroDeCostos = 0,
                                FechaPeriodoTributario = DateTime.Now
                            });
                    }



                }

                string QueryInsertPadre = "INSERT INTO BoletasCoVPadreModel (ClienteContableModelID_ClientesContablesModelID,FechaBoletas,FechaCreacion,TotalNeto,TotalIva) " +
                          "VALUES (@ClienteContableModelID_ClientesContablesModelID, @FechaBoletas, @FechaCreacion, @TotalNeto, @TotalIva)";

                var QueryResult = db.Execute(QueryInsertPadre,
                                    new
                                    {
                                        ClienteContableModelID_ClientesContablesModelID = ObjCliente.ClientesContablesModelID,
                                        FechaBoletas = DateTime.Now,
                                        FechaCreacion = DateTime.Now,
                                        tipoCentralizacion = (int)Tipo,
                                        TotalNeto = 1,
                                        TotalIva = 1
                                    });

                db.Close();
            }

            return result;
        }
    }
}