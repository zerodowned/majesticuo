/*
 * UOWindowGame class Created by Mikel Duke
 * http://mikelduke.sf.net
 *
 * Window where the game is displayed.
 */

import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

public class UOWindowGame extends JFrame {
    JTextArea sysArea;
    JScrollPane sp_sysArea;
    JTextField chatText;
    JButton nButton;
    JButton wButton;
    JButton eButton;
    JButton sButton;
    JButton nwButton;
    JButton swButton;
    JButton seButton;
    JButton neButton;
    JButton hideButton;
    JButton guardsButton;
    JButton sayButton;
    UOCoordViewer mapPanel;
    JButton updateButton;

    public UOWindowGame(ActionListener handler) {
        UOWindowGameLayout customLayout = new UOWindowGameLayout();

        getContentPane().setFont(new Font("Helvetica", Font.PLAIN, 12));
        getContentPane().setLayout(customLayout);

        sysArea = new JTextArea("");
        sp_sysArea = new JScrollPane(sysArea);
        getContentPane().add(sp_sysArea);

        chatText = new JTextField("");
        getContentPane().add(chatText);

        nButton = new JButton("n");
        getContentPane().add(nButton);

        wButton = new JButton("w");
        getContentPane().add(wButton);

        eButton = new JButton("e");
        getContentPane().add(eButton);

        sButton = new JButton("s");
        getContentPane().add(sButton);

        nwButton = new JButton("nv");
        getContentPane().add(nwButton);

        swButton = new JButton("sv");
        getContentPane().add(swButton);

        seButton = new JButton("se");
        getContentPane().add(seButton);

        neButton = new JButton("ne");
        getContentPane().add(neButton);

        hideButton = new JButton("Hide");
        getContentPane().add(hideButton);

        guardsButton = new JButton("Guards");
        getContentPane().add(guardsButton);

        sayButton = new JButton("Say");
        getContentPane().add(sayButton);

        mapPanel = new UOCoordViewer("MAP0-1.gif", 0, 0, 144, 128);
        getContentPane().add(mapPanel);

        updateButton = new JButton("Redraw");
        getContentPane().add(updateButton);

        sayButton.addActionListener(handler);
        nButton.addActionListener(handler);
        wButton.addActionListener(handler);
        eButton.addActionListener(handler);
        sButton.addActionListener(handler);
        nwButton.addActionListener(handler);
        swButton.addActionListener(handler);
        seButton.addActionListener(handler);
        neButton.addActionListener(handler);
        hideButton.addActionListener(handler);
        guardsButton.addActionListener(handler);
        updateButton.addActionListener(handler);

        setSize(getPreferredSize());

        addWindowListener(new WindowAdapter() {
            public void windowClosing(WindowEvent e) {
                System.exit(0);
            }
        });
    }

    protected void setCharLocation(int x, int y)
    {
			mapPanel.setMapCoords(x - (144 / 2), y - (128 / 2));
		}
}

class UOWindowGameLayout implements LayoutManager {

    public UOWindowGameLayout() {
    }

    public void addLayoutComponent(String name, Component comp) {
    }

    public void removeLayoutComponent(Component comp) {
    }

    public Dimension preferredLayoutSize(Container parent) {
        Dimension dim = new Dimension(0, 0);

        Insets insets = parent.getInsets();
        dim.width = 579 + insets.left + insets.right;
        dim.height = 367 + insets.top + insets.bottom;

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
        if (c.isVisible()) {c.setBounds(insets.left+8,insets.top+8,416,320);}
        c = parent.getComponent(1);
        if (c.isVisible()) {c.setBounds(insets.left+8,insets.top+336,416,24);}
        c = parent.getComponent(2);
        if (c.isVisible()) {c.setBounds(insets.left+528,insets.top+8,48,24);}
        c = parent.getComponent(3);
        if (c.isVisible()) {c.setBounds(insets.left+432,insets.top+8,48,24);}
        c = parent.getComponent(4);
        if (c.isVisible()) {c.setBounds(insets.left+528,insets.top+56,48,24);}
        c = parent.getComponent(5);
        if (c.isVisible()) {c.setBounds(insets.left+432,insets.top+56,48,24);}
        c = parent.getComponent(6);
        if (c.isVisible()) {c.setBounds(insets.left+480,insets.top+8,48,24);}
        c = parent.getComponent(7);
        if (c.isVisible()) {c.setBounds(insets.left+432,insets.top+32,48,24);}
        c = parent.getComponent(8);
        if (c.isVisible()) {c.setBounds(insets.left+480,insets.top+56,48,24);}
        c = parent.getComponent(9);
        if (c.isVisible()) {c.setBounds(insets.left+528,insets.top+32,48,24);}
        c = parent.getComponent(10);
        if (c.isVisible()) {c.setBounds(insets.left+432,insets.top+88,72,24);}
        c = parent.getComponent(11);
        if (c.isVisible()) {c.setBounds(insets.left+496,insets.top+336,80,24);}
        c = parent.getComponent(12);
        if (c.isVisible()) {c.setBounds(insets.left+432,insets.top+336,64,24);}
        c = parent.getComponent(13);
        if (c.isVisible()) {c.setBounds(insets.left+432,insets.top+120,144,128);}
        c = parent.getComponent(14);
        if (c.isVisible()) {c.setBounds(insets.left+504,insets.top+88,72,24);}
    }
}
