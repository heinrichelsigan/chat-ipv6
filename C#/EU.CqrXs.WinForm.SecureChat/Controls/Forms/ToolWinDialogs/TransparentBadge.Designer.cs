
namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    partial class TransparentBadge
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransparentBadge));
            labelTitle = new Label();
            notifyIcon = new NotifyIcon(components);
            SuspendLayout();
            // 
            // labelTitle
            // 
            labelTitle.BackColor = Color.Transparent;
            labelTitle.Font = new Font("Trebuchet MS", 11F);
            labelTitle.ForeColor = SystemColors.ButtonHighlight;
            labelTitle.Location = new Point(8, 0);
            labelTitle.Margin = new Padding(1);
            labelTitle.Name = "labelTitle";
            labelTitle.Padding = new Padding(1);
            labelTitle.Size = new Size(532, 32);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "enter badge text here";
            // 
            // notifyIcon
            // 
            notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
            notifyIcon.BalloonTipText = "Warning";
            notifyIcon.BalloonTipTitle = "Tooltip Warning";
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "Warning";
            notifyIcon.Visible = true;
            // 
            // TransparentBadge
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(550, 240);
            Controls.Add(labelTitle);
            Font = new Font("Lucida Sans Unicode", 10F);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TransparentBadge";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "TransparentBadge";
            TopMost = true;
            TransparencyKey = SystemColors.Control;
            Load += TransparentBadge_Load;
            ResumeLayout(false);
        }
        #endregion

        private NotifyIcon notifyIcon;
        private Label labelTitle;
    }
}