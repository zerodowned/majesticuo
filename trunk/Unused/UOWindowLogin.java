/*
 * UOWindowLogin class Created by Mikel Duke
 * http://mikelduke.sf.net
 *
 * Window used for logging into a server.
 */

import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import javax.swing.event.*;

public class UOWindowLogin extends JFrame {
    JLabel serverLabel;
    JTextField serverText;
    JLabel portLabel;
    JTextField portText;
    JLabel userLabel;
    JTextField userText;
    JLabel passLabel;
    JTextField passText;
    JCheckBox lockCheck;
    JList serverList;
    JScrollPane sp_serverList;
    JList charList;
    JScrollPane sp_charList;
    JButton loginButton;
    DefaultListModel listModel_serverList = new DefaultListModel();
    DefaultListModel listModel_charList = new DefaultListModel();
    JButton serverButton;
    JButton charButton;

    public UOWindowLogin(ActionListener handler) {
        UOWindowLoginLayout customLayout = new UOWindowLoginLayout();

        getContentPane().setFont(new Font("Helvetica", Font.PLAIN, 12));
        getContentPane().setLayout(customLayout);

        serverLabel = new JLabel("Server:");
        getContentPane().add(serverLabel);

        serverText = new JTextField("login.uorebirth.com");
        getContentPane().add(serverText);

        portLabel = new JLabel("Port:");
        getContentPane().add(portLabel);

        portText = new JTextField("2593");
        getContentPane().add(portText);

        userLabel = new JLabel("User:");
        getContentPane().add(userLabel);

        userText = new JTextField("");
        getContentPane().add(userText);

        passLabel = new JLabel("Password:");
        getContentPane().add(passLabel);

        passText = new JTextField("");
        getContentPane().add(passText);

        lockCheck = new JCheckBox("Does the sever send a locked client features packet?");
        getContentPane().add(lockCheck);

        serverList = new JList(listModel_serverList);
        sp_serverList = new JScrollPane(serverList);
        getContentPane().add(sp_serverList);

        charList = new JList(listModel_charList);
        sp_charList = new JScrollPane(charList);
        getContentPane().add(sp_charList);

        loginButton = new JButton("Login");
        getContentPane().add(loginButton);

        serverButton = new JButton("Server");
        getContentPane().add(serverButton);

        charButton = new JButton("Char");
        getContentPane().add(charButton);

        loginButton.addActionListener(handler);
        serverButton.addActionListener(handler);
        charButton.addActionListener(handler);

        setSize(getPreferredSize());

        addWindowListener(new WindowAdapter() {
            public void windowClosing(WindowEvent e) {
                System.exit(0);
            }
        });
    }

   public String getIP()
   {
		 return serverText.getText();
	 }

	 public int getPort()
	 {
		 return Integer.parseInt(portText.getText());
	 }

	 public String getUser()
	 {
		 return userText.getText();
	 }

	 public String getPass()
	 {
		 return passText.getText();
	 }
}

class UOWindowLoginLayout implements LayoutManager {

    public UOWindowLoginLayout() {
    }

    public void addLayoutComponent(String name, Component comp) {
    }

    public void removeLayoutComponent(Component comp) {
    }

    public Dimension preferredLayoutSize(Container parent) {
        Dimension dim = new Dimension(0, 0);

        Insets insets = parent.getInsets();
        dim.width = 298 + insets.left + insets.right;
        dim.height = 213 + insets.top + insets.bottom;

        return dim;
    }

    public Dimension minimumLayoutSize(Container parent) {
        Dimension dim = new Dimension(0, 0);
        return dim;
    }

    public void layoutContainer(Container parent) {
        Insets insets = parent.getInsets();

        Component c;
        c = parent.getComponent(0);
        if (c.isVisible()) {c.setBounds(insets.left+8,insets.top+8,48,24);}
        c = parent.getComponent(1);
        if (c.isVisible()) {c.setBounds(insets.left+56,insets.top+8,88,24);}
        c = parent.getComponent(2);
        if (c.isVisible()) {c.setBounds(insets.left+8,insets.top+32,48,24);}
        c = parent.getComponent(3);
        if (c.isVisible()) {c.setBounds(insets.left+56,insets.top+32,88,24);}
        c = parent.getComponent(4);
        if (c.isVisible()) {c.setBounds(insets.left+152,insets.top+8,72,24);}
        c = parent.getComponent(5);
        if (c.isVisible()) {c.setBounds(insets.left+224,insets.top+8,72,24);}
        c = parent.getComponent(6);
        if (c.isVisible()) {c.setBounds(insets.left+152,insets.top+32,72,24);}
        c = parent.getComponent(7);
        if (c.isVisible()) {c.setBounds(insets.left+224,insets.top+32,72,24);}
        c = parent.getComponent(8);
        //if (c.isVisible()) {c.setBounds(insets.left+8,insets.top+56,288,24);}
        c = parent.getComponent(9);
        if (c.isVisible()) {c.setBounds(insets.left+8,insets.top+120,136,88);}
        c = parent.getComponent(10);
        if (c.isVisible()) {c.setBounds(insets.left+152,insets.top+120,144,88);}
        c = parent.getComponent(11);
        if (c.isVisible()) {c.setBounds(insets.left+80,insets.top+88,72,24);}
        c = parent.getComponent(12);
        if (c.isVisible()) {c.setBounds(insets.left+152,insets.top+88,72,24);}
        c = parent.getComponent(13);
        if (c.isVisible()) {c.setBounds(insets.left+224,insets.top+88,72,24);}
    }
}
