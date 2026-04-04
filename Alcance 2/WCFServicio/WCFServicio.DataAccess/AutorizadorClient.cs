using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

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
                Debug.WriteLine($"=== AutorizadorClient ===");
                Debug.WriteLine($"Conectando a Python en {host}:{port}");
                Debug.WriteLine($"JSON a enviar: {json}");

                using (TcpClient client = new TcpClient(host, port))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    Debug.WriteLine("Conexión establecida, enviando datos...");

                    writer.WriteLine(json);
                    Debug.WriteLine("Datos enviados, esperando respuesta...");

                    string respuesta = reader.ReadLine();
                    Debug.WriteLine($"Respuesta recibida: {respuesta}");

                    return respuesta;
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"ERROR SOCKET: {ex.Message}");
                return $"{{\"ok\":false,\"mensaje\":\"No se pudo conectar a Python en {host}:{port}. ¿Python está corriendo? Error: {ex.Message}\"}}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR GENERAL: {ex.Message}");
                return $"{{\"ok\":false,\"mensaje\":\"Error socket autorizador: {Escapar(ex.Message)}\"}}";
            }
        }

        private string Escapar(string texto)
        {
            return texto.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}