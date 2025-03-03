using EU.CqrXs.WinForm.SecureChat.Controls.Forms;
using EU.CqrXs.WinForm.SecureChat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using EU.CqrXs.WinForm.SecureChat.Properties;
using Area23.At.Framework.Core.Static;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    partial class TransparentDialog : Dialog
    {
        public TransparentDialog() : base()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = String.Format("{0} v{1}", AssemblyProduct, AssemblyVersion);
            this.labelCompanyName.Text = AssemblyCompany;
            this.labelCopyright.Text = AssemblyCopyright;
            this.linkLabelGitUrl.Text = Constants.GIT_CQR_URL;
            this.linkLabelCqrXsEu.Text = Constants.CQRXS_HELP_URL;
            this.textBoxDescription.Text = AssemblyDescription;
            using (MemoryStream ms = new MemoryStream(Resources.WinFormAboutDialog))
            {
                this.logoPictureBox.Image = new Bitmap(ms);
            }

        }


        private void ShowCqrXsUrl(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.ShowHelp(this, Constants.CQRXS_HELP_URL);
        }

        private void ShowGitUrl(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.ShowHelp(this, Constants.GIT_CQR_URL);
        }
    }
}
