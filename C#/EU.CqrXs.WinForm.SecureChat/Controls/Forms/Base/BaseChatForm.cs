using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
// using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Net.NameService;
using Area23.At.Framework.Core.Net.WebHttp;
// using System.Windows.Controls;
// using static System.Windows.Forms.VisualStyles.VisualStyleElement;
// using static System.Windows.Forms.MonthCalendar;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Properties;
using EU.CqrXs.WinForm.SecureChat.Util;
using System.ComponentModel;
using System.Media;
using System.Net;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Area23FwCore = Area23.At.Framework.Core;
// using static System.Net.Mime.MediaTypeNames;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms.Base
{
    // make it abstract l8r again

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
        static protected internal bool loaded = false;
        protected internal string _CqrXsServerKey = string.Empty;
        protected string savedFile = string.Empty;
        protected string loadDir = string.Empty;
        private System.ComponentModel.IContainer components = null;
        protected internal static DateTime LastExternalTime = DateTime.MinValue, LastExternalTimeV6 = DateTime.MinValue;
        protected internal static IPAddress? _externalIPAddress, _externalIPAddressV6;
        protected internal static List<string> _sProxies = new List<string>();
        protected internal static List<IPAddress> _proxies = new List<IPAddress>();
        protected internal static Lock _sLock = new Lock(), _sLock0 = new Lock(), _sLock1 = new Lock();
        protected internal Lock _lock = new Lock();
        protected internal OpenFileDialog FileOpenDialog;
        protected internal SaveFileDialog FileSaveDialog;
        protected internal PeerSession3State PeerSessionTriState = PeerSession3State.None;
        protected internal BgWorkerMonitor bgWorkerMonitor;

        #endregion fields

        #region Properties

        public static bool ProxiesInSettings
        {
            get => Settings.Singleton != null && Settings.Singleton.Proxies != null &&
                Settings.Singleton.Proxies.Count > 0;
        }

        public static bool FriendIPsInSettings
        {
            get => Settings.Singleton != null && Settings.Singleton.FriendIPs != null &&
                Settings.Singleton.FriendIPs.Count > 0;
        }


        public string? CqrXsEuSrvKey
        {
            get
            {
                lock (_lock)
                {
                    // _CqrXsServerKey = Constants.AUTHOR_EMAIL;

                    if (string.IsNullOrEmpty(_CqrXsServerKey) ||  DateTime.Now.Subtract(LastExternalTime).TotalSeconds >= 1800)
                    {
                        if (ExternalIpAddressV6 != null && ExternalIpAddressV6?.AddressFamily == AddressFamily.InterNetworkV6)
                            _CqrXsServerKey += ExternalIpAddressV6.ToString();
                        else if (ExternalIpAddress != null && ExternalIpAddress.AddressFamily == AddressFamily.InterNetwork)
                            _CqrXsServerKey += ExternalIpAddress.ToString();
                        else // we cannot reach the server, because no network
                            return null;
                        _CqrXsServerKey += Constants.APP_NAME;
                    }
                }
                return _CqrXsServerKey;
            }
        }

        public static IPAddress? ExternalIpAddress
        {
            get
            {
                if (_externalIPAddress != null && DateTime.Now.Subtract(LastExternalTime).TotalSeconds < 1800)
                {
                    return _externalIPAddress;
                }

                LastExternalTime = DateTime.Now;
                try
                {
                    _externalIPAddress = WebClientRequest.ExternalClientIpFromServer("https://ipv4.cqrxs.eu/net/R.aspx");
                }
                catch (Exception exNoInet)
                {
                    Area23Log.LogOriginMsgEx("BaseChatForm", "No external ip address", exNoInet);
                    _externalIPAddress = IPAddress.Parse("0.0.0.0");
                }
                return _externalIPAddress;
            }
        }

        public static IPAddress? ExternalIpAddressV6
        {
            get
            {
                if (_externalIPAddressV6 != null && DateTime.Now.Subtract(LastExternalTimeV6).TotalSeconds < 1800)
                {
                    return _externalIPAddressV6;
                }

                try
                {
                    LastExternalTimeV6 = DateTime.Now;
                    _externalIPAddressV6 = WebClientRequest.ExternalClientIpFromServer("https://ipv6.cqrxs.eu/net/R.aspx");
                }
                catch (Exception noIPv6Ex)
                {
                    Area23Log.LogOriginMsgEx("BaseChatForm", "ExternalIpAddressV6.get", noIPv6Ex);
                    _externalIPAddressV6 = null;
                }
                return _externalIPAddressV6;
            }
        }


        public static List<IPAddress> Proxies { get => GetProxiesFromSettingsResources(); }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected internal List<IPAddress> InterfaceIpAddresses { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected internal List<IPAddress> ConnectedIpAddresses { get; set; }


        protected OpenFileDialog DialogFileOpen
        {
            get
            {
                if (FileOpenDialog == null)
                {
                    FileOpenDialog = new OpenFileDialog();

                    string? udir = Environment.GetEnvironmentVariable("USERPROFILE");
                    if (string.IsNullOrEmpty(udir) || !Directory.Exists(udir))
                        udir = Application.StartupPath;
                    if (string.IsNullOrEmpty(udir) || !Directory.Exists(udir))
                        udir = Application.ExecutablePath;
                    if (string.IsNullOrEmpty(udir) || !Directory.Exists(udir))
                        udir = AppDomain.CurrentDomain.BaseDirectory;
                    if (string.IsNullOrEmpty(udir) || !Directory.Exists(udir))
                        udir = Application.UserAppDataPath;
                    if (!string.IsNullOrEmpty(udir) && Directory.Exists(udir))
                        FileOpenDialog.InitialDirectory = udir;
                }

                FileOpenDialog.FileName = "";
                FileOpenDialog.Title = "CqrChat: choose file";

                FileOpenDialog.Filter = "All files (*.*)|*.*|TXT (*.txt)|*.txt|PDF (*.pdf)|*.pdf|JPG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif";

                FileOpenDialog.RestoreDirectory = true;
                FileOpenDialog.ShowHiddenFiles = true;
                FileOpenDialog.AddExtension = true;

                FileOpenDialog.SupportMultiDottedExtensions = true;
                FileOpenDialog.CheckPathExists = true;
                FileOpenDialog.CheckFileExists = true;

                return FileOpenDialog;
            }
        }


        /// <summary>
        /// DialogFileSave returns initialized FileSaveDialog type SaveFileDialog
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected SaveFileDialog DialogFileSave
        {
            get
            {
                if (FileSaveDialog == null)
                {
                    FileSaveDialog = new SaveFileDialog();
                    string? udir = Environment.GetEnvironmentVariable("TEMP");
                    if (string.IsNullOrEmpty(udir) || !Directory.Exists(udir))
                        udir = Application.StartupPath;
                    if (string.IsNullOrEmpty(udir) || !Directory.Exists(udir))
                        udir = Application.ExecutablePath;
                    if (string.IsNullOrEmpty(udir) || !Directory.Exists(udir))
                        udir = AppDomain.CurrentDomain.BaseDirectory;
                    if (string.IsNullOrEmpty(udir) || !Directory.Exists(udir))
                        udir = Application.UserAppDataPath;
                    if (!string.IsNullOrEmpty(udir) && Directory.Exists(udir))
                        FileSaveDialog.InitialDirectory = udir;
                }

                FileSaveDialog.Title = "Save File";
                FileSaveDialog.FileName = "";

                FileSaveDialog.RestoreDirectory = true;
                FileSaveDialog.ShowHiddenFiles = true;
                FileSaveDialog.SupportMultiDottedExtensions = true;
                FileSaveDialog.CheckPathExists = true;
                FileSaveDialog.CheckFileExists = true;

                return FileSaveDialog;
            }
        }


        #endregion Properties

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseChatForm() { }


        #region thread save WinForm delegate callbacks

        #region TextBox&RichTextBox

        internal delegate void EnableTextBoxCallback(System.Windows.Forms.TextBox textBox, bool enabled);

        internal delegate void EnableListBoxCallback(System.Windows.Forms.ListBox listBox, bool enabled);

        internal delegate string GetTextBoxTextCallback(System.Windows.Forms.TextBox textBox);

        internal delegate string GetRichTextBoxTextCallback(System.Windows.Forms.RichTextBox textBox);

        internal delegate void SetTextCallback(System.Windows.Forms.TextBox textBox, string text);

        internal delegate void SetRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, string text);

        internal delegate void AppendTextCallback(System.Windows.Forms.TextBox textBox, string text);

        internal delegate void AppendRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, string text);

        internal delegate void RichTextFromPositionWithLengthAlignCallback(
                System.Windows.Forms.RichTextBox richArea,
                int pos0,
                int len,
                HorizontalAlignment hlr
            );

        internal delegate void SelectionAlignmentRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, HorizontalAlignment leftRight);

        internal delegate int GetFirstCharIndexFromLineRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, int lineNr);

        internal delegate void ClearRichTextCallback(System.Windows.Forms.RichTextBox richTextBox, bool clear = true);

        internal delegate int GetLastIndexOfSubstringCallback(System.Windows.Forms.RichTextBox richTextBox, string pattern);

        internal delegate void DeselectAllRichTextCallback(System.Windows.Forms.RichTextBox richTextBox);


        internal void EnableTextBox(System.Windows.Forms.TextBox textBox, bool enable)
        {            
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (textBox.InvokeRequired)
            {
                EnableTextBoxCallback enableTextBoxCallback = // new EnableTextBoxCallback(EnableTextBox);
                    delegate (System.Windows.Forms.TextBox txtBx, bool ena)
                    {
                        if (txtBx != null)
                            txtBx.Enabled = ena;
                    };
                try
                {
                    textBox.Invoke(enableTextBoxCallback, new object[] { textBox, enable });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate EnableTextBox enable: \"{enable}\".\n", exDelegate);
                }
            }
            else
            {
                if (textBox != null)
                    textBox.Enabled = enable;
            }
        }


        internal void EnableListBox(System.Windows.Forms.ListBox listBox, bool enable)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBox.InvokeRequired)
            {
                EnableListBoxCallback enableListBoxCallback = // new EnableTextBoxCallback(EnableTextBox);
                    delegate (System.Windows.Forms.ListBox lstBx, bool ena)
                    {
                        if (lstBx != null)
                            lstBx.Enabled = ena;
                    };
                try
                {
                    listBox.Invoke(enableListBoxCallback, new object[] { listBox, enable });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate EnableTextBox enable: \"{enable}\".\n", exDelegate);
                }
            }
            else
            {
                if (listBox != null)
                    listBox.Enabled = enable;
            }
        }


        internal string GetTextBoxText(System.Windows.Forms.TextBox textBox)
        {
            string returnTxt = "";
            if (textBox.InvokeRequired)
            {
                GetTextBoxTextCallback getTextBoxTextCallback =
                    delegate (System.Windows.Forms.TextBox txtbox)
                    {
                        return (txtbox != null && txtbox.Text != null) ? txtbox.Text : "";
                    };
                try
                {
                    returnTxt = (string)textBox.Invoke(getTextBoxTextCallback, new object[] { textBox });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetTextBoxText(RichTextBox textBox): \"{exDelegate.Message}\".\n", exDelegate);
                }
            }
            else
            {
                returnTxt = (textBox != null && textBox.Text != null) ? textBox.Text : "";
            }

            return returnTxt;
        }

        internal string GetRichTextBoxText(System.Windows.Forms.RichTextBox textBox)
        {
            string returnTxt = "";
            if (textBox.InvokeRequired)
            {
                GetRichTextBoxTextCallback getRichTextBoxTextCallback =
                    delegate (System.Windows.Forms.RichTextBox txtbox)
                    {
                        return (txtbox != null && txtbox.Text != null) ? txtbox.Text : "";
                    };
                try
                {
                    returnTxt = (string)textBox.Invoke(getRichTextBoxTextCallback, new object[] { textBox });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetRichTextBoxText(RichTextBox textBox): \"{exDelegate.Message}\".\n", exDelegate);
                }
            }
            else
            {
                returnTxt = (textBox != null && textBox.Text != null) ? textBox.Text : "";                
            }

            return returnTxt;
        }

        /// <summary>
        /// AppendText - appends text on a <see cref="System.Windows.Forms.TextBox"/>
        /// </summary>
        /// <param name="textBox"><see cref="System.Windows.Forms.TextBox"/></param>
        /// <param name="text"><see cref="string">string text</see> to set</param>
        internal void AppendText(System.Windows.Forms.TextBox textBox, string text)
        {
            string textToAppend = !string.IsNullOrEmpty(text) ? text : string.Empty;

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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate AppendText text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (textBox != null && textBox.Text != null && textToAppend != null)
                    textBox.AppendText(textToAppend);
            }
        }

        internal void SetTextBoxText(System.Windows.Forms.TextBox textBox, string text)
        {
            string textToSet = !string.IsNullOrEmpty(text) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (textBox.InvokeRequired)
            {
                SetTextCallback setTextCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.TextBox textArea, string setTxt)
                    {
                        if (textArea != null && textArea.Text != null && setTxt != null)
                            textArea.Text = setTxt;
                    };
                try
                {
                    textBox.Invoke(setTextCallback, new object[] { textBox, textToSet });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetRichText text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (textBox != null && textBox.Text != null && textToSet != null)
                    textBox.Text = textToSet;
            }
        }


        internal void SetRichText(System.Windows.Forms.RichTextBox richTextBox, string text)
        {
            string textToSet = !string.IsNullOrEmpty(text) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                SetRichTextCallback setRichTextCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.RichTextBox textArea, string setTxt)
                    {
                        if (textArea != null && textArea.Text != null && setTxt != null)
                            textArea.Text = setTxt;
                    };
                try
                {
                    richTextBox.Invoke(setRichTextCallback, new object[] { richTextBox, textToSet });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetRichText text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && richTextBox.Text != null && textToSet != null)
                    richTextBox.Text = textToSet;
            }
        }

        internal void AppendRichText(System.Windows.Forms.RichTextBox richTextBox, string text)
        {
            string textToAppend = !string.IsNullOrEmpty(text) ? text : string.Empty;

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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate AppendRichText text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && richTextBox.Text != null && textToAppend != null)
                    richTextBox.AppendText(textToAppend);
            }
        }

        internal void RichTextFromPositionWithLengthAlign(
            System.Windows.Forms.RichTextBox richTextBox,
            int start,
            int length,
            HorizontalAlignment hAlignment = HorizontalAlignment.Left)
        {
            start = start < 0 ? 0 : start;
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                RichTextFromPositionWithLengthAlignCallback richTextFromPositionWithLengthAlignCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.RichTextBox richArea, int pos0, int len, HorizontalAlignment hlr)
                    {
                        if (richArea != null && !string.IsNullOrEmpty(richArea.Text))
                        {
                            pos0 = pos0 < 0 ? 0 : pos0;
                            richArea.Select(pos0, len);
                            richArea.SelectionAlignment = hlr;
                            //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(pos0.ToString() + len.ToString());
                            //string hex = bytes.ToHexString();

                            //Color c = Color.AliceBlue;
                            //if (hlr == HorizontalAlignment.Left)
                            //    c = new Color().FromXrgb($"ff{hex.Substring(0, 2)}{hex.Substring(2, 2)}");
                            //if (hlr == HorizontalAlignment.Right)
                            //    c = new Color().FromXrgb($"{hex.Substring(0, 2)}{hex.Substring(2, 2)}ff");
                            //if (hlr == HorizontalAlignment.Center)
                            //    c = new Color().FromXrgb($"{hex.Substring(0, 2)}ff{hex.Substring(2, 2)}");
                            //richArea.SelectionBackColor = c;
                            richArea.Update();
                            // richArea.DeselectAll();
                        }
                    };
                try
                {
                    richTextBox.Invoke(
                        richTextFromPositionWithLengthAlignCallback,
                        new object[] { richTextBox, start, length, hAlignment });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx("RichTextFromPositionWithLengthAlign",
                        $"Exception in delegate RichTextFromPositionWithLengthAlign RichTextBox: " +
                        $"RichTextFromPositionWithLengthAlign(RichTextBox richTextBox = \"{richTextBox.Name}\", int start = {start}, int length = {length}, HorizontalAlignment hAlignment = {hAlignment}) ...\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && !string.IsNullOrEmpty(richTextBox.Text))
                {

                    richTextBox.Select(start < 0 ? 0 : start, length);
                    richTextBox.SelectionAlignment = hAlignment;
                    //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(start.ToString() + length.ToString());
                    //string hex = bytes.ToHexString();

                    //Color c = Color.AliceBlue;
                    //if (hAlignment == HorizontalAlignment.Left)
                    //    c = new Color().FromXrgb($"ff{hex.Substring(0, 2)}{hex.Substring(2, 2)}");
                    //if (hAlignment == HorizontalAlignment.Right)
                    //    c = new Color().FromXrgb($"{hex.Substring(0, 2)}{hex.Substring(2, 2)}ff");
                    //if (hAlignment == HorizontalAlignment.Center)
                    //    c = new Color().FromXrgb($"{hex.Substring(0, 2)}ff{hex.Substring(2, 2)}");
                    //richTextBox.SelectionBackColor = c;
                    // richTextBox.DeselectAll();
                    richTextBox.Update();
                }
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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SelectionAlignmentRichText: \"{leftRight}\".\n", exDelegate);
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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetFirstCharIndexFromLineRichText({lineNr}).\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && richTextBox.Text != null)
                    return richTextBox.GetFirstCharIndexFromLine(lineNr);
            }
            return -1;
        }

        internal int GetLastIndexOfSubstring(System.Windows.Forms.RichTextBox richTextBox, string pattern)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                GetLastIndexOfSubstringCallback getLastIndexOfSubstringCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.RichTextBox textArea, string patterns)
                    {
                        if (textArea != null &&
                            !string.IsNullOrEmpty(textArea.Text) &&
                            !string.IsNullOrEmpty(patterns))
                        {
                            if (textArea.Text.Contains(patterns, StringComparison.InvariantCultureIgnoreCase) ||
                                textArea.Text.IndexOf(patterns) > -1)
                            {
                                return textArea.Text.LastIndexOf(patterns);
                            }
                        }

                        return -1;
                    };
                try
                {
                    richTextBox.Invoke(getLastIndexOfSubstringCallback, new object[] { richTextBox, pattern });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetLastIndexOfSubstring(richTextBox = {richTextBox.Name}, pattern = {pattern}).\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && !string.IsNullOrEmpty(richTextBox.Text) && !string.IsNullOrEmpty(pattern) && richTextBox.Text.Contains(pattern))
                    return richTextBox.Text.LastIndexOf(pattern);
            }
            return -1;
        }


        internal void DeselectAllRichText(System.Windows.Forms.RichTextBox richTextBox)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox.InvokeRequired)
            {
                DeselectAllRichTextCallback deselectAllRichTextCallback = // new AppendTextCallback(SetTextSpooler);
                    delegate (System.Windows.Forms.RichTextBox textArea)
                    {
                        if (textArea != null && !string.IsNullOrEmpty(textArea.Text))
                            textArea.DeselectAll();
                        return;
                    };
                try
                {
                    richTextBox.Invoke(deselectAllRichTextCallback, new object[] { richTextBox });
                    // textBox.Invoke((System.Reflection.MethodInvoker)delegate { textBox.AppendText(text); });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate DeselectAllRichText(richTextBox = {richTextBox.Name}).\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && !string.IsNullOrEmpty(richTextBox.Text))
                    richTextBox.DeselectAll();
            }
            return;
        }

        /// <summary>
        /// ClearRichText thread save accessor
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="clear"></param>
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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate ClearRichText: \"{exDelegate.Message}\".\n", exDelegate);
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
            string textToSet = !string.IsNullOrEmpty(text) ? text : string.Empty;

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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetLinkLabelText text: \"{text}\".\n", exDelegate);
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
            string linkToAdd = !string.IsNullOrEmpty(link) ? link : string.Empty;

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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate AddLinkLabelLinks: \"{link}\".\n", exDelegate);
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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetLinkLabelVisible visible: \"{visible}\".\n", exDelegate);
                }
            }
            else
            {
                if (linkLabel != null)
                    linkLabel.Visible = visible;
            }
        }

        #endregion LinkLabel

        #region ToolStripItemCollection

        internal delegate int GetProgressBarCallback(ToolStripProgressBar progressBar);        
        internal delegate void SetProgressBarCallback(ToolStripProgressBar progressBar, int progress);
        internal delegate string GetMenuItemTextCallback(ToolStripMenuItem menuItem);
        internal delegate void SetMenuItemEnabledCheckedCallback(ToolStripMenuItem tsmItem, bool miEnabled, bool miChecked);
        internal delegate void SetMenuItemTextCallback(ToolStripMenuItem menuItem, string text);

        internal void ResetProgressBar(ToolStripProgressBar progressBar) => SetProgressBar(progressBar, 0);

        internal int GetProgressBar(ToolStripProgressBar progressBar)
        {
            int progressValue = 0;
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (progressBar != null && progressBar.GetCurrentParent() != null && progressBar.GetCurrentParent().InvokeRequired)
            {
                GetProgressBarCallback getProgressBarCallback = delegate (ToolStripProgressBar pbar)
                {
                    return (pbar != null) ? pbar.Value : 0;
                };
                try
                {
                    progressValue = (int)progressBar.GetCurrentParent().Invoke(getProgressBarCallback, new object[] { progressBar });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetProgressBar progress might be {progressValue}.\n", exDelegate);
                }
            }
            else
            {
                progressValue = (progressBar != null) ? progressBar.Value : 0;
            }

            return progressValue;
        }

        internal void SetProgressBar(ToolStripProgressBar progressBar, int progress)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (progressBar != null && progressBar.GetCurrentParent() != null && progressBar.GetCurrentParent().InvokeRequired)
            {
                SetProgressBarCallback setProgressBarCallback = delegate (ToolStripProgressBar pbar, int prgrss)
                {
                    if (pbar != null)
                        pbar.Value = (prgrss < 100) ? progress : 100;
                };
                try
                {
                    progressBar.GetCurrentParent().Invoke(setProgressBarCallback, new object[] { progressBar, progress });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetProgressBar to {progress}.\n", exDelegate);
                }
            }
            else
            {
                if (progressBar != null)
                    progressBar.Value = (progress > 100) ? 100 : progress;
            }

            return;
        }

        internal string GetMenuItemText(ToolStripMenuItem mItem)
        {
            string reText = string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem != null && mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                GetMenuItemTextCallback getMenuItemTextCallback = delegate (ToolStripMenuItem tsMenuItem)
                {
                    return tsMenuItem != null && tsMenuItem.Text != null ? tsMenuItem.Text : string.Empty;
                };
                try
                {
                    reText = (string)mItem.GetCurrentParent().Invoke(getMenuItemTextCallback, new object[] { mItem });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItemText menuItem = {mItem.Name}.\n", exDelegate);
                }
            }
            else
            {
                reText = mItem != null && mItem.Text != null ? mItem.Text : string.Empty;
            }

            return reText;
        }
        
        internal void SetMenuItemText(ToolStripMenuItem mItem, string text)
        {
            string setText = !string.IsNullOrEmpty(text) ? text : string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                SetMenuItemTextCallback setMenuItemTextCallback = delegate (ToolStripMenuItem tsMenuItem, string setText)
                {
                    if (tsMenuItem != null && setText != null)
                        tsMenuItem.Text = setText;
                };
                try
                {
                    mItem.GetCurrentParent()?.Invoke(setMenuItemTextCallback, new object[] { mItem, setText });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemText menu item: {mItem.Name}, text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (mItem != null && setText != null)
                    mItem.Text = setText;
            }
        }

        internal void SetMenuItemEnabledChecked(ToolStripMenuItem tsMenuItem, bool tsmiEnabled, bool tsmiChecked)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (tsMenuItem.GetCurrentParent() != null && tsMenuItem.GetCurrentParent().InvokeRequired)
            {
                SetMenuItemEnabledCheckedCallback setMenuItemEnabledCheckedCallback = 
                    delegate (ToolStripMenuItem tsmItem, bool miEnabled, bool miChecked)
                {
                    if (tsmItem != null)
                    {
                        tsmItem.Enabled = miEnabled;
                        tsmItem.Checked = miChecked;
                    }                        
                };
                try
                {
                    tsMenuItem.GetCurrentParent()?.Invoke(setMenuItemEnabledCheckedCallback, new object[] { tsMenuItem, tsmiEnabled, tsmiChecked });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemEnabledChecked menu item: {tsMenuItem.Name}, enabled: \"{tsmiEnabled}\", checked: \"{tsmiChecked}\".\n", exDelegate);
                }
            }
            else
            {
                if (tsMenuItem != null)
                {
                    tsMenuItem.Enabled = tsmiEnabled;
                    tsMenuItem.Checked = tsmiChecked;
                }
            }

        }

        internal delegate Color GetMenuItemForeColorCallback(ToolStripMenuItem menuItem);
        internal delegate void SetMenuItemForeColorCallback(ToolStripMenuItem menuItem, Color foreColor);

        internal Color GetMenuItemForeColor(ToolStripMenuItem mItem)
        {
            Color foreColor = SystemColors.MenuText;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem != null && mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                GetMenuItemForeColorCallback getMenuItemForeColorCallback = delegate (ToolStripMenuItem tsMenuItem)
                {
                    return tsMenuItem != null && tsMenuItem.ForeColor != null ? tsMenuItem.ForeColor : SystemColors.MenuText;
                };
                try
                {
                    foreColor = (Color)mItem.GetCurrentParent().Invoke(getMenuItemForeColorCallback, new object[] { mItem });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItemForeColor menuItem = {mItem.Name}.\n", exDelegate);
                }
            }
            else
            {
                foreColor = mItem != null && mItem.ForeColor != null ? mItem.ForeColor : SystemColors.MenuText;
            }

            return foreColor;
        }
        internal void SetMenuItemForeColor(ToolStripMenuItem mItem, Color foreColor)
        {
            Color fgColor = foreColor != null ? foreColor : SystemColors.MenuText;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem != null && mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                SetMenuItemForeColorCallback setMenuItemForeColorCallback = delegate (ToolStripMenuItem tsMenuItem, Color fgColor)
                {
                    if (tsMenuItem != null && fgColor != null)
                        tsMenuItem.ForeColor = fgColor;
                };
                try
                {
                    mItem.GetCurrentParent()?.Invoke(setMenuItemForeColorCallback, new object[] { mItem, fgColor });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemText menu item: {mItem.Name}, Color: \"{foreColor}\".\n", exDelegate);
                }
            }
            else
            {
                if (mItem != null && fgColor != null)
                    mItem.ForeColor = fgColor;
            }
        }


        internal delegate Color GetMenuItemBackColorCallback(ToolStripMenuItem menuItem);
        internal delegate void SetMenuItemBackColorCallback(ToolStripMenuItem menuItem, Color backColor);

        internal Color GetMenuItemBackColor(ToolStripMenuItem mItem)
        {
            Color bgColor = SystemColors.MenuBar;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem != null && mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                GetMenuItemBackColorCallback getMenuItemBackColorCallback = delegate (ToolStripMenuItem tsMenuItem)
                {
                    return tsMenuItem != null && tsMenuItem.BackColor != null ? tsMenuItem.BackColor : SystemColors.MenuBar;
                };
                try
                {
                    bgColor = (Color)mItem.GetCurrentParent().Invoke(getMenuItemBackColorCallback, new object[] { mItem });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItemBackColor menuItem = {mItem.Name}.\n", exDelegate);
                }
            }
            else
            {
                bgColor = mItem != null && mItem.BackColor != null ? mItem.BackColor : SystemColors.MenuBar;
            }

            return bgColor;
        }
        internal void SetMenuItemBackColor(ToolStripMenuItem mItem, Color backColor)
        {
            Color bgColor = backColor != null ? backColor : SystemColors.MenuBar;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem != null && mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                SetMenuItemBackColorCallback setMenuItemBackColorCallback = delegate (ToolStripMenuItem tsMenuItem, Color bgColor)
                {
                    if (tsMenuItem != null && bgColor != null)
                        tsMenuItem.BackColor = bgColor;
                };
                try
                {
                    mItem.GetCurrentParent()?.Invoke(setMenuItemBackColorCallback, new object[] { mItem, bgColor });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemBackColor menu item: {mItem.Name}, Color: \"{bgColor}\".\n", exDelegate);
                }
            }
            else
            {
                if (mItem != null && bgColor != null)
                    mItem.BackColor = bgColor;
            }
        }


        internal delegate bool GetMenuItemCheckedCallback(ToolStripMenuItem menuItem);
        internal delegate void SetMenuItemCheckedCallback(ToolStripMenuItem menuItem, bool mchecked);
        internal bool GetMenuItemChecked(ToolStripMenuItem mItem)
        {
            bool mchecked = false;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem != null && mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                GetMenuItemCheckedCallback getMenuItemCheckedCallback = delegate (ToolStripMenuItem tsMenuItem)
                {
                    return tsMenuItem != null ? tsMenuItem.Checked : false;
                };
                try
                {
                    mchecked = (bool)mItem.GetCurrentParent().Invoke(getMenuItemCheckedCallback, new object[] { mItem });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItemChecked menuItem = {mItem.Name}.\n", exDelegate);
                }
            }
            else
            {
                mchecked = mItem != null ? mItem.Checked : false;
            }

            return mchecked;
        }
        internal void SetMenuItemChecked(ToolStripMenuItem mItem, bool mchecked)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem != null && mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                SetMenuItemCheckedCallback setMenuItemCheckedCallback = delegate (ToolStripMenuItem tsMenuItem, bool miChecked)
                {
                    if (tsMenuItem != null)
                        tsMenuItem.Checked = miChecked;
                };
                try
                {
                    mItem.GetCurrentParent()?.Invoke(setMenuItemCheckedCallback, new object[] { mItem, mchecked });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemChecked menu item: {mItem.Name}, checked: \"{mchecked}\".\n", exDelegate);
                }
            }
            else
            {
                if (mItem != null)
                    mItem.Checked = mchecked;
            }
        }

        internal delegate ToolStripItemCollection GetMenuItemsCallback(ToolStripMenuItem menuItem);
        internal delegate void AddMenuItemToItemsCallack(ToolStripMenuItem menuItem, ToolStripDropDownItem tsddItem);

        internal delegate System.Windows.Forms.ComboBox.ObjectCollection? GetMenuDropDownItemsCallback(ToolStripComboBox tsCombo);
        internal delegate void AddMenuItemToMenuComboBoxCallack(ToolStripComboBox tsCombo, object o);

        internal ToolStripItemCollection GetMenuItems(ToolStripMenuItem mItem)
        {

            ToolStripItemCollection tscol = new ToolStripItemCollection(mItem.GetCurrentParent(), new ToolStripDropDownItem[0]);

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem != null && mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                GetMenuItemsCallback getMenuItemsCallback = delegate (ToolStripMenuItem tsMenuItem)
                {
                    return tsMenuItem != null && tsMenuItem.DropDown != null && tsMenuItem.DropDownItems != null 
                        ? tsMenuItem.DropDown.Items 
                        : new ToolStripItemCollection(tsMenuItem.GetCurrentParent(), new ToolStripMenuItem[0]);
                };
                try
                {
                    tscol = (ToolStripItemCollection)mItem.GetCurrentParent().Invoke(getMenuItemsCallback, new object[] { mItem });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItems menuItem = {mItem.Name}.\n", exDelegate);
                }
            }
            else
            {
                tscol = mItem != null && mItem.DropDown != null && mItem.DropDownItems != null ?
                    mItem.DropDown.Items : new ToolStripItemCollection(mItem.GetCurrentParent(), new ToolStripMenuItem[0]);

            }

            return tscol;
        }
        internal void AddMenuItemToItems(ToolStripMenuItem mItem, ToolStripDropDownItem tsddItem)
        {
            ToolStripMenuItem addItem = mItem != null && tsddItem != null ?
                (ToolStripMenuItem)tsddItem : new ToolStripMenuItem();

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                AddMenuItemToItemsCallack addMenuItemToItemsCallack =
                    delegate (ToolStripMenuItem tsMenuItem, ToolStripDropDownItem ddItem)
                {
                    if (tsMenuItem != null && tsMenuItem.DropDown != null && tsMenuItem.DropDown.Items != null && ddItem != null)
                        tsMenuItem.DropDown.Items.Add((ToolStripMenuItem)ddItem);
                };
                try
                {
                    mItem.GetCurrentParent()?.Invoke(addMenuItemToItemsCallack, new object[] { mItem, addItem });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate AddMenuItemToItems menu item: {mItem.Name}, add item: \"{addItem.Name}\".\n", exDelegate);
                }
            }
            else
            {
                if (mItem != null && mItem.DropDown != null && mItem.DropDown.Items != null && addItem != null)
                    mItem.DropDown.Items.Add(addItem);
            }
        }

        internal System.Windows.Forms.ComboBox.ObjectCollection? GetMenuDropDownItems(ToolStripComboBox tsCbx)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (tsCbx != null && tsCbx.GetCurrentParent() != null && tsCbx.GetCurrentParent().InvokeRequired)
            {
                GetMenuDropDownItemsCallback getMenuDropDownItemsCallback = delegate (ToolStripComboBox tsComboBox)
                {
                    return tsComboBox != null && tsComboBox.Items != null ?
                        tsComboBox.Items : null;
                };
                try
                {
                    return (System.Windows.Forms.ComboBox.ObjectCollection?)tsCbx.GetCurrentParent().Invoke(getMenuDropDownItemsCallback, new object[] { tsCbx });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetMenuDropDownItems menuItemComboBox = {tsCbx.Name}.\n", exDelegate);
                }
            }
            else
            {
                return tsCbx != null && tsCbx.Items != null ?
                    tsCbx.Items : null;

            }

            return null;
        }
        internal void AddMenuItemToMenuComboBox(ToolStripComboBox tsCombo, object obj)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (tsCombo != null && obj != null && tsCombo.GetCurrentParent() != null && tsCombo.GetCurrentParent().InvokeRequired)
            {
                AddMenuItemToMenuComboBoxCallack addMenuItemToMenuComboBoxCallack =
                    delegate (ToolStripComboBox tsComboBox, object o)
                    {
                        if (tsComboBox != null && tsComboBox.Items != null && o != null)
                            tsComboBox.Items.Add(o);
                    };
                try
                {
                    tsCombo.GetCurrentParent()?.Invoke(addMenuItemToMenuComboBoxCallack, new object[] { tsCombo, obj });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate AddMenuItemToMenuComboBox menu combo box: {tsCombo.Name}, add object: \"{obj}\".\n", exDelegate);
                }
            }
            else
            {
                if (tsCombo != null && tsCombo.Items != null && obj != null)
                    tsCombo.Items.Add(obj);
            }
        }


        #endregion ToolStripItemCollection

        #region ToolStripStatusLabel

        internal delegate void SetToolStripStatusLabelTextCallback(ToolStripStatusLabel statusLabel, string text);

        internal void SetStatusText(ToolStripStatusLabel toolStatusLabel, string text)
        {
            string setText = !string.IsNullOrEmpty(text) ? text : string.Empty;

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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetStatusText text: \"{text}\".\n", exDelegate);
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

        internal delegate string? GetComboBoxNameCallback(System.Windows.Forms.ComboBox comboBox);
        internal delegate bool GetComboBoxEnabledCallback(System.Windows.Forms.ComboBox comboBox);
        internal delegate string GetComboBoxTextCallback(System.Windows.Forms.ComboBox comboBox);
        internal delegate System.Windows.Forms.ComboBox.ObjectCollection? GetComboBoxItemsCallback(System.Windows.Forms.ComboBox tsCombo);
        internal delegate void SetComboBoxEnabledCallback(System.Windows.Forms.ComboBox comboBox, bool enabled);
        internal delegate void SetComboBoxTextCallback(System.Windows.Forms.ComboBox comboBox, string text);
        internal delegate void SetComboBackColorCallback(System.Windows.Forms.ComboBox comboBox, Color color);
        internal delegate void FocusComboBoxCallback(System.Windows.Forms.ComboBox comboBox);
        internal delegate void AddItemToComboBoxCallack(System.Windows.Forms.ComboBox comboBox, object o);

        /// <summary>
        /// thread save deleagte to get name out of a <see cref="ComboBox"/>
        /// </summary>
        /// <param name="comboBox"><see cref="ComboBox"/> from which name to get</param>
        /// <returns><see cref="string"/> name from <see cref="ComboBox.Text" /></returns>
        internal string? GetComboBoxName(System.Windows.Forms.ComboBox comboBox)
        {
            string? getName = null;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox.InvokeRequired)
            {
                GetComboBoxNameCallback getComboBoxNameCallback =
                    delegate (System.Windows.Forms.ComboBox cmbx)
                    {
                        return (cmbx != null && cmbx.Name != null) ? cmbx.Name : null;
                    };
                try
                {
                    getName = (string)comboBox.Invoke(getComboBoxNameCallback, new object[] { comboBox });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetComboBoxName.\n", exDelegate);
                }
            }
            else
            {
                getName = (comboBox != null && comboBox.Name != null) ? comboBox.Name : null;
            }

            return getName;
        }


        /// <summary>
        /// thread save deleagte to get enabled state from <see cref="ComboBox"/>
        /// </summary>
        /// <param name="comboBox"><see cref="ComboBox"/> from which name to get</param>
        /// <returns><see cref="string"/> name from <see cref="ComboBox.Text" /></returns>
        internal bool GetComboBoxEnabled(System.Windows.Forms.ComboBox comboBox)
        {
            bool ena = false;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox.InvokeRequired)
            {
                GetComboBoxEnabledCallback getComboBoxEnabledCallback =
                    delegate (System.Windows.Forms.ComboBox cmbx)
                    {
                        return (cmbx != null) ? cmbx.Enabled : false;
                    };
                try
                {
                    ena = (bool)comboBox.Invoke(getComboBoxEnabledCallback, new object[] { comboBox });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetComboBoxName.\n", exDelegate);
                }
            }
            else
            {
                ena = (comboBox != null) ? comboBox.Enabled : false;
            }

            return ena;
        }


        /// <summary>
        /// thread save deleagte to get text out of a <see cref="ComboBox"/>
        /// </summary>
        /// <param name="comboBox"><see cref="ComboBox"/> from which text to get</param>
        /// <returns><see cref="string"/> text from <see cref="ComboBox.Text" /></returns>
        internal string GetComboBoxText(System.Windows.Forms.ComboBox comboBox)
        {
            string reText = string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox.InvokeRequired)
            {
                GetComboBoxTextCallback getComboBoxTextCallback =
                    delegate (System.Windows.Forms.ComboBox cmbx)
                    {
                        return cmbx != null && cmbx.Text != null ? cmbx.Text : string.Empty;
                    };
                try
                {
                    reText = (string)comboBox.Invoke(getComboBoxTextCallback, new object[] { comboBox });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetComboBoxText ComboBox = {comboBox.Name}.\n", exDelegate);
                }
            }
            else
            {
                reText = comboBox != null && comboBox.Text != null ? comboBox.Text : string.Empty;
            }

            return reText;
        }

        /// <summary>
        /// thread save deleagte to set text in a <see cref="ComboBox"/>
        /// </summary>
        /// <param name="comboBox"><see cref="ComboBox.Text" /> where set text <see cref="string"/></param>
        /// <param name="text">string text to set</param>
        internal void SetComboBoxText(System.Windows.Forms.ComboBox comboBox, string text)
        {
            string setText = !string.IsNullOrEmpty(text) ? text : string.Empty;

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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetComboBoxText ComboBox = {comboBox.Name}, text = \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null && setText != null)
                    comboBox.Text = setText;
            }
        }

        /// <summary>
        /// thread save deleagte to set enabled / disabled in a <see cref="ComboBox"/>
        /// </summary>
        /// <param name="comboBox"><see cref="ComboBox.Text" /> where set text <see cref="string"/></param>
        /// <param name="enabled"><see cref="true"/> for enable, <see cref="false"/> for disabled</param>
        internal void SetComboBoxEnabled(System.Windows.Forms.ComboBox comboBox, bool enabled)
        {           
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox.InvokeRequired)
            {
                SetComboBoxEnabledCallback setComboBoxEnabledCallback =
                    delegate (System.Windows.Forms.ComboBox cmbx, bool endisable)
                    {
                        if (cmbx != null)
                            cmbx.Enabled = endisable;
                    };
                try
                {
                    comboBox.Invoke(setComboBoxEnabledCallback, new object[] { comboBox, enabled });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetComboBoxEnabled ComboBox = {comboBox.Name}, enabled = \"{enabled}\".\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null)
                    comboBox.Enabled = enabled;
            }
        }
        
        /// <summary>
        /// thread save deleagte to change <see cref="ComboBox.BackColor" />
        /// </summary>
        /// <param name="comboBox"><see cref="ComboBox"/> where you want to change <see cref="ComboBox.BackColor"/></param>
        /// <param name="color"><see cref="Color"/> to set as new <see cref="ComboBox.BackColor"/></param>
        internal void SetComboBoxBackColor(System.Windows.Forms.ComboBox comboBox, Color color)
        {
            Color setColor = color != null ? color : Color.Transparent;
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
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate SetComboBoxBackColor ComboBox = {comboBox.Name}, Colore = \"{color}\".\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null && color != null)
                    comboBox.BackColor = color;
            }
        }

        internal void FocusComboBox(System.Windows.Forms.ComboBox comboBox)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox.InvokeRequired)
            {
                FocusComboBoxCallback focusComboBoxCallback =
                    delegate (System.Windows.Forms.ComboBox cmbx)
                    {
                        if (cmbx != null)
                        {
                            // cmbx.BringToFront();
                            cmbx.Focus();
                        }
                    };
                try
                {
                    comboBox.Invoke(focusComboBoxCallback, new object[] { comboBox });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate FocusComboBox ComboBox = {comboBox.Name}.\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null) 
                {                    
                    comboBox.Focus();
                    //comboBox.UseWaitCursor = true;
                }
            }
        }


        internal System.Windows.Forms.ComboBox.ObjectCollection? GetComboBoxItems(System.Windows.Forms.ComboBox comboBox)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox != null && comboBox.InvokeRequired)
            {
                GetComboBoxItemsCallback getComboBoxItemsCallback = delegate (System.Windows.Forms.ComboBox comboBx)
                {
                    return comboBx != null && comboBx.Items != null ? comboBx.Items : null;
                };
                try
                {
                    return (System.Windows.Forms.ComboBox.ObjectCollection?)comboBox.Invoke(getComboBoxItemsCallback, new object[] { comboBox });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate GetComboBoxItems ComboBox = {comboBox.Name}.\n", exDelegate);
                }
            }
            else
            {
                return comboBox != null && comboBox.Items != null ? comboBox.Items : null;

            }

            return null;
        }

        internal void AddItemToComboBox(System.Windows.Forms.ComboBox comboBox, object obj)
        {
            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (comboBox != null && obj != null && comboBox.InvokeRequired)
            {
                AddItemToComboBoxCallack addItemToComboBoxCallack =
                    delegate (System.Windows.Forms.ComboBox comboBx, object o)
                    {
                        if (comboBx != null && comboBx.Items != null && o != null)
                            comboBx.Items.Add(o);
                    };
                try
                {
                    comboBox.Invoke(addItemToComboBoxCallack, new object[] { comboBox, obj });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.LogOriginMsgEx(Name, $"Exception in delegate AddMenuItemToComboBox combo box: {comboBox.Name}, add object: \"{obj}\".\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null && comboBox.Items != null && obj != null)
                    comboBox.Items.Add(obj);
            }
        }



        #endregion ComboBox

        #endregion thread save WinForm delegate callbacks


        protected async Task BaseChatForm_Load(object sender, EventArgs e)
        {

            lock (_lock)
            {
                if (!loaded)
                {
                    loaded = true;
                    InterfaceIpAddresses = new List<IPAddress>();
                    ConnectedIpAddresses = new List<IPAddress>();
                }
            }

            bgWorkerMonitor = new BgWorkerMonitor();
            bgWorkerMonitor.Work_Monitor += new System.EventHandler(async (sender, e) => await BgWorkerMonitor_WorkMonitorAsync(sender, e));
            bgWorkerMonitor.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorkerMonitor_RunWorkerCompleted);
            bgWorkerMonitor.WorkerReportsProgress = true;
            bgWorkerMonitor.WorkerSupportsCancellation = true;                            
        }


        public virtual async Task BgWorkerMonitor_WorkMonitorAsync(object? sender, EventArgs e)
        { 

        }


        public virtual void BgWorkerMonitor_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Area23Log.LogOriginMsg("BaseChatForm", $"BgWorkerMonitor_RunWorkerCompleted(object sender = {sender}, RunWorkerCompletedEventArgs e = {e}) [Canceled]");
            }
            else if (e.Error != null)
            {
                
                string msg = (String.IsNullOrEmpty(e.Error.Message)) ? "[Error]" : "[Error: (msg = " + e.Error.Message + ")]";
                Area23Log.LogOriginMsg("BaseChatForm", $"BgWorkerMonitor_RunWorkerCompleted(object sender = {sender}, RunWorkerCompletedEventArgs e = {e}) [msg]");
            }
            else
            {
                Area23Log.LogOriginMsg("BaseChatForm", $"BgWorkerMonitor_RunWorkerCompleted(object sender = {sender}, RunWorkerCompletedEventArgs e = {e}) [Completed]");
            }
        }


        protected string? GetComboBoxMustHaveText(ref System.Windows.Forms.ComboBox comboBox)
        {
            string? cbName, cbValue;
            
            if (comboBox == null || ((cbName = GetComboBoxName(comboBox)) == null))
                return null;
            string cbText = GetComboBoxText(comboBox);
            if (string.IsNullOrEmpty(cbText) || 
                cbText.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase) ||
                cbText.Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase) ||
                cbText.Equals(Constants.ENTER_IP_CONTACT, StringComparison.InvariantCultureIgnoreCase) ||
                cbText.Equals(Constants.ENTER_CONTACT, StringComparison.InvariantCultureIgnoreCase) ||
                (cbName.ToLower() == "comboboxip" && !IPAddress.TryParse(cbText, out IPAddress? ipAddress)))
            {
                PlaySoundFromResource("sound_warning");

                switch (cbName.ToLower())
                {                    
                    case "comboboxsecretkey":
                        SetComboBoxBackColor(comboBox, Color.LightCyan);
                        InputDialog dialog = new InputDialog("secure key required", "enter secure symmetric key for en-/de-cryption", MessageBoxIcon.Warning);
                        dialog.ShowDialog();
                        string? appInputDialog0 = MemoryCache.CacheDict.GetValue<string>(Constants.APP_INPUT_DIALOG);
                        cbValue = (string.IsNullOrEmpty(appInputDialog0)) ? string.Empty : appInputDialog0;
                        if (!string.IsNullOrEmpty(cbValue))                       
                            SetComboBoxText(comboBox, cbValue);
                        
                        break;

                    case "comboboxip":
                        SetComboBoxBackColor(comboBox, Color.LightSkyBlue);
                        InputDialog dialogIp = new InputDialog("valid ip address required", "enter partner ip address for peer-2-peer chat", MessageBoxIcon.Warning);
                        dialogIp.ShowDialog();
                        string? appInputDialog1 = MemoryCache.CacheDict.GetValue<string>(Constants.APP_INPUT_DIALOG);
                        cbValue = (string.IsNullOrEmpty(appInputDialog1)) ? string.Empty : appInputDialog1;
                        if ((!string.IsNullOrEmpty(cbValue)) && (IPAddress.TryParse(cbValue, out IPAddress ipParsed)))
                            SetComboBoxText(comboBox, ipParsed.ToString());
                        
                        break;

                    case "comboboxcontacts":
                        SetComboBoxBackColor(comboBox, Color.LightGreen);
                        InputDialog dialogContact = new InputDialog("contact / email required", "enter contact or email address for server chat", MessageBoxIcon.Warning);
                        dialogContact.ShowDialog();
                        string? appInputDialog2 = MemoryCache.CacheDict.GetValue<string>(Constants.APP_INPUT_DIALOG);
                        cbValue = (string.IsNullOrEmpty(appInputDialog2)) ? string.Empty : appInputDialog2;
                        if (!string.IsNullOrEmpty(cbValue))
                        {
                            foreach (CContact c in Settings.Singleton.Contacts)
                            {
                                if (c.Name.Contains(cbValue) || c.NameEmail.Contains(cbValue, StringComparison.CurrentCultureIgnoreCase) || c.Email.Contains(cbValue, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    cbValue = c.NameEmail;
                                    SetComboBoxText(comboBox, cbValue.ToString());

                                    break;
                                }
                            }
                            if (cbValue.IsEmail())
                                SetComboBoxText(comboBox, cbValue.ToString());
                        }
                        
                        break;
                    
                    default: 
                        break;
                }

                if (((cbText = GetComboBoxText(comboBox)) != null) && (cbText.Length > 0))
                {
                    SetComboBoxBackColor(comboBox, Color.White);
                    return cbText;
                }
                
                FocusComboBox(comboBox);
                return null;
            }

            SetComboBoxBackColor(comboBox, Color.White);
            return cbText;
        }


        protected CFile? GetCFileFromPath(string filePath, string cryptPipe)
        {
            CFile? cfile = null;
            string md5 = Area23FwCore.Crypt.Hash.MD5Sum.Hash(filePath, true);
            string sha256 = Area23FwCore.Crypt.Hash.Sha256Sum.Hash(filePath, true);

            FileInfo fi = new FileInfo(filePath);
            if (fi.Length > Constants.MAX_FILE_BYTE_BUFFEER)
            {
                MessageBox.Show($"File size of {fi.Name} is {fi.Length} and exeeds {Constants.MAX_FILE_BYTE_BUFFEER} bytes.", "FileSize larger > 6MB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return cfile;
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileNameOnly = Path.GetFileName(filePath);
            string mimeType = Area23FwCore.Static.MimeType.GetMimeType(fileBytes, fileNameOnly);

            cfile = new CFile(fileNameOnly, mimeType, fileBytes, cryptPipe, md5, sha256, SerType.Json, EncodingType.Base64);
            return cfile;
        }

        public CFile? SendCFile(string filename, string secretKey, IPAddress partnerIpAddress)
        {

            CFile? cfile = null;

            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                string md5 = Area23FwCore.Crypt.Hash.MD5Sum.Hash(filename, true);
                string sha256 = Area23FwCore.Crypt.Hash.Sha256Sum.Hash(filename, true);

                byte[] fileBytes = File.ReadAllBytes(filename);
                string fileNameOnly = Path.GetFileName(filename);                

                string mimeType = MimeType.GetMimeType(fileBytes, fileNameOnly);
                
                CqrFacade facade = new CqrFacade(secretKey);
                cfile = new CFile(fileNameOnly, mimeType, fileBytes, facade.PipeString, md5, sha256);
                string response = facade.Send_CFile_Peer(cfile, partnerIpAddress, Constants.CHAT_PORT, SerType.Json, EncodingType.Base64);

                string base64FilePath = Path.Combine(LibPaths.AttachmentFilesDir, cfile.FileName + Constants.BASE64_EXT);
                File.WriteAllText(base64FilePath, cfile.ToBase64());
            }

            return cfile;

        }


        /// <summary>
        /// GetProxiesFromSettingsResources 
        /// </summary>
        /// <returns>list of ip addr of proxies</returns>
        public static List<IPAddress> GetProxiesFromSettingsResources()
        {
            lock (_sLock0)
            {
                if (_proxies != null && _proxies.Count > 0)
                    return _proxies;
            }

            lock (_sLock1)
            {
                _proxies = new List<IPAddress>();
                _sProxies = ProxiesInSettings ? Settings.Singleton.Proxies :
                    new List<string>(Resources.Proxies.Split(";,".ToCharArray()));
                foreach (string proxyS in _sProxies)
                {
                    try
                    {
                        if (IPAddress.TryParse(proxyS, out IPAddress? outAddr))
                            if (outAddr != null)
                                _proxies.Add(outAddr);
                    }
                    catch (Exception ex)
                    {
                        CqrException.SetLastException(ex);
                    }
                }
                List<IPAddress> cqrXsEuIpList;
                try
                {
                    cqrXsEuIpList = DnsHelper.GetIpAddrsByHostName(Constants.CQRXS_EU);
                }
                catch (Exception exDns)
                {
                    IPAddress? srvPIp, srvPIp6;
                    string srvIp4 = Properties.Resources.Proxies.Split(';')[0];
                    string srvIp6 = Properties.Resources.Proxies.Split(';')[1];
                    cqrXsEuIpList = new List<IPAddress>();
                    if (IPAddress.TryParse(srvIp4, out srvPIp))
                        cqrXsEuIpList.Add(srvPIp);
                    if (IPAddress.TryParse(srvIp6, out srvPIp6))
                        cqrXsEuIpList.Add(srvPIp6);

                    Area23Log.LogOriginMsgEx("BaseChatForm", "Exception on getting server ip address via dns", exDns);
                }

                foreach (IPAddress euIp in cqrXsEuIpList)
                {
                    try
                    {
                        if (!_proxies.Contains(euIp))
                            _proxies.Add(euIp);
                    }
                    catch (Exception ex)
                    {
                        CqrException.SetLastException(ex);
                    }
                }
                string[] proxyNameStrs = Resources.ProxyNames.Split(";,".ToCharArray());
                foreach (string proxyStr in proxyNameStrs)
                {
                    try
                    {
                        foreach (IPAddress netIp in DnsHelper.GetIpAddrsByHostName(proxyStr))
                            if (!_proxies.Contains(netIp))
                                _proxies.Add(netIp);

                    }
                    catch (Exception ex)
                    {
                        IPAddress? proxyAddr;
                        if (IPAddress.TryParse(proxyStr, out proxyAddr))
                            _proxies.Add(proxyAddr);

                        CqrException.SetLastException(ex);
                    }
                }

                _sProxies = _proxies.ConvertAll(x => x.ToString());
            }

            return _proxies;
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
                byte[] soundBytes = (byte[])Resources.ResourceManager.GetObject(soundName);

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
                        Area23Log.LogOriginMsgEx("BaseChatForm", $"PlaySoundFromResource(string soundName = {soundName})", exSound);     
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
            return await Task.Run(() => PlaySoundFromResource(soundName));
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
            MemoryCache.CacheDict.SetValue<int>(Constants.APP_TRANSPARENT_BADGE, 0);
            string infoText = $"{Dialog.AssemblyProduct} v{Dialog.AssemblyVersion}\n{Dialog.AssemblyCopyright} {Dialog.AssemblyCompany}";
            string titleText = $"{Dialog.AssemblyTitle} v{Dialog.AssemblyVersion}";

            bool? testApp = MemoryCache.CacheDict.GetValue<bool>(Constants.CQRXS_TEST_FORM);
            if (testApp.HasValue && testApp.Value)
            {
                TestForm testForm = new TestForm();
                testForm.Show();

                MessageBox.Show(infoText, $"{titleText}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            TransparentBadge badge = new TransparentBadge(titleText, infoText, MessageBoxIcon.Information, Properties.fr.Resources.CqrXsEuBadge);
            badge.ShowDialog();
            
        }

        #endregion Help About Info


        #region CloseForm Dispose AppExit

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            Entities.Settings.Singleton.Dispose(disposing);

            base.Dispose(disposing);
        }

        /// <summary>
        /// Closes Form, if this is the last form of application, then executes <see cref="AppCloseAllFormsExit"/>
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">FormClosingEventArgs e</param>
        protected internal virtual void FormClose_Click(object sender, FormClosingEventArgs e)
        {
            if (Application.OpenForms.Count < 2)
            {
                AppCloseAllFormsExit(sender, e);
                return;
            }
            try
            {
                Close();
            }
            catch (Exception exFormClose)
            {
                CqrException.SetLastException(exFormClose);
            }
            try
            {
                Dispose(true);
            }
            catch (Exception exFormDispose)
            {
                CqrException.SetLastException(exFormDispose);
            }

            return;

        }

        /// <summary>
        /// MenuFileItemExit_Click is fired, when selecting exit menu 
        /// and will nevertheless close all forms and exits application
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected virtual void MenuFileItemExit_Click(object sender, EventArgs e)
        {
            AppCloseAllFormsExit(sender, e);
        }


        protected virtual void DeleteAllAttachmentAndChatsBeforeExit(object sender, EventArgs e)
        {
            if (Settings.Singleton != null && Settings.Singleton.ClearAllOnClose)
            {                
                if (Directory.Exists(LibPaths.AttachmentFilesDir))
                {
                    string[] entries = Directory.GetFileSystemEntries(LibPaths.AttachmentFilesDir);
                    if (entries != null && entries.Length > 0)
                    {
                        DialogResult mResult =
                            MessageBox.Show($"There are {entries.Length} entries in {LibPaths.AttachmentFilesDir}.\n" +
                            "You want to clear them before exit?\n", "Clear attachment and chats?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                        if (mResult == DialogResult.Yes)
                        {
                            entries = Directory.GetDirectories(LibPaths.AttachmentFilesDir);
                            for (int d = 0; d < ((entries != null) ? entries.Length : 0); d++)
                                try
                                {
                                    Directory.Delete(entries[d], true);
                                }
                                catch { }
                            entries = Directory.GetFiles(LibPaths.AttachmentFilesDir);
                            for (int f = 0; f < ((entries != null) ? entries.Length : 0); f++)
                                try
                                {
                                    File.Delete(entries[f]);
                                }
                                catch { }
                        }
                    }
                }

                try
                {
                    CqrFacade closeFacade = new CqrFacade(CqrXsEuSrvKey);

                    string chatRoomNr = Settings.Singleton.ChatRoom.ChatRoomNr ?? "";
                    if (!string.IsNullOrEmpty(chatRoomNr))
                    {
                        var cList = new List<CContact>();
                        cList.Add(Settings.Singleton.MyContact);
                        CContact[] cContacts = cList.ToArray();
                        CSrvMsg<string> cSrvMsg = new CSrvMsg<string>(Settings.Singleton.MyContact, cContacts, "", CqrXsEuSrvKey, chatRoomNr);
                        CSrvMsg<string> cResonse = closeFacade.Send_CloseChatRoom_Soap<string>(cSrvMsg, chatRoomNr, EncodingType.Base64);                        
                    }
                }
                catch (Exception closeSesionRoomExc)
                {
                    CqrException.SetLastException(closeSesionRoomExc);
                }
            
            }
        
        }

        /// <summary>
        /// AppCloseAllFormsExit closes all open forms and exit and finally unlocks Mutex
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        internal virtual void AppCloseAllFormsExit(object sender, EventArgs e)
        {
            string settingsNotSavedReason = string.Empty;
            
            try
            {
                if (!Settings.SaveSettings())
                    settingsNotSavedReason = CqrException.LastException != null ?
                        CqrException.LastException.Message : "Unknown reason!";
            }
            catch (Exception exSetSave)
            {
                Area23Log.LogOriginMsgEx("BaseChatForm", $"AppCloseAllFormsExit(...)", exSetSave);
                settingsNotSavedReason = exSetSave.Message;
            }

            if (!string.IsNullOrEmpty(settingsNotSavedReason))
            {
                TransparentBadge badge = new TransparentBadge("Error saving settings!", $"Couldn't save chat settings to {Constants.JSON_SETTINGS_FILE}", MessageBoxIcon.Warning);
                badge.ShowDialog();
                MessageBox.Show(settingsNotSavedReason, "Couldn't save chat settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            DeleteAllAttachmentAndChatsBeforeExit(sender, e);
            // if (CqrException.LastException != null) // TODO: Remove this
            // MessageBox.Show(CqrException.LastException.ToString(), CqrException.LastException.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            int openForms = Application.OpenForms.Count;
            if (openForms > 1)
            {
                for (int frmidx = 0; frmidx < Application.OpenForms.Count; frmidx++)
                {
                    try
                    {
                        Form? form = Application.OpenForms[frmidx];
                        if (form != null && form.Name != Name)
                        {
                            form.Close();
                            form.Dispose();
                        }
                    }
                    catch (Exception exForm)
                    {
                        CqrException.SetLastException(exForm);
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
            }

            Application.ExitThread();
            Dispose();
            Application.Exit();
            Environment.Exit(0);

        }

        #endregion CloseForm Dispose AppExit

    }

}
