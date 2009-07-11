public class UOException extends Exception
{
	String error = "";
	String packet = "";

	public UOException(String errormsg)
	{
		error = errormsg;
	}

	public UOException(String errormsg, String packet)
	{
		error = errormsg;
		this.packet = packet;
	}

	public String toString()
	{
		return error + "\n\n" + packet;
	}
}
