namespace MusicKata.Domain;

public class Piano : InstrumentItem
{
    public string Type { get; set; }
    public int AmountAvailable { get; private set; }
    

    public Piano(int price, string type, string brand, string model, int amountAvailable) : base(price, brand, model)
    {
        Type = type;
        AmountAvailable = amountAvailable;
    }

    public override string Describe()
    {
        return $"{Id}: {Brand} {Model} {Type} piano. Price: ${Price}.";
    }
}