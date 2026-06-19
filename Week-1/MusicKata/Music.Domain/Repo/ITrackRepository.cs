namespace MusicKata.Domain;

public interface ITrackRepository
{
    void Add(Track track);
    Track GetById(int id);
    List<Track> GetAll();
}
