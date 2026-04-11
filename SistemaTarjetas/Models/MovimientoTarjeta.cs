using System;

namespace SistemaTarjetas.Models
{
    public class MovimientoTarjeta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string CodigoAutorizacion { get; set; } = string.Empty;
        public string Comercio { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string IdTarjeta { get; set; } = string.Empty;
        public bool Exitoso { get; set; } = true;
    }
}