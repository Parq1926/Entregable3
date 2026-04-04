namespace SistemaTarjetas.Models
{
    public class AutenticacionResponse
    {
        public bool Resultado { get; set; }
        public string Mensaje { get; set; }
        public int TipoUsuario { get; set; }  // ← Agregar campo
    }
}