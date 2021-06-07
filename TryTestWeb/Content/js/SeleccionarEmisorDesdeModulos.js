function SelecCliente(idEmisor) {
    $.ajax({
        url: "DoSeleccionarEmisorModulos?EmisorID=" + idEmisor,
        success: function (response) {
            if (response != null && response.success) {
                // alert(response.inicio);
                document.location.href = "/" + response.inicio;//"/Home/Index";
            }
            else {
                alert("No se puede operar con este emisor");
            }
        }
    });
}