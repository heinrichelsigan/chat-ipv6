using System.Net.Sockets;
using System.Net;
using CSharp_Server6.Framework.Net;

namespace CSharp_Server6
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
