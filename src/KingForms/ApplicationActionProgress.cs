namespace KingForms;

public sealed class ApplicationActionProgress
{
    public static ApplicationActionProgress None { get; } = new ApplicationActionProgress(x => { }, x => { });

    public IProgress<string> Text { get; }
    public IProgress<int> Percent { get; }

    public ApplicationActionProgress(Action<string> textHandler, Action<int> percentHandler)
    : this(new Progress<string>(textHandler), new Progress<int>(percentHandler)) { }

    public ApplicationActionProgress(IProgress<string> textProgress, IProgress<int> percentProgress)
    {
        Text = textProgress;
        Percent = percentProgress;
    }
}
