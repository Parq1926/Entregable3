namespace SistemaTarjetas.Models
{
    public class Tarjeta
    {
        public string Id { get; set; }
        public string NumeroTarjeta { get; set; }
        public string Tipo { get; set; } // "Debito" o "Credito"
        public string CuentaAsociada { get; set; }
        public string IdCliente { get; set; }
    }
}