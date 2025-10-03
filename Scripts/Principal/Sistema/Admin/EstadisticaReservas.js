
var myLineChart;

$(document).ready(Inicio());



function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    
    CargarClase();


    var dia = getDiasReserva();
    $('#inputFecha').daterangepicker({
        singleDatePicker: false,
        timePicker: false,
        singleClasses: "picker_4",
        timePicker24Hour: false,
        timePickerSeconds: false,
        autoApply: true,
        startDate: moment().endOf("day").add(-7, 'days'),
        endDate: moment().endOf("day"),
        minYear: 2022,
        maxSpan: {
            days: 7
        },
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


    CargarMeses();
    CargarYear();
    


    $("#selectorMes").change(function () {
        ObtenerRegistrados();
        ObtenerMensualidades();
    });
    $("#selectorYear").change(function () {
        ObtenerRegistrados();
        ObtenerMensualidades();
    });
    $("#selectorClase").change(function () {
        ChangeFecha();
    });

    ObtenerRegistrados();
    ObtenerMensualidades();

    MostrarContenido('loader', 'div_contenido');
}


function CargarMeses() {
    var select = $("#selectorMes")
    select.empty();
    select.append($('<option>', {
        value: 1,
        text: 'Enero'
    }));
    select.append($('<option>', {
        value: 2,
        text: 'Febrero'
    }));
    select.append($('<option>', {
        value: 3,
        text: 'Marzo'
    }));
    select.append($('<option>', {
        value: 4,
        text: 'Abril'
    }));
    select.append($('<option>', {
        value: 5,
        text: 'Mayo'
    }));
    select.append($('<option>', {
        value: 6,
        text: 'Junio'
    }));
    select.append($('<option>', {
        value: 7,
        text: 'Julio'
    }));
    select.append($('<option>', {
        value: 8,
        text: 'Agosto'
    }));
    select.append($('<option>', {
        value: 9,
        text: 'Septiembre'
    }));
    select.append($('<option>', {
        value: 10,
        text: 'Octubre'
    }));
    select.append($('<option>', {
        value: 11,
        text: 'Noviembre'
    }));
    select.append($('<option>', {
        value: 12,
        text: 'Diciembre'
    }));
    select.append($('<option>', {
        value: 13,
        text: 'Todos'
    }));
    $("#selectorMes option[value='13']").index();
}


function CargarYear() {
    var fecha = new Date();
    var year = fecha.getFullYear();
    var select = $("#selectorYear")

    for (var i = year; i >= 2022; i-- ) {
        select.append($('<option>', {
            value: ""+i+"",
            text: i
        }));
    }

}


/**
 * Carga el select con las clase disponibles para la fecha indicada
 * @param {any} arreglo_fecha
 */
function CargarClase() {
    $.ajax({
        type: 'POST',
        url: 'ObtenerClases',
        dataType: 'json',
        async: false,
        data: {},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectClase($("#selectorClase"), mensaje);
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

function CargarSelectClase(select, array_entrada) {

    select.empty();
    select.append($('<option>', {
        value: 'Todos',
        text: 'Todas Las Clases'
    }));

    array_entrada.forEach(function (Value, Index) {
        var idClas = Value[0];
        var Clas = Value[1];
        select.append($('<option>', {
            value: idClas,
            text: Clas
        }));
    }
    );
    
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay clases disponibles' }));
    }
}


function getDiasReserva() {
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


function ChangeFecha() {
    var fecha = $('#inputFecha').val();
    var fecha_array = fecha.split(" - ");
    var fecha_inicio = fecha_array[0];
    var fecha_final = fecha_array[1];
    var clase = $("#selectorClase").val();
    var arrayFechaInicio  = ObtenerFecha(fecha_inicio);
    var arrayFechaTermino = ObtenerFecha(fecha_final);

    if ((fecha_inicio == fecha_final)) {
        ObtenerReservasFecha(arrayFechaInicio[0], arrayFechaInicio[1], arrayFechaInicio[2], clase)
    }
    else {
        ObtenerReservasFechas(arrayFechaInicio[0], arrayFechaInicio[1], arrayFechaInicio[2], arrayFechaTermino[0], arrayFechaTermino[1], arrayFechaTermino[2], clase);
    }

    

}

function ObtenerFecha(fechaEntrada) {
    var arreglo_fecha = fechaEntrada.split("/");
    return arreglo_fecha;
}


function ObtenerRegistrados() {
    $.ajax({
        type: 'POST',
        url: 'ObtenerUsuariosRegistrados',
        dataType: 'json',
        data: {},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var usuariosRegistrados = mensaje[0];
                var usuariosSociosRegistrados = mensaje[1];
                var usuariosNoSociosRegistrados = mensaje[2];
                $('#numero_usuariosRegistrados').empty();
                $("#numero_usuariosRegistrados").append(usuariosRegistrados + ' Usuario Registrados');

                $('#numero_usuariosRegistradosSocios').empty();
                $("#numero_usuariosRegistradosSocios").append(usuariosSociosRegistrados + ' Socios Registrados');

                $('#numero_usuariosRegistradosNoSocios').empty();
                $("#numero_usuariosRegistradosNoSocios").append(usuariosNoSociosRegistrados + ' Usuario Registrados');
            }
            else {
                $('#Error_Estadistica').empty();
                $("#Error_Estadistica").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_Estadistica').empty();
            $("#Error_Estadistica").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function ObtenerMensualidades() {
    var mes = $("#selectorMes").val();
    var year = $("#selectorYear").val();

    $.ajax({
        type: 'POST',
        url: 'ObtenerEstadisticasMensualidades',
        dataType: 'json',
        data: { mes : mes, year: year},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {

                $('#TotalMensualidad').empty();
                $("#TotalMensualidad").append(mensaje[0] + ' Mensualidades Pagadas');

                $('#TotalMensualidadCLP').empty();
                $("#TotalMensualidadCLP").append("Total de : $" + mensaje[1]);

                $('#TotalClasesDirigidas').empty();
                $("#TotalClasesDirigidas").append(mensaje[2] + ' Clases Dirigidas Pagadas.');

                $('#TotalClasesDirigidasCLP').empty();
                $("#TotalClasesDirigidasCLP").append("Total de : $" + mensaje[3] + '');
            }
            else {
                $('#Error_Estadistica').empty();
                $("#Error_Estadistica").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_Estadistica').empty();
            $("#Error_Estadistica").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}



function ObtenerReservasFechas(diaInicio, mesInicio, yearInicio, diaTermino, mesTermino, yearTermino,  clase) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerReservasFechas',
        dataType: 'json',
        data: { diaInicio: diaInicio, mesInicio: mesInicio, yearInicio: yearInicio, diaTermino: diaTermino, mesTermino: mesTermino, yearTermino: yearTermino , clase: clase },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {

                var label = [];
                var valores = [];

                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var fecha_aux = fila[0];
                    var cantidad_aux = fila[1];
                    label.push(fecha_aux);
                    valores.push(cantidad_aux);
                }
                GraficarBarrasDias(label, valores);
            }
            else {
                $('#Error_Estadistica').empty();
                $("#Error_Estadistica").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_Estadistica').empty();
            $("#Error_Estadistica").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function ObtenerReservasFecha(diaInicio, mesInicio, yearInicio, clase) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerReservasFecha',
        dataType: 'json',
        data: { diaInicio: diaInicio, mesInicio: mesInicio, yearInicio: yearInicio, clase: clase },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var label = [];
                var valores = [];

                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var fecha_aux = fila[0];
                    var cantidad_aux = fila[1];
                    label.push(fecha_aux);
                    valores.push(cantidad_aux);
                }
                GraficarBarrasDias(label, valores);
            }
            else {
                $('#Error_Estadistica').empty();
                $("#Error_Estadistica").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_Estadistica').empty();
            $("#Error_Estadistica").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}



function GraficarBarrasDias(label, valores) {

    if (myLineChart != null) {
        myLineChart.destroy();

    }
   
    var ctx = document.getElementById("myBarChart");
    myLineChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: label,
            datasets: [{
                label: "Revenue",
                backgroundColor: "rgba(2,117,216,1)",
                borderColor: "rgba(2,117,216,1)",
                data: valores,
            }],
        },
        options: {
            scales: {
                xAxes: [{
                    gridLines: {
                        display: false
                    },
                    ticks: {
                        maxTicksLimit: 15
                    }
                }],
                yAxes: [{
                    ticks: {
                        min: 0,
                        maxTicksLimit: 5
                    },
                    gridLines: {
                        display: true
                    }
                }],
            },
            legend: {
                display: false
            }
        }
    });
    

    
}
