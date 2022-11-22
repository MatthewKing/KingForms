namespace KingForms;

public class ApplicationContextBuilder
{
    private Func<ApplicationActionForm> _splashFormFactory;
    private ApplicationAction _splashFormAction;

    private Func<ApplicationActionForm> _cleanupFormFactory;
    private ApplicationAction _cleanupFormAction;

    private Action<object, ApplicationScope> _main;

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

    public ApplicationContextBuilder WithSplashForm<TSplashForm, TSplashFormAction>()
        where TSplashForm : ApplicationActionForm, new()
        where TSplashFormAction : ApplicationAction, new()
    {
        _splashFormFactory = () => new TSplashForm();
        _splashFormAction = new TSplashFormAction();

        return this;
    }

    public ApplicationContextBuilder WithSplashForm(Func<ApplicationActionForm> splashFormFactory, ApplicationAction splashFormAction)
    {
        _splashFormFactory = splashFormFactory;
        _splashFormAction = splashFormAction;

        return this;
    }

    public ApplicationContextBuilder WithCleanupForm<TCleanupForm, TCleanupFormAction>()
        where TCleanupForm : ApplicationActionForm, new()
        where TCleanupFormAction : ApplicationAction, new()
    {
        _cleanupFormFactory = () => new TCleanupForm();
        _cleanupFormAction = new TCleanupFormAction();

        return this;
    }

    public ApplicationContextBuilder WithCleanupForm(Func<ApplicationActionForm> cleanupFormFactory, ApplicationAction cleanupFormAction)
    {
        _cleanupFormFactory = cleanupFormFactory;
        _cleanupFormAction = cleanupFormAction;

        return this;
    }

    public ApplicationContextBuilder WithMainForm<TResult>(Func<TResult, Form> mainFormFactory)
    {
        _main = (result, scope) =>
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
        _main = (_, scope) =>
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
        _main = (result, scope) =>
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
        _main = (_, scope) =>
        {
            foreach (var form in forms)
            {
                scope.AddForm(form, true);
            }
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

    public ApplicationContextBuilder RestrictToSingleInstance(string mutexName, InstanceRestorationMethod restorationMethod)
    {
        _singleInstanceMutexName = mutexName;
        _singleInstanceAlreadyInUseAction = () => InstanceRestorationHelper.Restore(restorationMethod);

        return this;
    }

    public ApplicationContextBuilder OnStart(Action<ApplicationScope> action)
    {
        _main = (_, scope) =>
        {
            action?.Invoke(scope);
        };

        return this;
    }

    public ApplicationContextBuilder OnStart<TResult>(Action<TResult, ApplicationScope> action)
    {
        _main = (result, scope) =>
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

        if (_splashFormFactory is not null)
        {
            applicationContext.AddScope(scope =>
            {
                var splashForm = _splashFormFactory.Invoke();

                if (_splashFormAction is not null)
                {
                    splashForm.AttachAction(_splashFormAction);
                }

                scope.AddForm(splashForm, !splashForm.KeepHidden);

                splashForm.FormClosed += (s, e) =>
                {
                    if (splashForm.IsActionComplete)
                    {
                        context = splashForm.ActionResult;
                    }
                };
            });
        }

        applicationContext.AddScope(scope => _main.Invoke(context, scope));

        if (_cleanupFormFactory is not null)
        {
            applicationContext.AddScope(scope =>
            {
                var cleanupForm = _cleanupFormFactory.Invoke();

                if (_cleanupFormAction is not null)
                {
                    cleanupForm.AttachAction(_cleanupFormAction);
                }

                scope.AddForm(cleanupForm, !cleanupForm.KeepHidden);
            });
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
}
