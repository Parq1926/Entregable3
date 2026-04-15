package core.Servidor;

import core.Servicio.CoreServicio;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

public class ServidorCore {
    private static final int PUERTO = 5000;

    public static void main(String[] args) {
        System.out.println("=== CORE BANCARIO INICIADO ===");

        try (ServerSocket serverSocket = new ServerSocket(PUERTO)) {
            System.out.println("✅ Core escuchando en el puerto " + PUERTO);

            while (true) {
                try (
                    Socket cliente = serverSocket.accept();
                    BufferedReader entrada = new BufferedReader(
                        new InputStreamReader(cliente.getInputStream())
                    );
                    PrintWriter salida = new PrintWriter(cliente.getOutputStream(), true)
                ) {
                    System.out.println("\n🔌 Conexión recibida");

                    String trama = entrada.readLine();
                    System.out.println("Trama recibida: " + trama);

                    CoreServicio servicio = new CoreServicio();
                    String respuesta = servicio.procesarTrama(trama);

                    salida.println(respuesta);
                    System.out.println("Respuesta enviada: " + respuesta);

                } catch (Exception e) {
                    System.out.println("❌ Error atendiendo cliente");
                    e.printStackTrace();
                }
            }

        } catch (Exception e) {
            System.out.println("❌ Error en el servidor del Core");
            e.printStackTrace();
        }
    }
}