import sys
 
def procesar_datos(parametro):
    return f"resultado de python: {parametro}"
 
if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Uso: py main.py <parametro>")
        sys.exit(1)
 
    parametro = sys.argv[1]
    print(procesar_datos(parametro))
 