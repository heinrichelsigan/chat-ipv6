using Area23FwCore = Area23.At.Framework.Core;
using Area23.At.Framework.Core;
using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.CqrXs.CqrSrv;
using Area23.At.Framework.Core.Crypt.Cipher;
using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Net;
using Area23.At.Framework.Core.Net.IpSocket;
using Area23.At.Framework.Core.Net.NameService;
using Area23.At.Framework.Core.Net.WebHttp;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.RichTextChat.Entities;
using EU.CqrXs.WinForm.RichTextChat.Properties;
using EU.CqrXs.WinForm.RichTextChat.Util;
using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Controls;



namespace EU.CqrXs.WinForm.RichTextChat.Gui.Forms
{


    /// <summary>
    /// RichTextChat main form
    /// </summary>
    public partial class RichTextChat : BaseChatForm
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

        private static IPAddress? serverIpAddress;
        internal IPAddress? ServerIpAddress
        {
            get
            {
                if (serverIpAddress != null)
                    return serverIpAddress;

                // TODO: change it
                IEnumerable<IPAddress> list = DnsHelper.GetIpAddrsByHostName(Constants.CQRXS_EU);
                foreach (IPAddress ip in list)
                {
                    foreach (string sip in Settings.Singleton.Proxies)
                    {
                        if (IPAddress.Parse(sip).Equals(ip))
                        {
                            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 &&
                                MenuNetworkItemIPv6Secure.Checked)
                            {
                                serverIpAddress = ip;
                                return serverIpAddress;
                            }
                            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                                !MenuNetworkItemIPv6Secure.Checked)
                            {
                                serverIpAddress = ip;
                                return serverIpAddress;
                            }
                        }
                    }
                }
                foreach (IPAddress ip in list)
                {
                    foreach (string sip in Settings.Singleton.Proxies)
                    {
                        if (IPAddress.Parse(sip).Equals(ip) && ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            serverIpAddress = ip;
                            return serverIpAddress;
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
        public RichTextChat() : base()
        {
            InitializeComponent();
            TextBoxSource.MaxLength = Constants.SOCKET_BYTE_BUFFEER;
            TextBoxDestionation.MaxLength = Constants.SOCKET_BYTE_BUFFEER;
            ComboBoxIp.Text = Constants.ENTER_IP;
            ComboBoxContacts.Text = Constants.ENTER_CONTACT;
            ComboBoxSecretKey.Text = Constants.ENTER_SECRET_KEY;
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
            dragnDropGroupBox.OnDragNDrop += OnDragNDrop;
            this.StripProgressBar.Value = 0;
        }


        private async void RichTextChat_Load(object sender, EventArgs e)
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

            StripStatusLabel.Text = "Setup Network";
            await PlaySoundFromResourcesAsync("sound_train");
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
                    string line = tuple.Value;

                    AppendRichText(RichTextBoxOneView, line + Environment.NewLine);
                    // RichTextBoxOneView.AppendText(line + Environment.NewLine);

                    int startPos = GetFirstCharIndexFromLineRichText(RichTextBoxOneView, lineIndex++);
                    SelectRichText(RichTextBoxOneView, startPos, line.Length + Environment.NewLine.Length);
                    if (chat.MyMsgTStamps.Contains(tuple.Key))
                    {
                        SelectionAlignmentRichText(RichTextBoxOneView, HorizontalAlignment.Right);
                    }
                    else if (chat.FriendMsgTStamps.Contains(tuple.Key))
                    {
                        SelectionAlignmentRichText(RichTextBoxOneView, HorizontalAlignment.Left);
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

            this.ComboBoxIp.BackColor = Color.White;

            if (Entities.Settings.Singleton != null && SendInit_Click())
            {
                PlaySoundFromResource("sound_laser");

                if (!Entities.Settings.Singleton.FriendIPs.Contains(this.ComboBoxIp.Text))
                    Entities.Settings.Singleton.FriendIPs.Add(this.ComboBoxIp.Text);
                if (!this.MenuNetworkComboBoxFriendIp.Items.Contains(partnerIpAddress.ToString()))
                    this.MenuNetworkComboBoxFriendIp.Items.Add(partnerIpAddress.ToString());
                this.MenuNetworkComboBoxFriendIp.Text = partnerIpAddress.ToString();

                if (!this.ComboBoxIp.Items.Contains(this.ComboBoxIp.Text))
                    this.ComboBoxIp.Items.Add(partnerIpAddress.ToString());

                Entities.Settings.SaveSettings(Entities.Settings.Singleton);
            }
            else
            {
                ButtonCheck.Image = Properties.de.Resources.CableWireCut;
                PlaySoundFromResource("sound_warning");
            }

            StripStatusLabel.Text = $"Added new partner ip address {partnerIpAddress.ToString()}.";
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
                MessageBox.Show($"Cannot parse Contact from string \"{ComboBoxContacts.Text}\": {exContactMsg}", "Please enter a valid contact address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxContacts.BackColor = Color.Violet;
                PlaySoundFromResource("sound_warning");
                return;
            }

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
                        if (!comboText.Equals(area23EvArgs.GenericTData.ClientIPAddr, StringComparison.InvariantCultureIgnoreCase))
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
                        friendMsg = mimeAttachment.GetFileNameContentLength();
                        PlaySoundFromResource("sound_wind");
                    }
                    else
                    {
                        friendMsg = msgContent.Message;
                        PlaySoundFromResource("sound_push");
                    }

                    chat.AddFriendMessage(friendMsg);
                    AppendText(TextBoxDestionation, friendMsg + Environment.NewLine);
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
                partnerIpAddress = IPAddress.Parse(this.ComboBoxIp.Text);
                Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                pmsg.Send_CqrPeerMsg(unencrypted, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);

                // chat.AddMyMessage(unencrypted);
                // AppendText(TextBoxSource, unencrypted);
                // Format_Lines_RichTextBox();
                this.RichTextBoxChat.Text = string.Empty;
                StripStatusLabel.Text = "Send init successfully";
                ButtonCheck.Image = Properties.de.Resources.RemoteConnect;
            }
            catch (Exception ex)
            {
                Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in SendInit_Click: {ex.Message}.\n", ex);
                StripStatusLabel.Text = "Send init FAILED: " + ex.Message;
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
        private void MenuItemSend_Click(object sender, EventArgs e)
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
            string unencrypted = this.RichTextBoxChat.Text.Replace("\r\n", "\n").Replace("\n", " " + Environment.NewLine);

            if (!string.IsNullOrEmpty(this.ComboBoxIp.Text) && !this.ComboBoxIp.Text.Equals(Constants.ENTER_IP, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    partnerIpAddress = IPAddress.Parse(this.ComboBoxIp.Text);
                    Peer2PeerMsg pmsg = new Peer2PeerMsg(myServerKey);
                    pmsg.Send_CqrPeerMsg(unencrypted, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);

                    chat.AddMyMessage(unencrypted);
                    AppendText(TextBoxSource, unencrypted);
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
        private void MenuItemAttach_Click(object sender, EventArgs e)
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

                            chat.AddMyMessage(mimeAttach.GetFileNameContentLength());
                            AppendText(TextBoxSource, mimeAttach.GetFileNameContentLength() + Environment.NewLine);
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


        public void OnDragNDrop(object sender, EventArgs e)
        {
            if (e is Area23EventArgs<string> ea)
            {
                string t = GetComboBoxText(this.ComboBoxIp);
                IPAddress pi = IPAddress.Parse(t);
                var s = SendAttachment(ea.GenericTData, myServerKey, pi);
            }
        }

        private void MenuItemRefresh_Click(object sender, EventArgs e)
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
        private void MenuItemClear_Click(object sender, EventArgs e)
        {
            // TODO: add warning and saving here
            PlaySoundFromResource("sound_glasses");
            this.TextBoxDestionation.Clear();
            this.TextBoxSource.Clear();
            this.RichTextBoxChat.Clear();
        }

        /// <summary>
        /// SetAttachmentTextLink saves attachment in attachment folder and adds link in <see cref="GroupBoxLinks"/>
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

            GroupBoxLinks.SetNameFilePath(fileName, filePath);
        }


        #endregion OnClientReceive MenuSend MenuAttach MenuRefresh MenuClear


        #region Contacts

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

            if (Settings.Singleton.MyContact != null && Settings.Singleton.MyContact.ContactImage != null && !string.IsNullOrEmpty(Settings.Singleton.MyContact.ContactImage.ImageBase64))
            {
                try
                {
                    Bitmap? bmp = Settings.Singleton.MyContact.ContactImage.ToDrawingBitmap();
                    if (bmp != null)
                        this.PictureBoxYou.Image = bmp;

                    Settings.SaveSettings(Settings.Singleton);
                }
                catch (Exception exBmp)
                {
                    CqrException.SetLastException(exBmp);
                }

                // var badge = new TransparentBadge("My contact added!");
                // badge.ShowDialog();
            }

        }

        private void MenuItemAddContact_Click(object sender, EventArgs e)
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
            string cname = string.Empty, cemail = string.Empty, cmobile = string.Empty, cphone = string.Empty, caddress = string.Empty;
            HashSet<string> names = new HashSet<string>();
            foreach (CqrContact c in Entities.Settings.Singleton.Contacts)
            {
                if (!string.IsNullOrEmpty(c.Name) && !names.Contains(c.Name))
                    names.Add(c.Name);
                contactId = Math.Max(contactId, c.ContactId);
            }
            contactId++;
            FileOpenDialog = FileOpenDialog ?? new OpenFileDialog();
            FileOpenDialog.RestoreDirectory = true;
            FileOpenDialog.AddExtension = false;
            FileOpenDialog.CheckFileExists = true;
            FileOpenDialog.CheckPathExists = true;
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
                                        cname += fields[j] + " ";
                                    }
                                    if (j == 3)
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
                                if (!string.IsNullOrEmpty(cname) && !names.Contains(cname))
                                {
                                    if (!string.IsNullOrEmpty(cemail))
                                    {
                                        CqrContact contact = new CqrContact() { ContactId = contactId++, Name = cname, Email = cemail, Mobile = cmobile };
                                        Entities.Settings.Singleton.Contacts.Add(contact);
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
                                    if (!string.IsNullOrEmpty(cname) && !names.Contains(cname))
                                    {
                                        if (!string.IsNullOrEmpty(cemail))
                                        {
                                            CqrContact contact = new CqrContact() { ContactId = contactId++, Name = cname, Email = cemail, Mobile = cmobile };
                                            Entities.Settings.Singleton.Contacts.Add(contact);
                                        }
                                    }
                                }


                            }

                            Entities.Settings.SaveSettings(Entities.Settings.Singleton);
                            break;
                        default:
                            break;

                    }
                }
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


        internal async Task SetupNetwork()
        {
            List<string> proxyList = new List<string>();
            List<IPAddress> addresses = GetProxiesFromSettingsResources(ref proxyList);
            List<IPAddress> interfaceIPAddrs = await NetworkAddresses.GetIpAddressesAsync();
            List<IPAddress> connectedIPs = await NetworkAddresses.GetConnectedIpAddressesAsync(addresses);

            List<string> myIpStrList = new List<string>();
            int mchecked = 0;
            this.MenuNetworkItemIPv6Secure.Checked = false;
            foreach (IPAddress addr in interfaceIPAddrs)
            {
                if (addr != null)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(addr.AddressFamily + " " + addr.ToString(), null, IPInterfaceAddressSelected, addr.ToString());
                    item.Checked = false;
                    item.BackColor = SystemColors.MenuBar;
                    item.ForeColor = SystemColors.GrayText;

                    if (connectedIPs != null && connectedIPs.Count > 0)
                    {
                        foreach (IPAddress connectedIp in connectedIPs)
                        {
                            if ((Extensions.BytesCompare(addr.GetAddressBytes(), connectedIp.GetAddressBytes()) == 0) &&
                                (addr.AddressFamily == connectedIp.AddressFamily))
                            {
                                item.ForeColor = SystemColors.MenuText;
                                item.BackColor = SystemColors.Menu;

                                if (mchecked++ == 0)
                                {
                                    item.BackColor = SystemColors.MenuHighlight;
                                    clientIpAddress = addr;
                                    item.Checked = true;
                                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                        this.MenuNetworkItemIPv6Secure.Checked = true;
                                    ipSockListener = new Area23.At.Framework.Core.Net.IpSocket.Listener(clientIpAddress, OnClientReceive);
                                }

                                break;
                            }
                        }

                    }

                    myIpStrList.Add(addr.ToString());
                    this.MenuNetworkItemMyIps.DropDownItems.Add(item);
                }
            }

            ToolStripMenuItem extIpItem = new ToolStripMenuItem(ExternalIpAddress.AddressFamily + " " + ExternalIpAddress.ToString(), null, null, ExternalIpAddress.ToString());
            extIpItem.Checked = true;
            extIpItem.Enabled = false;
            this.MenuItemExternalIp.DropDownItems.Add(extIpItem);



            mchecked = 0;
            foreach (IPAddress addrProxy in addresses)
            {
                if (addrProxy != null &&
                    ((addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ||
                    (addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)))
                {
                    proxyList.Add(addrProxy.ToString());


                    ToolStripMenuItem item = new ToolStripMenuItem(addrProxy.AddressFamily + " " + addrProxy.ToString(), null, null, addrProxy.ToString());
                    if ((addrProxy.AddressFamily == ServerIpAddress.AddressFamily) &&
                        (Extensions.BytesCompare(addrProxy.GetAddressBytes(), ServerIpAddress.GetAddressBytes()) == 0))
                    {
                        if (!MenuNetworkItemIPv6Secure.Checked && addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {; }
                        else
                            item.Checked = true;
                    }

                    this.MenuNetworkItemProxyServers.DropDownItems.Add(item);
                }

            }

            foreach (var friendIp in Entities.Settings.Singleton.FriendIPs)
            {
                if (!string.IsNullOrEmpty(friendIp))
                {
                    try
                    {
                        IPAddress ipFriendAddr = IPAddress.Parse(friendIp);
                        if (!MenuNetworkComboBoxFriendIp.Items.Contains(ipFriendAddr.ToString()))
                            MenuNetworkComboBoxFriendIp.Items.Add(ipFriendAddr.ToString());
                        if (!ComboBoxIp.Items.Contains(ipFriendAddr.ToString()))
                            ComboBoxIp.Items.Add(ipFriendAddr.ToString());
                    }
                    catch (Exception exFriendIp)
                    {
                        // TODO: log
                    }
                }
            }

            if (Entities.Settings.Singleton != null)
            {
                Entities.Settings.Singleton.Proxies = proxyList;
                Entities.Settings.Singleton.MyIPs = myIpStrList;
                Entities.Settings.SaveSettings(Entities.Settings.Singleton);
            }

        }


        /// <summary>
        /// IPAdressSelected Delegate is invoked, 
        /// when a different IP Address in context menu is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IPInterfaceAddressSelected(object sender, EventArgs e)
        {
            if (sender != null && sender is ToolStripMenuItem mi)
            {
                foreach (ToolStripMenuItem dditem in this.MenuNetworkItemMyIps.DropDownItems)
                    dditem.Checked = false;

                mi.Checked = true;
                clientIpAddress = IPAddress.Parse(mi.Name);

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
                StripStatusLabel.Text = "Listening on " + clientIpAddress.ToString() + ":" + Constants.CHAT_PORT;
            }
        }


        #region LoadSaveChatContent

        private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            FileOpenDialog = FileOpenDialog ?? new OpenFileDialog();
            FileOpenDialog.RestoreDirectory = true;
            DialogResult result = FileOpenDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                MessageBox.Show($"FileName: {FileOpenDialog.FileName} init directory: {FileOpenDialog.InitialDirectory}", $"{Text} type {FileOpenDialog.GetType()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void toolStripMenuItemLoad_Click(object sender, EventArgs e)
        {
            FileOpenDialog = FileOpenDialog ?? new OpenFileDialog();
            FileOpenDialog.RestoreDirectory = true;
            DialogResult res = FileOpenDialog.ShowDialog();
            if (res == DialogResult.OK)
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
