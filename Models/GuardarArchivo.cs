using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationTemplateBase.Models
{
    public class GuardarArchivo
    {
        public bool Correcto { get; set; }
        string Confirmacion { get; set; }
        Exception exceptionSalida { get; set; }

        public GuardarArchivo()
        {
            Correcto = false;
        }

        public void subirArchivo(string ruta, HttpPostedFileBase archivo)
        {
            try
            {
                archivo.SaveAs(ruta);
                this.Correcto = true;
            }
            catch(Exception exp)
            {
                this.Correcto = false;
                this.exceptionSalida = exp;
            }
        }




    }
}