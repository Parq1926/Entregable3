using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; //Conexión MySQL.

namespace WCFServicio.DataAccess
{
    public class ConexionMySQL
    {
        private static string connectionString =
            "Server=localhost;Database=cajeroautomatico;Uid=root;Pwd=70780541Yen;";

        public static MySqlConnection ObtenerConexion()
        {
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();
            return conexion;
        }
    }
}
