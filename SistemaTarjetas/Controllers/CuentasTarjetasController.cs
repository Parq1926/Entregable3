using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

namespace SistemaTarjetas.Controllers
{
    public class CuentasTarjetasController : Controller
    {
        public async Task<IActionResult> Dashboard()
        {
            var identificacion = HttpContext.Session.GetString("Identificacion");
            var usuario = HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(identificacion))
            {
                return RedirectToAction("Login", "Login");
            }

            var request = new { accion = "consulta_cuentas", Identificacion = identificacion };
            var jsonRequest = JsonSerializer.Serialize(request);

            var cuentas = new List<dynamic>();
            var tarjetas = new List<dynamic>();

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5060);

                using var stream = client.GetStream();
                var data = Encoding.UTF8.GetBytes(jsonRequest + "\n");
                await stream.WriteAsync(data);

                var buffer = new byte[4096];
                var bytesRead = await stream.ReadAsync(buffer);
                var jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                var resultado = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                if (resultado.GetProperty("ok").GetBoolean())
                {
                    var cuentasArray = resultado.GetProperty("cuentas");
                    foreach (var item in cuentasArray.EnumerateArray())
                    {
                        var numero = item.GetProperty("numero_tarjeta").GetString();
                        var saldo = item.GetProperty("saldo").GetDecimal();
                        var tipo = item.GetProperty("tipo").GetString();

                        cuentas.Add(new { NumeroTarjeta = numero, Saldo = saldo, Tipo = tipo });
                        tarjetas.Add(new { NumeroTarjeta = numero, Tipo = tipo });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Cuentas = cuentas;
            ViewBag.Tarjetas = tarjetas;
            ViewBag.Usuario = usuario;

            return View();
        }
    }
}