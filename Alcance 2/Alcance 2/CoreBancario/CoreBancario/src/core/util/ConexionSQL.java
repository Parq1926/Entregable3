package core.util;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class ConexionSQL {

    private static final String URL = 
        "jdbc:sqlserver://localhost:1433;"
      + "databaseName=core;"
      + "encrypt=true;"
      + "trustServerCertificate=true;"
      + "integratedSecurity=true;";

    public static Connection obtenerConexion() throws SQLException {
        try {
            Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
            return DriverManager.getConnection(URL);
        } catch (ClassNotFoundException e) {
            throw new SQLException("Driver no encontrado", e);
        }
    }
}
