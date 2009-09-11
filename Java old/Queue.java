/*
 * Queue Interface Created by Mikel Duke
 * http://mikelduke.sf.net
 */
public interface Queue
{
	/* Pre: None
	   Post: x is added to the back of the queue
	*/
	public void enQueue(Object x);

	/* Pre: Queue should not be emtpy
	   Post: If queue is empty, throw RuntimeException, but if it isnt,
	   the object at the front of the queue is returned and removed
	*/
	public Object deQueue() throws RuntimeException;

	/* Pre: Queue should not be emtpy
	   Post: Throws RuntimeException is queue is empty, otherwise returns
	   object at the front of the queue
	*/
	public Object getFront() throws RuntimeException;

	/* Pre: None
	   Post: Returns if Queue is empty
	*/
	public boolean isEmpty();

	/* Pre: None
	   Post: Makes queue empty
	*/
	public void makeEmpty();
}
