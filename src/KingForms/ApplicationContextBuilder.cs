namespace KingForms;

public class ApplicationContextBuilder
{
    private IApplicationInitializer _initializer;
    private Func<SplashFormBase> _splashFormFactory;
    private Func<object, Form[]> _mainFormsFactory;

    public ApplicationContextBuilder WithSplashForm(Func<SplashFormBase> splashFormFactory, TimeSpan displayDuration)
    {
        _splashFormFactory = splashFormFactory;
        _initializer = new ApplicationInitializer(async (progress, cancellationToken) =>
        {
            await Task.Delay(displayDuration, cancellationToken);
            return null;
        });

        return this;
    }

    public ApplicationContextBuilder WithSplashForm(Func<SplashFormBase> splashFormFactory, IApplicationInitializer initializer)
    {
        _splashFormFactory = splashFormFactory;
        _initializer = initializer;
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
        return SingleMainForm<object>(context => mainForm);
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
        return MultipleMainForms<object>(context => mainForms.ToArray());
    }

    public ApplicationContext Build()
    {
        var applicationContext = new ApplicationContextExtended(_mainFormsFactory, _initializer, _splashFormFactory);
        applicationContext.Run();

        return applicationContext;
    }
}
