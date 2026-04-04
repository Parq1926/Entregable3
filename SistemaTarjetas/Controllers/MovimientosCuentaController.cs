using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class MovimientosCuentaController : Controller
    {
        public IActionResult Index(string numeroCuenta = null)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Usuario = usuario;
            ViewBag.NumeroCuenta = numeroCuenta;

            return View("~/Views/MovimientosCuenta/Index.cshtml");
        }
    }
}