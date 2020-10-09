using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class EstCtasCtesConciliadasViewModel
{
    public string RutPrestador { get; set; }
    public string NombrePrestador { get; set; }
    public CuentaContableModel CuentaContable { get; set; }
    public DateTime Fecha { get; set; }
    public int Folio { get; set; }
    public int VoucherID { get; set; }
    public string Comprobante { get; set; }
    public TipoDte Documento { get; set; }
    public DateTime Vencim { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal Debe { get; set; }
    public decimal Haber { get; set; }
    public decimal Saldo { get; set; }
    public decimal DebeAnalisis { get; set; }
    public decimal HaberAnalisis { get; set; }
    public string TipoAux { get; set; }
    public decimal TotalAux { get; set; }
    public decimal TotalCtaContable { get; set; }
    public bool EstaConciliado { get; set; } = false;


    public static IQueryable<EstCtasCtesConciliadasViewModel> GetlstCtasCtesConciliadas(FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        IQueryable<EstCtasCtesConciliadasViewModel> LstCtaCorriente = (from Detalle in db.DBDetalleVoucher
                                                                       join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                                                       join Auxiliar in db.DBAuxiliares on Detalle.Auxiliar.AuxiliaresModelID equals Auxiliar.AuxiliaresModelID
                                                                       join AuxiliaresDetalle in db.DBAuxiliaresDetalle on Auxiliar.AuxiliaresModelID equals AuxiliaresDetalle.AuxiliaresModelID
                                                                       where Auxiliar.objCtaContable.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                             Voucher.DadoDeBaja == false &&
                                                                             Auxiliar.objCtaContable.TieneAuxiliar == 1

                                                                       select new EstCtasCtesConciliadasViewModel
                                                                       {
                                                                           RutPrestador = AuxiliaresDetalle.Individuo2.RUT,
                                                                           NombrePrestador = AuxiliaresDetalle.Individuo2.RazonSocial,
                                                                           Fecha = Detalle.FechaDoc,
                                                                           Folio = AuxiliaresDetalle.Folio,
                                                                           Comprobante = Voucher.Tipo.ToString() + "   " + Voucher.NumeroVoucher.ToString() + "   " + Detalle.Auxiliar.LineaNumeroDetalle.ToString(),
                                                                           Documento = AuxiliaresDetalle.TipoDocumento,
                                                                           TipoAux = AuxiliaresDetalle.Individuo2.tipoReceptor,
                                                                           DebeAnalisis = Detalle.MontoDebe,
                                                                           HaberAnalisis = Detalle.MontoHaber,
                                                                           CuentaContable = Detalle.ObjCuentaContable,
                                                                           MontoTotal = AuxiliaresDetalle.MontoTotalLinea,
                                                                           VoucherID = Voucher.VoucherModelID
                                                                       });


        return LstCtaCorriente;
    }

    public static IQueryable<EstCtasCtesConciliadasViewModel> ObtenerLstCtasCtesConciladasEsteAño(FacturaPoliContext db, ClientesContablesModel objCliente)
    {
        IQueryable<EstCtasCtesConciliadasViewModel> LstCtaCorriente = (from Detalle in db.DBDetalleVoucher
                                                                       join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                                                       join Auxiliar in db.DBAuxiliares on Detalle.Auxiliar.AuxiliaresModelID equals Auxiliar.AuxiliaresModelID
                                                                       join AuxiliaresDetalle in db.DBAuxiliaresDetalle on Auxiliar.AuxiliaresModelID equals AuxiliaresDetalle.AuxiliaresModelID
                                                                       where Auxiliar.objCtaContable.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                             Voucher.DadoDeBaja == false &&
                                                                             Auxiliar.objCtaContable.TieneAuxiliar == 1

                                                                       select new EstCtasCtesConciliadasViewModel
                                                                       {
                                                                           RutPrestador = AuxiliaresDetalle.Individuo2.RUT,
                                                                           NombrePrestador = AuxiliaresDetalle.Individuo2.RazonSocial,
                                                                           Fecha = Detalle.FechaDoc,
                                                                           Folio = AuxiliaresDetalle.Folio,
                                                                           Comprobante = Voucher.Tipo.ToString() + "   " + Voucher.NumeroVoucher.ToString() + "   " + Detalle.Auxiliar.LineaNumeroDetalle.ToString(),
                                                                           Documento = AuxiliaresDetalle.TipoDocumento,
                                                                           TipoAux = AuxiliaresDetalle.Individuo2.tipoReceptor,
                                                                           DebeAnalisis = Detalle.MontoDebe,
                                                                           HaberAnalisis = Detalle.MontoHaber,
                                                                           CuentaContable = Detalle.ObjCuentaContable,
                                                                           MontoTotal = AuxiliaresDetalle.MontoTotalLinea,
                                                                       });

        return LstCtaCorriente;
    }

    public static List<ObjetoCtasCtesPorConciliar> OrdenarListaCtasCtes(List<EstCtasCtesConciliadasViewModel> ListaPorConciliar)
    {
        var AgruparPorRutYCodigo = ListaPorConciliar.GroupBy(x => new { x.CuentaContable.CodInterno, x.RutPrestador })
                                                    .Select(grp => new CuentasCorrientesPorConciliar
                                                    {
                                                        Rut = grp.Key.RutPrestador,
                                                        CodInterno = grp.Key.CodInterno,
                                                        Contenido = grp.ToList(),
                                                        TotalDebe = grp.Sum(y => Math.Abs(y.Debe)),
                                                        TotalHaber = grp.Sum(y => Math.Abs(y.Haber)),
                                                        SaldoRut = grp.Sum(y => Math.Abs(y.Haber)) - grp.Sum(y => Math.Abs(y.Debe))
                                                    }).ToList();

        var ListaOrdenada = AgruparPorRutYCodigo.GroupBy(x => new { x.CodInterno })
                                                .Select(grp => new ObjetoCtasCtesPorConciliar
                                                {
                                                    CodigoInterno = grp.Key.CodInterno,
                                                    Contenido = grp.ToList(),
                                                    TotalDebe = grp.Sum(y => Math.Abs(y.TotalDebe)),
                                                    TotalHaber = grp.Sum(y => Math.Abs(y.TotalHaber)),
                                                    TotalSaldo = grp.Sum(y => Math.Abs(y.TotalHaber)) - grp.Sum(y => Math.Abs(y.TotalDebe)),
                                                }).ToList();
        return ListaOrdenada;
    }

    public static List<ObjetoCtasCtesPorConciliar> OrdenarListaCtasCtes(IQueryable<EstCtasCtesConciliadasViewModel> ListaCtaCteCompleta)
    {
        var AgruparPorRutYCodigo = ListaCtaCteCompleta.GroupBy(x => new { x.CuentaContable.CodInterno, x.RutPrestador })
                                                      .Select(grp => new CuentasCorrientesPorConciliar
                                                      {
                                                          Rut = grp.Key.RutPrestador,
                                                          CodInterno = grp.Key.CodInterno,
                                                          Contenido = grp.ToList(),
                                                          TotalDebe = grp.Sum(y => Math.Abs(y.Debe)),
                                                          TotalHaber = grp.Sum(y => Math.Abs(y.Haber)),
                                                          SaldoRut = grp.Sum(y => Math.Abs(y.Haber)) - grp.Sum(y => Math.Abs(y.Debe))
                                                      }).ToList();

        var ListaOrdenada = AgruparPorRutYCodigo.GroupBy(x => new { x.CodInterno })
                                                .Select(grp => new ObjetoCtasCtesPorConciliar
                                                {
                                                    CodigoInterno = grp.Key.CodInterno,
                                                    Contenido = grp.ToList(),
                                                    TotalDebe = grp.Sum(y => Math.Abs(y.TotalDebe)),
                                                    TotalHaber = grp.Sum(y => Math.Abs(y.TotalHaber)),
                                                    TotalSaldo = grp.Sum(y => Math.Abs(y.TotalHaber)) - grp.Sum(y => Math.Abs(y.TotalDebe)),
                                                }).ToList();
        return ListaOrdenada;
    }


    public static List<ObjetoCtasCtesPorConciliar> CalcularAcumulados(List<ObjetoCtasCtesPorConciliar> ListaOrdenada,IQueryable<EstCtasCtesConciliadasViewModel> ListaCtaCteCompleta,FacturaPoliContext db, ClientesContablesModel objCliente,FiltrosEstadoCtasCorrientes Filtros)
    {
        var FiltrarConciliados = CalcularYConciliarLista(db, objCliente,ListaCtaCteCompleta,Filtros).ToList();
        var ListaCompletaOrdenada = OrdenarListaCtasCtes(FiltrarConciliados).ToList();

        var SaldosYRutLstTodosLosAnios = ListaCompletaOrdenada.SelectMany(x => x.Contenido).Select(y => new { Rut = y.Rut, SaldoRut = y.SaldoRut, CtaCont = y.CodInterno, Contenido = y.Contenido }).ToList();
        SaldosYRutLstTodosLosAnios.RemoveAll(x => Convert.ToBoolean(x.Contenido.RemoveAll(y => y.Fecha.Year > Filtros.Anio)));

        //Mejorar en el futuro: hacerlo con linq
        foreach (var CtaContable in ListaOrdenada)
        {
            decimal TotalSaldoCtaContable = 0;
            foreach (var CtaCte in CtaContable.Contenido)
            {
                foreach (var ItemTodosLosAnios in SaldosYRutLstTodosLosAnios)
                {
                    if(CtaCte.Rut == ItemTodosLosAnios.Rut && CtaCte.CodInterno == ItemTodosLosAnios.CtaCont)
                    {
                        CtaCte.SaldoAcumuladoRut = Math.Abs(CtaCte.SaldoRut) - Math.Abs(ItemTodosLosAnios.SaldoRut);
                        TotalSaldoCtaContable += CtaCte.SaldoAcumuladoRut;
                    }
                }
            }
            CtaContable.TotalSaldoAcumulado = TotalSaldoCtaContable;
        }

        return ListaOrdenada;
    }

    public static decimal CalcularAcumuladosGenerales(List<ObjetoCtasCtesPorConciliar> ListaOrdenada, IQueryable<EstCtasCtesConciliadasViewModel> ListaCtaCteCompleta, FiltrosEstadoCtasCorrientes Filtros)
    {
        decimal TotalDebe = 0;
        decimal TotalHaber = 0;
        decimal TotalGeneralTodosLosAnios = 0;
        decimal TotalGeneralAnioConsultado = 0;
        decimal TotalAcumulado = 0;

        if(Filtros.TodosLosAnios == null) {

            var QueryASumar = ListaCtaCteCompleta.Where(x => x.Fecha.Year < Filtros.Anio).ToList();

            if(QueryASumar != null) {
                TotalDebe = QueryASumar.Sum(y => y.DebeAnalisis);
                TotalHaber = QueryASumar.Sum(x => x.HaberAnalisis);
            }
         
            TotalGeneralTodosLosAnios = Math.Abs(TotalHaber) - Math.Abs(TotalDebe);
            TotalGeneralAnioConsultado = ListaOrdenada.Sum(x => x.TotalSaldo);

            TotalAcumulado = TotalGeneralTodosLosAnios - TotalGeneralAnioConsultado;
        }
        else if (!string.IsNullOrWhiteSpace(Filtros.FechaInicio) && !string.IsNullOrWhiteSpace(Filtros.FechaFin))
        {
            //Query de esta busqueda
            DateTime dtFechaInicio = ParseExtensions.ToDD_MM_AAAA_Multi(Filtros.FechaInicio);
            var QueryASumar = ListaCtaCteCompleta.Where(x => x.Fecha < dtFechaInicio).ToList();

            if (QueryASumar != null)
            {
                TotalDebe = QueryASumar.Sum(y => y.DebeAnalisis);
                TotalHaber = QueryASumar.Sum(x => x.HaberAnalisis);
            }
        }

        else if(Filtros.TodosLosAnios == "on")
        {
            TotalAcumulado = 0;
        }

        return TotalAcumulado;
    }

    public static IQueryable<EstCtasCtesConciliadasViewModel> FiltrosCtaCorriente(IQueryable<EstCtasCtesConciliadasViewModel> LstCtaCorriente, FiltrosEstadoCtasCorrientes Filtros)
    {
        if (Filtros != null)
        {
            if (Filtros.Anio > 0 && Filtros.TodosLosAnios == null)
            {
                LstCtaCorriente = LstCtaCorriente.Where(anio => anio.Fecha.Year == Filtros.Anio);
            }

            if(Filtros.TodosLosAnios == "on")
            {
                Filtros.Anio = 0;
            }

            if (Filtros.Mes > 0)
            {
                LstCtaCorriente = LstCtaCorriente.Where(mes => mes.Fecha.Month == Filtros.Mes);
            }
            if (Filtros.CuentaAuxiliar > 0)
            {
                LstCtaCorriente = LstCtaCorriente.Where(cta => cta.CuentaContable.CuentaContableModelID == Filtros.CuentaAuxiliar);
            }
            if (!string.IsNullOrWhiteSpace(Filtros.RazonSocial))
            {
                LstCtaCorriente = LstCtaCorriente.Where(razonsocial => razonsocial.NombrePrestador.Contains(Filtros.RazonSocial));
            }
            if (!string.IsNullOrWhiteSpace(Filtros.Rut))
            {
                LstCtaCorriente = LstCtaCorriente.Where(rut => rut.RutPrestador.Contains(Filtros.Rut));
            }
            if (!string.IsNullOrWhiteSpace(Filtros.FechaInicio) && !string.IsNullOrWhiteSpace(Filtros.FechaFin))
            {
                DateTime dtFechaInicio = ParseExtensions.ToDD_MM_AAAA_Multi(Filtros.FechaInicio);
                DateTime dtFechaFin = ParseExtensions.ToDD_MM_AAAA_Multi(Filtros.FechaFin);
                LstCtaCorriente = LstCtaCorriente.Where(fecha => fecha.Fecha >= dtFechaInicio && fecha.Fecha <= dtFechaFin);
            }
        }

        return LstCtaCorriente;
    }

    public static List<EstCtasCtesConciliadasViewModel> CalcularYConciliarLista(FacturaPoliContext db, ClientesContablesModel ObjCliente, IQueryable<EstCtasCtesConciliadasViewModel> LstCtaCorriente, FiltrosEstadoCtasCorrientes Filtros)
        {
                List<EstCtasCtesConciliadasViewModel> lstCtasCorrientes = new List<EstCtasCtesConciliadasViewModel>(LstCtaCorriente);
                List<EstCtasCtesConciliadasViewModel> ListaConciliada = new List<EstCtasCtesConciliadasViewModel>();

                if (LstCtaCorriente.Count() > 0)
                {

                    string AuxCompra = "PR";
                    string AuxVenta = "CL";
                    string AuxHonorario = "H";
                    string AuxPersona = "P";

                    foreach (var ItemAanalizar in lstCtasCorrientes)
                    {

                        if(ItemAanalizar.TipoAux == AuxCompra)
                        {
                            if(ItemAanalizar.DebeAnalisis > 0)
                                ItemAanalizar.Debe = ItemAanalizar.MontoTotal;

                            if (ItemAanalizar.HaberAnalisis > 0)
                                ItemAanalizar.Haber = ItemAanalizar.MontoTotal;
                        }
                        if (ItemAanalizar.TipoAux == AuxVenta)
                        {
                            if (ItemAanalizar.DebeAnalisis > 0)
                                ItemAanalizar.Debe = ItemAanalizar.MontoTotal;
                        
                            if (ItemAanalizar.HaberAnalisis > 0)
                                ItemAanalizar.Haber = ItemAanalizar.MontoTotal;
                        }
                        if (ItemAanalizar.TipoAux == AuxHonorario)
                        {
                            if (ItemAanalizar.DebeAnalisis > 0)
                                ItemAanalizar.Debe = ItemAanalizar.MontoTotal;

                            if (ItemAanalizar.HaberAnalisis > 0)
                                ItemAanalizar.Haber = ItemAanalizar.MontoTotal;
                        }
                        if (ItemAanalizar.TipoAux == AuxPersona)
                        {
                            if (ItemAanalizar.DebeAnalisis > 0)
                                ItemAanalizar.Debe = ItemAanalizar.MontoTotal;

                            if (ItemAanalizar.HaberAnalisis > 0)
                                ItemAanalizar.Haber = ItemAanalizar.MontoTotal;
                        }
                    }

              

                    var AyudaParaAnalizar = lstCtasCorrientes.Select(x => new { x.Folio, x.RutPrestador, x.Documento, x.CuentaContable.CuentaContableModelID }).Distinct().ToList();

                    foreach (var PosibleConciliado in AyudaParaAnalizar)
                    {
                       var PosiblesAConciliar = lstCtasCorrientes.Where(busca => busca.Folio == PosibleConciliado.Folio &&
                                                                        busca.RutPrestador == PosibleConciliado.RutPrestador &&
                                                                        busca.Documento == PosibleConciliado.Documento &&
                                                                        busca.CuentaContable.CuentaContableModelID == PosibleConciliado.CuentaContableModelID).ToList();

                        if(PosiblesAConciliar.Count() > 1) {
                            decimal Haber = 0;
                            decimal Debe = 0;
                            decimal Resultado = 0;
                    
                            foreach (var VerificarSiSeConcilia in PosiblesAConciliar)
                            {
                                if(VerificarSiSeConcilia.Debe > 0)
                                {
                                    Debe += VerificarSiSeConcilia.MontoTotal;
                                }
                                if(VerificarSiSeConcilia.Haber > 0)
                                {
                                    Haber += VerificarSiSeConcilia.MontoTotal;
                                }
                        
                            }

                            Resultado = Math.Abs(Haber) - Math.Abs(Debe);
                            if(Resultado == 0)
                            {
                                foreach (var ItemConciliado in PosiblesAConciliar)
                                {
                                    if (Filtros.TipoListaAmostrar == 2)
                                        lstCtasCorrientes.Remove(ItemConciliado);
                                    else if (Filtros.TipoListaAmostrar == 1)
                                        ListaConciliada.Add(ItemConciliado);
                                    else if (Filtros.TipoListaAmostrar == 0)
                                        ItemConciliado.EstaConciliado = true;
                                }
                            }
                    
                }
            }
        }
        return ListaConciliada.Count() > 0 && Filtros.TipoListaAmostrar == 1 ? ListaConciliada : lstCtasCorrientes; 
        }

        
    }
    
    //Este será el objeto padre que hay que llenar para obtener toda la vista de cuentas corrientes por conciliar
    public class ObjetoCtasCtesPorConciliar
    {
        public string CodigoInterno { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public decimal TotalSaldo { get; set; }
        public decimal TotalSaldoAcumulado { get; set; }
        public List<CuentasCorrientesPorConciliar> Contenido { get; set; }
    }

    public class CuentasCorrientesPorConciliar
    {
        public string Rut { get; set; }
        public string CodInterno { get; set; }
        public decimal SaldoAcumuladoRut { get; set; }
        public decimal SaldoAcumuladoCuentaContable { get; set; }
        public List<EstCtasCtesConciliadasViewModel> Contenido { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public decimal SaldoRut { get; set; }
    }

    public class AcumuladosCtasCtesConciliadas
    {
        public decimal AcumuladoGeneral { get; set; }
        public decimal AcumuladoAuxRut { get; set; }
        public decimal AcumuladoAuxCtaCont { get; set; }
    }

