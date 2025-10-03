using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace WebApplicationTemplateBase.Models
{
    public class Usuario
    {
        public string Rut { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoPaternoUsuario { get; set; }
        public string ApellidoMaternoUsuario { get; set; }
        public string NTelefono { get; set; }
        public string NtelefonoEmergencia { get; set; }

        public string Correo { get; set; }



        public Boolean esAdministrador { get; set;  }

        public Models.MenuIzquierdo Menu { get; set; }

       




        public Usuario(string rut, string nombre, string apellidoPaterno, string apellidoMaterno, string correo, string telefono)
        {
            Rut        = rut;
            NombreUsuario   = nombre;
            ApellidoPaternoUsuario = apellidoPaterno;
            ApellidoMaternoUsuario = apellidoMaterno;
            Correo = correo;
            NTelefono = telefono;
        }



        public void SetMenu(DataTable menu)
        {
            Menu = new MenuIzquierdo(menu);
            
        }




    }
}