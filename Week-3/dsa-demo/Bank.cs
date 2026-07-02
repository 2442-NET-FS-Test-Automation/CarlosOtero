namespace DsaThreading;

public class Bank
{
    public long Balance; // mutable state

    // First let's create a lock object
    private readonly object _gate = new();

    public void DepositUnsafe(long amount) => Balance += amount; // read-modify-write: NOT ATOMIC

    public void DepositSafe(long amount)
    {
        lock (_gate) // Only one thread can enter this code block at a time
        {
            Balance += amount;
        }
    }
}