namespace Library.Kata.Domain;

//An exception is anny class that inherits from the base Exception class
public class LibraryException : Exception
{
    //The class just constains a message
    public LibraryException(string message) : base(message){}

    
}