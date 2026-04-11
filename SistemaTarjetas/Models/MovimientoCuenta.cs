using System;

namespace SistemaTarjetas.Models
{
    public class MovimientoCuenta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string IdCuenta { get; set; } = string.Empty;
    }
}