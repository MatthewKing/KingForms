namespace KingForms;

public class ApplicationContextBuilder
{
    private string _singleInstanceMutexName;
    private Action _singleInstanceAlreadyInUseAction;

    private bool _enableVisualStyles;
    private bool _setCompatibleTextRenderingDefault;
#if NET5_0_OR_GREATER
    private HighDpiMode _highDpiMode;
#endif

    // Keep splash form and main form initializers separate.
    // This lets us call the "WithSplashForm" / "WithMainForm" methods multiple times
    // without doubling up on the actual forms.
    private Action<ApplicationContextStage, object> _splashFormInitializer;
    private Action<ApplicationContextStage, object> _mainFormInitializer;

    // This tracks our generic stage initializers.
    private readonly List<Action<ApplicationContextStage, object>> _stageInitializers = new List<Action<ApplicationContextStage, object>>();

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

    public ApplicationContextBuilder WithSplashForm<TForm>(Func<IProgress<ApplicationProgress>, Task<object>> action = null, bool visible = true)
        where TForm : Form, new()
    {
        return WithSplashForm(() => new TForm(), action: action, visible: visible);
    }

    public ApplicationContextBuilder WithSplashForm(Func<Form> splashFormFactory, Func<IProgress<ApplicationProgress>, Task<object>> action = null, bool visible = true)
    {
        _splashFormInitializer = (ApplicationContextStage stage, object state) =>
        {
            var form = splashFormFactory?.Invoke();
            if (form is not null)
            {
                if (action is not null)
                {
                    AttachActionToForm(form, action, newState => stage.SetCompletedState(newState));
                }

                stage.AddForm(form, visible);
            }
            else
            {
                throw new ApplicationInitializationException("Splash form factory returned null.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder WithMainForm<TForm>()
        where TForm : Form, new()
    {
        return WithMainForm(() => new TForm());
    }

    public ApplicationContextBuilder WithMainForm(Func<Form> mainFormFactory)
    {
        _mainFormInitializer = (ApplicationContextStage stage, object state) =>
        {
            var form = mainFormFactory?.Invoke();
            if (form is not null)
            {
                stage.AddForm(form, true);
            }
            else
            {
                throw new ApplicationInitializationException("Main form factory returned null.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder WithMainForm<TState>(Func<TState, Form> mainFormFactory)
    {
        _mainFormInitializer = (ApplicationContextStage stage, object state) =>
        {
            if (state is TState stateT)
            {
                var form = mainFormFactory?.Invoke(stateT);
                if (form is not null)
                {
                    stage.AddForm(form, true);
                }
                else
                {
                    throw new ApplicationInitializationException("Main form factory returned null.");
                }
            }
            else if (state is null)
            {
                throw new ApplicationInitializationException($"Expected (non-null) state of type {typeof(TState).Name}, but state was null instead.");
            }
            else
            {
                throw new ApplicationInitializationException($"Expected state of type {typeof(TState).Name}, but state was type {state.GetType().Name} instead.");
            }
        };

        return this;
    }


    public ApplicationContextBuilder WithMainForms(Func<IEnumerable<Form>> mainFormsFactory)
    {
        _mainFormInitializer = (ApplicationContextStage stage, object state) =>
        {
            var forms = mainFormsFactory?.Invoke();
            if (forms is not null)
            {
                foreach (var form in forms)
                {
                    stage.AddForm(form, true);
                }
            }
            else
            {
                throw new ApplicationInitializationException("Main form factory returned null.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder WithMainForms<TState>(Func<TState, IEnumerable<Form>> mainFormsFactory)
    {
        _mainFormInitializer = (ApplicationContextStage stage, object state) =>
        {
            if (state is TState stateT)
            {
                var forms = mainFormsFactory?.Invoke(stateT);
                if (forms is not null)
                {
                    foreach (var form in forms)
                    {
                        stage.AddForm(form, true);
                    }
                }
                else
                {
                    throw new ApplicationInitializationException("Main form factory returned null.");
                }
            }
            else if (state is null)
            {
                throw new ApplicationInitializationException($"Expected (non-null) state of type {typeof(TState).Name}, but state was null instead.");
            }
            else
            {
                throw new ApplicationInitializationException($"Expected state of type {typeof(TState).Name}, but state was type {state.GetType().Name} instead.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder AddStage(Action<ApplicationContextStage> stageInitializer)
    {
        var initializer = (ApplicationContextStage stage, object state) =>
        {
            stageInitializer?.Invoke(stage);
        };

        _stageInitializers.Add(initializer);

        return this;
    }

    public ApplicationContextBuilder AddStage<TState>(Action<ApplicationContextStage, TState> stageInitializer)
    {
        var initializer = (ApplicationContextStage stage, object state) =>
        {
            if (state is TState stateOfCorrectType)
            {
                stageInitializer?.Invoke(stage, stateOfCorrectType);
            }
            else if (state is null)
            {
                throw new ApplicationInitializationException($"Expected (non-null) state of type {typeof(TState).Name}, but state was null instead.");
            }
            else
            {
                throw new ApplicationInitializationException($"Expected state of type {typeof(TState).Name}, but state was type {state.GetType().Name} instead.");
            }
        };

        _stageInitializers.Add(initializer);

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

    public ApplicationContext Build()
    {
        var applicationContext = new ApplicationContextImplementation();

        if (_splashFormInitializer is not null)
        {
            applicationContext.AddStageInitializer(_splashFormInitializer);
        }

        if (_mainFormInitializer is not null)
        {
            applicationContext.AddStageInitializer(_mainFormInitializer);
        }

        foreach (var stageInitializer in _stageInitializers)
        {
            applicationContext.AddStageInitializer(stageInitializer);
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

    private static void AttachActionToForm(Form form, Func<IProgress<ApplicationProgress>, Task<object>> action, Action<object> onComplete = null)
    {
        bool canBeClosed = false;

        form.Load += async (sender, e) =>
        {
            if (action is not null)
            {
                var progress = form is IProgressFactory<ApplicationProgress> progressFactory
                    ? progressFactory.GetProgress()
                    : new Progress<ApplicationProgress>();

                var result = await Task.Run(async () => await action.Invoke(progress));

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
