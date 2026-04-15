import mysql.connector
from conexion_core import enviar_a_core_java


def obtener_conexion():
    return mysql.connector.connect(
        host="localhost",
        user="root",
        password="70780541Yen",
        database="cajeroautomatico"
    )


def validar_pin(req: dict) -> dict:
    try:
        numero_tarjeta = req.get("Numero_Tarjeta") or req.get("NumeroTarjeta") or req.get("numeroTarjeta")
        pin = req.get("Pin") or req.get("pin")

        if not pin:
            return {"ok": False, "errores": ["PIN vacío"]}

        # ===== 1. INTENTAR CON CORE JAVA =====
        trama = f"3 00000000-0 {numero_tarjeta} {pin}"
        respuesta_java = enviar_a_core_java(trama)

        if respuesta_java == "OK":
            print("✅ Java validó el PIN")
            return {"ok": True}

        # ===== 2. VALIDACIÓN LOCAL =====
        conexion = obtener_conexion()
        cursor = conexion.cursor(dictionary=True)

        sql = """
        SELECT 1
        FROM tarjeta
        WHERE Numero_Tarjeta = %s AND Pin = %s
        """
        cursor.execute(sql, (numero_tarjeta, pin))
        resultado = cursor.fetchone()
        cursor.close()
        conexion.close()

        if resultado:
            return {"ok": True}
        else:
            return {"ok": False, "errores": ["PIN incorrecto"]}

    except Exception as e:
        return {"ok": False, "errores": [f"Error: {str(e)}"]}