using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationTemplateBase.ConsultasExternas
{
    public  class RespuestaSQL
    {

        public Boolean Correcto { get; }
        public object[] Salida { get; set; }
        public string Error { get; }

        public int SalidaEntero { get; set; }

        public Boolean Booleano { get; }


        public RespuestaSQL(object[] salida)
        {
            Correcto = true;
            Salida = salida;
            Error = "";
        }

        public RespuestaSQL(string error)
        {
            Correcto = false;
            Salida = new Object[0];
            Error =error;
        }


        public RespuestaSQL(int salida)
        {
            Correcto = true;
            SalidaEntero = salida;
            Error = "";
        }


        public RespuestaSQL(Boolean salida)
        {
            Correcto = true;
            Booleano = salida;
            Error = "";
        }

    }
}
