using Antlr.Runtime;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using WebApplicationTemplateBase.ConsultasExternas;
using WebApplicationTemplateBase.Filters;

namespace WebApplicationTemplateBase.Controllers
{
    public class AdminController : Controller
    {

        [AutorizarUsuario(nombreMetodo: "8")]
        public ActionResult VerReservas()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }

        [AutorizarUsuario(nombreMetodo: "9")]
        public ActionResult GestionarClases()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }

        [AutorizarUsuario(nombreMetodo: "10")]
        public ActionResult AdminUsuarios()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }

        [AutorizarUsuario(nombreMetodo: "12")]
        public ActionResult AdminClases()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "8")]
        public ActionResult AsistenciaClase()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "15")]
        public ActionResult AsignacionProfesores()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "17")]
        public ActionResult ReservaHoraAdmin()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "18")]
        public ActionResult TranferenciasRealizadas()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "20")]
        public ActionResult Certificados()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "21")]
        public ActionResult Socios()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "23")]
        public ActionResult ReporteMensualidad()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }


        [AutorizarUsuario(nombreMetodo: "30")]
        public ActionResult EstadisticaReservas()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();
        }

        [AutorizarUsuario(nombreMetodo: "32")]
        public ActionResult Empresas()
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


        [HttpPost]
        public JsonResult CargarClase(int dia, int mes, int year)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
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
        public JsonResult EliminarUsuarioBD(string rut)
        {
            try
            {
                if (!rut.IsEmpty())
                {
                    Boolean Actividades = ConsultasExternas.Consultas.EliminarUsuarioBD(rut);
                    if(Actividades)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Usuario eliminado"), Correcto = true, JsonRequestBehavior.AllowGet });
                    } else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Error al elminar"), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Verifique el Rut"), Correcto = false, JsonRequestBehavior.AllowGet });
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
        public JsonResult BuscarReservas(int dia, int mes, int year, int idClase, int idCalendario)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                int DiaDeLaSemana = this.DiaDeLaSemana(fecha);
                if (DiaDeLaSemana > 0)
                {
                    ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerReservas(Models.NumerosFormato.ObtenerFecha(fecha), idClase, idCalendario);
                    if (Actividades.Correcto)
                    {
                        //Podria validarse el detalle de pago
                        //Invocar funcion VerificarDetallePago
                        Object[] respuestas = Actividades.Salida;                        
                        Console.WriteLine(respuestas.Length);

                        for (int i = 0; i < respuestas.Length; i++)
                        {
                            ((object[])respuestas[i])[10] = VerificarDetallePago(((object[])respuestas[i])[8].ToString(), mes, year, dia);
                            //salida_final[i] = respuestas[i];
                        }

                        //foreach (var res in respuestas)
                        //{
                        //    ((object[])res)[9] = VerificarDetallePago(((object[])res)[8].ToString(), mes, year, dia);
                        //    salida_final = res;
                        //    Console.WriteLine(salida_final);
                        //}

                        //ConsultasExternas.RespuestaSQL detallePago = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(usuario, mes, year);
                        string datos_final = JsonConvert.SerializeObject(respuestas);
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

        //Verifica el tipo de pago, si es por mensualidad sino es pago por clase
        private bool VerificarDetallePago(string usuario, int mes, int year, int dia)
        {
            try
            {
                ConsultasExternas.RespuestaSQL EstadoMensualidad = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(usuario, mes, year);

                if (EstadoMensualidad.Correcto)
                {
                    Object[] respuestas = EstadoMensualidad.Salida;
                    if (respuestas.Length > 0)
                    {
                        return true;
                        //return new ConsultasExternas.RespuestaSQL(new object[1] { true });
                    }
                    else
                    {
                        if (Models.NumerosFormato.esPrimerDiaHabildelMes(dia, mes, year)) //hasta el primer dia habil del mes ()
                        {
                            mes = mes - 1;
                            if (mes == 0)
                            {
                                mes = 12;
                                year = year - 1;
                            }
                            EstadoMensualidad = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(usuario, mes, year);
                            if (EstadoMensualidad.Correcto)
                            {
                                respuestas = EstadoMensualidad.Salida;
                                if (respuestas.Length > 0)
                                {
                                    return true;
                                    //return new ConsultasExternas.RespuestaSQL(new object[1] { true });
                                }
                                return false;
                                //return new ConsultasExternas.RespuestaSQL(new object[1] { false });
                            }
                            else
                            {
                                return false;
                                //return new ConsultasExternas.RespuestaSQL("Error interno en consulta Base De Datos");
                            }

                        }
                        return false;
                        //return new ConsultasExternas.RespuestaSQL(new object[1] { false });
                    }
                }
                else
                {
                    return false;
                    //return new ConsultasExternas.RespuestaSQL(new object[1] { false });
                }
            }
            catch (Exception exc)
            {
                return false;
                //return new ConsultasExternas.RespuestaSQL(new object[1] { exc });
            }
        }



        [HttpPost]
        public JsonResult ObtenerClases()
        {
            try
            {

                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerClases();
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
        public JsonResult CargarInformacionClase(int dia, int mes, int year, int idCalendario)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                ConsultasExternas.RespuestaSQL EstadoReserva = ConsultasExternas.ConsultasCancelacionClases.estadoClase(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerReservas(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                if ((Actividades.Correcto)&&(EstadoReserva.Correcto))
                {
                    string datos_final = JsonConvert.SerializeObject(Actividades.Salida);
                    bool Cancelado = false;
                    if(EstadoReserva.Salida.Length > 0){
                        Cancelado = true;
                    }
                    return Json(new { Message = datos_final, Cancelado=Cancelado, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if (Actividades.Correcto)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject(EstadoReserva.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }

            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerTodosHorarios(int IdDia, int IdClase)
        {
            try
            {
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerHorarios();
                ConsultasExternas.RespuestaSQL HorasSeleccinadas = ConsultasExternas.ConsultasCalendario.ObtenerHorarios(IdDia, IdClase);
                ConsultasExternas.RespuestaSQL maxParticipantes = ConsultasExternas.ConsultasCalendario.CuposMaximosPorClase(IdClase, true);

                if ((Actividades.Correcto) && (HorasSeleccinadas.Correcto) && (maxParticipantes.Correcto))
                {
                    object[] salida = new object[3] { Actividades.Salida, HorasSeleccinadas.Salida, maxParticipantes.Salida };
                    string datos_final = JsonConvert.SerializeObject(salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if (Actividades.Correcto == false) {
                        return Json(new { Message = JsonConvert.SerializeObject(Actividades.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    if (maxParticipantes.Correcto == false) {
                        return Json(new { Message = JsonConvert.SerializeObject(maxParticipantes.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject(HorasSeleccinadas.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        [HttpPost]
        public JsonResult ObtenerHorariosClase(int IdDia, int IdClase)
        {
            try
            {
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerHorarios(IdDia, IdClase);

                if ((Actividades.Correcto))
                {
                    object[] salida = new object[1] { Actividades.Salida };
                    string datos_final = JsonConvert.SerializeObject(salida);
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
        public JsonResult ObtenerCuposMaximos(int IdClase)
        {
            try
            {
                ConsultasExternas.RespuestaSQL maxParticipantes = ConsultasExternas.ConsultasCalendario.CuposMaximosPorClase(IdClase, true);

                if (maxParticipantes.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject(maxParticipantes.Salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(maxParticipantes.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        [HttpPost]
        public JsonResult GuardarCambiosClases(int idClase, int maxparticipante, int idDia, string Horarios)
        {
            Object[] horarios = JsonConvert.DeserializeObject<object[]>(Horarios);

            try
            {
                ConsultasExternas.RespuestaSQL updateParticipantes = ConsultasExternas.ConsultasCalendario.ModificarParticipantesPorClase(idClase, maxparticipante);

                if (updateParticipantes.Correcto)
                {
                    ConsultasExternas.RespuestaSQL updateHorasClases = ConsultasExternas.ConsultasCalendario.DeshabilitarHorasClasesDia(idClase, idDia);
                    if (updateHorasClases.Correcto)
                    {
                        for (int i = 0; i < horarios.Length; i++)
                        {
                            int id_horario = int.Parse(horarios[i].ToString());
                            ConsultasExternas.RespuestaSQL idCalendario = ConsultasExternas.ConsultasCalendario.ExisteEnCalendario(idClase, idDia, id_horario);
                            if (idCalendario.Correcto)
                            {
                                if (idCalendario.Salida.Length > 0)
                                {
                                    ConsultasExternas.ConsultasCalendario.HabilitarHorasClasesDia(idClase, idDia, id_horario);
                                }
                                else
                                {
                                    int ubicacion = 1; //deberia tener una ubicacion
                                    ConsultasExternas.ConsultasCalendario.CrearCalendario(idClase, ubicacion, id_horario, idDia);
                                }
                            }
                        }
                        return Json(new { Message = JsonConvert.SerializeObject(""), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(updateHorasClases.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }

                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(updateParticipantes.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerUsuarios()
        {
            try
            {
                ConsultasExternas.RespuestaSQL usuarios = ConsultasExternas.Consultas.ObtenerUsuarios();

                if (usuarios.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject(usuarios.Salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(usuarios.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult ObtenerUsuario(string usuario)
        {
            try
            {
                ConsultasExternas.RespuestaSQL usuarios = ConsultasExternas.Consultas.ObtenerUsuario(usuario);

                if (usuarios.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject(usuarios.Salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(usuarios.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ActualizarUsuario(String nombreUsuario, String nombre, String apellidoPaterno, String apellidoMaterno, String Ntelefono, String NtelefonoEmergencia, String email, String Enfermedad, String password, String password_confirmacion, String Perfil, bool Socio, bool soloInstalaciones, bool bloqueado, String empresa, int idTipoSocio, String tipoSocio)
        {
            Boolean correcto_contraseña = true;
            Boolean correcto_usuario = true;
            Boolean correcto_perfil = true;
            Boolean correcto_acceso = true;
            Boolean correcto_socio = true;
            try
            {
                if (password.Equals("") == false)
                {
                    if (password.Equals(password_confirmacion))
                    {
                        correcto_contraseña = ConsultasExternas.Consultas.ActualizarContrasena(nombreUsuario, password);

                    }
                }
                //actualiazilar usuarios
                correcto_usuario = ConsultasExternas.Consultas.ActualizarUsuario(nombreUsuario, nombre, apellidoPaterno, apellidoMaterno, Ntelefono, NtelefonoEmergencia, email, Enfermedad, soloInstalaciones, bloqueado);
                //actualizar privilegios
                correcto_perfil = ConsultasExternas.Consultas.ActualizarPerfil(nombreUsuario, Perfil);
                correcto_acceso = ConsultasExternas.Consultas.ReiniciarPrivilegios(nombreUsuario);
                correcto_socio = ConsultasExternas.Consultas.ActualizarSocioEmpresa(nombreUsuario, Socio, empresa, idTipoSocio, tipoSocio);// nueva funcion
                //correcto_socio = ConsultasExternas.Consultas.ActualizarSocio(nombreUsuario, Socio); antigua
                if (correcto_acceso)
                {
                    if(soloInstalaciones == false)
                    {
                        correcto_acceso = ConsultasExternas.Consultas.CrearPrivilegiosIniciales(nombreUsuario);
                    }
                    else
                    {
                        correcto_acceso = ConsultasExternas.Consultas.CrearPrivilegiosIniciales(nombreUsuario,"soloReservas");
                    }
                        
                    
                    switch (Perfil)
                    {
                        case "Administrador":
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 8); //ver reservas
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 9); //gestionar  clases
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 10); //administrar usuarios
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 12); //administrar clases
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 13); //asistencia
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 15); //asignacion de profesores
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 17); //reserva de clases dirigidas
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 18); //tranferencias
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 20); //certificado
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 21); //socios
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 23); //reporte mensualidad
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 24); //administrar instalacion
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 26); //gestionar instalacion
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 28); //tranferencias instalacion
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 29); //calendario
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 30); //estadisticas
                            break;
                        case "Guardia":
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 8); //ver reservas
                            break;
                        case "Profesor":
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 8); //ver reservas
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 13); //asistencia
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 20); //certificado
                            correcto_acceso = ConsultasExternas.Consultas.agregarPrivilegios(nombreUsuario, 9); //gestionar  clases
                            break;
                    }
                }

                if ((correcto_contraseña) && (correcto_usuario) && (correcto_perfil) && (correcto_acceso))
                {
                    string datos_final = JsonConvert.SerializeObject(true);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error en Actualizar Perfil de Usuario"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult EnviarCorreosCancelacion(int dia, int mes, int year, int idCalendario, string motivo, string nombreClase, string horarioClase)
        {
            motivo = motivo.Trim();
            motivo = motivo.ToLower();
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                //obtener reservas
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.ObtenerReservas(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                if (Actividades.Correcto)
                {
                    //notificar cancelacioon
                    Boolean CorreosEnviados = ConsultasExternas.Correos.EnviarCorreo(Actividades.Salida, nombreClase, horarioClase, fecha);
                    if (CorreosEnviados)
                    {
                        //cancelar clase en base de datos
                        ConsultasExternas.RespuestaSQL Cancelacion = ConsultasExternas.ConsultasCancelacionClases.IngresarCancelacion(idCalendario, Models.NumerosFormato.ObtenerFecha(fecha), motivo);
                        if (Cancelacion.Correcto)
                        {
                            string datos_final = JsonConvert.SerializeObject(Actividades.Salida);
                            return Json(new { Message = "", Correcto = true, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            return Json(new { Message = JsonConvert.SerializeObject(Cancelacion.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                        }
                        
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("No se pudo cancelar, Error al enviar notificación por correo"), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
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
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.EliminarReserva(idReserva, userName);

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
        public JsonResult AgregarNuevaClase(string nombreClase)
        {
            try
            {
                ConsultasExternas.RespuestaSQL clases = ConsultasExternas.ConsultasCalendario.ExisteClase(nombreClase);
                if (clases.Correcto)
                {
                    if(clases.Booleano == false)
                    {
                        clases = ConsultasExternas.ConsultasCalendario.AgregarClase(nombreClase);
                        if (clases.Correcto)
                        {
                            clases = ConsultasExternas.ConsultasCalendario.ObtenerIDClase(nombreClase);
                            if (clases.Correcto)
                            {
                                clases = ConsultasExternas.ConsultasCalendario.AgregarCantidadMaxParticipantesClase(15, clases.SalidaEntero);
                                if (clases.Correcto)
                                {
                                    return Json(new { Message = "", Correcto = true, Existe = false, JsonRequestBehavior.AllowGet });
                                }
                            }
                            return Json(new { Message = "Error Interno En Realizar La Operación", Correcto = false, Existe = false, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            return Json(new { Message = "", Correcto = false, Existe = false, JsonRequestBehavior.AllowGet });
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
            catch(Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ActualizarAsistenciaClase(int idReserva, bool asistencia)
        {
            try
            {
                ConsultasExternas.RespuestaSQL clases = ConsultasExternas.ConsultasCalendario.ActualizarAsistencia(idReserva, asistencia);
                if (clases.Correcto)
                {
                    return Json(new { Message = "", Correcto = true, Existe = false, JsonRequestBehavior.AllowGet });
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
        public JsonResult ObtenerProfesoresClase(int IdCalendario)
        {
            try
            {
                ConsultasExternas.RespuestaSQL Profesores = ConsultasExternas.ConsultaProfesores.ObtenerProfesores();
                ConsultasExternas.RespuestaSQL ProfesorSeleccionado = ConsultasExternas.ConsultaProfesores.ObtenerProfesor(IdCalendario);

                if ((Profesores.Correcto) && (ProfesorSeleccionado.Correcto))
                {
                    object[] salida = new object[2] { Profesores.Salida, ProfesorSeleccionado.Salida };
                    string datos_final = JsonConvert.SerializeObject(salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    if(Profesores.Correcto == false)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(Profesores.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject(ProfesorSeleccionado.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult AsignarProfesor(int IdCalendario, string userName)
        {
            try
            {
                ConsultasExternas.RespuestaSQL Profesores = ConsultasExternas.ConsultaProfesores.AsignarProfesor(IdCalendario, userName);
                if ((Profesores.Correcto))
                {
                    string datos_final = JsonConvert.SerializeObject("");
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(Profesores.Error), Correcto = false, JsonRequestBehavior.AllowGet });   
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult PagarMensualidadManual(string rut, int mes, int year, int monto)
        {
            try
            {
                string ordenCompra = mes + "_" + year + "_" + rut;
                string detalleTarjeta = "Manual";
                string codTransaccion = "Manual";
                string mes_string = mes.ToString();
                string dia_string = DateTime.Now.ToString("dd");
                if (mes < 10) { mes_string = "0" + mes_string; }
                string fechaAutoriza = mes_string + dia_string;
                string fechaTransa = Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now);
                string token = CreateMD5(ordenCompra+ fechaTransa);
                ConsultasExternas.RespuestaSQL Transaccion = ConsultasExternas.ConsultasMensualidades.IngresarTransaccion(rut, ordenCompra, detalleTarjeta, monto, 0, 0, codTransaccion, fechaAutoriza, fechaTransa, "MANUAL", "MANUAL", "AUTHORIZED", token);
                if (Transaccion.Correcto)
                {
                    ConsultasExternas.RespuestaSQL id_transaccion = ConsultasExternas.ConsultasMensualidades.ObtenerIDTransaccion(rut, token, ordenCompra);
                    if (id_transaccion.Correcto)
                    {
                        Object[] idTransaccion = id_transaccion.Salida;
                        int idTransaccionAux = int.Parse(idTransaccion[0].ToString());
                        ConsultasExternas.RespuestaSQL mensualidad = ConsultasExternas.ConsultasMensualidades.IngresarMensualidad(rut, mes, year, idTransaccionAux);
                        if (mensualidad.Correcto)
                        {
                            string datos_final = JsonConvert.SerializeObject("");
                            return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            return Json(new { Message = JsonConvert.SerializeObject(mensualidad.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                        }
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(id_transaccion.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(Transaccion.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult BuscarMensualidades(string usuario)
        {
            try
            {
                ConsultasExternas.RespuestaSQL usuarios = ConsultasExternas.ConsultasMensualidades.Mensualidades(usuario);

                if (usuarios.Correcto)
                {
                    string datos_final = JsonConvert.SerializeObject(usuarios.Salida);
                    return Json(new { Message = datos_final, Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(usuarios.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult TieneCuposDisponibles(int dia, int mes, int year, int idCalendario, string rut)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);

                ConsultasExternas.RespuestaSQL CuposMax = ConsultasExternas.ConsultasCalendario.CuposMaximosPorClase(idCalendario);
                // ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, fecha.ToString("yyyy-MM-dd"));
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, Models.NumerosFormato.ObtenerFecha(fecha));
                //ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(Usuario.Rut, fecha.ToString("yyyy-MM-dd"), idCalendario);
                ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(rut, Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);
                ConsultasExternas.RespuestaSQL EstadoMensualidad = MensualidadPagada(rut, dia, mes, year);
                //ConsultasExternas.RespuestaSQL EstadoCancelacion = ConsultasExternas.ConsultasCancelacionClases.estadoClase(fecha.ToString("yyyy-MM-dd"), idCalendario);
                ConsultasExternas.RespuestaSQL EstadoCancelacion = ConsultasExternas.ConsultasCancelacionClases.estadoClase(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);

                //si todas las consultas son correctas
                if ((CuposMax.Correcto) && (Actividades.Correcto) && (CupoReservadoClaseMismoDia.Correcto) && (EstadoMensualidad.Correcto) && (EstadoCancelacion.Correcto))
                {
                    Boolean MensualidadPagada = (Boolean)(EstadoMensualidad.Salida[0]);
                    int cantidadReservasUsuario = CupoReservadoClaseMismoDia.Salida.Length;
                    object[] resultadoCupoMax = (object[])CuposMax.Salida[0]; //obtengo la primera fila
                    object[] resultadosCupos = (object[])Actividades.Salida[0]; //obtengo la primera fila
                    int cantidadReservasActual = int.Parse(resultadosCupos[0].ToString()); //obtengo la primera celda 
                    int maximoReservas = int.Parse(resultadoCupoMax[0].ToString()); //obtengo la primera celda 
                    bool existeCupo = false;
                    bool yaReservoDia = false;
                    if (cantidadReservasUsuario > 0) { yaReservoDia = true; }
                    if (cantidadReservasActual <= maximoReservas) { existeCupo = true; }
                    Object[] salida = new Object[5] { existeCupo, cantidadReservasActual, maximoReservas, MensualidadPagada, yaReservoDia };
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

                ConsultasExternas.RespuestaSQL CuposMax = ConsultasExternas.ConsultasCalendario.CuposMaximosPorClase(idCalendario);
                // ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, fecha.ToString("yyyy-MM-dd"));
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasCalendario.CuposActuales(idCalendario, Models.NumerosFormato.ObtenerFecha(fecha));
                //ConsultasExternas.RespuestaSQL CupoReservadoClaseMismoDia = ConsultasExternas.ConsultasCalendario.UsuarioTieneReservas(Usuario.Rut, fecha.ToString("yyyy-MM-dd"), idCalendario);
                //ConsultasExternas.RespuestaSQL EstadoCancelacion = ConsultasExternas.ConsultasCancelacionClases.estadoClase(fecha.ToString("yyyy-MM-dd"), idCalendario);
                ConsultasExternas.RespuestaSQL EstadoCancelacion = ConsultasExternas.ConsultasCancelacionClases.estadoClase(Models.NumerosFormato.ObtenerFecha(fecha), idCalendario);

                //si todas las consultas son correctas
                if ((CuposMax.Correcto) && (Actividades.Correcto)  && (EstadoCancelacion.Correcto))
                {
                    object[] resultadoCupoMax = (object[])CuposMax.Salida[0]; //obtengo la primera fila
                    object[] resultadosCupos = (object[])Actividades.Salida[0]; //obtengo la primera fila
                    int cantidadReservasActual = int.Parse(resultadosCupos[0].ToString()); //obtengo la primera celda 
                    int maximoReservas = int.Parse(resultadoCupoMax[0].ToString()); //obtengo la primera celda 
                    bool existeCupo = false;
                    bool yaReservoDia = false;
                    if (cantidadReservasActual <= maximoReservas) { existeCupo = true; }
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
                    return Json(new { Message = JsonConvert.SerializeObject("Error Interno Al Realizar Consulta de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult CrearReserva(int dia, int mes, int year, int idCalendario, string Rut)
        {
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                ConsultasExternas.RespuestaSQL PuedeReservas = puedeReservar(dia, mes, year, idCalendario, Rut);
                if (PuedeReservas.Correcto)
                {
                    ConsultasExternas.RespuestaSQL ingresarCupos = ConsultasExternas.ConsultasCalendario.CrearReserva(Rut, Models.NumerosFormato.ObtenerFecha(fecha), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idCalendario);

                    ConsultasExternas.RespuestaSQL calendario = ConsultasExternas.ConsultasCalendario.ObtenerDatosCalendario(idCalendario);
                    if (calendario.Correcto)
                    {
                        if (calendario.Salida.Length > 0)
                        {
                            object[] arreglo = (object[])calendario.Salida[0];
                            string nombreClase = arreglo[1].ToString();
                            string horarioClase = arreglo[2].ToString();
                            DataTable usuario_sql = ConsultasExternas.Consultas.DatosUsuarios(Rut);
                            if (usuario_sql.TableName.Equals("ERROR") == false)
                            {
                                if(usuario_sql.Rows.Count > 0)
                                {
                                    string correo = usuario_sql.Rows[0]["Email"].ToString();
                                    string nombreAux = usuario_sql.Rows[0]["Nombre"].ToString(); 
                                    string appAux = usuario_sql.Rows[0]["ApellidoPaterno"].ToString(); 
                                    string apmAux = usuario_sql.Rows[0]["ApellidoMaterno"].ToString(); 
                                    string nombre = nombreAux + " " + appAux + " " + apmAux;
                                    ConsultasExternas.Correos.EnviarCorreoConfirmacionReserva(correo, nombreClase, horarioClase, fecha, nombre);
                                }
                            }
                        }
                    }


                    if (ingresarCupos.Correcto)
                    {
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
        public JsonResult CrearReservaDia(int dia, int mes, int year, int idCalendario, string rut, string nombre, string app, string apm, string correo, bool boolMonto, int monto)
        {
            Boolean Correcto = true;
            try
            {
                DateTime fecha = new DateTime(year, mes, dia);
                if (boolMonto)
                {
                    Correcto = PagarClaseManual(rut, dia, mes, year, monto);
                }
                if (Correcto)
                {
                    //crear usuario invitado en tabla usuario
                    if (ConsultasExternas.Consultas.DatosUsuarios(rut).Rows.Count == 0)
                    {
                        //creo usuario invitado
                        Correcto = ConsultasExternas.Consultas.CrearUsuarioInvitado(rut, nombre, app, apm, "000", "000", correo, "Invitado2022", "");
                    }
                    if (Correcto)
                    {
                        ConsultasExternas.RespuestaSQL ingresarCupos = ConsultasExternas.ConsultasCalendario.CrearReserva(rut, Models.NumerosFormato.ObtenerFecha(fecha), Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now), idCalendario);
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
                                    string nombreCompleto = nombre + " " + app + " " + apm;
                                    ConsultasExternas.Correos.EnviarCorreoConfirmacionReserva(correo, nombreClase, horarioClase, fecha, nombreCompleto);
                                }
                            }
                            return Json(new { Message = JsonConvert.SerializeObject(""), Correcto = true, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            return Json(new { Message = JsonConvert.SerializeObject("Ingreso de reserva de clase dirigida"), Correcto = false, JsonRequestBehavior.AllowGet });
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
        }

        [HttpPost]
        public JsonResult ObtenerTranferencias(int year, int mes)
        {
            try
            {
                DateTime fechaInicio = new DateTime(year, mes, 1);
                DateTime fechaTermino = fechaInicio.AddMonths(1);

                ConsultasExternas.RespuestaSQL transacciones = ConsultasExternas.ConsultasMensualidades.ObtenerTransacciones(Models.NumerosFormato.ObtenerFecha(fechaInicio), Models.NumerosFormato.ObtenerFecha(fechaTermino));
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
        public JsonResult ObtenerCertificadosBorrar()
        {
            try
            {
                var pathCarpeta = Server.MapPath("~/Certificados/");
                string salida = "";
                if (System.IO.Directory.Exists(pathCarpeta))
                {
                    var listaArchivos = System.IO.Directory.GetFiles(pathCarpeta);
                    salida = "numero de archivos "+ listaArchivos.Length;
                    for (var i=0; i < listaArchivos.Length; i++)
                    {
                        string nombreArchivo = listaArchivos[i].Substring(listaArchivos[i].LastIndexOf("certificado_"));
                        string rut = nombreArchivo.Substring(0, nombreArchivo.LastIndexOf("."));
                        rut = rut.Substring(nombreArchivo.IndexOf("_") + 1);
                        rut = rut.Split('_')[0];
                        salida = salida + "//  "+nombreArchivo;
                        Boolean correcto = ConsultasExternas.Consultas.ActualizarCertificado(nombreArchivo, rut);
                        if(correcto == false)
                        {
                            return Json(new { Message = JsonConvert.SerializeObject("ERROR LEER ARCHIVO"), Correcto = true, JsonRequestBehavior.AllowGet });
                        }

                    }
                     return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    ViewBag.listaArchivos = new string[0];
                }
                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerCertificados()
        {
            try
            {
                ConsultasExternas.RespuestaSQL respuesta =  ConsultasExternas.Consultas.ObtenerCertificadosUsuarios();
                if (respuesta.Correcto)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(respuesta.Salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(respuesta.Error), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        [HttpPost]
        public JsonResult ObtenerSocios()
        {
            try
            {
                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.Consultas.ObtenerSociosUsuarios();
                if (respuesta.Correcto)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(respuesta.Salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(respuesta.Error), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult CargarTipoSocio()
        {
            try
            {
                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.Consultas.CargarTipoSocio();
                if (respuesta.Correcto)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(respuesta.Salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(respuesta.Error), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult agregarNuevoSocio(string  rut, string idTipoSocio, string socio)
        {
            try
            {
                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.Consultas.CrearActualizarUsuario(rut, socio, idTipoSocio);
                if (respuesta.Correcto)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(respuesta.Salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(respuesta.Error), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }



        [HttpPost]
        public JsonResult eliminarSocio(string rut)
        {
            try
            {
                bool respuesta = ConsultasExternas.Consultas.ActualizarSocio(rut, false);
                if (respuesta)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(true), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(false), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }




        [HttpPost]
        public JsonResult ReporteMensualidadMesEspecifico(int year, int mes)
        {
            try
            {
                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.ConsultasMensualidades.ReporteMensualidadMesCorregidoDuplicados(year, mes);
                if (respuesta.Correcto)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(respuesta.Salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(respuesta.Error), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ReporteMensualidadYearEspecifico(int year)
        {
            try
            {
                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.ConsultasMensualidades.ReporteMensualidadYear(year);
                if (respuesta.Correcto)
                {
                    //diccionario
                    Dictionary<string, Object[]> personal = new Dictionary<string, object[]>();

                    for(int i=0; i < respuesta.Salida.Length; i++)
                    {
                        object[] fila = (object[])respuesta.Salida[i];
                        string rut_aux = fila[0].ToString();
                        string nombre_aux = fila[1].ToString();
                        nombre_aux = nombre_aux +" "+ fila[2].ToString();
                        nombre_aux = nombre_aux +" "+ fila[3].ToString();
                        if (personal.ContainsKey(rut_aux) == false){
                            object[] meses = new object[13] { nombre_aux, 0,0,0,0,0,0,0,0,0,0,0,0 };
                            personal.Add(rut_aux, meses);
                        }
                        object[] meses_personal = personal[rut_aux];
                        int mes = int.Parse(fila[4].ToString());
                        string monto = fila[5].ToString();
                        meses_personal[mes] = monto;
                    }


                    Object[] salida_final = new Object[personal.Count];
                    int contador = 0;
                    foreach (string rut in personal.Keys){
                        object[] filaNueva = personal[rut];
                        salida_final[contador] = filaNueva;
                        contador++;
                    }

                    return Json(new { Message = JsonConvert.SerializeObject(salida_final), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(respuesta.Error), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult BorrarMensualidadManual(string idTranferencia, string idMensualidad)
        {
            try
            {

                bool respuesta = ConsultasExternas.ConsultasMensualidades.EliminarMensualidad(idMensualidad);
                if (respuesta)
                {
                    respuesta = ConsultasExternas.ConsultasMensualidades.EliminarTranferencia(idTranferencia);
                    if (respuesta)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject(true), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    return Json(new { Message = JsonConvert.SerializeObject(false), Correcto = false, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(false), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerUsuariosRegistrados()
        {
            try
            {

                ConsultasExternas.RespuestaSQL respuesta1 = ConsultasExternas.ConsultasEstadisticas.ObtenerUsuariosRegistrados();
                ConsultasExternas.RespuestaSQL respuesta2 = ConsultasExternas.ConsultasEstadisticas.ObtenerUsuariosSociosRegistrados();
                ConsultasExternas.RespuestaSQL respuesta3 = ConsultasExternas.ConsultasEstadisticas.ObtenerUsuariosNoSociosRegistrados();
                if((respuesta1.Correcto)&&(respuesta2.Correcto)&&(respuesta3.Correcto))
                {
                    string usuariosRegistrados = Models.NumerosFormato.ObtenerNumeroFormato(respuesta1.SalidaEntero.ToString());
                    string usuariosSociosRegistrados = Models.NumerosFormato.ObtenerNumeroFormato(respuesta2.SalidaEntero.ToString());
                    string usuariosNoSociosRegistrados = Models.NumerosFormato.ObtenerNumeroFormato(respuesta3.SalidaEntero.ToString());
                    object salida = new object[3] { usuariosRegistrados, usuariosSociosRegistrados, usuariosNoSociosRegistrados };
                    return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject("En Realizar Consultas En Base de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerEstadisticasMensualidades( int mes, int year)
        {
            try
            {
                ConsultasExternas.RespuestaSQL respuesta1 = ConsultasExternas.ConsultasEstadisticas.ObtenerMensualidadesPagadas(mes, year);
                ConsultasExternas.RespuestaSQL respuesta2 = ConsultasExternas.ConsultasEstadisticas.ObtenerTotalMensualidadesPagadas(mes, year);
                ConsultasExternas.RespuestaSQL respuesta3 = ConsultasExternas.ConsultasEstadisticas.ObtenerClasesDirigidasPagadas(mes, year);
                ConsultasExternas.RespuestaSQL respuesta4 = ConsultasExternas.ConsultasEstadisticas.ObtenerTotalClasesDirigidasPagadas(mes, year);
                if ((respuesta1.Correcto) && (respuesta2.Correcto) && (respuesta3.Correcto) && (respuesta4.Correcto))
                {
                    string mensualidades = Models.NumerosFormato.ObtenerNumeroFormato(respuesta1.SalidaEntero.ToString());
                    string totalMensualidades = Models.NumerosFormato.ObtenerNumeroFormato(respuesta2.SalidaEntero.ToString());
                    string clasesDirigidas = Models.NumerosFormato.ObtenerNumeroFormato(respuesta3.SalidaEntero.ToString());
                    string TotalClasesDirigidas = Models.NumerosFormato.ObtenerNumeroFormato(respuesta4.SalidaEntero.ToString());
                    object salida = new object[4] { mensualidades, totalMensualidades, clasesDirigidas, TotalClasesDirigidas };
                    return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject("En Realizar Consultas En Base de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerReservasFechas(int diaInicio, int mesInicio, int yearInicio, int diaTermino, int mesTermino, int yearTermino , string clase)
        {
            try
            {
                DateTime fecha_inicio  = new DateTime(yearInicio, mesInicio, diaInicio);
                DateTime fecha_termino = new DateTime(yearTermino,mesTermino,diaTermino);

                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.ConsultasEstadisticas.ObtenerReservasDia(Models.NumerosFormato.ObtenerFecha(fecha_inicio), Models.NumerosFormato.ObtenerFecha(fecha_termino), clase);
                if ((respuesta.Correcto))
                {
                    object salida = respuesta.Salida;
                    return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject("En Realizar Consultas En Base de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerReservasFecha(int diaInicio, int mesInicio, int yearInicio, string clase)
        {
            try
            {
                DateTime fecha_inicio = new DateTime(yearInicio, mesInicio, diaInicio);

                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.ConsultasEstadisticas.ObtenerReservasDia(Models.NumerosFormato.ObtenerFecha(fecha_inicio), clase);
                if ((respuesta.Correcto))
                {
                    object salida = respuesta.Salida;
                    return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject("En Realizar Consultas En Base de Datos"), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }





        private ConsultasExternas.RespuestaSQL MensualidadPagada(string usuario, int dia, int mes, int year)
        {
            ConsultasExternas.RespuestaSQL esSocio = ConsultasExternas.ConsultasMensualidades.esSocio(usuario);
            if (esSocio.Correcto)
            {
                if (esSocio.Salida.Length > 0) //si es mayor esta pagado
                {
                    return new ConsultasExternas.RespuestaSQL(new object[1] { true });
                }
            }

            ConsultasExternas.RespuestaSQL respuestaSQL = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(usuario, mes, year);
            if (respuestaSQL.Correcto)
            {
                Object[] respuestas = respuestaSQL.Salida;
                if (respuestas.Length > 0) //si es mayor esta pagado
                {
                    return new ConsultasExternas.RespuestaSQL(new object[1] { true });
                }
                else
                {
                    if (Models.NumerosFormato.esPrimerDiaHabildelMes(dia, mes, year)) //hasta el primer dia habil del mes ()
                    {
                        mes = mes - 1;
                        if (mes == 0)
                        {
                            mes = 12;
                            year = year - 1;
                        }
                        respuestaSQL = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(usuario, mes, year);
                        if (respuestaSQL.Correcto)
                        {
                            respuestas = respuestaSQL.Salida;
                            if (respuestas.Length > 0) //si es mayor esta pagado
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


        [HttpPost]
        public JsonResult EliminarClase(int IdClase)
        {
            try
            {
                if (!IdClase.ToString().IsEmpty())
                {
                    Boolean Actividades = ConsultasExternas.Consultas.EliminarClaseBD(IdClase);
                    if (Actividades)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Clase eliminada"), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Error al elminar"), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Verificar clase seleccionada"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult ObtenerEmpresas()
        {
            try
            {
                
                ConsultasExternas.RespuestaSQL Actividades = ConsultasExternas.ConsultasEmpresas.ObtenerEmpresas();
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
        public JsonResult AgregarNuevaEmpresa(string nombreEmpresa)
        {
            try
            {
                ConsultasExternas.RespuestaSQL respuesta = ConsultasExternas.ConsultasEmpresas.AgregarNuevaEmpresa(nombreEmpresa);
                if (respuesta.Correcto)
                {
                    return Json(new { Message = JsonConvert.SerializeObject(respuesta.Salida), Correcto = true, JsonRequestBehavior.AllowGet });
                }
                return Json(new { Message = JsonConvert.SerializeObject(respuesta.Error), Correcto = false, JsonRequestBehavior.AllowGet });
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult ModificarEmpresa(string nombreEmpresa, int idEmpresa)
        {
            try
            {
                if (!nombreEmpresa.IsEmpty())
                {
                    Boolean respuesta = ConsultasExternas.ConsultasEmpresas.ModificarEmpresa(nombreEmpresa, idEmpresa);
                    if (respuesta)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Empresa modificada"), Correcto = true, JsonRequestBehavior.AllowGet });
                    } else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Error al modificar"), Correcto = false, JsonRequestBehavior.AllowGet });
                    }
                    
                } else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Campo no puede estar vacío"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult EliminarEmpresa(int idEmpresa)
        {
            try
            {
                if (!idEmpresa.ToString().IsEmpty())
                {
                    Boolean respuesta = ConsultasExternas.ConsultasEmpresas.EliminarEmpresa(idEmpresa);
                    if (respuesta)
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Empresa Eliminada"), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        return Json(new { Message = JsonConvert.SerializeObject("Error al eliminar"), Correcto = false, JsonRequestBehavior.AllowGet });
                    }

                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject("Error desconocido, contacte al administrador"), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                return Json(new { Message = JsonConvert.SerializeObject(exc.Message), Correcto = false, JsonRequestBehavior.AllowGet });
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

                //si todas las consultas son correctas
                if ((CuposMax.Correcto) && (Actividades.Correcto) && (CupoReservadoClaseMismoDia.Correcto) && (EstadoMensualidad.Correcto))
                {
                    Boolean MensualidadPagada = (Boolean)(EstadoMensualidad.Salida[0]);
                    int cantidadReservasUsuario = CupoReservadoClaseMismoDia.Salida.Length;
                    object[] resultadoCupoMax = (object[])CuposMax.Salida[0]; //obtengo la primera fila
                    object[] resultadosCupos = (object[])Actividades.Salida[0]; //obtengo la primera fila
                    int cantidadReservasActual = int.Parse(resultadosCupos[0].ToString()); //obtengo la primera celda 
                    int maximoReservas = int.Parse(resultadoCupoMax[0].ToString()); //obtengo la primera celda 
                    bool existeCupo = false;
                    bool yaReservoDia = false;
                    if (cantidadReservasUsuario > 0) { yaReservoDia = true; }
                    if (cantidadReservasActual <= maximoReservas) { existeCupo = true; }
                    if (MensualidadPagada == true)
                    {
                        if (yaReservoDia == false)
                        {
                            if (existeCupo == true)
                            {
                                return new ConsultasExternas.RespuestaSQL(new object[1] { true });
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


        private bool PagarClaseManual(string rut,int dia, int mes, int year, int monto)
        {
            try
            {
                string ordenCompra = dia+"_" + mes + "_" + year + "_" + rut+"_" + (new Random().Next(10).ToString()); ;
                string detalleTarjeta = "Manual";
                string codTransaccion = "Manual";
                string mes_string = mes.ToString();
                string dia_string = DateTime.Now.ToString("dd");
                if (mes < 10) { mes_string = "0" + mes_string; }
                string fechaAutoriza = mes_string + dia_string;
                string fechaTransa = Models.NumerosFormato.ObtenerFechaCompleta(DateTime.Now);
                string token = CreateMD5(ordenCompra + fechaTransa);
                ConsultasExternas.RespuestaSQL Transaccion = ConsultasExternas.ConsultasMensualidades.IngresarTransaccion(rut, ordenCompra, detalleTarjeta, monto, 0, 0, codTransaccion, fechaAutoriza, fechaTransa, "MANUAL", "MANUAL", "AUTHORIZED", token);
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





       

    }
}
