function SelecCliente(idClienteContable) {
    $.ajax({
        url: "DoSeleccionarClienteContable?EmisorID=" + idClienteContable,
        success: function (response) {
            if (response != null && response.success) {
                //alert(document.location.href);
                document.location.href = "/Contabilidad/PanelClienteContable";
            }
            else {
                alert("No se puede operar con este cliente contable");
            }
        }
    });
}