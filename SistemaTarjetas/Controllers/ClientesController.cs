using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Clientes()
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }
    }
}