using Area23FwCore = Area23.At.Framework.Core;
using Area23.At.Framework.Core;
using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.CqrXs.CqrSrv;
using Area23.At.Framework.Core.Net;
using Area23.At.Framework.Core.Crypt.Cipher;
using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Net.NameService;
using Area23.At.Framework.Core.Net.IpSocket;
using Area23.At.Framework.Core.Net.WebHttp;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Gui.Controls;
using EU.CqrXs.WinForm.SecureChat.Gui.Forms;
using EU.CqrXs.WinForm.SecureChat.Gui.Forms.Base;
using EU.CqrXs.WinForm.SecureChat.Properties;
using EU.CqrXs.WinForm.SecureChat.Util;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;



namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms
{


    /// <summary>
    /// Peer2PeerChat main form
    /// </summary>
    public partial class Peer2PeerChat : BaseMenuForm
    {

        #region fields        

        private string myServerKey = string.Empty;
        internal static int attachCnt = 0;
        internal static int chatCnt = 0;
        internal static Chat? chat;

        private static IPAddress? clientIpAddress;
        private static IPAddress? partnerIpAddress;
        private static Listener? ipSockListener;

        #endregion fields

        #region Properties

        private static IPAddress? _serverIpAddress;
        internal IPAddress? ServerIpAddress
        {
            get
            {
                if (_serverIpAddress != null && !_serverIpAddress.IsIPv6UniqueLocal)
                    return _serverIpAddress;

                // TODO: change it
                List<IPAddress> list = DnsHelper.GetIpAddrsByHostName(Constants.CQRXS_EU);
                foreach (IPAddress ip in list)
                {
                    if (Proxies.Contains(ip))
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetworkV6 && MenuNetworkItemIPv6Secure.Checked)
                        {
                            _serverIpAddress = ip;
                            return _serverIpAddress;
                        }
                        if (ip.AddressFamily == AddressFamily.InterNetwork && !MenuNetworkItemIPv6Secure.Checked)
                        {
                            _serverIpAddress = ip;
                            return _serverIpAddress;
                        }
                    }
                }
                foreach (IPAddress ip in list)
                {
                    foreach (IPAddress proxyIp in Proxies)
                    {
                        if (ip.IsSameIp(proxyIp, AddressFamily.InterNetwork))
                        {
                            _serverIpAddress = ip;
                            return _serverIpAddress;
                        }
                    }
                }

                return null;
            }
        }

        #endregion Properties

        /// <summary>
        /// Ctor
        /// </summary>
        public Peer2PeerChat() : base()
        {
            InitializeComponent();
            TextBoxSource.MaxLength = Constants.SOCKET_BYTE_BUFFEER;
            TextBoxDestionation.MaxLength = Constants.SOCKET_BYTE_BUFFEER;
            ComboBoxIp.Text = Constants.ENTER_IP;
            ComboBoxContacts.Text = Constants.ENTER_CONTACT;
            ComboBoxSecretKey.Text = Constants.ENTER_SECRET_KEY;
            Load += new System.EventHandler(async (sender, e) => await Peer2PeerChat_Load(sender, e));
            attachmentListControl.OnDragNDrop += OnDragNDrop;
            dragnDropGroupBox.OnDragNDrop += OnDragNDrop;
            this.peerServerSwitchControl1.FireUpChanged += TooglePeerServer;
            this.StripProgressBar.Value = 0;
        }


        protected internal virtual async Task Peer2PeerChat_Load(object sender, EventArgs e)
        {
            send1stReg = false;
            this.StripProgressBar.Value = 10;
            try
            {
                if (!Directory.Exists(LibPaths.AttachmentFilesDir))
                    Directory.CreateDirectory(LibPaths.AttachmentFilesDir);
            }
            catch (Exception exBase64)
            {
                Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in MenuItemAttach_Click: {exBase64.Message}.\n", exBase64);
                StripStatusLabel.Text = "Attach FAILED: " + exBase64.Message;
            }

            this.StripProgressBar.Value = 20;
            await base.BaseMenuForm_Load(sender, e);

            this.StripProgressBar.Value = 30;
            await PlaySoundFromResourcesAsync("sound_train");

            this.StripProgressBar.Value = 40;
            StripStatusLabel.Text = "Setup Network";

            await SetupNetwork();
            this.StripProgressBar.Value = 50;

            if (Entities.Settings.Singleton != null && Entities.Settings.Singleton.MyContact != null && Entities.Settings.Singleton.MyContact.ContactImage != null &&
                !string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.ContactImage.ImageBase64))
            {
                Bitmap? bmp = (Bitmap?)Entities.Settings.Singleton.MyContact.ContactImage.ToDrawingBitmap();
                if (bmp != null)
                    this.PictureBoxYou.Image = bmp;
            }

            AddContactsToIpContact();
            this.StripProgressBar.Value = 70;

            if (send1stReg)
                Send_1st_Server_Registration(sender, e);

            this.StripProgressBar.Value = 100;
            StripStatusLabel.Text = "Secure Chat init done.";

        }

        #region thread save text and richtext box access       

        /// <summary>
        /// Displays and formats lines in <see cref="RichTextBoxOneView" />
        /// </summary>
        internal void Format_Lines_RichTextBox()
        {
            if (chat != null)
            {
                ClearRichText(RichTextBoxOneView);
                int lineIndex = 0;
                foreach (var tuple in chat.CqrMsgs)
                {

                    if (tuple.Key > chat.TimeStamp)
                    {
                        string patternDate = tuple.Key.ToString("[yy-MM-dd HH:mm:ss]");
                        string line = patternDate + " " + tuple.Value;
                        if (!line.EndsWith("\r\n") && !line.EndsWith("\n") && !line.EndsWith("\n\0") && !line.EndsWith(Environment.NewLine))
                            line += "\n";

                        AppendRichText(RichTextBoxOneView, line);
                        // RichTextBoxOneView.AppendText(line + Environment.NewLine);

                        int lastIdx = GetLastIndexOfSubstring(RichTextBoxOneView, patternDate);
                        // int startPos = GetFirstCharIndexFromLineRichText(RichTextBoxOneView, lineIndex++);

                        HorizontalAlignment hAlign = HorizontalAlignment.Center;

                        if (chat.MyMsgTStamps.Contains(tuple.Key))
                            hAlign = HorizontalAlignment.Left;
                        else if (chat.FriendMsgTStamps.Contains(tuple.Key))
                            hAlign = HorizontalAlignment.Right;
                        // RichTextFromPositionWithLengthAlign(RichTextBoxOneView, startPos, line.Length, HorizontalAlignment.Right);
                        // SelectionAlignmentRichText(RichTextBoxOneView, HorizontalAlignment.Left);

                        RichTextFromPositionWithLengthAlign(RichTextBoxOneView, lastIdx, line.Length, hAlign);
                        //DeselectAllRichText(RichTextBoxOneView);
                    }
                }

            }
        }

        #endregion thread save text and richtext box access

        #region SecretKey & SymmCipherPipe.PipeString + ComboBoxSecretKey FocusLeave TextUpdate SelectedIndexChanged

        /// <summary>
        /// ButtonKey_Click Event
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void ButtonKey_Click(object sender, EventArgs e)
        {
            myServerKey = ExternalIpAddress?.ToString() + Constants.APP_NAME;
            if (!string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) &&
                !this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                myServerKey = this.ComboBoxSecretKey.Text;
            }

            // TODO: test case later

            SrvMsg serverMessage = new SrvMsg(myServerKey, myServerKey);
            this.TextBoxPipe.Text = serverMessage.PipeString;
        }


        /// <summary>
        /// ComboBoxSecretKey_FocusLeave event is fired, when we leave focus of ComboBoxSecretKe
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void ComboBoxSecretKey_FocusLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) ||
                this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a secret key!", "Please enter a secret key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxSecretKey.BackColor = Color.OrangeRed;
                PlaySoundFromResource("sound_warning");
                return;
            }
            this.ComboBoxSecretKey.BackColor = Color.White;
            ButtonKey_Click(sender, e);
            if (Entities.Settings.Singleton != null)
            {
                if (!Entities.Settings.Singleton.SecretKeys.Contains(this.ComboBoxSecretKey.Text))
                    Entities.Settings.Singleton.SecretKeys.Add(this.ComboBoxSecretKey.Text);
                if (!this.ComboBoxSecretKey.Items.Contains(this.ComboBoxSecretKey.Text))
                    this.ComboBoxSecretKey.Items.Add(this.ComboBoxSecretKey.Text);
            }
            StripStatusLabel.Text = "Added new secret key => calculated new SecurePipe...";
        }

        /// <summary>
        /// ComboBoxSecretKey_TextUpdate is fired, when text entered in ComboBoxSecretKey changes.
        /// Event is fired, when 1 char will be added or deleted at each change of <see cref="ComboBoxSecretKey.Text"/>
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void ComboBoxSecretKey_TextUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) ||
                this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            this.ComboBoxSecretKey.BackColor = Color.White;
            ButtonKey_Click(sender, e);
        }

        /// <summary>
        /// ComboBoxSecretKey_SelectedIndexChanged is fired, when we select a previous secret key in ComboBoxSecretKey
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void ComboBoxSecretKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) ||
                this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a secret key!", "Please enter a secret key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxSecretKey.BackColor = Color.OrangeRed;
                PlaySoundFromResource("sound_warning");
                return;
            }
            this.ComboBoxSecretKey.BackColor = Color.White;
            ButtonKey_Click(sender, e);
            if (Entities.Settings.Singleton != null)
            {
                if (!Entities.Settings.Singleton.SecretKeys.Contains(this.ComboBoxSecretKey.Text))
                    Entities.Settings.Singleton.SecretKeys.Add(this.ComboBoxSecretKey.Text);
                if (!this.ComboBoxSecretKey.Items.Contains(this.ComboBoxSecretKey.Text))
                    this.ComboBoxSecretKey.Items.Add(this.ComboBoxSecretKey.Text);
            }
            StripStatusLabel.Text = "Added new secret key => calculated new SecurePipe...";
        }

        #endregion SecretKey & SymmCipherPipe.PipeString + ComboBoxSecretKey FocusLeave TextUpdate SelectedIndexChanged

        #region ComboBoxIp FocusLeave TextUpdate SelectedIndexChanged

        private void ComboBoxIp_FocusLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxIp.Text) ||
                this.ComboBoxIp.Text.Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a new ip address!", "Please enter a valid connectable ip address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxIp.BackColor = Color.PeachPuff;
                PlaySoundFromResource("sound_warning");
                return;
            }

            try
            {
                partnerIpAddress = IPAddress.Parse(this.ComboBoxIp.Text);
            }
            catch (Exception exIpContact)
            {
                MessageBox.Show($"Cannot parse IpAddress from string \"{ComboBoxIp.Text}\": {exIpContact.Message}", "Please enter a valid connectable ipv4 or ipv6 address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxIp.BackColor = Color.Violet;
                PlaySoundFromResource("sound_warning");
                return;
            }

            SetComboBoxBackColor(ComboBoxIp, Color.White);
            // this.ComboBoxIp.BackColor = Color.White;

            if (Entities.Settings.Singleton != null && SendInit_Click())
            {
                PlaySoundFromResource("sound_laser");

                if (!Entities.Settings.Singleton.FriendIPs.Contains(this.ComboBoxIp.Text))
                    Entities.Settings.Singleton.FriendIPs.Add(this.ComboBoxIp.Text);
                if (!this.MenuNetworkComboBoxFriendIp.Items.Contains(partnerIpAddress.ToString()))
                    this.MenuNetworkComboBoxFriendIp.Items.Add(partnerIpAddress.ToString());

                try
                {
                    this.MenuNetworkComboBoxFriendIp.Text = partnerIpAddress.ToString();
                }
                catch { }

                if (!this.ComboBoxIp.Items.Contains(this.ComboBoxIp.Text))
                    this.ComboBoxIp.Items.Add(partnerIpAddress.ToString());

                Entities.Settings.SaveSettings(Entities.Settings.Singleton);
            }
            else
            {
                ButtonCheck.Image = Properties.de.Resources.CableWireCut;
                PlaySoundFromResource("sound_warning");
            }

            SetStatusText(StripStatusLabel, $"Added new partner ip address {partnerIpAddress.ToString()}.");

        }


        private void ComboBoxIp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxIp.Text) ||
                this.ComboBoxIp.Text.Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a new ip address!", "Please enter a valid connectable ip address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxIp.BackColor = Color.PeachPuff;
                PlaySoundFromResource("sound_warning");
                return;
            }
            try
            {
                partnerIpAddress = IPAddress.Parse(this.ComboBoxIp.Text);
            }
            catch (Exception exIpContact)
            {
                MessageBox.Show($"Cannot parse IpAddress from string \"{ComboBoxIp.Text}\": {exIpContact.Message}", "Please enter a valid connectable ipv4 or ipv6 address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxIp.BackColor = Color.Violet;
                PlaySoundFromResource("sound_warning");
                return;
            }
            this.ComboBoxIp.BackColor = Color.White;
            StripStatusLabel.Text = $"Selected partner ip address {partnerIpAddress.ToString()}.";
            this.ComboBoxContacts.Text = Constants.ENTER_CONTACT;

            if (SendInit_Click())
            {
                PlaySoundFromResource("sound_laser");
            }
            else
            {
                ButtonCheck.Image = Properties.de.Resources.CableWireCut;
                PlaySoundFromResource("sound_warning");
            }
        }

        #endregion ComboBoxIp FocusLeave TextUpdate SelectedIndexChanged

        #region ComboBoxContacts FocusLeave SelectedIndexChanged


        private void ComboBoxContacts_FocusLeave(object sender, EventArgs e)
        {

        }

        private void ComboBoxContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxContacts.Text) ||
                this.ComboBoxContacts.Text.Equals(Constants.ENTER_CONTACT, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a valid contact address!", "Please enter a valid contact", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxContacts.BackColor = Color.PeachPuff;
                PlaySoundFromResource("sound_warning");
                return;
            }

            bool foundContact = false;
            CqrContact? friendContact = null;
            string exContactMsg = "";
            try
            {

                foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
                {
                    if (c.NameEmail.Equals(this.ComboBoxContacts.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundContact = true;
                        friendContact = c;
                        break;
                    }
                }
            }
            catch (Exception exContact)
            {
                exContactMsg = exContact.Message;
                foundContact = false;
            }

            if (!foundContact)
            {
                this.ComboBoxContacts.BackColor = Color.Violet;
                PlaySoundFromResource("sound_warning");
                MessageBox.Show($"Cannot parse Contact from string \"{ComboBoxContacts.Text}\": {exContactMsg}", "Please enter a valid contact address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetComboBoxText(ComboBoxIp, Constants.ENTER_IP);

            this.ComboBoxContacts.BackColor = Color.White;
            StripStatusLabel.Text = $"Selected Contact {this.ComboBoxContacts.Text}.";

            if (SendInit_Contact())
            {
                PlaySoundFromResource("sound_laser");
            }
            else
            {
                ButtonCheck.Image = Properties.de.Resources.CableWireCut;
                PlaySoundFromResource("sound_warning");
            }
        }


        #endregion ComboBoxContacts FocusLeave SelectedIndexChanged

        #region OnClientReceive MenuSend MenuAttach MenuRefresh MenuClear

        /// <summary>
        /// Send_1st_Server_Registration sends contact registration to cqrxs.eu server
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void Send_1st_Server_Registration(object sender, EventArgs e)
        {
            if (chat == null)
                chat = new Chat(0);

            myServerKey = ExternalIpAddress?.ToString() + Constants.APP_NAME;
            if (!string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) &&
                !this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                myServerKey = this.ComboBoxSecretKey.Text;
            }
            else
                this.ComboBoxSecretKey.Text = myServerKey;

            SrvMsg1 srv1stMsg = new SrvMsg1(myServerKey);
            this.TextBoxPipe.Text = srv1stMsg.PipeString;
            Thread.Sleep(100);

            this.StripProgressBar.Value = 50;

            CqrContact myContact = Entities.Settings.Singleton.MyContact;
            string ser = (string)AppDomain.CurrentDomain.GetData(Constants.MY_CONTACT);
            string encrypted = srv1stMsg.CqrSrvMsg1(myContact, EncodingType.Base64);
            Thread.Sleep(100);

            this.StripProgressBar.Value = 60;
            string response = srv1stMsg.Send1st_CqrSrvMsg1(myContact, ServerIpAddress, EncodingType.Base64);

            this.TextBoxSource.Text = "\n"; //  + "\r\n" + serverMessage.symmPipe.HexStages;
            if (srv1stMsg != null)
            {
                CqrContact? receivedMyContact = srv1stMsg.NCqrSrvMsg1(encrypted, EncodingType.Base64);
                if (receivedMyContact != null)
                    this.TextBoxSource.Text = receivedMyContact.ToJson() + "\n";
            }

            string reducedResponse = string.Empty;
            if (response.Contains(Constants.DECRYPTED_TEXT_AREA))
                reducedResponse = response.GetSubStringByPattern(Constants.DECRYPTED_TEXT_AREA, true, "",
                    Constants.DECRYPTED_TEXT_AREA_END, false, StringComparison.InvariantCulture);
            else if (response.Contains(Constants.DECRYPTED_TEXT_BOX))
                reducedResponse = response.GetSubStringByPattern(Constants.DECRYPTED_TEXT_BOX, true, ">",
                    Constants.DECRYPTED_TEXT_AREA_END, false, StringComparison.InvariantCulture);

            this.TextBoxDestionation.Text += reducedResponse + "\r\n"; // + serverMessage.symmPipe.HexStages;

            chat.AddMyMessage(myContact.ToJson());
            chat.AddFriendMessage(reducedResponse);

            // this.RichTextBoxOneView.Rtf = this.RichTextBoxChat.Rtf;
            Format_Lines_RichTextBox();

            StripStatusLabel.Text = "Finished 1st registration";
        }

        /// <summary>
        /// OnClientReceive event is fired, 
        /// when another secure chat client connects directly peer 2 peer 
        /// to server socket of our local chat app,
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        internal void OnClientReceive(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(myServerKey))
            {
                myServerKey = ExternalIpAddress?.ToString() + Constants.APP_NAME;

                string comboBoxSecKeyText = this.GetComboBoxText(ComboBoxSecretKey);
                if (!string.IsNullOrEmpty(comboBoxSecKeyText) &&
                    !comboBoxSecKeyText.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
                {
                    myServerKey = this.GetComboBoxText(ComboBoxSecretKey);
                }
            }

            if (sender != null)
            {
                if (ipSockListener?.BufferedData != null && ipSockListener.BufferedData.Length > 0)
                {
                    if (chat == null)
                        chat = new Chat(0);
                    string encrypted = EnDeCoder.GetString(ipSockListener.BufferedData);

                    Area23EventArgs<ReceiveData>? area23EvArgs = null;
                    if (e != null && e is Area23EventArgs<ReceiveData>)
                    {
                        area23EvArgs = ((Area23EventArgs<ReceiveData>)e);
                        //TODO: Enable cross thread via delegate
                        SetStatusText(StripStatusLabel, "Connection from " + area23EvArgs.GenericTData.ClientIPAddr + ":" + area23EvArgs.GenericTData.ClientIPPort);

                        string comboText = GetComboBoxText(ComboBoxIp);
                        if (!comboText.Equals(area23EvArgs.GenericTData.ClientIPAddr, StringComparison.CurrentCulture))
                        {
                            PlaySoundFromResource("sound_breakpoint");
                            SetComboBoxText(ComboBoxIp, area23EvArgs.GenericTData.ClientIPAddr);

                        }
                        encrypted = EnDeCoder.GetString(area23EvArgs.GenericTData.BufferedData);
                    }


                    Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                    MsgContent msgContent;
                    try
                    {
                        msgContent = pmsg.NCqrPeerMsg(encrypted);
                    }
                    catch (Exception exCrypt)
                    {
                        PlaySoundFromResource("sound_hammer");
                        if (exCrypt is InvalidOperationException)
                        {
                            MessageBox.Show(((InvalidOperationException)exCrypt).Message, "Invalid or non matching secret key for decrypt.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            SetComboBoxBackColor(ComboBoxSecretKey, Color.OrangeRed);
                        }
                        else
                        {
                            MessageBox.Show(exCrypt.Message, $"Error/Exception, when decrypting incoming message from {GetComboBoxText(ComboBoxIp)}.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }
                    string friendMsg = string.Empty;
                    if (msgContent.IsMimeAttachment())
                    {
                        MimeAttachment mimeAttachment = msgContent.ToMimeAttachment();
                        SetAttachmentTextLink(mimeAttachment);
                        friendMsg = mimeAttachment.GetFileNameContentLength() + Environment.NewLine;
                        PlaySoundFromResource("sound_wind");
                    }
                    else
                    {
                        friendMsg = msgContent.Message + Environment.NewLine;
                        PlaySoundFromResource("sound_push");
                    }

                    string appendDestMsg = chat.AddFriendMessage(friendMsg);
                    AppendText(TextBoxDestionation, appendDestMsg);
                    // AppendText(TextBoxDestionation, friendMsg);
                    // this.RichTextBoxOneView.Text = unencrypted;
                    Format_Lines_RichTextBox();
                }
            }
        }

        /// <summary>
        /// Sends a init secure message to peer ip address
        /// </summary>
        private bool SendInit_Click()
        {
            // TODO: implement it via socket directly or to registered user
            // if Ip is pingable and reachable and connectable
            // send HELLO to IP
            if (chat == null)
                chat = new Chat(0);

            if (string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) ||
                this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a secret key!", "Please enter a secret key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxSecretKey.BackColor = Color.OrangeRed;
                PlaySoundFromResource("sound_warning");
                return false;
            }

            myServerKey = this.ComboBoxSecretKey.Text;
            string unencrypted = "Init: " + clientIpAddress?.ToString() + " " + Entities.Settings.Singleton.MyContact.NameEmail;

            try
            {
                string comboIpText = GetComboBoxText(ComboBoxIp);
                partnerIpAddress = IPAddress.Parse(comboIpText);
                Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                pmsg.Send_CqrPeerMsg(unencrypted, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);

                // chat.AddMyMessage(unencrypted);
                // AppendText(TextBoxSource, unencrypted);
                // Format_Lines_RichTextBox();
                this.RichTextBoxChat.Text = string.Empty;
                SetStatusText(StripStatusLabel, "Send init successfully");
                // StripStatusLabel.Text = "Send init successfully";
                ButtonCheck.Image = Properties.de.Resources.RemoteConnect;
            }
            catch (Exception ex)
            {
                Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in SendInit_Click: {ex.Message}.\n", ex);
                SetStatusText(StripStatusLabel, "Send init FAILED: " + ex.Message);
                // StripStatusLabel.Text = "Send init FAILED: " + ex.Message;
                PlaySoundFromResource("sound_hammer");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sends a init secure message to contact over server proxy
        /// </summary>
        private bool SendInit_Contact()
        {
            // TODO: implement it via socket directly or to registered user
            // if Ip is pingable and reachable and connectable
            // send HELLO to IP
            if (chat == null)
                chat = new Chat(0);

            if (string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) ||
                this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a secret key!", "Please enter a secret key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxSecretKey.BackColor = Color.OrangeRed;
                PlaySoundFromResource("sound_warning");
                return false;
            }

            myServerKey = this.ComboBoxSecretKey.Text;
            if (string.IsNullOrEmpty(this.ComboBoxContacts.Text) || this.ComboBoxContacts.Text.Equals(Constants.ENTER_CONTACT, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't choosen a valid contact", "Please select a valid contact", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxContacts.BackColor = Color.OrangeRed;
                PlaySoundFromResource("sound_warning");
                return false;
            }
            ComboBoxContacts.BackColor = Color.White;


            string unencrypted = "Init: " + clientIpAddress?.ToString() + " " + Entities.Settings.Singleton.MyContact.NameEmail;

            CqrContact myContact = Entities.Settings.Singleton.MyContact;
            CqrContact? friendContact = null;
            foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
            {
                if (c.NameEmail.Equals(this.ComboBoxContacts.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    friendContact = c;
                    break;
                }
            }


            SrvMsg serverMessage = new SrvMsg(myContact, friendContact, myServerKey, myServerKey);
            this.TextBoxPipe.Text = serverMessage.PipeString;


            FullSrvMsg<CqrContact> fmsg = new FullSrvMsg<CqrContact>(myContact, friendContact, myContact, serverMessage.PipeString);

            string encrypted = serverMessage.CqrSrvMsg<CqrContact>(fmsg, MsgKind.Server, EncodingType.Base64);
            string response = serverMessage.Send_CqrSrvMsgT<CqrContact>(fmsg, ServerIpAddress, EncodingType.Base64);

            this.TextBoxSource.Text = fmsg.Message + "\n"; //  + "\r\n" + serverMessage.symmPipe.HexStages;
            FullSrvMsg<CqrContact> rfmsg = serverMessage.NCqrSrvMsg<CqrContact>(encrypted, EncodingType.Base64);
            this.TextBoxDestionation.Text = rfmsg.Message + "\n" + response + "\r\n"; // + serverMessage.symmPipe.HexStages;

            chat.AddMyMessage(fmsg.Message);
            chat.AddFriendMessage(rfmsg.Message);

            // this.RichTextBoxOneView.Rtf = this.RichTextBoxChat.Rtf;
            Format_Lines_RichTextBox();

            StripStatusLabel.Text = "Finished 1st registration";

            return true;

        }



        /// <summary>
        /// Sends a secure message
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected internal override void MenuItemSend_Click(object sender, EventArgs e)
        {
            // TODO: implement it via socket directly or to registered user
            // if Ip is pingable and reachable and connectable
            // send HELLO to IP
            if (chat == null)
                chat = new Chat(0);

            if (string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) ||
                this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a secret key!", "Please enter a secret key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxSecretKey.BackColor = Color.OrangeRed;
                PlaySoundFromResource("sound_warning");
                return;
            }

            if (string.IsNullOrEmpty(this.RichTextBoxChat.Text) || string.IsNullOrWhiteSpace(this.RichTextBoxChat.Text))
            {
                StripStatusLabel.Text = "Nothing to send!";
                PlaySoundFromResource("sound_warning");
                return;
            }

            myServerKey = this.ComboBoxSecretKey.Text;
            string unencrypted = this.RichTextBoxChat.Text; //.Replace("\r\n", "\n").Replace("\n", " " + Environment.NewLine);

            if (!string.IsNullOrEmpty(this.ComboBoxIp.Text) && !this.ComboBoxIp.Text.Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    partnerIpAddress = IPAddress.Parse(this.ComboBoxIp.Text);
                    Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                    pmsg.Send_CqrPeerMsg(unencrypted, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);

                    string userMsg = chat.AddMyMessage(unencrypted);
                    AppendText(TextBoxSource, userMsg);
                    Format_Lines_RichTextBox();
                    this.RichTextBoxChat.Text = string.Empty;
                    StripStatusLabel.Text = "Send successfully";
                    PlaySoundFromResource("sound_arrow");
                }
                catch (Exception ex)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in MenuCommandsItemSend_Click: {ex.Message}.\n", ex);
                    StripStatusLabel.Text = "Send FAILED: " + ex.Message;
                    PlaySoundFromResource("sound_warning");
                }
            }
            // otherwise send message to registered user via server
            // Always encrypt via key
        }


        /// <summary>
        /// Attaches a file to send
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected internal override void MenuItemAttach_Click(object sender, EventArgs e)
        {
            if (chat == null)
                chat = new Chat(0);

            if (string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) ||
                this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a secret key!", "Please enter a secret key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxSecretKey.BackColor = Color.OrangeRed;
                PlaySoundFromResource("sound_warning");
                return;
            }

            myServerKey = this.ComboBoxSecretKey.Text;

            FileOpenDialog = FileOpenDialog ?? new OpenFileDialog();
            FileOpenDialog.RestoreDirectory = true;
            FileOpenDialog.AddExtension = false;
            FileOpenDialog.CheckFileExists = true;
            FileOpenDialog.CheckPathExists = true;
            FileOpenDialog.Filter = "All files (*.*)|*.*|BMP (*.bmp)|*.bmp|PNG (*.png)|*.png|GIF (*.gif)|*.gif|JPG (*.jpg)|*.jpg|PDF (*.pdf)|*.pdf";
            DialogResult result = FileOpenDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                if (File.Exists(FileOpenDialog.FileName))
                {
                    string md5 = Area23FwCore.Crypt.Hash.MD5Sum.Hash(FileOpenDialog.FileName, true);
                    string sha256 = Area23FwCore.Crypt.Hash.Sha256Sum.Hash(FileOpenDialog.FileName, true);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(FileOpenDialog.FileName);
                    string fileNameOnly = Path.GetFileName(FileOpenDialog.FileName);
                    string mimeType = Area23FwCore.Util.MimeType.GetMimeType(fileBytes, fileNameOnly);

                    string base64Mime = Base64.Encode(fileBytes);

                    Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);

                    MimeAttachment mimeAttach; // = new MimeAttachment(fileNameOnly, mimeType, base64Mime, pmsg.symmPipe.PipeString, md5, sha256);
                    if (!string.IsNullOrEmpty(this.ComboBoxIp.Text) && !this.ComboBoxIp.Text.Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase))
                    {

                        try
                        {
                            partnerIpAddress = IPAddress.Parse(this.ComboBoxIp.Text);

                            // pmsg.SendCqrPeerMsg(mimeAttach.MimeMsg, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);
                            pmsg.Send_CqrPeerAttachment(fileNameOnly, mimeType, base64Mime, partnerIpAddress, out mimeAttach, Constants.CHAT_PORT, md5, sha256, MsgEnum.None, EncodingType.Base64);

                            string base64FilePath = Path.Combine(LibPaths.AttachmentFilesDir, mimeAttach.FileName + Constants.BASE64_EXT);
                            System.IO.File.WriteAllText(base64FilePath, mimeAttach.MimeMsg);

                            string userMsg = chat.AddMyMessage(mimeAttach.GetFileNameContentLength());
                            AppendText(TextBoxSource, userMsg);
                            Format_Lines_RichTextBox();
                            this.RichTextBoxChat.Text = string.Empty;
                            StripStatusLabel.Text = $"File {fileNameOnly} send successfully!";
                        }
                        catch (Exception ex)
                        {
                            Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in MenuItemAttach_Click: {ex.Message}.\n", ex);
                            StripStatusLabel.Text = "Attach FAILED: " + ex.Message;
                            PlaySoundFromResource("sound_warning");
                        }
                    }
                    // otherwise send message to registered user via server
                    // Always encrypt via key
                }

            }

        }

        public void TooglePeerServer(object sender, EventArgs e)
        {
            if (e is Area23EventArgs<int> ev)
            {
                if (ev.GenericTData < 1)
                {

                    SetComboBoxText(ComboBoxContacts, Constants.ENTER_CONTACT);
                    try
                    {
                        this.ComboBoxContacts.Enabled = false;
                        this.ComboBoxIp.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                    }

                }
                else if (ev.GenericTData > 1)
                {
                    SetComboBoxText(ComboBoxIp, Constants.ENTER_IP);
                    try
                    {
                        this.ComboBoxContacts.Enabled = true;
                        this.ComboBoxIp.Enabled = false;
                    }
                    catch (Exception ex)
                    {
                    }

                }
                else
                {
                    try
                    {
                        this.ComboBoxContacts.Enabled = true;
                        this.ComboBoxIp.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }


        public void OnDragNDrop(object sender, EventArgs e)
        {
            if (e is Area23EventArgs<string> ea)
            {
                string t = GetComboBoxText(this.ComboBoxIp);
                if (!string.IsNullOrEmpty(t) && IPAddress.TryParse(t, out IPAddress pi))
                {
                    var s = SendAttachment(ea.GenericTData, myServerKey, pi);
                }
            }
        }

        protected internal override void MenuItemRefresh_Click(object sender, EventArgs e)
        {
            byte[] b0 = ExternalIpAddress.ToExternalBytes();
            Version? assVersion = Assembly.GetExecutingAssembly().GetName().Version;
            byte[] b1 = (assVersion != null) ? assVersion.ToVersionBytes() : new byte[2] { 0x02, 0x18 };
            string privKey = ExternalIpAddress?.ToString() + ServerIpAddress?.ToString();
            string iv = Constants.BC_START_MSG;
            byte[] keyBytes = CryptHelper.GetUserKeyBytes(privKey, iv, 16);

            ZenMatrix.ZenMatrixGenWithBytes(keyBytes, true);
            TextBoxDestionation.Text = "| 0 | => | ";
            foreach (sbyte sb in ZenMatrix.PermKeyHash)
            {
                TextBoxDestionation.Text += sb.ToString("x1") + " ";
            }
            TextBoxDestionation.Text += "| \r\n";
            for (int zeni = 1; zeni < ZenMatrix.PermKeyHash.Count; zeni++)
            {
                sbyte sb = (sbyte)ZenMatrix.PermKeyHash.ElementAt(zeni);
                TextBoxDestionation.Text += "| " + zeni.ToString("x1") + " | => | " + sb.ToString("x1") + " | " + "\r\n";
            }
            // this.TextBoxDestionation.Text += ZenMatrix.EncryptString(this.RichTextBoxChat.Text) + "\n";
        }

        /// <summary>
        /// MenuItemClear_Click clears all input & output chat windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal override void MenuItemClear_Click(object sender, EventArgs e)
        {
            // TODO: add warning and saving here
            PlaySoundFromResource("sound_glasses");
            this.TextBoxDestionation.Clear();
            this.TextBoxSource.Clear();
            this.RichTextBoxOneView.Clear();
            if (chat != null)
            {
                List<DateTime> chatTimes = chat.CqrMsgs.Keys.ToList();
                chatTimes.Sort();
                DateTime max = DateTime.MinValue;
                foreach (DateTime chatTime in chatTimes)
                    if (chatTime > max)
                        max = chatTime;
                chat.TimeStamp = max.AddMicroseconds(500);

            }
            this.RichTextBoxChat.Clear();
        }

        /// <summary>
        /// SetAttachmentTextLink saves attachment in attachment folder and adds link in <see cref="AttachmentListControl"/>
        /// </summary>
        /// <param name="mimeAttachment"><see cref="MimeAttachment"/></param>
        protected internal void SetAttachmentTextLink(MimeAttachment mimeAttachment)
        {
            string fileName = mimeAttachment.FileName;
            string mimeFilePath = Path.Combine(LibPaths.AttachmentFilesDir, mimeAttachment.FileName + Constants.HTML_EXT);
            string filePath = Path.Combine(LibPaths.AttachmentFilesDir, mimeAttachment.FileName);

            byte[] attachBytes = EnDeCoder.GetBytes(mimeAttachment.GetWebPage());
            System.IO.File.WriteAllBytes(mimeFilePath, attachBytes);

            string base64 = mimeAttachment.Base64Mime;
            if (mimeAttachment.ContentLength < mimeAttachment.Base64Mime.Length)
                base64 = mimeAttachment.Base64Mime.Substring(0, mimeAttachment.ContentLength);

            byte[] fileBytes = Base64.Decode(base64);
            System.IO.File.WriteAllBytes(filePath, fileBytes);

            attachmentListControl.SetNameFilePath(fileName, filePath);
        }


        #endregion OnClientReceive MenuSend MenuAttach MenuRefresh MenuClear


        #region Contacts

        protected internal override void AddContactsToIpContact()
        {
            // base.AddContactsByRefComboBox(ref this.ComboBoxContacts);
            string ipContact = this.ComboBoxContacts.Text;
            this.ComboBoxContacts.Items.Clear();
            foreach (CqrContact ct in Entities.Settings.Singleton.Contacts)
            {
                if (ct != null && !string.IsNullOrEmpty(ct.NameEmail))
                    this.ComboBoxContacts.Items.Add(ct.NameEmail);
            }
            this.ComboBoxContacts.Text = ipContact;
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal override void MenuContactsItemMyContact_Click(object sender, EventArgs e)
        {
            Bitmap? bmp = null;
            ContactSettings contactSettings = new ContactSettings("My Contact Info", 0);
            contactSettings.ShowInTaskbar = true;
            contactSettings.ShowDialog();

            if (Settings.Singleton.MyContact != null && Settings.Singleton.MyContact.ContactImage != null && !string.IsNullOrEmpty(Settings.Singleton.MyContact.ContactImage.ImageBase64))
            {
                try
                {
                    bmp = Settings.Singleton.MyContact.ContactImage.ToDrawingBitmap();
                    if (bmp != null)
                        this.PictureBoxYou.Image = bmp;                    
                }
                catch (Exception exBmp)
                {
                    CqrException.SetLastException(exBmp);
                }

                Settings.SaveSettings(Settings.Singleton);
            }

        }

        #endregion Contacts


        #region SplitChatWindowLayout

        /// <summary>
        /// MenuView_ItemTopBottom_Click occures, when user clicks on Top-Bottom in chat app
        /// shows top bottom view of chat líke ancient talk/talks
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected internal override void MenuView_ItemTopBottom_Click(object sender, EventArgs e)
        {
            MenuViewItemLeftRíght.Checked = false;
            MenuViewItemTopBottom.Checked = true;
            MenuViewItem1View.Checked = false;

            PanelCenter.Visible = true;
            RichTextBoxOneView.Visible = false;

            SplitChatView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            SplitChatView.Panel1MinSize = 220;
            SplitChatView.Panel2MinSize = 220;
            SplitChatView.SplitterDistance = 226;
            SplitChatView.SplitterIncrement = 8;
            SplitChatView.SplitterWidth = 8;
            SplitChatView.MinimumSize = new System.Drawing.Size(800, 400);

            SplitChatView.Visible = true;
            SplitChatView.BringToFront();

        }

        /// <summary>
        /// MenuView_ItemLeftRíght_Click occurs, when user clicks on View->Left-Right in chat app
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected internal override void MenuView_ItemLeftRíght_Click(object sender, EventArgs e)
        {
            MenuViewItemLeftRíght.Checked = true;
            MenuViewItemTopBottom.Checked = false;
            MenuViewItem1View.Checked = false;

            PanelCenter.Visible = true;
            RichTextBoxOneView.Visible = false;

            SplitChatView.Orientation = System.Windows.Forms.Orientation.Vertical;
            SplitChatView.Panel1MinSize = 380;
            SplitChatView.Panel2MinSize = 380;
            SplitChatView.SplitterDistance = 396;
            SplitChatView.SplitterIncrement = 8;
            SplitChatView.SplitterWidth = 8;
            SplitChatView.MinimumSize = new System.Drawing.Size(800, 400);

            SplitChatView.Visible = true;
            SplitChatView.BringToFront();
        }

        /// <summary>
        /// MenuView_Item1View_Click, occurs, when user clicks on 1-View in chat app
        /// shows only a single rich textbox instead of sender and receiver view
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected internal override void MenuView_Item1View_Click(object sender, EventArgs e)
        {
            MenuViewItemLeftRíght.Checked = false;
            MenuViewItemTopBottom.Checked = false;
            MenuViewItem1View.Checked = true;

            PanelCenter.Visible = true;
            SplitChatView.Visible = false;
            RichTextBoxOneView.Visible = true;
            RichTextBoxOneView.BringToFront();
        }

        #endregion SplitChatWindowLayout


        /// <summary>
        /// SetupNetwork async method to setup network
        /// </summary>
        /// <returns><see cref="Task"/></returns>

        
        internal virtual async Task SetupNetwork()
        {
            List<IPAddress> addresses = GetProxiesFromSettingsResources();
            SetStatusText(StripStatusLabel, $"Setup Network: Several proxy addresses fetched.");
            List<IPAddress> interfaceIPAddrs = await NetworkAddresses.GetIpAddressesAsync();
            SetStatusText(StripStatusLabel, $"Setup Network: All network interfaces addresses fetched.");
            List<IPAddress> connectedIPs = await NetworkAddresses.GetConnectedIpAddressesAsync(addresses);
            SetStatusText(StripStatusLabel, $"Setup Network: All active connected ip addresses fetched.");


            List<string> myIpStrList = new List<string>();
            int mchecked = 0;
            this.MenuNetworkItemIPv6Secure.Checked = false;
            foreach (IPAddress addr in interfaceIPAddrs)
            {
                if (addr != null)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(addr.AddressFamily.ShortInfo() + addr.ToString(), null, IPInterfaceAddressSelected, addr.ToString());
                    item.Checked = false;

                    if (IPAddress.IsLoopback(addr))
                    {
                        item.BackColor = SystemColors.ActiveCaption;
                        item.ForeColor = SystemColors.ActiveCaptionText;
                        SetMenuItemForeColor(item, SystemColors.ActiveCaptionText);
                        SetMenuItemBackColor(item, SystemColors.ActiveCaption);
                    }
                    else
                    {
                        item.BackColor = SystemColors.MenuBar;
                        item.ForeColor = SystemColors.GrayText;
                        SetMenuItemForeColor(item, SystemColors.GrayText);
                        SetMenuItemBackColor(item, SystemColors.MenuBar);
                    }

                    if (connectedIPs != null && connectedIPs.Count > 0)
                    {
                        foreach (IPAddress connectedIp in connectedIPs)
                        {
                            if (addr.IsSameIp(connectedIp))
                            {
                                item.BackColor = SystemColors.MenuBar;
                                item.ForeColor = SystemColors.MenuText;
                                SetMenuItemForeColor(item, SystemColors.MenuText);
                                SetMenuItemBackColor(item, SystemColors.MenuBar);


                                if (mchecked++ == 0)
                                {
                                    item.BackColor = SystemColors.MenuHighlight;
                                    item.ForeColor = SystemColors.MenuText;
                                    SetMenuItemForeColor(item, SystemColors.MenuText);
                                    SetMenuItemBackColor(item, SystemColors.MenuHighlight);

                                    clientIpAddress = addr;
                                    item.Checked = true;
                                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                        SetMenuItemChecked(this.MenuNetworkItemIPv6Secure, true);
                                    // this.MenuNetworkItemIPv6Secure.Checked = true;
                                    ipSockListener = new Area23.At.Framework.Core.Net.IpSocket.Listener(clientIpAddress, OnClientReceive);
                                }

                                break;
                            }
                        }

                    }

                    myIpStrList.Add(addr.ToString());
                    AddMenuItemToItems(MenuNetworkItemMyIps, (ToolStripDropDownItem)item);
                    // this.MenuNetworkItemMyIps.DropDownItems.Add(item);
                }
            }

            SetStatusText(StripStatusLabel, $"Setup Network: All interface addresses added to menu. Not connected if addrs grayed.");

            ToolStripMenuItem extIpItem = new ToolStripMenuItem(ExternalIpAddress.AddressFamily.ShortInfo() + ExternalIpAddress.ToString(), null, null, ExternalIpAddress.ToString());
            extIpItem.Checked = true;
            extIpItem.Enabled = false;
            AddMenuItemToItems(this.MenuItemExternalIp, (ToolStripDropDownItem)extIpItem);
            try
            {
                if (ExternalIpAddressV6 != null)
                {
                    ToolStripMenuItem extIpV6Item = new ToolStripMenuItem(ExternalIpAddressV6.AddressFamily.ShortInfo() + ExternalIpAddressV6.ToString(), null, null, ExternalIpAddressV6.ToString());
                    extIpV6Item.Checked = true;
                    extIpV6Item.Enabled = false;
                    AddMenuItemToItems(this.MenuItemExternalIp, (ToolStripDropDownItem)extIpV6Item);
                }
            }
            catch (Exception exV6)
            {
                Area23Log.LogStatic(exV6);
            }
            // this.MenuItemExternalIp.DropDownItems.Add(extIpItem);

            SetStatusText(StripStatusLabel, $"Setup Network: External client ip address added to menu.");


            mchecked = 0;
            List<string> proxyList = new List<string>();
            foreach (IPAddress addrProxy in addresses)
            {
                if (addrProxy != null &&
                    ((addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ||
                    (addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)))
                {
                    proxyList.Add(addrProxy.ToString());
                    
                    ToolStripMenuItem item = new ToolStripMenuItem(addrProxy.AddressFamily.ShortInfo() + addrProxy.ToString(), null, ServerProxyAddressSelected, addrProxy.ToString());
                    if (ServerIpAddress != null && addrProxy.IsSameIp(ServerIpAddress))
                    {
                        if ((addrProxy.AddressFamily == AddressFamily.InterNetworkV6 && GetMenuItemChecked(MenuNetworkItemIPv6Secure)) ||
                            (addrProxy.AddressFamily == AddressFamily.InterNetwork))
                        {
                            SetMenuItemChecked(item, true);
                            // item.Checked = true;
                        }
                    }

                    AddMenuItemToItems(MenuNetworkItemProxyServers, (ToolStripDropDownItem)item);
                    // this.MenuNetworkItemProxyServers.DropDownItems.Add(item);
                }

            }

            SetStatusText(StripStatusLabel, $"Setup Network: Proxy ips added to menu.");

            foreach (var friendIp in Entities.Settings.Singleton.FriendIPs)
            {
                if (!string.IsNullOrEmpty(friendIp))
                {
                    try
                    {
                        if (IPAddress.TryParse(friendIp, out IPAddress ipFriendAddr))
                        {
                            var comboMenuItems = GetMenuDropDownItems(MenuNetworkComboBoxFriendIp);
                            if (!comboMenuItems.Contains(ipFriendAddr.ToString()))
                                AddMenuItemToMenuComboBox(MenuNetworkComboBoxFriendIp, ipFriendAddr.ToString());
                            var comboItems = GetComboBoxItems(this.ComboBoxIp);

                            if (!comboItems.Contains(ipFriendAddr.ToString()))
                                AddItemToComboBox(this.ComboBoxIp, ipFriendAddr.ToString());
                        }
                    }
                    catch (Exception exFriendIp)
                    {
                        Area23Log.LogStatic("Error when adding friendIps + " + exFriendIp.Message);
                    }
                }
            }

            if (Entities.Settings.Singleton != null)
            {
                Entities.Settings.Singleton.Proxies = proxyList;
                Entities.Settings.Singleton.MyIPs = myIpStrList;
                Entities.Settings.SaveSettings(Entities.Settings.Singleton);
            }

            SetStatusText(StripStatusLabel, $"Setup Network complete!");
        }


        /// <summary>
        /// IPAdressSelected Delegate is invoked, 
        /// when a different IP Address in context menu is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IPInterfaceAddressSelected(object sender, EventArgs e)
        {
            if (sender != null && sender is ToolStripMenuItem newAddrIf)
            {
                ToolStripMenuItem? oldAddrIf = null;

                foreach (ToolStripMenuItem dditem in this.MenuNetworkItemMyIps.DropDownItems)
                    if (dditem.Checked)
                        oldAddrIf = dditem;

                IPAddress clIp = clientIpAddress;
                try
                {
                    if (IPAddress.TryParse(newAddrIf.Name, out clIp))
                    {
                        newAddrIf.Checked = true;
                        if (oldAddrIf != null)
                            oldAddrIf.Checked = false;
                        clientIpAddress = clIp;

                        try
                        {
                            if (ipSockListener != null)
                                ipSockListener.Dispose();
                        }
                        catch (Exception exi)
                        {
                            Area23Log.LogStatic(exi);
                        }
                        try
                        {
                            ipSockListener = null;
                        }
                        catch (Exception exi)
                        {
                            Area23Log.LogStatic(exi);
                        }

                        Thread.Sleep(Constants.CLOSING_TIMEOUT);
                        ipSockListener = new Area23.At.Framework.Core.Net.IpSocket.Listener(clientIpAddress, OnClientReceive);
                        SetStatusText(StripStatusLabel, $"Listening on  {clientIpAddress.AddressFamily.ShortInfo()} {clientIpAddress.ToString()}:{Constants.CHAT_PORT}");
                    }
                }
                catch (Exception exc)
                {
                    Area23Log.LogStatic(exc);
                }
            }
        }


        public void ServerProxyAddressSelected(object sender, EventArgs e)
        {
            if (sender != null && sender is ToolStripMenuItem newProxyItem)
            {
                List<IPAddress> ips = new List<IPAddress>();
                ToolStripMenuItem? oldProxyItem = null;
                foreach (ToolStripMenuItem dditem in this.MenuNetworkItemProxyServers.DropDownItems)
                    if (dditem.Checked == true)
                        oldProxyItem = dditem;

                try
                {
                    IPAddress newSrvAddr = IPAddress.Parse(newProxyItem.Name);

                    string resp = TcpClientWebRequest.MakeWebRequest(newSrvAddr, out ips);

                    if (resp != null && ips != null && ips.Count > 2 && ips.ElementAt(2) != null)
                    {
                        _serverIpAddress = newSrvAddr;
                        newProxyItem.Checked = true;
                        if (oldProxyItem != null && oldProxyItem.Checked)
                            oldProxyItem.Checked = false;

                        SetStatusText(StripStatusLabel, $"ServerIp set to {_serverIpAddress.AddressFamily.ShortInfo()} {_serverIpAddress.ToString()}");
                    }
                }
                catch (Exception exi)
                {
                    Area23Log.LogStatic(exi);
                }

                Thread.Sleep(Constants.CLOSING_TIMEOUT);

            }
        }


        private void ButtonAttach_Click(object sender, EventArgs e)
        {
            this.MenuItemAttach_Click(sender, e);
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            this.MenuItemSend_Click(sender, e);
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            this.MenuItemClear_Click(sender, e);
        }

    }

}
