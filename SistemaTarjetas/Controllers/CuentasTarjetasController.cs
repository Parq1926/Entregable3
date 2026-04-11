using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Services;
using System.Threading.Tasks;

namespace SistemaTarjetas.Controllers
{
    public class CuentasTarjetasController : Controller
    {
        private readonly IConsultaClienteService _consultaService;

        public CuentasTarjetasController(IConsultaClienteService consultaService)
        {
            _consultaService = consultaService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            // Obtener la identificación (cédula) de la sesión
            var identificacion = HttpContext.Session.GetString("Identificacion");
            if (string.IsNullOrEmpty(identificacion))
            {
                identificacion = "123456789"; // Temporal para pruebas
            }

            // Obtener datos usando la cédula
            var cuentas = await _consultaService.ObtenerCuentasClienteAsync(identificacion);
            var tarjetas = await _consultaService.ObtenerTarjetasClienteAsync(identificacion);

            ViewBag.Cuentas = cuentas;
            ViewBag.Tarjetas = tarjetas;
            ViewBag.Usuario = usuario;

            return View();
        }
    }
}