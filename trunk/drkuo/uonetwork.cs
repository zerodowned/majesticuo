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
        private byte[] recbuffer;


        public uonetwork(String i, int p, String usera, String passa)
        {
            //, uointerface uoi
           // BinaryNode.CreateTree();
            ip = i;
            port = p;
            this.user = usera;
            this.pass = passa;
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
            int refsize = 0;
            while (newstate.avail > 0)
            {
                if (bDecompress)
                {
                    decomp.DecompressOnePacket(ref newstate.buffer, newstate.buffer.Length, ref newstate.decomp, ref refsize);
                    // store incoming packet in an arraylist
                    // decompress it etc, then xfer to byte[] and remove from list
                    // repeat
                    outbuffer = newstate.decomp;
                }
                else
                {
                    outbuffer = newstate.buffer;
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
                    default:
                        Console.WriteLine("Default case");
                        break;
                }

            }
            
            catch
            { }
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
            myvars = myvars + "omg vars :o";
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
