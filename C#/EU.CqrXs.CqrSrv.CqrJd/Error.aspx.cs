using EU.CqrXs.CqrSrv.CqrJd.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EU.CqrXs.CqrSrv.CqrJd
{
    public partial class Error : CqrJdBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            this.DivException.Visible = true;
            
        }
    }
}