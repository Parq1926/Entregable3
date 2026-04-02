using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class TarjetasController : Controller
    {
        public IActionResult Tarjetas()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Login", "Login");

            return View();
        }
    }
}