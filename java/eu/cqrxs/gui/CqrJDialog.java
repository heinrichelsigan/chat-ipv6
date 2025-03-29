/*
	CqrJDialog
	https://heinrichelsigan.area23.at
*/
package eu.cqrxs.gui;

import eu.cqrxs.gui.CqrJDialog;
import eu.cqrxs.gui.*;

import java.awt.*;
import java.awt.event.*;
import java.awt.FlowLayout;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.lang.*;
import java.net.*;
import java.net.http.*;
import java.time.Duration;
import javax.imageio.ImageIO;
import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.*;

public class CqrJDialog extends JDialog {
    		
	JPanel jPanelCenter = new JPanel();
	JLabel jLabel;
	JButton jButtonExit = new JButton();
	File file;
	BufferedImage img;
	ImageIcon icon;
	
	public CqrJDialog() throws IOException {
		
		String filename = "cqrxs-eu.jpg";
		file = new File(filename);
		img = ImageIO.read(file);
		
		setModal(true);		
        setResizable(false);		
        setTitle("About CqrJd: cqrxs.eu");
		
        Init();
	}
	
	public CqrJDialog(String filename) throws IOException {
		
		if (filename == null || filename.length() == 0)			
			filename = "cqrxs-eu.jpg";
		file = new File(filename);
		img = ImageIO.read(file);
		
		setModal(true);		
        setResizable(false);		
        setTitle("About CqrJd: cqrxs.eu");
		
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
			
        icon = new ImageIcon(img);
		jPanelCenter.setBounds(24, 20, 752, 560);
		jPanelCenter.setLayout(new FlowLayout());
		jPanelCenter.setBackground(Color.GRAY);  		
		JLabel jLabel = new JLabel();
		jLabel.setIcon(icon);
		jPanelCenter.add(jLabel);
		getContentPane().add(jPanelCenter);
			
		jButtonExit = new JButton();
		jButtonExit.setText("Exit");
		getContentPane().add(jButtonExit);
		jButtonExit.setBounds(24, 600, 76, 48);
		jButtonExit.setActionCommand("jexit");
	
		// setVisible(true);
		try {
			setDefaultCloseOperation(JDialog.EXIT_ON_CLOSE);	
		} catch (Exception ex) { }
		
		SymAction lSymAction = new SymAction();
		jButtonExit.addActionListener(lSymAction);
	}

	
	class SymAction implements ActionListener
	{
		public void actionPerformed(ActionEvent event)
		{
			Object object = event.getSource();
					
			if (object == jButtonExit)
				appExit(event);				
		}
	}
	

	public void appExit(ActionEvent event) {
		// We don't log exit events ;)
		System.exit(0);
	}

}
