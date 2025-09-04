/*
A basic extension of the JApplet class
*/


package eu.cqrxs.cqrframe;


import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import java.net.http.*;
import java.lang.*;


public class CqrMenuBar extends javax.swing.JMenuBar
{
	
	// JMenuBar jMenuBar = new JMenuBar();
	public JMenu menuFile;
	public JMenuItem menuFile_itemOpen;
	public JMenuItem menuFile_itemSave;
	public JMenuItem menuFile_itemExit;
	public JMenu menuView;
	public JMenuItem menuView_itemLeftRight;
	public JMenuItem menuView_itemTopBottom;
	public JMenuItem menuView_item1View;
	public JMenu menuIPAddrs;
	public JMenu menuIPAddrs_menuMyIps;
	public JMenu menuIPAddrs_menuFriendIps;
	public JMenu menuIPAddrs_menuProxies;
	public JMenuItem menuIPAddrs_itemIPv6Secure;
	public JMenu menuChat;
	public JMenuItem menuChat_itemSend;
	public JMenuItem menuChat_itemRefresh;
	public JMenuItem menuChat_itemClear;
	public JMenu menuContacts;
	public JMenuItem menuContacts_itemMy;
	public JMenuItem menuContacts_itemAdd;
	public JMenuItem menuContacts_itemImport;
	public JMenuItem menuContacts_itemView;
	
	public JMenu menuHelp;
	public JMenuItem menuHelp_itemAbout;
	

	public CqrMenuBar() {		
		super();
				
		AddMenus();		
	}


	
	public void AddMenus() {
		
		menuFile = new JMenu();
		menuFile.setText("File");
		menuFile.setActionCommand("File");
		menuFile.setFont(new Font("Dialog", Font.PLAIN, 12));
		menuFile.setMnemonic((int)'F');
		add(menuFile);
		
		menuFile_itemOpen = new JMenuItem();
		menuFile_itemOpen.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuFile_itemOpen.setText("Open...");
		menuFile_itemOpen.setActionCommand("Open...");
		menuFile_itemOpen.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_O, Event.CTRL_MASK));
		menuFile_itemOpen.setMnemonic((int)'O');
		menuFile.add(menuFile_itemOpen);
		
		menuFile_itemSave = new JMenuItem();
		menuFile_itemSave.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuFile_itemSave.setText("Save");
		menuFile_itemSave.setActionCommand("Save");
		menuFile_itemSave.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_S, Event.CTRL_MASK));
		menuFile_itemSave.setMnemonic((int)'S');
		menuFile.add(menuFile_itemSave);

		menuFile_itemExit = new JMenuItem();
		menuFile_itemExit.setText("Exit");
		menuFile_itemExit.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_X, Event.ALT_MASK));
		menuFile_itemExit.setActionCommand("Exit");
		menuFile_itemExit.setMnemonic((int)'X');
		menuFile.add(menuFile_itemExit);
		
		
		menuView = new JMenu();
		menuView.setText("View");
		menuView.setActionCommand("View");
		menuView.setMnemonic((int)'V');
		add(menuView);
						
		menuView_itemLeftRight = new JMenuItem();
		menuView_itemLeftRight.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuView_itemLeftRight.setText("Left-Right");
		menuView_itemLeftRight.setActionCommand("LeftRight");
		menuView_itemLeftRight.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_L, Event.CTRL_MASK));
		menuView_itemLeftRight.setMnemonic((int)'L');
		menuView.add(menuView_itemLeftRight);
		
		menuView_itemTopBottom = new JMenuItem();
		menuView_itemTopBottom.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuView_itemTopBottom.setText("Top-Bottom");
		menuView_itemTopBottom.setActionCommand("TopBottom");
		menuView_itemTopBottom.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_T, Event.CTRL_MASK));
		menuView_itemTopBottom.setMnemonic((int)'C');
		menuView.add(menuView_itemTopBottom);		
		
		menuView_item1View = new JMenuItem();
		menuView_item1View.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuView_item1View.setText("1-View");
		menuView_item1View.setActionCommand("1View");
		menuView_item1View.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_1, Event.CTRL_MASK));
		menuView_item1View.setMnemonic((int)'P');
		menuView.add(menuView_item1View);
		
		
		menuIPAddrs = new JMenu();
		menuIPAddrs.setText("IP Addresses");
		menuIPAddrs.setActionCommand("IPAddrs");
		menuIPAddrs.setMnemonic((int)'I');
		add(menuIPAddrs);		
		
		menuIPAddrs_menuMyIps = new JMenu();
		menuIPAddrs_menuMyIps.setText("My IP's");
		menuIPAddrs_menuMyIps.setActionCommand("MyIPs");
		menuIPAddrs_menuMyIps.setMnemonic((int)'M');
		menuIPAddrs.add(menuIPAddrs_menuMyIps);
		
		menuIPAddrs_menuFriendIps = new JMenu();
		menuIPAddrs_menuFriendIps.setText("Friend IP's");
		menuIPAddrs_menuFriendIps.setActionCommand("FriendIPs");
		menuIPAddrs_menuFriendIps.setMnemonic((int)'F');
		menuIPAddrs.add(menuIPAddrs_menuFriendIps);
		
		menuIPAddrs_menuProxies = new JMenu();
		menuIPAddrs_menuProxies.setText("Proxy IP's");
		menuIPAddrs_menuProxies.setActionCommand("ProxyIPs");
		menuIPAddrs_menuProxies.setMnemonic((int)'P');
		menuIPAddrs.add(menuIPAddrs_menuProxies);
		
		menuIPAddrs_itemIPv6Secure = new JMenuItem();
		menuIPAddrs_itemIPv6Secure.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuIPAddrs_itemIPv6Secure.setText("IPv6 Secure");
		menuIPAddrs_itemIPv6Secure.setActionCommand("IPv6secure");		
		menuIPAddrs_itemIPv6Secure.setMnemonic((int)'6');
		menuIPAddrs.add(menuIPAddrs_itemIPv6Secure);
		
		
		menuChat = new JMenu();
		menuChat.setText("Chat");
		menuChat.setActionCommand("Chat");
		menuChat.setMnemonic((int)'H');
		add(menuChat);
		
		menuChat_itemSend = new JMenuItem();
		menuChat_itemSend.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuChat_itemSend.setText("Send");
		menuChat_itemSend.setActionCommand("Send");
		menuChat_itemSend.setMnemonic((int)'S');
		menuChat.add(menuChat_itemSend);
		
		menuChat_itemRefresh = new JMenuItem();
		menuChat_itemRefresh.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuChat_itemRefresh.setText("Re-Fresh");
		menuChat_itemRefresh.setActionCommand("ReFresh");
		menuChat_itemRefresh.setMnemonic((int)'F');
		menuChat.add(menuChat_itemRefresh);
		
		menuChat_itemClear = new JMenuItem();
		menuChat_itemClear.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuChat_itemClear.setText("Clear");
		menuChat_itemClear.setActionCommand("Clear");
		menuChat_itemClear.setMnemonic((int)'C');
		menuChat.add(menuChat_itemClear);
		
		
		menuContacts = new JMenu();
		menuContacts.setText("Contacts");
		menuContacts.setActionCommand("Contacts");
		menuContacts.setMnemonic((int)'C');
		add(menuContacts);
		
		menuContacts_itemMy = new JMenuItem();
		menuContacts_itemMy.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuContacts_itemMy.setText("My Contact");
		menuContacts_itemMy.setActionCommand("My Contact");
		menuContacts_itemMy.setMnemonic((int)'M');
		menuContacts.add(menuContacts_itemMy);
		
		menuContacts_itemAdd = new JMenuItem();
		menuContacts_itemAdd.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuContacts_itemAdd.setText("Add Contact");
		menuContacts_itemAdd.setActionCommand("AddContact");
		menuContacts_itemAdd.setMnemonic((int)'A');
		menuContacts.add(menuContacts_itemAdd);
				
		menuContacts_itemImport = new JMenuItem();
		menuContacts_itemImport.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuContacts_itemImport.setText("Import Contacts");
		menuContacts_itemImport.setActionCommand("ImportContacts");
		menuContacts_itemImport.setMnemonic((int)'I');
		menuContacts.add(menuContacts_itemImport);		
		
		menuContacts_itemView = new JMenuItem();
		menuContacts_itemView.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuContacts_itemView.setText("View Contacts");
		menuContacts_itemView.setActionCommand("ViewContacts");
		menuContacts_itemView.setMnemonic((int)'V');
		menuContacts.add(menuContacts_itemView);
		
		
		menuHelp = new JMenu();
		menuHelp.setText("Help");
		menuHelp.setActionCommand("Help");
		menuHelp.setMnemonic((int)'H');		
		add(menuHelp);
		
		menuHelp_itemAbout = new JMenuItem();
		menuHelp_itemAbout.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuHelp_itemAbout.setText("About...");
		menuHelp_itemAbout.setActionCommand("About...");
		menuHelp_itemAbout.setMnemonic((int)'A');
		menuHelp.add(menuHelp_itemAbout);
		
	}
	

	
}
