using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


public class CuentaContableModel
{
    //TODO : Agregar soporte <CENTRO DE COSTO / ANALISIS DE CUENTAS>
    public int CuentaContableModelID { get; set; }

    public int ClientesContablesModelID { get; set; }

    public string nombre { get; set; }

    public string CodInterno { get; set; }

    public virtual ClasificacionCtaContable Clasificacion { get; set; }

    public virtual SubClasificacionCtaContable SubClasificacion { get; set; }

    public virtual SubSubClasificacionCtaContable SubSubClasificacion { get; set; }

    public bool AnalisisDeCuenta { get; set; }

    public bool InvertirMontosDepreciacion { get; set; }

    public TipoAuxiliar TipoAuxiliarQueUtiliza { get; set; } = TipoAuxiliar.ProveedorDeudor;

    public TipoCentralizacion TipoCentralizacionAuxiliares { get; set; } = TipoCentralizacion.Ninguno;

    //public virtual ImpuestoILAModel ImpuestoILAModel { get; set; }

    /*
        NOMBRE: ALEX MARTINEZ
        FECHA: 17-12-2018 
        OBSERVACIONES: AGREGO CAMPOS PARA CONTROL DE SELECCIONES SEGUN EXCEL ENVIADO POR GARY 
    */
    public int ItemsModelID { get; set; }
    public int CentroCostosModelID { get; set; }
    /*
     1. Cliente 
     2. Proveedor 

     */
    public int AnalisisContablesModelID { get; set; }

    public int Concilaciones { get; set; }

    public int TieneAuxiliar { get; set; }
    public int TieneCentroDeCosto { get; set; }

   // public decimal Presupuesto { get; set; } // Revisar como se controlará este nuevo atributo.



    //TODO : Eliminar mas adelante para manejar la creacion apropiada de los casos que correspondan
    public CuentaContableModel()
    {

    }

    public CuentaContableModel(int _ClientesContablesModelID, ClasificacionCtaContable _Clasificacion, SubClasificacionCtaContable _SubClasificacion, SubSubClasificacionCtaContable _SubSubClasificacion, string _CodInterno, string _Nombre, bool _analisisDeCuenta = false, TipoAuxiliar TipoAuxiliar = TipoAuxiliar.ProveedorDeudor, bool _invertirMontosDepreciacion = false, TipoCentralizacion _tipoAuxiliarCentralizacion = TipoCentralizacion.Ninguno, int _ItemsModelID  =  0, int _CentroCostosModelID = 0 , int _AnalisisContablesModelID = 0, int _Concilaciones = 0, int _TieneAuxiliar = 0 )
    {
        ClientesContablesModelID = _ClientesContablesModelID;
        Clasificacion = _Clasificacion;
        SubClasificacion = _SubClasificacion;
        SubSubClasificacion = _SubSubClasificacion;
        CodInterno = _CodInterno;
        nombre = _Nombre;
        AnalisisDeCuenta = _analisisDeCuenta;
        InvertirMontosDepreciacion = _invertirMontosDepreciacion;
        TipoAuxiliarQueUtiliza = TipoAuxiliar;
        TipoCentralizacionAuxiliares = _tipoAuxiliarCentralizacion;
        ItemsModelID = _ItemsModelID;
        CentroCostosModelID = _CentroCostosModelID;
        AnalisisContablesModelID = _AnalisisContablesModelID;
        Concilaciones = _Concilaciones;
        TieneAuxiliar = _TieneAuxiliar;

    }

  

    public string GetCtaContableDisplayName()
    {
        return CodInterno + " " + nombre;
    }

    public string GetClasificacionDisplaySTR()
    {
        return (int)Clasificacion + " " + ParseExtensions.EnumGetDisplayAttrib(Clasificacion);
    }

    public string GetSubClasificacionName()
    {
        if (SubClasificacion != null)
        {
            if (!string.IsNullOrWhiteSpace(SubClasificacion.NombreInterno))
                return SubClasificacion.NombreInterno;
        }
        return string.Empty;
    }
    public string GetSubSubClasificacionName()
    {
        if (SubSubClasificacion != null)
        {
            if (!string.IsNullOrWhiteSpace(SubSubClasificacion.NombreInterno))
                return SubSubClasificacion.NombreInterno;
        }
        return string.Empty;
    }

    public string GetSubClasificacionDisplaySTRLong()
    {
        return SubClasificacion.CodigoInterno + " " + SubClasificacion.NombreInterno;
    }



    public decimal GetPresupuesto(string UserID,int idCtaCont)
    {
        FacturaPoliContext db = ParseExtensions.GetDatabaseContext(UserID);
        CtasContablesPresupuestoModel lstCtasPresupuesto = db.DBCCPresupuesto.SingleOrDefault(r => r.CuentasContablesModelID == idCtaCont);

         decimal Presupuesto = 0;
        if(lstCtasPresupuesto !=  null)
        {
             Presupuesto = lstCtasPresupuesto.Presupuesto;
        }
    
        return Presupuesto;
    }

    public string GetSubSubClasificacionDisplaySTRLong()
    {
        return SubSubClasificacion.CodigoInterno + " " + SubSubClasificacion.NombreInterno;
    }

    //nuevo plan basico
    public static List<CuentaContableModel> ListNuevaCuentaContableBasica(int clienteID, FacturaPoliContext db)
    {
        //LISTA DE SUBCLASIFICACION
        List<SubClasificacionCtaContable> sc_Base = new List<SubClasificacionCtaContable>();
        //LISTA DE SUBSUBCLASIFICACION
        List<SubSubClasificacionCtaContable> ssc_Base = new List<SubSubClasificacionCtaContable>();

        #region CREACION SUBCLASIFICACION BASE
        SubClasificacionCtaContable sc11ActivoCirculanteOActivo = new SubClasificacionCtaContable(clienteID, "11", "ACTIVO CIRCULANTE O ACTIVO");
        sc_Base.Add(sc11ActivoCirculanteOActivo);
        SubClasificacionCtaContable sc12ActivoFijoONoCorriente = new SubClasificacionCtaContable(clienteID, "12", "ACTIVO FIJO O ACTIVO NO CORRIENTE");
        sc_Base.Add(sc12ActivoFijoONoCorriente);

        SubClasificacionCtaContable sc22PasivoCirculanteOCorriente = new SubClasificacionCtaContable(clienteID, "22", "PASIVO CIRCULANTE O PASIVO CORRIENTE");
        sc_Base.Add(sc22PasivoCirculanteOCorriente);

        SubClasificacionCtaContable sc23PasivoNoCirculanteOLarzoPlazoNoCorriente = new SubClasificacionCtaContable(clienteID, "23", "PASIVO NO CIRCULANTE O PASIVO LARGO PLAZO NO CORRIENTE");
        sc_Base.Add(sc23PasivoNoCirculanteOLarzoPlazoNoCorriente);
        SubClasificacionCtaContable sc24PatrimonioNeto = new SubClasificacionCtaContable(clienteID, "24", "PATRIMONIO NETO");
        sc_Base.Add(sc24PatrimonioNeto);

        SubClasificacionCtaContable sc41CostoDeExplotacion = new SubClasificacionCtaContable(clienteID, "41", "COSTO DE EXPLOTACION");
        sc_Base.Add(sc41CostoDeExplotacion);

        SubClasificacionCtaContable sc42GastosPorIntereses = new SubClasificacionCtaContable(clienteID, "42", "GASTOS POR INTERESES");
        sc_Base.Add(sc42GastosPorIntereses);

        SubClasificacionCtaContable sc51VentaNeta = new SubClasificacionCtaContable(clienteID, "51", "VENTA NETA");
        sc_Base.Add(sc51VentaNeta);

        SubClasificacionCtaContable sc52IngresosGastosPorIntereses = new SubClasificacionCtaContable(clienteID, "52", "INGRESOS POR INTERESES");
        sc_Base.Add(sc52IngresosGastosPorIntereses);

        db.DBSubClasificacion.AddRange(sc_Base);
        db.SaveChanges();

        #endregion

        #region CREACION SUBSUBCLASIFICACION BASE
        SubSubClasificacionCtaContable ssc1101DisponibleOEfectivoYEquivalentes = new SubSubClasificacionCtaContable(clienteID, "1101", "DISPONIBLE O EFECTIVO Y EQUIVALENTES");
        SubSubClasificacionCtaContable ssc1102OtrosActivosCirculantesOFinancierosCorriente = new SubSubClasificacionCtaContable(clienteID, "1102", "OTROS ACTIVOS CIRCULANTES O ACTIVOS FINANCIEROS CORRIENTES");
        SubSubClasificacionCtaContable ssc1103ValoresNegociables = new SubSubClasificacionCtaContable(clienteID, "1103", "VALORES NEGOCIABLES");
        SubSubClasificacionCtaContable ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes = new SubSubClasificacionCtaContable(clienteID, "1104", "DEUDORES COMERCIALES Y OTRAS CUENTAS POR COBRAR CORRIENTES");
        SubSubClasificacionCtaContable ssc1105Inventario = new SubSubClasificacionCtaContable(clienteID, "1105", "INVENTARIO");
        SubSubClasificacionCtaContable ssc1106Anticipos = new SubSubClasificacionCtaContable(clienteID, "1106", "ANTICIPOS");
        SubSubClasificacionCtaContable ssc1107ActivosPorImpuestosCorrientes = new SubSubClasificacionCtaContable(clienteID, "1107", "ACTIVOS POR IMPUESTOS CORRIENTES");


        SubSubClasificacionCtaContable ssc1201PropiedadesPlataYEquipos = new SubSubClasificacionCtaContable(clienteID, "1201", "PROPIEDADES, PLANTA Y EQUIPOS");
        SubSubClasificacionCtaContable ssc1202DepAcumuladaPropPlantesYEquipos = new SubSubClasificacionCtaContable(clienteID, "1202", "DEP. ACUMULADA PROP PLANTES Y EQUIPOS");
        SubSubClasificacionCtaContable ssc1203ActivosIntangibles = new SubSubClasificacionCtaContable(clienteID, "1203", "ACTIVOS INTANGIBLES");
        SubSubClasificacionCtaContable ssc1204DepAcumuladaSoftwareEIntangibles = new SubSubClasificacionCtaContable(clienteID, "1204", "DEP. ACUMULADA SOFTWARE E INTANGIBLES");
        SubSubClasificacionCtaContable ssc1205ActivosPorIMpuestosDiferidos = new SubSubClasificacionCtaContable(clienteID, "1205", "ACTIVOS POR IMPUESTOS DIFERIDOS");

        SubSubClasificacionCtaContable ssc2201ProveedoresPorPagar = new SubSubClasificacionCtaContable(clienteID, "2201", "PROVEEDORES POR PAGAR");
        SubSubClasificacionCtaContable ssc2202DeudasDeCortoPlazo = new SubSubClasificacionCtaContable(clienteID, "2202", "DEUDAS DE CORTO PLAZO");
        SubSubClasificacionCtaContable ssc2203ProvisionesCortoPlazo = new SubSubClasificacionCtaContable(clienteID, "2203", "PROVISIONES CORTO PLAZO");
        SubSubClasificacionCtaContable ssc2204ImpuestosPorPagar = new SubSubClasificacionCtaContable(clienteID, "2204", "IMPUESTOS POR PAGAR");
        SubSubClasificacionCtaContable ssc2205OtrasCuentasPorPagarCorrientes = new SubSubClasificacionCtaContable(clienteID, "2205", "OTRAS CUENTAS POR PAGAR CORRIENTES");

        SubSubClasificacionCtaContable ssc2301DeudasDeLargoPlazo = new SubSubClasificacionCtaContable(clienteID, "2301", "DEUDAS DE LARGO PLAZO");
        SubSubClasificacionCtaContable ssc2302ProvisionesLargoPlazo = new SubSubClasificacionCtaContable(clienteID, "2302", "PROVISIONES LARGO PLAZO");
        SubSubClasificacionCtaContable ssc2303ImpuestosPorPagar = new SubSubClasificacionCtaContable(clienteID, "2303", "IMPUESTOS POR PAGAR");

        SubSubClasificacionCtaContable ssc2304OtrascCuentasPorPagarNoCorrientes = new SubSubClasificacionCtaContable(clienteID, "2304", "OTRAS CUENTAS POR PAGAR NO CORRIENTES");

        SubSubClasificacionCtaContable ssc2401Capital = new SubSubClasificacionCtaContable(clienteID, "2401", "CAPITAL");

        SubSubClasificacionCtaContable ssc4101CostosDeLaMercaderiaVendida = new SubSubClasificacionCtaContable(clienteID, "4101", "COSTOS DE MERCADERIA VENDIDA");
        SubSubClasificacionCtaContable ssc4102GastosDeProduccion = new SubSubClasificacionCtaContable(clienteID, "4102", "GASTOS DE PRODUCCION");
        SubSubClasificacionCtaContable ssc4103GastosDeDistribucion = new SubSubClasificacionCtaContable(clienteID, "4103", "GASTOS DE DISTRIBUCION");
        SubSubClasificacionCtaContable ssc4104GastosDeComercializacion = new SubSubClasificacionCtaContable(clienteID, "4104", "GASTOS DE COMERCIALIZACION");
        SubSubClasificacionCtaContable ssc4105GastosDeMarketing = new SubSubClasificacionCtaContable(clienteID, "4105", "GASTOS DE MARKETING");
        SubSubClasificacionCtaContable ssc4106AjustesDeInventario = new SubSubClasificacionCtaContable(clienteID, "4106", "AJUSTES DE INVENTARIO");
        SubSubClasificacionCtaContable ssc4107RecursosHumanos = new SubSubClasificacionCtaContable(clienteID, "4107", "RECURSOS HUMANOS");
        SubSubClasificacionCtaContable ssc4108GastosLegales = new SubSubClasificacionCtaContable(clienteID, "4108", "GASTOS LEGALES");
        SubSubClasificacionCtaContable ssc4109Asesorias = new SubSubClasificacionCtaContable(clienteID, "4109", "ASESORIAS");
        SubSubClasificacionCtaContable ssc4110TI = new SubSubClasificacionCtaContable(clienteID, "4110", "TI");
        SubSubClasificacionCtaContable ssc4111GastosDeEquipamientoYOficina = new SubSubClasificacionCtaContable(clienteID, "4111", "GASTOS DE EQUIPAMIENTO Y OFICINA");
        SubSubClasificacionCtaContable ssc4112DepreciacionYAmortizacion = new SubSubClasificacionCtaContable(clienteID, "4112", "DEPRECIACION Y AMORTIZACION");
        SubSubClasificacionCtaContable ssc4113Impuestos = new SubSubClasificacionCtaContable(clienteID, "4113", "IMPUESTOS");

        SubSubClasificacionCtaContable ssc4201OtrosGastos = new SubSubClasificacionCtaContable(clienteID, "4201", "OTROS GASTOS");

        SubSubClasificacionCtaContable ssc5101IngresosPorVenta = new SubSubClasificacionCtaContable(clienteID, "5101", "INGRESOS POR VENTA");

        SubSubClasificacionCtaContable ssc5201IngresosGastosPorIntereses = new SubSubClasificacionCtaContable(clienteID, "5201", "INGRESOS/GASTOS POR INTERES");

        db.DBSubSubClasificacion.AddRange(ssc_Base);
        db.SaveChanges();
        #endregion

        #region CREACION CUENTAS CONTABLES BASE
        List<CuentaContableModel> CtaContableBase = new List<CuentaContableModel>();
        //1. ACTIVOS / 11.Activo Circulante o activo  / 1101.Disponible o Efectivo y equivalentes
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1101DisponibleOEfectivoYEquivalentes, "110101", "CAJA"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1101DisponibleOEfectivoYEquivalentes, "110102", "CAJA CHICA"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1101DisponibleOEfectivoYEquivalentes, "110103", "FONDOS POR RENDIR", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0,0, 1, 0, 1 ));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1101DisponibleOEfectivoYEquivalentes, "110104", "BANCOS CLP", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 0, 1, 0  ));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1101DisponibleOEfectivoYEquivalentes, "110105", "BANCOS USD", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 0, 1, 0));
        //1. ACTIVOS / 11.Activo Circulante o activo / 1102.Otros activos  Circulantes o Activos financieros corrientes                             null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1102OtrosActivosCirculantesOFinancierosCorriente, "110201", "DEPOSITOS A PLAZO"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1102OtrosActivosCirculantesOFinancierosCorriente, "110202", "FONDOS MUTUOS"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1102OtrosActivosCirculantesOFinancierosCorriente, "110203", "BOLETAS DE GARANTIA"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1102OtrosActivosCirculantesOFinancierosCorriente, "110204", "GARANTIA DE ARRIENDO"));
        //1. ACTIVOS / 11.Activo Circulante o activo / 1103.VALORES NEGOCIABLES                           null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1103ValoresNegociables, "110301", "ACCIONES"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1103ValoresNegociables, "110302", "ACCIONISTAS"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1103ValoresNegociables, "110303", "APORTES POR ENTERAR"));
        //1. ACTIVOS / 11.Activo Circulante o activo / 1104.Deudores comerciales y otras Cuentas por Cobrar corrientes                       null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes, "110401", "DEUDORES POR VENTA - NACIONALES", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0,  0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes, "110402", "DEUDORES POR VENTA - EXTRANJEROS", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes, "110403", "DEUDORES VARIOS", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Venta, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes, "110404", "DOCUMENTOS POR COBRAR", true));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes, "110405", "TRANSBANK POR COBRAR", false));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes, "110406", "DEUDORES INCOBRABLES", false));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes, "110407", "CORFO A RENDIR", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 0, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1104DeudoresComercialesYOtrasCuentasPorCobrarCorrientes, "110408", "RETIRO O DIVIDENDO SOCIO O ACCIONISTA", true));
        //1. ACTIVOS / 11.Activo Circulante o activo / 1105.INVENTARIO                             null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1105Inventario, "110501", "PRODUCTO TERMINADO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1105Inventario, "110502", "MATERIA PRIMA(F&V)", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1105Inventario, "110503", "INVENTARIO DE XXX", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1105Inventario, "110504", "ETIQUETAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1105Inventario, "110505", "IMPORTACIONES EN TRANSITO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1105Inventario, "110506", "ELEMENTOS MARKETING", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1105Inventario, "110507", "MATERIAL DE EMBALAJE", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1105Inventario, "110508", "MATERIALES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra, 0, 1, 0, 0, 0));
        //1. ACTIVOS / 11.Activo Circulante o activo / 1106.ANTICIPOS                               null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1106Anticipos, "110601", "ANTICIPO PROVEEDORES", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1106Anticipos, "110602", "PROVISION IMPORTAC.AGENCIA", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1106Anticipos, "110603", "ANTICIPOS BROKERS FINANCIEROS", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1106Anticipos, "110604", "ANTICIPOS AL PERSONAL", true, TipoAuxiliar.Honorarios, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1106Anticipos, "110605", "PRESTAMOS AL PERSONAL", true, TipoAuxiliar.Honorarios, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        //1. ACTIVOS / 11.Activo Circulante o activo / 1107.ANTIVOS POR IMPUESTOS CORRIENTES                          null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1107ActivosPorImpuestosCorrientes, "110701", "IVA CREDITO FISCAL", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1107ActivosPorImpuestosCorrientes, "110702", "PPM"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc11ActivoCirculanteOActivo, ssc1107ActivosPorImpuestosCorrientes, "110703", "OTROS IMPUESTOS"));

        //1. ACTIVOS / 12.ACTIVO FIJO O ACTIVO NO CORRIENTE / 1201.PROPIEDADES, PLANTA Y EQUIPOS                                       null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120101", "EQUIPOS DE PRODUCCION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120102", "EQUIPOS DE REFRIGERACION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120103", "EQUIPOS DE REFRIGERACION DADOS EN COMODATO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120104", "OTROS EQUIPOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120105", "EQUIP. DE INFORMATICA-HARDWARE", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120106", "EQUIP.DE INF-HARD EN LEASING", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120107", "MUEBLES, ENSERES Y UTILES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120108", "VEHICULOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120109", "CONSTRUCCIONES Y OBRAS DE INFRAESTRUCTURAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1201PropiedadesPlataYEquipos, "120110", "OTROS ACTIVO FIJOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        //1. ACTIVOS / 12.ACTIVO FIJO O ACTIVO NO CORRIENTE / 1202.DEPRECIACION Acumulada Prop plantas y equipos                                       null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1202DepAcumuladaPropPlantesYEquipos, "120201", "DEPRECIACIÓN ACUMULADA EQUIPOS DE PRODUCCION", false, TipoAuxiliar.ProveedorDeudor, true, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1202DepAcumuladaPropPlantesYEquipos, "120202", "DEPRECIACIÓN ACUMULADA EQUIPOS DE REFRIGERACION", false, TipoAuxiliar.ProveedorDeudor, true, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1202DepAcumuladaPropPlantesYEquipos, "120203", "DEPRECIACIÓN ACUMULADA EQUIPOS DE REFRIGERACION DADOS EN COMODATO", false, TipoAuxiliar.ProveedorDeudor, true, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1202DepAcumuladaPropPlantesYEquipos, "120204", "DEPRECIACIÓN ACUMULADA OTROS EQUIPOS", false, TipoAuxiliar.ProveedorDeudor, true, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1202DepAcumuladaPropPlantesYEquipos, "120205", "DEPRECIACIÓN ACUMULADA EQUIP. DE INFORMATICA-HARDWARE", false, TipoAuxiliar.ProveedorDeudor, true, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1202DepAcumuladaPropPlantesYEquipos, "120206", "DEPRECIACIÓN ACUMULADA MUEBLES, ENSERES Y UTILES", false, TipoAuxiliar.ProveedorDeudor, true, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1202DepAcumuladaPropPlantesYEquipos, "120207", "DEPRECIACIÓN ACUMULADA VEHÍCULOS", false, TipoAuxiliar.ProveedorDeudor, true, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1202DepAcumuladaPropPlantesYEquipos, "120208", "DEPRECIACIÓN ACUMULADA CONSTRUCCIONES Y OBRAS DE INFRAESTRUCTURAS", false, TipoAuxiliar.ProveedorDeudor, true, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));

        //1. ACTIVOS / 12.ACTIVO FIJO O ACTIVO NO CORRIENTE / 1203.ACTIVOS INTANGIBLES                                     null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1203ActivosIntangibles, "120301", "SOFTWARE Y LICENCIAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1203ActivosIntangibles, "120302", "MARCAS, DOMINIS Y PATENTES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1203ActivosIntangibles, "120303", "OTROS ACTIVOS INTANGIBLES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        //1. ACTIVOS / 12.ACTIVO FIJO O ACTIVO NO CORRIENTE / 1204.DEPRECIACION ACUMULADA SOFTWARE E INTANGIBLES                          null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1204DepAcumuladaSoftwareEIntangibles, "120401", "DEPRECIACIÓN ACUMULADA SOFTWARE Y LICENCIAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1204DepAcumuladaSoftwareEIntangibles, "120402", "DEPRECIACIÓN ACUMULADA OTROS INTANGIBLES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 1, 0, 0, 0));
        //1. ACTIVOS / 12.ACTIVO FIJO O ACTIVO NO CORRIENTE / 1205.ACTIVOS POR IMPUESTOS DIFERIDOS                                  null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.ACTIVOS, sc12ActivoFijoONoCorriente, ssc1205ActivosPorIMpuestosDiferidos, "120501", "ACTIVOS POR IMPUESTOS DIFERIDOS"));
       
        //2.PASIVOS / 22.PASIVO A LARGO PLAZO / 2201.PROVEEDORES POR PAGAR                        null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220101", "PROVEEDORES NACIONALES", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220102", "PROVEEDORES EXTRANJEROS", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220103", "DOCUMENTOS POR PAGAR (CHQF)", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220104", "OTRAS CUENTAS POR PAGAR", true, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220105", "FACTURAS POR RECIBIR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220106", "NOTAS DE CREDITO POR RECIBIR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220107", "FACTURAS POR RECIBIR IMPORTACION"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220108", "CORFO POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2201ProveedoresPorPagar, "220109", "SERCOTEC POR RENDIR", false, TipoAuxiliar.Honorarios, false, TipoCentralizacion.Honorarios));
        //2.PASIVOS / 22.PASIVO A LARGO PLAZO / 2202.DEUDAS DE CORTO PLAZO                       null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2202DeudasDeCortoPlazo, "220201", "OBLIGACIONES CON BANCOS E INSTITUCIONES FINANCIERAS CORTO PLAZO"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2202DeudasDeCortoPlazo, "220202", "TARJETA DE CREDITO CLP"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2202DeudasDeCortoPlazo, "220203", "TARJETA DE CREDITO USD"));
        //2.PASIVOS / 22.PASIVO A LARGO PLAZO / 2203.PROVISIONES CORTO PLAZO                      null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2203ProvisionesCortoPlazo, "220301", "PROVISIONES DEVOLUC. CLIENTES"));
        //2.PASIVOS / 22.PASIVO A LARGO PLAZO / 2204.IMPUESTOS POR PAGAR                       null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2204ImpuestosPorPagar, "220401", "IVA DEBITO FISCAL", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Venta ));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2204ImpuestosPorPagar, "220402", "PPM POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2204ImpuestosPorPagar, "220403", "IMPUESTO UNICO POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2204ImpuestosPorPagar, "220404", "IMPUESTO ADICIONAL POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2204ImpuestosPorPagar, "220405", "RETENCIONES PROFESIONALES POR PAGAR", false, TipoAuxiliar.Honorarios, false, TipoCentralizacion.Honorarios));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2204ImpuestosPorPagar, "220406", "IMPUESTO DIFERIDO CP"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2204ImpuestosPorPagar, "220407", "IMPUESTO RENTA POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2204ImpuestosPorPagar, "220408", "IMPUESTO POR PAGAR"));
        //2.PASIVOS / 22.PASIVO A LARGO PLAZO / 2205.OTRAS CUENTAS POR PAGAR CORRIENTES                       null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220501", "SUELDOS POR PAGAR", true, TipoAuxiliar.Remuneracion, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220502", "HONORARIOS POR PAGAR", true, TipoAuxiliar.Honorarios, false, TipoCentralizacion.Honorarios, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220503", "FINIQUITOS POR PAGAR", true, TipoAuxiliar.Remuneracion, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220504", "PROVISION VACACIONES"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220505", "PROVISION INDEMNIZACION"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220506", "AFP POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220507", "IPS POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220508", "ISAPRE POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220509", "MUTUAL POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220510", "CCAF POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220511", "APV POR PAGAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220512", "PROVISION DE BONOS"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220513", "PROVISION DE ADMINISTRACION", false, TipoAuxiliar.Honorarios, false, TipoCentralizacion.Ninguno, 0, 0, 1, 0, 1));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc22PasivoCirculanteOCorriente, ssc2205OtrasCuentasPorPagarCorrientes, "220514", "DIVIDENDOS POR PAGAR", true));
        
        //2.PASIVOS / 23.Pasivo No Circulante o Pasivo Largo Plazo no corriente / 2301.Deudas de largo plazo                                              null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc23PasivoNoCirculanteOLarzoPlazoNoCorriente, ssc2301DeudasDeLargoPlazo, "230101", "OBLIGACIONES CON BANCOS E INSTITUCIONES FINANCIERAS LARGO PLAZO CLP"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc23PasivoNoCirculanteOLarzoPlazoNoCorriente, ssc2301DeudasDeLargoPlazo, "230102", "OBLIGACIONES CON BANCOS E INSTITUCIONES FINANCIERAS LARGO PLAZO USD"));
        //2.PASIVOS / 23.Pasivo No Circulante o Pasivo Largo Plazo no corriente / 2302.PROVISIONES LARGO PLAZO                           null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc23PasivoNoCirculanteOLarzoPlazoNoCorriente, ssc2302ProvisionesLargoPlazo, "230201", "PROVISIONES LARGO PLAZO"));
        //2.PASIVOS / 23.Pasivo No Circulante o Pasivo Largo Plazo no corriente / 2303.IMPUESTOS A PAGAR                           null, 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc23PasivoNoCirculanteOLarzoPlazoNoCorriente, ssc2303ImpuestosPorPagar, "230301", "IMPUESTO DIFERIDO LP"));
        //2.PASIVOS / 23.Pasivo No Circulante o Pasivo Largo Plazo no corriente / 2304.OTRAS CUENTAS POR PAGAR NO CORRIENTES                                null,                                                 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc23PasivoNoCirculanteOLarzoPlazoNoCorriente, ssc2304OtrascCuentasPorPagarNoCorrientes, "230401", "OTROS PASIVOS A LARGO PLAZO"));
        
        //2.PASIVOS / 24.PATRIMONIO NETO / 2401.CAPITAL                                null,                                                 
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc24PatrimonioNeto, ssc2401Capital, "240101", "CAPITAL SUSCRITP Y PAGADO"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc24PatrimonioNeto, ssc2401Capital, "240102", "APORTE POR CAPITALIZAR"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc24PatrimonioNeto, ssc2401Capital, "240103", "ACCIONES SUSCRITAS NO PAGADAS"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc24PatrimonioNeto, ssc2401Capital, "240104", "REVALORIZACION CAPITAL PROPIO"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc24PatrimonioNeto, ssc2401Capital, "240105", "RESULTADO ACUMULADO"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc24PatrimonioNeto, ssc2401Capital, "240106", "RESULTADO DEL EJERCICIO"));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.PASIVOS, sc24PatrimonioNeto, ssc2401Capital, "240107", "DIVIDENDOS"));

        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4101.COSTOS DE MERCADERIA VENDIDA
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4101CostosDeLaMercaderiaVendida, "410101", "COSTO DE MATERIA PRIMA (F%V)", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4101CostosDeLaMercaderiaVendida, "410102", "COSTOS DE OTRAS MATERIAS PRIMAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4101CostosDeLaMercaderiaVendida, "410103", "COSTOS DE MATERIAS PRIMAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4102.GASTOS DE PRODUCCION
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410201", "MANO DE OBRA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410202", "OTROS M DE OBRA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410203", "LIMPIEZA Y SANITIZACION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410204", "MANTENCION DE EQUIP. E INSTALACIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410205", "ARRIENDO DE EQUIPOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410206", "ARRIENDO PLANTA DE PRODUCCION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410207", "GASTOS COMUNES PLANTA DE PRODUCCION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410208", "AGUA, ENERGIA ELECTRICA Y OTROS SERVICIOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410209", "PATENTES Y CONTRIBUCIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410210", "SEGUROS PLANTA DE PRODUCCION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410211", "OTROS GASTOS DE PRODUCCION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410212", "GASTOS DE CALIDAD", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4102GastosDeProduccion, "410213", "GASTOS OPERACIONALES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4103.GASTOS DE DISTRIBUCION
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4103GastosDeDistribucion, "410301", "DESPACHO MERCADERIA VENDIDA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4104.GASTOS DE COMERCIALIZACION
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4104GastosDeComercializacion, "410401", "SERVICIOS REPOSICION EN PUNTO DE VENTA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4104GastosDeComercializacion, "410402", "COMISION MEDIOS DE PAGO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4104GastosDeComercializacion, "410403", "PERDIDA DEUDORES INCOBRABLES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4104GastosDeComercializacion, "410404", "COMISION VENDEDORES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4104GastosDeComercializacion, "410405", "TRANSBANK", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4105.GASTOS DE MARKETING
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410501", "DESARROLLO DE PRODUCTO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410502", "MEDIOS OFFLINE", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410503", "MEDIOS ONLINE", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410504", "MATERIAL POP", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410505", "EVENTOS Y CONCURSOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410506", "DISEÑO Y AUDIOVISUAL", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410507", "SITIO WEB", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410508", "ACTIVACION DE PDV", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4105GastosDeMarketing, "410509", "FEE AGENCIAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4106.AJUSTES DE INVENTARIO
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4106AjustesDeInventario, "410601", "AJUSTES DE INVENTARIO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4106AjustesDeInventario, "410602", "UTILIDAD POR VENTA DE ACTIVO FIJO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4106AjustesDeInventario, "410603", "PERDIDA POR VENTA DE ACTIVO FIJO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4107.RECURSOS HUMANOS
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410701", "SUELDO, GRATIFICACION Y OTROS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410702", "CARGAS SOCIALES, SEGUROS Y MUTUAL", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410703", "SEGURO COMPLEMENTARIO DE SALUD Y/O DENTAL", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410704", "EVENTOS/AMBIENTE LABORAL", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410705", "INDEMNIZACIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410706", "CURSOS Y CAPACITACION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410707", "RECLUTAMIENTO Y SELECCION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410708", "SERVICIOS DE CONSULTORIA Y ASESORIAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4107RecursosHumanos, "410709", "HONORARIOS PROFESIONALES", false, TipoAuxiliar.Honorarios, false, TipoCentralizacion.Honorarios));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4108.GASTOS LEGALES
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4108GastosLegales, "410801", "ASESORIAS JURIDICAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4108GastosLegales, "410802", "GTOS POR CONTINGENCIAS LEGALES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4108GastosLegales, "410803", "OTROS GASTOS LEGALES",  false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4108GastosLegales, "410804", "GASTOS NOTARIALES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4109.ASESORIAS
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4109Asesorias, "410901", "ASESORIA CONTABLE", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4109Asesorias, "410902", "AUDITORIA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4109Asesorias, "410903", "CONSULTORIAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4110.TI
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4110TI, "411001", "SERVICIOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4110TI, "411002", "LICENCIAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4111.GASTOS DE EQUIPAMIENTO Y OFICINA
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411101", "TELEFONIA E INTERNET", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra, 1, 1, 0, 0, 0   ));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411102", "ARRIENDOS DE INMUEB. ADMINISTRAC.", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411103", "GASTOS COMUNES ADMINISTRAC.", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411104", "PATENTES Y CONTRIBUCIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411105", "UTILES Y ARTICULOS DE OFICINA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411106", "LIMPIEZA Y ASEO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411107", "MANTENCION DE EQIP. E INSTALAC", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411108", "ARRIENDO DE EQUIPOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411109", "GASTOS COURRIER", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411110", "VIAJES Y CONGRESOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411111", "LOCOMOCION - TAXI", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411112", "LOCOMOCION - PEAJES Y KILOMET.", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411113", "LOCOMOCION - ESTACIONAMIENTO Y OTROS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411114", "GASTOS DE ALIMENTACION", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411115", "SEGURIDAD Y VIGILANCIA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411116", "REVISTAS Y PERIODICOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411117", "SEGUROS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411118", "REMODELACIONES, MEJORAS Y MANTENCIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411119", "GASTOS GENERALES DE OFICINA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4111GastosDeEquipamientoYOficina, "411120", "GASTOS VEHICULO Y TRASLADO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Compra));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4112.DEPRECIACION Y AMORTIZACION
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4112DepreciacionYAmortizacion, "411201", "DEPRECIACION DEL EJERCICIO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        //4.RESULTADOS/PERDIDAS / 41.COSTO DE EXPLOTACION / 4113.IMPUESTOS
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4113Impuestos, "411301", "IMPUESTOS ADICIONAL (F50)", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc41CostoDeExplotacion, ssc4113Impuestos, "411302", "IMPUESTO A LA RENTA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));

        //4.RESULTADOS/PERDIDAS /42.GASTOS POR INTERESES / 4201.OTROS GASTOS
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420101", "GASTOS Y COMISIONES BANCARIAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420102", "INTERESES Y COSTOS DE LA DEUDA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420103", "IVA NO RECUPERABLE", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420104", "DIFERENCIA TIPO CAMBIO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420105", "CORRECION MONETARIA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420106", "GASTOS POR INVERSIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));

        //5.RESULTADOS/GANANCIAS / 51.VENTA NETA / 5101.INGRESOS POR VENTA              null,
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc51VentaNeta, ssc5101IngresosPorVenta, "510101", "INGRESOS POR VENTA ADICIONALES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Venta, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc51VentaNeta, ssc5101IngresosPorVenta, "510102", "OTROS INGRESOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc51VentaNeta, ssc5101IngresosPorVenta, "510103", "DESCUENTOS Y PROMOCIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        
        //5.RESULTADOS/GANANCIAS / 52.INGRESOS/GASTOS POR INTERESES / 5201.INGRESOS/GASTOS POR INTERESES                      null,
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc52IngresosGastosPorIntereses, ssc5201IngresosGastosPorIntereses, "520101", "COMISIONES BANCARIAS GANADAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc52IngresosGastosPorIntereses, ssc5201IngresosGastosPorIntereses, "520102", "INTERESES GANADOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc52IngresosGastosPorIntereses, ssc5201IngresosGastosPorIntereses, "520103", "IMPUESTOS GANADOS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc52IngresosGastosPorIntereses, ssc5201IngresosGastosPorIntereses, "520104", "DIFERENCIA TIPO CAMBIO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc52IngresosGastosPorIntereses, ssc5201IngresosGastosPorIntereses, "520105", "CORRECCION MONETARIA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc52IngresosGastosPorIntereses, ssc5201IngresosGastosPorIntereses, "520106", "INGRESOS POR INVERSIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));

        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc52IngresosGastosPorIntereses, ssc5201IngresosGastosPorIntereses, "520107", "IVA USO COMÚN", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOGANANCIA, sc52IngresosGastosPorIntereses, ssc5201IngresosGastosPorIntereses, "520108", "IVA ACTIVO FIJO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));


        
        
        

        db.DBCuentaContable.AddRange(CtaContableBase);
        db.SaveChanges();
        #endregion

        return null;
    }

    public static List<CuentaContableModel> UpdateCtaContParaAntiguos(int clienteID, FacturaPoliContext db)
    {
        //LISTA DE SUBCLASIFICACION
        List<SubClasificacionCtaContable> sc_Base = new List<SubClasificacionCtaContable>();

        SubClasificacionCtaContable sc42GastosPorIntereses = new SubClasificacionCtaContable(clienteID, "42", "GASTOS POR INTERESES");
        sc_Base.Add(sc42GastosPorIntereses);
        db.DBSubClasificacion.AddRange(sc_Base);
        db.SaveChanges();
        //LISTA DE SUBSUBCLASIFICACION
        List<SubSubClasificacionCtaContable> ssc_Base = new List<SubSubClasificacionCtaContable>();

        SubSubClasificacionCtaContable ssc4201OtrosGastos = new SubSubClasificacionCtaContable(clienteID, "4201", "OTROS GASTOS");
        ssc_Base.Add(ssc4201OtrosGastos);
        db.DBSubSubClasificacion.AddRange(ssc_Base);
        db.SaveChanges();
        //LISTA DE CUENTA CONTABLE
        List<CuentaContableModel> CtaContableBase = new List<CuentaContableModel>();

        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420101", "GASTOS Y COMISIONES BANCARIAS", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420102", "INTERESES Y COSTOS DE LA DEUDA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420103", "IVA NO RECUPERABLE", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420104", "DIFERENCIA TIPO CAMBIO", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420105", "CORRECION MONETARIA", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        CtaContableBase.Add(new CuentaContableModel(clienteID, ClasificacionCtaContable.RESULTADOPERDIDA, sc42GastosPorIntereses, ssc4201OtrosGastos, "420106", "GASTOS POR INVERSIONES", false, TipoAuxiliar.ProveedorDeudor, false, TipoCentralizacion.Ninguno, 1, 1, 0, 0, 0));
        db.DBCuentaContable.AddRange(CtaContableBase);
        db.SaveChanges();

        return null;
    }

}
public enum ClasificacionCtaContable
{
    [Display(Name = "ACTIVOS")]
    ACTIVOS = 1,
    [Display(Name = "PASIVOS")]
    PASIVOS = 2,
    [Display(Name = "RESULTADOS/PERDIDAS")]
    RESULTADOPERDIDA = 4,
    [Display(Name = "RESULTADOS/GANANCIAS")]
    RESULTADOGANANCIA = 5
}


