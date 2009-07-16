import javax.swing.*;
import java.awt.*;
import java.awt.image.*;
import java.io.*;

public class UOTileEngineTile
{
	private int width = 10; //44;
	private int height = 10; //44;
	private int x;
	private int y;
	private Color color;

	public UOTileEngineTile(int x, int y)
	{
		this.x = x;
		this.y = y;
		loadTileInfo();
	}

	public int getWidth()
	{
		return width;
	}

	public int getHeight()
	{
		return height;
	}

	public int getX()
	{
		return x;
	}

	public int getY()
	{
		return y;
	}

	public void setCoords(int x, int y)
	{
		this.x = x;
		this.y = y;
		loadTileInfo();
	}

	private void loadTileInfo()
	{
		int color16 = (UOTileEngineMap.getColor(x, y));
		int red = (color16 & 0x7C00) >>> 10;
		int green = (color16 & 0x3E0) >>> 5;
		int blue = (color16 & 0x1F);
		double conv = 255.0 / 31;
		color = new Color((int)(red * conv), (int)(green * conv), (int)(blue * conv));
	}

	public void draw(Graphics g, int gX, int gY, ImageObserver io)
	{
		g.setColor(color);
		g.fillRect(gX, gY, width, height);
	}

	public void setColor(Color c)
	{
		color = c;
	}
}