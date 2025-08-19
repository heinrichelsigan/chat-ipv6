
namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    partial class InputDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputDialog));
            notifyIcon = new NotifyIcon(components);
            buttonOK = new Button();
            TextBoxSecureKey = new TextBox();
            labelTextBox = new Label();
            panelHeader = new Panel();
            labelX = new Label();
            labelTitle = new Label();
            panelOuterDialog = new Panel();
            panelInnerDialog = new Panel();
            panelTop = new Panel();
            panelLeft = new Panel();
            panelBottom = new Panel();
            panelRight = new Panel();
            panelHeader.SuspendLayout();
            panelOuterDialog.SuspendLayout();
            panelInnerDialog.SuspendLayout();
            SuspendLayout();
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
            // buttonOK
            // 
            buttonOK.Cursor = Cursors.WaitCursor;
            buttonOK.Enabled = false;
            buttonOK.Location = new Point(280, 107);
            buttonOK.Margin = new Padding(2);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(84, 29);
            buttonOK.TabIndex = 6;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // TextBoxSecureKey
            // 
            TextBoxSecureKey.Location = new Point(4, 47);
            TextBoxSecureKey.Margin = new Padding(2);
            TextBoxSecureKey.Name = "TextBoxSecureKey";
            TextBoxSecureKey.Size = new Size(360, 28);
            TextBoxSecureKey.TabIndex = 5;
            TextBoxSecureKey.TextChanged += TextBoxSecureKey_TextChanged;
            // 
            // labelTextBox
            // 
            labelTextBox.Location = new Point(4, 20);
            labelTextBox.Margin = new Padding(2, 0, 2, 0);
            labelTextBox.Name = "labelTextBox";
            labelTextBox.Size = new Size(360, 24);
            labelTextBox.TabIndex = 4;
            labelTextBox.Text = "enter secure symmetric key for en-/de-cryption";
            labelTextBox.TextAlign = ContentAlignment.TopCenter;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = SystemColors.InactiveCaption;
            panelHeader.Controls.Add(labelX);
            panelHeader.Controls.Add(labelTitle);
            panelHeader.Location = new Point(4, 4);
            panelHeader.Margin = new Padding(0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(372, 27);
            panelHeader.TabIndex = 1;
            // 
            // labelX
            // 
            labelX.AutoSize = true;
            labelX.Cursor = Cursors.WaitCursor;
            labelX.Dock = DockStyle.Right;
            labelX.Font = new Font("Lucida Sans Unicode", 11F);
            labelX.ForeColor = Color.Gray;
            labelX.Location = new Point(355, 0);
            labelX.Margin = new Padding(1, 0, 1, 0);
            labelX.Name = "labelX";
            labelX.Size = new Size(17, 18);
            labelX.TabIndex = 3;
            labelX.Text = "X";
            labelX.Click += labelX_Click;
            labelX.MouseDown += LabelX_MouseDown;
            labelX.MouseEnter += LabelX_MouseEnter;
            labelX.MouseLeave += LabelX_MouseLeave;
            // 
            // labelTitle
            // 
            labelTitle.BackColor = Color.Transparent;
            labelTitle.Font = new Font("Trebuchet MS", 11F);
            labelTitle.ForeColor = SystemColors.ActiveCaptionText;
            labelTitle.Location = new Point(17, 0);
            labelTitle.Margin = new Padding(1);
            labelTitle.Name = "labelTitle";
            labelTitle.Padding = new Padding(1);
            labelTitle.Size = new Size(335, 24);
            labelTitle.TabIndex = 2;
            labelTitle.Text = "enter title text here";
            labelTitle.MouseDown += InputDialog_MouseDown;
            // 
            // panelOuterDialog
            // 
            panelOuterDialog.BackColor = SystemColors.Control;
            panelOuterDialog.Controls.Add(panelInnerDialog);
            panelOuterDialog.Controls.Add(panelHeader);
            panelOuterDialog.Controls.Add(panelTop);
            panelOuterDialog.Controls.Add(panelLeft);
            panelOuterDialog.Controls.Add(panelBottom);
            panelOuterDialog.Controls.Add(panelRight);
            panelOuterDialog.Dock = DockStyle.Fill;
            panelOuterDialog.Location = new Point(0, 0);
            panelOuterDialog.Margin = new Padding(0);
            panelOuterDialog.Name = "panelOuterDialog";
            panelOuterDialog.Size = new Size(380, 184);
            panelOuterDialog.TabIndex = 0;
            // 
            // panelInnerDialog
            // 
            panelInnerDialog.BorderStyle = BorderStyle.FixedSingle;
            panelInnerDialog.Controls.Add(TextBoxSecureKey);
            panelInnerDialog.Controls.Add(labelTextBox);
            panelInnerDialog.Controls.Add(buttonOK);
            panelInnerDialog.Location = new Point(4, 32);
            panelInnerDialog.Name = "panelInnerDialog";
            panelInnerDialog.Padding = new Padding(3);
            panelInnerDialog.Size = new Size(370, 146);
            panelInnerDialog.TabIndex = 7;
            // 
            // panelTop
            // 
            panelTop.BackColor = SystemColors.InactiveBorder;
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(4, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(372, 4);
            panelTop.TabIndex = 8;
            // 
            // panelLeft
            // 
            panelLeft.BackColor = SystemColors.InactiveBorder;
            panelLeft.Dock = DockStyle.Left;
            panelLeft.Location = new Point(0, 0);
            panelLeft.Name = "panelLeft";
            panelLeft.Size = new Size(4, 180);
            panelLeft.TabIndex = 9;
            // 
            // panelBottom
            // 
            panelBottom.BackColor = SystemColors.InactiveBorder;
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 180);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(376, 4);
            panelBottom.TabIndex = 10;
            // 
            // panelRight
            // 
            panelRight.BackColor = SystemColors.InactiveBorder;
            panelRight.Dock = DockStyle.Right;
            panelRight.Location = new Point(376, 0);
            panelRight.Name = "panelRight";
            panelRight.Size = new Size(4, 184);
            panelRight.TabIndex = 11;
            // 
            // InputDialog
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(380, 184);
            Controls.Add(panelOuterDialog);
            Font = new Font("Lucida Sans Unicode", 10F);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "InputDialog";
            TopMost = true;
            TransparencyKey = SystemColors.Control;
            Load += InputDialog_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelOuterDialog.ResumeLayout(false);
            panelInnerDialog.ResumeLayout(false);
            panelInnerDialog.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        private NotifyIcon notifyIcon;
        private Button buttonOK;
        private TextBox TextBoxSecureKey;
        private Label labelTextBox;
        private Panel panelHeader;
        private Label labelX;
        private Label labelTitle;
        private Panel panelOuterDialog;
        private Panel panelTop;
        private Panel panelBottom;
        private Panel panelLeft;
        private Panel panelRight;
        private Panel panelInnerDialog;
    }
}