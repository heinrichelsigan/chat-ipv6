using Area23.At.Framework.Core.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Controls
{
    public partial class PeerServerSwitchControl : UserControl
    {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventHandler<Area23EventArgs<int>> FireUpChanged {  get; set; }    

        public PeerServerSwitchControl()
        {
            InitializeComponent();
        }



        private void trackBarPeerServer_ValueChanged(object sender, EventArgs e)
        {
            if (FireUpChanged != null)
            {
                switch (trackBarPeerServer.Value)
                {
                    case 1:
                        labelServer.Location = new Point(72, 1);
                        labelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
                        labelServer.ForeColor = SystemColors.ControlText;
                        labelServer.BackColor = SystemColors.ActiveCaption;
                        labelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Regular);
                        labelPeer.ForeColor = SystemColors.ControlText;
                        labelPeer.BackColor = SystemColors.ActiveCaption;    
                        break;
                    case 2:
                        labelServer.Location = new Point(68, 1);
                        labelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold); 
                        labelServer.ForeColor = SystemColors.ControlText;
                        labelServer.BackColor = SystemColors.ActiveCaption;                        
                        labelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Italic, GraphicsUnit.Point, 0);
                        labelPeer.ForeColor = SystemColors.GrayText; 
                        labelPeer.BackColor = SystemColors.InactiveCaption;
                        break;
                    case 0:
                    default:
                        labelServer.Location = new Point(78, 1);
                        labelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Italic, GraphicsUnit.Point, 0);                        
                        labelServer.ForeColor = SystemColors.GrayText;
                        labelServer.BackColor = SystemColors.InactiveCaption;
                        labelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold);
                        labelPeer.ForeColor = SystemColors.ControlText;
                        labelPeer.BackColor = SystemColors.ActiveCaption;
                        break;
                }

                EventHandler<Area23EventArgs<int>> handler = FireUpChanged;
                Area23EventArgs<int> area23EventArgs = new Area23EventArgs<int>(trackBarPeerServer.Value);
                handler?.Invoke(this, area23EventArgs);


                System.Timers.Timer tLoadSwitchImage = new System.Timers.Timer { Interval = 800 };
                tLoadSwitchImage.Elapsed += (s, en) =>
                {
                    this.Invoke(new Action(() =>
                    {
                        this.SuspendLayout();
                        this.labelPeer.SuspendLayout();
                        this.labelServer.SuspendLayout();
                        this.trackBarPeerServer.SuspendLayout();

                        this.trackBarPeerServer.Value = 0;                        
                        labelServer.Location = new Point(78, 1);
                        labelServer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Italic, GraphicsUnit.Point, 0);
                        labelServer.ForeColor = SystemColors.GrayText;
                        labelServer.BackColor = SystemColors.InactiveCaption;
                        labelPeer.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold);
                        labelPeer.ForeColor = SystemColors.ControlText;
                        labelPeer.BackColor = SystemColors.ActiveCaption;

                        this.labelPeer.ResumeLayout();
                        this.labelServer.ResumeLayout();
                        this.trackBarPeerServer.ResumeLayout(true);
                        this.ResumeLayout(true);
                        this.PerformLayout();


                        EventHandler<Area23EventArgs<int>> hnd = FireUpChanged;
                        Area23EventArgs<int> area23EventArgs = new Area23EventArgs<int>(trackBarPeerServer.Value);
                        hnd?.Invoke(this, area23EventArgs);
                    }));
                    tLoadSwitchImage.Stop(); // Stop the timer(otherwise keeps on calling)
                };
                tLoadSwitchImage.Start();


            }
        }
    }
}
