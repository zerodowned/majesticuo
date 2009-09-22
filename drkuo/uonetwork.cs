using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections;

// C# to convert a byte array to a string.
//byte [] dBytes = ...
//string str;
//System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
//str = enc.GetString(dBytes);

namespace drkuo
{
    class uonetwork
    {
        private byte[] key = new byte[4];
        private Boolean bDecompress = false;
        public String myoutput = "";
        public string myvars = "";
        public uoplayer player = new uoplayer();
        public uoobject drawdata; // rename?
        //public UOopcodes opcode = new UOopcodes();

        private Socket mysocket;
        public StateObject mystate;

        HuffmanDecompression decomp = new HuffmanDecompression();
        private String ip;
        private int port;
        private String user;
        private String pass;
        private int charslot;
        private byte[] recbuffer;


        public uonetwork(String i, int p, String usera, String passa, int cslot)
        {
            //, uointerface uoi
            BinaryNode.CreateTree();
            ip = i;
            port = p;
            this.user = usera;
            this.pass = passa;
            charslot = (cslot - 1);
           // packops = uoi;


        }
        public void main()
        {
            mystate = new StateObject();
            //mystate.buffer = new byte[mystate.size];
            // Entry point for network thread
            Connect();
            Thread.Sleep(500);
            if (mysocket.Connected) { Login(); display("Connected!"); }
            while (mysocket.Connected)
            {
                if (mysocket.Available > 0)
                {
                    mystate.buffer = new byte[mysocket.Available];
                    mystate.decomp = new byte[mysocket.Available];
                    mystate.avail = mysocket.Available;
                    //display("data avail");
                    mysocket.BeginReceive(mystate.buffer, 0, mysocket.Available, SocketFlags.None, PacketReceived, mystate);
                    //mysocket.Receive(mystate.buffer);
                    

                    //maybe we shouldnt be looping this command?
                }
            }
            display("Connection Lost");
        }

        public void PacketReceived(IAsyncResult result)
        {
            mysocket.EndReceive(result);
            StateObject newstate = (StateObject)result.AsyncState;
            ArrayList incomingpackets = new ArrayList();
            incomingpackets.AddRange(newstate.buffer);
            byte[] outbuffer;

            while (incomingpackets.Count > 0)
            {
                if (bDecompress)
                {
                    outbuffer = new byte[incomingpackets.Count];
                    huffmanobject myhuf = new huffmanobject();
                    myhuf.buffer = (byte[])incomingpackets.ToArray(typeof(byte));
                    myhuf.src_size = incomingpackets.Count;
                    myhuf.out_size = 0;
                    myhuf = BinaryNode.drkDecompress(myhuf);

                    // store incoming packet in an arraylist
                    // decompress it etc, then xfer to byte[] and remove from list
                    // repeat
                    outbuffer = myhuf.output;
                    if (outbuffer.Length < 1) { incomingpackets.Clear(); break; }
                    if (myhuf.buffer.Length > outbuffer.Length)
                    {
                        display("consuming: " + myhuf.out_size + " bytes");
                        //for (int x = 0; x < outbuffer.Length; x++) { aOutput.add(temp[x]); } // outputs the packet
                        for (int x = 0; x < (myhuf.out_size + 1); x++) { incomingpackets.RemoveAt(0); } // removes it from queue
                    }else {
                        incomingpackets.Clear();
                    }

                }
       
                    else
                    {
                        outbuffer = newstate.buffer;
                        incomingpackets.Clear(); // << TEMP REMOVE AFTER
                    }

                    handlePackets(outbuffer);
                }
            }
      

        private void handlePackets(byte[] packetinfo)
        {
            
            try
            {
                int cmd = Convert.ToInt32(packetinfo[0]);
                string cmd2 = Convert.ToString(packetinfo[0]);
                display("Received >> " + BitConverter.ToString(packetinfo));
                switch(cmd)
                {
                    case UOopcodes.SMSG_GameServerList://0xA8:
                        handleGameServerList();
                        break;
                    case UOopcodes.SMSG_ConnectToGameServer: // decomp after this
                        handleConnectToGameServer(packetinfo);
                        bDecompress = true;
                        break;
                    case UOopcodes.SMSG_ClientFeatures://0xB9:
                        handleClientFeatures();
                        break;
                    case UOopcodes.SMSG_CharList://0xA9:
                        handleCharactersStartingLocations(packetinfo);
                        break;
                    case UOopcodes.SMSG_CharLocAndBody://0xA9:
                        handleCharLocAndBody(packetinfo);
                        break;
                    default:
                        display("UnknownPacket: " + cmd2);
                        break;
                }

            }
            
            catch
            { }
        }

        private void handleCharLocAndBody(byte[] buffer)
        {
            display("Handling Character Location and Body");
            byte[] charLoc = new byte[37];
		for (int i = 0; i < charLoc.Length; i++)
			charLoc[i] = buffer[i];
		player.serial = (((charLoc[1] <<24) | (charLoc[2] <<16) | (charLoc[3] <<8) | (charLoc[4])));
		player.type = ((charLoc[9] <<8 | charLoc[10] &0xFF));
                player.x = (((charLoc[11] & 0xFF) <<8) | (charLoc[12] & 0xFF));
		player.y = (((charLoc[13] & 0xFF) <<8) | (charLoc[14] & 0xFF));
		player.z = (((charLoc[15] & 0xFF) <<8) | (charLoc[16] & 0xFF));
        updatevars();
        }

        private void handleCharactersStartingLocations(byte[] buffer)
        {
            String[] charList = new String[1];
            String charName = "";
		int numOfChars = 0;
		if (buffer.Length > 4) numOfChars = (buffer[3] & 0xFF);
		else { display("Char List Error"); }
		charList = new String[numOfChars];
		for (int i = 0; i < numOfChars; i++)
		{
			//String charName = "";
			for (int j = 0; j < 30; j++)
				if ((buffer[(i * 60) + j + 4]) != 0x00)
					charName = charName + (char)buffer[(i * 60) + j + 4];
			charList[i] = charName;
		}
        chooseChar(charName, charslot);
        }

        public void chooseChar(String character, int slot)
	{
		byte[] charPacket = new byte[73];
		byte[] charName = GetBytes(character);
		charPacket[0] = UOopcodes.CMSG_LoginChar;
		charPacket[1] = (byte)0xED;
		charPacket[2] = (byte)0xED;
		charPacket[3] = (byte)0xED;
		charPacket[4] = (byte)0xED;
		for (int i = 0; i < charName.Length; i++)
			charPacket[i + 5] = (byte)charName[i];
		charPacket[68] = (byte)slot;
            // temp fix just sends 127.0.0.1
        byte[] localAddress = { 0x7F, 0x00, 0x00, 0x01 };//new byte[4];
        //localAddress = mysocket.LocalEndPoint;//(client.getLocalAddress()).getAddress();
		for (int i = 0; i < localAddress.Length; i++)
			charPacket[i + 69] = localAddress[i];

		Send(charPacket);
		sendClient("2.0.3");
	}

        private void sendClient(string p)
        {
            byte[] clientver = GetBytes(p);
            byte[] clientPacket = new byte[clientver.Length + 4];
            clientPacket[0] = UOopcodes.CMSG_ClientVersion;

		clientPacket[1] = (byte)(clientPacket.Length >>8);
		clientPacket[2] = (byte)clientPacket.Length;
		for (int i = 0; i < clientver.Length; i++)
			clientPacket[i + 3] = clientver[i];

		Send(clientPacket);
        }


        private void handleClientFeatures()
        {
            display("Client Features received");
            // todo display which features are enabled/disabled
            // bitflags are beyond me atm
        }

        private void handleConnectToGameServer(byte[] buffer)
        {
                    display("Handling Connect to Game Server");
        key[0] = buffer[7];
        key[1] = buffer[8];
        key[2] = buffer[9];
        key[3] = buffer[10];
        GameServerLogin();
        }

        private void GameServerLogin()
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x91;
            buffer[1] = key[0];
            buffer[2] = key[1];
            buffer[3] = key[2];
            buffer[4] = key[3];
            byte[] myuser = GetBytes(user);
            byte[] mypass = GetBytes(pass);

            for(int i = 0;i < (myuser.Length);i++)
            {
                buffer[i + 5] = myuser[i];
            }
            for (int i = 0; i < (mypass.Length); i++)
            {
                buffer[i + 35] = mypass[i];
            }
             Send(buffer);
        }
        private void handleGameServerList()
        {
            display("Handling GameServerList");
            byte[] mybyte = {0xA0, 0x0, 0x0};
            Send(mybyte);
        }
        public void Login()
        {
            byte[] firstbyte = { 0xf, 0x70, 0x0, 0x1 };
            Send(firstbyte);
            byte[] loginpack = new byte[62];
            byte[] myuser = Encoding.ASCII.GetBytes(user);
            byte[] mypass = Encoding.ASCII.GetBytes(pass);
            loginpack[0] = 0x80;

            for (int i = 0; i <= (myuser.Length - 1); i++)
            {
                loginpack[i + 1] = (byte)myuser[i];
            }
            for (int i = 0; i <= (myuser.Length - 1); i++)
            {
                loginpack[i + 31] = (byte)mypass[i];
            }
            loginpack[61] = 0xFE;
            Send(loginpack);
            //Sends user/pass and FE as the login key, can use any key
        }
        
        public static byte[] GetBytes(string text)
        {
            return ASCIIEncoding.UTF8.GetBytes(text);
        }
        public void Send(byte[] buffer)
        {
        //    StateObject newstate = new StateObject();
            //mysocket.BeginSend(newstate, 0, Data.Length, Sockets.SocketFlags.None, EndSend, newstate);
            //mysocket.BeginSend(buffer,0,buffer.Length,SocketFlags.None,EndSend,newstate);
            mysocket.Send(buffer);
            display("Sent >>" + BitConverter.ToString(buffer));
        }
        public void EndSend(IAsyncResult AR)
        {
            //int len;
            mysocket.EndSend(AR);
        }

        
        public void display(String temp)
        {
            myoutput = myoutput + "\r\n" + temp;
        }
        public void updatevars()
        {
            myvars = ("Player ID: " + player.serial + "\r\nPlayer Type: " + player.type + "\r\nPlayer X: " + player.x + " \r\nPlayer Y: " + player.y);
        }
        public void Connect()
        {    
            IPAddress serverip = IPAddress.Parse(ip);
            try
            {
                mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                mysocket.Connect(new IPEndPoint(serverip,port));
                display("Connecting...");
            }
             catch
            {
                display("Connection Failed");
            }
        }

    }


   public class StateObject
    {
       public Socket socket;
       public int size = 4096;
       public byte[] buffer;
       public byte[] decomp;
       public int avail;
    }

}
