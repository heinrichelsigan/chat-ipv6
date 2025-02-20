namespace EU.CqrXs.WinForm.SecureChat.Gui.Controls
{
    partial class PeerServerSwitchControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            labelServer = new Label();
            labelPeer = new Label();
            toolTipInfo = new ToolTip(components);
            trackBarPeerServer = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)trackBarPeerServer).BeginInit();
            SuspendLayout();
            // 
            // labelServer
            // 
            labelServer.AutoSize = true;
            labelServer.Font = new Font("Lucida Sans Unicode", 9.5F);
            labelServer.Location = new Point(78, 1);
            labelServer.Margin = new Padding(1, 0, 1, 0);
            labelServer.Name = "labelServer";
            labelServer.Size = new Size(73, 16);
            labelServer.TabIndex = 0;
            labelServer.Text = "Chat Sever";
            // 
            // labelPeer
            // 
            labelPeer.AutoSize = true;
            labelPeer.Font = new Font("Lucida Sans Unicode", 9.5F);
            labelPeer.Location = new Point(1, 1);
            labelPeer.Margin = new Padding(1, 0, 1, 0);
            labelPeer.Name = "labelPeer";
            labelPeer.Size = new Size(75, 16);
            labelPeer.TabIndex = 0;
            labelPeer.Text = "Peer 2 Peer";
            // 
            // trackBarPeerServer
            // 
            trackBarPeerServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarPeerServer.LargeChange = 2;
            trackBarPeerServer.Location = new Point(0, 17);
            trackBarPeerServer.Margin = new Padding(0);
            trackBarPeerServer.Maximum = 2;
            trackBarPeerServer.Name = "trackBarPeerServer";
            trackBarPeerServer.Size = new Size(151, 45);
            trackBarPeerServer.TabIndex = 1;
            trackBarPeerServer.TickStyle = TickStyle.TopLeft;
            trackBarPeerServer.ValueChanged += trackBarPeerServer_ValueChanged;
            // 
            // PeerServerSwitchControl
            // 
            AllowDrop = true;
            AutoScaleMode = AutoScaleMode.None;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            BackColor = SystemColors.GradientActiveCaption;
            Controls.Add(trackBarPeerServer);
            Controls.Add(labelServer);
            Controls.Add(labelPeer);
            Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(0);
            Name = "PeerServerSwitchControl";
            Size = new Size(152, 52);
            ((System.ComponentModel.ISupportInitialize)trackBarPeerServer).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelServer;
        private Label labelPeer;
        private ToolTip toolTipInfo;
        private TrackBar trackBarPeerServer;
    }
}
