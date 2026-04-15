package core.util;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class ConexionSQL {

    private static final String URL = 
        "jdbc:sqlserver://PAMELA:1433;"
      + "databaseName=core;"
      + "encrypt=true;"
      + "trustServerCertificate=true;";

    private static final String USER = "sa";
    private static final String PASSWORD = "Pamela.1998";

    public static Connection obtenerConexion() throws SQLException {
        try {
            Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
            System.out.println("✅ Driver cargado");
            Connection conn = DriverManager.getConnection(URL, USER, PASSWORD);
            System.out.println("✅ Conectado a SQL Server");
            return conn;
        } catch (ClassNotFoundException e) {
            throw new SQLException("Driver no encontrado", e);
        }
    }
}