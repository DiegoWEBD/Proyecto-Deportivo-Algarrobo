using Antlr.Runtime;
using K4os.Compression.LZ4.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplicationTemplateBase.Filters;

namespace WebApplicationTemplateBase.Controllers
{
    public class AdminInstalacionesController : Controller
    {




        [AutorizarUsuario(nombreMetodo: "24")]
        public ActionResult AdministrarInstalaciones()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "26")]
        public ActionResult GestionarInstalaciones()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }



        [AutorizarUsuario(nombreMetodo: "28")]
        public ActionResult TranferenciasRealizadas()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }




        [AutorizarUsuario(nombreMetodo: "24")]
        public ActionResult CalendarioMensual()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }




        [HttpPost]
        public JsonResult ObtenerTodosHorarios(int IdDia, int IdInstalacion, int IdTipo)
        {
            try
            {
                if (IdTipo == 1)
                {
                    return ObtenerHorariosHora(IdDia, IdInstalacion, IdTipo);
                }
                else
                {
                    return ObtenerHorariosDia(IdDia, IdInstalacion, IdTipo);
                }
            }
            catch (Exception exp)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exp.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult AgregarNuevaInstalacion(string nombreInstalacion)
        {
            try
            {
                ConsultasExternas.RespuestaSQL clases = ConsultasExternas.ConsultaInstalaciones.ExisteInstalacion(nombreInstalacion);
                if (clases.Correcto)
                {
                    if (clases.Booleano == false)
                    {
                        clases = ConsultasExternas.ConsultaInstalaciones.AgregarInstalacion(nombreInstalacion);
                        //regitrar log de user administrador
                        string fechaCreacion = Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now);
                        Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                        if (clases.Correcto)
                        {
                            clases = ConsultasExternas.ConsultaInstalaciones.ObtenerIDinstalacion(nombreInstalacion);
                            ConsultasExternas.RespuestaSQL log = ConsultasExternas.ConsultaInstalaciones.AgregarLogInstalacionRegistro(Usuario.Rut, fechaCreacion, clases.SalidaEntero, Usuario.NombreUsuario + " " + Usuario.ApellidoPaternoUsuario);
                            if (clases.Correcto && log.Correcto)
                            {

                                return Json(new { Message = "", Correcto = true, Existe = false, JsonRequestBehavior.AllowGet });

                            }
                            return Json(new { Message = "Error Interno En Realizar La Operación", Correcto = false, Existe = false, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            return Json(new { Message = "En ingresar nueva instalación en BD", Correcto = false, Existe = false, JsonRequestBehavior.AllowGet });
                        }

                    }
                    else
                    {
                        return Json(new { Message = "", Correcto = true, Existe = true, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(clases.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }


            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerInstalaciones()
        {
            try
            {
                ConsultasExternas.RespuestaSQL Instalaciones = ConsultasExternas.ConsultaInstalaciones.ObtenerInstalacion();


                if (Instalaciones.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject(Instalaciones.Salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(Instalaciones.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult ObtenerinformacionInstalacion(int IdInstalacion)
        {
            try
            {

                ConsultasExternas.RespuestaSQL PrecioInstalaciones = ConsultasExternas.ConsultaInstalaciones.ObtenerPrecioInstalacion(IdInstalacion);
                ConsultasExternas.RespuestaSQL informacionInstalaciones = ConsultasExternas.ConsultaInstalaciones.ObtenerInstalacion(IdInstalacion);


                if ((informacionInstalaciones.Correcto) && (PrecioInstalaciones.Correcto))
                {
                    object[] salida = new object[2] { informacionInstalaciones.Salida, PrecioInstalaciones.Salida };
                    string datos_final = JsonConvert.SerializeObject(salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if (informacionInstalaciones.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(informacionInstalaciones.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject(PrecioInstalaciones.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult GuardarInstalacion(int idInstalacion, int tipoArriendo, int Monto, bool registroParticipantes, int diaSemana, string Horarios)
        {
            try
            {
                DateTime fecha = DateTime.Now;
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);

                Object[] horarios = JsonConvert.DeserializeObject<object[]>(Horarios);

                ConsultasExternas.RespuestaSQL informacionInstalaciones = ConsultasExternas.ConsultaInstalaciones.GuardarInstalacion(idInstalacion, tipoArriendo, registroParticipantes);
                ConsultasExternas.RespuestaSQL PrecioInstalaciones = ConsultasExternas.ConsultaInstalaciones.GuardarPrecioArriendo(idInstalacion, Monto, Usuario.Rut, Models.NumerosFormato.ObtenerFecha(fecha));

                if (diaSemana > 0) {
                    ////restablecer todos los valores a falsos para los horarios de un dia especifico
                    ConsultasExternas.RespuestaSQL updateHorasClases = ConsultasExternas.ConsultaInstalaciones.DeshabilitarHorasInstalacionDia(idInstalacion, diaSemana);
                    if (updateHorasClases.Correcto)
                    {
                        for (int i = 0; i < horarios.Length; i++)
                        {
                            int id_horario = int.Parse(horarios[i].ToString());
                            ConsultasExternas.ConsultaInstalaciones.ActualizarHorario(idInstalacion, id_horario, diaSemana, true);
                        }
                    }
                }

                if ((informacionInstalaciones.Correcto) && (PrecioInstalaciones.Correcto))
                {
                    object[] salida = new object[2] { informacionInstalaciones.Salida, PrecioInstalaciones.Salida };
                    string datos_final = JsonConvert.SerializeObject(salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if (informacionInstalaciones.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(informacionInstalaciones.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject(PrecioInstalaciones.Error), Correcto = false, JsonRequestBehavior.AllowGet });
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
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerHorarios(DiaDeLaSemana, idClase);
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
        public JsonResult CargarTipoInstalacion(int dia, int mes, int year)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerTipoInstalacionDisponibleAdmin(DiaDeLaSemana);
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
        public JsonResult CargarTipoInstalacion_Todas()
        {
            try
            {
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerTipoInstalacionDisponibleAdmin();
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


        [HttpPost]
        public JsonResult CargarInstalacion(int dia, int mes, int year)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerInstalacionDisponible(DiaDeLaSemana);
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
        public JsonResult CargarInstalacionTipo(int dia, int mes, int year, int TipoInstalacion)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerInstalacionDisponible(DiaDeLaSemana, TipoInstalacion);
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
        public JsonResult CargarInstalacionTipoTodas(int TipoInstalacion)
        {
            try
            {

                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerInstalaciones(TipoInstalacion);
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

        
        [HttpPost]
        public JsonResult ObtenerReservas(int dia, int mes, int year, int idCalendario, int idInstalacion)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                Boolean cupoInstalacionCompartida = true;
                if (DiaDeLaSemana > 0)
                {
                    //obtener instalacion compartida
                    ConsultasExternas.RespuestaSQL instalacionCompartida = ConsultasExternas.ConsultaInstalaciones.idInstalacionCompartida(idInstalacion);
                    ///inicio
                    int idInstalacionCompartida = -1;
                    if (instalacionCompartida.Correcto)
                    {
                        idInstalacionCompartida = instalacionCompartida.SalidaEntero;
                        if (idInstalacionCompartida > 0){
                            ConsultasExternas.RespuestaSQL cuposCompartidos = ConsultasExternas.ConsultaInstalaciones.CupoDisponible(idInstalacionCompartida, DiaDeLaSemana, idCalendario, Models.NumerosFormato.ObtenerFecha(fecha));
                            if (cuposCompartidos.Correcto)
                            {
                                cupoInstalacionCompartida = cuposCompartidos.Booleano;
                            }
                        }
                    }
                    ///fin////
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerReservasInstalacion(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                    
                    if (Actividades.Correcto)
                    {
                        string datos_final = JsonConvert.SerializeObject(Actividades.Salida);
                        return Json(new { Message = datos_final, Correcto = true, cupoCompartido = cupoInstalacionCompartida, JsonRequestBehavior.AllowGet });
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
        public JsonResult ObtenerReservasTipoInstalacion(int dia, int mes, int year, int idTipoInstalacion)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerReservasHorasTipoInstalacion(Models.NumerosFormato.ObtenerFecha(fecha), idTipoInstalacion);

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
        public JsonResult ObtenerReservasInstalacion(int dia, int mes, int year, int idInstalacion)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerReservasHorasInstalacion(Models.NumerosFormato.ObtenerFecha(fecha), idInstalacion);

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
        public JsonResult ObtenerReservasDia(int dia, int mes, int year)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerReservasDia(Models.NumerosFormato.ObtenerFecha(fecha));

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
        public JsonResult ObtenerReservaInstalacion(int idReserva)
        {
            try
            {
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerReservaInstalacion(idReserva);
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



        [HttpPost]
        public JsonResult CancelarReserva(int idReserva, string userName)
        {
            try
            {
                userName = userName.Trim();
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.EliminarReserva(idReserva, userName);

                if (Actividades.Correcto)
                {
                    ConsultasExternas.ConsultaInstalaciones.EliminarAsistentes(idReserva.ToString());

                    string datos_final = JsonConvert.SerializeObject("");
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
        public JsonResult CancelarInstalacion(int idInstalacion, int dia, int mes, int year, string nombreInstalacion)
        {
            try
            {
                //necesito obtener  los usuarios que tienen reservado para notificar
                //obtener todos los id de calendario asociados
                //crear fecha de cancelacion
                //agregar a tabla de cancelacion
                DateTime fecha = new DateTime(year, mes, dia);
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerReservasHorasInstalacion(Models.NumerosFormato.ObtenerFecha(fecha), idInstalacion);
                if (Actividades.Correcto)
                {
                    //notificar cancelacioon
                    Boolean CorreosEnviados = ConsultasExternas.Correos.EnviarCorreoCancelacionInstalacion(Actividades.Salida, nombreInstalacion, fecha);
                    object[] datos = Actividades.Salida;
                    for (int i=0; i < datos.Length; i++){

                        object[] fila = (object[])datos[i];
                        int idReserva_aux   = int.Parse(fila[0].ToString());
                        string userName_aux = fila[1].ToString();
                        ConsultasExternas.ConsultaInstalaciones.EliminarReserva(idReserva_aux, userName_aux);
                    }
                    //bloquear instalacion
                    int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                    ConsultasExternas.RespuestaSQL idHorarios = ConsultasExternas.ConsultaInstalaciones.ObtenerHorarios(DiaDeLaSemana, idInstalacion);
                    if(idHorarios.Correcto)
                    {
                        object[] datos_aux = idHorarios.Salida;
                        for (int i = 0; i < datos_aux.Length; i++)
                        {
                            object[] fila = (object[])datos_aux[i];
                            int idCalendario_aux = int.Parse(fila[0].ToString());
                            ConsultasExternas.ConsultaInstalaciones.IngresarCancelacion(idCalendario_aux, Models.NumerosFormato.ObtenerFecha(fecha), "Motivo Fuerza Mayor");
                        }
                    }

                }

                   

                if (Actividades.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject("");
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
        public JsonResult CancelarInstalacionesDia(int dia, int mes, int year)
        {
            try
            {
                //necesito obtener  los usuarios que tienen reservado para notificar
                //obtener todos los id de calendario asociados
                //crear fecha de cancelacion
                //agregar a tabla de cancelacion
                DateTime fecha = new DateTime(year, mes, dia);
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultaInstalaciones.ObtenerReservasDia(Models.NumerosFormato.ObtenerFecha(fecha));
                if (Actividades.Correcto)
                {
                    //notificar cancelacioon
                    Boolean CorreosEnviados = ConsultasExternas.Correos.EnviarCorreoCancelacionInstalacionesDia(Actividades.Salida, fecha);
                    object[] datos = Actividades.Salida;
                    for (int i = 0; i < datos.Length; i++)
                    {

                        object[] fila = (object[])datos[i];
                        int idReserva_aux = int.Parse(fila[0].ToString());
                        string userName_aux = fila[1].ToString();
                        ConsultasExternas.ConsultaInstalaciones.EliminarReserva(idReserva_aux, userName_aux);
                    }
                    //bloquear instalacion
                    int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                    ConsultasExternas.RespuestaSQL ingresoCancelacion = ConsultasExternas.ConsultaInstalaciones.IngresarCancelacionDia(Models.NumerosFormato.ObtenerFecha(fecha), "Fuerza Mayor");
                    if(ingresoCancelacion.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(ingresoCancelacion.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                }

                if (Actividades.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject("");
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
        public JsonResult ObtenerTranferencias(int year, int mes)
        {
            try
            {
                DateTime fechaInicio = new DateTime(year, mes, 1);
                DateTime fechaTermino = fechaInicio.AddMonths(1);

                ConsultasExternas.RespuestaSQL transacciones = ConsultasExternas.ConsultasMensualidades.ObtenerTransaccionesInstalaciones(Models.NumerosFormato.ObtenerFecha(fechaInicio), Models.NumerosFormato.ObtenerFecha(fechaTermino));
                if (transacciones.Correcto)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(transacciones.Salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(transacciones.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerReservasMes(int year, int mes)
        {
            try
            {
                DateTime fechaInicio = new DateTime(year, mes, 1);
                DateTime fechaTermino = fechaInicio.AddMonths(1);

                ConsultasExternas.RespuestaSQL transacciones = ConsultasExternas.ConsultaInstalaciones.ObtenerReservasMes(Models.NumerosFormato.ObtenerFecha(fechaInicio), Models.NumerosFormato.ObtenerFecha(fechaTermino));
                if (transacciones.Correcto)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(transacciones.Salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(transacciones.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult VerificarSesion()
        {
            int cantidad = Session.Count;
            return Json(new { Correcto = true, JsonRequestBehavior.AllowGet });
        }


       



        [HttpPost]
        public JsonResult GuardarReservaInstalacion(int dia, int mes, int year, int idCalendario, string nombre, string app, string apm, string rut, string correo, int monto, string numeroTelefono, int Repetir, int idHorario, int idUbicacion, string requerimientos, string HorarioLlegada)
        {
           
            ///nuevo
            Boolean Correcto = true;
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);                
                Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
                Correcto = PagarClaseManual(rut, dia, mes, year, monto, idCalendario);
                if (Correcto)
                {
                    //crear usuario invitado en tabla usuario
                    if (ConsultasExternas.Consultas.DatosUsuarios(rut).Rows.Count == 0)
                    {
                        //creo usuario invitado
                        Correcto = ConsultasExternas.Consultas.CrearUsuarioInvitado(rut, nombre, app, apm, numeroTelefono, "000", correo, "Invitado2022", "");
                    }
                    if (Correcto)
                    {

                        object[] fecha_array = new object[1] { fecha };
                        object[] fecha_idCalendario = new object[1] { idCalendario };
                        if (Repetir == 1)
                        {
                            //repetir semana 
                            object[]  aux = ObtenerIDCalendarioSemana(year, mes, dia, idCalendario, idHorario, idUbicacion);
                            fecha_array = (object[])aux[1];
                            fecha_idCalendario = (object[])aux[0];
                        }
                        if (Repetir == 2)
                        {
                            //repetir mes
                            object[] aux = ObtenerIDCalendarioMes(year, mes, dia, idCalendario, idHorario, idUbicacion);
                            fecha_array = (object[])aux[1];
                            fecha_idCalendario = (object[])aux[0];
                        }
                        ConsultasExternas.RespuestaSQL ingresarCupos = new ConsultasExternas.RespuestaSQL(true);
                        ConsultasExternas.RespuestaSQL ingresarLog = new ConsultasExternas.RespuestaSQL(true);
                        ConsultasExternas.RespuestaSQL idReserva = new ConsultasExternas.RespuestaSQL(true);
                        for (int i=0; i< fecha_idCalendario.Length; i++)
                        {
                            int id_calendario_aux = (int)(fecha_idCalendario[i]);
                            DateTime fecha_aux = (DateTime)(fecha_array[i]); 
                            if(id_calendario_aux >= 0)
                            {
                                ingresarCupos = ConsultasExternas.ConsultaInstalaciones.CrearReserva(rut, Models.NumerosFormato.ObtenerFecha(fecha_aux), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), id_calendario_aux, requerimientos, HorarioLlegada);
                                //ingresar log admin
                                idReserva = ConsultasExternas.ConsultaInstalaciones.ObtenerIDReservaInstalacion();
                                ingresarLog = ConsultasExternas.ConsultaInstalaciones.RegistrarLogReservaInstalacion(Usuario.Rut, Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idReserva.SalidaEntero, Usuario.NombreUsuario + " " + Usuario.ApellidoPaternoUsuario);                                
                            }
                        }


                        if (ingresarCupos.Correcto)
                        {
                            ConsultasExternas.RespuestaSQL calendario = ConsultasExternas.ConsultaInstalaciones.ObtenerDatosCalendario(idCalendario);
                            if (calendario.Correcto)
                            {
                                if (calendario.Salida.Length > 0)
                                {
                                    object[] arreglo = (object[])calendario.Salida[0];
                                    string nombreClase = arreglo[1].ToString();
                                    string horarioClase = arreglo[2].ToString();
                                    string nombreCompleto = nombre + " " + app + " " + apm;

                                    //ConsultasExternas.Correos.EnviarCorreoConfirmacionArriendoInstalacion(correo, idCalendario, nombreCompleto, fecha);
                                }
                            }
                            return Json(new { Message = true, Correcto = true, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            return Json(new { Message = JsonConvert.SerializeObject("Ingreso de reserva de instalación"), Correcto = false, JsonRequestBehavior.AllowGet });
                        }
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Ingreso de registro de usuario visita"), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Ingreso de tranferencia en base de datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                }

            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }


            ///fin
        }






        private JsonResult ObtenerHorariosHora(int IdDia, int IdInstalacion, int IdTipo)
        {
            try
            {
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerHorarios();
                ConsultasExternas.RespuestaSQL HorasSeleccinadas = ConsultasExternas.ConsultaInstalaciones.ObtenerHorarios(IdDia, IdInstalacion, IdTipo);

                if ((Actividades.Correcto) && (HorasSeleccinadas.Correcto))
                {
                    object[] salida = new object[2] { Actividades.Salida, HorasSeleccinadas.Salida };
                    string datos_final = JsonConvert.SerializeObject(salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if (Actividades.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject(HorasSeleccinadas.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch(Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        private JsonResult ObtenerHorariosDia(int IdDia, int IdInstalacion, int idTipo)
        {
            try
            {
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerHorariosDia();
                ConsultasExternas.RespuestaSQL HorasSeleccinadas = ConsultasExternas.ConsultaInstalaciones.ObtenerHorarios(IdDia, IdInstalacion, idTipo);

                if ((Actividades.Correcto) && (HorasSeleccinadas.Correcto))
                {
                    object[] salida = new object[2] { Actividades.Salida, HorasSeleccinadas.Salida };
                    string datos_final = JsonConvert.SerializeObject(salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if (Actividades.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject(HorasSeleccinadas.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
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

        private bool PagarClaseManual(string rut, int dia, int mes, int year, int monto, int idCalendario)
        {
            try
            {
                string ordenCompra = dia + "_" + mes + "_" + year + "/" + idCalendario + "/" + rut;
                string detalleTarjeta = "Manual";
                string codTransaccion = "Manual";
                string mes_string = mes.ToString();
                string dia_string = DateTime.Now.ToString("dd");
                if (mes < 10) { mes_string = "0" + mes_string; }
                string fechaAutoriza = mes_string + dia_string;
                string fechaTransa = Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now);
                string token = CreateMD5(ordenCompra + fechaTransa);
                ConsultasExternas.RespuestaSQL Transaccion = ConsultasExternas.ConsultaInstalaciones.IngresarTransaccion(rut, ordenCompra, detalleTarjeta, monto, 0, 0, codTransaccion, fechaAutoriza, fechaTransa, "MANUAL", "MANUAL", "AUTHORIZED", token);
                if (Transaccion.Correcto)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string CreateMD5(string input)
        {
            string password = @"1234abcd";

            // byte array representation of that string
            byte[] encodedPassword = new UTF8Encoding().GetBytes(password);

            // need MD5 to calculate the hash
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            // string representation (similar to UNIX format)
            string encoded = BitConverter.ToString(hash)
               // without dashes
               .Replace("-", string.Empty)
               // make lowercase
               .ToLower();

            return encoded;

        }


        private static Object[] ObtenerIDCalendarioSemana(int year, int mes, int dia, int calendario, int idHorario, int idInstalacion)
        {

            DateTime fechaReserva = new DateTime(year, mes, dia);

            //obtengo el dia de la semana
            int diaSemana = ((int)fechaReserva.DayOfWeek);
            //creao arreglo de salida
            Object[] salida = new Object[7-diaSemana];
            Object[] salidaFecha = new Object[7 - diaSemana];
            salida[0] = calendario; //aca deberia agregar el id del calendario del primer dia
            salidaFecha[0] = fechaReserva; //aca deberia agregar el id del calendario del primer dia
            diaSemana = diaSemana + 1;
            fechaReserva = fechaReserva.AddDays(1);
            int contador = 1;
            for(int i= diaSemana; i <= 6; i++)
            {
                ConsultasExternas.RespuestaSQL idCalendarioRespuesta  = ConsultasExternas.ConsultaInstalaciones.ObtenerIDCalendario(i, idInstalacion, idHorario);
                int idCalendarioAUX = -1;
                if (idCalendarioRespuesta.Correcto)
                {
                    idCalendarioAUX = idCalendarioRespuesta.SalidaEntero;
                }
                salida[contador] = idCalendarioAUX;
                salidaFecha[contador] = fechaReserva;
                contador = contador + 1;
                fechaReserva = fechaReserva.AddDays(1);
            }

            return new object[2] { salida, salidaFecha };
        }



        private static Object[] ObtenerIDCalendarioMes(int year, int mes, int dia, int calendario, int idHorario, int idInstalacion)
        {

            DateTime fechaReserva = new DateTime(year, mes, dia);

            Boolean salir = false;
            int contador = 1;
            while(salir == false)
            {
                fechaReserva = fechaReserva.AddDays(7);
                if(fechaReserva.Month != mes)
                {
                    salir = true;
                }
                else
                {
                    contador = contador + 1;
                }
            }

            fechaReserva = new DateTime(year, mes, dia);
            Object[] salida = new Object[contador];
            salida[0] = calendario;
            Object[] salidaFecha = new Object[contador];
            salidaFecha[0] = fechaReserva;

            salir = false;
            contador = 1;
            
            while (salir == false)
            {
                fechaReserva = fechaReserva.AddDays(7);
                if (fechaReserva.Month != mes){
                    salir = true;
                }
                else
                {
                    int diaSemana = ((int)fechaReserva.DayOfWeek);
                    ConsultasExternas.RespuestaSQL idCalendarioRespuesta = ConsultasExternas.ConsultaInstalaciones.ObtenerIDCalendario(diaSemana, idInstalacion, idHorario);
                    int idCalendarioAUX = -1;
                    if (idCalendarioRespuesta.Correcto)
                    {
                        idCalendarioAUX = idCalendarioRespuesta.SalidaEntero;
                    }
                    salida[contador] = idCalendarioAUX;
                    salidaFecha[contador] = fechaReserva;
                    contador = contador + 1;
                }
            }


            return new object[2] { salida, salidaFecha };
        }


  


    }
}
