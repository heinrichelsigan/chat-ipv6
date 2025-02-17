using Area23.At.Framework.Library.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EU.CqrXs.CqrSrv.CqrJd
{
    public partial class R : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string userHostName;
            string userHostAddr = Request.UserHostAddress;

            literalUserHost.Text = userHostAddr;

            if (!this.IsPostBack)
            {
                userHostName = Request.UserHostName;
                title.Text = userHostName;
                // header.InnerHtml = "<title>" + userHostName + "</title>";
            }

        }
    }
}