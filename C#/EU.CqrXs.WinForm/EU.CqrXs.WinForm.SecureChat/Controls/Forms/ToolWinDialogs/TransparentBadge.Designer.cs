
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
            labelBadge = new Label();
            notifyIcon = new NotifyIcon(components);
            SuspendLayout();
            // 
            // labelBadge
            // 
            labelBadge.Font = new Font("Trebuchet MS", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelBadge.Location = new Point(0, 0);
            labelBadge.Margin = new Padding(0);
            labelBadge.Name = "labelBadge";
            labelBadge.Size = new Size(360, 122);
            labelBadge.TabIndex = 0;
            labelBadge.Text = "enter badge text here";
            labelBadge.TextAlign = ContentAlignment.MiddleCenter;
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
            ClientSize = new Size(360, 120);
            Controls.Add(labelBadge);
            Font = new Font("Lucida Sans Unicode", 10F);
            FormBorderStyle = FormBorderStyle.None;
            Name = "TransparentBadge";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "TransparentBadge";
            TopMost = true;
            TransparencyKey = SystemColors.Control;
            Load += TransparentBadge_Load;
            Shown += TransparentBadge_Shown;
            ResumeLayout(false);
        }
        #endregion

        private Label labelBadge;
        private NotifyIcon notifyIcon;
    }
}