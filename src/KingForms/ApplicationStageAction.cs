namespace KingForms;

public abstract class ApplicationStageAction
{
    public virtual Task<object> RunAsync(ApplicationStageProgress progress, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(null);
    }
}
