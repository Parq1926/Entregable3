using SistemaTarjetas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaTarjetas.Services
{
    public interface IConsultaClienteService
    {
        Task<List<Cuenta>> ObtenerCuentasClienteAsync(string idCliente);
        Task<List<Tarjeta>> ObtenerTarjetasClienteAsync(string idCliente);
        Task<List<MovimientoCuenta>> ObtenerMovimientosCuentaAsync(string numeroCuenta, string idCliente);
        Task<List<MovimientoTarjeta>> ObtenerMovimientosTarjetaCreditoAsync(string numeroTarjeta, string idCliente);
    }
}