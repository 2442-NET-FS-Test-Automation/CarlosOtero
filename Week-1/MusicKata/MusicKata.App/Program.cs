using MusicKata.Domain;

namespace MusicKata.App;

public class Program
{
    public static void Main()
    {
        List<List<InstrumentItem>> catalog = new List<List<InstrumentItem>>{};
        
            List<InstrumentItem> guitarSection = new List<InstrumentItem>
            {
                new Guitar(1500, "Electric", "Fender", "Stratocaster", 6, true, 3),
                new Guitar(1200, "Acoustic", "Gibson", "J-45", 6, false, 2),
                new Guitar(1000, "Bass", "Ibanez", "SR500", 4, true, 4),
                new Guitar(1800, "Electric", "PRS", "Custom 24", 6, true, 1),
                new Guitar(900, "Acoustic", "Taylor", "214ce", 6, false, 5)
            };
        catalog.Add(guitarSection);
        List<InstrumentItem> pianoSection = new List<InstrumentItem>
        {
                new Piano(3000, "Grand", "Steinway", "Model D", true, 1),
                new Piano(2500, "Upright", "Yamaha", "U3", true, 2),
                new Piano(2000, "Digital", "Roland", "FP-90X", false, 4),
                new Piano(3500, "Baby Grand", "Kawai", "GL-10", true, 1),
                new Piano(4000, "Concert Grand", "Bösendorfer", "280VC", true, 1),
        };
        catalog.Add(pianoSection);
        List<InstrumentItem> trumpetSection = new List<InstrumentItem>
        {
                new Trumpet(2000, "Bach", "Stradivarius", "BBb", 2),
                new Trumpet(1500, "Yamaha", "Xeno", "YTR-8335RS", 3),
                new Trumpet(1800, "Conn", "Stellavox", "52B", 1),
                new Trumpet(2200, "Getzen", "Eterna", "590S", 2),
                new Trumpet(2500, "Schilke", "B1", "Bb/A", 1),
        };
        catalog.Add(trumpetSection);
        List<InstrumentItem> microphoneSection = new List<InstrumentItem>
        {
                new Microphone(500, "Shure", "SM58", "Dynamic", false, 10),
                new Microphone(800, "Neumann", "U87", "Condenser", true, 5),
                new Microphone(300, "AKG", "C214", "Condenser", false, 7),
                new Microphone(600, "Sennheiser", "e935", "Dynamic", false, 8),
                new Microphone(400, "Audio-Technica", "AT2020", "Condenser", true, 6),
        };
        catalog.Add(microphoneSection);
        List<InstrumentItem> drumSection = new List<InstrumentItem>
        {
                new Drum(1500, "Yamaha", "Masterworks", 3, 5, 2),
                new Drum(1200, "Pearl", "Export", 5, 10, 3),
                new Drum(2000, "Tama", "Starclassic", 4, 7, 4),
                new Drum(1800, "Ludwig", "Breakbeats", 2, 4, 1),
                new Drum(2500, "Gretsch", "Renown", 6, 8, 5),
        };
        catalog.Add(drumSection);

        List<IRent> RentedItems = new List<IRent>();
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
                case 4: Renting(catalog,RentedItems); break;
                case 0: Console.WriteLine("Exiting..."); running = false; break;
            }
        }

    }
    private static void Renting(List<List<InstrumentItem>> catalog, List<IRent> rentedItem)
    {
        var running = true;
        while (running)
        {
            
        Console.WriteLine("1- List rentable items\n2- List rented items\n3- Rent by ID\n4- Return by ID\n0- Back to main menu");
        int choice = int.Parse(Console.ReadLine());
        switch (choice)       {
            case 1:
                Console.WriteLine("Listing rentable items...\n\n");
                foreach (var section in catalog)
                {
                    foreach (var item in section)
                    {
                        if (item is IRent rentableItem)
                        {
                            Console.WriteLine(item.Describe());
                        }
                    }
                }
                Console.WriteLine("\n");
            break;
            case 2: 
                Console.WriteLine("Listing rented items...\n\n");
                foreach (var item in rentedItem)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("\n");
            break;
            case 3: 
                Console.WriteLine("Enter the ID of the item you want to rent:");
                int id = int.Parse(Console.ReadLine());
                foreach (var section in catalog)
                {
                    var item = section.FirstOrDefault(i => i.Id == id);
                    if (item != null)
                    {
                        if (item is IRent rentableItem)
                        {
                            rentableItem.Rent();
                            rentedItem.Add(rentableItem);
                        }
                        else
                        {
                            Console.WriteLine($"Sorry, the item with ID {id} is not available for rent.");
                            return;
                        }
                    }
                }
                Console.WriteLine($"Sorry, no item with ID {id} was found.");
            break;
            case 4: 
                Console.WriteLine("Enter the ID of the item you want to return:");
                int returnId = int.Parse(Console.ReadLine());
                foreach (var section in catalog)
                {
                    var item = section.FirstOrDefault(i => i.Id == returnId);
                    if (item != null)
                    {
                        if (item is IRent rentableItem)
                        {
                            rentableItem.Return();
                            rentedItem.Remove(rentableItem);
                        }
                        else
                        {
                            Console.WriteLine($"Sorry, the item with ID {returnId} is not available for return.");
                            return;
                        }
                    }
                }
                Console.WriteLine($"Sorry, no item with ID {returnId} was found.");
            break;
            case 0: 
            running = false;
            return;
        }
        }
       
    }

    private static void PrintMenu()
    {
        Console.WriteLine("=== Welcome to the Music Store Manager! Here are your options: ===\n");
        Console.WriteLine("1. Add item\n2. List items\n3. Sell item\n4. Renting Service\n0. Exit\n");
    }
    private static void SellItem(List<List<InstrumentItem>> catalog)
    {
        Console.WriteLine("Enter the ID of the item you want to sell:");
        int id = int.Parse(Console.ReadLine());
        foreach (var section in catalog)
        {
            var item = section.FirstOrDefault(i => i.Id == id);
            if (item != null && item is not IRent rentableItem)  
            {
                section.Remove(item);
                Console.WriteLine($"You have sold {item.Brand} {item.Model}.");
                return;
            }
        }
        Console.WriteLine($"You have sold item with ID {id}.");
    }
    private static void ListItems(List<List<InstrumentItem>> catalog)
    {
        List<string> sectionNames = new List<string> { "Guitars", "Microphones", "Trumpets", "Drums", "Pianos" };
        Console.WriteLine("Listing items...\n\n");
        foreach (var section in catalog)
        {
            Console.WriteLine($"=== In {sectionNames[catalog.IndexOf(section)]} ===");
            foreach (var item in section)
            {
                Console.WriteLine(item.Describe());
            }
            Console.WriteLine("\n");
        }
        Console.WriteLine("\n");
    }

    private static void AddItem(List<List<InstrumentItem>> catalog)
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
    private static void AddGuitar(List<List<InstrumentItem>> catalog)
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
        catalog[0].Add(guitar); 
        Console.WriteLine($"Added {guitar.Brand} {guitar.Model} guitar to the catalog.");
    }
    private static void AddMicrophone(List<List<InstrumentItem>> catalog)
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
        catalog[1].Add(microphone); 
        Console.WriteLine($"Added {microphone.Brand} {microphone.Model} microphone to the catalog.");
    }

    private static void AddTrumpet(List<List<InstrumentItem>> catalog)
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
        catalog[2].Add(trumpet); 
        Console.WriteLine($"Added {trumpet.Brand} {trumpet.Model} trumpet to the catalog.");
    }

    private static void AddDrum(List<List<InstrumentItem>> catalog)
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
        catalog[3].Add(drum); 
        Console.WriteLine($"Added {drum.Brand} {drum.Model} drum set to the catalog.");
    }

    private static void AddPiano(List<List<InstrumentItem>> catalog)
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
        Console.WriteLine("Is it available for rent? (y/n)");
        bool canRent = Console.ReadLine().ToLower() == "y";

        var piano = new Piano(price, type, brand, model, canRent, amountAvailable);
        catalog[4].Add(piano); 
        Console.WriteLine($"Added {piano.Brand} {piano.Model} piano to the catalog.");
    }
}