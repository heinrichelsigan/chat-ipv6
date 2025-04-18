using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EU.CqrXs.Srv.Util;
using EU.CqrXs.Srv.Settings;
using EU.CqrXs.Srv.Properties;
using Area23.At.Framework.Library.Util;

namespace EU.CqrXs.Srv.Settings
{
    public partial class TestForm : CqrJdBasePage
    {

        protected CqrFacade facade;

        protected override void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            facade = new CqrFacade(myServerKey);
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
            facade = new CqrFacade(myServerKey);

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
                facade = new CqrFacade(myServerKey);

                CContent cMsg = new CContent(this.TextBoxSource.Text, facade.PipeString, CType.Json, "");
                try
                {
                    if (!string.IsNullOrEmpty(this.TextBoxKey.Text))
                    {
                        myServerKey = this.TextBoxKey.Text;
                        Application["ServerKey"] = myServerKey;                        
                        this.TextBoxPipeHash.Text = facade.PipeString;

                        if (!this.CheckBoxDecrypt.Checked)
                        {
                            string encrypted = cMsg.EncryptToJson(myServerKey);
                            this.TextBoxEnDeCrypted.Text = encrypted;
                        }
                        else
                        {
                            string decrypted = string.Empty;
                            CContent content = cMsg.DecryptFromJson(myServerKey, this.TextBoxSource.Text);
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
            if (!string.IsNullOrEmpty(this.TextBoxKey.Text))
            {
                myServerKey = this.TextBoxKey.Text;
                Application["ServerKey"] = myServerKey;
                facade = new CqrFacade(myServerKey);
                this.TextBoxPipeHash.Text = facade.PipeString;
            }
            CContent cMsg = new CContent(this.TextBoxSource.Text, facade.PipeString, CType.Json, "");

            try
            {
                if (!this.CheckBoxDecrypt.Checked)
                {
                    string encrypted = cMsg.EncryptToJson(myServerKey);
                    this.TextBoxEnDeCrypted.Text = encrypted;
                }
                else
                {
                    string decrypted = string.Empty;
                    CContent content = cMsg.DecryptFromJson(myServerKey, this.TextBoxSource.Text);
                    this.TextBoxEnDeCrypted.Text = content.Message;
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
                Application["ServerKey"] = myServerKey;
                facade = new CqrFacade(myServerKey);
                this.TextBoxPipeHash.Text = facade.PipeString;
            }
        }

        protected void Buttonclear_Click(object sender, EventArgs e)
        {
            this.TextBoxEnDeCrypted.Text = string.Empty;
            this.TextBoxSource.Text = string.Empty;

        }
    }

}