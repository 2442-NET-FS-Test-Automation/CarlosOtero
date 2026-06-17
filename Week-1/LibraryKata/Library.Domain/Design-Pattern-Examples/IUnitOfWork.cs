namespace LibraryKata.Domain;

public interface IUnitOfWork
{

    //This is not a method this is a property
    ILibraryRepository Items {get;}
    void Stage(string change); //A method to allow us to stage changes - like "git add")

    int Commit(); //A method to actually commit those changes 
}