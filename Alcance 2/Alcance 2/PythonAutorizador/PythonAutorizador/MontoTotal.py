import mysql.connector
from conexion_core import procesar_consulta_core


def consulta_cuenta(datos):
    """
    Consulta saldo de la cuenta
    """
    try:
        numeroTarjeta = datos.get("Numero_Tarjeta") or datos.get("numeroTarjeta") or datos.get("NumeroTarjeta")

        if not numeroTarjeta:
            return {"ok": False, "mensaje": "Número de tarjeta requerido"}

        # ===== 1. INTENTAR CON CORE JAVA =====
        try:
            resultado_core = procesar_consulta_core(numeroTarjeta)
            if resultado_core.get("ok"):
                print(f"✅ Consulta procesada por Core Java: {numeroTarjeta}")
                return {
                    "ok": True,
                    "saldo": resultado_core["saldo"],
                    "mensaje": "Consulta exitosa"
                }
            else:
                print("⚠️ Usando fallback local para consulta")
        except Exception as e:
            print(f"⚠️ Error con Core Java: {e}, usando fallback local")

        # ===== 2. FALLBACK LOCAL =====
        conn = mysql.connector.connect(
            host="localhost",
            user="root",
            password="70780541Yen",
            database="cajeroautomatico"
        )
        cursor = conn.cursor(dictionary=True)
        cursor.execute(
            "SELECT Monto_Total FROM tarjeta WHERE Numero_Tarjeta = %s",
            (numeroTarjeta,)
        )
        tarjeta = cursor.fetchone()
        cursor.close()
        conn.close()

        if tarjeta:
            return {
                "ok": True,
                "saldo": float(tarjeta["Monto_Total"]),
                "mensaje": "Consulta exitosa"
            }
        else:
            return {"ok": False, "mensaje": "Tarjeta no encontrada"}

    except Exception as e:
        print(f"❌ Error en consulta: {e}")
        return {"ok": False, "mensaje": f"Error: {str(e)}"}