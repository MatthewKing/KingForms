namespace KingForms;

internal sealed class ApplicationInitializer : IApplicationInitializer
{
    private readonly Func<ApplicationInitializationProgress, CancellationToken, Task<object>> _initializer;

    public ApplicationInitializer(Func<ApplicationInitializationProgress, CancellationToken, Task<object>> initializer)
    {
        _initializer = initializer;
    }

    public Task<object> InitializeAsync(ApplicationInitializationProgress progress, CancellationToken cancellationToken)
    {
        return _initializer?.Invoke(progress, cancellationToken) ?? Task.FromResult<object>(null);
    }

    public static IApplicationInitializer Empty()
    {
        return new ApplicationInitializer(null);
    }

    public static ApplicationInitializer Simple(TimeSpan duration, string text)
    {
        return new ApplicationInitializer(async (progress, cancellationToken) =>
        {
            progress.Text.Report(text);
            await Task.Delay(duration);
            return null;
        });
    }
}
