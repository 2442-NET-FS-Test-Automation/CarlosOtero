namespace MusicKata.Domain;

public class Drum : InstrumentItem
{
    public int DrumAmount { get; set; }
    public int PlateAmount { get; set; }


    public Drum(int price, string brand, string model, int drumAmount, int plateAmount, int amountAvailable) : base(price, brand, model, amountAvailable)
    {
        DrumAmount = drumAmount;
        PlateAmount = plateAmount;
        AmountAvailable = amountAvailable;
    }

    public override string Describe()
    {
        return $"{Id}: {Brand} {Model} drum set with {DrumAmount} drums and {PlateAmount} plates. Available: {AmountAvailable}. Price: ${Price}.";
    }
}