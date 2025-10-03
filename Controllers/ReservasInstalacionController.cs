using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;
using WebApplicationTemplateBase.Filters;

namespace WebApplicationTemplateBase.Controllers
{
    public class ReservasInstalacionController : Controller
    {



       

        [AutorizarUsuario(nombreMetodo: "25")]
        public ActionResult ReservarInstalacion()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            /*propios de este metodo*/
            ViewBag.nombreCompleto = Usuario.NombreUsuario + " " + Usuario.ApellidoPaternoUsuario + " " + Usuario.ApellidoMaternoUsuario;
            DateTime fecha_actual = DateTime.Now;
            ViewBag.fechaCompleta = fecha_actual.ToString("dd") + " de " + fecha_actual.ToString("MMMM") + " de " + fecha_actual.ToString("yyyy");
            return View();
        }



        public ActionResult ReservarInstalacionInvitado()
        {
            ViewBag.menu = new Dictionary<string, Object[]>();
            ViewBag.userName = "Invitado";
            ViewBag.nombre = "Invitado";
            DateTime fecha_actual = DateTime.Now;
            ViewBag.fechaCompleta = fecha_actual.ToString("dd") + " de " + fecha_actual.ToString("MMMM") + " de " + fecha_actual.ToString("yyyy");
            return View();
        }


        public ActionResult returnPagarArriendoInstalacion(String token_ws, String TBK_ID_SESION, String TBK_ORDEN_COMPRA, String TBK_TOKEN)
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

                        string dia = Session["Dia"].ToString();
                        string mes = Session["Mes"].ToString();
                        string year = Session["Year"].ToString();
                        int idCalendario = int.Parse(Session["IDCalendario"].ToString());
                        string requerimientos = Session["Requerimientos"].ToString();
                        string llegada = Session["Llegada"].ToString();
                        HttpPostedFileBase Asistentes = (HttpPostedFileBase)(Session["Asistentes"]);
                        string fecha = dia + "/" + mes + "/" + year;
                        ViewBag.Realizada = true;

                        ConsultasExternas.Correos.EnviarCorreoConfirmacionPagoArriendoInstalacion(correo, fecha, idCalendario, Tranferecia);
                       
                        bool correcto = GuardarTransaccionConfirmada(Tranferecia, rut);

                        int numeroIntentos = 0;
                        if (correcto == false)
                        {
                            numeroIntentos = 3;
                        }
                        for (int i = 0; i < numeroIntentos; i++)
                        {
                            correcto = GuardarTransaccionConfirmada(Tranferecia, rut);
                            if (correcto == true)
                            {
                                i = numeroIntentos;
                            }
                        }
                        ////****/////
                        if (correcto)
                        {
                            /* esto lo move arriba
                            string dia = Session["Dia"].ToString();
                            string mes = Session["Mes"].ToString();
                            string year = Session["Year"].ToString();
                            int idCalendario = int.Parse(Session["IDCalendario"].ToString());
                            string requerimientos = Session["Requerimientos"].ToString();
                            string llegada = Session["Llegada"].ToString();
                            HttpPostedFileBase Asistentes = (HttpPostedFileBase)(Session["Asistentes"]);
                            string fecha = dia + "/" + mes + "/" + year;
                            ViewBag.Realizada = true;
                            */

                            GuardarReservaInstalacionInterno(int.Parse(dia), int.Parse(mes), int.Parse(year), idCalendario, requerimientos, llegada);
                            int idReserva = ObtenerIDReserva(idCalendario, int.Parse(dia), int.Parse(mes), int.Parse(year));
                            if(SubirCertificado(Asistentes, idReserva))
                            {
                                string nameCertificado = nombreCertificado(Asistentes, idReserva.ToString());
                                ConsultasExternas.ConsultaInstalaciones.ActualizarAsistentes(nameCertificado, idReserva.ToString());
                            }
                            
                            //esto lo cambie arriba
                            //ConsultasExternas.Correos.EnviarCorreoConfirmacionPagoArriendoInstalacion(correo, fecha, idCalendario, Tranferecia);
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



   


        public ActionResult MisReservas()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;

            ViewBag.esSocio = false;
            ConsultasExternas.RespuestaSQL esSocio =  ConsultasExternas.ConsultasMensualidades.esSocio(Usuario.Rut);
            if(esSocio.Correcto == true)
            {
                if(esSocio.Salida.Length > 0)
                {
                    ViewBag.esSocio = true;
                }
            }

            return View();
        }


        [HttpPost]
        public JsonResult CargarTipoInstalacion(int dia, int mes, int year)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerTipoInstalacionDisponible(DiaDeLaSemana);
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
        public JsonResult CargarInstalacion(int dia, int mes, int year, int TipoInstalacion)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerInstalacionDisponibleSoloClientes(DiaDeLaSemana, TipoInstalacion);
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
        public JsonResult CargarInstalacionHorario(int dia, int mes, int year, int idClase)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);

              
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerHorariosUsuarios(DiaDeLaSemana, idClase);
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
        public JsonResult TieneCuposDisponibles(int dia, int mes, int year, int idCalendario, int idInstalacion)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);


                if(esFeriado(year, mes, dia, idInstalacion))
                {
                    return Json(new { Message = JsonConvert.SerializeObject("FERIADO"), Correcto = true,  JsonRequestBehavior.AllowGet });
                }


                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                Boolean cupoInstalacionCompartido = true;

                /*preguntar por el estado de la intalacion si existe alguna cancelacion*/
                ConsultasExternas.RespuestaSQL cancelacion = ConsultasExternas.ConsultaInstalaciones.estadoInstalacionCancelacion(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                
                if (cancelacion.Correcto)
                {
                    if (cancelacion.Salida.Length > 0)
                    {
                        return Json(new { Message = false, Monto = 0, Cancelacion = true, Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        ConsultasExternas.RespuestaSQL cancelacionDia = ConsultasExternas.ConsultaInstalaciones.estadoInstalacionCancelacionDia(Models.NumerosFormato.ObtenerFecha(fecha));
                        if (cancelacionDia.Correcto)
                        {
                            if (cancelacionDia.Salida.Length > 0)
                            {
                                return Json(new { Message = false, Monto = 0, Cancelacion = true, Correcto = true, JsonRequestBehavior.AllowGet });
                            }
                        }
                        else
                        {
                            return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                        }
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                }



                ConsultasExternas.RespuestaSQL instalacionCompartida = ConsultasExternas.ConsultaInstalaciones.idInstalacionCompartida(idInstalacion);
                ConsultasExternas.RespuestaSQL cupos = ConsultasExternas.ConsultaInstalaciones.CupoDisponible(idCalendario, Models.NumerosFormato.ObtenerFecha(fecha));
                ConsultasExternas.RespuestaSQL precio = ConsultasExternas.ConsultaInstalaciones.ObtenerPrecioInstalacion(idInstalacion);
                ConsultasExternas.RespuestaSQL exigirAsistencia = ConsultasExternas.ConsultaInstalaciones.ObtenerExigenciaAsistentes(idInstalacion);

                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL estadoSocio = ConsultasExternas.ConsultasMensualidades.esSocio(Usuario.Rut);

                int idInstalacionCompartida = 0;
                if(instalacionCompartida.Correcto)
                {
                    idInstalacionCompartida = instalacionCompartida.SalidaEntero;
                    if(idInstalacionCompartida > 0)
                    {

                        ConsultasExternas.RespuestaSQL cuposCompartidos = ConsultasExternas.ConsultaInstalaciones.CupoDisponible(idInstalacionCompartida, DiaDeLaSemana, idCalendario, Models.NumerosFormato.ObtenerFecha(fecha));
                        if (cuposCompartidos.Correcto)
                        {
                            cupoInstalacionCompartido = cuposCompartidos.Booleano;
                        }
                        else
                        {
                            return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                        }
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                }

                if ((cupos.Correcto) && (precio.Correcto) &&(estadoSocio.Correcto))
                {
                    string monto = "";
                    if (precio.Salida.Length > 0) {
                        object[] aux = (object[])precio.Salida[0];
                        monto = aux[2].ToString();
                    }
                    bool existeCupo = cupos.Booleano;
                    if (monto.Equals("")) {
                        existeCupo = false;
                    }
                    if(cupoInstalacionCompartido == false){
                        existeCupo = false;
                    }
                    if (estadoSocio.Salida.Length > 0){
                        monto = "0";
                        int monto_aux = ObtenerPrecioPadelSocio(idInstalacion, idCalendario);
                        monto = monto_aux.ToString();
                        

                    }
                    bool exigencia_aux = false;
                    if (exigirAsistencia.Correcto)
                    {
                        exigencia_aux = exigirAsistencia.Booleano;
                    }
                    return Json(new { Message = existeCupo, Monto = monto, Cancelacion = false, Correcto = true, Exigencia = exigencia_aux, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult CrearEnlaceArriendo(string dia, string mes, string year, int idCalendario, int idInstalacion, string Requerimientos, HttpPostedFileBase Asistentes, string Llegada)
        {
            try
            {
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL estadoSocio = ConsultasExternas.ConsultasMensualidades.esSocio(Usuario.Rut);
                ConsultasExternas.RespuestaSQL ConsultaMontoArriendo = ConsultasExternas.ConsultaInstalaciones.ObtenerPrecioInstalacionInt(idInstalacion);

                if((idInstalacion == 47) || (idInstalacion == 48) || (idInstalacion == 49) || (idInstalacion == 50))
                {
                    if(estadoSocio.Correcto == true)
                    {
                        if (estadoSocio.Salida.Length > 0)
                        {
                            estadoSocio.Salida = new object[0];
                            if (ConsultaMontoArriendo.Correcto == true)
                            {
                                ConsultaMontoArriendo.SalidaEntero = ObtenerPrecioPadelSocio(idInstalacion, idCalendario);
                            }
                        }
                    }
                }

                if ((estadoSocio.Correcto) && (ConsultaMontoArriendo.Correcto))
                {
                    if (estadoSocio.Salida.Length > 0)
                    {
                        string fechaArrienddo = dia + "-" + mes + "-" + year;
                        object[] salida = new object[4] { "ajaxGuardarReservaInstalacion(" + dia + "," + mes + "," + year + "," + idCalendario + ", '"+ Requerimientos + "','"+ Llegada + "')", "", "", fechaArrienddo };
                        return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, Socio = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        int montoArriendo = ConsultaMontoArriendo.SalidaEntero;
                        //Transaction Transaction = new Transaction(new Options("Consultar Codigo Transbanck empresa", "Consultar Codigo Transbanck empresa", WebpayIntegrationType.Live));
                        Transaction Transaction = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));

                        var urlHelper = new UrlHelper(ControllerContext.RequestContext);
                        var returnUrl = urlHelper.Action("returnPagarArriendoInstalacion", "ReservasInstalacion", null, Request.Url.Scheme); //modificar url derespuesta
                        var buyOrder = dia + "_" + mes + "_" + year  + "/" + idCalendario + "/" + Usuario.Rut;  ///aca debo mejorar
                        var sessionId = Usuario.Rut;
                        var amount = montoArriendo;
                        var result = Transaction.Create(buyOrder, sessionId, amount, returnUrl);
                        var action = result.Url;
                        var token = result.Token;
                        string total = Models.NumerosFormato.ObtenerNumeroFormato(amount.ToString());

                        Session["Dia"] = dia;
                        Session["Mes"] = mes;
                        Session["Year"] = year;
                        Session["IDCalendario"] = idCalendario;
                        Session["Requerimientos"] = Requerimientos;
                        Session["Asistentes"] = Asistentes;
                        Session["Llegada"] = Llegada;

                        /*Fin*/
                        string fechaArrienddo = dia + "-" + mes + "-" + year;
                        object[] salida = new object[4] { action, token, total, fechaArrienddo };
                        return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, Socio = false, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
            }
        }


      
        [HttpPost]
        public JsonResult CrearEnlaceArriendoInvitado(string dia, string mes, string year, int idCalendario, int idInstalacion, string rutUsuario, string Comentarios, string Nombre, string App, string Apm, string Correo, string Telefono)
        {
            try
            {
                //Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL estadoSocio = ConsultasExternas.ConsultasMensualidades.esSocio(rutUsuario);
                ConsultasExternas.RespuestaSQL ConsultaMontoArriendo = ConsultasExternas.ConsultaInstalaciones.ObtenerPrecioInstalacionInt(idInstalacion);

                if ((estadoSocio.Correcto) && (ConsultaMontoArriendo.Correcto))
                {
                    if (estadoSocio.Salida.Length > 0)
                    {
                        string fechaArrienddo = dia + "-" + mes + "-" + year;
                        string nombreCompleto = Nombre + " " + App + " " + Apm;
                        object[] salida = new object[4] { "ajaxGuardarReservaInstalacion(" + dia + "," + mes + "," + year + "," + idCalendario + ",'"+ rutUsuario +"','"+ Comentarios + "','"+nombreCompleto+ "','" + Correo + "'   )", "", "", fechaArrienddo };
                        return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, Socio = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {

                        if (ConsultasExternas.Consultas.DatosUsuarios(rutUsuario).Rows.Count == 0)
                        {
                            ConsultasExternas.Consultas.CrearUsuarioInvitado(rutUsuario, Nombre, App, Apm, Telefono, "000", Correo, "Invitado2022", "");
                        }

                        int montoArriendo = ConsultaMontoArriendo.SalidaEntero;
                        //Transaction Transaction = new Transaction(new Options("Consultar Codigo Transbanck empresa", "Consultar Codigo Transbanck empresa", WebpayIntegrationType.Live));
                        Transaction Transaction = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));

                        var urlHelper = new UrlHelper(ControllerContext.RequestContext);
                        var returnUrl = urlHelper.Action("returnPagarArriendoInstalacionInvitado", "ReservasInstalacion", null, Request.Url.Scheme); //modificar url derespuesta
                        var buyOrder = dia + "_" + mes + "_" + year + "/" + idCalendario + "/" + rutUsuario;  ///aca debo mejorar
                        var sessionId = rutUsuario;
                        var amount = montoArriendo;
                        var result = Transaction.Create(buyOrder, sessionId, amount, returnUrl);
                        var action = result.Url;
                        var token = result.Token;
                        string total = Models.NumerosFormato.ObtenerNumeroFormato(amount.ToString());
                        Session["Dia"] = dia;
                        Session["Mes"] = mes;
                        Session["Year"] = year;
                        Session["IDCalendario"] = idCalendario;
                        Session["Rut"] = rutUsuario;
                        Session["Correo"] = Correo;
                        Session["Requerimientos"] = Comentarios;
                        Session["Nombre"] = Nombre;
                        Session["APP"] = App;
                        Session["APM"] = Apm;
                        string fechaArrienddo = dia + "-" + mes + "-" + year;
                        object[] salida = new object[4] { action, token, total, fechaArrienddo };
                        return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, Socio = false, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult CrearEnlaceArriendoPadel(string dia, string mes, string year, int idCalendario, int idInstalacion, string Requerimientos, HttpPostedFileBase Asistentes, string Llegada, string AsistentesPadel)
        {
            try
            {

                int precio_socio_aux = 2000;
                int precio_no_socio_aux = 4000;
                if((idInstalacion == 49)||(idInstalacion == 50))
                {
                    precio_socio_aux = 4000;
                    precio_no_socio_aux = 8000;
                }

                string[] lines = new string[0];
                if (AsistentesPadel.Length > 0)
                {
                    lines = AsistentesPadel.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }
                
                int cantidad_asistentes = 0;
                int totalCancha = 0;

               
                for(int i=0; i < lines.Length; i++)
                {
                    string usuario_aux = lines[i].ToString();
                    string rut_aux = usuario_aux.Substring(1, usuario_aux.IndexOf(')')-1);
                    ConsultasExternas.RespuestaSQL estadoSocioAux = ConsultasExternas.ConsultasMensualidades.esSocio(rut_aux);
                    if (estadoSocioAux.Correcto)
                    {
                        if (estadoSocioAux.Salida.Length > 0)
                        {
                            totalCancha = totalCancha + precio_socio_aux;
                        }
                        else
                        {
                            totalCancha = totalCancha + precio_no_socio_aux;
                        }
                    }
                    cantidad_asistentes++;
                }

                totalCancha = totalCancha + ((4 - cantidad_asistentes) * precio_no_socio_aux);

                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);

                //Transaction Transaction = new Transaction(new Options("Consultar Codigo Transbanck empresa", "Consultar Codigo Transbanck empresa", WebpayIntegrationType.Live));
                Transaction Transaction = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));

                var urlHelper = new UrlHelper(ControllerContext.RequestContext);
                var returnUrl = urlHelper.Action("returnPagarArriendoInstalacion", "ReservasInstalacion", null, Request.Url.Scheme); //modificar url derespuesta
                var buyOrder = dia + "_" + mes + "_" + year + "/" + idCalendario + "/" + Usuario.Rut;  ///aca debo mejorar
                var sessionId = Usuario.Rut;
                var amount = totalCancha;
                var result = Transaction.Create(buyOrder, sessionId, amount, returnUrl);
                var action = result.Url;
                var token = result.Token;
                string total = Models.NumerosFormato.ObtenerNumeroFormato(amount.ToString());
                Session["Dia"] = dia;
                Session["Mes"] = mes;
                Session["Year"] = year;
                Session["IDCalendario"] = idCalendario;
                Session["Requerimientos"] = Requerimientos;
                Session["Asistentes"] = Asistentes;
                Session["Llegada"] = Llegada;

                string fechaArrienddo = dia + "-" + mes + "-" + year;
                object[] salida = new object[4] { action, token, total, fechaArrienddo };
                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, Socio = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
            }
        }





        [HttpPost]
        public JsonResult GuardarReservaInstalacion(int dia, int mes, int year, int idCalendario, string Requerimientos, HttpPostedFileBase Asistentes, string llegada)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL ingresarCupos = ConsultasExternas.ConsultaInstalaciones.CrearReserva(Usuario.Rut, Models.NumerosFormato.ObtenerFecha(fecha), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idCalendario, Requerimientos, llegada);
                if (ingresarCupos.Correcto)
                {

                    int idReserva = ObtenerIDReserva(idCalendario, dia, mes, year);

                    if (SubirCertificado(Asistentes, idReserva))
                    {
                        string nameCertificado = nombreCertificado(Asistentes, idReserva.ToString());
                        ConsultasExternas.ConsultaInstalaciones.ActualizarAsistentes(nameCertificado, idReserva.ToString());
                    }

                    string nombreCompleto = Usuario.NombreUsuario + " " + Usuario.ApellidoPaternoUsuario + " " + Usuario.ApellidoMaternoUsuario;
                    ConsultasExternas.Correos.EnviarCorreoConfirmacionArriendoInstalacion(Usuario.Correo, idCalendario, nombreCompleto, fecha);
                    return Json(new { Message = JsonConvert.SerializeObject(true), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(ingresarCupos.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult GuardarReservaInstalacionInvitado(int dia, int mes, int year, int idCalendario, string Rut, string Comentario, string nombreCompleto, string Correo)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                ConsultasExternas.RespuestaSQL ingresarCupos = ConsultasExternas.ConsultaInstalaciones.CrearReserva(Rut, Models.NumerosFormato.ObtenerFecha(fecha), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idCalendario, Comentario,"");
                if (ingresarCupos.Correcto)
                {
                    ConsultasExternas.Correos.EnviarCorreoConfirmacionArriendoInstalacion(Correo, idCalendario, nombreCompleto, fecha);
                    return Json(new { Message = JsonConvert.SerializeObject(true), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(ingresarCupos.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        public JsonResult BuscarReservas(int dia, int mes, int year)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerReservasInstalaciones(Models.NumerosFormato.ObtenerFecha(fecha), Usuario.Rut);
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
                string correo = Usuario.Correo;
                string nombreUsuario = Usuario.NombreUsuario+" "+Usuario.ApellidoPaternoUsuario+" "+Usuario.ApellidoMaternoUsuario;
                ConsultasExternas.RespuestaSQL detalle = ConsultasExternas.ConsultaInstalaciones.ObtenerReservaInstalacion(idReserva);


                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.EliminarReservaInstalacíon(idReserva, Usuario.Rut);
                if ((Actividades.Correcto)&&(detalle.Correcto))
                {
                    object[] datos = (object[])detalle.Salida[0];
                    string fecha = datos[7].ToString();
                    string instalacion = datos[6].ToString();
                    string hora = datos[8].ToString();

                    object[] correosDestino = new object[4] { "rcarrizo@canal-ltda.cl", "n.alvarez@deportivoalgarrobo.cl", "valentinaolmedog@gmail.com", correo };
                    ConsultasExternas.Correos.EnviarCorreoCancelacionInstalacionPorUsuario(correosDestino, instalacion, fecha, nombreUsuario, hora);
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


        [HttpPost]
        public JsonResult CorreoSolicitudArriendo(int dia, int mes, int year, string  nombreInstalacion, string Requerimientos)
        {
            try
            {
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                DateTime fecha = new DateTime(year, mes, dia);
                string nombreCompleto = Usuario.NombreUsuario + " " + Usuario.ApellidoPaternoUsuario + " " + Usuario.ApellidoMaternoUsuario;
                string correo = Usuario.Correo;
                string ntelefono = Usuario.NTelefono;
                Object[] destinos = new object[3];
                destinos[0] = "";
                destinos[1] = "n.alvarez @deportivoalgarrobo.cl";
                destinos[2] = "valentinaolmedog@gmail.com";

                bool correcto = ConsultasExternas.Correos.EnviarCorreoSolicitudReservaInstalacion(destinos, nombreInstalacion, Requerimientos, fecha, nombreCompleto, correo, ntelefono);
                if (correcto == true) 
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Correcto"), Correcto = true, Socio = false, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Solicitud de Arriendo"), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
            }
        }




        [HttpPost]
        public JsonResult CorreoSolicitudArriendoInvitado(int dia, int mes, int year, string nombreInstalacion, string Requerimientos, string nombre, string apellidoPaterno, string apellidoMaterno,string Correo, string Telefono)
        {
            try
            {
                //obtener correo de los administradores
                DateTime fecha = new DateTime(year, mes, dia);
                string nombreCompleto = nombre + " " + apellidoPaterno + " " + apellidoMaterno;
                string correo = Correo;
                string ntelefono = Telefono;
                Object[] destinos = new object[1];
                destinos[0] = "";
                

                bool correcto = ConsultasExternas.Correos.EnviarCorreoSolicitudReservaInstalacion(destinos, nombreInstalacion, Requerimientos, fecha, nombreCompleto, correo, ntelefono);
                if (correcto == true)
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Solicitud de Arriendo"), Correcto = true, Socio = false, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Solicitud de Arriendo"), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, Socio = false, JsonRequestBehavior.AllowGet });
            }
        }

        private int DiaDeLaSemana(DateTime fecha)
        {
            if (fecha.DayOfWeek == DayOfWeek.Monday)
            {
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



        private Boolean GuardarTransaccionConfirmada(Models.Tranferencia tran, string usuario)
        {
            string fecha = Models.NumerosFormato.ObtenerFechaCompleta(tran.getTransactionDateHorarioChile(4));
            ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.ConsultaInstalaciones.IngresarTransaccion(usuario, tran.getBuyOrder(), tran.getCardNumber(), tran.getMonto(), tran.getInstallmentsNumber(),
            tran.getInstallmentsAmount(), tran.getAuthorizationCode(), tran.getAccountingDate(), fecha, tran.getPaymentTypeCode(), tran.getVci(), tran.getStatus(), tran.Token_WS);

            if (respuesta.Correcto)
            {
                //respuesta = ConsultasExternas.ConsultaInstalaciones.ObtenerIDTransaccion(usuario, tran.Token_WS, tran.getBuyOrder());
                //if (respuesta.Correcto)
                //{
                    //realizar reserva
                    return true;
                //}
            }
            ConsultasExternas.Correos.EnviarCorreoError(usuario, tran.getBuyOrder(), tran.getCardNumber(), tran.getMonto(), tran.getInstallmentsNumber(),
                tran.getInstallmentsAmount(), tran.getAuthorizationCode(), tran.getAccountingDate(), fecha, tran.getPaymentTypeCode(), tran.getVci(), tran.getStatus(), tran.Token_WS);
            return false;
        }

        private Boolean GuardarReservaInstalacionInterno(int dia, int mes, int year, int idCalendario, string requerimientos, string llegada)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                ConsultasExternas.RespuestaSQL ingresarCupos = ConsultasExternas.ConsultaInstalaciones.CrearReserva(Usuario.Rut, Models.NumerosFormato.ObtenerFecha(fecha), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idCalendario, requerimientos, llegada);
                if (ingresarCupos.Correcto)
                {
                    //enviar correo de reserva de instalacion
                    string nombreCompleto = Usuario.NombreUsuario + " " + Usuario.ApellidoPaternoUsuario + " " + Usuario.ApellidoMaternoUsuario;
                    ConsultasExternas.Correos.EnviarCorreoConfirmacionArriendoInstalacion(Usuario.Correo, idCalendario, nombreCompleto, fecha);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exc)
            {
                string e = exc.Message;
                return false;
            }
        }

        private Boolean GuardarReservaInstalacionInterno(string rut, string nombre, string apellidoP, string apellidoM, string correo,  int dia, int mes, int year, int idCalendario, string requerimientos)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                ConsultasExternas.RespuestaSQL ingresarCupos = ConsultasExternas.ConsultaInstalaciones.CrearReserva(rut, Models.NumerosFormato.ObtenerFecha(fecha), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idCalendario, requerimientos, "");
                if (ingresarCupos.Correcto)
                {
                    string nombreCompleto = nombre + " " + apellidoP + " " + apellidoM;
                    ConsultasExternas.Correos.EnviarCorreoConfirmacionArriendoInstalacion(correo, idCalendario, nombreCompleto, fecha);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exc)
            {
                string e = exc.Message;
                return false;
            }
        }




        private int ObtenerIDReserva(int id_calendario, int dia, int mes, int year)
        {
            DateTime fecha = new DateTime(year, mes, dia);
            ConsultasExternas.RespuestaSQL salida =  ConsultasExternas.ConsultaInstalaciones.ObtenerIDReservasInstalacion(Models.NumerosFormato.ObtenerFecha(fecha), id_calendario);
            if (salida.Correcto)
            {
                return salida.SalidaEntero;
            }
            return -1;
        }



        private int ObtenerPrecioPadelSocio(int idInstalacion , int idCalendario)
        {
            int salida = 0;

            ArrayList idCalendarioNochePadel = new ArrayList();
            idCalendarioNochePadel.Add(1547); idCalendarioNochePadel.Add(1548); idCalendarioNochePadel.Add(1549);
            idCalendarioNochePadel.Add(1610); idCalendarioNochePadel.Add(1611); idCalendarioNochePadel.Add(1612);
            idCalendarioNochePadel.Add(1619); idCalendarioNochePadel.Add(1620); idCalendarioNochePadel.Add(1621);
            idCalendarioNochePadel.Add(1556); idCalendarioNochePadel.Add(1557); idCalendarioNochePadel.Add(1558);
            idCalendarioNochePadel.Add(1565); idCalendarioNochePadel.Add(1566); idCalendarioNochePadel.Add(1567);
            idCalendarioNochePadel.Add(1628); idCalendarioNochePadel.Add(1629); idCalendarioNochePadel.Add(1630);
            idCalendarioNochePadel.Add(1637); idCalendarioNochePadel.Add(1638); idCalendarioNochePadel.Add(1639);
            idCalendarioNochePadel.Add(1574); idCalendarioNochePadel.Add(1575); idCalendarioNochePadel.Add(1576);
            idCalendarioNochePadel.Add(1583); idCalendarioNochePadel.Add(1584); idCalendarioNochePadel.Add(1585);
            idCalendarioNochePadel.Add(1646); idCalendarioNochePadel.Add(1647); idCalendarioNochePadel.Add(1648);
            idCalendarioNochePadel.Add(1655); idCalendarioNochePadel.Add(1656); idCalendarioNochePadel.Add(1657);
            idCalendarioNochePadel.Add(1592); idCalendarioNochePadel.Add(1593); idCalendarioNochePadel.Add(1594);
            idCalendarioNochePadel.Add(1601); idCalendarioNochePadel.Add(1602); idCalendarioNochePadel.Add(1603);
            idCalendarioNochePadel.Add(1664); idCalendarioNochePadel.Add(1665); idCalendarioNochePadel.Add(1666);



            if ((idInstalacion == 47)|| (idInstalacion == 48)|| (idInstalacion == 49)|| (idInstalacion == 50))
            {
                salida = 6000;
                if (idCalendarioNochePadel.Contains(idCalendario))
                {
                    salida = 14000;
                }

            }


            return salida;
        }

        private Boolean SubirCertificado(HttpPostedFileBase Certificado,int id_reserva)
        {
            /*almacenar fichero*/
            if (Certificado != null)
            {
                Models.GuardarArchivo guardarCertificado = new Models.GuardarArchivo();
                string ruta = Server.MapPath("~/Asistentes/");
                string nombreCertificado = "asistentes_"+ id_reserva;
                string extensionCertificado = Certificado.FileName.Substring(Certificado.FileName.LastIndexOf("."));
                ruta += nombreCertificado + extensionCertificado;
                if (Certificado.ContentLength <= 4000000) //menor o igual a 4 megas
                {
                    guardarCertificado.subirArchivo(ruta, Certificado);
                }
                return guardarCertificado.Correcto;
            }
            return false;

        }


        private string nombreCertificado(HttpPostedFileBase Certificado, string idReserva)
        {
            string salida = "";
            /*almacenar fichero*/
            if (Certificado != null)
            {
                string nombreCertificado = "asistentes_" + idReserva;
                string extensionCertificado = Certificado.FileName.Substring(Certificado.FileName.LastIndexOf("."));
                salida += nombreCertificado + extensionCertificado;

            }
            return salida;
        }

        private Boolean fechasReservadas(int year, int mes, int dia, int idClase)
        {
            Boolean esFeriado = false;
            Dictionary<int, ArrayList> fechas = new Dictionary<int, ArrayList>();
            //picnic 1
            ArrayList lista_picnic_1 = new ArrayList();
            /*
            lista_picnic_1.Add(new DateTime(2023, 9, 8));
            lista_picnic_1.Add(new DateTime(2023, 9, 9));
            lista_picnic_1.Add(new DateTime(2023, 9, 10));
            lista_picnic_1.Add(new DateTime(2023, 9, 14));
            lista_picnic_1.Add(new DateTime(2023, 9, 15));
            lista_picnic_1.Add(new DateTime(2023, 9, 16));
            lista_picnic_1.Add(new DateTime(2023, 9, 17));
            lista_picnic_1.Add(new DateTime(2023, 9, 18));
            lista_picnic_1.Add(new DateTime(2023, 9, 19));
            lista_picnic_1.Add(new DateTime(2023, 9, 23));
            lista_picnic_1.Add(new DateTime(2023, 9, 24));
            lista_picnic_1.Add(new DateTime(2023, 9, 29));
            lista_picnic_1.Add(new DateTime(2023, 9, 30));
            lista_picnic_1.Add(new DateTime(2023, 10, 6));
            lista_picnic_1.Add(new DateTime(2023, 10, 7));
            lista_picnic_1.Add(new DateTime(2023, 10, 8));
            lista_picnic_1.Add(new DateTime(2023, 10, 14));
            lista_picnic_1.Add(new DateTime(2023, 10, 15));
            lista_picnic_1.Add(new DateTime(2023, 10, 21));
            lista_picnic_1.Add(new DateTime(2023, 10, 28));
            lista_picnic_1.Add(new DateTime(2023, 11, 11));
            lista_picnic_1.Add(new DateTime(2023, 11, 25));
            */
            fechas.Add(17, lista_picnic_1);
            //picnic 2 18
            ArrayList lista_picnic_2 = new ArrayList();
            /*
            lista_picnic_2.Add(new DateTime(2023, 9, 7));
            lista_picnic_2.Add(new DateTime(2023, 9, 8));
            lista_picnic_2.Add(new DateTime(2023, 9, 9));
            lista_picnic_2.Add(new DateTime(2023, 9, 10));
            lista_picnic_2.Add(new DateTime(2023, 9, 16));
            lista_picnic_2.Add(new DateTime(2023, 9, 17));
            lista_picnic_2.Add(new DateTime(2023, 9, 23));
            lista_picnic_2.Add(new DateTime(2023, 9, 24));
            lista_picnic_2.Add(new DateTime(2023, 9, 30));
            lista_picnic_2.Add(new DateTime(2023, 10, 7));
            lista_picnic_2.Add(new DateTime(2023, 10, 14));
            lista_picnic_2.Add(new DateTime(2023, 10, 21));
            lista_picnic_2.Add(new DateTime(2023, 10, 28));
            lista_picnic_2.Add(new DateTime(2023, 11, 3));
            */
            fechas.Add(18, lista_picnic_2);


            //picnic rayuela 1 19
            ArrayList lista_rauela_1 = new ArrayList();
            /*
            lista_rauela_1.Add(new DateTime(2023, 9, 7));
            lista_rauela_1.Add(new DateTime(2023, 9, 8));
            lista_rauela_1.Add(new DateTime(2023, 9, 9));
            lista_rauela_1.Add(new DateTime(2023, 9, 10));
            lista_rauela_1.Add(new DateTime(2023, 9, 12));
            lista_rauela_1.Add(new DateTime(2023, 9, 14));
            lista_rauela_1.Add(new DateTime(2023, 9, 15));
            lista_rauela_1.Add(new DateTime(2023, 9, 16));
            lista_rauela_1.Add(new DateTime(2023, 9, 23));
            lista_rauela_1.Add(new DateTime(2023, 10, 1));
            lista_rauela_1.Add(new DateTime(2023, 10, 7));
            lista_rauela_1.Add(new DateTime(2023, 10, 14));
            lista_rauela_1.Add(new DateTime(2023, 10, 21));
            */
            fechas.Add(19, lista_rauela_1);

            //picnic rayuela 2 20
            ArrayList lista_rauela_2 = new ArrayList();
            /*
            lista_rauela_2.Add(new DateTime(2023, 9, 7));
            lista_rauela_2.Add(new DateTime(2023, 9, 8));
            lista_rauela_2.Add(new DateTime(2023, 9, 9));
            lista_rauela_2.Add(new DateTime(2023, 9, 10));
            lista_rauela_2.Add(new DateTime(2023, 9, 12));
            lista_rauela_2.Add(new DateTime(2023, 9, 14));
            lista_rauela_2.Add(new DateTime(2023, 9, 15));
            lista_rauela_2.Add(new DateTime(2023, 9, 16));
            lista_rauela_2.Add(new DateTime(2023, 9, 23));
            lista_rauela_2.Add(new DateTime(2023, 10, 1));
            lista_rauela_2.Add(new DateTime(2023, 10, 7));
            lista_rauela_2.Add(new DateTime(2023, 10, 14));
            lista_rauela_2.Add(new DateTime(2023, 10, 21));
            */
            fechas.Add(20, lista_rauela_2);


            //salon de eventos 43
            ArrayList lista_eventos = new ArrayList();
            /*
            lista_eventos.Add(new DateTime(2023, 9, 5));
            lista_eventos.Add(new DateTime(2023, 9, 6));
            lista_eventos.Add(new DateTime(2023, 9, 7));
            lista_eventos.Add(new DateTime(2023, 9, 9));
            lista_eventos.Add(new DateTime(2023, 9, 14));
            lista_eventos.Add(new DateTime(2023, 10, 5));
            lista_eventos.Add(new DateTime(2023, 10, 6));
            lista_eventos.Add(new DateTime(2023, 10, 7));
            lista_eventos.Add(new DateTime(2023, 10, 22));
            lista_eventos.Add(new DateTime(2023, 10, 23));
            lista_eventos.Add(new DateTime(2023, 10, 24));
            lista_eventos.Add(new DateTime(2023, 10, 25));
            lista_eventos.Add(new DateTime(2023, 10, 29));
            lista_eventos.Add(new DateTime(2023, 11, 18));
            lista_eventos.Add(new DateTime(2023, 11, 23));
            lista_eventos.Add(new DateTime(2023, 11, 29));
            lista_eventos.Add(new DateTime(2023, 11, 30));
            lista_eventos.Add(new DateTime(2023, 12, 2));
            lista_eventos.Add(new DateTime(2023, 12, 6));
            lista_eventos.Add(new DateTime(2023, 12, 7));
            lista_eventos.Add(new DateTime(2023, 12, 8));
            lista_eventos.Add(new DateTime(2023, 12, 9));
            lista_eventos.Add(new DateTime(2023, 12, 15));
            lista_eventos.Add(new DateTime(2023, 12, 16));
            lista_eventos.Add(new DateTime(2023, 12, 21));
            lista_eventos.Add(new DateTime(2023, 12, 22));
            */
            fechas.Add(43, lista_eventos);

            //fin derinicion de feriados
            DateTime fechaConsulta = new DateTime(year, mes, dia);

            if (fechaConsulta.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            if(fechas.ContainsKey(idClase) == true)
            {
                esFeriado = fechas[idClase].Contains(fechaConsulta);

            }

            return esFeriado;
        }


        private Boolean esFeriado(int year, int mes, int dia, int idInstalacion)
        {

            Boolean esFeriado = false;
            ////definir feriados
            ArrayList feriados = new ArrayList();


            feriados.Add(new DateTime(2024, 1, 1));
            feriados.Add(new DateTime(2024, 3, 29));
            feriados.Add(new DateTime(2024, 3, 30));
            feriados.Add(new DateTime(2024, 5, 1));
            feriados.Add(new DateTime(2024, 5, 21));
            feriados.Add(new DateTime(2024, 6, 9));
            ///
            feriados.Add(new DateTime(2024, 6, 20));
            feriados.Add(new DateTime(2024, 6, 29));
            feriados.Add(new DateTime(2024, 7, 16));
            feriados.Add(new DateTime(2024, 8, 15));
            feriados.Add(new DateTime(2024, 9, 18));
            feriados.Add(new DateTime(2024, 9, 19));
            feriados.Add(new DateTime(2024, 9, 20));
            feriados.Add(new DateTime(2024, 10, 12));
            feriados.Add(new DateTime(2024, 10, 27));
            feriados.Add(new DateTime(2024, 10, 31));
            feriados.Add(new DateTime(2024, 11, 1));
            feriados.Add(new DateTime(2024, 11, 24));
            feriados.Add(new DateTime(2024, 12, 8));
            feriados.Add(new DateTime(2024, 12, 25));
            feriados.Add(new DateTime(2025, 1, 1));

            //fin derinicion de feriados
            DateTime fechaConsulta = new DateTime(year, mes, dia);

            esFeriado = feriados.Contains(fechaConsulta);

            ArrayList instalacionDomingos = new ArrayList();
            instalacionDomingos.Add(17);
            instalacionDomingos.Add(18);
            instalacionDomingos.Add(19);
            instalacionDomingos.Add(20);

            //reservas espciales
            //duarante el 2024 no permitir reservas a usuarios de la pergola 2 losdias domingo
            if (fechaConsulta.DayOfWeek == DayOfWeek.Sunday)
            {
                if (idInstalacion == 18)
                {
                    if(fechaConsulta.Year == 2024)
                    {
                        return true;
                    }
                    
                }
            }


            //duarante el 2024 desde el 08 de junio no permitir reservas a usuarios de la pergola 2 losdias sabado
            if (fechaConsulta.DayOfWeek == DayOfWeek.Saturday)
            {
                if (idInstalacion == 18)
                {
                    if (fechaConsulta.Year == 2024)
                    {
                        if(fechaConsulta.Month >=6)
                        {
                            if(fechaConsulta.Month == 6)
                            {
                                if(fechaConsulta.Day >= 8)
                                {
                                    return true;
                                }
                            }
                        }
                        
                    }

                }
            }


            if (fechaConsulta.DayOfWeek == DayOfWeek.Sunday)
            {
                if (instalacionDomingos.Contains(idInstalacion) && (esFeriado == false) )
                {
                    return false;
                }

                return true;
            }

            


                return esFeriado;
        }




    }
}