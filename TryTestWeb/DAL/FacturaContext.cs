using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
public class FacturaContext : DbContext
{
    public FacturaContext() : base("OtherConnection")
    {
     
    }


    //Tablas perfilamiento usuarios CERTIFICACION
    public DbSet<UsuarioModel> DBUsuarios { get; set; }
    public DbSet<EmisoresHabilitados> DBEmisoresHabilitados { get; set; }
    public DbSet<PosesionUsuarios> DBPosesionUsuarios { get; set; }

    public DbSet<QuickEmisorModel> Emisores { get; set; }
    public DbSet<QuickReceptorModel> Receptores { get; set; }
    public DbSet<CertificadosModels> Certificados { get; set; }
    public DbSet<EstadoResultadoModel> EstadoResultado { get; set; }


    public DbSet<DTEPagosModel> DBPagos { get; set; } 
    public DbSet<BoletasHonorariosModel> DBBoletasHonorarios { get; set; }
 

    //public DbSet<ClasificacionCuentaContableModel> CuentaContable { get; set; }

    //public DbSet<ItemContableModel> DocumentoContable { get; set; }

    public DbSet<ComunaModels> DBComunas { get; set; }
    public DbSet<RegionModels> DBRegiones { get; set; }

    public DbSet<ModulosHabilitados> DBModulosHabilitados { get; set; }
    public DbSet<FuncionesModel> DBFunciones { get; set; }

    public DbSet<ReportesImpagosLog> DBReportesImpagosLog { get; set; }

    //Bases de datos sistema contable
    public DbSet<ClientesContablesModel> DBClientesContables { get; set; }
    public DbSet<CuentaContableModel> DBCuentaContable { get; set; }
    public DbSet<VoucherModel> DBVoucher { get; set; }
    public DbSet<DetalleVoucherModel> DBDetalleVoucher { get; set; }
    public DbSet<CentroCostoModel> DBCentroCosto { get; set; }
    public DbSet<SubClasificacionCtaContable> DBSubClasificacion { get; set; }
    public DbSet<SubSubClasificacionCtaContable> DBSubSubClasificacion { get; set; }

    public DbSet<AuxiliaresModel> DBAuxiliares { get; set; }
    public DbSet<AuxiliaresDetalleModel> DBAuxiliaresDetalle { get; set; }
    public DbSet<AuxiliaresPrestadoresModel> DBAuxiliaresPrestadores { get; set; }

    public DbSet<LibrosContablesModel> DBLibrosContables { get; set; }
    public DbSet<UserClientesContablesModels> DBUserToClientesContables { get; set; }

    //adicional para diferenciar que componente de sistema es/son
    public DbSet<ModuloSistemaModel> DBModuloSistema { get; set; }

    //Monitoreo sistema contable
    public DbSet<MonitoreoModel> DBMonitoreo { get; set; }

    //Multi ACTECO
    public DbSet<ActividadEconomicaModel> DBActeco { get; set; }
    public DbSet<ActividadEconomicaModelCuentaContableModel> DBActividadECContable { get; set; }
    public DbSet<ItemModel> DBItems { get; set; }
    public DbSet<ClientesProveedoresModel> DBClientesProveedores { get; set; }
    public DbSet<ClientesContablesEmisorModel> DBClientesContablesEmisor { get; set; }
    public DbSet<CtasContablesPresupuestoModel> DBCCPresupuesto { get; set; }
    public DbSet<CentroCostoPresupuestoModels> DBCentroCostoPresupuesto { get; set; }
    public DbSet<SubSubCtaPresupuestoModel> DBSubSubCtaPresupuesto { get; set; }
    public DbSet<ImpuestosAdicionalesModel> DBImpuestosAdicionalesSII { get; set; }
    public DbSet<ImpuestosAdRelacionModel> DBImpuestosAdRelacionSII { get; set; }
    public DbSet<PerfilUsuarioModel> DBPerfilUsuario { get; set; }
    public DbSet<LibroDeHonorariosModel> DBLibroDeHonorarios { get; set; }
    
    //Base de datos Sistema de Remuneraciones
    public DbSet<EmpleadoModel> DBempleados { get; set; }

    public DbSet<CargoRemuModels>DBcargoRemu { get; set; }

    public DbSet<TipoContratoRemuModels> DBtipoContratoRemu { get; set; }

    public DbSet<GeneroRemuModels> DBgeneroRemu { get; set; }

    public DbSet<EstadoCivilRemuModels> DBestadoCivilRemu { get; set; }

    public DbSet<AfpRemuModels> DBafpRemu { get; set; }

    public DbSet<IsapreRemuModels> DBisapreRemu { get; set; }

    public DbSet<BancoRemumodels> DBbancoRemu { get; set; }

    public DbSet<GratificacionRemuModels> DBgratificacionRemu { get; set; }

    public DbSet<TramoFaRemumodels> DBtramoRemu { get; set; }

    public DbSet<SucursalRemuModels> DBsucursalRemu { get; set; }

    public DbSet<TsueldoBaRemuModels> DBtsueldoBaseRemu { get; set; }

    public DbSet<InsApvRemuModels> DBinsApvRemu { get; set; }

    public DbSet<EmpBancoRemuModels> DBbancoEmpRemu { get; set; }

    public DbSet<UfRemuModels> DBufRemu { get; set; }

    public DbSet<UtmRemuModels> DButmRemu { get; set; }

    public DbSet<SminimoRemuModels> DBsueldoMinimoRemu { get; set; }

    public DbSet<TcuentaRemuModels> DBtipoCuentaRemu { get; set; }

    public DbSet<DatosVariablesRemuModels> DBdatosVaribalesRemu { get; set; }

   



    // public DbSet<DatosBaseModel> DBDatosBase { get; set; }


    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        
        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        base.OnModelCreating(modelBuilder);  
    }
}

public class FacturaProduccionContext : DbContext
{
    public FacturaProduccionContext() : base("ProdConnection")
    {
       
    }

    //Tablas perfilamiento usuarios PRODUCCION
    public DbSet<UsuarioModel> DBUsuarios { get; set; }
    public DbSet<EmisoresHabilitados> DBEmisoresHabilitados { get; set; }
    public DbSet<PosesionUsuarios> DBPosesionUsuarios { get; set; }


    public DbSet<QuickEmisorModel> Emisores { get; set; }
    public DbSet<QuickReceptorModel> Receptores { get; set; }

    public DbSet<CertificadosModels> Certificados { get; set; }
    public DbSet<EstadoResultadoModel> EstadoResultado { get; set; }
    public DbSet<DTEPagosModel> DBPagos { get; set; }
    public DbSet<BoletasHonorariosModel> DBBoletasHonorarios { get; set; }
   

    //public DbSet<ClasificacionCuentaContableModel> CuentaContable { get; set; }

    //public DbSet<ItemContableModel> DocumentoContable { get; set; }

    public DbSet<ComunaModels> DBComunas { get; set; }
    public DbSet<RegionModels> DBRegiones { get; set; }

    public DbSet<ModulosHabilitados> DBModulosHabilitados { get; set; }
    public DbSet<FuncionesModel> DBFunciones { get; set; }

    public DbSet<ReportesImpagosLog> DBReportesImpagosLog { get; set; }

    //Bases de datos sistema contable
    public DbSet<ClientesContablesModel> DBClientesContables { get; set; }
    public DbSet<CuentaContableModel> DBCuentaContable { get; set; }
    public DbSet<VoucherModel> DBVoucher { get; set; }
    public DbSet<DetalleVoucherModel> DBDetalleVoucher { get; set; }
    public DbSet<CentroCostoModel> DBCentroCosto { get; set; }
    public DbSet<SubClasificacionCtaContable> DBSubClasificacion { get; set; }
    public DbSet<SubSubClasificacionCtaContable> DBSubSubClasificacion { get; set; }

    public DbSet<AuxiliaresModel> DBAuxiliares { get; set; }
    public DbSet<AuxiliaresDetalleModel> DBAuxiliaresDetalle { get; set; }
    public DbSet<AuxiliaresPrestadoresModel> DBAuxiliaresPrestadores { get; set; }

    public DbSet<LibrosContablesModel> DBLibrosContables { get; set; }
    public DbSet<UserClientesContablesModels> DBUserToClientesContables { get; set; }

    //adicional para diferenciar que componente de sistema es/son
    public DbSet<ModuloSistemaModel> DBModuloSistema { get; set; }

    //Monitoreo sistema contable
    public DbSet<MonitoreoModel> DBMonitoreo { get; set; }

    //Multi ACTECO
    public DbSet<ActividadEconomicaModel> DBActeco { get; set; }
    public DbSet<ActividadEconomicaModelCuentaContableModel> DBActividadECContable { get; set; }

    public DbSet<ItemModel> DBItems { get; set; }
    public DbSet<ClientesProveedoresModel> DBClientesProveedores { get; set; }

    public DbSet<ClientesContablesEmisorModel> DBClientesContablesEmisor { get; set; }

    public DbSet<CtasContablesPresupuestoModel> DBCCPresupuesto { get; set; }

    public DbSet<CentroCostoPresupuestoModels> DBCentroCostoPresupuesto { get; set; }

    public DbSet<SubSubCtaPresupuestoModel> DBSubSubCtaPresupuesto { get; set; }

    public DbSet<ImpuestosAdicionalesModel> DBImpuestosAdicionalesSII { get; set; }
    public DbSet<ImpuestosAdRelacionModel> DBImpuestosAdRelacionSII { get; set; }
    public DbSet<PerfilUsuarioModel> DBPerfilUsuario { get; set; }
    public DbSet<LibroDeHonorariosModel> DBLibroDeHonorarios { get; set; }

    //Base de datos Sistema de remuneraciones
    public DbSet<EmpleadoModel> DBempleados { get; set; }

    public DbSet<CargoRemuModels> DBcargoRemu { get; set; }

    public DbSet<TipoContratoRemuModels> DBtipoContratoRemu { get; set; }

    public DbSet<GeneroRemuModels> DBgeneroRemu { get; set; }

    public DbSet<EstadoCivilRemuModels> DBestadoCivilRemu { get; set; }

    public DbSet<AfpRemuModels> DBafpRemu { get; set; }

    public DbSet<IsapreRemuModels> DBisapreRemu { get; set; }

    public DbSet<BancoRemumodels> DBbancoRemu { get; set; }

    public DbSet<GratificacionRemuModels> DBgratificacionRemu { get; set; }

    public DbSet<TramoFaRemumodels> DBtramoRemu { get; set; }

    public DbSet<SucursalRemuModels> DBsucursalRemu { get; set; }

    public DbSet<TsueldoBaRemuModels> DBtsueldoBaseRemu { get; set; }

    public DbSet<InsApvRemuModels> DBinsApvRemu { get; set; }

    public DbSet<EmpBancoRemuModels> DBbancoEmpRemu { get; set; }

    public DbSet<UfRemuModels> DBufRemu { get; set; }

    public DbSet<UtmRemuModels> DButmRemu { get; set; }

    public DbSet<SminimoRemuModels> DBsueldoMinimoRemu { get; set; }

    public DbSet<TcuentaRemuModels> DBtipoCuentaRemu { get; set; }

    public DbSet<DatosVariablesRemuModels> DBdatosVaribalesRemu { get; set; }

    // public DbSet<DatosBaseModel> DBDatosBase { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        base.OnModelCreating(modelBuilder);
    }

}

public class FacturaPoliContext : DbContext
{
    public FacturaPoliContext(bool useProd) : base("ProdConnection")
    {

    }

    public FacturaPoliContext() : base("OtherConnection")
    {

    }

    //perfilamiento poliContext
    public DbSet<UsuarioModel> DBUsuarios { get; set; }
    public DbSet<EmisoresHabilitados> DBEmisoresHabilitados { get; set; }
    public DbSet<PosesionUsuarios> DBPosesionUsuarios { get; set; }

 
    public DbSet<QuickEmisorModel> Emisores { get; set; }
    public DbSet<QuickReceptorModel> Receptores { get; set; }
    public DbSet<CertificadosModels> Certificados { get; set; }
    public DbSet<EstadoResultadoModel> EstadoResultado { get; set; }
    public DbSet<DTEPagosModel> DBPagos { get; set; }
    public DbSet<BoletasHonorariosModel> DBBoletasHonorarios { get; set; }
    public DbSet<ComunaModels> DBComunas { get; set; }
    public DbSet<RegionModels> DBRegiones { get; set; }
    public DbSet<ModulosHabilitados> DBModulosHabilitados { get; set; }
    public DbSet<FuncionesModel> DBFunciones { get; set; }
    public DbSet<ReportesImpagosLog> DBReportesImpagosLog { get; set; }

    //Bases de datos sistema contable
    public DbSet<ClientesContablesModel> DBClientesContables { get; set; }
    public DbSet<CuentaContableModel> DBCuentaContable { get; set; }
    public DbSet<VoucherModel> DBVoucher { get; set; }
    public DbSet<DetalleVoucherModel> DBDetalleVoucher { get; set; }
    public DbSet<CentroCostoModel> DBCentroCosto { get; set; }
    public DbSet<SubClasificacionCtaContable> DBSubClasificacion { get; set; }
    public DbSet<SubSubClasificacionCtaContable> DBSubSubClasificacion { get; set; }
    public DbSet<AuxiliaresModel> DBAuxiliares { get; set; }
    public DbSet<AuxiliaresDetalleModel> DBAuxiliaresDetalle { get; set; }
    public DbSet<AuxiliaresPrestadoresModel> DBAuxiliaresPrestadores { get; set; }

    public DbSet<LibrosContablesModel> DBLibrosContables { get; set; }
    public DbSet<UserClientesContablesModels> DBUserToClientesContables { get; set; }

    //adicional para diferenciar que componente de sistema es/son
    public DbSet<ModuloSistemaModel> DBModuloSistema { get; set; }

    //Monitoreo sistema contable
    public DbSet<MonitoreoModel> DBMonitoreo { get; set; }

    //Multi ACTECO
    public DbSet<ActividadEconomicaModel> DBActeco { get; set; }
    public DbSet<ActividadEconomicaModelCuentaContableModel> DBActividadECContable { get; set; }
    public DbSet<ItemModel> DBItems { get; set; }

    public DbSet<ClientesProveedoresModel> DBClientesProveedores { get; set; }

    public DbSet<ClientesContablesEmisorModel> DBClientesContablesEmisor { get; set; }

    public DbSet<CtasContablesPresupuestoModel> DBCCPresupuesto { get; set; }

    public DbSet<CentroCostoPresupuestoModels> DBCentroCostoPresupuesto { get; set; }
    public DbSet<SubSubCtaPresupuestoModel> DBSubSubCtaPresupuesto { get; set; }
    public DbSet<ImpuestosAdicionalesModel> DBImpuestosAdicionalesSII { get; set; }
    public DbSet<ImpuestosAdRelacionModel> DBImpuestosAdRelacionSII { get; set; }
    public DbSet<PerfilUsuarioModel> DBPerfilUsuario { get; set; }
    public DbSet<LibroDeHonorariosModel> DBLibroDeHonorarios { get; set; }

    public DbSet<EmpleadoModel> DBempleados { get; set; }
    
    public DbSet<CargoRemuModels> DBcargoRemu { get; set; }

    public DbSet<TipoContratoRemuModels> DBtipoContratoRemu { get; set; }

    public DbSet<GeneroRemuModels> DBgeneroRemu { get; set; }

    public DbSet<EstadoCivilRemuModels> DBestadoCivilRemu { get; set; }

    public DbSet<AfpRemuModels> DBafpRemu { get; set; }

    public DbSet<IsapreRemuModels> DBisapreRemu { get; set; }

    public DbSet<BancoRemumodels> DBbancoRemu { get; set; }

    public DbSet<GratificacionRemuModels> DBgratificacionRemu { get; set; }

    public DbSet<TramoFaRemumodels> DBtramoRemu { get; set; }

    public DbSet<SucursalRemuModels> DBsucursalRemu { get; set; }

    public DbSet<TsueldoBaRemuModels> DBtsueldoBaseRemu { get; set; }

    public DbSet<InsApvRemuModels> DBinsApvRemu { get; set; }

    public DbSet<EmpBancoRemuModels> DBbancoEmpRemu { get; set; }

    public DbSet<UfRemuModels> DBufRemu { get; set; }

    public DbSet<UtmRemuModels> DButmRemu { get; set; }

    public DbSet<SminimoRemuModels> DBsueldoMinimoRemu { get; set; }

    public DbSet<TcuentaRemuModels> DBtipoCuentaRemu { get; set; }

    public DbSet<DatosVariablesRemuModels> DBdatosVaribalesRemu { get; set; }



    // public DbSet<DatosBaseModel> DBDatosBase { get; set; }


    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        base.OnModelCreating(modelBuilder);
    }


}
