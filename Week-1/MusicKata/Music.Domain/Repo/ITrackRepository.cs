namespace MusicKata.Domain;

public interface ITrackRepository
{
    void Add(Track track);
    Track GetById(int id);
    IEnumerable<Track> GetAll();
    IEnumerable<Track> Find(Predicate<Track> condition);

    void ReturnListedAttributes(int selection);
}
