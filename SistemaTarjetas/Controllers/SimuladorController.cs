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
            return View(new CompraViewModel());
        }

        // POST: Simulador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CompraViewModel model)
        {
            Console.WriteLine("=== SIMULADOR - INICIANDO COMPRA ===");
            Console.WriteLine($"Tarjeta: {model.NumeroTarjeta}");
            Console.WriteLine($"Monto: {model.Monto}");
            Console.WriteLine($"Comercio: {model.Comercio}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido");
                return View(model);
            }

            // Enviar a Python
            var request = new
            {
                accion = "compra",
                NumeroTarjeta = model.NumeroTarjeta,
                Monto = model.Monto,
                Comercio = model.Comercio,
                CVV = model.CVV,
                FechaVencimiento = model.FechaVencimiento,
                PIN = model.PIN ?? ""
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
                Console.WriteLine($"Respuesta recibida: {jsonResponse}");

                // Deserializar la respuesta
                var resultado = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                if (resultado.GetProperty("ok").GetBoolean())
                {
                    ViewBag.Mensaje = "Su compra se ha realizado de manera satisfactoria";
                    ViewBag.Tipo = "success";
                    ModelState.Clear();
                    model = new CompraViewModel();
                }
                else
                {
                    var mensaje = resultado.GetProperty("mensaje").GetString();
                    ViewBag.Mensaje = mensaje;
                    ViewBag.Tipo = "error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.Mensaje = $"Error de conexión: {ex.Message}";
                ViewBag.Tipo = "error";
            }

            return View(model);
        }
    }
}