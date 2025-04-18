using EU.CqrXs.Srv.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EU.CqrXs.Srv
{
    public partial class Error : CqrJdBasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            
            this.DivException.Visible = true;
            
        }
    }
}