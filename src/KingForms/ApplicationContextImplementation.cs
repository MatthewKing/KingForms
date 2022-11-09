namespace KingForms;

internal class ApplicationContextImplementation : ApplicationContext
{
    private readonly Queue<Action<ApplicationContextPhase>> _phaseConfigurations = new();

    public void AddPhase(Action<ApplicationContextPhase> configuration)
    {
        _phaseConfigurations.Enqueue(configuration);
    }

    public void Run()
    {
        if (_phaseConfigurations.Count == 0)
        {
            ExitThreadCore();
        }
        else
        {
            var phase = new ApplicationContextPhase();
            phase.Completed += OnPhaseCompleted;

            var phaseConfiguration = _phaseConfigurations.Dequeue();
            phaseConfiguration?.Invoke(phase);
        }
    }

    private void OnPhaseCompleted(object sender, EventArgs e)
    {
        if (sender is ApplicationContextPhase phase)
        {
            phase.Completed -= OnPhaseCompleted;
            Run();
        }
    }
}
