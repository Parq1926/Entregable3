import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class TestConexion {
    public static void main(String[] args) {
        System.out.println("🔄 Probando conexión a SQL Server...");

        String url = "jdbc:sqlserver://localhost:1433;"
                + "databaseName=core;"
                + "encrypt=true;"
                + "trustServerCertificate=true;"
                + "integratedSecurity=true;";

        try {
            Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
            System.out.println("✅ Driver JDBC cargado");

            Connection conn = DriverManager.getConnection(url);
            System.out.println("✅ CONEXIÓN EXITOSA a SQL Server");
            conn.close();

        } catch (ClassNotFoundException e) {
            System.out.println("❌ Driver no encontrado");
            e.printStackTrace();
        } catch (SQLException e) {
            System.out.println("❌ Error SQL:");
            e.printStackTrace();
        }
    }
}
