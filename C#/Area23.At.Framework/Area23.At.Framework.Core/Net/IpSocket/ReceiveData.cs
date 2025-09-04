﻿using Area23.At.Framework.Core.Static;

namespace Area23.At.Framework.Core.Net.IpSocket
{

    public class ReceiveData
    {

        public byte[] BufferedData { get; internal set; }

        public string ClientIPAddr { get; internal set; }


        public int ClientIPPort { get; internal set; }


        public ReceiveData()
        {
            BufferedData = new byte[Constants.MAX_SOCKET_BYTE_BUFFEER];
            ClientIPAddr = string.Empty;
            ClientIPPort = 0;
        }

        public ReceiveData(byte[] buffer, int bytesLen, string? clientIpAddr, int? clientIPPort)
        {
            if (buffer != null && buffer.Length > 0)
            {
                BufferedData = new byte[bytesLen];
                Array.Copy(buffer, this.BufferedData, bytesLen);
            }
            if (!string.IsNullOrEmpty(clientIpAddr))
                this.ClientIPAddr = clientIpAddr;
            if (clientIPPort.HasValue)
                this.ClientIPPort = clientIPPort.Value;
        }

    }

}
