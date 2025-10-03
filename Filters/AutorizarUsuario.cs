using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationTemplateBase.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =false)]
    public class AutorizarUsuario : AuthorizeAttribute
    {
        private string NombreMetodo;


        /// <summary>
        /// mando un identificador del reporte que se desea ingresar y lo asigno a una variable global
        /// </summary>
        /// <param name="nombreMetodo"></param>
        public AutorizarUsuario(string nombreMetodo)
        {
            NombreMetodo = nombreMetodo;
            
        }



        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                var Usuario = HttpContext.Current.Session["Usuario"];
                if (Usuario != null)
                {
                    Models.Usuario Usuario2 = (Models.Usuario)Usuario;
                    //si usuario es invitado y nombre de metodo es cualquiera menos reservar desviar a login
                    if (Usuario2.Rut.Equals("rut invitado") == true)
                    {
                        HttpContext.Current.Session.Abandon();
                        filterContext.Result = new RedirectResult("/Login/Index");
                    }

                    bool tienePrivilegio = ConsultasExternas.Consultas.TienePrivilegios(Usuario2.Rut, this.NombreMetodo);
                    if (tienePrivilegio == false)
                    {
                        if(!NombreMetodo.Equals("ErrorPermiso"))
                        {
                            filterContext.Result = new RedirectResult("/Home/ErrorPermiso");
                        }
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Login/Index");
                }
            }
            catch(Exception)
            {

            }          
        }



        



    }
}