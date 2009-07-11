import java.io.*;
import java.awt.Color;

public class UOTileEngineMap
{
	private static RandomAccessFile map0Reader;
	private static FileInputStream radarcolReader;

	private static UOTileEngineMapBlock[][] blocks;

	protected static int[] radarcol = new int[65536];

	public static void loadMap(String root, int blockX, int blockY)
	{
		try
		{
			blocks = new UOTileEngineMapBlock[blockX][blockY];

			map0Reader = new RandomAccessFile(root + "map0.mul", "r");
			radarcolReader = new FileInputStream(root + "radarcol.mul");

			readRadarCol();
			radarcolReader.close();
		}
		catch (Exception e)
		{
			System.out.println(e);
		}
	}

	public static void loadBlocks(int x, int y)
	{
		System.out.println("Loading blocks...");
		int BlockHeight = 512;
		int blockX = (int)(x / 8);
		if (blockX >= (int)(blocks.length * (1.0 / 3))) blockX -= (int)(blocks.length * (1.0 / 3));

		for (int i = 0; i < blocks.length; i++)
		{
			int blockY = (int)(y / 8);
			if (blockY >= (int)(blocks[i].length * (1.0 / 3))) blockY -= (int)(blocks[i].length * (1.0 / 3));

			for (int j = 0; j < blocks[i].length; j++)
			{
				long blockStart = ((blockX * BlockHeight) + blockY) * 196;
				try
				{
					byte inBlock[] = new byte[196];
					map0Reader.seek(blockStart);
					map0Reader.read(inBlock, 0, inBlock.length);
					blocks[i][j] = new UOTileEngineMapBlock(inBlock, blockX, blockY);
				}
				catch (Exception e)
				{
					//System.out.println("Error loading blocks: " + e);
				}
				blockY++;
			}
			blockX++;
		}
		System.out.println("Loading blocks DONE!");
	}

	private static void readRadarCol()
	{
		byte color[] = new byte[2];
		try
		{
			for (int i = 0; i < radarcol.length; i++)
			{
				radarcolReader.read(color, 0, color.length);
				radarcol[i] = ((color[0] & 0xFF) | ((color[1] & 0xFF) << 8));
			}
		}
		catch (Exception e)
		{
			System.out.println(e);
		}
	}

	public static int getColor(int x, int y)
	{
		int blockX = (int)(x / 8);// - (blocks.length / 2);
		int blockY = (int)(y / 8);// - (blocks.length / 2);
		int cellY = (y % 8);
		int cellX = (x % 8);

		int colors[][] = new int[8][8];
		boolean read = false;

		for (int i = 0; i < blocks.length; i++)
		{
			for (int j = 0; j < blocks[i].length; j++)
			{
				if ((blocks[i][j].blockX == blockX) && (blocks[i][j].blockY == blockY))
				{
					colors = blocks[i][j].colors;
					read = true;
					//currentBlock = blocks[i][j];
					break;
				}
			}
		}
		if (!read) //(colors == null)
		{
			System.out.println("BLOCK NOT FOUND");
			loadBlocks(x,y);
			return 0;
		}
		return colors[cellY][cellX];
	}
}

class UOTileEngineMapBlock
{
	byte cells[] = new byte[196];
	int colors[][] = new int[8][8];
	int blockX;
	int blockY;

	public UOTileEngineMapBlock(byte cells[], int blockX, int blockY)
	{
		this.cells = cells;
		this.blockX = blockX;
		this.blockY = blockY;

		loadColors();
	}

	private void loadColors()
	{
		byte cell[] = new byte[3];
		int number = 0;

		for (int i = 0; i < colors.length; i++)
		{
			for (int j = 0; j < colors[i].length; j++)
			{
				int start = 4 + (number * 3);
				cell[0] = cells[start];
				cell[1] = cells[start + 1];
				cell[2] = cells[start + 2];

				int cellID = ((cell[0] & 0xFF) | ((cell[1] & 0xFF) << 8));
				colors[i][j] = UOTileEngineMap.radarcol[cellID];
				number++;
			}
		}
	}
}

/*class TileEngineMapCell
{
	int tileID;
	int color;
	int x;
	int y;

	public TileEngineMapCell(int tileID, int color)
	{
		this.tileID = tileID;
		this.color = color;
		this.x = x;
		this.y = y;
	}
}*/