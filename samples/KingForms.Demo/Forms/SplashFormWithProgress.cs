namespace KingForms.Demo.Forms;

public partial class SplashFormWithProgress : Form, IProgressFactory<ApplicationProgress>
{
    public SplashFormWithProgress()
        : this(ProgressBarStyle.Marquee) { }

    public SplashFormWithProgress(ProgressBarStyle style)
    {
        InitializeComponent();

        uxProgressBar.Style = style;
        uxProgressLabel.Text = "Loading...";
    }

    private void SplashForm_Load(object sender, EventArgs e)
    {
        CenterToScreen();
    }

    public IProgress<ApplicationProgress> GetProgress()
    {
        return new Progress<ApplicationProgress>(x =>
        {
            uxProgressLabel.Text = x.Text;

            if (x.Percent.HasValue)
            {
                uxProgressBar.Value = Math.Clamp((int)(x.Percent.Value * 100), 0, 100);
            }
        });
    }
}
