namespace MusicKata.Domain;

public class WorkingBin<T>
{
    private readonly LinkedList<T> _items = new LinkedList<T>();

    public void Add(T item)
    {
        _items.AddLast(item);
    }

    public bool PromoteToFront(Predicate<T> match)
    {
        LinkedListNode<T>? current = _items.First;
        while (current != null)
        {
            if (match(current.Value))
            {
                _items.Remove(current);
                _items.AddFirst(current);
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    public IEnumerable<T> GetAll()
    {
        LinkedListNode<T>? current = _items.First;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }
}