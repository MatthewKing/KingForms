﻿namespace KingForms;

internal class ApplicationContextImplementation : ApplicationContext
{
    private readonly Queue<Action<ApplicationScope>> _scopeInitializers = new();

    public void AddScope(Action<ApplicationScope> scopeInitializer)
    {
        _scopeInitializers.Enqueue(scopeInitializer);
    }

    public void Run()
    {
        if (_scopeInitializers.Count == 0)
        {
            ExitThreadCore();
        }
        else
        {
            var scope = new ApplicationScope();
            scope.Completed += OnScopeCompleted;

            var scopeInitializer = _scopeInitializers.Dequeue();
            scopeInitializer?.Invoke(scope);
        }
    }

    private void OnScopeCompleted(object sender, EventArgs e)
    {
        if (sender is ApplicationScope scope)
        {
            scope.Completed -= OnScopeCompleted;
            Run();
        }
    }
}
