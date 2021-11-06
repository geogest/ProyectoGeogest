document.addEventListener("DOMContentLoaded", function () {
    LstCuentasContables();
});
var CuentasContables;
const LstCuentasContables = () => {
    $.ajax({
        type: 'GET',
        url: "getLstCuentasContables/Contabilidad",
        success: function (result) {

            if (result.ok) {
                CuentasContables = result.result;
                CrearTablaVoucher();
            }

        }
        
    });
}

var idDetalle = 0;
const CrearTablaVoucher = () => {
    var glosa = $("input[name=glosaDetalle]:last").val();
    if (glosa == null) {
        glosa = "";
    }

    idDetalle = idDetalle + 1;

    let DivPadre = document.getElementById("ContenidoDetalle");
    let DivTabla = document.createElement("div");
    DivTabla.id = "detalle" + idDetalle;
    DivTabla.className = "form-group col-lg-12";
    DivPadre.appendChild(DivTabla);

    //creacion de btnAuxiliar
    let DivBtnAux = document.createElement("div");
    DivBtnAux.className = "form-group col-lg-1";
    let BtnAux = document.createElement("button");
    BtnAux.className = "btn btn-success btn-sm redondo btnPress";
    BtnAux.type = "button";
    BtnAux.id = "BtnAux";
    BtnAux.textContent = "AUX";
    BtnAux.onclick = ClickAux;
    BtnAux.disabled = true;
    DivBtnAux.appendChild(BtnAux);
    DivTabla.appendChild(DivBtnAux);

    //creacion select Cuenta Contable
    let DivCuentaContable = document.createElement("div");
    DivCuentaContable.className = "form-group col-lg-2";
    let SelectCuentaContable = document.createElement("select");
    SelectCuentaContable.className = "basic-usage-select form-control custom-select";
    SelectCuentaContable.id = "cuentaCont";
    SelectCuentaContable.name = "ctacont";
    SelectCuentaContable.title = "Seleccionar";
    SelectCuentaContable.onchange = ActivaBtnAux;
    SelectCuentaContable.required = true;
    SelectCuentaContable.innerHTML = CuentasContables;
    DivCuentaContable.appendChild(SelectCuentaContable);
    DivTabla.appendChild(DivCuentaContable);

    //creacion input glosa
    let DivInputGlosa = document.createElement("div");
    DivInputGlosa.className = "form-group col-lg-2";
    let InputGlosa = document.createElement("input");
    InputGlosa.className = "selectpicker form-control";
    InputGlosa.type = "text";
    InputGlosa.id = "glosaDetalle";
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
    SelectCC.className = "basic-usage-select selectpicker form-control";
    SelectCC.id = "centrocosto";
    SelectCC.name = "centrocosto";
    SelectCC.tabIndex = -1;
    DivCentroCosto.appendChild(SelectCC);
    DivTabla.appendChild(DivCentroCosto);

    //creación Debe
    let DivMontoDebe = document.createElement("div");
    DivMontoDebe.className = "form-group col-lg-2";
    let InputDebe = document.createElement("input");
    InputDebe.className = "selectpicker form-control";
    InputDebe.id = "debe";
    InputDebe.name = "debe";
    InputDebe.min = 0;
    InputDebe.value = 0;
    InputDebe.onchange = Totales;
    DivMontoDebe.appendChild(InputDebe);
    DivTabla.appendChild(DivMontoDebe);

    //Creación Haber
    let DivMontoHaber = document.createElement("div");
    DivMontoHaber.className = "form-group col-lg-2";
    let InputHaber = document.createElement("input");
    InputHaber.className = "selectpicker form-control";
    InputHaber.id = "haber";
    InputHaber.name = "haber";
    InputHaber.min = 0;
    InputHaber.value = 0;
    InputHaber.onchange = Totales;
    DivMontoHaber.appendChild(InputHaber);
    DivTabla.appendChild(DivMontoHaber);

    //creacipon BtnEliminar
    let DivBtnEliminar = document.createElement("div");
    DivBtnEliminar.className = "form-group col-lg-1";
    let BtnEliminar = document.createElement("button");
    BtnEliminar.className = "btn btn-danger btn-sm redondo btnPress";
    BtnEliminar.id = "borrarTabla";
    BtnEliminar.type = "button";
    BtnEliminar.textContent = "X";
    BtnEliminar.onclick = EliminarUltimaFila;
    DivBtnEliminar.appendChild(BtnEliminar);
    DivTabla.appendChild(DivBtnEliminar);
}
function EliminarUltimaFila() {
    jQuery('#detalle' + idDetalle).remove();
    idDetalle--;
    if (idDetalle <= 0) {
        idDetalle = 0;
    }
    Totales();
}
document.getElementById("Btn_AgregarVoucher").addEventListener('click', function () {
    CrearTablaVoucher();
});
document.getElementById("duplica").addEventListener('click', function () {
    DuplicaGlosa();
});

var IdDivAuxProvDeudor = 0;
const CrearAuxProvDeudor = () => {

    IdDivAuxProvDeudor = IdDivAuxProvDeudor + 1;

    let ContenidoAux = document.getElementById("TablaProvDeudorAux");
    let DivContenidoAux = document.createElement("div");
    DivContenidoAux.id = "detalle" + IdDivAuxProvDeudor;
    DivContenidoAux.className = "form-group col-lg-12";
    ContenidoAux.appendChild(DivContenidoAux);

    //content folio
    let DivFolio = document.createElement("div");
    DivFolio.className = "form-group col-lg-1";
    let InputFolio = document.createElement("input");
    InputFolio.className = "form-control";
    InputFolio.name = "FechaPrestador";
    InputFolio.type = "text";
    InputFolio.required = true;
    DivFolio.appendChild(InputFolio);
    DivContenidoAux.appendChild(DivFolio);

    //content TipoDTE
    let DivTipoDTE = document.createElement("div");
    DivTipoDTE.className = "form-group col-lg-2";
    let selectTipoDte = document.createElement("select");
    selectTipoDte.className = "form-control";
    let Seleccionar = document.createElement("option");
    Seleccionar.setAttribute("value", "value1");
    let OptionText = document.createTextNode("Seleccionar");
    Seleccionar.appendChild(OptionText);
    selectTipoDte.appendChild(Seleccionar);
    DivTipoDTE.appendChild(selectTipoDte);
    DivContenidoAux.appendChild(DivTipoDTE);

    //contentMonto neto
    let DivMontoNeto = document.createElement("div");
    DivMontoNeto.className = "form-group col-lg-2";
    let InputMontoNeto = document.createElement("input");
    InputMontoNeto.className = "form-control";
    DivMontoNeto.appendChild(InputMontoNeto);
    DivContenidoAux.appendChild(DivMontoNeto);

    //content monto exento
    let DivMontoExento = document.createElement("div");
    DivMontoExento.className = "form-group col-lg-2";
    let InputMontoExento = document.createElement("input");
    InputMontoExento.className = "form-control";
    DivMontoExento.appendChild(InputMontoExento);
    DivContenidoAux.appendChild(DivMontoExento);

    //content monto iva
    let DivMontoIVA = document.createElement("div");
    DivMontoIVA.className = "form-group col-lg-2";
    let InputIVA = document.createElement("input");
    InputIVA.className = "form-control";
    DivMontoIVA.appendChild(InputIVA);
    DivContenidoAux.appendChild(DivMontoIVA);

    //content monto total
    let DivMontoTotal = document.createElement("div");
    DivMontoTotal.className = "form-group col-lg-2";
    let InputMontoTotal = document.createElement("input");
    InputMontoTotal.className = "form-control";
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
}
function BorrarFilaAuxiliarProvDeudor() {
    jQuery('#detalle' + IdDivAuxProvDeudor).remove();
    IdDivAuxProvDeudor--;
    if (IdDivAuxProvDeudor <= 0) {
        IdDivAuxProvDeudor = 0;
    }
}
document.getElementById("Btn_AgregaFilaProvDeudor").addEventListener('click', function () {
    CrearAuxProvDeudor();
});

var IdDivAuxRemu = 0;
const CrearTablaAuxRemu = () => {
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
    InputFolioRemu.id = "AuxFolio";
    InputFolioRemu.name = "AuxFolio";
    InputFolioRemu.required = true;
    DivRemuFolio.appendChild(InputFolioRemu);
    DivAuxRemu.appendChild(DivRemuFolio);

    //sueldo líquido
    let DivSueldoLiquido = document.createElement("div");
    DivSueldoLiquido.className = "col-lg-5";
    let InputSueldoLiquido = document.createElement("input");
    InputSueldoLiquido.className = "form-control";
    InputSueldoLiquido.id = "AuxTotal";
    InputSueldoLiquido.name = "AuxTotal";
    InputSueldoLiquido.type = "number";
    InputSueldoLiquido.required = true;
    DivSueldoLiquido.appendChild(InputSueldoLiquido);
    DivAuxRemu.appendChild(DivSueldoLiquido);

    //content eliminar fila
    let divElimSueldoLiq = document.createElement("div");
    divElimSueldoLiq.className = "form-group col-lg-1 col-lg-offset-3";
    let btnEliminarSueldo = document.createElement("button");
    btnEliminarSueldo.type = "button";
    btnEliminarSueldo.className = "btn btn-danger btn-sm redondo btnPress";
    btnEliminarSueldo.onclick = BorrarFilaAuxRemu;
    btnEliminarSueldo.tabIndex = -1;
    btnEliminarSueldo.textContent = "X";
    divElimSueldoLiq.appendChild(btnEliminarSueldo);
    DivAuxRemu.appendChild(divElimSueldoLiq);
}
document.getElementById("Btn_AgregaFilaRemu").addEventListener('click', function () {
    CrearTablaAuxRemu();
});
function BorrarFilaAuxRemu() {
    jQuery('#detalleRemu' + IdDivAuxRemu).remove();
    IdDivAuxRemu--;
    if (IdDivAuxRemu <= 0) {
        IdDivAuxRemu = 0;
    }
}

var IdAuxHonorarios = 0;
const CrearTablaHonorariosAux = () => {
    IdAuxHonorarios = IdAuxHonorarios + 1;
    let GetDivHonorariosAux = document.getElementById("TablaHonorAux");
    let DivHonoAux = document.createElement("div");
    DivHonoAux.id = "detalleHono" + IdAuxHonorarios;
    DivHonoAux.className = "col-lg-12";
    GetDivHonorariosAux.appendChild(DivHonoAux);

    //folio
    let DivFolioHonor = document.createElement("div");
    DivFolioHonor.className = "form-group col-lg-2";
    let InputFolio = document.createElement("input");
    InputFolio.type = "number";
    InputFolio.className = "form-control";
    InputFolio.min = 1;
    InputFolio.id = "AuxFolio";
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
    InputValorBruto.name = "AuxTotal";
    InputValorBruto.required = true;
    DivValorBruto.appendChild(InputValorBruto);
    DivHonoAux.appendChild(DivValorBruto);

    //tipo retención
    let DivTipoRetencion = document.createElement("div");
    DivTipoRetencion.className = "form-group col-lg-3";
    let SelectTipoRetencion = document.createElement("select");
    SelectTipoRetencion.className = "form-control";
    SelectTipoRetencion.id = "AUXtipoRetencion";
    SelectTipoRetencion.name = "AUXtipoRetencion";
    DivTipoRetencion.appendChild(SelectTipoRetencion);
    DivHonoAux.appendChild(DivTipoRetencion);

    //retención
    let DivRetencion = document.createElement("div");
    DivRetencion.className = "form-group col-lg-2";
    let InputRetencion = document.createElement("input");
    InputRetencion.type = "number";
    InputRetencion.className = "form-control";
    InputRetencion.name = "AUXValorRetencion";
    InputRetencion.readOnly = true;
    DivRetencion.appendChild(InputRetencion);
    DivHonoAux.appendChild(DivRetencion);

    //valor líquido
    let DivValorLiquido = document.createElement("div");
    DivValorLiquido.className = "form-group col-lg-2";
    let InputValorLiq = document.createElement("input");
    InputValorLiq.type = "number";
    InputValorLiq.className = "form-control";
    InputValorLiq.name = "AUXValorLiquido";
    InputValorLiq.required = true;
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
    BtnBorrarHono.onclick =BorrarFilaAuxRemu;
    BtnBorrarHono.textContent = "X";
    DivBtnBorrar.appendChild(BtnBorrarHono);
    DivHonoAux.appendChild(DivBtnBorrar);
}
document.getElementById("Btn_AgregaFilaHonorarios").addEventListener('click', function () {
    CrearTablaHonorariosAux();
});
function BorrarFilaAuxRemu() {
    jQuery('#detalleHono' + IdAuxHonorarios).remove();
    IdAuxHonorarios--;
    if (IdAuxHonorarios <= 0) {
        IdAuxHonorarios = 0;
    }
}

const ActivaBtnAux = () => {
    let SelectCtaContable = document.getElementById("cuentaCont");
    let OptionCtaContable = SelectCtaContable.options[SelectCtaContable.selectedIndex];
    console.log(OptionCtaContable);
    let BtnAux = document.getElementById("BtnAux");
    if (OptionCtaContable.dataset.auxiliar == 1 || OptionCtaContable.dataset.auxiliar == 2) {
        BtnAux.disabled = false;
    } else {
        BtnAux.disabled = true;
    }

}
const ClickAux = () => {
    let SelectCuentaContable = document.getElementById("cuentaCont").selectedOptions[0];
    console.log(SelectCuentaContable);
    if (SelectCuentaContable.dataset.tipoauxiliar == "ProveedorDeudor") {
        if (IdDivAuxProvDeudor == 0) {
            CrearAuxProvDeudor();
        }
        $('#ModalAuxiliarProvDeudor').modal('show');
    }
    if (SelectCuentaContable.dataset.tipoauxiliar == "Remuneracion") {
        if (IdDivAuxRemu==0) {
            CrearTablaAuxRemu();
        }
        $('#ModalAuxiliarRemu').modal('show');
    }
    if (SelectCuentaContable.dataset.tipoauxiliar == "Honorarios") {
        if (IdAuxHonorarios==0) {
            CrearTablaHonorariosAux();
        }
        $('#ModalAuxiliarHonor').modal('show');
    }
}

$(document).ready(function () {
    $('.basic-usage-select').select2();
});