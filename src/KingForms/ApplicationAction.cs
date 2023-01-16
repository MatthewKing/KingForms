namespace KingForms;

public abstract class ApplicationAction
{
    public virtual Task<object> RunAsync(IProgress<ApplicationProgress> progress, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(null);
    }
}
