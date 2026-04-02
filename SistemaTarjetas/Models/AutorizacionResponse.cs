//Para respuesta del WS

namespace SistemaTarjetas.Models
{
    public class AutorizacionResponse
    {
        public bool Resultado { get; set; }
        public string Mensaje { get; set; }
        public decimal? Saldo { get; set; }
    }
}
