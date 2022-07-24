namespace KingForms;

public sealed class ApplicationContextLifetime
{
    internal ManualResetEventSlim ExitEvent { get; }

    public ApplicationContextLifetime()
    {
        ExitEvent = new ManualResetEventSlim(false);
    }

    public void WaitForExit()
    {
        ExitEvent.Wait();
    }
}
