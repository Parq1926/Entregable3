using System.Runtime.Serialization;

namespace WCFServicio.Entities
{
    [DataContract]
    public class CambioPinResponse
    {
        [DataMember]
        public bool Resultado { get; set; }
        [DataMember]
        public string Mensaje { get; set; }
    }
}