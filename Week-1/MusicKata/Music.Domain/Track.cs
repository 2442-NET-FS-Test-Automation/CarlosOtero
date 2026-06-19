namespace MusicKata.Domain;

public class Track
{
    public int Id { get; }
    public string Title { get; }
    public string Artist { get; }
    public int DurationSeconds { get; }
    public TrackGenre Genre { get; }
    public GridPosition CatalogSpot { get; }

    public Track(
        int id,
        string title,
        string artist,
        int durationSeconds,
        TrackGenre genre,
        GridPosition catalogSpot)
    {
        Id = id;
        Title = title;
        Artist = artist;
        DurationSeconds = durationSeconds;
        Genre = genre;
        CatalogSpot = catalogSpot;
    }

    public string Describe() =>
        $"{Id}: {Title} by {Artist} ({DurationSeconds}s) — {Genre} @ {CatalogSpot}";

    public override string ToString() => Describe();
}
