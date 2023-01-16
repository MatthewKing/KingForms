namespace KingForms;

public interface IProgressFactory<T>
{
    public IProgress<T> GetProgress();
}
