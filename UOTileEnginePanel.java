import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

public class UOTileEnginePanel extends JPanel implements Runnable//, KeyListener
{
	private Graphics dbg;
	private Image dbImage;
	private Thread thread;

	private boolean run = true;
	private boolean tilesLoaded = false;

	private UOTileEngineTile tiles[][] = new UOTileEngineTile[80][50]; //[18][13]; //[10][10];

	private final int XMAX = 6144;
	private final int YMAX = 4096;
	private int xLoc = 0; //1440;
	private int yLoc = 0; //1640;

	public final int NONE = 0;
	public final int LEFT = 1;
	public final int RIGHT = 2;
	public final int UP = 3;
	public final int DOWN = 4;
	private int direction = NONE;

	private int blocksWide = 30;
	private int blocksTall = 30;

	private int x, y, width, height;

	public UOTileEnginePanel(int x, int y, int w, int h)
	{
		this.x = x;
		this.y = y;
		width = w;
		height = h;

		setLocation(x, y);
		setSize(w, h);
		setBackground(Color.BLACK);

		UOTileEngineMap.loadMap("", blocksWide, blocksTall);
		UOTileEngineMap.loadBlocks(xLoc, yLoc);
		loadTiles();

		//setFocusable(true);
		//addKeyListener(this);

		thread = new Thread(this, "Tile Engine");
		thread.start();
	}

	public void run()
	{
		while (run)
		{
			if (move())
			{
				moveTiles();
				repaint();
			}
			try
			{
				thread.sleep(20);
			}
			catch (Exception e)
			{
			}
		}
	}

	public void paintComponent(Graphics g)
	{
		//super.paintComponent(g);
		if (dbImage == null)
		{
			dbImage = createImage(width, height);
			dbg = dbImage.getGraphics();
		}
		dbg.setColor(getBackground());
		dbg.fillRect(x, y, width, height);
		if (tilesLoaded)
		{
			drawTiles(dbg);
			//drawCenter(dbg);
		}

		g.drawImage(dbImage, x, y, this);
	}

	public void loadTiles()
	{
		System.out.println("Loading Tiles...");
		for (int i = 0; i < tiles.length; i++)
		{
			for (int j = 0; j < tiles[i].length; j++)
			{
				tiles[i][j] = new UOTileEngineTile(i+xLoc, j+yLoc);
			}
		}
		System.out.println("Loading Tiles DONE!");
		tilesLoaded = true;
	}

	public void moveTiles()
	{
		tilesLoaded = false;
		for (int i = 0; i < tiles.length; i++)
		{
			for (int j = 0; j < tiles[i].length; j++)
			{
				if (xLoc <= 0) xLoc = XMAX - tiles.length;
				if (xLoc >= XMAX - tiles.length) xLoc = 0;
				if (yLoc <= 0) yLoc = YMAX - tiles[0].length;
				if (yLoc >= YMAX - tiles[0].length) yLoc = 0;
				tiles[i][j].setCoords(i+xLoc, j+yLoc);
			}
		}
		tilesLoaded = true;
	}

	public void drawTiles(Graphics dbg)
	{
		int x = 0;
		int y = 0;
		int width = tiles[0][0].getWidth();
		int height = tiles[0][0].getHeight();
		for (int i = 0; i < tiles.length; i++)
		{
			for (int j = 0; j < tiles[i].length; j++)
			{
				if ((i == (tiles.length / 2)) && (j == tiles[i].length / 2)) tiles[i][j].setColor(Color.WHITE);
				tiles[i][j].draw(dbg, x, y, this);
				y += height;
			}
			y = 0;
			x += width;
		}
	}

	/*public void keyTyped(KeyEvent e) {}
	public void keyPressed(KeyEvent e)
	{
		if (e.getKeyCode() == KeyEvent.VK_LEFT) direction = LEFT;
		if (e.getKeyCode() == KeyEvent.VK_RIGHT) direction = RIGHT;
		if (e.getKeyCode() == KeyEvent.VK_DOWN) direction = DOWN;
		if (e.getKeyCode() == KeyEvent.VK_UP) direction = UP;
	}

	public void keyReleased(KeyEvent e)
	{
		direction = NONE;
	}*/

	public void setDirection(int i)
	{
		direction = i;
	}

	private boolean move()
	{
		if (direction == LEFT) xLoc--;//-=5;
		if (direction == RIGHT) xLoc++;//+=5;
		if (direction == UP) yLoc--;//-=5;
		if (direction == DOWN) yLoc++;//+=5;

		if (direction == NONE) return false;
		else return true;
	}

	public void setXYLoc(int x, int y)
	{
		xLoc = x;
		yLoc = y;
		moveTiles();
		repaint();
	}

	public void setXYCenter(int x, int y)
	{
		x -= (tiles.length / 2);
		y -= (tiles[0].length / 2);
		setXYLoc(x, y);
	}
}
