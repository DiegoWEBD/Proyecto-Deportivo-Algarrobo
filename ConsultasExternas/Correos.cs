using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebApplicationTemplateBase.ConsultasExternas
{
    public static class Correos
    {

        public static Boolean EnviarCorreo(object[] destinos, string nombreClase, string horarioClase, DateTime fecha)
        {
            try
            {
                MailMessage mail = new MailMessage();

                for(int i=0; i < destinos.Length; i++)
                {
                    object[] fila_aux = (object[])destinos[i];
                    string correo = fila_aux[7].ToString();
                    mail.To.Add(correo);
                }
                if(destinos.Length > 0)
                {
                    if (horarioClase.Contains("/")){
                        horarioClase = horarioClase.Replace("/", " a ");
                    }
                    //aca enviar correos
                    string diaPalabra = fecha.ToString("dddd");
                    string mes = fecha.ToString("MMMM");
                    string dia = fecha.ToString("dd");
                    mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                    mail.Subject = "Cancelación de Clase " + nombreClase + " del " + diaPalabra;
                    string Body = "<div>Por Motivos de fuerza mayor la clase de " + nombreClase + " del " + diaPalabra + " " + dia + " de " + mes + " en el bloque de " + horarioClase + " a sido cancelada</div>" +
                        "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                    "</br></br></br><div>Atte..</div>" +
                    "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                    smtp.EnableSsl = false;
                    //smtp.Send(mail);
                }
               
                return true;
            }
            catch(Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }



        public static Boolean EnviarCorreoCancelacionInstalacion(object[] destinos, string nombreInstalacion,  DateTime fecha)
        {
            try
            {
                MailMessage mail = new MailMessage();

                for (int i = 0; i < destinos.Length; i++)
                {
                    object[] fila_aux = (object[])destinos[i];
                    string correo = fila_aux[7].ToString();
                    mail.To.Add(correo);
                }
                if (destinos.Length > 0)
                {
                    //aca enviar correos
                    string diaPalabra = fecha.ToString("dddd");
                    string mes = fecha.ToString("MMMM");
                    string dia = fecha.ToString("dd");
                    mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                    mail.Subject = "Cancelación de Reserva de Instalacion " + nombreInstalacion + " del " + diaPalabra;
                    string Body = "<div>Por Motivos de fuerza mayor las reservas para  " + nombreInstalacion + " del " + diaPalabra + " " + dia + " de " + mes + " ha sido cancelada</div>" +
                        "</br><div>Porfavor comunicarse con la administración</div>" +
                        "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                    "</br></br></br><div>Atte..</div>" +
                    "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                    smtp.EnableSsl = false;
                    //smtp.Send(mail);
                }

                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }




        public static Boolean EnviarCorreoCancelacionInstalacionPorUsuario(object[] destinos, string nombreInstalacion, string fecha, string NombreUsuario, string horario)
        {
            try
            {
                MailMessage mail = new MailMessage();

                for (int i = 0; i < destinos.Length; i++)
                {
                    string correo = destinos[i].ToString();
                    mail.To.Add(correo);
                }
                if (destinos.Length > 0)
                {
                    //aca enviar correos

                    mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                    mail.Subject = "Cancelación de Reserva de Instalacion " + nombreInstalacion + " del " +fecha+"a las"+ horario;
                    string Body = "<div>El Usuario "+NombreUsuario+" ha Cancelado la Reserva para  " + nombreInstalacion + " del " + fecha + " en el horario de : "+horario+".</div>" +
                        "</br><div></div>" +
                        "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                    "</br></br></br><div>Atte..</div>" +
                    "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                    smtp.EnableSsl = false;
                    //smtp.Send(mail);
                }

                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }


        public static Boolean EnviarCorreoCancelacionInstalacionesDia(object[] destinos, DateTime fecha)
        {
            try
            {
                MailMessage mail = new MailMessage();

                for (int i = 0; i < destinos.Length; i++)
                {
                    object[] fila_aux = (object[])destinos[i];
                    string correo = fila_aux[7].ToString();
                    mail.To.Add(correo);
                }
                if (destinos.Length > 0)
                {
                    //aca enviar correos
                    string diaPalabra = fecha.ToString("dddd");
                    string mes = fecha.ToString("MMMM");
                    string dia = fecha.ToString("dd");
                    mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                    mail.Subject = "Cancelación de Reserva de Instalaciones del " + diaPalabra;
                    string Body = "<div>Por Motivos de fuerza mayor todas las reservas para del " + diaPalabra + " " + dia + " de " + mes + " han sido canceladas</div>" +
                        "</br><div>Porfavor comunicarse con la administración</div>" +
                        "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                    "</br></br></br><div>Atte..</div>" +
                    "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                    smtp.EnableSsl = false;
                    //smtp.Send(mail);
                }

                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }



        public static Boolean EnviarCorreoConfirmacionReserva(string destino, string nombreClase, string horarioClase, DateTime fecha, string nombreCompleto)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(destino);

                if (horarioClase.Contains("/"))
                {
                    horarioClase = horarioClase.Replace("/", " a ");
                }
                //aca enviar correos
                string diaPalabra = fecha.ToString("dddd");
                string mes = fecha.ToString("MMMM");
                string dia = fecha.ToString("dd");
                mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                mail.Subject = "Confirmación de reserva de Clase " + nombreClase + " del " + diaPalabra+" "+ dia;
                string Body = "<div>Confirmación de reserva de Clase de " + nombreClase + " del " + diaPalabra + " " + dia + " de " + mes + " en el bloque de " + horarioClase + " a nombre de "+nombreCompleto+"</div>" +
                    "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                "</br></br></br><div>Atte..</div>" +
                "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                smtp.EnableSsl = false;
                //smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }



        
        public static Boolean EnviarCorreoConfirmacionPagoMensualidad(string destino, Models.Tranferencia Tranferencia )
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(destino);
                
                //aca enviar correos
                int mes = Tranferencia.ObtenerMesMensualidad();
                int year = Tranferencia.ObtenerYearMensualidad();
                string mes_string = obtenerMes(mes);

                mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                mail.Subject = "Confirmación de Pago Mensualidad del " + mes_string + " del " + year;
                string Body = "<div>"+ TablaMensualidad(Tranferencia) + "</div>" +

                    "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                "</br></br></br><div>Atte..</div>" +
                "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                smtp.EnableSsl = false;
                //smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }


        public static Boolean EnviarCorreoConfirmacionPagoDia(string destino, string fecha, int idCalendario, Models.Tranferencia Tranferencia)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(destino);
                string nombreClase = "";
                string horarioClase = "";
                ConsultasExternas.RespuestaSQL calendario = ConsultasExternas.ConsultasCalendario.ObtenerDatosCalendario(idCalendario);
                if (calendario.Correcto)
                {
                    if (calendario.Salida.Length > 0)
                    {
                        object[] arreglo = (object[])calendario.Salida[0];
                        nombreClase = arreglo[1].ToString();
                        horarioClase = arreglo[2].ToString();
                        horarioClase = horarioClase.Replace("/", " a ");
                    }
                }


                //aca enviar correos
                int mes = Tranferencia.ObtenerMesMensualidad();
                int year = Tranferencia.ObtenerYearMensualidad();
                string mes_string = obtenerMes(mes);

                mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                mail.Subject = "Confirmación de Pago para Clase : "+ nombreClase + " de las " + horarioClase + " del " + fecha;
                string Body = "<div>" + TablaMensualidad(Tranferencia) + "</div>" +

                    "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                "</br></br></br><div>Atte..</div>" +
                "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                smtp.EnableSsl = false;
                //smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }


        public static Boolean EnviarCorreoConfirmacionPagoArriendoInstalacion(string destino, string fecha, int idCalendario, Models.Tranferencia Tranferencia)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(destino);
                string nombreClase = "";
                string horarioClase = "";
                ConsultasExternas.RespuestaSQL calendario = ConsultasExternas.ConsultaInstalaciones.ObtenerDatosCalendario(idCalendario);
                if (calendario.Correcto)
                {
                    if (calendario.Salida.Length > 0)
                    {
                        object[] arreglo = (object[])calendario.Salida[0];
                        nombreClase = arreglo[1].ToString();
                        horarioClase = arreglo[2].ToString();
                        horarioClase = horarioClase.Replace("/", " a ");
                    }
                }


                //aca enviar correos
                int mes = Tranferencia.ObtenerMesMensualidad();
                int year = Tranferencia.ObtenerYearMensualidad();
                string mes_string = obtenerMes(mes);

                mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                mail.Subject = "Confirmación de Pago Arriendo de Instalación  : " + nombreClase + " de las " + horarioClase + " del " + fecha;
                string Body = "<div>" + TablaMensualidad(Tranferencia) + "</div>" +

                    "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                "</br></br></br><div>Atte..</div>" +
                "</br><div>Sistema de Reservas de Arriendo Deportivo Algarrobo</div>";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                smtp.EnableSsl = false;
                //smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }



        public static Boolean EnviarCorreoConfirmacionArriendoInstalacion(string destino, int idCalendario, string nombreCompleto, DateTime fecha)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(destino);
                string nombreClase = "";
                string horarioClase = "";
                ConsultasExternas.RespuestaSQL calendario = ConsultasExternas.ConsultaInstalaciones.ObtenerDatosCalendario(idCalendario);
                if (calendario.Correcto)
                {
                    if (calendario.Salida.Length > 0)
                    {
                        object[] arreglo = (object[])calendario.Salida[0];
                        nombreClase = arreglo[1].ToString();
                        horarioClase = arreglo[2].ToString();
                        horarioClase = horarioClase.Replace("/", " a ");
                    }
                }


                //aca enviar correos
                string diaPalabra = fecha.ToString("dddd");
                string mes = fecha.ToString("MMMM");
                string dia = fecha.ToString("dd");

                mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                mail.Subject = "Confirmación de Pago Arriendo de Instalación  : " + nombreClase + " de las " + horarioClase + " del " + fecha;
                mail.Subject = "Confirmación de Reserva de Instalación " + nombreClase + " del " + diaPalabra + " " + dia;
                string Body = "<div>Confirmación de reserva de instalación de " + nombreClase + " del " + diaPalabra + " " + dia + " de " + mes + " en el bloque de " + horarioClase + " a nombre de " + nombreCompleto + "</div>" +
                    "</br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                "</br></br></br><div>Atte..</div>" +
                "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                smtp.EnableSsl = false;
                //smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }




        public static Boolean EnviarCorreoError(string usuario, string OrdenCompra, string DetalleTarjeta, int Monto, int CantidadCuotas, decimal MontoCuotas, string CodigoAutorizacion, string FechaAutorizacion, string FechaTransaccion, string TipoTransaccion, string VCI, string Status, string Token)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add("informatica@canal-ltda.cl");
                //aca enviar correos
                mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                mail.Subject = "ERROR Transaccion " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string Body = "usuario : " + usuario + "</br>" +
                "Orden Compra : " + OrdenCompra + "</br>"+
                "Detalle Tarjeta : " + DetalleTarjeta + "</br>"+
                "Monto : " + Monto + "</br>"+
                "Cantidad Cuotas : " + CantidadCuotas + "</br>"+
                "Codigo Autorizacion : " + CodigoAutorizacion + "</br>"+
                "Fecha Autorizacion : " + FechaAutorizacion + "</br>"+
                "Fecha Transaccion : " + FechaTransaccion + "</br>"+
                "Tipo Transaccion : " + TipoTransaccion + "</br>"+
                "VCI : " + VCI + "</br>"+
                "Status : " + Status + "</br>"+
                "Token : " + Token + "</br>";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                smtp.EnableSsl = false;
                //smtp.Send(mail);

                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }




        public static Boolean EnviarCorreoSolicitudReservaInstalacion(object[] destinos, string nombreInstalacion, string requerimientos, DateTime fecha, string nombreCompleto, string correo, string ntelefono)
        {
            try
            {
                MailMessage mail = new MailMessage();
                for (int i = 0; i < destinos.Length; i++)
                {
                    string correo_aux = destinos[i].ToString();
                    mail.To.Add(correo_aux);
                }

                //aca enviar correos
                string diaPalabra = fecha.ToString("dddd");
                string mes = fecha.ToString("MMMM");
                string dia = fecha.ToString("dd");
                mail.From = new MailAddress("reservas@reservasdeportivoalgarrobo.cl");
                mail.Subject = "Solicitud Arriendo de " + nombreInstalacion + " del " + diaPalabra + " " + dia;
                string Body = "<div>Solicitud de reserva de " + nombreInstalacion + " del " + diaPalabra + " " + dia + " de " + mes + " a nombre de " + nombreCompleto + "</div>" +
                    "</br><div>Email : " + correo + "</div>" +
                    "</br><div>Teléfono : " + ntelefono + "</div>" +
                    "</br><div>Comentarios : " + requerimientos + "</div>" +
                    "</br></br><div>Este e-mail es generado de manera automática,por favor no respondas a este mensaje.</div>" +
                "</br></br></br><div>Atte..</div>" +
                "</br><div>Sistema de Reservas de Horarios Deportivo Algarrobo</div>";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.reservasdeportivoalgarrobo.cl";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("reservas@reservasdeportivoalgarrobo.cl", "vZ_1e93t5"); // Enter seders User name and password  
                smtp.EnableSsl = false;
                //smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                string borrar_ = e.Message;
                return false;
            }
        }








        private static string obtenerMes(int mes)
        {
            switch (mes)
            {
                case 1:
                    {
                        return "Enero";
                    }
                case 2:
                    {
                        return "Febrero";
                    }
                case 3:
                    {
                        return "Marzo";
                    }
                case 4:
                    {
                        return "Abril";
                    }
                case 5:
                    {
                        return "Mayo";
                    }
                case 6:
                    {
                        return "Junio";
                    }
                case 7:
                    {
                        return "Julio";
                    }
                case 8:
                    {
                        return "Agosto";
                    }
                case 9:
                    {
                        return "Septiembre";
                    }
                case 10:
                    {
                        return "Octubre";
                    }
                case 11:
                    {
                        return "Noviembre";
                    }
                case 12:
                    {
                        return "Diciembre";
                    }
            }

            return mes.ToString();
        }



        private static string TablaMensualidad(Models.Tranferencia Tranferencia)
        {
            string salida = "<table align=\"center\" cellpadding=\"1\" cellspacing =\"0\" width =\"90%\" style =\"border: solid 1px #0073cb; border-radius: 4px; padding: 8px; font-size: 16px; line-height: 20px; font-weight: 300; border-spacing: 0 2px\" >" +
                             "<thead><tr style=\"text-align:center; background-color:darkblue; color:whitesmoke\" >" +
                                        "<td colspan=\"3\" > Estado de Compra</td></tr></thead><tbody>" +
                                    "<tr style=\"background-color: #ebebec\" >" +
                                        "<td>Número de orden de pedido</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" > "+ Tranferencia.getBuyOrder()+"</td>" +
                                    "</tr> " +
                                    "<tr> " +
                                        "<td>Monto</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" >"+ Tranferencia.getMontoFormatoMiles()+"</td>" +
                                    "</tr> " +
                                    "<tr style=\"background-color: #ebebec\" >" +
                                        "<td>Código de autorización de la transacción</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" > "+Tranferencia.getAuthorizationCode()+"</td>" +
                                    "</tr> " +
                                    "<tr style=\"background-color: #ebebec\" >" +
                                        "<td>Detalle Tarjeta</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" > " + Tranferencia.getCardNumber() + "</td>" +
                                    "</tr> " +
                                    "<tr style=\"background-color: #ebebec\" >" +
                                        "<td>Codigo VCI</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" > " + Tranferencia.getVci() + "</td>" +
                                    "</tr> " +
                                    "<tr style=\"background-color: #ebebec\" >" +
                                        "<td>Codigo Tranferencia</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" > " + Tranferencia.Token_WS+ "</td>" +
                                    "</tr> " +
                                    "<tr> " +
                                        "<td>Fecha de la transacción</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" > "+Tranferencia.getTransactionDateHorarioChile(4)+"</td>" +
                                    "</tr> " +
                                    "<tr style=\"background-color: #ebebec\" >" +
                                        "<td>Tipo de pago realizado</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" > "+Tranferencia.pasarAFormatoTipoPago()+"</td>" +
                                    "</tr> " +
                                    "<tr> " +
                                        "<td>Tipo de cuota</td> " +
                                        "<td>:</td> " +
                                        "<td style=\"text-align:right\" >"+Tranferencia.esCuotas()+"</td>" +
                                    "</tr> ";
                                    if (Tranferencia.esCuotas().Equals("Sin Cuotas") == false) 
                                    {
                                        salida = salida + "<tr style=\"background-color: #ebebec\" >" +
                                            "<td>Cantidad de cuotas</td> " +
                                            "<td>:</td> " +
                                            "<td style= \"text-align:right\">"+Tranferencia.getInstallmentsNumber()+"</td>" +
                                        "</tr> " +
                                        "<tr> " +
                                            "<td>Monto de cada cuota</td> " +
                                            "<td>:</td> " +
                                            "<td style=\"text-align:right\" > "+Tranferencia.getInstallmentsAmount()+"</td>" +
                                        "</tr> ";
                                    }
                                salida = salida + "</tbody> </table> ";

            return salida;
        }
        

    }
}

