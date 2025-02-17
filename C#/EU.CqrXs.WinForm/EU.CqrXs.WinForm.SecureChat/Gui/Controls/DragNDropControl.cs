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
    public partial class DragNDropControl : UserControl
    {
        public DragNDropControl()
        {
            InitializeComponent();
        }

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Control_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                DateTime now = DateTime.UtcNow;
                labelInfo.Text = "";
                foreach (string file in files)
                {
                    try
                    {
                        labelInfo.Text += $"+{Path.GetFileName(file)}";
                    }
                    catch (Exception ex)
                    {
                        labelInfo.Text = $"Exc:{ex.Message}"; 
                    }
                    labelInfo.Text += " ";
                }
                toolTipInfo.ToolTipTitle = labelInfo.Text;
                toolTipInfo.SetToolTip(labelInfo, toolTipInfo.ToolTipTitle);
                toolTipInfo.ShowAlways = true;
            }
        }

        private void Control_DragLeave(object sender, EventArgs e)
        {

        }
    }
}
