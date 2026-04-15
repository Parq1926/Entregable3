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


def validar_Codigo(data):
    try:
        numero_tarjeta = data.get("NumeroTarjeta") or data.get("Numero_Tarjeta") or data.get("numeroTarjeta")
        fecha_vencimiento = data.get("FechaVencimiento") or data.get("Fecha_Vencimiento")
        codigo_verificacion = data.get("codigoVerificacion") or data.get("CodigoVerificacion") or data.get("CVV")

        if not codigo_verificacion:
            return {"ok": False, "errores": ["Código vacío"]}

        # ===== 1. INTENTAR CON CORE JAVA =====
        trama = f"6 00000000-0 {numero_tarjeta} {codigo_verificacion}"
        respuesta_java = enviar_a_core_java(trama)

        if respuesta_java == "OK":
            print("✅ Java validó el CVV")
            return {"ok": True}

        # ===== 2. VALIDACIÓN LOCAL EN MYSQL =====
        fecha_mysql = datetime.fromisoformat(fecha_vencimiento).date()

        conexion = obtener_conexion()
        cursor = conexion.cursor(dictionary=True)

        sql = """
        SELECT 1
        FROM tarjeta
        WHERE Numero_Tarjeta = %s
          AND DATE(Fecha_Vencimiento) = %s
          AND CVV = %s
        """
        cursor.execute(sql, (numero_tarjeta, fecha_mysql, codigo_verificacion))
        resultado = cursor.fetchone()
        cursor.close()
        conexion.close()

        if resultado:
            return {"ok": True}
        else:
            return {"ok": False, "errores": ["Datos incorrectos"]}

    except Exception as e:
        return {"ok": False, "errores": [f"Error: {str(e)}"]}