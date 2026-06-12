//If I have code from another namespace I want to use here - I use a using statement

using Library.Domain;

namespace LibraryKata.App; // A namespace is like a bucket or logical container for different related code files.

public class Program
{
    // We need a class ro hld our Main() method. The previous style with no class
    // or main - implicitly had a Main() under the hood.

    //public - accesible across the program
    //static - Main can be called upon without a Program object. It is a static/class method.
    //void - it doesn't return anything
    public static void Main()
    {
        // When I call dotnet run, it finds Main() and begins code execution at the first line of the main method.
        // I wrote my code, inside DataTypesAndOperatirs() - a separate method. So if I want
        //that code to run, I need to call it inside Main()
        DataTypesAndOperators();
        ClassesExample();
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

}
