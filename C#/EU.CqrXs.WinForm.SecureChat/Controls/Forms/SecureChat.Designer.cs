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
            toolStripSeparator1 = new ToolStripSeparator();
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
            toolStripSeparator2 = new ToolStripSeparator();
            MenuNetwork = new ToolStripMenuItem();
            MenuNetworkItemMyIps = new ToolStripMenuItem();
            MenuItemExternalIp = new ToolStripMenuItem();
            MenuItemFriendIp = new ToolStripMenuItem();
            MenuNetworkComboBoxFriendIp = new ToolStripComboBox();
            MenuNetworkItemProxyServers = new ToolStripMenuItem();
            MenuNetworkSeparatorIp = new ToolStripSeparator();
            MenuNetworkItemIPv6Secure = new ToolStripMenuItem();
            MenuOptions = new ToolStripMenuItem();
            MenuOptionsItemClearAllOnClose = new ToolStripMenuItem();
            MenuOptionsItemFileTypeSecure = new ToolStripMenuItem();
            MenuOptionsItemCompress = new ToolStripMenuItem();
            MenuOptionsSeparator = new ToolStripSeparator();
            MenuOptionsItemOnlyPeer2PeerChat = new ToolStripMenuItem();
            MenuOptionsMenuMode = new ToolStripMenuItem();
            MenuOptionsItemPeer2Peer = new ToolStripMenuItem();
            MenuOptionsItemServerSession = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            MenuView = new ToolStripMenuItem();
            MenuViewItemLeftRíght = new ToolStripMenuItem();
            MenuViewItemTopBottom = new ToolStripMenuItem();
            MenuViewItem1View = new ToolStripMenuItem();
            SeparatorMenu0 = new ToolStripSeparator();
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
            ButtonDel = new Button();
            ButtonAdd = new Button();
            listBoxContacts = new ListBox();
            ButtonInviteChatRoom = new Button();
            TextBoxPipe = new TextBox();
            TextBoxChatSession = new TextBox();
            ButtonCheck = new Button();
            ComboBoxContacts = new ComboBox();
            ComboBoxIp = new ComboBox();
            ComboBoxSecretKey = new ComboBox();
            RichTextBoxChat = new RichTextBox();
            PanelDestination = new Panel();
            PeerServerSwitch = new EU.CqrXs.WinForm.SecureChat.Controls.Panels.PeerServerSwitchPanel(components);
            LinkedLabelsBox = new EU.CqrXs.WinForm.SecureChat.Controls.GroupBoxes.LinkLabelsBox(components);
            DragnDropBoxFiles = new EU.CqrXs.WinForm.SecureChat.Controls.GroupBoxes.DragNDropBox(components);
            ButtonSend = new Button();
            ButtonAttach = new Button();
            PanelCenter = new Panel();
            RichTextBoxOneView = new RichTextBox();
            PanelBottom = new Panel();
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
            StripMenu.Items.AddRange(new ToolStripItem[] { MenuFile, toolStripSeparator1, MenuCommands, MenuContacts, toolStripSeparator2, MenuNetwork, MenuOptions, toolStripSeparator3, MenuView, SeparatorMenu0, MenuHelp });
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
            MenuFile.ShortcutKeys = Keys.Alt | Keys.F5;
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
            // toolStripSeparator1
            // 
            toolStripSeparator1.BackColor = SystemColors.MenuBar;
            toolStripSeparator1.ForeColor = SystemColors.MenuText;
            toolStripSeparator1.Margin = new Padding(1);
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 23);
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
            MenuCommandsItemSend.AutoToolTip = true;
            MenuCommandsItemSend.BackColor = SystemColors.MenuBar;
            MenuCommandsItemSend.Name = "MenuCommandsItemSend";
            MenuCommandsItemSend.ShortcutKeys = Keys.Control | Keys.S;
            MenuCommandsItemSend.Size = new Size(178, 22);
            MenuCommandsItemSend.Text = "send";
            MenuCommandsItemSend.ToolTipText = "sends a message";
            // 
            // MenuCommandsItemAttach
            // 
            MenuCommandsItemAttach.BackColor = SystemColors.MenuBar;
            MenuCommandsItemAttach.Name = "MenuCommandsItemAttach";
            MenuCommandsItemAttach.ShortcutKeys = Keys.Control | Keys.A;
            MenuCommandsItemAttach.Size = new Size(178, 22);
            MenuCommandsItemAttach.Text = "attach";
            MenuCommandsItemAttach.ToolTipText = "attaches file to send; in prototype only  file extension image audio and video is supported";
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
            // 
            // MenuCommandsItemClear
            // 
            MenuCommandsItemClear.BackColor = SystemColors.MenuBar;
            MenuCommandsItemClear.Name = "MenuCommandsItemClear";
            MenuCommandsItemClear.ShortcutKeys = Keys.Control | Keys.Delete;
            MenuCommandsItemClear.Size = new Size(178, 22);
            MenuCommandsItemClear.Text = "clear";
            MenuCommandsItemClear.ToolTipText = "clears completey all chat windows";
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
            // toolStripSeparator2
            // 
            toolStripSeparator2.BackColor = SystemColors.MenuBar;
            toolStripSeparator2.ForeColor = SystemColors.MenuText;
            toolStripSeparator2.Margin = new Padding(1);
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 23);
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
            // MenuOptions
            // 
            MenuOptions.BackColor = SystemColors.MenuBar;
            MenuOptions.DropDownItems.AddRange(new ToolStripItem[] { MenuOptionsItemClearAllOnClose, MenuOptionsItemFileTypeSecure, MenuOptionsItemCompress, MenuOptionsSeparator, MenuOptionsItemOnlyPeer2PeerChat, MenuOptionsMenuMode });
            MenuOptions.Name = "MenuOptions";
            MenuOptions.Size = new Size(72, 25);
            MenuOptions.Text = "options";
            // 
            // MenuOptionsItemClearAllOnClose
            // 
            MenuOptionsItemClearAllOnClose.AutoToolTip = true;
            MenuOptionsItemClearAllOnClose.BackColor = SystemColors.Menu;
            MenuOptionsItemClearAllOnClose.Name = "MenuOptionsItemClearAllOnClose";
            MenuOptionsItemClearAllOnClose.Size = new Size(271, 22);
            MenuOptionsItemClearAllOnClose.Text = "✂␡ chats, files 📁 on close";
            MenuOptionsItemClearAllOnClose.ToolTipText = "When closing this application, ␡ all chats, saved 📁attachments and private 🔑keys";
            MenuOptionsItemClearAllOnClose.Click += MenuOptionsItemClearAllOnClose_Click;
            // 
            // MenuOptionsItemFileTypeSecure
            // 
            MenuOptionsItemFileTypeSecure.BackColor = SystemColors.Menu;
            MenuOptionsItemFileTypeSecure.Name = "MenuOptionsItemFileTypeSecure";
            MenuOptionsItemFileTypeSecure.Size = new Size(271, 22);
            MenuOptionsItemFileTypeSecure.Text = "✉🔒secure file types 📁 only";
            MenuOptionsItemFileTypeSecure.ToolTipText = "e.g.images, pdf, ps, rtf, instead of possible macro virus containing formats";
            MenuOptionsItemFileTypeSecure.Click += MenuOptionsItemFileTypeSecure_Click;
            // 
            // MenuOptionsItemCompress
            // 
            MenuOptionsItemCompress.BackColor = SystemColors.Menu;
            MenuOptionsItemCompress.Name = "MenuOptionsItemCompress";
            MenuOptionsItemCompress.Size = new Size(271, 22);
            MenuOptionsItemCompress.Text = "zip 📁 before ✉";
            MenuOptionsItemCompress.ToolTipText = "🗜compress 📁file attachments before ✉sending";
            MenuOptionsItemCompress.Click += MenuOptionsItemCompress_Click;
            // 
            // MenuOptionsSeparator
            // 
            MenuOptionsSeparator.Name = "MenuOptionsSeparator";
            MenuOptionsSeparator.Size = new Size(268, 6);
            // 
            // MenuOptionsItemOnlyPeer2PeerChat
            // 
            MenuOptionsItemOnlyPeer2PeerChat.BackColor = SystemColors.Menu;
            MenuOptionsItemOnlyPeer2PeerChat.Name = "MenuOptionsItemOnlyPeer2PeerChat";
            MenuOptionsItemOnlyPeer2PeerChat.Size = new Size(271, 22);
            MenuOptionsItemOnlyPeer2PeerChat.Text = "peer-2-peer chat only";
            MenuOptionsItemOnlyPeer2PeerChat.ToolTipText = "Only use this chat for peer 2 peer network, not server sessions with invitations";
            MenuOptionsItemOnlyPeer2PeerChat.Click += MenuOptionsItemOnlyPeer2PeerChat_Click;
            // 
            // MenuOptionsMenuMode
            // 
            MenuOptionsMenuMode.DropDownItems.AddRange(new ToolStripItem[] { MenuOptionsItemPeer2Peer, MenuOptionsItemServerSession });
            MenuOptionsMenuMode.Name = "MenuOptionsMenuMode";
            MenuOptionsMenuMode.Size = new Size(271, 22);
            MenuOptionsMenuMode.Text = "🔒chat mode";
            MenuOptionsMenuMode.ToolTipText = "set chat mode either peer-2-peer or server session";
            // 
            // MenuOptionsItemPeer2Peer
            // 
            MenuOptionsItemPeer2Peer.CheckOnClick = true;
            MenuOptionsItemPeer2Peer.Name = "MenuOptionsItemPeer2Peer";
            MenuOptionsItemPeer2Peer.Size = new Size(206, 22);
            MenuOptionsItemPeer2Peer.Text = "peer-2-peer mode";
            MenuOptionsItemPeer2Peer.Click += MenuOptionsItemPeer2Peer_Click;
            // 
            // MenuOptionsItemServerSession
            // 
            MenuOptionsItemServerSession.CheckOnClick = true;
            MenuOptionsItemServerSession.Name = "MenuOptionsItemServerSession";
            MenuOptionsItemServerSession.Size = new Size(206, 22);
            MenuOptionsItemServerSession.Text = "chat server session";
            MenuOptionsItemServerSession.Click += MenuOptionsItemServerSession_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.BackColor = SystemColors.MenuBar;
            toolStripSeparator3.ForeColor = SystemColors.MenuText;
            toolStripSeparator3.Margin = new Padding(1);
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 23);
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
            // SeparatorMenu0
            // 
            SeparatorMenu0.BackColor = SystemColors.MenuBar;
            SeparatorMenu0.ForeColor = SystemColors.MenuText;
            SeparatorMenu0.Margin = new Padding(1);
            SeparatorMenu0.Name = "SeparatorMenu0";
            SeparatorMenu0.Size = new Size(6, 23);
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
            MenuHelp.Size = new Size(24, 25);
            MenuHelp.Text = "?";
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
            StripStatus.GripStyle = ToolStripGripStyle.Visible;
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
            StripStatusLabel.Size = new Size(540, 19);
            StripStatusLabel.Text = "Status";
            // 
            // StripProgressBar
            // 
            StripProgressBar.AutoSize = false;
            StripProgressBar.Margin = new Padding(1);
            StripProgressBar.Name = "StripProgressBar";
            StripProgressBar.Size = new Size(420, 20);
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
            PictureBoxYou.Location = new Point(6, 59);
            PictureBoxYou.Margin = new Padding(1);
            PictureBoxYou.Name = "PictureBoxYou";
            PictureBoxYou.Padding = new Padding(1);
            PictureBoxYou.Size = new Size(154, 154);
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
            ButtonKey.Location = new Point(304, 2);
            ButtonKey.Margin = new Padding(1);
            ButtonKey.Name = "ButtonKey";
            ButtonKey.Padding = new Padding(1);
            ButtonKey.Size = new Size(40, 27);
            ButtonKey.TabIndex = 12;
            ButtonKey.UseVisualStyleBackColor = false;
            ButtonKey.Click += SecretKey_Update;
            // 
            // PanelEnCodeCrypt
            // 
            PanelEnCodeCrypt.BackColor = SystemColors.ActiveBorder;
            PanelEnCodeCrypt.Controls.Add(ButtonDel);
            PanelEnCodeCrypt.Controls.Add(ButtonAdd);
            PanelEnCodeCrypt.Controls.Add(listBoxContacts);
            PanelEnCodeCrypt.Controls.Add(ButtonInviteChatRoom);
            PanelEnCodeCrypt.Controls.Add(TextBoxPipe);
            PanelEnCodeCrypt.Controls.Add(TextBoxChatSession);
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
            // ButtonDel
            // 
            ButtonDel.BackColor = SystemColors.ButtonFace;
            ButtonDel.BackgroundImageLayout = ImageLayout.Center;
            ButtonDel.Font = new Font("Lucida Sans Unicode", 9.5F, FontStyle.Bold);
            ButtonDel.ForeColor = SystemColors.ActiveCaptionText;
            ButtonDel.Location = new Point(616, 34);
            ButtonDel.Margin = new Padding(1);
            ButtonDel.Name = "ButtonDel";
            ButtonDel.Padding = new Padding(1);
            ButtonDel.Size = new Size(32, 25);
            ButtonDel.TabIndex = 24;
            ButtonDel.Text = "⇠";
            ButtonDel.TextAlign = ContentAlignment.MiddleRight;
            ButtonDel.UseVisualStyleBackColor = false;
            ButtonDel.Click += ButtonDel_Click;
            // 
            // ButtonAdd
            // 
            ButtonAdd.BackColor = SystemColors.ButtonFace;
            ButtonAdd.BackgroundImageLayout = ImageLayout.Center;
            ButtonAdd.Font = new Font("Lucida Sans Unicode", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ButtonAdd.ForeColor = SystemColors.ActiveCaptionText;
            ButtonAdd.Location = new Point(616, 4);
            ButtonAdd.Margin = new Padding(1);
            ButtonAdd.Name = "ButtonAdd";
            ButtonAdd.Padding = new Padding(1);
            ButtonAdd.Size = new Size(32, 28);
            ButtonAdd.TabIndex = 23;
            ButtonAdd.Text = "⇒";
            ButtonAdd.TextAlign = ContentAlignment.MiddleRight;
            ButtonAdd.UseVisualStyleBackColor = false;
            ButtonAdd.Click += ButtonAdd_Click;
            // 
            // listBoxContacts
            // 
            listBoxContacts.Enabled = false;
            listBoxContacts.FormattingEnabled = true;
            listBoxContacts.Location = new Point(654, 5);
            listBoxContacts.Margin = new Padding(1);
            listBoxContacts.Name = "listBoxContacts";
            listBoxContacts.ScrollAlwaysVisible = true;
            listBoxContacts.Size = new Size(334, 52);
            listBoxContacts.TabIndex = 22;
            // 
            // ButtonInviteChatRoom
            // 
            ButtonInviteChatRoom.BackColor = SystemColors.ButtonFace;
            ButtonInviteChatRoom.BackgroundImageLayout = ImageLayout.Center;
            ButtonInviteChatRoom.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ButtonInviteChatRoom.ForeColor = SystemColors.ActiveCaptionText;
            ButtonInviteChatRoom.Location = new Point(536, 32);
            ButtonInviteChatRoom.Margin = new Padding(1);
            ButtonInviteChatRoom.Name = "ButtonInviteChatRoom";
            ButtonInviteChatRoom.Padding = new Padding(1);
            ButtonInviteChatRoom.Size = new Size(76, 27);
            ButtonInviteChatRoom.TabIndex = 21;
            ButtonInviteChatRoom.Text = "ChatRoom";
            ButtonInviteChatRoom.TextAlign = ContentAlignment.MiddleRight;
            ButtonInviteChatRoom.UseVisualStyleBackColor = false;
            // 
            // TextBoxPipe
            // 
            TextBoxPipe.Font = new Font("Lucida Sans Unicode", 8.75F);
            TextBoxPipe.HideSelection = false;
            TextBoxPipe.Location = new Point(211, 4);
            TextBoxPipe.Margin = new Padding(1);
            TextBoxPipe.Name = "TextBoxPipe";
            TextBoxPipe.ReadOnly = true;
            TextBoxPipe.Size = new Size(85, 25);
            TextBoxPipe.TabIndex = 13;
            // 
            // TextBoxChatSession
            // 
            TextBoxChatSession.Enabled = false;
            TextBoxChatSession.Font = new Font("Lucida Sans Unicode", 8F);
            TextBoxChatSession.HideSelection = false;
            TextBoxChatSession.Location = new Point(350, 34);
            TextBoxChatSession.Margin = new Padding(1);
            TextBoxChatSession.Name = "TextBoxChatSession";
            TextBoxChatSession.Size = new Size(181, 24);
            TextBoxChatSession.TabIndex = 19;
            // 
            // ButtonCheck
            // 
            ButtonCheck.BackColor = SystemColors.ButtonHighlight;
            ButtonCheck.BackgroundImageLayout = ImageLayout.Center;
            ButtonCheck.Font = new Font("Lucida Sans Unicode", 10F, FontStyle.Bold);
            ButtonCheck.ForeColor = SystemColors.ActiveCaptionText;
            ButtonCheck.Image = Properties.de.Resources.CableWireCut;
            ButtonCheck.Location = new Point(304, 34);
            ButtonCheck.Margin = new Padding(1);
            ButtonCheck.Name = "ButtonCheck";
            ButtonCheck.Padding = new Padding(1);
            ButtonCheck.Size = new Size(40, 25);
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
            ComboBoxContacts.Location = new Point(350, 5);
            ComboBoxContacts.Margin = new Padding(1);
            ComboBoxContacts.Name = "ComboBoxContacts";
            ComboBoxContacts.Size = new Size(262, 24);
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
            ComboBoxIp.Location = new Point(4, 34);
            ComboBoxIp.Margin = new Padding(1);
            ComboBoxIp.Name = "ComboBoxIp";
            ComboBoxIp.Size = new Size(292, 24);
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
            ComboBoxSecretKey.Location = new Point(4, 5);
            ComboBoxSecretKey.Margin = new Padding(1);
            ComboBoxSecretKey.Name = "ComboBoxSecretKey";
            ComboBoxSecretKey.Size = new Size(201, 24);
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
            RichTextBoxChat.Location = new Point(4, 4);
            RichTextBoxChat.Margin = new Padding(1);
            RichTextBoxChat.Name = "RichTextBoxChat";
            RichTextBoxChat.Size = new Size(816, 122);
            RichTextBoxChat.TabIndex = 41;
            RichTextBoxChat.Text = "";
            // 
            // PanelDestination
            // 
            PanelDestination.AllowDrop = true;
            PanelDestination.BackColor = SystemColors.AppWorkspace;
            PanelDestination.Controls.Add(PeerServerSwitch);
            PanelDestination.Controls.Add(LinkedLabelsBox);
            PanelDestination.Controls.Add(PictureBoxYou);
            PanelDestination.ForeColor = SystemColors.ActiveCaptionText;
            PanelDestination.Location = new Point(824, 102);
            PanelDestination.Margin = new Padding(0);
            PanelDestination.Name = "PanelDestination";
            PanelDestination.Size = new Size(168, 460);
            PanelDestination.TabIndex = 70;
            // 
            // PeerServerSwitch
            // 
            PeerServerSwitch.AllowDrop = true;
            PeerServerSwitch.BackColor = SystemColors.GradientActiveCaption;
            PeerServerSwitch.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PeerServerSwitch.Location = new Point(6, 1);
            PeerServerSwitch.Margin = new Padding(1);
            PeerServerSwitch.Name = "PeerServerSwitch";
            PeerServerSwitch.Padding = new Padding(1);
            PeerServerSwitch.Size = new Size(154, 48);
            PeerServerSwitch.TabIndex = 84;
            // 
            // LinkedLabelsBox
            // 
            LinkedLabelsBox.AllowDrop = true;
            LinkedLabelsBox.BackColor = SystemColors.GradientActiveCaption;
            LinkedLabelsBox.Font = new Font("Lucida Sans Unicode", 9F);
            LinkedLabelsBox.Location = new Point(4, 220);
            LinkedLabelsBox.Margin = new Padding(0);
            LinkedLabelsBox.Name = "LinkedLabelsBox";
            LinkedLabelsBox.Padding = new Padding(0);
            LinkedLabelsBox.Size = new Size(164, 234);
            LinkedLabelsBox.TabIndex = 83;
            LinkedLabelsBox.TabStop = false;
            LinkedLabelsBox.Text = "Attachments";
            // 
            // DragnDropBoxFiles
            // 
            DragnDropBoxFiles.AllowDrop = true;
            DragnDropBoxFiles.BackColor = SystemColors.ControlLightLight;
            DragnDropBoxFiles.Font = new Font("Lucida Sans Unicode", 8.5F);
            DragnDropBoxFiles.Location = new Point(830, 4);
            DragnDropBoxFiles.Margin = new Padding(1);
            DragnDropBoxFiles.Name = "DragnDropBoxFiles";
            DragnDropBoxFiles.Padding = new Padding(1);
            DragnDropBoxFiles.Size = new Size(154, 91);
            DragnDropBoxFiles.TabIndex = 81;
            DragnDropBoxFiles.TabStop = false;
            DragnDropBoxFiles.Text = "DragnDropBoxFiles";
            // 
            // ButtonSend
            // 
            ButtonSend.BackColor = SystemColors.ButtonHighlight;
            ButtonSend.Font = new Font("Lucida Sans Unicode", 9F);
            ButtonSend.ForeColor = SystemColors.ActiveCaptionText;
            ButtonSend.Location = new Point(830, 100);
            ButtonSend.Margin = new Padding(1);
            ButtonSend.Name = "ButtonSend";
            ButtonSend.Padding = new Padding(1);
            ButtonSend.Size = new Size(75, 27);
            ButtonSend.TabIndex = 83;
            ButtonSend.Text = "Send";
            ButtonSend.UseVisualStyleBackColor = false;
            // 
            // ButtonAttach
            // 
            ButtonAttach.BackColor = SystemColors.ButtonHighlight;
            ButtonAttach.Font = new Font("Lucida Sans Unicode", 9F);
            ButtonAttach.ForeColor = SystemColors.ActiveCaptionText;
            ButtonAttach.Location = new Point(909, 100);
            ButtonAttach.Margin = new Padding(1);
            ButtonAttach.Name = "ButtonAttach";
            ButtonAttach.Padding = new Padding(1);
            ButtonAttach.Size = new Size(75, 27);
            ButtonAttach.TabIndex = 82;
            ButtonAttach.Text = "Attach";
            ButtonAttach.UseVisualStyleBackColor = false;
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
            PanelBottom.Controls.Add(DragnDropBoxFiles);
            PanelBottom.Controls.Add(ButtonAttach);
            PanelBottom.Controls.Add(ButtonSend);
            PanelBottom.Controls.Add(RichTextBoxChat);
            PanelBottom.ForeColor = SystemColors.ActiveCaption;
            PanelBottom.Location = new Point(0, 565);
            PanelBottom.Margin = new Padding(1);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(992, 130);
            PanelBottom.TabIndex = 40;
            // 
            // SecureChat
            // 
            AllowDrop = true;
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
            MaximizeBox = false;
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
        private ToolStripMenuItem MenuOptionsItemFileTypeSecure;
        private ToolStripMenuItem MenuOptionsItemClearAllOnClose;
        private ToolStripMenuItem MenuOptionsItemOnlyPeer2PeerChat;
        private ToolStripSeparator MenuOptionsSeparator;

        private ToolStripMenuItem MenuHelp;
        private ToolStripMenuItem MenuHelpItemViewHelp;
        private ToolStripMenuItem MenuHelpItemInfo;
        private ToolStripMenuItem MenuHelpItemAbout;
        private Controls.GroupBoxes.LinkLabelsBox LinkedLabelsBox;
        private Controls.Panels.PeerServerSwitchPanel PeerServerSwitch;
        private Button ButtonSend;
        private TextBox TextBoxChatSession;
        private ToolStripSeparator SeparatorMenu0;
        private Button ButtonInviteChatRoom;
        private ToolStripStatusLabel StripStatusLabel;
        private ToolStripProgressBar StripProgressBar;
        private ToolStripMenuItem MenuOptionsMenuMode;
        private ToolStripMenuItem MenuOptionsItemServerSession;
        private ToolStripMenuItem MenuOptionsItemPeer2Peer;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator3;
        private GroupBoxes.DragNDropBox DragnDropBoxFiles;
        private ListBox listBoxContacts;
        private Button ButtonAdd;
        private Button ButtonDel;
    }

}