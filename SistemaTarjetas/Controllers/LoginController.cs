using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Models;
using WSAutenticador;  // ← Este es el namespace de tu WS Autenticador
using System.Threading.Tasks;

namespace SistemaTarjetas.Controllers
{
    public class LoginController : Controller
    {
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

            try
            {
                // Crear cliente del WS Autenticador
                var client = new ServicioAutenticacionClient();

                // Preparar credenciales (encriptadas)
                var credenciales = new Credenciales
                {
                    UsuarioEncriptado = Encriptar(model.Usuario),
                    ContrasenaEncriptada = Encriptar(model.Contra)
                };

                // Llamar al WS
                var resultado = await client.ValidarLoginAsync(credenciales);

                // Cerrar cliente
                await client.CloseAsync();

                if (resultado.Resultado)
                {
                    // Guardar sesión
                    HttpContext.Session.SetString("Usuario", model.Usuario);
                    HttpContext.Session.SetInt32("TipoUsuario", resultado.TipoUsuario);

                    return RedirectToAction("Clientes", "Clientes");
                }
                else
                {
                    ViewBag.Error = resultado.Mensaje;
                    return View(model);
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = $"Error al conectar con el servicio de autenticación: {ex.Message}";
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

        // Método de encriptación (debe coincidir con el WS)
        private string Encriptar(string texto)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(texto));
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}