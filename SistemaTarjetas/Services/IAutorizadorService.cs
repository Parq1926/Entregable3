using SistemaTarjetas.Models;
using System.Threading.Tasks;

namespace SistemaTarjetas.Services
{
    public interface IAutorizadorService
    {
        Task<AutorizacionResponse> AutorizarRetiro(AutorizacionRequest request);
        Task<AutorizacionResponse> ConsultarSaldo(AutorizacionRequest request);
        Task<AutorizacionResponse> CambiarPIN(AutorizacionRequest request);
    }
}