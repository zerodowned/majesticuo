/*
 * UOCoordViewer class Created by Mikel Duke
 * http://mikelduke.sf.net
 *
 * Object to be placed in a window, extends from JPanel and is used similarly.
 * Draws a portion of the Ultima Online map coming from a .gif image or any
 * other kind of image. Overrides paintComponent method to do the drawing.
 * Needed fields to instantiate are the x and y coords to start drawing from
 * in the upper left corner, file to read from, and the width and height of
 * the image to be drawn.
 */

import javax.swing.*;
import java.awt.*;
import java.awt.image.*;

public class UOCoordViewer extends JPanel
{
	private final int maxX = 6143;
	private final int maxY = 4095;
	private int x;
	private int y;
	private int w;
	private int h;
	private int[] pixels;
	private ImageIcon map;

	private String filename;

	public UOCoordViewer(String file, int x1, int y1, int w1, int h1)
	{
		x = x1;
		y = y1;
		w = w1;
		h = h1;

		filename = file;
		map = new ImageIcon(file);

		setBackground(Color.BLACK);
		readPixels();
	}

	public void paintComponent(Graphics g)
	{
		super.paintComponent(g);
		//g.clearRect(0,0,w,h);
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				g.setColor(new Color(pixels[i * w + j]));
				g.drawLine(j,i,j,i);
			}
		}
		g.setColor(Color.WHITE);
		g.drawLine((w/2),(h/2),w,h);//(w/2)+1,(h/2)+1);
		try { Thread.sleep(10); }
		catch(Exception e) {}
	}

	public void setMapBounds(int x1, int y1, int w1, int h1)
	{
		x = x1;
		y = y1;
		w = w1;
		h = h1;
		readPixels();
	}

	public void setMapCoords(int x1, int y1)
	{
		x = x1;
		y = y1;
		readPixels();
	}

	private void readPixels()
	{
		System.out.println("UOCoordViewer Reading...");

		pixels = new int[w * h];

		PixelGrabber pg = new PixelGrabber(map.getImage(), x, y, w, h, pixels, 0, w);
		try
		{
			pg.grabPixels();
		}
		catch (InterruptedException e)
		{
			System.err.println("interrupted waiting for pixels!");
	    return;
		}
		if ((pg.getStatus() & ImageObserver.ABORT) != 0)
		{
	    System.err.println("image fetch aborted or errored");
	    return;
		}
		System.out.println("UOCoordViewer Reading... DONE");
		repaint();
	}
}