namespace SistemaTarjetas.Models
{
    public class Cuenta
    {
        public string Id { get; set; }
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public string IdCliente { get; set; }
    }
}