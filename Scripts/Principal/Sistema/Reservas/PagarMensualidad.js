$(document).ready(Inicio());


function Inicio() {
    MostrarLoader('loader', 'div_contenido')
    CargarYear();
    ObtenerVariablesPago();
    MostrarContenido('loader', 'div_contenido');
}


function ComprobarEstadoReserva(mes, year) {
    $.ajax({
        type: 'POST',
        url: 'ComprobarMensualidad',
        dataType: 'json',
        data: { mes: mes, year: year },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var pagado = mensaje[0];
                if (pagado) {
                    EstadoMensualidad(true, false,"");
                }
                else {
                    var precioMensualidad = mensaje[3];
                    var pagoInscripcion = mensaje[4];
                    var total = mensaje[5];
                    var texto = 'Mensualidad de $' + precioMensualidad;
                    if (pagoInscripcion) {
                        texto = texto + ' más Inscripción de $3.000';
                    }
                    EstadoMensualidad(false, false, texto);
                    
                    $("#form_PagarMensualidad input[name = Valor]").val(total);

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
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}


function IniciarModalPago() {
    var myModal = new bootstrap.Modal(document.getElementById('modalPagarMensualidad'), {
        keyboard: false
    })
    myModal.show();

    var mes = "-1";
    var year = "-1";
    var objetoYear = $("#form_PagarMensualidad select[name = year]").val();
    if (objetoYear.localeCompare("-1") != 0) {
        year = objetoYear;
    }
    var objetoMes = $("#form_PagarMensualidad select[name = mes]").val();
    if (objetoMes.localeCompare("-1") != 0) {
        mes = objetoMes;
    }

    if (year != '-1') {
        if (mes != '-1') {
            $("#form_PagarMensualidadFinal input[name = year]") .val($("#form_PagarMensualidad select[name = year]").val());
            $("#form_PagarMensualidadFinal input[name = mes]").val($("#form_PagarMensualidad select[name = mes]").get(0).selectedOptions[0].label);
            $("#form_PagarMensualidadFinal input[name = Valor]").val($("#form_PagarMensualidad input[name = Valor]").val());
            AjaxIniciarModalPago(mes, year);
        }
        else {
            myModal.hide();
        }
    }
    else {
        myModal.hide();
    }
}


function AjaxIniciarModalPago(mes, year) {
    $.ajax({
        type: 'POST',
        url: 'CrearEnlaceMensualidad',
        dataType: 'json',
        data: { mes: mes, year: year },
        
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var pagado = mensaje[0];
                if (pagado) {
                    EstadoMensualidad(true, false, "");
                }
                else {
                    //aca cargar enlace y token
                    $("#form_PagarMensualidadFinal input[name = token_ws]").val(mensaje[2]);
                    $("#form_PagarMensualidadFinal")[0].action = mensaje[1];
                }
            }
            else {
                EstadoMensualidad(false, true, "");
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> ' + mensaje + '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            }

        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            //MostrarContenido('loader', 'div_contenido');
        }
    });
}

function ChangeSelectYear() {
    //aca debo cargar los meses correspondientes
    var fecha = new Date();
    var year = fecha.getFullYear();
    var mes = fecha.getMonth() + 1;
    var dia = fecha.getDate();

    var year_seleccionado = "-1";
    var objetoYear = $("#form_PagarMensualidad select[name = year]").val();
    if (objetoYear.localeCompare("-1") != 0) {
        year_seleccionado = objetoYear;
    }

    var select_mes = $("#form_PagarMensualidad select[name = mes]");
    select_mes.empty();

    select_mes.append($('<option>', { value: -1, text: 'Seleccione un mes' }));

    if (year == year_seleccionado) {
        CargarMeses(select_mes, mes, dia);
    }
    else {
        if (year < year_seleccionado) {
            select_mes.append($('<option>', { value: 1, text: 'Enero' }));
        }
    }

    ObtenerVariablesPago();
}


function ChangeSelectMes() {
    ObtenerVariablesPago();
}


function ObtenerVariablesPago() {
    var mes = "-1";
    var year = "-1";
    var objetoYear = $("#form_PagarMensualidad select[name = year]").val();
    if (objetoYear.localeCompare("-1") != 0) {
        year = objetoYear;
    }
    var objetoMes = $("#form_PagarMensualidad select[name = mes]").val();
    if (objetoMes.localeCompare("-1") != 0) {
        mes = objetoMes;
    }

    if (year != '-1') {
        if (mes != '-1') {
            ComprobarEstadoReserva(mes, year);
        }
        else {
            EstadoMensualidad(false, true,"");
        }
    }
    else {
        EstadoMensualidad(false, true,"");
    }
}


function EstadoMensualidad(estado, defecto, texto) {
    $("#form_PagarMensualidad select[name = Estado]").empty();
    $("#form_PagarMensualidad select[name = Estado]").removeClass("alert-danger");
    $("#form_PagarMensualidad select[name = Estado]").removeClass("alert-success");
    $("#form_PagarMensualidad select[name = Estado]").removeClass("alert-secondary");
    $("#form_PagarMensualidad button[name = button]")[0].disabled = true;
    //$("#form_ReservarHora select[name = Cupos]").removeClass("rojo");
    if (defecto == false) {
        if (estado == true) {
            $("#form_PagarMensualidad select[name = Estado]").addClass("alert-success");
            $("#form_PagarMensualidad select[name = Estado]").append($('<option>', {
                value: 1,
                text: "Mensualidad Pagada"
            }));
        }
        else {
            $("#form_PagarMensualidad select[name = Estado]").addClass("alert-danger");
            $("#form_PagarMensualidad select[name = Estado]").append($('<option>', {
                value: 0,
                text: texto
            }));
            $("#form_PagarMensualidad button[name = button]")[0].disabled = false;
        }
    }
    else {
        $("#form_PagarMensualidad select[name = Estado]").append($('<option>', {
            value: 0,
            text: "Seleccione un mes y año"
        }));
        $("#form_PagarMensualidad select[name = Estado]").addClass("alert-secondary");
    }
}


function CargarYear() {
    var fecha = new Date();
    var year = fecha.getFullYear();
    var mes = fecha.getMonth() + 1;
    var dia = fecha.getDate();
    var select_year = $("#form_PagarMensualidad select[name = year]");
    select_year.empty();
    select_year.append($('<option>', { value: -1, text: 'Seleccione un año' }));
    
    if (mes == 12) {
        //if (dia <= 5) {
            select_year.append($('<option>', { value: year, text: year }));
        //}
        select_year.append($('<option>', { value: year + 1, text: year + 1 }));
    } else {
        select_year.append($('<option>', { value: year, text: year }));
    }
    
}


function CargarMeses(select, intMes, intDia) {
    if (intMes < 13) {
        var maxMes = intMes + 1;
        if (maxMes >= 13) {
            maxMes = 12;
        }
        for (var i = intMes; i <= maxMes; i++) {
            select.append($('<option>', { value: i, text: ObtenerStringMes(i) }));
        }
    }
}


function ObtenerStringMes(intMes) {

    switch (intMes) {
        case 1:
            return "Enero";
            break;
        case 2:
            return "Febrero";
            break;
        case 3:
            return "Marzo";
            break;
        case 4:
            return "Abril";
            break;
        case 5:
            return "Mayo";
            break;
        case 6:
            return "Junio";
            break;
        case 7:
            return "Julio";
            break;
        case 8:
            return "Agosto";
            break;
        case 9:
            return "Septiembre";
            break;
        case 10:
            return "Octubre";
            break;
        case 11:
            return "Noviembre";
            break;
        case 12:
            return "Diciembre";
            break;
    }
}