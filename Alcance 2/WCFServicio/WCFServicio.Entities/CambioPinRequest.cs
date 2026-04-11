using System.Runtime.Serialization;

namespace WCFServicio.Entities
{
    [DataContract]
    public class CambioPinRequest
    {
        [DataMember]
        public string NumeroTarjeta { get; set; }
        [DataMember]
        public string Cvv { get; set; }
        [DataMember]
        public string PinActual { get; set; }
        [DataMember]
        public string PinNuevo { get; set; }
        [DataMember]
        public string FechaVencimiento { get; set; }
        [DataMember]
        public string IdComercioOCajero { get; set; }
    }
}