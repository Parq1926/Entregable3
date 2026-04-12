using SistemaTarjetas.Models;
using ServiceReference2;
using System;
using System.Threading.Tasks;

namespace SistemaTarjetas.Services
{
    // Elimina esta parte:
    // public interface IAutorizadorService
    // {
    //     Task<AutorizacionResponse> AutorizarRetiro(AutorizacionRequest request);
    // }

    public class AutorizadorService : IAutorizadorService
    {
        private readonly ServiceClient _wsClient;

        public AutorizadorService()
        {
            _wsClient = new ServiceClient();
        }

        public async Task<AutorizacionResponse> AutorizarRetiro(AutorizacionRequest request)
        {
            try
            {
                var wsRequest = new RetiroRequest
                {
                    NumeroTarjeta = request.NumeroTarjeta,
                    Cvv = request.CVV,
                    Pin = request.PIN,
                    FechaVencimiento = request.FechaVencimiento,
                    NombreCliente = request.NombreCliente,
                    IdComercioOCajero = request.IdentificacionComercio,
                    Monto = request.Monto
                };

                var response = await _wsClient.RetiroAsync(wsRequest);

                return new AutorizacionResponse
                {
                    Resultado = response.Resultado,
                    Mensaje = response.Mensaje
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

        // Agrega también ConsultarSaldo y CambiarPIN si los necesitas
        public async Task<AutorizacionResponse> ConsultarSaldo(AutorizacionRequest request)
        {
            // Implementación pendiente
            return new AutorizacionResponse { Resultado = false, Mensaje = "No implementado" };
        }

        public async Task<AutorizacionResponse> CambiarPIN(AutorizacionRequest request)
        {
            // Implementación pendiente
            return new AutorizacionResponse { Resultado = false, Mensaje = "No implementado" };
        }
    }
}