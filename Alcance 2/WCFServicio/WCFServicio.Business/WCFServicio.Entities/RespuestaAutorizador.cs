using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFServicio.Entities
{
    public class RespuestaAutorizador
    {
        public bool ok { get; set; }
        public string mensaje { get; set; }
        public decimal saldo { get; set; }
    }
}
