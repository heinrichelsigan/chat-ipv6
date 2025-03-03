namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    partial class TransparentDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected internal System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            tableLayoutPanel = new TableLayoutPanel();
            logoPictureBox = new PictureBox();
            labelProductName = new Label();
            labelCompanyName = new Label();
            labelCopyright = new Label();
            linkLabelGitUrl = new LinkLabel();
            linkLabelCqrXsEu = new LinkLabel();
            textBoxDescription = new TextBox();
            okButton = new Button();
            labelVersion = new Label();
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.BackColor = SystemColors.Control;
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
            tableLayoutPanel.Controls.Add(logoPictureBox, 0, 0);
            tableLayoutPanel.Controls.Add(labelProductName, 1, 0);
            tableLayoutPanel.Controls.Add(labelCompanyName, 1, 1);
            tableLayoutPanel.Controls.Add(labelCopyright, 1, 2);
            tableLayoutPanel.Controls.Add(linkLabelGitUrl, 1, 3);
            tableLayoutPanel.Controls.Add(linkLabelCqrXsEu, 1, 4);
            tableLayoutPanel.Controls.Add(textBoxDescription, 1, 5);
            tableLayoutPanel.Controls.Add(okButton, 1, 6);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(9, 9);
            tableLayoutPanel.Margin = new Padding(5, 4, 5, 4);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 6;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 44F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 11F));
            tableLayoutPanel.Size = new Size(561, 330);
            tableLayoutPanel.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            logoPictureBox.BackColor = SystemColors.Control;
            logoPictureBox.Dock = DockStyle.Fill;
            logoPictureBox.Location = new Point(5, 4);
            logoPictureBox.Margin = new Padding(5, 4, 5, 4);
            logoPictureBox.Name = "logoPictureBox";
            tableLayoutPanel.SetRowSpan(logoPictureBox, 6);
            logoPictureBox.Size = new Size(175, 282);
            logoPictureBox.TabIndex = 12;
            logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            labelProductName.BackColor = SystemColors.Control;
            labelProductName.Dock = DockStyle.Fill;
            labelProductName.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelProductName.Location = new Point(193, 0);
            labelProductName.Margin = new Padding(8, 0, 5, 0);
            labelProductName.MaximumSize = new Size(0, 21);
            labelProductName.Name = "labelProductName";
            labelProductName.Size = new Size(363, 21);
            labelProductName.TabIndex = 19;
            labelProductName.Text = "Product Name";
            labelProductName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelCompanyName
            // 
            labelCompanyName.BackColor = SystemColors.Control;
            labelCompanyName.Dock = DockStyle.Fill;
            labelCompanyName.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            labelCompanyName.Location = new Point(193, 29);
            labelCompanyName.Margin = new Padding(8, 0, 5, 0);
            labelCompanyName.MaximumSize = new Size(0, 21);
            labelCompanyName.Name = "labelCompanyName";
            labelCompanyName.Size = new Size(363, 21);
            labelCompanyName.TabIndex = 22;
            labelCompanyName.Text = "Company Name";
            labelCompanyName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            labelCopyright.BackColor = SystemColors.Control;
            labelCopyright.Dock = DockStyle.Fill;
            labelCopyright.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Italic, GraphicsUnit.Point, 0);
            labelCopyright.Location = new Point(193, 58);
            labelCopyright.Margin = new Padding(8, 0, 5, 0);
            labelCopyright.MaximumSize = new Size(0, 21);
            labelCopyright.Name = "labelCopyright";
            labelCopyright.Size = new Size(363, 21);
            labelCopyright.TabIndex = 21;
            labelCopyright.Text = "Copyright";
            labelCopyright.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // linkLabelGitUrl
            // 
            linkLabelGitUrl.Dock = DockStyle.Fill;
            linkLabelGitUrl.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkLabelGitUrl.Location = new Point(188, 87);
            linkLabelGitUrl.Margin = new Padding(3, 0, 2, 0);
            linkLabelGitUrl.MaximumSize = new Size(0, 21);
            linkLabelGitUrl.Name = "linkLabelGitUrl";
            linkLabelGitUrl.Size = new Size(371, 21);
            linkLabelGitUrl.TabIndex = 22;
            linkLabelGitUrl.TabStop = true;
            linkLabelGitUrl.Text = "https://github.com/heinrichelsigan/chat-ipv6";
            linkLabelGitUrl.TextAlign = ContentAlignment.MiddleLeft;
            linkLabelGitUrl.LinkClicked += ShowGitUrl;
            // 
            // linkLabelCqrXsEu
            // 
            linkLabelCqrXsEu.Dock = DockStyle.Fill;
            linkLabelCqrXsEu.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkLabelCqrXsEu.Location = new Point(188, 116);
            linkLabelCqrXsEu.Margin = new Padding(3, 0, 2, 0);
            linkLabelCqrXsEu.MaximumSize = new Size(0, 21);
            linkLabelCqrXsEu.Name = "linkLabelCqrXsEu";
            linkLabelCqrXsEu.Size = new Size(371, 21);
            linkLabelCqrXsEu.TabIndex = 23;
            linkLabelCqrXsEu.TabStop = true;
            linkLabelCqrXsEu.Text = "https://cqrxs.eu/help";
            linkLabelCqrXsEu.TextAlign = ContentAlignment.MiddleLeft;
            linkLabelCqrXsEu.LinkClicked += ShowCqrXsUrl;
            // 
            // textBoxDescription
            // 
            textBoxDescription.BackColor = SystemColors.Control;
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBoxDescription.Location = new Point(193, 149);
            textBoxDescription.Margin = new Padding(8, 4, 5, 4);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.ScrollBars = ScrollBars.Both;
            textBoxDescription.Size = new Size(363, 137);
            textBoxDescription.TabIndex = 23;
            textBoxDescription.TabStop = false;
            textBoxDescription.Text = "Description";
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.BackColor = SystemColors.ControlLightLight;
            okButton.BackgroundImageLayout = ImageLayout.None;
            okButton.DialogResult = DialogResult.Cancel;
            okButton.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            okButton.Location = new Point(455, 301);
            okButton.Margin = new Padding(5, 4, 5, 4);
            okButton.Name = "okButton";
            okButton.Size = new Size(101, 25);
            okButton.TabIndex = 24;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = false;
            // 
            // labelVersion
            // 
            labelVersion.BackColor = SystemColors.Control;
            labelVersion.Dock = DockStyle.Fill;
            labelVersion.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelVersion.Location = new Point(193, 29);
            labelVersion.Margin = new Padding(8, 0, 5, 0);
            labelVersion.MaximumSize = new Size(0, 21);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(363, 21);
            labelVersion.TabIndex = 0;
            labelVersion.Text = "Version";
            labelVersion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // TransparentDialog
            // 
            AcceptButton = okButton;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Control;
            ClientSize = new Size(579, 348);
            Controls.Add(tableLayoutPanel);
            Margin = new Padding(5, 4, 5, 4);
            Name = "TransparentDialog";
            Padding = new Padding(9);
            Text = "TransparentDialog";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        protected internal System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        protected internal System.Windows.Forms.PictureBox logoPictureBox;
        protected internal System.Windows.Forms.Label labelProductName;
        protected internal System.Windows.Forms.Label labelVersion;
        protected internal System.Windows.Forms.Label labelCopyright;
        protected internal System.Windows.Forms.Label labelCompanyName;
        protected internal System.Windows.Forms.LinkLabel linkLabelGitUrl;
        protected internal System.Windows.Forms.LinkLabel linkLabelCqrXsEu;
        protected internal System.Windows.Forms.TextBox textBoxDescription;
        protected internal System.Windows.Forms.Button okButton;
    }
}
