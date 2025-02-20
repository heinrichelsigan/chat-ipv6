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
                EventHandler<Area23EventArgs<int>> handler = FireUpChanged;

                Area23EventArgs<int> area23EventArgs = new Area23EventArgs<int>(trackBarPeerServer.Value);
                handler?.Invoke(this, area23EventArgs);
            }
        }
    }
}
