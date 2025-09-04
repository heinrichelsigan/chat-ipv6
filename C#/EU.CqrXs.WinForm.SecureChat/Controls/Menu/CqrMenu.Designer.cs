using System.Drawing.Imaging;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Menu
{
    partial class CqrMenu
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
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
            toolStripSeparator1 = new ToolStripSeparator();
            MenuOptionsItemPeer2Peer = new ToolStripMenuItem();
            MenuOptionsItemServerSession = new ToolStripMenuItem();
            MenuHelp = new ToolStripMenuItem();
            MenuHelpItemViewHelp = new ToolStripMenuItem();
            MenuHelpItemInfo = new ToolStripMenuItem();
            MenuHelpItemAbout = new ToolStripMenuItem();
            SuspendLayout();            
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

            // 
            // MenuViewItemTopBottom
            // 
            MenuViewItemTopBottom.BackColor = SystemColors.MenuBar;
            MenuViewItemTopBottom.Name = "MenuViewItemTopBottom";
            MenuViewItemTopBottom.ShortcutKeys = Keys.Alt | Keys.T;
            MenuViewItemTopBottom.Size = new Size(204, 22);
            MenuViewItemTopBottom.Text = "top-bottom";
            MenuViewItemTopBottom.ToolTipText = "top-bottom shows a top->bottom splitted chat view";
            // 
            // MenuViewItem1View
            // 
            MenuViewItem1View.BackColor = SystemColors.MenuBar;
            MenuViewItem1View.Name = "MenuViewItem1View";
            MenuViewItem1View.ShortcutKeys = Keys.Alt | Keys.D1;
            MenuViewItem1View.Size = new Size(204, 22);
            MenuViewItem1View.Text = "1-view";
            MenuViewItem1View.ToolTipText = "displays a single box chat view";
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
            MenuCommands.Size = new Size(128, 21);
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
            // 
            // MenuCommandsItemAttach
            // 
            MenuCommandsItemAttach.BackColor = SystemColors.MenuBar;
            MenuCommandsItemAttach.Name = "MenuCommandsItemAttach";
            MenuCommandsItemAttach.ShortcutKeys = Keys.Control | Keys.A;
            MenuCommandsItemAttach.Size = new Size(180, 22);
            MenuCommandsItemAttach.Text = "attach";
            MenuCommandsItemAttach.ToolTipText = "attaches file to send; in prototype only  file extension image audio and video is supported";
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
            // 
            // MenuCommandsItemClear
            // 
            MenuCommandsItemClear.BackColor = SystemColors.MenuBar;
            MenuCommandsItemClear.Name = "MenuCommandsItemClear";
            MenuCommandsItemClear.ShortcutKeys = Keys.Control | Keys.Delete;
            MenuCommandsItemClear.Size = new Size(180, 22);
            MenuCommandsItemClear.Text = "clear";
            MenuCommandsItemClear.ToolTipText = "clears completey all chat windows";
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
            // 
            // MenuContactsItemAdd
            // 
            MenuContactsItemAdd.BackColor = SystemColors.Menu;
            MenuContactsItemAdd.Name = "MenuContactsItemAdd";
            MenuContactsItemAdd.ShortcutKeys = Keys.Alt | Keys.A;
            MenuContactsItemAdd.Size = new Size(233, 22);
            MenuContactsItemAdd.Text = "add contact";
            MenuContactsItemAdd.ToolTipText = "adds a friend contact to cqr chat";
            // 
            // MenuContactsItemView
            // 
            MenuContactsItemView.BackColor = SystemColors.Menu;
            MenuContactsItemView.Name = "MenuContactsItemView";
            MenuContactsItemView.ShortcutKeys = Keys.Alt | Keys.V;
            MenuContactsItemView.Size = new Size(233, 22);
            MenuContactsItemView.Text = "view contacts";
            MenuContactsItemView.ToolTipText = "view all added and imported contacts";
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
            MenuOptions.DropDownItems.AddRange(new ToolStripItem[] { MenuOptionsItemCompress, MenuOptionsItemFileSecure, MenuOptionsItemClearAllOnClose, MenuOptionsItemDontSendProfilePictures, toolStripSeparator1, MenuOptionsItemPeer2Peer, MenuOptionsItemServerSession });
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
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(332, 6);
            // 
            // MenuOptionsItemPeer2Peer
            // 
            MenuOptionsItemPeer2Peer.CheckOnClick = true;
            MenuOptionsItemPeer2Peer.Name = "MenuOptionsItemPeer2Peer";
            MenuOptionsItemPeer2Peer.Size = new Size(335, 22);
            MenuOptionsItemPeer2Peer.Text = "peer-2-peer mode";
            // 
            // MenuOptionsItemServerSession
            // 
            MenuOptionsItemServerSession.CheckOnClick = true;
            MenuOptionsItemServerSession.Name = "MenuOptionsItemServerSession";
            MenuOptionsItemServerSession.Size = new Size(335, 22);
            MenuOptionsItemServerSession.Text = "chat server session";
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
            // StripMenu
            //             
            this.BackgroundImageLayout = ImageLayout.None;            
            this.AllowItemReorder = true;
            this.BackColor = SystemColors.MenuBar;
            this.Font = new Font("Lucida Sans Unicode", 10F);
            this.GripStyle = ToolStripGripStyle.Visible;
            this.Items.AddRange(new ToolStripItem[] { MenuFile, MenuView, MenuNetwork, MenuCommands, MenuContacts, MenuOptions, MenuHelp });
            this.Location = new Point(0, 0);
            this.Name = "StripMenu";
            this.RenderMode = ToolStripRenderMode.System;
            this.Size = new Size(998, 25);
            this.ClientSize = new Size(998, 25);
            this.TabIndex = 1;
            this.Text = "StripMenu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


        internal ToolStripMenuItem MenuFile;
        internal ToolStripMenuItem MenuFileItemOpen;
        internal ToolStripMenuItem MenuFileItemSave;
        internal ToolStripSeparator MenuFileSeparatorExit;
        internal ToolStripMenuItem MenuFileItemExit;

        internal ToolStripMenuItem MenuView;
        internal ToolStripMenuItem MenuViewItemLeftRíght;
        internal ToolStripMenuItem MenuViewItemTopBottom;
        internal ToolStripMenuItem MenuViewItem1View;


        internal ToolStripMenuItem MenuNetwork;
        internal ToolStripMenuItem MenuNetworkItemMyIps;
        internal ToolStripMenuItem MenuItemExternalIp;
        internal ToolStripMenuItem MenuItemFriendIp;
        internal ToolStripComboBox MenuNetworkComboBoxFriendIp;
        internal ToolStripMenuItem MenuNetworkItemProxyServers;
        internal ToolStripMenuItem MenuNetworkItemIPv6Secure;
        internal ToolStripSeparator MenuNetworkSeparatorIp;

        internal ToolStripMenuItem MenuCommands;
        internal ToolStripMenuItem MenuCommandsItemSend;
        internal ToolStripMenuItem MenuCommandsItemRefresh;
        internal ToolStripMenuItem MenuCommandsItemClear;
        internal ToolStripMenuItem MenuCommandsItemAttach;
        internal ToolStripSeparator MenuCommandsSeperator;

        internal ToolStripMenuItem MenuContacts;
        internal ToolStripMenuItem MenuContactstemImport;
        internal ToolStripMenuItem MenuContactsItemAdd;
        internal ToolStripMenuItem MenuContactsItemView;
        internal ToolStripMenuItem MenuContactsItemMe;
        internal ToolStripSeparator MenuContactsSeparetor;
        internal ToolStripMenuItem MenuContactstemExport;

        internal ToolStripMenuItem MenuOptions;
        internal ToolStripMenuItem MenuOptionsItemCompress;
        internal ToolStripMenuItem MenuOptionsItemFileSecure;
        internal ToolStripMenuItem MenuOptionsItemClearAllOnClose;
        internal ToolStripMenuItem MenuOptionsItemDontSendProfilePictures;
        internal ToolStripSeparator toolStripSeparator1;
        internal ToolStripMenuItem MenuOptionsItemPeer2Peer;
        internal ToolStripMenuItem MenuOptionsItemServerSession;

        internal ToolStripMenuItem MenuHelp;
        internal ToolStripMenuItem MenuHelpItemViewHelp;
        internal ToolStripMenuItem MenuHelpItemInfo;
        internal ToolStripMenuItem MenuHelpItemAbout;
    }
}
