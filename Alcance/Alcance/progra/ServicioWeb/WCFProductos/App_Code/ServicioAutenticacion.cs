using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;
using System.Diagnostics;

/// <summary>
/// Implementación del servicio de autenticación
/// </summary>
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServicioAutenticacion : IServicioAutenticacion
{
    private IMongoCollection<UsuarioMongo> _coleccionUsuarios;

    public ServicioAutenticacion()
    {
        try
        {
            // Configurar conexión a MongoDB
            string connectionString = ConfigurationManager.AppSettings["MongoDBConnection"] ?? "mongodb://localhost:27017";
            string databaseName = ConfigurationManager.AppSettings["MongoDBDatabase"] ?? "BancoABC";
            string collectionName = ConfigurationManager.AppSettings["MongoDBCollection"] ?? "Usuarios";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _coleccionUsuarios = database.GetCollection<UsuarioMongo>(collectionName);

            Debug.WriteLine("[Servicio] Conectado a MongoDB correctamente");

            // Bitácora de inicio del servicio
            Bitacora.Registrar("INICIO_SERVICIO", new { mensaje = "Servicio iniciado" }, null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("[Servicio] Error conectando a MongoDB: " + ex.Message);
            Bitacora.Registrar("ERROR_INICIO", new { error = ex.Message }, null);
        }
    }

    public ResultadoAutenticacion ValidarLogin(Credenciales credenciales)
    {
        var resultado = new ResultadoAutenticacion();

        try
        {
            // Validar que las credenciales no sean nulas
            if (credenciales == null ||
                string.IsNullOrEmpty(credenciales.UsuarioEncriptado) ||
                string.IsNullOrEmpty(credenciales.ContrasenaEncriptada))
            {
                resultado.Resultado = false;
                resultado.Mensaje = "Credenciales no proporcionadas";

                // ✅ Bitácora con contraseña (pero no la tenemos aún)
                Bitacora.Registrar("LOGIN_INVALIDO", new
                {
                    credenciales = credenciales == null ? "null" : "recibidas",
                    mensaje = "Credenciales incompletas"
                }, resultado);

                return resultado;
            }

            // Desencriptar credenciales recibidas
            string usuario = Encriptacion.Desencriptar(credenciales.UsuarioEncriptado);
            string contrasena = Encriptacion.Desencriptar(credenciales.ContrasenaEncriptada);

            Debug.WriteLine("[Servicio] Validando login para usuario (desencriptado): " + usuario);

            // ENCRIPTAR EL USUARIO PARA BUSCARLO EN LA BD
            string usuarioEncriptado = Encriptacion.Encriptar(usuario);
            Debug.WriteLine("[Servicio] Buscando usuario encriptado: " + usuarioEncriptado);

            // Buscar usuario en MongoDB por usuario ENCRIPTADO
            var filter = Builders<UsuarioMongo>.Filter.Or(
                Builders<UsuarioMongo>.Filter.Eq(u => u.Usuario, usuarioEncriptado),
                Builders<UsuarioMongo>.Filter.Eq(u => u.Identificacion, usuario) //AdminUsuarios (CÉDULA).
);
            var usuarioEncontrado = _coleccionUsuarios.Find(filter).FirstOrDefault();

            // Verificar si el usuario existe
            if (usuarioEncontrado == null)
            {
                Debug.WriteLine("[Servicio] Usuario no encontrado en BD");
                resultado.Resultado = false;
                resultado.Mensaje = "Usuario y/o contraseña incorrectos";

                // ✅ Bitácora con usuario y contraseña (en texto plano, pero se encriptarán al guardarse)
                Bitacora.Registrar("LOGIN_USUARIO_NO_EXISTE", new
                {
                    usuario = usuario,                // 👈 Se encriptará automáticamente
                    contrasena = contrasena,          // 👈 SE ENCRIPTARÁ AUTOMÁTICAMENTE
                    usuarioEncriptadoRecibido = credenciales.UsuarioEncriptado
                }, resultado);

                return resultado;
            }

            // Desencriptar la contraseña almacenada
            string contrasenaAlmacenada = Encriptacion.Desencriptar(usuarioEncontrado.Contrasena);

            Debug.WriteLine("[Servicio] Contraseña ingresada: " + contrasena);
            Debug.WriteLine("[Servicio] Contraseña almacenada (desencriptada): " + contrasenaAlmacenada);

            // Verificar contraseña y estado
            if (contrasenaAlmacenada == contrasena)
            {
                if (usuarioEncontrado.Estado == 1) //AdminUsuario
                {
                    resultado.Resultado = true;
                    resultado.Mensaje = "Exitoso";
                    resultado.TipoUsuario = usuarioEncontrado.Tipo;

                    Debug.WriteLine("[Servicio] Login exitoso para usuario");

                    // ✅ Bitácora con usuario y contraseña (se encriptarán)
                    Bitacora.Registrar("LOGIN_EXITOSO", new
                    {
                        usuario = usuario,                // 👈 Se encriptará
                        contrasena = contrasena,          // 👈 SE ENCRIPTARÁ
                        tipo = usuarioEncontrado.Tipo
                    }, resultado);
                }
                else
                {
                    resultado.Resultado = false;
                    resultado.Mensaje = "Usuario inactivo";

                    // ✅ Bitácora con usuario y contraseña
                    Bitacora.Registrar("LOGIN_USUARIO_INACTIVO", new
                    {
                        usuario = usuario,                // 👈 Se encriptará
                        contrasena = contrasena,          // 👈 SE ENCRIPTARÁ
                        estado = usuarioEncontrado.Estado
                    }, resultado);
                }
            }
            else
            {
                resultado.Resultado = false;
                resultado.Mensaje = "Usuario y/o contraseña incorrectos";

                // ✅ Bitácora con usuario y contraseña (incorrecta)
                Bitacora.Registrar("LOGIN_CONTRASENA_INCORRECTA", new
                {
                    usuario = usuario,                // 👈 Se encriptará
                    contrasena = contrasena,          // 👈 SE ENCRIPTARÁ
                    motivo = "Contraseña no coincide"
                }, resultado);
            }
        }
        catch (Exception ex)
        {
            resultado.Resultado = false;
            resultado.Mensaje = "Error en el servicio";
            Debug.WriteLine("[Servicio] Error en ValidarLogin: " + ex.Message);

            // ✅ Bitácora de error interno
            Bitacora.Registrar("LOGIN_ERROR", new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace
            }, resultado);
        }

        return resultado;
    }

    public ResultadoRegistro RegistrarUsuario(UsuarioRegistro usuario)
    {
        var resultado = new ResultadoRegistro();

        try
        {
            Debug.WriteLine("[Servicio] Intentando registrar usuario: " + usuario.Usuario);

            // Validar campos obligatorios
            if (string.IsNullOrEmpty(usuario.Usuario) || string.IsNullOrEmpty(usuario.Contrasena))
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Usuario y contraseña son obligatorios";

                // ✅ Bitácora de registro inválido
                Bitacora.Registrar("REGISTRO_CAMPOS_INVALIDOS", usuario, resultado);

                return resultado;
            }

            // ENCRIPTAR EL USUARIO Y LA CONTRASEÑA ANTES DE GUARDAR
            // SOLO si viene vacío, usar cédula AdminUsuarios
            if (string.IsNullOrEmpty(usuario.Usuario))
            {
                usuario.Usuario = usuario.Identificacion;
            }
            string usuarioEncriptado = Encriptacion.Encriptar(usuario.Usuario);
            string contrasenaEncriptada = Encriptacion.Encriptar(usuario.Contrasena);

            Debug.WriteLine("[Servicio] Usuario original: " + usuario.Usuario);
            Debug.WriteLine("[Servicio] Usuario encriptado: " + usuarioEncriptado);
            Debug.WriteLine("[Servicio] Contraseña encriptada: " + contrasenaEncriptada);

            // Validar que el usuario (encriptado) no exista
            var filter = Builders<UsuarioMongo>.Filter.Eq(u => u.Usuario, usuarioEncriptado);
            var existe = _coleccionUsuarios.Find(filter).Any();

            if (existe)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "El nombre de usuario ya existe";

                // ✅ Bitácora de usuario duplicado
                Bitacora.Registrar("REGISTRO_USUARIO_DUPLICADO", new
                {
                    usuario = usuario.Usuario,
                    usuarioEncriptado = usuarioEncriptado
                }, resultado);

                return resultado;
            }

            // Validar que el email no exista (el email NO se encripta para poder buscarlo)
            if (!string.IsNullOrEmpty(usuario.Email))
            {
                filter = Builders<UsuarioMongo>.Filter.Eq(u => u.Email, usuario.Email);
                existe = _coleccionUsuarios.Find(filter).Any();

                if (existe)
                {
                    resultado.Exitoso = false;
                    resultado.Mensaje = "El email ya está registrado";

                    // ✅ Bitácora de email duplicado
                    Bitacora.Registrar("REGISTRO_EMAIL_DUPLICADO", new
                    {
                        email = usuario.Email
                    }, resultado);

                    return resultado;
                }
            }

            // Crear el documento para MongoDB con USUARIO Y CONTRASEÑA ENCRIPTADOS
            var nuevoUsuario = new UsuarioMongo
            {
                Identificacion = usuario.Identificacion ?? "",
                Nombre = usuario.Nombre ?? "",
                PrimerApellido = usuario.PrimerApellido ?? "",
                SegundoApellido = usuario.SegundoApellido ?? "",
                Email = usuario.Email ?? "",
                Usuario = usuarioEncriptado,        // USUARIO ENCRIPTADO
                Contrasena = contrasenaEncriptada,  // CONTRASEÑA ENCRIPTADA
                Estado = 1, //AdminUsuarios,
                Tipo = usuario.Tipo
            };

            // Guardar en MongoDB
            _coleccionUsuarios.InsertOne(nuevoUsuario);

            Debug.WriteLine("[Servicio] Usuario registrado exitosamente");

            resultado.Exitoso = true;
            resultado.Mensaje = "Usuario registrado exitosamente";

            // ✅ Bitácora de registro exitoso
            Bitacora.Registrar("REGISTRO_EXITOSO", new
            {
                usuario = usuario.Usuario,
                email = usuario.Email,
                tipo = usuario.Tipo
            }, resultado);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("[Servicio] Error registrando usuario: " + ex.Message);
            resultado.Exitoso = false;
            resultado.Mensaje = "Error en el servicio: " + ex.Message;

            // ✅ Bitácora de error interno
            Bitacora.Registrar("REGISTRO_ERROR", new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace
            }, resultado);
        }

        return resultado;
    }

    //------ADMINUSUARIOS-----
    /// <summary>
    /// Obtiene todos los usuarios registrados
    /// </summary>
    public List<UsuarioRegistro> ObtenerUsuarios()
    {
        var lista = new List<UsuarioRegistro>();

        try
        {
            var usuarios = _coleccionUsuarios.Find(_ => true).ToList();

            foreach (var u in usuarios)
            {
                lista.Add(new UsuarioRegistro
                {
                    Identificacion = u.Identificacion,
                    Nombre = u.Nombre,
                    PrimerApellido = u.PrimerApellido,
                    SegundoApellido = u.SegundoApellido,
                    Email = u.Email,
                    Usuario = Encriptacion.Desencriptar(u.Usuario), 
                    Contrasena = "", //NO devuelve la contraseña.
                    Tipo = u.Tipo
                });
            }

            //Bitácora
            Bitacora.Registrar("OBTENER_USUARIOS", null, new { cantidad = lista.Count });
        }
        catch (Exception ex)
        {
            Bitacora.Registrar("ERROR_OBTENER_USUARIOS", new { error = ex.Message }, null);
        }

        return lista;
    }
    //

    //-----ADMINUSUARIO-----
    /// <summary>
    /// Actualiza los datos de un usuario existente
    /// </summary>
    public ResultadoRegistro EditarUsuario(UsuarioRegistro usuario)
    {
        var resultado = new ResultadoRegistro();

        try
        {
            // Busca cédula
            var filter = Builders<UsuarioMongo>.Filter.Eq(u => u.Identificacion, usuario.Identificacion);

            var usuarioExistente = _coleccionUsuarios.Find(filter).FirstOrDefault();

            if (usuarioExistente == null)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Usuario no encontrado";

                Bitacora.Registrar("EDITAR_USUARIO_NO_EXISTE", usuario, resultado);
                return resultado;
            }

            var update = Builders<UsuarioMongo>.Update
                .Set(u => u.Nombre, usuario.Nombre)
                .Set(u => u.PrimerApellido, usuario.PrimerApellido)
                .Set(u => u.SegundoApellido, usuario.SegundoApellido)
                .Set(u => u.Email, usuario.Email)
                .Set(u => u.Tipo, usuario.Tipo);

            _coleccionUsuarios.UpdateOne(filter, update);

            resultado.Exitoso = true;
            resultado.Mensaje = "Usuario actualizado correctamente";

            Bitacora.Registrar("EDITAR_USUARIO_EXITOSO", usuario, resultado);
        }
        catch (Exception ex)
        {
            resultado.Exitoso = false;
            resultado.Mensaje = "Error al actualizar";

            Bitacora.Registrar("ERROR_EDITAR_USUARIO", new { error = ex.Message }, resultado);
        }

        return resultado;
    }

    //-----ADMINUSUARIOS-----
    public ResultadoRegistro InactivarUsuario(string identificacion)
    {
        var resultado = new ResultadoRegistro();

        try
        {
            var filter = Builders<UsuarioMongo>.Filter.Eq(u => u.Identificacion, identificacion);

            var update = Builders<UsuarioMongo>.Update
                .Set(u => u.Estado, 0);

            var res = _coleccionUsuarios.UpdateOne(filter, update);

            if (res.ModifiedCount > 0)
            {
                resultado.Exitoso = true;
                resultado.Mensaje = "Usuario inactivado correctamente";
            }
            else
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Usuario no encontrado";
            }

            Bitacora.Registrar("INACTIVAR_USUARIO", new { identificacion }, resultado);
        }
        catch (Exception ex)
        {
            resultado.Exitoso = false;
            resultado.Mensaje = "Error al inactivar";

            Bitacora.Registrar("ERROR_INACTIVAR", new { error = ex.Message }, resultado);
        }

        return resultado;
    }

    //-----ADMINUSUARIOS-----
    public ResultadoRegistro ActivarUsuario(string identificacion)
    {
        var resultado = new ResultadoRegistro();

        try
        {
            var filter = Builders<UsuarioMongo>.Filter.Eq(u => u.Identificacion, identificacion);

            var update = Builders<UsuarioMongo>.Update
                .Set(u => u.Estado, 1);

            var res = _coleccionUsuarios.UpdateOne(filter, update);

            if (res.ModifiedCount > 0)
            {
                resultado.Exitoso = true;
                resultado.Mensaje = "Usuario activado correctamente";
            }
            else
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Usuario no encontrado";
            }

            Bitacora.Registrar("ACTIVAR_USUARIO", new { identificacion }, resultado);
        }
        catch (Exception ex)
        {
            resultado.Exitoso = false;
            resultado.Mensaje = "Error al activar";

            Bitacora.Registrar("ERROR_ACTIVAR", new { error = ex.Message }, resultado);
        }

        return resultado;
    }
}