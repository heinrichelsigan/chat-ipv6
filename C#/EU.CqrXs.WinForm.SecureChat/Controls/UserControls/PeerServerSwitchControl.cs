using Area23.At.Framework.Core.Util;
using System.ComponentModel;

namespace EU.CqrXs.WinForm.SecureChat.Controls.UserControls
{
    public partial class PeerServerSwitchControl : UserControl
    {
        DateTime lastShownToolTip = DateTime.Today;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventHandler<Area23EventArgs<int>>? FireUpChanged {  get; set; }

        #region constructors

        public PeerServerSwitchControl()
        {
            components = new System.ComponentModel.Container();
            InitializeComponent();
        }

        public PeerServerSwitchControl(IContainer container)
        {
            if (container == null)
                container = new System.ComponentModel.Container();
            container.Add(this);
            components = container;
            InitializeComponent();
        }

        #endregion constructors

        private void TrackBarPeerServer_ValueChanged(object sender, EventArgs e)
        {
            if (FireUpChanged != null)
            {
                SuspendResumeLayout(false);

                switch (TrackBarPeerServer.Value)
                {
                    case 1:
                        LabelServer.Location = new Point(72, 1);
                        LabelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
                        LabelServer.ForeColor = SystemColors.ControlText;
                        LabelServer.BackColor = SystemColors.ActiveCaption;
                        LabelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Regular);
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
                    case 0:
                    default:
                        LabelServer.Location = new Point(78, 1);
                        LabelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Italic, GraphicsUnit.Point, 0);
                        LabelServer.ForeColor = SystemColors.GrayText;
                        LabelServer.BackColor = SystemColors.InactiveCaption;
                        LabelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold);
                        LabelPeer.ForeColor = SystemColors.ControlText;
                        LabelPeer.BackColor = SystemColors.ActiveCaption;
                        break;
                }

                SuspendResumeLayout(true);

                EventHandler<Area23EventArgs<int>> handler = FireUpChanged;
                Area23EventArgs<int> area23EventArgs = new Area23EventArgs<int>(TrackBarPeerServer.Value);
                handler?.Invoke(this, area23EventArgs);


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
                //        Area23EventArgs<int> area23EventArgs = new Area23EventArgs<int>(trackBarPeerServer.Value);
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
