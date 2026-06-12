namespace LibraryKata.Domain;

//Interfaces in C# - they are a contract for  behaviours - they do not define the implementation of the methods within
//
public interface ILendable
{
    //Only method signatures, not bodies, not even modifiers
    bool Checkout();
    void Return();
}