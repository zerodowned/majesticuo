/*
 * UOPacketOperation Interface Created by Mikel Duke
 * http://mikelduke.sf.net
 *
 * Objects using the UONetworking class must either implement
 * this or create an object imlementing it. The methods here
 * must be overridden and are called by the loop that reads
 * incoming packets as necessary. This allows the other
 * class to do what it likes with the data recieved in the
 * packets, and acts like the actionListeners in the Java API.
 */

public interface UOPacketOperation
{
	public void error(Exception e);
	public void processChatPacket(String msg);
	public void processDisconnect();
	public void processServerList(String[] list);
	public void processCharList(String[] list);
	public void processLoggedIn();
	public void processUpdatePlayer(int playerID, int model, int x, int y, int z, int direction, int hue, int flag, int highlightColor);
}