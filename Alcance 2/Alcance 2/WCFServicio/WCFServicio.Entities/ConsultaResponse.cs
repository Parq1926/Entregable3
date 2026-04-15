using System.Runtime.Serialization;

namespace WCFServicio.Entities
{
    [DataContract]
    public class ConsultaResponse
    {
        [DataMember]
        public bool Resultado { get; set; }
        [DataMember]
        public string Mensaje { get; set; }
        [DataMember]
        public string Saldo { get; set; }
    }
}