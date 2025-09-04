﻿using Area23.At.Framework.Library.Static;
using System;
using System.Web.UI.HtmlControls;

namespace EU.CqrXs.Srv.Util
{

    public partial class ImageFontControl : System.Web.UI.UserControl
    {

        string ipAddr = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {            
            ipAddr = Request.UserHostAddress;
            placeHolderImages.Visible = (placeHolderImages != null && placeHolderImages.Controls != null && placeHolderImages.Controls.Count > 1);                
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string rawurl = Request.Url.AbsoluteUri.ToString();
                string absurl = Request.Url.AbsoluteUri.ToString();
                rawurl = rawurl.Substring(0, rawurl.LastIndexOf("/") + 1).Replace("/Util/", "/").Replace("Util", "") + Constants.RES_DIR + LibPaths.SepCh + Constants.IMG_DIR + LibPaths.SepCh;
                absurl = absurl.Substring(0, absurl.LastIndexOf("/") + 1).Replace("/Util/", "/").Replace("Util", "") + Constants.RES_DIR + LibPaths.SepCh + Constants.IMG_DIR + LibPaths.SepCh;
                short imgNr = 0;
                foreach (char ch in ipAddr)
                {
                    HtmlImage htmlImage = new HtmlImage();
                    if (ch == '.')
                        htmlImage.Src = rawurl + "point.png";
                    else if (ch == ':')
                        htmlImage.Src = rawurl + "col.png";
                    else
                        htmlImage.Src = rawurl + ch + ".png";
                    htmlImage.Alt = ipAddr;
                    htmlImage.ID = $"htmlImage_{(imgNr++)}";
                    htmlImage.Visible = true;
                    placeHolderImages.Controls.Add(htmlImage);
                }

                placeHolderImages.Visible = true;
            }

        }

    }

}