namespace EU.CqrXs.WinForm.SecureChat.Controls.UserControls
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
            LabelServer = new Label();
            LabelPeer = new Label();
            ToolTipInfo = new ToolTip(components);
            TrackBarPeerServer = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)TrackBarPeerServer).BeginInit();
            SuspendLayout();
            // 
            // labLabelServerlServer
            // 
            LabelServer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LabelServer.AutoSize = true;
            LabelServer.BackColor = SystemColors.GradientInactiveCaption;
            LabelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Italic, GraphicsUnit.Point, 0);
            LabelServer.ForeColor = SystemColors.Highlight;
            LabelServer.Location = new Point(78, 1);
            LabelServer.Margin = new Padding(1, 0, 1, 0);
            LabelServer.Name = "LabelServer";
            LabelServer.Size = new Size(73, 16);
            LabelServer.TabIndex = 0;
            LabelServer.Text = "Chat Sever";
            // 
            // LabelPeer
            // 
            LabelPeer.AutoSize = true;
            LabelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold);
            LabelPeer.Location = new Point(1, 1);
            LabelPeer.Margin = new Padding(1, 0, 1, 0);
            LabelPeer.Name = "LabelPeer";
            LabelPeer.Size = new Size(76, 16);
            LabelPeer.TabIndex = 0;
            LabelPeer.Text = "Peer2Peer";
            // 
            // TrackBarPeerServer
            // 
            TrackBarPeerServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TrackBarPeerServer.LargeChange = 2;
            TrackBarPeerServer.Location = new Point(0, 17);
            TrackBarPeerServer.Margin = new Padding(0);
            TrackBarPeerServer.Maximum = 2;
            TrackBarPeerServer.Name = "TrackBarPeerServer";
            TrackBarPeerServer.Size = new Size(151, 45);
            TrackBarPeerServer.TabIndex = 1;
            TrackBarPeerServer.TickStyle = TickStyle.TopLeft;
            TrackBarPeerServer.ValueChanged += TrackBarPeerServer_ValueChanged;
            //
            // ToolTipInfo
            //
            ToolTipInfo.UseAnimation = true;
            ToolTipInfo.UseFading = true;
            ToolTipInfo.ToolTipIcon = ToolTipIcon.Info;
            ToolTipInfo.ToolTipTitle = "Change mode between \"peer 2 peer\" and \"group session server chat\"";
            // 
            // PeerServerSwitchControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            BackColor = SystemColors.GradientActiveCaption;
            Controls.Add(TrackBarPeerServer);
            Controls.Add(LabelServer);
            Controls.Add(LabelPeer);
            Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(0);
            Name = "PeerServerSwitchControl";
            Size = new Size(152, 52);
            MouseEnter += ShowToolTip;
            MouseLeave += HideToolTip;
            ((System.ComponentModel.ISupportInitialize)TrackBarPeerServer).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LabelServer;
        private Label LabelPeer;
        private ToolTip ToolTipInfo;
        private TrackBar TrackBarPeerServer;
    }
}
