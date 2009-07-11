/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author James
 */
public class OpCodes {
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
	private final byte pckHesay = (byte)0xAE;
	private final byte pckRequestWarMode = (byte)0x72;
	private final byte pckGameServerLogin = (byte)0x91;
	private final byte pckLoginChar = (byte)0x5D;
	private final byte pckTalkRequest = (byte)0xAD;
	private final byte pckClientVersion = (byte)0xBD;
	private final byte pckServerChat = (byte)0xAE;
	private final byte pckServerSpeech = (byte)0x1C;
	private final byte pckClientWalk = (byte)0x02;
}
