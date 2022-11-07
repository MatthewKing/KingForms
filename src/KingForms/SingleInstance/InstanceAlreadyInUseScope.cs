namespace KingForms.SingleInstance;

internal sealed class InstanceAlreadyInUseScope : IInstanceScope
{
    private readonly Mutex _mutex;

    public InstanceAlreadyInUseScope(Mutex mutex)
    {
        _mutex = mutex;
    }

    public void Dispose()
    {
        _mutex.Dispose();
    }
}
