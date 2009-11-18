using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drkuo.Packets.Send
{
    class Packets
    {
        public static byte[] say(String text)
	{
		byte[] sayPacket;
            if(text.ToLower() == "guards")
		{
			byte[] guards = { (byte)0xAD, 0x00, 0x16, (byte)0xC0,
			0x00, 0x5A, 0x00, 0x03, 0x45, 0x4E, 0x55,
			0x00, 0x00, 0x10, 0x07, 0x67, 0x75, 0x61,
			0x72, 0x64, 0x73, 0x00 };
			sayPacket = guards;
		}
		else if (text.ToLower() == "bank")
		{
			byte[] bank = { (byte)0xAD, 0x00, 0x14, (byte)0xC0,
			0x00, 0x5A, 0x00, 0x03, 0x45, 0x4E, 0x55,
			0x00, 0x00, 0x10, 0x02, 0x62, 0x61, 0x6E,
			0x6B, 0x00 };
			sayPacket = bank;
		}
		else
		{
			
            byte[] textAr = uonetwork.GetBytes(text);
			sayPacket = new byte[(textAr.Length * 2) + 14];
			sayPacket[0] = UOopcodes.CMSG_TalkRequest;
			sayPacket[1] = (byte)(sayPacket.Length >>8);
			sayPacket[2] = (byte)sayPacket.Length;
			sayPacket[3] = (byte)0x00; //mode 0xc0 if guards vender etc
			sayPacket[4] = 0x00; //hue
			sayPacket[5] = 0x04; //hue
			sayPacket[6] = 0x00; //font
			sayPacket[7] = 0x03; //font
			sayPacket[8] = (byte)'E';
			sayPacket[9] = (byte)'N';
			sayPacket[10] = (byte)'U';
			sayPacket[11] = 0x00;
			for (int i = 0; i < textAr.Length; i++)
				sayPacket[(i * 2)+13] = textAr[i];
		}
		return sayPacket;
	}
        public static byte[] GetPlayerStatus(bool basicstatus, int serial)
        {
            byte[] status = new byte[10];
            status[0] = UOopcodes.CMSG_GetPlayerStatus;
            status[1] = (byte)0xED;
            status[2] = (byte)0xED;
            status[3] = (byte)0xED;
            status[4] = (byte)0xED;
            if (basicstatus)
            {
                status[5] = (byte)0x04; // 0x05 is request skills? 0x3a
            }
            else
            {
                status[5] = (byte)0x05;
            }
            
            byte[] temp = uonetwork.intToByteArray(serial);
            status[6] = temp[0];
            status[7] = temp[1];
            status[8] = temp[2];
            status[9] = temp[3];
            return status;
        }
       
        /* public static void walk(String direction, int numOfSteps)
    {
        byte walkPacket[] = new byte[7];
        walkPacket[0] = UOopcodes.CMSG_MoveRequest;
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
    }*/

        public static byte[] dropg(int itemid, int x, int y, int z) {
            byte[] drop = new byte[14];
            drop[0] = UOopcodes.CMSG_DropItem;
            byte[] temp = uonetwork.intToByteArray(itemid);
            drop[1] = temp[0];
            drop[2] = temp[1];
            drop[3] = temp[2];
            drop[4] = temp[3];
            byte[] temp2 = uonetwork.intToByteArray(x);
            drop[5] = temp2[0];
            drop[6] = temp2[1];
            byte[] temp3 = uonetwork.intToByteArray(y);
            drop[7] = temp3[0];
            drop[8] = temp3[1];
            drop[9] = (byte)z;
            return drop;
        }
        public static byte[] drag(int itemid, int stacksize) {
            byte[] drag = new byte[7];
            drag[0] = UOopcodes.CMSG_PickUpItem;
            byte[] temp = uonetwork.intToByteArray(itemid);
            byte[] temp2 = uonetwork.intToByteArray(stacksize);
            drag[1] = temp[0];
            drag[2] = temp[1];
            drag[3] = temp[2];
            drag[4] = temp[3];
            drag[5] = temp2[0];
            drag[6] = temp2[1];
            return drag;
        }
        public static byte[] singleclick(int itemid)
        {
                byte[] pckuse = new byte[5];
            pckuse[0] = UOopcodes.CMSG_SingleClick;
            byte[] temp = uonetwork.intToByteArray(itemid);
            pckuse[1] = temp[0];
            pckuse[2] = temp[1];
            pckuse[3] = temp[2];
            pckuse[4] = temp[3];
            return pckuse;
           }
        

        private static void display(string p)
        {
            
            //throw new NotImplementedException();
        }

        public static byte[] DoubleClick(int Itemid) {
                byte[] pckuse = new byte[5];
            pckuse[0] = UOopcodes.CMSG_DoubleClick;
            byte[] temp = uonetwork.intToByteArray(Itemid);
            pckuse[1] = temp[0];
            pckuse[2] = temp[1];
            pckuse[3] = temp[2];
            pckuse[4] = temp[3];
            return pckuse;
           

        }
        

        public static byte[] useSkill(int skill2)
	{
        String skill = "";
        int pcksize = 8;
            skill = "" + skill2 + " 0"; // converts our number into a string eg "38 0"
            if(skill2 > 9) {pcksize = 9;} // packet is 9 not 8 if the skill is > 9
		byte[] skillPacket = new byte[pcksize];
		skillPacket[0] = UOopcodes.CMSG_RequestSkilluse;
		skillPacket[1] = 0x00; //block size	
        skillPacket[2] = (byte)pcksize;
		skillPacket[3] = 0x24;
        byte[] mskill = uonetwork.GetBytes(skill);
        for (int i = 0; i < (mskill.Length); i++)
        {
            skillPacket[i + 4] = mskill[i];
        }
        return skillPacket;
	}
        public static byte[] Cast(int skill2)
        {
            String skill = "";
            int pcksize = 8;
            skill = Convert.ToString(skill2); // converts our number into a string eg "38 0"
            if (skill2 > 9) { pcksize = 9; } // packet is 9 not 8 if the skill is > 9
            byte[] skillPacket = new byte[pcksize];
            skillPacket[0] = UOopcodes.CMSG_RequestSkilluse;
            skillPacket[1] = 0x00; //block size	
            skillPacket[2] = (byte)pcksize;
            skillPacket[3] = 0x56;
            byte[] mskill = uonetwork.GetBytes(skill);
            for (int i = 0; i < (mskill.Length); i++)
            {
                skillPacket[i + 4] = mskill[i];
            }
            return skillPacket;
        }   
        public static byte[] resync() {
            byte[] sync = new byte[3];
            sync[0] = (byte)0x22;
            sync[1] = 0;
            sync[2] = 0;
            return sync;
        }
    }
}
