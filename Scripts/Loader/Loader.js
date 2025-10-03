function MostrarLoader(id_loader, id_contenido) {
    document.getElementById(id_loader).style.display = "block";
    document.getElementById(id_contenido).style.display = "none";
}


function MostrarContenido(id_loader, id_contenido) {
    document.getElementById(id_loader).style.display = "none";
    document.getElementById(id_contenido).style.display = "block";
}