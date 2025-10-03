$(document).ready(Inicio());



function Inicio() {
    MostrarLoader('loader', 'div_contenido');
    $('#inputFecha').daterangepicker({
        singleDatePicker: true,
        timePicker: false,
        singleClasses: "picker_4",
        timePicker24Hour: false,
        timePickerSeconds: false,
        autoApply: true,
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
    IniciarTablaBuscarUsuarios('tabla_usuarios', true, true, false, false, true, true);
    IniciarTablaBuscar('tabla_mensualidad', true, true, false, false, false, false);
    CargarDatos();
    CargarYear();
    cargarEmpresas();
    cargarTipoSocio();
    $('.multiple-select').multipleSelect()
}


function CargarDatos(arreglo_fecha, id_calendario) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerUsuarios',
        dataType: 'json',
        data: { },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            var tabla = $('#tabla_usuarios').DataTable();
            tabla.clear().draw();
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
                    const socio = (fila[9]) ? fila[9] : 'No Socio'
                    var botonMensualidades = '<button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#mensualidadModal" onclick="BuscarMensualidad(\'' + fila[3] + '\',\'' + fila[0] + '\',\'' + fila[1] + '\',\'' + fila[2] + '\')">Ver</button>';
                    var botonProfile = '<button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#profileModal" onclick="BuscarPerfil(\'' + fila[3] + '\')">Modificar</button>';
                    var botonReserva = '<button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#profileReserva" onclick="PasarModalReservaHora(\'' + fila[0] + '\',\'' + fila[1] + '\',\'' + fila[3] + '\')">Reservar</button>';
                    var botonEliminar = '<button type="button" class="btn btn-primary"  onclick="EliminarUsuario(\'' + fila[0] + '\', \'' + fila[1] + '\', \'' + fila[3] + '\')">Eliminar</button>';
                    //tabla.row.add([fila[0], fila[1], fila[2], fila[3], fila[4], fila[5], fila[6], fila[7], botonMensualidades, botonProfile, botonReserva]).draw();
                    //deberia agregar como html y despues inicar la tabla
                    dataSet.push([fila[0], fila[1], fila[2], fila[3], fila[4], fila[5], fila[6], fila[7], socio, botonMensualidades, botonProfile, botonReserva, botonEliminar]);
                }
                
                tabla.destroy();
                IniciarTablaBuscarUsuariosDatos('tabla_usuarios', true, true, false, false, true, true, dataSet)
                //tabla.responsive.rebuild();
                //tabla.responsive.recalc();
                
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


function BuscarPerfil(idPerfil) {
    $.ajax({
        type: 'POST',
        url: 'ObtenerUsuario',
        dataType: 'json',
        data: { usuario: idPerfil },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            //limpiar check
            $("#checkEnfermedad")[0].checked = false;
            $("#checkSocio")[0].checked = false;
            $("#checkSoloReservaInstalacion")[0].checked = false;
            $("#checkBloquearCuenta")[0].checked = false;
            $("#modalUsuario select[name = Empresa]")[0].selectedIndex = -1;
            $("#modalUsuario button[name = BotonGuardar]")[0].disabled = false;
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);

            if (correcto == true) {
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    $("#modalUsuario input[name = nombreUsuario]").val(idPerfil);
                    $("#modalUsuario input[name = nombre]").val(fila[0]);
                    $("#modalUsuario input[name = apellidoPaterno]").val(fila[1]);
                    $("#modalUsuario input[name = apellidoMaterno]").val(fila[2]);
                    $("#modalUsuario input[name = Ntelefono]").val(fila[4]);
                    $("#modalUsuario input[name = NtelefonoEmergencia]").val(fila[5]);
                    $("#modalUsuario input[name = email]").val(fila[6]);
                    $("#modalUsuario input[name = Enfermedad]").val(fila[8]);
                    if (fila[8] != "") {
                        $("#checkEnfermedad")[0].checked = true;
                    }
                    if (fila[9] != '0') {
                        $("#checkSocio")[0].checked = true;

                        if (fila[14] != 'NULL' || fila[14] != null || fila[14] != '0') {
                            $("#modalUsuario select[name = TipoSocio]").val(fila[14]);
                            ChangeCheckUsuario()
                        }

                        if (fila[12] == 'NULL' || fila[12] == null || fila[12] == '') {
                            $("#modalUsuario select[name = Empresa]")[0].selectedIndex = -1;
                            $("#modalUsuario select[name = Empresa]")[0].disabled = true;
                        } else {
                            $("#modalUsuario select[name = Empresa]").val(fila[12]);
                        }
                    } else {
                        ChangeCheckUsuario();
                    }///nuevos
                    if (fila[10] != 'False') {
                        $("#checkSoloReservaInstalacion")[0].checked = true;
                    }
                    if (fila[11] != 'False') {
                        $("#checkBloquearCuenta")[0].checked = true;
                    }
                    

                    
                    $("#modalUsuario select[name = Clase]").val(fila[7]); //aca es un select y se debe agregar
                }
                
            }
            else {
                //desailitra boton
                $("#modalUsuario button[name = BotonGuardar]")[0].disabled = true;
                $('#Error_ReservarHora2').empty();
                $("#Error_ReservarHora2").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
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


function ChangeCheckUsuario() {
    const socio = $("#checkSocio")[0].checked;
    console.log(socio)
    if (socio == true) {
        //
        $("#modalUsuario select[name = TipoSocio]")[0].disabled = false;
        if ($("#modalUsuario select[name = TipoSocio]").val() != '') {
            $("#modalUsuario select[name = Empresa]")[0].disabled = false;
        }
    } else {
        $("#modalUsuario select[name = Empresa]")[0].disabled = true;
        $("#modalUsuario select[name = TipoSocio]")[0].disabled = true;
        $("#modalUsuario select[name = Empresa]")[0].selectedIndex = -1;
        $("#modalUsuario select[name = TipoSocio]")[0].selectedIndex = -1;
    }
}

function ActualizarPerfil() {
    var nombreUsuario = $("#modalUsuario input[name = nombreUsuario]").val();
    var nombre = $("#modalUsuario input[name = nombre]").val();
    var apellidoPaterno = $("#modalUsuario input[name = apellidoPaterno]").val();
    var apellidoMaterno = $("#modalUsuario input[name = apellidoMaterno]").val();
    var Ntelefono = $("#modalUsuario input[name = Ntelefono]").val();
    var NtelefonoEmergencia = $("#modalUsuario input[name = NtelefonoEmergencia]").val();
    var email = $("#modalUsuario input[name = email]").val();
    var Enfermedad = "";
    var password = $("#modalUsuario input[name = password]").val();
    var password_confirmacion = $("#modalUsuario input[name = password_confirmacion]").val();
    if ($("#checkEnfermedad")[0].checked == true) {
        var Enfermedad = $("#modalUsuario input[name = Enfermedad]").val();
    }
    var socio = $("#checkSocio")[0].checked;
    var soloReservasInstalaciones = $("#checkSoloReservaInstalacion")[0].checked;
    var bloqueado = $("#checkBloquearCuenta")[0].checked;

    var perfil = $("#modalUsuario select[name = Clase]").val();
    let tipoSocio = null
    if (socio == true) {
        var idTipoSocio = $("#modalUsuario select[name = TipoSocio]").val() || 0;
        tipoSocio = $("#modalUsuario select[name = TipoSocio]").find('option:selected').text()
        var empresa = $("#modalUsuario select[name = Empresa]").val() || 0;
    }

    $.ajax({
        type: 'POST',
        url: 'ActualizarUsuario',
        dataType: 'json',
        data: { nombreUsuario: nombreUsuario, nombre: nombre, apellidoPaterno: apellidoPaterno, apellidoMaterno: apellidoMaterno, Ntelefono: Ntelefono, NtelefonoEmergencia: NtelefonoEmergencia, email: email, Enfermedad: Enfermedad, password: password, password_confirmacion: password_confirmacion, Perfil: perfil, Socio: socio, soloInstalaciones: soloReservasInstalaciones, bloqueado: bloqueado, empresa: empresa, idTipoSocio: idTipoSocio, tipoSocio: tipoSocio },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> ¡Datos Actualizados!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                CargarDatos();
            }
            else {
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong>' + mensaje + ' <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
            }
            var myModalEl = document.getElementById('profileModal');
            var modal = bootstrap.Modal.getInstance(myModalEl);
            modal.hide();
            
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


function BuscarMensualidad(idPerfil, nombre, apellidoPaterno, apellidoMaterno) {
    $.ajax({
        type: 'POST',
        url: 'BuscarMensualidades',
        dataType: 'json',
        data: { usuario: idPerfil},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido');
            var tabla = $('#tabla_mensualidad').DataTable();
            tabla.clear().draw();
            $('#userNameMensualidad').val(idPerfil);
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var tabla = $('#tabla_mensualidad').DataTable();
                tabla.clear().draw();
                for (var i = 0; i < mensaje.length; i++) {
                    var fila = mensaje[i];
                    var tipo_transaccion = fila[5];
                    var tipo_transaccion_aux = "";
                    switch (tipo_transaccion) {
                        case 'VD':
                            tipo_transaccion_aux = 'Venta Débito';
                            break;
                        case 'VN':
                            tipo_transaccion_aux = 'Venta Normal';
                            break;
                        case 'VC':
                            tipo_transaccion_aux = 'Venta en cuotas';
                            break;
                        case 'SI':
                            tipo_transaccion_aux = '3 cuotas sin interés.';
                            break;
                        case 'S2':
                            tipo_transaccion_aux = '2 cuotas sin interés.';
                            break;
                        case 'NC':
                            tipo_transaccion_aux = 'N Cuotas sin interés.';
                            break;
                        case 'VP':
                            tipo_transaccion_aux = 'Venta Prepago.';
                            break;
                        case 'MANUAL':
                            tipo_transaccion_aux = 'Ingreso Manual.';
                            break;
                        default:
                            tipo_transaccion_aux = tipo_transaccion;
                            break;
                    }
                    var botonBorrarTranferencia = '';
                    if (fila[4].includes("ALL") == false) {
                        botonBorrarTranferencia = '<button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#profileBorrarTranferencia" onclick="PasarModalBorrarTranferencia(\'' + fila[6] + '\',\'' + fila[7] + '\',\'' + fila[0] + '\',\'' + fila[1] + '\',\'' + nombre + '\',\'' + apellidoPaterno + '\',\'' + apellidoMaterno + '\')">Borrar</button>';
                    }
                    tabla.row.add([fila[0], fila[1], fila[2], fila[3], fila[4], tipo_transaccion_aux, botonBorrarTranferencia]).draw();
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


function PagarMensualidad(rut, mes, year, monto) {
   
    $.ajax({
        type: 'POST',
        url: 'PagarMensualidadManual',
        dataType: 'json',
        data: { rut: rut, mes: mes, year: year, monto: monto },
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
            var myModalEl = document.getElementById('mensualidadPagoModal');
            var modal = bootstrap.Modal.getInstance(myModalEl);
            modal.hide();

            var myModalEl2 = document.getElementById('mensualidadModal');
            var modal2 = bootstrap.Modal.getInstance(myModalEl2);
            modal2.hide();


        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En ingresar mensualidad a base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });


}


function BorrarMensualidad() {
    var myModalEl = document.getElementById('profileBorrarTranferencia');
    var modal = bootstrap.Modal.getInstance(myModalEl);
    modal.hide();

    var idTranferencia = $("#form_borrarMensualidadModal input[name = idTranferencia]").val();
    var idMensualidad = $("#form_borrarMensualidadModal input[name = idMensualidad]").val();

    $.ajax({
        type: 'POST',
        url: 'BorrarMensualidadManual',
        dataType: 'json',
        data: { idTranferencia: idTranferencia, idMEnsualidad: idMensualidad },
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
            var myModalEl = document.getElementById('mensualidadModal');
            var modal = bootstrap.Modal.getInstance(myModalEl);
            modal.hide();
        },
        complete: function () {
            MostrarContenido('loader', 'div_contenido');
        },
        error: function () {
            $('#Error_ReservarHora').empty();
            $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR:!</strong> En ingresar mensualidad a base de datos.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>')
            MostrarContenido('loader', 'div_contenido');
        }
    });
}



function CargarYear() {
    var fecha = new Date();
    var year = fecha.getFullYear();
    var mes = fecha.getMonth() + 1;
    var dia = fecha.getDate();
    var select_year = $("#formIngresarMensualidad select[name = year]");
    select_year.empty();
    select_year.append($('<option>', { value: -1, text: 'Seleccione un año' }));

    if (mes == 12) {
        if (dia <= 31) {
            select_year.append($('<option>', { value: year, text: year }));
        }
        select_year.append($('<option>', { value: year + 1, text: year + 1 }));
    } else {
        select_year.append($('<option>', { value: year, text: year }));
    }

}


function ChangeSelectYear() {
    //aca debo cargar los meses correspondientes
    var fecha = new Date();
    var year = fecha.getFullYear();
    var mes = fecha.getMonth() + 1;
    var dia = fecha.getDate();

    var year_seleccionado = "-1";
    var objetoYear = $("#formIngresarMensualidad select[name = year]").val();
    if (objetoYear.localeCompare("-1") != 0) {
        year_seleccionado = objetoYear;
    }

    var select_mes = $("#formIngresarMensualidad select[name = mes]");
    select_mes.empty();

    select_mes.append($('<option>', { value: -1, text: 'Seleccione un mes' }));

    if (year == year_seleccionado) {
        CargarFULL(select_mes);
    }
    else {
        if (year < year_seleccionado) {
            select_mes.append($('<option>', { value: 1, text: 'Enero' }));
        }
    }
}


function CargarMeses(select, intMes, intDia) {
    if (intDia >= 5) {
        intMes = intMes + 1;
    }
    if (intMes < 13) {
        for (var i = intMes; i < 13; i++) {
            select.append($('<option>', { value: i, text: ObtenerStringMes(i) }));
        }
    }
}


function CargarFULL(select, intMes, intDia) {

        for (var i = 1; i < 13; i++) {
            select.append($('<option>', { value: i, text: ObtenerStringMes(i) }));
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


function FijarRutMensualidad() {
    
    $("#formIngresarMensualidad input[name = nombreUsuario]").val($('#userNameMensualidad').val());
    $("#formIngresarMensualidad select[name = year]")[0].selectedIndex = 0;
    $("#formIngresarMensualidad select[name = mes]")[0].selectedIndex = 0;
    $("#formIngresarMensualidad input[name = monto]").val("28000");

}


function ActualizarPago() {
    var mes = "-1";
    var year = "-1";
    var objetoYear = $("#formIngresarMensualidad select[name = year]").val();
    if (objetoYear.localeCompare("-1") != 0) { year = objetoYear; }
    var objetoMes = $("#formIngresarMensualidad select[name = mes]").val();
    if (objetoMes.localeCompare("-1") != 0) { mes = objetoMes; }
    var rut = $("#formIngresarMensualidad input[name = nombreUsuario]").val();
    var monto = $("#formIngresarMensualidad input[name = monto]").val();

    if (year != '-1') {
        if (mes != '-1') {
            PagarMensualidad(rut, mes, year, monto);
        }
        else {
            $('#Error_ActualizarPago').empty();
            $("#Error_ActualizarPago").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong> Debe seleccionar un mes <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
        }
    }
    else {
        $('#Error_ActualizarPago').empty();
        $("#Error_ActualizarPago").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> ERROR: !</strong> Debe seleccionar un año <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
    }
}


function CancelarEliminacionMensualidad() {
    var myModalEl = document.getElementById('profileBorrarTranferencia');
    var modal = bootstrap.Modal.getInstance(myModalEl);
    modal.hide();

    myModalEl = document.getElementById('mensualidadModal');
    modal = bootstrap.Modal.getInstance(myModalEl);
    modal.show();
}


function PasarModalBorrarTranferencia(idTranferencia, idMensualidad, mes, year, nombre, app, apm) {
    var myModalEl = document.getElementById('mensualidadModal');
    var modal = bootstrap.Modal.getInstance(myModalEl);
    modal.hide();    
    $('#contenidoEliminarTranferencia').empty();
    $("#contenidoEliminarTranferencia").append('¿Seguro que Desea Eliminar la Mensualidad de ' + mes + ' del ' + year + ' a Nombre de: ' + nombre + ' ' + app + ' ' + apm + ' ?');
    $("#form_borrarMensualidadModal input[name = idTranferencia]").val();
    $("#form_borrarMensualidadModal input[name = idMensualidad]").val();
    $("#form_borrarMensualidadModal input[name = idTranferencia]").val(idTranferencia);
    $("#form_borrarMensualidadModal input[name = idMensualidad]").val(idMensualidad);

}

function PasarModalReservaHora(nombre, apellido, rut) {
    $("#form_ReservarHora input[name = nombre]").val(nombre + " " + apellido);
    $("#form_ReservarHora input[name = rut]").val(rut);
    
    $("#form_ReservarHora select[name = Horario]")[0].selectedIndex = 0;
    $("#form_ReservarHora select[name = Clase]")[0].selectedIndex = 0;
    EstadoCupos(false, true, 0, 0);
}


function EliminarUsuario(nombre, apellidoPaterno, rut) {
    const eliminar = confirm(`¿Eliminar el usuario ${nombre} ${apellidoPaterno} ?`)
    if (eliminar) {
        EliminarUsuarioBD(rut)
    }
}


function ChangeFecha(entrada) {
    var fecha_arreglo = ObtenerFecha();
    CargarClase(fecha_arreglo);
    $("#form_ReservarHora select[name = Horario]").empty();
    EstadoCupos(false, true, 0, 0);
}


function ChangeSelectClase(entrada) {
    var fecha_arreglo = ObtenerFecha();
    var objeto = $("#form_ReservarHora select[name = Clase]").val();
    if (objeto.localeCompare("-1") != 0) {
        var nombreClase = $("#form_ReservarHora select[name = Clase]").text();
        var idClase = objeto;
        CargarHorario(fecha_arreglo, idClase, nombreClase);
    }
    else {
        $("#form_ReservarHora select[name = Horario]").empty();
        $("#form_ReservarHora select[name = Horario]").append($('<option>', {
            value: "-1",
            text: "Seleccione un Horario"
        }));
    }
    EstadoCupos(false, true, 0, 0);
}


function ChangeSelectHorario() {
    var fecha_arreglo = ObtenerFecha();
    var objeto = $("#form_ReservarHora select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        var idCalendario = objeto;
        var rut = $("#form_ReservarHora input[name = rut]").val();
        ComprobarCupos(fecha_arreglo, idCalendario, rut);
    }
    else {
        //limpiar el recuadro de cupos disponible
        EstadoCupos(false, true, 0, 0);
    }
}


function ObtenerFecha() {
    var fecha = $('#inputFecha').val();
    var arreglo_fecha = fecha.split("/");
    return arreglo_fecha;
}


function EstadoCupos(estado, defecto, cuposDisponibles, cuposMaximos) {
    $("#form_ReservarHora select[name = Cupos]").empty();
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-danger");
    $("#form_ReservarHora select[name = Cupos]").removeClass("alert-success");
    if (defecto == false) {
        if (estado == true) {
            $("#form_ReservarHora select[name = Cupos]").addClass("alert-success");
            $("#form_ReservarHora select[name = Cupos]").append($('<option>', {
                value: 1,
                text: cuposDisponibles + " Cupos Disponibles De " + cuposMaximos
            }));
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

function ReservarHora() {
    var fecha_arreglo = ObtenerFecha();
    //id calendario
    var idCalendario = "-1";
    var objeto = $("#form_ReservarHora select[name = Horario]").val();
    if (objeto.localeCompare("-1") != 0) {
        idCalendario = objeto;
    }


    if ((idCalendario > 0)) {
        var rut = $("#form_ReservarHora input[name = rut]").val();
        GuardarReserva(fecha_arreglo, idCalendario,  rut);
    }


}


function GuardarReserva(arreglo_fecha ,idCalendario, Rut) {
    $.ajax({
        type: 'POST',
        url: 'CrearReserva',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idCalendario: idCalendario,  Rut: Rut },
        beforeSend: function () {
              MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                var existeCupo = mensaje;
                if (existeCupo == true) {
                    EstadoCupos(false, true, 0, 0);
                    $('#Error_ReservarHora').empty();
                    $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> ¡Reserva Realizada!</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');

                }
                else {
                    $('#Error_ReservarHora').empty();
                    $("#Error_ReservarHora").append('<div  class="alert alert-danger alert-dismissible fade show"  role="alert" ><strong> Estado: !</strong>Ya No Quedan Cupos! <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                }
                var myModalEl = document.getElementById('profileReserva');
                var modal = bootstrap.Modal.getInstance(myModalEl);
                modal.hide();
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

function EliminarUsuarioBD(rut) {
    $.ajax({
        type: 'POST',
        url: 'EliminarUsuarioBD',
        dataType: 'json',
        data: { rut: rut },
        beforeSend: function () {
            //  MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            console.log(mensaje, correcto)
            if (correcto == true) {
                // mostrar mensaje de usuario eliminado
                $('#Error_ReservarHora').empty();
                $("#Error_ReservarHora").append('<div  class="alert alert-success alert-dismissible fade show"  role="alert" ><strong> Usuario Eliminado Correctamente</strong> <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                CargarDatos()
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

function CargarClase(arreglo_fecha) {
    $.ajax({
        type: 'POST',
        url: 'CargarClase',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2] },
        beforeSend: function () {
          //  MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectClase($("#form_ReservarHora select[name = Clase]"), mensaje);
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

function ComprobarCupos(arreglo_fecha, id_calendario, rut) {
    $.ajax({
        type: 'POST',
        url: 'TieneCuposDisponibles',
        dataType: 'json',
        data: { dia: arreglo_fecha[0], mes: arreglo_fecha[1], year: arreglo_fecha[2], idCalendario: id_calendario, rut: rut },
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
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


function CargarSelectClase(select, array_entrada) {

    select.empty();
    select.append($('<option>', {
        value: "-1",
        text: "Seleccione una Clase"
    }));

    array_entrada.forEach(function (Value, Index) {
        var idClas = Value[2];
        var Clas = Value[0];
        select.append($('<option>', {
            value: idClas,
            text: Clas
        }));
    }
    );
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay clases disponibles' }));
    }
    //cargar horarios
    $("#form_ReservarHora select[name = Horario]").empty();
    $("#form_ReservarHora select[name = Horario]").append($('<option>', {
        value: "-1",
        text: "Seleccione un Horario"
    }));
    select[0].selectedIndex = 0;
}


function CargarSelectHorario(select, array_entrada) {
    select.empty();
    select.append($('<option>', {
        value: "-1",
        text: "Todos Los Horarios"
    }));

    array_entrada.forEach(function (Value, Index) {
        var idCal = Value[0];
        var idUbi = Value[1];
        var idHor = Value[2];
        var bloq = Value[3];
        var detalle = Value[4];

        select.append($('<option>', {
            value: idCal,
            text: detalle
        }));
    }
    );
    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay horario disponible' }));
    }


    select[0].selectedIndex = 0;
}
function ButonPersonalizado() { 
    var table = $('#tabla_usuarios').DataTable();
    var wb = XLSX.utils.book_new();
    wb.Props = {
        Title: "Usuarios",
        Subject: "Plataforma Reservas",
        Author: "Raul Carrizo C."
    };
    wb.SheetNames.push("usuarios");

    var ws_data = [['Nombre', 'Apellido Paterno', 'Apellido Materno', 'Rut', 'Telefono', 'Tel. Emergencia', 'Correo', 'Perfil']];
    table.rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();
        ws_data.push([data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]]);
    });

    var ws = XLSX.utils.aoa_to_sheet(ws_data);
    wb.Sheets["usuarios"] = ws;
    var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
    function s2ab(s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }
   
    saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), 'test.xlsx');
   


}

function CargarSelectEmpresa(select, array_entrada) {
    select.empty();
    //select.append($('<option>', {
    //    value: "-1",
    //    text: "Seleccione una Empresa"
    //}));

    array_entrada.forEach(function (Value, Index) {
        var idEmp = Value[1];
        var Emp = Value[0];
        select.append($('<option>', {
            value: idEmp,
            text: Emp
        }));
    }
    );
    //if (array_entrada.length == 0) {
    //    select.append($('<option>', { value: -1, text: 'No hay empresas disponibles' }));
    //}

    select[0].selectedIndex = -1;
}

function cargarTipoSocio() {
    $.ajax({
        type: 'POST',
        url: 'CargarTipoSocio',
        dataType: 'json',
        data: {},
        beforeSend: function () {
            MostrarLoader('loader', 'div_contenido')
        },
        success: function (response) {
            var mensaje = JSON.parse(response.Message);
            var correcto = JSON.parse(response.Correcto);
            if (correcto == true) {
                CargarSelectTipoSocio($("#modalUsuario select[name = TipoSocio]"), mensaje);
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

function ChangeTipoSocio() {
    const tipoSocio = $("#modalUsuario select[name = TipoSocio]").val()
    if (tipoSocio == 1) {
        $("#modalUsuario select[name = Empresa]")[0].disabled = false;
    } else {
        $("#modalUsuario select[name = Empresa]")[0].disabled = true;
        $("#modalUsuario select[name = Empresa]")[0].selectedIndex = -1;
    }
}

function CargarSelectTipoSocio(select, array_entrada) {
    select.empty();

    array_entrada.forEach(function (Value, Index) {
        var idTipo = Value[0];
        var Tipo = Value[1];
        select.append($('<option>', {
            value: idTipo,
            text: Tipo
        }));
    });


    if (array_entrada.length == 0) {
        select.append($('<option>', { value: -1, text: 'No hay selección disponibles' }));
    }

    select[0].selectedIndex = -1;    
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
                CargarSelectEmpresa($("#modalUsuario select[name = Empresa]"), mensaje)
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

function onchangeTieneCarga() {
    const tieneCarga = $("#checkTieneCarga")[0].checked;
    if (tieneCarga) {

    } else {
        $("#modalUsuario select[name = Cargas]").hide;
    }
}



