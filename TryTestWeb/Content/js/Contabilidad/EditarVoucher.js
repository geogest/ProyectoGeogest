﻿"use strict";

const DuplicaGlosa=()=> {
    var glosa = document.getElementById("glosa").value;
    $("input[name='glosaDetalle']").each(function () {
        $("input[name='glosaDetalle']").val(glosa);
    });
}
// const AgregarEstiloSelect=()=>{
//     let TodosLosSelect = document.querySelectorAll("select");
//     console.log(TodosLosSelect);
//     for (let select = 0; select < TodosLosSelect.length; select++) {
//         const element = TodosLosSelect[select];
//         element.className = "form-control estiloSelect";
//         $(".estiloSelect").select2();
//         // console.log(element);
//     }
// }
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

    let TodosTotalesProv = document.getElementsByName("MontoTotal");
    let SumaDeTotales = 0;
    
    TodosTotalesProv.forEach(function (sumatotales) {
        SumaDeTotales = SumaDeTotales + parseFloat(sumatotales.value);
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
const SumaSueldoLiquidoRemu = () => {
    let LstTotalSLiquido = document.getElementsByName("AuxTotalRemu");
    let SumaTotalSLiquido = 0;
    LstTotalSLiquido.forEach(function(sumatotalremu){
        SumaTotalSLiquido=SumaTotalSLiquido+parseFloat(sumatotalremu.value);
    });
    CuadrarValorAuxRemu(SumaTotalSLiquido);
}


const CuadrarValorProvDeudor = (valor) => {
    let ValorTotal = Number(document.getElementById("AUXvalorProvDeudor").value);

    if (ValorTotal == valor) {
        if(ValorTotal!=0 && valor!=0){
            document.getElementById("BtnGuardarAuxProvDeudor").disabled = false;
            document.getElementById("Aviso").style.display = "none";
        }
    } else {
        document.getElementById("BtnGuardarAuxProvDeudor").disabled = true;
        document.getElementById("Aviso").textContent = "Montos NO cuadran con Total";
        document.getElementById("Aviso").className = "alert alert-danger";
        document.getElementById("Aviso").style.fontWeight = "700";
        document.getElementById("Aviso").style.textAlign = "right";
        document.getElementById("Aviso").style.display = "";
    }
}
const CuadrarValorAuxRetencion = (valor) => {
    let ValorTotal = Number(document.getElementById("AuxValorHonor").value);
    if (ValorTotal == valor) {
        if(ValorTotal!=0 && valor!=0){
            document.getElementById("BtnGuardarHonor").disabled = false;
            document.getElementById("AvisoHonor").style.display = "none";
        }
    } else {
        document.getElementById("BtnGuardarHonor").disabled = true;
        document.getElementById("AvisoHonor").textContent = "Montos NO cuadran con Total";
        document.getElementById("AvisoHonor").className = "alert alert-danger";
        document.getElementById("AvisoHonor").style.fontWeight = "700";
        document.getElementById("AvisoHonor").style.textAlign = "right";
        document.getElementById("AvisoHonor").style.display = "";
    }
}
const CuadrarValorAuxRemu = (valor) => {
    let ValorLineaRemu = Number(document.getElementById("AUXvaloritemRemu").value);

    if (ValorLineaRemu == valor) {
        if(ValorLineaRemu!=0 && valor!=0){
            document.getElementById("BtnGuardarAuxRemu").disabled=false;
            document.getElementById("AvisoAuxRemu").style.display="none";
        }
    }else{
        document.getElementById("BtnGuardarAuxRemu").disabled=true;
        document.getElementById("AvisoAuxRemu").textContent="Montos NO cuadran con Total";
        document.getElementById("AvisoAuxRemu").className="alert alert-danger";
        document.getElementById("AvisoAuxRemu").style.fontWeight="700";
        document.getElementById("AvisoAuxRemu").style.textAlign="right";
        document.getElementById("AvisoAuxRemu").style.display="";
    }
} 


const MostrarDatosVoucher=(datos)=>{
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
const MostrarDatosProvDeudor=(data)=>{
    let FechaContProvDeudor = moment.utc(data.FechaContabilizacion).format('DD-MM-YYYY');
    document.getElementById("FechaPrestador").value = FechaContProvDeudor;
    document.getElementById("TipoPrestadorProvDeudor").value = data.TipoReceptor;
    
    AuxPrestadorSeleccionado(data.RazonSocialID,data.Rut);
}
const MostrarDatosHonor=(data)=>{
    let FechaContHonor = moment.utc(data.FechaContabilizacion).format('DD-MM-YYYY');
    document.getElementById("FechaPrestadorHonor").value = FechaContHonor;
    document.getElementById("TipoPrestadorHonor").value = data.TipoReceptor;
    AuxPrestadorSeleccionado(data.RazonSocialID,data.Rut);
}
const MostrarDatosRemu=(data)=>{
    let FechaContRemu = moment.utc(data.FechaContabilizacion).format('DD-MM-YYYY');
    document.getElementById("FechaPrestadorRemu").value = FechaContRemu;
    document.getElementById("TipoPrestadorRemu").value = data.TipoReceptor;

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
        let NumLinea={
            NumeroLinea:Number(BtnAux.id)
        }
        AuxiliaresDetalle.splice(Number(BtnAux.id)-1,1,NumLinea);
    } else {
        BtnAux.disabled = true;
        AuxiliaresDetalle.splice(Number(BtnAux.id)-1,1,null);
    }
    console.log(AuxiliaresDetalle);
}
const ClickAuxNuevo = (idRow,IdBoton) => {
    let DivPadreBtn = document.getElementById(idRow);
    let DivHijosBtn = DivPadreBtn.childNodes;
    let SelectCuentaContable = DivHijosBtn[1].firstChild;
    let OptionSelected = SelectCuentaContable.selectedOptions[0];
    let AuxiliarExiste = AuxiliaresDetalle.find(aux => aux.NumeroLinea === Number(DivHijosBtn[0].firstChild.id ));
    if (OptionSelected.dataset.tipoauxiliar == "ProveedorDeudor") {
        if (Object.keys(AuxiliarExiste).length <= 1) {
            document.getElementById("TablaProvDeudorAux").innerHTML = "";
            document.getElementById("FechaPrestador").value = "";
            document.getElementById("TipoPrestadorProvDeudor").value = "Selecciona";
            document.getElementById("RazonPrestadorProvDeudor").value = "";
            document.getElementById("RutPrestadorProvDeudor").value = "";
            document.getElementById("BoletaVenta").cheked=false;
            document.getElementById("ContaLibroCompra").cheked=false;
            document.getElementById("ContaLibroVenta").cheked=false;
            CrearTablaAuxProvDeudor();
        }else{
            MostrarDatosProvDeudor(AuxiliarExiste);
        }
        $('#ModalAuxiliarProvDeudor').modal('show');
        EnviarDatosAuxProvDeudor(OptionSelected.textContent,OptionSelected.value,idRow,IdBoton);
    }
    if (OptionSelected.dataset.tipoauxiliar == "Remuneracion") {
        if (Object.keys(AuxiliarExiste).length <= 1) {
            document.getElementById("TablaRemuAux").innerHTML = "";
            document.getElementById("FechaPrestadorRemu").value = "";
            document.getElementById("TipoPrestadorRemu").value = "";
            document.getElementById("RazonPrestadorRemu").value = "";
            document.getElementById("RutPrestadorRemu").value = "";
            CrearTablaAuxRemu();
        }else{
            MostrarDatosRemu(AuxiliarExiste);
        }
        $('#ModalAuxiliarRemu').modal('show');
        EnviarDatosRemuneracion(OptionSelected.textContent,OptionSelected.value,idRow,IdBoton);
    }
    if (OptionSelected.dataset.tipoauxiliar == "Honorarios") {
        if (Object.keys(AuxiliarExiste).length <= 1) {
            document.getElementById("TablaHonorAux").innerHTML = "";
            document.getElementById("FechaPrestadorHonor").value = "";
            document.getElementById("TipoPrestadorHonor").value = "";
            document.getElementById("RazonPrestadorHonor").value = "";
            document.getElementById("RutPrestadorHonor").value = "";
            CrearTablaHonorariosAux();
        }else{
            MostrarDatosHonor(AuxiliarExiste);
        }
        $('#ModalAuxiliarHonor').modal('show');
        EnviarDatosHonorAux(OptionSelected.textContent,OptionSelected.value,idRow,IdBoton);
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
    let DivPadreBtn = document.getElementById(id);
    let DivHijosBtn = DivPadreBtn.childNodes;
    let SelectCuentaContable = DivHijosBtn[1].firstChild;
    let OptionSelected = SelectCuentaContable.selectedOptions[0];

   
    if (OptionSelected.dataset.tipoauxiliar == "ProveedorDeudor") {
        document.getElementById("TablaProvDeudorAux").innerHTML = "";
            AuxDetalle.map(function(AuxDetalleProv){
                RellenarTablaAuxProvDeudor(AuxDetalleProv);
            });
        $('#ModalAuxiliarProvDeudor').modal('show');
        EnviarDatosAuxProvDeudor(OptionSelected.textContent,OptionSelected.value,id,idBtn);
    }

    if (OptionSelected.dataset.tipoauxiliar == "Remuneracion") {
        document.getElementById("TablaRemuAux").innerHTML = "";
            AuxDetalle.map(function(AuxDetalleRemu){
                RellenarTablaAuxRemu(AuxDetalleRemu);
            });
        $('#ModalAuxiliarRemu').modal('show');
        EnviarDatosRemuneracion(OptionSelected.textContent,OptionSelected.value,id,idBtn);
    }

    if (OptionSelected.dataset.tipoauxiliar == "Honorarios") {
        document.getElementById("TablaHonorAux").innerHTML="";
            AuxDetalle.map(function(AuxDetalleHonor){
                RellenarTablaHonorariosAux(AuxDetalleHonor);
            });
        $('#ModalAuxiliarHonor').modal('show');
        EnviarDatosHonorAux(OptionSelected.textContent,OptionSelected.value,id,idBtn);
    }
}

const TieneAuxiliares=(Voucher)=>{
    //con esta función se sabrá cuantos auxiliares tiene el voucher que viene cargado
    //si tiene auxiliar se agrega a la variable global Detalle Vouchers
    //que contiene el detalle de cada voucher
    //READ
    let NumLinea=1;
    DetalleVouchers.push(Voucher);
    Voucher.flatMap(
        (x)=> {
            if(x.AuxiliarDetalle){
                let index = 0;
                let DetallesAuxAPushear = {
                    NumeroLinea: NumLinea,
                    AuxiliarDetalleID: x.AuxiliarDetalle[index].AuxiliarDetalleID,
                    FechaContabilizacion: x.AuxiliarDetalle[index].FechaContabilizacion,
                    RazonSocialID: x.AuxiliarDetalle[index].RazonSocialID,
                    Rut: x.AuxiliarDetalle[index].Rut,
                    TipoReceptor: x.AuxiliarDetalle[index].TipoReceptor,
                    DetalleAuxiliar:[]
                }
                // AuxiliaresDetalle.push(x.AuxiliarDetalle);
                x.AuxiliarDetalle.forEach(AuxDetallePush => {
                    let DetallesAux = {
                        Folio: AuxDetallePush.Folio,
                        MontoBruto: AuxDetallePush.MontoBruto,
                        MontoExento: AuxDetallePush.MontoExento,
                        MontoIvaActivoLinea: AuxDetallePush.MontoIvaActivoLinea,
                        MontoIvaLinea: AuxDetallePush.MontoIvaLinea,
                        MontoIvaNoRecuperable: AuxDetallePush.MontoIvaNoRecuperable,
                        MontoIvaUsoComun: AuxDetallePush.MontoIvaUsoComun,
                        MontoNeto: AuxDetallePush.MontoNeto,
                        MontoRetencion: AuxDetallePush.MontoRetencion,
                        MontoTotalLinea: AuxDetallePush.MontoTotalLinea,
                        TipoDTE: AuxDetallePush.TipoDTE,
                        ValorLiquido: AuxDetallePush.ValorLiquido
                    }
                    DetallesAuxAPushear.DetalleAuxiliar.push(DetallesAux);
                });
                NumLinea++;
                index++;
                AuxiliaresDetalle.push(DetallesAuxAPushear);
            }else{
                AuxiliaresDetalle.push(null);
                NumLinea++;
            }
        });
        console.log(AuxiliaresDetalle);
}

const GuardarAuxProvDeudor=()=>{
    let CContableProvDeudorID = Number(document.getElementById("AuxCuentaProvDeudor").value);
    let NumeroLinea = Number(document.getElementById("AUXitemProvDeudor").value);
    let Valorlinea = Number(document.getElementById("AUXvalorProvDeudor").value);

    let Fecha = document.getElementById("FechaPrestador").value;
    let TipoReceptorProv = document.getElementById("TipoPrestadorProvDeudor").value;
    let RazonSocialID = Number(document.getElementById("RazonPrestadorProvDeudor").value);
    let Rut = document.getElementById("RutPrestadorProvDeudor").value;

    let EsBoleta = document.getElementById("BoletaVenta").checked;
    let ContalibroCompra = document.getElementById("ContaLibroCompra").checked;
    let ContalibroVenta = document.getElementById("ContaLibroVenta").checked;


    
    if (AuxiliaresDetalle.filter(function(e) {
        if(e!=null){
            return e.NumeroLinea === NumeroLinea
        }
    }).length > 0) {
        AuxiliaresDetalle.map(function(AuxDetalle){
            if(AuxDetalle!=null){
                if(AuxDetalle.NumeroLinea == NumeroLinea){
                    let AuxProvDetalleEditado = {
                        NumeroLinea:NumeroLinea,
                        CuentaContableID:CContableProvDeudorID,
                        Fecha:Fecha,
                        RazonSocialID:RazonSocialID,
                        Rut:Rut,
                        TipoReceptor:TipoReceptorProv,
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
                        FolioDesde:Number(PropiedadesHijos[0].firstChild.value),
                        FolioHasta:Number(PropiedadesHijos[1].firstChild.value),
                        MontoBruto: 0,
                        MontoExento:Number(PropiedadesHijos[4].firstChild.value),
                        MontoIvaActivoLinea:0,
                        MontoIVALinea:Number(PropiedadesHijos[5].firstChild.value),
                        MontoIvaNoRecuperable:0,
                        MontoIvaUsoComun:0,
                        MontoNeto:Number(PropiedadesHijos[3].firstChild.value),
                        MontoRetencion:0,
                        MontoTotalLinea:Number(PropiedadesHijos[6].firstChild.value),
                        TipoDTE:Number(PropiedadesHijos[2].firstChild.value)
                        }
                        AuxProvDetalleEditado.DetalleAuxiliar.push(DetalleAux);
                    })
                    AuxiliaresDetalle.splice(NumeroLinea-1,1, AuxProvDetalleEditado);
                    console.log(AuxiliaresDetalle);
                }
            }
            
        })
    }else{
        let AuxProvDetalle = {
            NumeroLinea:NumeroLinea,
            CuentaContableID:CContableProvDeudorID,
            Fecha:Fecha,
            ValorLinea:Valorlinea,
            TipoReceptor:TipoReceptorProv,
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
                FolioDesde:Number(PropiedadesHijos[0].firstChild.value),
                FolioHasta:Number(PropiedadesHijos[1].firstChild.value),
                TipoDTE:Number(PropiedadesHijos[2].firstChild.value),
                MontoNeto:Number(PropiedadesHijos[3].firstChild.value),
                MontoExento:Number(PropiedadesHijos[4].firstChild.value),
                MontoIVA:Number(PropiedadesHijos[5].firstChild.value),
                MontoTotal:Number(PropiedadesHijos[6].firstChild.value)
            }
            AuxProvDetalle.DetalleAuxiliar.push(DetalleAux);
        })
        AuxiliaresDetalle.push(AuxProvDetalle);
        console.log(AuxiliaresDetalle);
    }
    $('#ModalAuxiliarProvDeudor').modal('toggle');
    
}
const GuardarAuxHonor=()=>{
    let CuentaContableHonorID = Number(document.getElementById("AuxCuenta").value);
    let NumeroLineaHonor = Number(document.getElementById("AUXitem").value);
    let ValorLineaHonor = Number(document.getElementById("AuxValorHonor").value);

    let FechaHonor = document.getElementById("FechaPrestadorHonor").value;
    let TipoHonor = document.getElementById("TipoPrestadorHonor").value;
    let RazonSocialHonorID = Number(document.getElementById("RazonPrestadorHonor").value);
    let RutHonor = document.getElementById("RutPrestadorHonor").value;
    if (AuxiliaresDetalle.filter(function(e) {
        if(e!=null){
            return e.NumeroLinea === NumeroLineaHonor
        }
    }).length > 0) {
        AuxiliaresDetalle.map(function(AuxDetalle){
            if(AuxDetalle!=null){
                if(AuxDetalle.NumeroLinea == NumeroLineaHonor){
                    let AuxHonorDetalleEditado = {
                        NumeroLinea:NumeroLineaHonor,
                        CuentaContableID:CuentaContableHonorID,
                        Fecha:FechaHonor,
                        RazonSocialID:RazonSocialHonorID,
                        Rut:RutHonor,
                        TipoReceptor:TipoHonor,
                        DetalleAuxHonor:[]
                    }
                    let LstAuxHonor = document.getElementsByClassName("GuardarDatosAuxHonor");
                    let LstHijosHonor = [...LstAuxHonor];
                    LstHijosHonor.map(function(data){
                        let propHijos = data.childNodes;
                        let DetalleAux = {
                            Folio:Number(propHijos[0].firstChild.value),
                            MontoBruto:Number(propHijos[1].firstChild.value),
                            MontoExento:0,
                            MontoIvaActivoLinea:0,
                            MontoIVALinea:0,
                            MontoIvaNoRecuperable:0,
                            MontoIvaUsoComun:0,
                            MontoNeto:0,
                            MontoRetencion:Number(propHijos[3].firstChild.value),
                            MontoTotalLinea:Number(propHijos[4].firstChild.value),
                            tipoDTE:0
                        }
                        AuxHonorDetalleEditado.DetalleAuxHonor.push(DetalleAux);
                    })
    
                    AuxiliaresDetalle.splice(NumeroLineaHonor-1,1, AuxHonorDetalleEditado);
                    console.log(AuxiliaresDetalle);
                }
            }
        })
    }else{
        let AuxHonorDetalle = {
            CuentaContableID:CuentaContableHonorID,
            NumeroLinea:NumeroLineaHonor,
            ValorLinea:ValorLineaHonor,
            Fecha:FechaHonor,
            TipoReceptor:TipoHonor,
            RazonSocialID:RazonSocialHonorID,
            Rut:RutHonor,
            DetalleAuxHonor:[]
        }
        let LstAuxHonor = document.getElementsByClassName("GuardarDatosAuxHonor");
        let LstHijosHonor = [...LstAuxHonor];
        LstHijosHonor.map(function(data){
            let propHijos = data.childNodes;
            let DetalleAux = {
                Folio:propHijos[0].firstChild.value,
                ValorBruto:propHijos[1].firstChild.value,
                TipoRetencion:propHijos[2].firstChild.value,
                MontoRetencion:propHijos[3].firstChild.value,
                MontoTotal:propHijos[4].firstChild.value
            }
            AuxHonorDetalle.DetalleAuxiliar.push(DetalleAux);
        })
        AuxiliaresDetalle.push(AuxHonorDetalle);
        console.log(AuxiliaresDetalle);
    }
    $('#ModalAuxiliarHonor').modal('toggle');
}
const GuardarAuxRemu=()=>{

    let CuentaContableRemuID = Number(document.getElementById("AuxRemu").value);
    let NumeroLineaRemu = Number(document.getElementById("AUXitemRemu").value);
    let ValorLineaRemu = Number(document.getElementById("AUXvaloritemRemu").value);

    let FechaRemu = document.getElementById("FechaPrestadorRemu").value;
    let TipoPrestadorRemu = document.getElementById("TipoPrestadorRemu").value;
    let RazonSocialRemuID = Number(document.getElementById("RazonPrestadorRemu").value);
    let RutRemu = document.getElementById("RutPrestadorRemu").value;

    if (AuxiliaresDetalle.filter(function(e) {
        if(e!=null){
            return e.NumeroLinea === NumeroLineaRemu
        }
    }).length > 0) {
        AuxiliaresDetalle.map(function(AuxDetalle){
            if(AuxDetalle!=null){
                if(AuxDetalle.NumeroLinea == NumeroLineaRemu){
                    let AuxRemuDetalleEditado = {
                        NumeroLinea:NumeroLineaRemu,
                        CuentaContableID:CuentaContableRemuID,
                        Fecha:FechaRemu,
                        RazonSocialID:RazonSocialRemuID,
                        Rut:RutRemu,
                        TipoReceptor:TipoPrestadorRemu,
                        DetalleAuxRemuneracion:[]
                    }
                    let LstAuxRemu = document.getElementsByClassName("GuardarDatosAuxRemu");
                    let LstHijosRemu = [...LstAuxRemu];
                    LstHijosRemu.map(function(data){
                        let propHijos = data.childNodes;
                        let DetalleAuxRemu = {
                            Folio:Number(propHijos[0].firstChild.value),
                            MontoBruto:0,
                            MontoExento:0,
                            MontoIvaActivoLinea:0,
                            MontoIVALinea:0,
                            MontoIvaNoRecuperable:0,
                            MontoIvaUsoComun:0,
                            MontoNeto:0,
                            MontoRetencion:0,
                            MontoTotalLinea:Number(propHijos[1].firstChild.value),
                            tipoDTE:0
                        }
                        AuxRemuDetalleEditado.DetalleAuxRemuneracion.push(DetalleAuxRemu);
                    })
                    AuxiliaresDetalle.splice(NumeroLineaRemu-1, 1, AuxRemuDetalleEditado);
                    console.log(AuxiliaresDetalle);
                }
            }    
        })

    }else{
        let AuxRemuDetalle = {
            NumeroLinea:NumeroLineaRemu,
            CuentaContableID:CuentaContableRemuID,
            Fecha:FechaRemu,
            ValorLinea:ValorLineaRemu,
            TipoReceptor:TipoPrestadorRemu,
            RazonSocialID:RazonSocialRemuID,
            Rut:RutRemu,
            DetalleAuxiliarRemu:[]
        }; 
    
        let LstAuxProvDeudor = document.getElementsByClassName("GuardarDatosAuxRemu");
        let LstHijos = [...LstAuxProvDeudor];
        LstHijos.map(function(data) {
            let PropiedadesHijos = data.childNodes;
            let DetalleAux = {
                Folio:PropiedadesHijos[0].firstChild.value,
                MontoBruto:0,
                MontoExento:0,
                MontoIvaActivoLinea:0,
                MontoIVALinea:0,
                MontoIvaNoRecuperable:0,
                MontoIvaUsoComun:0,
                MontoNeto:0,
                MontoRetencion:0,
                MontoTotalLinea:PropiedadesHijos[1].firstChild.value,
                tipoDTE:43
            }
            AuxRemuDetalle.DetalleAuxiliarRemu.push(DetalleAux);
        })
        AuxiliaresDetalle.push(AuxProvDetalle);
        console.log(AuxiliaresDetalle);
    }
    $('#ModalAuxiliarRemu').modal('toggle');
}
