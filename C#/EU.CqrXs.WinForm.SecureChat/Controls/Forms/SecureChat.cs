using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.CqrXs.CqrSrv;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Net;
using Area23.At.Framework.Core.Net.IpSocket;
using Area23.At.Framework.Core.Net.NameService;
using Area23.At.Framework.Core.Net.WebHttp;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Controls.Forms.Base;
using EU.CqrXs.WinForm.SecureChat.Controls.UserControls;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Util;
using System.ComponentModel;
using System.Formats.Tar;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Runtime.InteropServices.JavaScript;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{

    /// <summary>
    /// SecureChat main form
    /// </summary>
    public partial class SecureChat : BaseChatForm
    {

        #region fields        

        private string? myServerKey = null, ipAddrString = null, contactNameEmail = null;
        internal static int attachCnt = 0;
        internal static int chatCnt = 0;
        internal static Chat? chat;

        protected internal static IPAddress? clientIpAddress;
        protected internal static IPAddress? partnerIpAddress;
        protected internal static Listener? ipSockListener;
        // protected internal static SockTcpListener? sockTcpListener;
        internal delegate void ClientSocket_DataReceived(object sender, Area23EventArgs<ReceiveData> eventReceived);
        internal ClientSocket_DataReceived clientSocket_DataReceived;
        internal EventHandler<Area23EventArgs<ReceiveData>> receivedDataEventHandler;

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
        public SecureChat() : base()
        {
            InitializeComponent();
            TextBoxSource.MaxLength = Constants.SOCKET_BYTE_BUFFEER;
            TextBoxDestionation.MaxLength = Constants.SOCKET_BYTE_BUFFEER;
            ComboBoxIp.Text = Constants.ENTER_IP;
            ComboBoxContacts.Text = Constants.ENTER_CONTACT;
            ComboBoxSecretKey.Text = Constants.ENTER_SECRET_KEY;
            MiniToolBox.CreateAttachDirectory();
            this.DragNDropGroupBox.OnDragNDrop += OnDragNDrop;
            this.LinkedLabelsBox.OnDragNDrop += OnDragNDrop;
            this.PeerServerSwitch.FireUpChanged += TooglePeerServer;
            this.StripProgressBar.Value = 0;
        }

        private async void SecureChat_Load(object sender, EventArgs e)
        {
            bool send1stReg = false;

            if (Entities.Settings.LoadSettings() == null || Entities.Settings.Singleton == null || Entities.Settings.Singleton.MyContact == null)
            {
                // var badge = new TransparentBadge($"Error reading Settings from {LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE}.");
                // badge.Show();
                MenuContactsItemMyContact_Click(sender, e);
                this.StripProgressBar.Value = 10;
                while (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Email) || string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Name))
                {
                    string notFullReason = string.Empty;
                    if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Name))
                        notFullReason += "Name is missing!" + Environment.NewLine;
                    if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Email))
                        notFullReason += "Email Address is missing!" + Environment.NewLine;
                    // if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Mobile))
                    //     notFullReason += "Mobile phone is missing!" + Environment.NewLine;
                    MessageBox.Show(notFullReason, "Please fill out your info fully", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    this.StripProgressBar.Value = 20;
                    MenuContactsItemMyContact_Click(sender, e);
                }
                send1stReg = true;
            }
            this.StripProgressBar.Value = 30;

            await BaseChatForm_Load(sender, e);            
            bgWorkerMonitor.RunWorkerAsync();

            StripStatusLabel.Text = "Setup Network";
            await PlaySoundFromResourcesAsync("sound_volatage");
            await SetupNetwork();

            this.StripProgressBar.Value = 50;

            Bitmap? bmp = Properties.fr.Resources.DefaultF45;
            if (Entities.Settings.Singleton != null && Entities.Settings.Singleton.MyContact != null && Entities.Settings.Singleton.MyContact.ContactImage != null &&
                !string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.ContactImage.ImageBase64))
            {
                bmp = (Bitmap?)Entities.Settings.Singleton.MyContact.ContactImage.ToDrawingBitmap();
                if (bmp == null)
                    bmp = Properties.fr.Resources.DefaultF45;
            }
            this.PictureBoxYou.Image = bmp;

            AddContactsToIpContact();
            this.StripProgressBar.Value = 70;

            if (send1stReg)
                Send_1st_Server_Registration(sender, e);

            this.StripProgressBar.Value = 100;
            StripStatusLabel.Text = "Secure Chat init done.";

            System.Timers.Timer timerResetProgress = new System.Timers.Timer { Interval = 1000 };
            timerResetProgress.Elapsed += (s, en) =>
            {
                this.Invoke(new Action(() =>
                {
                    ResetProgressBar(StripProgressBar);
                }));
                timerResetProgress.Stop(); // Stop the timer(otherwise keeps on calling)
            };
            timerResetProgress.Start();
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
            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
                return;
            
            SrvMsg serverMessage = new SrvMsg(myServerKey, myServerKey);
            // TODO: SetText delegate AppendText()
            this.TextBoxPipe.Text = serverMessage.PipeString;
            this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString; 
        }


        /// <summary>
        /// ComboBoxSecretKey_FocusLeave event is fired, when we leave focus of ComboBoxSecretKe
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void ComboBoxSecretKey_FocusLeave(object sender, EventArgs e)
        {
            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
                return;

            ButtonKey_Click(sender, e);
            if (Entities.Settings.Singleton != null)
            {
                if (!Entities.Settings.Singleton.SecretKeys.Contains(this.ComboBoxSecretKey.Text))
                    Entities.Settings.Singleton.SecretKeys.Add(this.ComboBoxSecretKey.Text);
                if (!this.ComboBoxSecretKey.Items.Contains(this.ComboBoxSecretKey.Text))
                    this.ComboBoxSecretKey.Items.Add(this.ComboBoxSecretKey.Text);
            }
            SetStatusText(StripStatusLabel, "Added new secret key => calculated new SecurePipe...");
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
            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
                return;
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
            if (!GetComboBoxEnabled(ComboBoxIp))
                return;

            if ((ipAddrString = GetComboBoxMustHaveText(ref ComboBoxIp)) == null)
                return;
            try
            {
                if (!IPAddress.TryParse(ipAddrString, out partnerIpAddress))
                    throw new InvalidOperationException($"IPAddress {this.ComboBoxIp.Text ?? string.Empty} is not parsable!");
            }
            catch (Exception exIpContact)
            {
                MessageBox.Show($"Cannot parse IpAddress from string \"{ComboBoxIp.Text}\": {exIpContact.Message}", "Please enter a valid connectable ipv4 or ipv6 address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetComboBoxBackColor(ComboBoxIp, Color.Violet);
                PlaySoundFromResource("sound_warning");
                return;
            }

            SetComboBoxBackColor(ComboBoxIp, Color.White);
            // this.ComboBoxIp.BackColor = Color.White;

            if (Entities.Settings.Singleton != null && SendInit_Click())
            {
                PlaySoundFromResource("sound_laser");

                AddIpToFriendList(sender, e);
            }
            else
            {
                ButtonCheck.Image = Properties.de.Resources.CableWireCut;
                PlaySoundFromResource("sound_warning");
            }

            SetStatusText(StripStatusLabel, $"Added new partner ip address {partnerIpAddress.ToString()}.");

        }

        internal void AddIpToFriendList(object sender, EventArgs e)
        {
            string comboIpText = GetComboBoxText(this.ComboBoxIp);
            if (!string.IsNullOrEmpty(comboIpText))
            {
                if (IPAddress.TryParse(comboIpText, out partnerIpAddress))
                {
                    if (!Entities.Settings.Singleton.FriendIPs.Contains(comboIpText))
                        Entities.Settings.Singleton.FriendIPs.Add(comboIpText);
                    var comboMenuItems = GetMenuDropDownItems(MenuNetworkComboBoxFriendIp);
                    if (!comboMenuItems.Contains(partnerIpAddress.ToString()))
                        AddMenuItemToMenuComboBox(MenuNetworkComboBoxFriendIp, partnerIpAddress.ToString());

                    try
                    {
                        this.MenuNetworkComboBoxFriendIp.Text = partnerIpAddress.ToString();
                    }
                    catch { }

                    var ipComboItems = GetComboBoxItems(ComboBoxIp);
                    if (!ipComboItems.Contains(comboIpText))
                        AddItemToComboBox(ComboBoxIp, partnerIpAddress.ToString());

                    Entities.Settings.SaveSettings(Entities.Settings.Singleton);
                }
            }
        }


        private void ComboBoxIp_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!GetComboBoxEnabled(ComboBoxIp))
                return;

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
            if (!GetComboBoxEnabled(ComboBoxContacts))
                return;
        }

        private void ComboBoxContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!GetComboBoxEnabled(ComboBoxContacts))
                return;

            if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                return;

            bool foundContact = false;
            CqrContact? friendContact = null;
            string exContactMsg = "";
            try
            {

                foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
                {
                    if (c.NameEmail.Equals(contactNameEmail, StringComparison.InvariantCultureIgnoreCase))
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
                SetComboBoxBackColor(ComboBoxContacts, Color.Violet);
                PlaySoundFromResource("sound_warning");
                MessageBox.Show($"Cannot parse Contact from string \"{ComboBoxContacts.Text}\": {exContactMsg}", "Please enter a valid contact address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetComboBoxText(ComboBoxIp, Constants.ENTER_IP);
            SetComboBoxBackColor(ComboBoxContacts, Color.White);
            StripStatusLabel.Text = $"Selected Contact {contactNameEmail}.";

            bool sendInit = false;
            try
            {
                sendInit = SendInit_Contact();
            }
            catch (Exception exi)
            {
                Area23Log.LogStatic($"Excption {exi.GetType()}: {exi.Message}\n\t{exi}\n");
                sendInit = false;
                SetStatusText(this.StripStatusLabel, $"Excption {exi.GetType()} on init chat room invitation: {exi.Message}");
            }

            if (sendInit)
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

        #region MenuCommands MenuSend MenuAttach MenuRefresh MenuClear incl. Buttons

        /// <summary>
        /// Send_1st_Server_Registration sends contact registration to cqrxs.eu server
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void Send_1st_Server_Registration(object sender, EventArgs e)
        {
            if (chat == null)
                chat = new Chat(0);


            myServerKey = CqrXsEuSrvKey;


            SrvMsg1 srv1stMsg = new SrvMsg1(CqrXsEuSrvKey);
            this.TextBoxPipe.Text = srv1stMsg.PipeString;
            this.toolStripTextBoxCqrPipe.Text = srv1stMsg.PipeString;
            Thread.Sleep(100);

            this.StripProgressBar.Value = 50;

            CqrContact myContact = Entities.Settings.Singleton.MyContact;
            string ser = (string)AppDomain.CurrentDomain.GetData(Constants.MY_CONTACT);
            string encrypted = srv1stMsg.CqrSrvMsg1(myContact, EncodingType.Base64);
            Thread.Sleep(100);

            this.StripProgressBar.Value = 60;
            CqrContact? returnContact = srv1stMsg.SendFirstSrvMsg_Soap(myContact, ServerIpAddress, EncodingType.Base64);

            string usrMsg = $"Registering contact: {myContact.NameEmail}\n";
            string srvMsg = "";
            this.TextBoxSource.Text = chat.AddMyMessage(usrMsg);
            if (returnContact != null)
            {
                returnContact.ContactId = 0;
                Settings.Instance.MyContact = returnContact;
                srvMsg = $"Got Cuid: {returnContact.Cuid} for {returnContact.NameEmail}\n";
                this.TextBoxDestionation.Text = chat.AddFriendMessage(srvMsg);
                Settings.SaveSettings(Settings.Singleton);
            }

            // this.RichTextBoxOneView.Rtf = this.RichTextBoxChat.Rtf;
            Format_Lines_RichTextBox();
            StripStatusLabel.Text = "Finished 1st registration";
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

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
                return false;
            if ((ipAddrString = GetComboBoxMustHaveText(ref ComboBoxIp)) == null)
                return false;

            string unencrypted = "Init: " + clientIpAddress?.ToString() + " " + Entities.Settings.Singleton.MyContact.NameEmail;
            try
            {
                if (!IPAddress.TryParse(ipAddrString, out partnerIpAddress))
                    throw new InvalidDataException("Cannot parse " + ipAddrString + " to IPAddress!");

                Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                pmsg.Send_CqrPeerMsg(unencrypted, partnerIpAddress, Constants.CHAT_PORT, EncodingType.Base64);

                string userMsg = chat.AddMyMessage(unencrypted);
                AppendText(TextBoxSource, userMsg);
                // Format_Lines_RichTextBox();
                this.RichTextBoxChat.Text = string.Empty;
                SetStatusText(StripStatusLabel, $"Send init to {partnerIpAddress} successfully");
                ButtonCheck.Image = Properties.de.Resources.RemoteConnect;
            }
            catch (Exception ex)
            {
                Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in SendInit_Click: {ex.Message}.\n", ex);
                SetStatusText(StripStatusLabel, $"Sending to {ipAddrString} failed: {ex.Message}");
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

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
                return false;


            if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                return false;

            this.textBoxChatSession.Text = (Settings.Instance.MyContact.ChatRoomId) ?? string.Empty;

            string unencrypted = "Init: " + clientIpAddress?.ToString() + " " + Entities.Settings.Singleton.MyContact.NameEmail;

            
            CqrContact myContact = new CqrContact(Settings.Singleton.MyContact, this.textBoxChatSession.Text, this.TextBoxPipe.Text);
            CqrContact? friendContact = null;
            foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
            {
                if (c.NameEmail.Equals(contactNameEmail, StringComparison.InvariantCultureIgnoreCase))
                {
                    friendContact = new CqrContact(c, this.textBoxChatSession.Text, this.TextBoxPipe.Text);
                    break;
                }
            }


            SrvMsg serverMessage = new SrvMsg(myContact, friendContact, CqrXsEuSrvKey, myServerKey);
            this.TextBoxPipe.Text = serverMessage.PipeString;
            this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
            myContact._hash = GetHash();
            friendContact._hash = GetHash();
            serverMessage = new SrvMsg(myContact, friendContact, CqrXsEuSrvKey, myServerKey);

            FullSrvMsg<string> fmsg = new FullSrvMsg<string>(myContact, friendContact, myContact.Email, serverMessage.PipeString);
            string myReqMsg = $"{fmsg.Sender.NameEmail} requests a new chatroom from server\n";
            this.TextBoxSource.Text = chat.AddMyMessage(myReqMsg);

            FullSrvMsg<string> rfmsg = serverMessage.Send_InitChatRoom_Soap(fmsg, ServerIpAddress, EncodingType.Base64);            
            if (rfmsg == null || string.IsNullOrEmpty(rfmsg.ChatRoomNr))
            {
                MessageBox.Show($"Response message form server {ServerIpAddress} is null. Please call helpdesk +436507527928", "Invite Chatroom failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            this.textBoxChatSession.Text = rfmsg.ChatRoomNr;
            if (rfmsg != null && rfmsg.Sender != null && rfmsg.Sender.NameEmail.Equals(myContact.NameEmail))
            {
                Settings.Singleton.MyContact.Cuid = rfmsg.Sender.Cuid;
                Settings.Singleton.MyContact.LastPolled = rfmsg.Sender.LastPolled;
                Settings.Singleton.MyContact.LastPushed = rfmsg.Sender.LastPushed;
                Settings.Singleton.MyContact.ChatRoomId = rfmsg.Sender.ChatRoomId;

                Settings.SaveSettings(Settings.Singleton);

            }
            // TODO: Email zur Einladung
            string msgChatRoom = "Received ChatRoomNr: " + rfmsg.ChatRoomNr + "\nfor " + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
            this.TextBoxDestionation.Text = chat.AddFriendMessage(msgChatRoom);

            // this.RichTextBoxOneView.Rtf = this.RichTextBoxChat.Rtf;
            Format_Lines_RichTextBox();
            SetStatusText(StripStatusLabel, msgChatRoom.Replace("\n", " "));

            return true;

        }

        /// <summary>
        /// Sends a secure message
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void MenuCommandsItemSend_Click(object sender, EventArgs e)
        {
            // TODO: implement it via socket directly or to registered user
            // if Ip is pingable and reachable and connectable
            // send HELLO to IP
            if (chat == null)
                chat = new Chat(0);

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
            {
                StripStatusLabel.Text = "Nothing to send!";
                return;
            }

            string unencrypted = GetRichTextBoxText(this.RichTextBoxChat);  // Text; //.Replace("\r\n", "\n").Replace("\n", " " + Environment.NewLine);
            if (string.IsNullOrEmpty(unencrypted) || unencrypted.Trim(" \t\n\r\v".ToCharArray()).Length < 1)
            {
                SetStatusText(StripStatusLabel, "Empty message could not be send!");
                return;
            }

            if (this.PeerSessionTriState == PeerSession3State.Peer2Peer)
            {

                if ((ipAddrString = GetComboBoxMustHaveText(ref ComboBoxIp)) == null)
                {
                    StripStatusLabel.Text = "Nothing to send (no ip addr).";
                    return;
                }

                try
                {
                    if (!IPAddress.TryParse(ipAddrString, out partnerIpAddress))
                        throw new InvalidDataException("Cannot parse IPAddress " + ipAddrString);

                    Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                    pmsg.Send_CqrPeerMsg(unencrypted, partnerIpAddress, Constants.CHAT_PORT, EncodingType.Base64);

                    string userMsg = chat.AddMyMessage(unencrypted);
                    AppendText(TextBoxSource, userMsg);
                    Format_Lines_RichTextBox();
                    this.RichTextBoxChat.Text = string.Empty;
                    StripStatusLabel.Text = $"Send to {partnerIpAddress} successfully.";
                    PlaySoundFromResource("sound_arrow");
                }
                catch (Exception ex)
                {
                    Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in MenuCommandsItemSend_Click: {ex.Message}.\n", ex);
                    SetStatusText(StripStatusLabel, $"Sending to {ipAddrString} failed: {ex.Message}");
                    PlaySoundFromResource("sound_warning");
                }
            }
            else if (this.PeerSessionTriState == PeerSession3State.ChatServer)
            {

                // if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                //     return ;

                string chatRoomNr = textBoxChatSession.Text ?? Entities.Settings.Singleton.MyContact.ChatRoomId;
                if (string.IsNullOrEmpty(textBoxChatSession.Text))
                    textBoxChatSession.Text = chatRoomNr;
                
                if (string.IsNullOrEmpty(textBoxChatSession.Text))
                {
                    InputDialog dialog = new InputDialog("ChatRoomNr required", "Please enter a valid chat room number or register a new chatroom.", MessageBoxIcon.Warning);
                    dialog.ShowDialog();
                    chatRoomNr = (AppDomain.CurrentDomain.GetData("InputDialog") != null) ? ((string)AppDomain.CurrentDomain.GetData("InputDialog")) : string.Empty;
                    textBoxChatSession.Text = (!string.IsNullOrEmpty(chatRoomNr)) ? chatRoomNr : textBoxChatSession.Text;
                }

                CqrContact myContact = new CqrContact(Settings.Singleton.MyContact, chatRoomNr, TextBoxPipe.Text);
                CqrContact? friendContact = null;
                foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
                {
                    if (c.NameEmail.Equals(contactNameEmail, StringComparison.InvariantCultureIgnoreCase))
                    {
                        friendContact = new CqrContact(c, chatRoomNr, TextBoxPipe.Text);                        
                        break;
                    }
                }
                
                SrvMsg serverMessage = new SrvMsg(myContact, friendContact, CqrXsEuSrvKey, myServerKey);
                this.TextBoxPipe.Text = serverMessage.PipeString;
                this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
                myContact._hash = GetHash();
                myContact.ChatRoomId = chatRoomNr;

                if (friendContact != null)
                {
                    friendContact._hash = GetHash();
                    friendContact.ChatRoomId = chatRoomNr;
                    serverMessage = new SrvMsg(myContact, friendContact, CqrXsEuSrvKey, myServerKey);
                }
                else
                    serverMessage = new SrvMsg(myContact, myContact, CqrXsEuSrvKey, myServerKey);

                FullSrvMsg<string> fmsg = new FullSrvMsg<string>(myContact, friendContact ?? myContact, chatRoomNr, serverMessage.PipeString, chatRoomNr);
                // FullSrvMsg<string> cmsg = new FullSrvMsg<string>(myContact, friendContact, unencrypted, serverMessage.ClientPipeString, chatRoomNr);
                // ClientSrvMsg<string, string> ccmsg = new ClientSrvMsg<string, string>(fmsg, cmsg, chatRoomNr, unencrypted);
                // string encrypted[] = serverMessage.CqrSrvMsg(fmsg, cmsg, EncodingType.Base64);
                Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                string encrypted = pmsg.CqrPeerMsg(unencrypted);

                FullSrvMsg<string> rfmsg = serverMessage.SendChatMsg_Soap_Simple<string>(fmsg, encrypted, ServerIpAddress, EncodingType.Base64); 
                if (rfmsg != null && rfmsg.Sender != null)
                {
                    Settings.Singleton.MyContact.LastPolled = rfmsg.Sender.LastPolled;
                    Settings.Singleton.MyContact.LastPushed = rfmsg.Sender.LastPushed;
                    Settings.Singleton.MyContact.ChatRoomId = rfmsg.Sender.ChatRoomId;

                    Settings.SaveSettings(Settings.Singleton);
                }

                // string msgChatRoom = "ChatRoomNr: " + rfmsg.ChatRoomNr + "\n" + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
                // AppendText(TextBoxDestionation, chat.AddFriendMessage(msgChatRoom));
                string userMsg = chat.AddMyMessage(unencrypted);
                AppendText(TextBoxSource, userMsg);
                
                // this.RichTextBoxOneView.Rtf = this.RichTextBoxChat.Rtf;
                Format_Lines_RichTextBox();
                this.RichTextBoxChat.Text = string.Empty;
                StripStatusLabel.Text = $"Send to {chatRoomNr} via server {ServerIpAddress} successfully.";
                PlaySoundFromResource("sound_arrow");
                SetStatusText(StripStatusLabel, "Finished 1st registration");
            }
            // otherwise send message to registered user via server
            // Always encrypt via key
        }

        /// <summary>
        /// Attaches a file to send
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void MenuCommandsItemAttach_Click(object sender, EventArgs e)
        {
            if (chat == null)
                chat = new Chat(0);

            string encrypted = string.Empty;

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
            {
                SetStatusText(StripStatusLabel, "Nothing to send!");
                return;
            }

            if (this.PeerSessionTriState == PeerSession3State.Peer2Peer)
            {

                if ((ipAddrString = GetComboBoxMustHaveText(ref ComboBoxIp)) == null)
                {
                    StripStatusLabel.Text = "Nothing to send (no ip addr).";
                    return;
                }

                myServerKey = this.ComboBoxSecretKey.Text;

                FileOpenDialog = DialogFileOpen;
                DialogResult result = FileOpenDialog.ShowDialog();
                if ((result == DialogResult.OK || result == DialogResult.Yes) && File.Exists(FileOpenDialog.FileName))
                {
                    Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                    CqrFile? cqrFile = GetCqrFileFromPath(FileOpenDialog.FileName, pmsg.PipeString);

                    if (cqrFile != null && !string.IsNullOrEmpty(this.ComboBoxIp.Text) && !this.ComboBoxIp.Text.Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            partnerIpAddress = IPAddress.Parse(this.ComboBoxIp.Text);

                            // pmsg.SendCqrPeerMsg(mimeAttach.MimeMsg, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);
                            pmsg.Send_CqrFile(cqrFile, partnerIpAddress, Constants.CHAT_PORT, MsgEnum.Json, EncodingType.Base64);

                            string base64FilePath = Path.Combine(LibPaths.AttachmentFilesDir, cqrFile.CqrFileName + Constants.BASE64_EXT);
                            System.IO.File.WriteAllText(base64FilePath, cqrFile.ToBase64());

                            string userMsg = chat.AddMyMessage(cqrFile.GetFileNameContentLength());
                            AppendText(TextBoxSource, userMsg);
                            Format_Lines_RichTextBox();
                            this.RichTextBoxChat.Text = string.Empty;
                            SetStatusText(StripStatusLabel, $"File {cqrFile.CqrFileName} send to {partnerIpAddress} successfully!");
                        }
                        catch (Exception ex)
                        {
                            Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in MenuItemAttach_Click: {ex.Message}.\n", ex);
                            SetStatusText(StripStatusLabel, "Attach FAILED: " + ex.Message);
                            PlaySoundFromResource("sound_warning");
                        }
                    }
                    // otherwise send message to registered user via server
                    // Always encrypt via key
                }
            }
            else if (this.PeerSessionTriState == PeerSession3State.ChatServer)
            {
                //if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                //    return;

                string chatRoomNr = textBoxChatSession.Text ?? Entities.Settings.Singleton.MyContact.ChatRoomId;
                if (string.IsNullOrEmpty(textBoxChatSession.Text))
                    textBoxChatSession.Text = chatRoomNr;

                if (string.IsNullOrEmpty(textBoxChatSession.Text))
                {
                    InputDialog dialog = new InputDialog("ChatRoomNr required", "Please enter a valid chat room number or register a new chatroom.", MessageBoxIcon.Warning);
                    dialog.ShowDialog();
                    chatRoomNr = (AppDomain.CurrentDomain.GetData("InputDialog") != null) ? ((string)AppDomain.CurrentDomain.GetData("InputDialog")) : string.Empty;
                    textBoxChatSession.Text = (!string.IsNullOrEmpty(chatRoomNr)) ? chatRoomNr : textBoxChatSession.Text;
                }

                CqrContact myContact = new CqrContact(Settings.Singleton.MyContact, chatRoomNr, TextBoxPipe.Text);
                CqrContact? friendContact = null;
                foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
                {
                    if (c.NameEmail.Equals(contactNameEmail, StringComparison.InvariantCultureIgnoreCase))
                    {
                        friendContact = new CqrContact(c, chatRoomNr, TextBoxPipe.Text);
                        break;
                    }
                }

                SrvMsg serverMessage = new SrvMsg(myContact, friendContact, CqrXsEuSrvKey, myServerKey);
                this.TextBoxPipe.Text = serverMessage.PipeString;
                this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
                myContact._hash = GetHash();
                myContact.ChatRoomId = chatRoomNr;
                if (friendContact != null)
                {
                    friendContact._hash = GetHash();
                    friendContact.ChatRoomId = chatRoomNr;
                    serverMessage = new SrvMsg(myContact, friendContact, CqrXsEuSrvKey, myServerKey);
                }
                else
                    serverMessage = new SrvMsg(myContact, myContact, CqrXsEuSrvKey, myServerKey);

                FullSrvMsg<string> fmsg = new FullSrvMsg<string>(myContact, friendContact ?? myContact, chatRoomNr, serverMessage.PipeString, chatRoomNr);
                // FullSrvMsg<string> cmsg = new FullSrvMsg<string>(myContact, friendContact, unencrypted, serverMessage.ClientPipeString, chatRoomNr);
                // ClientSrvMsg<string, string> ccmsg = new ClientSrvMsg<string, string>(fmsg, cmsg, chatRoomNr, unencrypted);
                // string encrypted[] = serverMessage.CqrSrvMsg(fmsg, cmsg, EncodingType.Base64);
                Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);

                FileOpenDialog = DialogFileOpen;
                DialogResult result = FileOpenDialog.ShowDialog();
                if ((result == DialogResult.OK || result == DialogResult.Yes) && File.Exists(FileOpenDialog.FileName))
                {
                    CqrFile? cqrFile = GetCqrFileFromPath(FileOpenDialog.FileName, pmsg.PipeString);

                    if (cqrFile != null && !string.IsNullOrEmpty(this.textBoxChatSession.Text))
                    {
                        try
                        {
                            
                            // pmsg.SendCqrPeerMsg(mimeAttach.MimeMsg, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);
                            encrypted = pmsg.CqrFile(cqrFile, MsgEnum.Json, EncodingType.Base64);

                            string base64FilePath = Path.Combine(LibPaths.AttachmentFilesDir, cqrFile.CqrFileName + Constants.BASE64_EXT);
                            System.IO.File.WriteAllText(base64FilePath, cqrFile.ToBase64());
                            
                            FullSrvMsg<string> rfmsg = serverMessage.SendChatMsg_Soap_Simple<string>(fmsg, encrypted, ServerIpAddress, EncodingType.Base64);
                            if (rfmsg != null && rfmsg.Sender != null)
                            {
                                Settings.Singleton.MyContact.LastPolled = rfmsg.Sender.LastPolled;
                                Settings.Singleton.MyContact.LastPushed = rfmsg.Sender.LastPushed;
                                Settings.Singleton.MyContact.ChatRoomId = rfmsg.Sender.ChatRoomId;

                                Settings.SaveSettings(Settings.Singleton);
                            }

                            // string msgChatRoom = "ChatRoomNr: " + rfmsg.ChatRoomNr + "\n" + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
                            // this.TextBoxDestionation.Text = msgChatRoom;


                            string userMsg = chat.AddMyMessage(cqrFile.GetFileNameContentLength());
                            AppendText(TextBoxSource, userMsg);
                            Format_Lines_RichTextBox();
                            this.RichTextBoxChat.Text = string.Empty;
                            PlaySoundFromResource("sound_push");
                            SetStatusText(StripStatusLabel, $"File {cqrFile.CqrFileName} send to {partnerIpAddress} successfully!");
                        }
                        catch (Exception ex)
                        {
                            Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in MenuItemAttach_Click: {ex.Message}.\n", ex);
                            SetStatusText(StripStatusLabel, "Attach FAILED: " + ex.Message);
                            PlaySoundFromResource("sound_warning");
                        }
                    }

                }

            }

        }


        private void MenuCommandsItemRefresh_Click(object sender, EventArgs e)
        {

            if (this.PeerSessionTriState == PeerSession3State.ChatServer)
            {

                // if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                //     return;

                string chatRoomNr = textBoxChatSession.Text ?? Entities.Settings.Singleton.MyContact.ChatRoomId;
                if (string.IsNullOrEmpty(textBoxChatSession.Text))
                    textBoxChatSession.Text = chatRoomNr;

                if (string.IsNullOrEmpty(textBoxChatSession.Text))
                {
                    InputDialog dialog = new InputDialog("ChatRoomNr required", "Please enter a valid chat room number or register a new chatroom.", MessageBoxIcon.Warning);
                    dialog.ShowDialog();
                    chatRoomNr = (AppDomain.CurrentDomain.GetData("InputDialog") != null) ? ((string)AppDomain.CurrentDomain.GetData("InputDialog")) : string.Empty;
                    textBoxChatSession.Text = (!string.IsNullOrEmpty(chatRoomNr)) ? chatRoomNr : textBoxChatSession.Text;
                }

                CqrContact? friendContact = null;
                foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
                {
                    if (c.NameEmail.Equals(contactNameEmail, StringComparison.InvariantCultureIgnoreCase))
                    {
                        friendContact = new CqrContact(c, chatRoomNr, TextBoxPipe.Text);
                        break;
                    }
                }

                CqrContact myContact = new CqrContact(Entities.Settings.Singleton.MyContact, chatRoomNr, TextBoxPipe.Text);


                Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);

                myContact._hash = GetHash();
                myContact.ChatRoomId = chatRoomNr;

                if (friendContact != null)
                {
                    friendContact._hash = GetHash();
                    friendContact.ChatRoomId = chatRoomNr;
                }
                SrvMsg serverMessage = new SrvMsg(myContact, friendContact ?? myContact, CqrXsEuSrvKey, myServerKey);
                this.TextBoxPipe.Text = serverMessage.PipeString;
                this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
                

                serverMessage = new SrvMsg(myContact, friendContact ?? myContact, CqrXsEuSrvKey, myServerKey);
                               
                FullSrvMsg<string> fmsg = new FullSrvMsg<string>(myContact, friendContact ?? myContact, chatRoomNr, serverMessage.PipeString, chatRoomNr);
                FullSrvMsg<string> rfmsg = serverMessage.ReceiveChatMsg_Soap<string>(fmsg, ServerIpAddress, EncodingType.Base64);


                if (rfmsg != null && rfmsg.Sender != null)
                {
                    myContact = new CqrContact(rfmsg.Sender, rfmsg.ChatRoomNr, Settings.Instance.MyContact.ContactImage, rfmsg.Sender.Hash);
                    Settings.Singleton.MyContact = myContact;

                    Settings.SaveSettings(Settings.Singleton);
                }

                string msgChatRoom = "ChatRoomNr: " + rfmsg.ChatRoomNr + "\n" + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
                MsgContent msgContent;
                try
                {                    
                    msgContent = pmsg.NCqrPeerMsg(((string)rfmsg.TContent));
                    // serverMessage.NCqrClientMsgTC<string>((string)rfmsg.TContent);
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
                CqrFile? cqrReceivedFile = null;
                
                if (msgContent.IsCqrFile())
                {
                    cqrReceivedFile = msgContent.ToCqrFile();
                    if (cqrReceivedFile != null)
                    {
                        SetAttachmentTextLink(cqrReceivedFile);
                        friendMsg = cqrReceivedFile.GetFileNameContentLength() + Environment.NewLine;
                        PlaySoundFromResource("sound_wind");
                    }
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
                this.RichTextBoxChat.Text = string.Empty;
                StripStatusLabel.Text = $"Received msg from server {ServerIpAddress} chat room {chatRoomNr}.";                
            }
        }

        /// <summary>
        /// MenuItemClear_Click clears all input & output chat windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuCommandsItemClear_Click(object sender, EventArgs e)
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


        private void ButtonAttach_Click(object sender, EventArgs e) => this.MenuCommandsItemAttach_Click(sender, e);        

        private void ButtonSend_Click(object sender, EventArgs e) => this.MenuCommandsItemSend_Click(sender, e);

        private void ButtonClear_Click(object sender, EventArgs e) => this.MenuCommandsItemClear_Click(sender, e);

        #endregion MenuCommands MenuSend MenuAttach MenuRefresh MenuClear incl. Buttons


        #region OnClientReceive OnDragNDrop TooglePeerServer OnDragNDrop delegate jump back invocation target members

        /// <summary>
        /// OnClientReceive event is fired, 
        /// when another secure chat client connects directly peer 2 peer 
        /// to server socket of our local chat app,
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        internal void OnClientReceive(object sender, Area23EventArgs<ReceiveData> eventReceived)
        {

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
                ; // todo launch blocking entering window with input secret key

            if (PeerSessionTriState != PeerSession3State.Peer2Peer)
                TooglePeerSessionServerTriState(0, false);

            if (sender != null)
            {
                if (ipSockListener?.BufferedData != null && ipSockListener.BufferedData.Length > 0)
                {
                    if (chat == null)
                        chat = new Chat(0);
                    string encrypted = EnDeCodeHelper.GetString(ipSockListener.BufferedData);

                    Area23EventArgs<ReceiveData>? area23EvArgs = null;
                    if (eventReceived != null && eventReceived is Area23EventArgs<ReceiveData>)
                    {
                        area23EvArgs = ((Area23EventArgs<ReceiveData>)eventReceived);
                        //TODO: Enable cross thread via delegate
                        SetStatusText(StripStatusLabel, "Connection from " + area23EvArgs.GenericTData.ClientIPAddr + ":" + area23EvArgs.GenericTData.ClientIPPort);

                        string comboText = GetComboBoxText(ComboBoxIp);
                        if (!comboText.Equals(area23EvArgs.GenericTData.ClientIPAddr, StringComparison.CurrentCulture))
                        {
                            PlaySoundFromResource("sound_breakpoint");
                            if (IPAddress.TryParse(area23EvArgs.GenericTData.ClientIPAddr, out partnerIpAddress))
                            {
                                SetComboBoxText(ComboBoxIp, area23EvArgs.GenericTData.ClientIPAddr);
                                AddIpToFriendList(sender, new EventArgs());
                            }
                        }
                        if (ipSockListener.BufferedData.Length >= area23EvArgs.GenericTData.BufferedData.Length)
                            encrypted = EnDeCodeHelper.GetString(ipSockListener.BufferedData);
                        else
                            encrypted = EnDeCodeHelper.GetString(area23EvArgs.GenericTData.BufferedData);

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
                    if (msgContent.IsCqrFile())
                    {
                        CqrFile? cqrFile = msgContent.ToCqrFile();
                        if (cqrFile != null)
                        {
                            SetAttachmentTextLink(cqrFile);
                            friendMsg = cqrFile.GetFileNameContentLength() + Environment.NewLine;
                            PlaySoundFromResource("sound_wind");
                        }
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


        public void TooglePeerSessionServerTriState(short svalue, bool fireUp = true)
        {
            switch (svalue)
            {
                case 0:
                    PeerSessionTriState = PeerSession3State.Peer2Peer;
                    SetComboBoxText(ComboBoxContacts, Constants.ENTER_CONTACT);
                    try
                    {
                        this.ComboBoxContacts.Enabled = false;
                        this.ComboBoxIp.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                    }
                    this.MenuOptionsItemServerSession.Checked = false;
                    this.MenuOptionsItemPeer2Peer.Checked = true;
                    break;
                case 2:
                    this.PeerSessionTriState = PeerSession3State.ChatServer;
                    SetComboBoxText(ComboBoxIp, Constants.ENTER_IP);
                    try
                    {
                        this.ComboBoxContacts.Enabled = true;
                        this.ComboBoxIp.Enabled = false;
                    }
                    catch (Exception ex)
                    {
                    }
                    this.MenuOptionsItemServerSession.Checked = true;
                    this.MenuOptionsItemPeer2Peer.Checked = false;
                    break;
                case 1:
                default:
                    this.PeerSessionTriState = PeerSession3State.None;
                    try
                    {
                        this.ComboBoxContacts.Enabled = false;
                        this.ComboBoxIp.Enabled = false;
                    }
                    catch (Exception ex)
                    {
                    }
                    this.MenuOptionsItemServerSession.Checked = false;
                    this.MenuOptionsItemPeer2Peer.Checked = false;
                    break;
            }
            this.PeerServerSwitch.SetPeerServerSessionTriState(PeerSessionTriState, fireUp);
        }

        public void TooglePeerServer(object sender, EventArgs e)
        {
            if (e is Area23EventArgs<int> ev)
            {
                TooglePeerSessionServerTriState((short)ev.GenericTData);
                //if (ev.GenericTData < 1)
                //{

                //    SetComboBoxText(ComboBoxContacts, Constants.ENTER_CONTACT);
                //    try
                //    {
                //        this.ComboBoxContacts.Enabled = false;
                //        this.ComboBoxIp.Enabled = true;
                //    }
                //    catch (Exception ex)
                //    {
                //    }

                //}
                //else if (ev.GenericTData > 1)
                //{
                //    SetComboBoxText(ComboBoxIp, Constants.ENTER_IP);
                //    try
                //    {
                //        this.ComboBoxContacts.Enabled = true;
                //        this.ComboBoxIp.Enabled = false;
                //    }
                //    catch (Exception ex)
                //    {
                //    }

                //}
                //else
                //{
                //    try
                //    {
                //        this.ComboBoxContacts.Enabled = true;
                //        this.ComboBoxIp.Enabled = true;
                //    }
                //    catch (Exception ex)
                //    {
                //    }
                //}
            }
        }

        /// <summary>
        /// OnDragDrop is fired up, when files are dragged into <see cref="GroupBoxes.LinkLabelsBox"/> or <see cref="GroupBoxes.DragNDropBox"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDragNDrop(object sender, EventArgs e)
        {
            if (e is Area23EventArgs<string> ea)
            {
                if (chat == null)
                    chat = new Chat(0);
                if (ea.GenericTData != null && File.Exists(ea.GenericTData))
                {
                    FileInfo fi = new FileInfo(ea.GenericTData);
                    if (fi.Length > Constants.MAX_FILE_BYTE_BUFFEER)
                    {
                        MessageBox.Show($"File size of {fi.Name} is {fi.Length} and exeeds {Constants.MAX_FILE_BYTE_BUFFEER} bytes.", "FileSize larger > 6MB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (this.PeerSessionTriState == PeerSession3State.Peer2Peer)
                    {
                        string t = GetComboBoxText(this.ComboBoxIp);
                        if (!string.IsNullOrEmpty(t) && IPAddress.TryParse(t, out IPAddress pi))
                        {
                            CqrFile? cf = SendCqrFile(ea.GenericTData, myServerKey, pi);
                            if (cf != null && cf.Data != null && !string.IsNullOrEmpty(cf.CqrFileName))
                            {
                                string userMsg = chat.AddMyMessage(cf.GetFileNameContentLength());
                                AppendText(TextBoxSource, userMsg);
                                Format_Lines_RichTextBox();
                                this.RichTextBoxChat.Text = string.Empty;
                                SetStatusText(StripStatusLabel, $"File {cf.CqrFileName} send successfully!");
                            }
                        }
                    }
                    else if (this.PeerSessionTriState == PeerSession3State.ChatServer)
                    {

                        string chatRoomNr = textBoxChatSession.Text ?? Entities.Settings.Singleton.MyContact.ChatRoomId;
                        if (string.IsNullOrEmpty(textBoxChatSession.Text))
                            textBoxChatSession.Text = chatRoomNr;

                        if (string.IsNullOrEmpty(textBoxChatSession.Text))
                        {
                            InputDialog dialog = new InputDialog("ChatRoomNr required", "Please enter a valid chat room number or register a new chatroom.", MessageBoxIcon.Warning);
                            dialog.ShowDialog();
                            chatRoomNr = (AppDomain.CurrentDomain.GetData("InputDialog") != null) ? ((string)AppDomain.CurrentDomain.GetData("InputDialog")) : string.Empty;
                            textBoxChatSession.Text = (!string.IsNullOrEmpty(chatRoomNr)) ? chatRoomNr : textBoxChatSession.Text;
                        }

                        CqrContact myContact = new CqrContact(Settings.Singleton.MyContact, chatRoomNr, TextBoxPipe.Text);
                        CqrContact? friendContact = null;
                        foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
                        {
                            if (c.NameEmail.Equals(contactNameEmail, StringComparison.InvariantCultureIgnoreCase))
                            {
                                friendContact = new CqrContact(c, chatRoomNr, TextBoxPipe.Text);
                                break;
                            }
                        }

                        SrvMsg serverMessage = new SrvMsg(myContact, friendContact, CqrXsEuSrvKey, myServerKey);
                        this.TextBoxPipe.Text = serverMessage.PipeString;
                        this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
                        myContact._hash = GetHash();
                        myContact.ChatRoomId = chatRoomNr;
                        friendContact._hash = GetHash();
                        friendContact.ChatRoomId = chatRoomNr;
                        serverMessage = new SrvMsg(myContact, friendContact, CqrXsEuSrvKey, myServerKey);

                        string filename = ea.GenericTData;
                        FullSrvMsg<string> fmsg = new FullSrvMsg<string>(myContact, friendContact, chatRoomNr, serverMessage.PipeString, chatRoomNr);
                        // FullSrvMsg<string> cmsg = new FullSrvMsg<string>(myContact, friendContact, unencrypted, serverMessage.ClientPipeString, chatRoomNr);
                        // ClientSrvMsg<string, string> ccmsg = new ClientSrvMsg<string, string>(fmsg, cmsg, chatRoomNr, unencrypted);
                        // string encrypted[] = serverMessage.CqrSrvMsg(fmsg, cmsg, EncodingType.Base64);
                        Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                        string md5 = Area23.At.Framework.Core.Crypt.Hash.MD5Sum.Hash(filename, true);
                        string sha256 = Area23.At.Framework.Core.Crypt.Hash.Sha256Sum.Hash(filename, true);

                        byte[] fileBytes = File.ReadAllBytes(filename);
                        string fileNameOnly = Path.GetFileName(filename);

                        string mimeType = MimeType.GetMimeType(fileBytes, fileNameOnly);

                        CqrFile cfile = new CqrFile(fileNameOnly, mimeType, fileBytes, pmsg.PipeString, md5, sha256);
                        string encrypted = pmsg.CqrFile(cfile, MsgEnum.Json, EncodingType.Base64);
                        FullSrvMsg<string> rfmsg = serverMessage.SendChatMsg_Soap_Simple<string>(fmsg, encrypted, ServerIpAddress, EncodingType.Base64);
                        if (rfmsg != null && rfmsg.Sender != null)
                        {
                            Settings.Singleton.MyContact.LastPolled = rfmsg.Sender.LastPolled;
                            Settings.Singleton.MyContact.LastPushed = rfmsg.Sender.LastPushed;
                            Settings.Singleton.MyContact.ChatRoomId = rfmsg.Sender.ChatRoomId;

                            Settings.SaveSettings(Settings.Singleton);
                        }

                        // string msgChatRoom = "ChatRoomNr: " + rfmsg.ChatRoomNr + "\n" + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
                        // this.TextBoxDestionation.Text = msgChatRoom;
                        string userMsg = chat.AddMyMessage(cfile.GetFileNameContentLength());
                        AppendText(TextBoxSource, userMsg);
                        Format_Lines_RichTextBox();
                        this.RichTextBoxChat.Text = string.Empty;
                        PlaySoundFromResource("sound_push");
                        SetStatusText(StripStatusLabel, $"File {cfile.CqrFileName} send to chatroom number {chatRoomNr} successfully!");

                    }
                }
            }
        }

        /// <summary>
        /// SetAttachmentTextLink saves attachment in attachment folder and adds link in <see cref="AttachmentListControl"/>
        /// </summary>
        /// <param name="cqrFile"><see cref="CqrFile"/></param>
        /// <summary>
        /// SetAttachmentTextLink saves attachment in attachment folder and adds link in <see cref="AttachmentListControl"/>
        /// </summary>
        /// <param name="mimeAttachment"><see cref="MimeAttachment"/></param>
        protected internal void SetAttachmentTextLink(CqrFile cqrFile)
        {
            string fileName = cqrFile.CqrFileName;
            string mimeFilePath = Path.Combine(LibPaths.AttachmentFilesDir, cqrFile.CqrFileName + Constants.HTML_EXT);
            string filePath = Path.Combine(LibPaths.AttachmentFilesDir, cqrFile.CqrFileName);

            byte[] attachBytes = EnDeCodeHelper.GetBytes(cqrFile.GetWebPage());
            System.IO.File.WriteAllBytes(mimeFilePath, attachBytes);

            System.IO.File.WriteAllBytes(filePath, cqrFile.Data);

            LinkedLabelsBox.SetNameFilePath(fileName, filePath);
        }


        public override async Task BgWorkerMonitor_WorkMonitorAsync(object? sender, EventArgs e)
        {
            await base.BgWorkerMonitor_WorkMonitorAsync(sender, e);

            IPAddress? newAddr = null;
            ToolStripMenuItem? newIpIem = null;
            List<IPAddress> addresses = GetProxiesFromSettingsResources();
            InterfaceIpAddresses = await NetworkAddresses.GetIpAddressesAsync();            
            ConnectedIpAddresses = await NetworkAddresses.GetConnectedIpAddressesAsync(addresses);            

            if (PeerSessionTriState == PeerSession3State.Peer2Peer)
            {
                if (ipSockListener != null && ipSockListener.ServerSocket != null &&
                   (ipSockListener.ServerSocket.Connected || ipSockListener.ServerSocket.IsBound) &&
                   !ipSockListener.ServerSocket.Blocking)
                {
                    if (ipSockListener.ServerEndPoint != null)
                        Area23Log.LogStatic($"ipSockListener enpoint peforming normal: {ipSockListener.ServerEndPoint.ToString()}");
                }
                else // Rebind Server Socket
                {

                    foreach (ToolStripMenuItem tsmItem in MenuNetworkItemMyIps.DropDown.Items)
                    {
                        if (tsmItem.Checked)
                        {
                            foreach (IPAddress addr in InterfaceIpAddresses)
                            {
                                if (tsmItem.Text == addr.AddressFamily.ShortInfo() + addr.ToString())
                                {
                                    newIpIem = tsmItem;
                                    newAddr = addr;
                                    break;
                                }
                            }

                        }
                    }

                    if (newIpIem != null && newAddr != null)
                    {
                        ToolStripMenuItem? oldAddrIf = null;

                        foreach (ToolStripMenuItem dditem in this.MenuNetworkItemMyIps.DropDownItems)
                            if (dditem.Checked)
                                oldAddrIf = dditem;

                        IPAddress? clIp = clientIpAddress;
                        try
                        {
                            if (IPAddress.TryParse(newAddr.ToString(), out clIp))
                            {
                                newIpIem.Checked = true;
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
                                    SLog.Log(exi);
                                }
                                try
                                {
                                    ipSockListener = null;
                                }
                                catch (Exception exi)
                                {
                                    SLog.Log(exi);
                                }

                                Thread.Sleep(Constants.CLOSING_TIMEOUT);
                                clientSocket_DataReceived =
                                            delegate (object sender, Area23EventArgs<ReceiveData> eventReceived)
                                            {
                                                OnClientReceive(sender, eventReceived);
                                            };
                                receivedDataEventHandler = new EventHandler<Area23EventArgs<ReceiveData>>(clientSocket_DataReceived);
                                ipSockListener = new Area23.At.Framework.Core.Net.IpSocket.Listener(clientIpAddress, receivedDataEventHandler);
                                SetStatusText(StripStatusLabel, $"Listening on  {clientIpAddress.AddressFamily.ShortInfo()} {clientIpAddress.ToString()}:{Constants.CHAT_PORT}");

                                if (IPAddress.IsLoopback(clientIpAddress))
                                {
                                    if (clientIpAddress.AddressFamily == AddressFamily.InterNetwork)
                                        SetComboBoxText(this.ComboBoxIp, "127.0.0.1");
                                    else if (clientIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                                        if (clientIpAddress.ToString().Contains("::1"))
                                            SetComboBoxText(this.ComboBoxIp, "::1");

                                    //ComboBoxIp_FocusLeave(sender, e);
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            SLog.Log(exc);
                        }
                    }

                }
            }
            
        }


        public override void BgWorkerMonitor_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerMonitor_RunWorkerCompleted(sender, e);
        }

        #endregion OnClientReceive OnDragNDrop TooglePeerServer OnDragNDrop delegate jump back invocation target members


        #region MenuContacts

        private void AddContactsToIpContact()
        {
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
        private void MenuContactsItemMyContact_Click(object sender, EventArgs e)
        {
            ContactSettings contactSettings = new ContactSettings("My Contact Info", 0);
            contactSettings.ShowInTaskbar = true;
            contactSettings.ShowDialog();

            Bitmap? bmp = Properties.fr.Resources.DefaultF45;
            if (Settings.Singleton.MyContact != null && Settings.Singleton.MyContact.ContactImage != null && !string.IsNullOrEmpty(Settings.Singleton.MyContact.ContactImage.ImageBase64))
            {
                try
                {
                    bmp = Settings.Singleton.MyContact.ContactImage.ToDrawingBitmap();
                    if (bmp == null)
                        bmp = Properties.fr.Resources.DefaultF45;
                    else
                        Settings.SaveSettings(Settings.Singleton);
                }
                catch (Exception exBmp)
                {
                    CqrException.SetLastException(exBmp);
                }
                // var badge = new TransparentBadge("My contact added!");
                // badge.ShowDialog();
            }
            this.PictureBoxYou.Image = bmp;
        }

        private void MenuContactsItemAdd_Click(object sender, EventArgs e)
        {
            ContactSettings contactSettings = new ContactSettings("Add Contact Info", 1);
            contactSettings.ShowInTaskbar = true;
            contactSettings.ShowDialog();

            AddContactsToIpContact();
        }


        private void MenuContactsItemView_Click(object sender, EventArgs e)
        {
            ContactsView cview = new ContactsView();
            cview.ShowDialog();
        }

        private void MenuContactstemImport_Click(object sender, EventArgs e)
        {
            int contactId = Entities.Settings.Singleton.Contacts.Count;
            int contactsImported = 0;
            string cname = string.Empty, cemail = string.Empty, cmobile = string.Empty, cphone = string.Empty, caddress = string.Empty;
            string firstImport = string.Empty;
            string lastImport = string.Empty;

            HashSet<string> exCnames = new HashSet<string>();
            HashSet<string> exCemails = new HashSet<string>();
            foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
            {
                if (!string.IsNullOrEmpty(c.Name) && !exCnames.Contains(c.Name))
                    exCnames.Add(c.Name);
                if (!string.IsNullOrEmpty(c.Email) && c.Email.IsEmail() && !exCemails.Contains(c.Email))
                    exCemails.Add(c.Email);
                contactId = Math.Max(contactId, c.ContactId);
            }
            contactId++;

            FileOpenDialog = DialogFileOpen;
            FileOpenDialog.Filter = "CSV (*.csv)|*.csv|VCard (*.vcf)|*.vcf"; //|All files (*.*)|*.*";
            DialogResult result = FileOpenDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                if (File.Exists(FileOpenDialog.FileName))
                {
                    string extension = Path.GetExtension(FileOpenDialog.FileName).ToLower();
                    string[] lines = System.IO.File.ReadAllLines(FileOpenDialog.FileName);


                    switch (extension)
                    {
                        case "csv":
                        case ".csv":

                            int csvCnt = 0;
                            List<int> mailfields = new List<int>();
                            List<int> phonefields = new List<int>();
                            List<int> mobilefields = new List<int>();

                            string[] attributes = lines[0].Split(',');
                            foreach (string attribute in attributes)
                            {
                                if (attribute.ToLower().Contains("e-mail") || attribute.ToLower().Contains("email") || attribute.ToLower().Contains("mail"))
                                    mailfields.Add(csvCnt);
                                if (attribute.ToLower().Contains("phone"))
                                    phonefields.Add(csvCnt);
                                if (attribute.ToLower().Contains("mobil"))
                                    mobilefields.Add(csvCnt);
                                csvCnt++;
                            }

                            for (int i = 1; i < lines.Length; i++)
                            {
                                csvCnt = 0;
                                cname = string.Empty; cemail = string.Empty; cphone = string.Empty; cmobile = string.Empty;
                                string[] fields = lines[i].Split(',');
                                for (int j = 0; j < fields.Length; j++)
                                {
                                    if (j == 0 || j == 2)
                                    {
                                        if (!string.IsNullOrEmpty(fields[j]) && !string.IsNullOrWhiteSpace(fields[j]))
                                            cname += fields[j] + " ";
                                    }
                                    if (j == 3 && !string.IsNullOrWhiteSpace(cname) && cname.EndsWith(' '))
                                        cname = cname.TrimEnd(' ');

                                    if (mailfields.Contains(j) && !string.IsNullOrEmpty(fields[j]) && fields[j].IsEmail())
                                    {
                                        if (string.IsNullOrEmpty(cemail))
                                            cemail = fields[j];
                                    }

                                    if (phonefields.Contains(j) && !string.IsNullOrEmpty(fields[j]) && fields[j].IsPhoneOrMobile())
                                    {
                                        if (string.IsNullOrEmpty(cphone))
                                            cphone = fields[j];
                                    }
                                    if (mobilefields.Contains(j) && !string.IsNullOrEmpty(fields[j]) && fields[j].IsPhoneOrMobile())
                                    {
                                        if (string.IsNullOrEmpty(cmobile))
                                            cmobile = fields[j];
                                    }
                                }
                                cmobile = (string.IsNullOrEmpty(cmobile)) ? cphone : cmobile;
                                if (!string.IsNullOrEmpty(cname) && !exCnames.Contains(cname))
                                {
                                    if (!string.IsNullOrEmpty(cemail) && !exCemails.Contains(cemail))
                                    {
                                        CqrContact contact = new CqrContact()
                                        {
                                            ContactId = contactId++,
                                            Cuid = Guid.NewGuid(),
                                            Name = cname,
                                            Email = cemail,
                                            Mobile = cmobile
                                        };
                                        Entities.Settings.Singleton.Contacts.Add(contact);
                                        if (string.IsNullOrEmpty(firstImport) && contactsImported == 0)
                                            firstImport = contact.NameEmail;
                                        else if (contactsImported > 0)
                                            lastImport = contact.NameEmail;
                                        contactsImported++;
                                    }
                                }

                            }

                            Entities.Settings.SaveSettings(Entities.Settings.Singleton);
                            break;
                        case "vcf":
                        case ".vcf":

                            int vcfCnt = 0;
                            bool beginEndVcard = false;


                            for (int i = 0; i < lines.Length; i++)
                            {

                                if (lines[i].ToUpper().StartsWith("BEGIN:VCARD"))
                                {
                                    beginEndVcard = true;
                                    cname = string.Empty; cemail = string.Empty; cphone = string.Empty; cmobile = string.Empty; caddress = string.Empty;
                                }


                                if (beginEndVcard)
                                {
                                    string tmpString = string.Empty;
                                    if (lines[i].ToUpper().StartsWith("FN:"))
                                    {
                                        tmpString = lines[i].Substring(3);
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3)
                                            cname = tmpString;
                                    }
                                    if (lines[i].ToUpper().StartsWith("N:") && string.IsNullOrEmpty(cname))
                                    {
                                        tmpString = lines[i].Substring(2).Replace(";", " ").TrimEnd(' ');
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3)
                                            cname = tmpString;
                                    }
                                    if (lines[i].ToUpper().Contains("EMAIL") && lines[i].Contains("@") && string.IsNullOrEmpty(cemail))
                                    {
                                        tmpString = lines[i].Substring(lines[i].LastIndexOf(':')).Trim(':');
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3 && tmpString.IsEmail())
                                            cemail = tmpString;
                                    }

                                    if (lines[i].ToUpper().Contains("TEL") && lines[i].Contains("CELL") && string.IsNullOrEmpty(cmobile))
                                    {
                                        tmpString = lines[i].Substring(lines[i].LastIndexOf(':')).Trim(':');
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3 && tmpString.IsPhoneOrMobile())
                                            cmobile = tmpString;
                                    }
                                    if (lines[i].ToUpper().Contains("TEL") && string.IsNullOrEmpty(cmobile))
                                    {
                                        tmpString = lines[i].Substring(lines[i].LastIndexOf(':')).Trim(':');
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3 && tmpString.IsPhoneOrMobile())
                                            cmobile = tmpString;
                                    }
                                    if (lines[i].ToUpper().Contains("ADR") && string.IsNullOrEmpty(caddress))
                                    {
                                        tmpString = lines[i].Substring(lines[i].IndexOf(':')).Trim(':').Replace(";;;", " ").Replace(";;", " ").Replace(";", " ");
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3)
                                            caddress = tmpString;
                                    }

                                    // TODO Photo add


                                }


                                if (lines[i].ToUpper().StartsWith("END:VCARD"))
                                {
                                    vcfCnt++;
                                    beginEndVcard = false;
                                    if (!string.IsNullOrEmpty(cname) && !exCnames.Contains(cname))
                                    {
                                        if (!string.IsNullOrEmpty(cemail))
                                        {
                                            CqrContact contact = new CqrContact() { ContactId = contactId++, Cuid = Guid.NewGuid(), Name = cname, Email = cemail, Mobile = cmobile };
                                            Entities.Settings.Singleton.Contacts.Add(contact);
                                            if (string.IsNullOrEmpty(firstImport) && contactsImported == 0)
                                                firstImport = contact.NameEmail;
                                            else if (contactsImported > 0)
                                                lastImport = contact.NameEmail;
                                            contactsImported++;
                                        }
                                    }
                                }


                            }

                            Entities.Settings.SaveSettings(Entities.Settings.Singleton);

                            break;
                        default:
                            break;

                    }

                    AddContactsByRefComboBox(ref this.ComboBoxContacts);
                    string importedMsg = $"{contactsImported} new contacts imported!";
                    if (!string.IsNullOrEmpty(firstImport))
                        importedMsg += $"\nFirst: {firstImport}";
                    if (!string.IsNullOrEmpty(lastImport))
                        importedMsg += $"\n Last: {lastImport}";
                    MessageBox.Show(importedMsg, $"Contacts import finished", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
            }
        }

        protected internal virtual void AddContactsByRefComboBox(ref System.Windows.Forms.ComboBox contactCombo)
        {
            string ipContact = (contactCombo != null) ? (GetComboBoxText(contactCombo) ?? string.Empty) : string.Empty;
            var items = GetComboBoxItems(contactCombo);
            if (items != null)
                items.Clear();

            foreach (CqrContact ct in Entities.Settings.Singleton.Contacts)
            {
                if (ct != null && !string.IsNullOrEmpty(ct.NameEmail))
                    AddItemToComboBox(contactCombo, ct.NameEmail);
            }
            SetComboBoxText(contactCombo, ipContact);
        }

        #endregion MenuContacts


        #region SplitChatWindowLayout

        /// <summary>
        /// MenuView_ItemTopBottom_Click occures, when user clicks on Top-Bottom in chat app
        /// shows top bottom view of chat líke ancient talk/talks
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void MenuView_ItemTopBottom_Click(object sender, EventArgs e)
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
        private void MenuView_ItemLeftRíght_Click(object sender, EventArgs e)
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
        private void MenuView_Item1View_Click(object sender, EventArgs e)
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

        internal async Task SetupNetwork()
        {
            List<IPAddress> addresses = GetProxiesFromSettingsResources();
            SetStatusText(StripStatusLabel, $"Setup Network: Several proxy addresses fetched.");

            List<IPAddress> myIpList = await SetupAllNetworkInterfacesAndConnectedIpAddresses(addresses);

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
                SLog.Log(exV6);
            }

            // this.MenuItemExternalIp.DropDownItems.Add(extIpItem);

            SetStatusText(StripStatusLabel, $"Setup Network: External client ip address added to menu.");


            int mchecked = 0;
            List<string> proxyList = new List<string>();
            foreach (IPAddress addrProxy in addresses)
            {
                if (addrProxy != null &&
                    ((addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ||
                    (addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)))
                {
                    proxyList.Add(addrProxy.ToString());


                    ToolStripMenuItem item = new ToolStripMenuItem(addrProxy.AddressFamily.ShortInfo() + addrProxy.ToString(), null, ServerProxyAddressSelected, addrProxy.ToString());
                    if ((addrProxy.AddressFamily == ServerIpAddress?.AddressFamily) &&
                        (Extensions.BytesCompare(addrProxy.GetAddressBytes(), ServerIpAddress.GetAddressBytes()) == 0))
                    {

                        if (!GetMenuItemChecked(MenuNetworkItemIPv6Secure) && addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {; }
                        else
                            SetMenuItemChecked(item, true);
                        // item.Checked = true;
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
                        SLog.Log("Error when adding friendIps + " + exFriendIp.Message);
                    }
                }
            }

            if (Entities.Settings.Singleton != null)
            {
                Entities.Settings.Singleton.Proxies = proxyList;
                Entities.Settings.Singleton.MyIPs = myIpList.ConvertAll(x => x.ToString()).ToList();
                Entities.Settings.SaveSettings(Entities.Settings.Singleton);
            }

            SetStatusText(StripStatusLabel, $"Setup Network complete!");
        }



        internal async Task<List<IPAddress>> SetupAllNetworkInterfacesAndConnectedIpAddresses(List<IPAddress> addresses)
        {
            InterfaceIpAddresses = await NetworkAddresses.GetIpAddressesAsync();
            SetStatusText(StripStatusLabel, $"Setup Network: All network interfaces addresses fetched.");
            ConnectedIpAddresses = await NetworkAddresses.GetConnectedIpAddressesAsync(addresses);
            SetStatusText(StripStatusLabel, $"Setup Network: All active connected ip addresses fetched.");

            MenuNetworkItemMyIps.DropDown.Items.Clear();
            MenuItemExternalIp = new ToolStripMenuItem();
            MenuItemExternalIp.BackColor = SystemColors.MenuBar;
            MenuItemExternalIp.Name = "MenuItemExternalIp";
            MenuItemExternalIp.Size = new Size(160, 22);
            MenuItemExternalIp.Text = "External Ip's";
            MenuItemExternalIp.Visible = true;            
            MenuNetworkItemMyIps.BackColor = SystemColors.MenuBar;
            MenuNetworkItemMyIps.DropDownItems.AddRange(new ToolStripItem[] { MenuItemExternalIp });
            MenuNetworkItemMyIps.Name = "MenuNetworkItemMyIps";
            MenuNetworkItemMyIps.Size = new Size(177, 22);
            MenuNetworkItemMyIps.Text = "my ip's";
            

            List<IPAddress> myIpList = new List<IPAddress>();
            int mchecked = 0;
            this.MenuNetworkItemIPv6Secure.Checked = false;
            foreach (IPAddress addr in InterfaceIpAddresses)
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

                    if (ConnectedIpAddresses != null && ConnectedIpAddresses.Count > 0)
                    {
                        foreach (IPAddress connectedIp in ConnectedIpAddresses)
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
                                    clientSocket_DataReceived =
                                        delegate (object sender, Area23EventArgs<ReceiveData> eventReceived)
                                        {
                                            OnClientReceive(sender, eventReceived);
                                        };
                                    receivedDataEventHandler = new EventHandler<Area23EventArgs<ReceiveData>>(clientSocket_DataReceived);
                                    ipSockListener = new Area23.At.Framework.Core.Net.IpSocket.Listener(clientIpAddress, receivedDataEventHandler);
                                }

                                break;
                            }
                        }

                    }

                    myIpList.Add(addr);
                    AddMenuItemToItems(MenuNetworkItemMyIps, (ToolStripDropDownItem)item);
                    // this.MenuNetworkItemMyIps.DropDownItems.Add(item);
                }
            }

            SetStatusText(StripStatusLabel, $"Setup Network: All interface addresses added to menu. Not connected if addrs grayed.");

            return myIpList;

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

                IPAddress? clIp = clientIpAddress;
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
                            SLog.Log(exi);
                        }
                        try
                        {
                            ipSockListener = null;
                        }
                        catch (Exception exi)
                        {
                            SLog.Log(exi);
                        }

                        Thread.Sleep(Constants.CLOSING_TIMEOUT);
                        clientSocket_DataReceived =
                                        delegate (object sender, Area23EventArgs<ReceiveData> eventReceived)
                                        {
                                            OnClientReceive(sender, eventReceived);
                                        };
                        receivedDataEventHandler = new EventHandler<Area23EventArgs<ReceiveData>>(clientSocket_DataReceived);
                        ipSockListener = new Area23.At.Framework.Core.Net.IpSocket.Listener(clientIpAddress, receivedDataEventHandler);
                        SetStatusText(StripStatusLabel, $"Listening on  {clientIpAddress.AddressFamily.ShortInfo()} {clientIpAddress.ToString()}:{Constants.CHAT_PORT}");

                        if (IPAddress.IsLoopback(clientIpAddress))
                        {
                            if (clientIpAddress.AddressFamily == AddressFamily.InterNetwork)
                                SetComboBoxText(this.ComboBoxIp, "127.0.0.1");
                            else if (clientIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                                if (clientIpAddress.ToString().Contains("::1"))
                                    SetComboBoxText(this.ComboBoxIp, "::1");

                            ComboBoxIp_FocusLeave(sender, e);
                        }
                    }
                }
                catch (Exception exc)
                {
                    SLog.Log(exc);
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

                if (newProxyItem != null && !string.IsNullOrEmpty(newProxyItem.Name))
                {
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
                        SLog.Log(exi);
                    }

                    Thread.Sleep(Constants.CLOSING_TIMEOUT);

                }
            }
        }

        #region MenuOptions

        public void MenuOptionsItemClearAllOnClose_Click(object sender, EventArgs e)
        {
            // TODO add to settings
            this.MenuOptionsItemClearAllOnClose.Checked = (!this.MenuOptionsItemClearAllOnClose.Checked);
            AppDomain.CurrentDomain.SetData(Constants.CQRXS_DELETE_DATA_ON_CLOSE, MenuOptionsItemClearAllOnClose.Checked);
        }


        private void MenuOptionsItemPeer2Peer_Click(object sender, EventArgs e)
        {
            if (this.MenuOptionsItemPeer2Peer.Checked && !this.MenuOptionsItemServerSession.Checked)
                TooglePeerSessionServerTriState(0);
            else if (!this.MenuOptionsItemPeer2Peer.Checked && this.MenuOptionsItemServerSession.Checked)
                TooglePeerSessionServerTriState(2);
            else
                TooglePeerSessionServerTriState(1);

        }

        private void MenuOptionsItemServerSession_Click(object sender, EventArgs e)
        {
            if (this.MenuOptionsItemPeer2Peer.Checked && !this.MenuOptionsItemServerSession.Checked)
                TooglePeerSessionServerTriState(0);
            else if (!this.MenuOptionsItemPeer2Peer.Checked && this.MenuOptionsItemServerSession.Checked)
                TooglePeerSessionServerTriState(2);
            else
                TooglePeerSessionServerTriState(1);
        }

        #endregion MenuOptions

        #region LoadSaveChatContent

        private void MenuFileItemOpen_Click(object sender, EventArgs e)
        {
            FileOpenDialog = DialogFileOpen;
            FileOpenDialog.RestoreDirectory = true;
            DialogResult result = FileOpenDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                MessageBox.Show($"FileName: {FileOpenDialog.FileName} init directory: {FileOpenDialog.InitialDirectory}", $"{Text} type {FileOpenDialog.GetType()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        protected internal virtual void toolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            SafeFileName();
        }

        protected virtual byte[] OpenCryptFileDialog(ref string loadDir)
        {
            if (FileOpenDialog == null)
                FileOpenDialog = new OpenFileDialog();
            byte[] fileBytes;
            if (string.IsNullOrEmpty(loadDir))
                loadDir = Environment.GetEnvironmentVariable("TEMP") ?? System.AppDomain.CurrentDomain.BaseDirectory;
            if (loadDir != null)
            {
                FileOpenDialog.InitialDirectory = loadDir;
                FileOpenDialog.RestoreDirectory = true;
            }
            DialogResult diaOpenRes = FileOpenDialog.ShowDialog();
            if (diaOpenRes == DialogResult.OK || diaOpenRes == DialogResult.Yes)
            {
                if (!string.IsNullOrEmpty(FileOpenDialog.FileName) && File.Exists(FileOpenDialog.FileName))
                {
                    loadDir = Path.GetDirectoryName(FileOpenDialog.FileName) ?? System.AppDomain.CurrentDomain.BaseDirectory;
                    fileBytes = File.ReadAllBytes(FileOpenDialog.FileName);
                    return fileBytes;
                }
            }

            fileBytes = new byte[0];
            return fileBytes;
        }

        protected virtual string SafeFileName(string? filePath = "", byte[]? content = null)
        {
            string? saveDir = Environment.GetEnvironmentVariable("TEMP");
            string ext = ".hex";
            string fileName = DateTime.Now.Area23DateTimeWithSeconds() + ext;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileName = System.IO.Path.GetFileName(filePath);
                saveDir = System.IO.Path.GetDirectoryName(filePath);
                ext = System.IO.Path.GetExtension(filePath);
            }

            if (saveDir != null)
            {
                FileSaveDialog.InitialDirectory = saveDir;
                FileSaveDialog.RestoreDirectory = true;
                FileSaveDialog.DefaultExt = ext;
            }
            FileSaveDialog.FileName = fileName;
            DialogResult diaRes = FileSaveDialog.ShowDialog();
            if (diaRes == DialogResult.OK || diaRes == DialogResult.Yes)
            {
                if (content != null && content.Length > 0)
                    System.IO.File.WriteAllBytes(FileSaveDialog.FileName, content);

                // var badge = new TransparentBadge($"File {fileName} saved to directory {saveDir}.");
                // badge.Show();
            }

            return (FileSaveDialog != null && FileSaveDialog.FileName != null && File.Exists(FileSaveDialog.FileName)) ? FileSaveDialog.FileName : null;
        }

        #endregion LoadSaveChatContent


        protected string GetHash()
        {
            if (!string.IsNullOrEmpty(this.TextBoxPipe.Text))
                return this.TextBoxPipe.Text;
            if (!string.IsNullOrEmpty(this.toolStripTextBoxCqrPipe.Text))
                return this.toolStripTextBoxCqrPipe.Text;

            Peer2PeerMsg peerMsg = new Peer2PeerMsg(this.ComboBoxSecretKey.Text);
            return peerMsg.PipeString;
        }


    }

}
