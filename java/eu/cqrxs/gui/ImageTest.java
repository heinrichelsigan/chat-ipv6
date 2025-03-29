/*
	ImageTest 
	https://heinrichelsigan.area23.at
*/
package eu.cqrxs.gui;

import eu.cqrxs.gui.*;
import eu.cqrxs.gui.ImageTest;

import java.awt.FlowLayout;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import javax.imageio.ImageIO;
import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JLabel;


public class ImageTest {

    public static void main(String avg[]) throws IOException
    {
      ImageTest imgTest; 
      if (avg == null || avg.length == 0) {
        imgTest = new ImageTest("cqrxs-eu.jpg");
        return;
      }

      for (int i = 0; i < avg.length; i++) 
          imgTest = new ImageTest(avg[i]);
    }

    public ImageTest(String fileName) throws IOException
    {
        File file = new File(fileName);
        BufferedImage img=ImageIO.read(file);
        ImageIcon icon=new ImageIcon(img);
        JFrame frame=new JFrame();
        frame.setLayout(new FlowLayout());
        frame.setSize(640,480);
        JLabel lbl=new JLabel();
        lbl.setIcon(icon);
        frame.add(lbl);
        frame.setVisible(true);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
    }
}
