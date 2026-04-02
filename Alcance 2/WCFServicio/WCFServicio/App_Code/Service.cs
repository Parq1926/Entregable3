using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Web.Script.Serialization;
using WCFServicio.DataAccess;
using WCFServicio.Entities;
using WCFServicio.Security;

public class Service : IService
{
    private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

    public RetiroResponse Retiro(RetiroRequest request)
    {
        try
        {
            string numeroTarjeta = request.NumeroTarjeta;
            string cvv = request.Cvv;
            string pin = request.Pin;
            string fechaVencimiento = request.FechaVencimiento;
            string nombreCliente = request.NombreCliente;
            string idComercioOCajero = request.IdComercioOCajero;
            decimal monto = request.Monto;

            if (string.IsNullOrWhiteSpace(numeroTarjeta) ||
                string.IsNullOrWhiteSpace(cvv) ||
                string.IsNullOrWhiteSpace(fechaVencimiento) ||
                string.IsNullOrWhiteSpace(nombreCliente) ||
                string.IsNullOrWhiteSpace(idComercioOCajero) ||
                monto <= 0)
            {
                return new RetiroResponse
                {
                    Resultado = false,
                    Mensaje = "No se ha autorizado la transacción."
                };
            }

            if (monto > 50000 && string.IsNullOrWhiteSpace(pin))
            {
                return new RetiroResponse
                {
                    Resultado = false,
                    Mensaje = "No se ha autorizado la transacción."
                };
            }

            AutorizadorClient socket = new AutorizadorClient();

            //Cifrado.
            string jsonRetiro = _serializer.Serialize(new
            {
                accion = "retiro",
                NumeroTarjeta = AES.Encrypt(numeroTarjeta),
                Cvv = AES.Encrypt(cvv),
                Pin = string.IsNullOrEmpty(pin) ? "" : AES.Encrypt(pin),
                FechaVencimiento = AES.Encrypt(fechaVencimiento),
                NombreCliente = AES.Encrypt(nombreCliente),
                IdComercioOCajero = AES.Encrypt(idComercioOCajero),
                Monto = monto
            });

            Console.WriteLine("🔐 JSON enviado: " + jsonRetiro);

            string respuestaRetiro = socket.Enviar(jsonRetiro);

            RespuestaAutorizador retiro =
                _serializer.Deserialize<RespuestaAutorizador>(respuestaRetiro);

            if (retiro == null || !retiro.ok)
            {
                return new RetiroResponse
                {
                    Resultado = false,
                    Mensaje = "No se ha autorizado la transacción."
                };
            }

            //La Confirmación también cifrada.
            string jsonConfirmacion = _serializer.Serialize(new
            {
                accion = "confirmacion_retiro",
                NumeroTarjeta = AES.Encrypt(numeroTarjeta),
                IdComercioOCajero = AES.Encrypt(idComercioOCajero),
                Monto = monto
            });

            string respuestaConfirmacion = socket.Enviar(jsonConfirmacion);

            RespuestaAutorizador confirmacion =
                _serializer.Deserialize<RespuestaAutorizador>(respuestaConfirmacion);

            if (confirmacion != null && confirmacion.ok)
            {
                return new RetiroResponse
                {
                    Resultado = true,
                    Mensaje = "Transacción exitosa"
                };
            }

            return new RetiroResponse
            {
                Resultado = false,
                Mensaje = "No se ha autorizado la transacción."
            };
        }
        catch
        {
            return new RetiroResponse
            {
                Resultado = false,
                Mensaje = "No se ha autorizado la transacción."
            };
        }
    }

    public ConsultaResponse Consulta(ConsultaRequest request)
    {
        try
        {
            string numeroTarjeta = request.NumeroTarjeta;
            string cvv = request.Cvv;
            string fechaVencimiento = request.FechaVencimiento;
            string idComercioOCajero = request.IdComercioOCajero;

            if (string.IsNullOrWhiteSpace(numeroTarjeta) ||
                string.IsNullOrWhiteSpace(cvv) ||
                string.IsNullOrWhiteSpace(fechaVencimiento) ||
                string.IsNullOrWhiteSpace(idComercioOCajero))
            {
                return new ConsultaResponse
                {
                    Resultado = false,
                    Mensaje = "No se ha autorizado la transacción.",
                    Saldo = ""
                };
            }

            AutorizadorClient socket = new AutorizadorClient();

            string json = _serializer.Serialize(new
            {
                accion = "consulta",
                NumeroTarjeta = AES.Encrypt(numeroTarjeta),
                Cvv = AES.Encrypt(cvv),
                FechaVencimiento = AES.Encrypt(fechaVencimiento),
                IdComercioOCajero = AES.Encrypt(idComercioOCajero)
            });

            Console.WriteLine("🔐 JSON enviado: " + json);

            string respuesta = socket.Enviar(json);

            RespuestaAutorizador obj =
                _serializer.Deserialize<RespuestaAutorizador>(respuesta);

            if (obj != null && obj.ok)
            {
                return new ConsultaResponse
                {
                    Resultado = true,
                    Mensaje = "Transacción exitosa",
                    Saldo = obj.saldo.ToString("#,##0.00", CultureInfo.InvariantCulture)
                };
            }

            return new ConsultaResponse
            {
                Resultado = false,
                Mensaje = "No se ha autorizado la transacción.",
                Saldo = ""
            };
        }
        catch
        {
            return new ConsultaResponse
            {
                Resultado = false,
                Mensaje = "No se ha autorizado la transacción.",
                Saldo = ""
            };
        }
    }

    public CambioPinResponse CambioPin(CambioPinRequest request)
    {
        try
        {
            string numeroTarjeta = request.NumeroTarjeta;
            string pinActual = request.PinActual;
            string pinNuevo = request.PinNuevo;
            string fechaVencimiento = request.FechaVencimiento;
            string cvv = request.Cvv;
            string idComercioOCajero = request.IdComercioOCajero;

            if (string.IsNullOrWhiteSpace(numeroTarjeta) ||
                string.IsNullOrWhiteSpace(pinActual) ||
                string.IsNullOrWhiteSpace(pinNuevo) ||
                string.IsNullOrWhiteSpace(fechaVencimiento) ||
                string.IsNullOrWhiteSpace(cvv) ||
                string.IsNullOrWhiteSpace(idComercioOCajero))
            {
                return new CambioPinResponse
                {
                    Resultado = false,
                    Mensaje = "No se ha autorizado la transacción."
                };
            }

            AutorizadorClient socket = new AutorizadorClient();

            string json = _serializer.Serialize(new
            {
                accion = "cambio_pin",
                NumeroTarjeta = AES.Encrypt(numeroTarjeta),
                PinActual = AES.Encrypt(pinActual),
                PinNuevo = AES.Encrypt(pinNuevo),
                FechaVencimiento = AES.Encrypt(fechaVencimiento),
                Cvv = AES.Encrypt(cvv),
                IdComercioOCajero = AES.Encrypt(idComercioOCajero)
            });

            Console.WriteLine("🔐 JSON enviado: " + json);

            string respuesta = socket.Enviar(json);

            RespuestaAutorizador obj =
                _serializer.Deserialize<RespuestaAutorizador>(respuesta);

            if (obj != null && obj.ok)
            {
                return new CambioPinResponse
                {
                    Resultado = true,
                    Mensaje = "Transacción exitosa"
                };
            }

            return new CambioPinResponse
            {
                Resultado = false,
                Mensaje = "No se ha autorizado la transacción."
            };
        }
        catch
        {
            return new CambioPinResponse
            {
                Resultado = false,
                Mensaje = "No se ha autorizado la transacción."
            };
        }
    }
}