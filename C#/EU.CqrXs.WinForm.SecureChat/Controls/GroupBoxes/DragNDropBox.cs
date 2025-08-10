using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Util;
using Microsoft.VisualBasic.Logging;
using System.ComponentModel;
using WinRT;

namespace EU.CqrXs.WinForm.SecureChat.Controls.GroupBoxes
{
    public partial class DragNDropBox  : GroupBox
    {

        DateTime lastShownToolTip = DateTime.Today;

        #region Eventhandler and delegate callbacks

        public EventHandler<Area23EventArgs<string>>? OnDragNDrop;        

        internal delegate string GetGroupBoxTextCallback(GroupBox groupBox);
        internal delegate string SetGroupBoxTextCallback(GroupBox groupBox, string text);
        internal delegate string SetCtrlTextCallback(string text);

        internal string GetGroupBoxText(System.Windows.Forms.GroupBox groupBox)
        {
            string textToGet = string.Empty;

            if (groupBox.InvokeRequired)
            {
                GetGroupBoxTextCallback getTextDelegate = delegate (System.Windows.Forms.GroupBox gBox)
                {
                    return (gBox != null && gBox.Text != null) ? gBox.Text : string.Empty;
                };
                try
                {
                    object? oget = groupBox.Invoke(getTextDelegate, new object[] { groupBox });
                    if (oget != null)
                        textToGet = (string)oget;
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate GetGroupBoxText: \"{groupBox.Name}\".\n", exDelegate);
                }
            }
            else
            {
                if (groupBox != null && groupBox.Text != null)
                    textToGet = groupBox.Text;
            }

            return textToGet;
        }

        internal void SetGroupBoxText(System.Windows.Forms.GroupBox groupBox, string text)
        {
            string textToSet = (!string.IsNullOrEmpty(text)) ? text : string.Empty;
            if (groupBox.InvokeRequired)
            {
                SetGroupBoxTextCallback setBoxTextDelegate = delegate (System.Windows.Forms.GroupBox gBox, string setText)
                {
                    return (gBox != null && gBox.Name != null && !string.IsNullOrEmpty(setText)) ? gBox.Text : string.Empty;
                };
                try
                {
                    groupBox.Invoke(setBoxTextDelegate, new object[] { groupBox, textToSet });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetGroupBoxText text: \"{textToSet}\".\n", exDelegate);
                }
            }
            else
            {
                if (groupBox != null && groupBox.Name != null && textToSet != null)
                    groupBox.Text = textToSet;
            }
        }


        internal void SetCtrlText(string text)
        {
            string textToSet = (!string.IsNullOrEmpty(text)) ? text : string.Empty;
            if (InvokeRequired)
            {
                SetCtrlTextCallback setCtrlTextDelegate = delegate (string setText)
                {
                    return (this != null && this.Name != null && !string.IsNullOrEmpty(setText)) ? this.Text : string.Empty;
                };
                try
                {
                    Invoke(setCtrlTextDelegate, new object[] { textToSet });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetCtrlText text: \"{textToSet}\".\n", exDelegate);
                }
            }
            else
            {
                if (this != null && this.Name != null && textToSet != null)
                    this.Text = textToSet;
            }
        }

        #endregion Eventhandler and delegate callbacks

        #region constructors

        public DragNDropBox() : this(new System.ComponentModel.Container())
        {
            // MiniToolBox.CreateAttachDirectory();
        }

        public DragNDropBox(IContainer container)
        {
            if (container == null)
                container = new System.ComponentModel.Container();
            container.Add(this);
            components = container;
            InitializeComponent();
            MiniToolBox.CreateAttachDirectory();

            DragEnter += new DragEventHandler(async (sender, e) => await DragNDropBox_DragEnter(sender, e));
            // DragDrop += DragNDropBox_DragDrop;
            DragDrop += new DragEventHandler(async (sender, e) => await DragNDropBox_DragDropAsync(sender, e));
            DragLeave += new EventHandler(async (sender, e) => await DragNDropBox_DragLeave(sender, e));
            DragOver += new DragEventHandler(async (sender, e) => await DragNDropBox_DragOver(sender, e));
            // MouseEnter += DragNDropBox_MouseEnter;
            // MouseLeave += DragNDropBox_MouseLeave;
        }

        public DragNDropBox(string headerText, IContainer container) : this(container) 
        {
            this.Text = headerText;
        }

        #endregion constructors

        #region DragNDrop

        internal async Task DragNDropBox_DragEnter(object sender, DragEventArgs e)
        {
            string lopmsg = string.Empty;

            string[] files = new string[1];

            if (e != null && e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[])))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {                   
                    await Task.Run(() =>
                    {
                        lopmsg = "Effect = " + e.Effect + " files.length =  " + files.Length + " file[1].Name = " + files[0].ToString();
                        e.Effect = System.Windows.Forms.DragDropEffects.Copy;

                        string textSet = Path.GetFileName(files[0]);
                        if (string.IsNullOrEmpty(textSet))
                            textSet = files[0].Substring(files[0].Length - 8);
                        textSet += " " + e.Effect;
                        SetCtrlText(textSet);
                    });
                }
            }
        }

        internal void DragNDropBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = new string[0];
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[])))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    DateTime now = DateTime.UtcNow;
                    foreach (string file in files)
                    {
                        try
                        {
                            if (OnDragNDrop != null)
                            {
                                SetGroupBoxText(this, Path.GetFileName(Path.GetFileName(file)) + " " + e.Effect);

                                EventHandler<Area23EventArgs<string>> handler = OnDragNDrop;
                                Area23EventArgs<string> area23EventArgs = new Area23EventArgs<string>(file);
                                handler?.Invoke(this, area23EventArgs);
                            }

                        }
                        catch (Exception ex)
                        {
                            CqrException.SetLastException(ex);
                            // Text = $"Exc:{ex.Message}";

                        }
                    }
                }
            }
            else
            {
                CqrException.SetLastException(new CqrException(Name + " DragNDropBox_DragDrop => files  == null"));
            }

            return;

        }

        internal async Task DragNDropBox_DragDropAsync(object sender, DragEventArgs e)
        {
            string[] files = new string[1];
            string lopmsg = "";

            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[])))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    DateTime now = DateTime.UtcNow;
                    foreach (string file in files)
                    {
                        await Task.Run(() =>
                        {
                            lopmsg = "Effect = " + e.Effect + " files.length =  " + files.Length + " file[1].Name = " + files[0].ToString();
                            e.Effect = System.Windows.Forms.DragDropEffects.Copy;

                            string textSet = Path.GetFileName(files[0]);
                            if (string.IsNullOrEmpty(textSet))
                                textSet = files[0].Substring(files[0].Length - 8);
                            textSet += " " + e.Effect;
                            SetCtrlText(textSet);
                        });

                        try
                        {
                            if (OnDragNDrop != null)
                            {
                                EventHandler<Area23EventArgs<string>> handler = OnDragNDrop;
                                Area23EventArgs<string> area23EventArgs = new Area23EventArgs<string>(file);
                                handler?.Invoke(this, area23EventArgs);
                            }

                            e.Effect = DragDropEffects.None;
                        }
                        catch (Exception ex)
                        {
                            CqrException.SetLastException(ex);
                            // Text = $"Exc:{ex.Message}";
                        }
                    }
                }
            }
            else
            {
                CqrException.SetLastException(new CqrException(Name + " DragNDropBox_DragDrop => files  == null"));
            }

            return;
        }


        internal async Task DragNDropBox_DragOver(object sender, DragEventArgs e)
        {
            string lopmsg = string.Empty;

            string[] files = new string[1];

            if (e != null && e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[])))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    await Task.Run(() =>
                    {
                        lopmsg = "Effect = " + e.Effect + " files.length =  " + files.Length + " file[1].Name = " + files[0].ToString();
                        e.Effect = System.Windows.Forms.DragDropEffects.Copy;

                        string textSet = Path.GetFileName(files[0]);
                        if (string.IsNullOrEmpty(textSet))
                            textSet = files[0].Substring(files[0].Length - 8);
                        textSet += " " + e.Effect;
                        SetCtrlText(textSet);                        

                    });

                    DoDragDrop(e.Data, DragDropEffects.Copy);
                }
            }
        }

        internal async Task DragNDropBox_DragLeave(object sender, EventArgs e)
        {
            await Task.Run(() => 
            {
                SetGroupBoxText(this, "Files Group Box");                
            });
        }

        #endregion DragNDrop

        
        public void SetHeaderText(string headerText)
        {
            this.Text = headerText;
        }
    
    }


}
