using System.Runtime.Serialization;

namespace WCFServicio.Entities
{
    [DataContract]
    public class RetiroRequest
    {
        [DataMember]
        public string NumeroTarjeta { get; set; }
        [DataMember]
        public string Cvv { get; set; }
        [DataMember]
        public string Pin { get; set; }
        [DataMember]
        public string FechaVencimiento { get; set; }
        [DataMember]
        public string NombreCliente { get; set; }
        [DataMember]
        public string IdComercioOCajero { get; set; }
        [DataMember]
        public decimal Monto { get; set; }
    }
}