import socket

# Datos de prueba
data = '{"accion":"consulta_cuentas","Identificacion":"123456789"}'

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect(("127.0.0.1", 5060))
sock.sendall(data.encode())
respuesta = sock.recv(4096).decode()
print("Respuesta:", respuesta)
sock.close()