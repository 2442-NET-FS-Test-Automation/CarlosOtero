namespace MusicKata.Domain;

public class Trumpet : InstrumentItem
{
    public string Size { get; set; }

    public Trumpet(int price, string brand, string model, string size, int amountAvailable) : base(price, brand, model,amountAvailable)
    {
        Size = size;
        AmountAvailable = amountAvailable;
    }

    override public string Describe()
    {
        return $"{Id}: {Brand} {Model} {Size} trumpet. Price: ${Price}.";
    }
}