using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;


public class LibroHonorariosDeTerceros
{
    public int LibroHonorariosDeTercerosID { get; set; }
    public ClientesContablesModel ClienteContable { get; set; }
    public virtual VoucherModel VoucherModel { get; set; }
    public int NumOFolio { get; set; }
    public string Estado { get; set; }
    public DateTime FechaInicial { get; set; }
    public string RutEmpresa { get; set; }
    public string NombreEmpresa { get; set; }
    public DateTime FechaFinal { get; set; }
    public DateTime FechaContabilizacion { get; set; }
    public string RutReceptor { get; set; }
    public string NombreReceptor { get; set; }
    public decimal Brutos { get; set; }
    public decimal Retenidos { get; set; }
    public decimal Pagado { get; set; }
    public virtual QuickReceptorModel Receptor { get; set; }
    public TipoCentralizacion TipoLibro { get; set; } = TipoCentralizacion.Honorarios;
    public TipoVoucher TipoV { get; set; } = TipoVoucher.Traspaso;
    public TipoOrigen TipoO { get; set; } = TipoOrigen.HonorarioTercero;
    public bool HaSidoConvertidoAVoucher { get; set; } = false;
    public DateTime FechaDeCreacion { get; set; } = DateTime.Now;

    public static List<LibroHonorariosDeTerceros> ProcesarLibroHonorariosTerceros(FacturaPoliContext db, ClientesContablesModel ObjCliente, QuickEmisorModel EstaEmpresa, List<string[]> BoletasNoProcesadas, string FechaContabilizacion)
    {
        BoletasNoProcesadas.RemoveRange(0, 8);
        int NumeroFinal = BoletasNoProcesadas.Count(); //Arreglo para simular los elementos de un array partiendo del 0
        int IndexElementosNoDeseados = BoletasNoProcesadas.Count() - 3; //Arreglo para simular los elementos de un array partiendo del 0
        int CantidadDeElementosAeliminar = NumeroFinal - IndexElementosNoDeseados;
        BoletasNoProcesadas.RemoveRange(IndexElementosNoDeseados, CantidadDeElementosAeliminar);

        List<LibroHonorariosDeTerceros> ListaARetornar = new List<LibroHonorariosDeTerceros>();

        string TipoReceptor = "H";
        List<LibroHonorariosDeTerceros> SinRepetidos = new List<LibroHonorariosDeTerceros>();
        foreach (string[] ColumnaBoleta in BoletasNoProcesadas)
        {
            LibroHonorariosDeTerceros VerificadorLibroHonorTercero = new LibroHonorariosDeTerceros();

            DateTime FechaConta = ParseExtensions.ToDD_MM_AAAA_Multi(FechaContabilizacion);
            DateTime FechaInicial = ParseExtensions.ToDD_MM_AAAA_Multi(ColumnaBoleta[2]);
            DateTime FechaFinal = ParseExtensions.ToDD_MM_AAAA_Multi(ColumnaBoleta[5]);
            int Folio = Convert.ToInt32(ColumnaBoleta[0]);
            string Estado = ColumnaBoleta[1].Trim();
            string RutPrestador = ColumnaBoleta[6].Trim();
            string NombrePrestador = ColumnaBoleta[7].Trim();
            decimal Bruto = Convert.ToDecimal(ColumnaBoleta[8].Replace(".",""));
            decimal Retenido = Convert.ToDecimal(ColumnaBoleta[9].Replace(".", ""));
            decimal Pagado = Convert.ToDecimal(ColumnaBoleta[10].Replace(".", ""));

            SinRepetidos = db.DBLibroHonorariosTerceros.Where(x => x.ClienteContable.ClientesContablesModelID == ObjCliente.ClientesContablesModelID &&
                                                                   x.NumOFolio == Folio &&
                                                                   x.Receptor.RUT == RutPrestador &&
                                                                   x.HaSidoConvertidoAVoucher == true &&
                                                                   x.TipoLibro == TipoCentralizacion.Honorarios).ToList();

            List<VoucherModel> EstaVigenteEncontrado = new List<VoucherModel>();
            VoucherModel VoucherEncontrado = new VoucherModel();

            if (SinRepetidos != null || SinRepetidos.Count() > 0)
            {
                foreach (var ItemRepetido in SinRepetidos)
                {
                    VoucherEncontrado = db.DBVoucher.SingleOrDefault(x => x.VoucherModelID == ItemRepetido.VoucherModel.VoucherModelID);

                    if (VoucherEncontrado.DadoDeBaja == false)
                    {
                        EstaVigenteEncontrado.Add(VoucherEncontrado);
                    }
                }
            }

            if (SinRepetidos.Count() > 0 && SinRepetidos != null && EstaVigenteEncontrado.Count() > 0)
            {
                continue;
            }

            if (Estado == "VIGENTE")
            {
                VerificadorLibroHonorTercero.FechaContabilizacion = FechaConta;
                VerificadorLibroHonorTercero.Estado = Estado;
                VerificadorLibroHonorTercero.ClienteContable = ObjCliente;
                VerificadorLibroHonorTercero.NumOFolio = Folio;
                VerificadorLibroHonorTercero.FechaInicial = FechaInicial;
                VerificadorLibroHonorTercero.RutEmpresa = EstaEmpresa.RUTEmpresa;
                VerificadorLibroHonorTercero.NombreEmpresa = EstaEmpresa.RazonSocial;
                VerificadorLibroHonorTercero.FechaFinal = FechaFinal;
                VerificadorLibroHonorTercero.RutReceptor = RutPrestador;
                VerificadorLibroHonorTercero.NombreReceptor = NombrePrestador;
                VerificadorLibroHonorTercero.Brutos = Bruto;
                VerificadorLibroHonorTercero.Retenidos = Retenido;
                VerificadorLibroHonorTercero.Pagado = Pagado;

                QuickReceptorModel objPrestador = QuickReceptorModel.CrearOActualizarPrestadorPorRut(VerificadorLibroHonorTercero.RutReceptor, VerificadorLibroHonorTercero.NombreReceptor, ObjCliente, db, TipoReceptor);
                VerificadorLibroHonorTercero.Receptor = objPrestador;

                ListaARetornar.Add(VerificadorLibroHonorTercero);
            }
        }
        if (ListaARetornar.Count() > 0 && SinRepetidos.Count() == 0)
        {
            db.DBLibroHonorariosTerceros.AddRange(ListaARetornar);
            db.SaveChanges();
        }

        return ListaARetornar;
    }


    public static void ProcesarLibroHonorTerceroAVoucher(List<LibroHonorariosDeTerceros> lstAConvertir, ClientesContablesModel ObjCliente, FacturaPoliContext db, List<CuentaContableModel> lstCuentaConbtale)
    {
        if (lstAConvertir == null || lstAConvertir.Count == 0 || ObjCliente == null || ObjCliente.ParametrosCliente == null)
            return;


        if (lstAConvertir.Count != lstAConvertir.Count)
        {
            throw new Exception();
        }

        CuentaContableModel Retencion = null;
        CuentaContableModel Retencion2 = null;

        Retencion = db.DBCuentaContable.SingleOrDefault(x => x.CuentaContableModelID == ObjCliente.ParametrosCliente.CuentaRetencionHonorarios.CuentaContableModelID && x.ClientesContablesModelID == ObjCliente.ClientesContablesModelID);
        Retencion2 = db.DBCuentaContable.SingleOrDefault(x => x.CuentaContableModelID == ObjCliente.ParametrosCliente.CuentaRetencionesHonorarios2.CuentaContableModelID && x.ClientesContablesModelID == ObjCliente.ClientesContablesModelID);

        if (Retencion == null)
            return;

        List<VoucherModel> lstNuevosVouchers = new List<VoucherModel>();
        int contadorAnexo = 0;

        int? nullableProxVoucherNumber = ParseExtensions.ObtenerNumeroProximoVoucherINT(ObjCliente, db); //Contamos el prox voucher

        int baseNumberFolio = nullableProxVoucherNumber.Value;
        CuentaContableModel cuentaPrincipal = new CuentaContableModel();

        foreach (LibroHonorariosDeTerceros itemLibroHonor in lstAConvertir)
        {
            decimal MontoBruto = itemLibroHonor.Brutos; // Debe
            decimal MontoRetenido = itemLibroHonor.Retenidos; //Haber
            decimal MontoPagado = itemLibroHonor.Pagado; // Haber

            VoucherModel nuevoVoucher = new VoucherModel();

            nuevoVoucher.TipoOrigen = "HonorarioTercero";
            nuevoVoucher.TipoOrigenVoucher = itemLibroHonor.TipoO;

            nuevoVoucher.ClientesContablesModelID = ObjCliente.ClientesContablesModelID;
            nuevoVoucher.FechaEmision = itemLibroHonor.FechaContabilizacion;
            nuevoVoucher.Tipo = itemLibroHonor.TipoV; // Traspaso

            string FullDescripcionDocOriginal = lstCuentaConbtale[contadorAnexo].nombre + " / Folio: " + itemLibroHonor.NumOFolio + " / " + itemLibroHonor.Receptor.RazonSocial;

            nuevoVoucher.Glosa = FullDescripcionDocOriginal;
            nuevoVoucher.NumeroVoucher = baseNumberFolio;

            List<DetalleVoucherModel> DetalleVoucher = new List<DetalleVoucherModel>();

            DetalleVoucherModel DetalleVhonorarios = new DetalleVoucherModel(); // Linea 1 Del Voucher

            DetalleVhonorarios.FechaDoc = itemLibroHonor.FechaContabilizacion;

            DetalleVhonorarios.GlosaDetalle = "Honorarios Profesionales" + FullDescripcionDocOriginal;

            DetalleVhonorarios.MontoDebe = MontoBruto;
            DetalleVhonorarios.MontoHaber = 0;

            DetalleVhonorarios.ObjCuentaContable = lstCuentaConbtale[contadorAnexo];

            DetalleVoucher.Add(DetalleVhonorarios);

            DetalleVoucherModel DetalleVhonorarios2 = new DetalleVoucherModel(); //Linea 2 Del Voucher

            if (itemLibroHonor.Retenidos > 0)
            {
                DetalleVhonorarios2.FechaDoc = itemLibroHonor.FechaContabilizacion;

                DetalleVhonorarios2.MontoDebe = 0;
                DetalleVhonorarios2.MontoHaber = MontoRetenido;
                DetalleVhonorarios2.ObjCuentaContable = Retencion;


                DetalleVhonorarios2.GlosaDetalle = Retencion.nombre + FullDescripcionDocOriginal;

                DetalleVoucher.Add(DetalleVhonorarios2);
            }

            DetalleVoucherModel DetalleVhonorarios2parte2 = new DetalleVoucherModel(); // Linea 3 del voucher

            if (itemLibroHonor.Pagado > 0)
            {
                DetalleVhonorarios2parte2.FechaDoc = itemLibroHonor.FechaContabilizacion;
                DetalleVhonorarios2parte2.MontoDebe = 0;
                DetalleVhonorarios2parte2.MontoHaber = MontoPagado;

                DetalleVhonorarios2parte2.GlosaDetalle = Retencion2.nombre + FullDescripcionDocOriginal;
                DetalleVhonorarios2parte2.ObjCuentaContable = Retencion2;
                cuentaPrincipal = Retencion2;

                DetalleVoucher.Add(DetalleVhonorarios2parte2);
            }

            if (DetalleVoucher.Sum(x => x.MontoDebe) == DetalleVoucher.Sum(x => x.MontoHaber))
            {
                nuevoVoucher.ListaDetalleVoucher = DetalleVoucher;
                itemLibroHonor.HaSidoConvertidoAVoucher = true;
            }
            else
            {
                contadorAnexo++;
                continue;
            }

            lstNuevosVouchers.Add(nuevoVoucher);

            foreach (DetalleVoucherModel NuevoDetalleVoucher in nuevoVoucher.ListaDetalleVoucher)
            {
                if (NuevoDetalleVoucher.ObjCuentaContable == cuentaPrincipal)
                {
                    AuxiliaresModel Auxiliar = new AuxiliaresModel();

                    Auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                    Auxiliar.LineaNumeroDetalle = nuevoVoucher.ListaDetalleVoucher.Count;
                    Auxiliar.MontoTotal = NuevoDetalleVoucher.MontoDebe + NuevoDetalleVoucher.MontoHaber;
                    Auxiliar.objCtaContable = NuevoDetalleVoucher.ObjCuentaContable;
                    Auxiliar.Tipo = TipoAuxiliar.Honorarios;
                    db.DBAuxiliares.Add(Auxiliar);

                    AuxiliaresDetalleModel nuevoAuxDetalle = new AuxiliaresDetalleModel();

                    nuevoAuxDetalle.Fecha = itemLibroHonor.FechaInicial;
                    nuevoAuxDetalle.FechaContabilizacion = itemLibroHonor.FechaContabilizacion;
                    nuevoAuxDetalle.FechaVencimiento = itemLibroHonor.FechaFinal;
                    nuevoAuxDetalle.Folio = itemLibroHonor.NumOFolio;
                    nuevoAuxDetalle.Individuo2 = itemLibroHonor.Receptor;
                    nuevoAuxDetalle.MontoBrutoLinea = MontoBruto;
                    nuevoAuxDetalle.ValorLiquido = MontoPagado;
                    nuevoAuxDetalle.ValorRetencion = MontoRetenido;
                    nuevoAuxDetalle.MontoTotalLinea = MontoPagado;
                    nuevoAuxDetalle.AuxiliaresModelID = Auxiliar.AuxiliaresModelID;


                    db.DBAuxiliaresDetalle.Add(nuevoAuxDetalle);
                    db.SaveChanges();

                    //decimal MontoBruto = itemLibroHonor.Brutos; // Debe
                    //decimal MontoRetenido = itemLibroHonor.Retenido; //Haber
                    //decimal MontoPagado = itemLibroHonor.Pagado; // Haber
                }
            }
            contadorAnexo++;
            baseNumberFolio++;
        }

        if (lstNuevosVouchers != null && lstNuevosVouchers.Count > 0)
        {
            foreach (VoucherModel NuevoVoucher in lstNuevosVouchers)
            {
                db.DBVoucher.Add(NuevoVoucher);
            }
            db.SaveChanges();
            int posicion = 0;
            foreach (VoucherModel NuevoVoucher in lstNuevosVouchers)
            {
                int posicion2 = 0;
                foreach (LibroHonorariosDeTerceros itemHonor in lstAConvertir)
                {
                    if (posicion == posicion2)
                    {
                        itemHonor.VoucherModel = NuevoVoucher;
                        db.DBLibroHonorariosTerceros.AddOrUpdate(itemHonor);
                        db.SaveChanges();
                    }
                    posicion2++;
                }

                int total = NuevoVoucher.ListaDetalleVoucher.Count;

                foreach (DetalleVoucherModel NuevoDetalleVoucher in NuevoVoucher.ListaDetalleVoucher)
                {
                    if (NuevoDetalleVoucher.ObjCuentaContable == cuentaPrincipal)
                    {

                        AuxiliaresModel auxiliar = db.DBAuxiliares.Where(x => x.LineaNumeroDetalle == total &&
                                                                              x.objCtaContable.ClientesContablesModelID == cuentaPrincipal.ClientesContablesModelID &&
                                                                              x.DetalleVoucherModelID == 0).FirstOrDefault();

                        if (auxiliar != null)
                        {
                            auxiliar.DetalleVoucherModelID = NuevoDetalleVoucher.DetalleVoucherModelID;
                            db.DBAuxiliares.AddOrUpdate(auxiliar);

                            NuevoDetalleVoucher.Auxiliar = auxiliar;
                            db.DBDetalleVoucher.AddOrUpdate(NuevoDetalleVoucher);
                            db.SaveChanges();
                        }
                    }
                }
                posicion++;
            }

        }

    }
}



   