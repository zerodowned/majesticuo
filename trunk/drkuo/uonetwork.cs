/*
Written by James Kidd.
 * 
 * ToDo, Replace arraylist packet handler with stream.
 * Add packet size etc and check complete packet is recieved, rather than hoping
 * Better way than beginreceive?? breaking packets? maybe its splitting them
 * 
 * Possible fix is add every packet and its size to a list, then when packets come in,
 * check opcode against size, unknown opcodes etc would be left in a buffer
 * to wait for more data.
*/
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
using Ultima;
using System.Text.RegularExpressions;

// C# to convert a byte array to a string.
//byte [] dBytes = ...
//string str;
//System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
//str = enc.GetString(dBytes);

namespace drkuo
{
    class uonetwork
    {
        private Network.LoginEncryption LoginEncryption = new Network.LoginEncryption();
        private Encryption.TwoFish Twofish = new Encryption.TwoFish();

        public ArrayList Journal = new ArrayList();
        private Boolean bDebug = true;
        public int seq = 0; // walk sequence
        private static StringList clioclist = new StringList("ENU");
        public uoplayer player = new uoplayer();
        public uoclientvars UOClient = new uoclientvars();
        public Hashtable GameObjects = new Hashtable();
        public Hashtable GumpList = new Hashtable();
        public Boolean bLoginCrypt = false;
        public Boolean bTwofishcrypt = false;

        public bool bConnected = false;
        public String myoutput = "";
        public string myvars = "";

        private byte[] key = new byte[4];
        private uint loginkeyint = 0xFFFFFFFF;//167837955;//16820416;
        private uint twofishkey = 0x7f000001;//167837955;//16820416;
        private byte[] loginkey = new byte[4];
        private string Version = "3.00.00";
        
        private Boolean bDecompress = false;
        private Socket mysocket;
        private StateObject mystate;
        private String ip;
        private int port;
        private String user;
        private String pass;
        private int charslot;


        public uonetwork(String i, int p, String usera, String passa, int cslot)
        {
            BinaryNode.CreateTree();
            ip = i;
            port = p;
            this.user = usera;
            this.pass = passa;
            charslot = (cslot - 1);
        }
        public void main()
        {
            
            mystate = new StateObject();
            // Entry point for network thread
            Connect();
            Thread.Sleep(500);
            if (mysocket.Connected) { Login(); display("Connected!"); bConnected = true; }
            mysocket.DontFragment = true;
            
            //NetworkStream stream = new NetworkStream(mysocket);
            while (mysocket.Connected)
            {
                Thread.Sleep(1);
                //if (stream.DataAvailable)
                //{
                //    StateObject state = new StateObject();
                //    state.buffer = new byte[2048];
               //    stream.BeginRead(state.buffer, 0, state.buffer.Length, PacketReceived,state);
               // }
                if (mysocket.Available > 0)
                {
                    int mytempavail = mysocket.Available;
                    mystate.buffer = new byte[mytempavail];
                    mystate.decomp = new byte[mytempavail];
                    mystate.avail = mytempavail;
                    //display("data avail");
                    // line below used to use mysocket.Available, think this is safer
                    mysocket.BeginReceive(mystate.buffer, 0, mystate.avail, SocketFlags.None, PacketReceived, mystate);
                }
            }
            //display("Connection Lost");
        }

        public void PacketReceived(IAsyncResult result)
        {
            mysocket.EndReceive(result);
            StateObject newstate = (StateObject)result.AsyncState;
            ArrayList incomingpackets = new ArrayList();
            incomingpackets.AddRange(newstate.buffer);
            byte[] outbuffer;
            // store incoming packet in an arraylist
            // decompress it etc, then xfer to byte[] and remove from list
            // repeat
            while (incomingpackets.Count > 0)
            {
                if (bDecompress)
                {
                    HuffmanDecompression huff = new HuffmanDecompression();
           
                    outbuffer = new byte[incomingpackets.Count];
                    huffmanobject myhuf = new huffmanobject();
                    myhuf.buffer = (byte[])incomingpackets.ToArray(typeof(byte));
                    myhuf.src_size = incomingpackets.Count;
                    myhuf.out_size = 0;
                    //myhuf.output = BinaryNode.Decompress(myhuf.buffer);
                    myhuf = BinaryNode.drkDecompress(myhuf);
                    outbuffer = myhuf.output;
                    //myhuf.out_size = outbuffer.Length;
    
                    if (outbuffer.Length < 1) { incomingpackets.Clear(); break; }
                    if (myhuf.buffer.Length > outbuffer.Length)
                    {
                        if (bDebug) { display("consuming: " + myhuf.out_size + " bytes"); }
                        for (int x = 0; x < (myhuf.out_size + 1); x++) { incomingpackets.RemoveAt(0); } // removes it from queue
                    }else {
                        incomingpackets.Clear();
                    }

                }
                    else
                    {
                        // Wipe the buffer as pre decompression packets dont stack up.
                        outbuffer = newstate.buffer;
                        incomingpackets.Clear();
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
                if (bDebug) { display("Received >> " + BitConverter.ToString(packetinfo)); }
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
                    case UOopcodes.SMSG_DrawGamePlayer://0x20
                        handleDrawGamePlayer(packetinfo);
                        break;
                    case UOopcodes.SMSG_StatusBarInfo://0x11
                        handleStatusBarInfo(packetinfo);
                        break;
                    case UOopcodes.SMSG_DrawObject://0x78
                        handleDrawObject(packetinfo);
                        break;
                    case UOopcodes.SMSG_UpdatePlayer://0x77
                        handleUpdatePlayer(packetinfo);
                        break;
                    case UOopcodes.SMSG_ObjectInfo://0x77
                        handleObjectInfo(packetinfo);
                        break;
                    case UOopcodes.SMSG_SetWeather:
                        handleSetWeather(packetinfo);
                        break;
                    case UOopcodes.SMSG_KickPlayer:
                        handleKickPlayer();
                        break;
                    case UOopcodes.SMSG_Deleteobject:
                        handleDeleteObject(packetinfo);
                        break;
                    case UOopcodes.SMSG_AddItemToContainer:
                        handleAddItemToContainer(packetinfo);
                        break;
                    case UOopcodes.SMSG_AddmultipleItemsInContainer:
                        handleAddMultipleItemsInContainer(packetinfo);
                        break;
                    case UOopcodes.SMSG_AllowRefuseAttack:
                        handleAllowRefuseAttack(packetinfo);
                        break;
                    case UOopcodes.SMSG_AttackOK:
                        handleAttackOK(packetinfo);
                        break;
                    case UOopcodes.SMSG_Blood:
                        handleBlood(packetinfo);
                        break;
                    case UOopcodes.SMSG_CharacterAnimation:
                        handleCharAnimation(packetinfo);
                        break;
                    case UOopcodes.SMSG_CharMoveRejection:
                        handleCharMoveRejection(packetinfo);
                        break;
                    case UOopcodes.SMSG_CliocMessage:
                        handleCliocMessage(packetinfo);
                        break;
                    case UOopcodes.SMSG_Damage:
                        handleDamage(packetinfo);
                        break;
                    case UOopcodes.SMSG_DraggingOfItem:
                        handleDraggingOfItem(packetinfo);
                        break;
                    case UOopcodes.SMSG_DrawContainer:
                        handleDrawContainer(packetinfo);
                        break;
                    case UOopcodes.SMSG_DropItemApproved:
                        handleDropItemApproved(packetinfo);
                        break;
                    case UOopcodes.SMSG_LoginDenied:
                        handleLoginDenied(packetinfo);
                        break;
                    case UOopcodes.SMSG_FightOccuring:
                        handleFightOccuring(packetinfo);
                        break;
                    case UOopcodes.SMSG_GeneralInformation:
                        handleGeneralInformation(packetinfo);
                        break;
                    case UOopcodes.SMSG_GraphicalEffect:
                        handleGraphicalEffect(packetinfo);
                        break;
                    case UOopcodes.SMSG_GumpTextEntryDialog:
                        handleGumpTextEntryDialog(packetinfo);
                        break;
                    case UOopcodes.SMSG_IdleWarning:
                        handleIdleWarning(packetinfo);
                        break;
                    case UOopcodes.SMSG_MobAttribute:
                        handleMobAttribute(packetinfo);
                        break;
                    case UOopcodes.SMSG_OpenBuyWindow:
                        handleOpenBuyWindow(packetinfo);
                        break;
                    case UOopcodes.SMSG_OpenDialogBox:
                        handleOpenDialogBox(packetinfo);
                        break;
                    case UOopcodes.SMSG_OpenPaperdoll:
                        handleOpenPaperDoll(packetinfo);
                        break;
                    case UOopcodes.SMSG_OverallLightLevel:
                        handleOverallLightLevel(packetinfo);
                        break;
                    case UOopcodes.SMSG_PersonalLightLevel:
                        handlePersonalLightLevel(packetinfo);
                        break;
                    case UOopcodes.SMSG_PlaySoundEffect:
                        handlePlaySoundEffect(packetinfo);
                        break;
                    case UOopcodes.SMSG_RejectMoveItemRequest:
                        handleRejectMoveItemRequest(packetinfo);
                        break;
                    case UOopcodes.SMSG_SellList:
                        handleSellList(packetinfo);
                        break;
                    case UOopcodes.SMSG_SendGumpMenuDialog:
                        handleSendGumpMenuDialog(packetinfo);
                        break;
                    case UOopcodes.SMSG_ServerChat:
                        handleServerChat(packetinfo);
                        break;
                    case UOopcodes.MSG_SendSpeach:
                        handleSendSpeach(packetinfo);
                        break;
                    case UOopcodes.SMSG_UpdateCurrentHealth:
                        handleUpdateCurrentHealth(packetinfo);
                        break;
                    case UOopcodes.SMSG_UpdateCurrentMana:
                        handleUpdateCurrentMana(packetinfo);
                        break;
                    case UOopcodes.SMSG_UpdateCurrentStamina:
                        handleUpdateCurrentStamina(packetinfo);
                        break;
                    case UOopcodes.SMSG_WornItem:
                        handleWornItem(packetinfo);
                        break;
                    case UOopcodes.SMSG_Time:
                        handleTime(packetinfo);
                        break;
                    case UOopcodes.SMSG_SEintroducedRevision:
                        handleSEIntroducedRevision(packetinfo);
                        break;
                    case UOopcodes.SMSG_Seasonalinformation:
                        handleSeasonalInformation(packetinfo);
                        break;
                    case UOopcodes.MSG_ClientVersion:
                        handleClientVersion(packetinfo);
                        break;
                    case UOopcodes.MSG_RequestWarMode:
                        handleRequestWarMode(packetinfo);
                        break;
                    case UOopcodes.MSG_CharMoveACK:
                        handleCharMoveACK(packetinfo);
                        break;
                    case UOopcodes.MSG_TargetCursorCommands:
                        handleTargetCursorCommands(packetinfo);
                        break;
                    default:
                        display("UnknownPacket: " + BitConverter.ToString(packetinfo));
                        break;
                }

            }
            
            catch
            { }
        }

        private void handleLoginDenied(byte[] packetinfo)
        {
            Dissconnect();
            display("Connect failed");
        }

        private void handleTargetCursorCommands(byte[] packetinfo)
        {
            UOClient.CursorID = ((packetinfo[2] << 24) | (packetinfo[3] << 16) | (packetinfo[4] << 8) | (packetinfo[5]));
            UOClient.CursorTarget = packetinfo[1];
            UOClient.TargCurs = 1;
        }
        public byte[] ClickTargetPacket(int ID, int x, int y, int z, int Type, CursorTarget target)
        {
            byte[] packet = new byte[19];
            packet[0] = UOopcodes.MSG_TargetCursorCommands;
            packet[1] = (byte)target;
            byte[] temp = intToByteArray(UOClient.CursorID);
            packet[2] = temp[0];
            packet[3] = temp[1];
            packet[4] = temp[2];
            packet[5] = temp[3];
            packet[6] = 0x00; // maybe this should be setable?
            temp = new byte[4];
            temp = intToByteArray(ID);
            packet[7] = temp[0];
            packet[8] = temp[1];
            packet[9] = temp[2];
            packet[10] = temp[3];
            packet[11] = (byte)(x >> 8);
            packet[12] = (byte)x;
            packet[13] = (byte)(y >> 8);
            packet[14] = (byte)y;
            packet[15] = 0x00;
            packet[16] = (byte)z;
            packet[17] = (byte)(Type >> 8);
            packet[18] = (byte)Type;

            return packet;
            
        }
        public byte[] MoveRequestPacket(Direction direction, int sequence, int fastWalkPreventionKey, Boolean Run)
        {
            int dir = (int)direction;

            switch (dir)
            {
                case 0x00://North
                    player.tempx = player.CharPosX;
                    player.tempy = player.CharPosY - 1;
                    player.seq = sequence;
                    break;
                case 0x01://North East
                    player.tempx = player.CharPosX + 1;
                    player.tempy = player.CharPosY - 1;
                    player.seq = sequence;
                    break;
                case 0x02://East
                    player.tempx = player.CharPosX + 1;
                    player.tempy = player.CharPosY;
                    player.seq = sequence;
                    break;
                case 0x03://SouthEast
                    player.tempx = player.CharPosX + 1;
                    player.tempy = player.CharPosY + 1;
                    player.seq = sequence;
                    break;
                case 0x04://South
                    player.tempx = player.CharPosX;
                    player.tempy = player.CharPosY + 1;
                    player.seq = sequence;
                    break;
                case 0x05://SouthWest
                    player.tempx = player.CharPosX - 1;
                    player.tempy = player.CharPosY + 1;
                    player.seq = sequence;
                    break;
                case 0x06://West
                    player.tempx = player.CharPosX - 1;
                    player.tempy = player.CharPosY;
                    player.seq = sequence;
                    break;
                case 0x07://NorthWest
                    player.tempx = player.CharPosX - 1;
                    player.tempy = player.CharPosY - 1;
                    player.seq = sequence;
                    break;
                default:
                    break;
            }
            if (Run) { dir = (dir | 0x80); }// makes us run not walk
            byte[] packet = new byte[7];
            packet[0] = UOopcodes.CMSG_MoveRequest;
            packet[1] = (byte)dir;
            packet[2] = (byte)sequence;
            byte[] temp = uonetwork.intToByteArray(fastWalkPreventionKey);
            try
            {
                packet[3] = temp[0];
                packet[4] = temp[1];
                packet[5] = temp[2];
                packet[6] = temp[3];
            }
            catch { }
            return packet;
        }
        private void handleCharMoveACK(byte[] packetinfo)
        {
            display("seq: " + Convert.ToString(seq));
            seq = packetinfo[1];
            if (seq == player.seq) { player.CharPosX = player.tempx; player.CharPosY = player.tempy; }
            // if the received seq is the same as the sent, update our x/y
            // allow client to move 3-5 tiles in advance, this means storing 3-5 seq's and their x/y
            // 190ms is optimal time between steps
            updatevars();
        }
        private void handleCharMoveRejection(byte[] packetinfo)
        {
            if (packetinfo.Length != 8) { display("Char move reject packet len wrong"); return; }
            seq = packetinfo[1];
            player.CharPosX = (packetinfo[2] << 8) | (packetinfo[3]);
            player.CharPosY = (packetinfo[4] << 8) | (packetinfo[5]);
            player.Direction = packetinfo[6];
            player.CharPosZ = packetinfo[7];
            updatevars();
        }
        private void handleSendSpeach(byte[] buffer)
        {       
		    String chatMsg = "";
		    int size = (buffer[1] >>8) | buffer[2];
		    byte[] chat = new byte[size];
		    for (int i = 0; i < chat.Length; i++)
			    chat[i] = buffer[i];

		    String name = "";
		    String msg = "";
		    for (int i = 14; i < 44; i++)
			    if (chat[i] != 0x00) name = name + (char)chat[i];
		    for (int i = 44; i < chat.Length; i++)
			    if (chat[i] != 0x00) msg = msg + (char)chat[i];
		    chatMsg = name + ": " + msg;
            Journal.Insert(0,chatMsg);
		    display(chatMsg);
        }

        private void handleRequestWarMode(byte[] packetinfo)
        {
            if (packetinfo[1] == 0x01)
            {
                player.WarMode = true;
            }
            else
            {
                player.WarMode = false;
            }
        }

        private void handleClientVersion(byte[] packetinfo)
        {
            sendClient("2.0.3");
        }

        private void handleSeasonalInformation(byte[] packetinfo)
        {
            if (packetinfo.Length != 3) { display("Incorrect seasonalInfo packet len"); return; }
            int seasonflag = (int)packetinfo[1];
            switch(seasonflag)
            {
                case 0:
                    display("Its Spring");
                    break;
                case 1:
                    display("Its Summer");
                    break;
                case 2:
                    display("Its Fall");
                    break;
                case 3:
                    display("Its Winter");
                    break;
                case 4:
                    display("Its Desolate");
                    break;
            }
        }

        private void handleTime(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleSEIntroducedRevision(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleGeneralInformation(byte[] packetinfo)
        {
            int len = ((packetinfo[1] << 8) | packetinfo[2]);
            if (len != packetinfo.Length)
            {
                display("Bad length on G info packet");
                return;
            }
            int subcommand = ((packetinfo[3] << 8) | packetinfo[4]);
            switch (subcommand)
            {
                case 1:// Init fast walk prevention

                    break;
                case 2://Add key to fast walk stack

                    break;
                case 4://Close Generic Gump

                    break;
                case 5://Screen size

                    break;
                case 8: //Cursor color to get map 
                    player.Facet = (Facet)packetinfo[5];
                    // 0 =  fel 1 = tram 2 = islh 3=malas 4=tokuno
                    break;


            }
        }

        private void handleWornItem(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleUpdateCurrentStamina(byte[] packetinfo)
        {
            if (packetinfo.Length != 9) { display("Incorrect UpdateStam packet len"); return; }
            player.MaxStam = ((packetinfo[5] << 8) | packetinfo[6]);
            player.Stamina = ((packetinfo[7] << 8) | packetinfo[8]);
            updatevars();
        }

        private void handleUpdateCurrentMana(byte[] packetinfo)
        {
            if (packetinfo.Length != 9) { display("Incorrect UpdateMP packet len"); return; }
            player.MaxMana = ((packetinfo[5] << 8) | packetinfo[6]);
            player.Mana = ((packetinfo[7] << 8) | packetinfo[8]);
            updatevars();
        }

        private void handleUpdateCurrentHealth(byte[] packetinfo)
        {
            if (packetinfo.Length != 9) { display("Incorrect UpdateHP packet len"); return; }
            player.MaxHits = ((packetinfo[5] <<8) | packetinfo[6]);
            player.Hits = ((packetinfo[7] <<8) | packetinfo[8]);
            updatevars();
        }

        private void handleServerChat(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleSendGumpMenuDialog(byte[] packetinfo)
        {
            gump mygump = new gump();

            mygump.ID = ((packetinfo[3] << 24) | (packetinfo[4] << 16) | (packetinfo[5] << 8) | (packetinfo[6]));
            mygump.GumpID = ((packetinfo[7] << 24) | (packetinfo[8] << 16) | (packetinfo[9] << 8) | (packetinfo[10]));
            mygump.X = ((packetinfo[11] << 24) | (packetinfo[12] << 16) | (packetinfo[13] << 8) | (packetinfo[14]));
            mygump.Y = ((packetinfo[15] << 24) | (packetinfo[16] << 16) | (packetinfo[17] << 8) | (packetinfo[18]));
            int cmdlen = ((packetinfo[19] << 8) | (packetinfo[20]));
            mygump.Commands = "";// Add this
            int txtlines = ((packetinfo[21+cmdlen] << 8) | (packetinfo[22+cmdlen]));
            int textlen = ((packetinfo[23+cmdlen] << 8) | (packetinfo[24+cmdlen]));
            byte[] textt = new byte[textlen];
            for(int i = 0;i==textlen;i++) {
                textt[i] = packetinfo[25+cmdlen];
            }
            mygump.Text = GetString(textt);
            GumpList.Add(mygump.GumpID,mygump);
        }

        private void handleSellList(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleRejectMoveItemRequest(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handlePlaySoundEffect(byte[] packetinfo)
        {
            //display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handlePersonalLightLevel(byte[] packetinfo)
        {
           // display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleOverallLightLevel(byte[] packetinfo)
        {
            //display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleOpenPaperDoll(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleOpenDialogBox(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleOpenBuyWindow(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleMovePlayer(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleMobAttribute(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleIdleWarning(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleGumpTextEntryDialog(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleGraphicalEffect(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleFightOccuring(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleDropItemApproved(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleDrawContainer(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleDraggingOfItem(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleDamage(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleCliocMessage(byte[] packetinfo)
        {
            int cliocmsg = (( packetinfo[14] <<24) | ( packetinfo[15] <<16) | ( packetinfo[16] <<8) | ( packetinfo[17]));
            byte[] speaker = new byte[6];
            for (int i = 0; i < speaker.Length; i++)
            {
                speaker[i] = packetinfo[i + 18];
            }
            string myspeaker = GetString(speaker);// need to remove trailing spaces
            String msg = TrimString(myspeaker) + ": " + clioclist.Table[cliocmsg];
            Journal.Insert(0, msg);
            display(msg);
        }


        public string TrimString(string str)
        {
            try
            {
                string pattern = @"^[ \t]+|[ \t]+$";
                Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
                str = reg.Replace(str, "");
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void handleCharAnimation(byte[] packetinfo)
        {
            //display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleBlood(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleAttackOK(byte[] packetinfo)
        {
            UOClient.Combat = true;
            UOClient.EnemyID = ((packetinfo[1] << 24) | (packetinfo[2] << 16) | (packetinfo[3] << 8) | (packetinfo[4]));

        }

        private void handleAllowRefuseAttack(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleAddMultipleItemsInContainer(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }

        private void handleAddItemToContainer(byte[] packetinfo)
        {
            display("UnknownPacket: " + BitConverter.ToString(packetinfo));
        }
         private void handleDeleteObject(byte[] myobj) {
            if (myobj.Length != 5)
            {
                display("Packet length wrong for delete object");
                return;
            }
            int serial = (( myobj[1] <<24) | ( myobj[2] <<16) | ( myobj[3] <<8) | ( myobj[4]));
             if (GameObjects.ContainsKey(serial))
             {
                 GameObjects.Remove(serial);
             }
             else
             {
                 display("Object delete ignored, not found");
             }



        }

        private void handleKickPlayer()
        {
            display("Player Kicked");
            Dissconnect();

        }
        public void Dissconnect()
        {
            bConnected = false;
            if (mysocket.Connected)
            {
                mysocket.Disconnect(true);
            }
            display("Dissconnected!");
            
        }
        private void handleSetWeather(byte[] weather)
        {
         int weathertype = weather[1];
         //int weathernum = weather[2];

         switch(weathertype & 0xFF)
         {
             case 0:
                 display("It begins to rain");
                 break;
             case 1:
                  display("A Fierce Storm!");
                 break;
             case 2:
                  display("It begins to snow");
                 break;
             case 3:
                 display("A Storm is Brewing");
                 break;
             case 254:
                 display("0xFE recived unknown weather");
                 break;
             default:
                 display("Unknown weather: " + (weathertype & 0xFF));
                 break;
         }
        }


       private void handleObjectInfo(byte[] incMobile)
       {
            // This should be all correct
            // need to remove 8000000 from ID
            uoobject tmpob = new uoobject();
            int offset = 0;
            int direction = 0;                                   
             tmpob.serial = ((incMobile[3] <<24) | (incMobile[4] <<16) | (incMobile[5] <<8) | (incMobile[6] & 0xFF));
             tmpob.type = (incMobile[7] << 8) | (incMobile[8] & 0xFF);
             if ((tmpob.serial & 0x80000000) == 0x80000000)
             {
                 tmpob.stack = ((incMobile[9] <<8) | (incMobile[10] & 0xFF));
                 //int temp2 = (incMobile[4] & 0x7F);
                  tmpob.serial = (tmpob.serial & 0x7FFFFF);
                  // Removes the 8000000 if its found
             offset = offset + 2;
             }
             if ((tmpob.type & 0x8000) == 0x8000) {
                 offset = offset + 1;
             }
            
            tmpob.x = ((incMobile[9 + offset] <<8) | (incMobile[10 + offset] & 0xFF));
            tmpob.x = (tmpob.x & 0x7FFF);
            int temp = (incMobile[11 + offset] & 0xF);
             tmpob.y = ((temp << 8) | (incMobile[12 + offset] & 0xFF));

             if ((tmpob.x & 0x8000) == 0x8000)
             {
                 direction = incMobile[13 + offset];
                 offset = offset + 1;
             }
             tmpob.z = incMobile[13 + offset];
             if ((tmpob.y & 0x8000) == 0x8000)
             {
                 tmpob.color = ((incMobile[14 + offset] <<8) | (incMobile[15 + offset] & 0xFF));
             offset = offset + 2;
             }
             if ((tmpob.y & 0x4000) == 0x4000)
             {
                 tmpob.flags = incMobile[14 + offset];
             }
             if (GameObjects.ContainsKey(tmpob.serial))
             {
                 GameObjects.Remove(tmpob.serial);
                 GameObjects.Add(tmpob.serial, tmpob);
             }
             else
             {
                 GameObjects.Add(tmpob.serial, tmpob);
             }
           display("Object Info ID: " + tmpob.serial + " Type: " + tmpob.type + "X: " + tmpob.x + "Y: " + tmpob.y);

        }

        private void handleUpdatePlayer(byte[] buffer)
	{
            uoobject tmpob = new uoobject();
         tmpob.serial = (buffer[4] & 0xFF) | ((buffer[3] & 0xFF) << 8) | ((buffer[2] & 0xFF) << 16) | ((buffer[1] & 0xFF) << 24);
		tmpob.type = (buffer[6] & 0xFF) | ((buffer[5] & 0xFF) << 8);
		tmpob.x = (buffer[8] & 0xFF) | ((buffer[7] & 0xFF) << 8);
		tmpob.y = (buffer[10] & 0xFF) | ((buffer[9] & 0xFF) << 8);
		tmpob.z = (buffer[11] & 0xFF);
		int direction = (buffer[12] & 0xFF);
		tmpob.color = (buffer[14] & 0xFF) | ((buffer[13] & 0xFF) << 8);
		int flag = (buffer[15] & 0xFF);
		int highlightColor = (buffer[16] & 0xFF);

        if (GameObjects.ContainsKey(tmpob.serial))
            {
                GameObjects.Remove(tmpob.serial);
                GameObjects.Add(tmpob.serial, tmpob);
            }
            else
            {
                GameObjects.Add(tmpob.serial, tmpob);
            }
                if (tmpob.serial == player.CharID)
                {
                    player.CharPosX = tmpob.x;
                    player.CharPosY = tmpob.y;
                    player.CharPosZ = tmpob.z;
                }
                display("Update Player ID: " + tmpob.serial + " type: " + tmpob.type + "X: " + tmpob.x + "Y: " + tmpob.y);
	
	}

        private void handleDrawObject(byte[] incMobile)
        {
            // Draw object really means draw mobile
            // May want to move mobiles to their own list? rather than item list
            uoobject myobject = new uoobject();
             myobject.serial = ((incMobile[3] <<24) | (incMobile[4] <<16) | (incMobile[5] <<8) | (incMobile[6]));
             myobject.type = ((incMobile[7] <<8) | (incMobile[8] & 0xFF));
             myobject.x = ((incMobile[9] <<8) | (incMobile[10] & 0xFF));
             myobject.y = ((incMobile[11] <<8) | (incMobile[12] & 0xFF));
             myobject.z = incMobile[13];
             myobject.color = ((incMobile[14] <<8) | (incMobile[15]));
            
             // store the id in a 2nd array, if the id is found we update the data, if not we replace
             
            if(GameObjects.ContainsKey(myobject.serial))
            {
                GameObjects.Remove(myobject.serial);
                GameObjects.Add(myobject.serial,myobject);
            }
            else
            {
                GameObjects.Add(myobject.serial,myobject);
            }
             display("Drawn Object ID: " + myobject.serial + " type: " + myobject.type + "X: " + myobject.x + "Y: " + myobject.y);
        }


        private void handleStatusBarInfo(byte[] status)
        {
            display("Handling Status Bar Info");
            player.CharID = (((status[3] << 24) | (status[4] << 16) | (status[5] << 8) | (status[6])));
            byte[] myname = new byte[30];
            for (int i = 0; i < myname.Length; i++)
            {
                myname[i] = status[i + 7];
            }
            player.CharName = GetString(myname);// need to remove trailing spaces
            player.Hits = ((status[37] << 8) | (status[38] & 0xFF));
            player.MaxHits = ((status[39] << 8) | (status[40] & 0xFF));
            int namehchange = status[41];
            int statusflag = status[42];

            player.Sex = status[43];
            player.Str = ((status[44] << 8) | (status[45] & 0xFF));
            player.Dex = ((status[46] << 8) | (status[47] & 0xFF));
            player.Int = ((status[48] << 8) | (status[49] & 0xFF));
            player.Stamina = ((status[50] << 8) | (status[51] & 0xFF));
            player.MaxStam = ((status[52] << 8) | (status[53] & 0xFF));
            player.Mana = ((status[54] << 8) | (status[55] & 0xFF));
            player.MaxMana = ((status[56] << 8) | (status[57] & 0xFF));
            player.Gold = ((status[58] << 8) | (status[59] & 0xFF));
            int armor = ((status[60] << 8) | (status[61] & 0xFF));
            player.Weight = ((status[62] << 8) | (status[63] & 0xFF));
            if (statusflag > 4)
            {
                player.MaxWeight = ((status[64] << 8) | (status[65] & 0xFF));
            }
            updatevars();
        }
        
        private void handleDrawGamePlayer(byte[] packetinfo)
        {
            display("Handling Draw Game Player");
            player.CharID = (((packetinfo[1] <<24) | (packetinfo[2] <<16) | (packetinfo[3] <<8) | (packetinfo[4])));
            player.CharType = ((packetinfo[5] <<8) | (packetinfo[6]) & 0xFF);
            // 7 is unknown
            player.hue = ((packetinfo[8] << 8) | (packetinfo[9]) & 0xFF);
            player.flags = packetinfo[10];
            player.CharPosX = ((packetinfo[11] <<8) | (packetinfo[12]) & 0xFF);
            player.CharPosY = ((packetinfo[13] <<8) | (packetinfo[14]) & 0xFF);
            player.CharPosZ = (packetinfo[18]);
            updatevars();
        }

        private void handleCharLocAndBody(byte[] buffer)
        {
            display("Handling Character Location and Body");
            byte[] charLoc = new byte[37];
		for (int i = 0; i < charLoc.Length; i++)
			charLoc[i] = buffer[i];
		player.CharID = (((charLoc[1] <<24) | (charLoc[2] <<16) | (charLoc[3] <<8) | (charLoc[4])));
		player.CharType = ((charLoc[9] <<8 | charLoc[10] &0xFF));
                player.CharPosX = (((charLoc[11] & 0xFF) <<8) | (charLoc[12] & 0xFF));
		player.CharPosY = (((charLoc[13] & 0xFF) <<8) | (charLoc[14] & 0xFF));
		player.CharPosZ = (((charLoc[15] & 0xFF) <<8) | (charLoc[16] & 0xFF));
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

        private void handleClientFeatures()
        {
            display("Client Features received");
            // todo display which features are enabled/disabled
            // bitflags are beyond me atm
        }

       
        
        public static byte[] GetBytes(string text)
        {
            return ASCIIEncoding.UTF8.GetBytes(text);
        }
        public static String GetString(byte[] text)
        {
            return ASCIIEncoding.UTF8.GetString(text);
        }
        public void Send(byte[] buffer)
        {
        //    StateObject newstate = new StateObject();
            //mysocket.BeginSend(newstate, 0, Data.Length, Sockets.SocketFlags.None, EndSend, newstate);
            //mysocket.BeginSend(buffer,0,buffer.Length,SocketFlags.None,EndSend,newstate);
            if (!mysocket.Connected) { display("Dissconnected!!"); Dissconnect(); }
           
            if ((bLoginCrypt == true) & ((buffer[0] == 0x80) | (buffer[0] == 0xA0) | (buffer[0] == 0xD9)))
            {
                display("PreCrypt: " + BitConverter.ToString(buffer));
                    buffer = LoginEncryption.clientDecrypt(buffer); 
                    display("Login Crypted: " + BitConverter.ToString(buffer)); 
            }
            if ((bTwofishcrypt) & (buffer[0] != 0x7F)) // ox75 is the start of the key you use on reconnect. HACK should be linked to twofishkeyint
            {
                display("PreCrypt: " + BitConverter.ToString(buffer));
               // Twofish.serverEncrypt(ref buffer, buffer.Length);
                Twofish.clientEncryptData(ref buffer, buffer.Length);
                display("TwoFished!" + BitConverter.ToString(buffer));
            }
            mysocket.Send(buffer);
            
            if(bDebug) { display("Sent >>" + BitConverter.ToString(buffer)); }
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
        public void displaywipe()
        {
            myoutput = "";
        }
        public void updatevars()
        {
            myvars = ("Player ID: " + player.CharID + "\r\nPlayer Type: " + player.CharType + "\r\nPlayer X: " + player.CharPosX + " \r\nPlayer Y: " + player.CharPosY + "\r\nPlayer Z: " + player.CharPosZ + "\r\nPlayer Flags: " + player.flags + "\r\nPlayer Name: " + player.CharName + "\r\nGold: " + player.Gold + "\r\nWeight: " + player.Weight + "\r\nPlayer Max Weight: " + player.MaxWeight);
        }
       

        public static byte[] intToByteArray(int value)
        {
            return new byte[] {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value};
            
        }
        public static byte[] uintToByteArray(uint value)
        {
            return new byte[] {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value};

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

             for (int i = 0; i < (myuser.Length); i++)
             {
                 buffer[i + 5] = myuser[i];
             }
             for (int i = 0; i < (mypass.Length); i++)
             {
                 buffer[i + 35] = mypass[i];
             }
             bTwofishcrypt = true;
             twofishkey = (uint)((key[0] << 24) | (key[1] << 16) | (key[2] << 8) | (key[3]));
             Twofish.GameEncryption(twofishkey);
             Send(key);//Send(uintToByteArray(twofishkey));
             Send(buffer);
         }
         private void handleGameServerList()
         {
             display("Handling GameServerList");
             byte[] mybyte = { 0xA0, 0x0, 0x0 }; 
             Send(mybyte);
         }
         public void Connect()
         {
             IPAddress serverip = IPAddress.Parse(ip);
             try
             {
                 mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                 mysocket.Connect(new IPEndPoint(serverip, port));
                 mysocket.ReceiveBufferSize = 32768;
                 display("Connecting...");
             }
             catch
             {
                 display("Connection Failed");
             }
         }
         public void Login()
         {
             Send(uintToByteArray(loginkeyint));
             LoginEncryption.init(loginkeyint);
             
             bLoginCrypt = true;
             byte[] loginpack = new byte[62];
             byte[] myuser = Encoding.ASCII.GetBytes(user);
             byte[] mypass = Encoding.ASCII.GetBytes(pass);
             loginpack[0] = 0x80;

             for (int i = 0; i <= (myuser.Length - 1); i++)
             {
                 loginpack[i + 1] = (byte)myuser[i];
             }
             for (int i = 0; i <= (mypass.Length - 1); i++)
             {
                 loginpack[i + 31] = (byte)mypass[i];
             }
             loginpack[61] = 0xFE;
            
             Send(loginpack);
             //Sends user/pass and FE as the login key, can use any key
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
             sendClient(Version);
         }
         private void sendClient(string p)
         {
             byte[] clientver = GetBytes(p);
             byte[] clientPacket = new byte[clientver.Length + 4];
             clientPacket[0] = UOopcodes.CMSG_ClientVersion;

             clientPacket[1] = (byte)(clientPacket.Length >> 8);
             clientPacket[2] = (byte)clientPacket.Length;
             for (int i = 0; i < clientver.Length; i++)
                 clientPacket[i + 3] = clientver[i];

             Send(clientPacket);
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
