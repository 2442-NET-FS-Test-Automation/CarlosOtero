namespace MusicKata.Domain;

public static class InstrumentFactory
{

    public static InstrumentItem Create(
        InstrumentType kind,
        int price = 0,
        string type = "N/A",
        string brand = "N/A",
        string model = "N/A",
        int numberOfStrings = 0,
        string size = "N/A",
        bool isWireless = false,
        bool canRent = false,
        int drumAmount = 0, 
        int plateAmount = 0,
        int amountAvailable = 0
        )
    {
        //This method is going to use a switch to call the correct constructor
        switch (kind)
        {
            case InstrumentType.Guitar:
                return new Guitar(price, type, brand, model, numberOfStrings, canRent,amountAvailable);
            case InstrumentType.Piano:
            return new Piano(price, type, brand, model, canRent,amountAvailable);
            case InstrumentType.Trumpet:
            return new Trumpet(price, brand, model, size,amountAvailable);
            case InstrumentType.Microphone:
                return new Microphone(price, brand, model, type, isWireless,amountAvailable);
            case InstrumentType.Drums:
            return new Drum(price, brand, model, drumAmount, plateAmount,amountAvailable);
            default: 
            throw new MusicStoreException($"Unknown item kind {kind}");
        }
    }
}