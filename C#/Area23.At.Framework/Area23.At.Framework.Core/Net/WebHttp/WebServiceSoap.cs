using Area23.At.Framework.Core.Crypt.Cipher;
using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EU.CqrXs.CqrSrv.CqrJd;

namespace Area23.At.Framework.Core.Net.WebHttp
{

    /// <summary>
    /// WebServiceSoap implements a static WebServiceSoap Request via <see cref="CqrServiceSoapClient"/>
    /// and maily provides
    /// <see cref="ExternalClientIpFromServer()"
    /// <see cref="ExternalClientIpv6FromServer()"/>
    /// funtionality.
    /// </summary>
    public static class WebServiceSoap
    {
        
        /// <summary>
        /// static constructor
        /// </summary>
        static WebServiceSoap()
        { 
        }


        /// <summary>
        /// ExternalClientIpFromServer gets external network ip for client from server
        /// </summary>
        /// <returns>external official gateway <see cref="IPAddress">ip address</see> of client</returns>
        public static IPAddress? ExternalClientIpFromServer()
        {
            CqrServiceSoapClient client = new CqrServiceSoapClient(CqrServiceSoapClient.EndpointConfiguration.CqrServiceSoapv4);
            string resp = client.GetIPAddress();
            return IPAddress.Parse(resp);
        }

        /// <summary>
        /// ExternalClientIpv6FromServer gets external network ip for client from server
        /// </summary>
        /// <returns>external official gateway <see cref="IPAddress">ip address</see> of client</returns>
        public static IPAddress? ExternalClientIpv6FromServer()
        {
            CqrServiceSoapClient client = new CqrServiceSoapClient(CqrServiceSoapClient.EndpointConfiguration.CqrServiceSoapv6);
            string resp = client.GetIPAddress();
            return IPAddress.Parse(resp);
        }


    }

}
