namespace KingForms.Demo;

public partial class SplashForm : SplashFormBase
{
    public SplashForm()
    {
        InitializeComponent();
    }

    private void SplashForm_Load(object sender, EventArgs e)
    {
        CenterToScreen();
    }

    protected override void ReportProgressText(string progressText)
    {
        uxProgressLabel.Text = progressText;
    }

    protected override void ReportProgressPercent(int progressPercent)
    {
        if (uxProgressBar.Style is not ProgressBarStyle.Continuous)
        {
            uxProgressBar.Style = ProgressBarStyle.Continuous;
        }

        uxProgressBar.Value = progressPercent;
    }
}
