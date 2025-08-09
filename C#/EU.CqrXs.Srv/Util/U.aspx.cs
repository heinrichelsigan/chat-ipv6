using System;

namespace EU.CqrXs.Srv.Util
{

    /// <summary>
    /// W show ipv4 or ipv6 address
    /// </summary>
    public partial class U : System.Web.UI.Page
    {

        string ipAddress = string.Empty;
        string userHostName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            string userHostName = Request.UserHostName;
            string userHostAddr = Request.UserHostAddress;                        

            if (!this.IsPostBack)
            {
                foreach (char c in userHostAddr.ToLower())
                {
                    foreach (char vc in "0123456789abcdef:.")
                    {
                        if (c == vc)
                        {
                            ipAddress += c;
                            break;
                        }
                    }
                }

                literalUserHost.Text = ipAddress;
                title.Text = userHostName ?? Request.UserHostAddress + " " + ipAddress;
            }
                      
            
        }


    }
}