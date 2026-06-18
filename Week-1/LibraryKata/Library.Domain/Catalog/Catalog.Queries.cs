using System.Collections;

namespace LibraryKata.Domain;

//The second half of my class, I don't have to mirror the interface implementation or any inheritance across both class files
//however, I can still only inherit from parent

public partial class Catalog : IEnumerable<LibraryItem>
{
    //This is the one that we actuallu want to provide logic for, the one that uses a generic
    public IEnumerator<LibraryItem> GetEnumerator()
    {
        foreach(LibraryItem item in _items)
        {
            //We don't want to lazily return items one at a time, we don't want to return a second list
            //or anything like that. We will use "yield" with out return
            yield return item; //yield comes up when you do a custom collection
        }
    }

    //This version (non-generic) is OLD - kept in INumerable for backwards compatibility reasons.
    //What we are doing is simply routing it to the IEnumerator<LibraryItem> GetEnumerator() method
    IEnumerator IEnumerable.GetEnumerator()
    {
        //returns a call to IEnumerator<LibraryItem> GetEnumerator()
        return GetEnumerator();
    }

    // Let's make a method to return only lendable items (things that implement ILendable)
    public IEnumerable<LibraryItem> Lendable()
    {
        foreach(LibraryItem item in _items)
        {
            //Checking for type vis "is"
            if(item is ILendable)
                yield return item;
        }
    }

    //search function for the catalog
    //We are going to use Predicate to pass a delegate to our function
    //A delegate is just a reference to method in an argument list
    // Predicate<LibraryItem> match - This represents this function that takes a LibraryItem, and returns a boolean

    //when we call this Find() method, we will combine it with a Lambda. Lambda's are the C# implementation
    //of anonymous or arrow functions. Just a quick definition that we don't bother storing a reference to.
    //authorities = Find(item => item.Author == "Frank Herbert"); - Find every item where its author equals "Frank Herbert"
    public List<LibraryItem> Find(Predicate<LibraryItem> match)
    {
        //match is a method, not an object or a value
        //it's a pointer to some method that gets passed in when we call Find()
        List<LibraryItem> foundItems = new();
        foreach (LibraryItem item in _items)
        {
            if (match(item))
            {
                foundItems.Add(item);
            }
        }
        return foundItems;
    }
}