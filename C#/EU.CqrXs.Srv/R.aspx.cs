using System;

namespace EU.CqrXs.Srv
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