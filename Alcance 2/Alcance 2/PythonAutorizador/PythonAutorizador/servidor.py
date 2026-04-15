# -*- coding: utf-8 -*-

import socket
import json
import threading
import mysql.connector
import base64
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from datetime import datetime
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad

# =====================
# CONFIGURACIÓN DE CORREO (cambia estos valores)
# =====================

EMAIL_REMITENTE = "rojaspamela60@gmail.com"
EMAIL_PASSWORD = "bdui xlpx khfy ttga"  # Usar contraseña de aplicación de Gmail
EMAIL_DESTINATARIO = "rojaspamela60@gmail.com"

def enviar_correo_login(usuario):
    """Envía correo cuando alguien inicia sesión"""
    enviar_correo(
        asunto="🔐 Inicio de Sesión Exitoso",
        mensaje=f"Hola {usuario},\n\nHas iniciado sesión en el sistema bancario.\n\n"
                f"📅 Fecha: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n"
                f"💻 IP: 127.0.0.1\n\n"
                f"Si no fuiste tú, contacta al soporte inmediatamente."
    )

def enviar_correo_login_accion(datos):
    """Envía correo de login desde la web"""
    usuario = datos.get("Usuario")
    print(f"📧 Enviando correo de login a: {usuario}")
    enviar_correo_login(usuario)
    return {"ok": True}

def enviar_correo(asunto, mensaje, destinatario=None):
    """Envía un correo electrónico"""
    if destinatario is None:
        destinatario = EMAIL_DESTINATARIO
    
    try:
        msg = MIMEMultipart()
        msg['From'] = EMAIL_REMITENTE
        msg['To'] = destinatario
        msg['Subject'] = asunto
        
        msg.attach(MIMEText(mensaje, 'plain', 'utf-8'))
        
        server = smtplib.SMTP('smtp.gmail.com', 587)
        server.starttls()
        server.login(EMAIL_REMITENTE, EMAIL_PASSWORD)
        server.send_message(msg)
        server.quit()
        
        print(f"✅ Correo enviado: {asunto}")
        return True
    except Exception as e:
        print(f"❌ Error enviando correo: {e}")
        return False

# =====================
# CONFIGURACIÓN DE AES
# =====================

AES_KEY = b"12345678901234561234567890123456"
AES_IV = b"1234567890123456"

def cifrar_aes(texto):
    if texto is None:
        return None
    cipher = AES.new(AES_KEY, AES.MODE_CBC, AES_IV)
    texto_bytes = texto.encode('utf-8')
    padded = pad(texto_bytes, AES.block_size)
    cifrado = cipher.encrypt(padded)
    return base64.b64encode(cifrado).decode('utf-8')

def descifrar_aes(texto_cifrado):
    if texto_cifrado is None:
        return None
    try:
        cifrado = base64.b64decode(texto_cifrado)
        cipher = AES.new(AES_KEY, AES.MODE_CBC, AES_IV)
        descifrado = cipher.decrypt(cifrado)
        return unpad(descifrado, AES.block_size).decode('utf-8')
    except Exception as e:
        print(f"❌ Error descifrando: {e}")
        return texto_cifrado

def decrypt_if_needed(value):
    if value is None:
        return None
    try:
        return descifrar_aes(value)
    except:
        return value

# =====================
# CONEXION CORE JAVA
# =====================

JAVA_CORE_HOST = "127.0.0.1"
JAVA_CORE_PORT = 5000

def enviar_a_core_java(trama):
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        print("🔌 Conectando con CORE JAVA...")
        print("📤 Trama enviada:", trama)
        sock.connect((JAVA_CORE_HOST, JAVA_CORE_PORT))
        sock.sendall((trama + "\n").encode())
        respuesta = sock.recv(4096).decode().strip()
        sock.close()
        print("📥 Respuesta CORE:", respuesta)
        return respuesta
    except Exception as e:
        print("❌ Error CORE:", e)
        return None

def enviar_a_core_java_cifrado(numero_tarjeta, monto, numero_cuenta="00000000-0"):
    try:
        tarjeta_cifrada = cifrar_aes(numero_tarjeta)
        monto_cifrado = cifrar_aes(str(int(monto * 100)))
        trama = f"2|{numero_cuenta}|{tarjeta_cifrada}|{monto_cifrado}"
        respuesta = enviar_a_core_java(trama)
        if respuesta and respuesta.startswith("OK"):
            saldo_cifrado = respuesta[2:]
            saldo = float(descifrar_aes(saldo_cifrado)) / 100
            return {"ok": True, "saldo": saldo}
        return {"ok": False, "mensaje": respuesta}
    except Exception as e:
        print(f"❌ Error consulta core cifrada: {e}")
        return {"ok": False, "mensaje": str(e)}

# ===================
# FUNCIONES DEL CORE
# ===================

def procesar_retiro_core(numero_tarjeta, monto, numero_cuenta="00000000-0"):
    try:
        tarjeta_cifrada = cifrar_aes(numero_tarjeta)
        monto_cifrado = cifrar_aes(str(int(monto * 100)))
        trama = f"1|{numero_cuenta}|{tarjeta_cifrada}|{monto_cifrado}"
        print(f"📤 Enviando retiro a Java: {trama}")
        
        respuesta = enviar_a_core_java(trama)
        print(f"📥 Respuesta retiro Java (cifrada): {respuesta}")
        
        if respuesta is None:
            return {"ok": False, "mensaje": "No hay respuesta de Java"}
        
        if respuesta.startswith("ERROR"):
            return {"ok": False, "mensaje": respuesta}
        
        # Descifrar respuesta
        try:
            respuesta_descifrada = descifrar_aes(respuesta)
            print(f"🔓 Respuesta retiro descifrada: {respuesta_descifrada}")
            
            if respuesta_descifrada == "OK":
                return {"ok": True}
            elif respuesta_descifrada == "INSUF":
                return {"ok": False, "mensaje": "Fondos insuficientes"}
            else:
                return {"ok": False, "mensaje": respuesta_descifrada}
        except Exception as e:
            print(f"❌ Error descifrando respuesta retiro: {e}")
            return {"ok": False, "mensaje": str(e)}
            
    except Exception as e:
        print(f"❌ Error retiro core: {e}")
        return {"ok": False, "mensaje": str(e)}

def procesar_consulta_core(numero_tarjeta, numero_cuenta="00000000-0"):
    try:
        tarjeta_cifrada = cifrar_aes(numero_tarjeta)
        monto_cifrado = cifrar_aes("0")
        trama = f"2|{numero_cuenta}|{tarjeta_cifrada}|{monto_cifrado}"
        print(f"📤 Enviando a Java: {trama}")
        
        respuesta = enviar_a_core_java(trama)
        print(f"📥 Respuesta de Java (cifrada): {respuesta}")
        
        if respuesta is None:
            return {"ok": False, "mensaje": "No hay respuesta de Java"}
        
        if respuesta.startswith("ERROR"):
            return {"ok": False, "mensaje": respuesta}
        
        # DESCIFRAR la respuesta de Java
        try:
            respuesta_descifrada = descifrar_aes(respuesta)
            print(f"🔓 Respuesta descifrada: {respuesta_descifrada}")
            
            if respuesta_descifrada and respuesta_descifrada.startswith("OK"):
                saldo_str = respuesta_descifrada[2:]
                saldo = float(saldo_str) / 100
                print(f"💰 Saldo descifrado: {saldo}")
                return {"ok": True, "saldo": saldo}
            else:
                print(f"❌ Respuesta no es OK: {respuesta_descifrada}")
                return {"ok": False, "mensaje": respuesta_descifrada}
        except Exception as e:
            print(f"❌ Error descifrando respuesta: {e}")
            return {"ok": False, "mensaje": f"Error descifrando: {e}"}
            
    except Exception as e:
        print(f"❌ Error consulta core: {e}")
        return {"ok": False, "mensaje": str(e)}

def procesar_cambio_pin_core(numero_tarjeta, pin_nuevo, numero_cuenta="00000000-0"):
    try:
        pin_cifrado = cifrar_aes(pin_nuevo)
        tarjeta_cifrada = cifrar_aes(numero_tarjeta)
        trama = f"4|{numero_cuenta}|{tarjeta_cifrada}|{pin_cifrado}"
        respuesta = enviar_a_core_java(trama)
        if respuesta == "OK":
            return {"ok": True}
        return {"ok": False, "mensaje": respuesta}
    except Exception as e:
        print("❌ Error cambio pin core:", e)
        return {"ok": False, "mensaje": str(e)}

# ===================
# FUNCIONES DE MySQL
# ===================

def conectar_db():
    return mysql.connector.connect(
        host="localhost",
        user="root",
        password="Pame.1998",
        database="cajeroautomatico"
    )

def obtener_tipo_tarjeta(numero_tarjeta):
    try:
        conn = conectar_db()
        cursor = conn.cursor(dictionary=True)
        numero_cifrado = cifrar_aes(numero_tarjeta)
        cursor.execute("""
            SELECT tt.Tipo_Tarjeta 
            FROM Tarjeta t
            JOIN Tipo_Trajeta tt ON t.IDTipo_Tarjeta = tt.IDTipo_Tarjeta
            WHERE t.Numero_Tarjeta_Cifrado = %s OR t.Numero_Tarjeta = %s
        """, (numero_cifrado, numero_tarjeta))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        if result:
            return result["Tipo_Tarjeta"].lower()
        return None
    except Exception as e:
        print(f"❌ Error obteniendo tipo de tarjeta: {e}")
        return None

def validar_tarjeta_cifrada(numero_tarjeta, cvv, fecha_vencimiento):
    try:
        conn = conectar_db()
        cursor = conn.cursor(dictionary=True)
        numero_cifrado = cifrar_aes(numero_tarjeta)
        cvv_cifrado = cifrar_aes(cvv)
        cursor.execute("""
            SELECT Numero_Tarjeta_Cifrado, CVV_Cifrado, Fecha_Vencimiento 
            FROM Tarjeta 
            WHERE Numero_Tarjeta_Cifrado = %s OR Numero_Tarjeta = %s
        """, (numero_cifrado, numero_tarjeta))
        tarjeta = cursor.fetchone()
        cursor.close()
        conn.close()
        if not tarjeta:
            return {"ok": False, "mensaje": "Tarjeta no encontrada"}
        cvv_bd = tarjeta.get("CVV_Cifrado")
        if cvv_bd:
            cvv_descifrado = descifrar_aes(cvv_bd)
            if cvv_descifrado != cvv:
                return {"ok": False, "mensaje": "CVV incorrecto"}
        if tarjeta["Fecha_Vencimiento"].strftime("%m/%Y") != fecha_vencimiento:
            return {"ok": False, "mensaje": "Fecha de vencimiento incorrecta"}
        return {"ok": True, "mensaje": "Tarjeta válida"}
    except Exception as e:
        print(f"❌ Error validando tarjeta: {e}")
        return {"ok": False, "mensaje": str(e)}

def consultar_saldo_credito(numero_tarjeta):
    try:
        conn = conectar_db()
        cursor = conn.cursor(dictionary=True)
        numero_cifrado = cifrar_aes(numero_tarjeta)
        cursor.execute("""
            SELECT Monto_Total FROM Tarjeta 
            WHERE Numero_Tarjeta_Cifrado = %s OR Numero_Tarjeta = %s
        """, (numero_cifrado, numero_tarjeta))
        tarjeta = cursor.fetchone()
        cursor.close()
        conn.close()
        if tarjeta:
            return {"ok": True, "saldo": float(tarjeta["Monto_Total"])}
        return {"ok": False, "mensaje": "Tarjeta no encontrada"}
    except Exception as e:
        return {"ok": False, "mensaje": str(e)}

def procesar_retiro_credito(numero_tarjeta, monto):
    try:
        conn = conectar_db()
        cursor = conn.cursor()
        numero_cifrado = cifrar_aes(numero_tarjeta)
        cursor.execute("""
            SELECT Monto_Total FROM Tarjeta 
            WHERE Numero_Tarjeta_Cifrado = %s OR Numero_Tarjeta = %s
        """, (numero_cifrado, numero_tarjeta))
        resultado = cursor.fetchone()
        if not resultado:
            return {"ok": False, "mensaje": "Tarjeta no encontrada"}
        saldo_actual = resultado[0]
        if saldo_actual < monto:
            return {"ok": False, "mensaje": "Fondos insuficientes"}
        cursor.execute("""
            UPDATE Tarjeta SET Monto_Total = Monto_Total - %s 
            WHERE Numero_Tarjeta_Cifrado = %s OR Numero_Tarjeta = %s
        """, (monto, numero_cifrado, numero_tarjeta))
        conn.commit()
        cursor.close()
        conn.close()
        return {"ok": True}
    except Exception as e:
        return {"ok": False, "mensaje": str(e)}

# ============================================
# FUNCIONES PARA OBTENER CUENTAS Y TARJETAS
# ============================================

def obtener_tarjetas_por_cedula(identificacion):
    tarjetas = []
    print(f"🔍 Buscando tarjetas para cédula: '{identificacion}'")
    try:
        conn = conectar_db()
        cursor = conn.cursor(dictionary=True)
        cursor.execute("SELECT COUNT(*) as total FROM Tarjeta WHERE Numero_Identificacion = %s", (identificacion,))
        total = cursor.fetchone()
        print(f"📊 Total de tarjetas encontradas en MySQL: {total['total']}")
        cursor.execute("""
            SELECT t.Numero_Tarjeta, t.Monto_Total, tt.Tipo_Tarjeta, t.Numero_Identificacion
            FROM Tarjeta t
            JOIN Tipo_Trajeta tt ON t.IDTipo_Tarjeta = tt.IDTipo_Tarjeta
            WHERE t.Numero_Identificacion = %s
        """, (identificacion,))
        tarjetas_db = cursor.fetchall()
        cursor.close()
        conn.close()
        print(f"📋 Tarjetas encontradas: {len(tarjetas_db)}")
        for t in tarjetas_db:
            numero = t["Numero_Tarjeta"]
            tipo = t["Tipo_Tarjeta"]
            print(f"   - Tarjeta: {numero}, Tipo: {tipo}, Saldo: {t['Monto_Total']}")
            if tipo == "Credito":
                tarjetas.append({
                    "Numero_Tarjeta": numero,
                    "Monto_Total": float(t["Monto_Total"]),
                    "Tipo_Tarjeta": tipo,
                    "Numero_Identificacion": identificacion
                })
            elif tipo == "Debito":
                print(f"   🔍 Consultando Java para débito: {numero}")
                resultado = procesar_consulta_core(numero)
                if resultado.get("ok"):
                    tarjetas.append({
                        "Numero_Tarjeta": numero,
                        "Monto_Total": resultado["saldo"],
                        "Tipo_Tarjeta": tipo,
                        "Numero_Identificacion": identificacion
                    })
    except Exception as e:
        print(f"❌ Error: {e}")
        import traceback
        traceback.print_exc()
    print(f"📤 Total tarjetas a devolver: {len(tarjetas)}")
    return tarjetas

def consulta_cuentas_por_cedula(datos):
    try:
        identificacion = datos.get("Identificacion")
        print(f"📋 Buscando tarjetas para cédula: {identificacion}")
        tarjetas = obtener_tarjetas_por_cedula(identificacion)
        cuentas = []
        for t in tarjetas:
            cuentas.append({
                "numero_tarjeta": t["Numero_Tarjeta"],
                "saldo": float(t["Monto_Total"]),
                "tipo": t["Tipo_Tarjeta"]
            })
        print("📧 Enviando correo de bienvenida...")
        enviar_correo(
            asunto="🔐 Inicio de Sesión Exitoso",
            mensaje=f"Bienvenido al sistema bancario.\n\nFecha: {datetime.now()}"
    )
        return {"ok": True, "cuentas": cuentas}
    except Exception as e:
        return {"ok": False, "mensaje": str(e)}



def obtener_movimientos_cuenta(datos):
    try:
        numero_tarjeta = datos.get("NumeroCuenta")
        print(f"📋 Buscando movimientos para tarjeta: {numero_tarjeta}")
        conn = conectar_db()
        cursor = conn.cursor(dictionary=True)
        cursor.execute("""
            SELECT Fecha_Bitacora as fecha, Comercio as descripcion, Monto_Transaccion as monto
            FROM Bitacora 
            WHERE Numero_Tarjeta = %s AND Monto_Transaccion IS NOT NULL
            ORDER BY Fecha_Bitacora DESC
            LIMIT 20
        """, (numero_tarjeta,))
        movimientos = cursor.fetchall()
        cursor.close()
        conn.close()
        resultado = []
        for m in movimientos:
            resultado.append({
                "fecha": m["fecha"].strftime("%Y-%m-%d %H:%M:%S") if m["fecha"] else "",
                "descripcion": m["descripcion"] if m["descripcion"] else "Transacción",
                "monto": float(m["monto"]) if m["monto"] else 0
            })
        return {"ok": True, "movimientos": resultado}
    except Exception as e:
        return {"ok": True, "movimientos": []}

def obtener_movimientos_tarjeta_credito(datos):
    try:
        numero_tarjeta = datos.get("NumeroTarjeta")
        print(f"📋 Buscando movimientos para tarjeta de crédito: {numero_tarjeta}")
        conn = conectar_db()
        cursor = conn.cursor(dictionary=True)
        cursor.execute("""
            SELECT Fecha_Bitacora as fecha, Comercio as comercio, Monto_Transaccion as monto
            FROM Bitacora 
            WHERE Numero_Tarjeta = %s AND Monto_Transaccion IS NOT NULL
            ORDER BY Fecha_Bitacora DESC
            LIMIT 20
        """, (numero_tarjeta,))
        movimientos = cursor.fetchall()
        cursor.close()
        conn.close()
        resultado = []
        for m in movimientos:
            resultado.append({
                "fecha": m["fecha"].strftime("%Y-%m-%d %H:%M:%S") if m["fecha"] else "",
                "codigo": f"AUTH-{m['fecha'].strftime('%Y%m%d%H%M%S') if m['fecha'] else '000000'}",
                "comercio": m["comercio"] if m["comercio"] else "Comercio",
                "monto": float(m["monto"]) if m["monto"] else 0
            })
        return {"ok": True, "movimientos": resultado}
    except Exception as e:
        return {"ok": True, "movimientos": []}

def registrar_movimiento(numero_tarjeta, monto, comercio, tipo_transaccion, exitoso):
    try:
        conn = conectar_db()
        cursor = conn.cursor()
        cursor.execute("SELECT Numero_Identificacion FROM Tarjeta WHERE Numero_Tarjeta = %s", (numero_tarjeta,))
        result = cursor.fetchone()
        identificacion = result[0] if result else "DESCONOCIDO"
        cursor.execute("""
            INSERT INTO Bitacora (Fecha_Bitacora, Numero_Tarjeta, Numero_Identificacion, 
                                  Monto_Transaccion, Comercio, Exitoso)
            VALUES (NOW(), %s, %s, %s, %s, %s)
        """, (numero_tarjeta, identificacion, monto, comercio, 1 if exitoso else 0))
        conn.commit()
        cursor.close()
        conn.close()
        print(f"✅ Movimiento registrado")
    except Exception as e:
        print(f"❌ Error registrando movimiento: {e}")

def procesar_compra(datos):
    try:
        numero_tarjeta = datos.get("NumeroTarjeta")
        monto = float(datos.get("Monto"))
        comercio = datos.get("Comercio")
        cvv = datos.get("CVV")
        fecha_vencimiento = datos.get("FechaVencimiento")
        pin = datos.get("PIN", "")
        nombre_cliente = datos.get("NombreCliente", "")
        
        print(f"🛒 COMPRA RECIBIDA: {numero_tarjeta} - ₡{monto} - {comercio}")
        print(f"   Cliente: {nombre_cliente}")
        
        validacion = validar_tarjeta_cifrada(numero_tarjeta, cvv, fecha_vencimiento)
        if not validacion.get("ok"):
            return {"ok": False, "mensaje": validacion.get("mensaje")}
        
        if monto > 50000 and (pin is None or pin.strip() == ""):
            return {"ok": False, "mensaje": "PIN requerido para montos mayores a ₡50,000"}
        
        tipo = obtener_tipo_tarjeta(numero_tarjeta)
        print(f"📋 Tipo de tarjeta: {tipo}")
        
        if tipo == "credito":
            resultado = procesar_retiro_credito(numero_tarjeta, monto)
            if resultado.get("ok"):
                registrar_movimiento(numero_tarjeta, monto, comercio, "COMPRA", True)
                # Enviar correo de confirmación de compra
                enviar_correo(
                    asunto="💳 Compra Realizada Exitosamente",
                    mensaje=f"Estimado cliente,\n\nSe ha realizado una compra con su tarjeta:\n\n"
                            f"💰 Monto: ₡{monto:,.2f}\n"
                            f"🏪 Comercio: {comercio}\n"
                            f"🕒 Fecha: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n"
                            f"💳 Tarjeta: ****{numero_tarjeta[-4:]}\n\n"
                            f"Saldo restante: ₡{consultar_saldo_credito(numero_tarjeta)['saldo']:,.2f}\n\n"
                            f"Si no reconoce esta transacción, contacte al banco inmediatamente."
                )
            return resultado
        
        elif tipo == "debito":
            resultado = procesar_retiro_core(numero_tarjeta, monto)
            if resultado.get("ok"):
                registrar_movimiento(numero_tarjeta, monto, comercio, "COMPRA", True)
                # Enviar correo de confirmación de compra
                enviar_correo(
                    asunto="💳 Compra Realizada Exitosamente",
                    mensaje=f"Estimado cliente,\n\nSe ha realizado una compra con su tarjeta de débito:\n\n"
                            f"💰 Monto: ₡{monto:,.2f}\n"
                            f"🏪 Comercio: {comercio}\n"
                            f"🕒 Fecha: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n"
                            f"💳 Tarjeta: ****{numero_tarjeta[-4:]}\n\n"
                            f"Si no reconoce esta transacción, contacte al banco inmediatamente."
                )
                try:
                    conn = conectar_db()
                    cursor = conn.cursor()
                    numero_cifrado = cifrar_aes(numero_tarjeta)
                    cursor.execute("""
                        UPDATE Tarjeta SET Monto_Total = Monto_Total - %s 
                        WHERE Numero_Tarjeta_Cifrado = %s OR Numero_Tarjeta = %s
                    """, (monto, numero_cifrado, numero_tarjeta))
                    conn.commit()
                    cursor.close()
                    conn.close()
                except Exception as e:
                    print(f"⚠️ Error actualizando MySQL: {e}")
            return resultado
        
        else:
            return {"ok": False, "mensaje": f"Tipo no identificado: {tipo}"}
    except Exception as e:
        return {"ok": False, "mensaje": str(e)}

def obtener_cedula_por_usuario(datos):
    try:
        usuario = datos.get("Usuario")
        print(f"📋 Buscando cédula para usuario: {usuario}")
        conn = conectar_db()
        cursor = conn.cursor(dictionary=True)
        cursor.execute("SELECT Numero_Identificacion FROM Cliente WHERE Numero_Identificacion = %s", (usuario,))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        if result:
            return {"ok": True, "cedula": result["Numero_Identificacion"]}
        return {"ok": False, "mensaje": "Usuario no encontrado"}
    except Exception as e:
        return {"ok": False, "mensaje": str(e)}

# =========
# PROCESAR SOLICITUD
# =========

def procesar_solicitud(data):
    accion = data.get("accion", "").lower()
    print(f"📋 Acción recibida: {accion}")
    
    if accion == "consulta":
        return consulta_cuenta(data)
    elif accion == "consulta_cuentas":
        return consulta_cuentas_por_cedula(data)
    elif accion == "movimientos_cuenta":
        return obtener_movimientos_cuenta(data)
    elif accion == "movimientos_tarjeta_credito":
        return obtener_movimientos_tarjeta_credito(data)
    elif accion == "compra":
        return procesar_compra(data)
    elif accion == "retiro":
        return procesar_Retiro(data)
    elif accion == "cambio_pin":
        return cambio_Pin(data)
    elif accion == "obtener_cedula":
        return obtener_cedula_por_usuario(data)
    elif accion == "enviar_correo_login":
        return enviar_correo_login_accion(data)
    
    return {"ok": False, "mensaje": "Acción no reconocida"}

# ===============
# SOCKET SERVER
# ===============

HOST = "127.0.0.1"
PORT = 5060

def manejar_cliente(conn, addr):
    print("🔌 Conexion desde", addr)
    try:
        data = conn.recv(4096).decode().strip()
        data = data.replace('\ufeff', '')
        print("📥 JSON recibido:", data)
        solicitud = json.loads(data)
        respuesta = procesar_solicitud(solicitud)
        respuesta_json = json.dumps(respuesta)
        print("📤 JSON respuesta:", respuesta_json)
        conn.sendall((respuesta_json + "\n").encode())
    except Exception as e:
        print("❌ Error:", e)
        conn.sendall(json.dumps({"ok": False, "mensaje": str(e)}).encode())
    finally:
        conn.close()

def main():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server.bind((HOST, PORT))
    server.listen(5)
    print("🚀 AUTORIZADOR PYTHON INICIADO")
    print("📡 Escuchando en", HOST, PORT)
    
    while True:
        conn, addr = server.accept()
        thread = threading.Thread(target=manejar_cliente, args=(conn, addr))
        thread.start()

if __name__ == "__main__":
    main()