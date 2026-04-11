namespace SistemaTarjetas.Models
{
    public class Tarjeta
    {
        public string Id { get; set; } = string.Empty;
        public string NumeroTarjeta { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string CuentaAsociada { get; set; } = string.Empty;
        public string IdCliente { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;           // ← Agregar
        public string FechaVencimiento { get; set; } = string.Empty; // ← Agregar
    }
}