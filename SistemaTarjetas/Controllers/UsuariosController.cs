using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class UsuariosController : Controller
    {
        public IActionResult Usuarios()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Login", "Login");

            return View();
        }
    }
}