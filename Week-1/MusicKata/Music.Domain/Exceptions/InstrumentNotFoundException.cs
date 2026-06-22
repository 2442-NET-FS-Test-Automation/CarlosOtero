namespace MusicKata.Domain;

public class InstrumentNotFoundException : MusicStoreException
{
    public int Id { get; }

    public InstrumentNotFoundException(int id)
        : base($"No Instrument available with id {id} -- Please input a valid ID")
    {
        Id = id;
    }
}
