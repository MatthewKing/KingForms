namespace KingForms.SingleInstance;

internal static class InstanceScopeFactory
{
    public static IInstanceScope CreateScope(string mutexName)
    {
        if (string.IsNullOrEmpty(mutexName))
        {
            return new UnrestrictedInstanceScope();
        }

        var mutex = new Mutex(true, mutexName);

        if (mutex.WaitOne(TimeSpan.Zero, true))
        {
            return new SingleInstanceScope(mutex);
        }
        else
        {
            return new InstanceAlreadyInUseScope(mutex);
        }
    }
}
