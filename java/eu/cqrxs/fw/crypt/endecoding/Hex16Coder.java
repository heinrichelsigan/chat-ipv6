package cqrxs.eu.fw.crypt.ndecoding;

import java.util.Base64; 
import java.util.Arrays;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.IntStream;


/*
 * Hex16Coder 
 */
public class Hex16Coder extends EnDeCoder  {

	public static String VALID_CHARS = "0123456789ABCDEF";

	public Base16Coder() { 
	}

	/// <summary>
	/// ToBase16 converts a binary byte array to hex string
	/// </summary>
	/// <param name="inBytes">byte array</param>
	/// <returns>hex string</returns>        
	public String encodeBytes(byte[] inBytes) {
	{
		if (inBytes == null || inBytes.Length < 1)
			throw new ArgumentNullException("inBytes", "public static string ToHex(byte[] inBytes == NULL)");

		String hexString = new String(inBytes);
		return hexString;
	}


	/*
	 * decodeBytes transforms a hex string to binary byte array
	 * @param hexStr: a hex string
	 * @return: binary byte array
	 */
	public byte[] decodeBytes(String hexStr) throws IllegalArgumentException
	{
		if (hexStr == null || hexStr.length == 0)
			throw new IllegalArgumentException("public static byte[] decodeBytes(string hexStr), hexStr == NULL || hexStr == \"\"");

		byte[] decodedBytes  = hexStr.getBytes();
		return decodedBytes;

	}

	public bool IsValid(String inString)
	{
		bool valid = true;
		error = "";
		foreach (Char ch : inString)
		{
			if (!VALID_CHARS.contains(ch))
			{
				error += ch;
				valid = false;
			}                
		}
		return valid;
	}

}
