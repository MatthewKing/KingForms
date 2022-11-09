namespace KingForms;

public sealed class ApplicationStageProgress
{
    public IProgress<string> Text { get; }
    public IProgress<int> Percent { get; }

    public ApplicationStageProgress(IProgress<string> text, IProgress<int> percent)
    {
        Text = text;
        Percent = percent;
    }
}
