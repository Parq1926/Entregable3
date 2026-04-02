using System.ComponentModel.DataAnnotations;

namespace SistemaTarjetas.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        public string Usuario { get; set; } = string.Empty;  // ← Agrega = string.Empty

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Contra { get; set; } = string.Empty;   // ← Agrega = string.Empty
    }
}