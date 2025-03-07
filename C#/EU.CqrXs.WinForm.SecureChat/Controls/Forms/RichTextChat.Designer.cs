using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Net.WebHttp;
using System.Drawing.Imaging;
using System.Net;
using System.Windows.Forms.Design;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{


    partial class RichTextChat
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
            StripMenu = new Menu.CqrMenu();
            StripStatus = new StatusStrip();
            StripStatusLabel = new ToolStripStatusLabel();
            StripProgressBar = new ToolStripProgressBar();
            SplitChatView = new SplitContainer();
            TextBoxSource = new TextBox();
            TextBoxDestionation = new TextBox();
            PictureBoxYou = new PictureBox();
            ButtonKey = new Button();
            PanelEnCodeCrypt = new Panel();
            TextBoxPipe = new TextBox();
            ButtonCheck = new Button();
            textBoxChatSession = new TextBox();
            ComboBoxContacts = new ComboBox();
            ComboBoxIp = new ComboBox();
            ComboBoxSecretKey = new ComboBox();
            RichTextBoxChat = new RichTextBox();
            PanelDestination = new Panel();
            PeerServerSwitch = new Panels.PeerServerSwitchPanel(components);
            ButtonSend = new Button();
            ButtonAttach = new Button();
            DragNDropGroupBox = new GroupBoxes.DragNDropBox(components);
            LinkedLabelsBox = new GroupBoxes.LinkLabelsBox(components);
            PanelCenter = new Panel();
            RichTextBoxOneView = new RichTextBox();
            PanelBottom = new Panel();
            StripMenu.SuspendLayout();
            StripStatus.SuspendLayout();
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
            // StripMenu
            // 
            StripMenu.AllowItemReorder = true;
            StripMenu.BackColor = SystemColors.MenuBar;
            StripMenu.Font = new Font("Lucida Sans Unicode", 10F);
            StripMenu.GripStyle = ToolStripGripStyle.Visible;
            // StripMenu.Items.AddRange(new ToolStripItem[] { MenuFile, MenuView, MenuNetwork, MenuCommands, MenuContacts, MenuOptions, MenuHelp });
            StripMenu.Location = new Point(0, 0);
            StripMenu.Name = "StripMenu";
            StripMenu.RenderMode = ToolStripRenderMode.System;
            StripMenu.Size = new Size(998, 25);
            StripMenu.TabIndex = 1;
            StripMenu.Visible = true;
            StripMenu.Text = "StripMenu";
            // 
            // StripStatus
            // 
            StripStatus.GripMargin = new Padding(1);
            StripStatus.Items.AddRange(new ToolStripItem[] { StripStatusLabel, StripProgressBar });
            StripStatus.Location = new Point(0, 695);
            StripStatus.Name = "StripStatus";
            StripStatus.Size = new Size(998, 22);
            StripStatus.TabIndex = 102;
            StripStatus.Text = "StripStatus";
            // 
            // StripStatusLabel
            // 
            StripStatusLabel.AutoSize = false;
            StripStatusLabel.BackColor = SystemColors.Info;
            StripStatusLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            StripStatusLabel.BorderStyle = Border3DStyle.SunkenInner;
            StripStatusLabel.LinkVisited = true;
            StripStatusLabel.Margin = new Padding(0, 2, 0, 1);
            StripStatusLabel.Name = "StripStatusLabel";
            StripStatusLabel.Size = new Size(420, 19);
            StripStatusLabel.Text = "Status";
            // 
            // StripProgressBar
            // 
            StripProgressBar.AutoSize = false;
            StripProgressBar.Margin = new Padding(1);
            StripProgressBar.Name = "StripProgressBar";
            StripProgressBar.Size = new Size(548, 20);
            StripProgressBar.Step = 104;
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
            PictureBoxYou.BackColor = SystemColors.ButtonShadow;
            PictureBoxYou.BackgroundImageLayout = ImageLayout.None;
            PictureBoxYou.Location = new Point(7, 58);
            PictureBoxYou.Margin = new Padding(1);
            PictureBoxYou.Name = "PictureBoxYou";
            PictureBoxYou.Padding = new Padding(1);
            PictureBoxYou.Size = new Size(155, 155);
            PictureBoxYou.SizeMode = PictureBoxSizeMode.StretchImage;
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
            ButtonKey.Location = new Point(306, 3);
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
            PanelEnCodeCrypt.Controls.Add(TextBoxPipe);
            PanelEnCodeCrypt.Controls.Add(ButtonCheck);
            PanelEnCodeCrypt.Controls.Add(textBoxChatSession);
            PanelEnCodeCrypt.Controls.Add(ComboBoxContacts);
            PanelEnCodeCrypt.Controls.Add(ComboBoxIp);
            PanelEnCodeCrypt.Controls.Add(ComboBoxSecretKey);
            PanelEnCodeCrypt.Controls.Add(ButtonKey);
            PanelEnCodeCrypt.ForeColor = SystemColors.WindowText;
            PanelEnCodeCrypt.Location = new Point(0, 28);
            PanelEnCodeCrypt.Margin = new Padding(0);
            PanelEnCodeCrypt.Name = "PanelEnCodeCrypt";
            PanelEnCodeCrypt.Size = new Size(994, 64);
            PanelEnCodeCrypt.TabIndex = 10;
            // 
            // TextBoxPipe
            // 
            TextBoxPipe.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TextBoxPipe.HideSelection = false;
            TextBoxPipe.Location = new Point(353, 4);
            TextBoxPipe.Margin = new Padding(1);
            TextBoxPipe.Name = "TextBoxPipe";
            TextBoxPipe.ReadOnly = true;
            TextBoxPipe.Size = new Size(93, 26);
            TextBoxPipe.TabIndex = 13;
            // 
            // ButtonCheck
            // 
            ButtonCheck.BackColor = SystemColors.ButtonHighlight;
            ButtonCheck.BackgroundImageLayout = ImageLayout.Center;
            ButtonCheck.Font = new Font("Lucida Sans Unicode", 10F, FontStyle.Bold);
            ButtonCheck.ForeColor = SystemColors.ActiveCaptionText;
            ButtonCheck.Image = Properties.de.Resources.CableWireCut;
            ButtonCheck.Location = new Point(306, 30);
            ButtonCheck.Margin = new Padding(1);
            ButtonCheck.Name = "ButtonCheck";
            ButtonCheck.Padding = new Padding(1);
            ButtonCheck.Size = new Size(40, 32);
            ButtonCheck.TabIndex = 17;
            ButtonCheck.UseVisualStyleBackColor = false;
            // 
            // textBoxChatSession
            // 
            textBoxChatSession.Font = new Font("Lucida Sans Unicode", 8F);
            textBoxChatSession.HideSelection = false;
            textBoxChatSession.Location = new Point(456, 6);
            textBoxChatSession.Margin = new Padding(1);
            textBoxChatSession.Name = "textBoxChatSession";
            textBoxChatSession.Size = new Size(368, 24);
            textBoxChatSession.TabIndex = 19;
            // 
            // ComboBoxContacts
            // 
            ComboBoxContacts.BackColor = SystemColors.ControlLightLight;
            ComboBoxContacts.Enabled = false;
            ComboBoxContacts.Font = new Font("Lucida Sans Unicode", 9F);
            ComboBoxContacts.ForeColor = SystemColors.ControlText;
            ComboBoxContacts.FormattingEnabled = true;
            ComboBoxContacts.Location = new Point(456, 35);
            ComboBoxContacts.Margin = new Padding(1);
            ComboBoxContacts.Name = "ComboBoxContacts";
            ComboBoxContacts.Size = new Size(369, 24);
            ComboBoxContacts.TabIndex = 18;
            ComboBoxContacts.Text = "[enter peer IPv4 or IPv6 for directly connect]";
            ComboBoxContacts.SelectedIndexChanged += ComboBoxContacts_SelectedIndexChanged;
            ComboBoxContacts.Leave += ComboBoxContacts_FocusLeave;
            // 
            // ComboBoxIp
            // 
            ComboBoxIp.BackColor = SystemColors.ControlLightLight;
            ComboBoxIp.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ComboBoxIp.ForeColor = SystemColors.ControlText;
            ComboBoxIp.FormattingEnabled = true;
            ComboBoxIp.Location = new Point(4, 35);
            ComboBoxIp.Margin = new Padding(1);
            ComboBoxIp.Name = "ComboBoxIp";
            ComboBoxIp.Size = new Size(297, 24);
            ComboBoxIp.TabIndex = 15;
            ComboBoxIp.Text = "[enter peer IPv4/IPv6]";
            ComboBoxIp.SelectedIndexChanged += ComboBoxIp_SelectedIndexChanged;
            ComboBoxIp.Leave += ComboBoxIp_FocusLeave;
            // 
            // ComboBoxSecretKey
            // 
            ComboBoxSecretKey.BackColor = SystemColors.ControlLightLight;
            ComboBoxSecretKey.ForeColor = SystemColors.ControlText;
            ComboBoxSecretKey.FormattingEnabled = true;
            ComboBoxSecretKey.Location = new Point(3, 4);
            ComboBoxSecretKey.Margin = new Padding(1);
            ComboBoxSecretKey.Name = "ComboBoxSecretKey";
            ComboBoxSecretKey.Size = new Size(297, 24);
            ComboBoxSecretKey.TabIndex = 11;
            ComboBoxSecretKey.Text = "[enter secret key here]";
            ComboBoxSecretKey.SelectedIndexChanged += ComboBoxSecretKey_SelectedIndexChanged;
            ComboBoxSecretKey.TextUpdate += ComboBoxSecretKey_TextUpdate;
            ComboBoxSecretKey.Leave += ComboBoxSecretKey_FocusLeave;
            // 
            // RichTextBoxChat
            // 
            RichTextBoxChat.BackColor = SystemColors.ButtonHighlight;
            RichTextBoxChat.BorderStyle = BorderStyle.FixedSingle;
            RichTextBoxChat.ForeColor = SystemColors.WindowText;
            RichTextBoxChat.Location = new Point(3, 4);
            RichTextBoxChat.Margin = new Padding(1);
            RichTextBoxChat.Name = "RichTextBoxChat";
            RichTextBoxChat.Size = new Size(820, 123);
            RichTextBoxChat.TabIndex = 41;
            RichTextBoxChat.Text = "";
            // 
            // PanelDestination
            // 
            PanelDestination.BackColor = SystemColors.ActiveCaption;
            PanelDestination.Controls.Add(PeerServerSwitch);
            PanelDestination.Controls.Add(ButtonSend);
            PanelDestination.Controls.Add(ButtonAttach);
            PanelDestination.Controls.Add(DragNDropGroupBox);
            PanelDestination.Controls.Add(LinkedLabelsBox);
            PanelDestination.Controls.Add(PictureBoxYou);
            PanelDestination.ForeColor = SystemColors.ActiveCaptionText;
            PanelDestination.Location = new Point(826, 32);
            PanelDestination.Margin = new Padding(0);
            PanelDestination.Name = "PanelDestination";
            PanelDestination.Size = new Size(168, 663);
            PanelDestination.TabIndex = 70;
            // 
            // PeerServerSwitch
            // 
            PeerServerSwitch.AllowDrop = true;
            PeerServerSwitch.BackColor = SystemColors.GradientActiveCaption;
            PeerServerSwitch.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PeerServerSwitch.Location = new Point(8, 2);
            PeerServerSwitch.Margin = new Padding(1);
            PeerServerSwitch.Name = "PeerServerSwitch";
            PeerServerSwitch.Padding = new Padding(1);
            PeerServerSwitch.Size = new Size(152, 48);
            PeerServerSwitch.TabIndex = 84;
            // 
            // ButtonSend
            // 
            ButtonSend.Font = new Font("Lucida Sans Unicode", 9F);
            ButtonSend.Location = new Point(8, 633);
            ButtonSend.Margin = new Padding(1);
            ButtonSend.Name = "ButtonSend";
            ButtonSend.Padding = new Padding(1);
            ButtonSend.Size = new Size(75, 27);
            ButtonSend.TabIndex = 83;
            ButtonSend.Text = "Send";
            ButtonSend.UseVisualStyleBackColor = true;
            ButtonSend.Click += ButtonSend_Click;
            // 
            // ButtonAttach
            // 
            ButtonAttach.Font = new Font("Lucida Sans Unicode", 9F);
            ButtonAttach.Location = new Point(89, 633);
            ButtonAttach.Margin = new Padding(1);
            ButtonAttach.Name = "ButtonAttach";
            ButtonAttach.Padding = new Padding(1);
            ButtonAttach.Size = new Size(75, 27);
            ButtonAttach.TabIndex = 82;
            ButtonAttach.Text = "Attach";
            ButtonAttach.UseVisualStyleBackColor = true;
            ButtonAttach.Click += ButtonAttach_Click;
            // 
            // DragNDropGroupBox
            // 
            DragNDropGroupBox.AllowDrop = true;
            DragNDropGroupBox.BackColor = SystemColors.AppWorkspace;
            DragNDropGroupBox.Font = new Font("Lucida Sans Unicode", 8.5F);
            DragNDropGroupBox.ForeColor = SystemColors.ActiveCaptionText;
            DragNDropGroupBox.Location = new Point(8, 503);
            DragNDropGroupBox.Margin = new Padding(1);
            DragNDropGroupBox.Name = "DragNDropGroupBox";
            DragNDropGroupBox.Padding = new Padding(1);
            DragNDropGroupBox.Size = new Size(154, 127);
            DragNDropGroupBox.TabIndex = 81;
            DragNDropGroupBox.TabStop = false;
            DragNDropGroupBox.Text = "   Drag'N'Drop Box";
            // 
            // LinkedLabelsBox
            // 
            LinkedLabelsBox.AllowDrop = true;
            LinkedLabelsBox.BackColor = SystemColors.GradientActiveCaption;
            LinkedLabelsBox.Font = new Font("Lucida Sans Unicode", 9F);
            LinkedLabelsBox.Location = new Point(4, 223);
            LinkedLabelsBox.Margin = new Padding(0);
            LinkedLabelsBox.Name = "LinkedLabelsBox";
            LinkedLabelsBox.Padding = new Padding(0);
            LinkedLabelsBox.Size = new Size(160, 279);
            LinkedLabelsBox.TabIndex = 83;
            LinkedLabelsBox.TabStop = false;
            LinkedLabelsBox.Text = "Attachments";
            // 
            // PanelCenter
            // 
            PanelCenter.Controls.Add(SplitChatView);
            PanelCenter.Controls.Add(RichTextBoxOneView);
            PanelCenter.Location = new Point(8, 102);
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
            PanelBottom.BackColor = SystemColors.ActiveCaption;
            PanelBottom.Controls.Add(RichTextBoxChat);
            PanelBottom.ForeColor = SystemColors.ActiveCaptionText;
            PanelBottom.Location = new Point(0, 565);
            PanelBottom.Margin = new Padding(1);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(824, 130);
            PanelBottom.TabIndex = 40;
            // 
            // RichTextChat
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(998, 717);
            Controls.Add(PanelCenter);
            Controls.Add(PanelDestination);
            Controls.Add(PanelBottom);
            Controls.Add(PanelEnCodeCrypt);
            Controls.Add(StripStatus);
            Controls.Add(StripMenu);
            Font = new Font("Lucida Sans Unicode", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = StripMenu;
            Name = "RichTextChat";
            SizeGripStyle = SizeGripStyle.Show;
            Text = "RichTextChat";
            FormClosing += FormClose_Click;
            Load += RichTextChat_Load;
            StripMenu.ResumeLayout(false);
            StripMenu.PerformLayout();
            StripStatus.ResumeLayout(false);
            StripStatus.PerformLayout();
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
        private Button ButtonCheck;
        private Controls.GroupBoxes.LinkLabelsBox GroupBoxLinks;
        private Controls.Menu.CqrMenu StripMenu;
        private StatusStrip StripStatus;
        private ToolStripStatusLabel StripStatusLabel;

        private Controls.GroupBoxes.DragNDropBox DragNDropGroupBox;
        private ToolStripProgressBar StripProgressBar;
        private Controls.GroupBoxes.LinkLabelsBox LinkedLabelsBox;
        private Controls.Panels.PeerServerSwitchPanel PeerServerSwitch;
        private Button ButtonSend;
        private TextBox textBoxChatSession;
    
    }

}