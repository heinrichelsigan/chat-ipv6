using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EU.CqrXs.Service
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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