using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Models;
using SistemaTarjetas.Services;
using SistemaTarjetas.Helpers;
using System.Threading.Tasks;

namespace SistemaTarjetas.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAutenticadorService _authService;

        public LoginController(IAutenticadorService authService)
        {
            _authService = authService;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string usuarioEncriptado = Encriptacion.Encriptar(model.Usuario);
            string contrasenaEncriptada = Encriptacion.Encriptar(model.Contra);

            var response = await _authService.Autenticar(usuarioEncriptado, contrasenaEncriptada, 0);

            if (response.Resultado)
            {
                // Guardar sesión
                HttpContext.Session.SetString("Usuario", model.Usuario);
                HttpContext.Session.SetInt32("TipoUsuario", response.TipoUsuario);

                // Guardar la identificación (cédula) desde el WS
                // Debes obtenerla del WS Autenticador o de otro WS
                HttpContext.Session.SetString("Identificacion", response.Identificacion ?? model.Usuario);

                if (response.TipoUsuario == 1)
                {
                    return RedirectToAction("Clientes", "Clientes");
                }
                else if (response.TipoUsuario == 2)
                {
                    return RedirectToAction("Dashboard", "CuentasTarjetas");
                }
                else
                {
                    ViewBag.Error = "Tipo de usuario no válido";
                    return View(model);
                }
            }
            else
            {
                ViewBag.Error = response.Mensaje;
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}