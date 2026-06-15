namespace LibraryKata.Domain;

public class Catalog
{
    //Backing our catalog is going to be a list.
    //Lists are ordered, grow and shrink dinamically and are accesible via index.
    //Your default collection - even above Arrays



    public List<LibraryItem> _items = new(); //*Naming convention "_ETC" for readonly items

    //This method is technicaly redundant - this class basically just wraps the above list BUT if we wanted to restrict people from Adding or Removing or even accessing via index
    //from other places in the code, we coulf wrap not only the list, but its instance methods with our own wrapper methods and make them internal, private, protected, etc.
    public int Count => _items.Count();

    //public void Add(LibraryItem item) => _items;

    //Stack<T>: Last in first out - we will model a return car. The most recently returned item is re-shelved first
    //Primary methods - Push(): Puts an item at the top of the stack. Pop(): Removes the top most item.

    public readonly Stack<LibraryItem> _returnCart = new();
    // Queue<T>: First in first out - modeling a hold queue, cutomers placing holds on books
    // Primary methods - Enqueue(): Join the back of the line, Dequeue(): removed from the front of the line.
    public readonly Queue<string> _holdQueue = new();

    //Reading List
    //LinkedList<T>: cheap inserts/removals anywhere in my list, but NO index access.

    public readonly LinkedList<LibraryItem> _readingList = new();
}