using Microsoft.AspNetCore.Mvc;
using SistemaTarjetas.Models;
using SistemaTarjetas.Services;
using System.Threading.Tasks;

namespace SistemaTarjetas.Controllers
{
    public class SimuladorController : Controller
    {
        private readonly IAutorizadorService _autorizadorService;

        public SimuladorController(IAutorizadorService autorizadorService)
        {
            _autorizadorService = autorizadorService;
        }

        // GET: Simulador
        public IActionResult Index()
        {
            return View(new CompraViewModel());
        }

        // POST: Simulador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CompraViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Preparar request para el WS
            var request = new AutorizacionRequest
            {
                NumeroTarjeta = model.NumeroTarjeta,
                CVV = model.CVV,
                PIN = model.PIN,
                FechaVencimiento = model.FechaVencimiento,
                NombreCliente = model.NombreCliente,
                IdentificacionComercio = model.Comercio,
                Monto = model.Monto,
                TipoTransaccion = "Retiro"
            };

            // Llamar al servicio (esto llama al WS real)
            var response = await _autorizadorService.AutorizarRetiro(request);

            if (response.Resultado)
            {
                ViewBag.Mensaje = "Su compra se ha realizado de manera satisfactoria";
                ViewBag.Tipo = "success";
                ViewBag.Icono = "fa-check-circle";

                // Limpiar formulario
                ModelState.Clear();
                model = new CompraViewModel();
            }
            else
            {
                ViewBag.Mensaje = response.Mensaje;
                ViewBag.Tipo = "error";
                ViewBag.Icono = "fa-exclamation-circle";
            }

            return View(model);
        }
    }
}