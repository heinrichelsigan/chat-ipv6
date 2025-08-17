using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Panels
{
    public partial class PeerServerSwitchPanel : Panel
    {
        DateTime lastShownToolTip = DateTime.Today;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventHandler<Area23EventArgs<int>>? FireUpChanged { get; set; }

        public delegate void SetTrackBarPeerServerEnabledCallback(TrackBar track, bool enabled);
        public delegate void SetTrackBarPeerServerValueCallback(TrackBar track, int val);        

        public void SetTrackBarPeerServerEnabled(TrackBar trackbar, bool enable)
        {
            bool ena = enable;
            if (trackbar.InvokeRequired)
            {
                SetTrackBarPeerServerEnabledCallback setTrackBarPeerServerEnabledCallback = delegate (TrackBar track, bool enab)
                {
                    if (track != null) track.Enabled = enab;
                };
                try
                {
                    trackbar.Invoke(setTrackBarPeerServerEnabledCallback, new object[] { trackbar, ena });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate SetTrackBarPeerServerEnabled enabled: \"{ena}\".\n", exDelegate);
                }
            }
            else
            {
                if (trackbar != null)
                    trackbar.Enabled = ena;
            }
        }

        public void SetTrackBarPeerServerValue(TrackBar trackbar, int trackValue)
        {
            if (trackbar.InvokeRequired)
            {
                SetTrackBarPeerServerValueCallback setTrackBarPeerServerValue = delegate (TrackBar track, int val)
                {
                    if (track != null) track.Value = val;
                };
                try
                {
                    trackbar.Invoke(setTrackBarPeerServerValue, new object[] { trackbar, trackValue });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate SetTrackBarPeerServerValue val: \"{trackValue}\".\n", exDelegate);
                }
            }
            else
            {
                if (trackbar != null)
                    trackbar.Value = trackValue;
            }
        }

        public PeerServerSwitchPanel()
        {
            components = new System.ComponentModel.Container();
            components.Add(this);
            InitializeComponent();
            if (Entities.Settings.Singleton.OnlyPeer2PeerChat)
            {
                TrackBarPeerServer.Value = 0;
                TrackBarPeerServer_ValueChanged(0, new Area23EventArgs<string>("PeerServerSwitchPanel()"));
                TrackBarPeerServer.Enabled = false;
            }
        }

        public PeerServerSwitchPanel(IContainer container)
        {
            if (container == null)
                container = new System.ComponentModel.Container();
            container.Add(this);
            components = container;
            InitializeComponent();
            if (Entities.Settings.Singleton.OnlyPeer2PeerChat)
            {
                TrackBarPeerServer.Value = 0;
                TrackBarPeerServer_ValueChanged(0, new Area23EventArgs<string>("PeerServerSwitchPanel()"));
                TrackBarPeerServer.Enabled = false;
            }
        }

        public void SetPeerServerSessionTriState(PeerSession3State peerSession3State = PeerSession3State.None, bool fireUp = true)
        {
            switch (peerSession3State)
            {
                case PeerSession3State.Peer2Peer:
                    SetTrackBarPeerServerValue(TrackBarPeerServer, 0);
                    break;
                case PeerSession3State.ChatServer:
                    SetTrackBarPeerServerValue(TrackBarPeerServer, 2);
                    break;
                case PeerSession3State.None:
                default:
                    SetTrackBarPeerServerValue(TrackBarPeerServer, 1);
                    break;
            }
            if (fireUp)
                TrackBarPeerServer_ValueChanged("SetPeerServerSessionTriState", new EventArgs());
        }

        private void TrackBarPeerServer_ValueChanged(object sender, EventArgs e)
        {
            if (FireUpChanged != null)
            {
                SuspendResumeLayout(false);
                switch (TrackBarPeerServer.Value)
                {
                    case 0:
                        LabelServer.Location = new Point(78, 1);
                        LabelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Italic, GraphicsUnit.Point, 0);
                        LabelServer.ForeColor = SystemColors.GrayText;
                        LabelPeer.BackColor = SystemColors.InactiveCaption;
                        LabelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold);
                        LabelPeer.ForeColor = SystemColors.ControlText;
                        LabelPeer.BackColor = SystemColors.ActiveCaption;
                        break;
                    case 2:
                        LabelServer.Location = new Point(68, 1);
                        LabelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold);
                        LabelServer.ForeColor = SystemColors.ControlText;
                        LabelServer.BackColor = SystemColors.ActiveCaption;
                        LabelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Italic, GraphicsUnit.Point, 0);
                        LabelPeer.ForeColor = SystemColors.GrayText;
                        LabelPeer.BackColor = SystemColors.InactiveCaption;
                        break;                    
                    case 1:
                    default:
                        LabelServer.Location = new Point(72, 1);
                        LabelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
                        LabelServer.ForeColor = SystemColors.ControlText;
                        LabelServer.BackColor = SystemColors.ActiveCaption;
                        LabelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Regular);
                        LabelPeer.ForeColor = SystemColors.ControlText;
                        LabelPeer.BackColor = SystemColors.ActiveCaption;
                        break;                    
                }
                SuspendResumeLayout(true);

                if ((sender != null && !sender.ToString().Equals("SetPeerServerSessionTriState")) || sender == null)
                {

                    EventHandler<Area23EventArgs<int>> handler = FireUpChanged;
                    Area23EventArgs<int> area23EventArgs = new Area23EventArgs<int>(TrackBarPeerServer.Value);
                    handler?.Invoke(this, area23EventArgs);
                }


                #region old code switch always back to peer 2 peer

                //System.Timers.Timer tLoadSwitchImage = new System.Timers.Timer { Interval = 800 };
                //tLoadSwitchImage.Elapsed += (s, en) =>
                //{
                //    this.Invoke(new Action(() =>
                //    {
                //        this.SuspendLayout();
                //        this.LabelPeer.SuspendLayout();
                //        this.LabelServer.SuspendLayout();
                //        this.TrackBarPeerServer.SuspendLayout();

                //        this.TrackBarPeerServer.Value = 0;                        
                //        LabelServer.Location = new Point(78, 1);
                //        LabelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Italic, GraphicsUnit.Point, 0);
                //        LabelServer.ForeColor = SystemColors.GrayText;
                //        LabelServer.BackColor = SystemColors.InactiveCaption;
                //        LabelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold);
                //        LabelPeer.ForeColor = SystemColors.ControlText;
                //        LabelPeer.BackColor = SystemColors.ActiveCaption;

                //        this.LabelPeer.ResumeLayout();
                //        this.LabelServer.ResumeLayout();
                //        this.TrackBarPeerServer.ResumeLayout(true);
                //        this.ResumeLayout(true);
                //        this.PerformLayout();


                //        EventHandler<Area23EventArgs<int>> hnd = FireUpChanged;
                //        Area23EventArgs<int> area23EventArgs = new Area23EventArgs<int>(TrackBarPeerServer.Value);
                //        hnd?.Invoke(this, area23EventArgs);
                //    }));
                //    tLoadSwitchImage.Stop(); // Stop the timer(otherwise keeps on calling)
                //};
                //tLoadSwitchImage.Start();

                #endregion old code switch always back to peer 2 peer

            }

        }

        /// <summary>
        /// Suspends or resumes control layout mode
        /// </summary>
        /// <param name="perfromLayout">if <see cref="false"/>, suspends layout <see cref="Control.SuspendLayout()"/> for all controls
        /// if <see cref="true"/>, then <see cref="SuspendResumeLayout(bool)"/> for all controls</param>
        private void SuspendResumeLayout(bool perfromLayout = false)
        {
            if (!perfromLayout)
            {
                this.SuspendLayout();
                this.LabelPeer.SuspendLayout();
                this.LabelServer.SuspendLayout();
                this.TrackBarPeerServer.SuspendLayout();
            }
            else
            {
                this.LabelPeer.ResumeLayout();
                this.LabelServer.ResumeLayout();
                this.TrackBarPeerServer.ResumeLayout(true);
                this.ResumeLayout(true);
                this.PerformLayout();
            }
        }

        internal void SetTrackSwitchEnabled(bool enable)
        {
            SetTrackBarPeerServerEnabled(TrackBarPeerServer, enable);
        }

        #region show / hide tooltip

        private void ShowToolTip(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastShownToolTip).TotalSeconds > 60)
            {
                // this.toolTip1.SetToolTip(this, "Drag'n Drop Files here or click on \"Attach\"");
                this.ToolTipInfo.Show("Change mode between \"peer 2 peer\" and \"group session server chat\"", this, 36, 72, 6000);
                lastShownToolTip = DateTime.Now;
            }
        }

        private void HideToolTip(object sender, EventArgs e)
        {
            System.Timers.Timer tPerformToolTipHide = new System.Timers.Timer { Interval = 1500 };
            tPerformToolTipHide.Elapsed += (s, en) =>
            {
                this.Invoke(new Action(() =>
                {
                    ToolTipInfo.Hide(this);
                }));
                tPerformToolTipHide.Stop(); // Stop the timer(otherwise keeps on calling)
            };
            tPerformToolTipHide.Start();
        }


        #endregion show / hide tooltip

    }

}

