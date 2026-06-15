namespace LibraryKata.Domain;

//LibraryItem will be an abstract class - it cannot be instantiated. It will still constructor - because child classes NEED to be able
//to call their parent's constructor - but WE can't call it via new
public abstract class LibraryItem
{
    //Things about a book we can model - what is the "shape" of a book, because I want to use a no-arg Constructor, its best practice to make my properties nullable
    public string? Title {get; private set;} //auto property syntax - no writing setters and getters
    public string? Author {get; private set;}
    private static int _nextId = 1; // By convention, static properties have an underscore 

    public int Id {get;}//No setter, I don't want someone to reassign this.

    //My abstract class DOES have a constructor
    //So far, we've dealt with public and private access modifiers
    //public: anyone can see/call this
    //private: only accesible within the class
    //protected: this class and derived (child) classes only

    protected LibraryItem(string title, string author)
    {
        Id = _nextId++;
        Title = title;
        Author = author;
    }

    //Abstract method - only signature no body
    public abstract string Describe();

    //Abstract classes CAN contain concrete implementation - and we can mix out abstract methods to save time later potentially
    //Our child WILL implement Describe() - use that for the ToString()

    public override string ToString() => Describe();

    //Concrete methods have a body, Abstract methods MUST be overriden... virtual mehods have a body and MAY be overriden

    public virtual string ShelfLabel()
    {
        return $"{Id}: {Title}";
    }
}