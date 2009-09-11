using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace drkuo
{
    class uonetwork
    {
        public String myoutput = "";
        public uoplayer player = new uoplayer();
        public uoobject drawdata; // rename?
        public UOopcodes opcode;

        private Socket client;
        private StateObject mystate;


        private int mybufferpos = 0;
        HuffmanDecompression decomp;
        private String ip;
	    private int port;
	    private String user;
	    private String pass;
        private uointerface packops;
        private bool run = true;
	    private InputStream myin;
	    private OutputStream myout;
        public Vector Listitemsindex = new Vector();
        public Vector<UOObject> Listitems = new Vector<UOObject>();
        public byte pingstep = 0;
        private Boolean debug = true;
       
        public uonetwork(String i, int p, String usera, String passa, uointerface uoi)
        {
            BinaryNode.CreateTree();
		    ip = i;
		    port = p;
		    this.user = usera;
		    this.pass = passa;
		    packops = uoi;
            
            
        }
       
       
        //{ 0x0F, 0x70, 0x00, 0x01 };

    private void writeoutput(String temp) 
    {
    myoutput = myoutput + "\n" + temp;
    }
	private void write(byte[] buffer)
	{
		if (!client.isConnected())
		{

			packetOperator.processDisconnect();
		}
		try
		{

                            myout.write(buffer);

			
			if (debug) writeoutput("Client -> Server: \n" + printPacketHex(buffer));
		}
		catch (SocketException e)
		{
			System.err.println("Socket Write Error: " + e);
			packetOperator.processDisconnect();
		}
		catch (IOException e)
		{
			writeoutput("IOException, probably disconnect: " + e);
			packetOperator.error(e);
		}
		catch (Exception e)
		{
			writeoutput("WRITE ERROR: " + e);
		}
	}

	public void connect()
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
				myout = client.getOutputStream();
				myin = client.getInputStream();
                               
                                       client.setReceiveBufferSize(2048);
                                       int misize = client.getReceiveBufferSize();
                               writeoutput(" buffersize: " + misize);
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
			writeoutput("IOError: \n" + ioe);
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
			writeoutput("Error Disconnecting: " + ioe);
		}
	}

	private void login()
	{
		byte[] loginPacket = new byte[62];
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
                if (UseCrypt) {
                    lcrypt.k1 = 0x2D93A5FD;
                lcrypt.k2 = 0xA3DD527F;
               lcrypt.pseed = 0x0100007F;//0x0100007F;
                // lcrypt.pseed = ((pckFirstPacket[0] << 24) | (pckFirstPacket[1] <<16) | (pckFirstPacket[2] << 8) | (pckFirstPacket[3]));
                lcrypt.initlogin();
                writeoutput(printPacketHex(loginPacket));
               //writeoutput(lcrypt.m_key[0] + "SPACE" + lcrypt.m_key[1]);
                byte[] tempbyte = lcrypt.decrypt(loginPacket, loginPacket.length);
		lcrypt.initlogin();
                writeoutput(printPacketHex(tempbyte));
                write(tempbyte);
                byte[] tempbyte2 = lcrypt.encrypt(tempbyte, tempbyte.length);
                writeoutput("DECODED " + printPacketHex(tempbyte2));
                }
                else {
                    write(loginPacket);
                }
		thread.start();
	}  
	public void run()
	{
         // output needs to be the size of the data, remove excess 00 00?
        //byte[] buffer = new byte[2000]; // create a large buffer, should be able to hold all traffic
        ArrayList<Byte> aBuffer = new ArrayList<Byte>();
        ArrayList<Byte> aOutput = new ArrayList<Byte>();
         // create a large buffer, should be able to hold all traffic
       // byte decompressBuffer[] = new byte[2000];
		while (run)
		{
                System.Threading.Thread.Sleep(5);
                if (!client.isConnected())
				{
					writeoutput("Disconnected");
					packetOperator.processDisconnect();
				}

			try
			{
				int size = myin.available(); 
				if (size != 0)
				{
                                byte[] incoming = new byte[myin.available()];
                                myin.read(incoming);
                               for(int i = 0; i < incoming.length;i++) {
                                    aBuffer.add(incoming[i]);
                                } // fills our vecotor with the packet
                                }
                                while (!aBuffer.isEmpty()) {
                                    if (bDecompress) {
                                        byte[] tempbuf = new byte[aBuffer.size()]; // create a buffer to hold list
                                        for(int x = 0; x < aBuffer.size();x++) { tempbuf[x] = aBuffer.get(x); } // adds list to buffer
                                           if(aBuffer.size() > 1000) {
                                               int zx = 55+5;
                                           }
                                        huffmanobject myhuf = new huffmanobject();
                                        myhuf.buffer = tempbuf;
                                        myhuf.src_size = tempbuf.length;
                                        myhuf.out_size = 0;
                                        myhuf = BinaryNode.drkDecompress(myhuf);
                                        byte[] temp = myhuf.output;
                                        if (temp.length < 1) { aBuffer.clear(); break; }// if we are getting garbage wipe the buffer.
                                        if (tempbuf.length > temp.length) {
                                            writeoutput("Packet: " + aBuffer.size() + "bytes consumed: " + myhuf.out_size);
                                            int intx = 1;
                                             for(int x = 0; x < temp.length;x++) { aOutput.add(temp[x]); } // outputs the packet
                                             for(int x = 0; x < myhuf.out_size + intx;x++) { aBuffer.remove(0); } // removes it from queue
                                        }
                                        else {
                                             for(int x = 0; x < temp.length;x++) { aOutput.add(temp[x]); } // outputs the packet
                                              if (aBuffer.isEmpty() == false) {
                                                aBuffer.clear();
                                            }
                                        }
                                    }

                                    else {
                                        byte[] temp = new byte[aBuffer.size()]; // create a buffer to hold list
                                        for(int x = 0; x < aBuffer.size();x++) { temp[x] = aBuffer.get(x); } // adds list to buffer
                                              if (aBuffer.isEmpty() == false) {
                                                aBuffer.clear();
                                              }
                                            for(int x = 0; x < temp.length;x++) { aOutput.add(temp[x]); }
                                    }
                                    byte[] temp = new byte[aOutput.size()];
                                    for(int x = 0; x < aOutput.size();x++) { temp[x] = aOutput.get(x); }
                                    handlePacket(temp);
                                    aOutput.clear();
                                }

                        }// end  of newdata IF

                                 // end of try
			catch (SocketException e)
			{
				writeoutput("Socket Error: Most likely disconnect: " + e);
				try {	client.close(); } catch (IOException e2) {}
				packetOperator.processDisconnect();
			}
			catch (IOException e)
			{
				writeoutput("IOError: " + e);
			}
                        catch (NegativeArraySizeException e)
			{
			}
			catch (Exception e)
			{
                           
				writeoutput("Some other error in the thread: " + e);
				e.printStackTrace();
			}
                }
        }

        public void enablecrypt() {
            if (UseCrypt) {
          
            bCrypt = true;

            
            int mkey = ((mykey[0] <<24) | (mykey[1] <<16) | (mykey[2] <<8) | (mykey[3]));
            //int nkey = ((mkey<<96) | (mkey<<64) | (mkey<<32) | (mkey));
           byte[] zkey = new byte[16];
           zkey[0] = mykey[0];
           zkey[1] = mykey[1];
           zkey[2] = mykey[2];
           zkey[3] = mykey[3];
           zkey[4] = mykey[0];
           zkey[5] = mykey[1];
           zkey[6] = mykey[2];
           zkey[7] = mykey[3];
           zkey[8] = mykey[0];
           zkey[9] = mykey[1];
           zkey[10] = mykey[2];
           zkey[11] = mykey[3];
           zkey[12] = mykey[0];
           zkey[13] = mykey[1];
           zkey[14] = mykey[2];
           zkey[15] = mykey[3];
                    //zkey = ((mykey <<96) | (mykey <<64) | (mykey <<32) | mykey);
       writeoutput("KEY" + zkey);
        try {
            twofishob = twofish.makeKey(zkey);
        } catch (InvalidKeyException ex) {
           // Logger.getLogger(UONetworking2.class.getName()).log(Level.SEVERE, null, ex);
        }
        }
 }
                                        public void handlePacket(byte[] decompressBuffer) {
                                           //if (debug) writeoutput("Server -> Client: \n" + printPacketHex(decompressBuffer) + "\n\n");
                                      switch (decompressBuffer[0] & 0xFF) {
                                            case SMSG_GameServlist:
                                                handleServerList(decompressBuffer);
                                                break;
                                            case SMSG_KickPlayer:
                                                handleKickPacket(decompressBuffer);
                                                break;
                                            case 0x8C:
                                            { handleKey(decompressBuffer); bDecompress = true; enablecrypt(); }//set thread to start decompressing packets
                                                break;
                                            case SMSG_ClientFeatures:
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
                                            case SMSG_CharLocAndBody:
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
                                            case SMSG_OverallLightLevel:
                                                handleOverallLightLevel(decompressBuffer);
                                                break;
                                            case SMSG_ObjectInfo:
                                                handleObjectInfo(decompressBuffer);
                                                break;
                                            case SMSG_StatusBarInfo:
                                                handleStatusBarInfo(decompressBuffer);
                                                break;
                                            case SMSG_DrawGamePlayer:
                                                handleDrawGamePlayer(decompressBuffer);
                                                    break;
                                          case SMSG_GeneralInformation:
                                              handleGeneralInformation(decompressBuffer);
                                              break;
                                          default:  { if (debug) writeoutput("Unknown Packet: " + printPacketHex(decompressBuffer) + "\n" ); }
                                        }

                                }

	public void stop()
	{
		run = false;
	}
        private void handleGeneralInformation(byte[] buffer) {
            // do nothing
        }
        private void handleOverallLightLevel(byte[] buffer){
            // do nothing
        }
        private void handleStatusBarInfo(byte[] buffer) {
         int size = ((buffer[1] & 0xFF) <<8) | (buffer[2] & 0xFF);
            byte[] status = new byte[1];
            try { status = new byte[size]; }
		catch(ArrayIndexOutOfBoundsException e)
		{ writeoutput("Error: Mobile data too long"); }
            for (int i = 0; i < status.length; i++)
			status[i] = buffer[i];
            player.setserial((( status[3] <<24) | ( status[4] <<16) | ( status[5] <<8) | ( status[6])));
            byte[] myname = new byte[30];
            for (int i = 0; i < myname.length; i++) {
                myname[i] = status[i+7];
            }
            player.name = new String(myname); // need to remove trailing spaces
            player.curhp = ((status[37] <<8) | (status[38] & 0xFF));
            player.maxhp = ((status[39] <<8) | (status[40] & 0xFF));
            int statusflag = status[41];
            player.sex = status[42];
            player.str = ((status[43] <<8) | (status[44] & 0xFF));
            player.dex = ((status[45] <<8) | (status[46] & 0xFF));
            player.intel = ((status[47] <<8) | (status[48] & 0xFF));
            player.curstam = ((status[49] <<8) | (status[50] & 0xFF));
            player.maxstam = ((status[51] <<8) | (status[52] & 0xFF));
            player.curmana = ((status[53] <<8) | (status[54] & 0xFF));
            player.maxmana = ((status[55] <<8) | (status[56] & 0xFF));
            player.gold = ((status[57] <<8) | (status[58] & 0xFF));
            int armor = ((status[59] <<8) | (status[60] & 0xFF));
            player.weight = ((status[61] <<8) | (status[62] & 0xFF));
            if (statusflag > 4) {
                player.maxweight = ((status[63] <<8) | (status[64] & 0xFF));
            }

      if (debug) writeoutput("Status Update ID:" + player.getserial() + " Name: " + player.name + " Cur HP: " + player.curhp + "Weight: " + player.weight + "max: " + player.maxweight + "\n");
        }

        private void handleDeleteobject(byte[] buffer) {
            Boolean myresult = true;
            byte[] myobj = new byte[5];
             for(int i = 0; i < myobj.length; i++)
                myobj[i] = buffer[i];
            int itemid = (( myobj[1] <<24) | ( myobj[2] <<16) | ( myobj[3] <<8) | ( myobj[4]));
             for (int i = 0; i < Listitemsindex.size(); i++) {
                if (itemid == Listitemsindex.get(i) || (itemid & 0xBFFFFF) == Listitemsindex.get(i)) {
               Listitemsindex.remove(i);
                Listitems.remove(i);
                myresult = false;
                 if (debug) writeoutput("Object deleted " + printPacketHex(myobj) + "\n");
                break;
                }
            }
             if (myresult) {
                 if (debug) writeoutput("Object delete ignored, not found" + printPacketHex(myobj) + "\n\n");

             }


        }
        private void handlePingMessage(byte[] buffer) {
            byte[] myping = new byte[2];
            for(int i = 0; i < myping.length; i++)
                myping[i] = buffer[i];
            pingstep = myping[1];
        }
        private void handleWornItem(byte[] buffer)
        {
            try {
           Boolean myresult = true;
                byte[] wornitem = new byte[15];
            for (int i = 0; i < wornitem.length; i++)
			wornitem[i] = buffer[i];
            int itemid = ((wornitem[1] <<24) | (wornitem[2] <<16) | (wornitem[3] <<8) | (wornitem[4]));
            int mobileid = ((wornitem[9] <<24) | (wornitem[10] <<16) | (wornitem[11] <<8) | (wornitem[12]));
            int itemx = 0;
            int itemy = 0;
            int itemz = 0;
            int hue = 0;
            int type = 0;
            //if (debug) writeoutput("wormitemid: " + itemid + "MobID: " + mobileid);
             drawdata = new UOObject(itemid,type,itemx,itemy,itemz,hue);
                for (int i = 0; i < Listitemsindex.size(); i++) {
                if (itemid == Listitemsindex.get(i)) {

                Listitems.set(i,drawdata);
                myresult = false;
                break;
                }
            }
             if (myresult) {
                 Listitemsindex.add(itemid);
                 Listitems.add(drawdata);
             }
            }
            catch(ArrayIndexOutOfBoundsException e)
		{ writeoutput("Error: Wornitem packet too long: " + e); }
            }

        private void  handleSetWeather(byte[] buffer)
        {
         byte[] weather = new byte[4];
         for (int i = 0; i < weather.length; i++)
			weather[i] = buffer[i];
         int weathertype = weather[1];
         //int weathernum = weather[2];

         switch(weathertype & 0xFF)
         {
             case 0:
                 writeoutput("It begins to rain");
                 break;
             case 1:
                  writeoutput("A Fierce Storm!");
                 break;
             case 2:
                  writeoutput("It begins to snow");
                 break;
             case 3:
                  writeoutput("A Storm is Brewing");
                 break;
             case 254:
                 writeoutput("0xFE recived unknown weather");

             default:
                 writeoutput("Unknown weather: " + (weathertype & 0xFF));

         }

        }
        private void handleDrawGamePlayer(byte[] buffer) {
            byte[] gp = new byte[19];
             for (int i = 0; i < gp.length; i++)
			gp[i] = buffer[i];
            player.setserial(((gp[1] <<24) | (gp[2] <<16) | (gp[3] <<8) | (gp[4])));
            player.settype((gp[5] <<8) | (gp[6]) & 0xFF);
            // 7 is unknown
            player.hue = ((gp[8] <<8) | (gp[9]) & 0xFF);
            player.flags = gp[10];
            player.setX((gp[11] <<8) | (gp[12]) & 0xFF);
            player.setY((gp[13] <<8) | (gp[14]) & 0xFF);
            player.setZ(gp[18]);
            writeoutput("X: " + player.getX() + " Y: " + player.getY());

        }

        private void handleObjectInfo(byte[] buffer) {
            // This should be all correct
            // need to remove 8000000 from ID
            boolean myresult = true;
            int offset = 0;
            int direction = 0;
            int stack = 0;
            int size = ((buffer[1] & 0xFF) <<8) | (buffer[2] & 0xFF);
            byte[] incMobile = new byte[1];
            try { incMobile = new byte[size]; }
		catch(ArrayIndexOutOfBoundsException e)
		{ writeoutput("Error: Mobile data too long"); }
            for (int i = 0; i < incMobile.length; i++)
			incMobile[i] = buffer[i];
                        
             int itemid = ((incMobile[3] <<24) | (incMobile[4] <<16) | (incMobile[5] <<8) | (incMobile[6] & 0xFF));
             int type = (buffer[7] << 8) | (buffer[8] & 0xFF);
             if ((itemid & 0x80000000) == 0x80000000) {
                 stack = ((incMobile[9] <<8) | (incMobile[10] & 0xFF));
                 //int temp2 = (incMobile[4] & 0x7F);
                  itemid = (itemid & 0x7FFFFF);
                  // Removes the 8000000 if its found
             offset = offset + 2;
             }
             if ((type & 0x8000) == 0x8000) {
                 offset = offset + 1;
             }
            int itemx = ((incMobile[9 + offset] <<8) | (incMobile[10 + offset] & 0xFF));
            int temp = (incMobile[11 + offset] & 0xF);
             int itemy = ((temp << 8) | (incMobile[12 + offset] & 0xFF));
             
             if ((itemx & 0x8000) == 0x8000) {
                 direction = incMobile[13 + offset];
                 offset = offset + 1;
             }
             int itemz = incMobile[13 + offset];
             int hue = 0;
             if ((itemy & 0x8000) == 0x8000) {
                 hue = ((incMobile[14 + offset] <<8) | (incMobile[15 + offset] & 0xFF));
             offset = offset + 2;
             }
             int flags = 0;
             if ((itemy & 0x4000) == 0x4000) {
                 flags = incMobile[14 + offset];
             }
             drawdata = new UOObject(itemid,type,itemx,itemy,itemz,hue);
                for (int i = 0; i < Listitemsindex.size(); i++) {
                if (itemid == Listitemsindex.get(i)) {

                Listitems.set(i,drawdata);
                myresult = false;
                break;
                }
            }
             if (myresult) {
                 Listitemsindex.add(itemid);
                 Listitems.add(drawdata);
             }
             //System,out.println("Y: " (incMobile[]));
            writeoutput("Object Info ID: " + itemid + " type: " + type + "X: " + itemx + "Y: " + itemy + " Hue: " + hue + " Flags: " + flags);

        }
        public void printlist() {
            for (int i = 0; i < Listitemsindex.size(); i++) {
                writeoutput("ID:" + Listitemsindex.get(i) + "\n");
            }
        }
        private void handleDrawObject(byte[] buffer)
        {
            // Draw object really means draw mobile
            // May want to move mobiles to their own list? rather than item list
            boolean myresult = true;
            int size = ((buffer[1] & 0xFF) <<8) | (buffer[2] & 0xFF);
            byte[] incMobile = new byte[1];
            try { incMobile = new byte[size]; }
		catch(ArrayIndexOutOfBoundsException e)
		{ writeoutput("Error: Mobile data too long"); }
            for (int i = 0; i < incMobile.length; i++)
			incMobile[i] = buffer[i];
             int itemid = ((incMobile[3] <<24) | (incMobile[4] <<16) | (incMobile[5] <<8) | (incMobile[6]));
             int type = ((incMobile[7] <<8) | (incMobile[8] & 0xFF));
             int itemx = ((incMobile[9] <<8) | (incMobile[10] & 0xFF));
             int itemy = ((incMobile[11] <<8) | (incMobile[12] & 0xFF));
             int itemz = incMobile[13];
             int color = ((incMobile[14] <<8) | (incMobile[15]));
             // store the id in a 2nd array, if the id is found we update the data, if not we replace
            drawdata = new UOObject(itemid,type,itemx,itemy,itemz,color);
             for (int i = 0; i < Listitemsindex.size(); i++) {
                if (itemid == Listitemsindex.get(i)) {
                
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
            
            //writeoutput(incMobile[0] + " " + (incMobile[0] & 0xFF));
            
             writeoutput("Draw Object ID: " + itemid + " type: " + type + "X: " + itemx + "Y: " + itemy);
        }

	private void handleServerList(byte[] buffer)
	{
		try
		{
			String[] list = getServerList(buffer);
			packetOperator.processServerList(list);
		}
		catch (UOAccountException e)
		{
			packetOperator.error(e);
		}
	}

	private String[] getServerList(byte[] buffer)
	{
		String[] serverNames = new String[1];
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
		byte[] serverPacket = new byte[3];
		serverPacket[0] = pckSelectServer;
		serverPacket[1] = (byte)(server >>8);
		serverPacket[2] = (byte)server;

		write(serverPacket);
	}

	private void handleKickPacket(byte[] buffer)
	{
		writeoutput("Player Kicked");
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

		byte[] key = new byte[4]; //now copies bytes rather than try to decode the long
		key[0] = buffer[7];
		key[1] = buffer[8];
		key[2] = buffer[9];
		key[3] = buffer[10];
                mykey = key;
		return key;
	}

	private void sendKey(byte[] key)
	{
		byte[] keyPacket = new byte[65];
		keyPacket[0] = pckGameServerLogin;
		keyPacket[1] = key[0]; //copy key bytes out of array
		keyPacket[2] = key[1];
		keyPacket[3] = key[2];
		keyPacket[4] = key[3];

		char[] userAr = user.toCharArray();
		for (int i = 0; i < userAr.length; i++)
			keyPacket[i+5] = (byte)userAr[i];

		char[] passAr = pass.toCharArray();
		for (int i = 0; i < passAr.length; i++)
			keyPacket[i+35] = (byte)passAr[i];   
                    write(keyPacket);
		//write(crypted);
	}
	private void handleClientFeaturesPacket()
	{
		//do nothing
         
	}



	private void handleCharList(byte[] buffer)
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

	private String[] getCharList(byte[] buffer)
	{
		String[] charList = new String[1];
		int numOfChars = 0;
		if (buffer.length > 4) numOfChars = (buffer[3] & 0xFF);
		else { writeoutput("Char List Error"); System.exit(1); }
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
		byte[] charPacket = new byte[73];
		char[] charName = character.toCharArray();
		charPacket[0] = pckLoginChar;
		charPacket[1] = (byte)0xED;
		charPacket[2] = (byte)0xED;
		charPacket[3] = (byte)0xED;
		charPacket[4] = (byte)0xED;
		for (int i = 0; i < charName.length; i++)
			charPacket[i + 5] = (byte)charName[i];
		charPacket[68] = (byte)slot;
		byte[] localAddress = new byte[4];
		localAddress = (client.getLocalAddress()).getAddress();
		for (int i = 0; i < localAddress.length; i++)
			charPacket[i + 69] = localAddress[i];

		write(charPacket);
		sendClient("2.0.3");
	}

	private void sendClient(String client)
	{
		char[] clientAr = client.toCharArray();
		byte[] clientPacket = new byte[clientAr.length + 4];
		clientPacket[0] = pckClientVersion;
		clientPacket[1] = (byte)(clientPacket.length >>8);
		clientPacket[2] = (byte)clientPacket.length;
		for (int i = 0; i < clientAr.length; i++)
			clientPacket[i + 3] = (byte)clientAr[i];

		write(clientPacket);
	}

	public void say(String text)
	{
		byte[] sayPacket;
		if (text.equalsIgnoreCase("guards"))
		{
			byte[] guards = { (byte)0xAD, 0x00, 0x16, (byte)0xC0,
			0x00, 0x5A, 0x00, 0x03, 0x45, 0x4E, 0x55,
			0x00, 0x00, 0x10, 0x07, 0x67, 0x75, 0x61,
			0x72, 0x64, 0x73, 0x00 };
			sayPacket = guards;
		}
		else if (text.equalsIgnoreCase("bank"))
		{
			byte[] bank = { (byte)0xAD, 0x00, 0x14, (byte)0xC0,
			0x00, 0x5A, 0x00, 0x03, 0x45, 0x4E, 0x55,
			0x00, 0x00, 0x10, 0x02, 0x62, 0x61, 0x6E,
			0x6B, 0x00 };
			sayPacket = bank;
		}
		else
		{
			char[] textAr = text.toCharArray();
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
        public void resync() {
            byte[] sync = new byte[3];
            sync[0] = (byte)0x22;
            sync[1] = 0;
            sync[2] = 0;
            write(sync);
        }
        public void move(int x, int y, int prec) {
            Boolean result = true;
            int ld = 0;
            int ldc = 0;
            while(result) {
                int dx = player.getX() - x;
                if (dx < 0) {
                    dx = 0;
                }
                int dy = player.getY() - y;
                if (dy < 0) {
                    dy = 0;
                }
                if (dy > dx) {
                    dx = dy;
                }
                if (dx <= prec) {
                    return;
                }
                int mx = player.getX();
                int my = player.getY();
                dy = my - y;
                dx = mx - x;
                if (ld == dx) {
                    //begin
                    ldc = ldc + 1;
                    if (ldc > 100) {
                        writeoutput("Out of range");
                        return;
                    }
                    ld = dx;

                }
            }

        }
        private void handleCharMoveACK(byte[] buffer)
        {
            byte[] Moveack = new byte[3];
            for (int i = 0; i < Moveack.length; i++)
			Moveack[i] = buffer[i];
            int iSeq = Moveack[1];
            int iNot = Moveack[2];
            //if iSeq =
// Do nothing
        }
	public boolean walk(String direction, int numOfSteps)
	{
		byte[] walkPacket = new byte[7];
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
			writeoutput("Error In Walking Delay " + e);
		}
		return true;
	}

        public void GetPlayerStatus() {
            byte[] status = new byte[10];
            status[0] = CMSG_GetPlayerStatus;
            status[1] = (byte)0xED;
            status[2] = (byte)0xED;
            status[3] = (byte)0xED;
            status[4] = (byte)0xED;
            status[5] = (byte)0x04; // 0x05 is request skills? 0x3a
             byte[] temp = intToByteArray(player.getserial());
              status[6] = temp[0];
            status[7] = temp[1];
            status[8] = temp[2];
            status[9] = temp[3];
            write(status);
        }
        public void dropg(int itemid, int x, int y, int z) {
            byte[] drop = new byte[14];
            drop[0] = CMSG_DropItem;
            byte[] temp = intToByteArray(itemid);
            drop[1] = temp[0];
            drop[2] = temp[1];
            drop[3] = temp[2];
            drop[4] = temp[3];
            byte[] temp2 = intToByteArray2(x);
            drop[5] = temp2[0];
            drop[6] = temp2[1];
            byte[] temp3 = intToByteArray2(y);
            drop[7] = temp3[0];
            drop[8] = temp3[1];
            drop[9] = (byte)z;
            write(drop);
        }
        public void drag(int itemid, int stacksize) {
            byte[] drag = new byte[7];
            drag[0] = CMSG_PickUpItem;
            byte[] temp = intToByteArray(itemid);
            byte[] temp2 = intToByteArray2(stacksize);
            drag[1] = temp[0];
            drag[2] = temp[1];
            drag[3] = temp[2];
            drag[4] = temp[3];
            drag[5] = temp2[0];
            drag[6] = temp2[1];
            write(drag);

        }
        public void singleclick(int itemid) {
                       try {
                byte[] pckuse = new byte[5];
            pckuse[0] = CMSG_SingleClick;
            byte[] temp = intToByteArray(itemid);
            pckuse[1] = temp[0];
            pckuse[2] = temp[1];
            pckuse[3] = temp[2];
            pckuse[4] = temp[3];
            write(pckuse);
           }
           catch (Exception e){
               writeoutput("Single Click failed: " + e);
           }
        }
        public void useobject(int Itemid) {
           try {
                byte[] pckuse = new byte[5];
            pckuse[0] = CMSG_DoubleClick;
            byte[] temp = intToByteArray(Itemid);
            pckuse[1] = temp[0];
            pckuse[2] = temp[1];
            pckuse[3] = temp[2];
            pckuse[4] = temp[3];
            write(pckuse);
           }
           catch (Exception e){
               writeoutput("Use Object failed: " + e);
           }

        }
        public static byte[] intToByteArray2(int value) {
            return new byte[] {
                (byte)(value >> 8),
                (byte)(value)};
        }
        private static byte[] intToByteArray(int value) {
        return new byte[] {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value};

}       
	public void useSkill(String var1, String var2)
	{
                // This is broken needs to be fixed
		byte[] skillPacket = new byte[9];
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


	private void handleInitPlayer(byte[] buffer)
	{
		handleCharLoc(buffer);
		packetOperator.processLoggedIn();
	}

	private void handleCharLoc(byte[] buffer)
	{
		byte[] charLoc = new byte[37];
		for (int i = 0; i < charLoc.length; i++)
			charLoc[i] = buffer[i];
		player.setserial(((charLoc[1] <<24) | (charLoc[2] <<16) | (charLoc[3] <<8) | (charLoc[4])));
		player.settype((charLoc[9] <<8 | charLoc[10] &0xFF));
                player.setX(((charLoc[11] & 0xFF) <<8) | (charLoc[12] & 0xFF));
		player.setY(((charLoc[13] & 0xFF) <<8) | (charLoc[14] & 0xFF));
		player.setZ(((charLoc[15] & 0xFF) <<8) | (charLoc[16] & 0xFF));
                //writeoutput("Check if drawdata is in list" + Listitems.(drawdata.serial));
                //Listitems.add(Char);
            //UOObject currentobj = Listitems.firstElement();

            //writeoutput(Char);
packetOperator.processUpdatePlayer();
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
	private void handleChatPacketAE(byte[] buffer)
	{
		String chatMsg = "";
		//int size = (in.read() >>8) | in.read();
		int size = ((buffer[1] & 0xFF) <<8) | (buffer[2] & 0xFF);
		byte[] chat = new byte[1];
		try { chat = new byte[size]; }
		catch(ArrayIndexOutOfBoundsException e)
		{ writeoutput("Error: Message too long"); }
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

	private void handleChatPacket1C(byte[] buffer)
	{
		String chatMsg = "";
		int size = (buffer[1] >>8) | buffer[2];
		byte[] chat = new byte[size];
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

	private void handleUpdatePlayer(byte[] buffer)
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
                 drawdata = new UOObject(itemid,model,x,y,z,hue);
                for (int i = 0; i < Listitemsindex.size(); i++) {
                if (itemid == Listitemsindex.get(i)) {
               
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
                 packetOperator.processUpdatePlayer();
             }
writeoutput("Update Player ID: " + itemid + " type: " + model + "X: " + x + "Y: " + y + " Hue: " + hue + " Flags: " + flag);

               

		
	}

	public static String printPacket(char[] buffer)
	{
		byte[] byteBuffer = new byte[buffer.length];
		for (int i = 0; i < buffer.length; i++)
			byteBuffer[i] = (byte)buffer[i];
		return printPacket(byteBuffer);
	}

	public static String printPacket(byte[] buffer)
	{
		String packet = "";
		for (int i = 0; i < buffer.length; i++)
			packet = packet + (buffer[i] & 0xFF) + " ";
			//System.out.print((buffer[i] & 0xFF) + " ");
		packet = packet + "\n\n";
		//writeoutput("\n");
		for (int i = 0; i < buffer.length; i++)
			packet = packet + Long.toHexString(buffer[i] & 0xFF) + " ";
			//System.out.print(Long.toHexString(buffer[i] & 0xFF) + " ");
		packet = packet + "\n\n";
		//writeoutput("\n");
		for (int i = 0; i < buffer.length; i++)
			packet = packet + (char)buffer[i] + " ";
			//System.out.print((char)buffer[i] + " ");
		return packet;
		//writeoutput();
	}

	public static String printPacketHex(byte[] buffer)
	{
		String packet = new String();
		for (int i = 0; i < buffer.length; i++)
			packet = packet + Long.toHexString(buffer[i] & 0xFF) + " ";
		return packet;
	}

	public static byte[] charToByte(char[] charAr)
	{
		byte[] byteAr = new byte[charAr.length];
		for (int i = 0; i < charAr.length; i++)
			byteAr[i] = (byte)charAr[i];
		return byteAr;
	}

}
class twofishobj {
    public int sBox;
    public int subKeys;

}
   }

