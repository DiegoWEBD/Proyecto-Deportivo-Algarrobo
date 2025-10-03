using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebApplicationTemplateBase.Models
{
    public static  class NumerosFormato
    {

        static CultureInfo cultura = new CultureInfo("es-CL", false);
        //static string IdiomaBD = "dd-MM-yyyy"; //base de datos servidor
        static string IdiomaBD = "yyyy-MM-dd";
        static string IdiomaBDHora = "HH:mm:ss";


        public static string ObtenerNumeroFormato(string entrada)
        {
            try
            {
                double numero = double.Parse(entrada);
                string salida = numero.ToString("#,0.##", cultura);
                return salida;
            }
            catch(Exception)
            {
                return entrada;
            }
        }



        public static string ObtenerFecha(DateTime fecha)
        {
            return fecha.ToString(IdiomaBD);
        }

        public static string ObtenerFechaCompleta(DateTime fecha)
        {
            return fecha.ToString(IdiomaBD+" "+IdiomaBDHora);
        }


        public static Boolean esPrimerDiaHabildelMes(int dia, int mes, int year)
        {
            DateTime fecha = new DateTime(year, mes, dia);
            int dias_aux = dia;

            if(dia >= 5)
            {
                return false;
            }

            for(int i=1; i <= dia; i++)
            {
                DateTime dia_ = new DateTime(year, mes, i);
                int numeroDia = (int)(dia_.DayOfWeek);
                
                if (numeroDia  == 6) // si es sabado
                {
                    dias_aux = dias_aux - 1;
                }
                if (numeroDia == 0){ // si es domingo
                
                    dias_aux = dias_aux - 1;
                }
                if ((dia == 1) && (mes == 1)){ //si es año nuevo
                    dias_aux = dias_aux - 1;
                }
                if ((dia == 1) && (mes == 5)) //si es dia del trabajador
                {
                    dias_aux = dias_aux - 1;
                }
                if ((dia == 1) && (mes == 11)) //si es halloween
                {
                    dias_aux = dias_aux - 1;
                }

            }

            if(dias_aux <= 1)
            {
                return true;
            }
            

            return false;
            
        }


    }
}