using MusicKata.Domain;

namespace MusicKata.App;

public class Program
{
    public static void Main()
    {
        List<InstrumentItem> catalog = new List<InstrumentItem>
        {
            new Guitar(1000, "Electric", "Gibson", "Les Paul", 6, true, 5),
            new Microphone(500, "Shure", "SM58", "Dynamic", false, 10),
            new Trumpet(2000, "Bach", "Stradivarius", "BBb", 2),
            new Drum(1500, "Yamaha", "Masterworks", 3, 5, 2),
        new Guitar(1200, "Acoustic", "Fender", "Stratocaster", 6, false, 3)
    };
        var running = true;
        while (running)
        {
            PrintMenu();
            int choice = int.Parse(Console.ReadLine());   // naive: may throw on bad input — fine for now
            switch (choice)
            {
                case 1: AddItem(catalog); break;
                case 2: ListItems(catalog); break;
                case 3: SellItem(catalog); break;
                case 0: Console.WriteLine("Exiting..."); running = false; break;
            }
        }

    }
    

    private static void PrintMenu()
    {
        Console.WriteLine("=== Welcome to the Music Store Manager! Here are your options: ===\n");
        Console.WriteLine("1. Add item\n2. List items\n3. Sell item\n0. Exit\n");
    }
    private static void SellItem(List<InstrumentItem> catalog)
    {
        Console.WriteLine("Enter the ID of the item you want to sell:");
        int id = int.Parse(Console.ReadLine());
        Console.WriteLine($"You have sold item with ID {id}.");
    }
    private static void ListItems(List<InstrumentItem> catalog )
    {
        Console.WriteLine("Listing items...\n");
        foreach (var item in catalog)
        {
            Console.WriteLine(item.Describe());
        }
        Console.WriteLine("\n");
    }

    private static void AddItem(List<InstrumentItem> catalog)
    {
        Console.WriteLine("What type of item would you like to add?\n1. Guitar\n2. Microphone\n3. Trumpet\n4. Drum\n5. Piano");
        int itemType =  int.Parse(Console.ReadLine());
        switch (itemType)
        {
            case 1: AddGuitar(catalog); break;
            case 2: AddMicrophone(catalog); break;
            case 3: AddTrumpet(catalog); break;
            case 4: AddDrum(catalog); break;
            case 5: AddPiano(catalog); break;
        }
    }
    private static void AddGuitar(List<InstrumentItem> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter type:");
        string type = Console.ReadLine();
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine();
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine();
        Console.WriteLine("Enter number of strings:");
        int numberOfStrings = int.Parse(Console.ReadLine());
        Console.WriteLine("Is it available for rent? (y/n)");
        bool canRent = Console.ReadLine().ToLower() == "y";
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine());

        var guitar = new Guitar(price, type, brand, model, numberOfStrings, canRent, amountAvailable);
        catalog.Add(guitar);
        Console.WriteLine($"Added {guitar.Brand} {guitar.Model} guitar to the catalog.");
    }
    private static void AddMicrophone(List<InstrumentItem> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine();
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine();
        Console.WriteLine("Enter type:");
        string type = Console.ReadLine();
        Console.WriteLine("Is it wireless? (y/n)");
        bool isWireless = Console.ReadLine().ToLower() == "y";
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine());

        var microphone = new Microphone(price, brand, model, type, isWireless, amountAvailable);
        catalog.Add(microphone);
        Console.WriteLine($"Added {microphone.Brand} {microphone.Model} microphone to the catalog.");
    }

    private static void AddTrumpet(List<InstrumentItem> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine();
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine();
        Console.WriteLine("Enter size:");
        string size = Console.ReadLine();
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine());

        var trumpet = new Trumpet(price, brand, model, size, amountAvailable);
        catalog.Add(trumpet);
        Console.WriteLine($"Added {trumpet.Brand} {trumpet.Model} trumpet to the catalog.");
    }

    private static void AddDrum(List<InstrumentItem> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine();
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine();
        Console.WriteLine("Enter number of pieces:");
        int numberOfPieces = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter number of cymbals:");
        int numberOfCymbals = int.Parse(Console.ReadLine());

        var drum = new Drum(price, brand, model, numberOfPieces, amountAvailable, numberOfCymbals);
        catalog.Add(drum);
        Console.WriteLine($"Added {drum.Brand} {drum.Model} drum set to the catalog.");
    }

    private static void AddPiano(List<InstrumentItem> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter type:");
        string type = Console.ReadLine();
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine();
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine();
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine());
        
        var piano = new Piano(price, type, brand, model, amountAvailable);
        catalog.Add(piano);
        Console.WriteLine($"Added {piano.Brand} {piano.Model} piano to the catalog.");
    }
}