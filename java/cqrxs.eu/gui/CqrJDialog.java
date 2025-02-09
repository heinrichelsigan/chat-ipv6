/*
	CqrJDialog
*/
import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import java.net.http.*;
import java.net.*;
import java.lang.*;
import java.time.Duration;

public class CqrJDialog extends JDialog {
    
	public CqrJDialog(String title) {
        setModal(true);
        setResizable(false);
        setTitle(title);

        Init();
    }

    public int showDialog(Window parent) {
        setLocationRelativeTo(parent);
        setVisible(true);
        return 0;
    }
	
	public void showDialog(JFrame parentJFrame) {
        setLocationRelativeTo(parentJFrame);
        setVisible(true);
        return ;
    }
	
	
	public void Init() {
		setLayout(null);
		setSize(800, 680);	
		jComboBox.setBounds(48, 36, 640, 24);
		getContentPane().add(jComboBox);
		
		jPanelCenter.setBounds(48, 72, 640, 400);
		jPanelCenter.setLayout(new GridLayout(1, 2));
		jPanelCenter.setBackground(Color.BLACK);  
		jPanelCenter.add(jTextAreaSource);
		jTextAreaSource.setBounds(1,1,632,196);
		jTextAreaSource.setBackground(Color.GRAY);  
		jTextAreaSource.append("\t\r\n");		
		jPanelCenter.add(jTextAreaDestination);
		jTextAreaDestination.setBounds(1,240,632,196);
		jTextAreaDestination.setBackground(Color.YELLOW);  
		
		getContentPane().add(jPanelCenter);
		
		JButton1.setText("jbutton");
		getContentPane().add(JButton1);
		JButton1.setBounds(24,600,76,48);
		JButton1.setActionCommand("jbutton");
		
		setVisible(true);
		
		SymAction lSymAction = new SymAction();
		JButton1.addActionListener(lSymAction);
	}
	
	JPanel jPanelCenter = new JPanel();
	JComboBox jComboBox = new JComboBox();
	JButton JButton1 = new JButton();
	JTextArea jTextAreaSource = new JTextArea(), jTextAreaDestination = new JTextArea();
	
	
	class SymAction implements ActionListener
	{
		public void actionPerformed(ActionEvent event)
		{
			Object object = event.getSource();
					
			if (object == JButton1)
				JButton1_actionPerformed(event);
			else
				appExit(event);			
			
		}
	}
	
	void JButton1_actionPerformed(ActionEvent event)
	{
		// to do: code goes here.
		// MakeWebRequest();
		try {
			jTextAreaSource.setText("hallo");
		} catch (Exception e) {
		}
	}

	public void appExit(ActionEvent event) {
		// We don't log exit events ;)
		System.exit(0);
	}

}