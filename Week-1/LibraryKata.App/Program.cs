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
        total += 1; //arithmethic shorthand for the same thing, also works 
    }   
}