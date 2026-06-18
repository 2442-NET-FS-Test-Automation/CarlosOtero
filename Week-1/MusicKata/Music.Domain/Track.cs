namespace MusicKata.Domain;

public class Track
{
    public int Id { get; }
    public string Title { get; }
    public string Artist { get; }
    public int DurationSeconds { get; }
    public TrackGenre Genre { get; }

    public Track(int id, string title, string artist, int durationSeconds, TrackGenre genre)
    {
        Id = id;
        Title = title;
        Artist = artist;
        DurationSeconds = durationSeconds;
        Genre = genre;
    }

    public string Describe() =>
        $"{Id}: {Title} by {Artist} ({DurationSeconds}s) — {Genre}";

    public override string ToString() => Describe();
}
