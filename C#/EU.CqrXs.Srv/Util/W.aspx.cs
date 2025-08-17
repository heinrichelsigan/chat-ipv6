using Area23.At.Framework.Library.Util;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace EU.CqrXs.Srv.Util
{

    /// <summary>
    /// W show ipv4 or ipv6 address
    /// </summary>
    public partial class W : System.Web.UI.Page
    {

        string ipAddress = string.Empty;
        string userHostName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            userHostName = Request.UserHostName;
            foreach (char c in Request.UserHostAddress.ToString())
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

            title.Text = userHostName ?? Request.UserHostAddress + " " + ipAddress;
            string phypath = Server.MapPath("~/res/img/");
            string saveName = ipAddress.Replace(".", "_").Replace(":", "-") + ".png";

            Bitmap bmp;

            try
            {
                bmp = (Bitmap)MergeImage(ipAddress, saveName);

                string r = Request.RawUrl.ToString();
                ahrefId.HRef = r.Substring(0, r.LastIndexOf("/") + 1) + "res/img/" + saveName;
                ahrefId.InnerText = "res/img/" + saveName;
                ahrefId.Target = "_blank";
            }
            catch (Exception ex)
            {
                Area23Log.LogOriginMsgEx("W.aspx", "Page_Load", ex); 
            }
        }

        protected System.Drawing.Image MergeImage(string hexstring, string saveName)
        {
            string phypath = Server.MapPath("~/res/img/");
            if (!phypath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                phypath += Path.DirectorySeparatorChar;
            string bmpName = phypath;
            
            if (hexstring.Length > 60)
                hexstring = hexstring.Substring(0, 57) + "...";

            int wlen = 0;
            foreach (char ch in hexstring)
            {
                if (ch == ':' || ch == '.' || ch == ' ' || ch == ',' || ch == '!' || ch == ';' ||
                    ch == '|' || ch == '"' || ch == '`' || ch == '´' || ch == ("'".ToCharArray())[0])
                    wlen += 15;
                else
                    wlen += 60;
            }
            Bitmap yimage = new Bitmap(wlen, 200);
            
            hexstring = hexstring.ToLower();

            System.Drawing.Bitmap ximage = new System.Drawing.Bitmap(phypath + "whiteCanvas.png");
            System.Drawing.Bitmap zimage = new Bitmap(ximage, wlen, 200);
            try
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(zimage))
                {
                    int w = 60, offset = 0;
                    for (int i = 0; (i < hexstring.Length); i++)
                    {
                        w = 60;
                        char ch = hexstring[i];

                        if (ch == '0' || ch == '1' || ch == '2' || ch == '3' || ch == '4' ||
                            ch == '5' || ch == '6' || ch == '7' || ch == '8' || ch == '9' ||
                            ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f')
                            bmpName = phypath + ch.ToString() + ".png";
                        else if (ch == ':')
                        {
                            bmpName = phypath + "col.png";
                            w = 12;
                        }
                        else
                        {
                            bmpName = phypath + "point.png";
                            w = 12;
                        }

                        yimage = new Bitmap(bmpName);
                        g.DrawImage(yimage, new System.Drawing.Rectangle(offset, 0, w, 200));
                        try { g.Flush(); } catch (Exception exf) { Area23Log.LogOriginMsgEx("W.aspx", "MergeImage", exf); }
                        // try { g.Save(); } catch (Exception exs) { Area23Log.LogOriginMsgEx("W.aspx", "MergeImage", exs); }
                        offset += w;
                    }

                    // try { g.Flush(); } catch (Exception exf) { Area23Log.LogOriginMsgEx("W.aspx", "MergeImage", exf); }
                    try { g.Save(); } catch (Exception exs) { Area23Log.LogOriginMsgEx("W.aspx", "MergeImage", exs); }
                }

                string fName = phypath + saveName;
                zimage.Save(fName, ImageFormat.Png);
            }
            catch (Exception edr)
            {
                Area23Log.LogOriginMsgEx("W.aspx", "MergeImage", edr);
            }
            return (zimage);
        }

    }
}