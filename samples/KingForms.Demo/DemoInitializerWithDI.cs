﻿using KingForms.Demo.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace KingForms.Demo;

public class DemoInitializerWithDI : ApplicationAction
{
    public override async Task<object> RunAsync(ApplicationActionProgress progress, CancellationToken cancellationToken)
    {
        progress.Text.Report("Initializing...");
        var context = new DemoInitializationResult();
        progress.Percent.Report(20);

        progress.Text.Report("Connecting to web service...");
        await ConnectToWebService();
        progress.Percent.Report(40);

        progress.Text.Report("Acquiring license...");
        await AcquireLicense();
        progress.Percent.Report(60);

        progress.Text.Report("Loading information from database...");
        context.Data = await LoadInformationFromDatabase();
        progress.Percent.Report(80);

        progress.Text.Report("Preparing assets...");
        context.Assets = await PrepareAssets();
        progress.Percent.Report(100);

        progress.Text.Report("Ready to launch!");
        await Task.Delay(500);

        var services = new ServiceCollection();
        services.AddSingleton(context);
        services.AddTransient<MainForm1>();
        services.AddTransient<MainForm2>();
        services.AddTransient<ComboBoxDemoForm>();

        return services.BuildServiceProvider();
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
