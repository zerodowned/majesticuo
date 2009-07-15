/*
 * UONetworking2 class Created by Mikel Duke
 * http://mikelduke.sf.net
 * Underlying UONetwork code has been taken, connecting, grabbing packets and decompression
 * Modified by James Kidd
 * Packet handling, as well as all packet events not relating to server connection and login
 *
 * Packet handling and events
 *
 * Rewritten version of the old UONetworking -
 * I had to extend the UOPacketOperation class to make it
 * communicate better with the gui/console/game class that uses
 * this. Also ALL packets are recieved by a thread so it should
 * make connecting easier and more reliable. Also using the class
 * is drastically simplified in connecting, and info is relaying
 * similar to how action events work.
 *
 * Handles all networking code the Java UO Client
 * Classes using this must either implement the UOPacketOperation
 * class or have a separate class implementing it that is passed
 * to this when using new. This allows the other class to do what
 * it likes with the data recieved in the packets, and acts like
 * the actionListeners in the Java API.
 *
 */

import java.net.*;
import java.io.*;
import java.lang.Byte.*;
import java.util.Collections.*;
import java.util.Arrays.*;
import java.util.Vector;
import java.util.Vector.*;


public class UONetworking2 implements Runnable
{
	private boolean debug = true;

	private Socket client;

	private String ip;
	private int port;
	private String user;
	private String pass;
	private UOPacketOperation packetOperator;

	private boolean canSendServer = false;
	private boolean canSendChar = false;

	private Thread thread;
	private boolean run = true;
	private boolean decompress = false;

	private InputStream in;
	private OutputStream out;
        public UOObject drawdata;
        Player player = new Player();

        public Vector Listitemsindex = new Vector();

        //public Vector<int> Listitemsindex = new Vector<int>();
        //public UOObject[] Listitems = new UOObject[100];
        public Vector<UOObject> Listitems = new Vector<UOObject>();
        public byte pingstep = 0;
	private final byte[] pckFirstPacket = { 0x0F, 0x70, 0x00, 0x01 };
	private final byte pckLoginreq = (byte)0x80;
	private final byte pckGameServList = (byte)0xA8;
	private final byte pckSelectServer = (byte)0xA0;
	private final byte pckLoginDenied = (byte)0x82;
	private final byte pckContogs = (byte)0x8C;
	private final byte pckCharList = (byte)0xA9;
	private final byte pckSendSpeach = (byte)0x1C;
	private final byte pckStatWindow = (byte)0x11;
	private final byte pckCharLocAndBody = (byte)0x1B;
	private final byte pckRequestWarMode = (byte)0x72;
	private final byte pckGameServerLogin = (byte)0x91;
	private final byte pckLoginChar = (byte)0x5D;
	private final byte pckTalkRequest = (byte)0xAD;
	private final byte pckClientVersion = (byte)0xBD;
	private final byte pckServerChat = (byte)0xAE;
	private final byte pckServerSpeech = (byte)0x1C;
	private final byte pckClientWalk = (byte)0x02;
        private final int SMSG_GameServlist = 0xA8;
        private final int CMSG_Loginreq = 0x80;
        private final int SMSG_DrawObject = 0x78;
        private final int MSG_CharMoveACK = 0x22;
        private final int SMSG_SetWeather = 0x65;
        private final int SMSG_WornItem = 0x2E;
        private final int SMSG_Deleteobject = 0x1D;
        private final int SMSG_UpdatePlayer = 0x77;
        public final int MSG_PingMessage = 0x73;

         public native void LoginCryptInit();
    //public native void LoginCryptEncrypt();
    //public native void Compress(byte Dest[],byte source[], int destsize, int srcsize);

	public UONetworking2(String i, int p, String user, String pass, UOPacketOperation op)
	{
		BinaryNode.CreateTree();
		ip = i;
		port = p;
		this.user = user;
		this.pass = pass;
		packetOperator = op;
		thread = new Thread(this, "JavaUOClient: Networking");
	}
        public void passwrite(byte buffer[]) {
                write(buffer);
        }
	private void write(byte buffer[])
	{
		if (!client.isConnected())
		{
			System.err.println("Disconnected");
			packetOperator.processDisconnect();
		}
		try
		{
			out.write(buffer);
			if (debug) System.out.println("Client -> Server: \n" + printPacketHex(buffer) + "\n\n");
		}
		catch (SocketException e)
		{
			System.err.println("Socket Write Error: " + e);
			packetOperator.processDisconnect();
		}
		catch (IOException e)
		{
			System.out.println("IOException, probably disconnect: " + e);
			packetOperator.error(e);
		}
		catch (Exception e)
		{
			System.out.println("WRITE ERROR: " + e);
		}
	}

	public void connect() throws UOException
	{
		try
		{
			for (int i = 0; i < 5; i++)
			{
				client = new Socket(ip, port);
				if (client.isConnected()) break;
			}
			if (client.isConnected())
			{
				out = client.getOutputStream();
				in = client.getInputStream();
				write(pckFirstPacket);
			}
			else
				throw new UOException("Unable to Connect");
		}
		catch (UnknownHostException uhe)
		{
			throw new UOException("Unknown host: \n" + uhe);
		}
		catch (ConnectException e)
		{
			throw new UOException("Internet connection may be down: \n" + e);
		}
		catch (IOException ioe)
		{
			System.out.println("IOError: \n" + ioe);
		}

		try
		{
			login();
		}
		catch (UOAccountException e)
		{
			packetOperator.error(e);
		}
	}

	public void disconnect()
	{
		try
		{
			run = false;
			client.close();
		}
		catch (IOException ioe)
		{
			System.out.println("Error Disconnecting: " + ioe);
		}
	}

	private void login() throws UOAccountException
	{
		byte loginPacket[] = new byte[62];
		loginPacket[0] = (byte)CMSG_Loginreq;
		char[] loginName = user.toCharArray();
		char[] loginPass = pass.toCharArray();
		for (int i = 0; i < loginName.length; i++)
		{
			loginPacket[i + 1] = (byte)loginName[i];
		}
		for (int i = 0; i < loginPass.length; i++)
		{
			loginPacket[i + 31] = (byte)loginPass[i];
		}
		loginPacket[61] = (byte)0xFF;

		write(loginPacket);

		thread.start();
	}

	public void run()
	{
		while (run)
		{
			try
			{
				int size = in.available();
				if (size != 0)
				{
					byte buffer[] = new byte[size];
					byte decompressBuffer[] = new byte[size];
					in.read(buffer, 0, buffer.length);

					if (decompress)	decompressBuffer = BinaryNode.Decompress(buffer);
					else decompressBuffer = buffer;

					if (debug) System.out.println("Server -> Client: \n" + printPacketHex(decompressBuffer) + "\n\n");

					byte cmd = decompressBuffer[0];
                                        switch (cmd & 0xFF) {
                                            case SMSG_GameServlist:
                                                handleServerList(decompressBuffer);
                                                break;
                                            case 0x26:
                                                handleKickPacket(decompressBuffer);
                                                break;
                                            case 0x8C:
                                            { handleKey(decompressBuffer); decompress = true; }//set thread to start decompressing packets
                                                break;
                                            case 0xB9:
                                                handleClientFeaturesPacket();
                                                break;
                                            case 0xA9:
                                                handleCharList(decompressBuffer);
                                                break;
                                            case 0xAE:
                                                handleChatPacketAE(decompressBuffer);
                                                break;
                                            case 0x1C:
                                                handleChatPacket1C(decompressBuffer);
                                                break;
                                            case 0x1B:
                                                handleInitPlayer(decompressBuffer);
                                                break;
                                            case SMSG_UpdatePlayer:
                                                handleUpdatePlayer(decompressBuffer);
                                                break;
                                            case MSG_CharMoveACK:
                                                handleCharMoveACK(decompressBuffer);
                                                break;
                                            case SMSG_SetWeather:
                                                handleSetWeather(decompressBuffer);
                                                break;
                                           case SMSG_WornItem:
                                                handleWornItem(decompressBuffer);
                                                break;
                                           case SMSG_DrawObject:
                                                handleDrawObject(decompressBuffer);
                                                break;
                                            case MSG_PingMessage:
                                                handlePingMessage(decompressBuffer);
                                                break;
                                            case SMSG_Deleteobject:
                                                handleDeleteobject(decompressBuffer);
                                                break;
                                            default:  { if (debug) System.out.println("Unknown Packet cmd: " + (cmd & 0xFF) + "fullpacket: " + printPacketHex(decompressBuffer) + "\n" +  Long.toHexString(decompressBuffer[1] & 0xFF) + "-" + Long.toHexString(decompressBuffer[2] & 0xFF)); }
                                        }

					
				}
				if (!client.isConnected())
				{
					System.err.println("Disconnected");
					packetOperator.processDisconnect();
				}
				thread.sleep(5);
			}
			catch (SocketException e)
			{
				System.out.println("Socket Error: Most likely disconnect: " + e);
				try {	client.close(); } catch (IOException e2) {}
				packetOperator.processDisconnect();
			}
			catch (IOException e)
			{
				System.out.println("IOError: " + e);
			}
			catch (NegativeArraySizeException e)
			{
			}
			catch (InterruptedException e)
			{
				System.out.println("Thread Error: " + e);
			}
			catch (Exception e)
			{
                           
				System.out.println("Some other error in the thread: " + e);
				e.printStackTrace();
			}
		}
	}

	public void stop()
	{
		run = false;
	}
        private void handleDeleteobject(byte buffer[]) {
            Boolean myresult = true;
            byte myobj[] = new byte[5];
             for(int i = 0; i < myobj.length; i++)
                myobj[i] = buffer[i];
            int itemid = (( myobj[1] <<24) | ( myobj[2] <<16) | ( myobj[3] <<8) | ( myobj[4]));
             for (int i = 0; i < Listitemsindex.size(); i++) {
                if (itemid == Listitemsindex.get(i)) {
               Listitemsindex.remove(i);
                Listitems.remove(i);
                myresult = false;
                break;
                }
            }
             if (myresult) {
                 if (debug) System.out.println("Object delete ignored, not found" + printPacketHex(myobj) + "\n\n");

             }


        }
        private void handlePingMessage(byte buffer[]) {
            byte myping[] = new byte[2];
            for(int i = 0; i < myping.length; i++)
                myping[i] = buffer[i];
            pingstep = myping[1];
        }
        private void handleWornItem(byte buffer[])
        {
            try {
            byte wornitem[] = new byte[15];
            for (int i = 0; i < wornitem.length; i++)
			wornitem[i] = buffer[i];
            int itemid = ((wornitem[1] <<24) | (wornitem[2] <<16) | (wornitem[3] <<8) | (wornitem[4]));
            int mobileid = ((wornitem[9] <<24) | (wornitem[10] <<16) | (wornitem[11] <<8) | (wornitem[12]));

            if (debug) System.out.println("wormitemid: " + itemid + "MobID: " + mobileid);
            }
            catch(ArrayIndexOutOfBoundsException e)
		{ System.out.println("Error: Wornitem packet too long: " + e); }
            }

        private void  handleSetWeather(byte buffer[])
        {
         byte weather[] = new byte[4];
         for (int i = 0; i < weather.length; i++)
			weather[i] = buffer[i];
         int weathertype = weather[1];
         //int weathernum = weather[2];

         switch(weathertype & 0xFF)
         {
             case 0:
                 System.out.println("It begins to rain");
                 break;
             case 1:
                  System.out.println("A Fierce Storm!");
                 break;
             case 2:
                  System.out.println("It begins to snow");
                 break;
             case 3:
                  System.out.println("A Storm is Brewing");
                 break;
             case 254:
                 System.out.println("0xFE recived unknown weather");

             default:
                 System.out.println("Unknown weather: " + (weathertype & 0xFF));

         }

        }

        private void handleCharMoveACK(byte buffer[])
        {
            //byte Moveack[] = new byte[3];
            //for (int i = 0; i < Moveack.length; i++)
	//		Moveack[i] = buffer[i];
          //  int iSeq = Moveack[1];
           // int iNot = Moveack[2];
// Do nothing
        }


        private void handleDrawObject(byte buffer[])
        {
            boolean myresult = true;
            int size = ((buffer[1] & 0xFF) <<8) | (buffer[2] & 0xFF);
            byte incMobile[] = new byte[1];
            try { incMobile = new byte[size]; }
		catch(ArrayIndexOutOfBoundsException e)
		{ System.out.println("Error: Mobile data too long"); }
            for (int i = 0; i < incMobile.length; i++)
			incMobile[i] = buffer[i];
             int itemid = ((incMobile[3] <<24) | (incMobile[4] <<16) | (incMobile[5] <<8) | (incMobile[6]));
             int type = ((incMobile[7] <<8) | (incMobile[8] & 0xFF));
             int itemx = ((incMobile[9] <<8) | (incMobile[10] & 0xFF));
             int itemy = ((incMobile[11] <<8) | (incMobile[12] & 0xFF));
             int itemz = incMobile[13];
             int color = ((incMobile[14] <<8) | (incMobile[15]));
             // store the id in a 2nd array, if the id is found we update the data, if not we replace
            for (int i = 0; i < Listitemsindex.size(); i++) {
                if (itemid == Listitemsindex.get(i)) {
                drawdata = new UOObject(itemid,type,itemx,itemy,itemz,color);
                Listitems.set(i,drawdata);
                myresult = false;
                break;
                }
            }
             if (myresult) {
                 Listitemsindex.add(itemid);
                 Listitems.add(drawdata);
             }

            //UOObject currentobj = Listitems.firstElement();
            
            //System.out.println(incMobile[0] + " " + (incMobile[0] & 0xFF));
            
             System.out.println("Draw Object ID: " + itemid + " type: " + type + "X: " + itemx + "Y: " + itemy);
        }

	private void handleServerList(byte[] buffer)
	{
		try
		{
			String list[] = getServerList(buffer);
			packetOperator.processServerList(list);
		}
		catch (UOAccountException e)
		{
			packetOperator.error(e);
		}
	}

	private String[] getServerList(byte[] buffer) throws UOAccountException
	{
		String serverNames[] = new String[1];
		int size = ((((byte)buffer[1] & 0xFF) >>8) | ((byte)buffer[2] & 0xFF));

		int numOfServers = ((((byte)buffer[4] & 0xFF) >>8) | ((byte)buffer[5] & 0xFF));
		serverNames = new String[numOfServers];

		for (int i = 6; i < (size - 6); i+=40)
		{
			String serverName = "";
			for (int j = 2; j < 34; j++)
				if (buffer[j+i] != 0x00) serverName = serverName + (char)buffer[j+i];
			serverNames[(i - 6)/40] = serverName;
		}
		return serverNames;
	}

	public void selectServer(int server)
	{
		byte serverPacket[] = new byte[3];
		serverPacket[0] = pckSelectServer;
		serverPacket[1] = (byte)(server >>8);
		serverPacket[2] = (byte)server;

		write(serverPacket);
	}

	private void handleKickPacket(byte[] buffer)
	{
		System.out.println("Player Kicked");
	}

	private void handleKey(byte[] buffer)
	{
		//if (buffer.length != 11) return;
		sendKey(getKey(buffer));
	}

	private byte[] getKey(byte[] buffer)
	{
		long keyInt = 0;
		String ip = (int)buffer[1] + "." + (int)buffer[2] + "." + (int)buffer[3] + "." + (int)buffer[4];
		int port = ((buffer[5] <<8) | buffer[6]);

		byte key[] = new byte[4]; //now copies bytes rather than try to decode the long
		key[0] = buffer[7];
		key[1] = buffer[8];
		key[2] = buffer[9];
		key[3] = buffer[10];
		return key;
	}

	private void sendKey(byte key[])
	{
		byte keyPacket[] = new byte[65];
		keyPacket[0] = pckGameServerLogin;
		keyPacket[1] = key[0]; //copy key bytes out of array
		keyPacket[2] = key[1];
		keyPacket[3] = key[2];
		keyPacket[4] = key[3];

		char userAr[] = user.toCharArray();
		for (int i = 0; i < userAr.length; i++)
			keyPacket[i+5] = (byte)userAr[i];

		char passAr[] = pass.toCharArray();
		for (int i = 0; i < passAr.length; i++)
			keyPacket[i+35] = (byte)passAr[i];

		write(keyPacket);
	}

	private void handleClientFeaturesPacket()
	{
		//do nothing
	}



	private void handleCharList(byte buffer[])
	{
		try
		{
			String[] list = getCharList(buffer);
			packetOperator.processCharList(list);
		}
		catch (UOException e)
		{
			packetOperator.error(e);
		}
	}

	private String[] getCharList(byte buffer[]) throws UOException
	{
		String charList[] = new String[1];
		int numOfChars = 0;
		if (buffer.length > 4) numOfChars = (buffer[3] & 0xFF);
		else { System.out.println("Char List Error"); System.exit(1); }
		charList = new String[numOfChars];
		for (int i = 0; i < numOfChars; i++)
		{
			String charName = "";
			for (int j = 0; j < 30; j++)
				if ((buffer[(i * 60) + j + 4]) != 0x00)
					charName = charName + (char)buffer[(i * 60) + j + 4];
			charList[i] = charName;
		}
		return charList;
	}

	public void chooseChar(String character, int slot)
	{
		byte charPacket[] = new byte[73];
		char charName[] = character.toCharArray();
		charPacket[0] = pckLoginChar;
		charPacket[1] = (byte)0xED;
		charPacket[2] = (byte)0xED;
		charPacket[3] = (byte)0xED;
		charPacket[4] = (byte)0xED;
		for (int i = 0; i < charName.length; i++)
			charPacket[i + 5] = (byte)charName[i];
		charPacket[68] = (byte)slot;
		byte localAddress[] = new byte[4];
		localAddress = (client.getLocalAddress()).getAddress();
		for (int i = 0; i < localAddress.length; i++)
			charPacket[i + 69] = localAddress[i];

		write(charPacket);
		sendClient("2.0.3");
	}

	private void sendClient(String client)
	{
		char clientAr[] = client.toCharArray();
		byte clientPacket[] = new byte[clientAr.length + 4];
		clientPacket[0] = pckClientVersion;
		clientPacket[1] = (byte)(clientPacket.length >>8);
		clientPacket[2] = (byte)clientPacket.length;
		for (int i = 0; i < clientAr.length; i++)
			clientPacket[i + 3] = (byte)clientAr[i];

		write(clientPacket);
	}

	public void say(String text)
	{
		byte sayPacket[];
		if (text.equalsIgnoreCase("guards"))
		{
			byte guards[] = { (byte)0xAD, 0x00, 0x16, (byte)0xC0,
			0x00, 0x5A, 0x00, 0x03, 0x45, 0x4E, 0x55,
			0x00, 0x00, 0x10, 0x07, 0x67, 0x75, 0x61,
			0x72, 0x64, 0x73, 0x00 };
			sayPacket = guards;
		}
		else if (text.equalsIgnoreCase("bank"))
		{
			byte bank[] = { (byte)0xAD, 0x00, 0x14, (byte)0xC0,
			0x00, 0x5A, 0x00, 0x03, 0x45, 0x4E, 0x55,
			0x00, 0x00, 0x10, 0x02, 0x62, 0x61, 0x6E,
			0x6B, 0x00 };
			sayPacket = bank;
		}
		else
		{
			char textAr[] = text.toCharArray();
			sayPacket = new byte[(textAr.length * 2) + 14];
			sayPacket[0] = pckTalkRequest;
			sayPacket[1] = (byte)(sayPacket.length >>8);
			sayPacket[2] = (byte)sayPacket.length;
			sayPacket[3] = (byte)0x00; //mode 0xc0 if guards vender etc
			sayPacket[4] = 0x00; //hue
			sayPacket[5] = 0x04; //hue
			sayPacket[6] = 0x00; //font
			sayPacket[7] = 0x03; //font
			sayPacket[8] = (byte)'E';
			sayPacket[9] = (byte)'N';
			sayPacket[10] = (byte)'U';
			sayPacket[11] = 0x00;
			for (int i = 0; i < textAr.length; i++)
				sayPacket[(i * 2)+13] = (byte)textAr[i];
		}
		write(sayPacket);
	}

	public boolean walk(String direction, int numOfSteps)
	{
		byte walkPacket[] = new byte[7];
		walkPacket[0] = pckClientWalk;
		if ((direction.equalsIgnoreCase("n")) || (direction.equalsIgnoreCase("north")))
			walkPacket[1] = (byte)0x80;
		else if ((direction.equalsIgnoreCase("ne")) || (direction.equalsIgnoreCase("northeast")))
			walkPacket[1] = (byte)0x81;
		else if ((direction.equalsIgnoreCase("e")) || (direction.equalsIgnoreCase("east")))
			walkPacket[1] = (byte)0x82;
		else if ((direction.equalsIgnoreCase("se")) || (direction.equalsIgnoreCase("southeast")))
			walkPacket[1] = (byte)0x83;
		else if ((direction.equalsIgnoreCase("s")) || (direction.equalsIgnoreCase("south")))
			walkPacket[1] = (byte)0x84;
		else if ((direction.equalsIgnoreCase("sw")) || (direction.equalsIgnoreCase("southwest")))
			walkPacket[1] = (byte)0x85;
		else if ((direction.equalsIgnoreCase("w")) || (direction.equalsIgnoreCase("west")))
			walkPacket[1] = (byte)0x86;
		else if ((direction.equalsIgnoreCase("nw")) || (direction.equalsIgnoreCase("northwest")))
			walkPacket[1] = (byte)0x87;
		walkPacket[2] = (byte)0x00;
		walkPacket[3] = (byte)0xFF;
		walkPacket[4] = (byte)0xFF;
		walkPacket[5] = (byte)0xFF;
		walkPacket[6] = (byte)0xFF;

		try
		{
			for (int i = 0; i < numOfSteps; i++)
			{
				write(walkPacket);
				Thread.sleep(190);
			}
		}
		catch (InterruptedException e)
		{
			System.out.println("Error In Walking Delay " + e);
		}
		return true;
	}

	public void useSkill(String var1, String var2)
	{
		byte skillPacket[] = new byte[9];
		skillPacket[0] = (byte)0x12;
		skillPacket[1] = 0x00; //block size
		skillPacket[2] = 0x09;
		skillPacket[3] = 0x24;
                skillPacket[4] = java.lang.Byte.parseByte(var1);
                skillPacket[5] = java.lang.Byte.parseByte(var2);
                skillPacket[6] = (byte)' ';
		skillPacket[7] = (byte)'0';

		write(skillPacket);
	}
	public void useSkill2(String skill)
	{
		byte skillPacket[] = new byte[9];
		skillPacket[0] = (byte)0x12;
		skillPacket[1] = 0x00; //block size
		skillPacket[2] = 0x09;
		skillPacket[3] = 0x24;
		if (skill.equalsIgnoreCase("hide") || skill.equalsIgnoreCase("hiding"))
		{
			skillPacket[4] = (byte)'2';
			skillPacket[5] = (byte)'1';
			skillPacket[6] = (byte)' ';
			skillPacket[7] = (byte)'0';
		}
		write(skillPacket);
	}


	private void handleInitPlayer(byte buffer[])
	{
		handleCharLoc(buffer);
		packetOperator.processLoggedIn();
	}

	private void handleCharLoc(byte buffer[])
	{
		byte charLoc[] = new byte[37];
		for (int i = 0; i < charLoc.length; i++)
			charLoc[i] = buffer[i];
		player.setserial(((charLoc[1] <<24) | (charLoc[2] <<16) | (charLoc[3] <<8) | (charLoc[4])));
		player.settype((charLoc[9] <<8 | charLoc[10] &0xFF));
                player.setX(((charLoc[11] & 0xFF) <<8) | (charLoc[12] & 0xFF));
		player.setY(((charLoc[13] & 0xFF) <<8) | (charLoc[14] & 0xFF));
		player.setZ(((charLoc[15] & 0xFF) <<8) | (charLoc[16] & 0xFF));
                //System.out.println("Check if drawdata is in list" + Listitems.(drawdata.serial));
                //Listitems.add(Char);
            //UOObject currentobj = Listitems.firstElement();

            //System.out.println(Char);

		//packetOperator.processUpdatePlayer(playerID, model, x, y, z, direction, hue, flag, highlightColor);

	}
/*
	public int getX()
	{
		int i = 0;
		while ((Char2.x == 0) && (Char2.x == 0) && (Char2.x == 0) && (i < 10))
		{
			try { Thread.sleep(100); } catch (Exception e) {}
			i++;
		}
		return Char2.x;
	}

	public int getY()
	{
		//while ((playerX == 0) && (playerY == 0) && (playerZ == 0))
		//try { Thread.sleep(100); } catch (Exception e) {}
		return Char2.y;
	}

	public int getZ()
	{
		//while ((playerX == 0) && (playerY == 0) && (playerZ == 0))
		//try { Thread.sleep(100); } catch (Exception e) {}
		return Char2.z;
	}
*/
	private void handleChatPacketAE(byte buffer[])
	{
		String chatMsg = "";
		//int size = (in.read() >>8) | in.read();
		int size = ((buffer[1] & 0xFF) <<8) | (buffer[2] & 0xFF);
		byte chat[] = new byte[1];
		try { chat = new byte[size]; }
		catch(ArrayIndexOutOfBoundsException e)
		{ System.out.println("Error: Message too long"); }
		for (int i = 0; i < chat.length; i++)
			chat[i] = buffer[i];

		String name = "";
		String msg = "";
		for (int i = 17; i < 47; i++)
			if (chat[i] != 0x00) name = name + (char)chat[i];
		for (int i = 47; i < chat.length; i++)
			if (chat[i] != 0x00) msg = msg + (char)chat[i];
		chatMsg = name + ": " + msg;
		packetOperator.processChatPacket(chatMsg);
	}

	private void handleChatPacket1C(byte buffer[])
	{
		String chatMsg = "";
		int size = (buffer[1] >>8) | buffer[2];
		byte chat[] = new byte[size];
		for (int i = 0; i < chat.length; i++)
			chat[i] = buffer[i];

		String name = "";
		String msg = "";
		for (int i = 14; i < 44; i++)
			if (chat[i] != 0x00) name = name + (char)chat[i];
		for (int i = 44; i < chat.length; i++)
			if (chat[i] != 0x00) msg = msg + (char)chat[i];
		chatMsg = name + ": " + msg;
		packetOperator.processChatPacket(chatMsg);
	}

	private void handleUpdatePlayer(byte buffer[])
	{
                Boolean myresult = true;
                int itemid = (buffer[4] & 0xFF) | ((buffer[3] & 0xFF) << 8) | ((buffer[2] & 0xFF) << 16) | ((buffer[1] & 0xFF) << 24);
		int model = (buffer[6] & 0xFF) | ((buffer[5] & 0xFF) << 8);
		int x = (buffer[8] & 0xFF) | ((buffer[7] & 0xFF) << 8);
		int y = (buffer[10] & 0xFF) | ((buffer[9] & 0xFF) << 8);
		int z = (buffer[11] & 0xFF);
		int direction = (buffer[12] & 0xFF);
		int hue = (buffer[14] & 0xFF) | ((buffer[13] & 0xFF) << 8);
		int flag = (buffer[15] & 0xFF);
		int highlightColor = (buffer[16] & 0xFF);
for (int i = 0; i < Listitemsindex.size(); i++) {
                
                if (itemid == Listitemsindex.get(i)) {
                drawdata = new UOObject(itemid,model,x,y,z,hue);
                Listitems.set(i,drawdata);
                myresult = false;
                break;
                }
            }
             if (myresult) {
                 Listitemsindex.add(itemid);
                 Listitems.add(drawdata);
             }
             if (itemid == player.getserial()) {
                 player.setX(x);
                 player.setY(y);
                 player.setZ(z);
             }


               

		packetOperator.processUpdatePlayer(itemid, model, x, y, z, direction, hue, flag, highlightColor);
	}

	public static String printPacket(char buffer[])
	{
		byte byteBuffer[] = new byte[buffer.length];
		for (int i = 0; i < buffer.length; i++)
			byteBuffer[i] = (byte)buffer[i];
		return printPacket(byteBuffer);
	}

	public static String printPacket(byte buffer[])
	{
		String packet = "";
		for (int i = 0; i < buffer.length; i++)
			packet = packet + (buffer[i] & 0xFF) + " ";
			//System.out.print((buffer[i] & 0xFF) + " ");
		packet = packet + "\n\n";
		//System.out.println("\n");
		for (int i = 0; i < buffer.length; i++)
			packet = packet + Long.toHexString(buffer[i] & 0xFF) + " ";
			//System.out.print(Long.toHexString(buffer[i] & 0xFF) + " ");
		packet = packet + "\n\n";
		//System.out.println("\n");
		for (int i = 0; i < buffer.length; i++)
			packet = packet + (char)buffer[i] + " ";
			//System.out.print((char)buffer[i] + " ");
		return packet;
		//System.out.println();
	}

	public static String printPacketHex(byte buffer[])
	{
		String packet = new String();
		for (int i = 0; i < buffer.length; i++)
			packet = packet + Long.toHexString(buffer[i] & 0xFF) + " ";
		return packet;
	}

	public static byte[] charToByte(char charAr[])
	{
		byte byteAr[] = new byte[charAr.length];
		for (int i = 0; i < charAr.length; i++)
			byteAr[i] = (byte)charAr[i];
		return byteAr;
	}

}
