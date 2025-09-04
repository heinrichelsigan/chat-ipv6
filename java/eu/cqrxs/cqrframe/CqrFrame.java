/*
A basic extension of the JApplet class
*/

package eu.cqrxs.cqrframe;

import eu.cqrxs.cqrframe.CqrFrame;

import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import javax.swing.plaf.metal.*;
import javax.swing.plaf.synth.*;

public class CqrFrame extends JFrame
{

	//{{DECLARE_CONTROLS
	public static CqrFrame jFrameApp;
	JComboBox jComboBox;
	JPanel jPanelCenter;
	JButton jButton;
	JTextArea jTextAreaSource, jTextAreaDestination;

	public JMenuBar jMenuBar = new JMenuBar();
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
	
	//}}

	public CqrFrame() {
		super();
		
		Init();
	}


	
	public void Init()
	{
		// symantec.itools.lang.Context.setApplet(this);
		
		// getRootPane().putClientProperty("defeatSystemEventQueueCheck", Boolean.TRUE);
		jMenuBar = new JMenuBar();
		jComboBox = new JComboBox();
		jPanelCenter = new JPanel();
		jTextAreaSource = new JTextArea();
		jTextAreaDestination = new JTextArea();
		jButton = new JButton();
		
		setLayout(null);
		setSize(800, 680);
				
		AddMenus(jMenuBar);		
		// jMenuBar.setBounds(0, 0, 480, 24);		
		// jMenuBar.setSize(480,24);
		setJMenuBar(jMenuBar);
		// jf.add(jMenuBar);
		// jMenuBar.move(0,  0);
		
		jComboBox.setBounds(48, 36, 640, 24);
		add(jComboBox);
		
		jPanelCenter.setBounds(48, 72, 640, 400);
		jPanelCenter.setLayout(new GridLayout(1, 2));
		jPanelCenter.setBackground(Color.BLACK);  
		jPanelCenter.add(jTextAreaSource);
		jTextAreaSource.setBounds(1,1,632,196);
		jTextAreaSource.setBackground(Color.GRAY);  
		jTextAreaSource.append("jMenuBar.getUI() == " + jMenuBar.getUI() + "\n");		
		jPanelCenter.add(jTextAreaDestination);
		jTextAreaDestination.setBounds(1,240,632,196);
		jTextAreaDestination.setBackground(Color.YELLOW);  
		
		
		add(jPanelCenter);
		
		jButton.setText("jbutton");
		add(jButton);
		jButton.setBounds(24,600,76,48);
		jButton.setActionCommand("jbutton");
		
				
		setVisible(true);
		//}}
	
		//{{REGISTER_LISTENERS
		SymAction lSymAction = new SymAction();
		
		menuFile_itemExit.addActionListener(lSymAction);
		
		menuView_itemLeftRight.addActionListener(lSymAction);
		menuView_itemTopBottom.addActionListener(lSymAction);
		menuView_item1View.addActionListener(lSymAction);
		
		menuChat_itemSend.addActionListener(lSymAction);
		menuChat_itemRefresh.addActionListener(lSymAction);
		menuChat_itemClear.addActionListener(lSymAction);
		
		menuContacts_itemMy.addActionListener(lSymAction);
		menuContacts_itemAdd.addActionListener(lSymAction);
		menuContacts_itemImport.addActionListener(lSymAction);
		menuContacts_itemView.addActionListener(lSymAction);
		
		menuHelp_itemAbout.addActionListener(lSymAction);
		
		jButton.addActionListener(lSymAction);
		//}}
	}


	public void AddMenus(javax.swing.JMenuBar jMenuBar) {
			
		menuFile = new JMenu();
		menuFile.setText("File");
		menuFile.setActionCommand("File");
		menuFile.setFont(new Font("Dialog", Font.PLAIN, 12));
		menuFile.setMnemonic((int)'F');
		jMenuBar.add(menuFile);
		
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
		jMenuBar.add(menuView);
						
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
		jMenuBar.add(menuIPAddrs);		
		
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
		jMenuBar.add(menuChat);
		
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
		jMenuBar.add(menuContacts);
		
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
		jMenuBar.add(menuHelp);
		
		menuHelp_itemAbout = new JMenuItem();
		menuHelp_itemAbout.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuHelp_itemAbout.setText("About...");
		menuHelp_itemAbout.setActionCommand("About...");
		menuHelp_itemAbout.setMnemonic((int)'A');
		menuHelp.add(menuHelp_itemAbout);
		
	}



	public static void main(String args[]) {
		
		jFrameApp = new CqrFrame();
		jFrameApp.setLayout(null);
		jFrameApp.setSize(480,360);
		jFrameApp.Init();
		jFrameApp.setVisible(true);
		jFrameApp.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
	}
	

	public class SymAction implements ActionListener
	{
		public void actionPerformed(ActionEvent event)
		{
			Object object = event.getSource();

			if (object == menuFile_itemExit)
				appExit(event);
			
			else if (object == menuView_itemLeftRight)
				viewChange(event, "LeftRight");
			else if (object == menuView_itemTopBottom)
				viewChange(event, "TopBottom");
			else if (object == menuView_item1View)
				viewChange(event, "1View");
			
			else if (object == menuChat_itemSend) 
				chatCommand(event, "Send");
			else if (object == menuChat_itemRefresh) 
				chatCommand(event, "Refresh");
			else if (object == menuChat_itemClear) 
				chatCommand(event, "Clear");
			
			else if (object == menuContacts_itemMy) 
				addEditContact(event, 0);
			else if (object == menuContacts_itemAdd) 
				addEditContact(event, 1);
			else if (object == menuContacts_itemImport) 
				addEditContact(event, -1);
			else if (object == menuContacts_itemView) 
				viewContact(event);
			
			else if (object == menuHelp_itemAbout)
				about(event);
			
			else if (object == jButton)
				jButton_actionPerformed(event);
			
		}
	}

	public void appExit(ActionEvent event) {
		// We don't log exit events ;)
		System.exit(0);
	}

	public void viewChange(ActionEvent event, String whichView) {
		jTextAreaSource.append("View menu, view changed to " + whichView + ", event: " + event + "\n");
	
		if (whichView == "LeftRight") {
			
			jPanelCenter.remove(jTextAreaSource);
			jPanelCenter.remove(jTextAreaDestination);
			
			jPanelCenter.setBounds(48, 72, 640, 400);
			jPanelCenter.setLayout(new GridLayout(1, 2));
			jPanelCenter.setBackground(Color.BLACK);  
			jPanelCenter.add(jTextAreaSource);
			jTextAreaSource.setBounds(1,1,632,236);
			jTextAreaSource.setBackground(Color.GRAY);  
			jTextAreaSource.append("jMenuBar.getUI() == "  + jMenuBar.getUI() + "\n");		
			jPanelCenter.add(jTextAreaDestination);
			jTextAreaDestination.setBounds(1,240,632,236);
			jTextAreaDestination.setBackground(Color.YELLOW);  
		}
		else if (whichView == "TopBottom") {
			jPanelCenter.remove(jTextAreaSource);
			jPanelCenter.remove(jTextAreaDestination);
			
			jPanelCenter.setBounds(48, 72, 640, 400);
			jPanelCenter.setLayout(new GridLayout(2, 1));
			jPanelCenter.setBackground(Color.BLACK);  
			jPanelCenter.add(jTextAreaSource);
			jTextAreaSource.setBounds(1,1,632,236);
			jTextAreaSource.setBackground(Color.GRAY);  
			jTextAreaSource.append("jMenuBar.getUI() == " + jMenuBar.getUI() +  "\n");		
			jPanelCenter.add(jTextAreaDestination);
			jTextAreaDestination.setBounds(1,240,632,236);
			jTextAreaDestination.setBackground(Color.YELLOW);
		} else {
			jPanelCenter.remove(jTextAreaSource);
			jPanelCenter.remove(jTextAreaDestination);
			
			jPanelCenter.setBounds(48, 72, 640, 400);
			jPanelCenter.setLayout(new GridLayout(1, 1));
			jPanelCenter.setBackground(Color.BLACK);  
			jPanelCenter.add(jTextAreaSource);
			jTextAreaSource.setBounds(1,1,632,236);
			jTextAreaSource.setBackground(Color.GRAY);  
			jTextAreaSource.append("jMenuBar.getUI() == "  + jMenuBar.getUI() + "\n");		
			// jPanelCenter.add(jTextAreaDestination);
			// jTextAreaDestination.setBounds(1,240,632,236);
			// jTextAreaDestination.setBackground(Color.YELLOW)	
		}
	}
	
	
	
	public void chatCommand(ActionEvent event, String whichCommand) { 
		jTextAreaSource.append("Menu Chat = command " + whichCommand + ", event: " + event + "\n");
		
		
		
	}
	
	
	public void addEditContact(ActionEvent event, int who) {
		if (who == 0)
			jTextAreaSource.append("Menu Contact => edit \"My Contact\", event: " + event + "\n");
		else if (who > 0)
			jTextAreaSource.append("Menu Contact => add/edit contacts, event: " + event + "\n");
		else if (who < 0)
			jTextAreaSource.append("Menu Contact => import contacts, event: " + event + "\n");
	}
	
	public void viewContact(ActionEvent event) {
	
		jTextAreaSource.append("Menu Contact => view contacts, event: " + event + "\n");
	}


	public void about(ActionEvent event) {
	
		jTextAreaSource.append("About menu clicked, event: " + event + "\n");
	}
	

	public void jButton_actionPerformed(ActionEvent event)
	{
		try {
			jTextAreaSource.setText("hallo");
		} catch (Exception e) {
		}
	}
}
