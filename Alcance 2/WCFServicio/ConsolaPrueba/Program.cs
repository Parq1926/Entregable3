using System;
using ConsolaPrueba.WCFRef;

namespace ConsolaPruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("      CONSOLA DE PRUEBAS WCF");
                Console.WriteLine("====================================");
                Console.WriteLine("1. Consulta de saldo");
                Console.WriteLine("2. Retiro");
                Console.WriteLine("3. Cambio de PIN");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        ProbarConsulta();
                        break;

                    case "2":
                        ProbarRetiro();
                        break;

                    case "3":
                        ProbarCambioPin();
                        break;

                    case "4":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine("Opción inválida.");
                        Pausa();
                        break;
                }
            }
        }

        static void ProbarConsulta()
        {
            try
            {
                using (ServiceClient client = new ServiceClient())
                {
                    ConsultaRequest request = new ConsultaRequest();

                    Console.Write("Número de tarjeta: ");
                    request.NumeroTarjeta = Console.ReadLine();

                    Console.Write("CVV: ");
                    request.Cvv = Console.ReadLine();

                    Console.Write("Fecha vencimiento: ");
                    request.FechaVencimiento = Console.ReadLine();

                    Console.Write("Id Comercio o Cajero: ");
                    request.IdComercioOCajero = Console.ReadLine();

                    ConsultaResponse response = client.Consulta(request);

                    Console.WriteLine("\n===== RESPUESTA =====");
                    Console.WriteLine("Resultado: " + response.Resultado);
                    Console.WriteLine("Mensaje: " + response.Mensaje);
                    Console.WriteLine("Saldo: " + response.Saldo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consumir Consulta: " + ex.Message);
            }

            Pausa();
        }

        static void ProbarRetiro()
        {
            try
            {
                using (ServiceClient client = new ServiceClient())
                {
                    RetiroRequest request = new RetiroRequest();

                    Console.Write("Número de tarjeta: ");
                    request.NumeroTarjeta = Console.ReadLine();

                    Console.Write("CVV: ");
                    request.Cvv = Console.ReadLine();

                    Console.Write("PIN: ");
                    request.Pin = Console.ReadLine();

                    Console.Write("Fecha vencimiento: ");
                    request.FechaVencimiento = Console.ReadLine();

                    Console.Write("Nombre del cliente: ");
                    request.NombreCliente = Console.ReadLine();

                    Console.Write("Id Comercio o Cajero: ");
                    request.IdComercioOCajero = Console.ReadLine();

                    Console.Write("Monto: ");
                    decimal monto;
                    decimal.TryParse(Console.ReadLine(), out monto);
                    request.Monto = monto;

                    RetiroResponse response = client.Retiro(request);

                    Console.WriteLine("\n===== RESPUESTA =====");
                    Console.WriteLine("Resultado: " + response.Resultado);
                    Console.WriteLine("Mensaje: " + response.Mensaje);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consumir Retiro: " + ex.Message);
            }

            Pausa();
        }

        static void ProbarCambioPin()
        {
            try
            {
                using (ServiceClient client = new ServiceClient())
                {
                    CambioPinRequest request = new CambioPinRequest();

                    Console.Write("Número de tarjeta: ");
                    request.NumeroTarjeta = Console.ReadLine();

                    Console.Write("PIN actual: ");
                    request.PinActual = Console.ReadLine();

                    Console.Write("PIN nuevo: ");
                    request.PinNuevo = Console.ReadLine();

                    Console.Write("Fecha vencimiento: ");
                    request.FechaVencimiento = Console.ReadLine();

                    Console.Write("CVV: ");
                    request.Cvv = Console.ReadLine();

                    Console.Write("Id Comercio o Cajero: ");
                    request.IdComercioOCajero = Console.ReadLine();

                    CambioPinResponse response = client.CambioPin(request);

                    Console.WriteLine("\n===== RESPUESTA =====");
                    Console.WriteLine("Resultado: " + response.Resultado);
                    Console.WriteLine("Mensaje: " + response.Mensaje);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consumir CambioPin: " + ex.Message);
            }

            Pausa();
        }

        static void Pausa()
        {
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }
    }
}
