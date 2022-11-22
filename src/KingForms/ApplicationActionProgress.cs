namespace KingForms;

public sealed class ApplicationActionProgress
{
    public IProgress<string> Text { get; }
    public IProgress<int> Percent { get; }

    public ApplicationActionProgress(IProgress<string> text, IProgress<int> percent)
    {
        Text = text;
        Percent = percent;
    }
}
