using Area23.At.Framework.Core.Net.WebHttp;
using System.Net;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms
{

    partial class Peer2PeerChat
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected internal new System.ComponentModel.IContainer components = null;

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
        protected internal override void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            base.InitializeComponent();            
            SplitChatView = new SplitContainer();
            TextBoxSource = new TextBox();
            TextBoxDestionation = new TextBox();
            PictureBoxYou = new PictureBox();
            ButtonKey = new Button();
            PanelEnCodeCrypt = new Panel();
            ComboBoxContacts = new ComboBox();
            ButtonCheck = new Button();
            ComboBoxSecretKey = new ComboBox();
            ComboBoxIp = new ComboBox();
            TextBoxPipe = new TextBox();
            RichTextBoxChat = new RichTextBox();
            PanelDestination = new Panel();
            peerServerSwitchControl1 = new Controls.PeerServerSwitchControl();
            attachmentListControl = new Controls.AttachmentListControl(components);
            dragnDropGroupBox = new Controls.DragNDropGroupBox(components);
            PanelCenter = new Panel();
            RichTextBoxOneView = new RichTextBox();
            PanelBottom = new Panel();
            ButtonAttach = new Button();
            ButtonClear = new Button();
            ButtonSend = new Button();
            ((System.ComponentModel.ISupportInitialize)SplitChatView).BeginInit();
            SplitChatView.Panel1.SuspendLayout();
            SplitChatView.Panel2.SuspendLayout();
            SplitChatView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBoxYou).BeginInit();
            PanelEnCodeCrypt.SuspendLayout();
            PanelDestination.SuspendLayout();
            PanelCenter.SuspendLayout();
            PanelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // SplitChatView
            // 
            SplitChatView.BackColor = SystemColors.ControlLight;
            SplitChatView.IsSplitterFixed = true;
            SplitChatView.Location = new Point(0, 0);
            SplitChatView.Margin = new Padding(0);
            SplitChatView.MaximumSize = new Size(800, 600);
            SplitChatView.MinimumSize = new Size(600, 400);
            SplitChatView.Name = "SplitChatView";
            // 
            // SplitChatView.Panel1
            // 
            SplitChatView.Panel1.AllowDrop = true;
            SplitChatView.Panel1.BackgroundImageLayout = ImageLayout.None;
            SplitChatView.Panel1.Controls.Add(TextBoxSource);
            SplitChatView.Panel1MinSize = 300;
            // 
            // SplitChatView.Panel2
            // 
            SplitChatView.Panel2.BackgroundImageLayout = ImageLayout.None;
            SplitChatView.Panel2.Controls.Add(TextBoxDestionation);
            SplitChatView.Panel2MinSize = 300;
            SplitChatView.Size = new Size(800, 460);
            SplitChatView.SplitterDistance = 396;
            SplitChatView.SplitterIncrement = 8;
            SplitChatView.SplitterWidth = 8;
            SplitChatView.TabIndex = 31;
            SplitChatView.TabStop = false;
            // 
            // TextBoxSource
            // 
            TextBoxSource.BackColor = SystemColors.GradientActiveCaption;
            TextBoxSource.BorderStyle = BorderStyle.FixedSingle;
            TextBoxSource.Dock = DockStyle.Fill;
            TextBoxSource.Font = new Font("Lucida Sans Unicode", 9F);
            TextBoxSource.Location = new Point(0, 0);
            TextBoxSource.Margin = new Padding(1);
            TextBoxSource.MaxLength = 65536;
            TextBoxSource.Multiline = true;
            TextBoxSource.Name = "TextBoxSource";
            TextBoxSource.ScrollBars = ScrollBars.Both;
            TextBoxSource.Size = new Size(396, 460);
            TextBoxSource.TabIndex = 32;
            // 
            // TextBoxDestionation
            // 
            TextBoxDestionation.BackColor = SystemColors.GradientInactiveCaption;
            TextBoxDestionation.BorderStyle = BorderStyle.FixedSingle;
            TextBoxDestionation.Dock = DockStyle.Fill;
            TextBoxDestionation.Font = new Font("Lucida Sans Unicode", 9F);
            TextBoxDestionation.Location = new Point(0, 0);
            TextBoxDestionation.Margin = new Padding(1);
            TextBoxDestionation.MaxLength = 65536;
            TextBoxDestionation.Multiline = true;
            TextBoxDestionation.Name = "TextBoxDestionation";
            TextBoxDestionation.ScrollBars = ScrollBars.Both;
            TextBoxDestionation.Size = new Size(396, 460);
            TextBoxDestionation.TabIndex = 33;
            // 
            // PictureBoxYou
            // 
            PictureBoxYou.BackgroundImageLayout = ImageLayout.Stretch;
            PictureBoxYou.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBoxYou.Location = new Point(10, 56);
            PictureBoxYou.Margin = new Padding(1);
            PictureBoxYou.Name = "PictureBoxYou";
            PictureBoxYou.Padding = new Padding(1);
            PictureBoxYou.Size = new Size(150, 148);
            PictureBoxYou.TabIndex = 71;
            PictureBoxYou.TabStop = false;
            // 
            // ButtonKey
            // 
            ButtonKey.BackColor = SystemColors.ButtonHighlight;
            ButtonKey.BackgroundImageLayout = ImageLayout.Center;
            ButtonKey.Font = new Font("Lucida Sans Unicode", 10F, FontStyle.Bold);
            ButtonKey.ForeColor = SystemColors.ActiveCaptionText;
            ButtonKey.Image = Properties.Resources.a_right_key;
            ButtonKey.Location = new Point(213, 4);
            ButtonKey.Margin = new Padding(1);
            ButtonKey.Name = "ButtonKey";
            ButtonKey.Padding = new Padding(1);
            ButtonKey.Size = new Size(40, 27);
            ButtonKey.TabIndex = 12;
            ButtonKey.UseVisualStyleBackColor = false;
            ButtonKey.Click += ButtonKey_Click;
            // 
            // PanelEnCodeCrypt
            // 
            PanelEnCodeCrypt.BackColor = SystemColors.ActiveCaption;
            PanelEnCodeCrypt.Controls.Add(ComboBoxContacts);
            PanelEnCodeCrypt.Controls.Add(ButtonCheck);
            PanelEnCodeCrypt.Controls.Add(ComboBoxSecretKey);
            PanelEnCodeCrypt.Controls.Add(ComboBoxIp);
            PanelEnCodeCrypt.Controls.Add(TextBoxPipe);
            PanelEnCodeCrypt.Controls.Add(ButtonKey);
            PanelEnCodeCrypt.ForeColor = SystemColors.WindowText;
            PanelEnCodeCrypt.Location = new Point(0, 28);
            PanelEnCodeCrypt.Margin = new Padding(0);
            PanelEnCodeCrypt.Name = "PanelEnCodeCrypt";
            PanelEnCodeCrypt.Size = new Size(994, 36);
            PanelEnCodeCrypt.TabIndex = 10;
            // 
            // ComboBoxContacts
            // 
            ComboBoxContacts.BackColor = SystemColors.ControlLightLight;
            ComboBoxContacts.Font = new Font("Lucida Sans Unicode", 10F);
            ComboBoxContacts.ForeColor = SystemColors.ControlText;
            ComboBoxContacts.FormattingEnabled = true;
            ComboBoxContacts.Location = new Point(524, 6);
            ComboBoxContacts.Margin = new Padding(1);
            ComboBoxContacts.Name = "ComboBoxContacts";
            ComboBoxContacts.Size = new Size(261, 24);
            ComboBoxContacts.TabIndex = 18;
            ComboBoxContacts.Text = "[enter peer IPv4 or IPv6 for directly connect]";
            ComboBoxContacts.SelectedIndexChanged += ComboBoxContacts_SelectedIndexChanged;
            ComboBoxContacts.Leave += ComboBoxContacts_FocusLeave;
            // 
            // ButtonCheck
            // 
            ButtonCheck.BackColor = SystemColors.ButtonHighlight;
            ButtonCheck.BackgroundImageLayout = ImageLayout.Center;
            ButtonCheck.Font = new Font("Lucida Sans Unicode", 10F, FontStyle.Bold);
            ButtonCheck.ForeColor = SystemColors.ActiveCaptionText;
            ButtonCheck.Image = Properties.de.Resources.CableWireCut;
            ButtonCheck.Location = new Point(791, 2);
            ButtonCheck.Margin = new Padding(1);
            ButtonCheck.Name = "ButtonCheck";
            ButtonCheck.Padding = new Padding(1);
            ButtonCheck.Size = new Size(34, 32);
            ButtonCheck.TabIndex = 17;
            ButtonCheck.UseVisualStyleBackColor = false;
            // 
            // ComboBoxSecretKey
            // 
            ComboBoxSecretKey.BackColor = SystemColors.ControlLightLight;
            ComboBoxSecretKey.ForeColor = SystemColors.ControlText;
            ComboBoxSecretKey.FormattingEnabled = true;
            ComboBoxSecretKey.Location = new Point(10, 6);
            ComboBoxSecretKey.Margin = new Padding(1);
            ComboBoxSecretKey.Name = "ComboBoxSecretKey";
            ComboBoxSecretKey.Size = new Size(200, 24);
            ComboBoxSecretKey.TabIndex = 11;
            ComboBoxSecretKey.Text = "[enter secret key here]";
            ComboBoxSecretKey.SelectedIndexChanged += ComboBoxSecretKey_SelectedIndexChanged;
            ComboBoxSecretKey.TextUpdate += ComboBoxSecretKey_TextUpdate;
            ComboBoxSecretKey.Leave += ComboBoxSecretKey_FocusLeave;
            // 
            // ComboBoxIp
            // 
            ComboBoxIp.BackColor = SystemColors.ControlLightLight;
            ComboBoxIp.Font = new Font("Lucida Sans Unicode", 10F);
            ComboBoxIp.ForeColor = SystemColors.ControlText;
            ComboBoxIp.FormattingEnabled = true;
            ComboBoxIp.Location = new Point(355, 6);
            ComboBoxIp.Margin = new Padding(1);
            ComboBoxIp.Name = "ComboBoxIp";
            ComboBoxIp.Size = new Size(164, 24);
            ComboBoxIp.TabIndex = 15;
            ComboBoxIp.Text = "[enter peer IPv4/IPv6]";
            ComboBoxIp.SelectedIndexChanged += ComboBoxIp_SelectedIndexChanged;
            ComboBoxIp.Leave += ComboBoxIp_FocusLeave;
            // 
            // TextBoxPipe
            // 
            TextBoxPipe.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TextBoxPipe.HideSelection = false;
            TextBoxPipe.Location = new Point(257, 5);
            TextBoxPipe.Margin = new Padding(1);
            TextBoxPipe.Name = "TextBoxPipe";
            TextBoxPipe.ReadOnly = true;
            TextBoxPipe.Size = new Size(92, 26);
            TextBoxPipe.TabIndex = 13;
            // 
            // RichTextBoxChat
            // 
            RichTextBoxChat.BorderStyle = BorderStyle.FixedSingle;
            RichTextBoxChat.ForeColor = SystemColors.WindowText;
            RichTextBoxChat.Location = new Point(0, 0);
            RichTextBoxChat.Margin = new Padding(1);
            RichTextBoxChat.Name = "RichTextBoxChat";
            RichTextBoxChat.Size = new Size(824, 136);
            RichTextBoxChat.TabIndex = 41;
            RichTextBoxChat.Text = "";
            // 
            // PanelDestination
            // 
            PanelDestination.BackColor = SystemColors.ActiveCaption;
            PanelDestination.Controls.Add(peerServerSwitchControl1);
            PanelDestination.Controls.Add(attachmentListControl);
            PanelDestination.Controls.Add(dragnDropGroupBox);
            PanelDestination.Controls.Add(PictureBoxYou);
            PanelDestination.ForeColor = SystemColors.ActiveCaptionText;
            PanelDestination.Location = new Point(826, 32);
            PanelDestination.Margin = new Padding(0);
            PanelDestination.Name = "PanelDestination";
            PanelDestination.Size = new Size(168, 653);
            PanelDestination.TabIndex = 70;
            // 
            // peerServerSwitchControl1
            // 
            peerServerSwitchControl1.AllowDrop = true;
            peerServerSwitchControl1.AutoValidate = AutoValidate.EnableAllowFocusChange;
            peerServerSwitchControl1.BackColor = SystemColors.GradientActiveCaption;
            peerServerSwitchControl1.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            peerServerSwitchControl1.Location = new Point(8, 0);
            peerServerSwitchControl1.Margin = new Padding(1);
            peerServerSwitchControl1.Name = "peerServerSwitchControl1";
            peerServerSwitchControl1.Padding = new Padding(1);
            peerServerSwitchControl1.Size = new Size(152, 48);
            peerServerSwitchControl1.TabIndex = 84;
            // 
            // attachmentListControl
            // 
            attachmentListControl.BackColor = SystemColors.ControlLightLight;
            attachmentListControl.Font = new Font("Lucida Sans Unicode", 9F);
            attachmentListControl.Location = new Point(0, 215);
            attachmentListControl.Margin = new Padding(0);
            attachmentListControl.Name = "attachmentListControl";
            attachmentListControl.Size = new Size(168, 192);
            attachmentListControl.TabIndex = 83;
            // 
            // dragnDropGroupBox
            // 
            dragnDropGroupBox.AllowDrop = true;
            dragnDropGroupBox.BackColor = SystemColors.ControlLightLight;
            dragnDropGroupBox.Font = new Font("Lucida Sans Unicode", 8.5F);
            dragnDropGroupBox.Location = new Point(10, 416);
            dragnDropGroupBox.Margin = new Padding(1);
            dragnDropGroupBox.Name = "dragnDropGroupBox";
            dragnDropGroupBox.Padding = new Padding(1);
            dragnDropGroupBox.Size = new Size(150, 131);
            dragnDropGroupBox.TabIndex = 81;
            dragnDropGroupBox.TabStop = false;
            dragnDropGroupBox.Text = "   Drag'N'Drop Box";
            // 
            // PanelCenter
            // 
            PanelCenter.Controls.Add(SplitChatView);
            PanelCenter.Controls.Add(RichTextBoxOneView);
            PanelCenter.Location = new Point(8, 72);
            PanelCenter.Margin = new Padding(0);
            PanelCenter.Name = "PanelCenter";
            PanelCenter.Size = new Size(800, 460);
            PanelCenter.TabIndex = 30;
            // 
            // RichTextBoxOneView
            // 
            RichTextBoxOneView.AcceptsTab = true;
            RichTextBoxOneView.Dock = DockStyle.Fill;
            RichTextBoxOneView.Location = new Point(0, 0);
            RichTextBoxOneView.Margin = new Padding(2);
            RichTextBoxOneView.Name = "RichTextBoxOneView";
            RichTextBoxOneView.ReadOnly = true;
            RichTextBoxOneView.Size = new Size(800, 460);
            RichTextBoxOneView.TabIndex = 36;
            RichTextBoxOneView.Text = "";
            RichTextBoxOneView.Visible = false;
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = SystemColors.ControlLight;
            PanelBottom.Controls.Add(RichTextBoxChat);
            PanelBottom.ForeColor = SystemColors.ActiveCaptionText;
            PanelBottom.Location = new Point(0, 549);
            PanelBottom.Margin = new Padding(1);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(824, 136);
            PanelBottom.TabIndex = 40;
            // 
            // ButtonAttach
            // 
            ButtonAttach.Location = new Point(836, 582);
            ButtonAttach.Margin = new Padding(2);
            ButtonAttach.Name = "ButtonAttach";
            ButtonAttach.Padding = new Padding(1);
            ButtonAttach.Size = new Size(150, 33);
            ButtonAttach.TabIndex = 82;
            ButtonAttach.Text = "Attach";
            ButtonAttach.UseVisualStyleBackColor = true;
            ButtonAttach.Click += ButtonAttach_Click;
            // 
            // ButtonClear
            // 
            ButtonClear.Location = new Point(836, 655);
            ButtonClear.Margin = new Padding(2);
            ButtonClear.Name = "ButtonClear";
            ButtonClear.Padding = new Padding(1);
            ButtonClear.Size = new Size(150, 32);
            ButtonClear.TabIndex = 84;
            ButtonClear.Text = "Clear";
            ButtonClear.UseVisualStyleBackColor = true;
            ButtonClear.Click += ButtonClear_Click;
            // 
            // ButtonSend
            // 
            ButtonSend.Location = new Point(836, 618);
            ButtonSend.Margin = new Padding(2);
            ButtonSend.Name = "ButtonSend";
            ButtonSend.Padding = new Padding(1);
            ButtonSend.Size = new Size(150, 35);
            ButtonSend.TabIndex = 83;
            ButtonSend.Text = "Send";
            ButtonSend.UseVisualStyleBackColor = true;
            ButtonSend.Click += ButtonSend_Click;
            // 
            // Peer2PeerChat
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(996, 711);
            Controls.Add(ButtonClear);
            Controls.Add(ButtonSend);
            Controls.Add(ButtonAttach);
            Controls.Add(PanelCenter);
            Controls.Add(PanelDestination);
            Controls.Add(PanelBottom);
            Controls.Add(PanelEnCodeCrypt);
            Name = "Peer2PeerChat";
            Text = "Peer2PeerChat";
            FormClosing += FormClose_Click;
            Controls.SetChildIndex(PanelEnCodeCrypt, 0);
            Controls.SetChildIndex(PanelBottom, 0);
            Controls.SetChildIndex(PanelDestination, 0);
            Controls.SetChildIndex(PanelCenter, 0);
            Controls.SetChildIndex(ButtonAttach, 0);
            Controls.SetChildIndex(ButtonSend, 0);
            Controls.SetChildIndex(ButtonClear, 0);
            SplitChatView.Panel1.ResumeLayout(false);
            SplitChatView.Panel1.PerformLayout();
            SplitChatView.Panel2.ResumeLayout(false);
            SplitChatView.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SplitChatView).EndInit();
            SplitChatView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PictureBoxYou).EndInit();
            PanelEnCodeCrypt.ResumeLayout(false);
            PanelEnCodeCrypt.PerformLayout();
            PanelDestination.ResumeLayout(false);
            PanelCenter.ResumeLayout(false);
            PanelBottom.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private Panel PanelEnCodeCrypt;
        private Panel PanelDestination;
        private Panel PanelCenter;
        private Panel PanelBottom;
        private ComboBox ComboBoxIp;
        private ComboBox ComboBoxSecretKey;
        private ComboBox ComboBoxContacts;
        private SplitContainer SplitChatView;
        private TextBox TextBoxSource;
        private TextBox TextBoxDestionation;
        private TextBox TextBoxPipe;
        private RichTextBox RichTextBoxOneView;
        private RichTextBox RichTextBoxChat;
        private PictureBox PictureBoxYou;
        private Button ButtonKey;
        private Button ButtonAttach;
        private Button ButtonClear;
        private Button ButtonCheck;
        private Controls.GroupBoxLinkLabels GroupBoxLinks;
        private Controls.DragNDropGroupBox dragnDropGroupBox;
        private Controls.AttachmentListControl attachmentListControl;
        private Controls.PeerServerSwitchControl peerServerSwitchControl1;
        private Button ButtonSend;
    
    }

}