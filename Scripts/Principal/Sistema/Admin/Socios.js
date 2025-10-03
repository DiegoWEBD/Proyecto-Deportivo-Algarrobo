var myModal;
var myModalELiminar;
var myModalModificar;
$(document).ready(Inicio());

function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    MostrarContenido('loader', 'div_contenido');
    IniciarTablaSociosDatos('tabla_usuarios', true, true, false, false, false, true);
    cargarSocios();
    cargarTipoSocio();
    myModal = new bootstrap.Modal(document.getElementById('agregarModal'));
    myModalELiminar = new bootstrap.Modal(document.getElementById('eliminarModal'));
    myModalModificar = new bootstrap.Modal(document.getElementById('modificarModal'));
    
}


function cargarSocios() {
    $.ajax({
        type: 'POST',
        url: 'ObtenerSocios',
        dataType: 'json',
        data: {},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var tabla = $('#tabla_usuarios').DataTable();
                tabla.clear().draw();
                MostrarContenido('loader', 'div_contenido');
                var dataSet = [];
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var botonModificar = '<button class="btn btn-primary"  data-bs-toggle="modal" data-bs-target="#modificarModal" onclick="FijarRut(\'' + fila[0] + '\',\'' + fila[6] +'\')" >Modificar</button>';
                    var botonEliminar = '<button class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#eliminarModal"  onclick="EliminarRut(\'' + fila[0] + '\')">Eliminar</button>';
                    dataSet.push([fila[0], fila[1], fila[2], fila[3], fila[7], botonModificar, botonEliminar]);
                }

                tabla.destroy();
                IniciarTablaSociosDatos('tabla_usuarios', true, true, false, false, true, true, dataSet);

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


function ButonPersonalizado() {
    myModal.show();
}

function crearSocio() {
    var rut = $("#inputRut").val();
    var idTipoSocio = $("#modalUsuario select[name = tipoSocio]").val();
    var socio = $("#modalUsuario select[name = tipoSocio]").find('option:selected').text();
    $.ajax({
        type: 'POST',
        url: 'agregarNuevoSocio',
        dataType: 'json',
        data: { rut: rut, idTipoSocio: idTipoSocio, socio: socio.toUpperCase() },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> ¡Datos Actualizados!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                resetForm();
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            myModal.hide();
            cargarSocios();
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

function FijarRut(rut, socio) {
    $("#inputRutModificar").val(rut);
    $("#modalModificarUsuario select[name = tipoSocioModificar]").val(socio);

}

function resetForm() {
    $("#inputRut").val('');
    $("#modalUsuario select[name = tipoSocio]").val(-1);
}

function modificarSocio() {
    var rut = $("#inputRutModificar").val();
    var socio = $("#inputSocioModificar").val();
    $.ajax({
        type: 'POST',
        url: 'agregarNuevoSocio',
        dataType: 'json',
        data: { rut: rut, socio: socio },
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
            myModalModificar.hide();
            cargarSocios();
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


function EliminarRut(rut) {
    $("#rutBorrar").val(rut);
}

        
function eliminarSocio() {
    var rut = $("#rutBorrar").val();
    $.ajax({
        type: 'POST',
        url: 'eliminarSocio',
        dataType: 'json',
        data: { rut: rut },
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
            myModalELiminar.hide();
            cargarSocios();
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


function cargarTipoSocio() {
    $.ajax({
        type: 'POST',
        url: 'CargarTipoSocio',
        dataType: 'json',
        data: {},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectTipoSocio($("#modalUsuario select[name = tipoSocio]"), mensaje);
                CargarSelectTipoSocio($("#modalModificarUsuario select[name = tipoSocioModificar]"), mensaje);
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

function CargarSelectTipoSocio(select, array_entrada) {
    select.empty();

    array_entrada.forEach(function (Value, Index) {
        var idTipo = Value[0];
        var Tipo = Value[1];
        select.append($('<option>', {
            value: idTipo,
            text: Tipo
        }));
    }
    );
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay selección disponibles' }));
    }

    select[0].selectedIndex = -1;
}



//function cargarEmpresas() {
//    $.ajax({
//        type: 'POST',
//        url: 'ObtenerEmpresas',
//        dataType: 'json',
//        data: {},
//        beforeSend: function () {
//            MostrarLoader('loader', 'div_contenido')
//        },
//        success: function (response) {
//            var mensaje = JSON.parse(response.Message);
//            var correcto = JSON.parse(response.Correcto);
//            if (correcto == true) {
//                //MostrarContenido('loader', 'div_contenido');
//                for (var i = 0; i < mensaje.length; i++) {
//                    var fila = mensaje[i];
                    
//                }
//            }
//            else {
//                $('#Error_ReservarHora').empty();
//                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
//            }

//        },
//        complete: function () {
//            MostrarContenido('loader', 'div_contenido');
//        },
//        error: function () {
//            $('#Error_ReservarHora').empty();
//            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
//            MostrarContenido('loader', 'div_contenido');
//        }
//    });
//}
