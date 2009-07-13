/*
 * UOGraphicalWindow class Created by Mikel Duke
 * http://mikelduke.sf.net
 *
 * UOGraphicalWindow controls the use of the login window and networking code
 * for the graphical client. It is links the two windows together basically.
 */

import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import java.net.*;

public class UOGraphicalWindow implements ActionListener, KeyListener, UOPacketOperation, Runnable
{
	UONetworking2 uonet;

	UOWindowLogin loginWindow;
	UOGraphicalGameWindow gameWindow;

	UOTileEnginePanel game;
	UOChatPanel chat;

	private String ip = new String();
	private int port = 0;
	private String user = new String();
	private String pass = new String();
	int serverSelection;
	String charList[];
	String charName;
	int charNum;

	int playerX, playerY, playerZ, playerDirection;
	int playerID, playerModel, playerHue, playerFlag, playerHighlightColor;
	QueueLi macros = new QueueLi();
	boolean run = true;

	public final int NONE = 0;
	public final int LEFT = 1;
	public final int RIGHT = 2;
	public final int UP = 3;
	public final int DOWN = 4;
	private int direction = NONE;
	Thread thread;

	String command = new String();

	public UOGraphicalWindow()
	{
		startClient();
	}

	private void startClient()
	{
		loginWindow = new UOWindowLogin(this);

		loginWindow.setTitle("Mikel's JavaUOClient");
		loginWindow.pack();
		loginWindow.show();

		game = new UOTileEnginePanel(0,0,800,500);
		chat = new UOChatPanel(0,500,800,100);
		gameWindow = new UOGraphicalGameWindow(this, game, chat); //, 1440, 1660);
		thread = new Thread(this, "JavaUOClient: Graphical Network Handler");
		thread.start();
	}

	public static void main(String args[])
	{
		UOGraphicalWindow window = new UOGraphicalWindow();
	}

	public void run()
	{
		while (run)
		{
			move();
			try
			{
				thread.sleep(20);
			}
			catch (Exception e)
			{
			}
		}
	}

	public void keyTyped(KeyEvent e) {}
	public void keyPressed(KeyEvent e)
	{
		if (e.getKeyCode() == KeyEvent.VK_LEFT) { game.setDirection(game.LEFT); direction = LEFT; }
		if (e.getKeyCode() == KeyEvent.VK_RIGHT) { game.setDirection(game.RIGHT); direction = RIGHT; }
		if (e.getKeyCode() == KeyEvent.VK_DOWN) { game.setDirection(game.DOWN); direction = DOWN; }
		if (e.getKeyCode() == KeyEvent.VK_UP) { game.setDirection(game.UP); direction = UP; }
		if (e.getKeyCode() == KeyEvent.VK_ESCAPE) System.exit(0);
		if (e.getKeyCode() == KeyEvent.VK_ENTER)
		{
			chat.addChar(e.getKeyChar());
			doCommand();
			command = new String();
		}
		else
		{
			char c = e.getKeyChar();
			if (Character.isDefined(c))
			{
				command = command + e.getKeyChar();
				chat.addChar(e.getKeyChar());
			}
		}
	}

	public void keyReleased(KeyEvent e)
	{
		if ((e.getKeyCode() == KeyEvent.VK_LEFT) || (e.getKeyCode() == KeyEvent.VK_RIGHT)
			|| (e.getKeyCode() == KeyEvent.VK_DOWN) || (e.getKeyCode() == KeyEvent.VK_UP))
		{
			game.setDirection(game.NONE);
			direction = NONE;
		}
	}

	private boolean move()
	{
		if (direction == LEFT) doCommand("walk w");
		if (direction == RIGHT) doCommand("walk e");
		if (direction == UP) doCommand("walk n");
		if (direction == DOWN) doCommand("walk s");

		if (direction == NONE) return false;
		else return true;
	}

	public void actionPerformed(ActionEvent e)
	{
		if (e.getSource() == loginWindow.loginButton)
		{
			System.out.println("Login...");
			loginWindow.listModel_serverList.clear();
			loginWindow.listModel_charList.clear();
			ip = loginWindow.getIP();
			port = loginWindow.getPort();
			user = loginWindow.getUser();
			pass = loginWindow.getPass();
			login();
		}
		if (e.getSource() == loginWindow.serverButton)
		{
			loginServer();
		}
		if (e.getSource() == loginWindow.charButton)
		{
			loginChar();
		}
	}

	private void login()
	{
		try
		{
			uonet = new UONetworking2(ip, port, user, pass, this);
			uonet.connect();
		}
		catch (Exception ex)
		{
			JOptionPane.showMessageDialog(null, ex, "Error Logging In", JOptionPane.ERROR_MESSAGE);
		}
	}

	private void loginServer()
	{
		try
		{
			serverSelection = loginWindow.serverList.getSelectedIndex();
			System.out.println("Server: " + serverSelection);
			uonet.selectServer(serverSelection);
		}
		catch (Exception ex)
		{
			JOptionPane.showMessageDialog(null, ex, "Error Logging In", JOptionPane.ERROR_MESSAGE);
		}
	}

	private void loginChar()
	{
		try
		{
			charName = (String)(loginWindow.charList.getSelectedValue());
			charNum = loginWindow.charList.getSelectedIndex();
			charName = charList[charNum];
			uonet.chooseChar(charName, charNum);
		}
		catch (Exception ex)
		{
			JOptionPane.showMessageDialog(null, ex, "Error Logging In", JOptionPane.ERROR_MESSAGE);
		}
	}

	public void doCommand(String s)
	{
		String temp = command;
		command = s;
		doCommand();
		command = temp;
	}

	public void doCommand()
	{
		String commandSplit[] = command.split(" ");

		if (command.equals("quit"))
		{
			run = false; //close thread if running
			uonet.disconnect();
			incomingText("Disconnected\n");
			System.exit(0);
		}
		else if (commandSplit[0].equalsIgnoreCase("walk"))
		{
			if (commandSplit.length > 0)
			{
				//System.out.println("Walk: " + commandSplit[1]);
				if (commandSplit.length > 2)
					uonet.walk(commandSplit[1], Integer.parseInt(commandSplit[2]));
				else
					uonet.walk(commandSplit[1], 1);
			}
		}
		else if (commandSplit[0].equalsIgnoreCase("skill"))
		{
			if (commandSplit.length > 0)
				uonet.useSkill(commandSplit[1],commandSplit[2]);
			else incomingText("Skill Use Error\n");
		}
		else if (command.equals("?"))
		{
			incomingText("Type anything to talk.\n");
			incomingText("quit exits out.\n");
			incomingText("walk direction number of steps\n");
			incomingText("n, ne, e, se, s, sw, w, nw\n");
			incomingText("number of steps is optional\n");
			incomingText("skill skillname uses the skill, so far only hiding\n");
		}
		else
		{
			uonet.say(command);
		}
	}

	private void incomingText(String text)
	{
		//gameWindow.sysArea.append(text);
		//System.out.println(chat);
		//if (chat == null) chat = new UOChatPanel(0,500,800,100);
		chat.addLine(text);
	}

	public void processChatPacket(String msg)
	{
		incomingText(msg);
	}

	public void processDisconnect()
	{
		incomingText("DISCONNECTED\n");
		JOptionPane.showMessageDialog(null, "Disconnected", "Disconnected", JOptionPane.ERROR_MESSAGE);
	}

	public void processServerList(String[] list)
	{
		String[] serverList = list;
		for (int i = 0; i < serverList.length; i++)
			loginWindow.listModel_serverList.addElement(serverList[i]);
	}

	public void processCharList(String[] list)
	{
		charList = list;
		if (list.length == 0)
			JOptionPane.showMessageDialog(null, "No Characters or Problem Loading List", "Error Logging In", JOptionPane.ERROR_MESSAGE);
		else
		{
			for (int i = 0; i < list.length; i++)
				loginWindow.listModel_charList.addElement(list[i]);
		}
	}

	public void processLoggedIn()
	{
		loginWindow.hide();
		incomingText("Character Logged ON");

		playerX = uonet.player.getX();
		playerY = uonet.player.getY();
		playerZ = uonet.player.getZ();
		incomingText("X: " + playerX + " Y: " + playerY + " Z: " + playerZ + " \n");
		if ((playerX == 0) && (playerY == 0) && (playerZ == 0))
			JOptionPane.showMessageDialog(null, "Possible error in reading coordinates. \n"
			+ "Please try to relogin", "Coords Error", JOptionPane.ERROR_MESSAGE);

		//gameWindow.setCharLocation(playerX, playerY);
		game.setXYCenter(playerX, playerY);
		gameWindow.show();
	}

	public void processUpdatePlayer(int playerID, int model, int x, int y, int z, int direction, int hue, int flag, int highlightColor)
	{
		/*playerX = x;
		playerY = y;
		playerZ = z;
		playerDirection = direction;
		this.playerID = playerID;
		playerModel = model;
		playerHue = hue;
		playerFlag = flag;
		playerHighlightColor = highlightColor;
		game.setXYCenter(playerX, playerY);*/
	}

	public void error(Exception e)
	{
		JOptionPane.showMessageDialog(null, "Error: " + e, "Error", JOptionPane.ERROR_MESSAGE);
	}
}
