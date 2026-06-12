namespace LibraryKata.Domain;

public class Book : LibraryItem, ILendable
{
    //What is unique to a book (For our demo)
    public int CopiesAvailable {get; private set;}

    //Child class constructor look a little different
    //We tak in all our arguments for the parent + child, then call base with a colon

    public Book(string title, string author, int copiesAvailable) : base(title, author)
    {
       CopiesAvailable = copiesAvailable;
    }


   

    //Because we have an abstract method in the parent, we MUST override it or we can't compile

    public override string Describe()
    {
        return $"{Id}: {Title} by {Author}, has {CopiesAvailable} copies available for checkout";  
    }

    //Methods below pasted from OldBook.cs
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
}