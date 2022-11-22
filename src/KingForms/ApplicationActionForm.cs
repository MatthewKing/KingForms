namespace KingForms;

// Usually I'd make a class like this abstract, but that makes using the WinForms designer a bit of a hassle.

public class ApplicationActionForm : Form
{
    public bool IsActionAttached { get; private set; } = false;
    public bool IsActionComplete { get; protected set; } = false;
    public object ActionResult { get; protected set; } = null;

    public bool CanBeClosed { get; set; } = true;
    public bool KeepHidden { get; set; } = false;

    public ApplicationActionForm()
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

    public void AttachAction(ApplicationAction action)
    {
        if (!IsActionAttached)
        {
            IsActionAttached = true;

            var progress = CreateProgress();

            Load += async (sender, e) =>
            {
                ActionResult = await Task.Run(async () => await action.RunAsync(progress, CancellationToken.None));
                IsActionComplete = true;
                CanBeClosed = true;
                Close();
            };
        }
    }

    private ApplicationActionProgress CreateProgress()
    {
        return new ApplicationActionProgress(
            text: new Progress<string>(ReportProgressText),
            percent: new Progress<int>(ReportProgressPercent));
    }

    protected virtual void ReportProgressText(string progressText)
    {
        // Descendants should implement this as required.
    }

    protected virtual void ReportProgressPercent(int progressPercent)
    {
        // Descendants should implement this as required.
    }
}
