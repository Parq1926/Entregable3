using SistemaTarjetas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaTarjetas.Services
{
    public interface IAutenticador
    {
        Task<Cliente> ObtenerClientePorUsuarioAsync(string usuario);
        Task<List<Cuenta>> ObtenerCuentasPorClienteAsync(string idCliente);
        Task<List<Tarjeta>> ObtenerTarjetasPorClienteAsync(string idCliente);
    }
}