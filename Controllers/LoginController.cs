using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationTemplateBase.Models;

namespace WebApplicationTemplateBase.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.error = "";
            return View();
        }


        public ActionResult Index2()
        {
            ViewBag.error = "";
            return View();
        }


        public ActionResult NuevoUsuario()
        {
            ViewBag.error = "";
            ViewBag.Disabled = "disabled";
            ViewBag.EsEnfermedad = "";
            return View();
        }




        [HttpPost]
        public ActionResult Index(String usuario, String password)
        {
            try
            {
                Boolean correcto = ComprobarRut(usuario);
                if (correcto)
                {
                    //DataTable dt = ConsultasExternas.Consultas.Logear(usuario, password); Descomentar
                    DataTable dt = ConsultasExternas.Consultas.LogearSiempre(usuario);
                    if (dt.TableName.Equals("ERROR") == false)
                    {
                        //nombre es clave y debe existir un solo nombre
                        bool estado = ConsultasExternas.Consultas.estadoCuenta(usuario);
                        if (estado)
                        {
                            if (dt.Rows.Count == 1)
                            {
                                string nombre = dt.Rows[0]["Nombre"].ToString();
                                string apPaterno = dt.Rows[0]["ApellidoPaterno"].ToString();
                                string apMaterno = dt.Rows[0]["ApellidoMaterno"].ToString();
                                string rut = dt.Rows[0]["Rut"].ToString();
                                string correo = dt.Rows[0]["Email"].ToString();
                                string telefono = dt.Rows[0]["NTelefono"].ToString();


                                Models.Usuario Usuario = new Models.Usuario(rut, nombre, apPaterno, apMaterno, correo, telefono);
                                DataTable dt_menu = ConsultasExternas.Consultas.ObtenerMenu(usuario);
                                Usuario.SetMenu(dt_menu);
                                Session["Usuario"] = Usuario;
                                Session.Timeout = 20;
                                return RedirectToAction("Index", "Home");

                            }
                            ViewBag.nombreUsuario = usuario;
                            ViewBag.error = "Rut de Usuario o Contraseña Invalido";
                        }
                        else
                        {
                            ViewBag.nombreUsuario = usuario;
                            ViewBag.error = "Cuenta Bloqueada, Por Favor Contactar a Administrador";
                        }
                       
                        return View();
                    }
                    else
                    {
                        ViewBag.nombreUsuario = usuario;
                        ViewBag.error = "Error Interno en Conexión Con Servidor";
                        return View();
                    }
                }
                else
                {
                    ViewBag.nombreUsuario = usuario;
                    ViewBag.error = "Rut de Usuario Invalido";
                    return View();
                }
            }
            catch(Exception exp)
            {
                ViewBag.error = exp.Message;
                return View();
            }

        }


        [HttpPost]
        public ActionResult NuevoUsuario(String nombreUsuario, String nombre, String apellidoPaterno, String apellidoMaterno, DateTime fechaNacimiento, String Ntelefono, String NtelefonoEmergencia, String email, String Enfermedad, String password, String password_confirmacion, HttpPostedFileBase Certificado, string soloReservaInstalaciones)
        {
            try
            {
                /* variables importantes llevadas a minusculas */
                nombreUsuario = nombreUsuario.ToLower();
                email = email.ToLower();
                nombreUsuario = nombreUsuario.Trim();
                email = email.Trim();
                if (Enfermedad == null) { Enfermedad = ""; }
                Enfermedad = Enfermedad.Trim();
                Enfermedad = Enfermedad.ToLower();

                /*si tiene enfermedad o no*/

                ViewBag.EsEnfermedad = "";
                ViewBag.Disabled = "disabled";
                if (Enfermedad != "") { ViewBag.EsEnfermedad = "checked"; ViewBag.Disabled = ""; }

                /*variables de retorno*/
                ViewBag.nombreUsuario = nombreUsuario;
                ViewBag.nombre = nombre;
                ViewBag.apellidoPaterno = apellidoPaterno;
                ViewBag.apellidoMaterno = apellidoMaterno;
                ViewBag.Ntelefono = Ntelefono;
                ViewBag.NtelefonoEmergencia = NtelefonoEmergencia;
                ViewBag.correo = email;
                ViewBag.enfermedad = Enfermedad;
                ViewBag.fechaNacimiento = fechaNacimiento.ToString("yyyy-MM-dd");


                Boolean correcto = ComprobarRut(nombreUsuario);
                if (correcto)
                {
                    if (UsuarioDisponible(nombreUsuario))
                    {
                        if (EmailDisponible(email))
                        {
                            if(PasswordCoinciden(password, password_confirmacion))
                            {

                                if(Certificado != null)
                                {
                                    if (Certificado.ContentLength <= 4000000)
                                    {
                                        
                                        correcto = ConsultasExternas.Consultas.CrearUsuarioNuevo(nombreUsuario, nombre.Trim(), apellidoPaterno.Trim(), apellidoMaterno.Trim(), Ntelefono.Trim(), NtelefonoEmergencia.Trim(), email, password, Enfermedad, Models.NumerosFormato.ObtenerFechaCompleta(fechaNacimiento));
                                        SubirCertificado(Certificado, nombreUsuario);
                                        //subir certificado
                                        string nameCertificado = nombreCertificado(Certificado, nombreUsuario);
                                        correcto = ConsultasExternas.Consultas.ActualizarCertificado(nameCertificado, nombreUsuario);
                                        if (!correcto)
                                        {
                                            ViewBag.error = "Error en crear un nuevo usuario";
                                        }
                                        else
                                        {
                                            correcto = ConsultasExternas.Consultas.CrearPrivilegiosIniciales(nombreUsuario);
                                            if (correcto)
                                            {
                                                correcto = ConsultasExternas.Consultas.AgregarPerfil(nombreUsuario, "Usuario");
                                                if (correcto)
                                                {
                                                    ViewBag.status = "Usuario creado con éxito";
                                                    TempData["status"] = "Usuario creado con éxito";
                                                }
                                                else
                                                {
                                                    ViewBag.status = "Usuario creado con éxito, Pero no se concedieron privilegios";
                                                    TempData["status"] = "Usuario creado con éxito, Pero no se concedieron privilegios";
                                                }
                                                return RedirectToAction("Index", "Login");
                                            }
                                            else
                                            {
                                                ViewBag.status = "Usuario creado con éxito, Pero no se concedieron privilegios";
                                            }

                                        }
                                        return View();
                                    }
                                    else
                                    {
                                        ViewBag.error = "Certificado debe pesar menos de 4 MB";
                                        return View();
                                    }
                                }
                                else
                                {
                                    //aca ver si  solo es arriendo de instalacion
                                    if(soloReservaInstalaciones.Equals("FALSE"))
                                    {
                                        correcto = ConsultasExternas.Consultas.CrearUsuarioNuevo(nombreUsuario, nombre.Trim(), apellidoPaterno.Trim(), apellidoMaterno.Trim(), Ntelefono.Trim(), NtelefonoEmergencia.Trim(), email, password, Enfermedad, Models.NumerosFormato.ObtenerFechaCompleta(fechaNacimiento));
                                    }
                                    else
                                    {   //consulta solo para arrindo de instalaciones
                                        correcto = ConsultasExternas.Consultas.CrearUsuarioNuevoSoloInstalaciones(nombreUsuario, nombre.Trim(), apellidoPaterno.Trim(), apellidoMaterno.Trim(), Ntelefono.Trim(), NtelefonoEmergencia.Trim(), email, password, Enfermedad, Models.NumerosFormato.ObtenerFechaCompleta(fechaNacimiento));
                                    }
                                    
                                    if (!correcto)
                                    {
                                        ViewBag.error = "Error en crear un nuevo usuario";
                                    }
                                    else
                                    {
                                        correcto = ConsultasExternas.Consultas.CrearPrivilegiosIniciales(nombreUsuario, soloReservaInstalaciones);
                                        if (correcto)
                                        {
                                            correcto = ConsultasExternas.Consultas.AgregarPerfil(nombreUsuario, "Usuario");
                                            if (correcto)
                                            {
                                                ViewBag.status = "Usuario creado con éxito";
                                                TempData["status"] = "Usuario creado con éxito";
                                            }
                                            else
                                            {
                                                ViewBag.status = "Usuario creado con éxito, Pero no se concedieron privilegios";
                                                TempData["status"] = "Usuario creado con éxito, Pero no se concedieron privilegios";
                                            }
                                            return RedirectToAction("Index", "Login");
                                        }
                                        else
                                        {
                                            ViewBag.status = "Usuario creado con éxito, Pero no se concedieron privilegios";
                                        }

                                    }
                                    return View();
                                    
                                }

                            }
                            else
                            {
                                ViewBag.error = "Contraseña no coinciden";
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.error = "Email ya registrado";
                            return View();
                        }
                    }
                    else
                    {

                        ViewBag.error = "Rut ya registrado";
                        return View();
                    }
                }
                else
                {
                    ViewBag.error    = "Rut no tiene el formato correcto";
                    return View();
                }

            }
            catch(Exception exp)
            {
                ViewBag.error = exp.Message;
                return View();
            }

        }


        [HttpPost]
        public JsonResult TienePagadoOctubre(string rut)
        {
            try
            {
                ConsultasExternas.RespuestaSQL esSocio = ConsultasExternas.ConsultasMensualidades.esSocio(rut);
                if (esSocio.Correcto)
                {
                    if (esSocio.Salida.Length > 0) //si es mayor esta pagado
                    {
                        object[] salida = new object[3] { true, "", "" };
                        string datos_final = JsonConvert.SerializeObject(salida);
                        return Json(new { Message = JsonConvert.SerializeObject(datos_final), Correcto = true, JsonRequestBehavior.AllowGet });
                    }
                }

                ConsultasExternas.RespuestaSQL EstadoMensualidad_8 = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(rut, 9, 2022);
                ConsultasExternas.RespuestaSQL EstadoMensualidad_9 = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(rut, 10, 2022);
                ConsultasExternas.RespuestaSQL EstadoMensualidad_10 = ConsultasExternas.ConsultasMensualidades.MensualidadPagada(rut, 11, 2022);

                if (EstadoMensualidad_8.Correcto)
                {
                    Object[] respuestas_8 = EstadoMensualidad_8.Salida;
                    Object[] respuestas_9 = EstadoMensualidad_9.Salida;
                    Object[] respuestas_10 = EstadoMensualidad_10.Salida;
                    if ((respuestas_8.Length > 0)|| (respuestas_9.Length > 0)|| (respuestas_10.Length > 0)) //si esta pagado
                    {
                        object[] salida = new object[3] { true, "", "" };
                        string datos_final = JsonConvert.SerializeObject(salida);
                        return Json(new { Message = JsonConvert.SerializeObject(datos_final), Correcto = true, JsonRequestBehavior.AllowGet });
                    }

                    return Json(new { Message = JsonConvert.SerializeObject(EstadoMensualidad_8.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    return Json(new { Message = JsonConvert.SerializeObject(EstadoMensualidad_8.Error), Correcto = false, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception exc)
            {
                object[] salida = new object[3] { exc.Message, "", "" };
                return Json(new { Message = JsonConvert.SerializeObject(salida), Correcto = false, JsonRequestBehavior.AllowGet });
            }
        }


        public ActionResult RecuperarPassword()
        {
            return View();
        }


        private Boolean PasswordCoinciden(string password, string password_2)
        {
            if(password.Equals(password_2))
            {
                return true;
            }
            return false;
        }


        private Boolean UsuarioDisponible(string nombreUsuario)
        {

            ConsultasExternas.RespuestaSQL salida = ConsultasExternas.Consultas.NombreUsuarioDisponible(nombreUsuario);
            if (salida.Correcto)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }


        private Boolean EmailDisponible(string email)
        {
            ConsultasExternas.RespuestaSQL salida = ConsultasExternas.Consultas.CorreoDisponible(email);
            if (salida.Correcto)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private Boolean ComprobarRut(string rut)
        {
            //no contenga punto
            if(rut.Contains("."))
            {
                return false;
            }
            //contenga el guion
            if (rut.Contains("-"))
            {
                string numero            = rut.Split('-')[0];
                string digitoVerificador = rut.Split('-')[1];
                //comprobar que numero no exceda el maximo de caracteres
                //debe tener no mas de 8 caractereses y mas de 6
                if((numero.Length > 6)&&(numero.Length < 9)) {
                    //debe tener digito verificador y ser de un solo caracter
                    if ((digitoVerificador.Length > 0) &&(digitoVerificador.Length < 2))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }

            //return true;
        }



        private Boolean SubirCertificado(HttpPostedFileBase Certificado, string nombreUsuario)
        {
            /*almacenar fichero*/
            if (Certificado != null)
            {
                Models.GuardarArchivo guardarCertificado = new Models.GuardarArchivo();
                string ruta = Server.MapPath("~/Certificados/");
                string nombreCertificado = "certificado_" + nombreUsuario + "_" + DateTime.Now.ToString("dd-MM-yyyy");
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

        private string nombreCertificado(HttpPostedFileBase Certificado, string nombreUsuario)
        {
            string salida = "";
            /*almacenar fichero*/
            if (Certificado != null)
            {
                string nombreCertificado = "certificado_" + nombreUsuario + "_" + DateTime.Now.ToString("dd-MM-yyyy");
                string extensionCertificado = Certificado.FileName.Substring(Certificado.FileName.LastIndexOf("."));
                salida += nombreCertificado + extensionCertificado;
                
            }
            return salida;
        }


    }
}