using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Util;
using System.ComponentModel;
using System.Windows;

namespace EU.CqrXs.WinForm.SecureChat.Controls.GroupBoxes
{

    /// <summary>
    /// LinkLabelsBox inherited from <see cref="GroupBox"/>
    /// Displays LinkLabels
    /// Implements drag and drop, when files are dropped into the control.
    /// </summary>
    public partial class LinkLabelsBox : GroupBox
    {

        int linksCount = 0;
        DateTime lastShownToolTip = DateTime.Today;

        public int LinksCount { get => linksCount; }
        public int LinksNum { get => ((linksCount % 8) + 1); }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string HeaderText { get => this.Text; set => this.Text = value; }

        public EventHandler<Area23EventArgs<string>>? OnDragNDrop;
        public LinkLabelLinkClickedEventHandler? LinkLabelFileOpen;
        public LinkLabelLinkClickedEventHandler? LinkLabelUriOpen;

        public delegate void LinkLabel_UriOpen_Click_Callback(object sender, LinkLabelLinkClickedEventArgs e);

        #region constructors

        public LinkLabelsBox()
        {
            components = new System.ComponentModel.Container();
            InitializeComponent();

            MiniToolBox.CreateAttachDirectory();
        }

        public LinkLabelsBox(IContainer container)
        {
            if (container == null)
                container = new System.ComponentModel.Container();
            container.Add(this);           
            components = container;
            InitializeComponent();
            MiniToolBox.CreateAttachDirectory();
        }


        public LinkLabelsBox(string headerText, IContainer container) : this(container)
        {
            this.Text = headerText;
        }

        #endregion constructors


        #region Setting and Linking LinkedLabels

        #region thread save WinForm delegate callbacks

        internal delegate string GetGroupBoxTextCallback(GroupBox groupBox);
        internal delegate string SetGroupBoxTextCallback(GroupBox groupBox, string text);
        internal delegate string GetLinkLabelNameCallback(LinkLabel linkLabel);
        internal delegate void SetLinkLabelNameCallback(LinkLabel linkLabel, string name);
        internal delegate string GetLinkLabelTextCallback(LinkLabel linkLabel);    
        internal delegate void SetLinkLabelTextCallback(LinkLabel linkLabel, string text);
        internal delegate void AddLinkLabelLinksCallback(LinkLabel linkLabel, string linkUrl);
        internal delegate void SetLinkLabelVisibleCallback(LinkLabel linkLabel, bool visible);

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
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate GetGroupBoxText: \"{groupBox.Name}\".\n", exDelegate);
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
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate SetGroupBoxText text: \"{textToSet}\".\n", exDelegate);
                }
            }
            else
            {
                if (groupBox != null && groupBox.Name != null && textToSet != null)
                    groupBox.Text = textToSet;
            }
        }

        internal string GetLinkLabelName(System.Windows.Forms.LinkLabel linkLabel)
        {
            string nameToGet = string.Empty;

            if (linkLabel.InvokeRequired)
            {
                GetLinkLabelNameCallback getNameDelegate = delegate (System.Windows.Forms.LinkLabel lnkLabel)
                {
                    return (lnkLabel != null && lnkLabel.Name != null) ? lnkLabel.Name : string.Empty;
                };
                try
                {
                    object? oget = linkLabel.Invoke(getNameDelegate, new object[] { linkLabel });
                    if (oget != null)
                        nameToGet = (string)oget;
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate GetLinkLabelName LinkLabel: \"{linkLabel}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null && linkLabel.Name != null)
                    nameToGet = linkLabel.Name;
            }

            return nameToGet;
        }

        internal void SetLinkLabelName(System.Windows.Forms.LinkLabel linkLabel, string name)
        {
            string nameToSet = (!string.IsNullOrEmpty(name)) ? name : string.Empty;
            if (linkLabel.InvokeRequired)
            {
                SetLinkLabelNameCallback setNameDelegate = delegate (System.Windows.Forms.LinkLabel lnkLabel, string setName)
                {
                    if (lnkLabel != null && lnkLabel.Name != null && !string.IsNullOrEmpty(setName))
                        lnkLabel.Name = setName;
                };
                try
                {
                    linkLabel.Invoke(setNameDelegate, new object[] { linkLabel, nameToSet });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate SetLinkLabelText text: \"{nameToSet}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null && linkLabel.Name != null && nameToSet != null)
                    linkLabel.Name = nameToSet;
            }
        }

        internal string GetLinkLabelText(System.Windows.Forms.LinkLabel linkLabel)
        {
            string textToGet = string.Empty;

            if (linkLabel.InvokeRequired)
            {
                GetLinkLabelTextCallback getTextDelegate = delegate (System.Windows.Forms.LinkLabel lnkLabel)
                {
                    return (lnkLabel != null && lnkLabel.Text != null) ? lnkLabel.Text : string.Empty;
                };
                try
                {
                    object? oget = linkLabel.Invoke(getTextDelegate, new object[] { linkLabel });
                    if (oget != null)
                        textToGet = (string)oget;
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate GetLinkLabelText LinkLabel: \"{linkLabel.Name}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null && linkLabel.Text != null)
                    textToGet = linkLabel.Text;
            }

            return textToGet;
        }

        /// <summary>
        /// SetLinkLabelText - sets text on a <see cref="System.Windows.Forms.LinkLabel"/>
        /// </summary>
        /// <param name="linkLabel"><see cref="System.Windows.Forms.linkLabel"/></param>
        /// <param name="text"><see cref="string">string text</see> to set</param>
        internal void SetLinkLabelText(System.Windows.Forms.LinkLabel linkLabel, string text)
        {
            string textToSet = (!string.IsNullOrEmpty(text)) ? text : string.Empty;         
            if (linkLabel.InvokeRequired)
            {
                SetLinkLabelTextCallback setTextDelegate = delegate (System.Windows.Forms.LinkLabel lnkLabel, string setText)
                {
                    if (lnkLabel != null && lnkLabel.Text != null && setText != null)
                        lnkLabel.Text = setText;
                };
                try
                {
                    linkLabel.Invoke(setTextDelegate, new object[] { linkLabel, textToSet });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate SetLinkLabelText text: \"{textToSet}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null && linkLabel.Text != null && textToSet != null)
                    linkLabel.Text = textToSet;
            }
        }

        internal void AddLinkLabelLinks(System.Windows.Forms.LinkLabel linkLabel, string link)
        {
            string linkToAdd = (!string.IsNullOrEmpty(link)) ? link : string.Empty;
            if (linkLabel.InvokeRequired)
            {
                AddLinkLabelLinksCallback addLinkLabelLinksCallback = delegate (System.Windows.Forms.LinkLabel lnkLabel, string linkUrlToAdd)
                {
                    if (lnkLabel != null && lnkLabel.Text != null && linkUrlToAdd != null)
                    {
                        lnkLabel.Links.Clear();
                        lnkLabel.Links.Add(0, linkUrlToAdd.Length, linkUrlToAdd);
                    }
                };
                try
                {
                    linkLabel.Invoke(addLinkLabelLinksCallback, new object[] { linkLabel, linkToAdd });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate AddLinkLabelLinks: \"{linkToAdd}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null && linkLabel.Text != null && linkToAdd != null)
                {
                    linkLabel.Links.Clear();
                    linkLabel.Links.Add(0, link.Length, linkToAdd);
                }
                    
            }
        }

        internal void SetLinkLabelVisible(System.Windows.Forms.LinkLabel linkLabel, bool visible)
        {          
            if (linkLabel.InvokeRequired)
            {
                SetLinkLabelVisibleCallback setLinkLabelVisibleCallback = delegate (System.Windows.Forms.LinkLabel lnkLabel, bool vicible)
                {
                    if (lnkLabel != null)
                        lnkLabel.Visible = vicible;
                };
                try
                {
                    linkLabel.Invoke(setLinkLabelVisibleCallback, new object[] { linkLabel, visible });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(this.Name, $"Exception in delegate SetLinkLabelVisible visible: \"{visible}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null)
                    linkLabel.Visible = visible;
            }
        }

        #endregion thread save WinForm delegate callbacks

        /// <summary>
        /// SetNameUri sets a name and an internet hyperlink uri to the next round robin label
        /// </summary>
        /// <param name="linkLabelName">name to set</param>
        /// <param name="uri">internet <see cref="Uri"/> to set</param>
        public void SetNameUri(string linkLabelName, Uri uri)
        {
            LinkLabel linkLabelUri = new LinkLabel() { Name = $"linkLabel{LinksNum}" };

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                if (ctrl != null && ctrl is LinkLabel lbAttach &&
                    (ctrl.Name.EndsWith(LinksNum.ToString()) || ctrl.Name.Equals("LinkLabel" + LinksNum)))
                {
                    linkLabelUri = (LinkLabel)lbAttach;
                    SetLinkLabelName(linkLabelUri, $"LinkLabel{LinksNum}");
                    SetLinkLabelVisible(linkLabelUri, true);
                    break; // we got the next LinkLabel attachment in modulo slot
                }
            }
            LinkLabel_UriOpen_Click_Callback linkLabelUriOpen =
                delegate (object sender, LinkLabelLinkClickedEventArgs e)
                {
                    LinkLabel_Uri_LinkClicked(sender, e);
                };
            if (LinkLabelUriOpen == null)
                LinkLabelUriOpen = new LinkLabelLinkClickedEventHandler(linkLabelUriOpen);
            if (GetLinkLabelText(linkLabelUri).StartsWith("LinkLabel"))
                linkLabelUri.LinkClicked += LinkLabelUriOpen;
            
            SetLinkLabelText(linkLabelUri, linkLabelName);
            AddLinkLabelLinks(linkLabelUri, uri.ToString());                       

            ++linksCount;
        }

        /// <summary>
        /// SetNameFilePath sets a name, a file system path pointing to a regular file to open to next round robin link label
        /// </summary>
        /// <param name="linkLabelName">display name to ste</param>
        /// <param name="fullFilePath">full file path pointing to a regular file in filesystem UNC possible</param>
        /// <param name="chackFilePath">if true, an <see cref="FileNotFoundException"/> will be thrown, when file doesn't exist</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public void SetNameFilePath(string linkLabelName, string fullFilePath, bool chackFilePath = false)
        {
            if (chackFilePath && !File.Exists(fullFilePath))
            {
                throw new System.IO.FileNotFoundException($"FilePath: {fullFilePath} was not found on file system!");
            }

            LinkLabel linkLabelFile = new LinkLabel() { Name = $"linkLabel{LinksNum}" };

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                if (ctrl != null && ctrl is LinkLabel lbAttach &&
                    (ctrl.Name.EndsWith(LinksNum.ToString()) || ctrl.Name.Equals("LinkLabel" + LinksNum)))
                {
                    linkLabelFile = (LinkLabel)lbAttach;
                    SetLinkLabelName(linkLabelFile, $"LinkLabel{LinksNum}");
                    SetLinkLabelVisible(linkLabelFile, true);
                    break; // we got the next LinkLabel attachment in modulo slot
                }
            }

            if (LinkLabelFileOpen == null)
                LinkLabelFileOpen = new LinkLabelLinkClickedEventHandler(LinkLabel_File_LinkClicked);
            if (GetLinkLabelText(linkLabelFile).StartsWith("LinkLabel"))
                linkLabelFile.LinkClicked += LinkLabelFileOpen;

            SetLinkLabelText(linkLabelFile, linkLabelName);
            AddLinkLabelLinks(linkLabelFile, fullFilePath);           

            ++linksCount;
        }

        /// <summary>
        /// SetCqrFileTextLink is called, when receiving a <see cref="CqrFile"/> 
        /// Binary file <see cref="CqrFile.Data" /> will be written to <see cref="Path.Combine(LibPaths.AttachmentFilesDir, CqrFile.CqrFileName)"/>        
        /// <see cref="SetNameFilePath(string, string, bool)"/>
        /// </summary>
        /// <param name="cqrFile"></param>
        public void SetCqrFileTextLink(CFile cqrFile)
        {            
            string fullFilePath = Path.Combine(LibPaths.AttachmentFilesDir, cqrFile.FileName);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fullFilePath);
            byte[] fileBytes = cqrFile.Data;
                // Area23.At.Framework.Core.Crypt.EnDeCoding.Base64.Decode(mimeAttachment.Base64Mime);
            System.IO.File.WriteAllBytes(fullFilePath, fileBytes);
            
            SetNameFilePath(fileNameWithoutExt, fullFilePath);
        }

        /// <summary>
        /// LinkLabel_Uri_LinkClicked Event delegate function 
        /// occurs, when an internet uri is added to linked label links and LinkLabel will be link clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e"><see cref="LinkLabelLinkClickedEventArgs"/></param>
        protected internal void LinkLabel_Uri_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender != null && e.Link != null && e.Link.LinkData != null && File.Exists(e.Link.LinkData.ToString()))
            {
                Help.ShowHelp(this, e.Link.LinkData.ToString());
            }
        }

        /// <summary>
        /// LinkLabel_File_LinkClicked Event delegate functions
        /// occurs, when a file in file system is added to linked label links and LinkLabel is clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e"><see cref="LinkLabelLinkClickedEventArgs"/></param>
        protected internal void LinkLabel_File_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender != null && e.Link != null && e.Link.LinkData != null && File.Exists(e.Link.LinkData.ToString()))
            {
                ProcessCmd.Execute("explorer", e.Link.LinkData.ToString());
            }
        }

        #endregion Setting and Linking LinkedLabels


        #region DragNDrop

        /// <summary>
        /// Occurs, when a file drag enter the LinkLabelsBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LinkLabelsBox_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
            if (files != null)
            {
                e.Effect = System.Windows.Forms.DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// LinkLabelsBox_DragDrop occurs, when dropping a dragged file into LinkLabelsBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LinkLabelsBox_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
            if (files != null)
            {
                DateTime now = DateTime.UtcNow;
                string gtext = "";
                SetGroupBoxText(this, gtext);
                foreach (string file in files)
                {
                    try
                    {                        
                        gtext += $"+{Path.GetFileName(file)}";
                        SetGroupBoxText(this, gtext);
                        if (OnDragNDrop != null)
                        {
                            EventHandler<Area23EventArgs<string>>? handler = OnDragNDrop;
                            Area23EventArgs<string> area23EventArgs = new Area23EventArgs<string>(file);
                            handler?.Invoke(this, area23EventArgs);
                        }

                    }
                    catch (Exception ex)
                    {
                        Area23Log.LogOriginMsgEx("LinkLabelsBox", $"LinkLabelsBox_DragDrop(...)", ex);
                    }
                    gtext += " ";
                }
                System.Timers.Timer tPerformResetBoxName = new System.Timers.Timer { Interval = 2000 };
                tPerformResetBoxName.Elapsed += (s, en) =>
                {
                    this.Invoke(new Action(() =>
                    {
                        SetGroupBoxText(this, "Attachments"); // TODO Globalization ressorce based sometimes 
                    }));
                    tPerformResetBoxName.Stop(); // Stop the timer(otherwise keeps on calling)
                };
                tPerformResetBoxName.Start();
            }
        }

        /// <summary>
        /// LinkLabelsBox_DragLeave occurs, when mouse pointer left 
        /// button hold down with dragging file leaves the graphical area of the box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LinkLabelsBox_DragLeave(object sender, EventArgs e)
        {

        }

        #endregion DragNDrop


        #region show / hide tooltip

        //private void LinkLabelsBox_MouseEnter(object sender, EventArgs e)
        //{
        //    if (DateTime.Now.Subtract(lastShownToolTip).TotalSeconds > 54)
        //    {
        //        // this.toolTip1.SetToolTip(this, "Drag'n Drop Files here or click on \"Attach\"");
        //        toolTip1.Show("Drag'n Drop Files here or click on \"Attach\"", this, 36, 72, 6000);
        //        lastShownToolTip = DateTime.Now;
        //    }
        //}

        //private void LinkLabelsBox_MouseLeave(object sender, EventArgs e)
        //{
        //    System.Timers.Timer tPerformToolTipHide = new System.Timers.Timer { Interval = 1500 };
        //    tPerformToolTipHide.Elapsed += (s, en) =>
        //    {
        //        this.Invoke(new Action(() =>
        //        {
        //            toolTip1.Hide(this);
        //        }));
        //        tPerformToolTipHide.Stop(); // Stop the timer(otherwise keeps on calling)
        //    };
        //    tPerformToolTipHide.Start();
        //}


        #endregion show / hide tooltip

    }
}
