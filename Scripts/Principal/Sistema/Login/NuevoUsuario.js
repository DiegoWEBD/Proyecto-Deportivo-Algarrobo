var myModal;
$(document).ready(Inicio());



function Inicio() {
    myModal = new bootstrap.Modal(document.getElementById('staticBackdrop'));
}

function LeerTerminos()
{
    var objeto = $("#checkTerminos")[0].checked;

    if (objeto) {
        
        myModal.show();
    }
    
}

function HabilitarTextBox() {
    var objeto = $("#checkEnfermedad")[0].checked;
    $("#inputEnfermedad").val("");
    if (objeto) {
        $("#inputEnfermedad").prop('disabled', false);
        $("#inputEnfermedad").prop('required', true);
    }
    else {
        $("#inputEnfermedad").prop('disabled', true);
        $("#inputEnfermedad").prop('required', false);
    }
}

function AceptarTerminos() {
    myModal.hide();
}

function CancelarTerminos() {
    $("#checkTerminos")[0].checked = false;
}



function ComprobarSiNecesitaCertificado() {
    var rut = $("#inputRut").val();
    var fechaNac = $("#inputFechaNac").val();
    var fechaActual = Date;


    /*
    if (rut.length < 11) {
        $.ajax({
            type: 'POST',
            url: 'TienePagadoOctubre',
            dataType: 'json',
            data: { rut: rut },
            beforeSend: function () {
           
            },
            success: function (response) {
            
                var correcto = JSON.parse(response.Correcto);
                if (correcto == true) {
                
                    document.getElementById("div_certificado").style.display = "none";
                    document.getElementById("div_soloInstalaciones").style.display = "none";
                    $("#hiddenSoloInstalacion").val("FALSE");
                    $("#certificado").prop('required', false);
                }
                else {
                    document.getElementById("div_certificado").style.display = "inline";
                    document.getElementById("div_soloInstalaciones").style.display = "inline";
                    $("#certificado").prop('required', true);
                }

            },
            complete: function () {
           
            },
            error: function () {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            }
        });
    }
    */
}

function esSoloReservaInstalaciones() {
    var estadoCheck = $("#checkInstalacion")[0].checked;
    if (estadoCheck == true) {
        document.getElementById("div_certificado").style.display = "none";
        $("#certificado").prop('required', false);
        $("#hiddenSoloInstalacion").val("TRUE");

    }
    else {
        document.getElementById("div_certificado").style.display = "inline";
        $("#certificado").prop('required', true);
        $("#hiddenSoloInstalacion").val("FALSE");
    }
}

function ComprobarEdad() {
    var fecha = $("#inputFechaNac").val();
    var arrayFecha = fecha.split("-");
    var fecha_actual = new Date();
    var year_actual = fecha_actual.getFullYear();
    var mes_actual = fecha_actual.getMonth() + 1;
    var dia_actual = fecha_actual.getDate();

    var year = arrayFecha[0];
    var mes = arrayFecha[1];
    var dia = arrayFecha[2];

    //poner restriccion
    document.getElementById("div_certificado").style.display = "inline";
    $("#certificado").prop('required', true);

    var result_year = year_actual - year;
    if (result_year > 18) {
        //ocultar y quitar restriccion
        document.getElementById("div_certificado").style.display = "none";
        $("#hiddenSoloInstalacion").val("FALSE");
        $("#certificado").prop('required', false);
    }
    else {
        if (result_year == 18) {
            if (mes_actual < mes) {
                //ocultar y quitar restriccion
                document.getElementById("div_certificado").style.display = "none";
                $("#hiddenSoloInstalacion").val("FALSE");
                $("#certificado").prop('required', false);
            }
            else {
                if (mes_actual == mes) {
                    if (dia_actual <= dia) {
                        //ocultar y quitar restriccion
                        document.getElementById("div_certificado").style.display = "none";
                        $("#hiddenSoloInstalacion").val("FALSE");
                        $("#certificado").prop('required', false);
                    }
                }
            }
        }
    }


}