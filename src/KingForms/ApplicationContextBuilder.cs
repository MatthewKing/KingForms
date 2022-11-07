using KingForms.SingleInstance;

namespace KingForms;

public class ApplicationContextBuilder
{
    private IApplicationInitializer _initializer;
    private Func<SplashFormBase> _splashFormFactory;
    private Action<object, IApplicationContext> _runAction;

    private string _singleInstanceMutexName;
    private bool _singleInstanceRestoreExistingInstance;

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
        _runAction = (result, context) =>
        {
            if (result is TInitializationResult resultT)
            {
                var form = mainFormFactory?.Invoke(resultT);
                if (form is not null)
                {
                    context.AttachForm(form, true);
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
        _runAction = (result, context) =>
        {
            var form = mainFormFactory?.Invoke();
            if (form is not null)
            {
                context.AttachForm(form, true);
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
        _runAction = (result, context) =>
        {
            if (result is TInitializationResult resultT)
            {
                var forms = mainFormsFactory?.Invoke(resultT);
                if (forms is not null)
                {
                    foreach (var form in forms)
                    {
                        context.AttachForm(form, true);
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
        _runAction = (result, context) =>
        {
            foreach (var form in forms)
            {
                context.AttachForm(form, true);
            }
        };

        return this;
    }

    public ApplicationContextBuilder RestrictToSingleInstance(string mutexName, bool restoreExistingInstance)
    {
        _singleInstanceMutexName = mutexName;
        _singleInstanceRestoreExistingInstance = restoreExistingInstance;

        return this;
    }

    public ApplicationContextBuilder OnStart(Action<IApplicationContext> action)
    {
        _runAction = (result, context) =>
        {
            action?.Invoke(context);
        };

        return this;
    }

    public ApplicationContextBuilder OnStart<TInitializationResult>(Action<TInitializationResult, IApplicationContext> action)
    {
        _runAction = (result, context) =>
        {
            if (result is TInitializationResult resultT)
            {
                action?.Invoke(resultT, context);
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
        var applicationContext = new ApplicationContextExtended();
        applicationContext.Initializer = _initializer;
        applicationContext.SplashForm = _splashFormFactory;
        applicationContext.ApplicationStart = result => _runAction?.Invoke(result, applicationContext);
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

        using var instanceScope = InstanceScopeFactory.CreateScope(_singleInstanceMutexName);
        if (instanceScope is InstanceAlreadyInUseScope)
        {
            if (_singleInstanceRestoreExistingInstance)
            {
                SingleInstanceHelper.RestoreExistingInstance();
            }
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
