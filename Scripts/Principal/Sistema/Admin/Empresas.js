var myModal;
var myModalModificar;
var myModalELiminar;
$(document).ready(Inicio());

function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    MostrarContenido('loader', 'div_contenido');
    IniciarTablaSociosDatos('tabla_usuarios', true, true, false, false, false, true);
    cargarEmpresas()
    myModal = new bootstrap.Modal(document.getElementById('agregarModal'));
    myModalELiminar = new bootstrap.Modal(document.getElementById('eliminarModal'));
    myModalModificar = new bootstrap.Modal(document.getElementById('modificarModal'));
    //IniciarTablaBuscarUsuarios('tabla_usuarios', true, true, false, false, true, true);
    //IniciarTablaBuscar('tabla_mensualidad', true, true, false, false, false, false);
}

function cargarEmpresas() {
    $.ajax({
        type: 'POST',
        url: 'ObtenerEmpresas',
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
                    var botonModificar = '<button class="btn btn-primary"  data-bs-toggle="modal" data-bs-target="#modificarModal" onclick="FijarEmpresa(\'' + fila[0] + '\',\'' + fila[1] +'\')" >Modificar</button>';
                    var botonEliminar = '<button class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#eliminarModal"  onclick="EliminarIdEmpresa(\'' + fila[0] + '\',\'' + fila[1] + '\')">Eliminar</button>';
                    dataSet.push([fila[1], fila[0], botonModificar, botonEliminar]);
                    //tabla.row.add([fila[0], fila[0], botonModificar, botonEliminar]).draw();
                }

                //tabla.responsive.rebuild();
                //tabla.responsive.recalc();
                tabla.destroy();
                IniciarTablaSociosDatos('tabla_usuarios', true, true, false, false, true, true, dataSet);
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

function modificarEmpresa() {
    var nombreEmpresa = $("#inputEmpresaModificar").val();
    var idEmpresa = $("#inputIdModificar").val();
    $.ajax({
        type: 'POST',
        url: 'ModificarEmpresa',
        dataType: 'json',
        data: { nombreEmpresa: nombreEmpresa, idEmpresa: idEmpresa },
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
            cargarEmpresas();
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

function crearEmpresa() {
    var nombreEmpresa = $("#inputEmpresa").val();
    $.ajax({
        type: 'POST',
        url: 'AgregarNuevaEmpresa',
        dataType: 'json',
        data: { nombreEmpresa: nombreEmpresa },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> ¡Datos Ingresados!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            myModal.hide();
            ResetForm();
            cargarEmpresas();
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

function FijarEmpresa(empresa, id) {
    $("#inputEmpresaModificar").val(empresa);
    $("#inputIdModificar").val(id);
}

function EliminarIdEmpresa(nombre, id) {
    $("#idEmpresaEliminar").val(id);
    $("#nomEmpElim").val(nombre)
}
function EliminarEmpresa() {
    const idEmpresa = $("#idEmpresaEliminar").val();
    $.ajax({
        type: 'POST',
        url: 'EliminarEmpresa',
        dataType: 'json',
        data: { idEmpresa: idEmpresa },
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
            cargarEmpresas();
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

function ResetForm() {
    $("#inputEmpresa").val("");
}
