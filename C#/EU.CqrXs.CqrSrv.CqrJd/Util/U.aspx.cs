using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Media;
using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Util;

namespace EU.CqrXs.CqrSrv.CqrJd.Util
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