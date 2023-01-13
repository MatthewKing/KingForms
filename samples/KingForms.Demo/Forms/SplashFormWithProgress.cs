﻿namespace KingForms.Demo.Forms;

public partial class SplashFormWithProgress : Form, IProgressForm
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

    public void ReportProgressText(string progressText)
    {
        uxProgressLabel.Text = progressText;
    }

    public void ReportProgressPercent(double progressPercent)
    {
        uxProgressBar.Value = Math.Clamp((int)(progressPercent * 100), 0, 100);
    }
}