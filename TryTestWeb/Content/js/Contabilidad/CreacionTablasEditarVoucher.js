"use strict";
var GetLstCuentacontable = "";
var GetLstCentroDeCostos = "";
var GetLstTipoDTE="";
var VoucherCompleto=[];
var AuxiliaresDetalle=[];
var DetalleVouchers=[];

const GetLstCuentasContables = async () => {
    return (await fetch('getLstCuentasContables/Contabilidad')).json();
}
const LstCentroCosto = async () => {
    return (await fetch('getLstCentroCosto/Contabilidad')).json();
}
const LstTipoDTE = async () => {
    return (await fetch('getLstTipoDTE/Contabilidad')).json();
}
const GetRazonSocial = async (TipoPrestador) =>{
    return (await fetch('ObtenerPrestador/Contabilidad?TipoPrestador='+TipoPrestador)).json();
}
document.addEventListener("DOMContentLoaded", async () =>{
    try{
        GetLstCuentacontable = await GetLstCuentasContables();
        GetLstCentroDeCostos = await LstCentroCosto();
        GetLstTipoDTE = await LstTipoDTE();
        VoucherCompleto = await InfoVoucher();
        TieneAuxiliares(VoucherCompleto.DetalleVoucher);
    } catch(e){
        console.log(e);
    }
});

var idDetalle = 0;
const CrearTablaVoucher = () => {
    idDetalle = idDetalle + 1;
    let glosa = document.getElementById("glosa").value;
    if (glosa == null) {
        glosa = "";
    }

    let DivPadreC = document.getElementById("ContenidoDetalle");
    let DivTablaVoucher = document.createElement("div");
    DivTablaVoucher.id = "detalle" + idDetalle;
    DivTablaVoucher.className = "form-group col-lg-12 GuardarDatosVoucher";
    DivPadreC.appendChild(DivTablaVoucher);

    //creacion de btnAuxiliar
    let DivBtnAuxVoucher = document.createElement("div");
    DivBtnAuxVoucher.className = "form-group col-lg-1";
    let BtnAuxNuevo = document.createElement("button");
    BtnAuxNuevo.className = "btn btn-success btn-sm redondo btnPress";
    BtnAuxNuevo.type = "button";
    BtnAuxNuevo.id = idDetalle;
    BtnAuxNuevo.textContent = "AUX";
    BtnAuxNuevo.onclick = function () { ClickAuxNuevo(DivTablaVoucher.id,BtnAuxNuevo.id) };
    BtnAuxNuevo.disabled = true;
    DivBtnAuxVoucher.appendChild(BtnAuxNuevo);
    DivTablaVoucher.appendChild(DivBtnAuxVoucher);

    //creacion select Cuenta Contable
    let DivCuentaContable = document.createElement("div");
    DivCuentaContable.className = "form-group col-lg-2";
    let SelectCuentaContable = document.createElement("select");
    SelectCuentaContable.className = "form-control estiloSelectCtaCont"+idDetalle;
    SelectCuentaContable.onchange = function () { ActivaBtnAuxNuevo(DivTablaVoucher.id) };
    SelectCuentaContable.required = true;
    SelectCuentaContable.id="ctacont"+idDetalle;
    SelectCuentaContable.name="ctacont";
    SelectCuentaContable.innerHTML = GetLstCuentacontable.result;
    DivCuentaContable.appendChild(SelectCuentaContable);
    DivTablaVoucher.appendChild(DivCuentaContable);
    

    //creacion input glosa
    let DivInputGlosa = document.createElement("div");
    DivInputGlosa.className = "form-group col-lg-2";
    let InputGlosa = document.createElement("input");
    InputGlosa.className = "selectpicker form-control";
    InputGlosa.type = "text";
    InputGlosa.id = "glosaDetalle"+idDetalle;
    InputGlosa.name = "glosaDetalle";
    InputGlosa.value = glosa;
    InputGlosa.required = true;
    DivInputGlosa.appendChild(InputGlosa);
    DivTablaVoucher.appendChild(DivInputGlosa);

    //creación centro costo
    let DivCentroCosto = document.createElement("div");
    DivCentroCosto.className = "form-group col-lg-2";
    let SelectCC = document.createElement("select");
    SelectCC.className = "form-control estiloSelectCC"+idDetalle;
    SelectCC.id="centrocosto"+idDetalle;
    SelectCC.name="centrocosto";
    SelectCC.innerHTML = GetLstCentroDeCostos.result;
    DivCentroCosto.appendChild(SelectCC);
    DivTablaVoucher.appendChild(DivCentroCosto);
    
    //creación Debe
    let DivMontoDebe = document.createElement("div");
    DivMontoDebe.className = "form-group col-lg-2";
    let InputDebe = document.createElement("input");
    InputDebe.className = "selectpicker form-control";
    InputDebe.type = "number";
    InputDebe.id = "debe"+idDetalle;
    InputDebe.name = "debe";
    InputDebe.min = 0;
    InputDebe.value = 0;
    InputDebe.onchange = SumaTotalesVoucher;
    DivMontoDebe.appendChild(InputDebe);
    DivTablaVoucher.appendChild(DivMontoDebe);

    //Creación Haber
    let DivMontoHaber = document.createElement("div");
    DivMontoHaber.className = "form-group col-lg-2";
    let InputHaber = document.createElement("input");
    InputHaber.className = "selectpicker form-control";
    InputHaber.type = "number";
    InputHaber.id = "haber"+idDetalle;
    InputHaber.name = "haber";
    InputHaber.min = 0;
    InputHaber.value = 0;
    InputHaber.onchange = SumaTotalesVoucher;
    DivMontoHaber.appendChild(InputHaber);
    DivTablaVoucher.appendChild(DivMontoHaber);

    //creación BtnEliminar
    let DivBtnEliminarFila = document.createElement("div");
    DivBtnEliminarFila.className = "form-group col-lg-1";
    let BtnEliminarFilaVoucher = document.createElement("button");
    BtnEliminarFilaVoucher.className = "btn btn-danger btn-sm redondo btnPress";
    BtnEliminarFilaVoucher.name = "borrarTabla"+idDetalle;
    BtnEliminarFilaVoucher.type = "button";
    BtnEliminarFilaVoucher.textContent = "X";
    BtnEliminarFilaVoucher.onclick = function() {BorrarFilaVoucher(DivTablaVoucher.id,BtnAuxNuevo.id)};
    DivBtnEliminarFila.appendChild(BtnEliminarFilaVoucher);
    DivTablaVoucher.appendChild(DivBtnEliminarFila);
    $(".estiloSelectCtaCont"+idDetalle).select2();
    $(".estiloSelectCC"+idDetalle).select2();
}
var IdDivAuxProvDeudor = 0;
const CrearTablaAuxProvDeudor = () => {
    IdDivAuxProvDeudor = IdDivAuxProvDeudor + 1;
    let ContenidoAux = document.getElementById("TablaProvDeudorAux");
    let DivContenidoAux1 = document.createElement("div");
    DivContenidoAux1.id = "detalleAuxProvDeudor" + IdDivAuxProvDeudor;
    DivContenidoAux1.className = "form-group col-lg-12 GuardarDatosAuxProvDeudor";
    ContenidoAux.appendChild(DivContenidoAux1);

    //content folio desde
    let DivFolioDesde = document.createElement("div");
    DivFolioDesde.className="form-group col-lg-1 DivFolioDesde";
    DivFolioDesde.style.display="none";
    DivFolioDesde.id="AuxFolioDesde"+IdDivAuxProvDeudor;
    let InputFolioDesde = document.createElement("input");
    InputFolioDesde.className="form-control";
    InputFolioDesde.id = "FolioDesde"+IdDivAuxProvDeudor;
    InputFolioDesde.name = "FolioDesde";
    InputFolioDesde.min = 1;
    InputFolioDesde.required = true;
    InputFolioDesde.type = "number";
    DivFolioDesde.appendChild(InputFolioDesde);
    DivContenidoAux1.appendChild(DivFolioDesde);

    //content folio/folioHasta
    let DivFolio = document.createElement("div");
    DivFolio.className = "form-group col-lg-1";
    let InputFolio = document.createElement("input");
    InputFolio.className = "form-control";
    InputFolio.name = "AuxFolioHasta";
    InputFolio.id="AuxFolioHasta"+IdDivAuxProvDeudor;
    InputFolio.type = "text";
    InputFolio.required = true;
    DivFolio.appendChild(InputFolio);
    DivContenidoAux1.appendChild(DivFolio);

    //content TipoDTE
    let DivTipoDTE = document.createElement("div");
    DivTipoDTE.className = "form-group col-lg-2";
    let selectTipoDte = document.createElement("select");
    selectTipoDte.className = "form-control estiloSelectTipoDTE"+IdDivAuxProvDeudor;
    selectTipoDte.required=true;
    selectTipoDte.id="TipoDTE"+IdDivAuxProvDeudor;
    selectTipoDte.name="TipoDTE";
    selectTipoDte.innerHTML=GetLstTipoDTE.result;
    DivTipoDTE.appendChild(selectTipoDte);
    DivContenidoAux1.appendChild(DivTipoDTE);

    //contentMonto neto
    let DivMontoNeto = document.createElement("div");
    DivMontoNeto.className = "form-group col-lg-2";
    let InputMontoNeto = document.createElement("input");
    InputMontoNeto.className = "form-control";
    InputMontoNeto.id = "ValorNeto"+IdDivAuxProvDeudor;
    InputMontoNeto.name = "ValorNeto";
    InputMontoNeto.type = "number";
    InputMontoNeto.min = 0;
    InputMontoNeto.value = 0;
    InputMontoNeto.disabled = true;
    DivMontoNeto.appendChild(InputMontoNeto);
    DivContenidoAux1.appendChild(DivMontoNeto);

    //content monto exento
    let DivMontoExento = document.createElement("div");
    DivMontoExento.className = "form-group col-lg-2";
    let InputMontoExento = document.createElement("input");
    InputMontoExento.className = "form-control";
    InputMontoExento.id = "MontoExento"+IdDivAuxProvDeudor;
    InputMontoExento.name = "MontoExento";
    InputMontoExento.type = "number";
    InputMontoExento.min = 0;
    InputMontoExento.value = 0;
    InputMontoExento.onfocusout = function () {CalculoAuxMontoExento(DivContenidoAux1.id)};
    DivMontoExento.appendChild(InputMontoExento);
    DivContenidoAux1.appendChild(DivMontoExento);

    //content monto iva
    let DivMontoIVA = document.createElement("div");
    DivMontoIVA.className = "form-group col-lg-2 DMontoIVA";
    let InputIVA = document.createElement("input");
    InputIVA.id = "MontoIVA"+IdDivAuxProvDeudor;
    InputIVA.name = "MontoIVA";
    InputIVA.type = "number";
    InputIVA.disabled = true;
    InputIVA.min = 0;
    InputIVA.value = 0;
    InputIVA.className = "form-control";
    DivMontoIVA.appendChild(InputIVA);
    DivContenidoAux1.appendChild(DivMontoIVA);

    //content monto total
    let DivMontoTotal = document.createElement("div");
    DivMontoTotal.className = "form-group col-lg-2";
    let InputMontoTotal = document.createElement("input");
    InputMontoTotal.className = "form-control";
    InputMontoTotal.type = "number";
    InputMontoTotal.id = "MontoTotal"+IdDivAuxProvDeudor;
    InputMontoTotal.name = "MontoTotal";
    InputMontoTotal.min = 0;
    InputMontoTotal.value = 0;
    InputMontoTotal.required=true;
    InputMontoTotal.onfocusout = function () { CalculoAuxiliarProvDeudor(DivContenidoAux1.id) };
    DivMontoTotal.appendChild(InputMontoTotal);
    DivContenidoAux1.appendChild(DivMontoTotal);

    //content eliminar fila
    let divEliminarAuxiliar = document.createElement("div");
    divEliminarAuxiliar.className = "form-group col-lg-1";
    let buttonFila = document.createElement("button");
    buttonFila.type = "button";
    buttonFila.className = "btn btn-danger btn-sm redondo btnPress";
    buttonFila.onclick = BorrarFilaAuxiliarProvDeudor;
    buttonFila.tabIndex = -1;
    buttonFila.textContent = "X";
    divEliminarAuxiliar.appendChild(buttonFila);
    DivContenidoAux1.appendChild(divEliminarAuxiliar);
    // $(".estiloSelectTipoDTE"+IdDivAuxProvDeudor).select2();
}
var IdDivAuxRemu = 0;
const CrearTablaAuxRemu = () => {
    IdDivAuxRemu = IdDivAuxRemu + 1;
    let DivAuxRemuPadre = document.getElementById("TablaRemuAux");
    let DivAuxRemu = document.createElement("div");
    DivAuxRemu.className = "form-group col-lg-12 GuardarDatosAuxRemu";
    DivAuxRemu.id = "detalleRemu" + IdDivAuxRemu;
    DivAuxRemuPadre.appendChild(DivAuxRemu);

    //folio remuneracion
    let DivRemuFolio = document.createElement("div");
    DivRemuFolio.className = "col-lg-3";
    let InputFolioRemu = document.createElement("input");
    InputFolioRemu.className = "form-control";
    InputFolioRemu.type = "number";
    InputFolioRemu.min = 1;
    InputFolioRemu.id = "AuxFolio"+IdDivAuxRemu;
    InputFolioRemu.name = "AuxFolio";
    InputFolioRemu.required = true;
    DivRemuFolio.appendChild(InputFolioRemu);
    DivAuxRemu.appendChild(DivRemuFolio);

    //sueldo líquido
    let DivSueldoLiquido = document.createElement("div");
    DivSueldoLiquido.className = "col-lg-5";
    let InputSueldoLiquido = document.createElement("input");
    InputSueldoLiquido.className = "form-control";
    InputSueldoLiquido.id = "AuxTotalRemu"+IdDivAuxRemu;
    InputSueldoLiquido.name = "AuxTotalRemu";
    InputSueldoLiquido.type = "number";
    InputSueldoLiquido.value = 0;
    InputSueldoLiquido.required = true;
    InputSueldoLiquido.onchange = SumaSueldoLiquidoRemu;
    DivSueldoLiquido.appendChild(InputSueldoLiquido);
    DivAuxRemu.appendChild(DivSueldoLiquido);

    //content eliminar fila
    let divElimFila = document.createElement("div");
    divElimFila.className = "form-group col-lg-1 col-lg-offset-3";
    let btnEliminarFila = document.createElement("button");
    btnEliminarFila.type = "button";
    btnEliminarFila.className = "btn btn-danger btn-sm redondo btnPress";
    btnEliminarFila.onclick = BorrarFilaAuxRemu;
    btnEliminarFila.tabIndex = -1;
    btnEliminarFila.textContent = "X";
    divElimFila.appendChild(btnEliminarFila);
    DivAuxRemu.appendChild(divElimFila);
}
var IdAuxHonorarios = 0;
const CrearTablaHonorariosAux = () => {
    IdAuxHonorarios = IdAuxHonorarios + 1;
    let GetDivHonorariosAux = document.getElementById("TablaHonorAux");
    let DivHonoAux = document.createElement("div");
    DivHonoAux.id = "detalleHono" + IdAuxHonorarios;
    DivHonoAux.className = "form-group col-lg-12 GuardarDatosAuxHonor";
    GetDivHonorariosAux.appendChild(DivHonoAux);

    //folio
    let DivFolioHonor = document.createElement("div");
    DivFolioHonor.className = "form-group col-lg-2";
    let InputFolio = document.createElement("input");
    InputFolio.type = "number";
    InputFolio.className = "form-control";
    InputFolio.min = 1;
    InputFolio.id = "AuxFolio"+IdAuxHonorarios;
    InputFolio.name = "AuxFolio";
    InputFolio.required = true;
    DivFolioHonor.appendChild(InputFolio);
    DivHonoAux.appendChild(DivFolioHonor);

    //valor bruto
    let DivValorBruto = document.createElement("div");
    DivValorBruto.className = "form-group col-lg-2";
    let InputValorBruto = document.createElement("input");
    InputValorBruto.type = "number";
    InputValorBruto.className = "form-control";
    InputValorBruto.name = "AuxValorBruto";
    InputValorBruto.id = "AuxValorBruto"+IdAuxHonorarios;
    InputValorBruto.value = 0;
    InputValorBruto.required = true;
    InputValorBruto.onchange = function () { CalculoAuxRetencion(DivHonoAux.id) };
    DivValorBruto.appendChild(InputValorBruto);
    DivHonoAux.appendChild(DivValorBruto);
    
    //tipo retención
    let DivTipoRetencion = document.createElement("div");
    DivTipoRetencion.className = "form-group col-lg-3";
    let SelectTipoRetencion = document.createElement("select");
    SelectTipoRetencion.className = "form-control estiloSelectHonor"+IdAuxHonorarios;
    SelectTipoRetencion.id = "AUXtipoRetencion"+IdAuxHonorarios;
    SelectTipoRetencion.name = "AUXtipoRetencion"+IdAuxHonorarios;
    SelectTipoRetencion.required = true;
    SelectTipoRetencion.onchange = function () { CalculoAuxRetencion(DivHonoAux.id) };
    let OptionDefault = document.createElement("option");
    OptionDefault.selected = true;
    OptionDefault.value = "seleccionar";
    OptionDefault.text = "Selecciona";
    let OptionRet10 = document.createElement("option");
    OptionRet10.value = "Ret10";
    OptionRet10.text = "Retención 10%";
    let OptionRet1075 = document.createElement("option");
    OptionRet1075.value = "Ret105";
    OptionRet1075.text = "Retención 10.75%";
    let OptionRet1150 = document.createElement("option");
    OptionRet1150.value = "Ret1150";
    OptionRet1150.text = "Retención 11.5%";
    let OptionRet20 = document.createElement("option");
    OptionRet20.value = "Ret20";
    OptionRet20.text = "Retención 20%";
    let OptionSinRet = document.createElement("option");
    OptionSinRet.value = "RetNo";
    OptionSinRet.text = "Sin Retención";
    SelectTipoRetencion.add(OptionDefault);
    SelectTipoRetencion.add(OptionRet10);
    SelectTipoRetencion.add(OptionRet1075);
    SelectTipoRetencion.add(OptionRet1150);
    SelectTipoRetencion.add(OptionRet20);
    SelectTipoRetencion.add(OptionSinRet);
    DivTipoRetencion.appendChild(SelectTipoRetencion);
    DivHonoAux.appendChild(DivTipoRetencion);

    //retención
    let DivRetencion = document.createElement("div");
    DivRetencion.className = "form-group col-lg-2";
    let InputRetencion = document.createElement("input");
    InputRetencion.type = "number";
    InputRetencion.className = "form-control";
    InputRetencion.id="AUXValorRetencion"+IdAuxHonorarios;
    InputRetencion.name = "AUXValorRetencion";
    InputRetencion.value = 0;
    InputRetencion.disabled = true;
    InputRetencion.required = true;
    DivRetencion.appendChild(InputRetencion);
    DivHonoAux.appendChild(DivRetencion);

    //valor líquido
    let DivValorLiquido = document.createElement("div");
    DivValorLiquido.className = "form-group col-lg-2";
    let InputValorLiq = document.createElement("input");
    InputValorLiq.type = "number";
    InputValorLiq.className = "form-control";
    InputValorLiq.name = "AUXValorLiquido";
    InputValorLiq.id="AUXValorLiquido"+IdAuxHonorarios;
    InputValorLiq.value = 0;
    InputValorLiq.disabled = true;
    InputValorLiq.readOnly = true;
    DivValorLiquido.appendChild(InputValorLiq);
    DivHonoAux.appendChild(DivValorLiquido);

    //btn borrar
    let DivBtnBorrar = document.createElement("div");
    DivBtnBorrar.className = "form-group col-lg-1";
    let BtnBorrarHono = document.createElement("button");
    BtnBorrarHono.className = "btn btn-danger btn-sm redondo btnPress";
    BtnBorrarHono.type = "button";
    BtnBorrarHono.tabIndex = -1;
    BtnBorrarHono.onclick = BorrarFilaAuxHonor;
    BtnBorrarHono.textContent = "X";
    DivBtnBorrar.appendChild(BtnBorrarHono);
    DivHonoAux.appendChild(DivBtnBorrar);
    $(".estiloSelectHonor"+IdAuxHonorarios).select2();
}


document.getElementById("Btn_AgregarVoucher").addEventListener('click', function () {
    CrearTablaVoucher();
});
document.getElementById("Btn_AgregaFilaProvDeudor").addEventListener('click', function () {
    CrearTablaAuxProvDeudor();
    if(document.getElementById("BoletaVenta").checked){
        EsUnaBoleta();
    }
});
document.getElementById("Btn_AgregaFilaRemu").addEventListener('click', function () {
    CrearTablaAuxRemu();
});
document.getElementById("Btn_AgregaFilaHonorarios").addEventListener('click', function () {
    CrearTablaHonorariosAux();
});


function BorrarFilaVoucher(IdRow,IdBtn) {
    //DELETE
    let DivPadreABorrar = document.getElementById('ContenidoDetalle'); //toma el div padre 
    let FilaABorrar = document.getElementById(IdRow); //toma el div hijo que se borrará 

    if (idDetalle > 1) {
        DivPadreABorrar.removeChild(FilaABorrar); //borra div hijo
        AuxiliaresDetalle.splice(IdBtn-1,1); //borra dentro del array el auxiliar correspondiente al div que se eliminó
        console.log(AuxiliaresDetalle);
        idDetalle--;
        SumaTotalesVoucher();
        ReordenarFila();
    }
}
function BorrarFilaAuxiliarProvDeudor() {
    if (IdDivAuxProvDeudor > 1) {
        jQuery('#detalleAuxProvDeudor' + IdDivAuxProvDeudor).remove();
        IdDivAuxProvDeudor--;
        CuadrarValorProvDeudor();
    }
}
function BorrarFilaAuxRemu() {
    if (IdDivAuxRemu > 1) {
        jQuery('#detalleRemu' + IdDivAuxRemu).remove();
        IdDivAuxRemu--;
        CuadrarValorAuxRemu();
    }
}
function BorrarFilaAuxHonor() {
    if (IdAuxHonorarios > 1) {
        jQuery('#detalleHono' + IdAuxHonorarios).remove();
        IdAuxHonorarios--;
        CuadrarValorAuxRetencion();
    }
}


const RellenarTablaVoucher = (content) => {
    idDetalle = idDetalle + 1;
    let glosa = content.GlosaDetalle;
    let DivPadre = document.getElementById("ContenidoDetalle");
    let DivTabla = document.createElement("div");
    DivTabla.id = "detalle" + idDetalle;
    DivTabla.className = "form-group col-lg-12 GuardarDatosVoucher";
    DivPadre.appendChild(DivTabla);
    
    //creacion de btnAuxiliar
    let DivBtnAux = document.createElement("div");
    DivBtnAux.className = "form-group col-lg-1";
    let BtnAux = document.createElement("button");
    BtnAux.className = "btn btn-success btn-sm redondo btnPress";
    BtnAux.type = "button";
    BtnAux.id = idDetalle;
    BtnAux.textContent = "AUX";
    BtnAux.onclick = function () { ClickAux(DivTabla.id,content.AuxiliarDetalle,BtnAux.id) };
    BtnAux.disabled = true;
    DivBtnAux.appendChild(BtnAux);
    DivTabla.appendChild(DivBtnAux);

    //creacion select Cuenta Contable
    let DivCuentaContable = document.createElement("div");
    DivCuentaContable.className = "form-group col-lg-2";
    let SelectCuentaContable = document.createElement("select");
    SelectCuentaContable.className = "form-control";
    SelectCuentaContable.onchange = function () { ActivaBtnAuxNuevo(DivTablaVoucher.id) };
    SelectCuentaContable.required = true;
    SelectCuentaContable.id="ctacont"+idDetalle;
    SelectCuentaContable.name="ctacont";
    SelectCuentaContable.innerHTML = GetLstCuentacontable.result;
    DivCuentaContable.appendChild(SelectCuentaContable);
    DivTabla.appendChild(DivCuentaContable);
    
    //creacion input glosa
    let DivInputGlosa = document.createElement("div");
    DivInputGlosa.className = "form-group col-lg-2";
    let InputGlosa = document.createElement("input");
    InputGlosa.className = "selectpicker form-control";
    InputGlosa.type = "text";
    InputGlosa.id = "glosaDetalle"+idDetalle;
    InputGlosa.name = "glosaDetalle";
    InputGlosa.type = "text";
    InputGlosa.value = glosa;
    InputGlosa.required = true;
    DivInputGlosa.appendChild(InputGlosa);
    DivTabla.appendChild(DivInputGlosa);

    //creación centro costo
    let DivCentroCosto = document.createElement("div");
    DivCentroCosto.className = "form-group col-lg-2";
    let SelectCC = document.createElement("select");
    SelectCC.className = "form-control";
    SelectCC.id="centrocosto"+idDetalle;
    SelectCC.name="centrocosto";
    SelectCC.innerHTML = GetLstCentroDeCostos.result;
    DivCentroCosto.appendChild(SelectCC);
    DivTabla.appendChild(DivCentroCosto);
   
    //creación Debe
    let DivMontoDebe = document.createElement("div");
    DivMontoDebe.className = "form-group col-lg-2";
    let InputDebe = document.createElement("input");
    InputDebe.className = "selectpicker form-control";
    InputDebe.type = "number";
    InputDebe.id = "debe"+idDetalle;
    InputDebe.name = "debe";
    InputDebe.min = 0;
    InputDebe.value = content.MontoDebe;
    InputDebe.onchange = SumaTotalesVoucher;
    DivMontoDebe.appendChild(InputDebe);
    DivTabla.appendChild(DivMontoDebe);

    //Creación Haber
    let DivMontoHaber = document.createElement("div");
    DivMontoHaber.className = "form-group col-lg-2";
    let InputHaber = document.createElement("input");
    InputHaber.className = "selectpicker form-control";
    InputHaber.type = "number";
    InputHaber.id = "haber"+idDetalle;
    InputHaber.name = "haber";
    InputHaber.min = 0;
    InputHaber.value = content.MontoHaber;
    InputHaber.onchange = SumaTotalesVoucher;
    DivMontoHaber.appendChild(InputHaber);
    DivTabla.appendChild(DivMontoHaber);

    //creacipon BtnEliminar
    let DivBtnEliminar = document.createElement("div");
    DivBtnEliminar.className = "form-group col-lg-1";
    let BtnEliminar = document.createElement("button");
    BtnEliminar.className = "btn btn-danger btn-sm redondo btnPress";
    BtnEliminar.name = "borrarTabla"+idDetalle;
    BtnEliminar.type = "button";
    BtnEliminar.textContent = "X";
    BtnEliminar.onclick = function(){BorrarFilaVoucher(DivTabla.id,BtnAux.id)};
    DivBtnEliminar.appendChild(BtnEliminar);
    DivTabla.appendChild(DivBtnEliminar);
}
const RellenarTablaAuxProvDeudor = (data) => {
    IdDivAuxProvDeudor = IdDivAuxProvDeudor + 1;
    let ContenidoAux = document.getElementById("TablaProvDeudorAux");
    let DivContenidoAux = document.createElement("div");
    DivContenidoAux.id = "detalleAuxProvDeudor" + IdDivAuxProvDeudor;
    DivContenidoAux.className = "form-group col-lg-12 GuardarDatosAuxProvDeudor";
    ContenidoAux.appendChild(DivContenidoAux);

    //content folio desde

    let DivFolioDesde = document.createElement("div");
    DivFolioDesde.className="form-group col-lg-1 DivFolioDesde";
    DivFolioDesde.style.display="none";
    DivFolioDesde.id="AuxFolioDesde"+IdDivAuxProvDeudor;
    let InputFolioDesde = document.createElement("input");
    InputFolioDesde.className="form-control";
    InputFolioDesde.id = "AuxInputFolioDesde"+IdDivAuxProvDeudor;
    InputFolioDesde.name = "AuxInputFolioDesde";
    InputFolioDesde.min = 1;
    InputFolioDesde.required = true;
    InputFolioDesde.type = "number";
    DivFolioDesde.appendChild(InputFolioDesde);
    DivContenidoAux.appendChild(DivFolioDesde);

    //content folio/folioHasta
    let DivFolio = document.createElement("div");
    DivFolio.className = "form-group col-lg-1";
    let InputFolio = document.createElement("input");
    InputFolio.className = "form-control";
    InputFolio.name = "AuxFolioHasta";
    InputFolio.id="AuxFolioHasta"+IdDivAuxProvDeudor;
    InputFolio.type = "text";
    InputFolio.value = data.Folio;
    InputFolio.required = true;
    DivFolio.appendChild(InputFolio);
    DivContenidoAux.appendChild(DivFolio);

    //content TipoDTE
    let DivTipoDTE = document.createElement("div");
    DivTipoDTE.className = "form-group col-lg-2";
    let selectTipoDte = document.createElement("select");
    selectTipoDte.className = "form-control";
    selectTipoDte.required=true;
    selectTipoDte.id="TipoDTE"+IdDivAuxProvDeudor;
    selectTipoDte.name="TipoDTE";
    selectTipoDte.innerHTML=GetLstTipoDTE.result;
    DivTipoDTE.appendChild(selectTipoDte);
    DivContenidoAux.appendChild(DivTipoDTE);

    //contentMonto neto
    let DivMontoNeto = document.createElement("div");
    DivMontoNeto.className = "form-group col-lg-2";
    let InputMontoNeto = document.createElement("input");
    InputMontoNeto.className = "form-control";
    InputMontoNeto.id = "ValorNeto"+IdDivAuxProvDeudor;
    InputMontoNeto.name = "ValorNeto";
    InputMontoNeto.type = "number";
    InputMontoNeto.min = 0;
    InputMontoNeto.value = data.MontoNeto;
    InputMontoNeto.disabled = true;
    DivMontoNeto.appendChild(InputMontoNeto);
    DivContenidoAux.appendChild(DivMontoNeto);

    //content monto exento
    let DivMontoExento = document.createElement("div");
    DivMontoExento.className = "form-group col-lg-2";
    let InputMontoExento = document.createElement("input");
    InputMontoExento.className = "form-control";
    InputMontoExento.id = "MontoExento"+IdDivAuxProvDeudor;
    InputMontoExento.name = "MontoExento";
    InputMontoExento.type = "number";
    InputMontoExento.min = 0;
    InputMontoExento.value = data.MontoExento;
    InputMontoExento.onfocusout = function () {CalculoAuxMontoExento(DivContenidoAux.id)};
    DivMontoExento.appendChild(InputMontoExento);
    DivContenidoAux.appendChild(DivMontoExento);

    //content monto iva
    let DivMontoIVA = document.createElement("div");
    DivMontoIVA.className = "form-group col-lg-2 DMontoIVA";
    let InputIVA = document.createElement("input");
    InputIVA.id = "MontoIVA"+IdDivAuxProvDeudor;
    InputIVA.name = "MontoIVA";
    InputIVA.type = "number";
    InputIVA.disabled = true;
    InputIVA.min = 0;
    InputIVA.value = data.MontoIvaLinea;
    InputIVA.className = "form-control";
    DivMontoIVA.appendChild(InputIVA);
    DivContenidoAux.appendChild(DivMontoIVA);

    //content monto total
    let DivMontoTotal = document.createElement("div");
    DivMontoTotal.className = "form-group col-lg-2";
    let InputMontoTotal = document.createElement("input");
    InputMontoTotal.className = "form-control";
    InputMontoTotal.type = "number";
    InputMontoTotal.id = "MontoTotal"+IdDivAuxProvDeudor;
    InputMontoTotal.name = "MontoTotal";
    InputMontoTotal.min = 0;
    InputMontoTotal.value = data.MontoTotalLinea;
    InputMontoTotal.required=true;
    InputMontoTotal.onfocusout = function () { CalculoAuxiliarProvDeudor(DivContenidoAux.id) };
    DivMontoTotal.appendChild(InputMontoTotal);
    DivContenidoAux.appendChild(DivMontoTotal);

    //content eliminar fila
    let divEliminarAuxiliar = document.createElement("div");
    divEliminarAuxiliar.className = "form-group col-lg-1";
    let buttonFila = document.createElement("button");
    buttonFila.type = "button";
    buttonFila.className = "btn btn-danger btn-sm redondo btnPress";
    buttonFila.onclick = BorrarFilaAuxiliarProvDeudor;
    buttonFila.tabIndex = -1;
    buttonFila.textContent = "X";
    divEliminarAuxiliar.appendChild(buttonFila);
    DivContenidoAux.appendChild(divEliminarAuxiliar);
    document.getElementById("TipoDTE"+IdDivAuxProvDeudor).value = data.TipoDTE;
    MostrarDatosProvDeudor(data);
}
const RellenarTablaAuxRemu = (AuxDetalle) => {
    IdDivAuxRemu = IdDivAuxRemu + 1;
    let DivAuxRemuPadre = document.getElementById("TablaRemuAux");
    let DivAuxRemu = document.createElement("div");
    DivAuxRemu.className = "form-group col-lg-12";
    DivAuxRemu.id = "detalleRemu" + IdDivAuxRemu;
    DivAuxRemuPadre.appendChild(DivAuxRemu);

    //folio remuneracion
    let DivRemuFolio = document.createElement("div");
    DivRemuFolio.className = "col-lg-3";
    let InputFolioRemu = document.createElement("input");
    InputFolioRemu.className = "form-control";
    InputFolioRemu.type = "number";
    InputFolioRemu.min = 1;
    InputFolioRemu.id = "AuxFolio"+IdDivAuxRemu;
    InputFolioRemu.name = "AuxFolio";
    InputFolioRemu.value = AuxDetalle.Folio;
    InputFolioRemu.required = true;
    DivRemuFolio.appendChild(InputFolioRemu);
    DivAuxRemu.appendChild(DivRemuFolio);

    //sueldo líquido
    let DivSueldoLiquido = document.createElement("div");
    DivSueldoLiquido.className = "col-lg-5";
    let InputSueldoLiquido = document.createElement("input");
    InputSueldoLiquido.className = "form-control";
    InputSueldoLiquido.id = "AuxTotalRemu"+IdDivAuxRemu;
    InputSueldoLiquido.name = "AuxTotalRemu";
    InputSueldoLiquido.type = "number";
    InputSueldoLiquido.value = AuxDetalle.MontoTotalLinea;
    InputSueldoLiquido.required = true;
    InputSueldoLiquido.onchange = SumaSueldoLiquidoRemu;
    DivSueldoLiquido.appendChild(InputSueldoLiquido);
    DivAuxRemu.appendChild(DivSueldoLiquido);

    //content eliminar fila
    let divElimFila = document.createElement("div");
    divElimFila.className = "form-group col-lg-1 col-lg-offset-3";
    let btnEliminarFila = document.createElement("button");
    btnEliminarFila.type = "button";
    btnEliminarFila.className = "btn btn-danger btn-sm redondo btnPress";
    btnEliminarFila.onclick = BorrarFilaAuxRemu;
    btnEliminarFila.tabIndex = -1;
    btnEliminarFila.textContent = "X";
    divElimFila.appendChild(btnEliminarFila);
    DivAuxRemu.appendChild(divElimFila);

    MostrarDatosRemu(AuxDetalle);
}
const RellenarTablaHonorariosAux = (AuxDetalle) => {
    IdAuxHonorarios = IdAuxHonorarios + 1;
    let GetDivHonorariosAux = document.getElementById("TablaHonorAux");
    let DivHonoAux = document.createElement("div");
    DivHonoAux.id = "detalleHono" + IdAuxHonorarios;
    DivHonoAux.className = "form-group col-lg-12 GuardarDatosAuxHonor";
    GetDivHonorariosAux.appendChild(DivHonoAux);

    //folio
    let DivFolioHonor = document.createElement("div");
    DivFolioHonor.className = "form-group col-lg-2";
    let InputFolio = document.createElement("input");
    InputFolio.type = "number";
    InputFolio.className = "form-control";
    InputFolio.min = 1;
    InputFolio.id = "AuxFolio"+IdAuxHonorarios;
    InputFolio.name = "AuxFolio";
    InputFolio.value = AuxDetalle.Folio;
    InputFolio.required = true;
    DivFolioHonor.appendChild(InputFolio);
    DivHonoAux.appendChild(DivFolioHonor);

    //valor bruto
    let DivValorBruto = document.createElement("div");
    DivValorBruto.className = "form-group col-lg-2";
    let InputValorBruto = document.createElement("input");
    InputValorBruto.type = "number";
    InputValorBruto.className = "form-control";
    InputValorBruto.name = "AuxValorBruto";
    InputValorBruto.id = "AuxValorBruto"+IdAuxHonorarios;
    InputValorBruto.value = AuxDetalle.MontoBruto;
    InputValorBruto.required = true;
    InputValorBruto.onchange = function () { CalculoAuxRetencion(DivHonoAux.id) };
    DivValorBruto.appendChild(InputValorBruto);
    DivHonoAux.appendChild(DivValorBruto);

    
    //tipo retención
    let DivTipoRetencion = document.createElement("div");
    DivTipoRetencion.className = "form-group col-lg-3";
    let SelectTipoRetencion = document.createElement("select");
    SelectTipoRetencion.className = "form-control estiloSelectHonor"+IdAuxHonorarios;
    SelectTipoRetencion.id = "AUXtipoRetencion"+IdAuxHonorarios;
    SelectTipoRetencion.name = "AUXtipoRetencion";
    SelectTipoRetencion.required = true;
    SelectTipoRetencion.onchange = function () { CalculoAuxRetencion(DivHonoAux.id) };
    let OptionDefault = document.createElement("option");
    OptionDefault.value = "seleccionar";
    OptionDefault.text = "Selecciona";
    let OptionRet10 = document.createElement("option");
    OptionRet10.value = "Ret10";
    OptionRet10.text = "Retención 10%";
    let OptionRet1075 = document.createElement("option");
    OptionRet1075.value = "Ret105";
    OptionRet1075.text = "Retención 10.75%";
    let OptionRet1150 = document.createElement("option");
    OptionRet1150.value = "Ret1150";
    OptionRet1150.text = "Retención 11.5%";
    let OptionRet20 = document.createElement("option");
    OptionRet20.value = "Ret20";
    OptionRet20.text = "Retención 20%";
    let OptionSinRet = document.createElement("option");
    OptionSinRet.value = "RetNo";
    OptionSinRet.text = "Sin Retención";
    SelectTipoRetencion.add(OptionDefault);
    SelectTipoRetencion.add(OptionRet10);
    SelectTipoRetencion.add(OptionRet1075);
    SelectTipoRetencion.add(OptionRet1150);
    SelectTipoRetencion.add(OptionRet20);
    SelectTipoRetencion.add(OptionSinRet);
    DivTipoRetencion.appendChild(SelectTipoRetencion);
    DivHonoAux.appendChild(DivTipoRetencion);
    $(".estiloSelectHonor"+IdAuxHonorarios).select2();

    //retención
    let DivRetencion = document.createElement("div");
    DivRetencion.className = "form-group col-lg-2";
    let InputRetencion = document.createElement("input");
    InputRetencion.type = "number";
    InputRetencion.className = "form-control";
    InputRetencion.name = "AUXValorRetencion";
    InputRetencion.id="AUXValorRetencion"+IdAuxHonorarios;
    InputRetencion.value=AuxDetalle.MontoRetencion;
    InputRetencion.disabled = true;
    InputRetencion.required = true;
    DivRetencion.appendChild(InputRetencion);
    DivHonoAux.appendChild(DivRetencion);

    //valor líquido
    let DivValorLiquido = document.createElement("div");
    DivValorLiquido.className = "form-group col-lg-2";
    let InputValorLiq = document.createElement("input");
    InputValorLiq.type = "number";
    InputValorLiq.className = "form-control";
    InputValorLiq.name = "AUXValorLiquido";
    InputValorLiq.id="AUXValorLiquido"+IdAuxHonorarios;
    InputValorLiq.value = AuxDetalle.ValorLiquido;
    InputValorLiq.disabled = true;
    InputValorLiq.readOnly = true;
    DivValorLiquido.appendChild(InputValorLiq);
    DivHonoAux.appendChild(DivValorLiquido);

    //btn borrar
    let DivBtnBorrar = document.createElement("div");
    DivBtnBorrar.className = "form-group col-lg-1";
    let BtnBorrarHono = document.createElement("button");
    BtnBorrarHono.className = "btn btn-danger btn-sm redondo btnPress";
    BtnBorrarHono.type = "button";
    BtnBorrarHono.tabIndex = -1;
    BtnBorrarHono.onclick = BorrarFilaAuxHonor;
    BtnBorrarHono.textContent = "X";
    DivBtnBorrar.appendChild(BtnBorrarHono);
    DivHonoAux.appendChild(DivBtnBorrar);
    MostrarDatosHonor(AuxDetalle);
}


const EnviarDatosAuxProvDeudor = async(NombreCuentaContable,IdCtaContable, idRow, idBtnLinea) => {
    let DivPadreBtn = document.getElementById(idRow);
    let DivHijosBtn = DivPadreBtn.childNodes;

    let MontoDebe = DivHijosBtn[4].firstChild;
    let MontoHaber = DivHijosBtn[5].firstChild;
    let SelectCtaContableProv = document.getElementById("AuxCuentaProvDeudor");
    let option1 = document.createElement("option")
    option1.value = IdCtaContable;
    option1.textContent=NombreCuentaContable;
    SelectCtaContableProv.appendChild(option1);
    document.getElementById("AUXitemProvDeudor").value = idBtnLinea;
    document.getElementById("AUXvalorProvDeudor").value = 0;
    
    if (MontoDebe.value != 0 && MontoDebe.value != "" && MontoDebe.value != null) {
        document.getElementById("AUXvalorProvDeudor").value = MontoDebe.value;
    }
    
    if (MontoHaber.value != 0 && MontoHaber.value != "" && MontoHaber.value != null) {
        document.getElementById("AUXvalorProvDeudor").value = MontoHaber.value;
    }
}
const EnviarDatosHonorAux = async(NombreCuentaContable,idCtaContable, idRow,idBtnLinea) => {
    let DivPadreBtn = document.getElementById(idRow);
    let DivHijosBtn = DivPadreBtn.childNodes;
    let MontoDebe = DivHijosBtn[4].firstChild;
    let MontoHaber = DivHijosBtn[5].firstChild;

    let SelectCtaContableHonor = document.getElementById("AuxCuenta");
    let OptionSelected = document.createElement("option")
    OptionSelected.value=idCtaContable;
    OptionSelected.textContent=NombreCuentaContable;
    SelectCtaContableHonor.appendChild(OptionSelected);
    document.getElementById("AUXitem").value = idBtnLinea;
    document.getElementById("AuxValorHonor").value = 0;

    if (MontoDebe.value != 0 && MontoDebe.value != "" && MontoDebe.value != null) {
        document.getElementById("AuxValorHonor").value = MontoDebe.value;
    }

    if (MontoHaber.value != 0 && MontoHaber.value != "" && MontoHaber.value != null) {
        document.getElementById("AuxValorHonor").value = MontoHaber.value;
    }
}
const EnviarDatosRemuneracion = (NombreCuentaContable,IdCtaContable, idRow, idBtnLinea) => {
    let DivPadreBtn = document.getElementById(idRow);
    let DivHijosBtn = DivPadreBtn.childNodes;
    let MontoDebe = DivHijosBtn[4].firstChild;
    let MontoHaber = DivHijosBtn[5].firstChild;
    
    let SelectCtaContableRemu = document.getElementById("AuxRemu");
    let OptionSelect = document.createElement("option");
    OptionSelect.value = IdCtaContable;
    OptionSelect.textContent = NombreCuentaContable;
    SelectCtaContableRemu.appendChild(OptionSelect);
    document.getElementById("AUXitemRemu").value = idBtnLinea;
    document.getElementById("AUXvaloritemRemu").value = 0;


    if (MontoDebe.value != 0 && MontoDebe.value != "" && MontoDebe.value != null) {
        document.getElementById("AUXvaloritemRemu").value = MontoDebe.value;
    }

    if (MontoHaber.value != 0 && MontoHaber.value != "" && MontoHaber.value != null) {
        document.getElementById("AUXvaloritemRemu").value = MontoHaber.value;
    }
}

const AuxPrestadorSeleccionado = async(razonsocialID,rut)=> {
    let PrestadorSeleccionado="";
    let PrestadorProvDeudor = document.getElementById("TipoPrestadorProvDeudor").selectedOptions[0].value;
    let PrestadorRemu = document.getElementById("TipoPrestadorRemu").selectedOptions[0].value;
    let PrestadorHonor = document.getElementById("TipoPrestadorHonor").selectedOptions[0].value;

    if(PrestadorProvDeudor!=""){
        PrestadorSeleccionado = PrestadorProvDeudor;
        let GetResRazonSocial = await GetRazonSocial(PrestadorSeleccionado);
        document.getElementById("RazonPrestadorProvDeudor").innerHTML = GetResRazonSocial.selectInput;
        document.getElementById("RazonPrestadorProvDeudor").value = razonsocialID;
        document.getElementById("RutPrestadorProvDeudor").value = rut;
        // GuardarAuxProvDeudor(razonsocialID);
    }
    if(PrestadorRemu!=""){
        PrestadorSeleccionado = PrestadorRemu;
        let GetResRazonSocial = await GetRazonSocial(PrestadorSeleccionado);
        document.getElementById("RazonPrestadorRemu").innerHTML = GetResRazonSocial.selectInput;
        document.getElementById("RazonPrestadorRemu").value = razonsocialID;
        document.getElementById("RutPrestadorRemu").value = rut;
    }
    if(PrestadorHonor!=""){
        PrestadorSeleccionado = PrestadorHonor;
        let GetResRazonSocial = await GetRazonSocial(PrestadorSeleccionado);
        document.getElementById("RazonPrestadorHonor").innerHTML = GetResRazonSocial.selectInput;
        document.getElementById("RazonPrestadorHonor").value = razonsocialID;
        document.getElementById("RutPrestadorHonor").value = rut;
        // GuardarAuxHonor(razonsocialID);
    }
}
const AuxSelectPrestadorSeleccionado=async()=>{
    let PrestadorSeleccionado="";
    let PrestadorProvDeudor = document.getElementById("TipoPrestadorProvDeudor").selectedOptions[0].value;
    let PrestadorRemu = document.getElementById("TipoPrestadorRemu").selectedOptions[0].value;
    let PrestadorHonor = document.getElementById("TipoPrestadorHonor").selectedOptions[0].value;

    if(PrestadorProvDeudor!=""){
        PrestadorSeleccionado = PrestadorProvDeudor;
        let GetLstRazonSocial = await GetRazonSocial(PrestadorSeleccionado);
        document.getElementById("RazonPrestadorProvDeudor").innerHTML = GetLstRazonSocial.selectInput;
    }
    if(PrestadorRemu!=""){
        PrestadorSeleccionado = PrestadorRemu;
        let GetResRazonSocial = await GetRazonSocial(PrestadorSeleccionado);
        document.getElementById("RazonPrestadorRemu").innerHTML = GetResRazonSocial.selectInput;
    }
    if(PrestadorHonor!=""){
        PrestadorSeleccionado = PrestadorHonor;
        let GetResRazonSocial = await GetRazonSocial(PrestadorSeleccionado);
        document.getElementById("RazonPrestadorHonor").innerHTML = GetResRazonSocial.selectInput;
    }    
}
const ObtenerRUTPresSeleccionado=()=> {
    let Url = "ObtenerRutPrestador/Contabilidad";
    let PrestadorIDProvDeudor = document.getElementById("RazonPrestadorProvDeudor").value;
    let PrestadorIDRemu = document.getElementById("RazonPrestadorRemu").value;
    let PrestadorIDHonor = document.getElementById("RazonPrestadorHonor").value;
    
    if(PrestadorIDProvDeudor!="" && PrestadorIDProvDeudor!="Selecciona"){
        $.getJSON(Url, { IDPrestador: PrestadorIDProvDeudor }, function (data) {
            if (data.ok == true) {
                $('#RutPrestadorProvDeudor').val(data.RutPrestador);
            }
        });
    }
    
    if(PrestadorIDRemu!="" && PrestadorIDRemu!="Selecciona"){
        $.getJSON(Url, { IDPrestador: PrestadorIDRemu }, function (data) {
            if (data.ok == true) {
                $('#RutPrestadorRemu').val(data.RutPrestador);
            }
        });
    }

    if(PrestadorIDHonor!="" && PrestadorIDHonor!="Selecciona"){
        $.getJSON(Url, { IDPrestador: PrestadorIDHonor }, function (data) {
            if (data.ok == true) {
                $('#RutPrestadorHonor').val(data.RutPrestador);
            }
        });
    }

    
}

const EsUnaBoleta=()=>{
    
    let DivFolioDesde = document.getElementsByClassName("DivFolioDesde");
    let DivFolioHasta = document.getElementsByClassName("DMontoIVA");
    let labelFolioDesde = document.querySelector(".FolioDesdeL");
    let labelIVA = document.querySelector(".MontoIL");
    
    if(document.getElementById("BoletaVenta").checked){

        document.getElementById("FolioHastaL").innerText="Folio Hasta";
        labelFolioDesde.style.display="";
        labelIVA.className="col-lg-1 MontoIL";
        
        for (let i = 0; i < DivFolioDesde.length && i < DivFolioHasta.length; i++) {
            DivFolioDesde[i].style.display="";
            DivFolioHasta[i].className = "form-group col-lg-1 DMontoIVA";
            
        }
    }else{
        document.getElementById("FolioHastaL").innerText="Folio";
        labelFolioDesde.style.display="none";
        labelIVA.className="col-lg-2 MontoIL";

        for (let i = 0; i < DivFolioDesde.length && i < DivFolioHasta.length; i++) {
            DivFolioDesde[i].style.display="none";
            DivFolioHasta[i].className = "form-group col-lg-2 DMontoIVA";
        }
    }

}
const ReordenarFila=()=>{
    let VouchersExistentes = document.getElementsByClassName("GuardarDatosVoucher");
    for (let i = 0; i < VouchersExistentes.length; i++) {
        const BtnAuxID = VouchersExistentes[i].firstChild.firstChild;
        BtnAuxID.id = i+1;
        VouchersExistentes[i].id="detalle"+(i+1);
        if (AuxiliaresDetalle[i]!=null) {
            AuxiliaresDetalle[i].NumeroLinea = i+1;
        }
    }
}

// AgregarEstiloSelect();