import socket
import json

data = {
    "accion": "consulta_cuentas",
    "Identificacion": "101010101"
}

print("Enviando:", data)

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect(("127.0.0.1", 5060))
sock.sendall((json.dumps(data) + "\n").encode())
respuesta = sock.recv(4096).decode()
print("Respuesta:", respuesta)
sock.close()