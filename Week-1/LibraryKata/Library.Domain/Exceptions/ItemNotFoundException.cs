using System.Data.Common;
using Library.Kata.Domain;

namespace LibraryKata.Domain;

public class ItermNotFoundException : LibraryException
{
    // We can hold the offending Id that triggered the exception
    //We will use this for logging later
    public int Id {get;}

    public ItermNotFoundException (int id)
        : base($"No library item with id {id}")
    {
        Id = id;
    }
}