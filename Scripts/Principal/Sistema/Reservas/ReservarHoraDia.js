var myModal;
var myModalPagar;
$(document).ready(Inicio());


function Inicio() {
    MostrarLoader('loader', 'div_contenido')
    var dia = getDiasReserva(year_global, mes_global, dia_global);

    var moment_actual = moment([year_global, mes_global, dia_global])
    var moment_actual2 = moment([year_global, mes_global, dia_global])
    $('#inputFecha').daterangepicker({
        singleDatePicker: true,
        timePicker: false,
        singleClasses: "picker_4",
        timePicker24Hour: false,
        timePickerSeconds: false,
        autoApply: true,
        minDate: moment_actual,
        startDate: moment_actual,
        maxDate: moment_actual2.endOf("day").add(dia, 'days'),
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
    myModal = new bootstrap.Modal(document.getElementById('datosInvitadoModal'));
    myModalPagar = new bootstrap.Modal(document.getElementById('datosPagarModal'));
}


function CargarClase(arreglo_fecha) {
    var mostrarTodo = getDiasReservaClaseDirigida(arreglo_fecha[2], arreglo_fecha[1], arreglo_fecha[0]);
    $.ajax({
        type: 'POST',
        url: 'CargarClase',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2]},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectClase($("#form_ReservarHora select[name = Clase]"), mensaje, mostrarTodo);
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



function ComprobarCupos(arreglo_fecha, id_calendario) {
    $.ajax({
        type: 'POST',
        url: 'TieneCuposDisponiblesSinMensualidad',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idCalendario: id_calendario },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var feriado = JSON.parse(response.Message);
                if (feriado == 'FERIADO') {
                    EstadoPreReservao("¡Reservas de Instalación Cancelada Por Día Festivo!");
                }
                else {
                    var existeCupo = mensaje[0];
                    var mensualidadPagada = mensaje[3];
                    var yaTieneReserva = mensaje[4];
                    var cupos_actuales = mensaje[1];
                    var cupos_maximos = mensaje[2];
                    var estadoCancelacion = JSON.parse(response.Cancelacion);
                    if (estadoCancelacion == false) {
                        if (mensualidadPagada) {
                            if (yaTieneReserva == false) {
                                if (existeCupo == true) {
                                    var cuposDisponibles = cupos_maximos - cupos_actuales;
                                    EstadoCupos(true, false, cuposDisponibles, cupos_maximos);
                                }
                                else {
                                    EstadoCupos(false, false, 0, 0);
                                }
                            }
                            else {
                                EstadoPreReservao("Ya Realizo Una Reserva Para Esta Clase En Este Día ");
                            }
                        }
                        else {
                            EstadoPreReservao("Pago de Mensualidad No Realizado");
                        }
                    } else {
                        EstadoPreReservao("¡Clase Cancelada!");
                    }
                }
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



function IngresarReserva(arreglo_fecha, id_calendario) {
    $.ajax({
        type: 'POST',
        url: 'CrearReserva',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idCalendario: id_calendario },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var existeCupo = mensaje;
                if (existeCupo == true) {
                    ChangeFecha();
                    EstadoCupos(false, true, 0, 0);
                    $('#Error_ReservarHora').empty();
                    $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> ¡Reserva Realizada!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');

                }
                else {
                    $('#Error_ReservarHora').empty();
                    $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> Estado: !</strong>Ya No Quedan Cupos! <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                }
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



function EnviarModalUsuario()
{
    myModal.show();
}


function EnviarModalPagar() {
    myModal.hide();
    var rut = $("#inputRut").val();
    var nombre = $("#inputNombre").val();
    var apellidoPaterno = $("#inputAPP").val();
    var apellidoMaterno = $("#inputAPM").val();
    var correo = $("#inputEmail").val();
    //var telefono = $("#inputNtelefono").val();
    //var telefonoEmergencia = $("#inputNtelefonoEmer").val();

    var Arreglofecha = ObtenerFecha();
    var idclase = $("#form_ReservarHora select[name = Clase]").val();;
    var clase = $("#form_ReservarHora select[name = Clase] option:selected").text();
    var hora = $("#form_ReservarHora select[name = Horario] option:selected").text();
    var idCalendario = $("#form_ReservarHora select[name = Horario]").val();

    $("#form_PagarClase input[name = Fecha]").val(Arreglofecha[0]+"-"+Arreglofecha[1]+"-"+Arreglofecha[2]);
    $("#form_PagarClase input[name = Clase]").val(clase);
    $("#form_PagarClase input[name = Hora]").val(hora);
    $("#form_PagarClase input[name = Valor]").val(3000);
    //armar url para enviar datos
    if (ComprobarRut(rut)) {
        $.ajax({
            type: 'POST',
            url: 'CrearEnlaceMensualidadClase', 
            dataType: 'json',
            data: { mes: Arreglofecha[1], year: Arreglofecha[2], dia: Arreglofecha[0], idCalendario: idCalendario, rut: rut, correo: correo, nombre: nombre, apellidoPaterno: apellidoPaterno, apellidoMaterno: apellidoMaterno },
            beforeSend: function () {
                MostrarLoader('loader', 'div_contenido')
            },
            success: function (response) {
                var mensaje = JSON.parse(response.Message);
                var correcto = JSON.parse(response.Correcto);
                console.log(mensaje)
                console.log(correcto)
                if (correcto == true) {
                    var pagado = mensaje[0];
                    if (pagado) {
                        EstadoMensualidad(true, false, "");
                    }
                    else {
                        //aca cargar enlace y token
                        $("#form_PagarClase input[name = token_ws]").val(mensaje[2]);
                        $("#form_PagarClase")[0].action = mensaje[1];
                    }
                }
                else {
                    EstadoMensualidad(false, true, "");
                    $('#Error_ReservarHora').empty();
                    $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> ' + mensaje + '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
                }

            },
            complete: function () {
                MostrarContenido('loader', 'div_contenido');
                myModalPagar.show();
            },
            error: function (error) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
                MostrarContenido('loader', 'div_contenido');
            }
        });
    }
    else {
        $('#Error_ReservarHora').empty();
        $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> El Rut no tiene el formato correcto.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
    }
    


   
}

function ChangeFecha() {
    var fecha_arreglo = ObtenerFecha();
    CargarClase(fecha_arreglo);
    $("#form_ReservarHora select[name = Horario]").empty();
    EstadoCupos(false, true, 0, 0);
}


function ChangeSelectClase(entrada) {
    var fecha_arreglo = ObtenerFecha();
    EstadoCupos(false, true, 0, 0);
    var objeto = $("#form_ReservarHora select[name = Clase]").val();
    if (objeto.localeCompare("-1") != 0) {
        var nombreClase = $("#form_ReservarHora select[name = Clase]").text();
        var idClase = objeto;
        CargarHorario(fecha_arreglo, idClase, nombreClase);
    }
    else {
        $("#form_ReservarHora select[name = Horario]").empty();
        EstadoCupos(false, true, 0, 0);
    }
}


function ChangeSelectHorario(entrada) {
    var fecha_arreglo = ObtenerFecha();
    var objeto = $("#form_ReservarHora select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        var idCalendario = objeto;
        ComprobarCupos(fecha_arreglo, idCalendario);
    }
    else {
        //limpiar el recuadro de cupos disponible
        EstadoCupos(false, true, 0, 0 );
    }
}


function EnviarReserva() {
    var fecha_arreglo = ObtenerFecha();
    var idCalendario = $("#form_ReservarHora select[name = Horario]").val();
    if (idCalendario.localeCompare("-1") != 0) {
        IngresarReserva(fecha_arreglo, idCalendario);
    }
    else {
        alert("Error en JS, porfavor comunicarse con el administrador");
    }
    
}

function CargarSelectClase(select, array_entrada, MostrarTodo) {

    select.empty();

    array_entrada.forEach(function (Value, Index) {
        var idClas = Value[2];
        var Clas = Value[0];
        if (MostrarTodo == true) {
            select.append($('<option>', {
                value: idClas,
                text: Clas
            }));
        }
        else {
            if (idClas == 6) {
                select.append($('<option>', {
                    value: idClas,
                    text: Clas
                }));
            }
        }

    }
    );
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay clases disponibles' }));
    }

    select[0].selectedIndex = -1;
}



function CargarSelectHorario(select, array_entrada) {
    select.empty();

    array_entrada.forEach(function (Value, Index) {
        var idCal = Value[0];
        var idUbi = Value[1];
        var idHor = Value[2];
        var bloq = Value[3];
        var detalle = Value[4];
        var correcto = ComprobarHorario(detalle);
        if (correcto) {
            select.append($('<option>', {
                value: idCal,
                text: detalle
            }));
        }
    }
    );
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay horario disponible' }));
    }


    select[0].selectedIndex = -1;
}


function EstadoCupos(estado, defecto, cuposDisponibles, cuposMaximos) {
    $("#form_ReservarHora select[name = Cupos]").empty();
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-danger");
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-success");
    //$("#form_ReservarHora select[name = Cupos]").removeClass("rojo");
    if (defecto == false) {
        if (estado == true) {
            $("#form_ReservarHora select[name = Cupos]").addClass("alert-success");

            if (cuposDisponibles > 100) {
                $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
                    value: 1,
                    text: " Cupos Disponibles"
                }));
            }
            else {
                if (cuposDisponibles <= 0) {
                    $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
                        value: 1,
                        text: "Ultimos Cupos Extra Disponibles para Socios"
                    }));
                } else {
                    $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
                        value: 1,
                        text: cuposDisponibles + " Cupos Disponibles De " + cuposMaximos
                    }));
                }
            }
            $("#form_ReservarHora button[name = button]")[0].disabled = false;
        }
        else {
            $("#form_ReservarHora select[name = Cupos]").addClass("alert-danger");
            $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
                value: 0,
                text: "Sin Cupos"
            }));
            $("#form_ReservarHora button[name = button]")[0].disabled = true;
        }
    }
    else {
        $("#form_ReservarHora button[name = button]")[0].disabled = true;
    }

}


function ObtenerFecha() {
    var fecha = $('#inputFecha').val();
    var arreglo_fecha = fecha.split("/");
    return arreglo_fecha;
}


function EstadoPreReservao(texto) {
    $("#form_ReservarHora select[name = Cupos]").empty();
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-danger");
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-success");
    $("#form_ReservarHora select[name = Cupos]").addClass("alert-danger");
    $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
        value: 0,
        text: texto
    }));
    $("#form_ReservarHora button[name = button]")[0].disabled = true;
}



function ComprobarHorario(entrada) {

    if (esHoy()) {
        var today = new Date();
        // obtener la fecha y la hora Actual
        var now = today.toLocaleTimeString();
        var resultados = now.split(":");
        var hora = resultados[0];
        var minutos = resultados[1];

        //obtener hora entrada
        var hora_entrada = entrada.substring(0, 5);
        var hora_seleccionada = hora_entrada.split(":")[0];
        var minutos_seleccionada = hora_entrada.split(":")[1];

        ///llevar  a enteros (fracciones)
        var int_hora = parseInt(hora);
        var int_minutos = parseInt(minutos);
        int_hora = int_hora + (int_minutos / 60);

        var int_hora_seleccionada = parseInt(hora_seleccionada);
        var int_minutos_seleccionada = parseInt(minutos_seleccionada);
        int_hora_seleccionada = int_hora_seleccionada + (int_minutos_seleccionada / 60);
        //se resta una hora // nunca vamos a restar una hora alas 00 (no abren a medio dia)
        int_hora_seleccionada = int_hora_seleccionada - 1;

        //si mi hora es menor a la seleecionada la agrego
        if (int_hora < int_hora_seleccionada) {
            return true;
        }
        //return false;
        return true;
    }
    return true;
}


function esHoy() {
    arreglo_fecha = ObtenerFecha();
    var dia_seleccionado = arreglo_fecha[0];
    var mes_seleccionado = arreglo_fecha[1];
    var year_seleccionado = arreglo_fecha[2];

    var int_dia = parseInt(dia_seleccionado);
    var int_mes = parseInt(mes_seleccionado);
    var int_year = parseInt(year_seleccionado);
    

    var now = new Date();
    var year = now.getFullYear();
    var mes = now.getMonth() +1 ;
    var dia = now.getDate();

    if (int_year <= year) {
        if (int_mes <= mes) {
            if (int_dia <= dia) {
                return true;
            }
        }
    }

    return false;
}


function getDiasReserva(year, mes, dia) {
    var date_aux = new Date(year, mes, dia, 0, 0, 0, 0);
    var diaSemana = date_aux.getDay();
    return 7;
    if ((diaSemana >= 1) && (diaSemana <= 4)) {
        return 1;
    }
    if (diaSemana == 5) {
        return 1;
    }
    if (diaSemana == 6) {
        return 2;
    }
    if (diaSemana == 0) {
        return 1;
    }
    return 1;
}


function getDiasReservaClaseDirigida(year, mes, dia) {
    var date_aux = new Date(year, mes - 1, dia, 0, 0, 0, 0)
    var diaSemana = date_aux.getDay();

    var date_actual = new Date(year_global, mes_global, dia_global, 0, 0, 0, 0)

    var dias = date_aux - date_actual;
    dias = dias / (1000 * 60 * 60 * 24);

    var salto_dias = 1;
    if (diaSemana == 6) {
        return 2;
    }

    if (dias > salto_dias) {
        return false;  //solo debe mostrar gimnao si existe
    }
    return true; //debe mostrar todas las clases

}


function ComprobarRut(rut) {

    try {
        var bool_punto = rut.includes(".");
        if (bool_punto == true) {
            return false;
        }
        else {

            if (rut.includes("-")) {
                arrayRut = rut.split("-");
                var numero = arrayRut[0];
                var digitoVerificador = arrayRut[1];
                if ((numero.length > 6) && (numero.length < 9))
                {
                    if ((digitoVerificador.length > 0) && (digitoVerificador.length < 2)) {
                        return true;
                    }
                }
                return false;
            }
            else {
                return false
            }
        }
    }
    catch (err) {
        return false;
    }
}