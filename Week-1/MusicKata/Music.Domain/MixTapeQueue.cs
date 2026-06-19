namespace MusicKata.Domain;

public class MixTapeQueue
{
    private readonly Queue<Track> _playQueue = new();

    public int PendingCount => _playQueue.Count;

    public void Enqueue(Track track) => _playQueue.Enqueue(track);

    public Track? PlayNext()
    {
        if (_playQueue.Count == 0)
            return null;

        return _playQueue.Dequeue();
    }

    public IReadOnlyList<Track> PeekPending() => _playQueue.ToList();

    public void Clear() => _playQueue.Clear();
}
