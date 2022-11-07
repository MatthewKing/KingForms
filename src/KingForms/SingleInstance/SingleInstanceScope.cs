namespace KingForms.SingleInstance;

internal sealed class SingleInstanceScope : IInstanceScope
{
    private readonly Mutex _mutex;

    public SingleInstanceScope(Mutex mutex)
    {
        _mutex = mutex;
    }

    public void Dispose()
    {
        _mutex.ReleaseMutex();
        _mutex.Dispose();
    }
}
