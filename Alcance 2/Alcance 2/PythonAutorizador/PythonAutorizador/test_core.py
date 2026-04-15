import socket
import json
from datetime import datetime

JAVA_CORE_HOST = "127.0.0.1"
JAVA_CORE_PORT = 5000

def enviar_a_core_java(trama):
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(3)
        sock.connect((JAVA_CORE_HOST, JAVA_CORE_PORT))
        sock.send((trama + "\n").encode('utf-8'))
        respuesta = sock.recv(1024).decode('utf-8').strip()
        sock.close()
        return respuesta
    except Exception as e:
        print(f"❌ Error: {e}")
        return None

# ===== PRUEBA DE CADA VALIDACIÓN =====
print("\n🔍 Probando validación de PIN (tipo 3)...")
resp = enviar_a_core_java("3 00000000-0 4111111111111111 1111")
print(f"✅ Respuesta: {resp}")

print("\n🔍 Probando validación de fecha (tipo 5)...")
resp = enviar_a_core_java("5 00000000-0 4111111111111111 2027-12-31")
print(f"✅ Respuesta: {resp}")

print("\n🔍 Probando validación de CVV (tipo 6)...")
resp = enviar_a_core_java("6 00000000-0 4111111111111111 123")
print(f"✅ Respuesta: {resp}")