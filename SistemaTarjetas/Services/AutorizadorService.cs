using ServiceReference2;  // ← Tu WS Autorizador
using SistemaTarjetas.Models;
using SistemaTarjetas.Helpers;  // ← Para usar la misma encriptación AES
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SistemaTarjetas.Services
{
    public class AutorizadorService : IAutorizadorService
    {
        public async Task<AutorizacionResponse> AutorizarRetiro(AutorizacionRequest request)
        {
            try
            {
                Debug.WriteLine("=== LLAMANDO A WS AUTORIZADOR ===");
                Debug.WriteLine($"Monto: {request.Monto}");
                Debug.WriteLine($"Tarjeta: {request.NumeroTarjeta}");

                // Encriptar datos
                string tarjetaEncriptada = Encriptacion.Encriptar(request.NumeroTarjeta);
                string cvvEncriptado = Encriptacion.Encriptar(request.CVV);
                string pinEncriptado = string.IsNullOrEmpty(request.PIN) ? "" : Encriptacion.Encriptar(request.PIN);
                string fechaEncriptada = Encriptacion.Encriptar(request.FechaVencimiento);

                Debug.WriteLine($"Tarjeta encriptada: {tarjetaEncriptada}");

                // Crear cliente del WS
                var client = new ServiceClient();

                var wsRequest = new RetiroRequest
                {
                    NumeroTarjeta = tarjetaEncriptada,
                    Cvv = cvvEncriptado,
                    Pin = pinEncriptado,
                    FechaVencimiento = fechaEncriptada,
                    NombreCliente = request.NombreCliente,
                    IdComercioOCajero = request.IdentificacionComercio,
                    Monto = request.Monto
                };

                Debug.WriteLine("Enviando request al WS Autorizador...");

                // Llamar al WS
                var wsResponse = await client.RetiroAsync(wsRequest);
                await client.CloseAsync();

                Debug.WriteLine($"Respuesta del WS: Resultado={wsResponse.Resultado}, Mensaje={wsResponse.Mensaje}");

                return new AutorizacionResponse
                {
                    Resultado = wsResponse.Resultado,
                    Mensaje = wsResponse.Mensaje
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR en AutorizadorService: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return new AutorizacionResponse
                {
                    Resultado = false,
                    Mensaje = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<AutorizacionResponse> ConsultarSaldo(AutorizacionRequest request)
        {
            try
            {
                // Encriptar datos sensibles
                string tarjetaEncriptada = Encriptacion.Encriptar(request.NumeroTarjeta);
                string cvvEncriptado = Encriptacion.Encriptar(request.CVV);
                string fechaEncriptada = Encriptacion.Encriptar(request.FechaVencimiento);

                var client = new ServiceClient();

                var wsRequest = new ConsultaRequest
                {
                    NumeroTarjeta = tarjetaEncriptada,
                    Cvv = cvvEncriptado,
                    FechaVencimiento = fechaEncriptada,
                    IdComercioOCajero = request.IdentificacionComercio
                };

                var wsResponse = await client.ConsultaAsync(wsRequest);
                await client.CloseAsync();

                decimal saldo = 0;
                if (decimal.TryParse(wsResponse.Saldo, out decimal parsed))
                {
                    saldo = parsed;
                }

                return new AutorizacionResponse
                {
                    Resultado = wsResponse.Resultado,
                    Mensaje = wsResponse.Mensaje,
                    Saldo = saldo
                };
            }
            catch (Exception ex)
            {
                return new AutorizacionResponse
                {
                    Resultado = false,
                    Mensaje = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<AutorizacionResponse> CambiarPIN(AutorizacionRequest request)
        {
            try
            {
                // Encriptar datos sensibles
                string tarjetaEncriptada = Encriptacion.Encriptar(request.NumeroTarjeta);
                string cvvEncriptado = Encriptacion.Encriptar(request.CVV);
                string pinActualEncriptado = Encriptacion.Encriptar(request.PINActual);
                string pinNuevoEncriptado = Encriptacion.Encriptar(request.PINNuevo);
                string fechaEncriptada = Encriptacion.Encriptar(request.FechaVencimiento);

                var client = new ServiceClient();

                var wsRequest = new CambioPinRequest
                {
                    NumeroTarjeta = tarjetaEncriptada,
                    Cvv = cvvEncriptado,
                    PinActual = pinActualEncriptado,
                    PinNuevo = pinNuevoEncriptado,
                    FechaVencimiento = fechaEncriptada,
                    IdComercioOCajero = request.IdentificacionComercio
                };

                var wsResponse = await client.CambioPinAsync(wsRequest);
                await client.CloseAsync();

                return new AutorizacionResponse
                {
                    Resultado = wsResponse.Resultado,
                    Mensaje = wsResponse.Mensaje
                };
            }
            catch (Exception ex)
            {
                return new AutorizacionResponse
                {
                    Resultado = false,
                    Mensaje = $"Error: {ex.Message}"
                };
            }
        }
    }
}