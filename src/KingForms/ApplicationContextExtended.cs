namespace KingForms;

internal sealed class ApplicationContextExtended : ApplicationContext, IApplicationContext
{
    public Action<object> ApplicationStart { get; set; }
    public Func<SplashFormBase> SplashForm { get; set; }
    public IApplicationInitializer Initializer { get; set; }

    private readonly IList<Form> _forms;

    public ApplicationContextExtended()
    {
        _forms = new List<Form>();
    }

    public void Run()
    {
        if (SplashForm is not null)
        {
            var splashForm = SplashForm.Invoke();
            splashForm.AttachInitializer(Initializer ?? ApplicationInitializer.Simple(TimeSpan.FromSeconds(1), "Loading..."));

            AttachForm(splashForm, !splashForm.KeepHidden);

            splashForm.FormClosed += (s, e) =>
            {
                if (splashForm.InitializationComplete)
                {
                    ApplicationStart?.Invoke(splashForm.InitializationResult);
                }
            };
        }
        else
        {
            ApplicationStart?.Invoke(null);
        }
    }

    public void AttachForm(Form form)
    {
        AttachForm(form, true);
    }

    public void AttachForm(Form form, bool visible)
    {
        if (form is not null)
        {
            if (!visible)
            {
                form.Opacity = 0;
            }

            _forms.Add(form);
            form.HandleDestroyed += OnFormDestroy;
            form.Visible = true;

            if (!visible)
            {
                form.Visible = false;
                form.Opacity = 1;
            }
        }
    }

    private void OnFormDestroy(object sender, EventArgs e)
    {
        if (sender is Form form)
        {
            if (!form.RecreatingHandle)
            {
                form.HandleDestroyed -= OnFormDestroy;

                _forms.Remove(form);
                if (_forms.Count == 0)
                {
                    ExitThreadCore();
                }
            }
        }
    }
}
