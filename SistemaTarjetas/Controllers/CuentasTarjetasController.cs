using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class CuentasTarjetasController : Controller
    {
        public IActionResult Dashboard()
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Usuario = usuario;
            return View();
        }
    }
}