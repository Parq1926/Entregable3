using System.ComponentModel.DataAnnotations;

namespace SistemaTarjetas.Models
{
    public class CompraViewModel
    {
        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [Display(Name = "Nombre del Cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de tarjeta es obligatorio")]
        [Display(Name = "Número de Tarjeta")]
        public string NumeroTarjeta { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El comercio es obligatorio")]
        [Display(Name = "Comercio")]
        public string Comercio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CVV es obligatorio")]
        [Display(Name = "CVV")]
        public string CVV { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        [Display(Name = "Fecha Vencimiento")]
        public string FechaVencimiento { get; set; } = string.Empty;

        [Display(Name = "PIN")]
        public string PIN { get; set; } = string.Empty;
    }
}