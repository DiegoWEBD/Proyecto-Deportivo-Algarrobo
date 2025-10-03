$(document).ready(Inicio());


function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    MostrarContenido('loader', 'div_contenido');
    IniciarTablaImprimir('tabla_transferencias', true, true, true, false, true);
    CargarSelectMes();
    CargarYear();

}


function CargarSelectMes()
{
    var select = $("#form_tranferencias select[name = mes]");
    var array_entrada = [['Enero', 1], ['Febrero', 2], ['Marzo', 3], ['Abril', 4], ['Mayo', 5], ['Junio', 6], ['Julio', 7], ['Agosto', 8], ['Septiembre', 9], ['Octubre', 10], ['Noviembre', 11], ['Diciembre', 12] ];
    array_entrada.forEach(function (Value, Index) {
        var idClas = Value[1];
        var Clas = Value[0];
        select.append($('<option>', {
            value: idClas,
            text: Clas
        }));
    }
    );
    select[0].selectedIndex = -1;
}


function CargarYear() {
    var now = new Date();
    var year = now.getFullYear();
    var select = $("#form_tranferencias select[name = year]");
    var arregloyear = [];

    for (var i = 2022; i <= year; i++) {
        arregloyear.push([i, i.toString()]);
    }
    arregloyear.forEach(function (Value, Index) {
        var idClas = Value[1];
        var Clas = Value[0];
        select.append($('<option>', {
            value: idClas,
            text: Clas
        }));
    }
    );
    select[0].selectedIndex = -1;
}




function CargarTranferencias() {

    var mes = $("#form_tranferencias select[name = mes]").val();
    var year = $("#form_tranferencias select[name = year]").val();
    $.ajax({
        type: 'POST',
        url: 'ObtenerTranferencias',
        dataType: 'json',
        data: { year: year, mes: mes  },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var tabla = $('#tabla_transferencias').DataTable();
                MostrarContenido('loader', 'div_contenido');
                var dataSet = [];
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    //tabla.row.add([fila[0], fila[1], fila[2], fila[3], fila[4], ObtenerTipoTransaccion(fila[5]), fila[6], fila[7], fila[9]]).draw();
                    dataSet.push([fila[0], fila[1], fila[2], fila[3], fila[4], ObtenerTipoTransaccion(fila[5]), fila[6], fila[7], fila[9]]);
                }
                tabla.destroy();
                IniciarTablaBuscarUsuariosDatos('tabla_transferencias', true, true, false, false, true, true, dataSet)
                //tabla.responsive.rebuild();
                //tabla.responsive.recalc();
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




function ObtenerTipoTransaccion(tipo_transaccion) {
    var tipo_transaccion_aux;
    switch (tipo_transaccion) {
        case 'VD':
            tipo_transaccion_aux = 'Venta Débito';
            break;
        case 'VN':
            tipo_transaccion_aux = 'Venta Normal';
            break;
        case 'VC':
            tipo_transaccion_aux = 'Venta en cuotas';
            break;
        case 'SI':
            tipo_transaccion_aux = '3 cuotas sin interés.';
            break;
        case 'S2':
            tipo_transaccion_aux = '2 cuotas sin interés.';
            break;
        case 'NC':
            tipo_transaccion_aux = 'N Cuotas sin interés.';
            break;
        case 'VP':
            tipo_transaccion_aux = 'Venta Prepago.';
            break;
        case 'MANUAL':
            tipo_transaccion_aux = 'Ingreso Manual.';
            break;
        default:
            tipo_transaccion_aux = tipo_transaccion;
            break;
    }
    return tipo_transaccion_aux;
}




function ButonPersonalizado() {
    var table = $('#tabla_transferencias').DataTable();
    var wb = XLSX.utils.book_new();
    wb.Props = {
        Title: "Tranferencias",
        Subject: "Plataforma Reservas",
        Author: "Raul Carrizo C."
    };
    wb.SheetNames.push("Tranferencias");

    var ws_data = [['Fecha', 'Hora', 'Monto', 'Detalle Tarjeta', 'Código Autorización', 'Tipo Transacción', 'Nombre', 'Apellido', 'Rut']];
    table.rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();
        ws_data.push([data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8]]);
    });

    var ws = XLSX.utils.aoa_to_sheet(ws_data);
    wb.Sheets["Tranferencias"] = ws;
    var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
    function s2ab(s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

    saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), 'Tranferencias.xlsx');



}
