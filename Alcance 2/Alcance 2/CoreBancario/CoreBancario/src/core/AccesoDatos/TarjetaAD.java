package core.AccesoDatos;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import core.util.ConexionSQL;

public class TarjetaAD {

    public static boolean tarjetaExiste(String numeroTarjeta) {
        String sql = "SELECT 1 FROM Tarjeta WHERE Numero_Tarjeta = ? AND LOWER(LTRIM(RTRIM(Estado))) = 'activa'";

        try (Connection conn = ConexionSQL.obtenerConexion();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, numeroTarjeta.replaceAll("\\s", ""));

            try (ResultSet rs = stmt.executeQuery()) {
                boolean existe = rs.next();
                System.out.println("🔍 Tarjeta existe y activa: " + existe);
                return existe;
            }

        } catch (Exception e) {
            System.out.println("❌ Error verificando tarjeta:");
            e.printStackTrace();
            return false;
        }
    }

    public static boolean validarPin(String numeroTarjeta, String pin) {
        String sql = "SELECT 1 FROM Tarjeta WHERE Numero_Tarjeta = ? AND Pin = ? AND LOWER(LTRIM(RTRIM(Estado))) = 'activa'";

        try (Connection conn = ConexionSQL.obtenerConexion();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, numeroTarjeta.replaceAll("\\s", ""));
            stmt.setString(2, pin);

            try (ResultSet rs = stmt.executeQuery()) {
                boolean valido = rs.next();
                System.out.println("🔍 PIN válido: " + valido);
                return valido;
            }

        } catch (Exception e) {
            System.out.println("❌ Error validando PIN:");
            e.printStackTrace();
            return false;
        }
    }

    public static boolean cambiarPin(String numeroTarjeta, String pinNuevo) {
        String sql = "UPDATE Tarjeta SET Pin = ? WHERE Numero_Tarjeta = ? AND LOWER(LTRIM(RTRIM(Estado))) = 'activa'";

        try (Connection conn = ConexionSQL.obtenerConexion();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, pinNuevo);
            stmt.setString(2, numeroTarjeta.replaceAll("\\s", ""));

            int filas = stmt.executeUpdate();
            System.out.println("✅ PIN actualizado. Filas: " + filas);
            return filas > 0;

        } catch (Exception e) {
            System.out.println("❌ Error actualizando PIN:");
            e.printStackTrace();
            return false;
        }
    }

    public static boolean validarFecha(String numeroTarjeta, String fecha) {
        String sql = "SELECT 1 FROM Tarjeta WHERE Numero_Tarjeta = ? AND Fecha_Vencimiento = ? AND LOWER(LTRIM(RTRIM(Estado))) = 'activa'";

        try (Connection conn = ConexionSQL.obtenerConexion();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, numeroTarjeta.replaceAll("\\s", ""));
            stmt.setString(2, fecha);

            try (ResultSet rs = stmt.executeQuery()) {
                boolean valido = rs.next();
                System.out.println("🔍 Fecha válida: " + valido);
                return valido;
            }

        } catch (Exception e) {
            System.out.println("❌ Error validando fecha:");
            e.printStackTrace();
            return false;
        }
    }

    public static boolean validarCVV(String numeroTarjeta, String cvv) {
        String sql = "SELECT 1 FROM Tarjeta WHERE Numero_Tarjeta = ? AND CVV = ? AND LOWER(LTRIM(RTRIM(Estado))) = 'activa'";

        try (Connection conn = ConexionSQL.obtenerConexion();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, numeroTarjeta.replaceAll("\\s", ""));
            stmt.setString(2, cvv);

            try (ResultSet rs = stmt.executeQuery()) {
                boolean valido = rs.next();
                System.out.println("🔍 CVV válido: " + valido);
                return valido;
            }

        } catch (Exception e) {
            System.out.println("❌ Error validando CVV:");
            e.printStackTrace();
            return false;
        }
    }
}