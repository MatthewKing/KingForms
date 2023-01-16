namespace KingForms;

public class ApplicationContextBuilder
{
    private Action<ApplicationScope, Action<object>> _onInitialize;
    private Action<object, ApplicationScope> _onPreRun;
    private Action<object, ApplicationScope> _onRun;
    private Action<object, ApplicationScope> _onPostRun;

    private string _singleInstanceMutexName;
    private Action _singleInstanceAlreadyInUseAction;

    private bool _enableVisualStyles;
    private bool _setCompatibleTextRenderingDefault;
#if NET5_0_OR_GREATER
    private HighDpiMode _highDpiMode;
#endif

    public ApplicationContextBuilder()
    {
        _enableVisualStyles = true;
        _setCompatibleTextRenderingDefault = false;
#if NET5_0_OR_GREATER
        _highDpiMode = HighDpiMode.SystemAware;
#endif
    }

    public ApplicationContextBuilder EnableVisualStyles(bool enabled)
    {
        _enableVisualStyles = enabled;

        return this;
    }

    public ApplicationContextBuilder SetCompatibleTextRenderingDefault(bool defaultValue)
    {
        _setCompatibleTextRenderingDefault = defaultValue;

        return this;
    }

#if NET5_0_OR_GREATER
    public ApplicationContextBuilder SetHighDpiMode(HighDpiMode highDpiMode)
    {
        _highDpiMode = highDpiMode;

        return this;
    }
#endif

    public ApplicationContextBuilder WithMainForm<TResult>(Func<TResult, Form> mainFormFactory)
    {
        _onRun = (result, scope) =>
        {
            if (result is TResult resultT)
            {
                var form = mainFormFactory?.Invoke(resultT);
                if (form is not null)
                {
                    scope.AddForm(form, true);
                }
            }
            else
            {
                throw new ApplicationInitializationException($"Application action returned {result.GetType().Name} but expected {typeof(TResult).Name}.");
            }
        };
        return this;
    }

    public ApplicationContextBuilder WithMainForm(Func<Form> mainFormFactory)
    {
        _onRun = (_, scope) =>
        {
            var form = mainFormFactory?.Invoke();
            if (form is not null)
            {
                scope.AddForm(form, true);
            }
        };

        return this;
    }

    public ApplicationContextBuilder WithMainForm<TForm>()
        where TForm : Form, new()
    {
        return WithMainForm(() => new TForm());
    }

    public ApplicationContextBuilder WithMainForms<TResult>(Func<TResult, IEnumerable<Form>> mainFormsFactory)
    {
        _onRun = (result, scope) =>
        {
            if (result is TResult resultT)
            {
                var forms = mainFormsFactory?.Invoke(resultT);
                if (forms is not null)
                {
                    foreach (var form in forms)
                    {
                        scope.AddForm(form, true);
                    }
                }
            }
            else
            {
                throw new ApplicationInitializationException($"Application action returned {result.GetType().Name} but expected {typeof(TResult).Name}.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder WithMainForms(IEnumerable<Form> forms)
    {
        _onRun = (_, scope) =>
        {
            foreach (var form in forms)
            {
                scope.AddForm(form, true);
            }
        };

        return this;
    }

    public ApplicationContextBuilder WithSplashForm<TSplashForm, TSplashFormAction>(bool keepHidden = false)
        where TSplashForm : Form, new()
        where TSplashFormAction : ApplicationAction, new()
    {
        _onInitialize = (scope, onComplete) =>
        {
            var splashForm = new TSplashForm();
            var splashFormAction = new TSplashFormAction();

            AttachApplicationActionToForm(splashForm, splashFormAction, onComplete);

            scope.AddForm(splashForm, !keepHidden);
        };

        return this;
    }

    public ApplicationContextBuilder WithSplashForm(Func<Form> splashFormFactory, ApplicationAction splashFormAction, bool keepHidden = false)
    {
        _onInitialize = (scope, onComplete) =>
        {
            var splashForm = splashFormFactory?.Invoke();

            AttachApplicationActionToForm(splashForm, splashFormAction, onComplete);

            scope.AddForm(splashForm, !keepHidden);
        };

        return this;
    }

    public ApplicationContextBuilder WithCleanupForm<TCleanupForm, TCleanupFormAction>(bool keepHidden = false)
        where TCleanupForm : Form, new()
        where TCleanupFormAction : ApplicationAction, new()
    {
        _onPostRun = (result, scope) =>
        {
            var cleanupForm = new TCleanupForm();
            var cleanupFormAction = new TCleanupFormAction();

            AttachApplicationActionToForm(cleanupForm, cleanupFormAction);

            scope.AddForm(cleanupForm, !keepHidden);
        };

        return this;
    }

    public ApplicationContextBuilder WithCleanupForm(Func<Form> cleanupFormFactory, ApplicationAction cleanupFormAction, bool keepHidden = false)
    {
        _onPostRun = (result, scope) =>
        {
            var cleanupForm = cleanupFormFactory?.Invoke();

            AttachApplicationActionToForm(cleanupForm, cleanupFormAction);

            scope.AddForm(cleanupForm, !keepHidden);
        };

        return this;
    }

    public ApplicationContextBuilder RestrictToSingleInstance(string mutexName)
    {
        _singleInstanceMutexName = mutexName;
        _singleInstanceAlreadyInUseAction = null;

        return this;
    }

    public ApplicationContextBuilder RestrictToSingleInstance(string mutexName, Action action)
    {
        _singleInstanceMutexName = mutexName;
        _singleInstanceAlreadyInUseAction = action;

        return this;
    }

    public ApplicationContextBuilder RestrictToSingleInstance(string mutexName, Action<IntPtr> action)
    {
        _singleInstanceMutexName = mutexName;
        _singleInstanceAlreadyInUseAction = () => InstanceRestorationHelper.ActOnWindowHandle(action);

        return this;
    }

    public ApplicationContextBuilder RestrictToSingleInstance(string mutexName, InstanceRestorationMethod restorationMethod)
    {
        _singleInstanceMutexName = mutexName;
        _singleInstanceAlreadyInUseAction = () => InstanceRestorationHelper.Restore(restorationMethod);

        return this;
    }

    public ApplicationContextBuilder OnPreRun(Action<ApplicationScope> action)
    {
        _onPreRun = (_, scope) =>
        {
            action?.Invoke(scope);
        };

        return this;
    }

    public ApplicationContextBuilder OnPreRun<TResult>(Action<TResult, ApplicationScope> action)
    {
        _onPreRun = (result, scope) =>
        {
            if (result is TResult resultT)
            {
                action?.Invoke(resultT, scope);
            }
            else
            {
                throw new ApplicationInitializationException($"Application action returned {result.GetType().Name} but expected {typeof(TResult).Name}.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder OnRun(Action<ApplicationScope> action)
    {
        _onRun = (_, scope) =>
        {
            action?.Invoke(scope);
        };

        return this;
    }

    public ApplicationContextBuilder OnRun<TResult>(Action<TResult, ApplicationScope> action)
    {
        _onRun = (result, scope) =>
        {
            if (result is TResult resultT)
            {
                action?.Invoke(resultT, scope);
            }
            else
            {
                throw new ApplicationInitializationException($"Application action returned {result.GetType().Name} but expected {typeof(TResult).Name}.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder OnPostRun(Action<ApplicationScope> action)
    {
        _onPostRun = (_, scope) =>
        {
            action?.Invoke(scope);
        };

        return this;
    }

    public ApplicationContextBuilder OnPostRun<TResult>(Action<TResult, ApplicationScope> action)
    {
        _onPostRun = (result, scope) =>
        {
            if (result is TResult resultT)
            {
                action?.Invoke(resultT, scope);
            }
            else
            {
                throw new ApplicationInitializationException($"Application action returned {result.GetType().Name} but expected {typeof(TResult).Name}.");
            }
        };

        return this;
    }

    public ApplicationContext Build()
    {
        var applicationContext = new ApplicationContextImplementation();

        object context = null;

        if (_onInitialize is not null)
        {
            applicationContext.AddScope(scope =>
            {
                _onInitialize.Invoke(scope, result =>
                {
                    context = result;
                });
            });
        }

        if (_onPreRun is not null)
        {
            applicationContext.AddScope(scope => _onPreRun.Invoke(context, scope));
        }

        if (_onRun is not null)
        {
            applicationContext.AddScope(scope => _onRun.Invoke(context, scope));
        }

        if (_onPostRun is not null)
        {
            applicationContext.AddScope(scope => _onPostRun.Invoke(context, scope));
        }

        applicationContext.Run();

        return applicationContext;
    }

    public void Run()
    {
        if (Thread.CurrentThread.GetApartmentState() is not ApartmentState.STA)
        {
            throw new Exception("Current thread must be a STA thread. Consider adding the [STAThread] attribute.");
        }

        UIThreadStart();
    }

    public ApplicationContextLifetime RunOnNewThread()
    {
        var lifetime = new ApplicationContextLifetime();

        var thread = new Thread(() => UIThreadStart(() => lifetime.ExitEvent.Set()));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return lifetime;
    }

    private void UIThreadStart(Action onExit = null)
    {
        if (_enableVisualStyles)
        {
            Application.EnableVisualStyles();
        }

        Application.SetCompatibleTextRenderingDefault(_setCompatibleTextRenderingDefault);

#if NET5_0_OR_GREATER
        Application.SetHighDpiMode(_highDpiMode);
#endif

        using var instanceScope = SingleInstanceScope.Create(_singleInstanceMutexName);
        if (instanceScope.IsInstanceAlreadyInUse)
        {
            _singleInstanceAlreadyInUseAction?.Invoke();
        }
        else
        {
            var applicationContext = Build();

            Application.Run(applicationContext);
        }

        onExit?.Invoke();
    }

    public static implicit operator ApplicationContext(ApplicationContextBuilder builder)
    {
        return builder.Build();
    }

    public static ApplicationContextBuilder Create()
    {
        return new ApplicationContextBuilder();
    }

    private static void AttachApplicationActionToForm(Form form, ApplicationAction action, Action<object> onComplete = null)
    {
        bool canBeClosed = false;

        form.Load += async (sender, e) =>
        {
            if (action is not null)
            {
                var progress = form is IProgressFactory<ApplicationProgress> progressFactory
                    ? progressFactory.GetProgress()
                    : new Progress<ApplicationProgress>();

                var result = await Task.Run(async () => await action.RunAsync(progress, CancellationToken.None));

                onComplete?.Invoke(result);
            }

            canBeClosed = true;
            form.Close();
        };

        form.FormClosing += (sender, e) =>
        {
            if (e.CloseReason is CloseReason.UserClosing && !canBeClosed)
            {
                e.Cancel = true;
            }
        };
    }
}
