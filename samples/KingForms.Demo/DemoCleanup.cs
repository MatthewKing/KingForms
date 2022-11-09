namespace KingForms.Demo;

public class DemoCleanup : ApplicationStageAction
{
    public override async Task<object> RunAsync(ApplicationStageProgress progress, CancellationToken cancellationToken)
    {
        progress.Text.Report("Saving files...");
        await Task.Delay(1000);
        progress.Percent.Report(33);

        progress.Text.Report("Closing sockets...");
        await Task.Delay(1000);
        progress.Percent.Report(66);

        progress.Text.Report("Defragmenting database...");
        await Task.Delay(1000);
        progress.Percent.Report(99);

        progress.Text.Report("Done! Application will now close.");
        await Task.Delay(1000);
        progress.Percent.Report(100);

        return null;
    }
}
