/*
 * UOConsoleClient created by Mikel Duke
 * http://mikelduke.sf.net
 *
 * UOAccountException is an error class thrown by the get
 * server list method in UONetworking which checks for a
 * kick packet or an invalid packet.
 */


public class UOAccountException extends Exception
{
	String error = "";
	String packet = "";

	public UOAccountException(String errormsg)
	{
		error = errormsg;
	}

	public UOAccountException(String errormsg, String packet)
	{
		error = errormsg;
		this.packet = packet;
	}

	public String toString()
	{
		return error + "\n\n" + packet;
	}
}
