namespace KingForms;

internal class ApplicationContextImplementation : ApplicationContext
{
    private readonly Queue<Action<ApplicationStage>> _stageInitializers = new();

    public void AddStage(Action<ApplicationStage> stageInitializer)
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
            var stage = new ApplicationStage();
            stage.Completed += OnStageCompleted;

            var stageInitializer = _stageInitializers.Dequeue();
            stageInitializer?.Invoke(stage);
        }
    }

    private void OnStageCompleted(object sender, EventArgs e)
    {
        if (sender is ApplicationStage stage)
        {
            stage.Completed -= OnStageCompleted;
            Run();
        }
    }
}
