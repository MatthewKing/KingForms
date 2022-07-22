namespace KingForms;

public interface IApplicationInitializer
{
    Task<object> InitializeAsync(ApplicationInitializationProgress progress, CancellationToken cancellationToken);
}
