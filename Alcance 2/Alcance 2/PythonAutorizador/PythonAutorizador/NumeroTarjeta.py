# NumeroTarjeta.py

import mysql.connector
import socket
import json

def obtener_conexion():
    
    return mysql.connector.connect(
        host="localhost",
        user="root",
        password="70780541Yen",
        database="cajeroautomatico"
    )

def validar_tarjeta_con_core(numero_tarjeta):
    """
    Intenta validar la tarjeta con el Core Java
    Retorna: (resultado, usa_fallback)
    """
    try:
        print(f"🔄 Intentando validar tarjeta {numero_tarjeta} con Core Java...")
        
        # Crear socket
        core_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        core_socket.settimeout(3) 
        
        # Conectar al Core (puerto 5000)
        core_socket.connect(('127.0.0.1', 5000))
        print("✅ Conectado al Core Java")
        
        # "tipoTransaccion numeroCuenta numeroTarjeta monto"
        numero_cuenta_dummy = "00000000-0"
        monto = "00000000"
        
        trama = f"2 {numero_cuenta_dummy} {numero_tarjeta} {monto}"
        print(f"📤 Enviando trama: {trama}")
        
        # Enviar con \n al final
        core_socket.send((trama + "\n").encode('utf-8'))
        
        # Recibir respuesta
        respuesta = core_socket.recv(1024).decode('utf-8').strip()
        print(f"📥 Respuesta del Core: {respuesta}")
        
        core_socket.close()
        
        # Procesar respuesta
        if respuesta.startswith("OK"):
            return {"ok": True, "mensaje": "Tarjeta válida según Core"}, False
        elif respuesta == "ERROR":
            return {"ok": False, "errores": ["Tarjeta no válida según Core"]}, False
        else:
            return {"ok": False, "errores": [f"Respuesta inesperada: {respuesta}"]}, False
            
    except socket.timeout:
        print("⏰ Timeout: Core no respondió")
        return None, True  # Usar fallback
    except ConnectionRefusedError:
        print("🔌 Core no disponible (conexión rechazada)")
        return None, True  # Usar fallback
    except Exception as e:
        print(f"❌ Error conectando al Core: {e}")
        return None, True  # Usar fallback

def validar_tarjeta_local(numero_tarjeta):
    """Valida la tarjeta usando MySQL local"""
    try:
        print(f"🔍 Validando tarjeta localmente: {numero_tarjeta}")
        
        conexion = obtener_conexion()
        cursor = conexion.cursor(dictionary=True)
        
        sql = """
        SELECT Numero_Tarjeta, Estado
        FROM tarjeta
        WHERE Numero_Tarjeta = %s
        """
        cursor.execute(sql, (numero_tarjeta,))
        resultado = cursor.fetchone()
        
        cursor.close()
        conexion.close()
        
        if resultado:
            estado_original = resultado.get('Estado')
            estado = str(estado_original).strip().lower()
            
            if estado == "activa":
                return {"ok": True, "mensaje": "Tarjeta válida"}
            else:
                return {"ok": False, "errores": [f"Tarjeta inactiva. Estado actual: {estado_original}"]}
        else:
            return {"ok": False, "errores": ["Tarjeta no existe"]}
            
    except Exception as e:
        print(f"❌ Error en validación local: {e}")
        return {"ok": False, "errores": [f"Error en base de datos local: {str(e)}"]}


def validar_NumeroTarjeta(data):
    """
    Valida si el número de tarjeta existe
    Primero intenta con Core Java, si no está disponible usa MySQL local
    """
    try:
        numero_tarjeta = data.get("numero_tarjeta")
        
        if not numero_tarjeta:
            return {"ok": False, "errores": ["Número de tarjeta no proporcionado"]}
        
        print(f"\n🔍 Iniciando validación para tarjeta: {numero_tarjeta}")
        
        # Intentar con Core Java
        resultado_core, usar_fallback = validar_tarjeta_con_core(numero_tarjeta)
        
        if not usar_fallback and resultado_core is not None:
            print("✅ Usando respuesta del Core")
            return resultado_core
        else:
            print("⚠️ Core no disponible, usando validación local")
            return validar_tarjeta_local(numero_tarjeta)
            
    except Exception as e:
        print(f"❌ Error general: {e}")
        return {"ok": False, "errores": [f"Error: {str(e)}"]}