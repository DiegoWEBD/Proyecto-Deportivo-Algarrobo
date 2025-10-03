var myModal;
var myModalContrato;
var myModalSolicitud;
$(document).ready(Inicio());


function Inicio() {
    MostrarLoader('loader', 'div_contenido')
    var dia = getDiasReserva();
    $('#inputFecha').daterangepicker({
        singleDatePicker: true,
        timePicker: false,
        singleClasses: "picker_4",
        timePicker24Hour: false,
        timePickerSeconds: false,
        autoApply: true,
        minDate: moment().endOf("day"),
        maxDate: moment().endOf("day").add(dia, 'days'),
        locale: {
            format: 'DD/MM/YYYY',
            daysOfWeek: [
                "Do",
                "Lu",
                "Ma",
                "Mi",
                "Ju",
                "Vi",
                "Sa"
            ],
            monthNames: [
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo",
                "Junio",
                "Julio",
                "Agosto",
                "Septiembre",
                "Octubre",
                "Noviembre",
                "Deciembre"
            ],
            firstDay: 1
        }
    });

    myModal = new bootstrap.Modal(document.getElementById('modalPagarInstalacion'));
    myModalContrato = new bootstrap.Modal(document.getElementById('modalContratoInstalacion'));
    myModalSolicitud = new bootstrap.Modal(document.getElementById('modalAvisoSolicitud'));
    $("#floatingTextareaAsistentesPadel").attr("readonly", true);
}


function ChangeFecha() {
    var fecha_arreglo = ObtenerFecha();
    $("#form_ReservarHora select[name = TipoInstalacion]").empty();
    $("#form_ReservarHora select[name = instalacion]").empty();
    CargarTipoInstalacion(fecha_arreglo);
    $("#form_ReservarHora select[name = Horario]").empty();
    $("#form_ReservarHora textarea[name = requerimientos]").val("");
    $("#form_ReservarHora textarea[name = Asistentes]").val("");
    $("#floatingTextareaAsistentesPadel").val(""); 
    EstadoCupos(false, true, "-1", false);
   
}


function OcultarHoraLLegada() {
    document.getElementById('ContenedorHoraLlegada').style.display = "none";
}


function MostrarHoraLLegada() {
    document.getElementById('ContenedorHoraLlegada').style.display = "block";
}


function ChangeTipoInstalacion() {
    var fecha_arreglo = ObtenerFecha();
    var tipoInstalacion = $("#form_ReservarHora select[name = TipoInstalacion]").val();
    CargarInstalacion(fecha_arreglo, tipoInstalacion);
    $("#form_ReservarHora select[name = instalacion]").empty();
    $("#form_ReservarHora select[name = Horario]").empty();
    $("#form_ReservarHora textarea[name = requerimientos]").val("");
    $("#form_ReservarHora textarea[name = formHorarioLlegada]").val("");
    $("#floatingTextareaAsistentesPadel").val(""); 
    EstadoCupos(false, true, "-1", false);
}


function ChangeSelectInstalacion() {
    var fecha_arreglo = ObtenerFecha();
    EstadoCupos(false, true, "-1", false);
    $("#form_ReservarHora select[name = Horario]").empty();
    $("#form_ReservarHora textarea[name = requerimientos]").val("");
    $("#form_ReservarHora textarea[name = formHorarioLlegada]").val("");
    $("#floatingTextareaAsistentesPadel").val(""); 
    var objeto = $("#form_ReservarHora select[name = instalacion]").val();
    if (objeto.localeCompare("-1") != 0) {
        var nombreInstalacion = $("#form_ReservarHora select[name = instalacion]").text();
        var idInstalacion = objeto;
        CargarHorario(fecha_arreglo, idInstalacion);
        if ((idInstalacion == 47) || (idInstalacion == 48) || (idInstalacion == 49) || (idInstalacion == 50)) {
            AsistentesPadel(true);
        }
        else {
            AsistentesPadel(false);
        }
    }
    else {
        EstadoCupos(false, true, "-1", false);
    }


}


function ChangeSelectHorario() {
    var fecha_arreglo = ObtenerFecha();
    var idInstalacion = $("#form_ReservarHora select[name = instalacion]").val();
    var objeto = $("#form_ReservarHora select[name = Horario]").val();
    $("#form_ReservarHora textarea[name = requerimientos]").val("");
    $("#form_ReservarHora textarea[name = formHorarioLlegada]").val("");
    $("#floatingTextareaAsistentesPadel").val(""); 
    if (objeto.localeCompare("-1") != 0) {
        var idCalendario = objeto;
        ComprobarCupos(fecha_arreglo, idCalendario, idInstalacion);
    }
    else {
        EstadoCupos(false, true, "-1", false);
    }
}


function EnviarReserva() {
    var fecha_arreglo = ObtenerFecha();
    var dia = fecha_arreglo[0];
    var mes = fecha_arreglo[1];
    var year = fecha_arreglo[2];
    var idCalendario = $("#form_ReservarHora select[name = Horario]").val();

    var idInstalacion = $("#form_ReservarHora select[name = instalacion]").val();
    var nombreInstalacion = $("#form_ReservarHora select[name = instalacion] option:selected").text();
    var horaro_text = $("#form_ReservarHora select[name = Horario] option:selected").text();

    var requerimientos_text = $("#form_ReservarHora textarea[name = requerimientos]").val();
    var llegada = $("#form_ReservarHora textarea[name = formHorarioLlegada]").val();

    var asistentes = $("#form_ReservarHora input[name = Asistentes]");
    var dataString;
    var dataForm = new FormData();
    dataString = asistentes[0].files[0];
   
    
    
    dataForm.append("dia", dia);
    dataForm.append("mes", mes);
    dataForm.append("year", year);
    dataForm.append("idCalendario", idCalendario);
    dataForm.append("idInstalacion", idInstalacion);
    dataForm.append("Requerimientos", requerimientos_text);
    dataForm.append("Asistentes", dataString);
    dataForm.append("LLegada", llegada);
    


    if ((idInstalacion == 43)|| (idInstalacion == 46)) { //43 es el indicador de salon de  eventos
        AjaxCorreoSolicitud(dia, mes, year, nombreInstalacion, requerimientos_text);
    }
    else {
        if ((idInstalacion == 47) || (idInstalacion == 48) || (idInstalacion == 49) || (idInstalacion == 50)) {
            //aca llamar para canchas de padel
            var asistentesPadel = $("#floatingTextareaAsistentesPadel").val();
            dataForm.append("AsistentesPadel", asistentesPadel);
            AjaxIniciarModalPagoPadel(dia, mes, year, idCalendario, nombreInstalacion, horaro_text, idInstalacion, requerimientos_text, dataForm);
        }
        else {
            AjaxIniciarModalPago(dia, mes, year, idCalendario, nombreInstalacion, horaro_text, idInstalacion, requerimientos_text, dataForm);
        }
        
    }
    

}


function getDiasReserva() {
    return 31;
    var diaSemana = new Date().getDay();
    if ((diaSemana >= 1) && (diaSemana <= 4)) {
        return 1;
    }
    if (diaSemana == 5) {
        return 3;
    }
    if (diaSemana == 6) {
        return 2;
    }
    if (diaSemana == 0) {
        return 1;
    }
    return 1;
}


function ObtenerFecha() {
    var fecha = $('#inputFecha').val();
    var arreglo_fecha = fecha.split("/");
    return arreglo_fecha;
}


function EstadoCupos(estado, defecto, Monto, esSocio) {
    const noTruncarDecimales = { maximumFractionDigits: 20 };
    $("#form_ReservarHora select[name = Cupos]").empty();
    $("#form_ReservarHora select[name = Precio]").empty();
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-danger");
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-success");
    $("#form_ReservarHora select[name = Precio]").removeClass("alert-danger");
    $("#form_ReservarHora select[name = Precio]").removeClass("alert-success");
    //$("#form_ReservarHora select[name = Cupos]").removeClass("rojo");
    if (defecto == false) {
        if (estado == true) {
            $("#form_ReservarHora select[name = Cupos]").addClass("alert-success");
            $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
                value: 1,
                text: "Instalacion Disponibles"
            }));

            $("#form_ReservarHora select[name = Precio]").addClass("alert-success");
            $("#form_ReservarHora select[name = Precio]").append($('<option>', {
                value: Monto,
                text: parseInt(Monto).toLocaleString('es', noTruncarDecimales)
            }));

            $("#form_ReservarHora button[name = button]")[0].disabled = false;
        }
        else {
            $("#form_ReservarHora select[name = Cupos]").addClass("alert-danger");
            $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
                value: 0,
                text: "Instalacion No Disponible"
            }));
            $("#form_ReservarHora button[name = button]")[0].disabled = true;
        }
    }
    else {
        $("#form_ReservarHora button[name = button]")[0].disabled = true;
    }

}


function CargarSelectTipoInstalacion(select, array_entrada) {
    select.empty();
    array_entrada.forEach(function (Value, Index) {
        var idClas = Value[2];
        var Clas = Value[0];
        select.append($('<option>', {
            value: Clas,
            text: idClas
        }));
    });

    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay clases disponibles' }));
    }

    select[0].selectedIndex = -1;
}


function CargarSelectClase(select, array_entrada) {

    select.empty();

    array_entrada.forEach(function (Value, Index) {
        var idClas = Value[2];
        var Clas = Value[0];
        select.append($('<option>', {
            value: idClas,
            text: Clas
        }));
    }
    );
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay clases disponibles' }));
    }


    select[0].selectedIndex = -1;
}


function CargarSelectHorario(select, array_entrada) {
    select.empty();

    array_entrada.forEach(function (Value, Index) {
        var idCal = Value[0];
        var idUbi = Value[1];
        var idHor = Value[2];
        var bloq = Value[3];
        var detalle = Value[4];
        var correcto = ComprobarHorario(detalle);
        if (detalle == 'Día Completo') {
            MostrarHoraLLegada();
        }
        if (correcto) {
            select.append($('<option>', {
                value: idCal,
                text: detalle
            }));
        }
    }
    );
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay horario disponible' }));
    }
    select[0].selectedIndex = -1;
}


function ComprobarHorario(entrada) {


    if (entrada.indexOf("/") == -1) {
        return true;
    }

    if (esHoy()) {
        var today = new Date();
        // obtener la fecha y la hora Actual
        var now = today.toLocaleTimeString();
        var resultados = now.split(":");
        var hora = resultados[0];
        var minutos = resultados[1];

        //obtener hora entrada
        var hora_entrada = entrada.substring(0, 5);
        var hora_seleccionada = hora_entrada.split(":")[0];
        var minutos_seleccionada = hora_entrada.split(":")[1];

        ///llevar  a enteros (fracciones)
        var int_hora = parseInt(hora);
        var int_minutos = parseInt(minutos);
        int_hora = int_hora + (int_minutos / 60);

        var int_hora_seleccionada = parseInt(hora_seleccionada);
        var int_minutos_seleccionada = parseInt(minutos_seleccionada);
        int_hora_seleccionada = int_hora_seleccionada + (int_minutos_seleccionada / 60);
        //se resta una hora // nunca vamos a restar una hora alas 00 (no abren a medio dia)
        int_hora_seleccionada = int_hora_seleccionada - 1;

        //si mi hora es menor a la seleecionada la agrego
        if (int_hora < int_hora_seleccionada) {
            return true;
        }
        return false;
    }
    return true;
}


function esHoy() {
    arreglo_fecha = ObtenerFecha();
    var dia_seleccionado = arreglo_fecha[0];
    var mes_seleccionado = arreglo_fecha[1];
    var year_seleccionado = arreglo_fecha[2];

    var int_dia = parseInt(dia_seleccionado);
    var int_mes = parseInt(mes_seleccionado);
    var int_year = parseInt(year_seleccionado);


    var now = new Date();
    var year = now.getFullYear();
    var mes = now.getMonth() + 1;
    var dia = now.getDate();

    if (int_year <= year) {
        if (int_mes <= mes) {
            if (int_dia <= dia) {
                return true;
            }
        }
    }

    return false;
}


function ActivarContrato() {
    //myModal.hide();
    
    $("#instalacion_contrato").empty();
    $("#fechaArriendo_contrato").empty();
    $("#precio_contrato").empty();
    


    var idInstalacion = $("#form_ReservarHora select[name = instalacion]").val();
    var nombreInstalacion = $("#form_ReservarHora select[name = instalacion] option:selected").text();
    var precio = $("#form_ReservarHora select[name = Precio]").val();
    var horario = $("#form_ReservarHora select[name = Horario] option:selected").text();
    var tipoArriendo = $("#form_ReservarHora select[name = Horario] option:selected").text();
    arreglo_fecha = ObtenerFecha();
    var dia = arreglo_fecha[0];
    var mes = arreglo_fecha[1];
    var year = arreglo_fecha[2];

    if (horario.indexOf("/") >= 0) {
        horario = horario.replace('/', ' ');
        var array_horario = horario.split(" ")
        horario = " entre las  " + array_horario[0] + " hasta " + array_horario[1];
    }

    var fechaCompleta = dia + " de " + ObtenerMes(mes) + " del " + year;
    fechaCompleta = fechaCompleta + horario;

    $("#instalacion_contrato").append(nombreInstalacion);
    $("#fechaArriendo_contrato").append(fechaCompleta);
    $("#precio_contrato").append(precio);

    myModalContrato.show();
}


function ObtenerMes(mes) {
    var int_mes = parseInt(mes);
    switch (int_mes) {
        case 1:
            return "Enero";
        case 2:
            return "Febrero";
        case 3:
            return "Marzo";
        case 4:
            return "Abril";
        case 5:
            return "Mayo";
        case 6:
            return "Junio";
        case 7:
            return "Julio";
        case 8:
            return "Agosto";
        case 9:
            return "Septiembre";
        case 10:
            return "Octubre";
        case 11:
            return "Noviembre";
        case 12:
            return "Diciembre";
        default:
            return mes;
    }
}


function CancelarTerminos() {
   
}


function EstadoPreReservao(texto) {
    $("#form_ReservarHora select[name = Cupos]").empty();
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-danger");
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-success");
    $("#form_ReservarHora select[name = Cupos]").addClass("alert-danger");
    $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
        value: 0,
        text: texto
    }));
    $("#form_ReservarHora button[name = button]")[0].disabled = true;
}

function CargarInstalacion(arreglo_fecha, tipoInstalacion) {
    $.ajax({
        type: 'POST',
        url: 'CargarInstalacion',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], tipoInstalacion: tipoInstalacion  },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            var mensaje = JSON.parse(response.Message);
            if (correcto == true) {
                CargarSelectClase($("#form_ReservarHora select[name = instalacion]"), mensaje);
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function CargarHorario(arreglo_fecha, id_instalacion) {

    OcultarHoraLLegada();
    $.ajax({
        type: 'POST',
        url: 'CargarInstalacionHorario',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idClase: id_instalacion },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectHorario($("#form_ReservarHora select[name = Horario]"), mensaje)
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function ComprobarCupos(arreglo_fecha, id_calendario, id_instalacion, id_horario) {
    $.ajax({
        type: 'POST',
        url: 'TieneCuposDisponibles',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idCalendario: id_calendario, idInstalacion: id_instalacion, id_horario: id_horario},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var feriado = JSON.parse(response.Message);
                if (feriado == 'FERIADO') {
                    EstadoPreReservao("¡Reservas de Instalación Cancelada Por Día Festivo!");
                }
                else {
                    var cancelacion = JSON.parse(response.Cancelacion);
                    var existeCupo = mensaje;
                    var monto = JSON.parse(response.Monto);
                    if (cancelacion == true) {
                        EstadoPreReservao("¡Reservas de Instalación Cancelada!");
                    }
                    else {
                        var requiereAsistencia = false;
                        if (JSON.parse(response.Exigencia) != null) {
                            requiereAsistencia = JSON.parse(response.Exigencia);
                        }

                        if ((id_instalacion == 47) || (id_instalacion == 48) || (id_instalacion == 49) || (id_instalacion == 50)) {
                            document.querySelector('#inputAsistentes').required = false;
                            document.querySelector('#floatingTextareaAsistentesPadel').required = true;
                        }
                        else {
                            document.querySelector('#inputAsistentes').required = requiereAsistencia;
                        }
                        


                        //div_asistentes//
                        if ((id_instalacion == 47) || (id_instalacion == 48) || (id_instalacion == 49) || (id_instalacion == 50)) {
                            //no hacer mada porque lo hago antes
                        }
                        else {
                            if (requiereAsistencia == false) {
                                document.getElementById("div_asistentes").style.display = "none";
                            } else {
                                document.getElementById("div_asistentes").style.display = "inline";
                            }
                        }
                        


                        if (existeCupo) {
                            EstadoCupos(true, false, monto, false);
                        }
                        else {
                            EstadoCupos(false, false, monto, false);
                        }
                    }
                }
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


//data: { dia: dia, mes: mes, year: year, idCalendario: idCalendario, idInstalacion: idInstalacion, Requerimientos: requerimientos },
function AjaxIniciarModalPago(dia, mes, year, idCalendario, nombreInstalacion, horaro_text, idInstalacion, requerimientos, form) {

    $.ajax({
        type: 'POST',
        url: 'CrearEnlaceArriendo',
        dataType: 'json',
        data: form,
        contentType: false,
        processData: false,

        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var socio = JSON.parse(response.Socio);
                if (socio == true) {
                    $("#form_PagarInstalacion input[name = instalacion]").val(nombreInstalacion);
                    $("#form_PagarInstalacion input[name = fecha]").val(mensaje[3]);
                    $("#form_PagarInstalacion input[name = horario]").val(horaro_text);
                    $("#form_PagarInstalacion input[name = Valor]").val("0");
                    ///// aca el action debe ser el evento en controlador para reservar a socio
                    $("#form_PagarInstalacion input[name = token_ws]").val("");
                    $("#form_PagarInstalacion")[0].target = "";
                    $("#form_PagarInstalacion")[0].action = "javascript:" + mensaje[0] + ";";
                    $("#botonPagar").html("Reservar");
                }
                else {
                    //completar valores
                    $("#form_PagarInstalacion input[name = Valor]").val(mensaje[2]);
                    $("#form_PagarInstalacion input[name = fecha]").val(mensaje[3]);
                    $("#form_PagarInstalacion input[name = instalacion]").val(nombreInstalacion);
                    $("#form_PagarInstalacion input[name = horario]").val(horaro_text);

                    $("#form_PagarInstalacion input[name = token_ws]").val(mensaje[1]);
                    $("#form_PagarInstalacion")[0].action = mensaje[0];
                   // $("#form_PagarInstalacion")[0].target = "_blank";
                    $("#botonPagar").html("Pagar");
                }
                myModal.show();
            }
            else {
                EstadoMensualidad(false, true, "");
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> ' + mensaje + '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            }

        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            //MostrarContenido('loader', 'div_contenido');
        }
    });
}


function AjaxIniciarModalPagoPadel(dia, mes, year, idCalendario, nombreInstalacion, horaro_text, idInstalacion, requerimientos, form) {
    $.ajax({
        type: 'POST',
        url: 'CrearEnlaceArriendoPadel',
        dataType: 'json',
        data: form,
        contentType: false,
        processData: false,

        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                //completar valores
                $("#form_PagarInstalacion input[name = Valor]").val(mensaje[2]);
                $("#form_PagarInstalacion input[name = fecha]").val(mensaje[3]);
                $("#form_PagarInstalacion input[name = instalacion]").val(nombreInstalacion);
                $("#form_PagarInstalacion input[name = horario]").val(horaro_text);

                $("#form_PagarInstalacion input[name = token_ws]").val(mensaje[1]);
                $("#form_PagarInstalacion")[0].action = mensaje[0];
                // $("#form_PagarInstalacion")[0].target = "_blank";
                $("#botonPagar").html("Pagar");
                
                myModal.show();
            }
            else {
                EstadoMensualidad(false, true, "");
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> ' + mensaje + '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            }

        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            //MostrarContenido('loader', 'div_contenido');
        }
    });
}


function ajaxGuardarReservaInstalacion(dia, mes, year, idCalendario, requerimientos, llegada) {
    var asistentes = $("#form_ReservarHora input[name = Asistentes]");
    var dataString = asistentes[0].files[0];
    var dataForm = new FormData();

    dataForm.append("dia", dia);
    dataForm.append("mes", mes);
    dataForm.append("year", year);
    dataForm.append("idCalendario", idCalendario);
    dataForm.append("Requerimientos", requerimientos);
    dataForm.append("Asistentes", dataString);
    dataForm.append("Llegada", llegada);
  
    $.ajax({
        type: 'POST',
        url: 'GuardarReservaInstalacion',
        dataType: 'json',
        data: dataForm,
        contentType: false,
        processData: false,
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
            myModal.hide();
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var existeCupo = mensaje;
                if (existeCupo == true) {
                    ChangeFecha();
                    EstadoCupos(false, true, 0, 0);
                    $('#Error_ReservarHora').empty();
                    $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> ¡Reserva Realizada!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');

                }
                else {
                    $('#Error_ReservarHora').empty();
                    $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> Estado: !</strong>Ya No Quedan Cupos! <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                }
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function AjaxCorreoSolicitud(dia, mes, year,  nombreInstalacion, requerimientos) {

    $.ajax({
        type: 'POST',
        url: 'CorreoSolicitudArriendo',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year, nombreInstalacion: nombreInstalacion, Requerimientos: requerimientos },

        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                //myModalContrato.hide()
                myModalSolicitud.show();
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Solicitud Enviada con Éxito, Pronto se Contactaran con Usted. </strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> ' + mensaje + '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            }
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            //MostrarContenido('loader', 'div_contenido');
        }
    });
}


function CargarTipoInstalacion(arreglo_fecha) {
    $.ajax({
        type: 'POST',
        url: 'CargarTipoInstalacion',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2] },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            var mensaje = JSON.parse(response.Message);
            if (correcto == true) {
                CargarSelectTipoInstalacion($("#form_ReservarHora select[name = TipoInstalacion]"), mensaje); //modificar esto
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}

//#endregion


function LimpiarFormPadel() {
    $("#rutAsistentePadel").val("");
    $("#nombreAsistentePadel").val("");
    
}

function VerificarUsuarioPadelAsignar() {
    var nombre = $("#nombreAsistentePadel").val();
    var rut = $("#rutAsistentePadel").val();


    if ((nombre.localeCompare("") == 0) && (rut.localeCompare("") == 0)) {

    }
    else {
        if (rut.localeCompare("") == 0) {
            rut = "SIN RUT"
        }
        nombre = "(" + rut + ")" + nombre;
        var anterior = $("#floatingTextareaAsistentesPadel").val();
        if (anterior.length > 0) {
            anterior = anterior + "\r\n";
        }
        $("#floatingTextareaAsistentesPadel").val(anterior + nombre); 
    }

}


function AsistentesPadel(mostrar) {
    if (mostrar == false) {
        document.getElementById('div_asistentesPadel').style.display = "none"; // ocultar padel
        document.getElementById('div_asistentes').style.display = "block"; //mostrar otros
    }
    else {
        document.getElementById('div_asistentesPadel').style.display = "block"; // ocultar otros
        document.getElementById('div_asistentes').style.display = "none"; // mostarr padel
    }
    
}