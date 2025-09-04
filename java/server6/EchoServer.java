/*
	soucre: EchoServer.java
	mainclass: EchoServer 
*/
import java.net.*;
import java.io.*;
import java.lang.*;
import java.util.*;

public class EchoServer extends Thread implements Runnable {
    
	final static int MAXCHARS = 8192;
	final static int MAX_CONNECTIONS = 12;
	final static String IPV4_ADDR = "127.0.0.1";
	final static String IPV6_ADDR = "::1";
	static String server4Address = IPV4_ADDR;
	static String server6Address = IPV6_ADDR;
	static boolean multiAddr = true;
	static InetAddress saddress;
	static int sport = 7777; // default Port   
	ServerSocket ssock;	
	
	
    public static void main(String args[]) {
   				       
		int ipPort = sport;		
				
		if (args.length > 0)  {
			
			int argslen = args.length -1;
			
			if (argslen > 0)  {
				multiAddr = false;
				server4Address = args[0];
				System.out.println("server address set to " + server4Address);
			} 
      if (argslen > 1) {
        multiAddr = true;
        server6Address = args[1];
        System.out.println("server address6 set to " + server6Address);
      } 
      if (argslen > 2) 
			  ipPort = (Integer.valueOf(args[argslen])).intValue();
		}
		
		try {
			InetAddress inetAddr = InetAddress.getByName(server4Address);
			sport = ipPort;
			ServerSocket serverSock = new ServerSocket(ipPort, MAX_CONNECTIONS, inetAddr);	
			EchoServer server = new EchoServer(serverSock);
				
			if (multiAddr) {		
			
				InetAddress ipv6Addr = InetAddress.getByName(server6Address);
				ServerSocket ipv6Socket = new ServerSocket(ipPort, MAX_CONNECTIONS, ipv6Addr);	
				EchoServer server6 = new EchoServer(ipv6Socket);
					
				new Thread(server).start();
				new Thread(server6).start();			
			}
			else            		
				server.run();
			
		} catch (UnknownHostException uhEx) {			
			System.err.println("UnknownHostException thrown " + uhEx.toString());				
		} catch (Exception ex) {
			System.err.println("Exception thrown " + ex.toString());				
		}			
	}
        
    public EchoServer(ServerSocket ipServerSocket) {        			
		super();
		
		try {
			if (ipServerSocket != null) 
				ssock = ipServerSocket;
			else {
				InetAddress localAddr = InetAddress.getLocalHost();
				ssock = new ServerSocket(sport, MAX_CONNECTIONS, localAddr);	
			} 
		} catch(IOException ioEx) {			
			System.err.println("Unable to listen on " + saddress.toString() + ":" + sport + "\n" + ioEx.toString());
            System.exit(0);			
        }		
    }

    public void run() {
        
		System.out.println("Simple TCP Echo Server started ...");
        
		try {
            
			if (ssock == null)
				ssock = new ServerSocket(sport, MAX_CONNECTIONS, InetAddress.getLocalHost());
			
			saddress = ssock.getInetAddress();
			String serverHostIpName = ssock.getInetAddress().toString();			
            int localPort = ssock.getLocalPort();
            System.out.println("EchoServer is listening on address " + 
				// saddress.getHostAddress() + 
				serverHostIpName +
				" port " + localPort + ".");
            
			do {
            
				Socket client = ssock.accept();
            
				try {
			
					String clientHostIpName = describeConnection(client);
			
					BufferedOutputStream outStream = new BufferedOutputStream(client.getOutputStream());
					BufferedInputStream inStream = new BufferedInputStream(client.getInputStream());
					String inBuffer = receiveIn(inStream);
					// EchoInputStream inStream = new EchoInputStream(client.getInputStream());
					// String inBuffer = inStream.getRequest();
					String outBuffer = serverHostIpName + "\t=>\t" + clientHostIpName + "\t" + inBuffer;
					
          System.out.println("Finished, now sending back to socket: \n" + outBuffer);
					
					sendOut(outBuffer, outStream);
        
				} catch(IOException ex) {
					System.err.println("IOException occured when processing request.");
				}	
		
				try {			
					client.close();			
				} catch(IOException ex) {			
					System.err.println("IOException occured when closing socket.");
				}
          System.out.println("client socket close()");
            
			} while(true);
			
		} catch(IOException ioEx) {			
			System.err.println("Unable to listen on " + saddress.toString() + ":" + sport + "\n" + ioEx.toString());
            System.exit(0);			
        }
    }
	
	
	public String describeConnection(Socket client) {
		
        String destName = client.getInetAddress().getHostName();
        String destAddr = client.getInetAddress().getHostAddress();
        int destPort = client.getPort();
        
		System.out.println("Accepted connection to " + destName + " ("
            + destAddr + ")" + " on port " + destPort + ".");
		
		return client.getInetAddress().toString();
    }
	
	
	public String receiveIn(BufferedInputStream in) throws IOException {        		
		
		StringBuffer result = new StringBuffer(); 
    System.out.println("Receiving from socket: ");
		
		try {
			do {
				
				int ch = -1; // EOF
				ch = in.read();
				
				if (ch == -1 || ch == 10 || ch == 13) // { handles EOF -1, CR \r & LF \n
				  return result.toString();
				else
					result.append((char) ch);
				
			} while (result.length() < MAXCHARS);
			
        } catch(Exception ex){			
            System.err.println("Receiving input from socket failed:\n" + ex.toString());			
        }
		
		return result.toString();
    }
	  
	  
    public void sendOut(String buffer, BufferedOutputStream out) throws IOException {
        		
		try {
        
			for (int i = 0; i < buffer.length(); ++i) {
                out.write(buffer.charAt(i));
			}
			
            out.write(13); out.write(10);            
			out.flush();
            out.close();		
			
        } catch(Exception ex){			
            System.err.println("writing output to socket failed:\n" + ex.toString());			
        }
    }
	
}
