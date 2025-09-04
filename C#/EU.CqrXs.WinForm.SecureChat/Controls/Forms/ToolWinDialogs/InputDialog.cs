using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Static;
using System.Runtime.InteropServices;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    public partial class InputDialog : System.Windows.Forms.Form
    {

        static int i = 0;
        static string _title = string.Empty;
        static string _text= string.Empty;  

        public string TFormType
        {
            get => this.GetType().ToString();
        }


        public InputDialog()
        {
            InitializeComponent();
        }

        public InputDialog(string title) : this()
        {
            _title = title;
            this.labelTitle.Text = title;
            this.labelX.ForeColor = Color.Gray;
            this.labelX.Enabled = false;
            this.buttonOK.Enabled = false;
            notifyIcon.Visible = false;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;

        }

        public InputDialog(string title, string text, MessageBoxIcon icon) : this(title)
        {
            _title = title;
            _text = text;
            switch (icon)
            {
                case MessageBoxIcon.None: notifyIcon.BalloonTipIcon = ToolTipIcon.None; break;
                case MessageBoxIcon.Warning: notifyIcon.BalloonTipIcon = ToolTipIcon.Warning; break;
                case MessageBoxIcon.Error: notifyIcon.BalloonTipIcon = ToolTipIcon.Error; break;
                case MessageBoxIcon.Question:
                case MessageBoxIcon.Information:
                default: notifyIcon.BalloonTipIcon = ToolTipIcon.Info; break;
            }

            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = text;
            notifyIcon.Text = text;
            labelTitle.Text = title;
            labelTextBox.Text = text;
        }



        internal void InputDialog_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Cursor = Cursors.NoMove2D;
                Program.User32.ReleaseCapture();
                Program.User32.SendMessage(Handle, Program.User32.WM_NCLBUTTONDOWN, Program.User32.HT_CAPTION, 0);
            }
        }


        internal void InputDialog_Load(object sender, EventArgs e)
        {



        }

        private void TextBoxSecureKey_TextChanged(object sender, EventArgs e)
        {

            string? seckey = this.TextBoxSecureKey.Text;
            if (!string.IsNullOrEmpty(seckey) && !string.IsNullOrWhiteSpace(seckey))
            {
                MemoryCache.CacheDict.SetValue<string>(Constants.APP_INPUT_DIALOG, seckey);
                this.labelX.Enabled = true;
                this.labelX.ForeColor = Color.Black;
                this.labelX.Font = new Font("Lucida Sans Unicode", 11F, FontStyle.Bold);
                this.labelX.BackColor = SystemColors.ActiveCaption;
                this.panelHeader.BackColor = SystemColors.ActiveCaption;
                this.panelTop.BackColor = SystemColors.ActiveBorder;
                this.panelLeft.BackColor = SystemColors.ActiveBorder;
                this.panelBottom.BackColor = SystemColors.ActiveBorder;
                this.panelRight.BackColor = SystemColors.ActiveBorder;               
                this.buttonOK.Enabled = true;
                this.labelX.Cursor = Cursors.Hand;
                this.buttonOK.Cursor = Cursors.Default;
            }
            else
            {
                MemoryCache.CacheDict.SetValue<string>(Constants.APP_INPUT_DIALOG, null);                
                this.labelX.ForeColor = Color.Gray;
                this.labelX.BackColor = SystemColors.InactiveCaption;
                this.panelHeader.BackColor = SystemColors.InactiveCaption;
                this.labelX.Enabled = false;
                this.labelX.Font = new Font("Lucida Sans Unicode", 11F, FontStyle.Regular);
                this.buttonOK.Enabled = false;
                this.panelTop.BackColor = SystemColors.InactiveBorder;
                this.panelLeft.BackColor = SystemColors.InactiveBorder;
                this.panelBottom.BackColor = SystemColors.InactiveBorder;
                this.panelRight.BackColor = SystemColors.InactiveBorder;
                this.labelX.Cursor = Cursors.WaitCursor;
                this.buttonOK.Cursor = Cursors.WaitCursor;
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(3000, _title, _text, ToolTipIcon.Warning);
            }


        }

        private void labelX_Click(object sender, EventArgs e)
        {
            buttonOK_Click(sender, e);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string? seckey = this.TextBoxSecureKey.Text;
            if (string.IsNullOrEmpty(seckey) || string.IsNullOrWhiteSpace(seckey) || !buttonOK.Enabled || !labelX.Enabled)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(3000, _title, _text, ToolTipIcon.Warning);
            }
            else if (!string.IsNullOrEmpty(seckey) && !string.IsNullOrWhiteSpace(seckey) && buttonOK.Enabled && labelX.Enabled)
            {
                MemoryCache.CacheDict.SetValue<string>(Constants.APP_INPUT_DIALOG, seckey);
                this.Close();
            }
        }

        private void LabelX_MouseEnter(object sender, EventArgs e)
        {
            string? seckey = this.TextBoxSecureKey.Text;
            if (string.IsNullOrEmpty(seckey) || string.IsNullOrWhiteSpace(seckey) || !labelX.Enabled)
                this.labelX.BackColor = SystemColors.GradientInactiveCaption;
            else
                this.labelX.BackColor = SystemColors.GradientActiveCaption;
        }

        private void LabelX_MouseLeave(object sender, EventArgs e)
        {
            string? seckey = this.TextBoxSecureKey.Text;
            if (string.IsNullOrEmpty(seckey) || string.IsNullOrWhiteSpace(seckey) || !labelX.Enabled)
                this.labelX.BackColor = SystemColors.InactiveCaption;
            else
                this.labelX.BackColor = SystemColors.ActiveCaption;
        }

        private void LabelX_MouseDown(object sender, MouseEventArgs e)
        {

        }
    }
}
