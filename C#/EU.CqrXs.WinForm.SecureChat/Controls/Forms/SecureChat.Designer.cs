using Area23.At.Framework.Core.Net.WebHttp;
using System.Net;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{

    partial class SecureChat
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            MenuOptionsSeparator = new ToolStripSeparator();
            MenuOptionsItemPeer2Peer = new ToolStripMenuItem();
            MenuOptionsItemServerSession = new ToolStripMenuItem();
            MenuHelp = new ToolStripMenuItem();
            MenuHelpItemViewHelp = new ToolStripMenuItem();
            MenuHelpItemInfo = new ToolStripMenuItem();
            MenuHelpItemAbout = new ToolStripMenuItem();
            StripStatus = new StatusStrip();
            StripStatusLabel = new ToolStripStatusLabel();
            StripProgressBar = new ToolStripProgressBar();
            SplitChatView = new SplitContainer();
            TextBoxSource = new TextBox();
            TextBoxDestionation = new TextBox();
            PictureBoxYou = new PictureBox();
            ButtonKey = new Button();
            PanelEnCodeCrypt = new Panel();
            TextBoxPipe = new TextBox();
            textBoxChatSession = new TextBox();
            ButtonCheck = new Button();
            ComboBoxContacts = new ComboBox();
            ComboBoxIp = new ComboBox();
            ComboBoxSecretKey = new ComboBox();
            RichTextBoxChat = new RichTextBox();
            PanelDestination = new Panel();
            PeerServerSwitch = new Panels.PeerServerSwitchPanel(components);
            ButtonSend = new Button();
            ButtonAttach = new Button();
            DragNDropGroupBox = new GroupBoxes.DragNDropBox(components);
            LinkedLabelsBox = new GroupBoxes.LinkLabelsBox(components);
            PanelCenter = new Panel();
            RichTextBoxOneView = new RichTextBox();
            PanelBottom = new Panel();
            toolStripTextBoxCqrPipe = new ToolStripTextBox();
            MenuItemCqrPipe = new ToolStripMenuItem();
            MenuItemChatRoom = new ToolStripMenuItem();
            SeparatorMenu1 = new ToolStripSeparator();
            MenuTextBoxChatRoom = new ToolStripTextBox();
            SeparatorMenu0 = new ToolStripSeparator();
            StripMenu.SuspendLayout();
            StripStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SplitChatView).BeginInit();
            SplitChatView.Panel1.SuspendLayout();
            SplitChatView.Panel2.SuspendLayout();
            SplitChatView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBoxYou).BeginInit();
            PanelEnCodeCrypt.SuspendLayout();
            PanelDestination.SuspendLayout();
            PanelCenter.SuspendLayout();
            PanelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // StripMenu
            // 
            StripMenu.AllowItemReorder = true;
            StripMenu.BackColor = SystemColors.MenuBar;
            StripMenu.Font = new Font("Lucida Sans Unicode", 10F);
            StripMenu.GripStyle = ToolStripGripStyle.Visible;
            StripMenu.Items.AddRange(new ToolStripItem[] { MenuFile, MenuView, MenuNetwork, MenuCommands, MenuContacts, MenuOptions, MenuHelp, SeparatorMenu0, MenuItemCqrPipe, toolStripTextBoxCqrPipe, SeparatorMenu1, MenuItemChatRoom, MenuTextBoxChatRoom });
            StripMenu.Location = new Point(0, 0);
            StripMenu.Name = "StripMenu";
            StripMenu.RenderMode = ToolStripRenderMode.System;
            StripMenu.Size = new Size(998, 29);
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
            MenuFile.Size = new Size(73, 25);
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
            MenuFileItemSave.Click += toolStripMenuItemSave_Click;
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
            MenuView.Size = new Size(48, 25);
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
            MenuNetwork.Size = new Size(76, 25);
            MenuNetwork.Text = "network";
            MenuNetwork.ToolTipText = "network provides ip addresses & secure things";
            // 
            // MenuNetworkItemMyIps
            // 
            MenuNetworkItemMyIps.BackColor = SystemColors.MenuBar;
            MenuNetworkItemMyIps.DropDownItems.AddRange(new ToolStripItem[] { MenuItemExternalIp });
            MenuNetworkItemMyIps.Name = "MenuNetworkItemMyIps";
            MenuNetworkItemMyIps.Size = new Size(180, 22);
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
            MenuItemFriendIp.Size = new Size(180, 22);
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
            MenuNetworkItemProxyServers.Size = new Size(180, 22);
            MenuNetworkItemProxyServers.Text = "proxies";
            MenuNetworkItemProxyServers.ToolTipText = "proxies are needed mainly to connect to people, where no endpoint to endpoint ip connection is possible";
            // 
            // MenuNetworkSeparatorIp
            // 
            MenuNetworkSeparatorIp.BackColor = SystemColors.MenuBar;
            MenuNetworkSeparatorIp.ForeColor = SystemColors.ActiveBorder;
            MenuNetworkSeparatorIp.Name = "MenuNetworkSeparatorIp";
            MenuNetworkSeparatorIp.Size = new Size(177, 6);
            // 
            // MenuNetworkItemIPv6Secure
            // 
            MenuNetworkItemIPv6Secure.BackColor = SystemColors.MenuBar;
            MenuNetworkItemIPv6Secure.Name = "MenuNetworkItemIPv6Secure";
            MenuNetworkItemIPv6Secure.ShortcutKeys = Keys.Control | Keys.D6;
            MenuNetworkItemIPv6Secure.Size = new Size(180, 22);
            MenuNetworkItemIPv6Secure.Text = "ip6 cqr";
            MenuNetworkItemIPv6Secure.ToolTipText = "you can check it only, when you have an ipv6 address and you want to chat only to partners, where ip6 connect is possible";
            // 
            // MenuCommands
            // 
            MenuCommands.BackColor = SystemColors.MenuBar;
            MenuCommands.DropDownItems.AddRange(new ToolStripItem[] { MenuCommandsItemSend, MenuCommandsItemAttach, MenuCommandsSeperator, MenuCommandsItemRefresh, MenuCommandsItemClear });
            MenuCommands.Name = "MenuCommands";
            MenuCommands.Size = new Size(128, 25);
            MenuCommands.Text = "chat commands";
            MenuCommands.ToolTipText = "possible chat commands";
            // 
            // MenuCommandsItemSend
            // 
            MenuCommandsItemSend.BackColor = SystemColors.MenuBar;
            MenuCommandsItemSend.Name = "MenuCommandsItemSend";
            MenuCommandsItemSend.ShortcutKeys = Keys.Control | Keys.S;
            MenuCommandsItemSend.Size = new Size(180, 22);
            MenuCommandsItemSend.Text = "send";
            MenuCommandsItemSend.ToolTipText = "sends a message";
            MenuCommandsItemSend.Click += MenuCommandsItemSend_Click;
            // 
            // MenuCommandsItemAttach
            // 
            MenuCommandsItemAttach.BackColor = SystemColors.MenuBar;
            MenuCommandsItemAttach.Name = "MenuCommandsItemAttach";
            MenuCommandsItemAttach.ShortcutKeys = Keys.Control | Keys.A;
            MenuCommandsItemAttach.Size = new Size(180, 22);
            MenuCommandsItemAttach.Text = "attach";
            MenuCommandsItemAttach.ToolTipText = "attaches file to send; in prototype only  file extension image audio and video is supported";
            MenuCommandsItemAttach.Click += MenuCommandsItemAttach_Click;
            // 
            // MenuCommandsSeperator
            // 
            MenuCommandsSeperator.BackColor = SystemColors.MenuBar;
            MenuCommandsSeperator.Name = "MenuCommandsSeperator";
            MenuCommandsSeperator.Size = new Size(177, 6);
            // 
            // MenuCommandsItemRefresh
            // 
            MenuCommandsItemRefresh.BackColor = SystemColors.MenuBar;
            MenuCommandsItemRefresh.Name = "MenuCommandsItemRefresh";
            MenuCommandsItemRefresh.ShortcutKeys = Keys.Control | Keys.R;
            MenuCommandsItemRefresh.Size = new Size(180, 22);
            MenuCommandsItemRefresh.Text = "refresh";
            MenuCommandsItemRefresh.ToolTipText = "refreshes, when the terminal is flushed";
            MenuCommandsItemRefresh.Click += MenuCommandsItemRefresh_Click;
            // 
            // MenuCommandsItemClear
            // 
            MenuCommandsItemClear.BackColor = SystemColors.MenuBar;
            MenuCommandsItemClear.Name = "MenuCommandsItemClear";
            MenuCommandsItemClear.ShortcutKeys = Keys.Control | Keys.Delete;
            MenuCommandsItemClear.Size = new Size(180, 22);
            MenuCommandsItemClear.Text = "clear";
            MenuCommandsItemClear.ToolTipText = "clears completey all chat windows";
            MenuCommandsItemClear.Click += MenuCommandsItemClear_Click;
            // 
            // MenuContacts
            // 
            MenuContacts.BackColor = SystemColors.MenuBar;
            MenuContacts.DropDownItems.AddRange(new ToolStripItem[] { MenuContactsItemMe, MenuContactsItemAdd, MenuContactsItemView, MenuContactsSeparetor, MenuContactstemImport, MenuContactstemExport });
            MenuContacts.Name = "MenuContacts";
            MenuContacts.Size = new Size(77, 25);
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
            MenuContactsItemAdd.Click += MenuContactsItemAdd_Click;
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
            MenuOptions.DropDownItems.AddRange(new ToolStripItem[] { MenuOptionsItemCompress, MenuOptionsItemFileSecure, MenuOptionsItemClearAllOnClose, MenuOptionsItemDontSendProfilePictures, MenuOptionsSeparator, MenuOptionsItemPeer2Peer, MenuOptionsItemServerSession });
            MenuOptions.Name = "MenuOptions";
            MenuOptions.Size = new Size(72, 25);
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
            MenuOptionsItemClearAllOnClose.Click += MenuOptionsItemClearAllOnClose_Click;
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
            // MenuOptionsSeparator
            // 
            MenuOptionsSeparator.Name = "MenuOptionsSeparator";
            MenuOptionsSeparator.Size = new Size(332, 6);
            // 
            // MenuOptionsItemPeer2Peer
            // 
            MenuOptionsItemPeer2Peer.CheckOnClick = true;
            MenuOptionsItemPeer2Peer.Name = "MenuOptionsItemPeer2Peer";
            MenuOptionsItemPeer2Peer.Size = new Size(335, 22);
            MenuOptionsItemPeer2Peer.Text = "peer-2-peer mode";
            MenuOptionsItemPeer2Peer.Click += MenuOptionsItemPeer2Peer_Click;
            // 
            // MenuOptionsItemServerSession
            // 
            MenuOptionsItemServerSession.CheckOnClick = true;
            MenuOptionsItemServerSession.Name = "MenuOptionsItemServerSession";
            MenuOptionsItemServerSession.Size = new Size(335, 22);
            MenuOptionsItemServerSession.Text = "chat server session";
            MenuOptionsItemServerSession.Click += MenuOptionsItemServerSession_Click;
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
            MenuHelp.Size = new Size(48, 25);
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
            StripStatus.Location = new Point(0, 695);
            StripStatus.Name = "StripStatus";
            StripStatus.Size = new Size(998, 22);
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
            // SplitChatView
            // 
            SplitChatView.BackColor = SystemColors.ControlLight;
            SplitChatView.IsSplitterFixed = true;
            SplitChatView.Location = new Point(0, 0);
            SplitChatView.Margin = new Padding(0);
            SplitChatView.MaximumSize = new Size(800, 600);
            SplitChatView.MinimumSize = new Size(600, 400);
            SplitChatView.Name = "SplitChatView";
            // 
            // SplitChatView.Panel1
            // 
            SplitChatView.Panel1.AllowDrop = true;
            SplitChatView.Panel1.BackgroundImageLayout = ImageLayout.None;
            SplitChatView.Panel1.Controls.Add(TextBoxSource);
            SplitChatView.Panel1MinSize = 300;
            // 
            // SplitChatView.Panel2
            // 
            SplitChatView.Panel2.BackgroundImageLayout = ImageLayout.None;
            SplitChatView.Panel2.Controls.Add(TextBoxDestionation);
            SplitChatView.Panel2MinSize = 300;
            SplitChatView.Size = new Size(800, 460);
            SplitChatView.SplitterDistance = 396;
            SplitChatView.SplitterIncrement = 8;
            SplitChatView.SplitterWidth = 8;
            SplitChatView.TabIndex = 31;
            SplitChatView.TabStop = false;
            // 
            // TextBoxSource
            // 
            TextBoxSource.BackColor = SystemColors.GradientActiveCaption;
            TextBoxSource.BorderStyle = BorderStyle.FixedSingle;
            TextBoxSource.Dock = DockStyle.Fill;
            TextBoxSource.Font = new Font("Lucida Sans Unicode", 9F);
            TextBoxSource.Location = new Point(0, 0);
            TextBoxSource.Margin = new Padding(1);
            TextBoxSource.MaxLength = 65536;
            TextBoxSource.Multiline = true;
            TextBoxSource.Name = "TextBoxSource";
            TextBoxSource.ScrollBars = ScrollBars.Both;
            TextBoxSource.Size = new Size(396, 460);
            TextBoxSource.TabIndex = 32;
            // 
            // TextBoxDestionation
            // 
            TextBoxDestionation.BackColor = SystemColors.GradientInactiveCaption;
            TextBoxDestionation.BorderStyle = BorderStyle.FixedSingle;
            TextBoxDestionation.Dock = DockStyle.Fill;
            TextBoxDestionation.Font = new Font("Lucida Sans Unicode", 9F);
            TextBoxDestionation.Location = new Point(0, 0);
            TextBoxDestionation.Margin = new Padding(1);
            TextBoxDestionation.MaxLength = 65536;
            TextBoxDestionation.Multiline = true;
            TextBoxDestionation.Name = "TextBoxDestionation";
            TextBoxDestionation.ScrollBars = ScrollBars.Both;
            TextBoxDestionation.Size = new Size(396, 460);
            TextBoxDestionation.TabIndex = 33;
            // 
            // PictureBoxYou
            // 
            PictureBoxYou.BackColor = SystemColors.ButtonShadow;
            PictureBoxYou.BackgroundImageLayout = ImageLayout.None;
            PictureBoxYou.Location = new Point(7, 58);
            PictureBoxYou.Margin = new Padding(1);
            PictureBoxYou.Name = "PictureBoxYou";
            PictureBoxYou.Padding = new Padding(1);
            PictureBoxYou.Size = new Size(155, 155);
            PictureBoxYou.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBoxYou.TabIndex = 71;
            PictureBoxYou.TabStop = false;
            // 
            // ButtonKey
            // 
            ButtonKey.BackColor = SystemColors.ButtonHighlight;
            ButtonKey.BackgroundImageLayout = ImageLayout.Center;
            ButtonKey.Font = new Font("Lucida Sans Unicode", 10F, FontStyle.Bold);
            ButtonKey.ForeColor = SystemColors.ActiveCaptionText;
            ButtonKey.Image = Properties.Resources.a_right_key;
            ButtonKey.Location = new Point(306, 3);
            ButtonKey.Margin = new Padding(1);
            ButtonKey.Name = "ButtonKey";
            ButtonKey.Padding = new Padding(1);
            ButtonKey.Size = new Size(40, 27);
            ButtonKey.TabIndex = 12;
            ButtonKey.UseVisualStyleBackColor = false;
            ButtonKey.Click += ButtonKey_Click;
            // 
            // PanelEnCodeCrypt
            // 
            PanelEnCodeCrypt.BackColor = SystemColors.ActiveCaption;
            PanelEnCodeCrypt.Controls.Add(TextBoxPipe);
            PanelEnCodeCrypt.Controls.Add(textBoxChatSession);
            PanelEnCodeCrypt.Controls.Add(ButtonCheck);
            PanelEnCodeCrypt.Controls.Add(ComboBoxContacts);
            PanelEnCodeCrypt.Controls.Add(ComboBoxIp);
            PanelEnCodeCrypt.Controls.Add(ComboBoxSecretKey);
            PanelEnCodeCrypt.Controls.Add(ButtonKey);
            PanelEnCodeCrypt.ForeColor = SystemColors.WindowText;
            PanelEnCodeCrypt.Location = new Point(0, 28);
            PanelEnCodeCrypt.Margin = new Padding(0);
            PanelEnCodeCrypt.Name = "PanelEnCodeCrypt";
            PanelEnCodeCrypt.Size = new Size(994, 64);
            PanelEnCodeCrypt.TabIndex = 10;
            // 
            // TextBoxPipe
            // 
            TextBoxPipe.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TextBoxPipe.HideSelection = false;
            TextBoxPipe.Location = new Point(353, 4);
            TextBoxPipe.Margin = new Padding(1);
            TextBoxPipe.Name = "TextBoxPipe";
            TextBoxPipe.ReadOnly = true;
            TextBoxPipe.Size = new Size(93, 26);
            TextBoxPipe.TabIndex = 13;
            // 
            // textBoxChatSession
            // 
            textBoxChatSession.Font = new Font("Lucida Sans Unicode", 8F);
            textBoxChatSession.HideSelection = false;
            textBoxChatSession.Location = new Point(456, 6);
            textBoxChatSession.Margin = new Padding(1);
            textBoxChatSession.Name = "textBoxChatSession";
            textBoxChatSession.Size = new Size(368, 24);
            textBoxChatSession.TabIndex = 19;
            // 
            // ButtonCheck
            // 
            ButtonCheck.BackColor = SystemColors.ButtonHighlight;
            ButtonCheck.BackgroundImageLayout = ImageLayout.Center;
            ButtonCheck.Font = new Font("Lucida Sans Unicode", 10F, FontStyle.Bold);
            ButtonCheck.ForeColor = SystemColors.ActiveCaptionText;
            ButtonCheck.Image = Properties.de.Resources.CableWireCut;
            ButtonCheck.Location = new Point(306, 30);
            ButtonCheck.Margin = new Padding(1);
            ButtonCheck.Name = "ButtonCheck";
            ButtonCheck.Padding = new Padding(1);
            ButtonCheck.Size = new Size(40, 32);
            ButtonCheck.TabIndex = 17;
            ButtonCheck.UseVisualStyleBackColor = false;
            // 
            // ComboBoxContacts
            // 
            ComboBoxContacts.BackColor = SystemColors.ControlLightLight;
            ComboBoxContacts.Enabled = false;
            ComboBoxContacts.Font = new Font("Lucida Sans Unicode", 9F);
            ComboBoxContacts.ForeColor = SystemColors.ControlText;
            ComboBoxContacts.FormattingEnabled = true;
            ComboBoxContacts.Location = new Point(456, 35);
            ComboBoxContacts.Margin = new Padding(1);
            ComboBoxContacts.Name = "ComboBoxContacts";
            ComboBoxContacts.Size = new Size(369, 24);
            ComboBoxContacts.TabIndex = 18;
            ComboBoxContacts.Text = "[Select Contact]";
            ComboBoxContacts.SelectedIndexChanged += ComboBoxContacts_SelectedIndexChanged;
            ComboBoxContacts.Leave += ComboBoxContacts_FocusLeave;
            // 
            // ComboBoxIp
            // 
            ComboBoxIp.BackColor = SystemColors.ControlLightLight;
            ComboBoxIp.Enabled = false;
            ComboBoxIp.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ComboBoxIp.ForeColor = SystemColors.ControlText;
            ComboBoxIp.FormattingEnabled = true;
            ComboBoxIp.Location = new Point(4, 35);
            ComboBoxIp.Margin = new Padding(1);
            ComboBoxIp.Name = "ComboBoxIp";
            ComboBoxIp.Size = new Size(297, 24);
            ComboBoxIp.TabIndex = 15;
            ComboBoxIp.Text = "[enter peer IPv4/IPv6]";
            ComboBoxIp.SelectedIndexChanged += ComboBoxIp_SelectedIndexChanged;
            ComboBoxIp.Leave += ComboBoxIp_FocusLeave;
            // 
            // ComboBoxSecretKey
            // 
            ComboBoxSecretKey.BackColor = SystemColors.ControlLightLight;
            ComboBoxSecretKey.ForeColor = SystemColors.ControlText;
            ComboBoxSecretKey.FormattingEnabled = true;
            ComboBoxSecretKey.Location = new Point(3, 4);
            ComboBoxSecretKey.Margin = new Padding(1);
            ComboBoxSecretKey.Name = "ComboBoxSecretKey";
            ComboBoxSecretKey.Size = new Size(297, 24);
            ComboBoxSecretKey.TabIndex = 11;
            ComboBoxSecretKey.Text = "[enter secret key here]";
            ComboBoxSecretKey.SelectedIndexChanged += ComboBoxSecretKey_SelectedIndexChanged;
            ComboBoxSecretKey.TextUpdate += ComboBoxSecretKey_TextUpdate;
            ComboBoxSecretKey.Leave += ComboBoxSecretKey_FocusLeave;
            // 
            // RichTextBoxChat
            // 
            RichTextBoxChat.BackColor = SystemColors.ButtonHighlight;
            RichTextBoxChat.BorderStyle = BorderStyle.FixedSingle;
            RichTextBoxChat.ForeColor = SystemColors.WindowText;
            RichTextBoxChat.Location = new Point(3, 4);
            RichTextBoxChat.Margin = new Padding(1);
            RichTextBoxChat.Name = "RichTextBoxChat";
            RichTextBoxChat.Size = new Size(820, 123);
            RichTextBoxChat.TabIndex = 41;
            RichTextBoxChat.Text = "";
            // 
            // PanelDestination
            // 
            PanelDestination.BackColor = SystemColors.ActiveCaption;
            PanelDestination.Controls.Add(PeerServerSwitch);
            PanelDestination.Controls.Add(ButtonSend);
            PanelDestination.Controls.Add(ButtonAttach);
            PanelDestination.Controls.Add(DragNDropGroupBox);
            PanelDestination.Controls.Add(LinkedLabelsBox);
            PanelDestination.Controls.Add(PictureBoxYou);
            PanelDestination.ForeColor = SystemColors.ActiveCaptionText;
            PanelDestination.Location = new Point(826, 32);
            PanelDestination.Margin = new Padding(0);
            PanelDestination.Name = "PanelDestination";
            PanelDestination.Size = new Size(168, 663);
            PanelDestination.TabIndex = 70;
            // 
            // PeerServerSwitch
            // 
            PeerServerSwitch.AllowDrop = true;
            PeerServerSwitch.BackColor = SystemColors.GradientActiveCaption;
            PeerServerSwitch.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PeerServerSwitch.Location = new Point(8, 2);
            PeerServerSwitch.Margin = new Padding(1);
            PeerServerSwitch.Name = "PeerServerSwitch";
            PeerServerSwitch.Padding = new Padding(1);
            PeerServerSwitch.Size = new Size(152, 48);
            PeerServerSwitch.TabIndex = 84;
            // 
            // ButtonSend
            // 
            ButtonSend.Font = new Font("Lucida Sans Unicode", 9F);
            ButtonSend.Location = new Point(8, 633);
            ButtonSend.Margin = new Padding(1);
            ButtonSend.Name = "ButtonSend";
            ButtonSend.Padding = new Padding(1);
            ButtonSend.Size = new Size(75, 27);
            ButtonSend.TabIndex = 83;
            ButtonSend.Text = "Send";
            ButtonSend.UseVisualStyleBackColor = true;
            ButtonSend.Click += ButtonSend_Click;
            // 
            // ButtonAttach
            // 
            ButtonAttach.Font = new Font("Lucida Sans Unicode", 9F);
            ButtonAttach.Location = new Point(89, 633);
            ButtonAttach.Margin = new Padding(1);
            ButtonAttach.Name = "ButtonAttach";
            ButtonAttach.Padding = new Padding(1);
            ButtonAttach.Size = new Size(75, 27);
            ButtonAttach.TabIndex = 82;
            ButtonAttach.Text = "Attach";
            ButtonAttach.UseVisualStyleBackColor = true;
            ButtonAttach.Click += ButtonAttach_Click;
            // 
            // DragNDropGroupBox
            // 
            DragNDropGroupBox.AllowDrop = true;
            DragNDropGroupBox.BackColor = SystemColors.AppWorkspace;
            DragNDropGroupBox.Font = new Font("Lucida Sans Unicode", 8.5F);
            DragNDropGroupBox.ForeColor = SystemColors.ActiveCaptionText;
            DragNDropGroupBox.Location = new Point(8, 537);
            DragNDropGroupBox.Margin = new Padding(1);
            DragNDropGroupBox.Name = "DragNDropGroupBox";
            DragNDropGroupBox.Padding = new Padding(1);
            DragNDropGroupBox.Size = new Size(154, 93);
            DragNDropGroupBox.TabIndex = 81;
            DragNDropGroupBox.TabStop = false;
            DragNDropGroupBox.Text = "   Drag'N'Drop Box";
            // 
            // LinkedLabelsBox
            // 
            LinkedLabelsBox.AllowDrop = true;
            LinkedLabelsBox.BackColor = SystemColors.GradientActiveCaption;
            LinkedLabelsBox.Font = new Font("Lucida Sans Unicode", 9F);
            LinkedLabelsBox.Location = new Point(4, 223);
            LinkedLabelsBox.Margin = new Padding(0);
            LinkedLabelsBox.Name = "LinkedLabelsBox";
            LinkedLabelsBox.Padding = new Padding(0);
            LinkedLabelsBox.Size = new Size(160, 307);
            LinkedLabelsBox.TabIndex = 83;
            LinkedLabelsBox.TabStop = false;
            LinkedLabelsBox.Text = "Attachments";
            // 
            // PanelCenter
            // 
            PanelCenter.Controls.Add(SplitChatView);
            PanelCenter.Controls.Add(RichTextBoxOneView);
            PanelCenter.Location = new Point(8, 102);
            PanelCenter.Margin = new Padding(0);
            PanelCenter.Name = "PanelCenter";
            PanelCenter.Size = new Size(800, 460);
            PanelCenter.TabIndex = 30;
            // 
            // RichTextBoxOneView
            // 
            RichTextBoxOneView.AcceptsTab = true;
            RichTextBoxOneView.Dock = DockStyle.Fill;
            RichTextBoxOneView.Location = new Point(0, 0);
            RichTextBoxOneView.Margin = new Padding(2);
            RichTextBoxOneView.Name = "RichTextBoxOneView";
            RichTextBoxOneView.ReadOnly = true;
            RichTextBoxOneView.Size = new Size(800, 460);
            RichTextBoxOneView.TabIndex = 36;
            RichTextBoxOneView.Text = "";
            RichTextBoxOneView.Visible = false;
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = SystemColors.ActiveCaption;
            PanelBottom.Controls.Add(RichTextBoxChat);
            PanelBottom.ForeColor = SystemColors.ActiveCaptionText;
            PanelBottom.Location = new Point(0, 565);
            PanelBottom.Margin = new Padding(1);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(824, 130);
            PanelBottom.TabIndex = 40;
            // 
            // toolStripTextBoxCqrPipe
            // 
            toolStripTextBoxCqrPipe.BorderStyle = BorderStyle.None;
            toolStripTextBoxCqrPipe.Font = new Font("Lucida Sans Unicode", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripTextBoxCqrPipe.MaxLength = 1024;
            toolStripTextBoxCqrPipe.Name = "toolStripTextBoxCqrPipe";
            toolStripTextBoxCqrPipe.ReadOnly = true;
            toolStripTextBoxCqrPipe.Size = new Size(100, 25);
            toolStripTextBoxCqrPipe.ToolTipText = "CqrPipe";
            // 
            // MenuItemCqrPipe
            // 
            MenuItemCqrPipe.BackColor = SystemColors.Menu;
            MenuItemCqrPipe.BackgroundImageLayout = ImageLayout.None;
            MenuItemCqrPipe.ForeColor = SystemColors.MenuText;
            MenuItemCqrPipe.ImageAlign = ContentAlignment.MiddleRight;
            MenuItemCqrPipe.Name = "MenuItemCqrPipe";
            MenuItemCqrPipe.Padding = new Padding(2, 0, 2, 0);
            MenuItemCqrPipe.Size = new Size(72, 25);
            MenuItemCqrPipe.Text = "cqrpipe:";
            MenuItemCqrPipe.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MenuItemChatRoom
            // 
            MenuItemChatRoom.Name = "MenuItemChatRoom";
            MenuItemChatRoom.Size = new Size(94, 25);
            MenuItemChatRoom.Text = "chat room:";
            // 
            // SeparatorMenu1
            // 
            SeparatorMenu1.BackColor = SystemColors.MenuBar;
            SeparatorMenu1.ForeColor = SystemColors.MenuText;
            SeparatorMenu1.Margin = new Padding(1);
            SeparatorMenu1.Name = "SeparatorMenu1";
            SeparatorMenu1.Size = new Size(6, 23);
            // 
            // MenuTextBoxChatRoom
            // 
            MenuTextBoxChatRoom.Name = "MenuTextBoxChatRoom";
            MenuTextBoxChatRoom.Size = new Size(128, 25);
            // 
            // SeparatorMenu0
            // 
            SeparatorMenu0.BackColor = SystemColors.MenuBar;
            SeparatorMenu0.ForeColor = SystemColors.MenuText;
            SeparatorMenu0.Margin = new Padding(1);
            SeparatorMenu0.Name = "SeparatorMenu0";
            SeparatorMenu0.Size = new Size(6, 23);
            // 
            // SecureChat
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(998, 717);
            Controls.Add(PanelDestination);
            Controls.Add(PanelBottom);
            Controls.Add(PanelEnCodeCrypt);
            Controls.Add(StripStatus);
            Controls.Add(StripMenu);
            Controls.Add(PanelCenter);
            Font = new Font("Lucida Sans Unicode", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.fr.Resources.SatIcon;
            MainMenuStrip = StripMenu;
            Name = "SecureChat";
            SizeGripStyle = SizeGripStyle.Show;
            Text = "SecureChat";
            FormClosing += FormClose_Click;
            Load += SecureChat_Load;
            StripMenu.ResumeLayout(false);
            StripMenu.PerformLayout();
            StripStatus.ResumeLayout(false);
            StripStatus.PerformLayout();
            SplitChatView.Panel1.ResumeLayout(false);
            SplitChatView.Panel1.PerformLayout();
            SplitChatView.Panel2.ResumeLayout(false);
            SplitChatView.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SplitChatView).EndInit();
            SplitChatView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PictureBoxYou).EndInit();
            PanelEnCodeCrypt.ResumeLayout(false);
            PanelEnCodeCrypt.PerformLayout();
            PanelDestination.ResumeLayout(false);
            PanelCenter.ResumeLayout(false);
            PanelBottom.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private Panel PanelEnCodeCrypt;
        private Panel PanelDestination;
        private Panel PanelCenter;
        private Panel PanelBottom;
        private ComboBox ComboBoxIp;
        private ComboBox ComboBoxSecretKey;
        private ComboBox ComboBoxContacts;
        private SplitContainer SplitChatView;
        private TextBox TextBoxSource;
        private TextBox TextBoxDestionation;
        private TextBox TextBoxPipe;
        private RichTextBox RichTextBoxOneView;
        private RichTextBox RichTextBoxChat;
        private PictureBox PictureBoxYou;
        private Button ButtonKey;
        private Button ButtonAttach;
        private Button ButtonCheck;
        private Controls.GroupBoxes.LinkLabelsBox GroupBoxLinks;

        internal MenuStrip StripMenu;
        private StatusStrip StripStatus;

        private ToolStripMenuItem MenuFile;
        private ToolStripMenuItem MenuFileItemOpen;
        private ToolStripMenuItem MenuFileItemSave;
        private ToolStripSeparator MenuFileSeparatorExit;
        private ToolStripMenuItem MenuFileItemExit;

        private ToolStripMenuItem MenuView;
        private ToolStripMenuItem MenuViewItemLeftRíght;
        private ToolStripMenuItem MenuViewItemTopBottom;
        private ToolStripMenuItem MenuViewItem1View;


        private ToolStripMenuItem MenuNetwork;
        private ToolStripMenuItem MenuNetworkItemMyIps;
        private ToolStripMenuItem MenuItemExternalIp;
        private ToolStripMenuItem MenuItemFriendIp;
        private ToolStripComboBox MenuNetworkComboBoxFriendIp;
        private ToolStripMenuItem MenuNetworkItemProxyServers;
        private ToolStripMenuItem MenuNetworkItemIPv6Secure;
        private ToolStripSeparator MenuNetworkSeparatorIp;

        private ToolStripMenuItem MenuCommands;
        private ToolStripMenuItem MenuCommandsItemSend;
        private ToolStripMenuItem MenuCommandsItemRefresh;
        private ToolStripMenuItem MenuCommandsItemClear;
        private ToolStripMenuItem MenuCommandsItemAttach;
        private ToolStripSeparator MenuCommandsSeperator;

        private ToolStripMenuItem MenuContacts;
        private ToolStripMenuItem MenuContactstemImport;
        private ToolStripMenuItem MenuContactsItemAdd;
        private ToolStripMenuItem MenuContactsItemView;
        private ToolStripMenuItem MenuContactsItemMe;
        private ToolStripSeparator MenuContactsSeparetor;
        private ToolStripMenuItem MenuContactstemExport;

        private ToolStripMenuItem MenuOptions;
        private ToolStripMenuItem MenuOptionsItemCompress;
        private ToolStripMenuItem MenuOptionsItemFileSecure;
        private ToolStripMenuItem MenuOptionsItemClearAllOnClose;
        private ToolStripMenuItem MenuOptionsItemDontSendProfilePictures;
        private ToolStripSeparator MenuOptionsSeparator;
        private ToolStripMenuItem MenuOptionsItemPeer2Peer;
        private ToolStripMenuItem MenuOptionsItemServerSession;

        private ToolStripMenuItem MenuHelp;
        private ToolStripMenuItem MenuHelpItemViewHelp;
        private ToolStripMenuItem MenuHelpItemInfo;
        private ToolStripMenuItem MenuHelpItemAbout;
        private ToolStripStatusLabel StripStatusLabel;

        private Controls.GroupBoxes.DragNDropBox DragNDropGroupBox;
        private ToolStripProgressBar StripProgressBar;
        private Controls.GroupBoxes.LinkLabelsBox LinkedLabelsBox;
        private Controls.Panels.PeerServerSwitchPanel PeerServerSwitch;
        private Button ButtonSend;
        private TextBox textBoxChatSession;
        private ToolStripSeparator SeparatorMenu1;
        private ToolStripMenuItem MenuItemCqrPipe;
        private ToolStripTextBox toolStripTextBoxCqrPipe;
        private ToolStripMenuItem MenuItemChatRoom;
        private ToolStripSeparator SeparatorMenu0;
        private ToolStripTextBox MenuTextBoxChatRoom;
    }

}