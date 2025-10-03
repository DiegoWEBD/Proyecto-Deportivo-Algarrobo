var myModal;
$(document).ready(Inicio());



function Inicio() {
    MostrarLoader('loader', 'div_contenido')
    IniciarTabla('tabla_reservas', true, true, false, false, false);
    MostrarContenido('loader', 'div_contenido');
    myModal = new bootstrap.Modal(document.getElementById('cancelarClaseModal'));
    var fecha = new Date();
    var year = fecha.getFullYear();
    var mes = fecha.getMonth() + 1;
    var dia = fecha.getDate(); 
    CargarReservas(dia, mes, year);
   
}




function CargarReservas(dia, mes, year) {
   
    $.ajax({
        type: 'POST',
        url: 'BuscarReservas',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year },
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

                var fecha = new Date();
                var year = fecha.getFullYear();
                var mes = fecha.getMonth() + 1;
                var dia = fecha.getDate();
                CargarReservas(dia, mes, year);
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






