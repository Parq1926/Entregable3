package core.Servicio;

import core.AccesoDatos.MovimientoAD;
import core.AccesoDatos.TarjetaAD;
import core.AccesoDatos.TarjetaDAO;
import core.util.CifradoAES;

public class CoreServicio {

    public String procesarTrama(String trama) {
    if (trama == null || trama.isEmpty()) {
        return "ERROR";
    }

    try {
        System.out.println("📥 Trama recibida: " + trama);
        
        // Método manual para separar por |
        java.util.List<String> partes = new java.util.ArrayList<>();
        int inicio = 0;
        for (int i = 0; i < trama.length(); i++) {
            if (trama.charAt(i) == '|') {
                partes.add(trama.substring(inicio, i));
                inicio = i + 1;
            }
        }
        partes.add(trama.substring(inicio));
        
        System.out.println("🔍 Partes encontradas: " + partes.size());
        for (int i = 0; i < partes.size(); i++) {
            System.out.println("   Parte " + i + ": " + partes.get(i));
        }
        
        if (partes.size() < 4) {
            System.out.println("❌ Partes insuficientes: " + partes.size());
            return "ERROR";
        }
        
        String tipoTransaccion = partes.get(0);
        String numeroCuenta = partes.get(1);
        String tarjetaCifrada = partes.get(2);
        String parametroCifrado = partes.get(3);
        
        System.out.println("🔍 Tipo: " + tipoTransaccion);
        System.out.println("🔍 Tarjeta cifrada: " + tarjetaCifrada);
        System.out.println("🔍 Parámetro cifrado: " + parametroCifrado);
        
        // Descifrar datos
        String numeroTarjeta = CifradoAES.descifrar(tarjetaCifrada);
        String parametroDescifrado = CifradoAES.descifrar(parametroCifrado);
        
        System.out.println("🔓 Tarjeta descifrada: " + numeroTarjeta);
        System.out.println("🔓 Parámetro descifrado: " + parametroDescifrado);
        
        return procesarPorTipo(tipoTransaccion, numeroTarjeta, parametroDescifrado);

    } catch (Exception e) {
        e.printStackTrace();
        return "ERROR";
    }
}
    
    

    private String procesarPorTipo(String tipoTransaccion, String numeroTarjeta, String parametro) {
        try {
            // Tipo 1: Retiro
            if (tipoTransaccion.equals("1")) {
                double monto = Double.parseDouble(parametro) / 100;
                System.out.println("💰 Monto real: " + monto);

                // Verificar que la tarjeta existe
                if (!TarjetaAD.tarjetaExiste(numeroTarjeta)) {
                    return "ERROR";
                }

                Double saldo = TarjetaDAO.obtenerSaldo(numeroTarjeta);
                if (saldo == null) {
                    return "ERROR";
                }

                if (saldo < monto) {
                    MovimientoAD.registrarMovimiento(
                        numeroTarjeta, 1, monto,
                        "Fondos insuficientes", "INSUF"
                    );
                    return "INSUF";
                }

                double nuevoSaldo = saldo - monto;
                TarjetaDAO.actualizarSaldo(numeroTarjeta, nuevoSaldo);

                MovimientoAD.registrarMovimiento(
                    numeroTarjeta, 1, monto,
                    "Retiro aprobado", "OK"
                );
                
                // Cifrar respuesta
                String respuestaCifrada = CifradoAES.cifrar("OK");
                return respuestaCifrada;
            }

            // Tipo 2: Consulta de saldo
            if (tipoTransaccion.equals("2")) {
                Double saldo = TarjetaDAO.obtenerSaldo(numeroTarjeta);

                if (saldo == null) {
                    return "ERROR";
                }

                long saldoFormateado = Math.round(saldo * 100);
                String saldoTexto = String.format("%019d", saldoFormateado);
                
                // Cifrar respuesta
                String respuestaCifrada = CifradoAES.cifrar("OK" + saldoTexto);
                return respuestaCifrada;
            }

            // Tipo 3: Validar PIN
            if (tipoTransaccion.equals("3")) {
                boolean valido = TarjetaAD.validarPin(numeroTarjeta, parametro);
                String respuesta = valido ? "OK" : "ERROR";
                return CifradoAES.cifrar(respuesta);
            }

            // Tipo 4: Cambiar PIN
            if (tipoTransaccion.equals("4")) {
                boolean actualizado = TarjetaAD.cambiarPin(numeroTarjeta, parametro);
                String respuesta = actualizado ? "OK" : "ERROR";
                return CifradoAES.cifrar(respuesta);
            }

            // Tipo 5: Validar fecha de vencimiento
            if (tipoTransaccion.equals("5")) {
                boolean valido = TarjetaAD.validarFecha(numeroTarjeta, parametro);
                String respuesta = valido ? "OK" : "ERROR";
                return CifradoAES.cifrar(respuesta);
            }

            // Tipo 6: Validar CVV
            if (tipoTransaccion.equals("6")) {
                boolean valido = TarjetaAD.validarCVV(numeroTarjeta, parametro);
                String respuesta = valido ? "OK" : "ERROR";
                return CifradoAES.cifrar(respuesta);
            }

            return "ERROR";
            
        } catch (Exception e) {
            e.printStackTrace();
            return "ERROR";
        }
    }
}