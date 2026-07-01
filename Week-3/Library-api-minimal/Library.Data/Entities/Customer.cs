namespace Library.Data.Entities;

public  class Customer
{
    public int Id {get;set;}
    public string Name {get;set;} = default!;

    public string Email {get;set;} = default!;

    // One customer can have many orders
    public  List<Order> Orders {get;set;} = new(); // Good idea to initialize to empty list

}