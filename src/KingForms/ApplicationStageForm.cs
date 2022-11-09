namespace KingForms;

// Usually I'd make a class like this abstract, but that makes using the WinForms designer a bit of a hassle.
public class ApplicationStageForm : Form
{
    public bool ActionAttached { get; private set; } = false;
    public bool ActionComplete { get; protected set; } = false;
    public object ActionResult { get; protected set; } = null;

    public bool CanBeClosed { get; set; } = true;
    public bool KeepHidden { get; set; } = false;

    public ApplicationStageForm()
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

    public void AttachAction(ApplicationStageAction action)
    {
        if (!ActionAttached)
        {
            ActionAttached = true;

            var progress = CreateApplicationInitializerProgress();

            Load += async (sender, e) =>
            {
                ActionResult = await Task.Run(async () => await action.RunAsync(progress, CancellationToken.None));
                ActionComplete = true;
                CanBeClosed = true;
                Close();
            };
        }
    }

    private ApplicationStageProgress CreateApplicationInitializerProgress()
    {
        return new ApplicationStageProgress(
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
