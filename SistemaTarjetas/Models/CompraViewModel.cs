using System.ComponentModel.DataAnnotations;

namespace SistemaTarjetas.Models
{
    public class CompraViewModel
    {
        [Required(ErrorMessage = "El monto es requerido")]
        [Display(Name = "Monto de la transacción")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de tarjeta es requerido")]
        [Display(Name = "Número de tarjeta")]
        public string NumeroTarjeta { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de vencimiento es requerida")]
        [Display(Name = "Fecha de vencimiento")]
        public string FechaVencimiento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CVV es requerido")]
        [Display(Name = "CVV")]
        public string CVV { get; set; } = string.Empty;

        [Required(ErrorMessage = "El comercio es requerido")]
        [Display(Name = "Comercio / Cajero")]
        public string Comercio { get; set; } = string.Empty;

        [Display(Name = "PIN")]
        public string PIN { get; set; } = string.Empty;
    }
}