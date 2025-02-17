using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace EU.CqrXs.CqrSrv.CqrJd
{
    public partial class ImageFontControl : System.Web.UI.UserControl
    {

        string ipAddr = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {
            ipAddr = Request.UserHostAddress;

           
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string r = Request.RawUrl.ToString();
                r = r.Substring(0, r.LastIndexOf("/") + 1) + "res/";

                foreach (char ch in ipAddr)
                {
                    HtmlImage htmlImage = new HtmlImage();
                    if (ch == '.')
                        htmlImage.Src = r + "point.png";
                    else if (ch == ':')
                        htmlImage.Src = r + "col.png";
                    else
                        htmlImage.Src = r + ch + ".png";
                    htmlImage.Alt = ipAddr;
                    htmlImage.ID = System.DateTime.Now.Ticks.ToString();
                    htmlImage.Visible = true;
                    placeHolderImages.Controls.Add(htmlImage);
                }

                placeHolderImages.Visible = true;
            }
        }
    }
}