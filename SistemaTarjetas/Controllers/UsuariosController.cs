using Microsoft.AspNetCore.Mvc;
using WSAutenticador;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaTarjetas.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ServicioAutenticacionClient _client;

        public UsuariosController()
        {
            _client = new ServicioAutenticacionClient();
        }

        //LISTAR
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = (await _client.ObtenerUsuariosAsync()).ToList();
            return View(usuarios);
        }

        //CREAR
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioRegistro usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            //El usuario es la cédual.
            usuario.Usuario = usuario.Identificacion;

            var resultado = await _client.RegistrarUsuarioAsync(usuario);

            if (resultado.Exitoso)
                return RedirectToAction("Usuarios");

            ViewBag.Error = resultado.Mensaje;
            return View(usuario);
        }

        //EDITAR
        public async Task<IActionResult> Editar(string id)
        {
            var usuarios = (await _client.ObtenerUsuariosAsync()).ToList();

            var usuario = usuarios.FirstOrDefault(u => u.Identificacion == id);

            if (usuario == null)
                return RedirectToAction("Usuarios");

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(UsuarioRegistro usuario)
        {
            var resultado = await _client.EditarUsuarioAsync(usuario);

            if (resultado.Exitoso)
                return RedirectToAction("Usuarios");

            ViewBag.Error = resultado.Mensaje;
            return View(usuario);
        }

        //INACTIVAR
        public async Task<IActionResult> Inactivar(string id)
        {
            await _client.InactivarUsuarioAsync(id);
            return RedirectToAction("Usuarios");
        }

        //ACTIVAR
        public async Task<IActionResult> Activar(string id)
        {
            await _client.ActivarUsuarioAsync(id);
            return RedirectToAction("Usuarios");
        }
    }
}