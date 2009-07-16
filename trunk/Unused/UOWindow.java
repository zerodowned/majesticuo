/*
 * UOWindow class Created by Mikel Duke
 * http://mikelduke.sf.net
 *
 * UOWindow class controls the windows and contains the main method for
 * the windowed version of my JavaUOClient.
 */

import javax.swing.*;
import javax.swing.event.*;
import java.awt.*;
import java.awt.event.*;
import java.io.*;
import java.util.LinkedList;

public class UOWindow implements Runnable,
	ActionListener, UOPacketOperation
{
	protected UONetworking2 uonet;
	protected UOWindowLogin loginWindow;
	protected UOWindowGame gameWindow;

	private String ip = new String();
	private int port = 0;
	private String user = new String();
	private String pass = new String();
	int serverSelection;
	String charList[];
	String charName;
	int charNum;

	Thread thread = new Thread(this, "JavaUOClient: Macro");

	int playerX, playerY, playerZ, playerDirection;
	int playerID, playerModel, playerHue, playerFlag, playerHighlightColor;

	QueueLi macros = new QueueLi();
	boolean run = true;

	public UOWindow()
	{
		startClient();
	}

	public void startClient()
	{
		loginWindow = new UOWindowLogin(this);

		loginWindow.setTitle("Mikel's JavaUOClient");
		loginWindow.pack();
		loginWindow.show();

		gameWindow = new UOWindowGame(this);
	}

	public static void main(String[] args)
	{
		UOWindow uo = new UOWindow();
	}

	public void run()
	{
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
		if (e.getSource() == gameWindow.sayButton)
		{
			String command = gameWindow.chatText.getText();
			//incomingText("Saying: " + command + "\n");
			doCommand(command);
		}
		if (e.getSource() == gameWindow.nButton)
		{
			String command = "walk n";
			incomingText("Walking North" + "\n");
			doCommand(command);
			playerY--;
		}
		if (e.getSource() == gameWindow.sButton)
		{
			String command = "walk s";
			incomingText("Walking South" + "\n");
			doCommand(command);
			playerY++;
		}
		if (e.getSource() == gameWindow.eButton)
		{
			String command = "walk e";
			incomingText("Walking East" + "\n");
			doCommand(command);
			playerX++;
		}
		if (e.getSource() == gameWindow.wButton)
		{
			String command = "walk w";
			incomingText("Walking West" + "\n");
			doCommand(command);
			playerX--;
		}
		if (e.getSource() == gameWindow.nwButton)
		{
			String command = "walk nw";
			incomingText("Walking NorthWest" + "\n");
			doCommand(command);
			playerY--;
			playerX--;
		}
		if (e.getSource() == gameWindow.swButton)
		{
			String command = "walk sw";
			incomingText("Walking SouthWest" + "\n");
			doCommand(command);
			playerY++;
			playerX--;
		}
		if (e.getSource() == gameWindow.neButton)
		{
			String command = "walk ne";
			incomingText("Walking NorthEast" + "\n");
			doCommand(command);
			playerY--;
			playerX++;
		}
		if (e.getSource() == gameWindow.seButton)
		{
			String command = "walk se";
			incomingText("Walking SouthEast" + "\n");
			doCommand(command);
			playerY++;
			playerX++;
		}
		if (e.getSource() == gameWindow.hideButton)
		{
			String command = "skill hide";
			incomingText("Skill: Hide" + "\n");
			doCommand(command);
		}
		if (e.getSource() == gameWindow.guardsButton)
		{
			String command = "guards";
			incomingText("Calling Guards" + "\n");
			doCommand(command);
		}
		if (e.getSource() == gameWindow.updateButton)
		{
			gameWindow.setCharLocation(playerX, playerY);
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

	public void doCommand(String command)
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
		gameWindow.sysArea.append(text);
	}

	public void processChatPacket(String msg)
	{
		incomingText(msg + "\n");
	}

	public void processDisconnect()
	{
		incomingText("DISCONNECTED\n");
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
		gameWindow.setTitle("Mikel's JavaUOClient");
		gameWindow.pack();
		gameWindow.show();

		incomingText("Character Logged ON\n");

		playerX = uonet.player.getX();
		playerY = uonet.player.getY();
		playerZ = uonet.player.getZ();
		incomingText("X: " + playerX + " Y: " + playerY + " Z: " + playerZ + " \n");
		if ((playerX == 0) && (playerY == 0) && (playerZ == 0))
			JOptionPane.showMessageDialog(null, "Possible error in reading coordinates. \n"
			+ "Please try to relogin", "Coords Error", JOptionPane.ERROR_MESSAGE);
		gameWindow.setCharLocation(playerX, playerY);
	}

	public void processUpdatePlayer(int playerID, int model, int x, int y, int z, int direction, int hue, int flag, int highlightColor)
	{/*
		playerX = x;
		playerY = y;
		playerZ = z;
		playerDirection = direction;
		this.playerID = playerID;
		playerModel = model;
		playerHue = hue;
		playerFlag = flag;
		playerHighlightColor = highlightColor;
		gameWindow.setCharLocation(playerX, playerY);*/
	}

	public void error(Exception e)
	{
		JOptionPane.showMessageDialog(null, "Error: " + e, "Error", JOptionPane.ERROR_MESSAGE);
	}
}