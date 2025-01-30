using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EU.CqrXs.Framework.Core.Util;

namespace EU.CqrXs.Framework.Core.Net.IpSocket
{
    public class IPSockListener : IDisposable
    {
        private readonly object _lock = new object();
        private Thread t;

        public Socket? ServerSocket { get; protected internal set; }
        public IPAddress? ServerAddress { get; protected internal set; }
        public IPEndPoint? ServerEndPoint { get; protected internal set; }
        public Socket? ClientSocket { get; protected internal set; }

        public byte[] BufferedData { get; protected internal set; } = new byte[131070];

        

        public EventHandler<Area23EventArgs<IpSockReceiveData>> EventHandlerClientRequest { get; protected internal set; }

        protected internal EventHandler AcceptClientConnection { get; set; }

        /// <summary>
        /// constructs a listening at <see cref="ServerAddress"/> via <see cref="ServerEndPoint"/> bound <see cref="ServerSocket"/>
        /// </summary>
        /// <param name="connectedIpIfAddr"><see cref="ServerAddress"/></param>
        /// <exception cref="InvalidOperationException"></exception>
        public IPSockListener(IPAddress connectedIpIfAddr)
        {
            if (connectedIpIfAddr.AddressFamily != AddressFamily.InterNetwork && connectedIpIfAddr.AddressFamily != AddressFamily.InterNetworkV6)
                throw new InvalidOperationException("We can only handle AddressFamily == AddressFamily.InterNetwork and AddressFamily.InterNetworkV6");
            
            ServerAddress = connectedIpIfAddr; 
            ServerEndPoint = new IPEndPoint(ServerAddress, Constants.CHAT_PORT);
            ServerSocket = new Socket(ServerAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(ServerEndPoint);
            ServerSocket.Listen(Constants.BACKLOG);

            Task task = new Task(() => OnAcceptClientConnection("ctor", new EventArgs()));
            task.Start();
        }

        public IPSockListener(IPAddress connectedIpIfAddr, EventHandler<Area23EventArgs<IpSockReceiveData>> evClReq) : this(connectedIpIfAddr)
        {
            EventHandlerClientRequest = evClReq;
        }


        protected internal void OnAcceptClientConnection(object sender, EventArgs e)
        {
            if (ServerSocket != null && ServerSocket.IsBound)
            {
                Console.WriteLine(ListenToString());
                while (true)
                {
                    ClientSocket = ServerSocket.Accept();                                                         
                    // Task task = new Task(() => HandleClientRequest(sender, e));
                    // task.Start();
                    t = new Thread(new ThreadStart(() => HandleClientRequest(sender, e)));
                    t.Start();
                    Thread.Sleep(256);
                }
            }            
        }

        /// <summary>
        /// HandleClientRequest - handles client request
        /// </summary>
        protected internal void HandleClientRequest(object sender, EventArgs e)
        {
            if (ClientSocket != null)
            {
                lock (_lock)
                {
                    byte[] buffer = new byte[131070];

                    IPEndPoint clientIEP = (IPEndPoint?)ClientSocket.RemoteEndPoint;
                    string sstring = "Accept connection from " + clientIEP?.Address.ToString() + ":" + clientIEP?.Port.ToString() +
                        " => " + ServerAddress?.ToString() + ":" + ServerEndPoint?.ToString();
                    Area23Log.Logger.LogInfo(sstring);

                    int rsize = ClientSocket.Receive(buffer, 0, 131070, 0);
                    Array.Copy(buffer, BufferedData, rsize);
                    
                    IpSockReceiveData receiveData = new IpSockReceiveData(buffer, clientIEP?.Address.ToString(), clientIEP?.Port);                    
                    
                    byte[] sendData = new byte[8];
                    sendData = Encoding.Default.GetBytes("ACK\r\n\0");
                    ClientSocket.Send(sendData);

                    if (EventHandlerClientRequest != null)
                    {
                        EventHandler<Area23EventArgs<IpSockReceiveData>> handler = EventHandlerClientRequest;
                        Area23EventArgs<IpSockReceiveData> area23EventArgs = new Area23EventArgs<IpSockReceiveData>(receiveData);
                        handler?.Invoke(this, area23EventArgs);
                    }

                    ClientSocket.Close();
                }
            }
        }


        public virtual string ListenToString() => "Listening " +
            ((ServerEndPoint?.AddressFamily == AddressFamily.InterNetworkV6) ? "ip6 " : "ip4 ") +
            ServerAddress?.ToString() + ":" + ServerEndPoint?.Port + " " + ServerSocket?.SocketType.ToString();


        public virtual string AcceptToString() => "New connection from " + ClientSocket?.RemoteEndPoint?.ToString();

        public void Dispose()
        {
            if (ServerSocket != null && ServerSocket.IsBound)
            {
                if (ClientSocket != null && ClientSocket.Connected && ClientSocket.IsBound)
                {
                    ClientSocket.Disconnect(false);
                    ClientSocket.Close(Constants.CLOSING_TIMEOUT);
                }
                if (ServerSocket.Connected)
                    ServerSocket.Disconnect(false);

                ServerSocket.Close(Constants.CLOSING_TIMEOUT);
            }
            if (ClientSocket != null)
                ClientSocket.Dispose();
            if (ServerSocket != null)
                ServerSocket.Dispose();
        }

        ~IPSockListener()
        {
            this.Dispose();

            ClientSocket = null;
            ServerSocket = null;
            ServerEndPoint = null;
            ServerAddress = null;
        }            


    };
}
