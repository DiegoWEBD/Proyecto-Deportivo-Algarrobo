var myModalVerReserva;
var Calendar;
$(document).ready(Inicio());


function Inicio() {
    MostrarLoader('loader', 'div_contenido');
	MostrarContenido('loader', 'div_contenido');
	var eventos = [];
	IniciarCalendario(eventos);
	//CargarEventos();
	myModalVerReserva = new bootstrap.Modal(document.getElementById('modalReservaInstalacion'));
}



function IniciarCalendario(ArregloEntrada) {
	var calendarEl = document.getElementById('Calendario');
	var currentDate = new Date().toJSON().slice(0, 10);

	Calendar = new FullCalendar.Calendar(calendarEl, {
		headerToolbar: {
			left: 'prev,next today',
			center: 'title',
			right: 'dayGridMonth'
		},
		themeSystem: 'bootstrap5',
		locale: 'es',
		initialDate: currentDate,
		navLinks: true, // can click day/week names to navigate views
		selectable: true,
		selectMirror: true,
		eventClick: function (arg) {
			MostrarEvento(arg);
		},
		datesSet: event => {
		    CargarEventos();
		},
		editable: false,
		dayMaxEvents: false //, // allow "more" link when too many events
		//events: ArregloEntrada
	});
	Calendar.setOption('height', 700);
	Calendar.render();
}



function CargarEventos() {

	var year = Calendar.getDate().toJSON().slice(0, 4);
	var mes = Calendar.getDate().toJSON().slice(5, 7);
	$.ajax({
		type: 'POST',
		url: 'ObtenerReservasMes',
		dataType: 'json',
		data: { year: year, mes: mes},
		beforeSend: function () {
			MostrarLoader('loader', 'div_contenido')
		},
		success: function (response) {
			var correcto = JSON.parse(response.Correcto);
			var listEvent = Calendar.getEvents();
			listEvent.forEach(event => {
				event.remove()
			});
			var arregloTrabajos = [];
			if (correcto == true) {
				var mensaje = JSON.parse(response.Message);
				for (var i = 0; i < mensaje.length; i++) {
					var fila = mensaje[i];

					var idReserva = fila[0];
					var instalacion = fila[1];
					var fecha_inicio = fila[2];
					var fecha_termino = fila[2];
					var horario = fila[3];

					arregloTrabajos.push({
						title: instalacion,
						id: idReserva,
						start: CambiarFormatoFecha(fecha_inicio, horario, false),
						end: CambiarFormatoFecha(fecha_termino, horario, true) + 'T23:59:59',
						classNames: ObtenerColor(instalacion)
					})

					Calendar.addEvent({
						title: instalacion,
						id: idReserva,
						start: CambiarFormatoFecha(fecha_inicio, horario, false),
						end: CambiarFormatoFecha(fecha_termino, horario, true) + 'T23:59:59',
						classNames: ObtenerColor(instalacion)
					});

				}
				MostrarContenido('loader', 'div_contenido');
			}
			else {
				var mensaje = JSON.parse(response.message);
				$('#Error_Page').empty();
				$("#Error_Page").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
			}

		},
		complete: function () {
			MostrarContenido('loader', 'div_contenido');
		},
		error: function () {
			$('#Error_Page').empty();
			$("#Error_Page").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
			MostrarContenido('loader', 'div_contenido');
		}
	});
}





function CambiarFormatoFecha(entrada, hora, final) {
	

	var dia = entrada.split("-")[0];
	var mes = entrada.split("-")[1];
	var year = entrada.split("-")[2];

	if (hora == 'Día Completo') {
		var fecha = year + "-" + mes + "-" + dia;
		if (final == true) {
			fecha = fecha + 'T23:59:59'
        }
		return fecha;
	}
	var hora_inicio = hora.split('/')[0];
	var hora_termino = hora.split('/')[1];
	var fecha = year + "-" + mes + "-" + dia;
	if (final == false) {
		fecha = fecha + 'T' + hora_inicio + ':00'
	}
	else {
		fecha = fecha + 'T' + hora_termino + ':00'
    }
	return fecha;
	
}


function ObtenerColor(instalacion) {

	if (instalacion.includes("Futbol")) {
		instalacion = 'Futbol';
	}
	if (instalacion.includes("Tenis")) {
		instalacion = 'Tenis';
	}
	if (instalacion.includes("Picnic")) {
		instalacion = 'Picnic';
	}
	if (instalacion.includes("Multicancha")) {
		instalacion = 'Multicancha';
	}
	if (instalacion.includes("Reunion")) {
		instalacion = 'Reunion';
	}
	if (instalacion.includes("Sala")) {
		instalacion = 'Sala';
	}

	

	switch (instalacion) {
		case 'Futbol':
			return 'style_1';
		case 'Tenis':
			return 'style_2';
		case 'Picnic':
			return 'style_3';
		case 'Reunion':
			return 'style_4';
		case 'Sala':
			return 'style_5';
		default:
			return 'style_6';
    }
}


function MostrarEvento(entrada) {
	var idReserva   = entrada.event.id;
	var Instalacion = entrada.event.title;

	$.ajax({
		type: 'POST',
		url: 'ObtenerReservaInstalacion',
		dataType: 'json',
		data: { idReserva: idReserva },
		beforeSend: function () {
			MostrarLoader('loader', 'div_contenido')
		},
		success: function (response) {
			var correcto = JSON.parse(response.Correcto);
			if (correcto == true) {
				var mensaje = JSON.parse(response.Message);
				if (mensaje.length > 0)
				{
					var instalacion = Instalacion;
					var fecha = mensaje[0][7];
					var horario = mensaje[0][8];
					var rut = mensaje[0][0];
					var nombre = mensaje[0][1];
					var app = mensaje[0][2];
					var apm = mensaje[0][3];
					var correo = mensaje[0][4];
					var ntelefono = mensaje[0][5];
					var requerimientos = mensaje[0][9];
					$("#FormDatosUsuario input[name = instalacion]").val(instalacion);
					$("#FormDatosUsuario input[name = fecha]").val(fecha);
					$("#FormDatosUsuario input[name = horario]").val(horario);
					$("#FormDatosUsuario input[name = rutUsuario]").val(rut);
					$("#FormDatosUsuario input[name = nombre]").val(nombre);
					$("#FormDatosUsuario input[name = apellidoPaterno]").val(app);
					$("#FormDatosUsuario input[name = apellidoMaterno]").val(apm);
					$("#FormDatosUsuario input[name = correo]").val(correo);
					$("#FormDatosUsuario input[name = ntelefono]").val(ntelefono);
					$("#FormDatosUsuario input[name = requerimiento]").val(requerimientos);
					myModalVerReserva.show();
				}
			}
		},
		complete: function () {
			MostrarContenido('loader', 'div_contenido');
		},
		error: function () {
			$('#Error_Page').empty();
			$("#Error_Page").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En cargar datos desde la base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
			MostrarContenido('loader', 'div_contenido');
		}
	});

}