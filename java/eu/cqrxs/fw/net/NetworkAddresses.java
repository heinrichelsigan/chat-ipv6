import java.io.*;
import java.net.*;
import java.util.*;
import static java.lang.System.out;

public class NetworkAddresses {

    public static void main(String args[]) throws SocketException {
        Enumeration<NetworkInterface> nets = NetworkInterface.getNetworkInterfaces();
        for (NetworkInterface netint : Collections.list(nets))
            displayInterfaceInformation(netint);
		
		getConnectedAddress();
    }
	
	public static void getConnectedAddress() {
		Socket s;
		try {
			s = new Socket("18.100.254.167", 80);
			System.out.println(s.getLocalAddress().getHostAddress());
			s.close();
			return;
		} catch (Exception ex) { 
		}
		try {
			s = new Socket("15.188.56.225", 80);
			System.out.println(s.getLocalAddress().getHostAddress());
			s.close();
			return;
		} catch (Exception ex) { 
		}
		try {
			s = new Socket("35.168.3.151", 80);
			System.out.println(s.getLocalAddress().getHostAddress());
			s.close();
			return;			
		} catch (Exception ex) { 
		}
		
		return;
	}

    static void displayInterfaceInformation(NetworkInterface netint) throws SocketException {
        out.printf("Display name: %s\n", netint.getDisplayName());
        out.printf("Name: %s\n", netint.getName());
        Enumeration<InetAddress> inetAddresses = netint.getInetAddresses();
        for (InetAddress inetAddress : Collections.list(inetAddresses)) {
            out.printf("InetAddress: %s\n", inetAddress);
        }
        out.printf("\n");
     }
}  
