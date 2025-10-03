$(document).ready(Inicio());



function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    CargarClase();
   
}



function CargarClase() {
    $.ajax({
        type: 'POST',
        url: 'ObtenerClases',
        dataType: 'json',
        data: { },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectClase($("#form_AdminClases select[name = Clase]"), mensaje);
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


function CargarHorarios(IdDia, Clase) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerTodosHorarios',
        dataType: 'json',
        data: { IdDia: IdDia, IdClase: Clase },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            LimpiarHorarios();
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            var horarios = mensaje[0];
            var horariosSeleccionados = mensaje[1];
            if (correcto == true) {
                var max_elementos_fila = 4;
                AgregarHorarios(max_elementos_fila, horarios);
                SeleccionarHorarios(horariosSeleccionados);
                $("#form_AdminClases input[name = Participantes]").val(mensaje[2][0][1]);
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


function CargarMaximoCupos(Clase) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerCuposMaximos',
        dataType: 'json',
        data: { IdClase: Clase },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            LimpiarHorarios();
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {

                $("#form_AdminClases input[name = Participantes]").val(mensaje[0][1]);
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


function GuardarCambios(idClase, CuposMaximos, idDia, Horarios) {
    $.ajax({
        type: 'POST',
        url: 'GuardarCambiosClases',
        dataType: 'json',
        data: { idClase: idClase, maxparticipante: CuposMaximos, idDia: idDia, Horarios: JSON.stringify(Horarios)  },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
     
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Cambios Realizados !</strong> Operación realizada con éxito <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            else {
                var mensaje = JSON.parse(response.Message);
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


    select[0].selectedIndex = -1;
    $("#form_AdminClases select[name = Dia]")[0].selectedIndex = -1;
}



function ChangeClase() {
    $("#form_AdminClases select[name = Dia]")[0].selectedIndex = -1;
    LimpiarHorarios();
    $('#Error_ReservarHora').empty();
    $("#form_AdminClases button[name = button]")[0].disabled = true;
    var objetoClase = $("#form_AdminClases select[name = Clase]").val();
    $("#form_AdminClases input[name = Participantes]").val();
    CargarMaximoCupos(objetoClase);
}


function ChangeMaxParticipantes() {
    var objetoClase = $("#form_AdminClases select[name = Clase]").val();
    if (objetoClase > 0) {
        $("#form_AdminClases button[name = button]")[0].disabled = false;
    }
}


function ChangeDia() {
    var objetoClase = $("#form_AdminClases select[name = Clase]").val();
    var objeto = $("#form_AdminClases select[name = Dia]").val();
    CargarHorarios(objeto, objetoClase);
}



function ChangeCheckBox() {
    var objetoClase = $("#form_AdminClases select[name = Clase]").val();
    var objeto = $("#form_AdminClases select[name = Dia]").val();
    if ((objetoClase > 0) && (objeto > 0)) {
        $("#form_AdminClases button[name = button]")[0].disabled = false;
    }
}


function onclickButton() {
    var idClase           = $("#form_AdminClases select[name = Clase]").val();
    var maxparticipantes = $("#form_AdminClases input[name = Participantes]").val();
    var idDia = $("#form_AdminClases select[name = Dia]").val();
    var ArregloHoras = [];
    if (idDia != null) {
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
    else {
        idDia = - 1;
    }
    GuardarCambios(idClase, maxparticipantes, idDia, ArregloHoras);
}



function AgregarHorarios( max_columnas, entrada) {
    var contador = 0;
    var row = ""
    for (var i = 0; i < entrada.length; i++) {
        var fila = entrada[i];
        var id = fila[0];
        var horario = fila[1];
        if (contador == 0) { row = row+'<div class="row">';}
        var elemento = '<div class="col-sm"><div class="mt-3 mb-0">' +
            '<input name="checkbox_' + id + '" id="checkHorario_' + id + '"  value="' + id+'" class="form-check-input me-2" type="checkbox" onchange="ChangeCheckBox()">' +
            '<label class="form-check-label" for="checkHorario_' + id +'"> ' + horario + ' </label></div></div>';
        row = row + elemento;
        contador++;
        if (contador == max_columnas) { row = row + "</div>"; contador = 0; }
    }

    if (contador < max_columnas)
    {
        for (var i = contador; i < max_columnas; i++) {
            row = row + '<div class="col-sm"></div>';
        }
        row = row + "</div>";
    }
    $("#ContenedorHorarios").append(row);

}



function SeleccionarHorarios(entrada) {
    for (var i = 0; i < entrada.length; i++) {
        var fila = entrada[i];
        var id_horario = fila[2];
        $('#form_AdminClases input[name = "checkbox_'+id_horario+'"]')[0].checked = true;
    }
}


function LimpiarHorarios() {

    $("#ContenedorHorarios").empty();
}


function ResetAgregarClase() {
    $("#inputNombreClase").val("");
    $("#inputNombreClase2").val("");
}


function agregarNuevaClase() {
    var nombreClase  = $("#inputNombreClase").val();
    var nombreClase2 = $("#inputNombreClase2").val();
    if (nombreClase == nombreClase2) {
        $.ajax({
            type: 'POST',
            url: 'AgregarNuevaClase',
            dataType: 'json',
            data: { nombreClase: nombreClase},
            beforeSend: function () {
                MostrarLoader('loader', 'div_contenido')
            },
            success: function (response) {
                var correcto = JSON.parse(response.Correcto);
                var existe = JSON.parse(response.Existe);
                if (correcto == true) {
                    if (existe == false) {
                        $('#Error_ReservarHora').empty();
                        $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong>Nueva Clase Creada</strong><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')

                        var myModalEl = document.getElementById('agregarClaseModal');
                        var modal = bootstrap.Modal.getInstance(myModalEl);
                        modal.hide()
                    } else {
                        $('#Error_NuevaClase').empty();
                        $("#Error_NuevaClase").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:! </strong>¡Clase ya Existe! <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                    }
                    CargarClase();
                    
                }
                else {
                    var mensaje = JSON.parse(response.Message);
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


function confirmarEliminarClase() {
    const claseId = $("#form_AdminClases select[name = Clase]").val()
    const claseText = $("#form_AdminClases select[name = Clase] option:selected").text()
    if (claseId) {
        const confirmar = confirm(`Eliminar clase: ${claseText}`)
        if (confirmar) {
            eliminarClase(claseId)
        }
    } else {
        $('#Error_ReservarHora').empty();
        $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> Debe seleccionar la clase.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
    }
    
}

function eliminarClase(claseId) {
    console.log(claseId)
    if (claseId) {
        $.ajax({
            type: 'POST',
            url: 'EliminarClase',
            dataType: 'json',
            data: { idClase: claseId },
            beforeSend: function () {
            //  MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            console.log(mensaje, correcto)
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Clase Eliminada Correctamente</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                CargarClase()
                $("#form_AdminClases input[name = Participantes]").val('')
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            // MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
        });
    }
}