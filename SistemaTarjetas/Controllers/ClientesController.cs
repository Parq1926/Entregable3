using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaTarjetas.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Clientes()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Login", "Login");

            return View();
        }
    }
}