package cqrxs.eu.fw.crypt.endecoding;

import java.util.Base64; 
import java.util.Arrays;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

public class Base64Coder extends EnDeCoder  {

    public Base64Coder() {
    }

    public String encodeBytes(byte[] inBytes) {
        String encodedString = Base64.getEncoder().encodeToString(inBytes);
        return encodedString;
    }

    public byte[] decodeString(String encodedString) {
        byte[] decodedBytes = Base64.getDecoder().decode(encodedString);
        return decodedBytes;
    }

    public String encode(String inString) {
        String encoded = Base64.getEncoder().encodeToString(inString.getBytes());
        return encoded;
    }

    public String decode(String encodedString) {
        byte[] decodedBytes = Base64.getDecoder().decode(encodedString);
        String decodedString = new String(decodedBytes);
        return decodedString;
    }

}
