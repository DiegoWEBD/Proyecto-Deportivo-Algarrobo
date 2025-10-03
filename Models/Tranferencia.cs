using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;

namespace WebApplicationTemplateBase.Models
{
    public class Tranferencia
    {

        Transbank.Webpay.WebpayPlus.Responses.CommitResponse Result;
        public string Token_WS { get; }
        public Tranferencia(string token_ws)
        {
            Result = (new Transaction(new Options("CONSULTAR CODIGOS  TRANSBANK", "CONSULTAR CODIGOS TRANSBANK", WebpayIntegrationType.Live))).Commit(token_ws);
            Token_WS = token_ws;
        }


        public Tranferencia(string token_ws, Boolean falsa)
        {
           // Result = (new Transaction()).Commit(token_ws);
            //Token_WS = token_ws;
        }

        public int getResponseCode()
        {
            int? salida = Result.ResponseCode;
            if (salida.HasValue)
            {
                return salida.Value;
            }
            return -3;
        }

        public string getBuyOrder()
        {
            string salida = Result.BuyOrder;
            return salida;
        }

        public string getCardNumber()
        {
            string salida = Result.CardDetail.CardNumber; 
            return salida;
        }

        public int getMonto()
        {
            decimal? salida = Result.Amount;
            if (salida.HasValue)
            {
                return (int)(salida.Value);
            }
            return 0;
        }

        public int getInstallmentsNumber()
        {
            int? salida = Result.InstallmentsNumber;
            if (salida.HasValue)
            {
                return salida.Value;
            }
            return 0;
        }

        public decimal getInstallmentsAmount()
        {
            decimal? salida = Result.InstallmentsAmount;
            if (salida.HasValue)
            {
                return salida.Value;
            }
            return 0;
        }

        public string getAuthorizationCode()
        {
            string salida = Result.AuthorizationCode;
            return salida;
        }

        public string getAccountingDate()
        {
            string salida = Result.AccountingDate;
            return salida;
        }

        public DateTime getTransactionDate()
        {
            DateTime? salida = Result.TransactionDate;
            if (salida.HasValue)
            {
                return salida.Value;
            }
            return new DateTime();
        }

        public string getPaymentTypeCode()
        {
            string salida = Result.PaymentTypeCode;
            return salida;
        }

        public string getVci()
        {
            string salida = Result.Vci;
            return salida;
        }
        public string getStatus()
        {
            string salida = Result.Status;
            return salida;
        }


        public string getMontoFormatoMiles()
        {
            decimal? monto = Result.Amount;
            if (monto != null)
            {
                decimal monto_aux = monto.Value;
                NumberFormatInfo formato = new CultureInfo("es-CL").NumberFormat;
                formato.CurrencyGroupSeparator = ".";
                formato.NumberDecimalSeparator = ",";

                return monto_aux.ToString("C", formato);
            }
            return "";
        }

        public DateTime getTransactionDateHorarioChile(int horas)
        {
            
            DateTime ? fecha = Result.TransactionDate;
            if (fecha.HasValue)
            {
                DateTime fecha_aux = fecha.Value;
                fecha_aux =  fecha_aux.AddHours(-1 * horas);
                return fecha_aux;
            }
            return new DateTime();
        }

        public string esCuotas()
        {
            int? CantidadCuotas = Result.InstallmentsNumber;
            if (CantidadCuotas != null)
            {
                if (CantidadCuotas > 0)
                {
                    return "En Cuotas";
                }
                return "Sin Cuotas";
            }
            return "Sin Cuotas";
        }

        public string pasarAFormatoTipoPago()
        {
            string formatoPago = Result.PaymentTypeCode;
            switch (formatoPago)
            {
                case "VD":
                    return "Venta Débito";
                case "VN":
                    return "Venta Normal";
                case "VC":
                    return "Venta en cuotas.";
                case "SI":
                    return "3 cuotas sin interés.";
                case "S2":
                    return "2 cuotas sin interés.";
                case "NC":
                    return "N Cuotas sin interés";
                case "VP":
                    return "Venta Prepago.";
                default:
                    return formatoPago;
            }
        }

        public string getDetalleResponseCode()
        {
            int ResponseCode = this.getResponseCode();
            switch (ResponseCode)
            {
                case -1:
                    return "Rechazada, Posible error en el ingreso de datos de la transacción";
                case -2:
                    return "Rechazada, Se produjo fallo al procesar la transacción, este mensaje de rechazo se encuentra relacionado a parámetros de la tarjeta y/ o su cuenta asociada";
                case -3:
                    return "Rechazada, Error en Transacción";
                case -4:
                    return "Rechazada, Transacción con riesgo de posible fraude";
            }
            return "";
        }

        public int ObtenerMesMensualidad()
        {
            string ordenCompra = this.Result.BuyOrder;
            string[] valores = ordenCompra.Split('_');
            int mes = int.Parse(valores[0]);
            return mes;
        }

        public int ObtenerYearMensualidad()
        {
            string ordenCompra = this.Result.BuyOrder;
            string[] valores = ordenCompra.Split('_');
            int year = int.Parse(valores[1]);
            return year;
        }

    }
}