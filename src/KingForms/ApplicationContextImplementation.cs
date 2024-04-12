namespace KingForms;

internal class ApplicationContextImplementation : ApplicationContext
{
    private readonly Queue<Action<ApplicationContextStage, object>> _stageInitializers = new();

    private ApplicationContextStage _currentStage;
    private ApplicationContextStage _previousStage;

    public void AddStageInitializer(Action<ApplicationContextStage, object> stageInitializer)
    {
        _stageInitializers.Enqueue(stageInitializer);
    }

    public void Run()
    {
        if (_stageInitializers.Count == 0)
        {
            ExitThreadCore();
            return;
        }

        if (_currentStage is not null)
        {
            _previousStage = _currentStage;
        }

        _currentStage = new ApplicationContextStage();

        var stageInitializer = _stageInitializers.Dequeue();
        stageInitializer.Invoke(_currentStage, _previousStage?.State);

        if (_currentStage.HasForms)
        {
            _currentStage.Completed += OnStageCompleted;
        }
        else
        {
            Run();
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
