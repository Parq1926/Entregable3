import socket

JAVA_CORE_HOST = "127.0.0.1"
JAVA_CORE_PORT = 5000


def enviar_a_core_java(trama: str):
    """
    Envía una trama al Core Java por socket.
    Retorna la respuesta como string o None si falla.
    """
    sock = None
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(5)
        sock.connect((JAVA_CORE_HOST, JAVA_CORE_PORT))
        sock.sendall((trama + "\n").encode("utf-8"))

        respuesta = sock.recv(1024).decode("utf-8").strip()
        return respuesta

    except Exception as e:
        print(f"❌ Error enviando al Core Java: {e}")
        return None

    finally:
        if sock:
            try:
                sock.close()
            except:
                pass


def procesar_consulta_core(numero_tarjeta: str) -> dict:
    trama = f"2 00000000-0 {numero_tarjeta} 00000000"
    respuesta = enviar_a_core_java(trama)

    if not respuesta:
        return {"ok": False, "mensaje": "Core no disponible"}

    if respuesta.startswith("OK"):
        try:
            saldo_str = respuesta[2:]
            saldo = float(saldo_str) / 100
            return {
                "ok": True,
                "saldo": saldo,
                "respuesta": respuesta
            }
        except Exception as e:
            return {
                "ok": False,
                "mensaje": f"Respuesta inválida del Core: {respuesta}",
                "error": str(e)
            }

    if respuesta == "ERROR":
        return {"ok": False, "mensaje": "Tarjeta no válida", "respuesta": respuesta}

    return {"ok": False, "mensaje": f"Respuesta inesperada: {respuesta}", "respuesta": respuesta}


def procesar_retiro_core(numero_tarjeta: str, monto: float) -> dict:
    try:
        monto_centavos = int(round(float(monto) * 100))
        monto_formateado = str(monto_centavos).zfill(8)
    except Exception as e:
        return {"ok": False, "mensaje": "Monto inválido", "error": str(e)}

    trama = f"1 00000000-0 {numero_tarjeta} {monto_formateado}"
    respuesta = enviar_a_core_java(trama)

    if not respuesta:
        return {"ok": False, "mensaje": "Core no disponible"}

    if respuesta == "OK":
        return {"ok": True, "respuesta": respuesta}

    if respuesta == "INSUF":
        return {"ok": False, "respuesta": respuesta, "mensaje": "Fondos insuficientes"}

    if respuesta == "ERROR":
        return {"ok": False, "respuesta": respuesta, "mensaje": "Retiro rechazado"}

    return {"ok": False, "respuesta": respuesta, "mensaje": f"Respuesta inesperada: {respuesta}"}


def validar_pin_core(numero_tarjeta: str, pin: str) -> bool:
    trama = f"3 00000000-0 {numero_tarjeta} {pin}"
    respuesta = enviar_a_core_java(trama)
    return respuesta == "OK"


def validar_fecha_core(numero_tarjeta: str, fecha: str) -> bool:
    trama = f"5 00000000-0 {numero_tarjeta} {fecha}"
    respuesta = enviar_a_core_java(trama)
    return respuesta == "OK"


def validar_cvv_core(numero_tarjeta: str, cvv: str) -> bool:
    trama = f"6 00000000-0 {numero_tarjeta} {cvv}"
    respuesta = enviar_a_core_java(trama)
    return respuesta == "OK"


def cambiar_pin_core(numero_tarjeta: str, pin_nuevo: str) -> bool:
    trama = f"4 00000000-0 {numero_tarjeta} {pin_nuevo}"
    respuesta = enviar_a_core_java(trama)
    return respuesta == "OK"