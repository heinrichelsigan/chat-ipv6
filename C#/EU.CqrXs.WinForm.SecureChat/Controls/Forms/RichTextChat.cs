using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
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
using System.Net;
using System.Net.Sockets;
using CqrContact = Area23.At.Framework.Core.Cqr.Msg.CContact;


namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{


    /// <summary>
    /// SecureChat main form
    /// </summary>
    public partial class RichTextChat : BaseChatForm
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

                List<IPAddress> list;
                try
                {
                    list = DnsHelper.GetIpAddrsByHostName(Constants.CQRXS_EU);
                }
                catch (Exception exDns)
                {
                    string srvIp = Properties.Resources.Proxies.Split(';')[0];
                    if (IPAddress.TryParse(srvIp, out _serverIpAddress))
                    {
                        return _serverIpAddress;
                    }
                    
                    Area23Log.LogOriginMsgEx("RichTextChat", "Exception on getting server ip address via dns", exDns);
                    throw;
                }
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
        /// Parameterless constructor of <see cref="RichTextChat">RichTextChat  Windows Form</see> (extends <see cref="BaseChatForm"/>)
        /// 1. calls InitializeComponents in RichTextChat.designer.cs file
        /// 2. sets up constant definitions / limitations
        /// 3. initializes async event handlers for several interactive gui elements 
        /// 4. initializes network client socket <see cref="ClientSocket_DataReceived"/> callback in instance variable <see cref="clientSocket_DataReceived"/>
        ///    initializes <see cref="EventHandler{Area23EventArgs{ReceiveData}}  instance variable <see cref="receivedDataEventHandler"/> with delegate variable <see cref="clientSocket_DataReceived"/>
        /// 5. Creates (if they not already exist) sub directories for attached files via <see cref="MiniToolBox.CreateAttachDirectory()"/>
        /// 6. Initializes synchronous EventHandler for DragNDrop at <see cref="LinkedLabelsBox"/>
        ///    Initializes synchronous EventHandler for change chat mode peer2peer - server session switch bar in <see cref="PeerServerSwitch"/>
        /// 7. Last but not least sets <see cref="StripProgressBar"/> to initial value 0
        /// </summary>
        public RichTextChat() : base()
        {
            InitializeComponent();

            TextBoxSource.MaxLength = Constants.SOCKET_BYTE_BUFFEER;
            TextBoxDestionation.MaxLength = Constants.SOCKET_BYTE_BUFFEER;
            SetComboBoxText(ComboBoxIp, Constants.ENTER_IP);
            SetComboBoxText(ComboBoxContacts, Constants.ENTER_CONTACT);
            SetComboBoxText(ComboBoxSecretKey, Constants.ENTER_SECRET_KEY);

            MenuContactsItemMe.Click += new System.EventHandler(async (sender, e) => await MenuContactsItemMyContact_Click(sender, e)); ;
            MenuCommandsItemSend.Click += new System.EventHandler(async (sender, e) => await MenuCommandsItemSend_Click(sender, e));
            MenuCommandsItemAttach.Click += new System.EventHandler(async (sender, e) => await MenuCommandsItemAttach_Click(sender, e));
            MenuCommandsItemRefresh.Click += new System.EventHandler(async (sender, e) => await MenuCommandsItemRefresh_Click(sender, e));
            MenuCommandsItemClear.Click += new System.EventHandler(async (sender, e) => await MenuCommandsItemClear_Click(sender, e));
            ButtonInviteChatRoom.Click += new System.EventHandler(async (sender, e) => await ButtonInviteChatRoom_Click(sender, e));
            buttonVisitChatRoom.Click += new System.EventHandler(async (sender, e) => await ButtonVisitChatRoom_Click(sender, e));
            ButtonSend.Click += new System.EventHandler(async (sender, e) => await ButtonSend_Click(sender, e));
            ButtonAttach.Click += new System.EventHandler(async (sender, e) => await ButtonAttach_Click(sender, e));

            clientSocket_DataReceived = delegate (object sender, Area23EventArgs<ReceiveData> eventReceived) { OnClientReceive(sender, eventReceived); };
            receivedDataEventHandler = new EventHandler<Area23EventArgs<ReceiveData>>(clientSocket_DataReceived);

            MiniToolBox.CreateAttachDirectory();
            this.LinkedLabelsBox.OnDragNDrop += OnDragNDrop;
            this.DragnDropBoxFiles.OnDragNDrop += OnDragNDrop;
            this.PeerServerSwitch.FireUpChanged += TooglePeerServer;
            this.SetProgressBar(this.StripProgressBar, 0);
        }

        /// <summary>
        /// async RichTextChat_Load performs init at startup
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private async void RichTextChat_Load(object sender, EventArgs e)
        {
            bool? appFirstReg = MemoryCache.CacheDict.GetValue<bool>(Constants.APP_FIRST_REG);
            bool send1stReg = (appFirstReg.HasValue) ? appFirstReg.Value : false;
            if (Entities.Settings.LoadSettings() == null || Entities.Settings.Singleton == null || Entities.Settings.Singleton.MyContact == null ||
                    string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.NameEmail))
                send1stReg = true;

            await BaseChatForm_Load(sender, e);
            bgWorkerMonitor.RunWorkerAsync();

            this.SetProgressBar(this.StripProgressBar, 10);

            SetStatusText(StripStatusLabel, "Setup Network");

            await PlaySoundFromResourcesAsync("sound_volatage");
            await SetupNetwork();

            int progress = GetProgressBar(StripProgressBar);
            SetProgressBar(StripProgressBar, progress);

            if (send1stReg)
            {
                send1stReg = true;
                MemoryCache.CacheDict.SetValue<bool>(Constants.APP_FIRST_REG, send1stReg);
                // var badge = new TransparentBadge($"Error reading Settings from {LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE}.");
                // badge.Show();
                await MenuContactsItemMyContact_Click(sender, e);

                progress = GetProgressBar(StripProgressBar);
                SetProgressBar(StripProgressBar, progress + 10);
            }

            AddContactsToIpContact();

            Bitmap? bmp = Properties.fr.Resources.DefaultF45;
            if (Entities.Settings.Singleton != null)
            {

                if (Entities.Settings.Singleton.MyContact != null && Entities.Settings.Singleton.MyContact.ContactImage != null &&
                        Settings.Singleton.MyContact.ContactImage.ImageData != null &&
                        Settings.Singleton.MyContact.ContactImage.ImageData.Length > 0)
                {
                    bmp = (Bitmap?)Entities.Settings.Singleton.MyContact.ContactImage.ToDrawingBitmap();
                    if (bmp == null)
                        bmp = Properties.fr.Resources.DefaultF45;
                }
                PictureBoxYou.Image = bmp; // TODO: Thread safe delegate

                MenuOptionsItemClearAllOnClose.Checked = Settings.Singleton.ClearAllOnClose;  // TODO: Thread safe delegate
                if (Settings.Singleton.OnlyPeer2PeerChat)
                {
                    MenuOptionsItemOnlyPeer2PeerChat_Click("ctor", new EventArgs());
                }
                MenuOptionsItemCompress.Checked = Settings.Singleton.ZipBeforeSend; // TODO: Thread safe delegate
                MenuOptionsItemFileTypeSecure.Checked = Settings.Singleton.OnlySecureFileTypes; // TODO: Thread safe delegate
                if (Settings.Singleton.SecretKeys != null && Settings.Singleton.SecretKeys.Count > 0)
                {
                    string secLastKey = "";
                    foreach (string secKey in Settings.Singleton.SecretKeys)
                    {
                        secLastKey = secKey;
                        this.ComboBoxSecretKey.Items.Add(secLastKey); // TODO: Thread safe delegate
                    }
                    // this.ComboBoxSecretKey.Text = string.IsNullOrEmpty(secLastKey) ? "" : secLastKey;
                    // SecretKey_Update(sender, e);
                }

            }

            progress = GetProgressBar(StripProgressBar);
            progress = (progress < 100) ? progress + 5 : 100;
            SetProgressBar(StripProgressBar, progress);

            if (send1stReg && Settings.Singleton.RegisterUser)
            {
                DialogResult regServerResult = MessageBox.Show("Do you want to register?", "Register your account on server?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (regServerResult == DialogResult.Yes)
                    Send_1st_Server_Registration(sender, e);
                else
                {
                    Settings.Singleton.RegisterUser = false;
                    Settings.SaveSettings();
                }
            }
            send1stReg = false;
            MemoryCache.CacheDict.SetValue<bool>(Constants.APP_FIRST_REG, send1stReg);

            SetProgressBar(StripProgressBar, 100);
            SetStatusText(StripStatusLabel, "Secure Chat init done.");

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
        /// Gets hash from either <see cref="TextBoxPipe"/> thread safe or
        /// construct a <see cref="CqrFacade"/> with secret key from <see cref="ComboBoxSecretKey" /> text value (thread safe)
        /// and sets <see cref="CqrFacade.PipeString"/> as text in <see cref="TextBoxPipe"/>  (thread safe)
        /// </summary>
        /// <returns><see cref="CqrFacade.PipeString"/>  as hash for secret key</returns>
        protected string GetHash()
        {

            string comboSecKeyTxt = GetComboBoxText(ComboBoxSecretKey);
            CqrFacade clientFacade = new CqrFacade(comboSecKeyTxt);
            string? pipeText = GetTextBoxText(TextBoxPipe);
            if (!string.IsNullOrEmpty(pipeText))
            {
                if (clientFacade.PipeString.Equals(pipeText))
                    return pipeText;
            }

            pipeText = clientFacade.PipeString;
            SetTextBoxText(TextBoxPipe, clientFacade.PipeString);

            return pipeText;

        }


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
        /// SecretKey Update Event
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void SecretKey_Update(object sender, EventArgs e)
        {
            myServerKey = ExternalIpAddress?.ToString() + Constants.APP_NAME;
            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
                return;

            CqrFacade clientFacade = new CqrFacade(myServerKey);
            SetTextBoxText(TextBoxPipe, clientFacade.PipeString);
            SetStatusText(StripStatusLabel, $"Changed secret key to {myServerKey} => secure pipe: {clientFacade.PipeString}");

            if (!this.ComboBoxSecretKey.Items.Contains(GetComboBoxText(ComboBoxSecretKey)))
                this.ComboBoxSecretKey.Items.Add(GetComboBoxText(ComboBoxSecretKey));

            if (Entities.Settings.Singleton != null)
            {
                if (Settings.Singleton.SecretKeys.Contains(myServerKey))
                    Settings.Singleton.SecretKeys.Remove(myServerKey);
                Settings.Singleton.SecretKeys.Add(myServerKey);
                Settings.SaveSettings();
            }

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

            SecretKey_Update(sender, e);
        }

        /// <summary>
        /// ComboBoxSecretKey_TextUpdate is fired, when text entered in ComboBoxSecretKey changes.
        /// Event is fired, when 1 char will be added or deleted at each change of <see cref="ComboBoxSecretKey.Text"/>
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void ComboBoxSecretKey_TextUpdate(object sender, EventArgs e)
        {
            string secretKey = GetComboBoxText(ComboBoxSecretKey);
            if (string.IsNullOrEmpty(secretKey) ||
                secretKey.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            this.ComboBoxSecretKey.BackColor = Color.White;
            CqrFacade clientFacade = new CqrFacade(secretKey);
            SetTextBoxText(TextBoxPipe, clientFacade.PipeString);
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

            SecretKey_Update(sender, e);
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
                    throw new InvalidOperationException($"IPAddress {GetComboBoxText(ComboBoxIp) ?? string.Empty} is not parsable!");
            }
            catch (Exception exIpContact)
            {
                MessageBox.Show($"Cannot parse IpAddress from string \"{GetComboBoxText(ComboBoxIp)}\": {exIpContact.Message}", "Please enter a valid connectable ipv4 or ipv6 address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            string comboIpTxt = GetComboBoxText(this.ComboBoxIp);
            if (string.IsNullOrEmpty(comboIpTxt) ||
               comboIpTxt.Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a new ip address!", "Please enter a valid connectable ip address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetComboBoxBackColor(ComboBoxIp, Color.PeachPuff);
                PlaySoundFromResource("sound_warning");
                return;
            }
            try
            {
                partnerIpAddress = IPAddress.Parse(comboIpTxt);
            }
            catch (Exception exIpContact)
            {
                MessageBox.Show($"Cannot parse IpAddress from string \"{GetComboBoxText(ComboBoxIp)}\": {exIpContact.Message}", "Please enter a valid connectable ipv4 or ipv6 address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetComboBoxBackColor(ComboBoxIp, Color.Violet);
                PlaySoundFromResource("sound_warning");
                return;
            }
            SetComboBoxBackColor(ComboBoxIp, Color.White);
            SetStatusText(StripStatusLabel, $"Selected partner ip address {partnerIpAddress.ToString()}.");
            SetComboBoxText(this.ComboBoxContacts, Constants.ENTER_CONTACT);

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

            CqrFacade clientFacade = new CqrFacade(GetComboBoxText(this.ComboBoxSecretKey));
            string sessionChatText = GetTextBoxText(this.TextBoxChatSession);

            bool foundContact = false;
            CContact? friendContact = MiniToolBox.FindContactOrCreateByNameEmail(contactNameEmail, sessionChatText, clientFacade.PipeString);
            if (friendContact != null && !string.IsNullOrEmpty(friendContact.NameEmail))
                foundContact = true;

            if (!foundContact)
            {
                SetComboBoxBackColor(ComboBoxContacts, Color.Violet);
                PlaySoundFromResource("sound_warning");
                MessageBox.Show($"Cannot parse Contact from \"ComboBoxContacts.Text\": {contactNameEmail}", "Please enter a valid contact address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetComboBoxText(ComboBoxIp, Constants.ENTER_IP);
            SetComboBoxBackColor(ComboBoxContacts, Color.White);
            SetStatusText(StripStatusLabel, $"{contactNameEmail} selected. Click Invite to request new chatroom.");

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


            CqrFacade serverFacade = new CqrFacade(CqrXsEuSrvKey);
            SetTextBoxText(TextBoxPipe, serverFacade.PipeString);
            // this.TextBoxPipe.Text = srv1stMsg.PipeString;

            Thread.Sleep(32);

            int barProgress = this.GetProgressBar(this.StripProgressBar);
            barProgress = (barProgress <= 90) ? barProgress + 5 : 100;
            this.SetProgressBar(this.StripProgressBar, barProgress);

            CContact myContact = Entities.Settings.Singleton.MyContact;

            CContact? returnContact = serverFacade.SendFirstSrvMsg_Soap(myContact, EncodingType.Base64);

            barProgress = (barProgress <= 95) ? barProgress + 5 : 100;
            this.SetProgressBar(this.StripProgressBar, barProgress);

            string usrMsg = $"Registering contact: {myContact.NameEmail}\n";
            string srvMsg = "";
            SetTextBoxText(TextBoxSource, chat.AddMyMessage(usrMsg));

            if (returnContact != null)
            {
                returnContact.ContactId = 0;
                Settings.Singleton.MyContact = returnContact;
                srvMsg = $"Got Cuid: {returnContact.Cuid} for {returnContact.NameEmail}\n";
                SetTextBoxText(TextBoxDestionation, chat.AddFriendMessage(srvMsg));
                Settings.SaveSettings(Settings.Singleton);
            }

            // this.RichTextBoxOneView.Rtf = this.RichTextBoxChat.Rtf;
            Format_Lines_RichTextBox();
            SetStatusText(StripStatusLabel, "Finished 1st registration");
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

            CqrFacade clientFacade = new CqrFacade(myServerKey);
            string unencrypted = "Init: " + clientIpAddress?.ToString() + " " + Entities.Settings.Singleton.MyContact.NameEmail;
            try
            {
                if (!IPAddress.TryParse(ipAddrString, out partnerIpAddress))
                    throw new InvalidDataException("Cannot parse " + ipAddrString + " to IPAddress!");

                string peerResponse = clientFacade.Send_CContent_Peer(unencrypted, partnerIpAddress, Constants.CHAT_PORT, EncodingType.Base64);

                string userMsg = chat.AddMyMessage(unencrypted);
                AppendText(TextBoxSource, userMsg);
                // Format_Lines_RichTextBox();
                SetRichText(RichTextBoxChat, string.Empty);
                // this.RichTextBoxChat.Text = string.Empty;
                SetStatusText(StripStatusLabel, $"Send init to {partnerIpAddress} successfully");
                ButtonCheck.Image = Properties.de.Resources.RemoteConnect;
            }
            catch (Exception ex)
            {
                Area23Log.LogOriginMsgEx(this.Name, $"Exception in SendInit_Click: {ex.Message}.\n", ex);
                SetStatusText(StripStatusLabel, $"Sending to {ipAddrString} failed: {ex.Message}");
                PlaySoundFromResource("sound_hammer");
                return false;
            }

            return true;
        }

        /// <summary>
        /// SendInvite_ToChatRoom sends an invitation to chat room for a contact
        /// You have to tell your contact the secret key and chat room number
        /// </summary>
        internal async Task<bool> SendInvite_ToChatRoom()
        {
            // TODO: implement it via socket directly or to registered user
            // if Ip is pingable and reachable and connectable
            // send HELLO to IP
            if (chat == null)
                chat = new Chat(0);

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
                return false;


            if (this.PeerSessionTriState == PeerSession3State.Peer2Peer || this.PeerSessionTriState == PeerSession3State.None)
            {
                MessageBox.Show($"You must set chat mode to server session to invite to chat room.", $"SessionTriState is {this.PeerSessionTriState.ToString()}, expected: ChatServer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                return false;

            SetTextBoxText(this.TextBoxSource, "");
            SetTextBoxText(this.TextBoxDestionation, "");
            SetRichText(this.RichTextBoxOneView, "");

            string sessionChatText = (Settings.Singleton.ChatRoom != null && !string.IsNullOrEmpty(Settings.Singleton.ChatRoom.ChatRoomNr)) ?
                Settings.Singleton.ChatRoom.ChatRoomNr : GetTextBoxText(TextBoxChatSession);

            SetTextBoxText(TextBoxChatSession, sessionChatText);

            CqrFacade serverFacade = new CqrFacade(CqrXsEuSrvKey);
            CqrFacade clientFacade = new CqrFacade(myServerKey);
            string pipeText = GetTextBoxText(TextBoxPipe);

            string unencrypted = "Init: " + clientIpAddress?.ToString() + " " + Entities.Settings.Singleton.MyContact.NameEmail;

            if (Settings.Singleton.ChatRoom != null)
            {
                Settings.Singleton.ChatRoom.MsgDict = new Dictionary<long, string>();
                Settings.Singleton.ChatRoom.LastPushed = DateTime.MinValue;
                Settings.Singleton.ChatRoom.LastPolled = DateTime.MinValue;
                Settings.SaveSettings();
            }
            CContact myContact = new CContact(Settings.Singleton.MyContact, sessionChatText, clientFacade.PipeString);
            myContact.Message = sessionChatText;


            CContact? friendContact = MiniToolBox.FindContactOrCreateByNameEmail(contactNameEmail, sessionChatText, clientFacade.PipeString);


            SetTextBoxText(this.TextBoxPipe, clientFacade.PipeString);
            // this.TextBoxPipe.Text = serverMessage.PipeString;
            // this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
            myContact.Hash = GetHash();
            if (friendContact != null)
                friendContact.Hash = GetHash();


            CSrvMsg<string> fmsg = new CSrvMsg<string>(myContact, friendContact ?? myContact, myContact.NameEmail, serverFacade.PipeString);
            string myReqMsg = $"{fmsg.Sender.NameEmail} requests a new chatroom from server\r\n";
            SetTextBoxText(TextBoxSource, chat.AddMyMessage(myReqMsg));


            SetStatusText(StripStatusLabel, myReqMsg);
            // Send chat room invite via WebService
            CSrvMsg<string>? rfmsg = await serverFacade.Send_InitChatRoom_SoapAsync<string>(fmsg, EncodingType.Base64);

            if (rfmsg == null || rfmsg.CRoom == null || string.IsNullOrEmpty(rfmsg.CRoom.ChatRoomNr))
            {
                MessageBox.Show($"Response message form server {ServerIpAddress} is null. Please call helpdesk +436507527928", "Invite Chatroom failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            SetTextBoxText(TextBoxChatSession, rfmsg.CRoom.ChatRoomNr);


            if (rfmsg != null)
            {
                if (rfmsg.Sender != null && !string.IsNullOrEmpty(rfmsg.Sender.NameEmail) &&
                    rfmsg.Sender.NameEmail.Equals(myContact.NameEmail, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (rfmsg.Sender != null && !string.IsNullOrEmpty(rfmsg.Sender.NameEmail) &&
                        rfmsg.Sender.NameEmail.Equals(myContact.NameEmail, StringComparison.CurrentCultureIgnoreCase))
                    {
                        myContact = new CContact(rfmsg.Sender, rfmsg.CRoom.ChatRoomNr, rfmsg.Sender.Hash, myContact.ContactImage);
                        Settings.Singleton.MyContact = myContact;
                    }
                    if (rfmsg.CRoom != null && !string.IsNullOrEmpty(rfmsg.CRoom.ChatRoomNr))
                    {
                        SetTextBoxText(TextBoxChatSession, rfmsg.CRoom.ChatRoomNr);
                        Settings.Singleton.ChatRoom = new CChatRoom(rfmsg.CRoom);
                    }

                    SetStatusText(StripStatusLabel, $"Successfully sended invite request, now saving results from chatroom...");
                    Settings.SaveSettings(Settings.Singleton);
                }
            }

            // TODO: Email zur Einladung
            string msgChatRoom = "Received ChatRoomNr: " + rfmsg.CRoom.ChatRoomNr + " \nfor " + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
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
        internal async Task MenuCommandsItemSend_Click(object sender, EventArgs e)
        {
            // TODO: implement it via socket directly or to registered user
            // if Ip is pingable and reachable and connectable
            // send HELLO to IP
            if (chat == null)
                chat = new Chat(0);

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
            {
                SetStatusText(StripStatusLabel, "Nothing to send!");
                return;
            }

            string unencrypted = GetRichTextBoxText(this.RichTextBoxChat);  // Text; //.Replace("\r\n", "\n").Replace("\n", " " + Environment.NewLine);
            if (string.IsNullOrEmpty(unencrypted) || unencrypted.Trim(" \t\n\r\v".ToCharArray()).Length < 1)
            {
                SetStatusText(StripStatusLabel, "Empty message could not be send!");
                return;
            }

            CqrFacade clientFacade = new CqrFacade(myServerKey);
            try
            {
                if (this.PeerSessionTriState == PeerSession3State.Peer2Peer)
                {

                    if ((ipAddrString = GetComboBoxMustHaveText(ref ComboBoxIp)) == null)
                    {
                        SetStatusText(StripStatusLabel, "Nothing to send (no ip addr).");
                        return;
                    }


                    if (!IPAddress.TryParse(ipAddrString, out partnerIpAddress))
                        throw new InvalidDataException("Cannot parse IPAddress " + ipAddrString);

                    string peerResponse = clientFacade.Send_CContent_Peer(unencrypted, partnerIpAddress, Constants.CHAT_PORT, EncodingType.Base64);


                    string userMsg = chat.AddMyMessage(unencrypted);
                    AppendText(TextBoxSource, userMsg);
                    Format_Lines_RichTextBox();
                    this.RichTextBoxChat.Text = string.Empty;
                    SetStatusText(StripStatusLabel, $"Send to {partnerIpAddress} successfully.");
                    await PlaySoundFromResourcesAsync("sound_arrow");

                }
                else if (this.PeerSessionTriState == PeerSession3State.ChatServer)
                {

                    // if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                    //     return ;

                    string chatRoomNr = (Entities.Settings.Singleton.ChatRoom != null && !string.IsNullOrEmpty(Entities.Settings.Singleton.ChatRoom.ChatRoomNr))
                        ? Entities.Settings.Singleton.ChatRoom.ChatRoomNr
                        : GetTextBoxText(TextBoxChatSession) ?? "";
                    if (string.IsNullOrEmpty(GetTextBoxText(TextBoxChatSession)))
                        SetTextBoxText(TextBoxChatSession, chatRoomNr);

                    if (string.IsNullOrEmpty(GetTextBoxText(TextBoxChatSession)))
                    {
                        InputDialog dialog = new InputDialog("ChatRoomNr required", "Please enter a valid chat room number or register a new chatroom.", MessageBoxIcon.Warning);
                        dialog.ShowDialog();
                        string? appInputDialogChat = MemoryCache.CacheDict.GetValue<string>(Constants.APP_INPUT_DIALOG);
                        chatRoomNr = (!string.IsNullOrEmpty(appInputDialogChat)) ? appInputDialogChat : GetTextBoxText(TextBoxChatSession);
                        SetTextBoxText(TextBoxChatSession, chatRoomNr);
                    }

                    CqrFacade serverFacade = new CqrFacade(CqrXsEuSrvKey);
                    string contactNameEmail = GetComboBoxText(this.ComboBoxContacts);

                    CqrContact myContact = new CqrContact(Settings.Singleton.MyContact, chatRoomNr, clientFacade.PipeString);
                    CqrContact? friendContact = MiniToolBox.FindContactOrCreateByNameEmail(contactNameEmail, chatRoomNr, clientFacade.PipeString);

                    if (Settings.Singleton.ChatRoom == null || !Settings.Singleton.ChatRoom.ChatRoomNr.Equals(chatRoomNr, StringComparison.CurrentCultureIgnoreCase))
                        Settings.Singleton.ChatRoom = new CChatRoom(chatRoomNr);

                    SetTextBoxText(TextBoxPipe, clientFacade.PipeString);
                    // this.TextBoxPipe.Text = serverMessage.PipeString;
                    // this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
                    myContact.Hash = GetHash();
                    myContact.Message = chatRoomNr;

                    if (friendContact != null)
                    {
                        friendContact.Hash = GetHash();
                        friendContact.Message = chatRoomNr;
                    }


                    // client msg inside 
                    CContent msg = new CContent(unencrypted, clientFacade.PipeString, SerType.Json, MD5Sum.HashString(unencrypted, ""));
                    string encrypted = msg.EncryptToJson(myServerKey);

                    // Server message to webservice with myContact, friendContact, chatRoomNr, 
                    CSrvMsg<string> fmsg = new CSrvMsg<string>(myContact, friendContact ?? myContact, encrypted, serverFacade.PipeString, Settings.Singleton.ChatRoom);


                    SetStatusText(StripStatusLabel, $"Starting send to {chatRoomNr} via server {ServerIpAddress} ...");

                    // Send msg to WebService
                    CSrvMsg<List<string>>? rfmsg = await serverFacade.SendChatMsg_Soap_SimpleAsync(fmsg, encrypted, EncodingType.Base64);
                    if (rfmsg != null)
                    {
                        if (rfmsg.Sender != null && !string.IsNullOrEmpty(rfmsg.Sender.NameEmail) &&
                            rfmsg.Sender.NameEmail.Equals(myContact.NameEmail, StringComparison.CurrentCultureIgnoreCase))
                        {
                            myContact = new CContact(rfmsg.Sender, rfmsg.CRoom.ChatRoomNr, rfmsg.Sender.Hash, myContact.ContactImage);
                            Settings.Singleton.MyContact = myContact;
                        }
                        if (rfmsg.CRoom != null && !string.IsNullOrEmpty(rfmsg.CRoom.ChatRoomNr))
                            Settings.Singleton.ChatRoom = new CChatRoom(rfmsg.CRoom);

                        SetStatusText(StripStatusLabel, $"Send server message, now saving results from chatroom...");
                        Settings.SaveSettings(Settings.Singleton);
                    }


                    // string msgChatRoom = "ChatRoomNr: " + rfmsg.ChatRoomNr + "\n" + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
                    // AppendText(TextBoxDestionation, chat.AddFriendMessage(msgChatRoom));
                    string userMsg = chat.AddMyMessage(unencrypted);
                    AppendText(TextBoxSource, userMsg);

                    // this.RichTextBoxOneView.Rtf = this.RichTextBoxChat.Rtf;
                    Format_Lines_RichTextBox();
                    SetRichText(RichTextBoxChat, string.Empty);
                    // this.RichTextBoxChat.Text = string.Empty;
                    await PlaySoundFromResourcesAsync("sound_arrow");
                    SetStatusText(StripStatusLabel, $"Send to {chatRoomNr} via server {ServerIpAddress} successfully.");

                    await MenuCommandsItemRefresh_Click(sender, e);
                }
                // otherwise send message to registered user via server
                // Always encrypt via key
            }
            catch (Exception ex)
            {
                Area23Log.LogOriginMsgEx(this.Name, $"Exception in MenuCommandsItemSend_Click: {ex.Message}.\n", ex);
                SetStatusText(StripStatusLabel, $"Sending to {ipAddrString} failed: {ex.Message}");
                await PlaySoundFromResourcesAsync("sound_warning");
            }

        }

        /// <summary>
        /// Attaches a file to send
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        internal async Task MenuCommandsItemAttach_Click(object sender, EventArgs e)
        {
            if (chat == null)
                chat = new Chat(0);

            string encrypted = string.Empty;

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
            {
                SetStatusText(StripStatusLabel, "Nothing to send!");
                return;
            }

            try
            {

                myServerKey = GetComboBoxText(this.ComboBoxSecretKey);
                CqrFacade clientFacade = new CqrFacade(myServerKey);

                if (this.PeerSessionTriState == PeerSession3State.Peer2Peer)
                {

                    if ((ipAddrString = GetComboBoxMustHaveText(ref ComboBoxIp)) == null)
                    {
                        SetStatusText(StripStatusLabel, "Nothing to send (no ip addr).");
                        return;
                    }


                    FileOpenDialog = DialogFileOpen;
                    DialogResult result = FileOpenDialog.ShowDialog();
                    if ((result == DialogResult.OK || result == DialogResult.Yes) && File.Exists(FileOpenDialog.FileName))
                    {

                        CFile? cfile = GetCFileFromPath(FileOpenDialog.FileName, clientFacade.PipeString);


                        if (cfile != null && !string.IsNullOrEmpty(GetComboBoxText(ComboBoxIp)) && !GetComboBoxText(ComboBoxIp).Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string partnerIpAddrStr = GetComboBoxText(this.ComboBoxIp);
                            partnerIpAddress = IPAddress.Parse(partnerIpAddrStr);

                            string sndFileResponse = clientFacade.Send_CFile_Peer(cfile, partnerIpAddress, Constants.CHAT_PORT, SerType.Json, EncodingType.Base64);

                            string base64FilePath = Path.Combine(LibPaths.AttachmentFilesDir, cfile.FileName + Constants.BASE64_EXT);
                            System.IO.File.WriteAllText(base64FilePath, cfile.ToBase64());

                            string userMsg = chat.AddMyMessage(cfile.GetFileNameContentLength());
                            AppendText(TextBoxSource, userMsg);
                            Format_Lines_RichTextBox();
                            SetRichText(RichTextBoxChat, string.Empty);
                            // this.RichTextBoxChat.Text = string.Empty;
                            SetStatusText(StripStatusLabel, $"File {cfile.FileName} send to {partnerIpAddress} successfully!");
                        }
                        // otherwise send message to registered user via server
                        // Always encrypt via key
                    }
                }
                else if (this.PeerSessionTriState == PeerSession3State.ChatServer)
                {
                    //if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                    //    return;

                    string chatRoomNr = (Entities.Settings.Singleton.ChatRoom != null && !string.IsNullOrEmpty(Entities.Settings.Singleton.ChatRoom.ChatRoomNr))
                            ? Entities.Settings.Singleton.ChatRoom.ChatRoomNr
                            : GetTextBoxText(TextBoxChatSession) ?? "";
                    if (string.IsNullOrEmpty(GetTextBoxText(TextBoxChatSession)))
                        SetTextBoxText(TextBoxChatSession, chatRoomNr);

                    if (string.IsNullOrEmpty(GetTextBoxText(TextBoxChatSession)))
                    {
                        InputDialog dialog = new InputDialog("ChatRoomNr required", "Please enter a valid chat room number or register a new chatroom.", MessageBoxIcon.Warning);
                        dialog.ShowDialog();
                        string? appInputDialogChat = MemoryCache.CacheDict.GetValue<string>(Constants.APP_INPUT_DIALOG);
                        chatRoomNr = (!string.IsNullOrEmpty(appInputDialogChat)) ? appInputDialogChat : GetTextBoxText(TextBoxChatSession);
                        SetTextBoxText(TextBoxChatSession, chatRoomNr);
                    }

                    CqrFacade serverFacade = new CqrFacade(CqrXsEuSrvKey);
                    string contactNameEmail = GetComboBoxText(this.ComboBoxContacts);

                    CContact myContact = new CContact(Settings.Singleton.MyContact, chatRoomNr, clientFacade.PipeString);
                    CContact? friendContact = MiniToolBox.FindContactOrCreateByNameEmail(contactNameEmail, chatRoomNr, clientFacade.PipeString);

                    if (Settings.Singleton.ChatRoom == null || !Settings.Singleton.ChatRoom.ChatRoomNr.Equals(chatRoomNr, StringComparison.CurrentCultureIgnoreCase))
                        Settings.Singleton.ChatRoom = new CChatRoom(chatRoomNr);

                    SetTextBoxText(TextBoxPipe, clientFacade.PipeString);
                    // this.TextBoxPipe.Text = serverMessage.PipeString;
                    // this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
                    myContact.Hash = GetHash();
                    myContact.Message = chatRoomNr;
                    if (friendContact != null)
                    {
                        friendContact.Hash = GetHash();
                        friendContact.Message = chatRoomNr;
                    }


                    // get default file open choose dialog
                    FileOpenDialog = DialogFileOpen;
                    DialogResult result = FileOpenDialog.ShowDialog();
                    if ((result == DialogResult.OK || result == DialogResult.Yes) && File.Exists(FileOpenDialog.FileName))
                    {
                        CFile? cfile = GetCFileFromPath(FileOpenDialog.FileName, clientFacade.PipeString);

                        if (cfile != null && !string.IsNullOrEmpty(chatRoomNr))
                        {
                            // save base64 transformed file under attachments
                            // string base64FilePath = Path.Combine(LibPaths.AttachmentFilesDir, cfile.FileName + Constants.BASE64_EXT);
                            // System.IO.File.WriteAllText(base64FilePath, cfile.ToBase64());
                            // encrypt CFile with CqrXsEuSrvKey and json serialize it 
                            encrypted = cfile.EncryptToJson(myServerKey);
                            SetStatusText(StripStatusLabel, $"File {cfile.FileName} enrypted with client, now generating server message.");

                            // generate session chat server msg with serverFacade.PipeString
                            CSrvMsg<string> fmsg = new CSrvMsg<string>(myContact, friendContact ?? myContact, encrypted, serverFacade.PipeString, Settings.Singleton.ChatRoom);
                            SetStatusText(StripStatusLabel, $"Generated server message with encrypted file inside, prepating to send...");

                            // Send to WebService
                            CSrvMsg<List<string>>? rfmsg = await serverFacade.SendChatMsg_Soap_SimpleAsync(fmsg, encrypted, EncodingType.Base64);

                            if (rfmsg != null)
                            {
                                if (rfmsg.Sender != null && !string.IsNullOrEmpty(rfmsg.Sender.NameEmail) &&
                                    rfmsg.Sender.NameEmail.Equals(myContact.NameEmail, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    myContact = new CContact(rfmsg.Sender, rfmsg.CRoom.ChatRoomNr, rfmsg.Sender.Hash, myContact.ContactImage);
                                    Settings.Singleton.MyContact = myContact;
                                }
                                if (rfmsg.CRoom != null && !string.IsNullOrEmpty(rfmsg.CRoom.ChatRoomNr))
                                    Settings.Singleton.ChatRoom = new CChatRoom(rfmsg.CRoom);

                                SetStatusText(StripStatusLabel, $"Send server message with encrypted file inside, now saving results from chatroom...");
                                Settings.SaveSettings(Settings.Singleton);
                            }

                            string userMsg = chat.AddMyMessage(cfile.GetFileNameContentLength());
                            AppendText(TextBoxSource, userMsg);
                            Format_Lines_RichTextBox();
                            SetRichText(RichTextBoxChat, string.Empty);
                            // this.RichTextBoxChat.Text = string.Empty;
                            await PlaySoundFromResourcesAsync("sound_push");
                            SetStatusText(StripStatusLabel, $"File {cfile.FileName} send to {partnerIpAddress} successfully!");

                            await MenuCommandsItemRefresh_Click(sender, e);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Area23Log.LogOriginMsgEx(this.Name, $"Exception in MenuItemAttach_Click: {ex.Message}.\n", ex);
                SetStatusText(StripStatusLabel, "Attach FAILED: " + ex.Message);
                PlaySoundFromResource("sound_warning");
            }
        }


        /// <summary>
        /// MenuCommandsItemRefresh_Click refresh in session server mode from server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>

        internal async Task MenuCommandsItemRefresh_Click(object sender, EventArgs e)
        {
            if (chat == null)
                chat = new Chat(0);

            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
            {
                SetStatusText(StripStatusLabel, "Nothing to send!");
                return;
            }

            CqrFacade serverFacade = new CqrFacade(CqrXsEuSrvKey);
            CqrFacade clientFacade = new CqrFacade(myServerKey);


            if (this.PeerSessionTriState == PeerSession3State.ChatServer)
            {
                if ((contactNameEmail = GetComboBoxText(ComboBoxContacts)) == Constants.ENTER_CONTACT)
                    contactNameEmail = "";
                //     return;

                string chatRoomNr = (Entities.Settings.Singleton.ChatRoom != null && !string.IsNullOrEmpty(Entities.Settings.Singleton.ChatRoom.ChatRoomNr))
                    ? Entities.Settings.Singleton.ChatRoom.ChatRoomNr
                    : GetTextBoxText(TextBoxChatSession) ?? "";
                if (string.IsNullOrEmpty(GetTextBoxText(TextBoxChatSession)))
                    SetTextBoxText(TextBoxChatSession, chatRoomNr);

                if (string.IsNullOrEmpty(GetTextBoxText(TextBoxChatSession)))
                {
                    InputDialog dialog = new InputDialog("ChatRoomNr required", "Please enter a valid chat room number or register a new chatroom.", MessageBoxIcon.Warning);
                    dialog.ShowDialog();
                    string? appInputDialogChat = MemoryCache.CacheDict.GetValue<string>(Constants.APP_INPUT_DIALOG);
                    chatRoomNr = (!string.IsNullOrEmpty(appInputDialogChat)) ? appInputDialogChat : GetTextBoxText(TextBoxChatSession);
                    SetTextBoxText(TextBoxChatSession, chatRoomNr);
                }
                string pipeText = GetTextBoxText(TextBoxPipe);

                CContact myContact = new CContact(Settings.Singleton.MyContact, chatRoomNr, clientFacade.PipeString);
                CContact? friendContact = MiniToolBox.FindContactOrCreateByNameEmail(contactNameEmail, chatRoomNr, clientFacade.PipeString);

                myContact.Hash = GetHash();
                myContact.Message = chatRoomNr;

                if (friendContact != null)
                {
                    friendContact.Hash = GetHash();
                    friendContact.Message = chatRoomNr;
                }

                SetTextBoxText(TextBoxPipe, clientFacade.PipeString);
                // this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;

                CSrvMsg<string> fmsg = new CSrvMsg<string>(myContact, friendContact ?? myContact, chatRoomNr, serverFacade.PipeString, Settings.Singleton.ChatRoom);

                // Receive Msg from WebSerive
                CSrvMsg<List<string>>? rfmsg = await serverFacade.ReceiveChatMsg_SoapAsync<string>(fmsg, EncodingType.Base64);


                CContent msg = new CContent("", clientFacade.PipeString, SerType.Json, "");
                if (rfmsg == null)
                {
                    MessageBox.Show("Empty message or empty body", "Message from Service is null or body is empty!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (rfmsg != null)
                {
                    if (rfmsg.TContent == null && rfmsg.TContent.Count == 0) 
                    {
                        rfmsg.TContent = new List<string>();
                        rfmsg.TContent.Add(rfmsg.CRoom.ChatRoomNr);
                    }

                    if (rfmsg.Sender != null && !string.IsNullOrEmpty(rfmsg.Sender.NameEmail) &&
                        (rfmsg.Sender.NameEmail.Equals(myContact.NameEmail, StringComparison.CurrentCultureIgnoreCase) ||
                         rfmsg.Sender.Email.Equals(myContact.Email, StringComparison.CurrentCultureIgnoreCase) ||
                         (rfmsg.Sender.Name.Length > 5 && rfmsg.Sender.Name.Equals(myContact.Name, StringComparison.CurrentCultureIgnoreCase))))
                    {
                        CImage myCImg = (myContact.ContactImage != null) ? myContact.ContactImage :
                            CImage.FromDrawingImage(EU.CqrXs.WinForm.SecureChat.Properties.fr.Resources.DefaultF48, "F48");
                        myContact = new CContact(rfmsg.Sender, rfmsg.CRoom.ChatRoomNr, rfmsg.Sender.Hash, myCImg);
                        Settings.Singleton.MyContact = myContact;

                        if (rfmsg.Recipients != null && rfmsg.Recipients.Count > 0)
                        {
                            CImage friendCImg = CImage.FromDrawingImage(EU.CqrXs.WinForm.SecureChat.Properties.fr.Resources.DefaultF42, "F42");
                            CContact friendCtc = rfmsg.Recipients.ElementAt(0);
                            if (friendContact != null && friendCtc.NameEmail.Equals(myContact.NameEmail, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (rfmsg.Recipients.Count > 1)
                                    friendCtc = rfmsg.Recipients.ElementAt(1);
                            }

                            CContact partner = new CContact(friendCtc, rfmsg.CRoom.ChatRoomNr, rfmsg.Sender.Hash, friendCImg);
                            CContact? friendC = MiniToolBox.FindContactOrCreateByNameEmail(partner.NameEmail, chatRoomNr, clientFacade.PipeString);
                            string contactChatRoom = GetComboBoxText(this.ComboBoxContacts);
                            if (friendC != null && (string.IsNullOrEmpty(contactChatRoom) || contactChatRoom.Equals(Constants.ENTER_CONTACT, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                contactChatRoom = (!string.IsNullOrEmpty(friendC.NameEmail)) ? friendC.NameEmail : friendC.Email;
                                if (!string.IsNullOrEmpty(contactChatRoom))
                                    SetComboBoxText(this.ComboBoxContacts, contactChatRoom);
                            }
                        }
                    }
                    else
                    {
                        CImage friendCImg = CImage.FromDrawingImage(EU.CqrXs.WinForm.SecureChat.Properties.fr.Resources.DefaultF42, "F42");
                        CContact invitor = new CContact(rfmsg.Sender, rfmsg.CRoom.ChatRoomNr, rfmsg.Sender.Hash, friendCImg);
                        CContact? friendC = MiniToolBox.FindContactOrCreateByNameEmail(invitor.NameEmail, chatRoomNr, clientFacade.PipeString);
                        string contactChatRoom = GetComboBoxText(this.ComboBoxContacts);
                        if (friendC != null && (string.IsNullOrEmpty(contactChatRoom) || contactChatRoom.Equals(Constants.ENTER_CONTACT, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            contactChatRoom = (string.IsNullOrEmpty(friendC.NameEmail)) ? friendC.NameEmail : friendC.Email;
                            if (!string.IsNullOrEmpty(contactChatRoom))
                                SetComboBoxText(this.ComboBoxContacts, contactChatRoom);
                        }
                    }

                    if (rfmsg.CRoom != null && !string.IsNullOrEmpty(rfmsg.CRoom.ChatRoomNr))
                        Settings.Singleton.ChatRoom = new CChatRoom(rfmsg.CRoom);

                    SetStatusText(StripStatusLabel, $"Successfully sended server message, now saving results from chatroom...");
                    Settings.SaveSettings(Settings.Singleton);
                }

                string msgChatRoom = "ChatRoomNr: " + rfmsg.CRoom.ChatRoomNr + "\n" + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
                string friendMsg = "", appendDestMsg = "";
                CContent msgContent;
                CFile? msgFile, cReceivedFile;
                bool soundPlayed = false;

                foreach (string msgInnerContent in rfmsg.TContent)
                {
                    try
                    {
                        if (msgInnerContent.Equals(chatRoomNr, StringComparison.CurrentCultureIgnoreCase) ||
                            msgInnerContent.EndsWith(chatRoomNr, StringComparison.CurrentCultureIgnoreCase) ||
                            msgInnerContent.StartsWith(chatRoomNr.Replace(".json", ""), StringComparison.CurrentCultureIgnoreCase))
                        {
                            ;
                            // await PlaySoundFromResourcesAsync("sound_ups");
                        }
                        else if ((msgInnerContent.IsValidJson() || msgInnerContent.IsValidXml()) &&
                            msgInnerContent.Contains("FileName") && msgInnerContent.Contains("Base64Type"))
                        {
                            msgFile = new CFile(msgInnerContent, SerType.Json);
                            cReceivedFile = msgFile.DecryptFromJson(myServerKey, msgInnerContent);
                            if (cReceivedFile != null)
                            {
                                SetAttachmentTextLink(cReceivedFile);
                                friendMsg = cReceivedFile.GetFileNameContentLength() + Environment.NewLine;
                                if (!soundPlayed)
                                {
                                    await PlaySoundFromResourcesAsync("sound_wind");
                                    soundPlayed = true;
                                }
                            }
                        }
                        else
                        {
                            msgContent = msg.DecryptFromJson(myServerKey, msgInnerContent);
                            friendMsg = msgContent.Message + Environment.NewLine;
                            if (!soundPlayed)
                            {
                                await PlaySoundFromResourcesAsync("sound_push");
                                soundPlayed = true;
                            }
                        }
                    }
                    catch (Exception exCrypt)
                    {
                        await PlaySoundFromResourcesAsync("sound_hammer");
                        if (exCrypt is InvalidOperationException)
                        {
                            MessageBox.Show(((InvalidOperationException)exCrypt).Message, "Invalid or non matching secret key for decrypt.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            SetComboBoxBackColor(ComboBoxSecretKey, Color.OrangeRed);
                        }
                        else
                        {
                            MessageBox.Show(exCrypt.Message, $"Error/Exception, when decrypting incoming message from {GetComboBoxText(ComboBoxIp)}.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        CqrException.SetLastException(exCrypt);
                        SetStatusText(StripStatusLabel, $"Exception {exCrypt.Message} on receiving message from from server {ServerIpAddress} chat room {chatRoomNr}.");
                        return;
                    }

                    appendDestMsg = chat.AddFriendMessage(friendMsg);
                    AppendText(TextBoxDestionation, appendDestMsg);
                    Format_Lines_RichTextBox();
                }
                
                SetRichText(RichTextBoxChat, string.Empty);
                SetStatusText(StripStatusLabel, $"Received msg from server {ServerIpAddress} chat room {chatRoomNr}.");
            }
        }

        /// <summary>
        /// MenuItemClear_Click clears all input & output chat windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal async Task MenuCommandsItemClear_Click(object sender, EventArgs e)
        {
            // TODO: add warning and saving here
            await PlaySoundFromResourcesAsync("sound_glasses");
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


        internal async Task ButtonAttach_Click(object sender, EventArgs e) => await this.MenuCommandsItemAttach_Click(sender, e);

        internal async Task ButtonSend_Click(object sender, EventArgs e) => await this.MenuCommandsItemSend_Click(sender, e);

        #endregion MenuCommands MenuSend MenuAttach MenuRefresh MenuClear incl. Buttons

        #region ButtonVisitChatRoom_Click ButtonInviteChatRoom_Click

        /// <summary>
        /// ButtonVisitChatRoom_Click visits a chat room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal async Task ButtonVisitChatRoom_Click(object sender, EventArgs e)
        {
            if ((myServerKey = GetComboBoxMustHaveText(ref ComboBoxSecretKey)) == null)
            {
                SetStatusText(StripStatusLabel, "Nothing to send!");
                return;
            }

            if (this.PeerSessionTriState == PeerSession3State.Peer2Peer || this.PeerSessionTriState == PeerSession3State.None)
            {
                MessageBox.Show($"You must set chat mode to server session to visit a chat room.", $"SessionTriState is {this.PeerSessionTriState.ToString()}, expected: ChatServer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            string? chatRoomTxt = GetTextBoxText(this.TextBoxChatSession);

            if (string.IsNullOrEmpty(chatRoomTxt) ||
                !GetTextBoxText(TextBoxChatSession).StartsWith("room") ||
                !GetTextBoxText(TextBoxChatSession).EndsWith(".json"))
            {
                MessageBox.Show($"Invalid or empty chat room.", "Please enter a valid chat room image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Settings.Singleton.ChatRoom == null)
            {
                Settings.Singleton.ChatRoom = new CChatRoom(chatRoomTxt);
            }
            Settings.Singleton.ChatRoom.MsgDict = new Dictionary<long, string>();
            Settings.Singleton.ChatRoom.LastPushed = DateTime.MinValue;
            Settings.Singleton.ChatRoom.LastPolled = DateTime.MinValue;
            Settings.SaveSettings();

            try
            {
                SetTextBoxText(this.TextBoxSource, "");
                SetTextBoxText(this.TextBoxDestionation, "");
                SetRichText(this.RichTextBoxOneView, "");
                await MenuCommandsItemRefresh_Click(sender, e);
                ButtonCheck.Image = Properties.Resources.SatLink;
                await PlaySoundFromResourcesAsync("sound_push");
            }
            catch (Exception exi)
            {
                Area23Log.Log($"Excption {exi.GetType()}: {exi.Message}\n\t{exi}\n");
                SetStatusText(this.StripStatusLabel, $"Excption {exi.GetType()} on init chat room invitation: {exi.Message}");
                await PlaySoundFromResourcesAsync("sound_hammer");
            }

        }

        /// <summary>
        /// Invites a selected contact to chat room, request a new chatroom
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">ÊventArgs e</param>
        internal async Task ButtonInviteChatRoom_Click(object sender, EventArgs e)
        {
            if ((contactNameEmail = GetComboBoxMustHaveText(ref ComboBoxContacts)) == null)
                return;

            bool sendInit = false;
            try
            {
                sendInit = await SendInvite_ToChatRoom();
            }
            catch (Exception exi)
            {
                Area23Log.Log($"Excption {exi.GetType()}: {exi.Message}\n\t{exi}\n");
                sendInit = false;
                SetStatusText(this.StripStatusLabel, $"Excption {exi.GetType()} on init chat room invitation: {exi.Message}");
            }

            if (sendInit)
            {
                ButtonCheck.Image = Properties.Resources.SatLink;
                await PlaySoundFromResourcesAsync("sound_laser");
            }
            else
            {
                ButtonCheck.Image = Properties.Resources.CableWireCut;
                await PlaySoundFromResourcesAsync("sound_warning");
            }
        }

        #endregion ButtonVisitChatRoom_Click ButtonInviteChatRoom_Click

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
                Task.Run((async () => await TooglePeerSessionServerTriState(0, false)));

            CqrFacade clientFacade = new CqrFacade(myServerKey);

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

                    string msgInnerContent = (string)(encrypted);
                    string friendMsg = "";
                    CContent msgContent, msg = new CContent(msgInnerContent, SerType.Json);
                    CFile? msgFile, cReceivedFile;

                    try
                    {
                        if ((msgInnerContent.IsValidJson() || msgInnerContent.IsValidXml()) &&
                        msgInnerContent.Contains("FileName") && msgInnerContent.Contains("Base64Type"))
                        {
                            msgFile = new CFile(msgInnerContent, SerType.Json);
                            cReceivedFile = msgFile.DecryptFromJson(myServerKey, msgInnerContent);
                            if (cReceivedFile != null)
                            {
                                SetAttachmentTextLink(cReceivedFile);
                                friendMsg = cReceivedFile.GetFileNameContentLength() + Environment.NewLine;
                                PlaySoundFromResource("sound_wind");
                            }
                        }
                        else
                        {
                            msgContent = msg.DecryptFromJson(myServerKey, msgInnerContent);
                            friendMsg = msgContent.Message + Environment.NewLine;
                            PlaySoundFromResource("sound_push");
                        }
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

                    string appendDestMsg = chat.AddFriendMessage(friendMsg);
                    AppendText(TextBoxDestionation, appendDestMsg);
                    // AppendText(TextBoxDestionation, friendMsg);
                    // this.RichTextBoxOneView.Text = unencrypted;
                    Format_Lines_RichTextBox();
                }
            }
        }


        public async Task TooglePeerSessionServerTriState(short svalue, bool fireUp = true)
        {
            if (Settings.Singleton.OnlyPeer2PeerChat)
                svalue = 0;

            switch (svalue)
            {

                case 0:
                    PeerSessionTriState = PeerSession3State.Peer2Peer;
                    SetComboBoxText(ComboBoxContacts, Constants.ENTER_CONTACT);
                    try
                    {
                        EnableTextBox(this.TextBoxChatSession, false);
                        SetComboBoxEnabled(this.ComboBoxIp, true);
                        SetComboBoxEnabled(this.ComboBoxContacts, false);

                        SetMenuItemEnabledChecked(this.MenuOptionsItemPeer2Peer, true, true);
                        SetMenuItemEnabledChecked(this.MenuOptionsItemServerSession, true, false);

                    }
                    catch (Exception exTriState)
                    {
                        Area23Log.LogOriginMsgEx("RichTextChat", $"PeerSessionTriState = {PeerSession3State.Peer2Peer}", exTriState);
                    }
                    await BgWorkerMonitor_WorkMonitorAsync("TooglePeerSessionServerTriState", new EventArgs());
                    break;
                case 2:
                    this.PeerSessionTriState = PeerSession3State.ChatServer;
                    SetComboBoxText(ComboBoxIp, Constants.ENTER_IP);
                    try
                    {
                        EnableTextBox(this.TextBoxChatSession, true);
                        SetComboBoxEnabled(this.ComboBoxIp, false);
                        SetComboBoxEnabled(this.ComboBoxContacts, true);

                        SetMenuItemEnabledChecked(this.MenuOptionsItemPeer2Peer, true, false);
                        SetMenuItemEnabledChecked(this.MenuOptionsItemServerSession, true, true);
                    }
                    catch (Exception exTriState)
                    {
                        Area23Log.LogOriginMsgEx("RichTextChat", $"PeerSessionTriState = {PeerSession3State.Peer2Peer}", exTriState);
                    }
                    break;
                case 1:
                default:
                    this.PeerSessionTriState = PeerSession3State.None;
                    try
                    {
                        EnableTextBox(this.TextBoxChatSession, false);
                        SetComboBoxEnabled(this.ComboBoxIp, false);
                        SetComboBoxEnabled(this.ComboBoxContacts, false);
                        SetMenuItemEnabledChecked(this.MenuOptionsItemPeer2Peer, true, false);
                        SetMenuItemEnabledChecked(this.MenuOptionsItemServerSession, true, false);
                    }
                    catch (Exception exTriState)
                    {
                        Area23Log.LogOriginMsgEx("RichTextChat", $"PeerSessionTriState = {PeerSession3State.Peer2Peer}", exTriState);
                    }
                    await BgWorkerMonitor_WorkMonitorAsync("TooglePeerSessionServerTriState", new EventArgs());
                    break;
            }
            this.PeerServerSwitch.SetPeerServerSessionTriState(PeerSessionTriState, fireUp);
        }

        public void TooglePeerServer(object sender, EventArgs e)
        {
            if (e is Area23EventArgs<int> ev)
            {
                if (((short)PeerSessionTriState) != ((short)ev.GenericTData))
                    Task.Run((async () => await TooglePeerSessionServerTriState((short)ev.GenericTData)));

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

                myServerKey = GetComboBoxText(this.ComboBoxSecretKey);
                CqrFacade clientFacade = new CqrFacade(myServerKey);
                CqrFacade serverFacade = new CqrFacade(CqrXsEuSrvKey);

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
                            CFile? cf = SendCFile(ea.GenericTData, myServerKey, pi);
                            if (cf != null && cf.Data != null && !string.IsNullOrEmpty(cf.FileName))
                            {
                                string userMsg = chat.AddMyMessage(cf.GetFileNameContentLength());
                                AppendText(TextBoxSource, userMsg);
                                Format_Lines_RichTextBox();
                                this.RichTextBoxChat.Text = string.Empty;
                                SetStatusText(StripStatusLabel, $"File {cf.FileName} send successfully!");
                            }
                        }
                    }
                    else if (this.PeerSessionTriState == PeerSession3State.ChatServer)
                    {
                        string chatRoomNr = (Entities.Settings.Singleton.ChatRoom != null && !string.IsNullOrEmpty(Entities.Settings.Singleton.ChatRoom.ChatRoomNr))
                            ? Entities.Settings.Singleton.ChatRoom.ChatRoomNr
                            : GetTextBoxText(TextBoxChatSession) ?? "";
                        if (string.IsNullOrEmpty(GetTextBoxText(TextBoxChatSession)))
                            SetTextBoxText(TextBoxChatSession, chatRoomNr);

                        if (string.IsNullOrEmpty(GetTextBoxText(TextBoxChatSession)))
                        {
                            InputDialog dialog = new InputDialog("ChatRoomNr required", "Please enter a valid chat room number or register a new chatroom.", MessageBoxIcon.Warning);
                            dialog.ShowDialog();
                            string? appChatInputDialog = MemoryCache.CacheDict.GetValue<string>(Constants.APP_INPUT_DIALOG);
                            chatRoomNr = (string.IsNullOrEmpty(appChatInputDialog)) ? string.Empty : appChatInputDialog;
                            string textSessionChatRoom = (!string.IsNullOrEmpty(chatRoomNr)) ? chatRoomNr : GetTextBoxText(TextBoxChatSession);
                            SetTextBoxText(TextBoxChatSession, textSessionChatRoom);
                        }

                        string contactNameEmail = GetComboBoxText(ComboBoxContacts);

                        CqrContact myContact = new CqrContact(Settings.Singleton.MyContact, chatRoomNr, clientFacade.PipeString);
                        CContact? friendContact = MiniToolBox.FindContactOrCreateByNameEmail(contactNameEmail, chatRoomNr, clientFacade.PipeString);

                        SetTextBoxText(TextBoxPipe, clientFacade.PipeString);
                        // this.toolStripTextBoxCqrPipe.Text = serverMessage.PipeString;
                        myContact.Hash = GetHash();
                        myContact.Message = chatRoomNr;
                        friendContact.Message = chatRoomNr;

                        string filename = ea.GenericTData;

                        string md5 = Area23.At.Framework.Core.Crypt.Hash.MD5Sum.Hash(filename, true);
                        string sha256 = Area23.At.Framework.Core.Crypt.Hash.Sha256Sum.Hash(filename, true);
                        byte[] fileBytes = File.ReadAllBytes(filename);
                        string fileNameOnly = Path.GetFileName(filename);
                        string mimeType = MimeType.GetMimeType(fileBytes, fileNameOnly);

                        CFile cfile = new CFile(fileNameOnly, mimeType, fileBytes, clientFacade.PipeString, md5, sha256);
                        string encryptedFileMsg = cfile.EncryptToJson(myServerKey);
                        SetStatusText(StripStatusLabel, $"File {cfile.FileName} encrypted with client, now generating server message.");

                        CSrvMsg<string> fmsg = new CSrvMsg<string>(myContact, friendContact, encryptedFileMsg, serverFacade.PipeString, Settings.Singleton.ChatRoom);
                        SetStatusText(StripStatusLabel, $"Generated server message with encrypted file inside, prepating to send...");

                        // Send message to WebService
                        CSrvMsg<List<string>> rfmsg = serverFacade.SendChatMsg_Soap_Simple(fmsg, encryptedFileMsg, EncodingType.Base64);
                        if (rfmsg != null)
                        {
                            if (rfmsg.Sender != null && !string.IsNullOrEmpty(rfmsg.Sender.NameEmail) &&
                                rfmsg.Sender.NameEmail.Equals(myContact.NameEmail, StringComparison.CurrentCultureIgnoreCase))
                            {
                                myContact = new CContact(rfmsg.Sender, rfmsg.CRoom.ChatRoomNr, rfmsg.Sender.Hash, myContact.ContactImage);
                                Settings.Singleton.MyContact = myContact;
                            }
                            if (rfmsg.CRoom != null && !string.IsNullOrEmpty(rfmsg.CRoom.ChatRoomNr))
                            {
                                Settings.Singleton.ChatRoom = new CChatRoom(rfmsg.CRoom);
                                chatRoomNr = Settings.Singleton.ChatRoom.ChatRoomNr;
                            }

                            SetStatusText(StripStatusLabel, $"Encrypted file {cfile.FileName} and sended it to chat room {chatRoomNr}...");
                            Settings.SaveSettings(Settings.Singleton);
                        }


                        // string msgChatRoom = "ChatRoomNr: " + rfmsg.ChatRoomNr + "\n" + String.Join(", ", rfmsg.GetEmails()) + "\r\n"; // + serverMessage.symmPipe.HexStages;
                        // this.TextBoxDestionation.Text = msgChatRoom;
                        string userMsg = chat.AddMyMessage(cfile.GetFileNameContentLength());
                        AppendText(TextBoxSource, userMsg);
                        Format_Lines_RichTextBox();
                        SetRichText(RichTextBoxChat, string.Empty);
                        PlaySoundFromResource("sound_push");
                        SetStatusText(StripStatusLabel, $"File {cfile.FileName} successfully send to {chatRoomNr} !");

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
        protected internal void SetAttachmentTextLink(CFile cfile)
        {
            string fileName = cfile.FileName;
            string mimeFilePath = Path.Combine(LibPaths.AttachmentFilesDir, cfile.FileName + Constants.HTML_EXT);
            string filePath = Path.Combine(LibPaths.AttachmentFilesDir, cfile.FileName);

            byte[] attachBytes = EnDeCodeHelper.GetBytes(cfile.GetWebPage());
            System.IO.File.WriteAllBytes(mimeFilePath, attachBytes);

            System.IO.File.WriteAllBytes(filePath, cfile.Data);

            LinkedLabelsBox.SetNameFilePath(fileName, filePath);
        }


        public override async Task BgWorkerMonitor_WorkMonitorAsync(object? sender, EventArgs e)
        {
            await base.BgWorkerMonitor_WorkMonitorAsync(sender, e);

            if (this.PeerSessionTriState == PeerSession3State.ChatServer)
            {
                await MenuCommandsItemRefresh_Click(sender, e);
            }
            else if (this.PeerSessionTriState == PeerSession3State.Peer2Peer)
            {

                IPAddress? newAddr = null;
                ToolStripMenuItem? oldIpItem = null, newIpIem = null;
                List<IPAddress> addresses = GetProxiesFromSettingsResources();
                InterfaceIpAddresses = await NetworkAddresses.GetIpAddressesAsync();
                try
                {
                    ConnectedIpAddresses = await NetworkAddresses.GetConnectedIpAddressesAsync(addresses);
                }
                catch (Exception noInternetEx)
                {
                    SetStatusText(StripStatusLabel, $"No connection to internet {noInternetEx.Message}.");
                }

                if (PeerSessionTriState == PeerSession3State.Peer2Peer || PeerSessionTriState == PeerSession3State.None)
                {
                    bool anyChecked = false;
                    if (clientIpAddress != null)
                    {
                        foreach (ToolStripMenuItem tsItem in MenuNetworkItemMyIps.DropDown.Items)
                        {
                            string menuItemText = "";
                            try { menuItemText = tsItem.Text; } catch { menuItemText = GetMenuItemText(tsItem); }
                            if (menuItemText == clientIpAddress.AddressFamily + clientIpAddress.ToString())
                            {
                                newIpIem = tsItem;
                                newAddr = clientIpAddress;
                                if (tsItem.Checked)
                                    anyChecked = true;
                            }
                            else
                                if (tsItem.Checked)
                            {
                                oldIpItem = tsItem;
                                anyChecked = true;
                            }
                        }
                        if (anyChecked && oldIpItem != null && newIpIem != null)
                        {
                            newIpIem.Checked = true;
                            oldIpItem.Checked = false;
                        }
                    }

                    if (anyChecked && oldIpItem != null && newIpIem == null)
                    {
                        oldIpItem.Checked = true;
                    }


                    if (ipSockListener != null && ipSockListener.ServerSocket != null &&
                       (ipSockListener.ServerSocket.Connected || ipSockListener.ServerSocket.IsBound))
                    // && !ipSockListener.ServerSocket.Blocking)
                    {
                        if (ipSockListener.ServerEndPoint != null)
                            Area23Log.Log($"ipSockListener enpoint peforming normal: {ipSockListener.ServerEndPoint.ToString()}");
                    }
                    else // Rebind Server Socket
                    {
                        foreach (ToolStripMenuItem tsmItem in MenuNetworkItemMyIps.DropDown.Items)
                        {
                            if (tsmItem.Checked)
                            {
                                foreach (IPAddress addr in InterfaceIpAddresses)
                                {
                                    string menuItemAText = "";
                                    try { menuItemAText = tsmItem.Text; } catch { menuItemAText = GetMenuItemText(tsmItem); }
                                    if (menuItemAText == addr.AddressFamily.ShortInfo() + addr.ToString())
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
										Area23Log.Log(exi);
                                    }
                                    try
                                    {
                                        ipSockListener = null;
                                    }
                                    catch (Exception exi)
                                    {
										Area23Log.Log(exi);
                                    }

                                    Thread.Sleep(Constants.CLOSING_TIMEOUT);
                                    clientSocket_DataReceived = delegate (object sender, Area23EventArgs<ReceiveData> eventReceived)
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
								Area23Log.Log(exc);
                            }
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

        private async Task AddContactsToIpContactAsync()
        {
            await Task.Run(() => { AddContactsToIpContact(); });
        }

        private void AddContactsToIpContact()
        {
            string currentContactText = GetComboBoxText(this.ComboBoxContacts);
            List<string> currentContactItems = new List<string>();

            var cbItems = GetComboBoxItems(ComboBoxContacts);
            if (cbItems != null)
                foreach (var oitem in cbItems)
                    if (oitem != null && !string.IsNullOrEmpty(oitem.ToString()))
                        currentContactItems.Add(oitem.ToString());

            foreach (CqrContact ct in Entities.Settings.Singleton.Contacts)
            {
                if (ct != null && !string.IsNullOrEmpty(ct.NameEmail))
                {
                    if (!currentContactItems.Contains(ct.NameEmail))
                        AddItemToComboBox(ComboBoxContacts, ct.NameEmail);
                }
            }

            SetComboBoxText(ComboBoxContacts, currentContactText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task MenuContactsItemMyContact_Click(object sender, EventArgs e)
        {
            ContactSettings contactSettings = new ContactSettings("My Contact Info", 0);
            contactSettings.ShowInTaskbar = true;
            do
            {
#pragma warning disable WFO5002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                await contactSettings.ShowDialogAsync();
#pragma warning restore WFO5002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                string notFullReason = string.Empty;
                if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Name))
                    notFullReason += "Name is missing! ";
                if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Email))
                    notFullReason += "Email Address is missing!";
                // if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Mobile))
                //     notFullReason += "Mobile phone is missing!" + Environment.NewLine;
                if (!string.IsNullOrEmpty(notFullReason))
                    MessageBox.Show(notFullReason, "Please fill out your contact info", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            } while (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Email) || string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Name));

            Bitmap? bmp = Properties.fr.Resources.DefaultF45;
            if (Settings.Singleton.MyContact != null && Settings.Singleton.MyContact.ContactImage != null &&
                    Settings.Singleton.MyContact.ContactImage.ImageData != null &&
                    Settings.Singleton.MyContact.ContactImage.ImageData.Length > 0)
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


        #region network functionality

        /// <summary>
        /// SetupNetwork async method to setup network
        /// </summary>
        /// <returns><see cref="Task"/></returns>

        internal async Task SetupNetwork()
        {
            int progress = this.GetProgressBar(this.StripProgressBar);

            List<IPAddress> addresses = GetProxiesFromSettingsResources();
            this.SetProgressBar(this.StripProgressBar, progress + 5);
            SetStatusText(StripStatusLabel, $"Setup Network: Several proxy addresses fetched.");

            List<IPAddress> myIpList = await SetupAllNetworkInterfacesAndConnectedIpAddresses(addresses, progress + 10);
            progress = this.GetProgressBar(this.StripProgressBar);

            this.SetProgressBar(this.StripProgressBar, progress);


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
				Area23Log.Log(exV6);
            }


            // this.MenuItemExternalIp.DropDownItems.Add(extIpItem);
            this.SetProgressBar(this.StripProgressBar, progress + 10);
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
                        (Area23.At.Framework.Core.Static.Extensions.BytesCompare(addrProxy.GetAddressBytes(), ServerIpAddress.GetAddressBytes()) == 0))
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

            this.SetProgressBar(this.StripProgressBar, progress + 15);
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
					    Area23Log.Log("Error when adding friendIps + " + exFriendIp.Message);
                    }
                }
            }
            this.SetProgressBar(this.StripProgressBar, progress + 20);
            SetStatusText(StripStatusLabel, $"Setup Network complete!");

            if (Entities.Settings.Singleton != null)
            {
                Entities.Settings.Singleton.Proxies = proxyList;
                Entities.Settings.Singleton.MyIPs = myIpList.ConvertAll(x => x.ToString()).ToList();
                Entities.Settings.SaveSettings(Entities.Settings.Singleton);
            }

            this.SetProgressBar(this.StripProgressBar, progress + 25);
            SetStatusText(StripStatusLabel, $"Setup Network complete, saving settings!");
        }

        internal async Task<List<IPAddress>> SetupAllNetworkInterfacesAndConnectedIpAddresses(List<IPAddress> addresses, int pvalue = 0)
        {
            InterfaceIpAddresses = await NetworkAddresses.GetIpAddressesAsync();
            SetStatusText(StripStatusLabel, $"Setup Network: All network interfaces addresses fetched.");
            pvalue += 10;
            SetProgressBar(StripProgressBar, pvalue);

            try
            {
                ConnectedIpAddresses = await NetworkAddresses.GetConnectedIpAddressesAsync(addresses);
                SetStatusText(StripStatusLabel, $"Setup Network: All active connected ip addresses fetched.");
            }
            catch (Exception noInternetEx)
            {
                SetStatusText(StripStatusLabel, $"No connection to internet {noInternetEx.Message}.");
            }
            pvalue += 10;
            SetProgressBar(StripProgressBar, pvalue);


            MenuNetworkItemMyIps.DropDown.Items.Clear();
            MenuItemExternalIp = new ToolStripMenuItem();
            MenuItemExternalIp.BackColor = SystemColors.MenuBar;
            MenuItemExternalIp.Name = "MenuItemExternalIp";
            MenuItemExternalIp.Size = new Size(160, 22);
            SetMenuItemText(MenuItemExternalIp, "External Ip's");
            // MenuItemExternalIp.Text = "External Ip's";
            MenuItemExternalIp.Visible = true;
            MenuNetworkItemMyIps.BackColor = SystemColors.MenuBar;
            MenuNetworkItemMyIps.DropDownItems.AddRange(new ToolStripItem[] { MenuItemExternalIp });
            MenuNetworkItemMyIps.Name = "MenuNetworkItemMyIps";
            MenuNetworkItemMyIps.Size = new Size(177, 22);
            SetMenuItemText(MenuNetworkItemMyIps, "my ip's");
            // MenuNetworkItemMyIps.Text = "my ip'sc;

            pvalue += 5;
            SetProgressBar(StripProgressBar, pvalue);

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
                                    SetStatusText(StripStatusLabel, $"Interface {clientIpAddress.AddressFamily.ToString()} {clientIpAddress.ToString()} bound on listener socket.");
                                    SetProgressBar(StripProgressBar, pvalue);
                                    pvalue += 5;
                                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                        SetMenuItemChecked(this.MenuNetworkItemIPv6Secure, true);
                                    // this.MenuNetworkItemIPv6Secure.Checked = true;
                                    clientSocket_DataReceived = delegate (object sender, Area23EventArgs<ReceiveData> eventReceived)
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

                    IPAddress tmpAddr = IPAddress.Parse(addr.ToString());
                    myIpList.Add(addr);
                    AddMenuItemToItems(MenuNetworkItemMyIps, (ToolStripDropDownItem)item);
                    SetProgressBar(StripProgressBar, pvalue);
                    pvalue += 5;
                    SetStatusText(StripStatusLabel, $"Interface {tmpAddr.AddressFamily.ToString()} {tmpAddr.ToString()} added.");
                    // this.MenuNetworkItemMyIps.DropDownItems.Add(item);
                }
            }

            SetStatusText(StripStatusLabel, $"Setup Network: All interface addresses added to menu. Not connected if addrs grayed.");
            SetProgressBar(StripProgressBar, pvalue);
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
							Area23Log.Log(exi);
                        }
                        try
                        {
                            ipSockListener = null;
                        }
                        catch (Exception exi)
                        {
							Area23Log.Log(exi);
                        }

                        Thread.Sleep(Constants.CLOSING_TIMEOUT);
                        clientSocket_DataReceived = delegate (object sender, Area23EventArgs<ReceiveData> eventReceived)
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
					Area23Log.Log(exc);
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
						Area23Log.Log(exi);
                    }

                    Thread.Sleep(Constants.CLOSING_TIMEOUT);

                }
            }
        }

        #endregion network functionality


        #region MenuOptions


        internal void MenuOptionsItemFileTypeSecure_Click(object sender, EventArgs e)
        {
            this.MenuOptionsItemFileTypeSecure.Checked = !this.MenuOptionsItemFileTypeSecure.Checked;
            Settings.Singleton.OnlySecureFileTypes = this.MenuOptionsItemFileTypeSecure.Checked;
            Settings.SaveSettings();
        }

        internal void MenuOptionsItemClearAllOnClose_Click(object sender, EventArgs e)
        {
            // TODO add to settings
            this.MenuOptionsItemClearAllOnClose.Checked = (!this.MenuOptionsItemClearAllOnClose.Checked);
            Settings.Singleton.ClearAllOnClose = this.MenuOptionsItemClearAllOnClose.Checked;
            Settings.SaveSettings();
        }

        private void MenuOptionsItemCompress_Click(object sender, EventArgs e)
        {
            this.MenuOptionsItemCompress.Checked = !this.MenuOptionsItemCompress.Checked;
            Settings.Singleton.ZipBeforeSend = this.MenuOptionsItemCompress.Checked;
            Settings.SaveSettings();
        }

        internal void MenuOptionsItemOnlyPeer2PeerChat_Click(object sender, EventArgs e)
        {
            this.MenuOptionsItemOnlyPeer2PeerChat.Checked = (!this.MenuOptionsItemOnlyPeer2PeerChat.Checked);
            Settings.Singleton.OnlyPeer2PeerChat = this.MenuOptionsItemOnlyPeer2PeerChat.Checked;
            Settings.SaveSettings();
            this.PeerServerSwitch.SetTrackSwitchEnabled(!Settings.Singleton.OnlyPeer2PeerChat);
            MenuOptionsItemPeer2Peer_Click(sender, e);
            SetMenuItemEnabledChecked(MenuOptionsItemPeer2Peer, !Settings.Singleton.OnlyPeer2PeerChat, true);
            SetMenuItemEnabledChecked(MenuOptionsItemServerSession, !Settings.Singleton.OnlyPeer2PeerChat, false);
        }


        private void MenuOptionsItemPeer2Peer_Click(object sender, EventArgs e)
        {
            if (this.MenuOptionsItemPeer2Peer.Checked && !this.MenuOptionsItemServerSession.Checked && PeerSessionTriState == PeerSession3State.Peer2Peer)
                return;

            Task.Run((async () => await TooglePeerSessionServerTriState((short)0)));
        }


        private void MenuOptionsItemServerSession_Click(object sender, EventArgs e)
        {
            if (!this.MenuOptionsItemPeer2Peer.Checked && this.MenuOptionsItemServerSession.Checked && PeerSessionTriState == PeerSession3State.ChatServer)
                return;

            Task.Run((async () => await TooglePeerSessionServerTriState((short)2)));
        }

        #endregion MenuOptions


    }

}
