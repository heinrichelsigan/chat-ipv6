using EU.CqrXs.Framework.Core;
using EU.CqrXs.Framework.Core.Net;
using EU.CqrXs.Framework.Core.Crypt.Cipher;
using EU.CqrXs.Framework.Core.Crypt.Cipher.Symmetric;
using EU.CqrXs.Framework.Core.Crypt.CqrJd;
using EU.CqrXs.Framework.Core.Crypt.EnDeCoding;
using EU.CqrXs.Framework.Core.Net.IpSocket;
using EU.CqrXs.Framework.Core.Net.WebHttp;
using EU.CqrXs.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Properties;
using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using EU.CqrXs.WinForm.SecureChat.Util;
using EU.CqrXs.Framework.Core.Net.NameService;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms
{
    public partial class SecureChat : BaseChatForm
    {
        #region fields        

        private string myServerKey = string.Empty;
        internal static int attachCnt = 0;
        internal static int chatCnt = 0;
        internal static Chat? chat;

        private static IPAddress? clientIpAddress;
        private static IPAddress? partnerIpAddress;
        private static IPSockListener? ipSockListener;

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
                    foreach (string sip in Settings.Instance.Proxies)
                    {
                        if (IPAddress.Parse(sip).Equals(ip))
                        {
                            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 &&
                                menuItemIPv6Secure.Checked)
                            {
                                serverIpAddress = ip;
                                return serverIpAddress;
                            }
                            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                                !menuItemIPv6Secure.Checked)
                            {
                                serverIpAddress = ip;
                                return serverIpAddress;
                            }
                        }
                    }
                }
                foreach (IPAddress ip in list)
                {
                    foreach (string sip in Settings.Instance.Proxies)
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
        public SecureChat() : base()
        {
            InitializeComponent();
            TextBoxSource.MaxLength = Constants.MAX_BYTE_BUFFEER;
            TextBoxDestionation.MaxLength = Constants.MAX_BYTE_BUFFEER;
            ComboBoxIpContact.Text = Constants.ENTER_IP_CONTACT;
            ComboBoxSecretKey.Text = Constants.ENTER_SECRET_KEY;
        }


        private async void SecureChat_Load(object sender, EventArgs e)
        {
            bool send1stReg = false;
            if (Entities.Settings.Load() == null || Entities.Settings.Instance == null || Entities.Settings.Instance.MyContact == null)
            {
                // var badge = new TransparentBadge($"Error reading Settings from {LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE}.");
                // badge.Show();
                MenuItemMyContact_Click(sender, e);
                while (string.IsNullOrEmpty(Entities.Settings.Instance.MyContact.Email) || string.IsNullOrEmpty(Entities.Settings.Instance.MyContact.Name))
                {
                    string notFullReason = string.Empty;
                    if (string.IsNullOrEmpty(Entities.Settings.Instance.MyContact.Name))
                        notFullReason += "Name is missing!" + Environment.NewLine;
                    if (string.IsNullOrEmpty(Entities.Settings.Instance.MyContact.Email))
                        notFullReason += "Email Address is missing!" + Environment.NewLine;
                    // if (string.IsNullOrEmpty(Entities.Settings.Instance.MyContact.Mobile))
                    //     notFullReason += "Mobile phone is missing!" + Environment.NewLine;
                    MessageBox.Show(notFullReason, "Please fill out your info fully", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    MenuItemMyContact_Click(sender, e);
                }
                send1stReg = true;
            }

            toolStripStatusLabel.Text = "Setup Network";
            await SetupNetwork();

            if (Entities.Settings.Instance != null && Entities.Settings.Instance.MyContact != null && !string.IsNullOrEmpty(Entities.Settings.Instance.MyContact.ImageBase64))
            {
                Bitmap? bmp = (Bitmap?)Entities.Settings.Instance.MyContact.ImageBase64.Base64ToImage();
                if (bmp != null)
                    this.PictureBoxYou.Image = bmp;
            }

            if (send1stReg)
                Send_1st_Server_Registration(sender, e);

            AddContactsToIpContact();
            toolStripStatusLabel.Text = "Secure Chat init done.";
        }

        #region thread save text and richtext box access       

        /// <summary>
        /// Displays and formats lines in <see cref="richTextBoxOneView" />
        /// </summary>
        internal void Format_Lines_RichTextBox()
        {
            if (chat != null)
            {
                ClearRichText(richTextBoxOneView);
                int lineIndex = 0;
                foreach (var tuple in chat.CqrMsgs)
                {
                    string line = tuple.Value;

                    AppendRichText(richTextBoxOneView, line + Environment.NewLine);
                    // richTextBoxOneView.AppendText(line + Environment.NewLine);

                    int startPos = GetFirstCharIndexFromLineRichText(richTextBoxOneView, lineIndex++);
                    SelectRichText(richTextBoxOneView, startPos, line.Length + Environment.NewLine.Length);
                    if (chat.MyMsgTStamps.Contains(tuple.Key))
                    {
                        SelectionAlignmentRichText(richTextBoxOneView, HorizontalAlignment.Right);
                    }
                    else if (chat.FriendMsgTStamps.Contains(tuple.Key))
                    {
                        SelectionAlignmentRichText(richTextBoxOneView, HorizontalAlignment.Left);
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

            CqrServerMsg serverMessage = new CqrServerMsg(myServerKey);
            this.TextBoxPipe.Text = serverMessage.symmPipe.PipeString;

        }

        [Obsolete("Button_HashIv_Click is obsolete", false)]
        private void Button_HashIv_Click(object sender, EventArgs e)
        {
            string url = "https://cqrxs.eu/net/R.aspx";
            Uri uri = new Uri(url);
            HttpClient httpClientR = HttpClientRequest.GetHttpClient(url, "cqrxs.eu", Encoding.UTF8);
            Task<HttpResponseMessage> respTask = httpClientR.GetAsync(uri);

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
                return;
            }
            this.ComboBoxSecretKey.BackColor = Color.White;
            ButtonKey_Click(sender, e);
            if (Entities.Settings.Instance != null)
            {
                if (!Entities.Settings.Instance.SecretKeys.Contains(this.ComboBoxSecretKey.Text))
                    Entities.Settings.Instance.SecretKeys.Add(this.ComboBoxSecretKey.Text);
                if (!this.ComboBoxSecretKey.Items.Contains(this.ComboBoxSecretKey.Text))
                    this.ComboBoxSecretKey.Items.Add(this.ComboBoxSecretKey.Text);
            }
            toolStripStatusLabel.Text = "Added new secret key => calculated new SecurePipe...";
        }

        /// <summary>
        /// ComboBoxSecretKey_TextUpdate is fired, when text entered in ComboBoxSecretKey changes.
        /// Event is fired, when 1 char will be added or deleted at each change of <see cref="ComboBoxSecretKey.Text"/>
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void ComboBoxSecretKey_TextUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxSecretKey.Text))
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
                return;
            }
            this.ComboBoxSecretKey.BackColor = Color.White;
            ButtonKey_Click(sender, e);
            toolStripStatusLabel.Text = "Changed secret key => calculated new SecurePipe...";
        }

        #endregion SecretKey & SymmCipherPipe.PipeString + ComboBoxSecretKey FocusLeave TextUpdate SelectedIndexChanged

        #region ComboBoxIpContact FocusLeave TextUpdate SelectedIndexChanged


        private void ComboBoxIpContact_FocusLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxIpContact.Text) ||
                this.ComboBoxIpContact.Text.Equals(Constants.ENTER_IP_CONTACT, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a new ip address!", "Please enter a valid connectable ip address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxIpContact.BackColor = Color.PeachPuff;
                return;
            }
            try
            {
                partnerIpAddress = IPAddress.Parse(this.ComboBoxIpContact.Text);
            } 
            catch (Exception exIpContact)
            {
                MessageBox.Show($"Cannot parse IpAddress from string \"{ComboBoxIpContact.Text}\": {exIpContact.Message}", "Please enter a valid connectable ipv4 or ipv6 address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxIpContact.BackColor = Color.Violet;
                return;
            }

            this.ComboBoxIpContact.BackColor = Color.White;
           
            if (Entities.Settings.Instance != null)
            {
                if (!Entities.Settings.Instance.FriendIPs.Contains(this.ComboBoxIpContact.Text))
                    Entities.Settings.Instance.FriendIPs.Add(partnerIpAddress.ToString());
                if (!this.ComboBoxIpContact.Items.Contains(this.ComboBoxIpContact.Text))
                    this.ComboBoxIpContact.Items.Add(partnerIpAddress.ToString());
            }
            toolStripStatusLabel.Text = $"Added new partner ip address {partnerIpAddress.ToString()}.";
        }

        private void ComboBoxIpContact_TextUpdate(object sender, EventArgs e)
        {

        }

        private void ComboBoxIpContact_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ComboBoxIpContact.Text) ||
                this.ComboBoxIpContact.Text.Equals(Constants.ENTER_IP_CONTACT, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("You haven't entered a new ip address!", "Please enter a valid connectable ip address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxIpContact.BackColor = Color.PeachPuff;
                return;
            }
            try
            {
                partnerIpAddress = IPAddress.Parse(this.ComboBoxIpContact.Text);
            }
            catch (Exception exIpContact)
            {
                MessageBox.Show($"Cannot parse IpAddress from string \"{ComboBoxIpContact.Text}\": {exIpContact.Message}", "Please enter a valid connectable ipv4 or ipv6 address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ComboBoxIpContact.BackColor = Color.Violet;
                return;
            }
            this.ComboBoxIpContact.BackColor = Color.White;
            toolStripStatusLabel.Text = $"Selected partner ip address {partnerIpAddress.ToString()}.";
        }

        #endregion ComboBoxIpContact FocusLeave TextUpdate SelectedIndexChanged


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

            CqrServerMsg serverMessage = new CqrServerMsg(myServerKey);
            this.TextBoxPipe.Text = serverMessage.symmPipe.PipeString;

            Contact myContact = Entities.Settings.Instance.MyContact;
            string plain = myContact.Name + Environment.NewLine + myContact.Email + Environment.NewLine +
                myContact.Mobile + Environment.NewLine + myContact.Address + Environment.NewLine +
                myContact.SecretKey + Environment.NewLine;
            string encrypted = serverMessage.CqrMessage(plain);
            string response = serverMessage.SendCqrSrvMsg(plain, ServerIpAddress);

            this.TextBoxSource.Text = encrypted + "\n"; //  + "\r\n" + serverMessage.symmPipe.HexStages;
            string decrypted = serverMessage.NCqrMessage(encrypted);
            this.TextBoxDestionation.Text = decrypted + "\n" + response + "\r\n"; // + serverMessage.symmPipe.HexStages;

            chat.AddMyMessage(plain);
            chat.AddFriendMessage(decrypted);

            // this.richTextBoxOneView.Rtf = this.RichTextBoxChat.Rtf;
            Format_Lines_RichTextBox();

            toolStripStatusLabel.Text = "Finished 1st registration";
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
                if (!string.IsNullOrEmpty(this.ComboBoxSecretKey.Text) &&
                    !this.ComboBoxSecretKey.Text.Equals(Constants.ENTER_SECRET_KEY, StringComparison.InvariantCultureIgnoreCase))
                {
                    myServerKey = this.ComboBoxSecretKey.Text;
                }
            }

            if (sender != null)
            {
                if (ipSockListener?.BufferedData != null && ipSockListener.BufferedData.Length > 0)
                {
                    if (chat == null)
                        chat = new Chat(0);
                    string encrypted = EnDeCoder.GetString(ipSockListener.BufferedData);

                    Area23EventArgs<IpSockReceiveData>? area23EvArgs = null;
                    if (e != null && e is Area23EventArgs<IpSockReceiveData>)
                    {
                        area23EvArgs = ((Area23EventArgs<IpSockReceiveData>)e);
                        //TODO: Enable cross thread via delegate
                        SetStatusText(toolStripStatusLabel, "Connection from " + area23EvArgs.GenericTData.ClientIPAddr + ":" + area23EvArgs.GenericTData.ClientIPPort);
                        // toolStripStatusLabel.Text = "Connection from " + area23EvArgs.GenericTData.ClientIPAddr + ":" + area23EvArgs.GenericTData.ClientIPPort;
                        // if (!this.ComboBoxIpContact.Text.Equals(area23EvArgs.GenericTData.ClientIPAddr, StringComparison.InvariantCultureIgnoreCase))
                        //     this.ComboBoxIpContact.Text = area23EvArgs.GenericTData.ClientIPAddr;
                        encrypted = EnDeCoder.GetString(area23EvArgs.GenericTData.BufferedData);
                    }
                    //string encrypt = string.Empty;
                    //bool trimmed = false;
                    //int l = encrypted.Length - 1;
                    //while (!trimmed)
                    //{
                    //    if (encrypted[l] == '\0')
                    //        l--;
                    //    else
                    //    {
                    //        trimmed = true;
                    //        encrypt = encrypted.Substring(0, l);
                    //    }
                    //}

                    CqrPeer2PeerMsg pmsg = new CqrPeer2PeerMsg(myServerKey);
                    string unencrypted = pmsg.NCqrPeerMsg(encrypted);
                    string friendMsg = string.Empty;
                    if (unencrypted.StartsWith("Content-Type: ") || unencrypted.Contains("Content-Verification:"))
                    {
                        MimeAttachment mimeAttachment = MimeAttachment.GetBase64Attachment(unencrypted);
                        SetAttachmentTextLink(mimeAttachment);
                        friendMsg = unencrypted.Substring(0, unencrypted.IndexOf("Content-Verification: "));
                    }
                    else
                    {
                        friendMsg = unencrypted;                        
                    }

                    chat.AddFriendMessage(friendMsg);
                    AppendText(TextBoxDestionation, friendMsg);
                    // this.richTextBoxOneView.Text = unencrypted;
                    Format_Lines_RichTextBox();
                }
            }
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
                return;
            }

            myServerKey = this.ComboBoxSecretKey.Text;

            string unencrypted = this.RichTextBoxChat.Text;
            
            try
            {
                partnerIpAddress = IPAddress.Parse(this.ComboBoxIpContact.Text);
                CqrPeer2PeerMsg pmsg = new CqrPeer2PeerMsg(myServerKey);
                pmsg.SendCqrPeerMsg(unencrypted, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);

                chat.AddMyMessage(unencrypted);
                AppendText(TextBoxSource, unencrypted);
                Format_Lines_RichTextBox();
                this.RichTextBoxChat.Text = string.Empty;
                toolStripStatusLabel.Text = "Send successfully";
            }
            catch (Exception ex)
            {
                Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in menuItemSend_Click: {ex.Message}.\n", ex);
                toolStripStatusLabel.Text = "Send FAILED: " + ex.Message;
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
                return;
            }

            myServerKey = this.ComboBoxSecretKey.Text;

            openFileDialog = openFileDialog ?? new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.AddExtension = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "BMP (*.bmp)|*.bmp|PNG (*.png)|*.png|GIF (*.gif)|*.gif|JPG (*.jpg)|*.jpg|PDF (*.pdf)|*.pdf|All files (*.*)|*.*";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                if (File.Exists(openFileDialog.FileName))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                    string fileNameOnly = Path.GetFileName(openFileDialog.FileName);
                    string mimeType = Framework.Core.Util.MimeType.GetMimeType(fileBytes, fileNameOnly);
                    string base64Mime = Base64.Encode(fileBytes);
                    
                    CqrPeer2PeerMsg pmsg = new CqrPeer2PeerMsg(myServerKey);
                    string unencrypted = MimeAttachment.GetMimeMessage(fileNameOnly, mimeType, base64Mime, pmsg.symmPipe.PipeString);
                    
                    try
                    {
                        partnerIpAddress = IPAddress.Parse(this.ComboBoxIpContact.Text);
                        
                        pmsg.SendCqrPeerMsg(unencrypted, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);
                        // pmsg.SendCqrPeerAttachment(fileNameOnly, mimeType, base64Mime, partnerIpAddress, EncodingType.Base64, Constants.CHAT_PORT);

                        chat.AddMyMessage(unencrypted);
                        AppendText(TextBoxSource, unencrypted);
                        Format_Lines_RichTextBox();
                        this.RichTextBoxChat.Text = string.Empty;
                        toolStripStatusLabel.Text = $"File {fileNameOnly} send successfully!";
                    }
                    catch (Exception ex)
                    {
                        Area23Log.Logger.LogOriginMsgEx(this.Name, $"Exception in menuItemSend_Click: {ex.Message}.\n", ex);
                        toolStripStatusLabel.Text = "Send FAILED: " + ex.Message;
                    }
                }
                // otherwise send message to registered user via server
                // Always encrypt via key
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
            this.TextBoxDestionation.Clear();
            this.TextBoxSource.Clear();
            this.RichTextBoxChat.Clear();
        }


        protected internal void SetAttachmentTextLink(MimeAttachment mimeAttachment)
        {
            int attachNum = ((attachCnt % 8) + 1);
            LinkLabel linkLabelAttachment0 = new LinkLabel() { Name = $"linkLabelAttachment{attachNum}" };
            if (!Directory.Exists(LibPaths.AttachmentFilesDir))
                Directory.CreateDirectory(LibPaths.AttachmentFilesDir);


            foreach (System.Windows.Forms.Control ctrl in groupBoxAttachments.Controls)
            {
                if (ctrl != null && ctrl is LinkLabel lbAttach &&
                    (ctrl.Name.EndsWith(attachNum.ToString()) || ctrl.Name.Equals("linkLabelAttachment" + attachNum)))
                {
                    linkLabelAttachment0 = (LinkLabel)lbAttach;
                    linkLabelAttachment0.Name = $"linkLabelAttachment{attachNum}";
                    SetLinkLabelVisible(linkLabelAttachment0, true);
                    break; // we got the next LinkLabel attachment in modulo slot
                }
            }

            string filePath = Path.Combine(LibPaths.AttachmentFilesDir, mimeAttachment.FileName);
            byte[] fileBytes = Framework.Core.Crypt.EnDeCoding.Base64.Decode(mimeAttachment.Base64Mime);
            System.IO.File.WriteAllBytes(filePath, fileBytes);
            Uri uri = new Uri("file://" + filePath);
            SetLinkLabelText(linkLabelAttachment0, mimeAttachment.FileName);
            AddLinkLabelLinks(linkLabelAttachment0, filePath);
            linkLabelAttachment0.LinkClicked += LinkLabel_LinkClicked;
            ++attachCnt;

        }

        protected internal void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender != null && e.Link != null && e.Link.LinkData != null && File.Exists(e.Link.LinkData.ToString()))
            {
                ProcessCmd.Execute("explorer", e.Link.LinkData.ToString());
            }
        }

        #endregion OnClientReceive MenuSend MenuAttach MenuRefresh MenuClear


        #region Contacts

        private void AddContactsToIpContact()
        {
            string ipContact = this.ComboBoxIpContact.Text;
            this.ComboBoxIpContact.Items.Clear();
            foreach (Contact ct in Entities.Settings.Instance.Contacts)
            {
                if (ct != null && !string.IsNullOrEmpty(ct.NameEmail))
                    this.ComboBoxIpContact.Items.Add(ct.NameEmail);
            }
            this.ComboBoxIpContact.Text = ipContact;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemMyContact_Click(object sender, EventArgs e)
        {
            ContactSettings contactSettings = new ContactSettings("My Contact Info", 0);
            contactSettings.ShowInTaskbar = true;
            contactSettings.ShowDialog();

            if (Settings.Instance.MyContact != null)
            {
                string base64image = Settings.Instance.MyContact.ImageBase64 ?? string.Empty;

                if (!string.IsNullOrEmpty(base64image))
                {
                    try
                    {
                        Bitmap? bmp;
                        byte[] bytes = Base64.Decode(base64image);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            bmp = new Bitmap(ms);
                        }
                        if (bmp != null)
                            this.PictureBoxYou.Image = bmp;

                    }
                    catch (Exception exBmp)
                    {
                        CqrException.LastException = exBmp;
                    }

                    // var badge = new TransparentBadge("My contact added!");
                    // badge.ShowDialog();
                }

            }

        }

        private void MenuItemAddContact_Click(object sender, EventArgs e)
        {
            ContactSettings contactSettings = new ContactSettings("Add Contact Info", 1);
            contactSettings.ShowInTaskbar = true;
            contactSettings.ShowDialog();

            AddContactsToIpContact();
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
            menuViewItemLeftRíght.Checked = false;
            menuViewItemTopBottom.Checked = true;
            menuViewItem1View.Checked = false;

            PanelCenter.Visible = false;

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
            menuViewItemLeftRíght.Checked = true;
            menuViewItemTopBottom.Checked = false;
            menuViewItem1View.Checked = false;

            PanelCenter.Visible = false;

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
            menuViewItemLeftRíght.Checked = false;
            menuViewItemTopBottom.Checked = false;
            menuViewItem1View.Checked = true;

            PanelCenter.Visible = true;
            PanelCenter.BringToFront();
            SplitChatView.Visible = false;
            richTextBoxOneView.Visible = true;
        }

        #endregion SplitChatWindowLayout


        internal async Task SetupNetwork()
        {

            List<IPAddress> addresses = new List<IPAddress>();
            string[] proxyStrs = Resources.Proxies.Split(";,".ToCharArray());
            foreach (string proxyStr in proxyStrs)
            {
                try
                {
                    IPAddress ip = IPAddress.Parse(proxyStr);
                    addresses.Add(ip);

                }
                catch (Exception ex)
                {
                    CqrException.LastException = ex;
                    Area23Log.LogStatic(ex);
                }
            }
            string[] proxyNameStrs = Resources.ProxyNames.Split(";,".ToCharArray());
            List<string> proxyList = new List<string>();
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
                    CqrException.LastException = ex;
                    Area23Log.LogStatic(ex);
                }
            }


            List<IPAddress> interfaceIPAddrs = await NetworkAddresses.GetIpAddressesAsync();
            List<IPAddress> connectedIPs = await NetworkAddresses.GetConnectedIpAddressesAsync(addresses);

            List<string> myIpStrList = new List<string>();
            int mchecked = 0;
            this.menuItemIPv6Secure.Checked = false;
            foreach (IPAddress addr in interfaceIPAddrs)
            {
                if (addr != null)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(addr.AddressFamily + " " + addr.ToString(), null, IPAddressSelected, addr.ToString());
                    item.Checked = false;

                    if (connectedIPs != null && connectedIPs.Count > 0 &&
                        Extensions.BytesCompare(addr.GetAddressBytes(), connectedIPs.ElementAt(0).GetAddressBytes()) == 0 &&
                        addr.AddressFamily == connectedIPs.ElementAt(0).AddressFamily)
                    {
                        if (mchecked++ == 0)
                        {
                            clientIpAddress = addr;
                            item.Checked = true;
                            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                this.menuItemIPv6Secure.Checked = true;
                            ipSockListener = new EU.CqrXs.Framework.Core.Net.IpSocket.IPSockListener(clientIpAddress, OnClientReceive);
                        }
                    }

                    myIpStrList.Add(addr.ToString());
                    this.menuIItemMyIps.DropDownItems.Add(item);
                }
            }

            ToolStripMenuItem extIpItem = new ToolStripMenuItem(ExternalIpAddress.AddressFamily + " " + ExternalIpAddress.ToString(), null, null, ExternalIpAddress.ToString());
            extIpItem.Checked = true;
            extIpItem.Enabled = false;
            this.menuItemExternalIp.DropDownItems.Add(extIpItem);

            foreach (IPAddress addrProxy in addresses)
            {
                if (addrProxy != null)
                {
                    proxyList.Add(addrProxy.ToString());
                    if (addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ||
                        (addrProxy.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && this.menuItemIPv6Secure.Checked))
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem(addrProxy.AddressFamily + " " + addrProxy.ToString(), null, null, addrProxy.ToString());
                        this.menuItemProxyServers.DropDownItems.Add(item);
                    }
                }
            }

            if (Entities.Settings.Instance != null)
            {
                Entities.Settings.Instance.Proxies = proxyList;
                Entities.Settings.Instance.MyIPs = myIpStrList;
                Entities.Settings.Save(Entities.Settings.Instance);
            }

        }


        /// <summary>
        /// IPAdressSelected Delegate is invoked, 
        /// when a different IP Address in context menu is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IPAddressSelected(object sender, EventArgs e)
        {
            if (sender != null && sender is ToolStripMenuItem mi)
            {
                foreach (ToolStripMenuItem dditem in this.menuIItemMyIps.DropDownItems)
                    dditem.Checked = false;

                mi.Checked = true;
                clientIpAddress = IPAddress.Parse(mi.Name);

                ipSockListener?.Dispose();
                ipSockListener = new EU.CqrXs.Framework.Core.Net.IpSocket.IPSockListener(clientIpAddress, OnClientReceive);
                toolStripStatusLabel.Text = "Listening on " + clientIpAddress.ToString() + ":" + Constants.CHAT_PORT;
            }
        }


        #region LoadSaveChatContent

        private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            openFileDialog = openFileDialog ?? new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                MessageBox.Show($"FileName: {openFileDialog.FileName} init directory: {openFileDialog.InitialDirectory}", $"{Text} type {openFileDialog.GetType()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void toolStripMenuItemLoad_Click(object sender, EventArgs e)
        {
            openFileDialog = openFileDialog ?? new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                MessageBox.Show($"FileName: {openFileDialog.FileName} init directory: {openFileDialog.InitialDirectory}", $"{Text} type {openFileDialog.GetType()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        protected internal virtual void toolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            SafeFileName();
        }

        protected virtual byte[] OpenCryptFileDialog(ref string loadDir)
        {
            if (openFileDialog == null)
                openFileDialog = new OpenFileDialog();
            byte[] fileBytes;
            if (string.IsNullOrEmpty(loadDir))
                loadDir = Environment.GetEnvironmentVariable("TEMP") ?? System.AppDomain.CurrentDomain.BaseDirectory;
            if (loadDir != null)
            {
                openFileDialog.InitialDirectory = loadDir;
                openFileDialog.RestoreDirectory = true;
            }
            DialogResult diaOpenRes = openFileDialog.ShowDialog();
            if (diaOpenRes == DialogResult.OK || diaOpenRes == DialogResult.Yes)
            {
                if (!string.IsNullOrEmpty(openFileDialog.FileName) && File.Exists(openFileDialog.FileName))
                {
                    loadDir = Path.GetDirectoryName(openFileDialog.FileName) ?? System.AppDomain.CurrentDomain.BaseDirectory;
                    fileBytes = File.ReadAllBytes(openFileDialog.FileName);
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
                saveFileDialog.InitialDirectory = saveDir;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = ext;
            }
            saveFileDialog.FileName = fileName;
            DialogResult diaRes = saveFileDialog.ShowDialog();
            if (diaRes == DialogResult.OK || diaRes == DialogResult.Yes)
            {
                if (content != null && content.Length > 0)
                    System.IO.File.WriteAllBytes(saveFileDialog.FileName, content);

                // var badge = new TransparentBadge($"File {fileName} saved to directory {saveDir}.");
                // badge.Show();
            }

            return (saveFileDialog != null && saveFileDialog.FileName != null && File.Exists(saveFileDialog.FileName)) ? saveFileDialog.FileName : null;
        }

        #endregion LoadSaveChatContent


    }

}
