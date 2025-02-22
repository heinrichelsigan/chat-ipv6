using Area23FwCore = Area23.At.Framework.Core;
using Area23.At.Framework.Core;
using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.CqrXs.CqrSrv;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Net.WebHttp;
using Area23.At.Framework.Core.Net.NameService;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Properties;
using System.Media;
using System.Net;
using System.Windows.Forms;
using Area23.At.Framework.Core.Util;
using NLog.Targets.Wrappers;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using static System.Windows.Forms.MonthCalendar;
using Org.BouncyCastle.Utilities;
using System.Drawing.Imaging;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms.Base
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

        protected string savedFile = string.Empty;
        protected string loadDir = string.Empty;
        private System.ComponentModel.IContainer components = null;
        protected internal static DateTime LastExternalTime = DateTime.MinValue;
        protected internal static IPAddress? _externalIPAddress, _externalIPAddressV6;
        protected internal static List<string> _sProxies = new List<string>();
        protected internal static List<IPAddress> _proxies = new List<IPAddress>();
        protected internal static Lock _sLock = new Lock(), _sLock0 = new Lock(), _sLock1 = new Lock();
        protected internal Lock _lock = new Lock();

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


        public static IPAddress? ExternalIpAddress
        {
            get
            {
                if (_externalIPAddress != null && DateTime.Now.Subtract(LastExternalTime).TotalSeconds < 1800)
                {
                    return _externalIPAddress;
                }

                LastExternalTime = DateTime.Now;
                _externalIPAddress = WebClientRequest.ExternalClientIpFromServer("https://ipv4.cqrxs.eu/cqrsrv/cqrjd/R.aspx");
                return _externalIPAddress;
            }
        }


        public static IPAddress? ExternalIpAddressV6
        {
            get
            {
                if (_externalIPAddressV6 != null && DateTime.Now.Subtract(LastExternalTime).TotalSeconds < 1800)
                {
                    return _externalIPAddressV6;
                }

                try
                {
                    LastExternalTime = DateTime.Now;
                    _externalIPAddressV6 = WebClientRequest.ExternalClientIpFromServer("https://ipv6.cqrxs.eu/cqrsrv/cqrjd/R.aspx");
                }
                catch (Exception noIPv6Ex)
                {
                    Area23Log.LogStatic(noIPv6Ex);
                    _externalIPAddressV6 = null;
                }
                return _externalIPAddressV6;
            }
        }


        public static List<IPAddress> Proxies { get => GetProxiesFromSettingsResources(); }

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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate AppendText text: \"{text}\".\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate AppendRichText text: \"{text}\".\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx("RichTextFromPositionWithLengthAlign",
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SelectionAlignmentRichText: \"{leftRight}\".\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetFirstCharIndexFromLineRichText({lineNr}).\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetLastIndexOfSubstring(richTextBox = {richTextBox.Name}, pattern = {pattern}).\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate DeselectAllRichText(richTextBox = {richTextBox.Name}).\n", exDelegate);
                }
            }
            else
            {
                if (richTextBox != null && !string.IsNullOrEmpty(richTextBox.Text))
                    richTextBox.DeselectAll();
            }
            return;
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
                catch (Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate ClearRichText: \"{exDelegate.Message}\".\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetLinkLabelText text: \"{text}\".\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate AddLinkLabelLinks: \"{link}\".\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetLinkLabelVisible visible: \"{visible}\".\n", exDelegate);
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

        internal delegate string GetMenuItemTextCallback(ToolStripMenuItem menuItem);
        internal delegate void SetMenuItemTextCallback(ToolStripMenuItem menuItem, string text);

        internal string GetMenuItemText(ToolStripMenuItem mItem)
        {
            string reText = string.Empty;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItemText menuItem = {mItem.Name}.\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemText menu item: {mItem.Name}, text: \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (mItem != null && setText != null)
                    mItem.Text = setText;
            }
        }


        internal delegate Color GetMenuItemForeColorCallback(ToolStripMenuItem menuItem);
        internal delegate void SetMenuItemForeColorCallback(ToolStripMenuItem menuItem, Color foreColor);

        internal Color GetMenuItemForeColor(ToolStripMenuItem mItem)
        {
            Color foreColor = SystemColors.MenuText;

            // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItemForeColor menuItem = {mItem.Name}.\n", exDelegate);
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
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemText menu item: {mItem.Name}, Color: \"{foreColor}\".\n", exDelegate);
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
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItemBackColor menuItem = {mItem.Name}.\n", exDelegate);
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
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemBackColor menu item: {mItem.Name}, Color: \"{bgColor}\".\n", exDelegate);
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
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItemChecked menuItem = {mItem.Name}.\n", exDelegate);
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
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetMenuItemChecked menu item: {mItem.Name}, checked: \"{mchecked}\".\n", exDelegate);
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
            if (mItem.GetCurrentParent() != null && mItem.GetCurrentParent().InvokeRequired)
            {
                GetMenuItemsCallback getMenuItemsCallback = delegate (ToolStripMenuItem tsMenuItem)
                {
                    return tsMenuItem != null && tsMenuItem.DropDown != null && tsMenuItem.DropDownItems != null ?
                        tsMenuItem.DropDown.Items : new ToolStripItemCollection(tsMenuItem.GetCurrentParent(), new ToolStripMenuItem[0]);
                };
                try
                {
                    tscol = (ToolStripItemCollection)mItem.GetCurrentParent().Invoke(getMenuItemsCallback, new object[] { mItem });
                }
                catch (Exception exDelegate)
                {
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetMenuItems menuItem = {mItem.Name}.\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate AddMenuItemToItems menu item: {mItem.Name}, add item: \"{addItem.Name}\".\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetMenuDropDownItems menuItemComboBox = {tsCbx.Name}.\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate AddMenuItemToMenuComboBox menu combo box: {tsCombo.Name}, add object: \"{obj}\".\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetStatusText text: \"{text}\".\n", exDelegate);
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
        internal delegate System.Windows.Forms.ComboBox.ObjectCollection? GetComboBoxItemsCallback(System.Windows.Forms.ComboBox tsCombo);
        internal delegate void SetComboBoxTextCallback(System.Windows.Forms.ComboBox comboBox, string text);
        internal delegate void SetComboBackColorCallback(System.Windows.Forms.ComboBox comboBox, Color color);
        internal delegate void AddItemToComboBoxCallack(System.Windows.Forms.ComboBox comboBox, object o);

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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetComboBoxText ComboBox = {comboBox.Name}.\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetComboBoxText ComboBox = {comboBox.Name}, text = \"{text}\".\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null && setText != null)
                    comboBox.Text = setText;
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate SetComboBoxBackColor ComboBox = {comboBox.Name}, Colore = \"{color}\".\n", exDelegate);
                }
            }
            else
            {
                if (comboBox != null && color != null)
                    comboBox.BackColor = color;
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate GetComboBoxItems ComboBox = {comboBox.Name}.\n", exDelegate);
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
                    Area23Log.Logger.LogOriginMsgEx(Name, $"Exception in delegate AddMenuItemToComboBox combo box: {comboBox.Name}, add object: \"{obj}\".\n", exDelegate);
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


        public MimeAttachment? SendAttachment(string filename, string secretKey, IPAddress partnerIpAddress)
        {

            MimeAttachment? mimeAttach = null;

            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                string md5 = Area23FwCore.Crypt.Hash.MD5Sum.Hash(filename, true);
                string sha256 = Area23FwCore.Crypt.Hash.Sha256Sum.Hash(filename, true);

                byte[] fileBytes = File.ReadAllBytes(filename);
                string fileNameOnly = Path.GetFileName(filename);
                string mimeType = MimeType.GetMimeType(fileBytes, fileNameOnly);

                string base64Mime = Convert.ToBase64String(fileBytes, Base64FormattingOptions.InsertLineBreaks);

                Peer2PeerMsg pmsg = new Peer2PeerMsg(secretKey);


                // pmsg.SendCqrPeerMsg(mimeAttach.MimeMsg, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);
                pmsg.Send_CqrPeerAttachment(fileNameOnly, mimeType, base64Mime, partnerIpAddress, out mimeAttach, Constants.CHAT_PORT, md5, sha256, MsgEnum.None, EncodingType.Base64);

                string base64FilePath = Path.Combine(LibPaths.AttachmentFilesDir, mimeAttach.FileName + Constants.BASE64_EXT);
                File.WriteAllText(base64FilePath, mimeAttach.MimeMsg);
            }

            return mimeAttach;

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
                        Area23Log.LogStatic(ex);
                    }
                }
                List<IPAddress> cqrXsEuIpList = DnsHelper.GetIpAddrsByHostName(Constants.CQRXS_EU);
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
                        Area23Log.LogStatic(ex);
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
                        CqrException.SetLastException(ex);
                        Area23Log.LogStatic(ex);
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
            MessageBox.Show($"{Text} type {GetType()} Information MessageBox.", $"{Text} type {GetType()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                AppCloseAllFormsExit();
                return;
            }
            try
            {
                Close();
            }
            catch (Exception exFormClose)
            {
                CqrException.SetLastException(exFormClose);
                Area23Log.LogStatic(exFormClose);
            }
            try
            {
                Dispose(true);
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
        protected virtual void MenuFileItemExit_Click(object sender, EventArgs e)
        {
            AppCloseAllFormsExit();
        }

        /// <summary>
        /// AppCloseAllFormsExit closes all open forms and exit and finally unlocks Mutex
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        internal virtual void AppCloseAllFormsExit()
        {
            string settingsNotSavedReason = string.Empty;
            try
            {
                if (!Settings.SaveSettings(null))
                    settingsNotSavedReason = CqrException.LastException != null ?
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
                        if (form != null && form.Name != Name)
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
