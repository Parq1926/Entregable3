import sys
import json

from NumeroTarjeta import validar_NumeroTarjeta
from Pin import validar_pin
from NuevoPin import cambio_Pin


def main():
    try:
        entrada = sys.stdin.read().strip()

        if not entrada:
            print(json.dumps({
                "ok": False,
                "mensaje": "No se recibió entrada"
            }))
            return

        datos = json.loads(entrada)
        accion = datos.get("accion")

        if accion == "numero_tarjeta":
            req = {
                "numero_tarjeta": datos.get("numeroTarjeta")
            }
            resultado = validar_NumeroTarjeta(req)
            print(json.dumps(resultado))

        elif accion == "pin":
            req = {
                "NumeroTarjeta": datos.get("numeroTarjeta"),
                "Pin": datos.get("pin")
            }
            resultado = validar_pin(req)
            print(json.dumps(resultado))

        elif accion == "cambiopin":
            req = {
                "NumeroTarjeta": datos.get("numeroTarjeta"),
                "PinActual": datos.get("pinActual"),
                "NuevoPin": datos.get("pinNuevo")
            }

            resultado = cambio_Pin(req)
            print(json.dumps(resultado))

        else:
            print(json.dumps({
                "ok": False,
                "mensaje": f"Acción no soportada: {accion}"
            }))

    except Exception as e:
        print(json.dumps({
            "ok": False,
            "mensaje": f"Error en PuenteWCF.py: {str(e)}"
        }))


if __name__ == "__main__":
    main()
    main()