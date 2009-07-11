import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

public class UOGraphicalGameWindow extends JFrame //implements KeyListener
{
	int width = 800;
	int height = 600;

	public UOGraphicalGameWindow(KeyListener handler, UOTileEnginePanel game, UOChatPanel chat)//, int x, int y)
	{
		getContentPane().setFont(new Font("Helvetica", Font.PLAIN, 12));

    //game = new UOTileEnginePanel(0,0,800,500);
    game.setBounds(0,0,800,500);
    //game.setXYLoc(x, y);
    getContentPane().add(game);

    //chat = new UOChatPanel(0,500,800,100);
    chat.setBounds(0,500,800,100);
    getContentPane().add(chat);

    setFocusable(true);
		addKeyListener(handler);

    setSize(800,600+34);
		setTitle("UO Graphical Client");
		//show();

    addWindowListener(new WindowAdapter() {
				public void windowClosing(WindowEvent e) {
						System.exit(0);
				}
		});
	}
}
