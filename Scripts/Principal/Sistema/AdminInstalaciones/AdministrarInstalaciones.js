$(document).ready(Inicio());



function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    $("#form_AdminClases select[name = Instalacion]")[0].selectedIndex = -1;
    $("#form_AdminClases select[name = TipoArriendo]")[0].selectedIndex = -1;
    $("#form_AdminClases select[name = Dia]")[0].selectedIndex = -1;
    //CargarInstalacion();
    CargarTipoInstalacion();
    MostrarContenido('loader', 'div_contenido');
}



function resetForm() {
    LimpiarHorarios()
    $("#form_AdminClases select[name = Instalacion]")[0].selectedIndex = -1
    $("#form_AdminClases select[name = TipoArriendo]")[0].selectedIndex = -1
    $("#form_AdminClases input[name = ValorArriendo]").val("")
    $("#CheckRegistroParticipantes")[0].checked = false;
    $("#form_AdminClases select[name = Dia]")[0].selectedIndex = -1;
}


function ChangeTipoInstalacion() {
    var tipoInstalacion = $("#form_AdminClases select[name = TipoInstalacion]").val();
    CargarInstalacion(tipoInstalacion);
    LimpiarHorarios();
    $("#form_AdminClases select[name = TipoArriendo]")[0].selectedIndex = -1;
    $("#form_AdminClases input[name = ValorArriendo]").val("");
    $("#CheckRegistroParticipantes")[0].checked = false;
    $("#form_AdminClases select[name = Dia]")[0].selectedIndex = -1;
}

function ChangeInstalacion() {
    //seteo de valores por default al cambiar una instalacion
    LimpiarHorarios();
    $("#form_AdminClases select[name = TipoArriendo]")[0].selectedIndex = -1;
    $("#form_AdminClases input[name = ValorArriendo]").val("");
    $("#CheckRegistroParticipantes")[0].checked = false;
    $("#form_AdminClases select[name = Dia]")[0].selectedIndex = -1;


    //LimpiarHorarios(); //limpiar  horarios seleccionados
    //$("#form_AdminClases button[name = button]")[0].disabled = true;
    //var objetoClase = $("#form_AdminClases select[name = Instalacion]").val();
    //CargarLogInstalacion(objetoClase) 

    //CargarMaximoCupos(objetoClase); //fijar valor de arriendo

    ///si selecciono un valor (no nulo)
    if ($("#form_AdminClases select[name = Instalacion]")[0].selectedIndex >= 0) {
        var IDinstalacion = $("#form_AdminClases select[name = Instalacion]").val();
        CargarInformacionInstalacion(IDinstalacion);
    }


}


function ChangeTipoArriendo()
{
    $("#form_AdminClases select[name = Dia]")[0].selectedIndex = -1;
    LimpiarHorarios();
    //cargar horarios segun tipo de arriendo
}


function ResetAgregarInstalacion() {
    $("#inputNombreClase").val("");
    $("#inputNombreClase2").val("");
}


function ChangeDia() {
    LimpiarHorarios();
    var instalacion = $("#form_AdminClases select[name = Instalacion]").val();
    var dia = $("#form_AdminClases select[name = Dia]").val();
    var tipo = $("#form_AdminClases select[name = TipoArriendo]").val();

    if ($("#form_AdminClases select[name = Instalacion]")[0].selectedIndex >= 0) {
        if ($("#form_AdminClases select[name = Dia]")[0].selectedIndex >= 0) {
            CargarHorarios(instalacion, dia, tipo);
        }
    }
}


function LimpiarHorarios() {
    $("#ContenedorHorarios").empty();
}


function AgregarHorarios(max_columnas, entrada) {
    var contador = 0;
    var row = ""
    for (var i = 0; i < entrada.length; i++) {
        var fila = entrada[i];
        var id = fila[0];
        var horario = fila[1];
        if (contador == 0) { row = row + '<div class="row">'; }
        var elemento = '<div class="col-sm"><div class="mt-3 mb-0">' +
            '<input name="checkbox_' + id + '" id="checkHorario_' + id + '"  value="' + id + '" class="form-check-input me-2" type="checkbox" onchange="ChangeCheckBox()">' +
            '<label class="form-check-label" for="checkHorario_' + id + '"> ' + horario + ' </label></div></div>';
        row = row + elemento;
        contador++;
        if (contador == max_columnas) { row = row + "</div>"; contador = 0; }
    }

    if (contador < max_columnas) {
        for (var i = contador; i < max_columnas; i++) {
            row = row + '<div class="col-sm"></div>';
        }
        row = row + "</div>";
    }
    $("#ContenedorHorarios").append(row);
}


function CargarSelectInstalacion(select, array_entrada) {

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
        select.append($('<option>', { value: -1, text: 'No hay instalaciones disponibles' }));
    }


    select[0].selectedIndex = -1;

    $("#form_AdminClases select[name = TipoArriendo]")[0].selectedIndex = -1;
    $("#form_AdminClases input[name = ValorArriendo]").val("");
    $("#CheckRegistroParticipantes")[0].checked = false;
    $("#form_AdminClases select[name = Dia]")[0].selectedIndex = -1;
}


function obtenerHorariosSeleccionados() {
    var ArregloHoras = [];
    if ($("#form_AdminClases select[name = Instalacion]")[0].selectedIndex >= 0) {
        if ($("#form_AdminClases select[name = Dia]")[0].selectedIndex >= 0) {
            var checks = $("#ContenedorHorarios input");
            for (var i = 0; i < checks.length; i++) {
                var check_aux = checks[i];
                var esSeleccionado = check_aux.checked;
                var id_aux = check_aux.value;
                if (esSeleccionado) {
                    ArregloHoras.push(id_aux);
                }
            }
        }
    }
    return ArregloHoras;

}


function SeleccionarHorarios(entrada) {
    for (var i = 0; i < entrada.length; i++) {
        var fila = entrada[i];
        var id_horario = fila[2];

        $('#form_AdminClases input[name = "checkbox_' + id_horario + '"]')[0].checked = true;
    }
}

function CargarHorarios(IdClase, IdDia, IdTipo) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerTodosHorarios',
        dataType: 'json',
        data: { IdDia: IdDia, IdInstalacion: IdClase, IdTipo: IdTipo },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            LimpiarHorarios();
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var horarios = mensaje[0];
                var horariosSeleccionados = mensaje[1];
                var max_elementos_fila = 4;

                AgregarHorarios(max_elementos_fila, horarios);
                SeleccionarHorarios(horariosSeleccionados);
               // $("#form_AdminClases input[name = Participantes]").val(mensaje[2][0][1]); //rellena la cantidad de participantes
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


function agregarNuevaClase() {
    var nombreInstalacion = $("#inputNombreInstalacion").val();
    var nombreInstalacion2 = $("#inputNombreInstalacion2").val();
    if (nombreInstalacion == nombreInstalacion2) {
        $.ajax({
            type: 'POST',
            url: 'AgregarNuevaInstalacion',
            dataType: 'json',
            data: { nombreInstalacion: nombreInstalacion },
            beforeSend: function () {
                MostrarLoader('loader', 'div_contenido')
            },
            success: function (response) {
                var correcto = JSON.parse(response.Correcto);
                var existe = JSON.parse(response.Existe);
                if (correcto == true) {
                    if (existe == false) {
                        $('#Error_ReservarHora').empty();
                        $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong>Nueva Instalación Creada</strong><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')

                        var myModalEl = document.getElementById('agregarClaseModal');
                        var modal = bootstrap.Modal.getInstance(myModalEl);
                        modal.hide()
                    } else {
                        $('#Error_NuevaClase').empty();
                        $("#Error_NuevaClase").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:! </strong>¡Clase ya Existe! <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                    }
                    //CargarInstalacion();
                    ResetAgregarInstalacion()
                    resetForm()
                    CargarTipoInstalacion()
                }
                else {
                    var mensaje = response.Message;
                    $('#Error_NuevaClase').empty();
                    $("#Error_NuevaClase").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:! </strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                }

            },
            complete: function () {
                MostrarContenido('loader', 'div_contenido');
            },
            error: function () {
                $('#Error_NuevaClase').empty();
                $("#Error_NuevaClase").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:! </strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
                MostrarContenido('loader', 'div_contenido');
            }
        });
    }
    else {
        $('#Error_NuevaClase').empty();
        $("#Error_NuevaClase").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>Nombres De Clases No Coinciden <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');

    }
}


function CargarTipoInstalacion() {
    $.ajax({
        type: 'POST',
        url: 'CargarTipoInstalacion_Todas',
        dataType: 'json',
        data: {},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            var mensaje = JSON.parse(response.Message);
            if (correcto == true) {
                CargarSelectTipoInstalacion($("#form_AdminClases select[name = TipoInstalacion]"), mensaje);
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


function CargarInstalacion(tipoInstalacion) {
    $.ajax({
        type: 'POST',
        url: 'CargarInstalacionTipoTodas',
        dataType: 'json',
        data: { TipoInstalacion: tipoInstalacion },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var mensaje = JSON.parse(response.Message);
                CargarSelectInstalacion($("#form_AdminClases select[name = Instalacion]"), mensaje);
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
            $("#form_AdminClases button[name = button]")[0].disabled = false;
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function CargarInformacionInstalacion(IdInstalacion) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerinformacionInstalacion',
        dataType: 'json',
        data: { IdInstalacion: IdInstalacion },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var mensaje = JSON.parse(response.Message);
                var info = mensaje[0];
                var precio = mensaje[1];
                if (precio.length > 0) {
                    $("#form_AdminClases input[name = ValorArriendo]").val(precio[0][2]);
                }
                if (info.length > 0) {
                    var tipoArriendo = info[0][1];
                    var check = info[0][2];
                    if (check == 'True') {
                        $("#CheckRegistroParticipantes")[0].checked = true;
                    }
                    $("#form_AdminClases select[name = TipoArriendo]").val(tipoArriendo);
                }
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
            $("#form_AdminClases button[name = button]")[0].disabled = false;
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
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



function onclickButtonGuardar() {

    var IDinstalacion = $("#form_AdminClases select[name = Instalacion]").val();
    var verificarParticipantes = $("#CheckRegistroParticipantes")[0].checked;
    var tipoArriendo = $("#form_AdminClases select[name = TipoArriendo]").val();
    var valorArriendo = $("#form_AdminClases input[name = ValorArriendo]").val();
    var diaSemana = $("#form_AdminClases select[name = Dia]").val();
    var arregloHorarios = obtenerHorariosSeleccionados();
    

    //falta completar el valor de los horarios
    if (diaSemana == null) {
        diaSemana = 0;
    }

    $.ajax({
        type: 'POST',
        url: 'GuardarInstalacion',
        dataType: 'json',
        data: { idInstalacion: IDinstalacion, tipoArriendo: tipoArriendo, Monto: valorArriendo, registroParticipantes: verificarParticipantes, diaSemana: diaSemana, Horarios: JSON.stringify(arregloHorarios)},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> ¡Datos Actualizados!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
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
    /////
}



