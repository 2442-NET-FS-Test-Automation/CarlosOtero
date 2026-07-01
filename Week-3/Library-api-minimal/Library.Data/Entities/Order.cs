namespace Library.Data.Entities;

public class Order
{
    public int Id {get;set;}

    public int CustomerId{get;set;} // FK -> Customer
    // We add this so as to not call an entire object if we want the attribute
    public Customer Customer {get;set;} = default!;

    public Priority Priority {get;set;}
    public DateTime CreatedUtc {get;set;} = DateTime.UtcNow; // Stamp it upon object creation

    public DateTime? CompletedUtc {get;set;}

    // Every Order has one or more OrderLines
    // OrderLines are the actual product and quantity of a something on the order. 

    public List<OrderLines> Lines {get;set;} = new();


}