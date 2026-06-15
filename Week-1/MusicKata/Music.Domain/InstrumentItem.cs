namespace MusicKata.Domain;

public abstract class InstrumentItem
{
    public int Id { get; }
    private static int _nextId = 1;

    public int? Price { get; private set; }
    public string? Model { get; private set; }

    public string? Brand { get; private set; }
    public int AmountAvailable { get; set; }

    protected InstrumentItem(int price, string brand, string model, int amountAvailable)
    {
        Id = _nextId++;
        Price = price;
        Brand = brand;
        Model = model;
        AmountAvailable = amountAvailable;
    }

    public abstract string Describe();

    public override string ToString() => Describe();
}

