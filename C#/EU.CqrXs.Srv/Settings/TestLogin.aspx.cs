using Area23.At.Framework.Library.Cqr;
using EU.CqrXs.Srv.Util;
using System;
using System.Web.UI;

namespace EU.CqrXs.Srv.Settings
{
    public partial class TestLogin : CqrJdBasePage
    {

        protected CqrFacade facade;

        protected override void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            facade = new CqrFacade(myServerKey);
            if (!Page.IsPostBack)
            {
                this.TextBoxPassword.Text = string.Empty;
                this.TextBoxUserName.Text = string.Empty;
                this.preOut.InnerHtml = string.Empty;
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            facade = new CqrFacade(myServerKey);

            if (!Page.IsPostBack)
            {
                tmpStrg = string.Empty;

                if (Application["KeepSignIn"] != null)
                    this.CheckBoxKeepSignIn.Checked = (bool)Application["KeepSignIn"];

                if (Application["ServerKey"] != null)
                    myServerKey = Application["ServerKey"].ToString();
                facade = new CqrFacade(myServerKey);               
            }
        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.TextBoxUserName.Text))
            {
                this.preOut.InnerHtml = "You entered an <i style=\"color: purple\">empty</i> <b style=\"color: red\">username</b>.\n";
                return;
            }
            if (String.IsNullOrEmpty(this.TextBoxPassword.Text))
            {
                this.preOut.InnerHtml = "<i style=\"color: yellow\">Warning, empty password.</i>.\n";
            }

            Application["KeepSignIn"] = (bool)this.CheckBoxKeepSignIn.Checked;

            bool authenticated = this.AuthHtPasswd(this.TextBoxUserName.Text, this.TextBoxPassword.Text);
            if (!authenticated)
            {
                this.Title = "Login for \"" + TextBoxUserName.Text + "\" failed at " + DateTime.Now.ToString();
                this.preOut.InnerHtml = "Login for <b style=\"color: purple\">" + TextBoxUserName.Text + "</b> <b style=\"color: red\">failed</b> at " + DateTime.Now.ToString() + "\n";
            }
            else
            {
                this.Title = "Login for \"" + TextBoxUserName.Text + "\" successful at " + DateTime.Now.ToString();
                this.preOut.InnerHtml = "Login for <b style=\"color: purple\">" + TextBoxUserName.Text + "</b> <b style=\"color: green\">successful</b> at " + DateTime.Now.ToString() + "\n";
            }
           
        }

    }

}