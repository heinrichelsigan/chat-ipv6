using System;
using System.Configuration;

namespace EU.CqrXs.Service
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SpanInfo.InnerHtml = "<b>ApplicationPath</b>: " + Request.ApplicationPath + "<br />\n" +
                    "<b>RawUrl</b>: " + Request.RawUrl + "<br />\n" +
                    "<b>PhysicalApplicationPath</b>: " + Request.PhysicalApplicationPath + "<br />\n" +
                    "<b>UserAgent</b>: " + Request.UserAgent + "<br />\n";

                if (metaRefreshId != null && metaRefreshId.Attributes != null && metaRefreshId.Attributes.Count > 0 && metaRefreshId.Attributes["content"] != null)
                {                
                    aHrefId.InnerText = ConfigurationManager.AppSettings["AppUrl"] + "CqrService.asmx";                    
                    metaRefreshId.Attributes["content"] = "8; url=" + ConfigurationManager.AppSettings["AppUrl"] + "CqrService.asmx";
                }
            }
            // Response.Redirect("CqrService.asmx");
        }
    }
}