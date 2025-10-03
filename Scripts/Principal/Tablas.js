/*
$('#tab_content_1').addClass('active in');
$('#tabPrincipalEquipo_1').addClass('active');

*/



/**
 * Funcion que inicia una tabla (DataTable.net)
 * @param {string} id_tabla 
 * @param {boolean} responsive
 * @param {boolean} paging
 * @param {boolean} filter
 * @param {boolean} info
 * @param {boolean} ordering
 * @param {boolean} buscar
 */
function IniciarTablaBuscarUsuarios(id_tabla, responsive, paging, filter, info, ordering, buscar) {
    var tabla = $('#' + id_tabla).DataTable({
        responsive: responsive,
        paging: paging,
        searching: buscar,
        filter: filter,
        info: info,
        ordering: ordering,
        dom: 'Bfrtip',
        buttons: [
            {
                text: 'Exportar',
                action: function (e, dt, node, config) {
                    ButonPersonalizado();
                }
            }
        ],
        language: {
            lengthMenu: "Mostrar _MENU_  registros",
            zeroRecords: "Sin Registros",
            infoEmpty: "Sin Registros",
            search: "Buscar:"
        }
    });
    return tabla;
}


/**
 * Funcion que inicia una tabla (DataTable.net)
 * @param {string} id_tabla 
 * @param {boolean} responsive
 * @param {boolean} paging
 * @param {boolean} filter
 * @param {boolean} info
 * @param {boolean} ordering
 * @param {boolean} buscar
 */
function IniciarTablaBuscarUsuariosDatos(id_tabla, responsive, paging, filter, info, ordering, buscar, datos) {
    var tabla = $('#' + id_tabla).DataTable({
        responsive: responsive,
        paging: paging,
        searching: buscar,
        filter: filter,
        info: info,
        ordering: ordering,
        dom: 'Bfrtip',
        data: datos,
        buttons: [
            {
                text: 'Exportar',
                action: function (e, dt, node, config) {
                    ButonPersonalizado();
                }
            }
        ],
        language: {
            lengthMenu: "Mostrar _MENU_  registros",
            zeroRecords: "Sin Registros",
            infoEmpty: "Sin Registros",
            search: "Buscar:"
        }
    });
    return tabla;
}



/**
 * Funcion que inicia una tabla (DataTable.net)
 * @param {string} id_tabla 
 * @param {boolean} responsive
 * @param {boolean} paging
 * @param {boolean} filter
 * @param {boolean} info
 * @param {boolean} ordering
 * @param {boolean} buscar
 */
function IniciarTablaSocios(id_tabla, responsive, paging, filter, info, ordering, buscar) {
    var tabla = $('#' + id_tabla).DataTable({
        responsive: responsive,
        paging: paging,
        searching: buscar,
        filter: filter,
        info: info,
        ordering: ordering,
        dom: 'Bfrtip',
        buttons: [
            {
                text: 'Agregar',
                action: function (e, dt, node, config) {
                    ButonPersonalizado();
                }
            }
        ],
        language: {
            lengthMenu: "Mostrar _MENU_  registros",
            zeroRecords: "Sin Registros",
            infoEmpty: "Sin Registros",
            search: "Buscar:"
        }
    });
    return tabla;
}

/**
 * Funcion que inicia una tabla (DataTable.net)
 * @param {string} id_tabla 
 * @param {boolean} responsive
 * @param {boolean} paging
 * @param {boolean} filter
 * @param {boolean} info
 * @param {boolean} ordering
 * @param {boolean} buscar
 */
function IniciarTablaSociosDatos(id_tabla, responsive, paging, filter, info, ordering, buscar, datos) {
    var tabla = $('#' + id_tabla).DataTable({
        responsive: responsive,
        paging: paging,
        searching: buscar,
        filter: filter,
        info: info,
        ordering: ordering,
        dom: 'Bfrtip',
        data: datos, 
        buttons: [
            {
                text: 'Agregar',
                action: function (e, dt, node, config) {
                    ButonPersonalizado();
                }
            }
        ],
        language: {
            lengthMenu: "Mostrar _MENU_  registros",
            zeroRecords: "Sin Registros",
            infoEmpty: "Sin Registros",
            search: "Buscar:"
        }
    });
    return tabla;
}



/**
 * Funcion que inicia una tabla (DataTable.net)
 * @param {string} id_tabla 
 * @param {boolean} responsive
 * @param {boolean} paging
 * @param {boolean} filter
 * @param {boolean} info
 * @param {boolean} ordering
 * @param {boolean} buscar
 */
function IniciarTablaBuscar(id_tabla, responsive, paging, filter, info, ordering, buscar) {
    var tabla = $('#' + id_tabla).DataTable({
        responsive: responsive,
        paging: paging,
        searching: buscar,
        filter: filter,
        info: info,
        ordering: ordering,
        language: {
            lengthMenu: "Mostrar _MENU_  registros",
            zeroRecords: "Sin Registros",
            infoEmpty: "Sin Registros",
            search: "Buscar:"
        }
    });
    return tabla;
}


/**
 * Funcion que inicia una tabla (DataTable.net)
 * @param {string} id_tabla 
 * @param {boolean} responsive
 * @param {boolean} paging
 * @param {boolean} filter
 * @param {boolean} info
 * @param {boolean} ordering
 */
function IniciarTabla(id_tabla, responsive, paging, filter, info, ordering) {
    var tabla = $('#' + id_tabla).DataTable({
        responsive: responsive,
        paging: paging,
        filter: filter,
        info: info,
        ordering: ordering,
        language: {
            lengthMenu: "Mostrar _MENU_  registros",
            zeroRecords: "Sin Registros",
            infoEmpty: "Sin Registros",
            search: "Buscar:"
        }
    });
    return tabla;
}




/**
 * Funcion que inicia una tabla (DataTable.net)
 * @param {string} id_tabla
 * @param {boolean} responsive
 * @param {boolean} paging
 * @param {boolean} filter
 * @param {boolean} info
 * @param {boolean} ordering
 */
function IniciarTablaImprimir(id_tabla, responsive, paging, filter, info, ordering) {
    var tabla = $('#' + id_tabla).DataTable({
        responsive: responsive,
        paging: paging,
        filter: filter,
        info: info,
        ordering: ordering,
        dom: 'Bfrtip',
        buttons: [
            'excel'
        ],
        language: {
            lengthMenu: "Mostrar _MENU_  registros",
            zeroRecords: "Sin Registros",
            infoEmpty: "Sin Registros",
            search: "Buscar:"
        }
    });
    return tabla;
}



function EliminarFilaTabla(id_tabla, fila_entrada) {
    var tabla = $('#' + id_tabla).DataTable();
    tabla.row(fila_entrada.parentElement.parentElement).remove().draw();
}


function LimpiarTabla(id_tabla) {
    var tabla = $('#' + id_tabla).DataTable();
    tabla.clear().draw();
}







