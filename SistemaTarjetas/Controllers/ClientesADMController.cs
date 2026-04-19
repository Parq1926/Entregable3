using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SistemaTarjetas.Controllers
{
    public class ClientesADMController : Controller
    {
        private readonly string _connectionString;

        public ClientesADMController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        private bool EsAdmin()
        {
            return HttpContext.Session.GetInt32("TipoUsuario") == 1;
        }

        // LISTAR
        public IActionResult Clientes()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Login");

            var lista = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Cliente", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new
                    {
                        Identificacion = reader["Numero_Identificacion"].ToString(),
                        Nombre = reader["Nombre"].ToString(),
                        Apellido1 = reader["Apellido"].ToString(),
                        Apellido2 = reader["Apellido2"]?.ToString()
                    });
                }
            }

            ViewBag.Clientes = lista;
            return View();
        }

        // CREAR (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string Identificacion, string Nombre, string Apellido1, string Apellido2)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var check = new SqlCommand(
                    "SELECT COUNT(*) FROM Cliente WHERE Numero_Identificacion=@id", conn);
                check.Parameters.AddWithValue("@id", Identificacion);

                if ((int)check.ExecuteScalar() > 0)
                {
                    TempData["Error"] = "❌ Ya existe ese cliente";
                    return RedirectToAction("Crear");
                }

                var cmd = new SqlCommand(@"
                    INSERT INTO Cliente
                    (Numero_Identificacion, Nombre, Apellido, Apellido2)
                    VALUES
                    (@id, @nom, @ape1, @ape2)", conn);

                cmd.Parameters.AddWithValue("@id", Identificacion);
                cmd.Parameters.AddWithValue("@nom", Nombre);
                cmd.Parameters.AddWithValue("@ape1", Apellido1);
                cmd.Parameters.AddWithValue("@ape2", (object)Apellido2 ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "✔ Cliente creado";
            return RedirectToAction("Clientes");
        }

        // EDITAR (GET)
        public IActionResult Editar(string identificacion)
        {
            dynamic cliente = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(
                    "SELECT * FROM Cliente WHERE Numero_Identificacion=@id", conn);
                cmd.Parameters.AddWithValue("@id", identificacion);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cliente = new
                    {
                        Identificacion = reader["Numero_Identificacion"].ToString(),
                        Nombre = reader["Nombre"].ToString(),
                        Apellido1 = reader["Apellido"].ToString(),
                        Apellido2 = reader["Apellido2"]?.ToString()
                    };
                }
            }

            ViewBag.Cliente = cliente;
            return View();
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(string IdentificacionOriginal, string Identificacion, string Nombre, string Apellido1, string Apellido2)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Validar duplicados.
                if (IdentificacionOriginal != Identificacion)
                {
                    var check = new SqlCommand(
                        "SELECT COUNT(*) FROM Cliente WHERE Numero_Identificacion=@id", conn);
                    check.Parameters.AddWithValue("@id", Identificacion);

                    if ((int)check.ExecuteScalar() > 0)
                    {
                        TempData["Error"] = "❌ Ya existe esa cédula";
                        return RedirectToAction("Editar", new { identificacion = IdentificacionOriginal });
                    }
                }

                var cmd = new SqlCommand(@"
                    UPDATE Cliente
                    SET Numero_Identificacion=@newid,
                        Nombre=@nom,
                        Apellido=@ape1,
                        Apellido2=@ape2
                    WHERE Numero_Identificacion=@oldid", conn);

                cmd.Parameters.AddWithValue("@newid", Identificacion);
                cmd.Parameters.AddWithValue("@oldid", IdentificacionOriginal);
                cmd.Parameters.AddWithValue("@nom", Nombre);
                cmd.Parameters.AddWithValue("@ape1", Apellido1);
                cmd.Parameters.AddWithValue("@ape2", (object)Apellido2 ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "✔ Cliente actualizado";
            return RedirectToAction("Clientes");
        }

        // ELIMINAR
        public IActionResult Eliminar(string identificacion)
        {
            ViewBag.Id = identificacion;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(string Identificacion)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var check = new SqlCommand(
                    "SELECT COUNT(*) FROM Tarjeta WHERE Numero_Identificacion=@id", conn);
                check.Parameters.AddWithValue("@id", Identificacion);

                if ((int)check.ExecuteScalar() > 0)
                {
                    TempData["Error"] = "❌ No es posible eliminar el usuario, tiene datos asociados";
                    return RedirectToAction("Clientes");
                }

                var cmd = new SqlCommand(
                    "DELETE FROM Cliente WHERE Numero_Identificacion=@id", conn);
                cmd.Parameters.AddWithValue("@id", Identificacion);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "🗑️ Cliente eliminado";
            return RedirectToAction("Clientes");
        }
    }
}