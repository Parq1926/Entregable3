import socket
import time

JAVA_CORE_HOST = "127.0.0.1"
JAVA_CORE_PORT = 5000

def enviar_a_core_java(trama):
    try:
        print(f"📤 Conectando a Java en {JAVA_CORE_HOST}:{JAVA_CORE_PORT}...")
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(5)
        sock.connect((JAVA_CORE_HOST, JAVA_CORE_PORT))
        print(f"📤 Enviando: {trama}")
        sock.send((trama + "\n").encode('utf-8'))
        respuesta = sock.recv(1024).decode('utf-8').strip()
        sock.close()
        print(f"📥 Respuesta: {respuesta}")
        return respuesta
    except Exception as e:
        print(f"❌ Error: {e}")
        return None

print("=" * 50)
print("🔍 INICIANDO PRUEBAS DE VALIDACIONES CONTRA JAVA")
print("=" * 50)

# Prueba 1: Validar PIN (tipo 3)
print("\n🔍 Probando validación de PIN (tipo 3)...")
time.sleep(1)
resp = enviar_a_core_java("3 00000000-0 4111111111111111 1111")

# Prueba 2: Validar fecha (tipo 5)
print("\n🔍 Probando validación de fecha (tipo 5)...")
time.sleep(1)
resp = enviar_a_core_java("5 00000000-0 4111111111111111 2027-12-31")

# Prueba 3: Validar CVV (tipo 6)
print("\n🔍 Probando validación de CVV (tipo 6)...")
time.sleep(1)
resp = enviar_a_core_java("6 00000000-0 4111111111111111 123")

print("\n" + "=" * 50)
print("✅ PRUEBAS FINALIZADAS")
print("=" * 50)