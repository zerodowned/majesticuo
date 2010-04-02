using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// CLient sends 0a010103 then sends 0x80 crypted
//Sent >>0A-01-01-03
//PreCrypt: 80-63-61-6C-69-73-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-6A-75-6E-6B-32-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-FE
//Sent >>20-CC-C9-C7-43-99-0A-7A-C2-1E-70-C7-1C-71-C7-9C-31-E7-8C-B9-A3-2E-E8-0B-7A-42-DE-90-37-64-CD-F3-C6-48-87-3B-7B-42-DE-90-37-E4-0D-F9-03-FE-80-BF-A0-AF-A8-AB-2A-EA-0A-7A-C2-1E-70-C7-1C-8F
//Received >> 82-04


namespace drkuo.Network
{
    class Encryption
    {
        // Client 3.0.0x Keys
        public uint CurrentKey0;
        public uint CurrentKey1;
        public uint backup0;
        public uint backup1;
        public static uint FirstClientKey = 0x2D93A5FD;//289686529;//0x2d93a5fd;
        public static uint SecondClientKey = 0xA3DD527F;//2747883135;//0xa3dd527f;
        public uint EncryptionSeed;

        public void init(uint Seedbyte)
        {
            EncryptionSeed = Seedbyte;
            CurrentKey0 = (uint)((((~EncryptionSeed) ^ 0x00001357) << 16) | ((EncryptionSeed ^ 0xFFFFAAAA) & 0x0000FFFF));
            CurrentKey1 = (uint)(((EncryptionSeed ^ 0x43210000) >> 16) | (((~EncryptionSeed) ^ 0xABCDFFFF) & 0xFFFF0000));
            backup0 = CurrentKey0;
            backup1 = CurrentKey1;
        }
        public byte[] Encrypt(byte[] data)
        {
            int len = data.Length;
          for(int i = 0; i < len; i++)
          {
              // Decrypt the byte:
              data[i] = (byte)(CurrentKey0 ^ data[i]);
              
              // Reset the keys:
              uint oldkey0 = CurrentKey0;
              uint oldkey1 = CurrentKey1;
              CurrentKey0 = (uint)(((oldkey0 >> 1) | (oldkey1 << 31)) ^ SecondClientKey);
              CurrentKey1 = (uint)((((((oldkey1 >> 1) | (oldkey0 << 31)) ^ (FirstClientKey - 1)) >> 1) | (oldkey0 << 31)) ^ FirstClientKey);
              
          }
          CurrentKey0 = backup0;
          CurrentKey1 = backup1;
          return data;
        }
        public byte[] clientDecrypt(byte[] buffer)
        {
            uint eax, ecx, edx, esi;
            int len = buffer.Length;
            for (int i = 0; i < len; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ (byte)(CurrentKey0 & 0xFF));
                edx = CurrentKey1;
                esi = CurrentKey0 << 31;
                eax = CurrentKey1 >> 1;
                eax |= esi;
                eax ^= FirstClientKey - 1;
                edx <<= 31;
                eax >>= 1;
                ecx = CurrentKey0 >> 1;
                eax |= esi;
                ecx |= edx;
                eax ^= FirstClientKey;
                ecx ^= SecondClientKey;
                CurrentKey0 = ecx;
                CurrentKey1 = eax;
            }
            return buffer;
        }

        public byte[] Encrypt2(byte[] clear)
        {
            uint table0 = 0, table1 = 0;
            byte[] encrypted = new byte[clear.Length];

            for (int i = 0; i < clear.Length; i++)
            {
                encrypted[i] = (byte)((clear[i]) ^ ((byte)CurrentKey0));

                table0 = CurrentKey0;
                table1 = CurrentKey1;

                CurrentKey0 = ((table0 >> 1) | (table1 << 31)) ^ SecondClientKey;
                table1 = ((table1 >> 1) | (table0 << 31)) ^ FirstClientKey;
                CurrentKey1 = ((table1 >> 1) | (table0 << 31)) ^ FirstClientKey;
            }

            return encrypted;
        } 

    }
}
