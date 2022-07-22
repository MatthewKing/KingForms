namespace KingForms;

// Usually I'd make a class like this abstract, but that makes using the WinForms designer a bit of a hassle.
public class SplashFormBase : Form
{
    public bool InitializerAttached { get; private set; } = false;
    public bool InitializationComplete { get; protected set; } = false;
    public object InitializationResult { get; protected set; } = null;
    public bool CanBeClosed { get; set; } = true;
    public bool KeepHidden { get; set; } = false;

    public SplashFormBase()
    {
        FormClosing += HandleFormClosing;
    }

    private void HandleFormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason is CloseReason.UserClosing && !CanBeClosed)
        {
            e.Cancel = true;
        }
    }

    public void AttachInitializer(IApplicationInitializer initializer)
    {
        if (!InitializerAttached)
        {
            InitializerAttached = true;

            var progress = CreateApplicationInitializerProgress();

            Load += async (sender, e) =>
            {
                InitializationResult = await Task.Run(async () => await initializer.InitializeAsync(progress, CancellationToken.None));
                InitializationComplete = true;
                CanBeClosed = true;
                Close();
            };
        }
    }

    private ApplicationInitializationProgress CreateApplicationInitializerProgress()
    {
        return new ApplicationInitializationProgress(
            text: new Progress<string>(progressText => ReportProgressText(progressText)),
            percent: new Progress<int>(progressPercent => ReportProgressPercent(progressPercent)));
    }

    protected virtual void ReportProgressText(string progressText)
    {

    }

    protected virtual void ReportProgressPercent(int progressPercent)
    {

    }
}
