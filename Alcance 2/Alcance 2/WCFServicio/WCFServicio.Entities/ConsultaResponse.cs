using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization; //DataContract y DataMember referencia.

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