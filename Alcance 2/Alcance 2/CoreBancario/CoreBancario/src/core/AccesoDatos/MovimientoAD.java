package core.AccesoDatos;

import java.sql.Connection;
import java.sql.PreparedStatement;
import core.util.ConexionSQL;

public class MovimientoAD {

    public static void registrarMovimiento(
            String numeroTarjeta,
            int tipoMovimiento,
            double monto,
            String descripcion,
            String respuesta
    ) {
        String sql = "INSERT INTO Bitacora " +
            "(Fecha_Bitacora, Numero_Tarjeta, Numero_Identificacion, Codigo_Cajero, IdTransaccion, Monto_Transaccion) " +
            "VALUES (GETDATE(), ?, (SELECT Numero_Identificacion FROM Tarjeta WHERE Numero_Tarjeta = ?), ?, ?, ?)";

        try (Connection conn = ConexionSQL.obtenerConexion();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, numeroTarjeta);
            stmt.setString(2, numeroTarjeta);
            stmt.setString(3, "CJ-001");
            stmt.setInt(4, tipoMovimiento);
            
            if (monto > 0) {
                stmt.setDouble(5, monto);
            } else {
                stmt.setNull(5, java.sql.Types.DECIMAL);
            }

            int filas = stmt.executeUpdate();
            System.out.println("✅ Movimiento registrado en Bitacora. Filas: " + filas);

        } catch (Exception e) {
            System.out.println("❌ Error al registrar movimiento:");
            e.printStackTrace();
        }
    }
}