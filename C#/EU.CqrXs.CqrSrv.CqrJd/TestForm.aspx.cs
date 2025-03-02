using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.CqrXs.CqrSrv;
using Area23.At.Framework.Library.CqrXs;
using Area23.At.Framework.Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EU.CqrXs.CqrSrv.CqrJd.Util;
using Area23.At.Framework.Library.Util;

namespace EU.CqrXs.CqrSrv.CqrJd
{
    public partial class TestForm : CqrJdBasePage
    {

        protected override void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            if (!Page.IsPostBack)
            {

                //this.TextBoxSource.Text = string.Empty;
                //this.TextBoxEnDeCrypted.Text = string.Empty;
                //this.TextBoxPipeHash.Text = string.Empty;
                //this.TextBoxKey.Text = string.Empty;
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                tmpStrg = string.Empty;

                if (ConfigurationManager.AppSettings["ServerIPv4"] != null)
                {
                    LiteralServerIPv4.Text = (string)ConfigurationManager.AppSettings["ServerIPv4"];
                    Area23Log.LogStatic("ServerIPv4: " + LiteralServerIPv4.Text);
                    tmpStrg += "ServerIPv4: " + (string)ConfigurationManager.AppSettings["ServerIPv4"] + Environment.NewLine;
                }
                if (ConfigurationManager.AppSettings["ServerIPv6"] != null)
                {
                    this.LiteralServerIPv6.Text = (string)ConfigurationManager.AppSettings["ServerIPv6"];
                    Area23Log.LogStatic("ServerIPv6: " + LiteralServerIPv6.Text);
                    tmpStrg += "ServerIPv6: " + (string)ConfigurationManager.AppSettings["ServerIPv6"] + Environment.NewLine + allStrng;
                }

                if (Application["ServerKey"] != null)
                    myServerKey = Application["ServerKey"].ToString();

                this.TextBoxKey.Text = myServerKey;
                SrvMsg1 srv1stMsg = new SrvMsg1(myServerKey);

                try
                {
                    if (!string.IsNullOrEmpty(this.TextBoxKey.Text))
                    {
                        myServerKey = this.TextBoxKey.Text;
                        Application["ServerKey"] = myServerKey;
                        srv1stMsg = new SrvMsg1(myServerKey);
                        this.TextBoxPipeHash.Text = srv1stMsg.PipeString;

                        if (!this.CheckBoxDecrypt.Checked)
                        {
                            string encrypted = srv1stMsg.CqrBaseMsg(this.TextBoxSource.Text);
                            this.TextBoxEnDeCrypted.Text = encrypted;
                        }
                        else
                        {
                            string decrypted = string.Empty;
                            MsgContent content = srv1stMsg.NCqrBaseMsg(this.TextBoxSource.Text);
                            this.TextBoxEnDeCrypted.Text = content.Message;
                        }

                    }
                }
                catch (Exception ex)
                {
                    CqrException.SetLastException(ex);
                    this.preOut.InnerText = ex.Message + ex.ToString();
                    Area23Log.LogStatic(ex);
                }
                allStrng = tmpStrg + allStrng;


                this.LiteralClientIp.Text = clientIp.ToString();
                Area23Log.LogStatic("ClientIp: " + clientIp.ToString());
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            this.Title = "CqrJd Testform " + DateTime.Now.Ticks;
            try
            {
                if (!string.IsNullOrEmpty(this.TextBoxKey.Text))
                {
                    myServerKey = this.TextBoxKey.Text;
                    Application["ServerKey"] = myServerKey;
                    SrvMsg1 srv1stMsg = new SrvMsg1(myServerKey);
                    this.TextBoxPipeHash.Text = srv1stMsg.PipeString;

                    if (!this.CheckBoxDecrypt.Checked)
                    {
                        string encrypted = srv1stMsg.CqrBaseMsg(this.TextBoxSource.Text);
                        this.TextBoxEnDeCrypted.Text = encrypted;
                    }
                    else
                    {
                        string decrypted = string.Empty;
                        MsgContent content = srv1stMsg.NCqrBaseMsg(this.TextBoxSource.Text);
                        this.TextBoxEnDeCrypted.Text = content.Message;
                    }

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                this.preOut.InnerText = ex.Message + ex.ToString();
                Area23Log.LogStatic(ex);
            }
        }

        protected void TextBoxKey_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.TextBoxKey.Text))
            {
                myServerKey = this.TextBoxKey.Text;
                myServerKey = this.TextBoxKey.Text;
                Application["ServerKey"] = myServerKey;
                SrvMsg1 srv1stMsg = new SrvMsg1(myServerKey);
                this.TextBoxPipeHash.Text = myServerKey;
            }
        }

        protected void Buttonclear_Click(object sender, EventArgs e)
        {
            this.TextBoxEnDeCrypted.Text = string.Empty;
            this.TextBoxSource.Text = string.Empty;

        }
    }

}