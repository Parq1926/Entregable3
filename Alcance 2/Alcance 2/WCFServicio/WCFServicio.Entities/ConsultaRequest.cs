using System.Runtime.Serialization;

namespace WCFServicio.Entities
{
    [DataContract]
    public class ConsultaRequest
    {
        [DataMember]
        public string NumeroTarjeta { get; set; }
        [DataMember]
        public string Cvv { get; set; }
        [DataMember]
        public string FechaVencimiento { get; set; }
        [DataMember]
        public string IdComercioOCajero { get; set; }
    }
}