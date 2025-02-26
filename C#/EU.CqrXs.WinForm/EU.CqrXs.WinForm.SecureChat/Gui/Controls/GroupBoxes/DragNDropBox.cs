using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Util;
using System.ComponentModel;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Controls.GroupBoxes
{
    public partial class DragNDropBox  : GroupBox
    {

        DateTime lastShownToolTip = DateTime.Today;

        public EventHandler<Area23EventArgs<string>>? OnDragNDrop;

        #region constructors

        public DragNDropBox()
        {
            components = new System.ComponentModel.Container();
            InitializeComponent();
            MiniToolBox.CreateAttachDirectory();
        }

        public DragNDropBox(IContainer container)
        {
            if (container == null)
                container = new System.ComponentModel.Container();
            container.Add(this);
            components = container;
            InitializeComponent();
            MiniToolBox.CreateAttachDirectory();
        }

        public DragNDropBox(string headerText, IContainer container) : this(container) 
        {
            this.Text = headerText;
        }

        #endregion constructors

        #region DragNDrop

        private void DragNDropBox_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void DragNDropBox_DragDrop(object sender, DragEventArgs e)
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

        private void DragNDropBox_DragLeave(object sender, EventArgs e)
        {

        }

        #endregion DragNDrop

        #region show / hide tooltip

        private void DragNDropBox_MouseEnter(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastShownToolTip).TotalSeconds > 60)
            {
                // this.toolTip1.SetToolTip(this, "Drag'n Drop Files here or click on \"Attach\"");
                toolTip1.Show("Drag'n Drop Files here or click on \"Attach\"", this, 36, 72, 6000);
                lastShownToolTip = DateTime.Now;
            }
        }

        private void DragNDropBox_MouseLeave(object sender, EventArgs e)
        {
            System.Timers.Timer tPerformToolTipHide = new System.Timers.Timer { Interval = 1500 };
            tPerformToolTipHide.Elapsed += (s, en) =>
            {
                this.Invoke(new Action(() =>
                {
                    toolTip1.Hide(this);
                }));
                tPerformToolTipHide.Stop(); // Stop the timer(otherwise keeps on calling)
            };
            tPerformToolTipHide.Start();
        }


        #endregion show / hide tooltip


        public void SetHeaderText(string headerText)
        {
            this.Text = headerText;
        }
    
    }


}
