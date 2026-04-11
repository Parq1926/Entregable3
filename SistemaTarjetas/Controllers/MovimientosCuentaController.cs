using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Services;
using System.Threading.Tasks;

namespace SistemaTarjetas.Controllers
{
    public class MovimientosCuentaController : Controller
    {
        private readonly IConsultaClienteService _consultaService;

        public MovimientosCuentaController(IConsultaClienteService consultaService)
        {
            _consultaService = consultaService;
        }

        public async Task<IActionResult> Index(string numeroCuenta = null)
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
            ViewBag.NumeroCuenta = numeroCuenta;

            if (!string.IsNullOrEmpty(numeroCuenta))
            {
                var movimientos = await _consultaService.ObtenerMovimientosCuentaAsync(numeroCuenta, idCliente);
                ViewBag.Movimientos = movimientos;
            }

            return View();
        }
    }
}