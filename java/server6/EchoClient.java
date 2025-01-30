/* 
	source: EchoClient.java
	mainclass: EchoClient
*/
import java.util.*;
import java.net.*;
import java.lang.*;
import java.io.*;

public class EchoClient {
	
    Socket connection;                      // Socket
    EchoClient client;						// echo client instance
	BufferedOutputStream outStream;      	// Socket Output
    BufferedReader inStream;                // Socket Input
    StringBuffer inPut;                     // Ein- und Ausgabe.
    int inByte;                             // dient zum Lesen von den Streams
	final static int T_EOF = -1; 
	final static int T_CR  = 13;
    final static int T_LF  = 10; 
	final static int T_BS  = 8;
	final static int NOERR = 0;
	final static int SOCKERR = 1;
	final static int IOERR = 2;

    public  EchoClient() {                    // Constructor
		super();
	}
    
	public int connect(String HNAME, int HPORT) {
        
		try {                                      
		
            connection = new Socket(HNAME, HPORT);
			
        } catch (UnknownHostException ex){      		
			return (SOCKERR);
        } catch (IOException ex){ 
			return (IOERR);
        }
		
		// Opens Input and Output Stream
		try {
			
			inStream = new BufferedReader(new InputStreamReader(connection.getInputStream()));
			outStream = new BufferedOutputStream(connection.getOutputStream());
        
		} catch (IOException ex){ 
			return(IOERR);
        } 
		
		return (NOERR);
	}

	public int close() {
		
		try { 
		
			outStream.close();
			inStream.close();
			
		} catch (Exception exii) {
			return (IOERR);
		}
		
        try{
			
		    connection.close();
			
		} catch (Exception ex){
				System.exit(SOCKERR);
		}
		
		return (NOERR);
	}
	 
    public boolean writeOut(String outPut) {
		
		try {
			
			for (int ix = 0; ix < outPut.length(); ix++) {
				outStream.write((int)outPut.charAt(ix));
			}	
			outStream.write(T_CR);
			outStream.write(T_LF); 
		    outStream.flush();
			
		} catch (IOException ex) { 
			return (false); // writing to Socket failed
		}
		
		return (true);
	}
	
	public String readIn() { 
	
		boolean finished = false;
		inByte = -1;
		inPut = new StringBuffer();		
		
		System.err.println("Receiving:");
		
		do {
			
		    try {
				
				inByte = inStream.read();
				System.err.print((char)inByte);
				
				switch (inByte) {
			    	case T_EOF:
			    	case T_CR:
			    	case T_LF: 
						finished = true; break;
			    	case T_BS: 
						; break; //IGNORE BACKSPACE
			    	default:
						inPut.append((char) inByte);
				}
		    } catch (IOException ex){
				System.err.println("IOException when reading from socket");
				finished=true; // NO RETURN FALUE
	   		}                  // MAYBE GOT ACK
		} while (!finished);
		
		System.out.println("\nfinished"); 
		
		return ((inPut.toString()));
	}
	  
	public static void main(String[] args) throws Exception {
				
		if (args.length != 3) {
			System.out.println("Usage: java EchoClient hostname port message");
			return;
		}
		
		EchoClient client = new EchoClient();		
		String myhost = args[0];
		int myport = (Integer.valueOf(args[1])).intValue();
		String myecho = args[2];
		
		if (client.connect(myhost, myport) != 0) {
			System.err.println("Connection failed !");
			return ;
		}
		
		if ((client.writeOut(myecho)) == false) { 
			System.err.println("Writing to socket failed !");
		}
		
		String newLine = client.readIn();
		
		if (client.close() != 0) {
			System.err.println("Closing socket failed !");
		}
 	}
}	
