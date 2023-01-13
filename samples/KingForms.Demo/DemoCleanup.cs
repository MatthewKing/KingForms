﻿namespace KingForms.Demo;

public class DemoCleanup : ApplicationAction
{
    public override async Task<object> RunAsync(ApplicationActionProgress progress, CancellationToken cancellationToken)
    {
        progress.Text.Report("Saving files...");
        await Task.Delay(1000);
        progress.Percent.Report(0.33);

        progress.Text.Report("Closing sockets...");
        await Task.Delay(1000);
        progress.Percent.Report(0.66);

        progress.Text.Report("Defragmenting database...");
        await Task.Delay(1000);
        progress.Percent.Report(1.0);

        progress.Text.Report("Done! Application will now close.");
        await Task.Delay(1000);

        return null;
    }
}
