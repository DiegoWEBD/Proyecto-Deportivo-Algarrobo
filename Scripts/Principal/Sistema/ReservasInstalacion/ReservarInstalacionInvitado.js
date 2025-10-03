var myModal;
var myModalContrato;
var myModalInvitado;
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
    myModalInvitado = new bootstrap.Modal(document.getElementById('datosInvitadoModal'));
    myModalSolicitud = new bootstrap.Modal(document.getElementById('modalAvisoSolicitud'));
}


function ChangeFecha() {
    var fecha_arreglo = ObtenerFecha();
    $("#form_ReservarHora select[name = TipoInstalacion]").empty();
    $("#form_ReservarHora select[name = instalacion]").empty();
    CargarTipoInstalacion(fecha_arreglo);
    $("#form_ReservarHora select[name = Horario]").empty();
    $("#form_ReservarHora textarea[name = requerimientos]").val("");
    EstadoCupos(false, true, "-1", false);
   
}


function ChangeTipoInstalacion() {
    var fecha_arreglo = ObtenerFecha();
    var tipoInstalacion = $("#form_ReservarHora select[name = TipoInstalacion]").val();
    CargarInstalacion(fecha_arreglo, tipoInstalacion);
    $("#form_ReservarHora select[name = instalacion]").empty();
    $("#form_ReservarHora select[name = Horario]").empty();
    $("#form_ReservarHora textarea[name = requerimientos]").val("");
    EstadoCupos(false, true, "-1", false);
}


function ChangeSelectInstalacion() {
    var fecha_arreglo = ObtenerFecha();
    EstadoCupos(false, true, "-1", false);
    $("#form_ReservarHora select[name = Horario]").empty();
    var objeto = $("#form_ReservarHora select[name = instalacion]").val();
    $("#form_ReservarHora textarea[name = requerimientos]").val("");
    if (objeto.localeCompare("-1") != 0) {
        var nombreInstalacion = $("#form_ReservarHora select[name = instalacion]").text();
        var idInstalacion = objeto;
        CargarHorario(fecha_arreglo, idInstalacion);
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

    var rut_usuario = $("#FormDatosUsuario input[name = nombreUsuario]").val();

    var nombre = $("#FormDatosUsuario input[name = nombre]").val();
    var apellidoPaterno = $("#FormDatosUsuario input[name = apellidoPaterno]").val();
    var apellidoMaterno = $("#FormDatosUsuario input[name = apellidoMaterno]").val();
    var correo = $("#FormDatosUsuario input[name = email]").val();
    var telefono = $("#FormDatosUsuario input[name = telefono]").val();
    var texto_requerimientos = $("#FormDatosUsuario input[name = comentario]").val();


    if (idInstalacion == 43) { //43 es el indicador de salon de  eventos
        AjaxCorreoSolicitud(dia, mes, year, nombreInstalacion, texto_requerimientos, rut_usuario, nombre, apellidoPaterno, apellidoMaterno, correo, telefono  );
    }
    else {
        AjaxIniciarModalPago(dia, mes, year, idCalendario, nombreInstalacion, horaro_text, idInstalacion, rut_usuario, texto_requerimientos, nombre, apellidoPaterno, apellidoMaterno, correo, telefono);
    }

    

}


function getDiasReserva() {
    return 15;
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



function ActivarModalInvitado() {
    myModalInvitado.show();
}


function ActivarContrato() {
    myModalInvitado.hide();
    
    $("#instalacion_contrato").empty();
    $("#fechaArriendo_contrato").empty();
    $("#precio_contrato").empty();
    $("#nombreCompletoTitulo").empty();
    $("#nombreCompletoTexto").empty();
    $("#rutTexto").empty();
    

    var idInstalacion = $("#form_ReservarHora select[name = instalacion]").val();
    var nombreInstalacion = $("#form_ReservarHora select[name = instalacion] option:selected").text();
    var precio = $("#form_ReservarHora select[name = Precio]").val();
    var horario = $("#form_ReservarHora select[name = Horario] option:selected").text();
    var tipoArriendo = $("#form_ReservarHora select[name = Horario] option:selected").text();

    var nombre = $("#FormDatosUsuario input[name = nombre]").val();
    var app = $("#FormDatosUsuario input[name = apellidoPaterno]").val();
    var apm = $("#FormDatosUsuario input[name = apellidoMaterno]").val();
    var nombreCompleto = nombre + " " + app + " " + apm
    var rut = $("#FormDatosUsuario input[name = nombreUsuario]").val();

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

    
    $("#nombreCompletoTitulo").append(nombreCompleto);
    $("#nombreCompletoTexto").append(nombreCompleto);
    $("#rutTexto").append("RUT. "+rut);

    
    
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
    //myModalContrato.hide();
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
                var cancelacion = JSON.parse(response.Cancelacion);
                var existeCupo = mensaje;
                var monto = JSON.parse(response.Monto);
                if (cancelacion == true) {
                    EstadoPreReservao("¡Reservas de Instalación Cancelada!");
                }
                else {
                    if (existeCupo) {
                        EstadoCupos(true, false, monto, false);
                    }
                    else {
                        EstadoCupos(false, false, monto, false);
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


function AjaxIniciarModalPago(dia, mes, year, idCalendario, nombreInstalacion, horaro_text, idInstalacion, rut_usuario, comentarios, nombre, apellidoPaterno, apellidoMaterno, Correo, Telefono) {

    $.ajax({
        type: 'POST',
        url: 'CrearEnlaceArriendoInvitado',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year, idCalendario: idCalendario, idInstalacion: idInstalacion, rutUsuario: rut_usuario, Comentarios: comentarios, Nombre: nombre, App: apellidoPaterno, Apm: apellidoMaterno, Correo: Correo, Telefono: Telefono },
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



function ajaxGuardarReservaInstalacion(dia, mes, year, idCalendario, rut, comentarios, nombreCompleto, correo ) {
    $.ajax({
        type: 'POST',
        url: 'GuardarReservaInstalacionInvitado',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year, idCalendario: idCalendario, Rut: rut, Comentario: comentarios, nombreCompleto: nombreCompleto, Correo: correo},
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



function AjaxCorreoSolicitud(dia, mes, year, nombreInstalacion, requerimientos, rut_usuario, nombre, apellidoPaterno, apellidoMaterno, correo, telefono) {

    $.ajax({
        type: 'POST',
        url: 'CorreoSolicitudArriendoInvitado',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year, nombreInstalacion: nombreInstalacion, Requerimientos: requerimientos, nombre: nombre, apellidoPaterno: apellidoPaterno, apellidoMaterno: apellidoMaterno, Correo: correo, Telefono: telefono },

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
