using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SistemaTarjetas.Security;

namespace SistemaTarjetas.Controllers
{
    public class TarjetaADMController : Controller
    {
        private readonly string _connectionString;

        public TarjetaADMController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private bool EsAdmin()
        {
            return HttpContext.Session.GetInt32("TipoUsuario") == 1;
        }

        //LISTAR
        public IActionResult Tarjetas(string filtro)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            var lista = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
    SELECT t.Numero_Tarjeta,
           t.Numero_Identificacion,
           RTRIM(c.Nombre + ' ' + c.Apellido + ' ' + ISNULL(c.Apellido2, '')) AS NombreCompleto,
           t.Estado,
           tt.Tipo_Tarjeta
    FROM Tarjeta t
    INNER JOIN Cliente c 
        ON t.Numero_Identificacion = c.Numero_Identificacion
    LEFT JOIN Tipo_Tarjeta tt 
        ON t.IDTipo_Tarjeta = tt.IDTipo_Tarjeta
    WHERE (@filtro IS NULL OR t.Numero_Identificacion = @filtro)", conn);



                cmd.Parameters.AddWithValue("@filtro", (object?)filtro ?? DBNull.Value);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new
                    {
                        Numero = reader["Numero_Tarjeta"].ToString(),
                        Cliente = reader["Numero_Identificacion"].ToString(),
                        Nombre = reader["NombreCompleto"].ToString(),
                        Estado = reader["Estado"].ToString(),
                        Tipo = reader["Tipo_Tarjeta"].ToString()
                    });
                }
            }

            ViewBag.Tarjetas = lista;
            return View();
        }

        //AJAX
        [HttpGet]
        public JsonResult ObtenerCliente(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(null);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    SELECT Nombre + ' ' + Apellido
                    FROM Cliente 
                    WHERE Numero_Identificacion = @id", conn);

                cmd.Parameters.AddWithValue("@id", id);

                var nombre = cmd.ExecuteScalar();

                return Json(nombre?.ToString());
            }
        }

        [HttpGet]
        public JsonResult ObtenerCuentas(string id)
        {
            var cuentas = new List<string>();

            if (string.IsNullOrEmpty(id)) return Json(cuentas);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    SELECT Numero_Cuenta 
                    FROM Cuenta 
                    WHERE Numero_Identificacion = @id", conn);

                cmd.Parameters.AddWithValue("@id", id);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cuentas.Add(reader["Numero_Cuenta"].ToString());
                }
            }

            return Json(cuentas);
        }

        //CREAR
        public IActionResult Crear()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            var tipos = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT IDTipo_Tarjeta, Tipo_Tarjeta FROM Tipo_Tarjeta", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tipos.Add(new
                    {
                        Id = reader["IDTipo_Tarjeta"],
                        Nombre = reader["Tipo_Tarjeta"].ToString()
                    });
                }
            }

            ViewBag.Tipos = tipos;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string Identificacion, decimal Monto, int Tipo, string Cuenta)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Valida que hay un cliente.
                var cmdCliente = new SqlCommand(@"
                   SELECT COUNT(*) 
                   FROM Cliente 
                   WHERE Numero_Identificacion = @id", conn);

                cmdCliente.Parameters.AddWithValue("@id", Identificacion);

                int existe = (int)cmdCliente.ExecuteScalar();

                if (existe == 0)
                {
                    TempData["Error"] = "❌ El cliente no existe";
                    return RedirectToAction("Crear");
                }

                var tipoCmd = new SqlCommand(
                    "SELECT Tipo_Tarjeta FROM Tipo_Tarjeta WHERE IDTipo_Tarjeta = @id", conn);
                tipoCmd.Parameters.AddWithValue("@id", Tipo);

                if (Tipo == 0)
                {
                    TempData["Error"] = "❌ Debe seleccionar el tipo de tarjeta";
                    return RedirectToAction("Crear");
                }

                string tipoNombre = tipoCmd.ExecuteScalar()?.ToString();

                //DEBITO O CRÉDITO.
                if (tipoNombre == "Debito")
                {
                    if (string.IsNullOrEmpty(Cuenta))
                    {
                        TempData["Error"] = "❌ Debe seleccionar una cuenta";
                        return RedirectToAction("Crear");
                    }

                    Monto = 0; 
                }
                else if (tipoNombre == "Credito")
                {
                    if (Monto <= 0)
                    {
                        TempData["Error"] = "❌ Debe ingresar un límite válido";
                        return RedirectToAction("Crear");
                    }
                }

                //VALIDACIONES IMPORTANTES
                string numero = tipoNombre == "Debito"
                    ? "911111" + new Random().Next(100000, 999999)
                    : "911112" + new Random().Next(100000, 999999);

                string cvv = new Random().Next(100, 999).ToString();
                string pin = new Random().Next(1000, 9999).ToString();
                string fecha = DateTime.Now.AddYears(4).ToString("yyyy-MM-dd");

                var cmd = new SqlCommand(@"
                    INSERT INTO Tarjeta
                    (Numero_Tarjeta, Numero_Identificacion, Monto_Total,
                     Fecha_Vencimiento, CVV, Pin, IDTipo_Tarjeta, Estado, Numero_Cuenta)
                    VALUES
                    (@num, @cli, @mon, @fec, @cvv, @pin, @tipo, 'Activa', @cta)", conn);

                cmd.Parameters.AddWithValue("@num", numero);
                cmd.Parameters.AddWithValue("@cli", Identificacion);
                cmd.Parameters.AddWithValue("@mon", Monto);
                cmd.Parameters.AddWithValue("@fec", AES.Encrypt(fecha));
                cmd.Parameters.AddWithValue("@cvv", AES.Encrypt(cvv));
                cmd.Parameters.AddWithValue("@pin", AES.Encrypt(pin));
                cmd.Parameters.AddWithValue("@tipo", Tipo);
                cmd.Parameters.AddWithValue("@cta", (object?)Cuenta ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "💳 Tarjeta creada correctamente";
            return RedirectToAction("Tarjetas");
        }

        //INACTIVAR
        public IActionResult Inactivar(string numero)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    UPDATE Tarjeta
                    SET Estado = 'Inactiva'
                    WHERE Numero_Tarjeta = @num
                    AND Estado != 'Inactiva'", conn);

                cmd.Parameters.AddWithValue("@num", numero);
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "🔒 Tarjeta inactivada";
            return RedirectToAction("Tarjetas");
        }
    }
}