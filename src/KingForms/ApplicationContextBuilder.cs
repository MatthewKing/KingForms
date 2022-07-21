namespace KingForms;

public class ApplicationContextBuilder
{
    private IApplicationInitializer _initializer;
    private Func<SplashFormBase> _splashFormFactory;
    private Func<object, Form[]> _mainFormsFactory;

    public ApplicationContextBuilder WithInitializer(IApplicationInitializer initializer)
    {
        _initializer = initializer;

        return this;
    }

    public ApplicationContextBuilder WithSplashForm(SplashFormBase splashForm, TimeSpan duration, string text)
    {
        _splashFormFactory = () => splashForm;

        if (_initializer is null)
        {
            _initializer = new ApplicationInitializer(async (progress, cancellationToken) =>
            {
                progress.Text.Report(text);
                await Task.Delay(duration, cancellationToken);
                return null;
            });
        }

        return this;
    }

    public ApplicationContextBuilder WithSplashForm(Func<SplashFormBase> splashFormFactory)
    {
        _splashFormFactory = splashFormFactory;

        return this;
    }

    public ApplicationContextBuilder SingleMainForm<TContext>(Func<TContext, Form> mainFormFactory)
    {
        _mainFormsFactory = context => context is TContext contextT
            ? new Form[] { mainFormFactory.Invoke(contextT) }
            : throw new ApplicationInitializationException($"Application initialization returned {context.GetType().Name} but expected {typeof(TContext).Name}.");

        return this;
    }

    public ApplicationContextBuilder SingleMainForm(Form mainForm)
    {
        _mainFormsFactory = context => new Form[] { mainForm };

        return this;
    }

    public ApplicationContextBuilder MultipleMainForms<TContext>(Func<TContext, IEnumerable<Form>> mainFormsFactory)
    {
        _mainFormsFactory = context => context is TContext contextT
            ? mainFormsFactory.Invoke(contextT).ToArray()
            : throw new ApplicationInitializationException($"Application initialization returned {context.GetType().Name} but expected {typeof(TContext).Name}.");

        return this;
    }

    public ApplicationContextBuilder MultipleMainForms(IEnumerable<Form> mainForms)
    {
        _mainFormsFactory = context => mainForms.ToArray();

        return this;
    }

    public ApplicationContext Build()
    {
        var applicationContext = new ApplicationContextExtended(_mainFormsFactory, _initializer, _splashFormFactory);
        applicationContext.Run();

        return applicationContext;
    }

    public static implicit operator ApplicationContext(ApplicationContextBuilder builder)
    {
        return builder.Build();
    }
}
