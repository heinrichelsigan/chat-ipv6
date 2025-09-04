using EU.CqrXs.Srv.Util;
using System;

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