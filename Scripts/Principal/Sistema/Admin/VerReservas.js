$(document).ready(Inicio());


function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    MostrarContenido('loader', 'div_contenido');
    IniciarTablaImprimir('tabla_reservas', true, true, true, false, true);
    $('#inputFecha').daterangepicker({
        singleDatePicker: true,
        timePicker: false,
        singleClasses: "picker_4",
        timePicker24Hour: false,
        timePickerSeconds: false,
        autoApply: true,
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
                CargarSelectClase($("#form_ReservarHora select[name = Clase]"), mensaje);
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

function BuscarReservas(arreglo_fecha, idCalendario, idClase) {

    $.ajax({
        type: 'POST',
        url: 'BuscarReservas',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idClase: idClase, idCalendario: idCalendario },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            var tabla = $('#tabla_reservas').DataTable();
            tabla.clear().draw();
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            console.log(mensaje)
            if (correcto == true) {
                ///cargar tabla
                var tabla = $('#tabla_reservas').DataTable();
                MostrarContenido('loader', 'div_contenido');
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    let detallePago = 'Socio'
                    if (fila[9] != '') {
                        detallePago = `Socio (${fila[9]})`
                    } else if (fila[10]) {
                        detallePago = 'Mensualidad'
                    } else {
                        detallePago = 'Por Clase'
                    }
                     //console.log(fila[9])
                    //detallePago = fila[9] ? 'Mensualidad' : 'Por Clase'
                    tabla.row.add([fila[8], fila[0], fila[1], fila[2], fila[3], fila[4], fila[5], detallePago]).draw();
                }
                tabla.responsive.rebuild();
                tabla.responsive.recalc();
                
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

function ChangeFecha(entrada) {
    var fecha_arreglo = ObtenerFecha();
    CargarClase(fecha_arreglo);
    $("#form_ReservarHora select[name = Horario]").empty();
    var tabla = $('#tabla_reservas').DataTable();
    tabla.clear().draw();
}

function ChangeSelectClase(entrada) {
    var fecha_arreglo = ObtenerFecha();
    var objeto = $("#form_ReservarHora select[name = Clase]").val();
    if (objeto.localeCompare("-1") != 0) {
        var nombreClase = $("#form_ReservarHora select[name = Clase]").text();
        var idClase = objeto;
        CargarHorario(fecha_arreglo, idClase, nombreClase);
    }
    else {
        $("#form_ReservarHora select[name = Horario]").empty();
        $("#form_ReservarHora select[name = Horario]").append($('<option>', {
            value: "-1",
            text: "Todas Los Horarios"
        }));
    }
    var tabla = $('#tabla_reservas').DataTable();
    tabla.clear().draw();
}

function ChangeSelectHorario(entrada) {
    var fecha_arreglo = ObtenerFecha();
    var objeto = $("#form_ReservarHora select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        var idCalendario = objeto;
    }
    var tabla = $('#tabla_reservas').DataTable();
    tabla.clear().draw();
}

function ClickBuscarReservas() {
    var fecha_arreglo = ObtenerFecha();
    //id calendario
    var idCalendario = "-1";
    var idClase = "-1";
    var objeto = $("#form_ReservarHora select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        idCalendario = objeto;
    }
    //
    //id clase
    var objetoClase = $("#form_ReservarHora select[name = Clase]").val();
    if (objetoClase.localeCompare("-1") != 0) {
        var nombreClase = $("#form_ReservarHora select[name = Clase]").text();
        idClase = objetoClase;
    }
    else {
        $("#form_ReservarHora select[name = Horario]").empty();
        $("#form_ReservarHora select[name = Horario]").append($('<option>', {
            value: "-1",
            text: "Todas Los Horarios"
        }));
    }
    BuscarReservas(fecha_arreglo, idCalendario, idClase);
}


function CargarSelectClase(select, array_entrada) {

    select.empty();
    select.append($('<option>', {
        value: "-1",
        text: "Todas Las Clases"
    }));

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
    //cargar horarios
    $("#form_ReservarHora select[name = Horario]").empty();
    $("#form_ReservarHora select[name = Horario]").append($('<option>', {
        value: "-1",
        text: "Todas Los Horarios"
    }));
    select[0].selectedIndex = 0;
}

function CargarSelectHorario(select, array_entrada) {
    select.empty();
    select.append($('<option>', {
        value: "-1",
        text: "Todos Los Horarios"
    }));

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


    select[0].selectedIndex = 0;
}


function ObtenerFecha() {
    var fecha = $('#inputFecha').val();
    var arreglo_fecha = fecha.split("/");
    return arreglo_fecha;
}



