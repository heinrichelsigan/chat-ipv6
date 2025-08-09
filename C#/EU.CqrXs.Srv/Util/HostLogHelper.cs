using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using System;
using System.Web;

namespace EU.CqrXs.Srv.Util
{
    /// <summary>
    /// HostLogHelper
    /// </summary>
    internal static class HostLogHelper 
    {
        internal static string UserHost
        {
            get
            {
                string userHost = Constants.UNKNOWN;
                try
                {                    
                    if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.UserHostAddress != null)
                    {
                        userHost = HttpContext.Current.Request.UserHostAddress;                        
                    }                    
                }
                catch (Exception ex)
                {
                    userHost = Constants.UNKNOWN;
                    Area23Log.LogStatic($"Unknown Host Exception {ex.GetType()} {ex.Message}\n\t{ex}\n");
                }

                return userHost;
            }
        }


        internal static void LogRequest(object sender, EventArgs e, string preMsg = "")
        {
            object oNull = (object)(Constants.SNULL);
            string logMsg = String.Format("{0} {1} object sender={2}, EventArgs e={3}\n",
                UserHost,
                preMsg ?? "",
                (sender ?? oNull).ToString(),
                (e ?? oNull).ToString());

            Area23Log.LogStatic(logMsg);
        }

        internal static string LogRequest(object sender, EventArgs e)
        {
            object oNull = (object)(Constants.SNULL);
            string logReq = String.Format("from {0} object sender={1}, EventArgs e={2}\n",
                UserHost,
                (sender ?? oNull).ToString(),
                (e ?? oNull).ToString());

            return logReq;
        }
    }

}