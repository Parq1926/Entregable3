using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Models;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SistemaTarjetas.Controllers
{
    public class SimuladorController : Controller
    {
        // GET: Simulador
        public IActionResult Index()
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            ViewBag.Usuario = usuario ?? "Invitado";
            return View(new CompraViewModel());
        }

        // POST: Simulador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CompraViewModel model)
        {
            Console.WriteLine("========== POST RECIBIDO EN SIMULADOR ==========");
            Console.WriteLine($"Hora: {DateTime.Now}");

            if (model == null)
            {
                Console.WriteLine("ERROR: model es NULL");
                ViewBag.Mensaje = "Error en los datos";
                ViewBag.Tipo = "error";
                return View(new CompraViewModel());
            }

            Console.WriteLine($"Tarjeta: {model.NumeroTarjeta}");
            Console.WriteLine($"Monto: {model.Monto}");
            Console.WriteLine($"Comercio: {model.Comercio}");
            Console.WriteLine($"CVV: {model.CVV}");
            Console.WriteLine($"FechaVencimiento: {model.FechaVencimiento}");
            Console.WriteLine($"PIN: {model.PIN ?? "null"}");

            var usuario = HttpContext.Session.GetString("Usuario");
            ViewBag.Usuario = usuario ?? "Invitado";

            // Validar campos obligatorios
            if (string.IsNullOrEmpty(model.NumeroTarjeta))
            {
                ViewBag.Mensaje = "❌ Ingrese el número de tarjeta";
                ViewBag.Tipo = "error";
                return View(model);
            }

            if (model.Monto <= 0)
            {
                ViewBag.Mensaje = "❌ Ingrese un monto válido";
                ViewBag.Tipo = "error";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Comercio))
            {
                ViewBag.Mensaje = "❌ Ingrese el comercio";
                ViewBag.Tipo = "error";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.CVV) || (model.CVV.Length != 3 && model.CVV.Length != 4))
            {
                ViewBag.Mensaje = "❌ CVV inválido. Debe tener 3 o 4 dígitos";
                ViewBag.Tipo = "error";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.FechaVencimiento) || !model.FechaVencimiento.Contains("/"))
            {
                ViewBag.Mensaje = "❌ Fecha de vencimiento inválida. Use formato MM/YYYY";
                ViewBag.Tipo = "error";
                return View(model);
            }

            // Validar nombre del cliente
            if (string.IsNullOrEmpty(model.NombreCliente))
            {
                ViewBag.Mensaje = "❌ Ingrese el nombre del cliente";
                ViewBag.Tipo = "error";
                return View(model);
            }

            // Validar PIN para montos mayores a 50000
            if (model.Monto > 50000 && string.IsNullOrEmpty(model.PIN))
            {
                ViewBag.Mensaje = "❌ PIN requerido para montos mayores a ₡50,000";
                ViewBag.Tipo = "error";
                return View(model);
            }

            // Limpiar número de tarjeta (quitar espacios)
            model.NumeroTarjeta = model.NumeroTarjeta.Replace(" ", "");

            // Enviar a Python
            var request = new
            {
                accion = "compra",
                NumeroTarjeta = model.NumeroTarjeta,
                Monto = model.Monto,
                Comercio = model.Comercio,
                CVV = model.CVV,
                FechaVencimiento = model.FechaVencimiento,
                PIN = model.PIN ?? "",
                NombreCliente = model.NombreCliente
            };

            var jsonRequest = JsonSerializer.Serialize(request);
            Console.WriteLine($"Enviando a Python: {jsonRequest}");

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5060);
                Console.WriteLine("Conectado a Python");

                using var stream = client.GetStream();
                var data = Encoding.UTF8.GetBytes(jsonRequest + "\n");
                await stream.WriteAsync(data);
                Console.WriteLine("Datos enviados");

                var buffer = new byte[4096];
                var bytesRead = await stream.ReadAsync(buffer);
                var jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Respuesta Python: {jsonResponse}");

                var resultado = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                if (resultado.GetProperty("ok").GetBoolean())
                {
                    ViewBag.Mensaje = "✅ ¡SU COMPRA SE HA REALIZADO DE MANERA EXITOSA!";
                    ViewBag.Tipo = "success";
                    ModelState.Clear();
                    model = new CompraViewModel();
                }
                else
                {
                    var mensaje = resultado.GetProperty("mensaje").GetString();
                    ViewBag.Mensaje = $"❌ {mensaje}";
                    ViewBag.Tipo = "error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.Mensaje = $"❌ Error de conexión: {ex.Message}";
                ViewBag.Tipo = "error";
            }

            return View(model);
        }
    }
}