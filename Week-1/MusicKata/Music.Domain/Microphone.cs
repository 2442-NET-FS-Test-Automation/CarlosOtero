namespace MusicKata.Domain;

public class Microphone : InstrumentItem
{
    public string Type { get; set; }
    public bool IsWireless { get; set; }

    public Microphone(int price, string brand, string model, string type, bool isWireless, int amountAvailable) : base(price, brand, model,amountAvailable)
    {
        Type = type;
        IsWireless = isWireless;
        AmountAvailable = amountAvailable;
    }

    public override string Describe()
    {
        string wirelessInfo = IsWireless ? "wireless" : "wired";
        return $"{Id}: {Brand} {Model} {Type} ({wirelessInfo}) microphone. Available: {AmountAvailable}. Price: ${Price}.";
    }
}