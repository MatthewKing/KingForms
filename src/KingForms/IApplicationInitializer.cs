namespace KingForms;

public interface IApplicationInitializer
{
    Task<object> Run(ApplicationInitializationProgress progress, CancellationToken cancellationToken);
}
