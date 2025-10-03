using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace WebApplicationTemplateBase.ConsultasExternas
{



    public static class ConsultasSQL
    {
        //string de conexion                                                                                                                                                                                                                                                                         
        //static string stringConexion = @"Data Source = 201.148.104.16; Initial Catalog = Algarrobo; User Id =#consultarcredenciales# ; Password=#consultarcredenciales#;";
        //static string stringConexion = @"Data Source = localhost; Initial Catalog = Algarrobo; User Id =user_algarrobo ; Password=*1Canal2022;";
        static string stringConexion = @"Data Source = LP-SPINONES\SQLEXPRESS; Initial Catalog = Algarrobo; User Id =acceso; Password=Huasco.2025$;";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <returns>Datatable vacia = no enconto resultados </returns>
        public static DataTable Select(string select, string from, string where, string orderBy)
        {
            DataTable salida = new DataTable();
            string consulta = select + ' ' + from + ' ' + where + ' ' + orderBy;

            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                SqlDataReader resultados = resultado_consulta.ExecuteReader();
                if (resultados.HasRows)
                {
                    salida = newTabla(resultados.FieldCount);
                    while (resultados.Read())
                    {
                        Object[] fila = new Object[resultados.FieldCount];
                        for (int i = 0; i < resultados.FieldCount; i++)
                        {
                            /*definir nombres de columnas*/
                            if (salida.Columns[i].ColumnName.Equals("Column" + (i + 1)))
                            {
                                salida.Columns[i].ColumnName = resultados.GetName(i);
                            }
                            String columna_aux = resultados.GetValue(i).ToString().Trim();
                            fila[i] = columna_aux;
                        }
                        DataRow nuevaFila = salida.NewRow();
                        nuevaFila.ItemArray = fila;
                        salida.Rows.Add(nuevaFila);
                    }
                }
                else
                {
                    salida = new DataTable();
                }
                resultados.Close();
                conexion.Close();
                return salida;
            }
            catch (Exception exp)
            {
                salida = new DataTable();
                salida.TableName = "ERROR";
                salida.ExtendedProperties.Add("Mensaje",exp.Message);
                salida.ExtendedProperties.Add("StackTrace", exp.StackTrace);
                salida.ExtendedProperties.Add("Source", exp.Source);
                return salida;
            }

        }




        public static async Task<DataTable> SelectASYNC(string select, string from, string where, string orderBy)
        {
            DataTable salida = new DataTable();
            string consulta = select + ' ' + from + ' ' + where + ' ' + orderBy;

            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                await conexion.OpenAsync();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                SqlDataReader resultados = await resultado_consulta.ExecuteReaderAsync();
                if (resultados.HasRows)
                {
                    salida = newTabla(resultados.FieldCount);
                    while (resultados.Read())
                    {
                        Object[] fila = new Object[resultados.FieldCount];
                        for (int i = 0; i < resultados.FieldCount; i++)
                        {
                            /*definir nombres de columnas*/
                            if (salida.Columns[i].ColumnName.Equals("Column" + (i + 1)))
                            {
                                salida.Columns[i].ColumnName = resultados.GetName(i);
                            }
                            String columna_aux = resultados.GetValue(i).ToString().Trim();
                            fila[i] = columna_aux;
                        }
                        DataRow nuevaFila = salida.NewRow();
                        nuevaFila.ItemArray = fila;
                        salida.Rows.Add(nuevaFila);
                    }
                }
                else
                {
                    salida = new DataTable();
                }
                resultados.Close();
                conexion.Close();
                return salida;
            }
            catch (Exception exp)
            {
                salida = new DataTable();
                salida.TableName = "ERROR";
                salida.ExtendedProperties.Add("Mensaje", exp.Message);
                salida.ExtendedProperties.Add("StackTrace", exp.StackTrace);
                salida.ExtendedProperties.Add("Source", exp.Source);
                return salida;
            }

        }


        public static int Consulta(string consulta)
        {
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                int filas_afectadas = resultado_consulta.ExecuteNonQuery();
                conexion.Close();
                return filas_afectadas;
            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return -1;
            }

        }




        public static int Insert(string tabla, string columnas, string valores)
        {
            string consulta = " INSERT INTO  "+tabla+" ( "+columnas+" ) VALUES ("+valores+")";
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                int filas_afectadas = resultado_consulta.ExecuteNonQuery();
                conexion.Close();
                return filas_afectadas;
            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return -1;
            }

        }



        public static int InsertGetID(string tabla, string columnas, string valores)
        {
            string consulta = " INSERT INTO  " + tabla + " ( " + columnas + " ) VALUES (" + valores + ")";
            consulta = consulta + " SELECT SCOPE_IDENTITY();";
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                object filas_afectadas = resultado_consulta.ExecuteScalar();
                int id = int.Parse(filas_afectadas.ToString()); 
                conexion.Close();
                return id;
            }
            catch (Exception)
            {
                return -1;
            }

        }




        public static int Update(string update, string set, string where)
        {
            string consulta = update + " " + set + " " + where;
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                int filas_afectadas = resultado_consulta.ExecuteNonQuery();
                conexion.Close();
                return filas_afectadas;
            }
            catch (Exception)
            {
                return -1;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabla">debe comenzar con DELETE FROM "Nombre de tabla"</param>
        /// <param name="where">debe comenzar con WHERE "condicion"</param>
        /// <returns></returns>
        public static int Delete(string tabla, string where)
        {
            string consulta = tabla + " " + where;
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                int filas_afectadas = resultado_consulta.ExecuteNonQuery();
                conexion.Close();
                return filas_afectadas;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static DataTable newTabla(int numero_columnas)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < numero_columnas; i++)
            {
                dt.Columns.Add();
            }
            return dt;
        }





    }



    public static class ConsultasSQLRespaldo
    {
        static string stringConexion = @"Data Source = 190.96.85.142; Initial Catalog = Algarrobo; User Id =usuario_algarrobo ; Password=*1Canal2022; Connection Timeout=30;ConnectRetryCount=3;ConnectRetryInterval=5";




        /// <summary>
        /// 
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <returns>Datatable vacia = no enconto resultados </returns>
        public static DataTable Select(string select, string from, string where, string orderBy)
        {
            DataTable salida = new DataTable();
            string consulta = select + ' ' + from + ' ' + where + ' ' + orderBy;

            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                SqlDataReader resultados = resultado_consulta.ExecuteReader();
                if (resultados.HasRows)
                {
                    salida = newTabla(resultados.FieldCount);
                    while (resultados.Read())
                    {
                        Object[] fila = new Object[resultados.FieldCount];
                        for (int i = 0; i < resultados.FieldCount; i++)
                        {
                            /*definir nombres de columnas*/
                            if (salida.Columns[i].ColumnName.Equals("Column" + (i + 1)))
                            {
                                salida.Columns[i].ColumnName = resultados.GetName(i);
                            }
                            String columna_aux = resultados.GetValue(i).ToString().Trim();
                            fila[i] = columna_aux;
                        }
                        DataRow nuevaFila = salida.NewRow();
                        nuevaFila.ItemArray = fila;
                        salida.Rows.Add(nuevaFila);
                    }
                }
                else
                {
                    salida = new DataTable();
                }
                resultados.Close();
                conexion.Close();
                return salida;
            }
            catch (Exception exp)
            {
                salida = new DataTable();
                salida.TableName = "ERROR";
                salida.ExtendedProperties.Add("Mensaje", exp.Message);
                salida.ExtendedProperties.Add("StackTrace", exp.StackTrace);
                salida.ExtendedProperties.Add("Source", exp.Source);
                return salida;
            }

        }



        public static int Consulta(string consulta)
        {
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                int filas_afectadas = resultado_consulta.ExecuteNonQuery();
                conexion.Close();
                return filas_afectadas;
            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return -1;
            }

        }




        public static int Insert(string tabla, string columnas, string valores)
        {
            string consulta = " INSERT INTO  " + tabla + " ( " + columnas + " ) VALUES (" + valores + ")";
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                int filas_afectadas = resultado_consulta.ExecuteNonQuery();
                conexion.Close();
                return filas_afectadas;
            }
            catch (Exception exp)
            {
                string borrar_ = exp.Message;
                return -1;
            }

        }



        public static int InsertGetID(string tabla, string columnas, string valores)
        {
            string consulta = " INSERT INTO  " + tabla + " ( " + columnas + " ) VALUES (" + valores + ")";
            consulta = consulta + " SELECT SCOPE_IDENTITY();";
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                object filas_afectadas = resultado_consulta.ExecuteScalar();
                int id = int.Parse(filas_afectadas.ToString());
                conexion.Close();
                return id;
            }
            catch (Exception)
            {
                return -1;
            }

        }




        public static int Update(string update, string set, string where)
        {
            string consulta = update + " " + set + " " + where;
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                int filas_afectadas = resultado_consulta.ExecuteNonQuery();
                conexion.Close();
                return filas_afectadas;
            }
            catch (Exception)
            {
                return -1;
            }
        }



        public static int Delete(string tabla, string where)
        {
            string consulta = tabla + " " + where;
            try
            {
                SqlConnection conexion = new SqlConnection(stringConexion);
                conexion.Open();

                SqlCommand resultado_consulta = new SqlCommand(consulta, conexion);
                int filas_afectadas = resultado_consulta.ExecuteNonQuery();
                conexion.Close();
                return filas_afectadas;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static DataTable newTabla(int numero_columnas)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < numero_columnas; i++)
            {
                dt.Columns.Add();
            }
            return dt;
        }




    }


}

