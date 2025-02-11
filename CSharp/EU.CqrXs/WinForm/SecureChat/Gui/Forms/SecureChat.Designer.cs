using EU.CqrXs.Framework.Core.Net.WebHttp;
using System.Net;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms
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
            StripMenu = new MenuStrip();
            MenuFile = new ToolStripMenuItem();
            MenuFileItemOpen = new ToolStripMenuItem();
            MenuFileItemSave = new ToolStripMenuItem();
            MenuFileSeparator = new ToolStripSeparator();
            MenuFileItemPersist = new ToolStripMenuItem();
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
            toolMenuConnect = new ToolStripMenuItem();
            menuConnectItemFriend = new ToolStripMenuItem();
            menuConnectSeparator = new ToolStripSeparator();
            menuConnectComboBoxIps = new ToolStripComboBox();
            menuConnectSeparatorLast = new ToolStripSeparator();
            menuConnectItemLoopback = new ToolStripMenuItem();
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
            MenuHelp = new ToolStripMenuItem();
            MenuHelpItemViewHelp = new ToolStripMenuItem();
            MenuHelpItemInfo = new ToolStripMenuItem();
            MenuHelpItemAbout = new ToolStripMenuItem();
            MenuEdit = new ToolStripMenuItem();
            MenuEditItemCut = new ToolStripMenuItem();
            MenuEditItemCopy = new ToolStripMenuItem();
            MenuIEditItemPaste = new ToolStripMenuItem();
            MenuEditItemSelectAll = new ToolStripMenuItem();
            FileOpenDialog = new OpenFileDialog();
            FileSaveDialog = new SaveFileDialog();
            StripStatus = new StatusStrip();
            StripProgressBar = new ToolStripProgressBar();
            StripStatusLabel = new ToolStripStatusLabel();
            SplitChatView = new SplitContainer();
            TextBoxSource = new TextBox();
            TextBoxDestionation = new TextBox();
            PictureBoxYou = new PictureBox();
            ButtonKey = new Button();
            PanelEnCodeCrypt = new Panel();
            ButtonCheck = new Button();
            ComboBoxSecretKey = new ComboBox();
            ComboBoxIpContact = new ComboBox();
            TextBoxPipe = new TextBox();
            RichTextBoxChat = new RichTextBox();
            PictureBoxPartner = new PictureBox();
            PanelDestination = new Panel();
            GroupBoxLinks = new Controls.GroupBoxLinkLabels();
            PanelCenter = new Panel();
            RichTextBoxOneView = new RichTextBox();
            PanelBottom = new Panel();
            ButtonAttach = new Button();
            ButtonSend = new Button();
            ButtonClear = new Button();
            StripMenu.SuspendLayout();
            StripStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SplitChatView).BeginInit();
            SplitChatView.Panel1.SuspendLayout();
            SplitChatView.Panel2.SuspendLayout();
            SplitChatView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBoxYou).BeginInit();
            PanelEnCodeCrypt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBoxPartner).BeginInit();
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
            StripMenu.Items.AddRange(new ToolStripItem[] { MenuFile, MenuView, MenuNetwork, toolMenuConnect, MenuCommands, MenuContacts, MenuHelp });
            StripMenu.Location = new Point(0, 0);
            StripMenu.Name = "StripMenu";
            StripMenu.RenderMode = ToolStripRenderMode.System;
            StripMenu.Size = new Size(976, 25);
            StripMenu.TabIndex = 1;
            StripMenu.Text = "StripMenu";
            // 
            // MenuFile
            // 
            MenuFile.BackColor = SystemColors.MenuBar;
            MenuFile.DropDownItems.AddRange(new ToolStripItem[] { MenuFileItemOpen, MenuFileItemSave, MenuFileSeparator, MenuFileItemPersist, MenuFileSeparatorExit, MenuFileItemExit });
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
            MenuFileItemOpen.ForeColor = SystemColors.MenuText;
            MenuFileItemOpen.Margin = new Padding(1);
            MenuFileItemOpen.Name = "MenuFileItemOpen";
            MenuFileItemOpen.ShortcutKeys = Keys.Control | Keys.I;
            MenuFileItemOpen.Size = new Size(214, 22);
            MenuFileItemOpen.Text = "import chats";
            MenuFileItemOpen.TextImageRelation = TextImageRelation.TextAboveImage;
            MenuFileItemOpen.ToolTipText = "imports saved chats from a file";
            MenuFileItemOpen.Click += toolStripMenuItemLoad_Click;
            // 
            // MenuFileItemSave
            // 
            MenuFileItemSave.AutoToolTip = true;
            MenuFileItemSave.BackColor = SystemColors.MenuBar;
            MenuFileItemSave.BackgroundImageLayout = ImageLayout.Center;
            MenuFileItemSave.ForeColor = SystemColors.MenuText;
            MenuFileItemSave.Margin = new Padding(1);
            MenuFileItemSave.Name = "MenuFileItemSave";
            MenuFileItemSave.ShortcutKeys = Keys.Control | Keys.E;
            MenuFileItemSave.Size = new Size(214, 22);
            MenuFileItemSave.Text = "export chats";
            MenuFileItemSave.TextImageRelation = TextImageRelation.TextAboveImage;
            MenuFileItemSave.ToolTipText = "saves chats on local harddisk not on server";
            MenuFileItemSave.Click += toolStripMenuItemSave_Click;
            // 
            // MenuFileSeparator
            // 
            MenuFileSeparator.BackColor = SystemColors.MenuBar;
            MenuFileSeparator.ForeColor = SystemColors.MenuText;
            MenuFileSeparator.Margin = new Padding(1);
            MenuFileSeparator.Name = "MenuFileSeparator";
            MenuFileSeparator.Size = new Size(211, 6);
            // 
            // MenuFileItemPersist
            // 
            MenuFileItemPersist.AutoToolTip = true;
            MenuFileItemPersist.BackColor = SystemColors.MenuHighlight;
            MenuFileItemPersist.BackgroundImageLayout = ImageLayout.Center;
            MenuFileItemPersist.Checked = true;
            MenuFileItemPersist.CheckState = CheckState.Checked;
            MenuFileItemPersist.ForeColor = SystemColors.MenuText;
            MenuFileItemPersist.Margin = new Padding(1);
            MenuFileItemPersist.Name = "MenuFileItemPersist";
            MenuFileItemPersist.ShortcutKeys = Keys.Control | Keys.P;
            MenuFileItemPersist.Size = new Size(214, 22);
            MenuFileItemPersist.Text = "persist chats";
            MenuFileItemPersist.TextImageRelation = TextImageRelation.TextAboveImage;
            MenuFileItemPersist.ToolTipText = "if cheked chats will be saved on local harddisk automatically, so that you can reimport elsewhere";
            // 
            // MenuFileSeparatorExit
            // 
            MenuFileSeparatorExit.BackColor = SystemColors.MenuBar;
            MenuFileSeparatorExit.ForeColor = SystemColors.MenuText;
            MenuFileSeparatorExit.Margin = new Padding(1);
            MenuFileSeparatorExit.Name = "MenuFileSeparatorExit";
            MenuFileSeparatorExit.Size = new Size(211, 6);
            // 
            // MenuFileItemExit
            // 
            MenuFileItemExit.BackColor = SystemColors.MenuBar;
            MenuFileItemExit.BackgroundImageLayout = ImageLayout.Center;
            MenuFileItemExit.ForeColor = SystemColors.MenuText;
            MenuFileItemExit.Name = "MenuFileItemExit";
            MenuFileItemExit.ShortcutKeys = Keys.Alt | Keys.F4;
            MenuFileItemExit.Size = new Size(214, 22);
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
            MenuItemExternalIp.ShortcutKeys = Keys.Alt | Keys.E;
            MenuItemExternalIp.Size = new Size(206, 22);
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
            // toolMenuConnect
            // 
            toolMenuConnect.BackColor = SystemColors.MenuBar;
            toolMenuConnect.DropDownItems.AddRange(new ToolStripItem[] { menuConnectItemFriend, menuConnectSeparator, menuConnectComboBoxIps, menuConnectSeparatorLast, menuConnectItemLoopback });
            toolMenuConnect.Name = "toolMenuConnect";
            toolMenuConnect.Size = new Size(74, 21);
            toolMenuConnect.Text = "connect";
            toolMenuConnect.ToolTipText = "connects directly to an peered ip address or contact over a proxy";
            // 
            // menuConnectItemFriend
            // 
            menuConnectItemFriend.BackColor = SystemColors.Menu;
            menuConnectItemFriend.Name = "menuConnectItemFriend";
            menuConnectItemFriend.ShortcutKeys = Keys.Control | Keys.F;
            menuConnectItemFriend.Size = new Size(275, 22);
            menuConnectItemFriend.Text = "friend (over proxy)";
            menuConnectItemFriend.ToolTipText = "connects to a friend over proxy server";
            // 
            // menuConnectSeparator
            // 
            menuConnectSeparator.Name = "menuConnectSeparator";
            menuConnectSeparator.Size = new Size(272, 6);
            // 
            // menuConnectComboBoxIps
            // 
            menuConnectComboBoxIps.Name = "menuConnectComboBoxIps";
            menuConnectComboBoxIps.Size = new Size(144, 23);
            // 
            // menuConnectSeparatorLast
            // 
            menuConnectSeparatorLast.Name = "menuConnectSeparatorLast";
            menuConnectSeparatorLast.Size = new Size(272, 6);
            // 
            // menuConnectItemLoopback
            // 
            menuConnectItemLoopback.BackColor = SystemColors.Menu;
            menuConnectItemLoopback.Name = "menuConnectItemLoopback";
            menuConnectItemLoopback.ShortcutKeys = Keys.Control | Keys.D0;
            menuConnectItemLoopback.Size = new Size(275, 22);
            menuConnectItemLoopback.Text = "me myself (loopback)";
            menuConnectItemLoopback.ToolTipText = "import contacts from address book";
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
            MenuContactstemExport.Name = "MenuContactstemExport";
            MenuContactstemExport.ShortcutKeys = Keys.Alt | Keys.E;
            MenuContactstemExport.Size = new Size(233, 22);
            MenuContactstemExport.Text = "export contacts";
            MenuContactstemExport.ToolTipText = "export contacts to a json file";
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
            MenuHelp.Size = new Size(24, 21);
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
            MenuHelpItemViewHelp.Size = new Size(167, 24);
            MenuHelpItemViewHelp.Text = "help";
            MenuHelpItemViewHelp.ToolTipText = "displays help";
            MenuHelpItemViewHelp.Click += MenuHelpItemViewHelp_Click;
            // 
            // MenuHelpItemInfo
            // 
            MenuHelpItemInfo.BackColor = SystemColors.MenuBar;
            MenuHelpItemInfo.BackgroundImageLayout = ImageLayout.None;
            MenuHelpItemInfo.ForeColor = SystemColors.MenuText;
            MenuHelpItemInfo.Name = "MenuHelpItemInfo";
            MenuHelpItemInfo.Size = new Size(167, 22);
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
            MenuHelpItemAbout.Size = new Size(167, 24);
            MenuHelpItemAbout.Text = "about";
            MenuHelpItemAbout.TextImageRelation = TextImageRelation.TextAboveImage;
            MenuHelpItemAbout.ToolTipText = "displays a large modal dialog with version info and  copy left info";
            MenuHelpItemAbout.Click += MenuHelpItemAbout_Click;
            // 
            // MenuEdit
            // 
            MenuEdit.BackColor = SystemColors.MenuBar;
            MenuEdit.DropDownItems.AddRange(new ToolStripItem[] { MenuEditItemCut, MenuEditItemCopy, MenuIEditItemPaste, MenuEditItemSelectAll });
            MenuEdit.Enabled = false;
            MenuEdit.Name = "MenuEdit";
            MenuEdit.Size = new Size(46, 21);
            MenuEdit.Text = "Edit";
            // 
            // MenuEditItemCut
            // 
            MenuEditItemCut.BackColor = SystemColors.Menu;
            MenuEditItemCut.Enabled = false;
            MenuEditItemCut.Name = "MenuEditItemCut";
            MenuEditItemCut.ShortcutKeys = Keys.Control | Keys.X;
            MenuEditItemCut.Size = new Size(164, 22);
            MenuEditItemCut.Text = "Cat ✂";
            // 
            // MenuEditItemCopy
            // 
            MenuEditItemCopy.BackColor = SystemColors.Menu;
            MenuEditItemCopy.Enabled = false;
            MenuEditItemCopy.Name = "MenuEditItemCopy";
            MenuEditItemCopy.ShortcutKeys = Keys.Control | Keys.C;
            MenuEditItemCopy.Size = new Size(164, 22);
            MenuEditItemCopy.Text = "Copy";
            // 
            // MenuIEditItemPaste
            // 
            MenuIEditItemPaste.BackColor = SystemColors.Menu;
            MenuIEditItemPaste.Enabled = false;
            MenuIEditItemPaste.Name = "MenuIEditItemPaste";
            MenuIEditItemPaste.ShortcutKeys = Keys.Control | Keys.V;
            MenuIEditItemPaste.Size = new Size(164, 22);
            MenuIEditItemPaste.Text = "Paste";
            // 
            // MenuEditItemSelectAll
            // 
            MenuEditItemSelectAll.BackColor = SystemColors.Menu;
            MenuEditItemSelectAll.Enabled = false;
            MenuEditItemSelectAll.Name = "MenuEditItemSelectAll";
            MenuEditItemSelectAll.ShortcutKeys = Keys.Control | Keys.A;
            MenuEditItemSelectAll.Size = new Size(164, 22);
            MenuEditItemSelectAll.Text = "Select All";
            // 
            // FileOpenDialog
            // 
            FileOpenDialog.FileName = "FileOpenDialog";
            FileOpenDialog.Title = "FileOpenDialog";
            // 
            // FileSaveDialog
            // 
            FileSaveDialog.InitialDirectory = "C:\\Windows\\Temp";
            FileSaveDialog.RestoreDirectory = true;
            FileSaveDialog.ShowHiddenFiles = true;
            FileSaveDialog.SupportMultiDottedExtensions = true;
            FileSaveDialog.Title = "Save File";
            // 
            // StripStatus
            // 
            StripStatus.GripMargin = new Padding(1);
            StripStatus.Items.AddRange(new ToolStripItem[] { StripProgressBar, StripStatusLabel });
            StripStatus.Location = new Point(0, 689);
            StripStatus.Name = "StripStatus";
            StripStatus.Size = new Size(976, 22);
            StripStatus.TabIndex = 2;
            StripStatus.Text = "StripStatus";
            // 
            // StripProgressBar
            // 
            StripProgressBar.Margin = new Padding(1);
            StripProgressBar.Name = "StripProgressBar";
            StripProgressBar.Size = new Size(300, 20);
            StripProgressBar.Step = 4;
            // 
            // StripStatusLabel
            // 
            StripStatusLabel.Margin = new Padding(0, 2, 0, 1);
            StripStatusLabel.Name = "StripStatusLabel";
            StripStatusLabel.Size = new Size(659, 19);
            StripStatusLabel.Spring = true;
            StripStatusLabel.Text = "Status";
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
            TextBoxSource.Font = new Font("Lucida Sans Unicode", 10F);
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
            TextBoxDestionation.Font = new Font("Lucida Sans Unicode", 10F);
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
            PictureBoxYou.BackgroundImageLayout = ImageLayout.Stretch;
            PictureBoxYou.Location = new Point(2, 1);
            PictureBoxYou.Margin = new Padding(1);
            PictureBoxYou.Name = "PictureBoxYou";
            PictureBoxYou.Padding = new Padding(1);
            PictureBoxYou.Size = new Size(148, 148);
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
            ButtonKey.Location = new Point(254, 4);
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
            PanelEnCodeCrypt.Controls.Add(ButtonCheck);
            PanelEnCodeCrypt.Controls.Add(ComboBoxSecretKey);
            PanelEnCodeCrypt.Controls.Add(ComboBoxIpContact);
            PanelEnCodeCrypt.Controls.Add(TextBoxPipe);
            PanelEnCodeCrypt.Controls.Add(ButtonKey);
            PanelEnCodeCrypt.ForeColor = SystemColors.WindowText;
            PanelEnCodeCrypt.Location = new Point(0, 28);
            PanelEnCodeCrypt.Margin = new Padding(0);
            PanelEnCodeCrypt.Name = "PanelEnCodeCrypt";
            PanelEnCodeCrypt.Size = new Size(976, 36);
            PanelEnCodeCrypt.TabIndex = 10;
            // 
            // ButtonCheck
            // 
            ButtonCheck.BackColor = SystemColors.ButtonHighlight;
            ButtonCheck.BackgroundImageLayout = ImageLayout.Center;
            ButtonCheck.Font = new Font("Lucida Sans Unicode", 10F, FontStyle.Bold);
            ButtonCheck.ForeColor = SystemColors.ActiveCaptionText;
            ButtonCheck.Image = Properties.de.Resources.CableWireCut;
            ButtonCheck.Location = new Point(774, 2);
            ButtonCheck.Margin = new Padding(1);
            ButtonCheck.Name = "ButtonCheck";
            ButtonCheck.Padding = new Padding(1);
            ButtonCheck.Size = new Size(34, 32);
            ButtonCheck.TabIndex = 17;
            ButtonCheck.UseVisualStyleBackColor = false;
            // 
            // ComboBoxSecretKey
            // 
            ComboBoxSecretKey.BackColor = SystemColors.ControlLightLight;
            ComboBoxSecretKey.ForeColor = SystemColors.ControlText;
            ComboBoxSecretKey.FormattingEnabled = true;
            ComboBoxSecretKey.Location = new Point(10, 6);
            ComboBoxSecretKey.Margin = new Padding(1);
            ComboBoxSecretKey.Name = "ComboBoxSecretKey";
            ComboBoxSecretKey.Size = new Size(237, 24);
            ComboBoxSecretKey.TabIndex = 11;
            ComboBoxSecretKey.Text = "[enter secret key here]";
            ComboBoxSecretKey.SelectedIndexChanged += ComboBoxSecretKey_SelectedIndexChanged;
            ComboBoxSecretKey.TextUpdate += ComboBoxSecretKey_TextUpdate;
            ComboBoxSecretKey.Leave += ComboBoxSecretKey_FocusLeave;
            // 
            // ComboBoxIpContact
            // 
            ComboBoxIpContact.BackColor = SystemColors.ControlLightLight;
            ComboBoxIpContact.Font = new Font("Lucida Sans Unicode", 10F);
            ComboBoxIpContact.ForeColor = SystemColors.ControlText;
            ComboBoxIpContact.FormattingEnabled = true;
            ComboBoxIpContact.Location = new Point(412, 6);
            ComboBoxIpContact.Margin = new Padding(1);
            ComboBoxIpContact.Name = "ComboBoxIpContact";
            ComboBoxIpContact.Size = new Size(342, 24);
            ComboBoxIpContact.TabIndex = 15;
            ComboBoxIpContact.Text = "[enter peer IPv4 or IPv6 for directly connect]";
            ComboBoxIpContact.SelectedIndexChanged += ComboBoxIpContact_SelectedIndexChanged;
            ComboBoxIpContact.TextUpdate += ComboBoxIpContact_TextUpdate;
            ComboBoxIpContact.Leave += ComboBoxIpContact_FocusLeave;
            // 
            // TextBoxPipe
            // 
            TextBoxPipe.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TextBoxPipe.HideSelection = false;
            TextBoxPipe.Location = new Point(304, 5);
            TextBoxPipe.Margin = new Padding(1);
            TextBoxPipe.Name = "TextBoxPipe";
            TextBoxPipe.ReadOnly = true;
            TextBoxPipe.Size = new Size(100, 26);
            TextBoxPipe.TabIndex = 13;
            // 
            // RichTextBoxChat
            // 
            RichTextBoxChat.BorderStyle = BorderStyle.FixedSingle;
            RichTextBoxChat.ForeColor = SystemColors.WindowText;
            RichTextBoxChat.Location = new Point(0, 0);
            RichTextBoxChat.Margin = new Padding(1);
            RichTextBoxChat.Name = "RichTextBoxChat";
            RichTextBoxChat.Size = new Size(824, 136);
            RichTextBoxChat.TabIndex = 41;
            RichTextBoxChat.Text = "";
            // 
            // PictureBoxPartner
            // 
            PictureBoxPartner.Location = new Point(2, 366);
            PictureBoxPartner.Margin = new Padding(1);
            PictureBoxPartner.Name = "PictureBoxPartner";
            PictureBoxPartner.Padding = new Padding(1);
            PictureBoxPartner.Size = new Size(148, 148);
            PictureBoxPartner.TabIndex = 72;
            PictureBoxPartner.TabStop = false;
            // 
            // PanelDestination
            // 
            PanelDestination.BackColor = SystemColors.ActiveCaption;
            PanelDestination.Controls.Add(PictureBoxYou);
            PanelDestination.Controls.Add(GroupBoxLinks);
            PanelDestination.Controls.Add(PictureBoxPartner);
            PanelDestination.ForeColor = SystemColors.ActiveCaptionText;
            PanelDestination.Location = new Point(824, 32);
            PanelDestination.Margin = new Padding(0);
            PanelDestination.Name = "PanelDestination";
            PanelDestination.Size = new Size(152, 515);
            PanelDestination.TabIndex = 70;
            // 
            // GroupBoxLinks
            // 
            GroupBoxLinks.BackColor = SystemColors.ControlLightLight;
            GroupBoxLinks.Font = new Font("Lucida Sans Unicode", 9F);
            GroupBoxLinks.Location = new Point(1, 151);
            GroupBoxLinks.Margin = new Padding(1);
            GroupBoxLinks.Name = "GroupBoxLinks";
            GroupBoxLinks.Padding = new Padding(1);
            GroupBoxLinks.Size = new Size(151, 213);
            GroupBoxLinks.TabIndex = 80;
            GroupBoxLinks.TabStop = false;
            GroupBoxLinks.Text = "Attachments";
            // 
            // PanelCenter
            // 
            PanelCenter.Controls.Add(SplitChatView);
            PanelCenter.Controls.Add(RichTextBoxOneView);
            PanelCenter.Location = new Point(8, 72);
            PanelCenter.Margin = new Padding(0);
            PanelCenter.Name = "PanelCenter";
            PanelCenter.Size = new Size(800, 460);
            PanelCenter.TabIndex = 30;
            // 
            // RichTextBoxOneView
            // 
            RichTextBoxOneView.Dock = DockStyle.Fill;
            RichTextBoxOneView.Location = new Point(0, 0);
            RichTextBoxOneView.Margin = new Padding(2);
            RichTextBoxOneView.Name = "RichTextBoxOneView";
            RichTextBoxOneView.Size = new Size(800, 460);
            RichTextBoxOneView.TabIndex = 36;
            RichTextBoxOneView.Text = "";
            RichTextBoxOneView.Visible = false;
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = SystemColors.ControlLight;
            PanelBottom.Controls.Add(RichTextBoxChat);
            PanelBottom.ForeColor = SystemColors.ActiveCaptionText;
            PanelBottom.Location = new Point(0, 549);
            PanelBottom.Margin = new Padding(1);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(824, 136);
            PanelBottom.TabIndex = 40;
            // 
            // ButtonAttach
            // 
            ButtonAttach.Location = new Point(827, 552);
            ButtonAttach.Margin = new Padding(2);
            ButtonAttach.Name = "ButtonAttach";
            ButtonAttach.Padding = new Padding(1);
            ButtonAttach.Size = new Size(144, 40);
            ButtonAttach.TabIndex = 82;
            ButtonAttach.Text = "Attach";
            ButtonAttach.UseVisualStyleBackColor = true;
            ButtonAttach.Click += ButtonAttach_Click;
            // 
            // ButtonSend
            // 
            ButtonSend.Location = new Point(827, 598);
            ButtonSend.Margin = new Padding(2);
            ButtonSend.Name = "ButtonSend";
            ButtonSend.Padding = new Padding(1);
            ButtonSend.Size = new Size(144, 40);
            ButtonSend.TabIndex = 83;
            ButtonSend.Text = "Send";
            ButtonSend.UseVisualStyleBackColor = true;
            ButtonSend.Click += ButtonSend_Click;
            // 
            // ButtonClear
            // 
            ButtonClear.Location = new Point(827, 644);
            ButtonClear.Margin = new Padding(2);
            ButtonClear.Name = "ButtonClear";
            ButtonClear.Padding = new Padding(1);
            ButtonClear.Size = new Size(144, 40);
            ButtonClear.TabIndex = 84;
            ButtonClear.Text = "Clear";
            ButtonClear.UseVisualStyleBackColor = true;
            ButtonClear.Click += ButtonClear_Click;
            // 
            // SecureChat
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(976, 711);
            Controls.Add(ButtonClear);
            Controls.Add(ButtonSend);
            Controls.Add(ButtonAttach);
            Controls.Add(PanelCenter);
            Controls.Add(PanelDestination);
            Controls.Add(PanelBottom);
            Controls.Add(PanelEnCodeCrypt);
            Controls.Add(StripStatus);
            Controls.Add(StripMenu);
            Font = new Font("Lucida Sans Unicode", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
            ((System.ComponentModel.ISupportInitialize)PictureBoxPartner).EndInit();
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
        private TextBox TextBoxPipe;
        private ComboBox ComboBoxIpContact;
        private ComboBox ComboBoxSecretKey;
        private SplitContainer SplitChatView;
        private TextBox TextBoxSource;
        private TextBox TextBoxDestionation;
        private Button ButtonKey;
        private RichTextBox RichTextBoxChat;
        private PictureBox PictureBoxYou;
        private PictureBox PictureBoxPartner;

        internal MenuStrip StripMenu;
        private StatusStrip StripStatus;
        private ToolStripMenuItem toolStripMenuMain;
        private ToolStripMenuItem MenuHelpItemAbout;
        private ToolStripMenuItem toolStripMenuItemOld;
        private ToolStripSeparator MenuNetworkSeparatorIp;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItemOpen;
        private ToolStripMenuItem toolStripMenuItemClose;
        private ToolStripMenuItem MenuHelpItemInfo;
        private ToolStripMenuItem toolStripMenuItemExit;        
        private ToolStripMenuItem MenuView;
        private ToolStripMenuItem MenuFile;
        private ToolStripMenuItem MenuFileItemOpen;
        private ToolStripMenuItem MenuFileItemSave;
        private ToolStripMenuItem MenuFileItemExit;
        private ToolStripMenuItem toolStripMenuTForms;
        private ToolStripMenuItem MenuHelp;
        private ToolStripMenuItem MenuHelpItemViewHelp;        
        private ToolStripProgressBar StripProgressBar;
        private ToolStripStatusLabel StripStatusLabel;      
        private ToolStripSeparator MenuFileSeparator;        
        private ToolStripMenuItem MenuNetwork;
        private ToolStripMenuItem MenuNetworkItemMyIps;
        private ToolStripMenuItem MenuItemFriendIp;
        private ToolStripMenuItem MenuNetworkItemProxyServers;
        private ToolStripComboBox MenuNetworkComboBoxFriendIp;
        private ToolStripMenuItem MenuNetworkItemIPv6Secure;
        private ToolStripMenuItem MenuCommands;
        private ToolStripMenuItem MenuCommandsItemSend;
        private ToolStripMenuItem MenuEdit;
        private ToolStripMenuItem MenuCommandsItemRefresh;
        private ToolStripMenuItem MenuCommandsItemClear;
        private ToolStripMenuItem MenuEditItemCut;
        private ToolStripMenuItem MenuEditItemCopy;
        private ToolStripMenuItem MenuIEditItemPaste;
        private ToolStripMenuItem MenuEditItemSelectAll;
        private ToolStripMenuItem MenuViewItemLeftRíght;
        private ToolStripMenuItem MenuViewItemTopBottom;
        private ToolStripMenuItem MenuViewItem1View;
        private ToolStripMenuItem MenuContacts;
        private ToolStripMenuItem MenuContactstemImport;
        private ToolStripMenuItem MenuContactsItemAdd;
        private ToolStripMenuItem MenuContactsItemView;
        private ToolStripMenuItem MenuContactsItemMe;        
        private RichTextBox RichTextBoxOneView;
        private ToolStripMenuItem MenuItemExternalIp;
        private ToolStripSeparator MenuContactsSeparetor;
        private ToolStripMenuItem MenuContactstemExport;
        private ToolStripMenuItem MenuFileItemPersist;
        private ToolStripSeparator MenuFileSeparatorExit;
        private ToolStripMenuItem MenuCommandsItemAttach;
        private ToolStripSeparator MenuCommandsSeperator;
        private ToolStripMenuItem toolMenuConnect;
        private ToolStripMenuItem menuConnectItemFriend;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem menuConnectItemLoopback;
        private ToolStripComboBox menuConnectComboBoxIps;
        private ToolStripSeparator menuConnectSeparator;
        private ToolStripSeparator menuConnectSeparatorLast;                      

        private OpenFileDialog FileOpenDialog;
        private SaveFileDialog FileSaveDialog;
        
        private Button ButtonAttach;
        private Controls.GroupBoxLinkLabels GroupBoxLinks;
        private Button ButtonSend;
        private Button ButtonClear;
        private Button ButtonCheck;
    }
}