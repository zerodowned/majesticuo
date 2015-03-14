Call init once at the start of your connection, then First three packets sent are encrypted via the encrypt routine.


```
class LoginEncryption
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
}
```