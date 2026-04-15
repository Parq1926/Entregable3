using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Dynamic;

/// <summary>
/// Clase para manejar la bitácora de operaciones en formato JSON
/// </summary>
public static class Bitacora
{
    private static readonly string rutaBitacora = @"C:\Users\rojas\OneDrive\Alcance\progra\ServicioWeb\BitacorasWCF\bitacora.json";
    private static readonly object lockObj = new object();

    /// <summary>
    /// Registra una operación en la bitácora con datos encriptados
    /// </summary>
    public static void Registrar(string operacion, object solicitud, object respuesta)
    {
        try
        {
            // Encriptar los datos sensibles antes de guardarlos
            object solicitudEncriptada = EncriptarObjeto(solicitud);
            object respuestaEncriptada = EncriptarObjeto(respuesta);

            var registro = new
            {
                fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                operacion = operacion,
                solicitud = solicitudEncriptada,
                respuesta = respuestaEncriptada
            };

            string jsonRegistro = Newtonsoft.Json.JsonConvert.SerializeObject(registro, Newtonsoft.Json.Formatting.Indented);

            lock (lockObj)
            {
                string directorio = Path.GetDirectoryName(rutaBitacora);
                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                if (!File.Exists(rutaBitacora))
                {
                    File.WriteAllText(rutaBitacora, "[\n" + jsonRegistro + "\n]");
                }
                else
                {
                    string contenido = File.ReadAllText(rutaBitacora);
                    if (contenido.TrimEnd().EndsWith("]"))
                    {
                        contenido = contenido.TrimEnd().Substring(0, contenido.TrimEnd().Length - 1);
                        contenido += ",\n" + jsonRegistro + "\n]";
                        File.WriteAllText(rutaBitacora, contenido);
                    }
                    else
                    {
                        File.AppendAllText(rutaBitacora, ",\n" + jsonRegistro);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[Bitacora] Error: " + ex.Message);
        }
    }

    /// <summary>
    /// Encripta los campos sensibles de un objeto (usuario, contraseña, tipoUsuario)
    /// </summary>
    private static object EncriptarObjeto(object obj)
    {
        if (obj == null) return null;

        try
        {
            // Usar reflexión para examinar las propiedades del objeto
            var tipo = obj.GetType();
            var propiedades = tipo.GetProperties();

            // Crear un objeto anónimo con los valores encriptados
            var resultado = new ExpandoObject();
            var dict = (IDictionary<string, object>)resultado;

            foreach (var prop in propiedades)
            {
                var valor = prop.GetValue(obj);
                var nombreProp = prop.Name;

                // Encriptar campos sensibles
                if (nombreProp.ToLower().Contains("usuario") ||
                    nombreProp.ToLower().Contains("contrasena") ||
                    nombreProp.ToLower().Contains("password") ||
                    nombreProp.ToLower().Contains("contrasena") ||
                    nombreProp.ToLower().Contains("tipousuario"))  // 👈 AGREGADO
                {
                    if (valor != null && !string.IsNullOrEmpty(valor.ToString()))
                    {
                        // Guardar SOLO el valor encriptado, sin el texto "[ENCRIPTADO]"
                        dict[nombreProp] = Encriptacion.Encriptar(valor.ToString());
                    }
                    else
                    {
                        dict[nombreProp] = valor;
                    }
                }
                else
                {
                    dict[nombreProp] = valor;
                }
            }

            return resultado;
        }
        catch
        {
            // Si no se puede encriptar, devolver el objeto original
            return obj;
        }
    }

    /// <summary>
    /// Versión simplificada que también encripta datos
    /// </summary>
    public static void RegistrarSimple(string operacion, object solicitud, object respuesta)
    {
        try
        {
            string fechaArchivo = DateTime.Now.ToString("yyyy-MM-dd");
            string ruta = Path.Combine(Path.GetTempPath(), "BitacoraWCF", "bitacora_" + fechaArchivo + ".json");

            object solicitudEncriptada = EncriptarObjeto(solicitud);
            object respuestaEncriptada = EncriptarObjeto(respuesta);

            var registro = new
            {
                fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                operacion = operacion,
                solicitud = solicitudEncriptada,
                respuesta = respuestaEncriptada
            };

            string jsonRegistro = Newtonsoft.Json.JsonConvert.SerializeObject(registro, Newtonsoft.Json.Formatting.Indented);

            string directorio = Path.GetDirectoryName(ruta);
            if (!Directory.Exists(directorio))
                Directory.CreateDirectory(directorio);

            if (!File.Exists(ruta))
            {
                File.WriteAllText(ruta, "[\n" + jsonRegistro + "\n]");
            }
            else
            {
                string contenido = File.ReadAllText(ruta);
                if (contenido.TrimEnd().EndsWith("]"))
                {
                    contenido = contenido.TrimEnd().Substring(0, contenido.TrimEnd().Length - 1);
                    contenido += ",\n" + jsonRegistro + "\n]";
                    File.WriteAllText(ruta, contenido);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[Bitacora] Error: " + ex.Message);
        }
    }
}