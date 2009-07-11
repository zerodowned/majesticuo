/*
 * QueueLi Created by Mikel Duke
 * http://mikelduke.sf.net
 *
 * Conforms to Queue, uses a LinkedList to store the
 * queue info.
 */

public class QueueLi implements Queue
{
	ListNode front, back;
	public QueueLi()
	{
		makeEmpty();
	}

	/* Pre: None
	   Post: Makes queue empty
	*/
	public void makeEmpty()
	{
		front = back = null;
	}

	/* Pre: None
	   Post: Returns if Queue is empty
	*/
	public boolean isEmpty()
	{
		return front == null;
	}

	/* Pre: Queue should not be emtpy
		   Post: If queue is empty, throw RuntimeException, but if it isnt,
		   the object at the front of the queue is returned and removed
	*/
	public Object deQueue() throws RuntimeException
	{
		if (isEmpty())
			throw new RuntimeException("Empty Queue");
		Object item = front.element;
		front = front.next;
		return item;
	}

	/* Pre: Queue should not be emtpy
	   Post: Throws RuntimeException is queue is empty, otherwise returns
	   object at the front of the queue
	*/
	public Object getFront() throws RuntimeException
	{
		if (isEmpty())
			throw new RuntimeException("Empty Queue");
		return front.element;
	}

	/* Pre: None
		   Post: x is added to the back of the queue
	*/
	public void enQueue(Object x)
	{
		if (isEmpty())
			back = front = new ListNode(x, null);
		else
			back = back.next = new ListNode(x, null);
	}
}

//Class used by LinkedList that holds data and points to the next element.
class ListNode
{
	public Object element;
	public ListNode next;

	public ListNode(Object x, ListNode n)
	{
		element = x;
		next = n;
	}
}
