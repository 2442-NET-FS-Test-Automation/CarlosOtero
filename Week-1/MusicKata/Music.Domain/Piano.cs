namespace MusicKata.Domain;

public class Piano : InstrumentItem, IRent
{
    public string Type { get; set; }
    
    public bool CanRent { get; set; }


    public Piano(int price, string type, string brand, string model, bool canRent, int amountAvailable) : base(price, brand, model, amountAvailable)
    {
        Type = type;
        CanRent = canRent;
    }
    public bool Rent()
    {
        if (CanRent && AmountAvailable > 0)
        {
            Console.WriteLine($"You have rented a {Brand} {Model} piano.");
            return true;
        }
        else
        {
            Console.WriteLine($"Sorry, the {Brand} {Model} piano is not available for rent.");
            return false;
        }
    }
    public void Return() => AmountAvailable++;

    public void IsRented()=> AmountAvailable--;
    public override string Describe()
    {
        return $"{Id}: {Brand} {Model} {Type} piano. Available: {AmountAvailable}. Price: ${Price}.";
    }
}