using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SistemaTarjetas.Controllers
{
    public class CuentaADMController : Controller
    {
        private readonly string _connectionString;

        public CuentaADMController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private bool EsAdmin()
        {
            return HttpContext.Session.GetInt32("TipoUsuario") == 1;
        }

        // LISTAR
        public IActionResult Cuentas()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            var lista = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    SELECT c.Numero_Cuenta, c.Saldo,
                           cl.Numero_Identificacion,
                           cl.Nombre + ' ' + cl.Apellido AS NombreCompleto
                    FROM Cuenta c
                    INNER JOIN Cliente cl 
                        ON c.Numero_Identificacion = cl.Numero_Identificacion", conn);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new
                    {
                        Numero = reader["Numero_Cuenta"].ToString(),
                        Saldo = reader["Saldo"].ToString(),
                        Identificacion = reader["Numero_Identificacion"].ToString(),
                        Nombre = reader["NombreCompleto"].ToString()
                    });
                }
            }

            ViewBag.Cuentas = lista;
            return View();
        }

        // CREAR (GET)
        public IActionResult Crear()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string Identificacion, decimal Monto)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            if (Monto < 0)
            {
                TempData["Error"] = "❌ El monto no puede ser negativo";
                return RedirectToAction("Crear");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmdCliente = new SqlCommand(
                    "SELECT Nombre + ' ' + Apellido FROM Cliente WHERE Numero_Identificacion = @id", conn);
                cmdCliente.Parameters.AddWithValue("@id", Identificacion);

                var nombre = cmdCliente.ExecuteScalar();

                if (nombre == null)
                {
                    TempData["Error"] = "❌ Cliente no existe";
                    return RedirectToAction("Crear");
                }

                string numeroCuenta = "CTA" + new Random().Next(100000, 999999);

                var cmd = new SqlCommand(@"
                    INSERT INTO Cuenta
                    (Numero_Cuenta, Numero_Identificacion, Saldo)
                    VALUES (@num, @cli, @saldo)", conn);

                cmd.Parameters.AddWithValue("@num", numeroCuenta);
                cmd.Parameters.AddWithValue("@cli", Identificacion);
                cmd.Parameters.AddWithValue("@saldo", Monto);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "💰 Cuenta creada correctamente";
            return RedirectToAction("Cuentas");
        }

        // EDITAR (GET)
        public IActionResult Editar(string numero)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            dynamic cuenta = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    SELECT Numero_Cuenta, Numero_Identificacion, Saldo
                    FROM Cuenta
                    WHERE Numero_Cuenta = @num", conn);

                cmd.Parameters.AddWithValue("@num", numero);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cuenta = new
                    {
                        Numero = reader["Numero_Cuenta"].ToString(),
                        Identificacion = reader["Numero_Identificacion"].ToString(),
                        Saldo = reader["Saldo"].ToString()
                    };
                }
            }

            ViewBag.Cuenta = cuenta;
            return View();
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(string Numero, decimal Saldo)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            if (Saldo < 0)
            {
                TempData["Error"] = "❌ El saldo no puede ser negativo";
                return RedirectToAction("Editar", new { numero = Numero });
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    UPDATE Cuenta
                    SET Saldo = @saldo
                    WHERE Numero_Cuenta = @num", conn);

                cmd.Parameters.AddWithValue("@num", Numero);
                cmd.Parameters.AddWithValue("@saldo", Saldo);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "✏️ Cuenta actualizada";
            return RedirectToAction("Cuentas");
        }

        // ELIMINAR (GET)
        public IActionResult Eliminar(string numero)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            ViewBag.Numero = numero;
            return View();
        }

        // ELIMINAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(string Numero)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var check = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Tarjeta 
                    WHERE Numero_Identificacion = (
                        SELECT Numero_Identificacion 
                        FROM Cuenta 
                        WHERE Numero_Cuenta = @num
                    )", conn);

                check.Parameters.AddWithValue("@num", Numero);

                if ((int)check.ExecuteScalar() > 0)
                {
                    TempData["Error"] = "❌ No es posible eliminar la cuenta";
                    return RedirectToAction("Cuentas");
                }

                var cmd = new SqlCommand("DELETE FROM Cuenta WHERE Numero_Cuenta = @num", conn);
                cmd.Parameters.AddWithValue("@num", Numero);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "🗑️ Cuenta eliminada";
            return RedirectToAction("Cuentas");
        }
    }
}