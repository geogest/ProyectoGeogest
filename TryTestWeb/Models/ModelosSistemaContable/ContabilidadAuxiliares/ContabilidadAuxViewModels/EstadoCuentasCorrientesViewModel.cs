﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Routing;

public class EstadoCuentasCorrientesViewModel
    {
        public string RutPrestador { get; set; }
        public string NombrePrestador { get; set; }
        public CuentaContableModel CuentaContable { get; set; }
        public DateTime Fecha { get; set; }
        public int Folio { get; set; }  
        public string Comprobante { get; set; }
        public TipoDte Documento { get; set; }
        public DateTime Vencim { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Saldo { get; set; }




        public static IQueryable<EstadoCuentasCorrientesViewModel> GetLstCtaCorriente(FacturaPoliContext db, ClientesContablesModel objCliente)
        {

        IQueryable<EstadoCuentasCorrientesViewModel> LstCtaCorriente = (from Detalle in db.DBDetalleVoucher
                                                                          join Voucher in db.DBVoucher on Detalle.VoucherModelID equals Voucher.VoucherModelID
                                                                          join Auxiliar in db.DBAuxiliares on Detalle.Auxiliar.AuxiliaresModelID equals Auxiliar.AuxiliaresModelID
                                                                          where Auxiliar.objCtaContable.ClientesContablesModelID == objCliente.ClientesContablesModelID &&
                                                                                Voucher.DadoDeBaja == false &&
                                                                                Auxiliar.objCtaContable.TieneAuxiliar == 1
                                                                          select new EstadoCuentasCorrientesViewModel
                                                                          {
                                                                              RutPrestador = Auxiliar.ListaDetalleAuxiliares.FirstOrDefault().Individuo2.RUT,
                                                                              NombrePrestador = Auxiliar.ListaDetalleAuxiliares.FirstOrDefault().Individuo2.RazonSocial,
                                                                              Fecha = Detalle.FechaDoc,
                                                                              Folio = Auxiliar.ListaDetalleAuxiliares.FirstOrDefault().Folio,
                                                                              Comprobante = Voucher.Tipo.ToString() + " " + Voucher.NumeroVoucher.ToString() + " " + Detalle.Auxiliar.LineaNumeroDetalle.ToString(),
                                                                              Documento = Auxiliar.ListaDetalleAuxiliares.FirstOrDefault().TipoDocumento,
                                                                              Debe = Detalle.MontoDebe,
                                                                              Haber = Detalle.MontoHaber,
                                                                              Saldo = 0,
                                                                              CuentaContable = Detalle.ObjCuentaContable
                                                                          });
        return LstCtaCorriente;

    }

    public static IQueryable<EstadoCuentasCorrientesViewModel> FiltrosCtaCorriente(IQueryable<EstadoCuentasCorrientesViewModel> LstCtaCorriente, FiltrosEstadoCtasCorrientes Filtros)
    {
        if (Filtros != null)
        {
            if (Filtros.Anio > 0)
            {
                LstCtaCorriente = LstCtaCorriente.Where(anio => anio.Fecha.Year == Filtros.Anio);
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

    public static PaginadorModel PaginacionCtasCorrientes(IQueryable<EstadoCuentasCorrientesViewModel> LstCtaCorriente, FiltrosEstadoCtasCorrientes Filtros)
    {

        int TotalRegistros = LstCtaCorriente.ToList().Count();

        if (Filtros.cantidadRegistrosPorPagina > 0)
        {
            LstCtaCorriente = LstCtaCorriente.OrderBy(cta => cta.CuentaContable.Clasificacion)
                                             .Skip((Filtros.pagina - 1) * Filtros.cantidadRegistrosPorPagina)
                                             .Take(Filtros.cantidadRegistrosPorPagina);
        }else if(Filtros.cantidadRegistrosPorPagina == 0)
        {
            LstCtaCorriente = LstCtaCorriente.OrderBy(cta => cta.CuentaContable.Clasificacion);
        }

        var Paginacion = new PaginadorModel();

        Paginacion.LstCtasCorrientes = LstCtaCorriente.ToList();
        Paginacion.PaginaActual = Filtros.pagina;
        Paginacion.TotalDeRegistros = TotalRegistros;
        Paginacion.RegistrosPorPagina = Filtros.cantidadRegistrosPorPagina;
        Paginacion.ValoresQueryString = new RouteValueDictionary();

        if (Filtros.cantidadRegistrosPorPagina != 25)
            Paginacion.ValoresQueryString["cantidadRegistrosPorPagina"] = Filtros.cantidadRegistrosPorPagina;

        if (Filtros.Anio > 0)
            Paginacion.ValoresQueryString["Anio"] = Filtros.Anio;

        if (Filtros.Mes > 0)
            Paginacion.ValoresQueryString["Mes"] = Filtros.Mes;

        if (Filtros.CuentaAuxiliar > 0)
            Paginacion.ValoresQueryString["CuentaAuxiliar"] = Filtros.CuentaAuxiliar;

        if (!string.IsNullOrWhiteSpace(Filtros.Rut))
            Paginacion.ValoresQueryString["Rut"] = Filtros.Rut;

        if (Filtros.FechaInicio != null && Filtros.FechaFin != null)
        {
            Paginacion.ValoresQueryString["FechaInicio"] = Filtros.FechaInicio;
            Paginacion.ValoresQueryString["FechaFin"] = Filtros.FechaFin;
        }

        if (!string.IsNullOrWhiteSpace(Filtros.RazonSocial))
            Paginacion.ValoresQueryString["RazonSocial"] = Filtros.RazonSocial;

        return Paginacion;

    }




    public static byte[] GetExcelCtaCorriente(List<EstadoCuentasCorrientesViewModel> lstCtaCorriente, ClientesContablesModel objCliente, bool InformarMembrete)
        {
  
            var UnicosEnLaLista = ParseExtensions.ObtieneLstAuxNombre(lstCtaCorriente);

            string RutaPlanillaLibroMayor = ParseExtensions.Get_AppData_Path("ESTADOCTASCORRIENTES.xlsx");

            byte[] ExcelByteArray = null;
            using (XLWorkbook excelFile = new XLWorkbook(RutaPlanillaLibroMayor))
            {
                var workSheet = excelFile.Worksheet(1);

                if (InformarMembrete == true)
                {
                    workSheet.Cell("A1").Value = objCliente.RazonSocial;
                    workSheet.Cell("A2").Value = objCliente.RUTEmpresa;
                    workSheet.Cell("A3").Value = objCliente.Giro;
                    workSheet.Cell("A4").Value = objCliente.Direccion;
                    workSheet.Cell("A5").Value = objCliente.Ciudad;
                    workSheet.Cell("A6").Value = objCliente.Representante;
                    workSheet.Cell("A7").Value = objCliente.RUTRepresentante;
                }
                else
                {
                    workSheet.Cell("A1").Value = string.Empty;
                    workSheet.Cell("A2").Value = string.Empty;
                    workSheet.Cell("A3").Value = string.Empty;
                    workSheet.Cell("A4").Value = string.Empty;
                    workSheet.Cell("A5").Value = string.Empty;
                    workSheet.Cell("A6").Value = string.Empty;
                    workSheet.Cell("A7").Value = string.Empty;
                }
                workSheet.Columns("C","F").Style.DateFormat.Format = "dd-MM-yyyy";
                workSheet.Columns("G:I").Style.NumberFormat.Format = "#,##0 ;-#,##0";

                decimal TotalHaber = 0;
                decimal TotalDebe = 0;
                decimal TotalSaldo = 0;

                int NumeroFilaExcel = 12;

                for (int i = 0; i < UnicosEnLaLista.Count(); i++)
                {
                    decimal Debe = 0;
                    decimal Haber = 0;
                    decimal Saldo = 0;

                    workSheet.Cell(NumeroFilaExcel, "A").Value = UnicosEnLaLista[i];
                    workSheet.Cell(NumeroFilaExcel, "A").Style.Font.Bold = true;
                    NumeroFilaExcel++;

                    workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows
                         ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows
                    ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    workSheet.Cell(NumeroFilaExcel, "A").Value = "RUT";
                    workSheet.Cell(NumeroFilaExcel, "B").Value = "NOMBRE";
                    workSheet.Cell(NumeroFilaExcel, "C").Value = "FECHA";
                    workSheet.Cell(NumeroFilaExcel, "D").Value = "COMPROBANTE";
                    workSheet.Cell(NumeroFilaExcel, "E").Value = "DOCUMENTO";
                    workSheet.Cell(NumeroFilaExcel, "F").Value = "VENCIMIENTO";
                    workSheet.Cell(NumeroFilaExcel, "G").Value = "DEBE";
                    workSheet.Cell(NumeroFilaExcel, "H").Value = "HABER";
                    workSheet.Cell(NumeroFilaExcel, "I").Value = "SALDO";

                    NumeroFilaExcel++;
    
                    foreach (var ItemCtaCte in lstCtaCorriente)
                    {

                        if (ItemCtaCte.CuentaContable.nombre == UnicosEnLaLista[i])
                        {

                            Debe += ItemCtaCte.Debe;
                            Haber += ItemCtaCte.Haber;
           
                            workSheet.Cell(NumeroFilaExcel, "A").Value = ItemCtaCte.RutPrestador;
                            workSheet.Cell(NumeroFilaExcel, "B").Value = ItemCtaCte.NombrePrestador;
                            workSheet.Cell(NumeroFilaExcel, "C").Value = ItemCtaCte.Fecha;
                            workSheet.Cell(NumeroFilaExcel, "D").Value = ItemCtaCte.Comprobante;
                            workSheet.Cell(NumeroFilaExcel, "E").Value = ItemCtaCte.Documento;
                            workSheet.Cell(NumeroFilaExcel, "F").Value = "";
                            workSheet.Cell(NumeroFilaExcel, "G").Value = ItemCtaCte.Debe;
                            workSheet.Cell(NumeroFilaExcel, "H").Value = ItemCtaCte.Haber;
                            workSheet.Cell(NumeroFilaExcel, "I").Value = ItemCtaCte.Saldo;


                            
                            workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows
                            ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows
                            ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                            NumeroFilaExcel++;
                        }
                    }
                    workSheet.Cell(NumeroFilaExcel, "F").Value = "TOTAL";
                    workSheet.Cell(NumeroFilaExcel, "F").Style.Font.Bold = true;
                    workSheet.Cell(NumeroFilaExcel, "G").Value = Debe;
                    workSheet.Cell(NumeroFilaExcel, "G").Style.Font.Bold = true;
                    workSheet.Cell(NumeroFilaExcel, "H").Value = Haber;
                    workSheet.Cell(NumeroFilaExcel, "H").Style.Font.Bold = true;
                
                    Saldo = Haber - Debe;

                    workSheet.Cell(NumeroFilaExcel, "I").Value = Math.Abs(Saldo);
                    workSheet.Cell(NumeroFilaExcel, "I").Style.Font.Bold = true;

                    workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows
                    ().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    workSheet.Range("A" + NumeroFilaExcel + ":I" + NumeroFilaExcel).Rows
                    ().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    TotalHaber += Haber;
                    TotalDebe += Debe;
                    TotalSaldo += Saldo;

                    NumeroFilaExcel += 2;
                }

            workSheet.Cell(NumeroFilaExcel, "F").Value = "TOTAL FINAL:";
            workSheet.Cell(NumeroFilaExcel, "F").Style.Font.Bold = true;
            workSheet.Cell(NumeroFilaExcel, "G").Value = TotalDebe;
            workSheet.Cell(NumeroFilaExcel, "G").Style.Font.Bold = true;
            workSheet.Cell(NumeroFilaExcel, "H").Value = TotalHaber;
            workSheet.Cell(NumeroFilaExcel, "H").Style.Font.Bold = true;
            workSheet.Cell(NumeroFilaExcel, "I").Value = TotalSaldo;
            workSheet.Cell(NumeroFilaExcel, "I").Style.Font.Bold = true;

            ExcelByteArray = ParseExtensions.GetExcelStream(excelFile);
            }
            if (ExcelByteArray == null)
                return null;
            else
                return ExcelByteArray;
        }
    
    }
