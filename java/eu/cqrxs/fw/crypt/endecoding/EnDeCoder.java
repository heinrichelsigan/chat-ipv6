package cqrxs.eu.fw.crypt.endecoding;

public abstract class EnDeCoder {

    public abstract String encodeBytes(byte[] inBytes);
    
    public abstract byte[] decodeString(String inString);

}
