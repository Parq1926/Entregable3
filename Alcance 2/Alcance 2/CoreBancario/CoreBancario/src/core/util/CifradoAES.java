package core.util;

import java.util.Base64;
import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

public class CifradoAES {
    private static final String KEY = "12345678901234561234567890123456";
    private static final String IV = "1234567890123456";
    
    public static String descifrar(String textoCifrado) throws Exception {
    byte[] cipherData = Base64.getDecoder().decode(textoCifrado);
    byte[] keyBytes = KEY.getBytes("UTF-8");
    byte[] ivBytes = IV.getBytes("UTF-8");
    
    SecretKeySpec keySpec = new SecretKeySpec(keyBytes, "AES");
    IvParameterSpec ivSpec = new IvParameterSpec(ivBytes);
    Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
    cipher.init(Cipher.DECRYPT_MODE, keySpec, ivSpec);
    
    byte[] decrypted = cipher.doFinal(cipherData);
    return new String(decrypted, "UTF-8");
}
    
    public static String cifrar(String texto) throws Exception {
        byte[] keyBytes = KEY.getBytes("UTF-8");
        byte[] ivBytes = IV.getBytes("UTF-8");
        
        SecretKeySpec keySpec = new SecretKeySpec(keyBytes, "AES");
        IvParameterSpec ivSpec = new IvParameterSpec(ivBytes);
        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        cipher.init(Cipher.ENCRYPT_MODE, keySpec, ivSpec);
        
        byte[] encrypted = cipher.doFinal(texto.getBytes("UTF-8"));
        return Base64.getEncoder().encodeToString(encrypted);
    }
}