using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SistemaTarjetas.Controllers
{
    public class MovimientosTarjetaCreditoController : Controller
    {
        public async Task<IActionResult> Index(string numeroTarjeta = null)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            var identificacion = HttpContext.Session.GetString("Identificacion");

            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            // Obtener tarjetas de crédito del cliente desde Python
            var tarjetas = await ObtenerTarjetasCreditoCliente(identificacion);
            ViewBag.Tarjetas = tarjetas;
            ViewBag.NumeroTarjeta = numeroTarjeta;
            ViewBag.Usuario = usuario;

            if (!string.IsNullOrEmpty(numeroTarjeta))
            {
                var movimientos = await ObtenerMovimientosTarjetaCredito(numeroTarjeta);
                ViewBag.Movimientos = movimientos;
            }

            return View();
        }

        private async Task<List<dynamic>> ObtenerTarjetasCreditoCliente(string identificacion)
        {
            var request = new { accion = "consulta_cuentas", Identificacion = identificacion };
            var jsonRequest = JsonSerializer.Serialize(request);

            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 5060);

            using var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(jsonRequest + "\n");
            await stream.WriteAsync(data);

            var buffer = new byte[4096];
            var bytesRead = await stream.ReadAsync(buffer);
            var jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var resultado = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            var tarjetas = new List<dynamic>();
            if (resultado.GetProperty("ok").GetBoolean())
            {
                var cuentasArray = resultado.GetProperty("cuentas");
                foreach (var item in cuentasArray.EnumerateArray())
                {
                    var tipo = item.GetProperty("tipo").GetString();
                    if (tipo == "Credito")
                    {
                        tarjetas.Add(new
                        {
                            NumeroTarjeta = item.GetProperty("numero_tarjeta").GetString(),
                            Saldo = item.GetProperty("saldo").GetDecimal(),
                            Tipo = tipo
                        });
                    }
                }
            }
            return tarjetas;
        }

        private async Task<List<dynamic>> ObtenerMovimientosTarjetaCredito(string numeroTarjeta)
        {
            var request = new { accion = "movimientos_tarjeta_credito", NumeroTarjeta = numeroTarjeta };
            var jsonRequest = JsonSerializer.Serialize(request);

            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 5060);

            using var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(jsonRequest + "\n");
            await stream.WriteAsync(data);

            var buffer = new byte[4096];
            var bytesRead = await stream.ReadAsync(buffer);
            var jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var resultado = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            var movimientos = new List<dynamic>();
            if (resultado.GetProperty("ok").GetBoolean())
            {
                var movArray = resultado.GetProperty("movimientos");
                foreach (var item in movArray.EnumerateArray())
                {
                    movimientos.Add(new
                    {
                        Fecha = item.GetProperty("fecha").GetString(),
                        Codigo = item.GetProperty("codigo").GetString(),
                        Comercio = item.GetProperty("comercio").GetString(),
                        Monto = item.GetProperty("monto").GetDecimal()
                    });
                }
            }
            return movimientos;
        }
    }
}