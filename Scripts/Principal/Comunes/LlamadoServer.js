$(document).ready(InicioTimmer());


function InicioTimmer() {
    setTimeout(ValidarSesion, 300000);
}


function ValidarSesion() {
    $.ajax({
        type: 'POST',
        url: 'VerificarSesion',
        dataType: 'json',
        data: { },
        beforeSend: function () {
            
        },
        success: function (response) {
            setTimeout(ValidarSesion, 300000);
        },
        error: function () {
            //cerrar session
        }
    });
}

