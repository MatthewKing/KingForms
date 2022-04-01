namespace KingForms;

public class ApplicationInitializer : IApplicationInitializer
{
    private readonly Func<ApplicationInitializationProgress, CancellationToken, Task<object>> _initializer;

    public ApplicationInitializer(Func<ApplicationInitializationProgress, CancellationToken, Task<object>> initializer)
    {
        _initializer = initializer;
    }

    public Task<object> Run(ApplicationInitializationProgress progress, CancellationToken cancellationToken)
    {
        return _initializer.Invoke(progress, cancellationToken);
    }
}
