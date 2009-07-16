import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

public class UOChatPanel extends JPanel implements Runnable
{
	private String currentText = new String();
	private Color backColor = Color.BLACK;
	private Color textColor = Color.WHITE;

	private Graphics dbg;
	private Image dbImage;

	private boolean run = true;
	private Thread thread;

	private int x, y, width, height;
	private int maxLines;

	private Font font;

	public UOChatPanel(int x, int y, int w, int h)
	{
		this.x = x;
		this.y = y;
		width = w;
		height = h;

		setLocation(x, y);
		setSize(w, h);

		setBackground(backColor);
		setForeground(textColor);

		font = new Font("Helvetica", Font.PLAIN, 12);
		setFont(font);
		maxLines = height / font.getSize();

		thread = new Thread(this, "Tile Engine");
		thread.start();
	}

	public void run()
	{
		while (run)
		{
			repaint();
			try
			{
				thread.sleep(10);
			}
			catch (Exception e)
			{
			}
		}
	}

	public void paintComponent(Graphics g)
	{
		g.setColor(backColor);
		g.fillRect(x, y, width, height);
		drawText(g);
	}

	private void drawText(Graphics g)
	{
		String sAr[] = currentText.split("\n", -1);
		g.setColor(textColor);

		int s;
		//if (sAr.length - maxLines <= 0) s = 0;
		//else s = sAr.length - maxLines;
		s = sAr.length - 8; //maxLines;

		for (int i = 0; i < maxLines; i++)
		{
			if ((s + i >= 0) && (s + i < sAr.length))
				g.drawString(sAr[s + i], x, y + ((i + 1) * font.getSize()));
		}
	}

	public void setChatText(String text)
	{
		currentText = text;
	}

	public void addLine(String text)
	{
		currentText = currentText + text + '\n';
	}

	public void addChar(char c)
	{
		currentText = currentText + c;
	}
}