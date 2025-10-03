$(document).ready(Inicio());

function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    var year_actual = 2024;
    for (var i = 2022; i <= 2025; i++ ) {
        var x = document.getElementById("SelectYear");
        var option = document.createElement("option");
        option.text = i;
        x.add(option);
    }
    MostrarContenido('loader', 'div_contenido');
    IniciarTablaBuscarUsuarios('tabla_reporte', true, true, false, false, false, true);
    IniciarTablaBuscarUsuarios('tabla_reporte2', true, true, false, false, false, true);
}


function ChangeSelectYear() {
    $("#form_ReporteMensualidad select[name = SelectMes]").val("Todos");
    document.getElementById("div_tabla_reporte2").style.display = "inline";
    document.getElementById("div_tabla_reporte").style.display = "none";

    var tabla = $('#tabla_reporte2').DataTable();
    tabla.clear().draw();
    tabla = $('#tabla_reporte').DataTable();
    tabla.clear().draw();

}


function ChangeSelectMes() {
    var mes = $("#form_ReporteMensualidad select[name = SelectMes]").val();
    if (mes == "Todos") {
        document.getElementById("div_tabla_reporte2").style.display = "inline";
        document.getElementById("div_tabla_reporte").style.display = "none";
    } else {
        document.getElementById("div_tabla_reporte2").style.display = "none";
        document.getElementById("div_tabla_reporte").style.display = "inline";
    }

    var tabla = $('#tabla_reporte2').DataTable();
    tabla.clear().draw();
    tabla = $('#tabla_reporte').DataTable();
    tabla.clear().draw();
}


function ClickBuscarReportes() {
    var year = $("#form_ReporteMensualidad select[name = SelectYear]").val();
    var mes  = $("#form_ReporteMensualidad select[name = SelectMes]").val();

    if (mes == "Todos") {
        buscarReporteAnual(year);

    } else {
        buscarMesEspecifico(year, mes);
    }
}

function buscarReporteAnual(year) {
    $.ajax({
        type: 'POST',
        url: 'ReporteMensualidadYearEspecifico',
        dataType: 'json',
        data: { year: year },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            var tabla = $('#tabla_reporte2').DataTable();
            tabla.clear().draw();
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            var tabla = $('#tabla_reporte2').DataTable();
            if (correcto == true) {
                var mensaje = JSON.parse(response.Message);
                MostrarContenido('loader', 'div_contenido');
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var rut = fila[0];
                    var enero = fila[1];
                    var febrero = fila[2];
                    var marzo = fila[3];
                    var abril = fila[4];
                    var mayo = fila[5];
                    var junio = fila[6];
                    var julio = fila[7];
                    var agosto = fila[8];
                    var sept = fila[9];
                    var oct = fila[10];
                    var nov = fila[11];
                    var dic = fila[12];
                    tabla.row.add([rut, enero, febrero, marzo, abril, mayo, junio, julio, agosto, sept, oct, nov, dic]).draw();
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

function buscarMesEspecifico(year, mes) {
    $.ajax({
        type: 'POST',
        url: 'ReporteMensualidadMesEspecifico',
        dataType: 'json',
        data: { year: year, mes: mes },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
            //limpiar tabla
            var tabla = $('#tabla_reporte').DataTable();
            tabla.clear().draw();
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            var tabla = $('#tabla_reporte').DataTable();
            if (correcto == true) {
                MostrarContenido('loader', 'div_contenido');
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var nombre = fila[1];
                    var app = fila[2];
                    var apm = fila[3];
                    app = app + " " + apm;
                    var monto = fila[4];
                    tabla.row.add([nombre, app, monto]).draw();
                }
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            tabla.responsive.rebuild();
            tabla.responsive.recalc();
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

function ButonPersonalizado() {

    var table = $('#tabla_reporte').DataTable();
    var isYear = false;
    var mes = $("#form_ReporteMensualidad select[name = SelectMes]").val();
    if (mes == "Todos") {
        var table = $('#tabla_reporte2').DataTable();
        isYear = true
    } 

    var wb = XLSX.utils.book_new();
    wb.Props = {
        Title: "Usuarios",
        Subject: "Plataforma Reservas",
        Author: "Raul Carrizo C."
    };
    wb.SheetNames.push("usuarios");

    var ws_data = [['Nombre', 'Apellidos', 'Valor']];
    if (isYear == true) {
        ws_data = [['Nombre', 'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre']];
    }
    table.rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();
        if (isYear == true) {
            ws_data.push([data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12]]);
        }
        else {
            ws_data.push([data[0], data[1], data[2]]);
        }

    });

    var ws = XLSX.utils.aoa_to_sheet(ws_data);
    wb.Sheets["usuarios"] = ws;
    var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
    function s2ab(s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

    saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), 'Reporte.xlsx');



}




