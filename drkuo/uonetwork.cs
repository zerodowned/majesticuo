using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace drkuo
{
    class uonetwork
    {
        public String myoutput = "";
        public string myvars = "";
        public uoplayer player = new uoplayer();
        public uoobject drawdata; // rename?
        public UOopcodes opcode;

        private Socket mysocket;
        private StateObject mystate;

        HuffmanDecompression decomp;
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
            mystate.buffer = new byte[mystate.size];
            // Entry point for network thread
            Connect();
            Thread.Sleep(500);
            if (mysocket.Connected) { Login(); display("Connected!"); }
            while (mysocket.Connected)
            {
                if (mysocket.Available > 0)
                {
                    display("data avail");
                    mysocket.BeginReceive(mystate.buffer, 0, mystate.size, SocketFlags.None, PacketReceived, mystate);
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
            try
            {
                string cmd = Convert.ToString(newstate.buffer[0]);
                display("Received >> " + BitConverter.ToString(newstate.buffer));
            }
            
            catch
            { }

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

        public string PacketToHex(byte[] buffer)
        {
            string mystring = "";
            for (int i = 0; i <= (buffer.Length - 1); i++)
            {
                if (i > 80)
                {
                    return mystring;
                }
                mystring = mystring + Convert.ToString(buffer[i]) + " ";
            }
            return mystring;
        }

        public void display(String temp)
        {
            myoutput = myoutput + "\r\n" + temp;
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
        
    }

}
