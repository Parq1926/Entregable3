import mysql.connector
from datetime import datetime
from conexion_core import enviar_a_core_java


def obtener_conexion():
    return mysql.connector.connect(
        host="localhost",
        user="root",
        password="70780541Yen",
        database="cajeroautomatico"
    )


def validar_fecha(req: dict) -> dict:
    try:
        numero_tarjeta = req.get("Numero_Tarjeta") or req.get("NumeroTarjeta") or req.get("numeroTarjeta")
        fecha_vencimiento = req.get("Fecha_Vencimiento") or req.get("FechaVencimiento")

        if not fecha_vencimiento:
            return {"ok": False, "errores": ["Fecha vacía"]}

        # ===== 1. INTENTAR CON CORE JAVA =====
        trama = f"5 00000000-0 {numero_tarjeta} {fecha_vencimiento}"
        respuesta_java = enviar_a_core_java(trama)

        if respuesta_java == "OK":
            print("✅ Java validó la fecha")
            return {"ok": True}

        # ===== 2. VALIDACIÓN LOCAL =====
        fecha_mysql = datetime.fromisoformat(fecha_vencimiento).date()

        conexion = obtener_conexion()
        cursor = conexion.cursor(dictionary=True)

        sql = """
        SELECT 1
        FROM tarjeta
        WHERE Numero_Tarjeta = %s AND DATE(Fecha_Vencimiento) = %s
        """
        cursor.execute(sql, (numero_tarjeta, fecha_mysql))
        resultado = cursor.fetchone()
        cursor.close()
        conexion.close()

        if resultado:
            return {"ok": True}
        else:
            return {"ok": False, "errores": ["Fecha incorrecta"]}

    except Exception as e:
        return {"ok": False, "errores": [f"Error: {str(e)}"]}