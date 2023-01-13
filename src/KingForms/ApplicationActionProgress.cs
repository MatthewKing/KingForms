namespace KingForms;

public sealed class ApplicationActionProgress
{
    public static ApplicationActionProgress None { get; } = new ApplicationActionProgress(x => { }, x => { });

    public IProgress<string> Text { get; }
    public IProgress<double> Percent { get; }

    public ApplicationActionProgress(Action<string> textHandler, Action<double> percentHandler)
    : this(new Progress<string>(textHandler), new Progress<double>(percentHandler)) { }

    public ApplicationActionProgress(IProgress<string> textProgress, IProgress<double> percentProgress)
    {
        Text = textProgress;
        Percent = percentProgress;
    }
}
