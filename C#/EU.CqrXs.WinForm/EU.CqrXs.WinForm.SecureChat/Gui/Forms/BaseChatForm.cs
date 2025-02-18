using Area23FwCore = Area23.At.Framework.Core;
using Area23.At.Framework.Core;
using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.CqrXs.CqrSrv;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Net.WebHttp;
using EU.CqrXs.WinForm.SecureChat.Entities;
using System.Media;
using System.Net;
using Area23.At.Framework.Core.Net.NameService;
using EU.CqrXs.WinForm.SecureChat.Properties;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms
{


    /// <summary>
    /// BaseChat Form is common base class for <see cref="SecureChat"/>, <see cref="Peer2PeerChat"/> and <see cref="RichTextChat"/>
    /// BaseChat contains mainly:
    /// thread independent delegate accessor to <see cref="System.Windows.Forms"  /> 
    /// basic member to play sounds <see cref="PlaySoundFromResource(string)" /> and <see cref="PlaySoundFromResourcesAsync(string)"/> 
    /// help, info and <see cref="TransparentDialog">about dialog</see> menu events 
    /// close and exit <see cref="AppCloseAllFormsExit"/>
    /// </summary>
    public class BaseChatForm : Form
    {

        #region fields

        protected string savedFile = string.Empty;
        protected string loadDir = string.Empty;
        private System.ComponentModel.IContainer components = null;

        #endregion fields

        #region Properties



        protected internal static IPAddress? externalIPAddress;
        public static IPAddress? ExternalIpAddress
        {
            get
            {
                if (externalIPAddress != null)
                    return externalIPAddress;

                externalIPAddress = WebClientRequest.ExternalClientIpFromServer("https://ipv4.cqrxs.eu/net/R.aspx");
                return externalIPAddress;
            }
        }
        
        #endregion Properties

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseChatForm() { }


        #region thread save WinForm delegate callbacks



        
        #region TextBox&RichTextBox

        internal delegate void SetTextCallback(System.Windows.Forms.TextBox textBox, string text);

        internal delegate void AppendTextCallback(System.Windows.Forms.TextBox textBox, string text);

        internal delegate void AppendRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, string text);

        internal delegate void SelectRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, int start, int length);

        internal delegate void SelectionAlignmentRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, HorizontalAlignment leftRight);

        internal delegate int GetFirstCharIndexFromLineRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, int lineNr);

        internal delegate void ClearRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, bool clear = true);


        /// <summary>
        /// AppendText - appends text on a <see cref="System.Windows.Forms.TextBox"/>
        /// </summary>
        /// <param name="textBox"><see cref="System.Windows.Forms.TextBox"/></param>
        /// <param name="text"><see cref="string">string text</see> to set</param>
        internal void AppendText(System.Windows.Forms.TextBox textBox, string text)
        {
            string textToAppend = (!string.IsNullOrEmpty(text)) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (textBox.InvokeRequired)
            {
                AppendTextCallback appendTextDelegate = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.TextBox textArea, string appendText)
                    {
                        if (textArea != null && textArea.Text != null && appendText != null)
                            textArea.AppendText(appendText);
                    };
                try
                {
                    textBox.Invoke(appendTextDelegate, new object[] { textBox, textToAppend });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate AppendText text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (textBox != null && textBox.Text != null && textToAppend != null)
                    textBox.AppendText(textToAppend);
            }
        }

        internal void AppendRichText(System.Windows.Forms.RichTextBox richTextBox, string text)
        {
            string textToAppend = (!string.IsNullOrEmpty(text)) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                AppendRichTextCallback appendRichTextDelegate = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.RichTextBox textArea, string appendText)
                    {
                        if (textArea != null && textArea.Text != null && appendText != null)
                            textArea.AppendText(appendText);
                    };
                try
                {
                    richTextBox.Invoke(appendRichTextDelegate, new object[] { richTextBox, textToAppend });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate AppendRichText text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && richTextBox.Text != null && textToAppend != null)
                    richTextBox.AppendText(textToAppend);
            }
        }

        internal void SelectRichText(System.Windows.Forms.RichTextBox richTextBox, int start, int length)
        {
            if (start < 0)
                start = 0;
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                SelectRichTextCallback selectRichTextCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.RichTextBox textArea, int charStart, int charLength)
                    {
                        if (textArea != null && textArea.Text != null)
                            textArea.Select(charStart, charLength);
                    };
                try
                {
                    richTextBox.Invoke(selectRichTextCallback, new object[] { richTextBox, start, length });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate select rich text: \"{start},{length}\".\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && richTextBox.Text != null)
                    richTextBox.Select(start, length);
            }
        }

        internal void SelectionAlignmentRichText(System.Windows.Forms.RichTextBox richTextBox, HorizontalAlignment leftRight)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                SelectionAlignmentRichTextCallback selectionAlignmentRichTextCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.RichTextBox textArea, HorizontalAlignment lr)
                    {
                        if (textArea != null && textArea.Text != null)
                            textArea.SelectionAlignment = lr;
                    };
                try
                {
                    richTextBox.Invoke(selectionAlignmentRichTextCallback, new object[] { richTextBox, leftRight });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SelectionAlignmentRichText: \"{leftRight}\".\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && richTextBox.Text != null)
                    richTextBox.SelectionAlignment = leftRight;
            }
        }

        internal int GetFirstCharIndexFromLineRichText(System.Windows.Forms.RichTextBox richTextBox, int lineNr)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                GetFirstCharIndexFromLineRichTextCallback getFirstCharIndexFromLineRichTextCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.RichTextBox textArea, int lnr)
                    {
                        if (textArea != null && textArea.Text != null)
                            return textArea.GetFirstCharIndexFromLine(lnr);
                        return -1;
                    };
                try
                {
                    richTextBox.Invoke(getFirstCharIndexFromLineRichTextCallback, new object[] { richTextBox, lineNr });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate GetFirstCharIndexFromLineRichText({lineNr}).\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && richTextBox.Text != null)
                    return richTextBox.GetFirstCharIndexFromLine(lineNr);
            }
            return -1;
        }

        internal void ClearRichText(System.Windows.Forms.RichTextBox richTextBox, bool clear = true)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                ClearRichTextCallback clearRichTextCallback =
                    delegate (System.Windows.Forms.RichTextBox textArea, bool clr)
                    {
                        if (textArea != null)
                            textArea.Clear();
                        return;
                    };
                try
                {
                    richTextBox.Invoke(clearRichTextCallback, new object[] { richTextBox, clear });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate ClearRichText: \"{exDelegate.Message}\".\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null)
                    richTextBox.Clear();
            }
        }

        #endregion TextBox&RichTextBox

        #region LinkLabel

        internal delegate void SetLinkLabelTextCallback(LinkLabel linkLabel, string text);

        internal delegate void AddLinkLabelLinksCallback(LinkLabel linkLabel, string linkUrl);

        internal delegate void SetLinkLabelVisibleCallback(LinkLabel linkLabel, bool visible);

        /// <summary>
        /// SetLinkLabelText - sets text on a <see cref="LinkLabel"/>
        /// </summary>
        /// <param name="linkLabel"><see cref="linkLabel"/></param>
        /// <param name="text"><see cref="string">string text</see> to set</param>
        internal void SetLinkLabelText(LinkLabel linkLabel, string text)
        {
            string textToSet = (!string.IsNullOrEmpty(text)) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (linkLabel.InvokeRequired)
            {
                SetLinkLabelTextCallback setTextDelegate = 
                    delegate (LinkLabel lnkLabel, string setText)
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
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetLinkLabelText text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null && linkLabel.Text != null && textToSet != null)
                    linkLabel.Text = textToSet;
            }
        }

        internal void AddLinkLabelLinks(LinkLabel linkLabel, string link)
        {
            string linkToAdd = (!string.IsNullOrEmpty(link)) ? link : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (linkLabel.InvokeRequired)
            {
                AddLinkLabelLinksCallback addLinkLabelLinksCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (LinkLabel lnkLabel, string linkUrlToAdd)
                    {
                        if (lnkLabel != null && lnkLabel.Text != null && linkUrlToAdd != null)
                            lnkLabel.Links.Add(0, linkUrlToAdd.Length, linkUrlToAdd);
                    };
                try
                {
                    linkLabel.Invoke(addLinkLabelLinksCallback, new object[] { linkLabel, link });
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

        internal void SetLinkLabelVisible(LinkLabel linkLabel, bool visible)
        {           
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (linkLabel.InvokeRequired)
            {
                SetLinkLabelVisibleCallback setLinkLabelVisibleCallback = 
                    delegate (LinkLabel lnkLabel, bool vicible)
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
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetLinkLabelVisible visible: \"{visible}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null)
                    linkLabel.Visible = visible;
            }
        }

        #endregion LinkLabel

        #region ToolStripStatusLabel

        internal delegate void SetToolStripStatusLabelTextCallback(ToolStripStatusLabel statusLabel, string text);

        internal void SetStatusText(ToolStripStatusLabel toolStatusLabel, string text)
        {
            string setText = (!string.IsNullOrEmpty(text)) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (toolStatusLabel.GetCurrentParent() != null && toolStatusLabel.GetCurrentParent().InvokeRequired)
            {
                SetToolStripStatusLabelTextCallback statusLabelSetTextCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (ToolStripStatusLabel statusLabel, string setText)
                    {
                        if (statusLabel != null && setText != null)
                            statusLabel.Text = setText;
                    };
                try
                {
                    toolStatusLabel.GetCurrentParent().Invoke(statusLabelSetTextCallback, new object[] { toolStatusLabel, setText });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetStatusText text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (toolStatusLabel != null && setText != null)
                    toolStatusLabel.Text = setText;
            }
        }

        #endregion

        #region ComboBox

        internal delegate string GetComboBoxTextCallback(System.Windows.Forms.ComboBox comboBox);

        internal delegate void SetComboBoxTextCallback(System.Windows.Forms.ComboBox comboBox, string text);

        internal delegate void SetComboBackColorCallback(System.Windows.Forms.ComboBox comboBox, Color color);

        internal string GetComboBoxText(System.Windows.Forms.ComboBox comboBox)
        {
            string reText = string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox.InvokeRequired)
            {
                GetComboBoxTextCallback getComboBoxTextCallback = 
                    delegate (System.Windows.Forms.ComboBox cmbx)
                    {
                        return (cmbx != null && cmbx.Text != null) ? cmbx.Text : string.Empty;
                    };
                try
                {
                    reText = (string)comboBox.Invoke(getComboBoxTextCallback, new object[] { comboBox });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate GetComboBoxText ComboBox = {comboBox.Name}.\n", exDelegate);
                }
            }
            else
            {
                reText = (comboBox != null && comboBox.Text != null) ? comboBox.Text : string.Empty;
            }

            return reText;
        }


        internal void SetComboBoxText(System.Windows.Forms.ComboBox comboBox, string text)
        {
            string setText = (!string.IsNullOrEmpty(text)) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox.InvokeRequired)
            {
                SetComboBoxTextCallback setComboBoxTextCallback =
                    delegate (System.Windows.Forms.ComboBox cmbx, string txt)
                    {
                        if (cmbx != null && txt != null)
                            cmbx.Text = setText;
                    };
                try
                {
                    comboBox.Invoke(setComboBoxTextCallback, new object[] { comboBox, setText });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetComboBoxText ComboBox = {comboBox.Name}, text = \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null && setText != null)
                    comboBox.Text = setText;
            }
        }

        internal void SetComboBoxBackColor(System.Windows.Forms.ComboBox comboBox, Color color)
        {
            Color setColor = (color != null) ? color : Color.Transparent;
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox.InvokeRequired)
            {
                SetComboBackColorCallback setComboBackColorCallback =
                    delegate (System.Windows.Forms.ComboBox cmbx, Color colr)
                    {
                        if (cmbx != null && colr != null)
                            cmbx.BackColor = colr;
                    };
                try
                {
                    comboBox.Invoke(setComboBackColorCallback, new object[] { comboBox, color });
                }
                catch (System.Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in delegate SetComboBoxBackColor ComboBox = {comboBox.Name}, Colore = \"{color}\".\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null && color != null)
                    comboBox.BackColor = color;
            }
        }

        #endregion ComboBox

        #endregion thread save WinForm delegate callbacks


        public MimeAttachment? SendAttachment(string filename, string secretKey, IPAddress partnerIpAddress)
        {

            MimeAttachment? mimeAttach = null;

            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                string md5 = Area23FwCore.Crypt.Hash.MD5Sum.Hash(filename, true);
                string sha256 = Area23FwCore.Crypt.Hash.Sha256Sum.Hash(filename, true);

                byte[] fileBytes = System.IO.File.ReadAllBytes(filename);
                string fileNameOnly = Path.GetFileName(filename);
                string mimeType = Area23FwCore.Util.MimeType.GetMimeType(fileBytes, fileNameOnly);

                string base64Mime = Base64.Encode(fileBytes);

                Peer2PeerMsg pmsg = new Peer2PeerMsg(secretKey);

                
                    // pmsg.SendCqrPeerMsg(mimeAttach.MimeMsg, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);
                pmsg.Send_CqrPeerAttachment(fileNameOnly, mimeType, base64Mime, partnerIpAddress, out mimeAttach, Constants.CHAT_PORT, md5, sha256, MsgEnum.None, EncodingType.Base64);

                string base64FilePath = Path.Combine(LibPaths.AttachmentFilesDir, mimeAttach.FileName + Constants.BASE64_EXT);
                System.IO.File.WriteAllText(base64FilePath, mimeAttach.MimeMsg);           
            }

            return mimeAttach;

        }

        /// <summary>
        /// GetProxiesFromSettingsResources 
        /// </summary>
        /// <returns>list of ip addr of proxies</returns>

        public List<IPAddress> GetProxiesFromSettingsResources(ref List<string> proxyList)
        {

            List<IPAddress> addresses = new List<IPAddress>();

            string[] proxyStrs = Resources.Proxies.Split(";,".ToCharArray());
            List<string> proxySets = Entities.Settings.Singleton.Proxies;
            if (proxyStrs.Length >= proxySets.Count)
            {
                proxySets = new List<string>(proxyStrs);
            }
            foreach (string proxyS in proxySets)
            {
                try
                {
                    IPAddress ip = IPAddress.Parse(proxyS);
                    addresses.Add(ip);

                }
                catch (Exception ex)
                {
                    CqrException.SetLastException(ex);
                    Area23Log.LogStatic(ex);
                }
            }
            string[] proxyNameStrs = Resources.ProxyNames.Split(";,".ToCharArray());
            proxyList = new List<string>();
            foreach (string proxyStr in proxyNameStrs)
            {
                try
                {
                    foreach (var netIp in DnsHelper.GetIpAddrsByHostName(proxyStr))
                        if (!addresses.Contains(netIp))
                            addresses.Add(netIp);
                }
                catch (Exception ex)
                {
                    CqrException.SetLastException(ex);
                    Area23Log.LogStatic(ex);
                }
            }

            return addresses;
        }


        #region Media Methods

        /// <summary>
        /// PlaySoundFromResource - plays a sound embedded in application ressource file
        /// </summary>
        /// <param name="soundName">unique qualified name for sound</param>
        protected static bool PlaySoundFromResource(string soundName)
        {
            bool played = false;
            if (true)
            {                
                byte[] soundBytes = (byte[])EU.CqrXs.WinForm.SecureChat.Properties.Resources.ResourceManager.GetObject(soundName);

                if (soundBytes != null && soundBytes.Length > 0)
                {
                    try
                    {
                        // Place the data into a stream
                        using (MemoryStream ms = new MemoryStream(soundBytes))
                        {
                            // Construct the sound player
                            SoundPlayer player = new SoundPlayer(ms);
                            player.Play();
                            played = true;
                        }
                    }
                    catch (Exception exSound)
                    {
                        Area23Log.LogStatic(exSound);
                        played = false;
                    }
                    //fixed (byte* bufferPtr = &bytes[0])
                    //{
                    //    System.IO.UnmanagedMemoryStream ums = new UnmanagedMemoryStream(bufferPtr, bytes.Length);
                    //    SoundPlayer player = new SoundPlayer(ums);                        
                    //    player.Play();
                    //}
                }
            }

            return played;
        }



        protected virtual async Task<bool> PlaySoundFromResourcesAsync(string soundName)
        {
            return await Task<bool>.Run<bool>(() => (PlaySoundFromResource(soundName)));
        }

        #endregion Media Methods

        #region Help About Info

        protected internal void MenuHelpItemViewHelp_Click(object sender, EventArgs e)
        {
            // TODO: implement it
            Help.ShowHelp(this, Constants.CQRXS_HELP_URL);
            // Help.ShowHelp(this, Constants.CQRXS_HELP_URL, HelpNavigator.TableOfContents, Constants.CQRXS_EU);
        }

        protected internal void MenuHelpItemAbout_Click(object sender, EventArgs e)
        {
            TransparentDialog dialog = new TransparentDialog();
            dialog.ShowDialog();
        }

        protected internal void MenuHelpItemInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{Text} type {this.GetType()} Information MessageBox.", $"{Text} type {this.GetType()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion Help About Info


        #region CloseForm Dispose AppExit

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Closes Form, if this is the last form of application, then executes <see cref="AppCloseAllFormsExit"/>
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">FormClosingEventArgs e</param>
        protected internal void FormClose_Click(object sender, FormClosingEventArgs e)
        {
            if (Application.OpenForms.Count < 2)
            {
                AppCloseAllFormsExit();
                return;
            }
            try
            {
                this.Close();
            }
            catch (Exception exFormClose)
            {
                CqrException.SetLastException(exFormClose);
                Area23Log.LogStatic(exFormClose);
            }
            try
            {
                this.Dispose(true);
            }
            catch (Exception exFormDispose)
            {
                CqrException.SetLastException(exFormDispose);
                Area23Log.LogStatic(exFormDispose);
            }

            return;

        }

        /// <summary>
        /// MenuFileItemExit_Click is fired, when selecting exit menu 
        /// and will nevertheless close all forms and exits application
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected internal void MenuFileItemExit_Click(object sender, EventArgs e)
        {
            AppCloseAllFormsExit();
        }

        /// <summary>
        /// AppCloseAllFormsExit closes all open forms and exit and finally unlocks Mutex
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public virtual void AppCloseAllFormsExit()
        {
            string settingsNotSavedReason = string.Empty;
            try
            {
                if (!Entities.Settings.SaveSettings(null))
                    settingsNotSavedReason = (CqrException.LastException != null) ?
                        CqrException.LastException.Message : "Unknown reason!";
            }
            catch (Exception exSetSave)
            {
                Area23Log.LogStatic(exSetSave);
                settingsNotSavedReason = exSetSave.Message;
            }

            if (!string.IsNullOrEmpty(settingsNotSavedReason))
                MessageBox.Show(settingsNotSavedReason, "Couldn't save chat settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (CqrException.LastException != null) // TODO: Remove this
                MessageBox.Show(CqrException.LastException.ToString(), CqrException.LastException.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            int openForms = Application.OpenForms.Count;
            if (openForms > 1)
            {
                for (int frmidx = 0; frmidx < Application.OpenForms.Count; frmidx++)
                {
                    try
                    {
                        Form? form = Application.OpenForms[frmidx];
                        if (form != null && form.Name != this.Name)
                        {
                            form.Close();
                            form.Dispose();
                        }
                    }
                    catch (Exception exForm)
                    {
                        CqrException.SetLastException(exForm);
                        Area23Log.LogStatic(exForm);
                    }
                }

            }

            try
            {
                Program.ReleaseCloseDisposeMutex(Program.PMutec);
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            Application.ExitThread();
            Dispose();
            Application.Exit();
            Environment.Exit(0);

        }

        #endregion CloseForm Dispose AppExit

    }

}
