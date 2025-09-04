namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    partial class BaseMenuForm
    {      

        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected internal System.ComponentModel.IContainer components = null;


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
        protected internal virtual void InitializeComponent()
        {
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
            MenuOptionsSeparator1 = new ToolStripSeparator();
            MenuOptionsPeer2Peer = new ToolStripMenuItem();
            MenuOptionsItemServerSession = new ToolStripMenuItem();
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
            MenuOptions.DropDownItems.AddRange(new ToolStripItem[] { MenuOptionsItemCompress, MenuOptionsItemFileSecure, MenuOptionsItemClearAllOnClose, MenuOptionsItemDontSendProfilePictures, MenuOptionsSeparator1, MenuOptionsPeer2Peer, MenuOptionsItemServerSession });
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
            // MenuOptionsSeparator1
            // 
            MenuOptionsSeparator1.Name = "MenuOptionsSeparator1";
            MenuOptionsSeparator1.Size = new Size(332, 6);
            // 
            // MenuOptionsPeer2Peer
            // 
            MenuOptionsPeer2Peer.CheckOnClick = true;
            MenuOptionsPeer2Peer.Name = "MenuOptionsPeer2Peer";
            MenuOptionsPeer2Peer.Size = new Size(335, 22);
            MenuOptionsPeer2Peer.Text = "peer-2-peer mode";
            MenuOptionsPeer2Peer.ToolTipText = "peer 2 peer network over IP Address direct";
            // 
            // MenuOptionsItemServerSession
            // 
            MenuOptionsItemServerSession.CheckOnClick = true;
            MenuOptionsItemServerSession.Name = "MenuOptionsItemServerSession";
            MenuOptionsItemServerSession.Size = new Size(335, 22);
            MenuOptionsItemServerSession.Text = "chat server session";
            // 
            // BaseMenuForm
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(998, 717);
            Controls.Add(StripStatus);
            Controls.Add(StripMenu);
            Font = new Font("Lucida Sans Unicode", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.fr.Resources.SatIcon;
            MainMenuStrip = StripMenu;
            Name = "BaseMenuForm";
            SizeGripStyle = SizeGripStyle.Show;
            Text = "BaseMenuForm";
            FormClosing += FormClose_Click;
            StripMenu.ResumeLayout(false);
            StripMenu.PerformLayout();
            StripStatus.ResumeLayout(false);
            StripStatus.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        protected internal MenuStrip StripMenu;
        protected internal StatusStrip StripStatus;

        protected internal ToolStripMenuItem MenuFile;
        protected internal ToolStripMenuItem MenuFileItemOpen;
        protected internal ToolStripMenuItem MenuFileItemSave;
        protected internal ToolStripSeparator MenuFileSeparatorExit;
        protected internal ToolStripMenuItem MenuFileItemExit;

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
        protected internal ToolStripSeparator MenuContactsSeparetor;
        protected internal ToolStripMenuItem MenuContactstemExport;

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
        private ToolStripSeparator MenuOptionsSeparator1;
        private ToolStripMenuItem MenuOptionsPeer2Peer;
        private ToolStripMenuItem MenuOptionsItemServerSession;
    }

}