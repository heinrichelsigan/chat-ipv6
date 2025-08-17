using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Util;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace EU.CqrXs.WinForm.SecureChat.Controls.GroupBoxes
{
    public partial class DragNDropBox  : GroupBox
    {
        protected internal static DragDropEffects _dragDropEffect = System.Windows.Forms.DragDropEffects.None;
        internal static HashSet<string>  HashFiles = new HashSet<string>();
        static bool fileUploaded = false;
        static Lock _fileUploadLock = new Lock(), _fileLock = new Lock();

        #region eventhandler delegate callbacks

        public EventHandler<Area23EventArgs<string>>? OnDragNDrop;        

        internal delegate string GetCtrlTextCallback(System.Windows.Forms.Control ctrl);
        internal delegate string SetCtrlTextCallback(string text);

        internal virtual string GetCtrlText(System.Windows.Forms.Control ctrl)
        {
            string textToGet = string.Empty;

            if (ctrl.InvokeRequired)
            {
                GetCtrlTextCallback getTextDelegate = delegate (System.Windows.Forms.Control control)
                {
                    return (control != null && control.Text != null) ? control.Text : string.Empty;
                };
                try
                {
                    object? oget = ctrl.Invoke(getTextDelegate, new object[] { ctrl });
                    if (oget != null)
                        textToGet = (string)oget;
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate GetGroupBoxText: {ctrl.Name}.\n", exDelegate);
                }
            }
            else
            {
                if (ctrl != null && ctrl.Text != null)
                    textToGet = ctrl.Text;
            }

            return textToGet;
        }
      
        internal virtual void SetCtrlText(string text)
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
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate SetCtrlText text: \"{textToSet}\".\n", exDelegate);
                }
            }
            else
            {
                if (this != null && this.Name != null && textToSet != null)
                    this.Text = textToSet;
            }
        }

        #endregion eventhandler delegate callbacks

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

            // DragEnter += new DragEventHandler(async (sender, e) => await DragEnterAsync(sender, e));            
            // DragOver += new DragEventHandler(async (sender, e) => await DragOverAsync(sender, e));
            // DragDrop += new DragEventHandler(async (sender, e) => await DragDropAsync(sender, e));
            // DragLeave += new EventHandler(async (sender, e) => await DragLeaveAsync(sender, e));

            // MouseEnter += DragNDropBox_MouseEnter;
            // MouseLeave += DragNDropBox_MouseLeave;
        }

        public DragNDropBox(string headerText, IContainer container) : this(container) 
        {
            SetCtrlText(headerText);
        }

        #endregion constructors

        #region DragNDrop

        #region Synchronous DragNDrop

        internal void DragNDropBox_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = new string[1];
            fileUploaded = false; 

            if (e != null && e.Data != null) 
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[])))
                {
                    files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files != null && files.Length > 0)
                    {
                        HashFiles = new HashSet<string>(files);
                        _dragDropEffect = e.Effect;
                        string textSet = Path.GetFileName(files[0]) ?? files[0] ?? "";                       
                        textSet += " DragEnter: " + _dragDropEffect;
                        SetCtrlText(textSet);
                    }
                }
            }
        }

        internal void DragNDropBox_DragOver(object sender, DragEventArgs e)
        {
            string[] files = new string[1];

            if (e != null && e.Data != null && (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[]))))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    if (HashFiles == null || HashFiles.Count  == 0)
                        HashFiles = new HashSet<string>(files);

                    string textSet = Path.GetFileName(files[0]) ?? files[0] ?? "";
                    if (e.Effect != DragDropEffects.None)
                    {
                        _dragDropEffect = e.Effect;
                        textSet += " DragOver: " + e.Effect;
                        SetCtrlText(textSet);
                    }                    
                }
            }
        }

        internal void DragNDropBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = new string[1];            

            if (e != null && e.Data != null && (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[]))))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    if (HashFiles == null || HashFiles.Count == 0)
                        HashFiles = new HashSet<string>(files);

                    string textSet = Path.GetFileName(files[0]) ?? files[0] ?? "";
                    if (e.Effect != DragDropEffects.None)
                    {
                        _dragDropEffect = e.Effect;
                        textSet += " DragDrop: " + e.Effect;
                        SetCtrlText(textSet);
                    }

                    UpFireDraggedFiles();
                }
            }

            if (fileUploaded)
                HashFiles = new HashSet<string>();

            return;

        }

        internal void DragNDropBox_DragLeave(object sender, EventArgs e)
        {
            UpFireDraggedFiles();

            lock (_fileUploadLock)
            {
                fileUploaded = false;
                HashFiles = new HashSet<string>();
                _dragDropEffect = DragDropEffects.None;
                SetCtrlText("Files Group Box");
            }
        }

        #endregion Synchronous DragNDrop

        internal void UpFireDraggedFiles()
        {
            lock (_fileUploadLock)
            {
                if (HashFiles != null && HashFiles.Count > 0 && !fileUploaded)
                {
                    lock (_fileLock)
                    {
                        fileUploaded = true;
                        foreach (string file in HashFiles)
                        {
                            try
                            {
                                if (OnDragNDrop != null)
                                {
                                    string textSet = "Up firing " + Path.GetFileName(file) ?? file ?? "";
                                    SetCtrlText(textSet);

                                    EventHandler<Area23EventArgs<string>> handler = OnDragNDrop;
                                    Area23EventArgs<string> area23EventArgs = new Area23EventArgs<string>(file);
                                    handler?.Invoke(this, area23EventArgs);
                                }

                            }
                            catch (Exception ex)
                            {
                                CqrException.SetLastException(ex);
                            }
                        }

                        HashFiles = new HashSet<string>();
                        _dragDropEffect = DragDropEffects.None;
                    }
                }
            }
        }

        #region Async DragNDrop

        /// <summary>
        /// DragEnterAsync async EventHandlder for DragEnter 
        /// </summary>
        /// <returns></returns>
        internal async Task DragEnterAsync(object sender, DragEventArgs e)
        {
            string[] files = new string[1];
            if (e != null && e.Data != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[])))
                {
                    files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files != null && files.Length > 0)
                    {                        
                        await Task.Run(() =>
                        {
                            string textSet = Path.GetFileName(files[0]) ?? files[0];
                            textSet += " DragEnter: " + e.Effect;
                            SetCtrlText(textSet);
                        });
                    }
                }
            }
        }

        internal async Task DragOverAsync(object sender, DragEventArgs e)
        {
            string[] files = new string[1];
            if (e != null && e.Data != null && (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[]))))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    // DoDragDrop(e.Data, DragDropEffects.Copy);
                    await Task.Run(() =>
                    {
                        e.Effect = System.Windows.Forms.DragDropEffects.Copy;
                        string textSet = Path.GetFileName(files[0]) ?? files[0] ?? "";
                        textSet += " DragOver: " + e.Effect;
                        SetCtrlText(textSet);
                    });
                }
            }
        }


        internal async Task<HashSet<string>> DragDropAsync(object sender, DragEventArgs e)
        {
            string[] files = new string[0];
                        
            var filesHashSetTask = await Task<HashSet<string>>.Run<HashSet<string>>(() =>
            {
                HashSet<string> hashSetFiles = new HashSet<string>(files);
                if (e != null && e.Data != null && (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string[]))))
                {
                    files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files != null && files.Length > 0)
                    {
                        hashSetFiles = new HashSet<string>(files);
                        foreach (string file in files)
                        {
                            e.Effect = System.Windows.Forms.DragDropEffects.Copy;
                            string textSet = Path.GetFileName(file) ?? file ?? "";
                            textSet += " DragOver: " + e.Effect;
                            SetCtrlText(textSet);
                        }
                    }
                }

                return hashSetFiles;
            });

            return filesHashSetTask;
        }

        internal async Task DragLeaveAsync(object sender, EventArgs e)
        {
            await Task.Run(() => 
            {
                SetCtrlText("Files Group Box");
            });
        }

        #endregion Async DragNDrop

        #endregion DragNDrop

        public void SetHeaderText(string headerText) => SetCtrlText(headerText);
    
    }


}
