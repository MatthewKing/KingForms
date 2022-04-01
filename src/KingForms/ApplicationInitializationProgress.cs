namespace KingForms;

public class ApplicationInitializationProgress
{
    public IProgress<string> Text { get; }
    public IProgress<int> Percent { get; }

    public ApplicationInitializationProgress(IProgress<string> text, IProgress<int> percent)
    {
        Text = text;
        Percent = percent;
    }
}
