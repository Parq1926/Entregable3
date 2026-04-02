using ServiceReference2;  // ← Tu WS Autorizador
using SistemaTarjetas.Models;
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
                Debug.WriteLine($"URL: http://localhost:64771/Service.svc");
                Debug.WriteLine($"Monto: {request.Monto}");
                Debug.WriteLine($"Tarjeta: {request.NumeroTarjeta}");
                Debug.WriteLine($"CVV: {request.CVV}");
                Debug.WriteLine($"PIN: {request.PIN ?? "No proporcionado"}");
                Debug.WriteLine($"Comercio: {request.IdentificacionComercio}");

                // Crear cliente del WS Autorizador
                var client = new ServiceClient();

                // Preparar request para el WS
                var wsRequest = new RetiroRequest
                {
                    NumeroTarjeta = request.NumeroTarjeta,
                    Cvv = request.CVV,
                    Pin = request.PIN ?? "",
                    FechaVencimiento = request.FechaVencimiento,
                    NombreCliente = request.NombreCliente,
                    IdComercioOCajero = request.IdentificacionComercio,
                    Monto = request.Monto
                };

                Debug.WriteLine("Enviando request al WS...");

                // Llamar al WS
                var wsResponse = await client.RetiroAsync(wsRequest);

                Debug.WriteLine($"Respuesta WS - Resultado: {wsResponse.Resultado}, Mensaje: {wsResponse.Mensaje}");

                // Cerrar cliente
                await client.CloseAsync();

                return new AutorizacionResponse
                {
                    Resultado = wsResponse.Resultado,
                    Mensaje = wsResponse.Mensaje
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR WS: {ex.Message}");
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
                var client = new ServiceClient();

                var wsRequest = new ConsultaRequest
                {
                    NumeroTarjeta = request.NumeroTarjeta,
                    Cvv = request.CVV,
                    FechaVencimiento = request.FechaVencimiento,
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
                var client = new ServiceClient();

                var wsRequest = new CambioPinRequest
                {
                    NumeroTarjeta = request.NumeroTarjeta,
                    Cvv = request.CVV,
                    PinActual = request.PINActual,
                    PinNuevo = request.PINNuevo,
                    FechaVencimiento = request.FechaVencimiento,
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