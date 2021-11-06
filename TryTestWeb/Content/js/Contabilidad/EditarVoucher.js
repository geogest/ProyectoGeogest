function DuplicaGlosa() {
    var glosa = document.getElementById("glosa").value;
    $("input[name='glosaDetalle']").each(function () {
        $("input[name='glosaDetalle']").val(glosa);
    });
}