"use strict";

function DuplicaGlosa() {
    var glosa = document.getElementById("glosa").value;
    $("input[name='glosaDetalle']").each(function () {
        $("input[name='glosaDetalle']").val(glosa);
    });
}

$(document).ready(function () {
    $(".basic-usage-select").select2();
});

//function Totales() {

//    var TotDebe = 0;
//    var TotHaber = 0;
//    $("input[name='debe']").each(function () {
//        TotDebe = TotDebe + parseFloat($(this).val());
//        TotDebeString = TotDebe.toString();
//        document.getElementById("TotDebe").textContent = TotDebeString;

//    });
//    $("input[name='haber']").each(function () {
//        TotHaber = TotHaber + parseFloat($(this).val());
//        TotHaberString = TotHaber.toString();
//        document.getElementById("TotHaber").textContent = TotHaberString;
//    });



//    if (TotDebe == TotHaber) {
//        if (TotDebe == 0 & TotHaber == 0) {
//            $("#aviso").hide();
//            $('input[type="submit"]').prop('disabled', true);
//        }
//        else {
//            $("#aviso").show();
//            document.getElementById("aviso").textContent = "Montos Cuadrados";
//            document.getElementById("aviso").style.fontWeight = "700";
//            document.getElementById("aviso").className = "alert alert-success";
//            $('input[type="submit"]').prop('disabled', false);
//        }
//    }
//    else {
//        $("#aviso").show();
//        document.getElementById("aviso").textContent = "Montos NO Cuadrados, por diferencia de " + Math.abs(TotDebe - TotHaber);
//        document.getElementById("aviso").className = "alert alert-danger";
//        document.getElementById("aviso").style.fontWeight = "700";
//        $('input[type="submit"]').prop('disabled', true);
//    }
//}
function SumaTotales() {
    let TotDebe = 0;
    let TotHaber = 0;
    let ValorDebe = document.getElementsByName("debe");
    let ValorHaber = document.getElementsByName("haber");
    ValorDebe.forEach(function (valordebe) {
        if (valordebe.value != 0) {
            TotDebe = TotDebe + parseFloat(valordebe.value);
            //document.getElementById("haber").disabled = true;
        }
        document.getElementById("TotDebe").textContent = TotDebe;
    });
    ValorHaber.forEach(function (valorhaber) {
        if (valorhaber.value != 0) {
            TotHaber = TotHaber + parseFloat(valorhaber.value);
            //document.getElementById("debe").disabled = true;
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

//function SumarTotales() {
//    let ValorDebe = $("input[name='debe']").val();
//    let TotDebe = new Decimal(0);
//    let TotHaber = 0;

//    $("input[name='debe']").each(function () {
//        TotDebe = TotDebe.plus(ValorDebe).toNumber();
//        TotDebeString = TotDebe.toString();
//        document.getElementById("TotDebe").textContent = TotDebeString;

//        console.log(TotDebe);
//    });

//}
