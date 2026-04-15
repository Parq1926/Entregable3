# Bitacora.py

import json

import os

import threading

import queue

import mysql.connector

from datetime import datetime

 
BITACORA_JSON = "bitacora.json"

cola_bitacora = queue.Queue()
 
#Configuración del MySQL.

MYSQL_CONFIG = {

    "host": "localhost",

    "user": "root",

    "password": "70780541Yen",

    "database": "cajeroautomatico"

}
 
def enmascarar_tarjeta(numero_tarjeta: str) -> str:

    """Enmascara el número de tarjeta para mostrar solo primeros y últimos dígitos"""

    if not numero_tarjeta or len(numero_tarjeta) < 8:

        return "****"

    return f"{numero_tarjeta[:4]} {numero_tarjeta[4:6]}** **** {numero_tarjeta[-4:]}"
 
def registrar_en_mysql(numero_tarjeta, id_cajero, id_cliente, tipo, monto):

    """Registra la transacción en MySQL"""

    try:

        conn = mysql.connector.connect(**MYSQL_CONFIG)

        cursor = conn.cursor()

        sql = """INSERT INTO bitacora 

                 (numero_tarjeta, id_cajero, id_cliente, tipo, monto, fecha) 

                 VALUES (%s, %s, %s, %s, %s, %s)"""

        cursor.execute(sql, (

            numero_tarjeta, 

            id_cajero, 

            id_cliente, 

            tipo, 

            monto, 

            datetime.now()

        ))

        conn.commit()

        cursor.close()

        conn.close()

        print(f"✅ Bitácora MySQL: {tipo} registrado")

    except Exception as e:

        print(f"❌ Error registrando en MySQL: {e}")
 
def worker_bitacora():

    """Hilo en segundo plano para escribir en archivo JSON"""

    while True:

        registro = cola_bitacora.get()

        if registro is None:  #Señal para que termine.

            break

        try:

            historial = []

            if os.path.exists(BITACORA_JSON):

                with open(BITACORA_JSON, "r", encoding="utf-8") as f:

                    try:

                        historial = json.load(f)

                        if not isinstance(historial, list):

                            historial = []

                    except json.JSONDecodeError:

                        historial = []

            historial.append(registro)

            with open(BITACORA_JSON, "w", encoding="utf-8") as f:

                json.dump(historial, f, indent=4, ensure_ascii=False)

            print(f"✅ Bitácora JSON: {registro.get('tipo')} registrado")

        except Exception as e:

            print("❌ Error escribiendo bitácora JSON:", e)

        finally:

            cola_bitacora.task_done()
 
def registrar_bitacora(numero_tarjeta, id_cajero, id_cliente, tipo, monto=None):

    """

    Registra una transacción en bitácora (MySQL y JSON)

    """

    try:

        #Registrar en MySQL.

        registrar_en_mysql(numero_tarjeta, id_cajero, id_cliente, tipo, monto)

        #Registrar en JSON (en segundo plano).

        registro = {

            "fecha": datetime.now().strftime("%d/%m/%Y %H:%M:%S"),

            "fecha_iso": datetime.now().isoformat(),

            "tarjeta": enmascarar_tarjeta(str(numero_tarjeta)),

            "tarjeta_completa": numero_tarjeta,

            "cajero": id_cajero,

            "cliente": id_cliente,

            "tipo": tipo

        }

        if monto is not None:

            registro["monto"] = f"{float(monto):.2f}"

            registro["monto_numero"] = float(monto)

        cola_bitacora.put(registro)

    except Exception as e:

        print(f"❌ Error en registrar_bitacora: {e}")
 
#Iniciar el hilo worker al importar el módulo.

threading.Thread(target=worker_bitacora, daemon=True).start()
 