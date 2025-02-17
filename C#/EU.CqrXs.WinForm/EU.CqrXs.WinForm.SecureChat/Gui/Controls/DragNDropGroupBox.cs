using Area23.At.Framework.Core.Net.IpSocket;
using Area23.At.Framework.Core.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Controls
{
    public partial class DragNDropGroupBox  : GroupBox
    {

        public EventHandler<Area23EventArgs<string>> OnDragNDrop;
    

        public DragNDropGroupBox()
        {
            InitializeComponent();
        }

        public DragNDropGroupBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void GroupBox_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void GroupBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                DateTime now = DateTime.UtcNow;
                Text = "";
                foreach (string file in files)
                {
                    try
                    {
                        Text += $"+{Path.GetFileName(file)}";
                        if (OnDragNDrop != null)
                        {
                            EventHandler<Area23EventArgs<string>> handler = OnDragNDrop;
                            Area23EventArgs<string> area23EventArgs = new Area23EventArgs<string>(file);
                            handler?.Invoke(this, area23EventArgs);
                        }

                    }
                    catch (Exception ex)
                    {
                        Text = $"Exc:{ex.Message}";
                    }
                    Text += " ";
                }
            }
        }

        private void GroupBox_DragLeave(object sender, EventArgs e)
        {

        }
    }


}
