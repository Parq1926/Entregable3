package core.Bitacora;

import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public class BitacoraCore {
    private static final String ARCHIVO = "bitacoraCore.json";

    public static void registrar(
            String numeroTarjeta,
            String tipoOperacion,
            Double monto,
            String resultado
    ) {
        String fecha = LocalDateTime.now().format(DateTimeFormatter.ofPattern("dd/MM/yyyy"));
        String hora = LocalDateTime.now().format(DateTimeFormatter.ofPattern("HH:mm:ss"));

        String montoStr = monto != null ? String.format("%.2f", monto) : "null";
        String tarjetaEnmascarada = enmascararTarjeta(numeroTarjeta);

        String registroJson = String.format(
            "%s: {\"tarjeta\": \"%s\", \"tipo\": \"%s\", \"monto\": %s, \"resultado\": \"%s\", \"hora\": \"%s\"}",
            fecha,
            tarjetaEnmascarada,
            tipoOperacion,
            montoStr,
            resultado,
            hora
        );

        try (FileWriter fw = new FileWriter(ARCHIVO, true);
             PrintWriter pw = new PrintWriter(fw)) {
            pw.println(registroJson);
        } catch (IOException e) {
            System.out.println("❌ Error escribiendo bitácora: " + e.getMessage());
        }
    }

    private static String enmascararTarjeta(String tarjeta) {
        if (tarjeta == null || tarjeta.length() < 16) {
            return "**** **** **** ****";
        }
        String limpia = tarjeta.replaceAll("\\s", "");
        if (limpia.length() >= 16) {
            return limpia.substring(0, 4) + " " +
                   limpia.substring(4, 6) + "** **** " +
                   limpia.substring(12, 16);
        }
        return tarjeta;
    }
}