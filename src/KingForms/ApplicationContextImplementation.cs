namespace KingForms;

internal class ApplicationContextImplementation : ApplicationContext
{
    private readonly Queue<Action<ApplicationContextStage>> _stageInitializers = new();

    public void AddStage(Action<ApplicationContextStage> stageInitializer)
    {
        _stageInitializers.Enqueue(stageInitializer);
    }

    public void Run()
    {
        if (_stageInitializers.Count == 0)
        {
            ExitThreadCore();
        }
        else
        {
            var stage = new ApplicationContextStage();
            stage.Completed += OnStageCompleted;

            var stageInitializer = _stageInitializers.Dequeue();
            stageInitializer?.Invoke(stage);
        }
    }

    private void OnStageCompleted(object sender, EventArgs e)
    {
        if (sender is ApplicationContextStage stage)
        {
            stage.Completed -= OnStageCompleted;
            Run();
        }
    }
}
