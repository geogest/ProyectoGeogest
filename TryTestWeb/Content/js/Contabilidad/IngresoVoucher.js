function DuplicaGlosa() {
    var glosa = document.getElementById("glosa").value;
    // if (checkboxElem.checked) {
    $("input[name='glosaDetalle']").each(function () {
        $("input[name='glosaDetalle']").val(glosa);
    });
    //}
}
$(document).ready(function () {
    $('.basic-usage-select').select2();
});

function Totales() {
    var TotDebe = 0;
    var TotHaber = 0;
    $("input[name='debe']").each(function () {
        TotDebe = TotDebe + parseFloat($(this).val());
        TotDebeString = TotDebe.toString();
        document.getElementById("TotDebe").textContent = TotDebeString;

    });
    $("input[name='haber']").each(function () {
        TotHaber = TotHaber + parseFloat($(this).val());
        TotHaberString = TotHaber.toString();
        document.getElementById("TotHaber").textContent = TotHaberString;
    });



    if (TotDebe == TotHaber) {
        if (TotDebe == 0 & TotHaber == 0) {
            $("#aviso").hide();
            $(':input[type="submit"]').prop('disabled', true);
            //$('#DatosVoucher').validator('validate');
        }
        else {
            $("#aviso").show();
            document.getElementById("aviso").textContent = "Montos Cuadrados";
            document.getElementById("aviso").style.fontWeight = "700";
            document.getElementById("aviso").className = "alert alert-success";
            $(':input[type="submit"]').prop('disabled', false);
            //$('#DatosVoucher').validator('validate');
        }
    }
    else {
        $("#aviso").show();
        document.getElementById("aviso").textContent = "Montos NO Cuadrados, por diferencia de " + Math.abs(TotDebe - TotHaber);
        document.getElementById("aviso").className = "alert alert-danger";
        document.getElementById("aviso").style.fontWeight = "700";
        $(':input[type="submit"]').prop('disabled', true);
        //$('#DatosVoucher').validator('validate');
    }
}
 