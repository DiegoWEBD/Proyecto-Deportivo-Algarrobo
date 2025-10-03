var myModalCancelarClase;
var myModalAgregarReserva;
var myModalCancelarInstalacion;
var myModalCancelarDiaInstalacion;
$(document).ready(Inicio());

function Inicio() {
    MostrarLoader('loader', 'div_contenido')
    $('#inputFecha').daterangepicker({
        singleDatePicker: true,
        timePicker: false,
        singleClasses: "picker_4",
        timePicker24Hour: false,
        timePickerSeconds: false,
        autoApply: true,
        //maxDate: moment().endOf("day").add(, 'days'),
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
    MostrarContenido('loader', 'div_contenido');
    //IniciarTablaImprimir('tabla_reservas', true, true, false, false, false, true);
    IniciarTablaBuscarUsuarios('tabla_reservas', true, true, false, false, false, false);
    myModalCancelarClase = new bootstrap.Modal(document.getElementById('cancelarClaseModal')); //cancelar una instalacion espeficica
    myModalAgregarReserva = new bootstrap.Modal(document.getElementById('datosAgregarReservaModal'));
    myModalCancelarInstalacion = new bootstrap.Modal(document.getElementById('cancelarInstalacionModal')); //todo el dia para una instalacion
    myModalCancelarDiaInstalacion = new bootstrap.Modal(document.getElementById('cancelarInstalacionDiaModal')); //cancelar todas las intalaciones de todo un dia

    
}


function ChangeFecha(entrada) {
    var fecha_arreglo = ObtenerFecha();
    //CargarClase(fecha_arreglo);
    $("#form_AdminClases select[name = Horario]").empty();
    $("#form_AdminClases select[name = Instalacion]").empty(); //no deberia ir

    CargarTipoInstalacion(fecha_arreglo);

    $("#form_AdminClases button[name = buttonCancelarDia]")[0].disabled = false;
    $("#form_AdminClases button[name = buttonCancelarClase]")[0].disabled = true;
    $("#form_AdminClases button[name = buttonAgregarReserva]")[0].disabled = true;
}


function ChangeTipoInstalacion() {
    var fecha_arreglo = ObtenerFecha();
    var tipoInstalacion = $("#form_AdminClases select[name = TipoInstalacion]").val();
    CargarInstalacion(fecha_arreglo, tipoInstalacion);
    $("#form_ReservarHora select[name = instalacion]").empty();
    $("#form_ReservarHora select[name = Horario]").empty();
}


function ChangeSelectInstalacion() {
    $("#form_AdminClases select[name = Horario]").empty();
    $("#form_AdminClases button[name = buttonCancelarDia]")[0].disabled = false;
    $("#form_AdminClases button[name = buttonCancelarClase]")[0].disabled = false;
    $("#form_AdminClases button[name = buttonAgregarReserva]")[0].disabled = true;

    $("#form_ReservarHora select[name = Horario]").empty();
    var fecha_arreglo = ObtenerFecha();
    var objeto = $("#form_AdminClases select[name = Instalacion]").val();
    if (objeto.localeCompare("-1") != 0) {
        var nombreInstalacion = $("#form_AdminClases select[name = Instalacion]").text();
        var idInstalacion = objeto;
        CargarHorario(fecha_arreglo, idInstalacion);
    }
    else {
        $("#form_AdminClases select[name = Horario]").empty();
    }
}


function ChangeSelectHorario() {
    //buscar reservas para instalacion.
    //si no existen reservas desbloquear boton para agregar reservas.
    $("#form_AdminClases button[name = buttonCancelarDia]")[0].disabled = false;
    $("#form_AdminClases button[name = buttonCancelarClase]")[0].disabled = false;
    $("#form_AdminClases button[name = buttonAgregarReserva]")[0].disabled = false;
    var fecha_arreglo = ObtenerFecha();
    var objeto = $("#form_AdminClases select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        var idCalendario = objeto.split("/")[0];
        var idInstalacion = $("#form_AdminClases select[name = Instalacion]").val();
        CargarReserva(fecha_arreglo[0], fecha_arreglo[1], fecha_arreglo[2], idCalendario, idInstalacion);
    }
    else {
        //error
    }
}

function CargarReservasDia(dia, mes, year) {
    
    $.ajax({
        type: 'POST',
        url: 'ObtenerReservasDia',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year },
        beforeSend: function () {
            var tabla = $('#tabla_reservas').DataTable();
            tabla.clear().draw();
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var tabla = $('#tabla_reservas').DataTable();
                tabla.clear().draw();
                MostrarContenido('loader', 'div_contenido');
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    //idReserva, clase, horario, nombre, apellido, userName
                    var onclick = "ActualizarPanel('" + fila[0] + "', '" + fila[7] + "',' " + fila[9] + "',' " + fila[2] + "',' " + fila[3] + " "+fila[4] + "',' " + fila[1] + "')"
                    var buttonBorrar = '<div><button name="button" data-bs-toggle="modal" data-bs-target="#cancelarClaseModal" class="btn btn-danger" onclick="' + onclick + '" >Cancelar</button></div>';

                    /*boton para descaragar archivo */
                    var nameFile = fila[12];
                    var link = '';
                    var texBoton = "Descargar";
                    var style = "btn btn-secondary"
                    var style2 = "cursor: not-allowed"
                    if (nameFile != '') {
                        link = href ='https://reservasdeportivoalgarrobo.cl/Asistentes/' + nameFile;
                        texBoton = "Descargar";
                        style = "btn btn-primary";
                        style2 = "";
                    }
                    var buttonFile = '<a href="'+ link +'" download class="' + style + '" target="_blank" style="' + style2 + '" >' + texBoton + '</a>';
                    /*fin boton para descaragar archivo */

                    var telefono = fila[6];
                    var correo = fila[7];
                    var contacto = '<a href="https://api.whatsapp.com/send?phone=56' + telefono + '" target="_blank"><i class="fab fa-whatsapp"></i></a>';
                    contacto = contacto + ' <a href = "mailto: ' + correo + '"  target="_blank"><i class="far fa-envelope"></i></a>';
                    var telefono = ' <a href="tel:+' + telefono + '"><i class="fas fa-phone"></i></a>';
                    contacto = contacto + telefono;
                    tabla.row.add([fila[2], fila[3], fila[4], fila[7], fila[8], fila[9], contacto, fila[10], fila[11], fila[13], buttonFile, buttonBorrar]).draw();

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


function CargarReservasInstalacion(dia, mes, year, idInstalacion) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerReservasInstalacion',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year, idInstalacion: idInstalacion },
        beforeSend: function () {
            var tabla = $('#tabla_reservas').DataTable();
            tabla.clear().draw();
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var tabla = $('#tabla_reservas').DataTable();
                tabla.clear().draw();
                MostrarContenido('loader', 'div_contenido');
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];

                    /*boton para descaragar archivo */
                    var nameFile = fila[12];
                    var link = '';
                    var texBoton = "Descargar";
                    var style = "btn btn-secondary"
                    var style2 = "cursor: not-allowed"
                    if (nameFile != '') {
                        link = href = 'https://reservasdeportivoalgarrobo.cl/Asistentes/' + nameFile;
                        texBoton = "Descargar";
                        style = "btn btn-primary";
                        style2 = "";
                    }
                    var buttonFile = '<a href="' + link + '" download class="' + style + '" target="_blank" style="' + style2 + '" >' + texBoton + '</a>';
                    /*fin boton para descaragar archivo */


                    var onclick = "ActualizarPanel('" + fila[0] + "', '" + fila[7] + "',' " + fila[9] + "',' " + fila[2] + "',' " + fila[3] + " " + fila[4] + "',' " + fila[1] + "')"
                    var buttonBorrar = '<div><button name="button" data-bs-toggle="modal" data-bs-target="#cancelarClaseModal" class="btn btn-danger" onclick="' + onclick + '" >Cancelar</button></div>';

                    var telefono = fila[6];
                    var correo = fila[7];
                    var contacto = '<a href="https://api.whatsapp.com/send?phone=56' + telefono + '" target="_blank"><i class="fab fa-whatsapp"></i></a>';
                    contacto = contacto + ' <a href = "mailto: ' + correo + '"  target="_blank"><i class="far fa-envelope"></i></a>';
                    var telefono = ' <a href="tel:+' + telefono + '"><i class="fas fa-phone"></i></a>';
                    contacto = contacto + telefono;
                    tabla.row.add([fila[2], fila[3], fila[4], fila[7], fila[8], fila[9], contacto, fila[10], fila[11], fila[13], buttonFile, buttonBorrar]).draw();

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


function CargarReservasTipoInstalacion(dia, mes, year, idTipoInstalacion) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerReservasTipoInstalacion',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year, idTipoInstalacion: idTipoInstalacion },
        beforeSend: function () {
            var tabla = $('#tabla_reservas').DataTable();
            tabla.clear().draw();
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var tabla = $('#tabla_reservas').DataTable();
                tabla.clear().draw();
                MostrarContenido('loader', 'div_contenido');
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var onclick = "ActualizarPanel('" + fila[0] + "', '" + fila[7] + "',' " + fila[9] + "',' " + fila[2] + "',' " + fila[3] + " " + fila[4] + "',' " + fila[1] + "')"
                    var buttonBorrar = '<div><button name="button" data-bs-toggle="modal" data-bs-target="#cancelarClaseModal" class="btn btn-danger" onclick="' + onclick + '" >Cancelar</button></div>';

                    /*boton para descaragar archivo */
                    var nameFile = fila[12];
                    var link = '';
                    var texBoton = "Descargar";
                    var style = "btn btn-secondary"
                    var style2 = "cursor: not-allowed"
                    if (nameFile != '') {
                        link = href = 'https://reservasdeportivoalgarrobo.cl/Asistentes/' + nameFile;
                        texBoton = "Descargar";
                        style = "btn btn-primary";
                        style2 = "";
                    }
                    var buttonFile = '<a href="' + link + '" download class="' + style + '" target="_blank" style="' + style2 + '" >' + texBoton + '</a>';
                    /*fin boton para descaragar archivo */


                    var telefono = fila[6];
                    var correo = fila[7];
                    var contacto = '<a href="https://api.whatsapp.com/send?phone=56' + telefono + '" target="_blank"><i class="fab fa-whatsapp"></i></a>';
                    contacto = contacto + ' <a href = "mailto: ' + correo + '"  target="_blank"><i class="far fa-envelope"></i></a>';
                    var telefono = ' <a href="tel:+' + telefono + '"><i class="fas fa-phone"></i></a>';
                    contacto = contacto + telefono;
                    tabla.row.add([fila[2], fila[3], fila[4], fila[7], fila[8], fila[9], contacto, fila[10], fila[11], fila[13], buttonFile, buttonBorrar]).draw();

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

function CargarTipoInstalacion(arreglo_fecha) {
    $.ajax({
        type: 'POST',
        url: 'CargarTipoInstalacion',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2] },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            var mensaje = JSON.parse(response.Message);
            console.log(mensaje)
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
            CargarReservasDia(arreglo_fecha[0], arreglo_fecha[1], arreglo_fecha[2]);
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function CargarInstalacion(arreglo_fecha, tipoInstalacion) {
    $.ajax({
        type: 'POST',
        url: 'CargarInstalacionTipo',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], tipoInstalacion: tipoInstalacion },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var correcto = JSON.parse(response.Correcto);
            var mensaje = JSON.parse(response.Message);
            if (correcto == true) {
                CargarSelectClase($("#form_AdminClases select[name = Instalacion]"), mensaje);
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }

        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
            CargarReservasTipoInstalacion(arreglo_fecha[0], arreglo_fecha[1], arreglo_fecha[2], tipoInstalacion);
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function CargarHorario(arreglo_fecha, id_instalacion) {
    $.ajax({
        type: 'POST',
        url: 'CargarInstalacionHorario',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idClase: id_instalacion },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectHorario($("#form_AdminClases select[name = Horario]"), mensaje)
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
            CargarReservasInstalacion(arreglo_fecha[0], arreglo_fecha[1], arreglo_fecha[2], id_instalacion);
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function CargarReserva(dia, mes, year, idCalendario, idInstalacion ) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerReservas',
        dataType: 'json',
        data: { dia: dia, mes: mes, year: year, idCalendario: idCalendario, idInstalacion: idInstalacion },
        beforeSend: function () {
            var tabla = $('#tabla_reservas').DataTable();
            tabla.clear().draw();
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var cupoCompartido = JSON.parse(response.cupoCompartido);
                var tabla = $('#tabla_reservas').DataTable();
                tabla.clear().draw();
                MostrarContenido('loader', 'div_contenido');
                $("#form_AdminClases button[name = buttonAgregarReserva]")[0].disabled = false;
                if ((mensaje.length > 0)||(cupoCompartido == false)) {
                        $("#form_AdminClases button[name = buttonAgregarReserva]")[0].disabled = true;
                }

                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var onclick = "ActualizarPanel('" + fila[0] + "', '" + fila[7] + "',' " + fila[9] + "',' " + fila[2] + "',' " + fila[3] + " " + fila[4] + "',' " + fila[1] + "')"
                    var buttonBorrar = '<div><button name="button" data-bs-toggle="modal" data-bs-target="#cancelarClaseModal" class="btn btn-danger" onclick="' + onclick + '" >Cancelar</button></div>';

                    var telefono = fila[6];
                    var correo = fila[7];
                    /*boton para descaragar archivo */
                    var nameFile = fila[12];
                    var link = '';
                    var texBoton = "Descargar";
                    var style = "btn btn-secondary"
                    var style2 = "cursor: not-allowed"
                    if (nameFile != '') {
                        link = href = 'https://reservasdeportivoalgarrobo.cl/Asistentes/' + nameFile;
                        texBoton = "Descargar";
                        style = "btn btn-primary";
                        style2 = "";
                    }
                    var buttonFile = '<a href="' + link + '" download class="' + style + '" target="_blank" style="' + style2 + '" >' + texBoton + '</a>';


                    var contacto = '<a href="https://api.whatsapp.com/send?phone=56' + telefono + '" target="_blank"><i class="fab fa-whatsapp"></i></a>';
                    contacto = contacto + ' <a href = "mailto: ' + correo + '"  target="_blank"><i class="far fa-envelope"></i></a>';
                    var telefono = ' <a href="tel:+' + telefono + '"><i class="fas fa-phone"></i></a>';
                    contacto = contacto + telefono;
                    tabla.row.add([fila[2], fila[3], fila[4], fila[7], fila[8], fila[9], contacto, fila[10], fila[11], buttonFile, buttonBorrar]).draw();
                   
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


function CancelarReserva() {
    var idReserva = $("#form_cancelarClases input[name = idReserva]").val();
    var userName  = $("#form_cancelarClases input[name = userName]").val();
    $.ajax({
        type: 'POST',
        url: 'CancelarReserva',
        dataType: 'json',
        data: { idReserva: idReserva, userName: userName },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Reserva Cancelada con Éxito:!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');

            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR!: </strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            myModalCancelarClase.hide();
        },
        complete: function () {
            ChangeFecha();
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });

}


function CancelarInstalacionAjax() {
    var id_instalacion = $("#form_AdminClases select[name = Instalacion]").val();
    var fecha_arreglo = ObtenerFecha();
    var nombreInstalacion = $("#form_AdminClases select[name = Instalacion] option:selected").text();

    $.ajax({
        type: 'POST',
        url: 'CancelarInstalacion',
        dataType: 'json',
        data: { idInstalacion: id_instalacion, dia: fecha_arreglo[0], mes: fecha_arreglo[1], year: fecha_arreglo[2], nombreInstalacion: nombreInstalacion },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Reservas e Instalaciones Canceladas con Éxito!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');

            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR!: </strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            myModalCancelarClase.hide();
        },
        complete: function () {
            myModalCancelarInstalacion.hide();
            ChangeFecha();
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });


}


function CancelarInstalacionDiaAjax() {
    var id_instalacion = $("#form_AdminClases select[name = Instalacion]").val();
    var fecha_arreglo = ObtenerFecha();

    $.ajax({
        type: 'POST',
        url: 'CancelarInstalacionesDia',
        dataType: 'json',
        data: { dia: fecha_arreglo[0], mes: fecha_arreglo[1], year: fecha_arreglo[2]},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Reservas e Instalaciones Canceladas con Éxito!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');

            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR!: </strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            myModalCancelarClase.hide();
        },
        complete: function () {
            myModalCancelarDiaInstalacion.hide();
            ChangeFecha();
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });


}



function ObtenerFecha() {
    var fecha = $('#inputFecha').val();
    var arreglo_fecha = fecha.split("/");
    return arreglo_fecha;
}

/**
 * Carga una arreglo con el select de clases
 * @param {any} select
 * @param {any} array_entrada
 */
function CargarSelectClase(select, array_entrada) {

    select.empty();

    array_entrada.forEach(function (Value, Index) {
        var idClas = Value[2];
        var Clas = Value[0];
        select.append($('<option>', {
            value: idClas,
            text: Clas
        }));
    });

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
        //var correcto = ComprobarHorario(detalle);
        var correcto = true;
        if (correcto) {
            select.append($('<option>', {
                value: idCal + "/" + idUbi + "/" + idHor,
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


function ComprobarHorario(entrada) {

    if (esHoy()) {
        var today = new Date();
        // obtener la fecha y la hora Actual
        var now = today.toLocaleTimeString();
        var resultados = now.split(":");
        var hora = resultados[0];
        var minutos = resultados[1];


        var hora_entrada = entrada.substring(0, 5);
        var hora_seleccionada = hora_entrada.split(":")[0];
        var minutos_seleccionada = hora_entrada.split(":")[1];


        var int_hora = parseInt(hora);
        var int_minutos = parseInt(minutos);
        int_hora = int_hora + (int_minutos / 60);

        var int_hora_seleccionada = parseInt(hora_seleccionada);
        var int_minutos_seleccionada = parseInt(minutos_seleccionada);
        int_hora_seleccionada = int_hora_seleccionada + (int_minutos_seleccionada / 60);
        int_hora_seleccionada = int_hora_seleccionada - 1;

        if (int_hora < int_hora_seleccionada) {
            return true;
        }
        return false;
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
    var mes = now.getMonth() + 1;
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


function ActualizarPanel(idReserva, clase, horario, nombre, apellido, userName) {
    $('#form_cancelarClases input[name="idReserva"]').empty();
    $('#form_cancelarClases input[name="idReserva"]').val(idReserva);
    $('#form_cancelarClases input[name="userName"]').empty();
    $('#form_cancelarClases input[name="userName"]').val(userName);
    $("#contenidoModel").empty();
    $("#contenidoModel").append("Desea cancelar la reserva de " + nombre + " " + apellido + " para " + clase + " de las " + horario);
}



function CancelarDia() {

    var fecha_arreglo = ObtenerFecha();
    $("#contenidoCancelarInstalacionDiaModal").empty();
    $("#contenidoCancelarInstalacionDiaModal").append("Desea cancelar todas las reservas y bloquear las futuras reservas de instalaciones para el dia  " + fecha_arreglo[0] + "  del " + fecha_arreglo[1] + " del " + fecha_arreglo[2] + "");
}


function CancelarClase() {

    var fecha_arreglo = ObtenerFecha();
    var nombreInstalacion = $("#form_AdminClases select[name = Instalacion] option:selected").text();
    $("#contenidoCancelarInstalacionModal").empty();
    $("#contenidoCancelarInstalacionModal").append("Desea cancelar todas las reservas y bloquear las futuras reservas de la " + nombreInstalacion+" para el dia  " + fecha_arreglo[0] + "  del " + fecha_arreglo[1] + " del " + fecha_arreglo[2] + "");
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


function RealizarReserva() {
    var fecha_arreglo = ObtenerFecha();
    var id_calendario = $("#form_AdminClases select[name = Horario]").val();
    var nombre = $("#FormDatosUsuario input[name = nombre]").val();
    var apellido_paterno = $("#FormDatosUsuario input[name = apellidoPaterno]").val();
    var apellido_materno = $("#FormDatosUsuario input[name = apellidoMaterno]").val();
    var rut = $("#FormDatosUsuario input[name = rutUsuario]").val();
    var correo = $("#FormDatosUsuario input[name = email]").val();
    var telefono = $("#FormDatosUsuario input[name = telefono]").val();
    var monto = $("#FormDatosUsuario input[name = monto]").val();

    var HorarioLlegada = $("#FormDatosUsuario input[name = HorarioLlegada]").val();
    var Requerimientos = $("#FormDatosUsuario input[name = Requerimientos]").val();

    var noRepetir = $("#flexRadioDefault1")[0].checked;
    var RepetirSemana = $("#flexRadioDefault2")[0].checked;
    var RepetirMes = $("#flexRadioDefault3")[0].checked;

    var arrayid = id_calendario.split("/");
    id_calendario = arrayid[0];
    var id_ubi = arrayid[1];
    var id_hor = arrayid[2];

    var Repetir = 0;
    if (RepetirSemana == true) {
        Repetir = 1;
    }
    if (RepetirMes == true) {
        Repetir = 2;
    }

    $.ajax({
        type: 'POST',
        url: 'GuardarReservaInstalacion',
        dataType: 'json',
        data: { dia: fecha_arreglo[0], mes: fecha_arreglo[1], year: fecha_arreglo[2], idCalendario: id_calendario, nombre: nombre, app: apellido_paterno, apm: apellido_materno, rut: rut, correo: correo, monto: monto, numeroTelefono: telefono, repetir: Repetir, idHorario: id_hor, idUbicacion: id_ubi, requerimientos: Requerimientos, HorarioLlegada: HorarioLlegada},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
            myModalAgregarReserva.hide();
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var existeCupo = mensaje;
                if (existeCupo == true) {
                    //ChangeFecha();
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
            ChangeFecha();
            MostrarContenido('loader', 'div_contenido');
            /*volver valores a 0*/

            $("#FormDatosUsuario input[name = nombre]").val("");
            $("#FormDatosUsuario input[name = apellidoPaterno]").val("");
            $("#FormDatosUsuario input[name = apellidoMaterno]").val("");
            $("#FormDatosUsuario input[name = rutUsuario]").val("");
            $("#FormDatosUsuario input[name = email]").val("");
            $("#FormDatosUsuario input[name = telefono]").val("");
            $("#FormDatosUsuario input[name = monto]").val("");

            $("#FormDatosUsuario input[name = HorarioLlegada]").val("");
            $("#FormDatosUsuario input[name = Requerimientos]").val("");

            $("#flexRadioDefault1")[0].checked = true;
            $("#flexRadioDefault2")[0].checked = false;
            $("#flexRadioDefault3")[0].checked = false;
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function ButonPersonalizado() {
    var table = $('#tabla_reservas').DataTable();
    var wb = XLSX.utils.book_new();
    wb.Props = {
        Title: "Usuarios",
        Subject: "Plataforma Reservas",
        Author: "Raul Carrizo C."
    };
    wb.SheetNames.push("Reservas");

    var ws_data = [['Nombre', 'Apellido Paterno', 'Apellido Materno', 'Instalación', 'Fecha', 'Horario', 'Requerimientos', 'Horario Llegada']];
    table.rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();
        ws_data.push([data[0], data[1], data[2], data[3], data[4], data[5], data[7], { v: data[8], s: { font: { name: "Courier", sz: 24 } } } ]);
    });

    var ws = XLSX.utils.aoa_to_sheet(ws_data);
    wb.Sheets["Reservas"] = ws;
    var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
    function s2ab(s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

    saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), 'Reservas.xlsx');



}


function PonerNombreSemanaRepetir()
{
    var fecha_arreglo = ObtenerFecha();
    var year = fecha_arreglo[2];
    var mes =  fecha_arreglo[1]- 1;
    var dia = fecha_arreglo[0];
    var datetime = new Date(year, mes, dia, 0, 0, 0, 0);
    new Date()
    var dia = datetime.getDay();
    var diaPalabra = "Lunes";
    if (dia == 2) {
        diaPalabra = "Martes";
    }
    if (dia == 3) {
        diaPalabra = "Miércoles";
    }
    if (dia == 4) {
        diaPalabra = "Jueves";
    }
    if (dia == 5) {
        diaPalabra = "Viernes";
    }
    if (dia == 6) {
        diaPalabra = "Sábado";
    }
    if (dia == 0) {
        diaPalabra = "Domingo";
    }

    document.getElementById("LabelRepetirSemana").innerHTML = "Repetir Todos Los " + diaPalabra + " Que Quedan Del Mes (Mismo Horario).";
}