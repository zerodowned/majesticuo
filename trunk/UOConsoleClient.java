/*
 * UOConsoleClient created by Mikel Duke
 * http://mikelduke.sf.net
 * Base class with user input and connection with macro support
 *
 Modified by James Kidd
 * Everything apart from above mentioned
 * uonet.
 * useskill(n,n)
 * useobject(id)
 * singleclick(id)
 * walk(dir)
 *
 */

import java.io.*;
import java.util.Timer;
import java.util.TimerTask;
public class UOConsoleClient implements Runnable, UOPacketOperation
{
    Timer pingtimer;
	UONetworking2 uonet;
	Thread thread = new Thread(this, "JavaUOClient: Macro");
        //Runnable myping = new sendping();
        //Thread pingthread = new Thread(myping);
	//int playerX, playerY, playerZ, playerDirection;
	//int playerID, playerModel, playerHue, playerFlag, playerHighlightColor;
        //Player player = uonet.player;
	QueueLi macros = new QueueLi();
	boolean run = true;

	BufferedReader brFile;
	String filename;

	boolean fromFile = false;
	boolean loggedin = false;

	public static void main(String args[])
	{


		System.out.println("Console Java UO Client\n");
		System.out.println("http://mikelduke.sourceforge.net");
		UOConsoleClient consoleClient[] = new UOConsoleClient[1];
		BufferedReader br;
		try
		{
			boolean load = false;
			int numOfFiles = 0;
			br = new BufferedReader(new InputStreamReader(System.in));
			System.out.print("Load Info from file (Y/N)? ");
			if (br.readLine().equalsIgnoreCase("y")) load = true;
			if (load)
			{
				System.out.print("How many files to load? ");
				numOfFiles = Integer.parseInt(br.readLine());
				consoleClient = new UOConsoleClient[numOfFiles];
				for (int i = 1; i <= numOfFiles; i++)
				{
					consoleClient[(i-1)] = new UOConsoleClient("autologin" + i + ".uoj");
					try { Thread.sleep(500); } catch (Exception ex) {}
				}
			}
                        else consoleClient[0] = new UOConsoleClient("");
		}
		catch (Exception e)
		{
			System.out.println("Error Loading Login Info " + e);
		}
		//consoleClient[0] = new UOConsoleClient("autologin.uoj");
	}

	public UOConsoleClient(String filename)
	{
		this.filename = filename;
		startClient();
	}

	private void startClient()
	{
		String ip = "";
		int port = 0;
		String user = "";
		String pass = "";
		boolean clientFeatures = true;
		//ConsolePacketOp packetOperator = new ConsolePacketOp();
		fromFile = true;

		try
		{
			//load data from file
			brFile = new BufferedReader(new FileReader(filename));
			System.out.println("Loading Info From File");
			ip = brFile.readLine();
			port = Integer.parseInt(brFile.readLine());
			user = brFile.readLine();
			pass = brFile.readLine();

			//connect w/ the data
			uonet = new UONetworking2(ip, port, user, pass, this);
			uonet.connect();
		}
		catch (IllegalThreadStateException ex)
		{
			try
			{
				Thread.sleep(200);
			}
			catch (Exception e)
			{
				System.out.println(ex);
			}
		}
		catch (Exception ex)
		{
			BufferedReader br;
			fromFile = false;
			br = new BufferedReader(new InputStreamReader(System.in));



                        ip = "localhost";
                        port = 2593;
                        user = "admin2";
                        pass= "admin";
			//System.out.print("Enter Server IP: ");
			//try	{ ip = br.readLine(); }
			//catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }

			//System.out.print("Enter Server Port: ");
			//try	{ port = Integer.parseInt(br.readLine()); }
			//catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }

			//System.out.print("Enter User Name: ");
			//try	{ user = br.readLine(); }
			//catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }

			//System.out.print("Enter User Password: ");
			//try	{ pass = br.readLine(); }
			//catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }

			clearConsole();

			uonet = new UONetworking2(ip, port, user, pass, this);

			try
			{
				uonet.connect();
                                // dark ping timer
                                pingtimer = new Timer();
                                pingtimer.scheduleAtFixedRate(new sendping(), 30000, 30000);
				enterCommand();
			}
			catch (Exception e)
			{
				System.out.println(e);
			}
		}
	}

	public void enterCommand()
	{
		while (!loggedin) { try { Thread.sleep(10); } catch (Exception e) {} }
		boolean quit = false;
		while (!quit)
		{
			BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
			String command = new String();
			try	{ command = br.readLine(); }
			catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }
			doCommand(command);
		}
	}

	public void doCommand(String command)
	{
		String commandSplit[] = command.split(" ");

		if (command.equalsIgnoreCase("quit"))
		{
			run = false; //close thread if running
			uonet.disconnect();
			System.out.println("Disconnected");
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
			else System.out.println("Skill Use Error");
		}
		else if (command.equals("?"))
		{
			System.out.println("Type anything to talk.");
			System.out.println("quit exits out.");
			System.out.println("walk direction number of steps");
			System.out.println("n, ne, e, se, s, sw, w, nw");
			System.out.println("number of steps is optional");
			System.out.println("skill skillname uses the skill, so far only hiding");
		}
                else if(commandSplit[0].equalsIgnoreCase("useobject")) {
                    int mytemp = Integer.parseInt(commandSplit[1]);
                    uonet.useobject(mytemp);
                }
                 else if(commandSplit[0].equalsIgnoreCase("clickobject")) {
                    int mytemp = Integer.parseInt(commandSplit[1]);
                    uonet.singleclick(mytemp);
                }
                 else if(commandSplit[0].equalsIgnoreCase("status")) {
                    uonet.GetPlayerStatus();
                }
                else if(commandSplit[0].equalsIgnoreCase("printlist")) {
                    uonet.printlist();
                }
                else if(commandSplit[0].equalsIgnoreCase("sync")) {
                    uonet.resync();
                }
                 else if(commandSplit[0].equalsIgnoreCase("drag")) {
                    int mytemp = Integer.parseInt(commandSplit[1]);
                    int mytemp2 = Integer.parseInt(commandSplit[2]);
                    uonet.drag(mytemp,mytemp2);
                }
                    else if(commandSplit[0].equalsIgnoreCase("dropg")) {
                    int mytemp = Integer.parseInt(commandSplit[1]);
                    int mytemp2 = Integer.parseInt(commandSplit[2]);
                    int mytemp3 = Integer.parseInt(commandSplit[3]);
                    int mytemp4 = Integer.parseInt(commandSplit[4]);
                    uonet.dropg(mytemp,mytemp2,mytemp3,mytemp4);
                }
		else
		{
			uonet.say(command);
		}
	}
       
	public void clearConsole()
	{
		for (int i = 0; i < 100; i++)
			System.out.println();
	}

	private void getPlayerCoords()
	{
            
		//while (uonet.getX() == 0) try { Thread.sleep(100); } catch (Exception e) {}
		//playerX = uonet.getX();
		//playerY = uonet.getY();
		//playerZ = uonet.getZ();
                //player.setX(playerX) = uonet.player.x;
                //player.y = uonet.player.y;
                //player.z = uonet.player.z;
		System.out.println("ID: " + uonet.player.getserial() + "X: " + uonet.player.getX() + " Y: " + uonet.player.getY() + " Z: " + uonet.player.getZ());
	}

	private void getPlayerStats()
	{
	}

	private void loadMacros()
	{
		BufferedReader br;

		try
		{
			br = new BufferedReader(new FileReader("macro.uoj"));
			String line;
			line = br.readLine();
			while (line != null)
			{
				String lineWords[] = line.split(",");
				MacroAction newMacro = new MacroAction(lineWords[0], Integer.parseInt(lineWords[1]));
				macros.enQueue(newMacro);
				System.out.println(lineWords[0] + "," + Integer.parseInt(lineWords[1]));
				line = br.readLine();
			}
			thread.start();
		}
		catch (FileNotFoundException u) {}
		catch (Exception e) { System.out.println("Error Reading Macro File: " + e); }
	}

	public void run()
	{
		while (run)
		{
			MacroAction currentMacro = (MacroAction)macros.deQueue();
			doCommand(currentMacro.getAction());
			try
			{
				thread.sleep(currentMacro.getDelay());
			}
			catch (InterruptedException e)
			{
				System.out.println("Error In Macro Delay: " + e);
				run = false;
			}
			macros.enQueue(currentMacro);
			try { thread.sleep(200); } catch (Exception ex) {}
		}
	}

	public void stop()
	{
		run = false;
	}

	public void processChatPacket(String msg)
	{
		System.out.println(msg);
	}

	public void processDisconnect()
	{
		System.out.println("Disconnected");
		uonet.stop();
		this.stop();
		uonet.disconnect();
		uonet = null;
		try { Thread.sleep(60000); } catch (Exception e) {}
		startClient();
	}

	public void processServerList(String[] list)
	{
		int serverChoice = 0;
		System.out.println("Choose a Server: \n");
		for (int i = 0; i < list.length; i++)
			System.out.println(i+1 + ") "+ list[i]);

		if (brFile != null)
		{
			try { serverChoice = Integer.parseInt(brFile.readLine()); }
			catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }
		}
		else
		{
			BufferedReader br;
			br = new BufferedReader(new InputStreamReader(System.in));
			serverChoice = 0;
			try	{ serverChoice = Integer.parseInt(br.readLine()); }
			catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }
		}

		serverChoice--;
		uonet.selectServer(serverChoice);
	}

	public void processCharList(String[] list)
	{
		int charChoice = 0;
		System.out.println("Choose a Character: \n");
		for (int i = 0; i < list.length; i++)
			System.out.println(i+1 + ") " + list[i]);

		if (brFile != null)
		{
			//String charName = brFile.readLine();
			try { charChoice = Integer.parseInt(brFile.readLine()); }
			catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }
		}
		else
		{
			BufferedReader br;
			br = new BufferedReader(new InputStreamReader(System.in));
			System.out.print("Enter Character Choice: ");
			charChoice = 0;
			try	{ charChoice = Integer.parseInt(br.readLine()); }
			catch (Exception e)	{	System.out.println("Error Getting Input: " + e); System.exit(1); }
		}

		charChoice--;
		uonet.chooseChar(list[charChoice], charChoice);
	}

	public void processLoggedIn()
	{
		loggedin = true;
		System.out.println("Character Logged ON");

		if (!fromFile)
		{
			getPlayerCoords();
			BufferedReader br;
			br = new BufferedReader(new InputStreamReader(System.in));
			System.out.print("Load Macros? ");
			String sel = "n";
			try { sel = br.readLine(); }
			catch (Exception e) { System.out.println("Error Getting Input: " + e); System.exit(1); }
			if (sel.equalsIgnoreCase("y")) loadMacros();
			//else enterCommand();
		}
		else loadMacros();
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
		System.out.println("X: " + playerX + " Y: " + playerY + " Z: " + playerZ);*/
            System.out.println("X: " + uonet.player.getX() + " Y: " + uonet.player.getY());
	}

	public void error(Exception e)
	{
		System.out.println(e);
	}
class sendping extends TimerTask {
        public void run() {
        // DarkLotus
        //thread = new Thread(this, "JavaUOClient: Networking");
       byte pingpacket[] = new byte[2];
                    pingpacket[0] = (byte)0x73;
                    pingpacket[1] = 0;
                    uonet.passwrite(pingpacket);
               //throw new UnsupportedOperationException("Not supported yet.");
    }
}


}

class MacroAction
{
	private String action;
	private int delay;

	public MacroAction(String a, int d)
	{
		action = a;
		delay = d;
	}

	public String getAction()
	{
		return action;
	}

	public int getDelay()
	{
		return delay;
	}
}


