namespace MusicKata.Domain;

public class Track
{
    public int Id { get; }
    public string Title { get; }
    public string Artist { get; }
    public int DurationSeconds { get; }
    public TrackGenre Genre { get; }
    public GridPosition CatalogSpot { get; }

    private string[,] _grid;

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
        SeedGrid();
    }

    public string Describe() =>
        $"{Id}: {Title} by {Artist} ({DurationSeconds}s) — {Genre} @ {CatalogSpot}";

    public override string ToString() => Describe();

    private void SeedGrid(int rows=2, int cols=4)
    {
        _grid = new string[rows, cols];

        for (int r=0; r<rows; r++)
            for(int c=0; c<cols; c++)
                _grid[r,c] = " 0";
    }

    public void PlaceTrack(int row, int col)
    {
        if (row >= 0 && row < _grid.GetLength(0) &&
            col >= 0 && col < _grid.GetLength(1))
        {
            _grid[row, col] = " X";
        }
    }

    public void PrintLocation()
    {
        int rows = _grid.GetLength(0);
        int cols = _grid.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            Console.Write("[");
            for(int c = 0; c<cols; c++)
            {
                Console.Write(_grid[r,c]);
            }
            Console.WriteLine(" ]");

        }
    }
}
