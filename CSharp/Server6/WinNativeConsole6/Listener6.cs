using Area23.At.Framework.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;


namespace CSharpServer6.WinNativeConsole6
{
    /// <summary>
    /// Listener6 is listening server
    /// </summary>
    public class Listener6
    {
        private static Socket? serverSocket;
        private static IPAddress? serverAddress;
        private Socket clientSocket;
        private IPEndPoint? clientIEP;
        private byte[] data = new byte[8192];
        Thread t;

        /// <summary>
        /// Constructor for Listeneer6
        /// </summary>
        /// <param name="address">ip address to listen on</param>
        public Listener6(IPAddress address)
        {
            serverAddress = address;
            serverSocket = GetTcpServer(address);            
        }

        /// <summary>
        /// RunServer - runs server oo serverSocket
        /// </summary>
        public void RunServer()
        {
            if (serverSocket != null)
            {
                Console.WriteLine("Listening on " + serverSocket.LocalEndPoint?.ToString());

                while (true)
                { 
                    clientSocket = serverSocket.Accept();
                    Console.WriteLine("New connection from " + clientSocket.RemoteEndPoint?.ToString());
                    t = new Thread(new ThreadStart(HandleClientRequest));
                    t.Start();
                    Thread.Sleep(500);
                }
            }
        }


        /// <summary>
        /// HandleClientRequest - handles client request
        /// </summary>
        public void HandleClientRequest()
        {

            if (clientSocket != null)
            {
                clientIEP = (IPEndPoint?)clientSocket.RemoteEndPoint;
                byte[] receiveData = new byte[8192];
                int rsize = clientSocket.Receive(receiveData, 0, 8192, 0);
                Array.Copy(receiveData, data, rsize);
                string rstring = Encoding.Default.GetString(data, 0, rsize);
                Console.WriteLine(rstring);
                string sstring = serverAddress?.ToString() + " => " + clientIEP?.Address.ToString() + " : " + rstring;
                byte[] sendData = new byte[8192];
                sendData = Encoding.Default.GetBytes(sstring);
                clientSocket.Send(sendData);
                clientSocket.Close();
                Console.WriteLine("Closing socket.");
            }
        }



        /// <summary>
        /// GetTcpServer gets a server enpoint and a server socket
        /// </summary>
        /// <param name="address">ip address to listen on</param>
        /// <returns><see cref="Socket">socket</see> where server listens</returns>
        public static Socket GetTcpServer(IPAddress address)
        {
            IPEndPoint ipEo = new IPEndPoint(address, Constants.CHAT_PORT);
            Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        
            socket.Bind(ipEo);
            socket.Listen(Constants.BACKLOG);

            return socket;
                
        }

    }

}
