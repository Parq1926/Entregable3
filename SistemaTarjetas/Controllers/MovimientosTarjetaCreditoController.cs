using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class MovimientosTarjetaCreditoController : Controller
    {
        public IActionResult Index(string numeroTarjeta = null)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Usuario = usuario;
            ViewBag.NumeroTarjeta = numeroTarjeta;
            return View("~/Views/MovimientosTarjetaCredito/Index.cshtml");
        }
    }
}