using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Util;
using Area23.At.CqrXs.CqrJd.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Text;
using Area23.At.Framework.Library.Util;
using Area23.At.Framework.Library.Crypt.CqrJd;

namespace Area23.At.CqrXs.CqrJd
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Init(object sender, EventArgs e)
        {            
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            HashSet<CqrContact> chash = JsonContacts.LoadJsonContacts();
            HostLogHelper.LogRequest(sender, e, "Application_Start loaded contacts with " + chash.Count + " entries.");           
        }

        protected void Application_Disposed(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {            
        }



        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HostLogHelper.LogRequest(sender, e, "begin request");

            string url = HttpContext.Current.Request.Url.ToString();
            byte[] bytes = Request.InputStream.ToByteArray();
            string rq = Encoding.UTF8.GetString(bytes);
            if (url.Contains("Default.aspx") || url.Contains("Cqr") || url.Contains("Error.aspx"))
            {

            } else {
                if (rq.Contains("TextBoxEncrypted=") && rq.Contains("nTextBoxDecrypted=") && rq.Contains("ButtonSubmit=Submit"))
                    Response.Redirect(LibPaths.BaseAppPath + "Default.aspx");
            }

            return;
        }


        //protected void Application_EndRequest(object sender, EventArgs e)
        //{
        //    string msg = String.Format("application end request at {0} object sender = {1}, EventArgs e = {2}",
        //        DateTime.UtcNow.ToString("yyyy-MM-dd_HH:mm:ss"),
        //        (sender == null) ? "(null)" : sender.ToString(),
        //        (e == null) ? "(null)" : e.ToString());
        //    Area23Log.LogStatic(msg);
        //}
        

        protected void Application_Error(object sender, EventArgs e)
        {
            HostLogHelper.LogRequest(sender, e, "Application Error");
            Response.Redirect(Request.ApplicationPath + "/Error.aspx");
        }


        protected void Session_Start(object sender, EventArgs e)
        {
            HostLogHelper.LogRequest(sender, e, "new Session started");
        }


        protected void Session_End(object sender, EventArgs e)
        {
            HostLogHelper.LogRequest(sender, e, "Session ended");            
        }
    }
}