/* 
    class NetworkAddresses provides basic network interfaces and local addresses facility

*/
package eu.cqrxs.fw.net;

import java.io.*;
import java.net.InetAddress;
import java.net.Socket;
import java.net.NetworkInterface;
import java.net.*;
import java.util.*;
import java.util.HashSet;
import java.util.Set;

import static java.lang.System.out;

public class NetworkAddresses {

    public static void main(String args[]) throws SocketException {
		
        getNetworkInterfaces();
		getConnectedAddress();
    }
	
	public static void getConnectedAddress() {
		Socket s;
		try {
			s = new Socket("18.101.101.108", 80);
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

    static Set<InetAddress> getIpAddrsFromNetIf(NetworkInterface netint) throws SocketException {
        Set<InetAddress> addrss = new HashSet<InetAddress>();

        Enumeration<InetAddress> inetAddresses = netint.getInetAddresses();
        for (InetAddress inetAddress : Collections.list(inetAddresses)) {
            addrss.add(inetAddress);
            out.printf("InetAddress: %s\n", inetAddress);
        }
        out.printf("\n");
        
        return addrss; 
     }


    public static Set<InetAddress> getNetworkInterfaces() throws SocketException {
            
        Set<InetAddress> myAddrs = new HashSet<InetAddress>();
        Enumeration<NetworkInterface> nets = NetworkInterface.getNetworkInterfaces(); 
        for (NetworkInterface netIf : Collections.list(nets)) { 
            out.printf("Display name: %s\n", netIf.getDisplayName()); 
            out.printf("Name: %s\n", netIf.getName()); 
            displaySubInterfaces(netIf); 
            Set<InetAddress> ifAddrSet = getIpAddrsFromNetIf(netIf);
            for (InetAddress inetAddr : ifAddrSet)
               myAddrs.add(inetAddr); 

            out.printf("\n"); 
        } 
        return myAddrs;
    }

    static void displaySubInterfaces(NetworkInterface netIf) throws SocketException { 
        Enumeration<NetworkInterface> subIfs = netIf.getSubInterfaces(); 
        for (NetworkInterface subIf : Collections.list(subIfs)) { 
            out.printf("\tSub Interface Display name: %s\n", subIf.getDisplayName()); 
            out.printf("\tSub Interface Name: %s\n", subIf.getName()); 
        }
                                                                                                                                                                         }

}  
