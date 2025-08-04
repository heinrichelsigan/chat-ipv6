package cqrxs.eu.fw.crypt.endecoding;

import java.lang.Character;
import java.util.Base64; 
import java.util.Arrays;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

/**
 * Base16 En-/Decoder
 *
 */
public class Base16Coder extends EnDeCoder  {

	public static String VALID_CHARS = "0123456789abcdef";

	public Base16Coder() { 
	}

	/**
     * converts a binary byte array to hex string
     * @param inBytes byte array
     * @return hex string
     * @exception IllegalArgumentException is thrown when inBytes is null or empty
     */
	public String encodeBytes(byte[] inBytes) throws IllegalArgumentException {
		if (inBytes == null || inBytes.length < 1)
			throw new IllegalArgumentException("inBytes is a null or empty byte[] array.");

		String hexString = new String(inBytes);
		return hexString;
	}


	/*
	 * decodeBytes transforms a hex string to binary byte array
	 * @param inString: a hex string
	 * @return: binary byte array
     * @exception IllegalArgumentException is thrown when hexStr is null or empty
	 */
	public byte[] decodeString(String inString) throws IllegalArgumentException {
		if (inString == null || inString.length() == 0)
			throw new IllegalArgumentException("inString is a null or empty String");

		byte[] decodedBytes  = inString.getBytes();
		return decodedBytes;
	}

    /**
     * isValid checks if a string is in valid base16 format
     * @param inString the string to validate
     * @return true, if String is a valid Base16 encoded String, otherwise false
     */
	public boolean isValid(String inString) {
		for (char ch : inString.toCharArray()) {
            boolean isValid = false;
            for (char validCh : VALID_CHARS.toCharArray()) {
                if (validCh == ch) {
                    isValid = true; 
                    break;
                }
            }
            if (!isValid)
				return false;
		}
		return true;
	}

    /*
     * validates, if inString is in Base16 encoding format
     * @param inString string to validate for Base16 encoding format
     * @exception java.io.InvalidObjectException containing all illegal characters is thrown, when inString is not in Base16 encoding format
     */
    public void validate(String inString) throws java.io.InvalidObjectException { 
        String error = ""; 
        for (char ch : inString.toCharArray()) { 
            String chars = new Character(ch).toString();
            if (!VALID_CHARS.contains(chars)) 
                error += ch; 
        } 
        if (error != "" || error.length() > 0)
            throw new java.io.InvalidObjectException(error);

        return ;
    }

}
