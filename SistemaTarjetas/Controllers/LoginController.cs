using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Models;
using SistemaTarjetas.Services;
using SistemaTarjetas.Helpers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
                // Guardar usuario
                HttpContext.Session.SetString("Usuario", model.Usuario);
                HttpContext.Session.SetInt32("TipoUsuario", response.TipoUsuario);

                // Temporal: usa el nombre de usuario como cédula
                string identificacion = model.Usuario;
                HttpContext.Session.SetString("Identificacion", identificacion);

                // ================================================
                // ENVIAR CORREO DE INICIO DE SESIÓN
                // ================================================
                await EnviarCorreoLogin(model.Usuario);

                if (response.TipoUsuario == 1)
                {
                    return RedirectToAction("Clientes", "ClientesADM");
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

        // Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ================================================
        // ENVIAR CORREO DE LOGIN
        // ================================================
        private async Task EnviarCorreoLogin(string usuario)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5060);

                var request = new
                {
                    accion = "enviar_correo_login",
                    Usuario = usuario
                };

                var jsonRequest = JsonSerializer.Serialize(request);
                var data = Encoding.UTF8.GetBytes(jsonRequest + "\n");
                await client.GetStream().WriteAsync(data);

                var buffer = new byte[4096];
                var bytesRead = await client.GetStream().ReadAsync(buffer);
                var jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var resultado = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                if (resultado.GetProperty("ok").GetBoolean())
                {
                    Console.WriteLine($"✅ Correo de login enviado para: {usuario}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando correo de login: {ex.Message}");
            }
        }
    }
}