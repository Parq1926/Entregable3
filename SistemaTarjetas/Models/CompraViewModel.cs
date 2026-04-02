using System.ComponentModel.DataAnnotations;

namespace SistemaTarjetas.Models
{
    public class CompraViewModel
    {
        // Monto de la transacción
        [Display(Name = "Monto de la transacción")]
        public decimal Monto { get; set; }

        // Nombre del cliente
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; }

        // Número de tarjeta
        [Display(Name = "Número de tarjeta")]
        public string NumeroTarjeta { get; set; }

        // Fecha de vencimiento
        [Display(Name = "Fecha de vencimiento")]
        public string FechaVencimiento { get; set; }

        // CVV
        [Display(Name = "CVV")]
        public string CVV { get; set; }

        // Comercio / Cajero
        [Display(Name = "Comercio / Cajero")]
        public string Comercio { get; set; }

        // PIN (opcional, solo si monto > 50,000)
        [Display(Name = "PIN")]
        public string PIN { get; set; }
    }
}