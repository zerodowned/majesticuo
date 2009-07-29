/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author James
 */
public class LoginCrypt {
    int pseed;
    long k1,k2;
    long m_key[] = new long[2];
    long m_k1,m_k2;

    private int N2L(int C) {
        int LL = 0;
        LL = ((C & 0xFF000000) >>24);
        LL = LL + ((C & 0x00FF0000) >>8);
        LL = LL + ((C & 0x0000FF00) <<8);
        LL = LL + ((C & 0x000000FF) <<24);
        //LL = (((C&0xFF000000) >>24) | ((C&0x00FF0000) >>8) | ((C&0x0000FF00) <<8) | ((C&0x000000FF) <<24));
        return LL;
    }
    public void initlogin() {
        //int seed = pseed;
        int seed = N2L(pseed);
        m_key[0] = ( ( ( ~seed ) ^ 0x00001357 ) << 16 ) | ( ( seed ^ 0xffffaaaa ) & 0x0000ffff );
       m_key[1] = ( ( seed ^ 0x43210000 ) >> 16 ) | ( ( ( ~seed ) ^ 0xabcdffff ) & 0xffff0000 );
     m_k1 = k1;
     m_k2 = k2;

    }

    public byte[] encrypt(byte in[],int len) {
        byte out[] = new byte[in.length];
        long table0 = 0;
        long table1 = 0;
        for (int i = 0;i < len;i++) {

        // out[i] = in[i];
         //out[i] = (byte)((in[i]) ^ ((byte)m_key[0]));
        //data[i] = (byte)(CurrentKey0 ^ data[i]);
        out[i] = (byte) (m_key[0] ^ in[i]);
        //out[i] = (byte) (m_key[0] ^ in[i]);
         table0 = m_key[0];
         table1 = m_key[1];
         m_key[0] = (((table0 >> 1) | (table1 << 31)) ^ m_k2);
         table1 = (((table1 >> 1) | (table0 << 31)) ^ m_k1);
         m_key[1] = (((table1 >> 1) | (table0 << 31)) ^ m_k1);  // -1)) >>1) | (table0 <<31)) ^ m_k1);
         //m_key[1] = ((((((table1 >> 1) | (table0 << 31)) ^ (m_k1 - 1)) >> 1) | (table0 << 31)) ^ m_k1);
        }
        return out;
    }

 public byte[] decrypt(byte in[],int len) {
        byte out[] = new byte[in.length];
        long eax,ecx,edx,esi;
        for (int i = 0; i < len;i++) {
            out[i] = (byte) (in[i] ^ (byte) (m_key[0] & 0xFF));
            edx = m_key[1];
            esi = m_key[0] << 31;
            eax = m_key[1] >> 1;
            eax |= esi;
            eax ^= m_k1 - 1;
	    edx <<= 31;
	eax >>= 1;
	ecx = m_key[0] >> 1;
	eax |= esi;
	ecx |= edx;
	eax ^= m_k1;
	ecx ^= m_k2;
	m_key[0] = ecx;
	m_key[1] = eax;

        }
        return out;
    }
}