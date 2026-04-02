namespace SistemaTarjetas.Models
{
    public class AutorizacionRequest
    {
        // Para retiros
        public string NumeroTarjeta { get; set; }
        public string CVV { get; set; }
        public string PIN { get; set; }
        public string FechaVencimiento { get; set; }
        public string NombreCliente { get; set; }
        public string IdentificacionComercio { get; set; }
        public decimal Monto { get; set; }
        public string TipoTransaccion { get; set; }

        // Para cambio de PIN
        public string PINActual { get; set; }
        public string PINNuevo { get; set; }
    }
}