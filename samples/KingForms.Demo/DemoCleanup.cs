namespace KingForms.Demo;

public class DemoCleanup
{
    public static async Task<object> RunAsync(IProgress<ApplicationProgress> progress)
    {
        progress.Report(0, "Saving files...");
        await Task.Delay(1000);

        progress.Report(0.33, "Closing sockets...");
        await Task.Delay(1000);

        progress.Report(0.66, "Defragmenting database...");
        await Task.Delay(1000);

        progress.Report(1.0, "Done! Application will now close");
        await Task.Delay(1000);

        return null;
    }
}
