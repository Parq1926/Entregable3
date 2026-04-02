using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class CuentasController : Controller
    {
        public IActionResult Cuentas()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Login", "Login");

            return View();
        }
    }
}