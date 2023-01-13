namespace KingForms.Demo;

public class DemoInitializer : ApplicationAction
{
    public override async Task<object> RunAsync(ApplicationActionProgress progress, CancellationToken cancellationToken)
    {
        progress.Text.Report("Initializing...");
        var context = new DemoInitializationResult();
        progress.Percent.Report(0.2);

        progress.Text.Report("Connecting to web service...");
        await ConnectToWebService();
        progress.Percent.Report(0.4);

        progress.Text.Report("Acquiring license...");
        await AcquireLicense();
        progress.Percent.Report(0.6);

        progress.Text.Report("Loading information from database...");
        context.Data = await LoadInformationFromDatabase();
        progress.Percent.Report(0.8);

        progress.Text.Report("Preparing assets...");
        context.Assets = await PrepareAssets();
        progress.Percent.Report(1.0);

        progress.Text.Report("Ready to launch!");
        await Task.Delay(1000);

        return context;
    }

    private async Task<string> LoadInformationFromDatabase()
    {
        await Task.Delay(1000);
        return $"Data from DB @ {DateTimeOffset.Now:O}";
    }

    private async Task ConnectToWebService()
    {
        await Task.Delay(1000);
    }

    private async Task AcquireLicense()
    {
        await Task.Delay(1000);
    }

    private async Task<string> PrepareAssets()
    {
        await Task.Delay(1000);
        return $"Assets @ {DateTimeOffset.Now:O}";
    }
}
