namespace MusicKata.Domain;

//Interfaces in C# - they are a contract for  behaviours - they do not define the implementation of the methods within
//
public interface IRent
{
    //Only method signatures, not bodies, not even modifiers
    bool Rent();
    void Return();

    bool CanRent { get; set; }

    void IsRented ();
}