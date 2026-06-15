namespace MusicKata.Domain;

public class Piano : InstrumentItem, IRent
{
    public string Type { get; set; }
    public int AmountAvailable { get; private set; }
    
    public bool CanRent { get; set; }


    public Piano(int price, string type, string brand, string model, bool canRent, int amountAvailable) : base(price, brand, model)
    {
        Type = type;
        AmountAvailable = amountAvailable;
        CanRent = canRent;
    }
    public bool Rent()
    {
        if (CanRent && AmountAvailable > 0)
        {
            AmountAvailable--;
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
    public override string Describe()
    {
        return $"{Id}: {Brand} {Model} {Type} piano. Price: ${Price}.";
    }
}