using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EU.CqrXs.WinForm.SecureChat.Controls.Forms.Base;
using Area23.At.Framework.Core.Static;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{

    /// <summary>
    /// MenuChat inherited from <see cref="BaseChatForm"/> is abstraction for all menu items provided by derived classed,
    /// e.g. <see cref="SecureChat"/>, <see cref="RichTextChat"/>, <see cref="Peer2PeerChat"/>
    /// </summary>
    public partial  class ChatMenuForm : BaseChatForm
    {

        #region variable fields

        protected internal static bool send1stReg = false;

        #endregion variable fields


        #region win form component fields
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected internal MenuStrip StripMenu;
        protected internal StatusStrip StripStatus;

        protected internal ToolStripMenuItem MenuFile;
        protected internal ToolStripMenuItem MenuFileItemOpen;
        protected internal ToolStripMenuItem MenuFileItemSave;
        protected internal ToolStripMenuItem MenuFileItemExit;
        protected internal ToolStripSeparator MenuFileSeparatorExit;
        
        protected internal ToolStripMenuItem MenuView;
        protected internal ToolStripMenuItem MenuViewItemLeftRíght;
        protected internal ToolStripMenuItem MenuViewItemTopBottom;
        protected internal ToolStripMenuItem MenuViewItem1View;

        protected internal ToolStripMenuItem MenuNetwork;
        protected internal ToolStripMenuItem MenuNetworkItemMyIps;
        protected internal ToolStripMenuItem MenuItemExternalIp;
        protected internal ToolStripMenuItem MenuItemFriendIp;
        protected internal ToolStripComboBox MenuNetworkComboBoxFriendIp;
        protected internal ToolStripMenuItem MenuNetworkItemProxyServers;
        protected internal ToolStripMenuItem MenuNetworkItemIPv6Secure;
        protected internal ToolStripSeparator MenuNetworkSeparatorIp;

        protected internal ToolStripMenuItem MenuCommands;
        protected internal ToolStripMenuItem MenuCommandsItemSend;
        protected internal ToolStripMenuItem MenuCommandsItemRefresh;
        protected internal ToolStripMenuItem MenuCommandsItemClear;
        protected internal ToolStripMenuItem MenuCommandsItemAttach;
        protected internal ToolStripSeparator MenuCommandsSeperator;

        protected internal ToolStripMenuItem MenuContacts;
        protected internal ToolStripMenuItem MenuContactstemImport;
        protected internal ToolStripMenuItem MenuContactsItemAdd;
        protected internal ToolStripMenuItem MenuContactsItemView;
        protected internal ToolStripMenuItem MenuContactsItemMe;        
        protected internal ToolStripMenuItem MenuContactstemExport;
        protected internal ToolStripSeparator MenuContactsSeparetor;

        protected internal ToolStripMenuItem MenuOptions;
        protected internal ToolStripMenuItem MenuOptionsItemCompress;
        protected internal ToolStripMenuItem MenuOptionsItemFileSecure;
        protected internal ToolStripMenuItem MenuOptionsItemClearAllOnClose;
        protected internal ToolStripMenuItem MenuOptionsItemDontSendProfilePictures;

        protected internal ToolStripMenuItem MenuHelp;
        protected internal ToolStripMenuItem MenuHelpItemViewHelp;
        protected internal ToolStripMenuItem MenuHelpItemInfo;
        protected internal ToolStripMenuItem MenuHelpItemAbout;

        protected internal ToolStripStatusLabel StripStatusLabel;
        protected internal ToolStripProgressBar StripProgressBar;

        #endregion win form component fields

        #region constructor InitializeComponent async MenuChat_Load startup eventhanlder

        /// <summary>
        /// Ctor
        /// </summary>
        public ChatMenuForm() : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// MenuChatForm_Load first load event to add own contact
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected internal virtual async void MenuChatForm_Load(object sender, EventArgs e)
        {            

            if (Entities.Settings.LoadSettings() == null || Entities.Settings.Singleton == null || Entities.Settings.Singleton.MyContact == null)
            {
                // var badge = new TransparentBadge($"Error reading Settings from {LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE}.");
                // badge.Show();
                MenuContactsItemMyContact_Click(sender, e);                
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

                    MenuContactsItemMyContact_Click(sender, e);
                }

                send1stReg = true;
            }
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            StripMenu = new MenuStrip();
            MenuFile = new ToolStripMenuItem();
            MenuFileItemOpen = new ToolStripMenuItem();
            MenuFileItemSave = new ToolStripMenuItem();
            MenuFileSeparatorExit = new ToolStripSeparator();
            MenuFileItemExit = new ToolStripMenuItem();
            MenuView = new ToolStripMenuItem();
            MenuViewItemLeftRíght = new ToolStripMenuItem();
            MenuViewItemTopBottom = new ToolStripMenuItem();
            MenuViewItem1View = new ToolStripMenuItem();
            MenuNetwork = new ToolStripMenuItem();
            MenuNetworkItemMyIps = new ToolStripMenuItem();
            MenuItemExternalIp = new ToolStripMenuItem();
            MenuItemFriendIp = new ToolStripMenuItem();
            MenuNetworkComboBoxFriendIp = new ToolStripComboBox();
            MenuNetworkItemProxyServers = new ToolStripMenuItem();
            MenuNetworkSeparatorIp = new ToolStripSeparator();
            MenuNetworkItemIPv6Secure = new ToolStripMenuItem();
            MenuCommands = new ToolStripMenuItem();
            MenuCommandsItemSend = new ToolStripMenuItem();
            MenuCommandsItemAttach = new ToolStripMenuItem();
            MenuCommandsSeperator = new ToolStripSeparator();
            MenuCommandsItemRefresh = new ToolStripMenuItem();
            MenuCommandsItemClear = new ToolStripMenuItem();
            MenuContacts = new ToolStripMenuItem();
            MenuContactsItemMe = new ToolStripMenuItem();
            MenuContactsItemAdd = new ToolStripMenuItem();
            MenuContactsItemView = new ToolStripMenuItem();
            MenuContactsSeparetor = new ToolStripSeparator();
            MenuContactstemImport = new ToolStripMenuItem();
            MenuContactstemExport = new ToolStripMenuItem();
            MenuOptions = new ToolStripMenuItem();
            MenuOptionsItemCompress = new ToolStripMenuItem();
            MenuOptionsItemFileSecure = new ToolStripMenuItem();
            MenuOptionsItemClearAllOnClose = new ToolStripMenuItem();
            MenuOptionsItemDontSendProfilePictures = new ToolStripMenuItem();
            MenuHelp = new ToolStripMenuItem();
            MenuHelpItemViewHelp = new ToolStripMenuItem();
            MenuHelpItemInfo = new ToolStripMenuItem();
            MenuHelpItemAbout = new ToolStripMenuItem();
            StripStatus = new StatusStrip();
            StripStatusLabel = new ToolStripStatusLabel();
            StripProgressBar = new ToolStripProgressBar();
            StripMenu.SuspendLayout();
            StripStatus.SuspendLayout();
            SuspendLayout();
            // 
            // StripMenu
            // 
            StripMenu.AllowItemReorder = true;
            StripMenu.BackColor = SystemColors.MenuBar;
            StripMenu.Font = new Font("Lucida Sans Unicode", 10F);
            StripMenu.GripStyle = ToolStripGripStyle.Visible;
            StripMenu.Items.AddRange(new ToolStripItem[] { MenuFile, MenuView, MenuNetwork, MenuCommands, MenuContacts, MenuOptions, MenuHelp });
            StripMenu.Location = new Point(0, 0);
            StripMenu.Name = "StripMenu";
            StripMenu.RenderMode = ToolStripRenderMode.System;
            StripMenu.Size = new Size(996, 25);
            StripMenu.TabIndex = 1;
            StripMenu.Text = "StripMenu";
            // 
            // MenuFile
            // 
            MenuFile.BackColor = SystemColors.MenuBar;
            MenuFile.DropDownItems.AddRange(new ToolStripItem[] { MenuFileItemOpen, MenuFileItemSave, MenuFileSeparatorExit, MenuFileItemExit });
            MenuFile.ForeColor = SystemColors.MenuText;
            MenuFile.Name = "MenuFile";
            MenuFile.Padding = new Padding(3, 0, 3, 0);
            MenuFile.ShortcutKeys = Keys.Alt | Keys.F;
            MenuFile.Size = new Size(73, 21);
            MenuFile.Text = "cqr chat";
            // 
            // MenuFileItemOpen
            // 
            MenuFileItemOpen.AutoToolTip = true;
            MenuFileItemOpen.BackColor = SystemColors.MenuBar;
            MenuFileItemOpen.BackgroundImageLayout = ImageLayout.Center;
            MenuFileItemOpen.Enabled = false;
            MenuFileItemOpen.ForeColor = SystemColors.MenuText;
            MenuFileItemOpen.Margin = new Padding(1);
            MenuFileItemOpen.Name = "MenuFileItemOpen";
            MenuFileItemOpen.ShortcutKeys = Keys.Control | Keys.O;
            MenuFileItemOpen.Size = new Size(206, 22);
            MenuFileItemOpen.Text = "open chats";
            MenuFileItemOpen.TextImageRelation = TextImageRelation.TextAboveImage;
            MenuFileItemOpen.ToolTipText = "imports saved chats from a file";
            MenuFileItemOpen.Click += MenuFileItemOpen_Click;
            // 
            // MenuFileItemSave
            // 
            MenuFileItemSave.AutoToolTip = true;
            MenuFileItemSave.BackColor = SystemColors.MenuBar;
            MenuFileItemSave.BackgroundImageLayout = ImageLayout.Center;
            MenuFileItemSave.Enabled = false;
            MenuFileItemSave.ForeColor = SystemColors.MenuText;
            MenuFileItemSave.Margin = new Padding(1);
            MenuFileItemSave.Name = "MenuFileItemSave";
            MenuFileItemSave.ShortcutKeys = Keys.Control | Keys.S;
            MenuFileItemSave.Size = new Size(206, 22);
            MenuFileItemSave.Text = "save chats";
            MenuFileItemSave.TextImageRelation = TextImageRelation.TextAboveImage;
            MenuFileItemSave.ToolTipText = "saves chats on local harddisk not on server";
            MenuFileItemSave.Click += MenuFileItemSave_Click;
            // 
            // MenuFileSeparatorExit
            // 
            MenuFileSeparatorExit.BackColor = SystemColors.MenuBar;
            MenuFileSeparatorExit.ForeColor = SystemColors.MenuText;
            MenuFileSeparatorExit.Margin = new Padding(1);
            MenuFileSeparatorExit.Name = "MenuFileSeparatorExit";
            MenuFileSeparatorExit.Size = new Size(203, 6);
            // 
            // MenuFileItemExit
            // 
            MenuFileItemExit.BackColor = SystemColors.MenuBar;
            MenuFileItemExit.BackgroundImageLayout = ImageLayout.Center;
            MenuFileItemExit.ForeColor = SystemColors.MenuText;
            MenuFileItemExit.Name = "MenuFileItemExit";
            MenuFileItemExit.ShortcutKeys = Keys.Alt | Keys.F4;
            MenuFileItemExit.Size = new Size(206, 22);
            MenuFileItemExit.Text = "exit";
            MenuFileItemExit.ToolTipText = "exit chat application";
            MenuFileItemExit.Click += MenuFileItemExit_Click;
            // 
            // MenuView
            // 
            MenuView.BackColor = SystemColors.MenuBar;
            MenuView.BackgroundImageLayout = ImageLayout.None;
            MenuView.DropDownItems.AddRange(new ToolStripItem[] { MenuViewItemLeftRíght, MenuViewItemTopBottom, MenuViewItem1View });
            MenuView.ForeColor = SystemColors.MenuText;
            MenuView.ImageScaling = ToolStripItemImageScaling.None;
            MenuView.Name = "MenuView";
            MenuView.Padding = new Padding(3, 0, 3, 0);
            MenuView.ShortcutKeys = Keys.Alt | Keys.V;
            MenuView.Size = new Size(48, 21);
            MenuView.Text = "view";
            // 
            // MenuViewItemLeftRíght
            // 
            MenuViewItemLeftRíght.BackColor = SystemColors.MenuBar;
            MenuViewItemLeftRíght.Checked = true;
            MenuViewItemLeftRíght.CheckState = CheckState.Checked;
            MenuViewItemLeftRíght.Name = "MenuViewItemLeftRíght";
            MenuViewItemLeftRíght.ShortcutKeys = Keys.Alt | Keys.L;
            MenuViewItemLeftRíght.Size = new Size(204, 22);
            MenuViewItemLeftRíght.Text = "left-ríght";
            MenuViewItemLeftRíght.ToolTipText = "shows a left->right splitted chat view";
            MenuViewItemLeftRíght.Click += MenuView_ItemLeftRíght_Click;
            // 
            // MenuViewItemTopBottom
            // 
            MenuViewItemTopBottom.BackColor = SystemColors.MenuBar;
            MenuViewItemTopBottom.Name = "MenuViewItemTopBottom";
            MenuViewItemTopBottom.ShortcutKeys = Keys.Alt | Keys.T;
            MenuViewItemTopBottom.Size = new Size(204, 22);
            MenuViewItemTopBottom.Text = "top-bottom";
            MenuViewItemTopBottom.ToolTipText = "top-bottom shows a top->bottom splitted chat view";
            MenuViewItemTopBottom.Click += MenuView_ItemTopBottom_Click;
            // 
            // MenuViewItem1View
            // 
            MenuViewItem1View.BackColor = SystemColors.MenuBar;
            MenuViewItem1View.Name = "MenuViewItem1View";
            MenuViewItem1View.ShortcutKeys = Keys.Alt | Keys.D1;
            MenuViewItem1View.Size = new Size(204, 22);
            MenuViewItem1View.Text = "1-view";
            MenuViewItem1View.ToolTipText = "displays a single box chat view";
            MenuViewItem1View.Click += MenuView_Item1View_Click;
            // 
            // MenuNetwork
            // 
            MenuNetwork.BackColor = SystemColors.MenuBar;
            MenuNetwork.DropDownItems.AddRange(new ToolStripItem[] { MenuNetworkItemMyIps, MenuItemFriendIp, MenuNetworkItemProxyServers, MenuNetworkSeparatorIp, MenuNetworkItemIPv6Secure });
            MenuNetwork.Name = "MenuNetwork";
            MenuNetwork.ShortcutKeys = Keys.Alt | Keys.N;
            MenuNetwork.Size = new Size(76, 21);
            MenuNetwork.Text = "network";
            MenuNetwork.ToolTipText = "network provides ip addresses & secure things";
            // 
            // MenuNetworkItemMyIps
            // 
            MenuNetworkItemMyIps.BackColor = SystemColors.MenuBar;
            MenuNetworkItemMyIps.DropDownItems.AddRange(new ToolStripItem[] { MenuItemExternalIp });
            MenuNetworkItemMyIps.Name = "MenuNetworkItemMyIps";
            MenuNetworkItemMyIps.Size = new Size(177, 22);
            MenuNetworkItemMyIps.Text = "my ip's";
            // 
            // MenuItemExternalIp
            // 
            MenuItemExternalIp.BackColor = SystemColors.MenuBar;
            MenuItemExternalIp.Name = "MenuItemExternalIp";
            MenuItemExternalIp.Size = new Size(160, 22);
            MenuItemExternalIp.Text = "External Ip's";
            // 
            // MenuItemFriendIp
            // 
            MenuItemFriendIp.BackColor = SystemColors.MenuBar;
            MenuItemFriendIp.DropDownItems.AddRange(new ToolStripItem[] { MenuNetworkComboBoxFriendIp });
            MenuItemFriendIp.Name = "MenuItemFriendIp";
            MenuItemFriendIp.Size = new Size(177, 22);
            MenuItemFriendIp.Text = "friend ip's";
            MenuItemFriendIp.ToolTipText = "You can enter here directly friend ip's, if your connection is free of SNAT/DNAT";
            // 
            // MenuNetworkComboBoxFriendIp
            // 
            MenuNetworkComboBoxFriendIp.BackColor = SystemColors.ControlLightLight;
            MenuNetworkComboBoxFriendIp.Name = "MenuNetworkComboBoxFriendIp";
            MenuNetworkComboBoxFriendIp.Size = new Size(121, 23);
            // 
            // MenuNetworkItemProxyServers
            // 
            MenuNetworkItemProxyServers.BackColor = SystemColors.MenuBar;
            MenuNetworkItemProxyServers.Name = "MenuNetworkItemProxyServers";
            MenuNetworkItemProxyServers.Size = new Size(177, 22);
            MenuNetworkItemProxyServers.Text = "proxies";
            MenuNetworkItemProxyServers.ToolTipText = "proxies are needed mainly to connect to people, where no endpoint to endpoint ip connection is possible";
            // 
            // MenuNetworkSeparatorIp
            // 
            MenuNetworkSeparatorIp.BackColor = SystemColors.MenuBar;
            MenuNetworkSeparatorIp.ForeColor = SystemColors.ActiveBorder;
            MenuNetworkSeparatorIp.Name = "MenuNetworkSeparatorIp";
            MenuNetworkSeparatorIp.Size = new Size(174, 6);
            // 
            // MenuNetworkItemIPv6Secure
            // 
            MenuNetworkItemIPv6Secure.BackColor = SystemColors.MenuBar;
            MenuNetworkItemIPv6Secure.Name = "MenuNetworkItemIPv6Secure";
            MenuNetworkItemIPv6Secure.ShortcutKeys = Keys.Control | Keys.D6;
            MenuNetworkItemIPv6Secure.Size = new Size(177, 22);
            MenuNetworkItemIPv6Secure.Text = "ip6 cqr";
            MenuNetworkItemIPv6Secure.ToolTipText = "you can check it only, when you have an ipv6 address and you want to chat only to partners, where ip6 connect is possible";
            // 
            // MenuCommands
            // 
            MenuCommands.BackColor = SystemColors.MenuBar;
            MenuCommands.DropDownItems.AddRange(new ToolStripItem[] { MenuCommandsItemSend, MenuCommandsItemAttach, MenuCommandsSeperator, MenuCommandsItemRefresh, MenuCommandsItemClear });
            MenuCommands.Name = "MenuCommands";
            MenuCommands.Size = new Size(128, 21);
            MenuCommands.Text = "chat commands";
            MenuCommands.ToolTipText = "possible chat commands";
            // 
            // MenuCommandsItemSend
            // 
            MenuCommandsItemSend.BackColor = SystemColors.MenuBar;
            MenuCommandsItemSend.Name = "MenuCommandsItemSend";
            MenuCommandsItemSend.ShortcutKeys = Keys.Control | Keys.S;
            MenuCommandsItemSend.Size = new Size(178, 22);
            MenuCommandsItemSend.Text = "send";
            MenuCommandsItemSend.ToolTipText = "sends a message";
            MenuCommandsItemSend.Click += MenuItemSend_Click;
            // 
            // MenuCommandsItemAttach
            // 
            MenuCommandsItemAttach.BackColor = SystemColors.MenuBar;
            MenuCommandsItemAttach.Name = "MenuCommandsItemAttach";
            MenuCommandsItemAttach.ShortcutKeys = Keys.Control | Keys.A;
            MenuCommandsItemAttach.Size = new Size(178, 22);
            MenuCommandsItemAttach.Text = "attach";
            MenuCommandsItemAttach.ToolTipText = "attaches file to send; in prototype only  file extension image audio and video is supported";
            MenuCommandsItemAttach.Click += MenuItemAttach_Click;
            // 
            // MenuCommandsSeperator
            // 
            MenuCommandsSeperator.BackColor = SystemColors.MenuBar;
            MenuCommandsSeperator.Name = "MenuCommandsSeperator";
            MenuCommandsSeperator.Size = new Size(175, 6);
            // 
            // MenuCommandsItemRefresh
            // 
            MenuCommandsItemRefresh.BackColor = SystemColors.MenuBar;
            MenuCommandsItemRefresh.Name = "MenuCommandsItemRefresh";
            MenuCommandsItemRefresh.ShortcutKeys = Keys.Control | Keys.R;
            MenuCommandsItemRefresh.Size = new Size(178, 22);
            MenuCommandsItemRefresh.Text = "refresh";
            MenuCommandsItemRefresh.ToolTipText = "refreshes, when the terminal is flushed";
            MenuCommandsItemRefresh.Click += MenuItemRefresh_Click;
            // 
            // MenuCommandsItemClear
            // 
            MenuCommandsItemClear.BackColor = SystemColors.MenuBar;
            MenuCommandsItemClear.Name = "MenuCommandsItemClear";
            MenuCommandsItemClear.ShortcutKeys = Keys.Control | Keys.Delete;
            MenuCommandsItemClear.Size = new Size(178, 22);
            MenuCommandsItemClear.Text = "clear";
            MenuCommandsItemClear.ToolTipText = "clears completey all chat windows";
            MenuCommandsItemClear.Click += MenuItemClear_Click;
            // 
            // MenuContacts
            // 
            MenuContacts.BackColor = SystemColors.MenuBar;
            MenuContacts.DropDownItems.AddRange(new ToolStripItem[] { MenuContactsItemMe, MenuContactsItemAdd, MenuContactsItemView, MenuContactsSeparetor, MenuContactstemImport, MenuContactstemExport });
            MenuContacts.Name = "MenuContacts";
            MenuContacts.Size = new Size(77, 21);
            MenuContacts.Text = "contacts";
            // 
            // MenuContactsItemMe
            // 
            MenuContactsItemMe.BackColor = SystemColors.Menu;
            MenuContactsItemMe.Name = "MenuContactsItemMe";
            MenuContactsItemMe.ShortcutKeys = Keys.Alt | Keys.M;
            MenuContactsItemMe.Size = new Size(233, 22);
            MenuContactsItemMe.Text = "me myself mine";
            MenuContactsItemMe.ToolTipText = "edits my contact";
            MenuContactsItemMe.Click += MenuContactsItemMyContact_Click;
            // 
            // MenuContactsItemAdd
            // 
            MenuContactsItemAdd.BackColor = SystemColors.Menu;
            MenuContactsItemAdd.Name = "MenuContactsItemAdd";
            MenuContactsItemAdd.ShortcutKeys = Keys.Alt | Keys.A;
            MenuContactsItemAdd.Size = new Size(233, 22);
            MenuContactsItemAdd.Text = "add contact";
            MenuContactsItemAdd.ToolTipText = "adds a friend contact to cqr chat";
            MenuContactsItemAdd.Click += MenuItemAddContact_Click;
            // 
            // MenuContactsItemView
            // 
            MenuContactsItemView.BackColor = SystemColors.Menu;
            MenuContactsItemView.Name = "MenuContactsItemView";
            MenuContactsItemView.ShortcutKeys = Keys.Alt | Keys.V;
            MenuContactsItemView.Size = new Size(233, 22);
            MenuContactsItemView.Text = "view contacts";
            MenuContactsItemView.ToolTipText = "view all added and imported contacts";
            MenuContactsItemView.Click += MenuContactsItemView_Click;
            // 
            // MenuContactsSeparetor
            // 
            MenuContactsSeparetor.Name = "MenuContactsSeparetor";
            MenuContactsSeparetor.Size = new Size(230, 6);
            // 
            // MenuContactstemImport
            // 
            MenuContactstemImport.BackColor = SystemColors.Menu;
            MenuContactstemImport.Name = "MenuContactstemImport";
            MenuContactstemImport.ShortcutKeys = Keys.Alt | Keys.I;
            MenuContactstemImport.Size = new Size(233, 22);
            MenuContactstemImport.Text = "import contacts";
            MenuContactstemImport.ToolTipText = "import contacts from address book";
            MenuContactstemImport.Click += MenuContactstemImport_Click;
            // 
            // MenuContactstemExport
            // 
            MenuContactstemExport.BackColor = SystemColors.Menu;
            MenuContactstemExport.Enabled = false;
            MenuContactstemExport.Name = "MenuContactstemExport";
            MenuContactstemExport.ShortcutKeys = Keys.Alt | Keys.E;
            MenuContactstemExport.Size = new Size(233, 22);
            MenuContactstemExport.Text = "export contacts";
            MenuContactstemExport.ToolTipText = "export contacts to a json file";
            // 
            // MenuOptions
            // 
            MenuOptions.BackColor = SystemColors.MenuBar;
            MenuOptions.DropDownItems.AddRange(new ToolStripItem[] { MenuOptionsItemCompress, MenuOptionsItemFileSecure, MenuOptionsItemClearAllOnClose, MenuOptionsItemDontSendProfilePictures });
            MenuOptions.Name = "MenuOptions";
            MenuOptions.Size = new Size(72, 21);
            MenuOptions.Text = "options";
            // 
            // MenuOptionsItemCompress
            // 
            MenuOptionsItemCompress.BackColor = SystemColors.Menu;
            MenuOptionsItemCompress.Enabled = false;
            MenuOptionsItemCompress.Name = "MenuOptionsItemCompress";
            MenuOptionsItemCompress.Size = new Size(335, 22);
            MenuOptionsItemCompress.Text = "🗜compress 📁files before ✉sending";
            // 
            // MenuOptionsItemFileSecure
            // 
            MenuOptionsItemFileSecure.BackColor = SystemColors.Menu;
            MenuOptionsItemFileSecure.Enabled = false;
            MenuOptionsItemFileSecure.Name = "MenuOptionsItemFileSecure";
            MenuOptionsItemFileSecure.Size = new Size(335, 22);
            MenuOptionsItemFileSecure.Text = "✉send only 🔒secure 📁files";
            MenuOptionsItemFileSecure.ToolTipText = "e.g.images, pdf, ps, rtf, instead of possible macro virus containing formats";
            // 
            // MenuOptionsItemClearAllOnClose
            // 
            MenuOptionsItemClearAllOnClose.BackColor = SystemColors.Menu;
            MenuOptionsItemClearAllOnClose.Name = "MenuOptionsItemClearAllOnClose";
            MenuOptionsItemClearAllOnClose.Size = new Size(335, 22);
            MenuOptionsItemClearAllOnClose.Text = "✂␡ all chats, 📁attachments on close";
            MenuOptionsItemClearAllOnClose.ToolTipText = "When closing this application, ␡ all chats, saved 📁attachments and private 🔑keys";
            // 
            // MenuOptionsItemDontSendProfilePictures
            // 
            MenuOptionsItemDontSendProfilePictures.BackColor = SystemColors.Menu;
            MenuOptionsItemDontSendProfilePictures.Checked = true;
            MenuOptionsItemDontSendProfilePictures.CheckState = CheckState.Checked;
            MenuOptionsItemDontSendProfilePictures.Enabled = false;
            MenuOptionsItemDontSendProfilePictures.Name = "MenuOptionsItemDontSendProfilePictures";
            MenuOptionsItemDontSendProfilePictures.Size = new Size(335, 22);
            MenuOptionsItemDontSendProfilePictures.Text = "only peer-2-peer chat";
            MenuOptionsItemDontSendProfilePictures.ToolTipText = "Only use this chat for peer 2 peer network, not server sessions with invitations";
            // 
            // MenuHelp
            // 
            MenuHelp.BackColor = SystemColors.MenuBar;
            MenuHelp.BackgroundImageLayout = ImageLayout.None;
            MenuHelp.DisplayStyle = ToolStripItemDisplayStyle.Text;
            MenuHelp.DropDownItems.AddRange(new ToolStripItem[] { MenuHelpItemViewHelp, MenuHelpItemInfo, MenuHelpItemAbout });
            MenuHelp.ForeColor = SystemColors.MenuText;
            MenuHelp.ImageScaling = ToolStripItemImageScaling.None;
            MenuHelp.Name = "MenuHelp";
            MenuHelp.Padding = new Padding(3, 0, 3, 0);
            MenuHelp.ShortcutKeys = Keys.Alt | Keys.F7;
            MenuHelp.Size = new Size(48, 21);
            MenuHelp.Text = "help";
            // 
            // MenuHelpItemViewHelp
            // 
            MenuHelpItemViewHelp.BackColor = SystemColors.MenuBar;
            MenuHelpItemViewHelp.BackgroundImageLayout = ImageLayout.None;
            MenuHelpItemViewHelp.ForeColor = SystemColors.MenuText;
            MenuHelpItemViewHelp.Name = "MenuHelpItemViewHelp";
            MenuHelpItemViewHelp.Padding = new Padding(0, 2, 0, 2);
            MenuHelpItemViewHelp.ShortcutKeys = Keys.Control | Keys.F1;
            MenuHelpItemViewHelp.Size = new Size(201, 24);
            MenuHelpItemViewHelp.Text = "view help";
            MenuHelpItemViewHelp.ToolTipText = "displays help";
            MenuHelpItemViewHelp.Click += MenuHelpItemViewHelp_Click;
            // 
            // MenuHelpItemInfo
            // 
            MenuHelpItemInfo.BackColor = SystemColors.MenuBar;
            MenuHelpItemInfo.BackgroundImageLayout = ImageLayout.None;
            MenuHelpItemInfo.ForeColor = SystemColors.MenuText;
            MenuHelpItemInfo.Name = "MenuHelpItemInfo";
            MenuHelpItemInfo.Size = new Size(201, 22);
            MenuHelpItemInfo.Text = "info";
            MenuHelpItemInfo.TextImageRelation = TextImageRelation.TextAboveImage;
            MenuHelpItemInfo.ToolTipText = "displays a tiny message box with version info";
            MenuHelpItemInfo.Click += MenuHelpItemInfo_Click;
            // 
            // MenuHelpItemAbout
            // 
            MenuHelpItemAbout.BackColor = SystemColors.MenuBar;
            MenuHelpItemAbout.BackgroundImageLayout = ImageLayout.None;
            MenuHelpItemAbout.ForeColor = SystemColors.MenuText;
            MenuHelpItemAbout.Name = "MenuHelpItemAbout";
            MenuHelpItemAbout.Padding = new Padding(0, 2, 0, 2);
            MenuHelpItemAbout.Size = new Size(201, 24);
            MenuHelpItemAbout.Text = "about";
            MenuHelpItemAbout.TextImageRelation = TextImageRelation.TextAboveImage;
            MenuHelpItemAbout.ToolTipText = "displays a large modal dialog with version info and  copy left info";
            MenuHelpItemAbout.Click += MenuHelpItemAbout_Click;            
            // 
            // StripStatus
            // 
            StripStatus.GripMargin = new Padding(1);
            StripStatus.Items.AddRange(new ToolStripItem[] { StripStatusLabel, StripProgressBar });
            StripStatus.Location = new Point(0, 689);
            StripStatus.Name = "StripStatus";
            StripStatus.Size = new Size(996, 22);
            StripStatus.TabIndex = 102;
            StripStatus.Text = "StripStatus";
            // 
            // StripStatusLabel
            // 
            StripStatusLabel.AutoSize = false;
            StripStatusLabel.BackColor = SystemColors.Info;
            StripStatusLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            StripStatusLabel.BorderStyle = Border3DStyle.SunkenInner;
            StripStatusLabel.LinkVisited = true;
            StripStatusLabel.Margin = new Padding(0, 2, 0, 1);
            StripStatusLabel.Name = "StripStatusLabel";
            StripStatusLabel.Size = new Size(420, 19);
            StripStatusLabel.Text = "Status";
            // 
            // StripProgressBar
            // 
            StripProgressBar.AutoSize = false;
            StripProgressBar.Margin = new Padding(1);
            StripProgressBar.Name = "StripProgressBar";
            StripProgressBar.Size = new Size(548, 20);
            StripProgressBar.Step = 104;
            // 
            // MenuChatForm
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(996, 711);
            Controls.Add(StripStatus);
            Controls.Add(StripMenu);
            Font = new Font("Lucida Sans Unicode", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = StripMenu;
            Name = "ChatMenuForm";
            SizeGripStyle = SizeGripStyle.Show;
            Text = "ChatMenuForm";
            FormClosing += FormClose_Click;
            Load += MenuChatForm_Load;
            StripMenu.ResumeLayout(false);
            StripMenu.PerformLayout();
            StripStatus.ResumeLayout(false);
            StripStatus.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        #endregion constructor InitializeComponent async MenuChat_Load startup eventhanlder

        #region menu click event handlers

        #region menu view menuitem click handlers

        protected internal virtual void MenuView_ItemLeftRíght_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuView_ItemTopBottom_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuView_Item1View_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        #endregion menu view menuitem click handlers

        #region menu chat commands menuitem click handlers

        protected internal virtual void MenuItemSend_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuItemAttach_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuItemRefresh_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuItemClear_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        #endregion menu chat commands menuitem click handlers

        #region menu contact menuitem click handlers

        protected internal virtual void AddContactsToIpContact() { /* make it abstract l8r again */ ; }

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


        /// <summary>
        /// MenuContactsItemMyContact_Click edits or adds at 1st time starting own contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void MenuContactsItemMyContact_Click(object sender, EventArgs e)
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
                    // if (bmp != null)
                    //  this.PictureBoxYou.Image = bmp;                    
                }
                catch (Exception exBmp)
                {
                    CqrException.SetLastException(exBmp);
                }

                Settings.SaveSettings(Settings.Singleton);
            }

            return; // bmp;

        }

        /// <summary>
        /// MenuItemAddContact_Click launches <see cref="ContactSettings"/> to add or edit a contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void MenuItemAddContact_Click(object sender, EventArgs e)
        {
            ContactSettings contactSettings = new ContactSettings("Add Contact Info", 1);
            contactSettings.ShowInTaskbar = true;
            contactSettings.ShowDialog();

            AddContactsToIpContact();
        }

        /// <summary> 
        /// MenuContactsItemView_Click launches <see cref="ContactsView"/> to view contacts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void MenuContactsItemView_Click(object sender, EventArgs e)
        {
            ContactsView cview = new ContactsView();
            cview.ShowDialog();
        }

        /// <summary>
        /// MenuContactstemImport_Click performs an import of existing contacts from csv or vcard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void MenuContactstemImport_Click(object sender, EventArgs e)
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


                    string importedMsg = $"{contactsImported} new contacts imported!";
                    if (!string.IsNullOrEmpty(firstImport))
                        importedMsg += $"\nFirst: {firstImport}";
                    if (!string.IsNullOrEmpty(lastImport))
                        importedMsg += $"\n Last: {lastImport}";
                    MessageBox.Show(importedMsg, $"Contacts import finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #endregion menu contact menuitem click handlers

        #region load save menu handlers

        protected internal virtual void MenuFileItemOpen_Click(object sender, EventArgs e)
        {
            FileOpenDialog = DialogFileOpen;
            DialogResult res = FileOpenDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                MessageBox.Show($"FileName: {FileOpenDialog.FileName} init directory: {FileOpenDialog.InitialDirectory}", $"{Text} type {FileOpenDialog.GetType()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected internal virtual void MenuFileItemSave_Click(object sender, EventArgs e)
        {
            SafeFileName();
        }

        protected internal virtual byte[] OpenCryptFileDialog(ref string loadDir)
        {
            FileOpenDialog = DialogFileOpen;
            byte[] fileBytes;            
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

        protected internal virtual string SafeFileName(string? filePath = "", byte[]? content = null)
        {
            FileSaveDialog = DialogFileSave;
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

        #endregion #region load save menu handlers

        #endregion menu click event handlers


        #region dispose destructor

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

        #endregion dispose destructor

    }
}
