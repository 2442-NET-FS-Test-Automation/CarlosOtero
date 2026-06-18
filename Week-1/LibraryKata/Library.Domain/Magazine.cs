namespace LibraryKata.Domain;

//Sealed is pretty simple, it means this class is not inheritable
//Nobody can be a child of Magazine. More a signal of intent and design than anything, but still useful
public sealed class Magazine : LibraryItem, ILendable
{
    public int CirculationCopies {get; private set;}
    public string Publisher {get; private set;}

    public Magazine(string title, string author, int circulationCopies, string publisher) : base (title, publisher)
    {
        CirculationCopies = circulationCopies;
        Publisher = publisher;
    }

    public override string Describe()
    {
        return $"{Title} magazine published by {Publisher} -- {CirculationCopies} copies available";
    }

    //Providing implementation via new instead of override - has implications for later
    //This is technically Method hiding - depends on the reference type
    //Calling this method in an object instantiated like this:
    //LibraryItem sportsIllustrated = new Magazine(...); - calls LibraryItem's ShelfLabel
    //This is most likely not what you want
    //new vs override - very different behaviour 
    public new string ShelfLabel()
    {
        return $"MAG-{Id} {Title}";
    }

        public bool Checkout()
    {
        //Attempt to checkout a magazine - if copies is already 0, return false
        if(CirculationCopies == 0)
            return false;
        //Otherwise, we pass over the above code block
        //We can decrement the available copies and return true
        CirculationCopies--;
        return true;
    }

    //Providing for return behavior
    public void Return() => CirculationCopies++;
}