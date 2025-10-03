$(document).ready(Inicio());

function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    MostrarContenido('loader', 'div_contenido');
    IniciarTablaBuscar('tabla_usuarios', true, true, false, false, true, true);
    cargarCertificados();
}



function cargarCertificados() {
    $.ajax({
        type: 'POST',
        url: 'ObtenerCertificados',
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
                MostrarContenido('loader', 'div_contenido');
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var botonMensualidades = '<a download href="https://reservasdeportivoalgarrobo.cl/Certificados/' + fila[4] +'" class="btn btn-primary" target="_blank">Descargar</a>';
                    tabla.row.add([fila[0], fila[1], fila[2], fila[3], botonMensualidades]).draw();
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