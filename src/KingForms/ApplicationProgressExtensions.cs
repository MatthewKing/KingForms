namespace KingForms;

public static class ApplicationProgressExtensions
{
    public static void Report(this IProgress<ApplicationProgress> progress, double percent, string message)
    {
        progress.Report(new() { Text = message, Percent = percent });
    }
}
