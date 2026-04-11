using SistemaTarjetas.Models;
using ServiceReference2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaTarjetas.Services
{
    public class ConsultaClienteService : IConsultaClienteService
    {
        private readonly ServiceClient _wsAutorizador;

        public ConsultaClienteService()
        {
            _wsAutorizador = new ServiceClient();
        }

        // Método para consultar saldo real al WS Autorizador
        public async Task<decimal> ObtenerSaldoRealAsync(string numeroTarjeta, string cvv, string fechaVencimiento)
        {
            try
            {
                var request = new ConsultaRequest
                {
                    NumeroTarjeta = Helpers.Encriptacion.Encriptar(numeroTarjeta),
                    Cvv = Helpers.Encriptacion.Encriptar(cvv),
                    FechaVencimiento = Helpers.Encriptacion.Encriptar(fechaVencimiento),
                    IdComercioOCajero = "BANCO001"
                };

                var response = await _wsAutorizador.ConsultaAsync(request);

                if (response.Resultado && decimal.TryParse(response.Saldo, out decimal saldo))
                {
                    return saldo;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar saldo: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<Cuenta>> ObtenerCuentasClienteAsync(string identificacion)
        {
            var cuentas = new List<Cuenta>();

            try
            {
                // Obtener tarjetas del cliente
                var tarjetas = await ObtenerTarjetasClienteAsync(identificacion);

                foreach (var tarjeta in tarjetas)
                {
                    if (tarjeta.Tipo == "Debito" && !string.IsNullOrEmpty(tarjeta.CuentaAsociada))
                    {
                        // Consultar saldo real desde el WS
                        var saldoReal = await ObtenerSaldoRealAsync(tarjeta.NumeroTarjeta, tarjeta.CVV, tarjeta.FechaVencimiento);

                        cuentas.Add(new Cuenta
                        {
                            Id = tarjeta.Id,
                            NumeroCuenta = tarjeta.CuentaAsociada,
                            IdCliente = identificacion,
                            Saldo = saldoReal
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener cuentas: {ex.Message}");
            }

            // Si no se encontraron cuentas, devolver datos de ejemplo
            if (cuentas.Count == 0)
            {
                cuentas.Add(new Cuenta { Id = "1", NumeroCuenta = "100-123456-7", IdCliente = identificacion, Saldo = 150000 });
                cuentas.Add(new Cuenta { Id = "2", NumeroCuenta = "100-765432-1", IdCliente = identificacion, Saldo = 25500 });
            }

            return cuentas;
        }

        public async Task<List<Tarjeta>> ObtenerTarjetasClienteAsync(string identificacion)
        {
            // TODO: Aquí debes llamar al WS real que devuelve las tarjetas del cliente
            // Por ahora, datos de ejemplo
            await Task.Delay(100);

            return new List<Tarjeta>
            {
                new Tarjeta
                {
                    Id = "1",
                    NumeroTarjeta = "9111111111111234",
                    Tipo = "Debito",
                    CuentaAsociada = "100-123456-7",
                    IdCliente = identificacion,
                    CVV = "123",
                    FechaVencimiento = "12/2028"
                },
                new Tarjeta
                {
                    Id = "2",
                    NumeroTarjeta = "9111122222225678",
                    Tipo = "Credito",
                    CuentaAsociada = "",
                    IdCliente = identificacion,
                    CVV = "456",
                    FechaVencimiento = "10/2027"
                }
            };
        }

        public async Task<List<MovimientoCuenta>> ObtenerMovimientosCuentaAsync(string numeroCuenta, string identificacion)
        {
            // TODO: Llamar a WS real de movimientos de cuenta
            await Task.Delay(100);

            return new List<MovimientoCuenta>
            {
                new MovimientoCuenta { Fecha = DateTime.Parse("15/03/2026 10:30"), Descripcion = "Compra Supermercado", Monto = 25000 },
                new MovimientoCuenta { Fecha = DateTime.Parse("10/03/2026 14:15"), Descripcion = "Retiro Cajero", Monto = 50000 },
                new MovimientoCuenta { Fecha = DateTime.Parse("05/03/2026 09:00"), Descripcion = "Pago Servicios", Monto = 15000 },
                new MovimientoCuenta { Fecha = DateTime.Parse("01/03/2026 18:45"), Descripcion = "Transferencia", Monto = 10000 }
            };
        }

        public async Task<List<MovimientoTarjeta>> ObtenerMovimientosTarjetaCreditoAsync(string numeroTarjeta, string identificacion)
        {
            // TODO: Llamar a WS real de movimientos de tarjeta de crédito
            await Task.Delay(100);

            return new List<MovimientoTarjeta>
            {
                new MovimientoTarjeta { Fecha = DateTime.Parse("15/03/2026 10:30"), CodigoAutorizacion = "AUTH-123456", Comercio = "Tienda Online S.A.", Monto = 75000, Exitoso = true },
                new MovimientoTarjeta { Fecha = DateTime.Parse("10/03/2026 14:15"), CodigoAutorizacion = "AUTH-789012", Comercio = "Restaurante El Buen Sabor", Monto = 35000, Exitoso = true },
                new MovimientoTarjeta { Fecha = DateTime.Parse("05/03/2026 09:00"), CodigoAutorizacion = "AUTH-345678", Comercio = "Supermercado Mega", Monto = 120000, Exitoso = true },
                new MovimientoTarjeta { Fecha = DateTime.Parse("01/03/2026 18:45"), CodigoAutorizacion = "AUTH-901234", Comercio = "Gasolinera La Más Barata", Monto = 45000, Exitoso = true }
            };
        }
    }
}