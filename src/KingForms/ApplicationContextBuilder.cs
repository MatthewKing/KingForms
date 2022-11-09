﻿namespace KingForms;

public class ApplicationContextBuilder
{
    private IApplicationInitializer _initializer;
    private Func<SplashFormBase> _splashFormFactory;
    private Action<object, ApplicationContextStage> _mainStage;

    private string _singleInstanceMutexName;
    private Action _singleInstanceAlreadyInUseAction;

    private bool _enableVisualStyles;
    private bool _setCompatibleTextRenderingDefault;
#if NET5_0_OR_GREATER
    private HighDpiMode _highDpiMode;
#endif

    public ApplicationContextBuilder()
    {
        _initializer = ApplicationInitializer.Empty();
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

    public ApplicationContextBuilder WithInitializer(IApplicationInitializer initializer)
    {
        _initializer = initializer;

        return this;
    }

    public ApplicationContextBuilder WithInitializer<TInitializer>()
        where TInitializer : IApplicationInitializer, new()
    {
        return WithInitializer(new TInitializer());
    }

    public ApplicationContextBuilder WithSplashForm<TSplashForm>()
        where TSplashForm : SplashFormBase, new()
    {
        _splashFormFactory = () => new TSplashForm();

        return this;
    }

    public ApplicationContextBuilder WithSplashForm(Func<SplashFormBase> splashFormFactory)
    {
        _splashFormFactory = splashFormFactory;

        return this;
    }

    public ApplicationContextBuilder WithMainForm<TInitializationResult>(Func<TInitializationResult, Form> mainFormFactory)
    {
        _mainStage = (result, stage) =>
        {
            if (result is TInitializationResult resultT)
            {
                var form = mainFormFactory?.Invoke(resultT);
                if (form is not null)
                {
                    stage.AddForm(form, true);
                }
            }
            else
            {
                throw new ApplicationInitializationException($"Application initialization returned {result.GetType().Name} but expected {typeof(TInitializationResult).Name}.");
            }
        };
        return this;
    }

    public ApplicationContextBuilder WithMainForm(Func<Form> mainFormFactory)
    {
        _mainStage = (result, stage) =>
        {
            var form = mainFormFactory?.Invoke();
            if (form is not null)
            {
                stage.AddForm(form, true);
            }
        };

        return this;
    }

    public ApplicationContextBuilder WithMainForm<TForm>()
        where TForm : Form, new()
    {
        return WithMainForm(() => new TForm());
    }

    public ApplicationContextBuilder WithMainForms<TInitializationResult>(Func<TInitializationResult, IEnumerable<Form>> mainFormsFactory)
    {
        _mainStage = (result, stage) =>
        {
            if (result is TInitializationResult resultT)
            {
                var forms = mainFormsFactory?.Invoke(resultT);
                if (forms is not null)
                {
                    foreach (var form in forms)
                    {
                        stage.AddForm(form, true);
                    }
                }
            }
            else
            {
                throw new ApplicationInitializationException($"Application initialization returned {result.GetType().Name} but expected {typeof(TInitializationResult).Name}.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder WithMainForms(IEnumerable<Form> forms)
    {
        _mainStage = (result, stage) =>
        {
            foreach (var form in forms)
            {
                stage.AddForm(form, true);
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

    public ApplicationContextBuilder OnStart(Action<ApplicationContextStage> action)
    {
        _mainStage = (result, stage) =>
        {
            action?.Invoke(stage);
        };

        return this;
    }

    public ApplicationContextBuilder OnStart<TInitializationResult>(Action<TInitializationResult, ApplicationContextStage> action)
    {
        _mainStage = (result, stage) =>
        {
            if (result is TInitializationResult resultT)
            {
                action?.Invoke(resultT, stage);
            }
            else
            {
                throw new ApplicationInitializationException($"Application initialization returned {result.GetType().Name} but expected {typeof(TInitializationResult).Name}.");
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
            applicationContext.AddStage(stage =>
            {
                var splashForm = _splashFormFactory.Invoke();

                if (_initializer is not null)
                {
                    splashForm.AttachInitializer(_initializer);
                }

                stage.AddForm(splashForm, !splashForm.KeepHidden);

                splashForm.FormClosed += (s, e) =>
                {
                    if (splashForm.InitializationComplete)
                    {
                        context = splashForm.InitializationResult;
                    }
                };
            });
        }

        applicationContext.AddStage(stage => _mainStage.Invoke(context, stage));

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
