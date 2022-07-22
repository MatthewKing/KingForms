namespace KingForms;

public class ApplicationContextBuilder
{
    private IApplicationInitializer _initializer;
    private Func<SplashFormBase> _splashFormFactory;
    private Action _onStarting;
    private Action _onStopping;
    private Action<object, IApplicationFormLauncher> _start;

    public ApplicationContextBuilder()
    {
        _initializer = ApplicationInitializer.Empty();
    }

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

    public ApplicationContextBuilder WithSplashForm(SplashFormBase splashForm)
    {
        _splashFormFactory = () => splashForm;

        return this;
    }

    public ApplicationContextBuilder WithSplashForm(Func<SplashFormBase> splashFormFactory)
    {
        _splashFormFactory = splashFormFactory;

        return this;
    }

    public ApplicationContextBuilder SingleMainForm<TContext>(Func<TContext, Form> mainFormFactory)
    {
        _start = (context, launcher) =>
        {
            if (context is TContext contextT)
            {
                var form = mainFormFactory?.Invoke(contextT);
                if (form is not null)
                {
                    launcher.Launch(form, true);
                }
            }
            else
            {
                throw new ApplicationInitializationException($"Application initialization returned {context.GetType().Name} but expected {typeof(TContext).Name}.");
            }
        };
        return this;
    }

    public ApplicationContextBuilder SingleMainForm(Form form)
    {
        _start = (context, launcher) =>
        {
            launcher.Launch(form, true);
        };

        return this;
    }

    public ApplicationContextBuilder SingleMainForm<TForm>()
        where TForm : Form, new()
    {
        return SingleMainForm(new TForm());
    }

    public ApplicationContextBuilder MultipleMainForms<TContext>(Func<TContext, IEnumerable<Form>> mainFormsFactory)
    {
        _start = (context, launcher) =>
        {
            if (context is TContext contextT)
            {
                var forms = mainFormsFactory?.Invoke(contextT);
                if (forms is not null)
                {
                    foreach (var form in forms)
                    {
                        launcher.Launch(form, true);
                    }
                }
            }
            else
            {
                throw new ApplicationInitializationException($"Application initialization returned {context.GetType().Name} but expected {typeof(TContext).Name}.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder MultipleMainForms(IEnumerable<Form> forms)
    {
        _start = (context, launcher) =>
        {
            foreach (var form in forms)
            {
                launcher.Launch(form, true);
            }
        };

        return this;
    }

    public ApplicationContextBuilder CustomMainForms<TContext>(Action<TContext, IApplicationFormLauncher> action)
    {
        _start = (context, launcher) =>
        {
            if (context is TContext contextT)
            {
                action?.Invoke(contextT, launcher);
            }
            else
            {
                throw new ApplicationInitializationException($"Application initialization returned {context.GetType().Name} but expected {typeof(TContext).Name}.");
            }
        };

        return this;
    }

    public ApplicationContextBuilder OnStarting(Action action)
    {
        _onStarting = action;

        return this;
    }

    public ApplicationContextBuilder OnStopping(Action action)
    {
        _onStopping = action;

        return this;
    }

    public ApplicationContext Build()
    {
        var applicationContext = new ApplicationContextExtended();
        applicationContext.Initializer = _initializer;
        applicationContext.SplashForm = _splashFormFactory;
        applicationContext.ApplicationStart = initializationContext => _start?.Invoke(initializationContext, applicationContext);
        applicationContext.ApplicationStarting = _onStarting;
        applicationContext.ApplicationStopping = _onStopping;
        applicationContext.Run();

        return applicationContext;
    }

    public static implicit operator ApplicationContext(ApplicationContextBuilder builder)
    {
        return builder.Build();
    }
}
