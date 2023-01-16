namespace KingForms.Demo;

public class DemoInitializer
{
    public static async Task<object> RunAsync(IProgress<ApplicationProgress> progress)
    {
        progress.Report(0, "Initializing...");
        var context = new DemoInitializationResult();
        context.Id = Guid.NewGuid();

        progress.Report(0.2, "Connecting to web service...");
        await ConnectToWebService();

        progress.Report(0.4, "Acquiring license...");
        await AcquireLicense();

        progress.Report(0.6, "Loading information from database...");
        context.Data = await LoadInformationFromDatabase();

        progress.Report(0.8, "Preparing assets...");
        context.Assets = await PrepareAssets();

        progress.Report(1.0, "Ready to launch!");
        await Task.Delay(1000);

        return context;
    }

    private static async Task<string> LoadInformationFromDatabase()
    {
        await Task.Delay(1000);
        return $"Data from DB @ {DateTimeOffset.Now:O}";
    }

    private static async Task ConnectToWebService()
    {
        await Task.Delay(1000);
    }

    private static async Task AcquireLicense()
    {
        await Task.Delay(1000);
    }

    private static async Task<string> PrepareAssets()
    {
        await Task.Delay(1000);
        return $"Assets @ {DateTimeOffset.Now:O}";
    }
}
