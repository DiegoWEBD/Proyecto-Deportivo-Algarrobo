using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace WebApplicationTemplateBase.Models
{

    public class MenuIzquierdo
    {

        
        public Dictionary<string, ArrayList> MenuFinal { get; }
        public MenuIzquierdo(DataTable dt)
        {

            MenuFinal = new Dictionary<string, ArrayList>();
            if(dt != null)
            {
                for(int i = 0; i< dt.Rows.Count; i++)
                {
                    int id_seccion = int.Parse(dt.Rows[i]["seccion_id"].ToString());
                    string seccion = dt.Rows[i]["Seccion"].ToString();
                    int id_menu = int.Parse(dt.Rows[i]["ID"].ToString());
                    string menu = dt.Rows[i]["Menu"].ToString();
                    string controlador = dt.Rows[i]["Controlador"].ToString();
                    string metodo = dt.Rows[i]["Metodo"].ToString();
                    Boolean soloLectura = Boolean.Parse(dt.Rows[i]["SoloLectura"].ToString());
                    string icono = dt.Rows[i]["Icono"].ToString();


                    Menu menu_aux = new Menu(id_menu, menu, controlador, metodo, soloLectura, icono);
                    if (!MenuFinal.ContainsKey(seccion))
                    {
                        MenuFinal[seccion] = new ArrayList();
                    }
                    ArrayList aux_menus = MenuFinal[seccion];
                    aux_menus.Add(menu_aux);
                }
            }

        }

        public class Seccion
        {
            int Identificador { get; set; }
            String NombreSeccion { get; set; }

            public Seccion(int id, string nombre)
            {
                Identificador = id;
                NombreSeccion = nombre;
            }
        }


        public class Menu
        {
            public int Identificador { get; set; }

            public String NombreMenu { get; set; }

            public String NombreControlador { get; set; }

            public String NombreMetodo { get; set; }

            public Boolean SoloLectura { get; set; }

            public String Icono { get; set; }

            public Menu(int id, string nombreMenu, String nombreControlador, string nombreMetodo, string icono)
            {
                Identificador = id;
                NombreMenu = nombreMenu;
                NombreControlador = nombreControlador;
                NombreMetodo = nombreMetodo;
                SoloLectura = true;
                Icono = icono;
            }

            public Menu(int id, string nombreMenu, String nombreControlador, string nombreMetodo, Boolean soloLectura, string icono)
            {
                Identificador = id;
                NombreMenu = nombreMenu;
                NombreControlador = nombreControlador;
                NombreMetodo = nombreMetodo;
                SoloLectura = soloLectura;
                Icono = icono;
            }




        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// [0] Nombre Menu
        /// [1] Nombre Controlador
        /// [2] Nombre Metodos
        /// [3] Solo Lectura
        /// [4] Id Menu
        /// [5] Icono
        /// </returns>
        public Dictionary<string, Object[]> ObtenerMenu()
        {
            Dictionary<string, Object[]> salida = new Dictionary<string, object[]>();
            
            //para cada seccion
            foreach (var seccion in MenuFinal)
            {
                //obtengo el nombre de seccion
                string seccion_aux       = seccion.Key;
                //obtengo el conjunto de menu que la componen
                ArrayList array_menu     =  seccion.Value;
                //creo un arreglo con la cantidad de espacio de los submenu 
                Object[] menu_aux_object = new object[array_menu.Count];
                for(int i=0; i < array_menu.Count; i++)
                {
                    Menu menu_aux = (Menu)array_menu[i];
                    Object[] datos_menu_aux = new Object[6];
                    datos_menu_aux[0]  = menu_aux.NombreMenu;
                    datos_menu_aux[1]  = menu_aux.NombreControlador;
                    datos_menu_aux[2]  = menu_aux.NombreMetodo;
                    datos_menu_aux[3]  = menu_aux.SoloLectura;
                    datos_menu_aux[4]  = menu_aux.Identificador;
                    datos_menu_aux[5]  = menu_aux.Icono;
                    menu_aux_object[i] = datos_menu_aux;
                }
                salida.Add(seccion_aux, menu_aux_object);
            }
            return salida;

        }


    }






}
