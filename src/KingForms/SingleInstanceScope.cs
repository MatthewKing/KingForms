namespace KingForms;

internal sealed class SingleInstanceScope : IDisposable
{
    public bool IsInstanceValid => _state;
    public bool IsInstanceAlreadyInUse => !_state;

    private bool _state;
    private Mutex _mutex;
    private bool _releaseMutexOnDispose;

    private SingleInstanceScope(bool state, Mutex mutex, bool releaseMutexOnDispose)
    {
        _state = state;
        _mutex = mutex;
        _releaseMutexOnDispose = releaseMutexOnDispose;
    }

    public void Dispose()
    {
        if (_mutex != null)
        {
            if (_releaseMutexOnDispose)
            {
                _mutex.ReleaseMutex();
            }

            _mutex.Dispose();

            _mutex = null;
        }
    }

    public static SingleInstanceScope Create(string mutexName)
    {
        if (string.IsNullOrEmpty(mutexName))
        {
            return new SingleInstanceScope(true, null, true);
        }

        var mutex = new Mutex(true, mutexName);

        if (mutex.WaitOne(TimeSpan.Zero, true))
        {
            return new SingleInstanceScope(true, mutex, true);
        }
        else
        {
            return new SingleInstanceScope(false, mutex, false);
        }
    }
}
