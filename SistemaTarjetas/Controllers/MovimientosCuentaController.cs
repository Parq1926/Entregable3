using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SistemaTarjetas.Controllers
{
    public class MovimientosCuentaController : Controller
    {
        public async Task<IActionResult> Index(string numeroCuenta = null)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            var identificacion = HttpContext.Session.GetString("Identificacion");

            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Login");
            }

            // Obtener cuentas del cliente
            var cuentas = await ObtenerCuentasCliente(identificacion);
            ViewBag.Cuentas = cuentas;
            ViewBag.NumeroCuenta = numeroCuenta;
            ViewBag.Usuario = usuario;

            if (!string.IsNullOrEmpty(numeroCuenta))
            {
                var movimientos = await ObtenerMovimientosCuenta(numeroCuenta);
                ViewBag.Movimientos = movimientos;
            }

            return View();
        }

        private async Task<List<dynamic>> ObtenerCuentasCliente(string identificacion)
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

            var cuentas = new List<dynamic>();
            if (resultado.GetProperty("ok").GetBoolean())
            {
                var cuentasArray = resultado.GetProperty("cuentas");
                foreach (var item in cuentasArray.EnumerateArray())
                {
                    cuentas.Add(new
                    {
                        NumeroTarjeta = item.GetProperty("numero_tarjeta").GetString(),
                        Saldo = item.GetProperty("saldo").GetDecimal(),
                        Tipo = item.GetProperty("tipo").GetString()
                    });
                }
            }
            return cuentas;
        }

        private async Task<List<dynamic>> ObtenerMovimientosCuenta(string numeroCuenta)
        {
            var request = new { accion = "movimientos_cuenta", NumeroCuenta = numeroCuenta };
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
                        Descripcion = item.GetProperty("descripcion").GetString(),
                        Monto = item.GetProperty("monto").GetDecimal()
                    });
                }
            }
            return movimientos;
        }
    }
}