//Aquí se escriben las funcionalidades para guardar en memoria las acciones que hace el usuario con los filtros.
//Para que estos filtros funcionen los identificadores de estos deben ser los siguientes:

//-#Guardar : botón para iniciar la busqueda.
//-#Mes : Combobox para buscar por mes.
//-#cantidadRegistrosPorPagina : Combobox para modificar la cantidad de registros mostrados.
//-#Anio : Input para buscar por año.
//-#Glosa : Input para buscar por glosa.
//-#fechainicio : Input para poner el rango inicial.
//-#fechafin : Input para poner el rango final.
//-#Rut : Input para buscar por rut

// En caso de tener tablas con diferentes filtros agregarlos a este script y llamar a la funcionalidad en la vista.



function GuardarSeleccionado() {
    var MesSeleccionado = $("#Mes option:selected").text();
    $('#Mes').on('change', function () {
        MesSeleccionado = $("#Mes option:selected").text();
        $('#Guardar').on('click', function () {
            localStorage.setItem("MesAguardar", MesSeleccionado);
        });
    });

    var CantRegistrosSelect = $('#cantidadRegistrosPorPagina option:selected').text();
    $('#cantidadRegistrosPorPagina').on('change', function () {
        CantRegistrosSelect = $('#cantidadRegistrosPorPagina option:selected').text();
        $('#Guardar').on('click', function () {
            localStorage.setItem("CantidadDeRegistros", CantRegistrosSelect);
        });
    });

    var CuentaContSelect = $('#CuentaContableID option:selected').text();
    $('#CuentaContableID').on('change', function () {
        CuentaContSelect = $('#CuentaContableID option:selected').text();
        $('#Guardar').on('click', function () {
            localStorage.setItem("CuentaContable", CuentaContSelect);
        });
    });
}

function ObtenerSeleccionado() {
    $('#Mes option').each(function () {
        let valorSeleccionado = $(this).text();
        var ObtenerMesSeleccionado = localStorage.getItem("MesAguardar");

        if (ObtenerMesSeleccionado == valorSeleccionado) {
            let AñadirAtributo = $(this).attr("selected", "selected");
        }
    });

    $('#cantidadRegistrosPorPagina option').each(function () {
        let valorSeleccionado = $(this).text();
        var ObtenerCantidadRegistros = localStorage.getItem("CantidadDeRegistros");

        if (ObtenerCantidadRegistros == valorSeleccionado) {
            let AñadirAtributo = $(this).attr("selected", "selected");
        }
    });

    $('#CuentaContableID option').each(function () {
        let valorSeleccionado = $(this).text();
        var ObtenerCuentaContable = localStorage.getItem("CuentaContable");

        if (ObtenerCuentaContable == valorSeleccionado) {
            let AñadirAtributo = $(this).attr("selected", "selected");
        }
    });
}

function ObtenerInputs() {

    if(document.getElementById("Glosa") != null){
        var Glosa = document.getElementById("Glosa");
    }
    if(document.getElementById("Anio") != null){
        var Anio = document.getElementById("Anio");
    }
    if(document.getElementById("FechaInicio") != null && document.getElementById("FechaFin") != null){
        var FechaInicio = document.getElementById("FechaInicio");
        var FechaFin = document.getElementById("FechaFin");
    }
    if(document.getElementById("Rut") != null){
        var Rut = document.getElementById("Rut");
    }

    $('#Guardar').on('click', function () {
    
        localStorage.setItem("AnioAGuardar", Anio.value);
        localStorage.setItem("GlosaAGuardar", Glosa.value);
        localStorage.setItem("FechaInicioAGuardar", FechaInicio.value);
        localStorage.setItem("FechaFinAGuardar", FechaFin.value);
        localStorage.setItem("RutAGuardar", Rut.value);
        
    });


    var ObtenerAnio = localStorage.getItem("AnioAGuardar");
    var ObtenerGlosa = localStorage.getItem("GlosaAGuardar");
    var ObtenerFechaInicio = localStorage.getItem("FechaInicioAGuardar");
    var ObtenerFechaFin = localStorage.getItem("FechaFinAGuardar");
    var ObtenerRut = localStorage.getItem("RutAGuardar");

    if (ObtenerAnio != null) {
        Anio.value = ObtenerAnio;
    }
    if (ObtenerGlosa != null) {
        Glosa.value = ObtenerGlosa;
    }
    if (ObtenerRut != null) {
        Rut.value = ObtenerRut;
    }
    if (ObtenerFechaInicio != null && ObtenerFechaFin != null) {
        FechaInicio.value = ObtenerFechaInicio;
        FechaFin.value = ObtenerFechaFin;
    }
}

function CondicionesParaLimpiar() {
    //Eliminamos la memoria de los Storage solo si se recarga la página sin realizar ninguna busqueda.
    var Click = false;
    $('#Guardar').on("click", function () {
        Click = true;
    });
    $('#Paginador li').on("click", function () {
        Click = true;
    });
    $(window).on("beforeunload", function () {
        if (Click == false) {
            localStorage.removeItem("MesAguardar");
            localStorage.removeItem("CantidadDeRegistros");
            localStorage.removeItem("CuentaContable");
            localStorage.removeItem("GlosaAGuardar");
            localStorage.removeItem("AnioAGuardar");
            localStorage.removeItem("FechaInicioAGuardar");
            localStorage.removeItem("FechaFinAGuardar");
            localStorage.removeItem("RutAGuardar");
        }
    });
}