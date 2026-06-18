using Serilog;

namespace MusicKata.Domain;

public class InMemoryTrackRepository : ITrackRepository
{
    private readonly List<Track> _tracks = new();

    public InMemoryTrackRepository()
    {
        SeedTracks();
    }

    private void SeedTracks()
    {
        _tracks.Add(new Track(1, "Bohemian Rhapsody", "Queen", 354, TrackGenre.Rock));
        _tracks.Add(new Track(2, "Billie Jean", "Michael Jackson", 294, TrackGenre.Pop));
        _tracks.Add(new Track(3, "Take Five", "Dave Brubeck", 324, TrackGenre.Jazz));
        _tracks.Add(new Track(4, "Clair de Lune", "Claude Debussy", 300, TrackGenre.Classical));
        _tracks.Add(new Track(5, "Lose Yourself", "Eminem", 326, TrackGenre.HipHop));
        _tracks.Add(new Track(6, "Strobe", "Deadmau5", 636, TrackGenre.Electronic));
        _tracks.Add(new Track(7, "Smells Like Teen Spirit", "Nirvana", 301, TrackGenre.Rock));
        _tracks.Add(new Track(8, "Blinding Lights", "The Weeknd", 200, TrackGenre.Pop));
    }

    public void Add(Track track)
    {
        _tracks.Add(track);
        Log.Information("Added track {Title} - id: {Id}", track.Title, track.Id);
    }

    public List<Track> GetAll() => _tracks.ToList();

    public Track GetById(int id)
    {
        foreach (Track track in _tracks)
        {
            if (track.Id == id)
                return track;
        }

        Log.Warning("Track lookup failed for id {Id}", id);
        throw new TrackNotFoundException(id);
    }
}
