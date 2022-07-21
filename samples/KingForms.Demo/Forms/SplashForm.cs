﻿namespace KingForms.Demo;

public partial class SplashForm : SplashFormBase
{
    public SplashForm(ProgressBarStyle style)
    {
        InitializeComponent();

        uxProgressBar.Style = style;
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
        uxProgressBar.Value = progressPercent;
    }
}
