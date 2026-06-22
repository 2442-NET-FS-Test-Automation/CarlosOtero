//If I have code from another namespace I want to use here - I use a using statement
using Library.Kata.Domain;
using LibraryKata.Domain;
using Serilog;

namespace LibraryKata.App; // A namespace is like a bucket or logical container for different related code files.

public class Program
{
    // We need a class ro hld our Main() method. The previous style with no class
    // or main - implicitly had a Main() under the hood.

    //public - accesible across the program
    //static - Main can be called upon without a Program object. It is a static/class method.
    //void - it doesn't return anything
    public static async Task Main()
    {
        // Let's configure Serilog here before any code execution
        // Serilog works via a singleton object. It's shared globally
        //Throughout the app, configure once use anywhere.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information() // Verbose > Debug > Info > Warning > Error > Fatal
            .WriteTo.Console() //Sink: Where do my logs go? Console, text file, database, etc?
            .CreateLogger(); //Create logger based on the config above


        // When I call dotnet run, it finds Main() and begins code execution at the first line of the main method.
        // I wrote my code, inside DataTypesAndOperatirs() - a separate method. So if I want
        //that code to run, I need to call it inside Main()
        DataTypesAndOperators();
        //Wired in: These were deined but never called from Main, so the control-flow,
        //loops, and arrays demos never actually ran. Call them in source order.
        ClassesExample();
        OopDemo();
        CollectionsDemo();
        ExceptionsDemo();
        AdvancedClassesDemo();
        await AsyncHttpDemo();
        //In case there are any lingering logs by the time we hit line 37 above
        //Don't just stop execution, write the logs to their sink THEN close the program
        Log.CloseAndFlush();

    }

    private static void DataTypesAndOperators() //If I had arguments, or inputs for this method, they would go inside the parenthesis after the method name.
    {
        Console.WriteLine("=== Data types and operators ===");

        // C# is a Strongly types language
        // We cannot just create variables and shive whatever we want into them like 35 or Python.

        int copies = 3; // whole numbers
        double lateFee = 1; //floating point numbers (decimals)
        bool isMember = true; // true or false values
        char shelf = 'A'; // single character
        string title = "Clean Code"; // text, strings are reference types

        // Operators
        string user = "Jon"; // Single = is the assignment operator.
        int total = copies * 2; // example of an arithmetic operator, like + - * /
        bool isEnough = total > 4; // Comparison - This line compares the value in total to 4, if it is greater than 4,
                                // isEnough will get 'true', otherwise it will get 'false'.
        // >, <, >=, <= - comparison operators.
        bool exactlySix = total == 6; // equality. Single equals is assignment, double equals is equality.
        // unlike JS there is NO === all equality in C# is Strict equality.
        bool lendable = isMember && isEnough; // Logical operators
        // && - and, || - or, ! - reverses the condition that follows, ^ Logical XOR - Returns true is only one condition is true.

        //Tis is a basic way to construct strings from other strings 
        //String concat - it works, but it can be messy
        Console.WriteLine(title + " has been checked out by "+ user);

        //We can create much cleaner formatted strings
        // using String interpolation - a string with a $ before the opening quote
        Console.WriteLine($"{title} on shelf {shelf}: {copies} copies, fee {lateFee}");

        //C# has a lot of shorthancds and little shortcuts that you can find and use
        // to make youur code easier to write. For example, Let's say I want to add 1 to the value of total
        // I could do something like
        // total = total + 1; - ORRR
        total += 1; //arithmethic shorthand for the same thing, also works for *= /= -=


    }   


    private static void ControlFlow(){
        Console.WriteLine("\n== Control Flow ==");
        int copiesAvailable = 0;
        bool isMember = true;

        if(copiesAvailable > 1)
            Console.WriteLine("Available for checkout!");
        else if (copiesAvailable == 1)
            Console.WriteLine("Last copy!");
        else{
            Console.WriteLine("Out of stock!");
            Console.WriteLine("Check again later!");
            }

        // Switch

        //Classic switch - notice C# cares about intent a lot! No fall through like in other languages
        string genre = "Mistery";
        switch (genre)
        {
            case "Mystery":
                Console.WriteLine("Check section A");
                break;
            case "Science Fiction":
                Console.WriteLine("Check section F");
                break;
            default: //While optional, a default case to catch any edge cases is best practices
                Console.WriteLine("uh oh");
                break;
        }

        //New in .NET 8, Switch Expressions! You don't have to use these - they probably won't come up in QC, but they're used out in real world code, so here is an example.
        //In a switch expression, we want a return value from the switch - we can then use that value to print out a result

        string section = genre switch
        {
          //This is my expression body
            "Mystery" => "Section A",
            "Science Fiction" => "Section F",
            _ => "Uh oh" //default
        };
        Console.WriteLine(section);
    }
        private static void Loops()
    {
        for(int day = 1; day <= 3; day++)
        {
            Console.WriteLine($"Reminder day {day}: fee so far {CalculateLateFee(day)}");
        }
        int onShelf = 3;
        while (onShelf > 0)
        
            Console.WriteLine($"{onShelf} copies on the shelf!");
            onShelf--; // quick decrement shorthand

        Console.WriteLine("No copies on shelf!");
        string myString ="dog";
        myString = "cat";
        //Value types (ints, double, char) Stored in the stack they are primitive
        //Heap a big blob that has continuous blocks of memory, a string is an example of a reference type that is stored on the heap (It point to the location in the heap where the string is actually stored).

    }

    private static decimal CalculateLateFee(int daysLate) => daysLate * 2;
    
    private static void ArraysWork()
    {
        //C# provide for Arrays as well as Lists and other collections - we'll get to those later.
        string [] books = {"Dune", "Harry Potter", "Percy Jackson", "Lord of the Rings"};

        Console.WriteLine(books[2]); //I can get access to individual elements - keeping in mind we index at 0
        
        //C# Allows for for-each loops
        foreach(string book in books)
        {
            Console.WriteLine(book);
        }
    }

    private static void ClassesExample()
    {
        Console.WriteLine("Using out domain Book class");

        //Instantiating my first book, calling the constructor via "new" keyword
        Book dune = new Book("Dune", "Frank Herbert", 3);
        Book littlePrince = new Book("The Little Prince", "Antoine de Saint-Exupéry", 0);
        
        //If I want to print book info, I can just pass the book variable
        // It calls the toString() for me.. The next lines do the same thing
        Console.WriteLine(dune);
        Console.WriteLine(littlePrince.ToString());

        Console.WriteLine($"Checking out Dune: {dune.Checkout()}"); // true
        Console.WriteLine($"Checking out The Little Prince: {littlePrince.Checkout()}"); // false
    }


    public static void OopDemo()
    {
        Console.WriteLine("\n\n == OOP DEMO ==");

        //Leveraging polymorphism - Books, ReferenceBooks, and Magazines - all are LibraryItem
        LibraryItem[] catalog =
        {
          new Book("Dune", "Frank Herbert", 2),
          new ReferenceBook("C# Language Standards", "Microsoft", "Technology"),
          new Magazine("Sports Illustrated", "Francisco", 5, "Conde Naste")
        };

        foreach(LibraryItem item in catalog){
        Console.WriteLine(item.Describe());
        }

        //We can even use interfaces as reference types
        foreach(LibraryItem item in catalog)
        {
            if(item is ILendable lendable)
            {
                Console.WriteLine($"{item.Title} : checkout -> {lendable.Checkout()}");
            }
            else
            {
                Console.WriteLine($"{item.Title} is reference only");  
            }
        }
        // overide vs new behaviour
        Magazine wired = new Magazine("Wired", "Luis", 3, "Conde Naste");
        LibraryItem baseMag = wired;

        Console.WriteLine("== Override vs New  on the same object, different ref type ==");
        Console.WriteLine($"Magazine reference -> {wired.ShelfLabel()}");
        Console.WriteLine($"LibraryItem reference -> {baseMag.ShelfLabel()}");

    }

    //Collections demo stuff
    private static void CollectionsDemo()
    {
        Console.WriteLine("=== COLLECTIONS DEMO STUFF ===");

        //Creating a catalog object
        //Because this is backed by a list, it grows and shrinks for us
        Catalog catalog = new();

        //I could create my objects
        Book dune = new ("Dune", "Frank Herbert", 3);

        //then add them
        catalog.Add(dune);

        //I can also just call a constructor inside the Add() method call
        //Methods having their arguments satissfied by the return of the other methods is a common pattern
        //and sometimes you0ll get like 4-5 callbacks deep in tools like ASP.NET

        catalog.Add(new ReferenceBook("C# Language specs", "Microsoft", "Technology"));
        catalog.Add(new Magazine("Nat Geo", "Charlie", 4, "Conde Naste"));

        Console.WriteLine($"Catalog holds {catalog.Count}; first is {catalog[0].Title}");

        //Enum + Struct use
        ItemKind kind = ItemKind.Magazine; //Example of selecting an enum value
        ShelfLocation location = new (3,12); //Struct - looks a lot like a class, but it is a VALUE type (AISLE 3, SHELF 12)

        Console.WriteLine($"{kind} sits at {location}");

        Book duneCopy = dune; //Copies the reference
        //Let's say I modify duneCopy, what happens to the data in dune?
        //All we copies was the pointer - these two are not independent

        ShelfLocation location2 = location; //Copies the data/fields
        //These are not linked in the same way, I can edit the data in one without touching the other

        //Generics: Our own Shelf<T> that can hold anything - though technically all the collections
        // we used thus far have been generic classes themselves

        Shelf<LibraryItem> shelf = new Shelf<LibraryItem>(10);
        Shelf<int> intShelf = new Shelf<int>(200);

        shelf.TryAdd(catalog[0]);
        shelf.TryAdd(catalog[1]);

        Console.WriteLine($"Trying to add a third thing in our catalog: {shelf.TryAdd(catalog[2])}");
    }

    public static void ExceptionsDemo()
    {
        Console.WriteLine("\n == Exceptions, patterns, logging ==");
        //By using liskov substitution from SOLID, if I later swap to a SQLLibraryRepo or whatever, this is
        // the only line I have to change
        ILibraryRepository repo = new InMemoryLibraryRepository();

        //Injection our existing repo object to satisfy LibraryUnitOfWork's dependency
        IUnitOfWork libraryWork = new LibraryUnitOfWork(repo);

        //Create a book, but using our factory method
        LibraryItem dune = LibraryItemFactory.Create(ItemKind.Book, "Dune", "Frank Herbert", copies: 3);

        repo.Add(dune);
        //Magazines need a publisher, but we provided a default values for the publisher argument in Create
        //Let's see if it works
        repo.Add(LibraryItemFactory.Create(ItemKind.Magazine, "Wired", "Axel", copies: 2));

        //Pretend we are commiting changes to a DB or something.
        libraryWork.Stage("Added 2 items");
        libraryWork.Commit();

        //We went through the trouble of creating custom exceptions
        //Let's actually see them work for us. If you have code that can potentially fail
        //wrap it in a try-catch (optional finally)
        try
        {
            //Potentially offending code goes here
            LibraryItem missing = repo.GetById(99);
            Console.WriteLine(missing.Describe()); //We won't hit this I believe
        }
        catch (ItermNotFoundException ex)
        {
            //Your code can potentially throw more than one exception type. Handle them from most -> least specific
            //We stored the offending id on the exception itself, here we can ask for it logging
            Log.Error("Lookup failed for id {Id}: {Message}", ex.Id, ex.Message);
        }
        catch (LibraryException ex)
        {
            Log.Error("Library Error: {Message}", ex.Message);
        }
           catch (Exception ex)
        {
            Log.Error("Non Library Error: {Message}", ex.Message);
        }
        finally //Optional but adding a finally block add code that runs whether an exception is caught or not.
        {
            //Code in a finally block will run even if the try ends in a return
            //Useful for DB operations wher you want to cleanup but you found the objecto to return
            Console.WriteLine("Hit our finally block - lookup attempt done");
        }

        Book noCopies = new Book ("Count of Montecristo", "Alejandro Dumas", 0);

        try
        {
            Borrow(noCopies);
        }
        catch (ItemNotAvailableException ex)
        {
            Log.Warning("Borrow refused: {Message}", ex.Message);
        }

    }

    public static void Borrow (Book book)
    {
        //We can use the Checkout (boolean return) method from the book object
        //in an if or something
        if (!book.Checkout())
        {
            throw new ItemNotAvailableException(book.Title);
        }
    }

    public static void AdvancedClassesDemo()
    {
        Console.WriteLine("\n == Advanced classes ==");
        //First, a quick detour, let's interact with the garbage collector
        Console.WriteLine(GC.GetTotalMemory(forceFullCollection:false)/1024);

        ILibraryRepository repo = new InMemoryLibraryRepository();

        LibraryItem dune = LibraryItemFactory.Create(ItemKind.Book, "Dune", "Frank Herbert", copies: 3);
        repo.Add(dune);

        repo.Add(LibraryItemFactory.Create(ItemKind.Magazine, "Wired", "Axel", copies: 2));
        repo.Add(LibraryItemFactory.Create(ItemKind.Book, "Dune Messiah", "Frank Herbert", copies: 3));
        repo.Add(LibraryItemFactory.Create(ItemKind.ReferenceBook, "C# Language Reference", "Microsoft", 1, section: "3"));

        Catalog catalog = new();
        foreach (LibraryItem item in repo.GetAll())
        {
            catalog.Add(item);
        }

        Console.WriteLine($"We have {catalog.Authors.Count} unique authors in our catalog");

        foreach(string author in catalog.Authors)
        {
            Console.WriteLine(author);
        }
        //Let's search our catalog now that it's backed by a dictionary
        //Let's use our Find() method
        List<LibraryItem> byFrankHerbert = catalog.Find(item => item.Author == "Frank Herbert");
        Console.WriteLine($"There are {byFrankHerbert.Count} books by Frank Herbert");

        //Let's see how many items in the catalog re Lendable
        Console.WriteLine("We have a mix of lendable and non-lendable items");

        foreach(LibraryItem item in catalog.Lendable())
        {
            Console.WriteLine($"{item.Title}");
        }

    }

    public static async Task AsyncHttpDemo()
    {
        //We wrote our client object so let's use it
        OpenLibraryClient client = new();

        //Array to hold some isbn's
        string[] isbns = { "9780132350884", "9780201633610"};

        //I want to fetch the data from the Openlibrary for both ISBNs
        //I do not want to sit here and type the same code calling the same method for both ISBNs
        //I would end up awaiting two almost identical calls - that's valid bit the curricula says "optimizing async code"

        Task<LibraryItem?>[] fetchedBooks = new Task<LibraryItem?>[isbns.Length];

        //Next we loop through the array and call FetchByIdAsync - we use a traditional C-syntax for-Loop
        //because we care about indexes for this
        for(int i = 0; i < isbns.Length; i++)
        {
            //Notice, this is an async method call - but we didn't await it.
            fetchedBooks[i] = client.FetchByIsbnAsync(isbns[i]);
        }

        //If we ONLY wannted ONE book, and we just had one isbn, we could do something like the following:
        //foundbook = await client.FetchByIsbnAsync("1234564945644");


        //In between starting the request in line 407, and the Task.WhenAll() call, I can do other stuff. I can call other methods
        //I can acces other systems, etc.

        LibraryItem?[] foundBooks = await Task.WhenAll(fetchedBooks);

        //this works, but what if there's nothing there?
        //LibraryItem? firstBookFound = foundBooks[0]:

        //To be safe, we can use a quick ternary operator. Like a quick if-else check
        //Ternary syntax (Some condition to check) ? trueValue : falseValue
        LibraryItem? firstBookFound = foundBooks.Length > 0 ? foundBooks[0] : null;

        //Using WhenAll to do concurrent fetching. If we didn't do this, and we awaited EVERY SINGLE call one by one
        //Think about the amount of latency we'd be eating.
        Console.WriteLine($"Fetched: {firstBookFound?.Describe() ?? "nothing"}");

        //Boxing and unboxing - mostly deprecated, replaced by generics
        //Sometimes we needed to store value types on the heap, think of adding an int to a list. Before generics (List<T>)
        //we had ArrayList to accomplish the same thing. Under the hood, an ArrayList couldn't accept value types.

        //We have an int
        int toBeBoxed = 6;

        //We "box" it, by giving wrapping it in an object reference
        //So now it's on the heap
        object boxed = 5; //This boxing process is something like 15 - 20x slower than just assigning an int

        //Later, say, when we read something from the ArrayList into an int variable
        int unboxed = (int)boxed;

        //How can we avoid this?
        //DON'T USE OLD NON GENERIC COLLECTIONS
        //List<T> is modern, uses generics, avoid box-unbox
        //ArrayList - deprecated, slow, uses boxing and unboxing
    }

}



//Concrete & Abstract classes
// A concrete class is a class that can be instantiated directly.
// An abstract class is a class that cannot be instantiated directly and is meant to be inherited by other classes.



//Collections
//Lists "List<type>": Their behaviour consists of growing dynamically, ordered, typed, index access.
//Stack "Stack<type>": Allows me to Push and Pop, Last-in/First-out
//Queue "Queue<type>": Allows me to Enqueue and Dequeue, First-in/First-out
//Linked Lists "LinkedList<type>": Allows me to insert anywhere and reorder but is slightly heavier 