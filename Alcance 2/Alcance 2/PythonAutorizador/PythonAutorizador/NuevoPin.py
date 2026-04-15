import mysql.connector
from conexion_core import enviar_a_core_java
from Bitacora import registrar_bitacora
import traceback

def obtener_conexion():
    return mysql.connector.connect(
        host="localhost",
        user="root",
        password="70780541Yen",
        database="cajeroautomatico",
        autocommit=False
    )

def cambio_Pin(req: dict) -> dict:
    numero_tarjeta = req.get("NumeroTarjeta")
    pin_actual = req.get("PinActual")
    pin_nuevo = req.get("NuevoPin")

    print(f"🔍 Datos recibidos: {numero_tarjeta}, actual={pin_actual}, nuevo={pin_nuevo}")

    if not numero_tarjeta or not pin_actual or not pin_nuevo:
        return {"ok": False, "mensaje": "Datos incompletos"}

    if not pin_nuevo.isdigit() or len(pin_nuevo) != 4:
        return {"ok": False, "mensaje": "El nuevo PIN debe tener 4 dígitos"}

    conexion = None
    cursor = None

    try:
        conexion = obtener_conexion()
        cursor = conexion.cursor(dictionary=True)

        cursor.execute(
            "SELECT Numero_Tarjeta, Numero_Identificacion, Pin FROM tarjeta WHERE Numero_Tarjeta = %s",
            (numero_tarjeta,)
        )
        tarjeta = cursor.fetchone()

        if not tarjeta:
            return {"ok": False, "mensaje": "Tarjeta no encontrada"}

        if str(tarjeta["Pin"]) != str(pin_actual):
            return {"ok": False, "mensaje": "PIN actual incorrecto"}

        print("🔄 Enviando cambio de PIN a Java...")
        trama = f"4 00000000-0 {numero_tarjeta} {pin_nuevo}"
        print(f"📤 TRAMA ENVIADA: {trama}")

        try:
            respuesta_java = enviar_a_core_java(trama)
            print(f"📥 RESPUESTA DE JAVA: {respuesta_java}")
        except Exception as e:
            print(f"❌ Error al enviar a Java: {e}")
            traceback.print_exc()
            respuesta_java = None

        java_ok = respuesta_java == "OK"

        print("🔄 Actualizando PIN en MySQL...")
        cursor.execute(
            "UPDATE tarjeta SET Pin = %s WHERE Numero_Tarjeta = %s",
            (pin_nuevo, numero_tarjeta)
        )
        conexion.commit()
        print("✅ PIN actualizado en MySQL")

        cursor.execute("SELECT IdCajero FROM Cajero LIMIT 1")
        cajero = cursor.fetchone()
        id_cajero = cajero["IdCajero"] if cajero else 1

        registrar_bitacora(
            numero_tarjeta=numero_tarjeta,
            id_cajero=id_cajero,
            id_cliente=tarjeta["Numero_Identificacion"],
            tipo="CambioPin",
            monto=None
        )

        if java_ok:
            return {"ok": True, "mensaje": "PIN actualizado en ambas bases"}
        else:
            return {"ok": True, "mensaje": "PIN actualizado solo en MySQL"}

    except mysql.connector.Error as e:
        if conexion:
            conexion.rollback()
        traceback.print_exc()
        return {"ok": False, "mensaje": f"Error en base de datos: {str(e)}"}

    except Exception as e:
        if conexion:
            conexion.rollback()
        traceback.print_exc()
        return {"ok": False, "mensaje": f"Error: {str(e)}"}

    finally:
        if cursor:
            cursor.close()
        if conexion:
            conexion.close()
        print("🔒 Conexión cerrada")