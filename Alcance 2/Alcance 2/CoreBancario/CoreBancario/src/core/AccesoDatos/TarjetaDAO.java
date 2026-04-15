package core.AccesoDatos;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import core.util.ConexionSQL;

public class TarjetaDAO {

    public static Double obtenerSaldo(String numeroTarjeta) {
        String sql = "SELECT Monto_Total FROM Tarjeta WHERE Numero_Tarjeta = ?";

        try (Connection conn = ConexionSQL.obtenerConexion();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, numeroTarjeta.replaceAll("\\s", ""));
            ResultSet rs = stmt.executeQuery();

            if (rs.next()) {
                double saldo = rs.getDouble("Monto_Total");
                System.out.println("💰 Saldo encontrado: " + saldo);
                return saldo;
            } else {
                System.out.println("❌ Tarjeta no encontrada: " + numeroTarjeta);
                return null;
            }

        } catch (Exception e) {
            System.out.println("❌ Error obteniendo saldo:");
            e.printStackTrace();
            return null;
        }
    }

    public static boolean actualizarSaldo(String numeroTarjeta, double nuevoSaldo) {
        String sql = "UPDATE Tarjeta SET Monto_Total = ? WHERE Numero_Tarjeta = ?";

        try (Connection conn = ConexionSQL.obtenerConexion();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setDouble(1, nuevoSaldo);
            stmt.setString(2, numeroTarjeta.replaceAll("\\s", ""));

            int filas = stmt.executeUpdate();
            System.out.println("✅ Saldo actualizado. Filas: " + filas);
            return filas > 0;

        } catch (Exception e) {
            System.out.println("❌ Error actualizando saldo:");
            e.printStackTrace();
            return false;
        }
    }
}