namespace KingForms;

internal sealed class ApplicationContextExtended : ApplicationContext
{
    private readonly Func<object, IEnumerable<Form>> _mainFormsFactory;
    private readonly Func<SplashFormBase> _splashFormFactory;
    private readonly IApplicationInitializer _initializer;
    private readonly IList<Form> _mainForms;

    public ApplicationContextExtended(
        Func<object, IEnumerable<Form>> mainFormsFactory,
        IApplicationInitializer initializer,
        Func<SplashFormBase> splashFormFactory)
    {
        _initializer = initializer;
        _splashFormFactory = splashFormFactory;
        _mainFormsFactory = mainFormsFactory;
        _mainForms = new List<Form>();
    }

    public void Run()
    {
        if (_splashFormFactory != null)
        {
            var splashForm = _splashFormFactory.Invoke();
            if (_initializer != null)
            {
                splashForm.AttachInitializer(_initializer);
            }

            if (splashForm.KeepHidden)
            {
                splashForm.Opacity = 0;
            }

            ShowForm(splashForm);

            if (splashForm.KeepHidden)
            {
                splashForm.Visible = false;
            }

            splashForm.FormClosed += (s, e) =>
            {
                if (splashForm.InitializationComplete)
                {
                    foreach (var mainForm in _mainFormsFactory.Invoke(splashForm.InitializationResult))
                    {
                        ShowForm(mainForm);
                    }
                }
            };
        }
        else
        {
            foreach (var mainForm in _mainFormsFactory.Invoke(null))
            {
                ShowForm(mainForm);
            }
        }
    }

    private void ShowForm(Form form)
    {
        if (form is not null)
        {
            _mainForms.Add(form);
            form.HandleDestroyed += OnFormDestroy;
            form.Visible = true;
        }
    }

    private void OnFormDestroy(object sender, EventArgs e)
    {
        if (sender is Form form)
        {
            if (!form.RecreatingHandle)
            {
                form.HandleDestroyed -= OnFormDestroy;

                _mainForms.Remove(form);
                if (_mainForms.Count == 0)
                {
                    ExitThreadCore();
                }
            }
        }
    }
}
