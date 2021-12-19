"use strict";

const DuplicaGlosa=()=> {
    var glosa = document.getElementById("glosa").value;
    $("input[name='glosaDetalle']").each(function () {
        $("input[name='glosaDetalle']").val(glosa);
    });
}

$(document).ready(function () {
    // $(".estiloSelect").select2();
});
function SumaTotalesVoucher() {
    let TotDebe = 0;
    let TotHaber = 0;
    let ValorDebe = document.getElementsByName("debe");
    let ValorHaber = document.getElementsByName("haber");
    ValorDebe.forEach(function (valordebe) {
        if (valordebe.value != 0) {
            TotDebe = TotDebe + parseFloat(valordebe.value);
        }
        document.getElementById("TotDebe").textContent = TotDebe;
    });
    ValorHaber.forEach(function (valorhaber) {
        if (valorhaber.value != 0) {
            TotHaber = TotHaber + parseFloat(valorhaber.value);
        }

        document.getElementById("TotHaber").textContent = TotHaber;
        document.getElementById("TotDebe").textContent = TotDebe;
    });

    if (TotDebe == TotHaber) {
        if (TotDebe == 0 && TotHaber == 0) {
            document.getElementById("aviso").style.display = "none";
        } else {
            document.getElementById("aviso").style.display = "";
            document.getElementById("aviso").textContent = "Montos Cuadrados";
            document.getElementById("aviso").style.fontWeight = "700";
            document.getElementById("aviso").className = "alert alert-success";
        }
    } else {
        document.getElementById("aviso").style.display = "";
        document.getElementById("aviso").textContent = "Montos NO Cuadrados, por diferencia de " + Math.abs(TotDebe - TotHaber);
        document.getElementById("aviso").className = "alert alert-danger";
        document.getElementById("aviso").style.fontWeight = "700";

    }
}
const CalculoAuxiliarProvDeudor = (id) => {
    let DivPadreAuxProv = document.getElementById(id);
    let DivHijos = DivPadreAuxProv.childNodes;

    let MontoNeto = DivHijos[2].firstChild;
    let MontoExento = DivHijos[3].firstChild;
    let MontoIva = DivHijos[4].firstChild;
    let MontoTotal = DivHijos[5].firstChild;

    if (MontoTotal.value != "" && MontoTotal.value != null) {
        MontoIva.value = Math.round((MontoTotal.value * 0.19) / 1.19);
        MontoNeto.value = MontoTotal.value - MontoIva.value;
        MontoExento.value = 0;
    }

    const TodosTotales = document.getElementsByName("MontoTotal");
    let SumaDeTotales = 0;
    TodosTotales.forEach(function (sumatotales) {
        SumaDeTotales = SumaDeTotales + parseFloat(sumatotales);
    });

    CuadrarValorProvDeudor(SumaDeTotales);

}
const CalculoAuxMontoExento = (id) => {
    let DivPadreAuxMontoExento = document.getElementById(id);
    let DivHijos = DivPadreAuxMontoExento.childNodes;

    let MontoNeto = DivHijos[2].firstChild;
    let MontoExento = DivHijos[3].firstChild;
    let MontoIva = DivHijos[4].firstChild;
    let MontoTotal = DivHijos[5].firstChild;

    if (MontoExento.value != "" && MontoExento.value != null) {
        MontoTotal.value = MontoExento.value;
        MontoIva.value = 0;
        MontoNeto.value = 0;
    }

    const TodosTotales = document.getElementsByName("MontoTotal");
    let SumaDeTotales = 0;
    TodosTotales.forEach(function (sumatotales) {
        SumaDeTotales = SumaDeTotales + parseFloat(sumatotales.value);
    });

    CuadrarValorProvDeudor(SumaDeTotales);
}
const CuadrarValorProvDeudor = (valor) => {
    let ValorTotal = document.getElementById("AUXvalorProvDeudor").value;

    if (ValorTotal == valor) {
        document.getElementById("BtnGuardarAux").disabled = false;
        document.getElementById("Aviso").style.display = "none";
    } else {
        document.getElementById("BtnGuardarAux").disabled = true;
        document.getElementById("Aviso").textContent = "Montos NO cuadran con Total";
        document.getElementById("Aviso").className = "alert alert-danger";
        document.getElementById("Aviso").style.fontWeight = "700";
        document.getElementById("Aviso").style.textAlign = "right";
        document.getElementById("Aviso").style.display = "";
    }
}
const CalculoAuxRetencion = (id) => {
    let DivPadreAuxRetencion = document.getElementById(id);
    let DivHijo = DivPadreAuxRetencion.childNodes;
    let ValorBruto = DivHijo[1].firstChild;
    let TipoRetencion = DivHijo[2].firstChild;
    let SelectedOption = TipoRetencion.selectedOptions[0];
    let Retencion = DivHijo[3].firstChild;
    let ValorLiquido = DivHijo[4].firstChild;

    if (SelectedOption.text == "Retención 10%") {
        Retencion.value = Math.round(ValorBruto.value * 0.1);
        ValorLiquido.value = Math.round(ValorBruto.value - Retencion.value);
    }
    if (SelectedOption.text == "Retención 10.75%") {
        Retencion.value = Math.round(ValorBruto.value * 0.1075);
        ValorLiquido.value = Math.round(ValorBruto.value - Retencion.value);
    }
    if (SelectedOption.text == "Retención 11.5%") {
        Retencion.value = Math.round(ValorBruto.value * 0.1150);
        ValorLiquido.value = Math.round(ValorBruto.value - Retencion.value);
    }
    if (SelectedOption.text == "Retención 20%") {
        Retencion.value = Math.round(ValorBruto.value * 0.2);
        ValorLiquido.value = Math.round(ValorBruto.value - Retencion.value);
    }
    if (SelectedOption.text == "Sin Retención") {
        ValorLiquido.value = ValorBruto.value;
        Retencion.value = 0;
    }

    let SumaTotalAuxRetencion = 0;
    let ValoresTotalesAuxRetencion = document.getElementsByName("AUXValorLiquido");

    ValoresTotalesAuxRetencion.forEach(function (totalexauxret) {
        SumaTotalAuxRetencion = SumaTotalAuxRetencion + parseFloat(totalexauxret.value);
    });

    CuadrarValorAuxRetencion(SumaTotalAuxRetencion);
}
const CuadrarValorAuxRetencion = (valor) => {
    let ValorTotal = document.getElementById("AuxValorHonor").value;
    if (ValorTotal == valor) {
        document.getElementById("BtnGuardarHonor").disabled = false;
        document.getElementById("AvisoHonor").style.display = "none";
    } else {
        document.getElementById("BtnGuardarHonor").disabled = true;
        document.getElementById("AvisoHonor").textContent = "Montos NO cuadran con Total";
        document.getElementById("AvisoHonor").className = "alert alert-danger";
        document.getElementById("AvisoHonor").style.fontWeight = "700";
        document.getElementById("AvisoHonor").style.textAlign = "right";
        document.getElementById("AvisoHonor").style.display = "";
    }
}
const SumaSueldoLiquidoRemu = () => {
    let LstTotalSLiquido = document.getElementsByName("AuxTotalRemu");
    let SumaTotalSLiquido = 0;
    LstTotalSLiquido.forEach(function(sumatotalremu){
        SumaTotalSLiquido=SumaTotalSLiquido+parseFloat(sumatotalremu.value);
    });
    CuadrarValorAuxRemu(SumaTotalSLiquido);
}


const MostrarDatosVoucher=(datos)=>{
    debugger;
    console.log(datos);
    let FechaContabilizacion = moment.utc(datos.FechaContabilizacion).format('DD-MM-YYYY');
    document.getElementById("glosa").value = datos.Glosa;
    document.getElementById("numVoucher").value=datos.NumVoucher;
    document.getElementById("fecha").value=FechaContabilizacion;
    document.getElementById("tipo").value=datos.Tipo;
    document.getElementById("TipoOrigen").value = datos.TipoOrigenVoucher;

    datos.DetalleVoucher.map(function(dato){
        RellenarTablaVoucher(dato);
    })
    SumaTotalesVoucher();
    SeleccionarCtaContable(datos.DetalleVoucher);
    
}
let q = 0;
const MostrarDatosProvDeudor=(data)=>{
    let FechaContProvDeudor = moment.utc(data.FechaContabilizacion).format('DD-MM-YYYY');
    document.getElementById("FechaPrestador").value = FechaContProvDeudor;
    document.getElementById("TipoPrestadorProvDeudor").value = data.TipoReceptor;
    document.getElementById("TipoDTE"+q).value = data.TipoDTE;
    AuxPrestadorSeleccionado(data.RazonSocialID,data.Rut);
    q=q+1;
}
const MostrarDatosHonor=(data)=>{
    let FechaContHonor = moment.utc(data.FechaContabilizacion).format('DD-MM-YYYY');
    document.getElementById("FechaPrestadorHonor").value = FechaContHonor;
    document.getElementById("TipoPrestadorHonor").value = data.TipoReceptor;
    AuxPrestadorSeleccionado(data.RazonSocialID,data.Rut);
}
const SeleccionarCtaContable=(data)=>{
    let id=1;
    data.map(function(select){
        document.getElementById("ctacont"+id).value=select.CuentaContableID;
        ActivaBtnAux("detalle"+id,select);
        if(select.CentroDeCostoID == -1){
            document.getElementById("centrocosto"+id).value="0";
        }else{
            document.getElementById("centrocosto"+id).value=select.CentroDeCostoID;
        }
        id=id+1;
    });    
}


const ActivaBtnAuxNuevo = (id) => {
    let DivPadreBtn = document.getElementById(id);
    let DivHijosBtn = DivPadreBtn.childNodes;

    let SelectCtaContable = DivHijosBtn[1].firstChild;
    let OptionCtaContable = SelectCtaContable.selectedOptions[0];
    let BtnAux = DivHijosBtn[0].firstChild;
    if (OptionCtaContable.dataset.auxiliar == 1 || OptionCtaContable.dataset.auxiliar == 2) {
        BtnAux.disabled = false;
    } else {
        BtnAux.disabled = true;
    }

}
const ClickAuxNuevo = (idPadre,IdBoton) => {
    let DivPadreBtn = document.getElementById(idPadre);
    let DivHijosBtn = DivPadreBtn.childNodes;
    let SelectCuentaContable = DivHijosBtn[1].firstChild;
    let OptionSelected = SelectCuentaContable.selectedOptions[0];

   
    if (OptionSelected.dataset.tipoauxiliar == "ProveedorDeudor") {
        if (IdDivAuxProvDeudor == 0) {
            CrearTablaAuxProvDeudor();
        }
        $('#ModalAuxiliarProvDeudor').modal('show');
        EnviarDatosAuxProvDeudor(OptionSelected.textContent,OptionSelected.value,idPadre,IdBoton);
    }
    if (OptionSelected.dataset.tipoauxiliar == "Remuneracion") {
        if (IdDivAuxRemu == 0) {
            CrearTablaAuxRemu();
        }
        $('#ModalAuxiliarRemu').modal('show');
        EnviarDatosRemuneracion(OptionSelected.textContent,OptionSelected.value,idPadre,IdBoton);
    }
    if (OptionSelected.dataset.tipoauxiliar == "Honorarios") {
        if (IdAuxHonorarios == 0) {
            CrearTablaHonorariosAux();
        }
        $('#ModalAuxiliarHonor').modal('show');
        EnviarDatosHonorAux(OptionSelected.textContent,OptionSelected.value,idPadre,IdBoton);
    }
}


const ActivaBtnAux = (id,AuxDetalle) => {
    let DivPadreBtn = document.getElementById(id);
    let DivHijosBtn = DivPadreBtn.childNodes;
    let BtnAux = DivHijosBtn[0].firstChild;

    if (AuxDetalle.AuxiliarDetalle==null) {
        BtnAux.disabled = true;
    } else {
        BtnAux.disabled = false;
    }
}
const ClickAux = (id,AuxDetalle,idBtn) => {
    // debugger;
    let DivPadreBtn = document.getElementById(id);
    let DivHijosBtn = DivPadreBtn.childNodes;
    let SelectCuentaContable = DivHijosBtn[1].firstChild;
    let OptionSelected = SelectCuentaContable.selectedOptions[0];

   
    if (OptionSelected.dataset.tipoauxiliar == "ProveedorDeudor") {
        if (IdDivAuxProvDeudor == 0) {
            RellenarTablaAuxProvDeudor(AuxDetalle);
        }else{
        }
        $('#ModalAuxiliarProvDeudor').modal('show');
         EnviarDatosAuxProvDeudor(OptionSelected.textContent,OptionSelected.value,id,idBtn);
    }

    if (OptionSelected.dataset.tipoauxiliar == "Remuneracion") {
        if (IdDivAuxRemu == 0) {
            RellenarTablaAuxRemu(AuxDetalle);
        }else{

        }
        $('#ModalAuxiliarRemu').modal('show');
        EnviarDatosRemuneracion(OptionSelected.textContent,id);
    }

    if (OptionSelected.dataset.tipoauxiliar == "Honorarios") {
        if (IdAuxHonorarios == 0) {
            RellenarTablaHonorariosAux(AuxDetalle);
        }
        else{
            document.getElementById("AuxFolio").value=AuxDetalle.Folio;
            document.getElementById("RazonPrestadorHonor").value=AuxDetalle.RazonSocialID;
            document.getElementById("AuxValorBruto").value=AuxDetalle.MontoBruto;
            document.getElementById("AUXValorRetencion").value = AuxDetalle.MontoRetencion;
            document.getElementById("AUXValorLiquido").value = AuxDetalle.ValorLiquido;
        }
        $('#ModalAuxiliarHonor').modal('show');
        EnviarDatosHonorAux(OptionSelected.textContent,OptionSelected.value,id,idBtn);
    }
}

const GuardarAuxProvDeudor=()=>{
    debugger;
    let CContableProvDeudorID = document.getElementById("AuxCuentaProvDeudor").value;
    let NumeroLinea = document.getElementById("AUXitemProvDeudor").value;
    let Valorlinea = document.getElementById("AUXvalorProvDeudor").value;

    let Fecha = document.getElementById("FechaPrestador").value;
    let Tipo = document.getElementById("TipoPrestadorProvDeudor").value;
    let RazonSocialID = document.getElementById("RazonPrestadorProvDeudor").value;
    let Rut = document.getElementById("RutPrestadorProvDeudor").value;

    let EsBoleta = document.getElementById("BoletaVenta").checked;
    let ContalibroCompra = document.getElementById("ContaLibroCompra").checked;
    let ContalibroVenta = document.getElementById("ContaLibroVenta").checked;

    let AuxProvDetalle = {
        CuentaContableID:CContableProvDeudorID,
        NumeroLinea:NumeroLinea,
        ValorLinea:Valorlinea,
        Fecha:Fecha,
        Tipo:Tipo,
        RazonSocialID:RazonSocialID,
        Rut:Rut,
        EsBoleta:EsBoleta,
        ContaLibroCompra:ContalibroCompra,
        ContaLibroVenta:ContalibroVenta,
        DetalleAuxiliar:[]
    }; 

    let LstAuxProvDeudor = document.getElementsByClassName("GuardarDatosAuxProvDeudor");
    let LstHijos = [...LstAuxProvDeudor];
    LstHijos.map(function(data) {
        let PropiedadesHijos = data.childNodes;
        let DetalleAux = {
            FolioDesde:PropiedadesHijos[0].firstChild.value,
            FolioHasta:PropiedadesHijos[1].firstChild.value,
            TipoDte:PropiedadesHijos[2].firstChild.value,
            MontoNeto:PropiedadesHijos[3].firstChild.value,
            MontoExento:PropiedadesHijos[4].firstChild.value,
            MontoIVA:PropiedadesHijos[5].firstChild.value,
            MontoTotal:PropiedadesHijos[6].firstChild.value
        }
        AuxProvDetalle.DetalleAuxiliar.push(DetalleAux)
    })
   
    console.log(AuxProvDetalle);
}
const GuardarAuxHonor=()=>{
    debugger;
    let CuentaContableHonorID = document.getElementById("AuxCuenta").value;
    let NumeroLineaHonor = document.getElementById("AUXitem").value;
    let ValorLineaHonor = document.getElementById("AuxValorHonor").value;

    let FechaHonor = document.getElementById("FechaPrestadorHonor").value;
    let TipoHonor = document.getElementById("TipoPrestadorHonor").value;
    let RazonSocialHonorID = document.getElementById("RazonPrestadorHonor").value;
    let RutHonor = document.getElementById("RutPrestadorHonor").value;
    
    let AuxHonorDetalle = {
        CuentaContableID:CuentaContableHonorID,
        NumeroLinea:NumeroLineaHonor,
        ValorLinea:ValorLineaHonor,
        Fecha:FechaHonor,
        Tipo:TipoHonor,
        RazonSocialID:RazonSocialHonorID,
        Rut:RutHonor,
        DetalleAuxHonor:[]
    }

    let LstAuxHonor = document.getElementsByClassName("GuardarDatosAuxHonor");
    let LstHijosHonor = [...LstAuxHonor];
    // console.log(LstHijosHonor);

    LstHijosHonor.map(function(data){
        let propHijos = data.childNodes;
        let DetalleAux = {
            Folio:propHijos[0].firstChild.value,
            ValorBruto:propHijos[1].firstChild.value,
            TipoRetencion:propHijos[2].firstChild.value,
            MontoRetencion:propHijos[3].firstChild.value,
            MontoTotal:propHijos[4].firstChild.value
        }
        AuxHonorDetalle.DetalleAuxHonor.push(DetalleAux);
    })
    console.log(AuxHonorDetalle);
}
const GuardarVoucher=()=>{
    let VoucherModelID= document.getElementById("VoucherAEditar").value;
    let NumVoucher = document.getElementById("numVoucher").value;
    let Fecha = document.getElementById("fecha").value;
    let TipoVoucher = document.getElementById("tipo").value;
    let TipoOrigen = document.getElementById("TipoOrigen").value;
    let Glosa = document.getElementById("glosa").value;
   
    let Voucher = {
        NumVoucher:NumVoucher,
        Fecha:Fecha,
        TipoVoucher:TipoVoucher,
        TipoOrigen:TipoOrigen,
        Glosa:Glosa,
        VoucherModelId:VoucherModelID,
        DetalleVoucher:[]
    }

    let LstItemsVoucher = document.getElementsByClassName("GuardarDatosVoucher");
    let HijosVoucher = [...LstItemsVoucher]

    HijosVoucher.map(function(data){
        let PropHijos = data.childNodes;
        // console.log(PropHijos);
        let DetalleVoucher = {
            CuentaContableID: PropHijos[1].firstChild.value,
            GlosaDetalle: PropHijos[2].firstChild.value,
            CentroCostoID:PropHijos[3].firstChild.value,
            MontoDebe:PropHijos[4].firstChild.value,
            MontoHaber:PropHijos[5].firstChild.value,
        }
        Voucher.DetalleVoucher.push(DetalleVoucher)
    })
    console.log(Voucher);
}

document.getElementById("BtnGuardarHonor").addEventListener('click', function () {
    GuardarAuxHonor();
    $('#ModalAuxiliarHonor').modal('hide');
    // alert('Auxiliar guardado correctamente.');
});

document.getElementById("BtnGuardarAuxProvDeudor").addEventListener('click', function () {
    GuardarAuxProvDeudor();
    $('#ModalAuxiliarProvDeudor').modal('hide');
    // alert('Auxiliar guardado correctamente.');
});