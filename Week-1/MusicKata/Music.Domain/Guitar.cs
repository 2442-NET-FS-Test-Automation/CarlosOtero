namespace MusicKata.Domain;

public class Guitar : InstrumentItem, IRent
{
    public string Type { get; set; }
    public int NumberOfStrings { get; set; }

    public bool CanRent { get; set; }

    public Guitar(int price, string type, string brand, string model, int numberOfStrings, bool canRent, int amountAvailable) : base(price, brand, model,amountAvailable)
    {
        Type = type;
        NumberOfStrings = numberOfStrings;
        CanRent = canRent;
        AmountAvailable = amountAvailable;
    }

    public override string Describe()
    {
        return $"{Id}: {Brand} {Model} {Type} guitar with {NumberOfStrings} strings. Available: {AmountAvailable}. Price: ${Price}.";  
    }

    public bool Rent()
    {
        if (CanRent && AmountAvailable > 0)
        {
            CanRent = false;
            Console.WriteLine($"You have rented a {Brand} {Model} guitar.");
            return true;
        }
        else
        {
            Console.WriteLine($"Sorry, the {Brand} {Model} guitar is not available for rent.");
            return false;
        }
    }

    public void IsRented()=> AmountAvailable--;


    public void Return() => AmountAvailable++;

}