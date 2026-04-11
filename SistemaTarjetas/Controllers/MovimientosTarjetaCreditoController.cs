using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Services;
using System.Threading.Tasks;

namespace SistemaTarjetas.Controllers
{
    public class MovimientosTarjetaCreditoController : Controller
    {
        private readonly IConsultaClienteService _consultaService;

        public MovimientosTarjetaCreditoController(IConsultaClienteService consultaService)
        {
            _consultaService = consultaService;
        }

        public async Task<IActionResult> Index(string numeroTarjeta = null)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            var idCliente = HttpContext.Session.GetString("IdCliente");
            if (string.IsNullOrEmpty(idCliente))
            {
                idCliente = "CLI001";
            }

            ViewBag.Usuario = usuario;
            ViewBag.NumeroTarjeta = numeroTarjeta;

            if (!string.IsNullOrEmpty(numeroTarjeta))
            {
                var movimientos = await _consultaService.ObtenerMovimientosTarjetaCreditoAsync(numeroTarjeta, idCliente);
                ViewBag.Movimientos = movimientos;
            }

            return View();
        }
    }
}