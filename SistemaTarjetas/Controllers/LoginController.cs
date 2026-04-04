using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Models;
using SistemaTarjetas.Services;
using SistemaTarjetas.Helpers;  // Para AES
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

            // Encriptar con AES (mismo método que el WS)
            string usuarioEncriptado = Encriptacion.Encriptar(model.Usuario);
            string contrasenaEncriptada = Encriptacion.Encriptar(model.Contra);

            // Validar contra WS Autenticador (tipo 0 = cualquier tipo, o usa 1/2 según necesites)
            var response = await _authService.Autenticar(usuarioEncriptado, contrasenaEncriptada, 0);

            if (response.Resultado)
            {
                // Guardar sesión
                HttpContext.Session.SetString("Usuario", model.Usuario);
                HttpContext.Session.SetInt32("TipoUsuario", response.TipoUsuario);

                // Redirigir según el tipo de usuario
                if (response.TipoUsuario == 1)  // Administrador
                {
                    return RedirectToAction("Clientes", "Clientes");
                }
                else if (response.TipoUsuario == 2)  // Cliente
                {
                    return RedirectToAction("Dashboard", "Cliente");
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

        // Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}