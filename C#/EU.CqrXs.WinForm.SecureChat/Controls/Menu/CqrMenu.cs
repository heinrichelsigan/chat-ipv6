using Area23.At.Framework.Core.Static;
using EU.CqrXs.WinForm.SecureChat.Controls.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Menu
{
    public partial class CqrMenu : MenuStrip
    {
        public CqrMenu()
        {
            InitializeComponent();
        }

        public CqrMenu(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region Help About Info

        protected internal void MenuHelpItemViewHelp_Click(object sender, EventArgs e)
        {
            // TODO: implement it
            Help.ShowHelp(this, Constants.CQRXS_HELP_URL);
            // Help.ShowHelp(this, Constants.CQRXS_HELP_URL, HelpNavigator.TableOfContents, Constants.CQRXS_EU);
        }

        protected internal void MenuHelpItemAbout_Click(object sender, EventArgs e)
        {

            if ((AppDomain.CurrentDomain.GetData(Constants.CQRXS_TEST_FORM) != null) &&
                Convert.ToBoolean(AppDomain.CurrentDomain.GetData(Constants.CQRXS_TEST_FORM)))
            {
                TestForm testForm = new TestForm();
                testForm.Show();
            }
            TransparentDialog dialog = new TransparentDialog();
            dialog.ShowDialog();
        }

        protected internal void MenuHelpItemInfo_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("TransparentBadge", 0);
            string infoText = $"{Dialog.AssemblyProduct} v{Dialog.AssemblyVersion}\n{Dialog.AssemblyCopyright} {Dialog.AssemblyCompany}";
            string titleText = $"{Dialog.AssemblyTitle} v{Dialog.AssemblyVersion}";
            TransparentBadge badge = new TransparentBadge(titleText, infoText, MessageBoxIcon.Information, Properties.fr.Resources.CqrXsEuBadge);
            badge.ShowDialog();
            // MessageBox.Show(infoText, $"{titleText}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion Help About Info

    }
}
