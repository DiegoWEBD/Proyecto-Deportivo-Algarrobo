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
        data: {  },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectClase($("#form_AsignarProfesores select[name = Clase]"), mensaje);
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
    //limpiar otros selectores
    $("#form_AsignarProfesores select[name = Dia]").empty();
    $("#form_AsignarProfesores select[name = Horario]").empty();
    $("#form_AsignarProfesores select[name = Profesor]").empty();

    select[0].selectedIndex = -1;
}




function ChangeClase() {
    var objeto = $("#form_AsignarProfesores select[name = Clase]").val();
    if (objeto.localeCompare("-1") != 0) {
        var nombreClase = $("#form_AsignarProfesores select[name = Clase]").text();
        var idClase = objeto;
        CargarDiaSemana($("#form_AsignarProfesores select[name = Dia]"));
    }
    else {
        $("#form_AsignarProfesores select[name = Horario]").empty();
        $("#form_AsignarProfesores select[name = Horario]").append($('<option>', {
            value: "-1",
            text: "Todas Los Horarios"
        }));
    }
}


function CargarDiaSemana(select) {
    select.empty();

    select.append($('<option>', {
        value: '1',
        text: 'Lunes'
    }));
    select.append($('<option>', {
        value: '2',
        text: 'Martes'
    }));
    select.append($('<option>', {
        value: '3',
        text: 'Miercoles'
    }));
    select.append($('<option>', {
        value: '4',
        text: 'Jueves'
    }));
    select.append($('<option>', {
        value: '5',
        text: 'Viernes'
    }));
    select.append($('<option>', {
        value: '6',
        text: 'Sabado'
    }));
    
    //limpiar otros selectores
    $("#form_AsignarProfesores select[name = Horario]").empty();
    $("#form_AsignarProfesores select[name = Profesor]").empty();

    select[0].selectedIndex = -1;
}



function ChangeDia() {
    var objeto = $("#form_AsignarProfesores select[name = Dia]").val();
    var objetoClase = $("#form_AsignarProfesores select[name = Clase]").val();
    if (objeto.localeCompare("-1") != 0) {
        CargarHorarios(objetoClase, objeto);
    }
    else {
        $("#form_AsignarProfesores select[name = Horario]").empty();
       
    }
}


function CargarHorarios(clase, diaSemana) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerHorariosClase',
        dataType: 'json',
        data: { IdDia: diaSemana, IdClase: clase },
        beforeSend: function () {
            $("#form_AsignarProfesores select[name = Horario]").empty();
            $("#form_AsignarProfesores select[name = Profesor]").empty();
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var mensaje = JSON.parse(response.Message);

                if (mensaje.length > 0) {
                    var horario = mensaje[0];
                    
                    for (var i = 0; i < horario.length; i++) {
                        var horario_text = horario[i][4];
                        var idCalendario = horario[i][0];
                        //agregra a selector
                        $("#form_AsignarProfesores select[name = Horario]").append($('<option>', {
                            value: idCalendario,
                            text: horario_text
                        }));
                    }
                    $("#form_AsignarProfesores select[name = Horario]")[0].selectedIndex = -1;
                }
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


function ChangeHorario() {
    var objeto = $("#form_AsignarProfesores select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        CargarProfesor(objeto);
    }
    else {
        $("#form_AsignarProfesores select[name = Profesor]").empty();
    }  
}


function CargarProfesor(idCalendario) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerProfesoresClase',
        dataType: 'json',
        data: { IdCalendario: idCalendario },
        beforeSend: function () {
            $("#form_AsignarProfesores select[name = Profesor]").empty();
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var mensaje = JSON.parse(response.Message);
                var profesores = mensaje[0];
                var profesor_seleccionado = mensaje[1]; 
                if (profesores.length > 0) {
                    for (var i = 0; i < profesores.length; i++) {
                        var profesor_text = profesores[i][0];
                        var idProfesor = profesores[i][1];
                        //agregra a selector
                        $("#form_AsignarProfesores select[name = Profesor]").append($('<option>', {
                            value: idProfesor,
                            text: profesor_text
                        }));
                    }
                }

                

                if (profesor_seleccionado.length > 0) {
                    var profesor_text = profesor_seleccionado[0][0];
                    var idProfesor = profesor_seleccionado[0][1]
                    //seleccionar el profesor indicado
                    $("#form_AsignarProfesores select[name = Profesor]").val(idProfesor);
                }
                else {
                    $("#form_AsignarProfesores select[name = Profesor]")[0].selectedIndex = -1;
                }
                
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

function onclickButton() {

    var IdCalendario = $("#form_AsignarProfesores select[name = Horario]").val();
    var userName     = $("#form_AsignarProfesores select[name = Profesor]").val();

    $.ajax({
        type: 'POST',
        url: 'AsignarProfesor',
        dataType: 'json',
        data: { IdCalendario: IdCalendario, userName: userName },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Asignación Correcta</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
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






