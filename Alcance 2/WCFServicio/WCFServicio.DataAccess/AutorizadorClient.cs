using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets; //Sockets.
using System.Text;
using System.Threading.Tasks;

namespace WCFServicio.DataAccess
{
    public class AutorizadorClient
    {
        private readonly string host = "127.0.0.1";
        private readonly int port = 5060;

        public string Enviar(string json)
        {
            try
            {
                using (TcpClient client = new TcpClient(host, port))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    writer.WriteLine(json);
                    return reader.ReadLine();
                }
            }
            catch (Exception ex)
            {
                return "{\"ok\":false,\"mensaje\":\"Error socket autorizador: " + Escapar(ex.Message) + "\"}";
            }
        }

        private string Escapar(string texto)
        {
            return texto.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}