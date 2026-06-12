//Let's start modeling stuff
namespace Library.Domain;

public class Book
{
    // Things about a book we can model - what is the "shape" of a book
    // Because I want to use a no-arg Constructor, it's is best practice to make my properties nullable

    public string? Title {get; private set;} //auto property syntax - no writing setters and getters
    public string? Author {get; private set;}
    public int? CopiesAvailable {get; private set;}

    //The same way we can have static methods (belong to the class)
    //We can have static properties/members

    private static int _nextId = 1; // By convention, static properties have an underscore 

    public int Id {get;}//No setter, I don't want someone to reassign this.

    //Every class a very specific method within it
    // The constructor, you can have as many as you need/want
    //Let's make a full argument constructor

    public Book(string title, string author, int copiesAvailable)
    {
        Id = _nextId++; //Get the value of _nextId, assign it, increment it
        Title = title;
        Author = author;
        CopiesAvailable = copiesAvailable;
    }

    public Book(){}

    //First instance method - no "static" keyword just an access modifier + return type + any arguments if any
    public bool Checkout()
    {
        //Attempt to checkout a book - if copies is already 0, return false
        if(CopiesAvailable == 0)
            return false;
        //Otherwise, we pass over the above code block
        //We can decrement the available copies and return true
        CopiesAvailable--;
        return true;
    }

    //Providing for return behavior
    public void Return() => CopiesAvailable++;

    //Overriding a toString
    public override string ToString()
    {
        //Commented out below is a call to base.ToString()
        //We can use the base keyword to refer to the parent class of the class we are working in
        //Book's parent is object, so this is calling the default toString()
        //return base.ToString()
        return $"{Title} by {Author}: {CopiesAvailable} available for checkout";
    }
}

