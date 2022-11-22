namespace KingForms;

public abstract class ApplicationAction
{
    public virtual Task<object> RunAsync(ApplicationActionProgress progress, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(null);
    }
}
