using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;
using WebApplicationTemplateBase.Filters;
using WebApplicationTemplateBase.Models;

namespace WebApplicationTemplateBase.Controllers
{
    public class ReservasController : Controller
    {
        //GET: Reservas
        public ActionResult Index()
        {
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "1")]
        public ActionResult ReservarHora()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            ///
            ViewBag.dia = DateTime.Now.Day;
            ViewBag.mes = DateTime.Now.Month-1;
            ViewBag.year = DateTime.Now.Year;
            ViewBag.hora = DateTime.Now.Hour;
            //
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "5")]
        public ActionResult PagarMensualidad()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;

            return View();
        }


        [HttpPost]
        public JsonResult VerificarSesion()
        {
            int cantidad = Session.Count;
            return Json(new { Correcto = true, JsonRequestBehavior.AllowGet });
        }

        /// <summary>
        /// para reservar hora pagando sola la clase, sin tener pagada la mensualidad
        /// </summary>
        /// <returns></returns>
        public ActionResult ReservarHoraDia()
        {
            ViewBag.menu = new Dictionary<string, Object[]>();
            ViewBag.userName = "Invitado";
            ViewBag.nombre = "Invitado";
            ///
            ViewBag.dia = DateTime.Now.Day;
            ViewBag.mes = DateTime.Now.Month - 1;
            ViewBag.year = DateTime.Now.Year;
            ViewBag.hora = DateTime.Now.Hour;
            //
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "7")]
        public ActionResult MisReservas()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;

            return View();
        }


        public ActionResult returnPagarMensualidad(String token_ws, String TBK_ID_SESION, String TBK_ORDEN_COMPRA, String TBK_TOKEN )
        {
            ViewBag.Realizada = false;
            try
            {
                if(token_ws != null)
                {
                    Models.Usuario UsuarioAux = ((Models.Usuario)Session["Usuario"]);
                    Models.Tranferencia Tranferecia = new Models.Tranferencia(token_ws);
                    if (Tranferecia.getResponseCode() == 0)
                    {
                       
                        string rut = UsuarioAux.Rut;
                        string correo = UsuarioAux.Correo;
                       
                        ViewBag.OrdenCompra = Tranferecia.getBuyOrder();
                        ViewBag.Monto = Tranferecia.getMontoFormatoMiles();
                        ViewBag.CodigoAutorizacion = Tranferecia.getAuthorizationCode();
                        ViewBag.FechaTransaccion = Tranferecia.getTransactionDateHorarioChile(4);
                        ViewBag.CantidadCuotas = Tranferecia.getInstallmentsNumber();
                        ViewBag.TipoCuotas = Tranferecia.esCuotas();
                        ViewBag.MontoCuotas = Tranferecia.getInstallmentsAmount();
                        ViewBag.TipoTransaccion = Tranferecia.pasarAFormatoTipoPago();

                        ConsultasExternas.Correos.EnviarCorreoConfirmacionPagoMensualidad(correo, Tranferecia);

                        bool correcto = GuardarTransaccionConfirmada(Tranferecia, rut);
                        int numeroIntentos = 0;
                        if(correcto == false){
                            numeroIntentos = 3;
                        }
                        for (int i = 0; i < numeroIntentos; i++)
                        {
                            correcto = GuardarTransaccionConfirmada(Tranferecia, rut);
                            if (correcto == true){
                                i = numeroIntentos;
                            }
                        }
                        ////****/////
                        

                        if (correcto)
                        {
                            ViewBag.Realizada = true;
                            //ConsultasExternas.Correos.EnviarCorreoConfirmacionPagoMensualidad(correo, Tranferecia);
                        }
                        else
                        {
                            ViewBag.Realizada = false;
                            ViewBag.Detalle = "Transferencia Realizada, pero no registrada en la base de datos, por favor contáctese con la administración";
                        }
                    }
                    else
                    {
                        ViewBag.Realizada = false;
                        ViewBag.Detalle = Tranferecia.getDetalleResponseCode();
                    }
                }
                else
                {
                    if(TBK_TOKEN != null)
                    {
                        ViewBag.Detalle = "Tranferencia Cancelada";
                        Models.Usuario UsuarioAux =  RecuperarSession(TBK_ID_SESION);
                        Session["Usuario"] = UsuarioAux;
                    }
                    else
                    {        
                        ViewBag.Detalle = "Tiempo de Espera Expirado";
                    }
                }

                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
                ViewBag.menu = MenuIzquierdo.ObtenerMenu();
                ViewBag.userName = Usuario.Rut;
                ViewBag.nombre = Usuario.NombreUsuario;
                return View();
            }
            catch(Exception exp)
            {
                ViewBag.Realizada = false;
                ViewBag.Detalle = exp.Message;
                return View();
            }
        }


        public ActionResult  returnPagarMensualidadClase(String token_ws, String TBK_ID_SESION, String TBK_ORDEN_COMPRA, String TBK_TOKEN)
        {
            ViewBag.Realizada = false;
            try
            {

                if (token_ws != null)
                {
                    Models.Usuario UsuarioAux = ((Models.Usuario)Session["Usuario"]);
                   

                    Models.Tranferencia Tranferecia = new Models.Tranferencia(token_ws);
                    if (Tranferecia.getResponseCode() == 0)
                    {

    
                        string rut = Session["Rut"].ToString();

                        ViewBag.OrdenCompra = Tranferecia.getBuyOrder();
                        ViewBag.Monto = Tranferecia.getMontoFormatoMiles();
                        ViewBag.CodigoAutorizacion = Tranferecia.getAuthorizationCode();
                        ViewBag.FechaTransaccion = Tranferecia.getTransactionDateHorarioChile(4);
                        ViewBag.CantidadCuotas = Tranferecia.getInstallmentsNumber();
                        ViewBag.TipoCuotas = Tranferecia.esCuotas();
                        ViewBag.MontoCuotas = Tranferecia.getInstallmentsAmount();
                        ViewBag.TipoTransaccion = Tranferecia.pasarAFormatoTipoPago();

                        int dia = int.Parse(Session["Dia"].ToString());
                        int mes = int.Parse(Session["Mes"].ToString());
                        int year = int.Parse(Session["Year"].ToString());
                        int idCalendario = int.Parse(Session["IDCalendario"].ToString());
                        string rut_aux = Session["Rut"].ToString();
                        string correo_aux = Session["Correo"].ToString();
                        string nombre_aux = Session["Nombre"].ToString();
                        string fecha = dia + "-" + mes + "-" + year;
                        ConsultasExternas.Correos.EnviarCorreoConfirmacionPagoDia(correo_aux, fecha, idCalendario, Tranferecia);


                        bool correcto = GuardarTransaccionConfirmadaClase(Tranferecia, rut);

                        int numeroIntentos = 0;
                        if (correcto == false)
                        {
                            numeroIntentos = 4;
                        }
                        for (int i = 0; i < numeroIntentos; i++)
                        {
                            correcto = GuardarTransaccionConfirmadaClase(Tranferecia, rut);
                            if (correcto == true)
                            {
                                i = numeroIntentos;
                            }
                        }
                        ////****/////

                        if (correcto)
                        {
                            ViewBag.Realizada = true;
                            /*
                            int dia = int.Parse(Session["Dia"].ToString());
                            int mes = int.Parse(Session["Mes"].ToString());
                            int year = int.Parse(Session["Year"].ToString());
                            int idCalendario = int.Parse(Session["IDCalendario"].ToString());
                            string rut_aux = Session["Rut"].ToString();
                            string correo_aux = Session["Correo"].ToString();
                            string nombre_aux = Session["Nombre"].ToString();
                            string fecha = dia + "-" + mes + "-" + year;
                            */
                            CrearReservaDia(dia, mes, year,idCalendario, rut_aux, correo_aux, nombre_aux);
                            //ConsultasExternas.Correos.EnviarCorreoConfirmacionPagoDia(correo_aux, fecha, idCalendario, Tranferecia);

                        }
                        else
                        {
                            ViewBag.Realizada = false;
                            ViewBag.Detalle = "Transferencia Realizada, pero no registrada en la base de datos, por favor contáctese con la administración";
                        }
                    }
                    else
                    {
                        ViewBag.Realizada = false;
                        ViewBag.Detalle = Tranferecia.getDetalleResponseCode();
                    }
                }
                else
                {
                    if (TBK_TOKEN != null)
                    {
                        ViewBag.Detalle = "Tranferencia Cancelada";
                        Models.Usuario UsuarioAux = RecuperarSession(TBK_ID_SESION);
                        Session["Usuario"] = UsuarioAux;
                    }
                    else
                    {
                        ViewBag.Detalle = "Tiempo de Espera Expirado";
                    }
                }

                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
                ViewBag.menu = MenuIzquierdo.ObtenerMenu();
                ViewBag.userName = Usuario.Rut;
                ViewBag.nombre = Usuario.NombreUsuario;
                return View();
            }
            catch (Exception exp)
            {
                ViewBag.Realizada = false;
                ViewBag.Detalle = exp.Message;
                return View();
            }
        }


        [HttpPost]
        public JsonResult CargarClase(int dia, int mes, int year)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if(DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerClases(DiaDeLaSemana);
                    if (Actividades.Correcto)
                    {
                        string datos_final = JsonConvert.SerializeObject(Actividades.Salida);
                        return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Con Procesar Fecha"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult CargarClaseHorario(int dia, int mes, int year, int idClase)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerHorarios(DiaDeLaSemana, idClase);
                    if (Actividades.Correcto)
                    {
                        string datos_final = JsonConvert.SerializeObject(Actividades.Salida);
                        return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Con Procesar Fecha"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult TieneCuposDisponibles(int dia, int mes, int year, int idCalendario)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);

                if (esFeriado(year, mes, dia))
                {
                    return Json(new { Message = JsonConvert.SerializeObject("FERIADO"), Correcto = true, JsonRequestBehavior.AllowGet });
                }

                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);

                ConsultasExternas.RespuestaSQL CuposMax = ConsultasExternas.ConsultasCalendario.CuposMaximosPorClase(idCalendario);
                // ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, fecha.ToString("yyyy-MM-dd"));
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, Models.NumerosFormato.ObtenerFecha(fecha));
                //ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(Usuario.Rut, fecha.ToString("yyyy-MM-dd"), idCalendario);
               ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(Usuario.Rut, Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                ConsultasExternas.RespuestaSQL EstadoMensualidad = MensualidadPagada(Usuario.Rut, dia, mes, year);
                //ConsultasExternas.RespuestaSQL EstadoCancelacion = ConsultasExternas.ConsultasCancelacionClases.estadoClase(fecha.ToString("yyyy-MM-dd"), idCalendario);
                ConsultasExternas.RespuestaSQL EstadoCancelacion = ConsultasExternas.ConsultasCancelacionClases.estadoClase(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                //consultar si no asisitio a una reserva pasada
                //bool AsisitioClasePasada = this.AsistioClasePasada(dia, mes, year, Usuario.Rut);
                //consultar si es socio

                ConsultasExternas.RespuestaSQL EstadoClase = ConsultasExternas.ConsultasCalendario.ObtenerDatosCalendario(idCalendario);

                //si todas las consultas son correctas
                if ((CuposMax.Correcto)&&(Actividades.Correcto)&&(CupoReservadoClaseMismoDia.Correcto)&&(EstadoMensualidad.Correcto)&&(EstadoCancelacion.Correcto)&&(EstadoClase.Correcto))
                {
                    Boolean MensualidadPagada = (Boolean)(EstadoMensualidad.Salida[0]);
                    int cantidadReservasUsuario = CupoReservadoClaseMismoDia.Salida.Length;
                    object[] resultadoCupoMax = (object[])CuposMax.Salida[0]; //obtengo la primera fila
                    object[] resultadosCupos = (object[])Actividades.Salida[0]; //obtengo la primera fila
                    int cantidadReservasActual = int.Parse(resultadosCupos[0].ToString()); //obtengo la primera celda 
                    int maximoReservas = int.Parse(resultadoCupoMax[0].ToString()); //obtengo la primera celda 
                    bool existeCupo = false;
                    bool yaReservoDia = false;
                    if((idCalendario == 97)|| (idCalendario == 98))
                    {
                        maximoReservas = 100000;
                    }

                    if (cantidadReservasUsuario > 0) { yaReservoDia = true; }

                    object[] clase_aux = (object[])EstadoClase.Salida[0];
                    string clase_string = clase_aux[1].ToString();

                    /*
                    if (cantidadReservasActual < maximoReservas) { existeCupo = true; }
                    */
                    int CuposExtraSocio = 0;
                    if(clase_string.Equals("Gimnasio"))
                    {
                        CuposExtraSocio = 5;
                    }

                    existeCupo = CuposDisponibles(cantidadReservasActual, maximoReservas, Usuario.Rut, CuposExtraSocio);

                    Object[] salida = new Object[5] { existeCupo, cantidadReservasActual, maximoReservas, MensualidadPagada, yaReservoDia };
                    string datos_final = JsonConvert.SerializeObject(salida);

                    bool estadoCancelacion = false;
                    if(EstadoCancelacion.Salida.Length > 0)
                    {
                        estadoCancelacion = true;
                    }

                    return Json(new { Message = datos_final,Cancelacion = estadoCancelacion,   Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if (CuposMax.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(CuposMax.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    if (Actividades.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    if (CupoReservadoClaseMismoDia.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(CupoReservadoClaseMismoDia.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    if (EstadoMensualidad.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(EstadoMensualidad.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult TieneCuposDisponiblesSinMensualidad(int dia, int mes, int year, int idCalendario)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);

                if (esFeriado(year, mes, dia))
                {
                    return Json(new { Message = JsonConvert.SerializeObject("FERIADO"), Correcto = true, JsonRequestBehavior.AllowGet });
                }


                ConsultasExternas.RespuestaSQL CuposMax = ConsultasExternas.ConsultasCalendario.CuposMaximosPorClase(idCalendario);
                // ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, fecha.ToString("yyyy-MM-dd"));
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, Models.NumerosFormato.ObtenerFecha(fecha));
                //ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(Usuario.Rut, fecha.ToString("yyyy-MM-dd"), idCalendario);
                ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(Usuario.Rut, Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                //ConsultasExternas.RespuestaSQL EstadoCancelacion = ConsultasExternas.ConsultasCancelacionClases.estadoClase(fecha.ToString("yyyy-MM-dd"), idCalendario);
                ConsultasExternas.RespuestaSQL EstadoCancelacion = ConsultasExternas.ConsultasCancelacionClases.estadoClase(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);

                //si todas las consultas son correctas
                if ((CuposMax.Correcto) && (Actividades.Correcto) && (CupoReservadoClaseMismoDia.Correcto) && (EstadoCancelacion.Correcto))
                {
                    int cantidadReservasUsuario = CupoReservadoClaseMismoDia.Salida.Length;
                    object[] resultadoCupoMax = (object[])CuposMax.Salida[0]; //obtengo la primera fila
                    object[] resultadosCupos = (object[])Actividades.Salida[0]; //obtengo la primera fila
                    int cantidadReservasActual = int.Parse(resultadosCupos[0].ToString()); //obtengo la primera celda 
                    int maximoReservas = int.Parse(resultadoCupoMax[0].ToString()); //obtengo la primera celda 
                    if ((idCalendario == 97) || (idCalendario == 98))
                    {
                        maximoReservas = 100000;
                    }
                    bool existeCupo = false;
                    bool yaReservoDia = false;
                    if (cantidadReservasUsuario > 0) { yaReservoDia = true; }
                    if (cantidadReservasActual < maximoReservas) { existeCupo = true; }
                    Object[] salida = new Object[5] { existeCupo, cantidadReservasActual, maximoReservas, true, yaReservoDia };
                    string datos_final = JsonConvert.SerializeObject(salida);

                    bool estadoCancelacion = false;
                    if (EstadoCancelacion.Salida.Length > 0)
                    {
                        estadoCancelacion = true;
                    }

                    return Json(new { Message = datos_final, Cancelacion = estadoCancelacion, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if (CuposMax.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(CuposMax.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    if (Actividades.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    if (CupoReservadoClaseMismoDia.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(CupoReservadoClaseMismoDia.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        [HttpPost]        
        public JsonResult BuscarNombreProfesor(int idCalendario)
        {
            try
            {
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaProfesores.ObtenerProfesor(idCalendario);
                if (Actividades.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject(Actividades.Salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = true, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        [HttpPost]
        public JsonResult CrearReserva(int dia, int mes, int year, int idCalendario)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL PuedeReservas = puedeReservar(dia, mes, year, idCalendario, Usuario.Rut);
                if (PuedeReservas.Correcto)
                {
                    ConsultasExternas.RespuestaSQL ingresarCupos = ConsultasExternas.ConsultasCalendario.CrearReserva(Usuario.Rut, Models.NumerosFormato.ObtenerFecha(fecha), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idCalendario);
                    if (ingresarCupos.Correcto)
                    {
                        ConsultasExternas.RespuestaSQL calendario = ConsultasExternas.ConsultasCalendario.ObtenerDatosCalendario(idCalendario);
                        if (calendario.Correcto)
                        {
                            if(calendario.Salida.Length> 0)
                            {
                                object[] arreglo = (object[])calendario.Salida[0];
                                string nombreClase = arreglo[1].ToString();
                                string horarioClase = arreglo[2].ToString();
                                string correo = Usuario.Correo;
                                string nombre = Usuario.NombreUsuario + " " + Usuario.ApellidoPaternoUsuario + " " + Usuario.ApellidoMaternoUsuario;
                                ConsultasExternas.Correos.EnviarCorreoConfirmacionReserva(correo, nombreClase, horarioClase, fecha, nombre);
                            }
                        }
                        
                        return Json(new { Message = JsonConvert.SerializeObject(true), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(ingresarCupos.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(PuedeReservas.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        [HttpPost]
        public JsonResult ComprobarMensualidad(int mes, int year)
        {
            try
            {
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL EstadoMensualidad = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(Usuario.Rut, mes, year);
                
                if (EstadoMensualidad.Correcto)
                {
                    Object[] respuestas = EstadoMensualidad.Salida;
                    if (respuestas.Length > 0)
                    {
                        object[] salida = new object[3] { false, "", "" };
                        string datos_final = JsonConvert.SerializeObject(salida);
                        return Json(new { Message = JsonConvert.SerializeObject(datos_final), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        EstadoMensualidad = MensualidadPagadaAnterior(Usuario.Rut, mes, year);
                        if (EstadoMensualidad.Correcto)
                        {
                            ConsultasExternas.RespuestaSQL PrecioMensualidad = ConsultasExternas.ConsultasMensualidades.PrecioMensualidad(year);
                            Boolean PagoInscripcion = false;
                            int dia_actual = DateTime.Now.Day;
                            int mes_actual = DateTime.Now.Month;
                            Boolean mesPasadoVencido = false;
                            if(mes_actual == mes)
                            {
                                if(dia_actual > 5)
                                {
                                    mesPasadoVencido = true;
                                }
                            }
                            if ((EstadoMensualidad.Salida.Length == 0)&&(mesPasadoVencido == true))
                            {
                                if(mes == DateTime.Now.Month)
                                {
                                    PrecioMensualidad = MensualidadProporcional();
                                }

                                PagoInscripcion = true;
                            }


                            if (PrecioMensualidad.Correcto)
                            {
                                ///esto no dberia ir aca
                                var buyOrder = mes + "_" + year + "_" + Usuario.Rut;
                                var sessionId = Usuario.Rut;
                                var amount = PrecioMensualidad.SalidaEntero;
                                if (PagoInscripcion)
                                {
                                    amount = amount + 3000;
                                }

                                string MensualidadPagada = Models.NumerosFormato.ObtenerNumeroFormato(PrecioMensualidad.SalidaEntero.ToString());
                                string total = Models.NumerosFormato.ObtenerNumeroFormato(amount.ToString());
                                /*fin de transbank*/
                                object[] salida = new object[6] { false, "", "", MensualidadPagada, PagoInscripcion, total };
                                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
                            }
                            else
                            {
                                return Json(new { Message = JsonConvert.SerializeObject(PrecioMensualidad.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                            }
                        }
                        else
                        {
                            return Json(new { Message = JsonConvert.SerializeObject(EstadoMensualidad.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                        }
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(EstadoMensualidad.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                object[] salida = new object[3] { exc.Message, "", ""};
                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        [HttpPost]
        public JsonResult CrearEnlaceMensualidad(int mes, int year)
        {
            try
            {
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL EstadoMensualidad = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(Usuario.Rut, mes, year);
                if (EstadoMensualidad.Correcto)
                {
                    Object[] respuestas = EstadoMensualidad.Salida;
                    if (respuestas.Length > 0)//si esta pagado
                    {
                        object[] salida = new object[3] { false, "", "" };
                        string datos_final = JsonConvert.SerializeObject(salida);
                        return Json(new { Message = JsonConvert.SerializeObject(datos_final), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        EstadoMensualidad = MensualidadPagadaAnterior(Usuario.Rut, mes, year);
                        if (EstadoMensualidad.Correcto)
                        {
                            ConsultasExternas.RespuestaSQL PrecioMensualidad = ConsultasExternas.ConsultasMensualidades.PrecioMensualidad(year);
                            Boolean PagoInscripcion = false;
                            int dia_actual = DateTime.Now.Day;
                            int mes_actual = DateTime.Now.Month;
                            Boolean mesPasadoVencido = false;
                            if (mes_actual == mes)
                            {
                                if (dia_actual > 5)
                                {
                                    mesPasadoVencido = true;
                                }
                            }
                            if ((EstadoMensualidad.Salida.Length == 0) && (mesPasadoVencido == true))
                            {
                                if (mes == DateTime.Now.Month)
                                {
                                    PrecioMensualidad = MensualidadProporcional();
                                }

                                PagoInscripcion = true;
                            }
                            if (PrecioMensualidad.Correcto)
                            {
                                var buyOrder = mes + "_" + year + "_" + Usuario.Rut;
                                var sessionId = Usuario.Rut;
                                var amount = PrecioMensualidad.SalidaEntero;
                                if (PagoInscripcion)
                                {
                                    amount = amount + 3000;
                                }
                                var urlHelper = new UrlHelper(ControllerContext.RequestContext);
                                var returnUrl = urlHelper.Action("returnPagarMensualidad", "Reservas", null, Request.Url.Scheme);
                                //datos de la empresa
                                //Transaction Transaction = new Transaction(new Options("Consultar codigos Transbanck empresa", "Consultar codigos Transbanck empresa", WebpayIntegrationType.Live));
                                Transaction Transaction = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));


                                var result = Transaction.Create(buyOrder, sessionId, amount, returnUrl);
                                var action = result.Url;
                                var token = result.Token;
                                string MensualidadPagada = Models.NumerosFormato.ObtenerNumeroFormato(PrecioMensualidad.SalidaEntero.ToString());
                                string total = Models.NumerosFormato.ObtenerNumeroFormato(amount.ToString());
                                object[] salida = new object[6] { false, action, token, MensualidadPagada, PagoInscripcion, total };
                                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
                            }
                            else
                            {
                                return Json(new { Message = JsonConvert.SerializeObject(PrecioMensualidad.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                            }
                        }
                        else
                        {
                            return Json(new { Message = JsonConvert.SerializeObject(EstadoMensualidad.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                        }
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(EstadoMensualidad.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                object[] salida = new object[3] { exc.Message, "", "" };
                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult CrearEnlaceMensualidadClase(int mes, int year, int dia, int idCalendario, string rut, string correo, string nombre, string apellidoPaterno, string apellidoMaterno)
        {
            try
            {
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);

                var buyOrder = dia+"_"+mes + "_" + year + "_" + rut + "_"+(new Random().Next(10).ToString());
                var sessionId = Usuario.Rut;
                var amount = 3000;
                
                var urlHelper = new UrlHelper(ControllerContext.RequestContext);
                var returnUrl = urlHelper.Action("returnPagarMensualidadClase", "Reservas", null, Request.Url.Scheme);

                //Transaction Transaction = new Transaction(new Options("Consultar codigos Transbanck empresa", "Consultar codigos Transbanck empresa", WebpayIntegrationType.Live));
                Transaction Transaction = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));

                var result = Transaction.Create(buyOrder, rut, amount, returnUrl);
                var action = result.Url;
                var token = result.Token;
                string MensualidadPagada = Models.NumerosFormato.ObtenerNumeroFormato(3000.ToString());
                string total = Models.NumerosFormato.ObtenerNumeroFormato(amount.ToString());
                object[] salida = new object[6] { false, action, token, MensualidadPagada, 3000, total };

                Session["Dia"] = dia;
                Session["Mes"] = mes;
                Session["Year"] = year;
                Session["IDCalendario"] = idCalendario;
                Session["Rut"] = rut;
                Session["Correo"] = correo;
                Session["Nombre"] = nombre+" "+apellidoPaterno+" "+apellidoMaterno;

                if(ConsultasExternas.Consultas.DatosUsuarios(rut).Rows.Count == 0)
                {
                    //creo usuario invitado
                    ConsultasExternas.Consultas.CrearUsuarioInvitado(rut, nombre, apellidoPaterno, apellidoMaterno, "000", "000", correo, "Invitado2022", "");
                }

                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
                
            }
            catch (Exception exc)
            {
                object[] salida = new object[3] { exc.Message, "", "" };
                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult BuscarReservas(int dia, int mes, int year)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerReservas(Models.NumerosFormato.ObtenerFecha(fecha), Usuario.Rut);
                    if (Actividades.Correcto)
                    {
                        string datos_final = JsonConvert.SerializeObject(Actividades.Salida);
                        return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }

                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Con Procesar Fecha"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        


        [HttpPost]
        public JsonResult CancelarReserva(int idReserva)
        {
            try
            {
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.EliminarReserva(idReserva, Usuario.Rut);
                if (Actividades.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject(Actividades.Salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        public Boolean CrearReservaDia(int dia, int mes, int year, int idCalendario, string userName, string correo, string nombreCompleto)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(userName, Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);

                if(CupoReservadoClaseMismoDia.Salida.Length == 0)
                {
                    ConsultasExternas.RespuestaSQL ingresarCupos = ConsultasExternas.ConsultasCalendario.CrearReserva(userName, Models.NumerosFormato.ObtenerFecha(fecha), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idCalendario);
                    if (ingresarCupos.Correcto)
                    {
                        ConsultasExternas.RespuestaSQL calendario = ConsultasExternas.ConsultasCalendario.ObtenerDatosCalendario(idCalendario);
                        if (calendario.Correcto)
                        {
                            if (calendario.Salida.Length > 0)
                            {
                                object[] arreglo = (object[])calendario.Salida[0];
                                string nombreClase = arreglo[1].ToString();
                                string horarioClase = arreglo[2].ToString();
                                //ConsultasExternas.Correos.EnviarCorreoConfirmacionReserva(correo, nombreClase, horarioClase, fecha, nombreCompleto);
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    ConsultasExternas.RespuestaSQL calendario = ConsultasExternas.ConsultasCalendario.ObtenerDatosCalendario(idCalendario);
                    if (calendario.Correcto)
                    {
                        if (calendario.Salida.Length > 0)
                        {
                            object[] arreglo = (object[])calendario.Salida[0];
                            string nombreClase = arreglo[1].ToString();
                            string horarioClase = arreglo[2].ToString();
                            ConsultasExternas.Correos.EnviarCorreoConfirmacionReserva(correo, nombreClase, horarioClase, fecha, nombreCompleto);
                        }
                    }
                    return true;
                }
               
            }
            catch (Exception exc)
            {
                string borrar_ = exc.Message;
                return false;
            }
        }


        private ConsultasExternas.RespuestaSQL MensualidadProporcional()
        {
            int dia = DateTime.Now.Day;
            return ConsultasExternas.ConsultasMensualidades.PrecioMensualidadProporcional(dia);
        }



        private ConsultasExternas.RespuestaSQL MensualidadPagadaAnterior(string usuario, int mes, int year)
        {
            int mesPasado = mes - 1;
            int yearPasado = year;
            if (mesPasado == 0)
            {
                mesPasado = 12;
                yearPasado = yearPasado - 1;
            }

            return ConsultasExternas.ConsultasMensualidades.MensualidadPagada(usuario, mesPasado, yearPasado);
        }



        private ConsultasExternas.RespuestaSQL MensualidadPagada(string usuario, int dia, int mes, int year)
        {
            ConsultasExternas.RespuestaSQL esSocio = ConsultasExternas.ConsultasMensualidades.esSocio(usuario);
            if (esSocio.Correcto)
            {
                if (esSocio.Salida.Length > 0) 
                {
                    return new ConsultasExternas.RespuestaSQL(new object[1] { true });
                }
            }

            ConsultasExternas.RespuestaSQL respuestaSQL =  ConsultasExternas.ConsultasMensualidades.MensualidadPagada(usuario, mes, year);
            if (respuestaSQL.Correcto)
            {
                Object[] respuestas = respuestaSQL.Salida;
                if (respuestas.Length > 0) 
                {
                    return new ConsultasExternas.RespuestaSQL(new object[1] { true });
                }
                else
                {
                    if (Models.NumerosFormato.esPrimerDiaHabildelMes( dia, mes,year)) //hasta el primer dia habil del mes ()
                    {
                        mes = mes - 1;
                        if(mes == 0){
                            mes = 12;
                            year = year - 1;
                        }
                        respuestaSQL = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(usuario, mes, year);
                        if (respuestaSQL.Correcto)
                        {
                            respuestas = respuestaSQL.Salida;
                            if (respuestas.Length > 0) 
                            {
                                return new ConsultasExternas.RespuestaSQL(new object[1] { true });
                            }
                            return new ConsultasExternas.RespuestaSQL(new object[1] { false });
                        }
                        else
                        {
                            return new ConsultasExternas.RespuestaSQL("Error interno en consulta Base De Datos");
                        }
                            
                    }
                }
            }
            else
            {
                return new ConsultasExternas.RespuestaSQL("Error interno en consulta Base De Datos");
            }
            return new ConsultasExternas.RespuestaSQL(new object[1] { false });
        }


        private Boolean GuardarTransaccionConfirmada(Models.Tranferencia tran, string usuario)
        {
            string fecha = Models.NumerosFormato.ObtenerFechaCompleta(tran.getTransactionDateHorarioChile(4));
            ConsultasExternas.RespuestaSQL respuesta =  ConsultasExternas.ConsultasMensualidades.IngresarTransaccion(usuario,tran.getBuyOrder(), tran.getCardNumber(), tran.getMonto(), tran.getInstallmentsNumber(),
                tran.getInstallmentsAmount(), tran.getAuthorizationCode(), tran.getAccountingDate(), fecha, tran.getPaymentTypeCode(), tran.getVci(), tran.getStatus(), tran.Token_WS);
            if (respuesta.Correcto)
            {
                respuesta = ConsultasExternas.ConsultasMensualidades.ObtenerIDTransaccion(usuario, tran.Token_WS, tran.getBuyOrder());
                if (respuesta.Correcto)
                {
                    Object[] id_array = respuesta.Salida;
                    if(id_array.Length > 0)
                    {
                        string valor = id_array[0].ToString();
                        int idTranferencia = int.Parse(valor);
                        int mes = tran.ObtenerMesMensualidad();
                        int year = tran.ObtenerYearMensualidad();
                        respuesta = ConsultasExternas.ConsultasMensualidades.IngresarMensualidad(usuario, mes, year, idTranferencia);
                        if (respuesta.Correcto){
                            return true;
                        }
                    }
                }
            }
            ConsultasExternas.Correos.EnviarCorreoError(usuario, tran.getBuyOrder(), tran.getCardNumber(), tran.getMonto(), tran.getInstallmentsNumber(),
                tran.getInstallmentsAmount(), tran.getAuthorizationCode(), tran.getAccountingDate(), fecha, tran.getPaymentTypeCode(), tran.getVci(), tran.getStatus(), tran.Token_WS);
            return false;
        }


        private Boolean GuardarTransaccionConfirmadaClase(Models.Tranferencia tran, string usuario)
        {

            string fecha = Models.NumerosFormato.ObtenerFechaCompleta(tran.getTransactionDateHorarioChile(4));


            if(ConsultasExternas.ConsultasMensualidades.ObtenerTransaccionRealizada(usuario, tran.getBuyOrder(), tran.getCardNumber(), tran.getMonto(), tran.getInstallmentsNumber(),
                tran.getInstallmentsAmount(), tran.getAuthorizationCode(), tran.getAccountingDate(), fecha, tran.getPaymentTypeCode(), tran.getVci(), tran.getStatus(), tran.Token_WS) == false)
            {
                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.ConsultasMensualidades.IngresarTransaccion(usuario, tran.getBuyOrder(), tran.getCardNumber(), tran.getMonto(), tran.getInstallmentsNumber(),
               tran.getInstallmentsAmount(), tran.getAuthorizationCode(), tran.getAccountingDate(), fecha, tran.getPaymentTypeCode(), tran.getVci(), tran.getStatus(), tran.Token_WS);
                if (respuesta.Correcto)
                {
                    return true;
                }
                ConsultasExternas.Correos.EnviarCorreoError(usuario, tran.getBuyOrder(), tran.getCardNumber(), tran.getMonto(), tran.getInstallmentsNumber(),
                   tran.getInstallmentsAmount(), tran.getAuthorizationCode(), tran.getAccountingDate(), fecha, tran.getPaymentTypeCode(), tran.getVci(), tran.getStatus(), tran.Token_WS);
                return false;
            }
            else
            {
                return true; 
            }

           
        }


       

        private int DiaDeLaSemana(DateTime fecha)
        {
            if (fecha.DayOfWeek == DayOfWeek.Monday){
                return 1;
            }
            if (fecha.DayOfWeek == DayOfWeek.Tuesday)
            {
                return 2;
            }
            if (fecha.DayOfWeek == DayOfWeek.Wednesday)
            {
                return 3;
            }
            if (fecha.DayOfWeek == DayOfWeek.Thursday)
            {
                return 4;
            }
            if (fecha.DayOfWeek == DayOfWeek.Friday)
            {
                return 5;
            }
            if (fecha.DayOfWeek == DayOfWeek.Saturday)
            {
                return 6;
            }
            if (fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                return 7;
            }
            return -1;
        }


        private Models.Usuario RecuperarSession(string userName)
        {
            try
            {
                DataTable dt = ConsultasExternas.Consultas.DatosUsuarios(userName);
                if (dt.TableName.Equals("ERROR") == false)
                {
                    //nombre es clave y debe existir un solo nombre
                    if (dt.Rows.Count == 1)
                    {
                        string nombre = dt.Rows[0]["Nombre"].ToString();
                        string apPaterno = dt.Rows[0]["ApellidoPaterno"].ToString();
                        string apMaterno = dt.Rows[0]["ApellidoMaterno"].ToString();
                        string rut = dt.Rows[0]["Rut"].ToString();
                        string correo = dt.Rows[0]["Email"].ToString();
                        string telefono = dt.Rows[0]["NTelefono"].ToString();


                        Models.Usuario Usuario = new Models.Usuario(rut, nombre, apPaterno, apMaterno, correo, telefono);
                        DataTable dt_menu = ConsultasExternas.Consultas.ObtenerMenu(userName);
                        Usuario.SetMenu(dt_menu);
                         return Usuario;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        private ConsultasExternas.RespuestaSQL puedeReservar(int dia, int mes, int year, int idCalendario, string rut)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);

                ConsultasExternas.RespuestaSQL CuposMax = ConsultasExternas.ConsultasCalendario.CuposMaximosPorClase(idCalendario);
                //ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, fecha.ToString("yyyy-MM-dd"));
                //ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(rut, fecha.ToString("yyyy-MM-dd"), idCalendario);
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, Models.NumerosFormato.ObtenerFecha(fecha));
                ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(rut, Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                ConsultasExternas.RespuestaSQL EstadoMensualidad = MensualidadPagada(rut, dia, mes, year);
                ConsultasExternas.RespuestaSQL EstadoClase = ConsultasExternas.ConsultasCalendario.ObtenerDatosCalendario(idCalendario);


                if ((CuposMax.Correcto) && (Actividades.Correcto) && (CupoReservadoClaseMismoDia.Correcto) && (EstadoMensualidad.Correcto) && (EstadoClase.Correcto))
                {
                    Boolean MensualidadPagada = (Boolean)(EstadoMensualidad.Salida[0]);
                    int cantidadReservasUsuario = CupoReservadoClaseMismoDia.Salida.Length;
                    object[] resultadoCupoMax = (object[])CuposMax.Salida[0]; //obtengo la primera fila
                    object[] resultadosCupos = (object[])Actividades.Salida[0]; //obtengo la primera fila
                    int cantidadReservasActual = int.Parse(resultadosCupos[0].ToString()); //obtengo la primera celda 
                    int maximoReservas = int.Parse(resultadoCupoMax[0].ToString()); //obtengo la primera celda 
                    bool existeCupo = false;
                    bool yaReservoDia = false;
                    if (cantidadReservasUsuario > 0){ yaReservoDia = true;  }
                    // if (cantidadReservasActual < maximoReservas) { existeCupo = true; }

                    object[] clase_aux = (object[])EstadoClase.Salida[0];
                    string clase_string = clase_aux[1].ToString();

                    int CuposExtraSocio = 0;
                    if (clase_string.Equals("Gimnasio"))
                    {
                        CuposExtraSocio = 5;
                    }


                    existeCupo = CuposDisponibles(cantidadReservasActual, maximoReservas, rut, CuposExtraSocio);
                    if (MensualidadPagada == true)
                    {
                        if(yaReservoDia == false)
                        {
                            if(existeCupo == true)
                            {
                                return new ConsultasExternas.RespuestaSQL(new object[1] {true });
                            }
                        }
                    }
                    return new ConsultasExternas.RespuestaSQL(new object[0] { }); ;
                }
                else
                {
                    if (CuposMax.Correcto == false)
                    {
                        return CuposMax;
                    }
                    if (Actividades.Correcto == false)
                    {
                        return Actividades;
                    }
                    if (CupoReservadoClaseMismoDia.Correcto == false)
                    {
                        return CupoReservadoClaseMismoDia;
                    }

                    return EstadoMensualidad;
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        private bool AsistioClasePasada(int dia, int mes, int year, string rut)
        {
            DateTime fecha = new DateTime(year, mes, dia);
            fecha = fecha.AddDays(-1);
            if(fecha.DayOfWeek == DayOfWeek.Sunday) //si el dia anterior es domingo, resto un nuevo dia para obtener el sabado
            {
                fecha = fecha.AddDays(-1);
            }

            ConsultasExternas.RespuestaSQL respuesa =  ConsultasExternas.ConsultasCalendario.AsistioClaseAnterior(Models.NumerosFormato.ObtenerFecha(fecha), rut);

            if (respuesa.Correcto)
            {
                return respuesa.Booleano;
            }

            return true;
        }
       


        private bool CuposDisponibles(int cuposActuales, int cuposMaximos, string usuario, int MaxSocio)
        {

            bool existeCupo = false;
            if (cuposActuales < cuposMaximos) 
            { 
                existeCupo = true;  //existen cuppos
            }
            else
            {
                if(cuposActuales < cuposMaximos + MaxSocio)
                {
                    ConsultasExternas.RespuestaSQL esSocio = ConsultasExternas.ConsultasMensualidades.esSocio(usuario);
                    if (esSocio.Correcto) 
                    {
                        if (esSocio.Salida.Length > 0) 
                        {
                            existeCupo = true;
                        }
                    }
                    
                }
            }

            return existeCupo;
        }


        private Boolean esFeriado(int year, int mes, int dia)
        {

            Boolean esFeriado = false;
            ArrayList feriados = new ArrayList();


            feriados.Add(new DateTime(2025, 1, 1));
            feriados.Add(new DateTime(2025, 4, 18));
            feriados.Add(new DateTime(2025, 4, 19));
            feriados.Add(new DateTime(2025, 5, 1));
            feriados.Add(new DateTime(2025, 5, 21));
            feriados.Add(new DateTime(2025, 6, 20));
            ///
            feriados.Add(new DateTime(2025, 7, 16));
            feriados.Add(new DateTime(2025, 8, 15));
            feriados.Add(new DateTime(2025, 9, 18));
            feriados.Add(new DateTime(2025, 9, 19));
            feriados.Add(new DateTime(2025, 10, 12));
            feriados.Add(new DateTime(2025, 10, 31));
            feriados.Add(new DateTime(2025, 11, 1));
            feriados.Add(new DateTime(2025, 11, 16));
            feriados.Add(new DateTime(2025, 12, 8));
            feriados.Add(new DateTime(2025, 12, 25));
            feriados.Add(new DateTime(2026, 1, 1));




            //fin derinicion de feriados
            DateTime fechaConsulta = new DateTime(year, mes, dia);

            if (fechaConsulta.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            esFeriado = feriados.Contains(fechaConsulta);

            return esFeriado;
        }

     



    }
}