namespace MusicKata.Domain;

public class TrackNotFoundException : MusicStoreException
{
    public int Id { get; }

    public TrackNotFoundException(int id)
        : base($"No track found with id {id}")
    {
        Id = id;
    }
}
