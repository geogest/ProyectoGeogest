"use strict";

function DuplicaGlosa() {
    var glosa = document.getElementById("glosa").value;
    $("input[name='glosaDetalle']").each(function () {
        $("input[name='glosaDetalle']").val(glosa);
    });
}

$(document).ready(function () {
    $(".estiloSelect").select2();
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

    let MontoNeto = DivHijos[3].firstChild;
    let MontoExento = DivHijos[4].firstChild;
    let MontoIva = DivHijos[5].firstChild;
    let MontoTotal = DivHijos[6].firstChild;

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

    let MontoNeto = DivHijos[3].firstChild;
    let MontoExento = DivHijos[4].firstChild;
    let MontoIva = DivHijos[5].firstChild;
    let MontoTotal = DivHijos[6].firstChild;

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