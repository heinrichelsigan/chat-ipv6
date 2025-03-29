/*
    JFrameApp - a basic extension of the JApplet class
*/

package eu.cqrxs;

import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import java.net.http.*;
import java.net.*;
import java.lang.*;
import java.time.Duration;

public class JFrameApp extends JFrame
{


	public static void main(String args[]) {
		
		jframeApp = new JFrameApp();
		jframeApp.setLayout(null);
		jframeApp.setSize(480,360);
		jframeApp.Init(jframeApp);
		jframeApp.setVisible(true);
		jframeApp.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
	}
	

	public void JFrameApp() {

	}

	
	public void AddMenus(JMenuBar jBar) {
		
		menuFile = new JMenu();
		menuFile.setText("File");
		menuFile.setActionCommand("File");
		menuFile.setFont(new Font("Dialog", Font.PLAIN, 12));
		menuFile.setMnemonic((int)'F');
		jBar.add(menuFile);
		
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

		menuFile_itemExit.setText("Exit");
		menuFile_itemExit.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_X, Event.ALT_MASK));
		menuFile_itemExit.setActionCommand("Exit");
		menuFile_itemExit.setMnemonic((int)'X');
		menuFile.add(menuFile_itemExit);
		
		
		menuView = new JMenu();
		menuView.setText("View");
		menuView.setActionCommand("View");
		menuView.setMnemonic((int)'V');
		jBar.add(menuView);
				
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
		jBar.add(menuIPAddrs);
		
		
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
		jBar.add(menuChat);
		
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
		jBar.add(menuContacts);
		
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
		jBar.add(menuHelp);
		
		menuHelp_itemAbout = new JMenuItem();
		menuHelp_itemAbout.setHorizontalTextPosition(SwingConstants.RIGHT);
		menuHelp_itemAbout.setText("About...");
		menuHelp_itemAbout.setActionCommand("About...");
		menuHelp_itemAbout.setMnemonic((int)'A');
		menuHelp.add(menuHelp_itemAbout);
		
	}
	

	public void Init(JFrame jf)
	{
		// symantec.itools.lang.Context.setApplet(this);
		
		// getRootPane().putClientProperty("defeatSystemEventQueueCheck", Boolean.TRUE);
		

		jf.setLayout(null);
		jf.setSize(800, 680);
		
		jMenuBar = new JMenuBar();
		AddMenus(jMenuBar);
		
		// jMenuBar.setBounds(0, 0, 480, 24);
		
		// jMenuBar.setSize(480,24);
		jf.setJMenuBar(jMenuBar);
		// jf.add(jMenuBar);
		// jMenuBar.move(0,  0);
		
		jComboBox.setBounds(48, 36, 640, 24);
		jf.getContentPane().add(jComboBox);
		
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
		
		jf.getContentPane().add(jPanelCenter);
		
		JButton1.setText("jbutton");
		jf.getContentPane().add(JButton1);
		JButton1.setBounds(24,600,76,48);
		JButton1.setActionCommand("jbutton");
		
		
		
		jf.setVisible(true);
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
		
		JButton1.addActionListener(lSymAction);
		//}}
	}

	//{{DECLARE_CONTROLS
	public static JFrameApp jframeApp;
	JComboBox jComboBox = new JComboBox();
	JPanel jPanelCenter = new JPanel();
	JButton JButton1 = new JButton();
	JTextArea jTextAreaSource = new JTextArea(), jTextAreaDestination = new JTextArea();
	
	JMenuBar jMenuBar = new JMenuBar();
	// JMenuBar jMenuBar = new JMenuBar();
	JMenu menuFile;
	JMenuItem menuFile_itemOpen;
	JMenuItem menuFile_itemSave;
	JMenuItem menuFile_itemExit = new JMenuItem();
	JMenu menuView;
	JMenuItem menuView_itemLeftRight;
	JMenuItem menuView_itemTopBottom;
	JMenuItem menuView_item1View;
	JMenu menuIPAddrs;
	JMenu menuIPAddrs_menuMyIps;
	JMenu menuIPAddrs_menuFriendIps;
	JMenu menuIPAddrs_menuProxies;
	JMenuItem menuIPAddrs_itemIPv6Secure;
	JMenu menuChat;
	JMenuItem menuChat_itemSend;
	JMenuItem menuChat_itemRefresh;
	JMenuItem menuChat_itemClear;
	JMenu menuContacts;
	JMenuItem menuContacts_itemMy;
	JMenuItem menuContacts_itemAdd;
	JMenuItem menuContacts_itemImport;
	JMenuItem menuContacts_itemView;
	
	JMenu menuHelp = new JMenu();
	JMenuItem menuHelp_itemAbout = new JMenuItem();
	//}}


	class SymAction implements ActionListener
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
			
			else if (object == JButton1)
				JButton1_actionPerformed(event);
			
		}
	}

	public void MakeWebRequest() {
		
		HttpClient client = HttpClient.newBuilder()         
         .connectTimeout(Duration.ofSeconds(10))
         .build(); 
		 
		String area23R = "https://cqrxs.eu/net/R.aspx";
		URI uri23 = URI.create(area23R);
		
		 //.version(HttpClient.Version.HTTP_2)
		 
		// HttpClient client = new HttpClient();
			// .uri(URI.create("https://area23.at/net/R.aspx"))					
			// .followRedirects(Redirect.NORMAL)
			// .version(Version.HTTP_1_1)			
			// .connectTimeout(Duration.ofSeconds(20))   			
			// .authenticator(Authenticator.getDefault())			
			// .build();
		
		// HttpRequest.newBuilder(new URI("https://area23.at/net/R.aspx"))
		
		HttpRequest request = HttpRequest.newBuilder()
			.uri(URI.create(area23R))	
			.GET()
			.build();
	
		HttpResponse<String> response;
   
		try {   
			response = client.send(request, HttpResponse.BodyHandlers.ofString());
					
			jTextAreaDestination.append("GET " + area23R + " status = " + response.statusCode() + "\n");
			jTextAreaDestination.append("Headers: " + response.headers().allValues("content-type"));
			jTextAreaDestination.append("Body: \n " + response.body());  
		} catch (Exception ioEx) {
			jTextAreaDestination.append("Exception: " + ioEx + "\n");		
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
	

	void JButton1_actionPerformed(ActionEvent event)
	{
		// to do: code goes here.
		 MakeWebRequest();
		try {
			jTextAreaSource.setText("hallo");
		} catch (Exception e) {
		}
	}
}
