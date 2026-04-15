import socket
import json

data = {
    "accion": "compra",
    "NumeroTarjeta": "4111111111111111",
    "Monto": 10000,
    "Comercio": "Tienda Test",
    "CVV": "123",
    "FechaVencimiento": "12/2028",
    "PIN": ""
}

print("Enviando compra:", data)

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect(("127.0.0.1", 5060))
sock.sendall((json.dumps(data) + "\n").encode())
respuesta = sock.recv(4096).decode()
print("Respuesta:", respuesta)
sock.close()