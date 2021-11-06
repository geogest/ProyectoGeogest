function DuplicaGlosa() {
    var glosa = document.getElementById("glosa").value;
    $("input[name='glosaDetalle']").each(function () {
        $("input[name='glosaDetalle']").val(glosa);
    });
}


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
            $('input[type="submit"]').prop('disabled', true);
        }
        else {
            $("#aviso").show();
            document.getElementById("aviso").textContent = "Montos Cuadrados";
            document.getElementById("aviso").style.fontWeight = "700";
            document.getElementById("aviso").className = "alert alert-success";
            $('input[type="submit"]').prop('disabled', false);
        }
    }
    else {
        $("#aviso").show();
        document.getElementById("aviso").textContent = "Montos NO Cuadrados, por diferencia de " + Math.abs(TotDebe - TotHaber);
        document.getElementById("aviso").className = "alert alert-danger";
        document.getElementById("aviso").style.fontWeight = "700";
        $('input[type="submit"]').prop('disabled', true);
    }

    let valorDebe = $("input[name='debe']").val();
    let valorHaber = $("input[name='haber']").val();

    if (valorDebe.length != 1) {
        console.log(valorDebe);
        document.getElementById("haber").disabled = true;
    } else {
        document.getElementById("haber").disabled = false;
    }

    if (valorHaber.length != 1) {
        document.getElementById("debe").disabled = true;

    } else {
        document.getElementById("debe").disabled = false;
    }
}
