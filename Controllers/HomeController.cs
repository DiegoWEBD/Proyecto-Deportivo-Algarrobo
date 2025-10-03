using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationTemplateBase.Filters;

namespace WebApplicationTemplateBase.Controllers
{
    public class HomeController : Controller
    {



        [AutorizarUsuario(nombreMetodo: "-1")]
        public ActionResult Index()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;


            return View();
        }


        [AutorizarUsuario(nombreMetodo: "")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        [AutorizarUsuario(nombreMetodo: "")]
        public ActionResult Contact()
        {
            Models.Usuario Usuario = ((Models.Usuario)Session["Usuario"]);
            Models.MenuIzquierdo MenuIzquierdo = Usuario.Menu;
            ViewBag.menu = MenuIzquierdo.ObtenerMenu();
            ViewBag.userName = Usuario.Rut;
            ViewBag.nombre = Usuario.NombreUsuario;
            return View();

        }





        [AutorizarUsuario(nombreMetodo: "-1")]
        public ActionResult ErrorPermiso()
        {
            //Dictionary<string, object[]> menu = Models.MenuIzquierdo.ObtenerMenu();
            //ViewBag.menu = menu;
            string nombreUsuario = ((Models.Usuario)Session["Usuario"]).Rut;
            ViewBag.userName = nombreUsuario;
            return View();
        }



        public ActionResult CerrarSesion()
        {
            //Session["Usuario"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Login");
            
        }




    }
}