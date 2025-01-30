using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EU.CqrXs.Framework.Core.Net.IpSocket
{
    public class IpSockReceiveData
    {
        public byte[] BufferedData { get; internal set; } = new byte[131070];

        public string ClientIPAddr { get; internal set; } = string.Empty;


        public int ClientIPPort { get; internal set; } = 0;


        public IpSockReceiveData()
        {                
        }

        public IpSockReceiveData(byte[] buffer, string? clientIpAddr, int? clientIPPort)
        {
            if (buffer != null && buffer.Length > 0)
                Array.Copy(buffer, this.BufferedData, buffer.Length);
            if (!string.IsNullOrEmpty(clientIpAddr))
                this.ClientIPAddr = clientIpAddr;
            if (clientIPPort.HasValue)
                this.ClientIPPort = clientIPPort.Value;
        }

    }
}
