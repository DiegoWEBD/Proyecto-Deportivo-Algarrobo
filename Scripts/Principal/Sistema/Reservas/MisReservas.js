var myModal;
$(document).ready(Inicio());



function Inicio() {
    MostrarLoader('loader', 'div_contenido')
    IniciarTabla('tabla_reservas', true, true, false, false, false);
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
    myModal = new bootstrap.Modal(document.getElementById('cancelarClaseModal'));
   
}




function CargarReservas(arreglo_fecha) {
   
    $.ajax({
        type: 'POST',
        url: 'BuscarReservas',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2] },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            var tabla = $('#tabla_reservas').DataTable();
            tabla.clear().draw();
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
                    var onclick = "ActualizarPanel('" + fila[8] + "', '" + fila[3] + "',' " + fila[5]+"')"
                    var buttonBorrar = '<div><button name="button" data-bs-toggle="modal" data-bs-target="#cancelarClaseModal" class="btn btn-danger"  onclick="' +onclick+'">Cancelar</button></div>';
                    tabla.row.add([fila[3], fila[4], fila[5], buttonBorrar]).draw();
                    //clase, fecha, horario, cancelar //
                }
                tabla.responsive.rebuild();
                tabla.responsive.recalc();
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

function ActualizarPanel(idReserva, clase, horario) {
    $('#form_cancelarClases input[name="idReserva"]').empty();
    $('#form_cancelarClases input[name="idReserva"]').val(idReserva);
    $("#contenidoModel").empty();
    $("#contenidoModel").append("Desea cancelar la reserva de " + clase + " de las " + horario);
}


function CancelarReserva(arreglo_fecha) {
    var idReserva =  $('#form_cancelarClases input[name="idReserva"]').val();
    $.ajax({
        type: 'POST',
        url: 'CancelarReserva',
        dataType: 'json',
        data: { idReserva: idReserva},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            if (correcto) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Reserva Cancelada !</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                myModal.hide();
                var fecha_arreglo = ObtenerFecha();
                CargarReservas(fecha_arreglo);
            }
            else {
                var message = JSON.parse(response.Message);
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong>' + message + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
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


function ChangeFecha(entrada) {
    var fecha_arreglo = ObtenerFecha();
    CargarReservas(fecha_arreglo);
}


function ObtenerFecha() {
    var fecha = $('#inputFecha').val();
    var arreglo_fecha = fecha.split("/");
    return arreglo_fecha;
}


