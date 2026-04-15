# Retiro.py
import json
import mysql.connector
from datetime import datetime
from Bitacora import registrar_bitacora
from conexion_core import procesar_retiro_core

def procesar_Retiro(datos):
    """
    Procesa retiro - Versión que el C# puede entender
    """
    conn = None
    cursor = None
    try:
        # Obtener datos
        numeroTarjeta = datos.get("numeroTarjeta") or datos.get("NumeroTarjeta")
        montoRetirar = datos.get("montoRetirar") or datos.get("MontoRetirar")

        # Validaciones básicas
        if not numeroTarjeta or montoRetirar is None:
            return {"ok": False, "mensaje": "Faltan datos obligatorios"}

        try:
            montoRetirar = float(montoRetirar)
        except:
            return {"ok": False, "mensaje": "Monto inválido"}

        if montoRetirar <= 0:
            return {"ok": False, "mensaje": "Monto debe ser mayor a cero"}

        # ===== 1. INTENTAR CON CORE JAVA =====
        print("🔄 Intentando retiro con Core Java...")
        resultado_core = procesar_retiro_core(numeroTarjeta, montoRetirar)

        if resultado_core.get("ok"):
            print(f"✅ Retiro aprobado por Core Java: ${montoRetirar}")

            try:
                conn = mysql.connector.connect(
                    host="localhost",
                    user="root",
                    password="70780541Yen",
                    database="cajeroautomatico",
                    autocommit=False
                )
                cursor = conn.cursor(dictionary=True)

                # Obtener datos para bitácora y actualizar
                cursor.execute("SELECT Monto_Total, Numero_Identificacion FROM tarjeta WHERE Numero_Tarjeta = %s FOR UPDATE", 
                              (numeroTarjeta,))
                tarjeta = cursor.fetchone()

                if tarjeta:
                    # Actualizar saldo local
                    saldo_actual = float(tarjeta["Monto_Total"])
                    saldo_nuevo = saldo_actual - montoRetirar
                    cursor.execute("UPDATE tarjeta SET Monto_Total = %s WHERE Numero_Tarjeta = %s", 
                                  (saldo_nuevo, numeroTarjeta))
                    conn.commit()

                    # Registrar bitácora
                    cursor.execute("SELECT IdCajero FROM Cajero LIMIT 1")
                    cajero = cursor.fetchone()
                    id_cajero = cajero["IdCajero"] if cajero else 1

                    registrar_bitacora(
                        numero_tarjeta=numeroTarjeta,
                        id_cajero=id_cajero,
                        id_cliente=tarjeta["Numero_Identificacion"],
                        tipo="Retiro",
                        monto=montoRetirar
                    )
            except Exception as e:
                print(f"⚠️ Error actualizando MySQL después de Core: {e}")
            finally:
                if cursor: cursor.close()
                if conn: conn.close()

            return {
                "ok": True,
                "mensaje": "Retiro aprobado.",
                "montoRetirar": round(montoRetirar, 2)
            }

        elif resultado_core.get("respuesta") == "INSUF":
            print("❌ Core Java: Fondos insuficientes")
            return {
                "ok": False,
                "mensaje": "Fondos insuficientes."
            }
        else:
            # Core no disponible o error
            print("⚠️ Core Java no disponible o rechazó, usando fallback local")

        # ===== 2. FALLBACK: Base de datos local =====
        print("💰 Procesando retiro con base de datos local")

        
        conn = mysql.connector.connect(
            host="localhost",
            user="root",
            password="70780541Yen",
            database="cajeroautomatico",
            autocommit=False
        )
        cursor = conn.cursor(dictionary=True)

        # Verificar tarjeta y saldo
        cursor.execute("SELECT Monto_Total, Numero_Identificacion FROM tarjeta WHERE Numero_Tarjeta = %s FOR UPDATE", 
                      (numeroTarjeta,))
        tarjeta = cursor.fetchone()

        if tarjeta is None:
            conn.rollback()
            return {"ok": False, "mensaje": "Tarjeta no encontrada"}

        saldo_actual = float(tarjeta["Monto_Total"])

        if montoRetirar > saldo_actual:
            conn.rollback()
            return {"ok": False, "mensaje": "Fondos insuficientes."}

        # Actualizar saldo
        saldo_nuevo = saldo_actual - montoRetirar
        cursor.execute("UPDATE tarjeta SET Monto_Total = %s WHERE Numero_Tarjeta = %s", 
                      (saldo_nuevo, numeroTarjeta))
        conn.commit()

        # Registrar bitácora
        cursor.execute("SELECT IdCajero FROM Cajero LIMIT 1")
        cajero = cursor.fetchone()
        id_cajero = cajero["IdCajero"] if cajero else 1

        registrar_bitacora(
            numero_tarjeta=numeroTarjeta,
            id_cajero=id_cajero,
            id_cliente=tarjeta["Numero_Identificacion"],
            tipo="Retiro",
            monto=montoRetirar
        )

        return {
            "ok": True,
            "mensaje": "Retiro aprobado.",
            "montoRetirar": round(montoRetirar, 2)
        }

    except Exception as e:
        if conn: conn.rollback()
        print(f"❌ Error en retiro: {e}")
        return {"ok": False, "mensaje": f"Error: {str(e)}"}

    finally:
        if cursor: cursor.close()
        if conn: conn.close()