using MusicKata.Domain;
using Serilog;

using System.Text.RegularExpressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicKata.App;

public class Program
{

    private static readonly HttpClient _httpClient = new HttpClient();
    public static async Task Main()
    {
        //logger to register from intital run
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/logs.text", rollingInterval: RollingInterval.Infinite)
            .CreateLogger();

        List<List<InstrumentItem>> catalog = new List<List<InstrumentItem>>{};
        
            List<InstrumentItem> guitarSection = new List<InstrumentItem>
            {
                new Guitar(1500, "Electric", "Fender", "Stratocaster", 6, true, 3),
                new Guitar(950, "Acoustic", "Taylor", "214aa", 6, false, 3),
                new Guitar(1200, "Acoustic", "Gibson", "J-45", 6, false, 2),
                new Guitar(920, "Acoustic", "Taylor", "214bb", 6, true, 5),
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
                new Piano(3000, "Grand", "Steinway", "Model A", true, 2),
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
        Stack<(InstrumentItem Item, int OuterIndex, int InnerIndex)> undoStack = new Stack<(InstrumentItem, int, int)>();
        bool hasUndo = false;

        List<IRent> RentedItems = new List<IRent>();
        ITrackRepository trackRepo = new InMemoryTrackRepository();
        MixTapeQueue mixTape = new MixTapeQueue();



        Log.Information("Music Store Manager started with {TrackCount} seeded tracks", trackRepo.GetAll().Count);

        var running = true;
        while (running)
        {
            PrintMenu();
            int choice = int.Parse(Console.ReadLine()?? "");   // naive: may throw on bad input — fine for now
            switch (choice)
            {
                case 1: AddItem(catalog); break;
                case 2: RemoveItem(catalog, ref hasUndo, undoStack); break;
                case 3: ListItems(catalog); break;
                case 4: SellItem(catalog); break;
                case 5: Renting(catalog,RentedItems); break;
                case 6: MixTapeCreator(trackRepo, mixTape); break;
                case 7: MusicRecord(trackRepo); break;
                case 8: await FetchLiveTrackFromApi(trackRepo); break;
                case 0: Console.WriteLine("Exiting..."); running = false; break;
            }
        }

        Log.CloseAndFlush();
    }

    // method to parse the menu input and return the value as an integer as an human error handling 
    private static bool TryParseMenuInput(string? input, out int value)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            value = 0;
            return false;
        }

        return int.TryParse(input, out value);
    }

    private static void MixTapeCreator(ITrackRepository trackRepo, MixTapeQueue mixTape)
    {
        var running = true;
        while (running)
        {
            Console.WriteLine("\n== MIXTAPE CREATOR ==");
            Console.WriteLine("1- Add tracks to queue");
            Console.WriteLine("2- Play queue");
            Console.WriteLine("3- Clear queue");
            Console.WriteLine("0- Back to main menu");

            if (!TryParseMenuInput(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input — please enter a menu option.\n");
                continue;
            }

            switch (choice)
            {
                case 1:
                    EnqueueTrack(trackRepo, mixTape);
                    break;
                case 2:
                    PlayMixTape(mixTape);
                    break;
                case 3:
                    mixTape.Clear();
                    Log.Information("Mixtape queue cleared");
                    Console.WriteLine("\nMixtape queue cleared.\n");
                    break;
                case 0:
                    running = false;
                    break;
                default:
                    Console.WriteLine("Unknown option — try again.\n");
                    break;
            }
        }
    }

    private static void PrintTrackCatalog(ITrackRepository trackRepo)
    {
        Console.WriteLine("\n=== Track Catalog ===\n");
        foreach (Track track in trackRepo.GetAll())
        {
            Console.WriteLine(track.Describe());
        }
        Console.WriteLine("\n");
    }

    private static void EnqueueTrack(ITrackRepository trackRepo, MixTapeQueue mixTape)
    {
        PrintTrackCatalog(trackRepo);
        Console.WriteLine("Enter track IDs to add to your mixtape.");
        Console.WriteLine("Select 0 when you are done building the mix.\n");

        var selecting = true;
        while (selecting)
        {
            Console.WriteLine($"[{mixTape.PendingCount} track(s) in queue] Enter track ID:");
            if (!TryParseMenuInput(Console.ReadLine(), out int trackId))
            {
                Console.WriteLine("Invalid input — please enter a track ID or 0 to finish.\n");
                continue;
            }

            if (trackId == 0)
            {
                Console.WriteLine("\nMixtape selection complete.\n");
                selecting = false;
                continue;
            }

            try
            {
                Track track = trackRepo.GetById(trackId);
                mixTape.Enqueue(track);
                Log.Information("Enqueued {Title} - id: {Id}", track.Title, track.ISRC);
                Console.WriteLine($"Added \"{track.Title}\" to the mixtape queue (position {mixTape.PendingCount}).\n");
            }
            catch (TrackNotFoundException ex)
            {
                Log.Warning("Track lookup failed for {Id}: {Message}", ex.Id, ex.Message);
                Console.WriteLine($"No track found with id {ex.Id}. Try again.\n");
            }
            catch (MusicStoreException ex)
            {
                Log.Error("Music store error: {Message}", ex.Message);
                Console.WriteLine($"{ex.Message}\n");
            }
            finally
            {
                Console.WriteLine("Queue operation complete.");
            }
        }
    }

    private static void PlayMixTape(MixTapeQueue mixTape)
    {
        try
        {
            if (mixTape.PendingCount == 0)
                throw new EmptyPlayQueueException();

            Console.WriteLine("\n=== Now playing your mixtape ===\n");
            while (mixTape.PendingCount > 0)
            {
                Track track = mixTape.PlayNext();
                Log.Information("Now playing {Title} by {Artist}", track.Title, track.Artist);
                Console.WriteLine($"▶ {track.Title} — {track.Artist} ({track.DurationSeconds}s)");
                // method from C# to create a "timeout" to pass to "the next song in the queue"
                Thread.Sleep(3000);
            }
            Console.WriteLine("\nMixtape finished!\n");
        }
        catch (EmptyPlayQueueException ex)
        {
            Log.Warning("Play refused: {Message}", ex.Message);
            Console.WriteLine($"\n{ex.Message}\n");
        }
        catch (MusicStoreException ex)
        {
            Log.Error("Playback error: {Message}", ex.Message);
            Console.WriteLine($"\n{ex.Message}\n");
        }
        finally
        {
            Console.WriteLine("Playback session ended.");
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("\n=== Welcome to the Music Store Manager! Here are your options: ===\n");
        Console.WriteLine("1. Add item\n2. Remove item\n3. List items\n4. Sell item\n5. Renting Service\n6. Mixtape Creator\n7. Music Records\n8. Fetch Online Track Data\n0. Exit\n");
    }

    private static void AddItem(List<List<InstrumentItem>> catalog)
    {
        Console.WriteLine("What type of item would you like to add?\n1. Guitar\n2. Microphone\n3. Trumpet\n4. Drum\n5. Piano");
        int itemType =  int.Parse(Console.ReadLine()?? "");
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
        int price = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Enter type:");
        string type = Console.ReadLine()?? "";
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine()?? "";
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine()?? "";
        Console.WriteLine("Enter number of strings:");
        int numberOfStrings = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Is it available for rent? (y/n)");
        bool canRent = (Console.ReadLine() ?? "").ToLower() == "y";
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine()?? "");

        var guitar = new Guitar(price, type, brand, model, numberOfStrings, canRent, amountAvailable);
        catalog[0].Add(guitar); 
        Console.WriteLine($"Added {guitar.Brand} {guitar.Model} guitar to the catalog.\n");
    }

    private static void AddMicrophone(List<List<InstrumentItem>> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine()?? "";
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine()?? "";
        Console.WriteLine("Enter type:");
        string type = Console.ReadLine()?? "";
        Console.WriteLine("Is it wireless? (y/n)");
        bool isWireless = (Console.ReadLine() ?? "").ToLower() == "y";
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine()?? "");

        var microphone = new Microphone(price, brand, model, type, isWireless, amountAvailable);
        catalog[1].Add(microphone); 
        Console.WriteLine($"Added {microphone.Brand} {microphone.Model} microphone to the catalog.\n");
    }

    private static void AddTrumpet(List<List<InstrumentItem>> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine()?? "";
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine()?? "";
        Console.WriteLine("Enter size:");
        string size = Console.ReadLine()?? "";
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine()?? "");

        var trumpet = new Trumpet(price, brand, model, size, amountAvailable);
        catalog[2].Add(trumpet); 
        Console.WriteLine($"Added {trumpet.Brand} {trumpet.Model} trumpet to the catalog.\n");
    }

    private static void AddDrum(List<List<InstrumentItem>> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine()?? "";
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine()?? "";
        Console.WriteLine("Enter number of pieces:");
        int numberOfPieces = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Enter number of cymbals:");
        int numberOfCymbals = int.Parse(Console.ReadLine()?? "");

        var drum = new Drum(price, brand, model, numberOfPieces, amountAvailable, numberOfCymbals);
        catalog[3].Add(drum); 
        Console.WriteLine($"Added {drum.Brand} {drum.Model} drum set to the catalog.\n");
    }

    private static void AddPiano(List<List<InstrumentItem>> catalog)
    {
        Console.WriteLine("Enter price:");
        int price = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Enter type:");
        string type = Console.ReadLine()?? "";
        Console.WriteLine("Enter brand:");
        string brand = Console.ReadLine()?? "";
        Console.WriteLine("Enter model:");
        string model = Console.ReadLine()?? "";
        Console.WriteLine("Enter amount available:");
        int amountAvailable = int.Parse(Console.ReadLine()?? "");
        Console.WriteLine("Is it available for rent? (y/n)");
        bool canRent = (Console.ReadLine() ?? "").ToLower() == "y";


        var piano = new Piano(price, type, brand, model, canRent, amountAvailable);
        catalog[4].Add(piano);
        Console.WriteLine($"Added {piano.Brand} {piano.Model} piano to the catalog.\n");
    }


    #region Music Records
    /// <summary>
    /// Display of Music Record menu.
    /// </summary>
    private const string MUSIC_RECORD_MENU =
        """
        1: Locate by ID
        2: Get music records information by ID
        3: List all (Artists, Genres) available
        4: Go back
        """;
    
    private const string MR_LIST_ATTRIBUTES =
        """
        1: List available artists
        2: List available genres
        3: Go back
        """;

    /// <summary>
    /// Music Record menu logic.
    /// </summary>
    private static void MusicRecord(ITrackRepository trackRepo)
    {
        Console.Clear();
        Console.WriteLine(MUSIC_RECORD_MENU);
        int? option = int.TryParse(Console.ReadLine(), out int result) ? result : null;
        switch (option)
        {
            case 1:
                LocateTrack(trackRepo);
                break;
            case 2:
                MusicInformation(trackRepo);
                break;
            case 3:
                ListAtrributes(trackRepo);
                break;
            case 4:
            default:
                return;
        }


    }

    private static void LocateTrack(ITrackRepository trackRepo)
    {
        try
        {
            Console.Clear();
            Console.WriteLine("Enter Id of track.");
            int? trackId = int.TryParse(Console.ReadLine(), out int result) ? result : null;

            Track? track = trackId is not null ? trackRepo.GetById((int)trackId) : null;

            if(track is not null)
            {
                track.PlaceTrack(track.CatalogSpot.Row, track.CatalogSpot.Col);
                track.PrintLocation();
                return;
            }
                Console.WriteLine("Track not found"); return;

            
        }catch(Exception ex)
        {
            Log.Warning("Something went wrong locating the track. {Message}", ex.Message);
        }
    }

    private static void MusicInformation(ITrackRepository trackRepo)
    {
        Console.Clear();
        Console.WriteLine("Input Music record key:");
        int? recordIsrc = int.TryParse(Console.ReadLine(), out int result) ? result : null;
        if (recordIsrc is null) {
            Log.Warning("Invalid input. Going back.}"); 
            return;
        }
        Track? track = trackRepo.GetById((int)recordIsrc);
        if (track is null)
        {
            Log.Warning("Track with ISRC: {Isrc}", recordIsrc);
            return;
        }

        Dictionary<int, Track> myTrack = new Dictionary<int, Track>
        {
            { track.ISRC, track }
        };

        Console.WriteLine(myTrack.ToPrettyString());


    }

    private static void ListAtrributes(ITrackRepository trackRepo)
    {
        try
        {
            Console.Clear();
            Console.WriteLine(MR_LIST_ATTRIBUTES);
            int? option = int.TryParse(Console.ReadLine(), out int result) ? result : null;
            if(option is null || option > 3)
            {
                Log.Information("Invalid input.");
                return;
            }
            if (option == 3) return;
            trackRepo.ReturnListedAttributes((int)option);
            
        }
        catch (Exception ex)
        {
            Log.Information("Something went wrong listing attributes: {Message}", ex.Message);
        }
    }
    #endregion Music Records
    private static void RemoveItem(List<List<InstrumentItem>> catalog, ref bool hasUndo, Stack<(InstrumentItem, int, int)> undoStack)
    {
        var running = true;
        while (running)
        {
            Console.WriteLine("1. Remove item");
            if(hasUndo == true)
                Console.WriteLine("2. Undo Removal");
            Console.WriteLine("0. Exit\n");
            int choice = int.Parse(Console.ReadLine()?? "");
            switch ((choice,hasUndo))
            {
                case (1, _): 
                    Console.WriteLine("Enter the ID of the item you want to sell:");
                    int id = int.Parse(Console.ReadLine()?? "");
                    for(int outerIndex = 0; outerIndex < catalog.Count; outerIndex++)
                    {
                        var subList = catalog[outerIndex];
                        int innerIndex = subList.FindIndex(i => i.Id == id);
                        if (innerIndex != -1) 
                        {
                            InstrumentItem removedItem = subList[innerIndex];
                            undoStack.Push((removedItem, outerIndex, innerIndex));
                            subList.RemoveAt(innerIndex);
                            hasUndo = true;
                            Console.WriteLine($"Removed {removedItem.Brand} {removedItem.Model} successfully.");
                            break; 
                        }
                    }
                    Console.WriteLine($"\nYou have removed item with ID {id}.\n");
                break;
                case (2, true): 
                    Console.WriteLine("Reverting action...");
                    if (undoStack.Count > 0)
                        {var (item, outerIndex, innerIndex) = undoStack.Pop();
                    if (outerIndex >= 0 && outerIndex < catalog.Count)
                    {
                        var subList = catalog[outerIndex];

                        if (innerIndex >= 0 && innerIndex <= subList.Count)
                        {
                            subList.Insert(innerIndex, item);
                            Console.WriteLine($"Undo successful: Restored {item.Brand} {item.Model} to catalog.");
                        }
                        hasUndo=false;
                    }
                    else
                    {
                        Console.WriteLine("Error: The original category section no longer exists.");
                    }
                }
                else
                {
                    Console.WriteLine("Nothing to undo!");
                }
                    
                break;
                case (0, _): 
                    running = false;
                break;
                    
            }
            
        }
    }

    
    private static void ListItems(List<List<InstrumentItem>> catalog)
    {
        List<string> sectionNames = new List<string> { "Guitars", "Pianos", "Trumpets", "Microphone", "Drums" };
        var running = true;
        while (running){
        Console.WriteLine("1. ORDER BY Brand\n2. ORDER BY Price\n3. List all items\n0. Exit\n");
        int choice = int.Parse(Console.ReadLine()?? "");
            switch (choice)
            {
                case 1:
                    Console.WriteLine("\nListing items...\n\n");
                    foreach (var section in catalog)
                    {
                        Console.WriteLine($"=== In {sectionNames[catalog.IndexOf(section)]} ===");
                        var query = section.OrderBy(brand => brand.Brand);
                        foreach (var item in query)
                        {
                            Console.WriteLine(item.Describe());
                        }
                        Console.WriteLine("\n");
                    }
                    Console.WriteLine("\n");
                break;
                case 2:
                Console.WriteLine("Insert a budget estimate\n");
                int priceRange = int.Parse(Console.ReadLine()?? "");
                   Console.WriteLine("\nListing items...\n\n");
                    foreach (var section in catalog)
                    {
                        Console.WriteLine($"=== In {sectionNames[catalog.IndexOf(section)]} ===");
                        var query = section.OrderBy(price => price.Price);
                        foreach (var item in query)
                        {
                            if(item.Price < priceRange)
                                Console.WriteLine(item.Describe());
                        }
                        Console.WriteLine("\n");
                    }
                    Console.WriteLine("\n");
                break;
                case 3:
                    Console.WriteLine("\nListing items...\n\n");
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
                break;
                case 0:
                running = false;
                break;
        }
        }
    }

    private static void SellItem(List<List<InstrumentItem>> catalog)
    {
                Console.WriteLine("Enter the ID of the item you want to sell:");
        int id = int.Parse(Console.ReadLine()?? "");
        foreach (var section in catalog)
        {
            var item = section.FirstOrDefault(i => i.Id == id);
            if (item != null && item.AmountAvailable >= 1) 
                item.AmountAvailable--;
            if (item == null || item.AmountAvailable == 0){
                Console.WriteLine($"\nI'm sorry, the item is not currently available.\n");
                return;
            }

        }
        Console.WriteLine($"\nYou have sold item with ID {id}.");
    }

    private static void Renting(List<List<InstrumentItem>> catalog, List<IRent> rentedItem)
    {
        var running = true;
        while (running)
        {
            
        Console.WriteLine("\n==RENTING MANAGER==\n1- List rentable items\n2- List rented items\n3- Rent by ID\n4- Return by ID\n0- Back to main menu");
        int choice = int.Parse(Console.ReadLine()?? "");
        switch (choice)       {
            case 1:
                PrintRentables(catalog);
            break;
            case 2: 
                PrintRented(rentedItem);
            break;
            case 3: 
                PrintRentables(catalog);
                Console.WriteLine("Enter the item number you wish to rent:");
                int rentNumber = int.Parse(Console.ReadLine()?? "");
                foreach (var section in catalog)
                {
                    var item = section.FirstOrDefault(i => i.Id == rentNumber);
                    if (item != null)
                    {
                        if (item is IRent rentableItem)
                        {
                            rentableItem.Rent();
                            rentedItem.Add(rentableItem);
                            rentableItem.IsRented();
                            Console.WriteLine($"\nItem with ID {rentNumber} has been rented.\n");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"\nSorry, the item with ID {rentNumber} is not available for rent.\n");
                            return;
                        }
                    }
                    
                }
                Console.WriteLine($"\nSorry, no item with ID {rentNumber} was found.\n");
            break;
            case 4: 
                PrintRented(rentedItem);
                Console.WriteLine("Enter the item number you wish to return:\n");
                rentNumber = int.Parse(Console.ReadLine()?? "");
                foreach (var section in catalog)
                {
                    var item = section.FirstOrDefault(i => i.Id == rentNumber);
                    if (item != null)
                    {
                        if (item is IRent rentableItem)
                        {
                            rentableItem.Return();
                            rentedItem.Remove(rentableItem);
                            Console.WriteLine($"\nThanks for returning item with ID {rentNumber}.\n");
                            return;
                        }
                        else
                        {   
                            Console.WriteLine($"\nSorry, no item with ID {rentNumber} was found.\n");
                            return;
                        }
                    }
                }
            break;
            case 0: 
            running = false;
            return;
        }
        }
       
    }

    private static void PrintRentables(List<List<InstrumentItem>> catalog)
    {
        Console.WriteLine("\nListing rentable items...\n\n");
                foreach (var section in catalog)
                {
                    foreach (var item in section)
                    {
                        if (item is IRent rentableItem && rentableItem.CanRent)
                        {
                            Console.WriteLine(item.Describe());
                        }
                    }
                }
        Console.WriteLine("\n");
    }

    private static void PrintRented(List<IRent> rentedItem)
    {
        Console.WriteLine("\nListing rented items...\n\n");
        foreach (var item in rentedItem)
            {
                Console.WriteLine(item);
            }
        Console.WriteLine("\n");
    }

    private static void MixtapeCreator()
    {
        throw new NotImplementedException();
    }
    private static void MusicRecords()
    {
        throw new NotImplementedException();
    }

    #region Api usage
    private static async Task FetchLiveTrackFromApi(ITrackRepository trackRepo)
    {
        Console.WriteLine("Enter TheAudioDB Track ID to search (numbers only):");
        string input = Console.ReadLine() ?? "";

        Regex numericRegex = new Regex(@"^\d+$");
        if (!numericRegex.IsMatch(input))
        {
            Console.WriteLine("Rejected entry: The identifier must contain only numbers.");
            Log.Warning("API input rejected due to invalid format: {Input}", input);
            return;
        }

        try
        {
            Log.Information("Searching for track ID {Id} in TheAudioDB...", input);
            
            HttpResponseMessage response = await _httpClient.GetAsync($"https://www.theaudiodb.com/api/v1/json/123/track.php?h={input}");
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            
            using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
            {
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("track", out JsonElement trackArray) && trackArray.GetArrayLength() > 0)
                {
                    JsonElement trackData = trackArray[0];
                    string title = trackData.GetProperty("strTrack").GetString() ?? "Unknown";
                    string artist = trackData.GetProperty("strArtist").GetString() ?? "Unknown";
                    int duration = int.TryParse(trackData.GetProperty("intDuration").GetString() ?? "0", out int seconds) ? seconds : 0;
                    string hasGenre = trackData.GetProperty("strGenre").GetString() ?? "Unknown";
                    TrackGenre genre = new();
                    if (Enum.TryParse<TrackGenre>(hasGenre, ignoreCase: true, out TrackGenre resultGenre))
                    {
                        genre = resultGenre;
                    }
                    else
                    {
                        genre = TrackGenre.Classical;
                    }
                    
                    Console.WriteLine($"\nTrack Found!");
                    Console.WriteLine($"Title: {title} | Artist: {artist}\n");
                    
                    Track track = new Track(int.Parse(input), title, artist, duration, genre, new GridPosition(1,1));

                    trackRepo.Add(track);

                }
                else
                {
                    Console.WriteLine("The API did not find any tracks with that ID.");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Log.Error("Connection error when querying TheAudioDB: {Message}", ex.Message);
            Console.WriteLine($"\nCould not contact server: {ex.Message}. The application continues to run.\n");
        }
        catch (Exception ex)
        {
            Log.Error("Unexpected error processing the API: {Message}", ex.Message);
            Console.WriteLine("An unexpected error occurred. Try again.");
        }
    }

    #endregion Api usage

}