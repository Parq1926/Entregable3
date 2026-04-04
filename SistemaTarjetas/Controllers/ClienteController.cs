using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class ClienteController : Controller
    {
        // Redirige a la pantalla USR2
        public IActionResult Dashboard()
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Dashboard", "CuentasTarjetas");
        }

        // USR3: Movimientos de cuentas
        public IActionResult MovimientosCuenta(string numeroCuenta = null)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Usuario = usuario;
            ViewBag.NumeroCuenta = numeroCuenta;
            return View();
        }

        // USR4: Movimientos de tarjetas de crédito
        public IActionResult MovimientosTarjetaCredito(string numeroTarjeta = null)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Usuario = usuario;
            ViewBag.NumeroTarjeta = numeroTarjeta;
            return View();
        }
    }
}