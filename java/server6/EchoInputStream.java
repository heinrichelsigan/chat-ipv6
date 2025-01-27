/*
	soucre: EchoInputStream.java
	class: EchoInputStream 
*/

import java.net.*;
import java.io.*;
import java.util.*;

class EchoInputStream extends FilterInputStream {
    
    final static int MAXCHARS = 8192;
	
    public EchoInputStream(InputStream in) {
        super(in);
    }
    
	public String readLine() throws IOException {
		
        StringBuffer result = new StringBuffer();
        System.out.println("Receiving from socket: ");
		
        do {
			
            int ch = -1; // EOF
            ch = read();
            
			if (ch == -1 || ch == 10 || ch == 13) // { handles EOF -1, CR \r & LF \n
				break; // } return result.toString();
			else
				result.append((char) ch);
			
        } while (result.length() < MAXCHARS);
		
        return result.toString();
    }
		
    public String getRequest() throws IOException {
		
        String line = readLine();
		
        if (line.length() < 1) 
            return ("(NULL)");
        
		return line;  
    }
}

