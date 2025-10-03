using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using WebApplicationTemplateBase.Models;

namespace WebApplicationTemplateBase.ConsultasExternas
{
    public class Consultas
    {


        public static DataTable Logear(string UserName, string Password)
        {
            string select = "SELECT * ";
            string from = "FROM Usuario"; ;
            string where = "WHERE(Rut like '" + UserName + "' ) and   (PWDCOMPARE('" + Password + "',[Password]) = 1) AND Deshabilitado = 0 ";

            DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
            return resultados;

        }


        public static bool estadoCuenta(string UserName)
        {
            string select = "SELECT *";
            string from = "FROM Usuario";
            string where = "WHERE (Rut like '" + UserName + "' ) and  Bloqueado = 'True' ";
            try
            {
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (resultados.TableName != "ERROR")
                {
                    if (resultados.Rows.Count > 0)
                    {
                        return false;

                    }
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static DataTable LogearSiempre(string UserName)
        {
            string select = "SELECT * ";
            string from = "FROM Usuario";
            string where = "WHERE(Rut like '" + UserName + "' AND Deshabilitado = 0 ) ";

            DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
            return resultados;

        }


        public static DataTable DatosUsuarios(string UserName)
        {
            string select = "SELECT * ";
            string from = "FROM Usuario";
            string where = "WHERE(Rut like '" + UserName + "' AND Deshabilitado = 0 ) ";

            DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
            return resultados;
        }


        public static DataTable ObtenerMenu(string usuario)
        {
            string select = " SELECT Seccion.ID as seccion_id, Seccion.Seccion, Menu.ID, Menu.Menu, Menu.Controlador, Menu.Metodo, SoloLectura, Icono ";
            string from = " FROM UsuarioMenu inner join Menu on UsuarioMenu.ID_Menu = Menu.ID inner join Seccion on Seccion.ID = Menu.IDSeccion ";
            string where = " WHERE(UsuarioMenu.ID_Usuario = '" + usuario + "'  )";

            DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
            return resultados;
        }



        public static ConsultasExternas.RespuestaSQL NombreUsuarioDisponible(string nombreUsuario)
        {
            string select = " SELECT UserName ";
            string from = " FROM Usuario";
            string where = " WHERE UserName = '" + nombreUsuario + "'   and Visita = 'False' AND Deshabilitado = 0 ";
            try
            {
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (resultados.TableName != "ERROR")
                {
                    if (resultados.Rows.Count > 0) {
                        return new ConsultasExternas.RespuestaSQL("Rut ya existe en los registros");
                    }
                    return new ConsultasExternas.RespuestaSQL(new object[0]);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(resultados.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL CorreoDisponible(string correo)
        {
            string select = " SELECT UserName ";
            string from = " FROM Usuario";
            string where = " WHERE Email = '" + correo + "' and Visita = 'False' AND Deshabilitado = 0";

            try
            {
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (resultados.TableName != "ERROR")
                {
                    if (resultados.Rows.Count > 0)
                    {
                        return new ConsultasExternas.RespuestaSQL("Correo ya existe en los registros");
                    }
                    return new ConsultasExternas.RespuestaSQL(new object[0]);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(resultados.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static bool CrearUsuarioNuevo(String nombreUsuario, String nombre, String apellidoPaterno, String apellidoMaterno, String Ntelefono, String NtelefonoEmergencia, String email, String password, String enfermedad, String fechaNac)
        {
            bool salida = false;
            string columnas = " Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, NtelefonoEmergencia, Rut, Password, UserName, Email, Enfermedad, FechaNacimiento";
            string valores = "'" + nombre + "','" + apellidoPaterno + "','" + apellidoMaterno + "','" + Ntelefono + "','" + NtelefonoEmergencia + "','" + nombreUsuario + "', PWDENCRYPT('" + password + "'),'" + nombreUsuario + "','" + email + "','" + enfermedad + "','" + fechaNac + "'";
            string tabla = "Usuario";

            string select = " SELECT UserName ";
            string from = " FROM Usuario";
            string where = " WHERE UserName = '" + nombreUsuario + "'   and Visita = 'True' ";

            DataTable dtVisita = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");

            if (dtVisita.TableName.Equals("ERROR") == false)
            {
                if (dtVisita.Rows.Count > 0)
                {
                    int filas_borradas = ConsultasExternas.ConsultasSQL.Delete("DELETE FROM Usuario ", " WHERE UserName = '" + nombreUsuario + "'  and Visita = 'True' ");
                }

                int filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filas_afectadas >= 1)
                {
                    salida = true;
                }
            }

            return salida;
        }

        /*
         * Elimina al usuario deshabilitando la cuenta
         */
        public static Boolean EliminarUsuarioBD(String rut)
        {
            try
            {
                string update = " UPDATE Usuario ";
                string set = " SET Deshabilitado = 1 ";
                string where = " WHERE UserName  = '" + rut + "' ";

                int filasModificadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasModificadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /*
         * Elimina al clase deshabilitando la clse
         */
        public static Boolean EliminarClaseBD(int IdClase)
        {
            try
            {
                string update = " UPDATE Clase ";
                string set = " SET Deshabilitado = 1 ";
                string where = " WHERE IdClases  = '" + IdClase + "' ";

                int filasModificadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasModificadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool CrearUsuarioNuevoSoloInstalaciones(String nombreUsuario, String nombre, String apellidoPaterno, String apellidoMaterno, String Ntelefono, String NtelefonoEmergencia, String email, String password, String enfermedad, String fechaNac)
        {
            bool salida = false;
            string columnas = " Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, NtelefonoEmergencia, Rut, Password, UserName, Email, Enfermedad, FechaNacimiento, soloReservaInstalacion";
            string valores = "'" + nombre + "','" + apellidoPaterno + "','" + apellidoMaterno + "','" + Ntelefono + "','" + NtelefonoEmergencia + "','" + nombreUsuario + "', PWDENCRYPT('" + password + "'),'" + nombreUsuario + "','" + email + "','" + enfermedad + "','" + fechaNac + "', 'True'";
            string tabla = "Usuario";

            string select = " SELECT UserName ";
            string from = " FROM Usuario";
            string where = " WHERE UserName = '" + nombreUsuario + "'   and Visita = 'True' ";

            DataTable dtVisita = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");

            if (dtVisita.TableName.Equals("ERROR") == false)
            {
                if (dtVisita.Rows.Count > 0)
                {
                    int filas_borradas = ConsultasExternas.ConsultasSQL.Delete("DELETE FROM Usuario ", " WHERE UserName = '" + nombreUsuario + "'  and Visita = 'True' ");
                }

                int filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filas_afectadas >= 1)
                {
                    salida = true;
                }
            }

            return salida;
        }



        public static bool CrearUsuarioInvitado(String nombreUsuario, String nombre, String apellidoPaterno, String apellidoMaterno, String Ntelefono, String NtelefonoEmergencia, String email, String password, String enfermedad)
        {
            bool salida = false;
            string columnas = " Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, NtelefonoEmergencia, Rut, Password, UserName, Email, Enfermedad, Visita";
            string valores = "'" + nombre + "','" + apellidoPaterno + "','" + apellidoMaterno + "','" + Ntelefono + "','" + NtelefonoEmergencia + "','" + nombreUsuario + "', PWDENCRYPT('" + password + "'),'" + nombreUsuario + "','" + email + "','" + enfermedad + "', 'True'";
            string tabla = "Usuario";
            int filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
            if (filas_afectadas >= 1)
            {
                salida = true;
            }
            return salida;
        }



        public static bool CrearPrivilegiosIniciales(String nombreUsuario)
        {
            int id_reservaHoras = 1;
            int id_reservaInstalaciones = 25;
            int id_pagoMensualidad = 5;
            int id_misReservas = 7;
            int id_misReservasInstalaciones = 31;
            try
            {
                string columnas = " ID_Usuario, ID_Menu , SoloLectura ";
                string valores = "'" + nombreUsuario + "'," + id_reservaHoras + ",'False'";
                string tabla = "UsuarioMenu";
                int filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filas_afectadas > 0)
                {
                    valores = "'" + nombreUsuario + "'," + id_pagoMensualidad + ",'False'";
                    filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                    if (filas_afectadas > 0)
                    {
                        valores = "'" + nombreUsuario + "'," + id_misReservas + ",'False'";
                        filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                        if (filas_afectadas > 0)
                        {
                            //ACTIVAR ESTA OPCION
                            valores = "'" + nombreUsuario + "'," + id_reservaInstalaciones + ",'False'";
                            filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                            if (filas_afectadas > 0)
                            {

                                valores = "'" + nombreUsuario + "'," + id_misReservasInstalaciones + ",'False'";
                                filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                                if (filas_afectadas > 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public static bool CrearPrivilegiosIniciales(String nombreUsuario, string soloReservaInstalaciones)
        {

            if(!soloReservaInstalaciones.Equals("FALSE"))
            {
                int id_reservaInstalaciones = 25;
                int id_MisReservaInstalaciones = 31;
                try
                {
                    string columnas = " ID_Usuario, ID_Menu , SoloLectura ";
                    string tabla = "UsuarioMenu";

                    string valores = "'" + nombreUsuario + "'," + id_reservaInstalaciones + ",'False'";
                    int filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                    if (filas_afectadas > 0)
                    {
                        valores = "'" + nombreUsuario + "'," + id_MisReservaInstalaciones + ",'False'";
                        filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);

                        return true;
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return CrearPrivilegiosIniciales(nombreUsuario);
            }

            
        }


        public static bool TienePrivilegios(string nombreUsuario, string idReporte)
        {
            string select = " SELECT ID_Usuario ";
            string from = " FROM UsuarioMenu ";
            string where = " WHERE ID_Usuario = '" + nombreUsuario + "' and ID_Menu = " + idReporte + " ";
            try
            {
                if (idReporte == "-1") {
                    return true;
                }
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (resultados.TableName != "ERROR")
                {
                    if (resultados.Rows.Count > 0)
                    {
                        return true;
                    }
                    return false;
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


        public static ConsultasExternas.RespuestaSQL ObtenerUsuarios()
        {
            string select = " SELECT Nombre, ApellidoPaterno, ApellidoMaterno, Usuario.Rut, NTelefono, NtelefonoEmergencia, Email, Perfil, Socio, Empresa ";
            string from = " FROM[Usuario] inner join[Perfil] on Usuario.UserName = Perfil.UserName left join Socios on Usuario.UserName = Socios.FK_UserName left join Empresas on Socios.FK_Empresa = Empresas.idEmpresa ";
            string where = " WHERE Usuario.Deshabilitado = 0 ";
            try
            {
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (resultados.TableName != "ERROR")
                {
                    object[] salida = new object[resultados.Rows.Count];
                    for (int i = 0; i < resultados.Rows.Count; i++)
                    {
                        string Nombre = resultados.Rows[i]["Nombre"].ToString();
                        string ApellidoPaterno = resultados.Rows[i]["ApellidoPaterno"].ToString();
                        string ApellidoMaterno = resultados.Rows[i]["ApellidoMaterno"].ToString();
                        string Rut = resultados.Rows[i]["Rut"].ToString();
                        string NTelefono = resultados.Rows[i]["NTelefono"].ToString();
                        string NtelefonoEmergencia = resultados.Rows[i]["NtelefonoEmergencia"].ToString();
                        string Email = resultados.Rows[i]["Email"].ToString();
                        string Perfil = resultados.Rows[i]["Perfil"].ToString();
                        string Socio = resultados.Rows[i]["Socio"].ToString();
                        string Empresa = resultados.Rows[i]["Empresa"].ToString();


                        object[] fila_aux = new object[10];
                        fila_aux[0] = Nombre;
                        fila_aux[1] = ApellidoPaterno;
                        fila_aux[2] = ApellidoMaterno;
                        fila_aux[3] = Rut;
                        fila_aux[4] = NTelefono;
                        fila_aux[5] = NtelefonoEmergencia;
                        fila_aux[6] = Email;
                        fila_aux[7] = Perfil;
                        fila_aux[8] = Socio;
                        fila_aux[9] = Empresa;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(resultados.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerUsuario(string rut)
        {
            string select = " SELECT Nombre, ApellidoPaterno, ApellidoMaterno, Usuario.Rut, NTelefono, NtelefonoEmergencia, Email, Perfil, Enfermedad, ISNULL(idSocio, 0) as Socio, soloReservaInstalacion, Bloqueado, idEmpresa, Empresa, FK_IdTipoSocio, TipoSocio ";
            string from = " FROM [Usuario] inner join[Perfil] on Usuario.UserName = Perfil.UserName left join Socios on Usuario.UserName = Socios.FK_UserName left join Empresas on Socios.FK_Empresa = Empresas.idEmpresa left join TipoSocio on Socios.FK_IdTipoSocio = TipoSocio.IdTipoSocio";
            string where = " WHERE Usuario.UserName = '" + rut + "' AND Usuario.Deshabilitado = 0 ";

            try
            {
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (resultados.TableName != "ERROR")
                {
                    object[] salida = new object[resultados.Rows.Count];
                    for (int i = 0; i < resultados.Rows.Count; i++)
                    {
                        string Nombre = resultados.Rows[i]["Nombre"].ToString();
                        string ApellidoPaterno = resultados.Rows[i]["ApellidoPaterno"].ToString();
                        string ApellidoMaterno = resultados.Rows[i]["ApellidoMaterno"].ToString();
                        string Rut = resultados.Rows[i]["Rut"].ToString();
                        string NTelefono = resultados.Rows[i]["NTelefono"].ToString();
                        string NtelefonoEmergencia = resultados.Rows[i]["NtelefonoEmergencia"].ToString();
                        string Email = resultados.Rows[i]["Email"].ToString();
                        string Perfil = resultados.Rows[i]["Perfil"].ToString();
                        string Socio = resultados.Rows[i]["Socio"].ToString();
                        string soloInstalaciones = resultados.Rows[i]["soloReservaInstalacion"].ToString();
                        string bloqueado = resultados.Rows[i]["Bloqueado"].ToString();
                        string idEmpresa = resultados.Rows[i]["IdEmpresa"].ToString();
                        string Empresa = resultados.Rows[i]["Empresa"].ToString();
                        string FK_IdTipoSocio = resultados.Rows[i]["FK_IdTipoSocio"].ToString();
                        string TipoSocio = resultados.Rows[i]["TipoSocio"].ToString();
                        string Enfermedad = "";
                        if (resultados.Rows[i]["Enfermedad"] != null)
                        {
                            Enfermedad = resultados.Rows[i]["Enfermedad"].ToString();
                        }

                        object[] fila_aux = new object[16];
                        fila_aux[0] = Nombre;
                        fila_aux[1] = ApellidoPaterno;
                        fila_aux[2] = ApellidoMaterno;
                        fila_aux[3] = Rut;
                        fila_aux[4] = NTelefono;
                        fila_aux[5] = NtelefonoEmergencia;
                        fila_aux[6] = Email;
                        fila_aux[7] = Perfil;
                        fila_aux[8] = Enfermedad;
                        fila_aux[9] = Socio;
                        fila_aux[10] = soloInstalaciones;
                        fila_aux[11] = bloqueado;
                        fila_aux[12] = idEmpresa;
                        fila_aux[13] = Empresa;
                        fila_aux[14] = FK_IdTipoSocio;
                        fila_aux[15] = TipoSocio;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(resultados.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static Boolean ActualizarContrasena(string usuario, string password)
        {
            try
            {
                string update = " UPDATE Usuario ";
                string set = " SET Password = PWDENCRYPT('" + password + "') ";
                string where = " WHERE UserName  = '" + usuario + "' ";

                int filasModificadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasModificadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static Boolean ActualizarUsuario(String nombreUsuario, String nombre, String apellidoPaterno, String apellidoMaterno, String Ntelefono, String NtelefonoEmergencia, String email, String enfermedad, bool SoloInstalaciones, bool bloqueado)
        {
            try
            {
                string update = " UPDATE Usuario ";
                string set = " SET Nombre = '" + nombre + "', ApellidoPaterno = '" + apellidoPaterno + "', ApellidoMaterno = '" + apellidoMaterno + "', NTelefono = '" + Ntelefono + "', NtelefonoEmergencia = '" + NtelefonoEmergencia + "',  Email = '" + email + "', Enfermedad = '" + enfermedad + "',soloReservaInstalacion = '"+SoloInstalaciones+ "', Bloqueado = '"+bloqueado+"'  ";
                string where = " WHERE UserName  = '" + nombreUsuario + "' ";

                int filasModificadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasModificadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return false;
            }
        }


        public static Boolean AgregarPerfil(string usuario, string perfil)
        {
            try
            {
                int filasModificadas = ConsultasExternas.ConsultasSQL.Insert("Perfil", "UserName, Perfil", "'" + usuario + "', '" + perfil + "' ");
                if (filasModificadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static Boolean ActualizarPerfil(string usuario, string perfil)
        {
            try
            {
                string update = " UPDATE Perfil ";
                string set = " SET Perfil = '" + perfil + "' ";
                string where = " WHERE UserName  = '" + usuario + "' ";

                int filasModificadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasModificadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public static bool agregarPrivilegios(String nombreUsuario, int idReporte)
        {
            try
            {

                string columnas = " ID_Usuario, ID_Menu , SoloLectura ";
                string valores = "'" + nombreUsuario + "'," + idReporte + ",'False'";
                string tabla = "UsuarioMenu";
                int filas_afectadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filas_afectadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }



        public static bool EliminarPrivilegios(String nombreUsuario, int idReporte)
        {
            try
            {
                string delete = "  DELETE FROM UsuarioMenu ";
                string where = " WHERE ID_Usuario = '" + nombreUsuario + "' AND  ID_Menu = "+idReporte+"";

                int filas_afectadas = ConsultasExternas.ConsultasSQL.Delete(delete, where);
                if (filas_afectadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ReiniciarPrivilegios(String nombreUsuario)
        {
            try
            {
                string delete = "  DELETE FROM UsuarioMenu ";
                string where = " WHERE ID_Usuario = '" + nombreUsuario + "'";

                int filas_afectadas = ConsultasExternas.ConsultasSQL.Delete(delete, where);
                if (filas_afectadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public static bool ActualizarSocio(string nombreUsuario, Boolean socio)
        {
            string consulta = "DELETE FROM Socios WHERE FK_UserName = '" + nombreUsuario + "';";
            if (socio == true)
            {
                consulta = "DECLARE @MAXRECORD VARCHAR(500) SET @MAXRECORD = '" + nombreUsuario + "' " +
                    " IF EXISTS(SELECT 1 FROM Socios WHERE FK_UserName like @MAXRECORD  ) BEGIN Select 'EXISTE' as existe END ELSE  BEGIN " +
                    " INSERT INTO Socios(FK_UserName, Rut, Socio ) VALUES(@MAXRECORD, @MAXRECORD, 'CMP'); END";
            }
            try
            {
                int filas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
                return true;
            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return false;
            }
        }

        public static bool ActualizarSocioEmpresa(string nombreUsuario, Boolean socio, String empresa, int idTipoSocio, String tipoSocio)
        {
            string consulta = "DELETE FROM Socios WHERE FK_UserName = '" + nombreUsuario + "';";
            if (socio == true)
            {
                consulta = "DECLARE @MAXRECORD VARCHAR(500) SET @MAXRECORD = '" + nombreUsuario + "' " +
                    " IF EXISTS(SELECT 1 FROM Socios WHERE FK_UserName like @MAXRECORD  ) BEGIN UPDATE Socios SET FK_Empresa = '" + empresa + "', FK_IdTipoSocio = " + idTipoSocio + ", Socio = '" + tipoSocio + "' WHERE Rut = '" + nombreUsuario + "' END ELSE  BEGIN " +
                    " INSERT INTO Socios(FK_UserName, Rut, Socio, Fk_IdTipoSocio, FK_Empresa ) VALUES(@MAXRECORD, @MAXRECORD, '" + tipoSocio + "', " + idTipoSocio + ", '" + empresa + "'); END";
            }
            try
            {
                int filas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
                return true;
            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return false;
            }
        }


        public static bool ActualizarCertificado(string nombreArchivo, string rut)
        {
            string consulta = "  IF EXISTS(SELECT 1 FROM Certificado WHERE (UserName = '" + rut + "')) BEGIN UPDATE Certificado " +
                              " SET UserName = '" + rut + "', NombreArchivo = '" + nombreArchivo + "'  WHERE UserName = '" + rut + "' END ELSE BEGIN " +
                              " INSERT INTO Certificado(UserName, NombreArchivo) VALUES('" + rut + "', '" + nombreArchivo + "'); END";

            int filasIngresadas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
            if (filasIngresadas > 0)
            {
                object[] salida = new object[0];
                return true;
            }
            return false;
        }


        public static ConsultasExternas.RespuestaSQL ObtenerCertificadosUsuarios()
        {
            string select = " SELECT Rut, Nombre, ApellidoPaterno, ApellidoMaterno, NombreArchivo ";
            string from = " FROM Certificado inner join Usuario on Certificado.UserName = Usuario.UserName ";
            string where = " ";

            try
            {
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (resultados.TableName != "ERROR")
                {
                    object[] salida = new object[resultados.Rows.Count];
                    for (int i = 0; i < resultados.Rows.Count; i++)
                    {
                        string Rut = resultados.Rows[i]["Rut"].ToString();
                        string Nombre = resultados.Rows[i]["Nombre"].ToString();
                        string ApellidoPaterno = resultados.Rows[i]["ApellidoPaterno"].ToString();
                        string ApellidoMaterno = resultados.Rows[i]["ApellidoMaterno"].ToString();
                        string nombreArchivo = resultados.Rows[i]["NombreArchivo"].ToString();


                        object[] fila_aux = new object[5];
                        fila_aux[0] = Rut;
                        fila_aux[1] = Nombre;
                        fila_aux[2] = ApellidoPaterno;
                        fila_aux[3] = ApellidoMaterno;
                        fila_aux[4] = nombreArchivo;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(resultados.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }


        }


        public static ConsultasExternas.RespuestaSQL ObtenerSociosUsuarios()
        {
            string select = " SELECT Socios.Rut, ISNULL(Nombre, 'zzzz' ) as Nombre2 , Nombre, ApellidoPaterno, ApellidoMaterno, Socio, FK_IdTipoSocio, TipoSocio ";
            string from = " FROM Socios left join Usuario on Socios.FK_UserName = Usuario.UserName left join TipoSocio on Socios.FK_IdTipoSocio = TipoSocio.IdTipoSocio ";
            string where = " ";

            try
            {
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "  ORDER by Nombre2 asc");
                if (resultados.TableName != "ERROR")
                {
                    object[] salida = new object[resultados.Rows.Count];
                    for (int i = 0; i < resultados.Rows.Count; i++)
                    {
                        string Rut = resultados.Rows[i]["Rut"].ToString();
                        string Nombre = resultados.Rows[i]["Nombre"].ToString();
                        string ApellidoPaterno = resultados.Rows[i]["ApellidoPaterno"].ToString();
                        string ApellidoMaterno = resultados.Rows[i]["ApellidoMaterno"].ToString();
                        string Socio = resultados.Rows[i]["Socio"].ToString();
                        string Nombre2 = resultados.Rows[i]["Nombre2"].ToString();
                        string FkIdTipoSocio = resultados.Rows[i]["FK_IdTipoSocio"].ToString();
                        string TipoSocio = resultados.Rows[i]["TipoSocio"].ToString();


                        object[] fila_aux = new object[8];
                        fila_aux[0] = Rut;
                        fila_aux[1] = Nombre;
                        fila_aux[2] = ApellidoPaterno;
                        fila_aux[3] = ApellidoMaterno;
                        fila_aux[4] = Socio;
                        fila_aux[5] = Nombre2;
                        fila_aux[6] = FkIdTipoSocio;
                        fila_aux[7] = TipoSocio;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(resultados.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }


        }


        public static ConsultasExternas.RespuestaSQL CargarTipoSocio()
        {
            string select = " SELECT IdTipoSocio, TipoSocio ";
            string from = " FROM TipoSocio ";
            string where = " WHERE Deshabilitado = 0 ";

            try
            {
                DataTable resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, "  ORDER BY TipoSocio asc");
                if (resultados.TableName != "ERROR")
                {
                    object[] salida = new object[resultados.Rows.Count];
                    for (int i = 0; i < resultados.Rows.Count; i++)
                    {
                        string IdTipoSocio = resultados.Rows[i]["IdTipoSocio"].ToString();
                        string TipoSocio = resultados.Rows[i]["TipoSocio"].ToString();


                        object[] fila_aux = new object[2];
                        fila_aux[0] = IdTipoSocio;
                        fila_aux[1] = TipoSocio;                    
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(resultados.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }


        }

        public static ConsultasExternas.RespuestaSQL CrearActualizarUsuario(string rut, string socio, string idTipoSocio)
        {
            string consulta = " IF EXISTS(SELECT 1 FROM Socios WHERE FK_UserName = '" + rut + "' ) BEGIN UPDATE Socios  ";
            consulta = consulta + " SET Socio = '" + socio + "', FK_IdTipoSocio = '" + idTipoSocio + "' WHERE FK_UserName = '" + rut + "' END ELSE BEGIN  ";
            consulta = consulta + " INSERT INTO Socios([FK_UserName], [Rut], [Socio], [FK_IdTipoSocio]) VALUES( '" + rut + "', '" + rut + "', '" + socio + "', '" + idTipoSocio + "'); END ";
            try
            {
                int filas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
                return new ConsultasExternas.RespuestaSQL(true);
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        //public static ConsultasExternas.RespuestaSQL CrearActualizarUsuario(string rut, string socio)
        //{
        //    string consulta = " IF EXISTS(SELECT 1 FROM Socios WHERE FK_UserName = '" + rut + "' ) BEGIN UPDATE Socios  ";
        //    consulta = consulta + " SET Socio = '" + socio + "' WHERE FK_UserName = '" + rut + "' END ELSE BEGIN  ";
        //    consulta = consulta + " INSERT INTO Socios([FK_UserName], [Rut], [Socio]) VALUES( '" + rut + "', '" + rut + "', '" + socio + "'); END ";
        //    try
        //    {
        //        int filas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
        //        return new ConsultasExternas.RespuestaSQL(true);
        //    }
        //    catch (Exception exp)
        //    {
        //        return new ConsultasExternas.RespuestaSQL(exp.Message);
        //    }
        //}




    }

    


    public class ConsultasCalendario
    {


        public static ConsultasExternas.RespuestaSQL ObtenerClases(int diaSemana)
        {
            try
            {
                //string select = " SELECT distinct[NombreClase], [Fk_Ubicacion], [Fk_Clase] ";
                string select = " SELECT distinct[NombreClase], [Fk_Clase] ";
                string from = " FROM[Calendario] inner join[Clase] on[Calendario].Fk_Clase = [Clase].IdClases ";
                string where = " WHERE[Fk_DiasSemana] = " + diaSemana + "  and Disponible = 'true' ";
                string order = " ORDER BY NombreClase ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreClase = dt_Actividades.Rows[i]["NombreClase"].ToString();
                        string Fk_Ubicacion = "Ubicacion";
                        string fk_Clase = dt_Actividades.Rows[i]["Fk_Clase"].ToString();

                        object[] fila_aux = new object[3];
                        fila_aux[0] = nombreClase;
                        fila_aux[1] = Fk_Ubicacion;
                        fila_aux[2] = fk_Clase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerClases()
        {
            try
            {
                string select = " SELECT IdClases, NombreClase ";
                string from = " FROM Clase ";
                string where = " WHERE Deshabilitado = 0 ";
                string order = " ORDER BY NombreClase ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idClase = dt_Actividades.Rows[i]["IdClases"].ToString();
                        string NombreClase = dt_Actividades.Rows[i]["NombreClase"].ToString();


                        object[] fila_aux = new object[2];
                        fila_aux[0] = idClase;
                        fila_aux[1] = NombreClase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        /// <summary>
        /// BORRAR BORRAR BORRAR
        /// esto no aplica al parecer, se cambio el nombre por si se llama 
        /// </summary>
        /// <param name="id_clase"></param>
        /// <returns></returns>
        public static ConsultasExternas.RespuestaSQL ObtenerHorarios2(int id_clase)
        {
            try
            {

                string select = " SELECT IdCalendario, Bloque, Horario, IdSemana, Semana ";
                string from = " FROM Calendario inner join Horario on Horario.IdHorario = Calendario.Fk_Horario inner join DiasSemana on DiasSemana.IdSemana = Calendario.Fk_DiasSemana ";
                string where = " WHERE Fk_Clase = " + id_clase + " ";
                string order = " ORDER BY IdSemana asc, Bloque ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        string bloque = dt_Actividades.Rows[i]["Bloque"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string idSemana = dt_Actividades.Rows[i]["IdSemana"].ToString();
                        string semana = dt_Actividades.Rows[i]["Semana"].ToString();



                        object[] fila_aux = new object[5];
                        fila_aux[0] = idCalendario;
                        fila_aux[1] = bloque;
                        fila_aux[2] = horario;
                        fila_aux[3] = idSemana;
                        fila_aux[4] = semana;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los horarios disponibles en la tabla Horarios
        /// </summary>
        /// <returns></returns>
        public static ConsultasExternas.RespuestaSQL ObtenerHorarios()
        {
            try
            {
                string select = " SELECT IdHorario, Horario, Bloque ";
                string from = " FROM Horario ";
                string where = " WHERE Bloque < 100 ";
                string order = "  ORDER BY Horario ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdHorario = dt_Actividades.Rows[i]["IdHorario"].ToString();
                        string Horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string Bloque = dt_Actividades.Rows[i]["Bloque"].ToString();

                        object[] fila_aux = new object[5];
                        fila_aux[0] = IdHorario;
                        fila_aux[1] = Horario;
                        fila_aux[2] = Bloque;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        /// <summary>
        /// Obtiene todos los horarios disponibles en la tabla Horarios
        /// </summary>
        /// <returns></returns>
        public static ConsultasExternas.RespuestaSQL ObtenerHorariosDia()
        {
            try
            {
                string select = " SELECT IdHorario, Horario, Bloque ";
                string from = " FROM Horario ";
                string where = " WHERE Bloque >= 100 ";
                string order = "  ORDER BY Bloque ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdHorario = dt_Actividades.Rows[i]["IdHorario"].ToString();
                        string Horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string Bloque = dt_Actividades.Rows[i]["Bloque"].ToString();

                        object[] fila_aux = new object[5];
                        fila_aux[0] = IdHorario;
                        fila_aux[1] = Horario;
                        fila_aux[2] = Bloque;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        /// <summary>
        ///  Obtiene los horarios para una clase y un dia de la semana
        /// </summary>
        /// <param name="diaSemana"></param>
        /// <param name="id_clase"></param>
        /// <returns></returns>
        public static ConsultasExternas.RespuestaSQL ObtenerHorarios(int diaSemana, int id_clase)
        {
            try
            {
                string select = " SELECT IdCalendario, Fk_Ubicacion,  Horario.IdHorario, Horario.Bloque, Horario.Horario ";
                string from = " FROM [Calendario]  inner join Horario on Horario.IdHorario = Calendario.Fk_Horario";
                string where = " WHERE(Fk_DiasSemana = " + diaSemana + ") AND (Fk_Clase = " + id_clase + ") AND (Calendario.Disponible = 'true')  ";
                string order = " ORDER BY Bloque";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        string idUbicacion = dt_Actividades.Rows[i]["Fk_Ubicacion"].ToString();
                        string idHorario = dt_Actividades.Rows[i]["IdHorario"].ToString();
                        string Bloque = dt_Actividades.Rows[i]["Bloque"].ToString();
                        string Horario = dt_Actividades.Rows[i]["Horario"].ToString();

                        object[] fila_aux = new object[5];
                        fila_aux[0] = idCalendario;
                        fila_aux[1] = idUbicacion;
                        fila_aux[2] = idHorario;
                        fila_aux[3] = Bloque;
                        fila_aux[4] = Horario;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL CuposActuales(int id_calendario, string fecha)
        {
            try
            {
                string select = " SELECT COUNT(IdCalendario) as cupos ";
                string from = " FROM Reserva inner join Calendario on Reserva.FkCalendario = Calendario.IdCalendario ";
                string where = " WHERE FechaReserva = '" + fecha + "' AND Calendario.IdCalendario = " + id_calendario + " ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string cupos = dt_Actividades.Rows[i]["cupos"].ToString();
                        object[] fila_aux = new object[1];
                        fila_aux[0] = cupos;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL CuposMaximosPorClase(int idCalendario)
        {
            try
            {
                string select = " SELECT Maximo  ";
                string from = " FROM ReservasMaxima inner join  Calendario on Calendario.Fk_Clase = ReservasMaxima.Fk_Clase ";
                string where = " WHERE Calendario.IdCalendario = " + idCalendario + " ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string MaximoCupos = dt_Actividades.Rows[i]["Maximo"].ToString();
                        object[] fila_aux = new object[1];
                        fila_aux[0] = MaximoCupos;
                        //fila_aux[0] = 33;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL CuposMaximosPorClase(int idClase, bool esClase)
        {
            try
            {

                string select = " SELECT[idReservasMaximaClase], [Fk_Clase], [Maximo] ";
                string from = " FROM ReservasMaxima ";
                string where = " WHERE Fk_Clase = " + idClase + " ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string id_reserva = dt_Actividades.Rows[i]["idReservasMaximaClase"].ToString();
                        string MaximoCupos = dt_Actividades.Rows[i]["Maximo"].ToString();
                        object[] fila_aux = new object[2];
                        fila_aux[0] = id_reserva;
                        fila_aux[1] = MaximoCupos;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL CrearReserva(string usuario, string fecha, string fecha_creacion, int calendario)
        {
            try
            {
                string tabla = " Reserva  ";
                string columnas = " FkCalendario, UserName, IngresoReserva, FechaReserva ";
                string valores = " " + calendario + ", '" + usuario + "' , '" + fecha_creacion + "' , '" + fecha + "'  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva :" + columnas + ":" + valores);
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL EliminarReserva(int idReserva, string userName)
        {
            try
            {
                string tabla = "DELETE FROM Reserva  ";
                string columnas = "  WHERE IdReserva = " + idReserva + " and UserName = '" + userName + "'; ";

                int filasEliminadas = ConsultasExternas.ConsultasSQL.Delete(tabla, columnas);
                if (filasEliminadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la cancelación :");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL EliminarReservaInstalacíon(int idReserva, string userName)
        {
            try
            {
                string tabla = "DELETE FROM ReservasInstalacion  ";
                string columnas = "  WHERE IdReserva = " + idReserva + " and UserName = '" + userName + "'; ";

                int filasEliminadas = ConsultasExternas.ConsultasSQL.Delete(tabla, columnas);
                if (filasEliminadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la cancelación :");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerReservas(string fecha, int id_clase, int id_calendario)
        {
            try
            {
                string select = "   SELECT IdReserva,  Nombre, ApellidoPaterno, ApellidoMaterno, NombreClase, FechaReserva, Horario, Asistencia, Usuario.Rut, FK_UserName, Socio ";
                string from = " FROM   [Algarrobo].[dbo].[Reserva] inner join Calendario on Reserva.FkCalendario = Calendario.IdCalendario " +
                              " inner join Clase on Calendario.Fk_Clase = Clase.IdClases " +
                              " inner join Horario on Horario.IdHorario = Calendario.Fk_Horario " +
                              " inner join DiasSemana on Calendario.Fk_DiasSemana = DiasSemana.IdSemana " +
                              " inner join Usuario on Usuario.UserName = Reserva.UserName" +
                              " left join Socios on Socios.Rut = Usuario.UserName ";

                string where = "  WHERE FechaReserva = '" + fecha + "' and Calendario.Fk_Clase = " + id_clase + " and Calendario.IdCalendario = " + id_calendario + " ";
                if (id_clase == -1)
                {
                    where = "  WHERE FechaReserva = '" + fecha + "'";
                }
                else
                {
                    if (id_calendario == -1)
                    {
                        where = "  WHERE FechaReserva = '" + fecha + "' and Calendario.Fk_Clase = " + id_clase + "";
                    }
                }

                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {

                        string idReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string clase = dt_Actividades.Rows[i]["NombreClase"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string asistencia = dt_Actividades.Rows[i]["Asistencia"].ToString();
                        string rut = dt_Actividades.Rows[i]["Rut"].ToString();
                        string socio = dt_Actividades.Rows[i]["Socio"].ToString();
                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();

                        object[] fila_aux = new object[11];
                        fila_aux[0] = nombre;
                        fila_aux[1] = apellidoPaterno;
                        fila_aux[2] = apellidoMaterno;
                        fila_aux[3] = clase;
                        fila_aux[4] = fecha_reserva;
                        fila_aux[5] = horario;
                        fila_aux[6] = idReserva;
                        fila_aux[7] = asistencia;
                        fila_aux[8] = rut;
                        fila_aux[9] = socio;

                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL ObtenerReservas(string fecha, int id_calendario)
        {
            try
            {
                string select = "   SELECT  IdReserva, Usuario.UserName, Nombre, ApellidoPaterno, ApellidoMaterno, NombreClase, FechaReserva, Horario, NTelefono, Email, Socio ";
                string from = " FROM   [Algarrobo].[dbo].[Reserva] inner join Calendario on Reserva.FkCalendario = Calendario.IdCalendario " +
                              " inner join Clase on Calendario.Fk_Clase = Clase.IdClases " +
                              " inner join Horario on Horario.IdHorario = Calendario.Fk_Horario " +
                              " inner join DiasSemana on Calendario.Fk_DiasSemana = DiasSemana.IdSemana " +
                              " inner join Usuario on Usuario.UserName = Reserva.UserName " +
                              " left join Socios on Socios.Rut = Usuario.UserName ";

                string where = "  WHERE FechaReserva = '" + fecha + "'  AND Calendario.IdCalendario = " + id_calendario + " ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string UserName = dt_Actividades.Rows[i]["UserName"].ToString();
                        string IdReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string clase = dt_Actividades.Rows[i]["NombreClase"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string NTelefono = dt_Actividades.Rows[i]["NTelefono"].ToString();
                        string Email = dt_Actividades.Rows[i]["Email"].ToString();
                        string socio = dt_Actividades.Rows[i]["Socio"].ToString();
                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[11];
                        fila_aux[0] = nombre;
                        fila_aux[1] = apellidoPaterno;
                        fila_aux[2] = apellidoMaterno;
                        fila_aux[3] = clase;
                        fila_aux[4] = fecha_reserva;
                        fila_aux[5] = horario;
                        fila_aux[6] = NTelefono;
                        fila_aux[7] = Email;
                        fila_aux[8] = IdReserva;
                        fila_aux[9] = UserName;
                        fila_aux[10] = socio;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL ObtenerReservas(string fecha, string userName)
        {
            try
            {
                string select = "   SELECT  IdReserva, Nombre, ApellidoPaterno, ApellidoMaterno, NombreClase, FechaReserva, Horario, NTelefono, Email ";
                string from = " FROM   [Algarrobo].[dbo].[Reserva] inner join Calendario on Reserva.FkCalendario = Calendario.IdCalendario " +
                              " inner join Clase on Calendario.Fk_Clase = Clase.IdClases " +
                              " inner join Horario on Horario.IdHorario = Calendario.Fk_Horario " +
                              " inner join DiasSemana on Calendario.Fk_DiasSemana = DiasSemana.IdSemana " +
                              " inner join Usuario on Usuario.UserName = Reserva.UserName ";

                string where = "  WHERE FechaReserva = '" + fecha + "'  AND  Usuario.UserName = '" + userName + "'";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string clase = dt_Actividades.Rows[i]["NombreClase"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string NTelefono = dt_Actividades.Rows[i]["NTelefono"].ToString();
                        string Email = dt_Actividades.Rows[i]["Email"].ToString();
                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[9];

                        fila_aux[0] = nombre;
                        fila_aux[1] = apellidoPaterno;
                        fila_aux[2] = apellidoMaterno;
                        fila_aux[3] = clase;
                        fila_aux[4] = fecha_reserva;
                        fila_aux[5] = horario;
                        fila_aux[6] = NTelefono;
                        fila_aux[7] = Email;
                        fila_aux[8] = idReserva;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }





        public static ConsultasExternas.RespuestaSQL ObtenerReservasInstalaciones(string fecha, string userName)
        {
            try
            {
                string select = "   SELECT  IdReserva, Nombre, ApellidoPaterno, ApellidoMaterno, NombreInstalacion, FechaReserva, Horario, NTelefono, Email  ";
                string from = " FROM   [Algarrobo].[dbo].[ReservasInstalacion] inner join CalendarioInstalacion on ReservasInstalacion.FkCalendario = CalendarioInstalacion.IdCalendario    " +
                              " inner join Instalacion on CalendarioInstalacion.Fk_Instalacion = Instalacion.idInstalacion  " +
                              " inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario  " +
                              " inner join DiasSemana on CalendarioInstalacion.Fk_DiasSemana = DiasSemana.IdSemana  " +
                              " inner join Usuario on Usuario.UserName = ReservasInstalacion.UserName   ";

                string where = "  WHERE FechaReserva >= '" + fecha + "'  AND  Usuario.UserName = '" + userName + "'";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string clase = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string NTelefono = dt_Actividades.Rows[i]["NTelefono"].ToString();
                        string Email = dt_Actividades.Rows[i]["Email"].ToString();
                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[9];

                        fila_aux[0] = nombre;
                        fila_aux[1] = apellidoPaterno;
                        fila_aux[2] = apellidoMaterno;
                        fila_aux[3] = clase;
                        fila_aux[4] = fecha_reserva;
                        fila_aux[5] = horario;
                        fila_aux[6] = NTelefono;
                        fila_aux[7] = Email;
                        fila_aux[8] = idReserva;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL UsuarioTieneReservas(string usuario, string fecha, int id_calendario)
        {
            try
            {
                string select = "   SELECT IdReserva  ";
                string from = " FROM Reserva inner join Calendario on Calendario.IdCalendario = Reserva.FkCalendario  ";
                string where = "  WHERE UserName = '" + usuario + "'  AND FechaReserva = '" + fecha + "'  AND  Calendario.Fk_Clase = (SELECT Fk_Clase FROM Calendario WHERE IdCalendario = " + id_calendario + ")";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();

                        object[] fila_aux = new object[1];
                        fila_aux[0] = IdReserva;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }





        public static ConsultasExternas.RespuestaSQL ModificarParticipantesPorClase(int idClase, int Cantidad)
        {
            try
            {

                string update = " UPDATE ReservasMaxima ";
                string set = " SET Maximo = " + Cantidad + " ";
                string where = " WHERE Fk_Clase = " + idClase + "  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL DeshabilitarHorasClasesDia(int idClase, int dia)
        {
            try
            {
                string update = " UPDATE Calendario ";
                string set = " SET Disponible =  'false' ";
                string where = " WHERE Fk_Clase = " + idClase + "  AND Fk_DiasSemana = " + dia + " ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasIngresadas >= 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



 
        public static ConsultasExternas.RespuestaSQL HabilitarHorasClasesDia(int idClase, int dia, int horario)
        {
            try
            {
                string update = " UPDATE Calendario ";
                string set = " SET Disponible =  'true' ";
                string where = " WHERE Fk_Clase = " + idClase + "  AND Fk_DiasSemana = " + dia + " AND  Fk_Horario = " + horario + "  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }





        public static ConsultasExternas.RespuestaSQL ExisteEnCalendario(int idClase, int dia, int idHorario)
        {
            try
            {
                string select = " SELECT IdCalendario   ";
                string from = " FROM [Calendario]  inner join Horario on Horario.IdHorario = Calendario.Fk_Horario  ";
                string where = " WHERE(Fk_DiasSemana = " + dia + ") AND (Fk_Clase = " + idClase + ")   and(IdHorario = " + idHorario + ") ";
                string order = " ORDER BY Bloque ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        object[] fila_aux = new object[1];
                        fila_aux[0] = IdCalendario;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL CrearCalendario(int idClase, int idUbicacion, int idHorario, int idDia)
        {
            try
            {
                string tabla = " Calendario ";
                string columnas = " Fk_Clase, Fk_Ubicacion, Fk_Horario, Fk_DiasSemana, Disponible ";
                string valores = " " + idClase + ", " + idUbicacion + " , " + idHorario + " , " + idDia + ", 'true'  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerDatosCalendario(int idCalendario)
        {
            try
            {
                string select = " SELECT IdCalendario, NombreClase, Horario ";
                string from = " FROM[Calendario] inner join Clase on Calendario.Fk_Clase = Clase.IdClases inner join Horario on Calendario.Fk_Horario = Horario.IdHorario  ";
                string where = "   WHERE IdCalendario = " + idCalendario + " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        string NombreClase = dt_Actividades.Rows[i]["NombreClase"].ToString();
                        string Horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        object[] fila_aux = new object[3];
                        fila_aux[0] = IdCalendario;
                        fila_aux[1] = NombreClase;
                        fila_aux[2] = Horario;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ExisteClase(string nombreClase)
        {
            try
            {
                string select = " SELECT IdClases, NombreClase ";
                string from = " FROM Clase ";
                string where = " WHERE NombreClase = '" + nombreClase + "'";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        return new ConsultasExternas.RespuestaSQL(true);
                    }
                    return new ConsultasExternas.RespuestaSQL(false);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerIDClase(string nombreClase)
        {
            try
            {
                string select = " SELECT IdClases ";
                string from = " FROM Clase ";
                string where = " WHERE NombreClase = '" + nombreClase + "'";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int idClase = -1;
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        idClase = int.Parse(dt_Actividades.Rows[0]["IdClases"].ToString());
                    }
                    return new ConsultasExternas.RespuestaSQL(idClase);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL AgregarClase(string nombreClase)
        {
            try
            {
                string tabla = " Clase ";
                string columnas = " NombreClase ";
                string valores = " '" + nombreClase + "'  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL AgregarCantidadMaxParticipantesClase(int cantidadMaxParticipantes, int idClase)
        {
            try
            {
                string tabla = " ReservasMaxima ";
                string columnas = " Fk_Clase , Maximo ";
                string valores = " " + idClase + ", " + cantidadMaxParticipantes + "  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ActualizarAsistencia(int idReserva, bool estadoAsistencia)
        {
            try
            {
                string update = " UPDATE Reserva ";
                string set = " SET Asistencia = '" + estadoAsistencia + "' ";
                string where = " WHERE IdReserva = " + idReserva + "  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la Actualización de Asistencia");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL AsistioClaseAnterior(string fecha, string idUsuario)
        {
            try
            {
                string select = " SELECT Asistencia ";
                string from = " FROM [Reserva] ";
                string where = " WHERE UserName = '"+ idUsuario + "' and FechaReserva = '"+ fecha + "' ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    bool asistencia =  false;
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        asistencia = (bool)dt_Actividades.Rows[0]["Asistencia"];
                    }
                    return new ConsultasExternas.RespuestaSQL(asistencia);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


    }


    public class ConsultasMensualidades
    {

        public static ConsultasExternas.RespuestaSQL IngresarTransaccion(string usuario, string OrdenCompra, string DetalleTarjeta, int Monto, int CantidadCuotas, decimal MontoCuotas, string CodigoAutorizacion, string FechaAutorizacion, string FechaTransaccion, string TipoTransaccion, string VCI, string Status, string Token)
        {
            try
            {
                string tabla = " Transaccion  ";
                string columnas = " UserName, OrdenCompra, DetalleTarjeta, Monto, CantidadCuotas, MontoCuotas, CodigoAutorizacion, FechaAutorizacion, FechaTransaccion, TipoTransaccion, VCI, Status, Token";
                string valores = " '" + usuario + "','" + OrdenCompra + "','" + DetalleTarjeta + "','" + Monto + "','" + CantidadCuotas + "','" + MontoCuotas + "','" + CodigoAutorizacion + "','" + FechaAutorizacion + "','" + FechaTransaccion + "','" + TipoTransaccion + "','" + VCI + "','" + Status + "','" + Token + "'";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se pudo almacenar la transacción");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerIDTransaccion(string usuario, string token, string ordenCompra)
        {
            try
            {
                string select = " SELECT IdTranferencias ";
                string from = " FROM Transaccion ";
                string where = " WHERE UserName = '" + usuario + "' and Token = '" + token + "' and OrdenCompra = '" + ordenCompra + "' ";
                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idTranferencia = dt_Actividades.Rows[i]["IdTranferencias"].ToString();
                        salida[i] = idTranferencia;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL IngresarMensualidad(string usuario, int mes, int year, int FkTransaccion)
        {
            try
            {
                string tabla = " Mensualidad ";
                string columnas = " [UserName], [Mes], [Year], [FkTransaccion] ";
                string valores = " '" + usuario + "'," + mes + "," + year + "," + FkTransaccion + " ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se pudo almacenar el registro de mensualidad");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL MensualidadPagada(string usuario, int mes, int year)
        {
            try
            {
                string select = " SELECT IdMensualidad ";
                string from = " FROM Mensualidad ";
                string where = " WHERE UserName = '" + usuario + "' AND Mes = " + mes + " AND year = " + year + " ";
                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdMensualidad = dt_Actividades.Rows[i]["IdMensualidad"].ToString();
                        salida[i] = IdMensualidad;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL Mensualidades(string usuario)
        {
            try
            {
                string select = " SELECT Mes, Year, Monto, FechaTransaccion, OrdenCompra, TipoTransaccion, IdTranferencias, IdMensualidad ";
                string from = " FROM Mensualidad inner join  Transaccion on Transaccion.IdTranferencias = Mensualidad.FkTransaccion ";
                string where = " WHERE Mensualidad.UserName = '" + usuario + "' ";
                string order = " ORDER BY Year desc, Mes desc ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string Mes = dt_Actividades.Rows[i]["Mes"].ToString();
                        string Year = dt_Actividades.Rows[i]["Year"].ToString();
                        string Monto = dt_Actividades.Rows[i]["Monto"].ToString();
                        Monto = Models.NumerosFormato.ObtenerNumeroFormato(Monto);
                        string OrdenCompra = dt_Actividades.Rows[i]["OrdenCompra"].ToString();
                        string FechaTransaccion = dt_Actividades.Rows[i]["FechaTransaccion"].ToString();
                        string TipoTransaccion = dt_Actividades.Rows[i]["TipoTransaccion"].ToString();
                        string idTransaccion = dt_Actividades.Rows[i]["IdTranferencias"].ToString();
                        string idMensualidad = dt_Actividades.Rows[i]["IdMensualidad"].ToString();
                        //if (FechaTransaccion.Contains(" "))
                        //{
                        //    FechaTransaccion = FechaTransaccion.Split(' ')[0];
                        //}

                        object[] fila_aux = new object[8];
                        fila_aux[0] = Mes;
                        fila_aux[1] = Year;
                        fila_aux[2] = Monto;
                        fila_aux[3] = FechaTransaccion;
                        fila_aux[4] = OrdenCompra;
                        fila_aux[5] = TipoTransaccion;
                        fila_aux[6] = idTransaccion;
                        fila_aux[7] = idMensualidad;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL esSocio(string usuario)
        {
            try
            {
                string select = " SELECT IdSocio ";
                string from = " FROM Socios ";
                string where = " WHERE FK_UserName = '" + usuario + "' ";
                string order = "";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idSocio = dt_Actividades.Rows[i]["IdSocio"].ToString();
                        object[] fila_aux = new object[1];
                        fila_aux[0] = idSocio;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL PrecioMensualidad(int year)
        {
            try
            {
                string select = " SELECT TOP(1) Monto ";
                string from = " FROM PrecioMensualidad ";
                string where = " WHERE Year <= " + year + "";
                string order = " ORDER BY YEAR DESC ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    int salida = 28000; //precio basico
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string Monto = dt_Actividades.Rows[i]["Monto"].ToString();
                        salida = int.Parse(Monto);
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL PrecioMensualidadProporcional(int dia)
        {
            try
            {
                string select = " SELECT Valor ";
                string from = " FROM MensualidadProporcional ";
                string where = " WHERE Dia = " + dia + "";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    int salida = 28000; //precio basico
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string Monto = dt_Actividades.Rows[i]["Valor"].ToString();
                        salida = int.Parse(Monto);
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerTransacciones(string fechaInicio, string fechaTermino)
        {
            try
            {
                string select = " SELECT FechaTransaccion, Monto, DetalleTarjeta, CodigoAutorizacion, TipoTransaccion,  Nombre, ApellidoPaterno,ApellidoMaterno, Rut, OrdenCompra ";
                string from = " FROM [Transaccion] inner join Usuario ON Usuario.UserName = Transaccion.UserName ";
                string where = " WHERE FechaTransaccion >= '" + fechaInicio + "' AND FechaTransaccion< '" + fechaTermino + "' ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string FechaTransaccion = dt_Actividades.Rows[i]["FechaTransaccion"].ToString();
                        string Monto = dt_Actividades.Rows[i]["Monto"].ToString();
                        Monto = Models.NumerosFormato.ObtenerNumeroFormato(Monto);
                        string DetalleTarjeta = dt_Actividades.Rows[i]["DetalleTarjeta"].ToString();
                        string CodigoAutorizacion = dt_Actividades.Rows[i]["CodigoAutorizacion"].ToString();
                        string TipoTransaccion = dt_Actividades.Rows[i]["TipoTransaccion"].ToString();
                        string Nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string ApellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string ApellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string Rut = dt_Actividades.Rows[i]["Rut"].ToString();
                        string OrdenCompra = dt_Actividades.Rows[i]["OrdenCompra"].ToString();
                        string HoraTransaccion = "";
                        if (FechaTransaccion.Contains(" "))
                        {
                            HoraTransaccion = FechaTransaccion.Split(' ')[1];
                            FechaTransaccion = FechaTransaccion.Split(' ')[0];
                        }

                        object[] fila_aux = new object[11];
                        fila_aux[0] = FechaTransaccion;
                        fila_aux[1] = HoraTransaccion;
                        fila_aux[2] = Monto;
                        fila_aux[3] = DetalleTarjeta;
                        fila_aux[4] = CodigoAutorizacion;
                        fila_aux[5] = TipoTransaccion;
                        fila_aux[6] = Nombre;
                        fila_aux[7] = ApellidoPaterno;
                        fila_aux[8] = ApellidoMaterno;
                        fila_aux[9] = Rut;
                        fila_aux[10] = OrdenCompra;

                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static Boolean ObtenerTransaccionRealizada(string usuario, string OrdenCompra, string DetalleTarjeta, int Monto, int CantidadCuotas, decimal MontoCuotas, string CodigoAutorizacion, string FechaAutorizacion, string FechaTransaccion, string TipoTransaccion, string VCI, string Status, string Token)
        {
            try
            {
                string select = " SELECT Token";
                string from = " FROM dbo.Transaccion ";
                string where = " WHERE UserName = '" + usuario + "' AND OrdenCompra = '" + OrdenCompra + "' AND DetalleTarjeta = '" + DetalleTarjeta + "'AND  Monto = '" + Monto + "' AND CantidadCuotas = '" + CantidadCuotas + "' AND MontoCuotas = '" + MontoCuotas + "' AND CodigoAutorizacion= '" + CodigoAutorizacion + "' AND FechaAutorizacion= '" + FechaAutorizacion + "' AND FechaTransaccion= '" + FechaTransaccion + "' AND TipoTransaccion = '" + TipoTransaccion + "' AND  VCI = '" + VCI + "' AND  Status = '" + Status + "' AND Token = '" + Token + "' ";
                string order = "  ";

                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        return true;
                    }

                    return false;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return false;
            }
        }


        public static async Task<Boolean>  ObtenerTransaccionRealizadaASYNC(string usuario, string OrdenCompra, string DetalleTarjeta, int Monto, int CantidadCuotas, decimal MontoCuotas, string CodigoAutorizacion, string FechaAutorizacion, string FechaTransaccion, string TipoTransaccion, string VCI, string Status, string Token)
        {
            try
            {
                string select = " SELECT Token";
                string from = " FROM dbo.Transaccion ";
                string where = " WHERE UserName = '" + usuario + "' AND OrdenCompra = '" + OrdenCompra + "' AND DetalleTarjeta = '" + DetalleTarjeta + "'AND  Monto = '" + Monto + "' AND CantidadCuotas = '" + CantidadCuotas + "' AND MontoCuotas = '" + MontoCuotas + "' AND CodigoAutorizacion= '" + CodigoAutorizacion + "' AND FechaAutorizacion= '" + FechaAutorizacion + "' AND FechaTransaccion= '" + FechaTransaccion + "' AND TipoTransaccion = '" + TipoTransaccion + "' AND  VCI = '" + VCI + "' AND  Status = '" + Status + "' AND Token = '" + Token + "' ";
                string order = "  ";

                DataTable dt_Actividades = await ConsultasExternas.ConsultasSQL.SelectASYNC(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        return true;
                    }

                    return false;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return false;
            }
        }

        public static ConsultasExternas.RespuestaSQL ReporteMensualidadMes(int year, int mes)
        {
            string select = "SELECT Mensualidad.UserName , Nombre, ApellidoPaterno, ApellidoMaterno, Monto ";
            string from = "FROM Mensualidad inner join Transaccion on Mensualidad.FkTransaccion = Transaccion.IdTranferencias inner join Usuario on Mensualidad.UserName = Usuario.UserName";
            string where = "WHERE(Year = " + year + ") and(Mes = " + mes + ")";
            string order = " ORDER BY Nombre ";

            DataTable dt_resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
            if (dt_resultados.TableName != "ERROR")
            {
                object[] salida = new object[dt_resultados.Rows.Count];
                for (int i = 0; i < dt_resultados.Rows.Count; i++)
                {
                    string Rut = dt_resultados.Rows[i]["UserName"].ToString();
                    string Nombre = dt_resultados.Rows[i]["Nombre"].ToString();
                    string ApellidoPaterno = dt_resultados.Rows[i]["ApellidoPaterno"].ToString();
                    string ApellidoMaterno = dt_resultados.Rows[i]["ApellidoMaterno"].ToString();
                    string Monto = dt_resultados.Rows[i]["Monto"].ToString();
                    Monto = Models.NumerosFormato.ObtenerNumeroFormato(Monto);

                    object[] fila_aux = new object[5];
                    fila_aux[0] = Rut;
                    fila_aux[1] = Nombre;
                    fila_aux[2] = ApellidoPaterno;
                    fila_aux[3] = ApellidoMaterno;
                    fila_aux[4] = Monto;
                    salida[i] = fila_aux;
                }
                return new ConsultasExternas.RespuestaSQL(salida);
            }
            else
            {
                return new ConsultasExternas.RespuestaSQL(dt_resultados.ExtendedProperties["Mensaje"].ToString());
            }

        }



        public static ConsultasExternas.RespuestaSQL ReporteMensualidadMesCorregidoDuplicados(int year, int mes)
        {
            string select = "  SELECT Mensualidad.UserName , Nombre, ApellidoPaterno, ApellidoMaterno, Monto , OrdenCompra, Token ";
            string from = "FROM Mensualidad inner join Transaccion on Mensualidad.FkTransaccion = Transaccion.IdTranferencias inner join Usuario on Mensualidad.UserName = Usuario.UserName";
            string where = "WHERE(Year = " + year + ") and(Mes = " + mes + ")";
            string order = " Group by Mensualidad.UserName,  Nombre, ApellidoPaterno, ApellidoMaterno, Monto ,OrdenCompra, Token ORDER BY Nombre ";

            DataTable dt_resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
            if (dt_resultados.TableName != "ERROR")
            {
                object[] salida = new object[dt_resultados.Rows.Count];
                for (int i = 0; i < dt_resultados.Rows.Count; i++)
                {
                    string Rut = dt_resultados.Rows[i]["UserName"].ToString();
                    string Nombre = dt_resultados.Rows[i]["Nombre"].ToString();
                    string ApellidoPaterno = dt_resultados.Rows[i]["ApellidoPaterno"].ToString();
                    string ApellidoMaterno = dt_resultados.Rows[i]["ApellidoMaterno"].ToString();
                    string Monto = dt_resultados.Rows[i]["Monto"].ToString();
                    Monto = Models.NumerosFormato.ObtenerNumeroFormato(Monto);

                    object[] fila_aux = new object[5];
                    fila_aux[0] = Rut;
                    fila_aux[1] = Nombre;
                    fila_aux[2] = ApellidoPaterno;
                    fila_aux[3] = ApellidoMaterno;
                    fila_aux[4] = Monto;
                    salida[i] = fila_aux;
                }
                return new ConsultasExternas.RespuestaSQL(salida);
            }
            else
            {
                return new ConsultasExternas.RespuestaSQL(dt_resultados.ExtendedProperties["Mensaje"].ToString());
            }

        }


        public static ConsultasExternas.RespuestaSQL ReporteMensualidadYear(int year)
        {
            string select = "SELECT Mensualidad.UserName , Nombre, ApellidoPaterno, ApellidoMaterno, Mes, Monto  ";
            string from = "FROM Mensualidad inner join Transaccion on Mensualidad.FkTransaccion = Transaccion.IdTranferencias inner join Usuario on Mensualidad.UserName = Usuario.UserName";
            string where = "WHERE(Year = " + year + ")";
            string order = " ORDER BY Mes, Nombre ";

            DataTable dt_resultados = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
            if (dt_resultados.TableName != "ERROR")
            {
                object[] salida = new object[dt_resultados.Rows.Count];
                for (int i = 0; i < dt_resultados.Rows.Count; i++)
                {
                    string Rut = dt_resultados.Rows[i]["UserName"].ToString();
                    string Nombre = dt_resultados.Rows[i]["Nombre"].ToString();
                    string ApellidoPaterno = dt_resultados.Rows[i]["ApellidoPaterno"].ToString();
                    string ApellidoMaterno = dt_resultados.Rows[i]["ApellidoMaterno"].ToString();
                    string Mes = dt_resultados.Rows[i]["Mes"].ToString();
                    string Monto = dt_resultados.Rows[i]["Monto"].ToString();
                    Monto = Models.NumerosFormato.ObtenerNumeroFormato(Monto);

                    object[] fila_aux = new object[6];
                    fila_aux[0] = Rut;
                    fila_aux[1] = Nombre;
                    fila_aux[2] = ApellidoPaterno;
                    fila_aux[3] = ApellidoMaterno;
                    fila_aux[4] = Mes;
                    fila_aux[5] = Monto;
                    salida[i] = fila_aux;
                }
                return new ConsultasExternas.RespuestaSQL(salida);
            }
            else
            {
                return new ConsultasExternas.RespuestaSQL(dt_resultados.ExtendedProperties["Mensaje"].ToString());
            }

        }




        public static ConsultasExternas.RespuestaSQL ObtenerTransaccionesInstalaciones(string fechaInicio, string fechaTermino)
        {
            try
            {
                string select = " SELECT FechaTransaccion, Monto, DetalleTarjeta, CodigoAutorizacion, TipoTransaccion,  Nombre, ApellidoPaterno,ApellidoMaterno, Rut, OrdenCompra ";
                string from = "  FROM TransaccionInstalacion inner join Usuario ON Usuario.UserName = TransaccionInstalacion.UserName ";
                string where = " WHERE FechaTransaccion >= '" + fechaInicio + "' AND FechaTransaccion< '" + fechaTermino + "' ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string FechaTransaccion = dt_Actividades.Rows[i]["FechaTransaccion"].ToString();
                        string Monto = dt_Actividades.Rows[i]["Monto"].ToString();
                        Monto = Models.NumerosFormato.ObtenerNumeroFormato(Monto);
                        string DetalleTarjeta = dt_Actividades.Rows[i]["DetalleTarjeta"].ToString();
                        string CodigoAutorizacion = dt_Actividades.Rows[i]["CodigoAutorizacion"].ToString();
                        string TipoTransaccion = dt_Actividades.Rows[i]["TipoTransaccion"].ToString();
                        string Nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string ApellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string ApellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string Rut = dt_Actividades.Rows[i]["Rut"].ToString();
                        string OrdenCompra = dt_Actividades.Rows[i]["OrdenCompra"].ToString();
                        string HoraTransaccion = "";
                        if (FechaTransaccion.Contains(" "))
                        {
                            HoraTransaccion = FechaTransaccion.Split(' ')[1];
                            FechaTransaccion = FechaTransaccion.Split(' ')[0];
                        }

                        object[] fila_aux = new object[11];
                        fila_aux[0] = FechaTransaccion;
                        fila_aux[1] = HoraTransaccion;
                        fila_aux[2] = Monto;
                        fila_aux[3] = DetalleTarjeta;
                        fila_aux[4] = CodigoAutorizacion;
                        fila_aux[5] = TipoTransaccion;
                        fila_aux[6] = Nombre;
                        fila_aux[7] = ApellidoPaterno;
                        fila_aux[8] = ApellidoMaterno;
                        fila_aux[9] = Rut;
                        fila_aux[10] = OrdenCompra;

                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static Boolean EliminarTranferencia(string idTranferencia)
        {
            string update = "UPDATE Transaccion";
            string set = "SET Status = 'ELIMINADO'";
            string where = "WHERE IdTranferencias = " + idTranferencia + " ;";

            int filasAfectadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
            if (filasAfectadas >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        public static Boolean EliminarMensualidad(string idMensualidad)
        {
            try
            {
                string tabla = "DELETE FROM Mensualidad  ";
                string columnas = "  WHERE IdMensualidad = " + idMensualidad + "; ";

                int filasEliminadas = ConsultasExternas.ConsultasSQL.Delete(tabla, columnas);
                if (filasEliminadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }





    }



    public class ConsultasCancelacionClases
    {

        public static ConsultasExternas.RespuestaSQL IngresarCancelacion(int idCalendario, string fecha, string motivo)
        {
            try
            {
                string tabla = " CancelacionClases ";
                string columnas = " FkCalendario, FechaReserva, Motivo";
                string valores = " " + idCalendario + ", '" + fecha + "' , '" + motivo + "'  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la cancelación");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


    
        public static ConsultasExternas.RespuestaSQL estadoClase(string fecha, int id_calendario)
        {
            try
            {
                string select = " SELECT [idCancelacion], [FkCalendario], [FechaReserva], [Motivo] ";
                string from = " FROM   CancelacionClases ";
                string where = "  WHERE FkCalendario = " + id_calendario + "  AND FechaReserva = '" + fecha + "' ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombre = dt_Actividades.Rows[i]["idCancelacion"].ToString();
                        object[] fila_aux = new object[1];
                        fila_aux[0] = nombre;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


    }



    public class ConsultaProfesores
    {

        public static ConsultasExternas.RespuestaSQL ObtenerProfesores()
        {
            try
            {
                string select = " SELECT Nombre, ApellidoPaterno, Usuario.UserName ";
                string from = " FROM  Usuario inner join Perfil on Perfil.UserName = Usuario.UserName ";
                string where = "  WHERE Perfil = 'Profesor' ";
                string order = " ORDER BY Nombre ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string app = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string username = dt_Actividades.Rows[i]["UserName"].ToString();
                        object[] fila_aux = new object[2];
                        fila_aux[0] = nombre + " " + app;
                        fila_aux[1] = username;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerProfesor(int idCalendario)
        {
            try
            {
                string select = " SELECT Nombre, ApellidoPaterno, Usuario.UserName ";
                string from = " FROM [CalendarioProfesor] inner join Usuario ON CalendarioProfesor.UserName = Usuario.UserName ";
                string where = "  WHERE Fk_Calendario = " + idCalendario + " ";
                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string app = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string username = dt_Actividades.Rows[i]["UserName"].ToString();
                        object[] fila_aux = new object[2];
                        fila_aux[0] = nombre + " " + app;
                        fila_aux[1] = username;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

    

        public static ConsultasExternas.RespuestaSQL AsignarProfesor(int idCalendario, string userName)
        {
            try
            {
                //deberia crear si no existe o eactualziar
                string consulta = " IF EXISTS(SELECT 1 FROM CalendarioProfesor WHERE Fk_Calendario = " + idCalendario + ") BEGIN UPDATE CalendarioProfesor ";
                consulta = consulta + " SET UserName = '" + userName + "' WHERE Fk_Calendario = " + idCalendario + " END ELSE BEGIN ";
                consulta = consulta + " INSERT INTO CalendarioProfesor (Fk_Calendario, UserName) VALUES ( " + idCalendario + ", '" + userName + "'); END";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la cancelación");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



    }



    public class ConsultaInstalaciones
    {
        public static ConsultasExternas.RespuestaSQL ExisteInstalacion(string nombreInstalacion)
        {
            try
            {
                string select = " SELECT idInstalacion, NombreInstalacion ";
                string from = " FROM Instalacion ";
                string where = " WHERE NombreInstalacion = '" + nombreInstalacion + "'";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        return new ConsultasExternas.RespuestaSQL(true);
                    }
                    return new ConsultasExternas.RespuestaSQL(false);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL AgregarInstalacion(string nombreInstalacion)
        {
            try
            {
                string tabla = " Instalacion ";
                string columnas = " NombreInstalacion  ";
                string valores = " '" + nombreInstalacion + "'  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No Se Realizó Ingreso de Nueva Instalación");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        public static ConsultasExternas.RespuestaSQL AgregarLogInstalacionRegistro(string username, string fechaRegistro, int idInstalacion, string nombreUsuario)
        {
            try
            {
                string tabla = " LogInstalaciones ";
                string columnas = " UserName, FechaRegistro, FkIdInstalacion, NombreUsuario ";
                string valores = "  '" + username + "', '" + fechaRegistro + "', " + idInstalacion + ", '" + nombreUsuario + "' ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No Se Realizó Ingreso de Nueva Instalación");
            }
            catch (Exception ex)
            {
                return new ConsultasExternas.RespuestaSQL(ex.Message);
            }
        }

        public static ConsultasExternas.RespuestaSQL ObtenerIDinstalacion(string nombreInstalacion)
        {
            try
            {
                string select = " SELECT idInstalacion ";
                string from = " FROM Instalacion ";
                string where = " WHERE NombreInstalacion = '" + nombreInstalacion + "'";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int idClase = -1;
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        idClase = int.Parse(dt_Actividades.Rows[0]["idInstalacion"].ToString());
                    }
                    return new ConsultasExternas.RespuestaSQL(idClase);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        public static ConsultasExternas.RespuestaSQL ObtenerIDReservaInstalacion()
        {
            try
            {
                string select = " SELECT TOP (1) IdReserva  ";
                string from = " FROM ReservasInstalacion ";
                string where = " ";
                string order = "ORDER BY IdReserva DESC";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    int idReserva = -1;
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        idReserva = int.Parse(dt_Actividades.Rows[0]["IdReserva"].ToString());
                    }
                    return new ConsultasExternas.RespuestaSQL(idReserva);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        public static ConsultasExternas.RespuestaSQL ObtenerInstalacion()
        {
            try
            {
                string select = " SELECT idInstalacion, NombreInstalacion ";
                string from = " FROM Instalacion ";
                string where = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idInstalacion = dt_Actividades.Rows[i]["idInstalacion"].ToString();
                        string nombreinstalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        object[] fila_aux = new object[2];
                        fila_aux[0] = idInstalacion;
                        fila_aux[1] = nombreinstalacion;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerTipoInstalacionDisponible(int diaSemana)
        {
            try
            {
                string select = " SELECT distinct idTipoInstalacion, TipoInstalacion.TipoInstalacion ";
                string from = " FROM TipoInstalacion inner join Instalacion on TipoInstalacion.idTipoInstalacion = Instalacion.FkTipoInstalacion inner join[CalendarioInstalacion] on[CalendarioInstalacion].Fk_Instalacion = [Instalacion].idInstalacion ";
                string where = " WHERE Fk_DiasSemana = "+diaSemana+ " AND CalendarioInstalacion.soloAdmin = 'false'";
                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreClase = dt_Actividades.Rows[i]["idTipoInstalacion"].ToString();
                        string Fk_Ubicacion = "Ubicacion";
                        string fk_Clase = dt_Actividades.Rows[i]["TipoInstalacion"].ToString();

                        object[] fila_aux = new object[3];
                        fila_aux[0] = nombreClase;
                        fila_aux[1] = Fk_Ubicacion;
                        fila_aux[2] = fk_Clase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerTipoInstalacionDisponibleAdmin(int diaSemana)
        {
            try
            {
                string select = " SELECT distinct idTipoInstalacion, TipoInstalacion.TipoInstalacion ";
                string from = " FROM TipoInstalacion inner join Instalacion on TipoInstalacion.idTipoInstalacion = Instalacion.FkTipoInstalacion inner join[CalendarioInstalacion] on[CalendarioInstalacion].Fk_Instalacion = [Instalacion].idInstalacion ";
                string where = " WHERE Fk_DiasSemana = " + diaSemana + "  ";
                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreClase = dt_Actividades.Rows[i]["idTipoInstalacion"].ToString();
                        string Fk_Ubicacion = "Ubicacion";
                        string fk_Clase = dt_Actividades.Rows[i]["TipoInstalacion"].ToString();

                        object[] fila_aux = new object[3];
                        fila_aux[0] = nombreClase;
                        fila_aux[1] = Fk_Ubicacion;
                        fila_aux[2] = fk_Clase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerTipoInstalacionDisponibleAdmin()
        {
            try
            {
                string select = " SELECT distinct idTipoInstalacion, TipoInstalacion.TipoInstalacion ";
                string from = " FROM TipoInstalacion inner join Instalacion on TipoInstalacion.idTipoInstalacion = Instalacion.FkTipoInstalacion inner join[CalendarioInstalacion] on[CalendarioInstalacion].Fk_Instalacion = [Instalacion].idInstalacion ";
                string where = " ";
                string order = " order by TipoInstalacion.TipoInstalacion ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreClase = dt_Actividades.Rows[i]["idTipoInstalacion"].ToString();
                        string Fk_Ubicacion = "Ubicacion";
                        string fk_Clase = dt_Actividades.Rows[i]["TipoInstalacion"].ToString();

                        object[] fila_aux = new object[3];
                        fila_aux[0] = nombreClase;
                        fila_aux[1] = Fk_Ubicacion;
                        fila_aux[2] = fk_Clase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerInstalacionDisponible(int diaSemana)
        {
            try
            {
                string select = " SELECT distinct[NombreInstalacion], [Fk_Instalacion] ";
                string from = " FROM [CalendarioInstalacion] inner join [Instalacion] on[CalendarioInstalacion].Fk_Instalacion = [Instalacion].idInstalacion ";
                string where = " WHERE[Fk_DiasSemana] = " + diaSemana + " ";
                string order = " ORDER BY NombreInstalacion ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreClase = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string Fk_Ubicacion = "Ubicacion";
                        string fk_Clase = dt_Actividades.Rows[i]["Fk_Instalacion"].ToString();

                        object[] fila_aux = new object[3];
                        fila_aux[0] = nombreClase;
                        fila_aux[1] = Fk_Ubicacion;
                        fila_aux[2] = fk_Clase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerInstalacionDisponible(int diaSemana, int tipoInstalacion)
        {
            try
            {
                string select = " SELECT distinct[NombreInstalacion], [Fk_Instalacion] ";
                string from = " FROM [CalendarioInstalacion] inner join [Instalacion] on[CalendarioInstalacion].Fk_Instalacion = [Instalacion].idInstalacion ";
                string where = " WHERE [Fk_DiasSemana] = " + diaSemana + " and Instalacion.FkTipoInstalacion = "+ tipoInstalacion + " ";
                string order = " ORDER BY NombreInstalacion ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreClase = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string Fk_Ubicacion = "Ubicacion";
                        string fk_Clase = dt_Actividades.Rows[i]["Fk_Instalacion"].ToString();

                        object[] fila_aux = new object[3];
                        fila_aux[0] = nombreClase;
                        fila_aux[1] = Fk_Ubicacion;
                        fila_aux[2] = fk_Clase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerInstalacionDisponibleSoloClientes(int diaSemana, int tipoInstalacion)
        {
            try
            {
                string select = " SELECT distinct[NombreInstalacion], [Fk_Instalacion] ";
                string from = " FROM [CalendarioInstalacion] inner join [Instalacion] on[CalendarioInstalacion].Fk_Instalacion = [Instalacion].idInstalacion ";
                string where = " WHERE [Fk_DiasSemana] = " + diaSemana + " and Instalacion.FkTipoInstalacion = " + tipoInstalacion + "  and CalendarioInstalacion.soloAdmin = 'false' ";
                string order = " ORDER BY NombreInstalacion ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreClase = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string Fk_Ubicacion = "Ubicacion";
                        string fk_Clase = dt_Actividades.Rows[i]["Fk_Instalacion"].ToString();

                        object[] fila_aux = new object[3];
                        fila_aux[0] = nombreClase;
                        fila_aux[1] = Fk_Ubicacion;
                        fila_aux[2] = fk_Clase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerInstalaciones(int tipoInstalacion)
        {
            try
            {
                 string select = " SELECT distinct [NombreInstalacion], [idInstalacion] ";
                 string from = "  FROM [Instalacion] ";
                 string where = " WHERE  Instalacion.FkTipoInstalacion = " + tipoInstalacion + " ";
                 string order = " ORDER BY NombreInstalacion ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreClase = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string Fk_Ubicacion = "Ubicacion";
                        string fk_Clase = dt_Actividades.Rows[i]["idInstalacion"].ToString();

                        object[] fila_aux = new object[3];
                        fila_aux[0] = nombreClase;
                        fila_aux[1] = Fk_Ubicacion;
                        fila_aux[2] = fk_Clase;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        
        public static ConsultasExternas.RespuestaSQL ObtenerPrecioInstalacion(int idInstalacion)
        {
            try
            {
                
                string select = " SELECT idPrecio, FK_Instalacion, Precio, Fecha, FK_UserName ";
                string from = " FROM PrecioArriendo ";
                string where = " WHERE FK_Instalacion = "+ idInstalacion + "";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idPrecio = dt_Actividades.Rows[i]["idPrecio"].ToString();
                        string FK_Instalacion = dt_Actividades.Rows[i]["FK_Instalacion"].ToString();
                        string Precio = dt_Actividades.Rows[i]["Precio"].ToString();
                        string Fecha = dt_Actividades.Rows[i]["Fecha"].ToString();
                        string FK_UserName = dt_Actividades.Rows[i]["FK_UserName"].ToString();

                        object[] fila_aux = new object[5];
                        fila_aux[0] = idPrecio;
                        fila_aux[1] = FK_Instalacion;
                        fila_aux[2] = Precio;
                        fila_aux[3] = Fecha;
                        fila_aux[4] = FK_UserName;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerExigenciaAsistentes(int idInstalacion)
        {
            try
            {
                string select = " SELECT RegistroParticipantes ";
                string from = " FROM Instalacion ";
                string where = " WHERE idInstalacion = " + idInstalacion + "";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    bool salida = false;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string aux = dt_Actividades.Rows[i]["RegistroParticipantes"].ToString();
                        if(aux.Equals("False") == false)
                        {
                            salida = true;
                        }
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerPrecioInstalacionInt(int idInstalacion)
        {
            try
            {

                string select = " SELECT idPrecio, FK_Instalacion, Precio, Fecha, FK_UserName ";
                string from = " FROM PrecioArriendo ";
                string where = " WHERE FK_Instalacion = " + idInstalacion + "";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int salida = 999999999;

                    if(dt_Actividades.Rows.Count > 0)
                    {
                        salida = int.Parse(dt_Actividades.Rows[0]["Precio"].ToString());
                    }
 ;

                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerInstalacion(int idInstalacion)
        {
            try
            {
                string select = " SELECT  [NombreInstalacion], [TipoInstalacion], [RegistroParticipantes] ";
                string from = " FROM Instalacion ";
                string where = " WHERE idInstalacion = "+idInstalacion;
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombreinstalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string TipoInstalacion = dt_Actividades.Rows[i]["TipoInstalacion"].ToString();
                        string RegistroParticipantes = dt_Actividades.Rows[i]["RegistroParticipantes"].ToString();

                        object[] fila_aux = new object[3];
                        
                        fila_aux[0] = nombreinstalacion;
                        fila_aux[1] = TipoInstalacion;
                        fila_aux[2] = RegistroParticipantes;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL GuardarInstalacion(int idInstalacion, int tipoArriendo, bool registroParticipantes)
        {
            try
            {
                /*
                string consulta = " IF EXISTS(SELECT 1 FROM Socios WHERE FK_UserName = '" + rut + "' ) BEGIN UPDATE Socios  ";
                consulta = consulta + " SET Socio = '" + socio + "' WHERE FK_UserName = '" + rut + "' END ELSE BEGIN  ";
                consulta = consulta + " INSERT INTO Socios([FK_UserName], [Rut], [Socio]) VALUES( '" + rut + "', '" + rut + "', '" + socio + "'); END ";
                */


                string update = "UPDATE Instalacion";
                string set = "SET TipoInstalacion = "+tipoArriendo+", RegistroParticipantes = '"+ registroParticipantes + "'";
                string where = "WHERE idInstalacion = "+ idInstalacion + " ;";

                int filasAfectadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasAfectadas >= 0)
                {
                    return new ConsultasExternas.RespuestaSQL(true);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL("Error en Actualizar Datos");
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL GuardarPrecioArriendo(int idInstalacion, int monto, string userName, string fecha)
        {
            try
            {
                string consulta = " IF EXISTS(SELECT 1 FROM PrecioArriendo WHERE FK_Instalacion = '" + idInstalacion + "' ) BEGIN UPDATE PrecioArriendo  ";
                consulta = consulta + " SET Precio = " + monto + ", Fecha = '" + fecha + "', FK_UserName = '" + userName + "'    WHERE FK_Instalacion = '" + idInstalacion + "' END ELSE BEGIN  ";
                consulta = consulta + " INSERT INTO PrecioArriendo ([FK_Instalacion], [Precio], [Fecha], [FK_UserName]) VALUES( "+idInstalacion +", "+ monto +", '" +fecha+ "', '" +userName+ "'); END ";

                int filasAfectadas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
                if (filasAfectadas >= 0)
                {
                    return new ConsultasExternas.RespuestaSQL(true);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL("Error en Actualizar Datos");
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ActualizarHorario( int idInstalacion, int idHorario, int diaSemana, bool estado)
        {

            try
            {
                string consulta = "IF EXISTS(SELECT 1 FROM CalendarioInstalacion WHERE((FK_Instalacion = "+ idInstalacion + ") AND(Fk_Horario = "+ idHorario + ") AND(Fk_DiasSemana = "+ diaSemana + ")) ) BEGIN UPDATE CalendarioInstalacion ";
                consulta = consulta + "SET Disponible = '"+ estado + "'   WHERE((FK_Instalacion = "+idInstalacion+ ") AND(Fk_Horario = "+ idHorario + ") AND(Fk_DiasSemana = "+ diaSemana + ")) END ELSE BEGIN ";
                consulta = consulta + "INSERT INTO CalendarioInstalacion([Fk_Instalacion], [Fk_Horario], [Fk_DiasSemana], [Disponible]) VALUES("+ idInstalacion + ", "+ idHorario + ", "+ diaSemana + ", '"+ estado + "'); END ";

                int filasAfectadas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
                if (filasAfectadas >= 0)
                {
                    return new ConsultasExternas.RespuestaSQL(true);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL("Error en Actualizar Datos");
                }
            }
            catch(Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
            
        }



        public static ConsultasExternas.RespuestaSQL DeshabilitarHorasInstalacionDia(int idinstalacion, int dia)
        {
            try
            {
                string update = " UPDATE CalendarioInstalacion ";
                string set = " SET Disponible =  'false' ";
                string where = " WHERE Fk_Instalacion = " + idinstalacion + "  AND Fk_DiasSemana = " + dia + " ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasIngresadas >= 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ExisteEnCalendarioInstalacion(int idinstalacion, int dia, int idHorario)
        {
            try
            {
                string select = " SELECT IdCalendario   ";
                string from = " FROM CalendarioInstalacion  inner join Horario on Horario.IdHorario = Calendario.Fk_Horario  ";
                string where = " WHERE (Fk_DiasSemana = " + dia + ") AND ( Fk_Instalacion = " + idinstalacion + ")   and ( Fk_Horario = " + idHorario + ") ";
                string order = " ORDER BY Bloque ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    int salida = -1;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        salida = int.Parse(IdCalendario);
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerHorarios(int diaSemana, int id_instalacion)
        {
            try
            {
                string select = " SELECT IdCalendario, Fk_Instalacion, Horario.IdHorario, Horario.Bloque, Horario.Horario ";
                string from = " FROM [CalendarioInstalacion]  inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario";
                string where = " WHERE(Fk_DiasSemana = " + diaSemana + ") AND (Fk_Instalacion = " + id_instalacion + ") AND (CalendarioInstalacion.Disponible = 'true')  ";
                string order = " ORDER BY Bloque";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        string idUbicacion = dt_Actividades.Rows[i]["Fk_Instalacion"].ToString();
                        string idHorario = dt_Actividades.Rows[i]["IdHorario"].ToString();
                        string Bloque = dt_Actividades.Rows[i]["Bloque"].ToString();
                        string Horario = dt_Actividades.Rows[i]["Horario"].ToString();

                        object[] fila_aux = new object[5];
                        fila_aux[0] = idCalendario;
                        fila_aux[1] = idUbicacion;
                        fila_aux[2] = idHorario;
                        fila_aux[3] = Bloque;
                        fila_aux[4] = Horario;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }







        public static ConsultasExternas.RespuestaSQL ObtenerHorariosUsuarios(int diaSemana, int id_instalacion)
        {
            try
            {
                string select = " SELECT IdCalendario, Fk_Instalacion, Horario.IdHorario, Horario.Bloque, Horario.Horario ";
                string from = " FROM [CalendarioInstalacion]  inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario";
                string where = " WHERE(Fk_DiasSemana = " + diaSemana + ") AND (Fk_Instalacion = " + id_instalacion + ") AND (CalendarioInstalacion.Disponible = 'true') AND (soloAdmin = 'False') ";
                string order = " ORDER BY Bloque";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        string idUbicacion = dt_Actividades.Rows[i]["Fk_Instalacion"].ToString();
                        string idHorario = dt_Actividades.Rows[i]["IdHorario"].ToString();
                        string Bloque = dt_Actividades.Rows[i]["Bloque"].ToString();
                        string Horario = dt_Actividades.Rows[i]["Horario"].ToString();

                        object[] fila_aux = new object[5];
                        fila_aux[0] = idCalendario;
                        fila_aux[1] = idUbicacion;
                        fila_aux[2] = idHorario;
                        fila_aux[3] = Bloque;
                        fila_aux[4] = Horario;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL ObtenerHorarios(int diaSemana, int id_instalacion, int Tipo)
        {
            try
            {
                string select = " SELECT IdCalendario, Fk_Instalacion, Horario.IdHorario, Horario.Bloque, Horario.Horario ";
                string from = " FROM [CalendarioInstalacion]  inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario";
                string where = " WHERE(Fk_DiasSemana = " + diaSemana + ") AND (Fk_Instalacion = " + id_instalacion + ") AND (CalendarioInstalacion.Disponible = 'true')  ";

                string whereFiltro = "AND Horario.Bloque < 100";
                if(Tipo > 1)
                {
                    whereFiltro = "AND Horario.Bloque > 100";
                }
                where = where + whereFiltro;

                string order = " ORDER BY Bloque";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        string idUbicacion = dt_Actividades.Rows[i]["Fk_Instalacion"].ToString();
                        string idHorario = dt_Actividades.Rows[i]["IdHorario"].ToString();
                        string Bloque = dt_Actividades.Rows[i]["Bloque"].ToString();
                        string Horario = dt_Actividades.Rows[i]["Horario"].ToString();

                        object[] fila_aux = new object[5];
                        fila_aux[0] = idCalendario;
                        fila_aux[1] = idUbicacion;
                        fila_aux[2] = idHorario;
                        fila_aux[3] = Bloque;
                        fila_aux[4] = Horario;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL CupoDisponible(int idCalendario, string fecha)
        {
            try
            {

                string select = " Select IdReserva, FkCalendario, UserName, IngresoReserva, FechaReserva ";
                string from = " FROM ReservasInstalacion ";
                string where = " WHERE FechaReserva = '" + fecha + "' AND FkCalendario = " + idCalendario + " ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    if(salida.Length > 0){ //si existe ess ssqeu esta ocupado
                        return new ConsultasExternas.RespuestaSQL(false);  //falso porqeu no hay cupos
                    }
                    return new ConsultasExternas.RespuestaSQL(true);  //existen cupos
                }
                return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL idInstalacionCompartida(int idInstalacion)
        {
            try
            {

                string select = " SELECT Fk_Instalacion_B ";
                string from = " FROM InstalacionCompartida ";
                string where = " WHERE Fk_Instalacion_A = "+ idInstalacion + " ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    int instalacionCompartida = 0;
                    if (dt_Actividades.Rows.Count > 0)
                    {
                        instalacionCompartida = int.Parse(dt_Actividades.Rows[0].ItemArray[0].ToString());
                        return new ConsultasExternas.RespuestaSQL(instalacionCompartida);  //falso porqeu no hay cupos
                    }
                    return new ConsultasExternas.RespuestaSQL(true);  //existen cupos
                }
                return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }





        public static ConsultasExternas.RespuestaSQL CupoDisponible(int idInstalacion, int idDiaSemana, int idCalendario, string fecha)
        {
            try
            {
                string select = " Select IdReserva, FkCalendario, UserName, IngresoReserva, FechaReserva ";
                string from = " FROM ReservasInstalacion ";
                string where = "where FechaReserva = '"+fecha+"' AND FkCalendario = (SELECT IdCalendario FROM CalendarioInstalacion WHERE Fk_Instalacion = "+idInstalacion+" AND  Fk_DiasSemana = "+idDiaSemana+" AND Disponible = 'true' "+
                    " AND Fk_Horario = ( SELECT Fk_Horario FROM CalendarioInstalacion WHERE IdCalendario = "+ idCalendario + ")) ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    if (salida.Length > 0)
                    { //si existe ess ssqeu esta ocupado
                        return new ConsultasExternas.RespuestaSQL(false);  //falso porqeu no hay cupos
                    }
                    return new ConsultasExternas.RespuestaSQL(true);  //existen cupos
                }
                return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL ObtenerReservasInstalacion(string fecha, int id_calendario)
        {
            try
            {
                string select = " SELECT  IdReserva, Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, Rut, Email, NombreInstalacion , FechaReserva, Horario, Requerimiento, HorarioLlegada, NombreArchivo";
                string from = " FROM ReservasInstalacion inner join Usuario on Usuario.UserName = ReservasInstalacion.UserName " +
                              " inner join CalendarioInstalacion on CalendarioInstalacion.IdCalendario = ReservasInstalacion.FkCalendario " +
                              " inner join Instalacion on Instalacion.idInstalacion = CalendarioInstalacion.Fk_Instalacion " +
                              " inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario left join Asistentes on   ReservasInstalacion.IdReserva = Asistentes.idReservaInstalacion";

                string where = "  WHERE FechaReserva = '" + fecha + "' AND  CalendarioInstalacion.IdCalendario = " + id_calendario + " ";

                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {

                        string idReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string rut = dt_Actividades.Rows[i]["Rut"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string correo = dt_Actividades.Rows[i]["Email"].ToString();
                        string ntelefono = dt_Actividades.Rows[i]["NTelefono"].ToString();
                        string instalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string requerimiento = dt_Actividades.Rows[i]["Requerimiento"].ToString();
                        string HorarioLlegada = dt_Actividades.Rows[i]["HorarioLlegada"].ToString();
                        string NombreArchivo = dt_Actividades.Rows[i]["NombreArchivo"].ToString();

                        if (fecha_reserva.Contains(" ")){
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[13];
                        fila_aux[0] = idReserva;
                        fila_aux[1] = rut;
                        fila_aux[2] = nombre;
                        fila_aux[3] = apellidoPaterno;
                        fila_aux[4] = apellidoMaterno;
                        fila_aux[5] = correo;
                        fila_aux[6] = ntelefono;
                        fila_aux[7] = instalacion;
                        fila_aux[8] = fecha_reserva;
                        fila_aux[9] = horario;
                        fila_aux[10] = requerimiento;
                        fila_aux[11] = HorarioLlegada;
                        fila_aux[12] = NombreArchivo;

                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerReservasHorasTipoInstalacion(string fecha, int id_instalacion)
        {
            try
            {
                string select = " SELECT  IdReserva, Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, Rut, Email, NombreInstalacion , FechaReserva, Horario, Requerimiento, HorarioLlegada, NombreArchivo, IdLog, NombreUsuario ";
                string from = " FROM ReservasInstalacion inner join Usuario on Usuario.UserName = ReservasInstalacion.UserName " +
                              " inner join CalendarioInstalacion on CalendarioInstalacion.IdCalendario = ReservasInstalacion.FkCalendario " +
                              " inner join Instalacion on Instalacion.idInstalacion = CalendarioInstalacion.Fk_Instalacion " +
                              " inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario" +
                              " inner join TipoInstalacion on TipoInstalacion.idTipoInstalacion = Instalacion.FkTipoInstalacion  left join Asistentes on   ReservasInstalacion.IdReserva = Asistentes.idReservaInstalacion left join LogInstalaciones on ReservasInstalacion.IdReserva = LogInstalaciones.FkIdReservaInstalacion ";

                string where = "  WHERE FechaReserva = '" + fecha + "' AND  TipoInstalacion.idTipoInstalacion = " + id_instalacion + " ";

                string order = "  ORDER BY Bloque  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {

                        string idReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string rut = dt_Actividades.Rows[i]["Rut"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string correo = dt_Actividades.Rows[i]["Email"].ToString();
                        string ntelefono = dt_Actividades.Rows[i]["NTelefono"].ToString();
                        string instalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string requerimiento = dt_Actividades.Rows[i]["Requerimiento"].ToString();
                        string HorarioLlegada = dt_Actividades.Rows[i]["HorarioLlegada"].ToString();
                        string nombreArchivo = dt_Actividades.Rows[i]["NombreArchivo"].ToString();
                        string nombreAdminReserva = dt_Actividades.Rows[i]["NombreUsuario"].ToString();

                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[14];
                        fila_aux[0] = idReserva;
                        fila_aux[1] = rut;
                        fila_aux[2] = nombre;
                        fila_aux[3] = apellidoPaterno;
                        fila_aux[4] = apellidoMaterno;
                        fila_aux[5] = correo;
                        fila_aux[6] = ntelefono;
                        fila_aux[7] = instalacion;
                        fila_aux[8] = fecha_reserva;
                        fila_aux[9] = horario;
                        fila_aux[10] = requerimiento;
                        fila_aux[11] = HorarioLlegada;
                        fila_aux[12] = nombreArchivo;
                        fila_aux[13] = nombreAdminReserva;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }





        public static ConsultasExternas.RespuestaSQL ObtenerReservasHorasInstalacion(string fecha, int id_instalacion)
        {
            try
            {
                string select = " SELECT  IdReserva, Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, Rut, Email, NombreInstalacion , FechaReserva, Horario, Requerimiento, HorarioLlegada, NombreArchivo, IdLog, NombreUsuario ";
                string from = " FROM ReservasInstalacion inner join Usuario on Usuario.UserName = ReservasInstalacion.UserName " +
                              " inner join CalendarioInstalacion on CalendarioInstalacion.IdCalendario = ReservasInstalacion.FkCalendario " +
                              " inner join Instalacion on Instalacion.idInstalacion = CalendarioInstalacion.Fk_Instalacion " +
                              " inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario  left join Asistentes on   ReservasInstalacion.IdReserva = Asistentes.idReservaInstalacion left join LogInstalaciones on ReservasInstalacion.IdReserva = LogInstalaciones.FkIdReservaInstalacion";

                string where = "  WHERE FechaReserva = '" + fecha + "' AND  CalendarioInstalacion.Fk_Instalacion = " + id_instalacion + " ";

                string order = "  ORDER BY Bloque  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {

                        string idReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string rut = dt_Actividades.Rows[i]["Rut"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string correo = dt_Actividades.Rows[i]["Email"].ToString();
                        string ntelefono = dt_Actividades.Rows[i]["NTelefono"].ToString();
                        string instalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string requerimiento = dt_Actividades.Rows[i]["Requerimiento"].ToString();
                        string horarioLlegada = dt_Actividades.Rows[i]["HorarioLlegada"].ToString();
                        string nombreArchivo = dt_Actividades.Rows[i]["NombreArchivo"].ToString();
                        string nombreAdminReserva = dt_Actividades.Rows[i]["NombreUsuario"].ToString();

                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[14];
                        fila_aux[0] = idReserva;
                        fila_aux[1] = rut;
                        fila_aux[2] = nombre;
                        fila_aux[3] = apellidoPaterno;
                        fila_aux[4] = apellidoMaterno;
                        fila_aux[5] = correo;
                        fila_aux[6] = ntelefono;
                        fila_aux[7] = instalacion;
                        fila_aux[8] = fecha_reserva;
                        fila_aux[9] = horario;
                        fila_aux[10] = requerimiento;
                        fila_aux[11] = horarioLlegada;
                        fila_aux[12] = nombreArchivo;
                        fila_aux[13] = nombreAdminReserva;

                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL ObtenerReservaInstalacion(int idReserva)
        {
            try
            {
                string select = " SELECT  IdReserva, Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, Rut, Email, NombreInstalacion , FechaReserva, Horario, Requerimiento ";
                string from = " FROM ReservasInstalacion inner join Usuario on Usuario.UserName = ReservasInstalacion.UserName " +
                              " inner join CalendarioInstalacion on CalendarioInstalacion.IdCalendario = ReservasInstalacion.FkCalendario " +
                              " inner join Instalacion on Instalacion.idInstalacion = CalendarioInstalacion.Fk_Instalacion " +
                              " inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario";

                string where = "  WHERE IdReserva = '" + idReserva + "'  ";

                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string rut = dt_Actividades.Rows[i]["Rut"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string correo = dt_Actividades.Rows[i]["Email"].ToString();
                        string ntelefono = dt_Actividades.Rows[i]["NTelefono"].ToString();
                        string instalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string requerimiento = dt_Actividades.Rows[i]["Requerimiento"].ToString();
                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[10];
                        fila_aux[0] = rut;
                        fila_aux[1] = nombre;
                        fila_aux[2] = apellidoPaterno;
                        fila_aux[3] = apellidoMaterno;
                        fila_aux[4] = correo;
                        fila_aux[5] = ntelefono;
                        fila_aux[6] = instalacion;
                        fila_aux[7] = fecha_reserva;
                        fila_aux[8] = horario;
                        fila_aux[9] = requerimiento;

                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL ObtenerReservasDia(string fecha)
        {
            try
            {
                string select = " SELECT  IdReserva, Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, Rut, Email, NombreInstalacion , FechaReserva, Horario, Requerimiento, HorarioLlegada, NombreArchivo, IdLog, NombreUsuario ";
                string from = " FROM ReservasInstalacion inner join Usuario on Usuario.UserName = ReservasInstalacion.UserName " +
                              " inner join CalendarioInstalacion on CalendarioInstalacion.IdCalendario = ReservasInstalacion.FkCalendario " +
                              " inner join Instalacion on Instalacion.idInstalacion = CalendarioInstalacion.Fk_Instalacion " +
                              " inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario left join Asistentes on   ReservasInstalacion.IdReserva = Asistentes.idReservaInstalacion left join LogInstalaciones on ReservasInstalacion.IdReserva = LogInstalaciones.FkIdReservaInstalacion ";

                string where = "  WHERE FechaReserva = '" + fecha + "'  ";

                string order = "  ORDER BY Bloque  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {

                        string idReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string rut = dt_Actividades.Rows[i]["Rut"].ToString();
                        string nombre = dt_Actividades.Rows[i]["Nombre"].ToString();
                        string apellidoPaterno = dt_Actividades.Rows[i]["ApellidoPaterno"].ToString();
                        string apellidoMaterno = dt_Actividades.Rows[i]["ApellidoMaterno"].ToString();
                        string correo = dt_Actividades.Rows[i]["Email"].ToString();
                        string ntelefono = dt_Actividades.Rows[i]["NTelefono"].ToString();
                        string instalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        string requerimiento = dt_Actividades.Rows[i]["Requerimiento"].ToString();
                        string horarioLlegada = dt_Actividades.Rows[i]["HorarioLlegada"].ToString();
                        string NombreArchivo = dt_Actividades.Rows[i]["NombreArchivo"].ToString();
                        string NombreAdminReserva= dt_Actividades.Rows[i]["NombreUsuario"].ToString();

                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[14];
                        fila_aux[0] = idReserva;
                        fila_aux[1] = rut;
                        fila_aux[2] = nombre;
                        fila_aux[3] = apellidoPaterno;
                        fila_aux[4] = apellidoMaterno;
                        fila_aux[5] = correo;
                        fila_aux[6] = ntelefono;
                        fila_aux[7] = instalacion;
                        fila_aux[8] = fecha_reserva;
                        fila_aux[9] = horario;
                        fila_aux[10] = requerimiento;
                        fila_aux[11] = horarioLlegada;
                        fila_aux[12] = NombreArchivo;
                        fila_aux[13] = NombreAdminReserva;

                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerReservasMes(string fecha_inicio, string FechaFin)
        {
            try
            {
                string select = " SELECT  IdReserva, Nombre, ApellidoPaterno, ApellidoMaterno, NTelefono, Rut, Email, NombreInstalacion , FechaReserva, Horario, Requerimiento ";
                string from = " FROM ReservasInstalacion inner join Usuario on Usuario.UserName = ReservasInstalacion.UserName " +
                              " inner join CalendarioInstalacion on CalendarioInstalacion.IdCalendario = ReservasInstalacion.FkCalendario " +
                              " inner join Instalacion on Instalacion.idInstalacion = CalendarioInstalacion.Fk_Instalacion " +
                              " inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario";

                string where = "  WHERE (FechaReserva >= '" + fecha_inicio + "') AND ( FechaReserva < '"+ FechaFin + "')";

                string order = "  ORDER BY Bloque  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {

                        string idReserva = dt_Actividades.Rows[i]["IdReserva"].ToString();
                        string instalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string fecha_reserva = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                        string horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        if (fecha_reserva.Contains(" "))
                        {
                            fecha_reserva = fecha_reserva.Split(' ')[0];
                        }

                        object[] fila_aux = new object[4];
                        fila_aux[0] = idReserva;
                        fila_aux[1] = instalacion;
                        fila_aux[2] = fecha_reserva;
                        fila_aux[3] = horario;

                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }





        public static ConsultasExternas.RespuestaSQL ObtenerIDReservasInstalacion(string fecha, int id_calendario)
        {
            try
            {
                string select = " SELECT IdReserva ";
                string from = " FROM ReservasInstalacion inner join Usuario on Usuario.UserName = ReservasInstalacion.UserName " +
                              " inner join CalendarioInstalacion on CalendarioInstalacion.IdCalendario = ReservasInstalacion.FkCalendario " +
                              " inner join Instalacion on Instalacion.idInstalacion = CalendarioInstalacion.Fk_Instalacion " +
                              " inner join Horario on Horario.IdHorario = CalendarioInstalacion.Fk_Horario";

                string where = "  WHERE FechaReserva = '" + fecha + "' AND  CalendarioInstalacion.IdCalendario = " + id_calendario + " ";

                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    int idReserva = -1;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        idReserva = int.Parse(dt_Actividades.Rows[i]["IdReserva"].ToString());

                    }
                    return new ConsultasExternas.RespuestaSQL(idReserva);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL CrearReserva(string usuario, string fecha, string fecha_creacion, int calendario, string requerimientos, string llegada)
        {
            try
            {
                string tabla = " ReservasInstalacion  ";
                string columnas = " FkCalendario, UserName, IngresoReserva, FechaReserva, Requerimiento, HorarioLlegada ";
                string valores = " " + calendario + ", '" + usuario + "' , '" + fecha_creacion + "' , '" + fecha + "','"+requerimientos+"','"+llegada+"'";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva :" + columnas + ":" + valores);
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        public static ConsultasExternas.RespuestaSQL RegistrarLogReservaInstalacion(string rut, string fecha_creacion, int fkIdReservaInstalacion, string username)
        {
            try
            {
                string tabla = " LogInstalaciones  ";
                string columnas = " UserName, FechaRegistro, FkIdReservaInstalacion, NombreUsuario ";
                string valores = " '" + rut + "', '" + fecha_creacion + "' , " + fkIdReservaInstalacion + ", '" + username + "' ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la reserva :" + columnas + ":" + valores);
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL IngresarTransaccion(string usuario, string OrdenCompra, string DetalleTarjeta, int Monto, int CantidadCuotas, decimal MontoCuotas, string CodigoAutorizacion, string FechaAutorizacion, string FechaTransaccion, string TipoTransaccion, string VCI, string Status, string Token)
        {
            try
            {
                string update = " UPDATE TransaccionInstalacion ";
                update = update + "SET UserName = '"+usuario+"', DetalleTarjeta = '"+DetalleTarjeta+"', Monto = "+Monto+", CantidadCuotas = "+CantidadCuotas+", MontoCuotas = "+MontoCuotas+", CodigoAutorizacion = '"+CodigoAutorizacion+"', FechaAutorizacion = '"+FechaAutorizacion+"', FechaTransaccion = '"+FechaTransaccion+"', TipoTransaccion = '"+TipoTransaccion+"', VCI = '"+VCI+"' ";
                update = update + " WHERE (Token = '"+Token+"' and  OrdenCompra = '"+OrdenCompra+"' and Status != 'AUTHORIZED') ";

                string insert = " INSERT INTO TransaccionInstalacion(UserName, OrdenCompra, DetalleTarjeta, Monto, CantidadCuotas, MontoCuotas, CodigoAutorizacion, FechaAutorizacion, FechaTransaccion, TipoTransaccion, VCI, Status, Token) ";
                insert = insert + " VALUES( '" + usuario + "','" + OrdenCompra + "','" + DetalleTarjeta + "','" + Monto + "','" + CantidadCuotas + "','" + MontoCuotas + "','" + CodigoAutorizacion + "','" + FechaAutorizacion + "','" + FechaTransaccion + "','" + TipoTransaccion + "','" + VCI + "','" + Status + "','" + Token + "');";

                string consulta = "  IF EXISTS(SELECT 1 FROM TransaccionInstalacion WHERE (Token = '"+Token+"' AND OrdenCompra  = '"+OrdenCompra+"' AND Status = 'AUTHORIZED')) BEGIN " + update+
                            "  END ELSE BEGIN " +
                            insert+ "  END";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
                if (filasIngresadas >= 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se pudo almacenar la transacción");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerIDTransaccion(string usuario, string token, string ordenCompra)
        {
            try
            {
                string select = " SELECT IdTranferencias ";
                string from = " FROM TransaccionInstalacion ";
                string where = " WHERE UserName = '" + usuario + "' and Token = '" + token + "' and OrdenCompra = '" + ordenCompra + "' ";
                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string idTranferencia = dt_Actividades.Rows[i]["IdTranferencias"].ToString();
                        salida[i] = idTranferencia;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerDatosCalendario(int idCalendario)
        {
            try
            {
                string select = " SELECT IdCalendario, NombreInstalacion, Horario ";
                string from = " FROM CalendarioInstalacion inner join Instalacion on CalendarioInstalacion.Fk_Instalacion = Instalacion.idInstalacion inner join Horario on CalendarioInstalacion.Fk_Horario = Horario.IdHorario ";
                string where = " WHERE IdCalendario =  " + idCalendario + " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdCalendario = dt_Actividades.Rows[i]["IdCalendario"].ToString();
                        string NombreInstalacion = dt_Actividades.Rows[i]["NombreInstalacion"].ToString();
                        string Horario = dt_Actividades.Rows[i]["Horario"].ToString();
                        object[] fila_aux = new object[3];
                        fila_aux[0] = IdCalendario;
                        fila_aux[1] = NombreInstalacion;
                        fila_aux[2] = Horario;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL EliminarReserva(int idReserva, string userName)
        {
            try
            {
                string tabla = "DELETE FROM ReservasInstalacion  ";
                string columnas = "  WHERE IdReserva = " + idReserva + " and UserName = '" + userName + "'; ";

                int filasEliminadas = ConsultasExternas.ConsultasSQL.Delete(tabla, columnas);
                if (filasEliminadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la cancelación :");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL IngresarCancelacion(int idCalendario, string fecha, string motivo)
        {
            try
            {
                string tabla = " CancelacionInstalacion ";
                string columnas = " FkCalendario, FechaReserva, Motivo ";
                string valores = " " + idCalendario + ", '" + fecha + "' , '" + motivo + "'  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la cancelación");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL IngresarCancelacionDia(string fecha, string motivo)
        {
            try
            {
                string tabla = " CancelacionInstalacionDia ";
                string columnas = " FechaReserva, Motivo ";
                string valores = "'" + fecha + "' , '" + motivo + "'  ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la cancelación");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL estadoInstalacionCancelacion(string fecha, int id_calendario)
        {
            try
            {
                string select = " SELECT [idCancelacion], [FkCalendario], [FechaReserva], [Motivo] ";
                string from = " FROM   [CancelacionInstalacion] ";
                string where = "  WHERE FkCalendario = " + id_calendario + "  AND FechaReserva = '" + fecha + "' ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombre = dt_Actividades.Rows[i]["idCancelacion"].ToString();
                        object[] fila_aux = new object[1];
                        fila_aux[0] = nombre;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL estadoInstalacionCancelacionDia(string fecha)
        {
            try
            {
                string select = " SELECT [idCancelacionDia],  [FechaReserva], [Motivo] ";
                string from = " FROM   [CancelacionInstalacionDia] ";
                string where = "  WHERE  FechaReserva = '" + fecha + "' ";
                string order = "  ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string nombre = dt_Actividades.Rows[i]["idCancelacionDia"].ToString();
                        object[] fila_aux = new object[1];
                        fila_aux[0] = nombre;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static ConsultasExternas.RespuestaSQL ObtenerIDCalendario(int diaSemana, int id_instalacion, int idHorario)
        {
            try
            {
                string select = " SELECT [IdCalendario] ";
                string from = " FROM [CalendarioInstalacion] ";
                string where = " WHERE Fk_Instalacion = "+ id_instalacion + " AND Fk_Horario = "+idHorario+" and Fk_DiasSemana = "+diaSemana+" AND Disponible = 1";
                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    int salida = -1;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        salida = int.Parse(dt_Actividades.Rows[i]["IdCalendario"].ToString());
                        
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }




        public static bool ActualizarAsistentes(string nombreArchivo, string idReserva)
        {
            string consulta = "  IF EXISTS(SELECT 1 FROM Asistentes WHERE (idReservaInstalacion = " + idReserva + ")) BEGIN UPDATE Asistentes " +
                              " SET idReservaInstalacion = " + idReserva + ", NombreArchivo = '" + nombreArchivo + "'  WHERE idReservaInstalacion = " + idReserva + " END ELSE BEGIN " +
                              " INSERT INTO Asistentes( idReservaInstalacion, NombreArchivo) VALUES( " + idReserva + ", '" + nombreArchivo + "'); END";

            int filasIngresadas = ConsultasExternas.ConsultasSQL.Consulta(consulta);
            if (filasIngresadas > 0)
            {
                object[] salida = new object[0];
                return true;
            }
            return false;
        }



        public static ConsultasExternas.RespuestaSQL ObtenerAsistentes(string idReserva)
        {
            try
            {
                string select = " SELECT [NombreArchivo] ";
                string from = " FROM Asistentes ";
                string where = " WHERE  idReservaInstalacion = "+idReserva+" ";
                string order = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        salida[i] = dt_Actividades.Rows[i]["NombreArchivo"].ToString();

                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }

            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL EliminarAsistentes(string idReserva)
        {
            try
            {
                string tabla = "DELETE FROM Asistentes  ";
                string columnas = "  WHERE idReservaInstalacion = " + idReserva + " ";

                int filasEliminadas = ConsultasExternas.ConsultasSQL.Delete(tabla, columnas);
                if (filasEliminadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se realizó la cancelación :");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


    }



    public class ConsultasEstadisticas
    {
        public static ConsultasExternas.RespuestaSQL ObtenerUsuariosRegistrados()
        {
            try
            {
                string select = " SELECT COUNT(*) as Registrados ";
                string from = "FROM [Usuario] left join Socios on Usuario.Rut = Socios.FK_UserName ";
                string where = " ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int cantidadUsuarios = 0;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        cantidadUsuarios = int.Parse(dt_Actividades.Rows[i]["Registrados"].ToString());
                    }
                    return new ConsultasExternas.RespuestaSQL(cantidadUsuarios);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerUsuariosSociosRegistrados()
        {
            try
            {

                string select = " SELECT COUNT(*) as Socios ";
                string from = " FROM [Usuario] left join Socios on Usuario.Rut = Socios.FK_UserName ";
                string where = "  WHERE Socio is not null ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int cantidadUsuarios = 0;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        cantidadUsuarios = int.Parse(dt_Actividades.Rows[i]["Socios"].ToString());
                    }
                    return new ConsultasExternas.RespuestaSQL(cantidadUsuarios);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        public static ConsultasExternas.RespuestaSQL ObtenerUsuariosNoSociosRegistrados()
        {
            try
            {

                string select = " SELECT COUNT(*) as [No Socios] ";
                string from = " FROM [Usuario] left join Socios on Usuario.Rut = Socios.FK_UserName ";
                string where = "   WHERE Socio is null ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int cantidadUsuarios = 0;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        cantidadUsuarios = int.Parse(dt_Actividades.Rows[i]["No Socios"].ToString());
                    }
                    return new ConsultasExternas.RespuestaSQL(cantidadUsuarios);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerMensualidadesPagadas(int mes, int year)
        {
            try
            {
                string select = " Select COUNT(*) as TotalMensualidadPagadas ";
                string from = "  FROM "+
                    " (SELECT Mensualidad.UserName, OrdenCompra, Token"+
                    " FROM Mensualidad inner join Transaccion on Mensualidad.FkTransaccion = Transaccion.IdTranferencias inner join Usuario on Mensualidad.UserName = Usuario.UserName"+
                    " WHERE(Year ="+year+") and(Mes = "+mes+") Group by Mensualidad.UserName, OrdenCompra, Token) as tabla; ";
                string where = "";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int cantidadUsuarios = 0;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        cantidadUsuarios = int.Parse(dt_Actividades.Rows[i]["TotalMensualidadPagadas"].ToString());
                    }
                    return new ConsultasExternas.RespuestaSQL(cantidadUsuarios);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }


        public static ConsultasExternas.RespuestaSQL ObtenerTotalMensualidadesPagadas(int mes, int year)
        {
            try
            {
                string select = "  Select SUM (Monto) as TotalMensualidadPagadas";
                string from = "  FROM("+
                    " Select Monto, OrdenCompra, Token"+
                    " FROM Mensualidad inner join Transaccion on Transaccion.IdTranferencias = Mensualidad.FkTransaccion "+
                    " WHERE Mes = "+mes+" and Year = "+year+" GROUP BY Monto, OrdenCompra, Token) as tabla";
                string where=  "";

                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int cantidadUsuarios = 0;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        if (dt_Actividades.Rows[i]["TotalMensualidadPagadas"].ToString().Equals("") == false)
                        {
                            cantidadUsuarios = int.Parse(dt_Actividades.Rows[i]["TotalMensualidadPagadas"].ToString());
                        }
                            
                    }
                    return new ConsultasExternas.RespuestaSQL(cantidadUsuarios);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerClasesDirigidasPagadas(int mes, int year)
        {
            try
            {

                string select = " SELECT COUNT(*) as cantidad ";
                string from = "  FROM Transaccion ";
                string where = " WHERE OrdenCompra like '%_"+mes+"_"+year+"%' ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int cantidadUsuarios = 0;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        cantidadUsuarios = int.Parse(dt_Actividades.Rows[i]["cantidad"].ToString());
                    }
                    return new ConsultasExternas.RespuestaSQL(cantidadUsuarios);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerTotalClasesDirigidasPagadas(int mes, int year)
        {
            try
            {
                string select = " SELECT  SUM(Monto) as total ";
                string from = "  FROM Transaccion ";
                string where = " WHERE OrdenCompra like '%_" + mes + "_" + year + "%' ";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, "");
                if (dt_Actividades.TableName != "ERROR")
                {
                    int cantidadUsuarios = 0;
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        if(dt_Actividades.Rows[i]["total"].ToString().Equals("") == false)
                        {
                            cantidadUsuarios = int.Parse(dt_Actividades.Rows[i]["total"].ToString());
                        }
                    }
                    return new ConsultasExternas.RespuestaSQL(cantidadUsuarios);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL ObtenerReservasDia(string fecha_inicio, string fecha_termino, string clase)
        {
            string select = " SELECT [FechaReserva], COUNT([FechaReserva]) as cantidad ";
            string from = " FROM[Algarrobo].[dbo].[Reserva] inner join Calendario on Reserva.FkCalendario = Calendario.IdCalendario ";
            string where = " WHERE(FechaReserva >= '"+fecha_inicio+"' and FechaReserva <= '"+fecha_termino+"' ) ";
            string group = " GROUP BY FechaReserva ";

            if(clase.Equals("Todos") == false){
                where = " WHERE(FechaReserva >= '" + fecha_inicio + "' and FechaReserva <= '" + fecha_termino + "')  and Fk_Clase = " + clase;
            }

            DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, group);
            if (dt_Actividades.TableName != "ERROR")
            {
                object[] salida = new object[dt_Actividades.Rows.Count];
                for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                {
                    string fecha = dt_Actividades.Rows[i]["FechaReserva"].ToString();
                    string cantidad = dt_Actividades.Rows[i]["cantidad"].ToString();
                    fecha = fecha.Split(' ')[0];

                    object[] fila_aux = new object[2];
                    fila_aux[0] = fecha;
                    fila_aux[1] = cantidad;
                    salida[i] = fila_aux;


                }
                return new ConsultasExternas.RespuestaSQL(salida);
            }
            else
            {
                return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
            }

        }



        public static ConsultasExternas.RespuestaSQL ObtenerReservasDia(string fecha_inicio, string clase)
        {
            string select = " SELECT Horario, COUNT(Horario) as cantidad ";
            string from = " FROM Reserva inner join Calendario on Reserva.FkCalendario = Calendario.IdCalendario inner join Horario on  Calendario.Fk_Horario = Horario.IdHorario ";
            string where = " WHERE(FechaReserva = '" + fecha_inicio + "'  ) ";
            string group = " GROUP BY Horario ";

            if (clase.Equals("Todos") == false)
            {
                where = " WHERE(FechaReserva = '" + fecha_inicio + "' )  and Fk_Clase = " + clase;
            }

            DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, group);
            if (dt_Actividades.TableName != "ERROR")
            {
                object[] salida = new object[dt_Actividades.Rows.Count];
                for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                {
                    string fecha = dt_Actividades.Rows[i]["Horario"].ToString();
                    string cantidad = dt_Actividades.Rows[i]["cantidad"].ToString();
   

                    object[] fila_aux = new object[2];
                    fila_aux[0] = fecha;
                    fila_aux[1] = cantidad;
                    salida[i] = fila_aux;


                }
                return new ConsultasExternas.RespuestaSQL(salida);
            }
            else
            {
                return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
            }

        }




    }


    public class ConsultasEmpresas
    {
        public static ConsultasExternas.RespuestaSQL ObtenerEmpresas()
        {
            try
            {
                string select = " SELECT Empresa, IdEmpresa ";
                string from = " FROM Empresas ";
                string where = " WHERE Deshabilitado = 0 ";
                string order = " ORDER BY Empresa";
                DataTable dt_Actividades = ConsultasExternas.ConsultasSQL.Select(select, from, where, order);
                if (dt_Actividades.TableName != "ERROR")
                {
                    object[] salida = new object[dt_Actividades.Rows.Count];
                    for (int i = 0; i < dt_Actividades.Rows.Count; i++)
                    {
                        string IdEmpresa= dt_Actividades.Rows[i]["IdEmpresa"].ToString();
                        string NombreEmpresa = dt_Actividades.Rows[i]["Empresa"].ToString();

                        object[] fila_aux = new object[2];
                        fila_aux[0] = NombreEmpresa;
                        fila_aux[1] = IdEmpresa;
                        salida[i] = fila_aux;
                    }
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                else
                {
                    return new ConsultasExternas.RespuestaSQL(dt_Actividades.ExtendedProperties["Mensaje"].ToString());
                }
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }



        public static ConsultasExternas.RespuestaSQL AgregarNuevaEmpresa(string nombreEmpresa)
        {
            try
            {
                string tabla = " Empresas ";
                string columnas = " Empresa ";
                string valores = "'" + nombreEmpresa + "' ";

                int filasIngresadas = ConsultasExternas.ConsultasSQL.Insert(tabla, columnas, valores);
                if (filasIngresadas > 0)
                {
                    object[] salida = new object[0];
                    return new ConsultasExternas.RespuestaSQL(salida);
                }
                return new ConsultasExternas.RespuestaSQL("Error Interno, No se ingresó la Empresa");
            }
            catch (Exception exp)
            {
                return new ConsultasExternas.RespuestaSQL(exp.Message);
            }
        }

        public static Boolean ModificarEmpresa(string nombreEmpresa, int idEmpresa)
        {
            try
            {
                string update = " UPDATE Empresas ";
                string set = " SET Empresa = '" + nombreEmpresa + "' ";
                string where = " WHERE IdEmpresa = " + idEmpresa + " ";

                int filasModificadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasModificadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Boolean EliminarEmpresa(int idEmpresa)
        {
            try
            {
                string update = " UPDATE Empresas ";
                string set = " SET Deshabilitado = 1 ";
                string where = " WHERE IdEmpresa = " + idEmpresa + " ";

                int filasModificadas = ConsultasExternas.ConsultasSQL.Update(update, set, where);
                if (filasModificadas > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }



    }

}