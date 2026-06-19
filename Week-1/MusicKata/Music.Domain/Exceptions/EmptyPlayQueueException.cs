namespace MusicKata.Domain;

public class EmptyPlayQueueException : MusicStoreException
{
    public EmptyPlayQueueException()
        : base("The mixtape queue is empty — add tracks before playing.") { }
}
