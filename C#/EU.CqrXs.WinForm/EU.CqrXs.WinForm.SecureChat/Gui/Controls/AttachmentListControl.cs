using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Area23.At.Framework.Core.Util;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Controls
{
    public partial class AttachmentListControl : UserControl
    {

        int linksCount = 0;
        public int LinksCount { get => linksCount; }
        public int LinksNum { get => ((linksCount % 8) + 1); }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string HeaderText { get => this.labelHeaderText.Text; set => this.labelHeaderText.Text = value; }

        public EventHandler<Area23EventArgs<string>> OnDragNDrop;

        internal delegate void SetLinkLabelTextCallback(LinkLabel linkLabel, string text);
        internal delegate void AddLinkLabelLinksCallback(LinkLabel linkLabel, string linkUrl);
        internal delegate void SetLinkLabelVisibleCallback(LinkLabel linkLabel, bool visible);

        public AttachmentListControl()
        {
            components = new System.ComponentModel.Container();
            InitializeComponent();
        }

        public AttachmentListControl(IContainer container)
        {
            if (container == null)
                container = new System.ComponentModel.Container();
            container.Add(this);
            components = container;

            InitializeComponent();

            CreateAttachDirectory();
        }


        public AttachmentListControl(string headerText, IContainer container) : this(container)
        {
            this.Text = headerText;
        }



        public void CreateAttachDirectory()
        {

            try
            {
                if (!Directory.Exists(LibPaths.AttachmentFilesDir))
                    Directory.CreateDirectory(LibPaths.AttachmentFilesDir);
            }
            catch (Exception dirEx)
            {
                Area23Log.LogStatic(dirEx);
            }
        }


        private void AttachmentList_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void AttachmentList_DragDrop(object sender, DragEventArgs e)
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

        private void AttachmentList_DragLeave(object sender, EventArgs e)
        {

        }


        public void SetNameUri(string linkLabelName, Uri uri)
        {

            LinkLabel linkLabel0 = new LinkLabel() { Name = $"linkLabel{LinksNum}" };

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                if (ctrl != null && ctrl is LinkLabel lbAttach &&
                    (ctrl.Name.EndsWith(LinksNum.ToString()) || ctrl.Name.Equals("LinkLabel" + LinksNum)))
                {
                    linkLabel0 = (LinkLabel)lbAttach;
                    linkLabel0.Name = $"LinkLabel{LinksNum}";
                    SetLinkLabelVisible(linkLabel0, true);
                    break; // we got the next LinkLabel attachment in modulo slot
                }
            }

            SetLinkLabelText(linkLabel0, linkLabelName);
            AddLinkLabelLinks(linkLabel0, uri.ToString());
            linkLabel0.LinkClicked += LinkLabel_Uri_LinkClicked;

            ++linksCount;
        }

        public void SetNameFilePath(string linkLabelName, string filePath, bool chackFilePath = false)
        {
            if (chackFilePath && !File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException($"FilePath: {filePath} was not found on file system!");
            }

            LinkLabel linkLabel0 = new LinkLabel() { Name = $"linkLabel{LinksNum}" };

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                if (ctrl != null && ctrl is LinkLabel lbAttach &&
                    (ctrl.Name.EndsWith(LinksNum.ToString()) || ctrl.Name.Equals("LinkLabel" + LinksNum)))
                {
                    linkLabel0 = (LinkLabel)lbAttach;
                    linkLabel0.Name = $"LinkLabel{LinksNum}";
                    SetLinkLabelVisible(linkLabel0, true);
                    break; // we got the next LinkLabel attachment in modulo slot
                }
            }

            SetLinkLabelText(linkLabel0, linkLabelName);
            AddLinkLabelLinks(linkLabel0, filePath);
            linkLabel0.LinkClicked += LinkLabel_File_LinkClicked;

            ++linksCount;
        }


        public void SetMimeAttachmentTextLink(MimeAttachment mimeAttachment)
        {
            CreateAttachDirectory();
            string fileName = mimeAttachment.FileName;
            string filePath = Path.Combine(LibPaths.AttachmentFilesDir, mimeAttachment.FileName);
            byte[] fileBytes = Area23.At.Framework.Core.Crypt.EnDeCoding.Base64.Decode(mimeAttachment.Base64Mime);
            System.IO.File.WriteAllBytes(filePath, fileBytes);

            SetNameFilePath(fileName, filePath);
        }


        protected internal void LinkLabel_Uri_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender != null && e.Link != null && e.Link.LinkData != null && File.Exists(e.Link.LinkData.ToString()))
            {
                Help.ShowHelp(this, e.Link.LinkData.ToString());
            }
        }

        protected internal void LinkLabel_File_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender != null && e.Link != null && e.Link.LinkData != null && File.Exists(e.Link.LinkData.ToString()))
            {
                ProcessCmd.Execute("explorer", e.Link.LinkData.ToString());
            }
        }

        #region thread save WinForm delegate callbacks

        /// <summary>
        /// SetLinkLabelText - sets text on a <see cref="System.Windows.Forms.LinkLabel"/>
        /// </summary>
        /// <param name="linkLabel"><see cref="System.Windows.Forms.linkLabel"/></param>
        /// <param name="text"><see cref="string">string text</see> to set</param>
        internal void SetLinkLabelText(System.Windows.Forms.LinkLabel linkLabel, string text)
        {
            string textToSet = (!string.IsNullOrEmpty(text)) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (linkLabel.InvokeRequired)
            {
                SetLinkLabelTextCallback setTextDelegate = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.LinkLabel lnkLabel, string setText)
                    {
                        if (lnkLabel != null && lnkLabel.Text != null && setText != null)
                            lnkLabel.Text = setText;
                    };
                try
                {
                    linkLabel.Invoke(setTextDelegate, new object[] { linkLabel, textToSet });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetLinkLabelText text: \"{text}\".\n", exDelegate);
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

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (linkLabel.InvokeRequired)
            {
                AddLinkLabelLinksCallback addLinkLabelLinksCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.LinkLabel lnkLabel, string linkUrlToAdd)
                    {
                        if (lnkLabel != null && lnkLabel.Text != null && linkUrlToAdd != null)
                            lnkLabel.Links.Add(0, linkUrlToAdd.Length, linkUrlToAdd);
                    };
                try
                {
                    linkLabel.Invoke(addLinkLabelLinksCallback, new object[] { linkLabel, link });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate AddLinkLabelLinks: \"{link}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null && linkLabel.Text != null && link != null)
                    linkLabel.Links.Add(0, link.Length, link);
            }
        }

        internal void SetLinkLabelVisible(System.Windows.Forms.LinkLabel linkLabel, bool visible)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (linkLabel.InvokeRequired)
            {
                SetLinkLabelVisibleCallback setLinkLabelVisibleCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.LinkLabel lnkLabel, bool vicible)
                    {
                        if (lnkLabel != null)
                            lnkLabel.Visible = vicible;
                    };
                try
                {
                    linkLabel.Invoke(setLinkLabelVisibleCallback, new object[] { linkLabel, visible });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetLinkLabelVisible visible: \"{visible}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null)
                    linkLabel.Visible = visible;
            }
        }


        #endregion thread save WinForm delegate callbacks


        private void AttachmentList_ShowToolTip(object sender, EventArgs e)
        {
            // this.toolTip1.SetToolTip(this, "Drag'n Drop Files here or click on \"Attach\"");
            this.toolTip1.Show("Drag'n Drop Files here or click on \"Attach\"", this, 24, 48, 4000);
        }

        private void AttachmentList_HideToolTip(object sender, EventArgs e)
        {
            this.toolTip1.Hide(this);
        }
    }


}
