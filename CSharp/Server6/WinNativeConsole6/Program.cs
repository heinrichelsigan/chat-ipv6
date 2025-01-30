using Area23.At.Framework.Library.Core;
using System.Net.Sockets;
using System.Net;
using CSharpServer6.WinNativeConsole6;
using Area23.At.Framework.Library.Core.Net;

namespace CSharpServer6.WinNativeConsole6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            List<IPAddress> addrs = NetworkAddresses.GetConnectedIpAddresses();
            foreach (IPAddress addr in addrs)
            {                
                Listener6 listener = new Listener6(addr);                
                // Task.Run(() =>
                // {
                    listener.RunServer();
                // });
            }
        }
    }

}
