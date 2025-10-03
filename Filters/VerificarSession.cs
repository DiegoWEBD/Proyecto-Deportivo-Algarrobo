using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationTemplateBase.ConsultasExternas;
using WebApplicationTemplateBase.Models;

namespace WebApplicationTemplateBase.Filters
{

    public class VerificarSession : ActionFilterAttribute
    {

        /// <summary>
        /// filtro que se ejecuta antes de iniciar la pagina.
        /// si tiene una sesion iniciada lo deja entrar a la pagina seleccionada (antes de validar permiso)
        /// si no tiene sesion iniciada lo envia a la pagina de login
        /// este es el primer filtro para que una persona que no inicio sesion ,no entre a la plataforma 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);
                var Usuario = HttpContext.Current.Session["Usuario"];
                if (Usuario == null)
                {
                    if((filterContext.ActionDescriptor.ActionName.Equals("ReservarHoraDia"))|| filterContext.ActionDescriptor.ActionName.Equals("ReservarInstalacionInvitado"))
                    {
                        Models.Usuario UsuarioAUX = new Models.Usuario("rut invitado", "invitado", "invitado", "invitado", "invitado@invitado.cl", "Invitado");
                        UsuarioAUX.SetMenu(new DataTable());
                        HttpContext.Current.Session["Usuario"] = UsuarioAUX;
                    }
                    else
                    {
                        if (filterContext.Controller is Controllers.LoginController == false) //si NO estoy llamando desde el controlador login, aplico filtro
                        {
                            filterContext.HttpContext.Response.Redirect("/Login");
                        }
                    }
                }
                else
                {
                    if (filterContext.Controller is Controllers.LoginController == true) //si NO estoy llamando desde el controlador login, aplico filtro
                    {
                        //aca deberia filtrar si viene desde transbanck
                        filterContext.HttpContext.Response.Redirect("/Home");
                    }
                }
            }
            catch(Exception)
            {
                filterContext.HttpContext.Response.Redirect("/Login");
            }
        }




    }
}
