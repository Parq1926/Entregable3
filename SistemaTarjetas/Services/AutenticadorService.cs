using ServiceReference2;  // WS Autenticador
using SistemaTarjetas.Models;
using System;
using System.Threading.Tasks;
using WSAutenticador;

namespace SistemaTarjetas.Services
{
    public interface IAutenticadorService
    {
        Task<AutenticacionResponse> Autenticar(string usuario, string contrasena, int tipoUsuario);
    }

    public class AutenticadorService : IAutenticadorService
    {
        public async Task<AutenticacionResponse> Autenticar(string usuario, string contrasena, int tipoUsuario)
        {
            try
            {
                var client = new ServicioAutenticacionClient();

                var credenciales = new Credenciales
                {
                    UsuarioEncriptado = usuario,
                    ContrasenaEncriptada = contrasena
                };

                var resultado = await client.ValidarLoginAsync(credenciales);
                await client.CloseAsync();

                return new AutenticacionResponse
                {
                    Resultado = resultado.Resultado,
                    Mensaje = resultado.Mensaje,
                    TipoUsuario = resultado.TipoUsuario
                };
            }
            catch (Exception ex)
            {
                return new AutenticacionResponse
                {
                    Resultado = false,
                    Mensaje = $"Error: {ex.Message}",
                    TipoUsuario = 0
                };
            }
        }
    }
}