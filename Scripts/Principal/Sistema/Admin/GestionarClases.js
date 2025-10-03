var myModal;
$(document).ready(Inicio());



function Inicio() {
    MostrarLoader('loader', 'div_contenido')
    $('#inputFecha').daterangepicker({
        singleDatePicker: true,
        timePicker: false,
        singleClasses: "picker_4",
        timePicker24Hour: false,
        timePickerSeconds: false,
        autoApply: true,
        minDate: moment().endOf("day"),
        maxDate: moment().endOf("day").add(7, 'days'),
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
    MostrarContenido('loader', 'div_contenido');
    IniciarTabla('tabla_reservas', true, true, false, false, false, true);
    myModal = new bootstrap.Modal(document.getElementById('cancelarClaseModal'));
}

function CargarClase(arreglo_fecha) {
    $.ajax({
        type: 'POST',
        url: 'CargarClase',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2] },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                //$('#Error_ReservarHora').empty();
                //$("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>Reserva Realizada Con Exito <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                CargarSelectClase($("#form_AdminClases select[name = Clase]"), mensaje);
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


function CargarHorario(arreglo_fecha, id_clase, clase) {
    $.ajax({
        type: 'POST',
        url: 'CargarClaseHorario',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idClase: id_clase },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectHorario($("#form_AdminClases select[name = Horario]"), mensaje)
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


function CargarDatos(arreglo_fecha, id_calendario) {
    $.ajax({
        type: 'POST',
        url: 'CargarInformacionClase',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idCalendario: id_calendario },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            var tabla = $('#tabla_reservas').DataTable();
            tabla.clear().draw();
            EstadoClases(false, true);
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var tabla = $('#tabla_reservas').DataTable();
                tabla.clear().draw();
                MostrarContenido('loader', 'div_contenido');
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var onclick = "ActualizarPanel('" + fila[8] + "', '" + fila[3] + "',' " + fila[5] + "',' " + fila[0] + "',' " + fila[1] + "',' " + fila[9] + "')"
                    var buttonBorrar = '<div><button name="button" data-bs-toggle="modal" data-bs-target="#cancelarClaseModal" class="btn btn-danger" onclick="' + onclick +'" >Cancelar</button></div>';
                    var telefono = fila[6];
                    var correo = fila[7];
                    var contacto = '<a href="https://api.whatsapp.com/send?phone=56' + telefono + '" target="_blank"><i class="fab fa-whatsapp"></i></a>';
                    contacto = contacto + ' <a href = "mailto: ' + correo + '"  target="_blank"><i class="far fa-envelope"></i></a>';
                    var telefono = ' <a href="tel:+34555005500"><i class="fas fa-phone"></i></a>';
                    
                    contacto = contacto + telefono;
                    const socio = (!fila[10]) ? 'No Socio' : fila[10]

                    tabla.row.add([fila[9], fila[0], fila[1], fila[2], fila[3], fila[4], fila[5], socio, contacto, buttonBorrar]).draw();
                }
                tabla.responsive.rebuild();
                tabla.responsive.recalc();
                EstadoClases(true, false);
                var cancelado = JSON.parse(response.Cancelado);
                if (cancelado == true) {
                    $("#form_AdminClases button[name = button]")[0].disabled = true;
                    $('#Error_ReservarHora').empty();
                    $("#Error_ReservarHora").append('<div  class="alert alert-warning alert-dismissible fade show"  role="alert" ><strong>¡Clase Cancelada!</strong><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
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

function ActualizarPanel(idReserva, clase, horario, nombre, apellido, userName) {
    $('#form_cancelarClases input[name="idReserva"]').empty();
    $('#form_cancelarClases input[name="idReserva"]').val(idReserva);
    $('#form_cancelarClases input[name="userName"]').empty();
    $('#form_cancelarClases input[name="userName"]').val(userName);
    $("#contenidoModel").empty();
    $("#contenidoModel").append("Desea cancelar la reserva de "+nombre+" "+apellido+" para " + clase + " de las " + horario);
}


function CancelarReservaUsuario() {

    var idReserva = $("#form_cancelarClases input[name = idReserva]").val();
    var userName = $("#form_cancelarClases input[name = userName]").val();

    $.ajax({
        type: 'POST',
        url: 'CancelarReserva',
        dataType: 'json',
        data: { idReserva: idReserva, userName: userName },
        beforeSend: function () {
            //var tabla = $('#tabla_reservas').DataTable();
            //tabla.clear().draw();
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Reserva Cancelada con Éxito:!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                ChangeFecha();
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR!: </strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            myModalCancelarClase.hide();
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

function ChangeFecha(entrada) {
    var fecha_arreglo = ObtenerFecha();
    CargarClase(fecha_arreglo);
    $("#form_AdminClases select[name = Horario]").empty();
    EstadoClases(false, true);
}


function ChangeSelectClase(entrada) {
    var fecha_arreglo = ObtenerFecha();
    EstadoClases(false, true);
    var objeto = $("#form_AdminClases select[name = Clase]").val();
    if (objeto.localeCompare("-1") != 0) {
        var nombreClase = $("#form_AdminClases select[name = Clase]").text();
        var idClase = objeto;
        CargarHorario(fecha_arreglo, idClase, nombreClase);
    }
    else {
        $("#form_AdminClases select[name = Horario]").empty();
    }
    
}


function ChangeSelectHorario(entrada) {
    var fecha_arreglo = ObtenerFecha();
    var objeto = $("#form_AdminClases select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        var idCalendario = objeto;
        CargarDatos(fecha_arreglo, idCalendario);
    }
    else {
        EstadoClases(false, true);
    }

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

        select.append($('<option>', {
            value: idCal,
            text: detalle
        }));
    }
    );
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay horario disponible' }));
    }


    select[0].selectedIndex = -1;
}

function ObtenerFecha() {
    var fecha = $('#inputFecha').val();
    var arreglo_fecha = fecha.split("/");
    return arreglo_fecha;
}

function EstadoClases(estado, defecto) {
    if (defecto == false) {
        if (estado == true) {
            $("#form_AdminClases button[name = button]")[0].disabled = false;
        }
        else {
            $("#form_AdminClases button[name = button]")[0].disabled = true;
        }
    }
    else {
        $("#form_AdminClases button[name = button]")[0].disabled = true;
    }
}

function cancelarReserva() {
    //envio id calndario + fecha
    var fecha_arreglo = ObtenerFecha();
    var nombreClase = $('#form_AdminClases select[name = Clase]').find(":selected").text();
    var nombreHorario = $('#form_AdminClases select[name = Horario]').find(":selected").text();
    
    var objeto = $("#form_AdminClases select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        var idCalendario = objeto;
        CancelarClaseConfirmado(fecha_arreglo, idCalendario, nombreClase, nombreHorario);
    }
    else {
        EstadoClases(false, true);
    }
}

function CancelarClaseConfirmado(arreglo_fecha, id_calendario, nombreClase, horarioClase)
{
    $.ajax({
        type: 'POST',
        url: 'EnviarCorreosCancelacion',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idCalendario: id_calendario, motivo: "Descripción motivo", nombreClase, horarioClase  },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            var tabla = $('#tabla_reservas').DataTable();
            tabla.clear().draw();
            EstadoClases(false, true);
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                //enviar un correcto
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Cancelación Exitosa</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            else {
                var mensaje = JSON.parse(response.Message);
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